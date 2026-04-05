// ═══════════════════════════════════════════════════════════════════
// ║  frmSales.cs - LẬP CHỨNG TỪ BÁN HÀNG                          ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Chọn phiếu giao hàng đã giao, điền thông tin hóa đơn,
// lưu đơn bán hàng và xem danh sách hóa đơn đã lưu.
// Toàn bộ DB ủy thác cho SalesRepository.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace PrintingManagement
{
    public partial class frmSales : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly SalesRepository _repo = new();
        private readonly DeliveryNoteRepository _deliveryRepo = new();

        /// <summary>Phiếu giao hàng đã chọn (nguồn lập đơn bán).</summary>
        private int _phieuGiaoId = 0;
        private int _customerId = 0;

        /// <summary>Hạch toán bán hàng (đặt tên tiếng Anh — cùng quy tắc frmInventoryReceive).</summary>
        private ComboBox cboDebitAccount = null!;
        private ComboBox cboCreditAccount = null!;
        private NumericUpDown nudCreditDays = null!;
        private DateTimePicker dtpExpectedPaymentDate = null!;
        private Guna2Button _btnPrintSales = null!;

        // Dữ liệu in — lưu vào field để PrintPageHandler dùng (tránh capture lambda mỗi lần)
        private PrintDocument? _currentPrintDoc;
        private string _printDocCode = "";
        private string _printCustomer = "";
        private string _printAddress = "";
        private string _printTax = "";
        private string _printSaleDate = "";
        private string _printInvMeta = "";
        private string _printInvDate = "";
        private string _printTkNo = "";
        private string _printTkCo = "";
        private string _printDue = "";
        private string _printTotal = "";
        private List<string[]> _printLines = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmSales()
        {
            InitializeComponent();
            // Load chỉ gắn trong Designer — không gắn thêm ở đây (tránh chạy Load 2 lần → lệch layout / chồng control).
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private bool _salesAccountingLayoutDone;

        private void frmSales_Load(object sender, EventArgs e)
        {
            // Ẩn các control “ma” trên panel3 (trùng vị trí với panel5, một số máy DPI hiện chồng lên nhau).
            foreach (Control c in panel3.Controls)
            {
                if (!ReferenceEquals(c, panel5))
                    c.Visible = false;
            }

            cboDocumentType.Items.Clear();
            cboDocumentType.Items.AddRange(new string[] { "Phiếu giao hàng đã giao" });
            cboDocumentType.SelectedIndex = 0;
            cboDocumentType.SelectedIndexChanged += cboDocumentType_SelectedIndexChanged;

            txtDocumentCode.Text = _repo.GenerateSalesCode();
            txtDocumentCode.ReadOnly = true;

            BuildSalesAccountingSection();
            SetupDetailGrid();
            SetupInvoiceGrid();
            LoadInvoiceList();

            // Combo nguồn: SelectedIndex=0 gán trước khi gắn event nên gọi tải danh sách PGH ở đây
            LoadDeliveredNotesForSalesCombo();

            RefreshSalesInvoiceNumber();

            dtpNgayBanHang.ValueChanged -= SalesDate_ValueChanged;
            dtpNgayBanHang.ValueChanged += SalesDate_ValueChanged;
            SyncSalesExpectedPaymentDate();

            // Đảm bảo có vùng cuộn khi nhúng trong panel nhỏ / màn hình thấp
            ApplySalesPanelScrollExtent();

            // Xóa highlight đỏ khi người dùng bắt đầu nhập
            txtInvoiceForm.TextChanged += (s, _) => txtInvoiceForm.BackColor = Color.White;
            txtInvoiceSymbol.TextChanged += (s, _) => txtInvoiceSymbol.BackColor = Color.White;
            txtInvoiceNumber.TextChanged += (s, _) => txtInvoiceNumber.BackColor = Color.White;
        }

        private void SalesDate_ValueChanged(object? sender, EventArgs e) => SyncSalesExpectedPaymentDate();

        /// <summary>
        /// Tính chiều cao nội dung thực tế để panel5 luôn cuộn được tới nút Lưu / In.
        /// </summary>
        private void ApplySalesPanelScrollExtent()
        {
            int maxBottom = 0;
            foreach (Control c in panel5.Controls)
                maxBottom = Math.Max(maxBottom, c.Bottom);
            panel5.AutoScroll = true;
            panel5.AutoScrollMargin = new Size(24, 48);
            panel5.AutoScrollMinSize = new Size(Math.Max(panel5.ClientSize.Width, 1080), maxBottom + 48);
        }


        // ─────────────────────────────────────────────────────
        // HẠCH TOÁN / HẠN NỢ (giống pattern frmInventoryReceive)
        // ─────────────────────────────────────────────────────

        private static string[] DebitAccountItems =>
            new[]
            {
                "131 - Phải thu của khách hàng",
                "136 - Phải thu nội bộ",
                "138 - Phải thu khác",
                "511 - Doanh thu bán hàng và cung cấp dịch vụ",
                "515 - Doanh thu hoạt động tài chính"
            };

        private static string[] CreditAccountItems =>
            new[]
            {
                "511 - Doanh thu bán hàng và cung cấp dịch vụ",
                "3331 - Thuế GTGT phải nộp",
                "131 - Phải thu của khách hàng",
                "111 - Tiền mặt",
                "112 - Tiền gửi Ngân hàng"
            };

        private static string ExtractLedgerAccountCode(string? displayText)
        {
            if (string.IsNullOrWhiteSpace(displayText))
                return "";
            int sep = displayText.IndexOf(" - ", StringComparison.Ordinal);
            return sep > 0 ? displayText[..sep].Trim() : displayText.Trim();
        }

        private void ClearSalesAccountingSection()
        {
            if (panel5 == null) return;
            var toRemove = new System.Collections.Generic.List<Control>();
            foreach (Control c in panel5.Controls)
            {
                string t = c.Text;
                if (t == "Hạch toán & hạn nợ" || t == "TK nợ:" || t == "TK có:"
                    || t == "Số ngày nợ:" || t == "Ngày ước thanh toán:"
                    || c == _btnPrintSales)
                    toRemove.Add(c);
            }
            foreach (var c in toRemove) { panel5.Controls.Remove(c); c.Dispose(); }
            _btnPrintSales = null;
        }

        /// <summary>Quy đổi khoảng cách thiết kế 96 DPI sang pixel thực tế (máy 125%/150% không bị đè).</summary>
        private int ScaleGap(int designPixelsAt96) =>
            (int)Math.Round(designPixelsAt96 * (double)DeviceDpi / 96.0);

        private void BuildSalesAccountingSection()
        {
            if (_salesAccountingLayoutDone)
                return;
            _salesAccountingLayoutDone = true;

            ClearSalesAccountingSection();

            int Px(int d) => ScaleGap(d);

            // Đáy khối nhập hóa đơn (đã được WinForms scale theo font/DPI) — không dùng Y cố định 448.
            int invoiceBottom = 0;
            foreach (var c in new Control?[] { txtInvoiceForm, txtInvoiceSymbol, txtInvoiceNumber, dtpInvoiceDate })
            {
                if (c != null)
                    invoiceBottom = Math.Max(invoiceBottom, c.Bottom);
            }

            int y = invoiceBottom + Px(12);
            Font smallF = label73.Font;

            var lblAcc = new Label
            {
                Text = "Hạch toán & hạn nợ",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.DarkBlue,
                Location = new Point(Px(17), y)
            };
            panel5.Controls.Add(lblAcc);
            y = lblAcc.Bottom + Px(8);

            var lblTkNo = new Label { Text = "TK nợ:", AutoSize = true, Font = smallF, Location = new Point(Px(19), y + Px(3)) };
            cboDebitAccount = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = smallF,
                Location = new Point(Px(72), y),
                Width = Px(360)
            };
            cboDebitAccount.Items.AddRange(DebitAccountItems);
            cboDebitAccount.SelectedIndex = 0;

            var lblTkCo = new Label { Text = "TK có:", AutoSize = true, Font = smallF, Location = new Point(Px(448), y + Px(3)) };
            cboCreditAccount = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = smallF,
                Location = new Point(Px(502), y),
                Width = Px(360)
            };
            cboCreditAccount.Items.AddRange(CreditAccountItems);
            cboCreditAccount.SelectedIndex = 0;

            y = Math.Max(Math.Max(lblTkNo.Bottom, cboDebitAccount.Bottom),
                Math.Max(lblTkCo.Bottom, cboCreditAccount.Bottom)) + Px(10);

            var lblDays = new Label { Text = "Số ngày nợ:", AutoSize = true, Font = smallF, Location = new Point(Px(19), y + Px(3)) };
            nudCreditDays = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 3650,
                Value = 30,
                Font = smallF,
                Location = new Point(Px(100), y),
                Width = Px(72)
            };
            nudCreditDays.ValueChanged += (_, _) => SyncSalesExpectedPaymentDate();

            var lblDue = new Label { Text = "Ngày ước thanh toán:", AutoSize = true, Font = smallF, Location = new Point(Px(190), y + Px(3)) };
            dtpExpectedPaymentDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Font = smallF,
                Location = new Point(Px(322), y),
                Width = Px(130)
            };

            panel5.Controls.Add(lblTkNo);
            panel5.Controls.Add(cboDebitAccount);
            panel5.Controls.Add(lblTkCo);
            panel5.Controls.Add(cboCreditAccount);
            panel5.Controls.Add(lblDays);
            panel5.Controls.Add(nudCreditDays);
            panel5.Controls.Add(lblDue);
            panel5.Controls.Add(dtpExpectedPaymentDate);

            int accBottom = y;
            foreach (var c in new Control?[] { lblDays, nudCreditDays, lblDue, dtpExpectedPaymentDate, lblTkNo, cboDebitAccount, lblTkCo, cboCreditAccount, lblAcc })
            {
                if (c != null)
                    accBottom = Math.Max(accBottom, c.Bottom);
            }

            // Chi tiết sản phẩm — luôn ngay dưới khối hạch toán (không cộng blockHeight cố định).
            y = accBottom + Px(14);
            label49.Location = new Point(label49.Left, y);
            pictureBox14.Location = new Point(pictureBox14.Left, y);
            y = Math.Max(label49.Bottom, pictureBox14.Bottom) + Px(8);
            dgvOrderDetails.SetBounds(dgvOrderDetails.Left, y, dgvOrderDetails.Width, Px(146));

            y = dgvOrderDetails.Bottom + Px(14);
            label36.Location = new Point(label36.Left, y);
            pictureBox18.Location = new Point(pictureBox18.Left, y);
            y = Math.Max(label36.Bottom, pictureBox18.Bottom) + Px(8);
            dgvInvoiceList.SetBounds(dgvInvoiceList.Left, y, dgvInvoiceList.Width, Px(184));

            y = dgvInvoiceList.Bottom + Px(18);
            btnSave.Location = new Point(btnSave.Left, y);
            lblGrandTotal.Location = new Point(lblGrandTotal.Left, y);

            _btnPrintSales = new Guna2Button
            {
                BorderRadius = 10,
                FillColor = Color.Olive,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.White,
                Location = new Point(btnSave.Right + Px(14), y),
                Size = btnSave.Size,
                Text = "In phiếu bán",
                TextOffset = new Point(-2, 0)
            };
            _btnPrintSales.Click += BtnPrintSales_Click;
            panel5.Controls.Add(_btnPrintSales);
            _btnPrintSales.BringToFront();

            ApplySalesPanelScrollExtent();
        }

        private void SyncSalesExpectedPaymentDate()
        {
            if (dtpExpectedPaymentDate == null || nudCreditDays == null)
                return;
            var baseDate = dtpNgayBanHang.Value.Date;
            dtpExpectedPaymentDate.Value = baseDate.AddDays((double)nudCreditDays.Value);
        }

        private void RefreshSalesInvoiceNumber()
        {
            try
            {
                txtInvoiceNumber.Text = _repo.GenerateSalesInvoiceNumber();
            }
            catch
            {
                txtInvoiceNumber.Text = $"HD-{DateTime.Today.Year}-000001";
            }

            txtInvoiceNumber.ReadOnly = true;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID CHI TIẾT ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        private void SetupDetailGrid()
        {
            var dgv = dgvOrderDetails;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            var styleRight = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 5,
                MinimumWidth = 45,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRight)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colProductName",
                HeaderText = "Tên sản phẩm",
                FillWeight = 35,
                MinimumWidth = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f),
                    Padding = new Padding(10, 0, 8, 0),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    WrapMode = DataGridViewTriState.False
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQuantity",
                HeaderText = "Số lượng",
                FillWeight = 10,
                MinimumWidth = 80,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUnitPrice",
                HeaderText = "Đơn giá",
                FillWeight = 13,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLineTotal",
                HeaderText = "Thành tiền",
                FillWeight = 15,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRight)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 80, 160)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVatRate",
                HeaderText = "VAT (%)",
                FillWeight = 7,
                MinimumWidth = 65,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRight)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVatAmount",
                HeaderText = "Tiền VAT",
                FillWeight = 15,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetailId", Visible = false });
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID DANH SÁCH HÓA ĐƠN
        // ─────────────────────────────────────────────────────

        private void SetupInvoiceGrid()
        {
            var dgv = dgvInvoiceList;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(76, 175, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            var styleCenter = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var styleRight = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleRight };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colInvoiceCode", HeaderText = "Số HĐ", FillWeight = 12, MinimumWidth = 100, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colInvoiceDate", HeaderText = "Ngày HĐ", FillWeight = 10, MinimumWidth = 90, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colCustomer", HeaderText = "Khách hàng", FillWeight = 28, MinimumWidth = 130 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colProduct", HeaderText = "Sản phẩm", FillWeight = 25, MinimumWidth = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colQuantity", HeaderText = "Số lượng", FillWeight = 10, MinimumWidth = 80, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalAmount",
                HeaderText = "Tổng tiền",
                FillWeight = 15,
                MinimumWidth = 120,
                DefaultCellStyle = new DataGridViewCellStyle(styleRight)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(76, 175, 80)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStatus", HeaderText = "Trạng thái", FillWeight = 10, MinimumWidth = 85, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colOrderId", Visible = false });
        }


        // ─────────────────────────────────────────────────────
        // CHỌN LOẠI CHỨNG TỪ
        // ─────────────────────────────────────────────────────

        private void cboDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string documentType = cboDocumentType.SelectedItem?.ToString() ?? "";

            if (documentType == "Phiếu giao hàng đã giao")
            {
                cboSalesOrder.Enabled = true;
                LoadDeliveredNotesForSalesCombo();
            }
            else
            {
                cboSalesOrder.Enabled = false;
                cboSalesOrder.Items.Clear();
                cboSalesOrder.Text = "";
            }
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH PHIẾU GIAO ĐÃ GIAO (chưa lập đơn bán)
        // ─────────────────────────────────────────────────────

        private void LoadDeliveredNotesForSalesCombo()
        {
            cboSalesOrder.SelectedIndexChanged -= cboSalesOrder_SelectedIndexChanged;
            cboSalesOrder.Items.Clear();
            cboSalesOrder.Items.Add(new DeliveryNoteItem()); // dòng trống

            try
            {
                var dt = _repo.GetDeliveredNotesForSales();

                foreach (DataRow row in dt.Rows)
                {
                    var ngay = row["Ngay_Giao"] != DBNull.Value
                        ? Convert.ToDateTime(row["Ngay_Giao"])
                        : DateTime.Today;

                    cboSalesOrder.Items.Add(new DeliveryNoteItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        MaPhieu = row["Ma_Phieu_Giao"]?.ToString() ?? "",
                        CustomerName = row["Ten_KH"]?.ToString() ?? "",
                        CustomerId = row["id_Khach_Hang"] != DBNull.Value
                            ? Convert.ToInt32(row["id_Khach_Hang"]) : 0,
                        NgayGiao = ngay
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }

            cboSalesOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSalesOrder.SelectedIndexChanged += cboSalesOrder_SelectedIndexChanged;
            cboSalesOrder.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // CHỌN PHIẾU GIAO → NẠP KHÁCH + CHI TIẾT
        // ─────────────────────────────────────────────────────

        private void cboSalesOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboSalesOrder.SelectedItem is DeliveryNoteItem note) || note.Id == 0)
            {
                _phieuGiaoId = 0;
                _customerId = 0;
                txtCustomerName.Clear();
                txtAddress.Clear();
                txtTaxCode.Clear();
                dgvOrderDetails.Rows.Clear();
                lblGrandTotal.Text = "0đ";
                return;
            }

            _phieuGiaoId = note.Id;
            _customerId = note.CustomerId;

            try
            {
                var ds = _deliveryRepo.GetDeliveryNoteById(_phieuGiaoId);
                if (ds.Tables.Count < 1 || ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Không tải được phiếu giao hàng.", "Lỗi");
                    return;
                }

                var pgh = ds.Tables[0].Rows[0];
                if (pgh["id_Khach_Hang"] != DBNull.Value)
                    _customerId = Convert.ToInt32(pgh["id_Khach_Hang"]);

                if (_customerId > 0)
                    LoadCustomerInfo(_customerId);
                else
                {
                    txtCustomerName.Text = pgh["Ten_KH"]?.ToString() ?? "";
                    txtAddress.Text = pgh["Dia_Chi_Giao_Hang"]?.ToString() ?? "";
                    txtTaxCode.Clear();
                }

                if (ds.Tables.Count > 1)
                    LoadDeliveryNoteDetailsIntoGrid(ds.Tables[1]);
                else
                    dgvOrderDetails.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // NẠP THÔNG TIN KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void LoadCustomerInfo(int customerId)
        {
            if (customerId == 0)
            {
                txtCustomerName.Text = "(Khách lẻ)";
                txtAddress.Clear();
                txtTaxCode.Clear();
                return;
            }

            try
            {
                // Toàn bộ SQL nằm trong SalesRepository (dùng lại sp_GetCustomerById)
                var dt = _repo.GetCustomerById(customerId);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtCustomerName.Text = row["Ten_Khach_Hang"].ToString();
                    txtAddress.Text = row["Dia_Chi"]?.ToString() ?? "";
                    txtTaxCode.Text = row["MST"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // NẠP CHI TIẾT PHIẾU GIAO VÀO GRID (CHI_TIET_PGH)
        // ─────────────────────────────────────────────────────

        private void LoadDeliveryNoteDetailsIntoGrid(DataTable chiTiet)
        {
            dgvOrderDetails.Rows.Clear();

            try
            {
                int rowIndex = 1;
                double subTotal = 0;
                double vatRate = 10;
                bool hasBgCols = chiTiet.Columns.Contains("Tong_Bao_Gia_Muc");

                foreach (DataRow dataRow in chiTiet.Rows)
                {
                    double qty = dataRow["So_Luong_Giao"] != DBNull.Value
                        ? Convert.ToDouble(dataRow["So_Luong_Giao"]) : 0;
                    double unitFromPhieu = dataRow["Don_Gia"] != DBNull.Value
                        ? Convert.ToDouble(dataRow["Don_Gia"]) : 0;
                    double lineTotalPhieu = dataRow["Thanh_Tien"] != DBNull.Value
                        ? Convert.ToDouble(dataRow["Thanh_Tien"]) : 0;

                    bool linkedBg = chiTiet.Columns.Contains("id_Chi_Tiet_Bao_Gia")
                        && dataRow["id_Chi_Tiet_Bao_Gia"] != DBNull.Value
                        && Convert.ToInt32(dataRow["id_Chi_Tiet_Bao_Gia"]) > 0;

                    decimal tongBg = 0;
                    int slMucBg = 0;
                    decimal donGiaBg = 0;
                    if (hasBgCols && linkedBg)
                    {
                        if (dataRow["Tong_Bao_Gia_Muc"] != DBNull.Value)
                            tongBg = Convert.ToDecimal(dataRow["Tong_Bao_Gia_Muc"]);
                        if (dataRow["So_Luong_Muc_Bao_Gia"] != DBNull.Value)
                            slMucBg = Convert.ToInt32(dataRow["So_Luong_Muc_Bao_Gia"]);
                        if (dataRow["Don_Gia_Bao_Gia"] != DBNull.Value)
                            donGiaBg = Convert.ToDecimal(dataRow["Don_Gia_Bao_Gia"]);
                    }

                    // Ưu tiên đơn giá từ mức báo giá đã liên kết: Tong_Gia_Bao_Khach / So_Luong
                    // (đồng bộ với màn Quản lý báo giá), rồi thành tiền = SL giao × đơn giá.
                    // Fallback: Gia_Bao_Khach; cuối cùng mới dùng Thanh_Tien/Don_Gia trên PGH.
                    double unitPrice;
                    double lineTotal;
                    if (linkedBg && tongBg > 0 && slMucBg > 0)
                    {
                        decimal unitDec = tongBg / slMucBg;
                        unitPrice = (double)unitDec;
                        lineTotal = (double)Math.Round((decimal)qty * unitDec, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (linkedBg && donGiaBg > 0 && qty > 0)
                    {
                        unitPrice = (double)donGiaBg;
                        lineTotal = (double)Math.Round((decimal)qty * donGiaBg, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (qty > 0 && lineTotalPhieu > 0)
                    {
                        unitPrice = lineTotalPhieu / qty;
                        lineTotal = lineTotalPhieu;
                    }
                    else if (qty > 0)
                    {
                        unitPrice = unitFromPhieu;
                        lineTotal = qty * unitPrice;
                    }
                    else
                    {
                        unitPrice = unitFromPhieu;
                        lineTotal = lineTotalPhieu;
                    }

                    double vatAmount = Math.Round(lineTotal * vatRate / 100, 0);
                    int lineId = dataRow["id"] != DBNull.Value ? Convert.ToInt32(dataRow["id"]) : 0;

                    dgvOrderDetails.Rows.Add(
                        rowIndex,
                        dataRow["Ten_San_Pham"],
                        qty.ToString("N0"),
                        unitPrice.ToString("N0"),
                        lineTotal.ToString("N0"),
                        vatRate,
                        vatAmount.ToString("N0"),
                        lineId);

                    subTotal += lineTotal;
                    rowIndex++;
                }

                double totalVat = Math.Round(subTotal * vatRate / 100, 0);
                double grandTotal = subTotal + totalVat;
                lblGrandTotal.Text = grandTotal.ToString("N0") + "đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // IN PHIẾU BÁN (xem trước khi in)
        // ─────────────────────────────────────────────────────

        private void BtnPrintSales_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Không có dòng sản phẩm để in.", "Thiếu dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lưu dữ liệu vào field — tránh capture lambda
            _printLines.Clear();
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                if (row.IsNewRow) continue;
                _printLines.Add(new[]
                {
                    row.Cells["colProductName"].Value?.ToString() ?? "",
                    row.Cells["colQuantity"].Value?.ToString() ?? "",
                    row.Cells["colUnitPrice"].Value?.ToString() ?? "",
                    row.Cells["colLineTotal"].Value?.ToString() ?? "",
                    row.Cells["colVatRate"].Value?.ToString() ?? "",
                    row.Cells["colVatAmount"].Value?.ToString() ?? ""
                });
            }

            _printDocCode = txtDocumentCode.Text.Trim();
            _printCustomer = txtCustomerName.Text.Trim();
            _printAddress = txtAddress.Text.Trim();
            _printTax = txtTaxCode.Text.Trim();
            _printSaleDate = dtpNgayBanHang.Value.ToString("dd/MM/yyyy");
            _printInvMeta = $"{txtInvoiceForm.Text.Trim()} / {txtInvoiceSymbol.Text.Trim()} / {txtInvoiceNumber.Text.Trim()}";
            _printInvDate = dtpInvoiceDate.Value.ToString("dd/MM/yyyy");
            _printTkNo = cboDebitAccount?.SelectedItem?.ToString() ?? "";
            _printTkCo = cboCreditAccount?.SelectedItem?.ToString() ?? "";
            _printDue = dtpExpectedPaymentDate?.Value.ToString("dd/MM/yyyy") ?? "";
            _printTotal = lblGrandTotal.Text;

            // Gỡ handler cũ trước khi thêm mới — tránh in nhiều trang
            if (_currentPrintDoc != null)
                _currentPrintDoc.PrintPage -= PrintSalesPage;

            _currentPrintDoc = new PrintDocument();
            _currentPrintDoc.DefaultPageSettings.Landscape = true;
            _currentPrintDoc.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            _currentPrintDoc.PrintPage += PrintSalesPage;

            using var preview = new PrintPreviewDialog
            {
                Document = _currentPrintDoc,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            };
            preview.ShowDialog(this);
        }

        private void PrintSalesPage(object sender, PrintPageEventArgs ev)
        {
            Graphics g = ev.Graphics!;
            float x = 40f, y = 40f;
            float pageW = ev.PageBounds.Width - 80f;

            using var fTitle = new Font("Segoe UI", 15f, FontStyle.Bold);
            using var fSub = new Font("Segoe UI", 9.5f, FontStyle.Italic);
            using var f = new Font("Segoe UI", 9.5f);
            using var fBold = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var fCell = new Font("Segoe UI", 9f);
            using var fTot = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var fSign = new Font("Segoe UI", 9f);
            using var fSignIt = new Font("Segoe UI", 8f, FontStyle.Italic);
            using var headerBg = new SolidBrush(Color.FromArgb(220, 230, 255));
            using var totalBrush = new SolidBrush(Color.FromArgb(30, 80, 160));

            g.DrawString("PHIẾU BÁN HÀNG / CHỨNG TỪ BÁN", fTitle, Brushes.Black, x, y); y += 28;
            g.DrawString("(Bản in từ màn hình — kiểm tra trước khi ký xác nhận)", fSub, Brushes.Gray, x, y); y += 24;

            void Info(string label, string val)
            {
                g.DrawString(label, f, Brushes.Gray, x, y);
                g.DrawString(val, fBold, Brushes.Black, x + 140, y);
                y += 18;
            }

            Info("Mã chứng từ:", _printDocCode);
            Info("Ngày bán:", _printSaleDate);
            Info("Khách hàng:", _printCustomer);
            Info("Địa chỉ:", _printAddress);
            Info("MST:", _printTax);
            Info("HĐ (mẫu / ký hiệu / số):", _printInvMeta);
            Info("Ngày hóa đơn:", _printInvDate);
            Info("TK nợ — TK có:", $"{_printTkNo}  |  {_printTkCo}");
            Info("Ngày ước thanh toán:", _printDue);

            y += 6;
            g.DrawLine(new Pen(Color.FromArgb(180, 190, 220), 1f), x, y, x + pageW, y); y += 10;

            float rowH = 22f;
            float[] colX = { x, x + 32, x + 230, x + 310, x + 400, x + 500, x + 560 };
            g.FillRectangle(headerBg, x, y, pageW, rowH - 2);
            string[] hdr = { "STT", "Tên hàng", "SL", "Đơn giá", "Thành tiền", "%VAT", "Tiền VAT" };
            for (int i = 0; i < hdr.Length; i++)
                g.DrawString(hdr[i], fBold, Brushes.Black, colX[i] + 3, y + 2);
            y += rowH;

            int stt = 1;
            foreach (string[] line in _printLines)
            {
                g.DrawString(stt.ToString(), fCell, Brushes.Black, colX[0] + 3, y + 2);
                for (int j = 0; j < line.Length; j++)
                    g.DrawString(line[j], fCell, Brushes.Black, colX[j + 1] + 3, y + 2);
                y += rowH;
                stt++;
            }

            y += 12;
            float totRowH = 26f;
            var fmtLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            var fmtRight = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            g.DrawString("TỔNG THANH TOÁN:", fTot, Brushes.Black, new RectangleF(x, y, pageW * 0.55f, totRowH), fmtLeft);
            g.DrawString(_printTotal, fTot, totalBrush, new RectangleF(x, y, pageW, totRowH), fmtRight); y += 36;

            string[] signers = { "Người lập chứng từ", "Kế toán", "Khách hàng" };
            float signW = pageW / 3f;
            for (int i = 0; i < 3; i++)
            {
                g.DrawString(signers[i], fSign, Brushes.Black, x + i * signW + 16, y);
                g.DrawString("(Ký, ghi rõ họ tên)", fSignIt, Brushes.Gray, x + i * signW + 16, y + 18);
            }
        }


        // ─────────────────────────────────────────────────────
        // NÚT LƯU CHỨNG TỪ
        // ─────────────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_phieuGiaoId == 0)
            { MessageBox.Show("⚠️ Vui lòng chọn phiếu giao hàng!", "Thiếu thông tin"); return; }

            if (dgvOrderDetails.Rows.Count == 0)
            { MessageBox.Show("⚠️ Không có sản phẩm nào!", "Thiếu thông tin"); return; }

            // Validate thông tin hóa đơn
            txtInvoiceForm.BackColor = Color.White;
            txtInvoiceSymbol.BackColor = Color.White;
            txtInvoiceNumber.BackColor = Color.White;

            bool isInvoiceValid = true;
            string invoiceErrors = "";

            if (string.IsNullOrWhiteSpace(txtInvoiceForm.Text))
            {
                txtInvoiceForm.BackColor = Color.FromArgb(255, 235, 235);
                txtInvoiceForm.Focus();
                invoiceErrors += "• Mẫu số hóa đơn\n";
                isInvoiceValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtInvoiceSymbol.Text))
            {
                txtInvoiceSymbol.BackColor = Color.FromArgb(255, 235, 235);
                if (isInvoiceValid) txtInvoiceSymbol.Focus();
                invoiceErrors += "• Ký hiệu hóa đơn\n";
                isInvoiceValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtInvoiceNumber.Text))
            {
                txtInvoiceNumber.BackColor = Color.FromArgb(255, 235, 235);
                if (isInvoiceValid) txtInvoiceNumber.Focus();
                invoiceErrors += "• Số hóa đơn\n";
                isInvoiceValid = false;
            }

            if (!isInvoiceValid)
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin hóa đơn:\n\n" + invoiceErrors,
                    "Thiếu thông tin hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                $"Xác nhận lưu đơn bán hàng?\n\nKhách hàng: {txtCustomerName.Text}\nTổng tiền: {lblGrandTotal.Text}",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // Tính tổng từ grid
                double subTotal = 0;
                double vatRate = 10;
                foreach (DataGridViewRow row in dgvOrderDetails.Rows)
                {
                    double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                    subTotal += lineTotal;
                }
                double totalVat = Math.Round(subTotal * vatRate / 100, 0);
                double grandTotal = subTotal + totalVat;

                // Đóng gói chi tiết thành DataTable
                var details = BuildSalesDetailTable(vatRate);

                // Toàn bộ SQL nằm trong SalesRepository
                int newOrderId = _repo.SaveSalesOrder(
                    txtDocumentCode.Text.Trim(),
                    _customerId,
                    dtpNgayBanHang.Value.Date,
                    txtInvoiceForm.Text.Trim(),
                    txtInvoiceSymbol.Text.Trim(),
                    txtInvoiceNumber.Text.Trim(),
                    dtpInvoiceDate.Value.Date,
                    (decimal)subTotal,
                    (decimal)vatRate,
                    (decimal)totalVat,
                    (decimal)grandTotal,
                    quoteId: 0,
                    phieuGiaoId: _phieuGiaoId,
                    details,
                    dtpExpectedPaymentDate?.Value.Date,
                    ExtractLedgerAccountCode(cboDebitAccount?.SelectedItem?.ToString()),
                    ExtractLedgerAccountCode(cboCreditAccount?.SelectedItem?.ToString()));

                MessageBox.Show($"✅ Lưu thành công!\nMã đơn: {txtDocumentCode.Text}", "Thành công");
                LoadInvoiceList();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DỮ LIỆU GRID SANG DATATABLE
        // ─────────────────────────────────────────────────────

        private DataTable BuildSalesDetailTable(double vatRate)
        {
            var dt = new DataTable();
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("Quantity", typeof(double));
            dt.Columns.Add("UnitPrice", typeof(double));
            dt.Columns.Add("LineTotal", typeof(double));
            dt.Columns.Add("VatRate", typeof(double));
            dt.Columns.Add("VatAmount", typeof(double));

            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                string productName = row.Cells["colProductName"].Value?.ToString() ?? "";
                double.TryParse(row.Cells["colQuantity"].Value?.ToString().Replace(",", ""), out double qty);
                double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double unitPrice);
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                double vatAmount = Math.Round(lineTotal * vatRate / 100, 0);

                dt.Rows.Add(productName, qty, unitPrice, lineTotal, vatRate, vatAmount);
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH HÓA ĐƠN
        // ─────────────────────────────────────────────────────

        private void LoadInvoiceList()
        {
            dgvInvoiceList.Rows.Clear();

            try
            {
                // Toàn bộ SQL nằm trong SalesRepository
                var dt = _repo.GetSalesInvoiceList();

                foreach (DataRow dataRow in dt.Rows)
                {
                    string dateStr = dataRow["Ngay_Ban_Hang"] != DBNull.Value
                        ? Convert.ToDateTime(dataRow["Ngay_Ban_Hang"]).ToString("dd/MM/yyyy") : "";
                    double totalAmount = dataRow["Tong_Thanh_Toan"] != DBNull.Value
                        ? Convert.ToDouble(dataRow["Tong_Thanh_Toan"]) : 0;

                    string hienThiHd = dataRow["Ma_Don_Ban"]?.ToString() ?? "";
                    if (dataRow.Table.Columns.Contains("So_Hien_Thi_Hoa_Don")
                        && dataRow["So_Hien_Thi_Hoa_Don"] != DBNull.Value)
                    {
                        var s = dataRow["So_Hien_Thi_Hoa_Don"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(s)) hienThiHd = s;
                    }

                    dgvInvoiceList.Rows.Add(
                        hienThiHd,
                        dateStr,
                        dataRow["Ten_Khach_Hang"],
                        dataRow["Ten_San_Pham"],
                        Convert.ToDouble(dataRow["So_Luong"]).ToString("N0"),
                        totalAmount.ToString("N0") + "đ",
                        dataRow["Trang_Thai"],
                        dataRow["id"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // RESET FORM
        // ─────────────────────────────────────────────────────

        private void ResetForm()
        {
            cboDocumentType.SelectedIndex = 0;
            if (cboSalesOrder.Items.Count > 0) cboSalesOrder.SelectedIndex = 0;

            txtCustomerName.Clear();
            txtAddress.Clear();
            txtTaxCode.Clear();
            txtInvoiceForm.Clear(); txtInvoiceForm.BackColor = Color.White;
            txtInvoiceSymbol.Clear(); txtInvoiceSymbol.BackColor = Color.White;
            txtInvoiceNumber.Clear(); txtInvoiceNumber.BackColor = Color.White;
            dtpInvoiceDate.Value = DateTime.Today;
            dgvOrderDetails.Rows.Clear();
            lblGrandTotal.Text = "0đ";

            // Sinh mã mới
            txtDocumentCode.Text = _repo.GenerateSalesCode();
            RefreshSalesInvoiceNumber();
            if (nudCreditDays != null)
                nudCreditDays.Value = 30;
            // [FIX] Reset ngày ước thanh toán về ngày bán + credit days mới (tránh in sai hạn của đơn cũ)
            if (dtpExpectedPaymentDate != null)
                dtpExpectedPaymentDate.Value = dtpNgayBanHang.Value.Date.AddDays((double)nudCreditDays.Value);
            SyncSalesExpectedPaymentDate();

            _phieuGiaoId = 0;
            _customerId = 0;

            LoadDeliveredNotesForSalesCombo();
        }


        // ─────────────────────────────────────────────────────
        // INNER CLASS — PHIẾU GIAO TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class DeliveryNoteItem
        {
            public int Id { get; set; }
            public string MaPhieu { get; set; } = "";
            public string CustomerName { get; set; } = "";
            public int CustomerId { get; set; }
            public DateTime NgayGiao { get; set; }

            public override string ToString() =>
                Id == 0 ? "-- Chọn phiếu giao hàng --"
                        : $"{MaPhieu} - {CustomerName} ({NgayGiao:dd/MM/yyyy})";
        }
    }
}