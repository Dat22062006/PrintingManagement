// ═══════════════════════════════════════════════════════════════════
// ║  frmPaymentReceive.cs - THU TIỀN KHÁCH HÀNG                    ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Chọn KH, xem danh sách đơn hàng còn nợ, đánh dấu
// và thực hiện thu tiền; in phiếu thu.
// Toàn bộ DB ủy thác cho PaymentReceiveRepository.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPaymentReceive : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly PaymentReceiveRepository _repo = new();

        // Cờ ngăn xử lý song song khi đang thu tiền
        private bool _isProcessing = false;

        private ComboBox _cboRecvDebit = null!;
        private ComboBox _cboRecvCredit = null!;
        private bool _receiveAccountingLayoutDone;


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmPaymentReceive()
        {
            InitializeComponent();
            // Load chỉ trong Designer — tránh Load 2 lần (đẩy lưới / TK chồng lên nhau).
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmPaymentReceive_Load(object sender, EventArgs e)
        {
            SetupGrid();
            BuildReceiveAccountingCombos();

            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Séc" });
            cboPaymentMethod.SelectedIndex = 0;

            dtpPaymentDate.Value = DateTime.Today;

            LoadCustomers();

            // Đăng ký event — dùng -= trước để tránh trùng
            cboCustomer.SelectedIndexChanged -= cboCustomer_SelectedIndexChanged;
            cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;

            dgvDebtList.CurrentCellDirtyStateChanged -= dgvDebtList_CurrentCellDirtyStateChanged;
            dgvDebtList.CurrentCellDirtyStateChanged += dgvDebtList_CurrentCellDirtyStateChanged;

            dgvDebtList.CellValueChanged -= dgvDebtList_CellValueChanged;
            dgvDebtList.CellValueChanged += dgvDebtList_CellValueChanged;

            dgvDebtList.CellEndEdit -= dgvDebtList_CellEndEdit;
            dgvDebtList.CellEndEdit += dgvDebtList_CellEndEdit;

            ApplyPaymentReceiveScrollExtent();
        }

        private void ApplyPaymentReceiveScrollExtent()
        {
            int maxBottom = 0;
            foreach (Control c in paneldscongno.Controls)
                maxBottom = Math.Max(maxBottom, c.Bottom);
            paneldscongno.AutoScroll = true;
            paneldscongno.AutoScrollMargin = new Size(24, 40);
            paneldscongno.AutoScrollMinSize = new Size(Math.Max(paneldscongno.ClientSize.Width, 1040), maxBottom + 40);
        }

        private int ScaleRecvGap(int designPixelsAt96) =>
            (int)Math.Round(designPixelsAt96 * (double)DeviceDpi / 96.0);

        /// <summary>
        /// TK nợ / TK có khi thu tiền (in phiếu và ghi chép kế toán tay).
        /// Bố trí theo đáy hàng Khách hàng / Ngày / Phương thức — không dùng Top cố định + dy (lệch DPI).
        /// </summary>
        private void BuildReceiveAccountingCombos()
        {
            if (_receiveAccountingLayoutDone)
                return;
            _receiveAccountingLayoutDone = true;

            int Px(int d) => ScaleRecvGap(d);

            int headerBottom = 0;
            foreach (var c in new Control?[] { cboCustomer, dtpPaymentDate, cboPaymentMethod })
            {
                if (c != null)
                    headerBottom = Math.Max(headerBottom, c.Bottom);
            }

            int y = headerBottom + Px(12);
            Font f = cboCustomer.Font;

            var lblN = new Label
            {
                Text = "TK nợ:",
                AutoSize = true,
                Font = f,
                Location = new Point(Px(13), y + Px(3))
            };
            _cboRecvDebit = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = f,
                Location = new Point(Px(72), y),
                Width = Px(430)
            };
            _cboRecvDebit.Items.AddRange(new object[]
            {
                "111 - Tiền mặt",
                "112 - Tiền gửi Ngân hàng",
                "113 - Tiền đang chuyển"
            });
            _cboRecvDebit.SelectedIndex = 0;

            var lblC = new Label
            {
                Text = "TK có:",
                AutoSize = true,
                Font = f,
                Location = new Point(Px(520), y + Px(3))
            };
            _cboRecvCredit = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = f,
                Location = new Point(Px(578), y),
                Width = Px(430)
            };
            _cboRecvCredit.Items.AddRange(new object[]
            {
                "131 - Phải thu của khách hàng",
                "136 - Phải thu nội bộ",
                "138 - Phải thu khác"
            });
            _cboRecvCredit.SelectedIndex = 0;

            paneldscongno.Controls.Add(lblN);
            paneldscongno.Controls.Add(_cboRecvDebit);
            paneldscongno.Controls.Add(lblC);
            paneldscongno.Controls.Add(_cboRecvCredit);

            int tkBottom = Math.Max(Math.Max(lblN.Bottom, _cboRecvDebit.Bottom), Math.Max(lblC.Bottom, _cboRecvCredit.Bottom));

            y = tkBottom + Px(12);
            label2.Location = new Point(label2.Left, y);
            pictureBox3.Location = new Point(pictureBox3.Left, y - Px(2));
            y = Math.Max(label2.Bottom, pictureBox3.Bottom) + Px(8);
            dgvDebtList.SetBounds(dgvDebtList.Left, y, dgvDebtList.Width, Px(387));

            y = dgvDebtList.Bottom + Px(20);
            btnCollect.Location = new Point(btnCollect.Left, y);
            btnPrint.Location = new Point(btnPrint.Left, y);
            paneltongthuttkh.Location = new Point(paneltongthuttkh.Left, y + Px(6));
            panelsotientongthu.Location = new Point(panelsotientongthu.Left, y + Px(6));

            ApplyPaymentReceiveScrollExtent();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            dgvDebtList.Columns.Clear();
            dgvDebtList.RowHeadersVisible = false;
            dgvDebtList.AllowUserToAddRows = false;
            dgvDebtList.AllowUserToDeleteRows = false;
            dgvDebtList.MultiSelect = false;
            dgvDebtList.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvDebtList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDebtList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDebtList.ScrollBars = ScrollBars.Vertical;

            var styleRight = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0)
            };
            var styleBoldRed = new DataGridViewCellStyle(styleRight)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };
            var styleYellowRight = new DataGridViewCellStyle(styleRight)
            {
                BackColor = Color.FromArgb(255, 251, 220)
            };
            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvDebtList.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colSelect",
                HeaderText = "Chọn",
                FillWeight = 6,
                DefaultCellStyle = styleCenter
            });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colOrderCode", HeaderText = "Số HĐ", FillWeight = 14 });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colOrderDate",
                HeaderText = "Ngày HĐ",
                FillWeight = 11,
                DefaultCellStyle = styleCenter
            });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colProduct", HeaderText = "Sản phẩm", FillWeight = 20 });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalAmount",
                HeaderText = "Tổng tiền",
                FillWeight = 13,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCollected",
                HeaderText = "Đã thu",
                FillWeight = 12,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRemaining",
                HeaderText = "Còn nợ",
                FillWeight = 13,
                ReadOnly = true,
                DefaultCellStyle = styleBoldRed
            });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAmountToCollect",
                HeaderText = "Số tiền thu",
                FillWeight = 14,
                DefaultCellStyle = styleYellowRight
            });

            // Cột ẩn
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRemainingRaw", Visible = false });
            dgvDebtList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrderId", Visible = false });

            // Header style
            dgvDebtList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDebtList.ColumnHeadersHeight = 45;
            dgvDebtList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            dgvDebtList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDebtList.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);

            foreach (DataGridViewColumn col in dgvDebtList.Columns)
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void LoadCustomers()
        {
            try
            {
                cboCustomer.Items.Clear();
                cboCustomer.Items.Add(new CustomerItem { Id = 0, Name = "-- Chọn khách hàng --" });

                // Toàn bộ SQL nằm trong PaymentReceiveRepository
                var dt = _repo.GetCustomersWithDebt();

                foreach (DataRow row in dt.Rows)
                {
                    cboCustomer.Items.Add(new CustomerItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["Ten_Khach_Hang"].ToString()
                    });
                }

                cboCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHỌN KHÁCH HÀNG → NẠP ĐƠN HÀNG CÒN NỢ
        // ─────────────────────────────────────────────────────

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isProcessing) return;

            dgvDebtList.Rows.Clear();
            lblTotalCollected.Text = "0 đ";

            var customer = cboCustomer.SelectedItem as CustomerItem;
            if (customer == null || customer.Id == 0) return;

            LoadOrderDebt(customer.Id);
        }

        private void LoadOrderDebt(int customerId)
        {
            try
            {
                // Toàn bộ SQL nằm trong PaymentReceiveRepository
                var dt = _repo.GetOrderDebtByCustomer(customerId);

                foreach (DataRow dataRow in dt.Rows)
                {
                    decimal totalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                    decimal collected = Convert.ToDecimal(dataRow["Collected"]);
                    decimal remaining = Convert.ToDecimal(dataRow["Remaining"]);

                    int idx = dgvDebtList.Rows.Add();
                    var row = dgvDebtList.Rows[idx];

                    row.Cells["colSelect"].Value = false;
                    row.Cells["colOrderCode"].Value = dataRow["OrderCode"].ToString();
                    row.Cells["colOrderDate"].Value = Convert.ToDateTime(dataRow["OrderDate"]).ToString("dd/MM/yyyy");
                    row.Cells["colProduct"].Value = dataRow["ProductName"].ToString();
                    row.Cells["colTotalAmount"].Value = totalAmount.ToString("N0") + " đ";
                    row.Cells["colCollected"].Value = collected.ToString("N0") + " đ";
                    row.Cells["colRemaining"].Value = remaining.ToString("N0") + " đ";
                    row.Cells["colAmountToCollect"].Value = remaining.ToString("N0");
                    row.Cells["colRemainingRaw"].Value = remaining.ToString();
                    row.Cells["colOrderId"].Value = dataRow["id"].ToString();

                    // Lưu giá trị số vào Tag để validate khi nhập
                    row.Tag = new decimal[] { totalAmount, collected, remaining };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN GRID
        // ─────────────────────────────────────────────────────

        private void dgvDebtList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDebtList.IsCurrentCellDirty)
                dgvDebtList.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvDebtList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvDebtList.Columns["colSelect"].Index) return;

            bool isChecked = Convert.ToBoolean(dgvDebtList.Rows[e.RowIndex].Cells["colSelect"].Value ?? false);
            decimal[] values = dgvDebtList.Rows[e.RowIndex].Tag as decimal[];

            if (isChecked && values != null)
                dgvDebtList.Rows[e.RowIndex].Cells["colAmountToCollect"].Value = values[2].ToString("N0");
            else if (!isChecked)
                dgvDebtList.Rows[e.RowIndex].Cells["colAmountToCollect"].Value = "0";

            UpdateTotal();
        }

        private void dgvDebtList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvDebtList.Columns["colAmountToCollect"].Index) return;

            var row = dgvDebtList.Rows[e.RowIndex];
            string rawValue = (row.Cells["colAmountToCollect"].Value ?? "0")
                .ToString().Replace(",", "").Replace(" đ", "").Trim();

            if (!decimal.TryParse(rawValue, out decimal amountToCollect) || amountToCollect < 0)
            {
                MessageBox.Show("Số tiền thu không hợp lệ!", "Cảnh báo");
                row.Cells["colAmountToCollect"].Value = "0";
                return;
            }

            decimal[] values = row.Tag as decimal[];
            if (values != null && amountToCollect > values[2])
            {
                MessageBox.Show(
                    $"Số tiền thu không được vượt quá còn nợ ({values[2]:N0} đ)!",
                    "Cảnh báo");
                row.Cells["colAmountToCollect"].Value = values[2].ToString("N0");
                amountToCollect = values[2];
            }

            // Tự check nếu nhập số tiền > 0
            if (amountToCollect > 0) row.Cells["colSelect"].Value = true;

            UpdateTotal();
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT TỔNG TIỀN THU
        // ─────────────────────────────────────────────────────

        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvDebtList.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colSelect"].Value ?? false)) continue;

                string rawValue = (row.Cells["colAmountToCollect"].Value ?? "0")
                    .ToString().Replace(",", "").Replace(" đ", "").Trim();

                if (decimal.TryParse(rawValue, out decimal amount)) total += amount;
            }

            lblTotalCollected.Text = total.ToString("N0") + " đ";
        }


        // ─────────────────────────────────────────────────────
        // NÚT THU TIỀN
        // ─────────────────────────────────────────────────────

        private void btnCollect_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;

            var customer = cboCustomer.SelectedItem as CustomerItem;
            if (customer == null || customer.Id == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thiếu thông tin");
                return;
            }

            // Kiểm tra có dòng nào được chọn với số tiền > 0
            bool hasSelection = false;
            foreach (DataGridViewRow row in dgvDebtList.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colSelect"].Value ?? false)) continue;
                string rawValue = (row.Cells["colAmountToCollect"].Value ?? "0")
                    .ToString().Replace(",", "").Replace(" đ", "").Trim();
                if (decimal.TryParse(rawValue, out decimal amount) && amount > 0)
                { hasSelection = true; break; }
            }

            if (!hasSelection)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 đơn hàng và nhập số tiền thu!", "Thiếu thông tin");
                return;
            }

            if (MessageBox.Show("Xác nhận thu tiền khách hàng?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _isProcessing = true;

            try
            {
                // Đóng gói chi tiết thành DataTable
                var details = BuildCollectionDetailTable();

                // Toàn bộ SQL nằm trong PaymentReceiveRepository
                decimal actualTotal = _repo.SaveCollection(
                    customer.Id,
                    dtpPaymentDate.Value.Date,
                    cboPaymentMethod.Text,
                    details);

                // Tính lại còn lại để hiển thị
                decimal totalDebt = 0;
                foreach (DataGridViewRow row in dgvDebtList.Rows)
                {
                    if (row.Tag is decimal[] vals)
                        totalDebt += vals[2]; // remaining của từng đơn
                }
                decimal remainingDebt = Math.Max(totalDebt - actualTotal, 0);

                MessageBox.Show(
                    $"Thu tiền thành công!\n" +
                    $"Tổng thu: {actualTotal:N0} đ\n" +
                    $"Còn lại công nợ: {remainingDebt:N0} đ",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh danh sách sau khi thu
                LoadOrderDebt(customer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
            finally
            {
                _isProcessing = false;
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DỮ LIỆU GRID THÀNH DATATABLE GỬI REPOSITORY
        // ─────────────────────────────────────────────────────

        private DataTable BuildCollectionDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("OrderId", typeof(int));
            dt.Columns.Add("AmountToCollect", typeof(decimal));

            foreach (DataGridViewRow row in dgvDebtList.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colSelect"].Value ?? false)) continue;

                string rawValue = (row.Cells["colAmountToCollect"].Value ?? "0")
                    .ToString().Replace(",", "").Replace(" đ", "").Trim();
                if (!decimal.TryParse(rawValue, out decimal amount) || amount <= 0) continue;

                int.TryParse(row.Cells["colOrderId"].Value?.ToString(), out int orderId);
                dt.Rows.Add(orderId, amount);
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // NÚT IN PHIẾU THU
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var customer = cboCustomer.SelectedItem as CustomerItem;
            if (customer == null || customer.Id == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng trước khi in phiếu.", "Thiếu thông tin");
                return;
            }

            var printLines = new List<string[]>();
            decimal totalToPrint = 0;

            foreach (DataGridViewRow row in dgvDebtList.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colSelect"].Value ?? false)) continue;

                string rawValue = (row.Cells["colAmountToCollect"].Value ?? "0")
                    .ToString().Replace(",", "").Replace(" đ", "").Trim();
                if (!decimal.TryParse(rawValue, out decimal amount) || amount <= 0) continue;

                decimal.TryParse(row.Cells["colRemainingRaw"].Value?.ToString() ?? "0", out decimal remaining);
                totalToPrint += amount;

                printLines.Add(new string[]
                {
                    row.Cells["colOrderCode"].Value?.ToString() ?? "",
                    row.Cells["colOrderDate"].Value?.ToString() ?? "",
                    row.Cells["colProduct"].Value?.ToString()   ?? "",
                    remaining.ToString("N0") + " đ",
                    amount.ToString("N0")    + " đ"
                });
            }

            if (printLines.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 hóa đơn và nhập Số tiền thu > 0.", "Thiếu dữ liệu");
                return;
            }

            // Tiêu đề theo phương thức
            string paymentMethod = cboPaymentMethod.Text;
            string mainTitle, subTitle;

            switch (paymentMethod)
            {
                case "Tiền mặt":
                    mainTitle = "PHIẾU THU TIỀN MẶT";
                    subTitle = "Thu tiền mặt từ khách hàng";
                    break;
                case "Séc":
                    mainTitle = "PHIẾU THU BẰNG SÉC";
                    subTitle = "Thu tiền bằng séc từ khách hàng";
                    break;
                default:
                    mainTitle = "PHIẾU THU CHUYỂN KHOẢN";
                    subTitle = "Thu công nợ theo các hóa đơn đính kèm";
                    break;
            }

            string customerName = customer.Name;
            string paymentDateStr = dtpPaymentDate.Value.ToString("dd/MM/yyyy");
            decimal totalSnapshot = totalToPrint;
            string tkNo = _cboRecvDebit?.SelectedItem?.ToString() ?? "";
            string tkCo = _cboRecvCredit?.SelectedItem?.ToString() ?? "";

            var printDoc = new PrintDocument();
            printDoc.DefaultPageSettings.Landscape = true;
            printDoc.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            printDoc.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var fTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
                var fSub = new Font("Segoe UI", 10f, FontStyle.Italic);
                var fLabel = new Font("Segoe UI", 10f);
                var fBold = new Font("Segoe UI", 10f, FontStyle.Bold);
                var fCell = new Font("Segoe UI", 9.5f);
                var fTotal = new Font("Segoe UI", 12f, FontStyle.Bold);
                var fSign = new Font("Segoe UI", 9.5f);
                var fSignSub = new Font("Segoe UI", 8.5f, FontStyle.Italic);

                float pageW = ev.PageBounds.Width - 80f;
                float x = 40f, y = 40f;

                // Tiêu đề
                g.DrawString(mainTitle, fTitle, Brushes.Black, x, y); y += 32;
                g.DrawString(subTitle, fSub, Brushes.Gray, x, y); y += 28;

                // Thông tin
                g.DrawString("Khách hàng:", fLabel, Brushes.Gray, x, y);
                g.DrawString(customerName, fBold, Brushes.Black, x + 120, y); y += 20;
                g.DrawString("Ngày thu:", fLabel, Brushes.Gray, x, y);
                g.DrawString(paymentDateStr, fBold, Brushes.Black, x + 120, y); y += 20;
                g.DrawString("Phương thức:", fLabel, Brushes.Gray, x, y);
                g.DrawString(paymentMethod, fBold, Brushes.Black, x + 120, y); y += 20;
                g.DrawString("TK nợ — TK có:", fLabel, Brushes.Gray, x, y);
                g.DrawString($"{tkNo}  |  {tkCo}", fBold, Brushes.Black, x + 120, y); y += 26;

                g.DrawLine(new Pen(Color.FromArgb(180, 180, 210), 1f), x, y, x + pageW, y);
                y += 10;

                // Bảng chi tiết
                float[] colX = { x, x + 105, x + 180, x + 430, x + 610 };
                float[] colW = { 105, 75, 250, 180, 160 };
                bool[] isRight = { false, false, false, true, true };
                string[] headers = { "Số HĐ", "Ngày HĐ", "Sản phẩm", "Còn nợ (đ)", "Số tiền thu (đ)" };

                // Header bảng
                g.FillRectangle(new SolidBrush(Color.FromArgb(220, 230, 255)), x, y, pageW, 26);
                for (int i = 0; i < headers.Length; i++)
                {
                    var sf = new StringFormat
                    {
                        Alignment = isRight[i] ? StringAlignment.Far : StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(headers[i], fBold, Brushes.Black,
                        new RectangleF(colX[i] + 4, y, colW[i] - 8, 26), sf);
                }
                y += 26;

                // Dữ liệu
                var blueBrush = new SolidBrush(Color.FromArgb(30, 80, 200));
                var altBg = new SolidBrush(Color.FromArgb(245, 248, 255));
                bool alt = false;

                foreach (var line in printLines)
                {
                    if (alt) g.FillRectangle(altBg, x, y, pageW, 22);

                    for (int i = 0; i < line.Length && i < colX.Length; i++)
                    {
                        var sf = new StringFormat
                        {
                            Alignment = isRight[i] ? StringAlignment.Far : StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        Brush textBrush = (i == 4) ? blueBrush : Brushes.Black;
                        g.DrawString(line[i], fCell, textBrush,
                            new RectangleF(colX[i] + 4, y, colW[i] - 8, 22), sf);
                    }

                    g.DrawLine(new Pen(Color.FromArgb(210, 215, 230), 0.5f),
                        x, y + 22, x + pageW, y + 22);
                    y += 22;
                    alt = !alt;
                }

                // Tổng tiền
                y += 12;
                g.DrawLine(new Pen(Color.FromArgb(100, 120, 200), 1.5f), x + 350, y, x + pageW, y);
                y += 8;

                g.DrawString("TỔNG TIỀN THU:", fTotal, Brushes.Black,
                    new RectangleF(x + 350, y, 200, 28),
                    new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                g.DrawString(totalSnapshot.ToString("N0") + " đ", fTotal, blueBrush,
                    new RectangleF(x + 350, y, pageW - 350, 28),
                    new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
                y += 38;

                // Chữ ký
                string[] signers = { "Người lập phiếu", "Kế toán trưởng", "Giám đốc" };
                float signW = pageW / 3f;
                for (int i = 0; i < 3; i++)
                {
                    float sx = x + i * signW + signW / 2f - 55;
                    g.DrawString(signers[i], fSign, Brushes.Black, sx, y);
                    g.DrawString("(Ký, ghi rõ họ tên)", fSignSub, Brushes.Gray, sx, y + 16);
                }

                fTitle.Dispose(); fSub.Dispose(); fLabel.Dispose(); fBold.Dispose();
                fCell.Dispose(); fTotal.Dispose(); fSign.Dispose(); fSignSub.Dispose();
                blueBrush.Dispose(); altBg.Dispose();
            };

            new PrintPreviewDialog
            {
                Document = printDoc,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            }.ShowDialog(this);
        }


        // ─────────────────────────────────────────────────────
        // INNER CLASS — ĐỐI TƯỢNG KHÁCH HÀNG TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class CustomerItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";

            public override string ToString() => Name;
        }
    }
}