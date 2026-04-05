// ═══════════════════════════════════════════════════════════════════
// ║  frmInventoryReceive.cs - PHIẾU NHẬP KHO                       ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] Thêm:
//   1. Panel hạch toán kế toán (TK Nợ 152/153/156 / TK Có 331/111)
//   2. Số ngày nợ (txtSoNgayNo) + DateTimePicker ngày đến hạn TT
//   3. Phiếu in hiển thị đầy đủ định khoản kế toán + ngày đến hạn
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryReceive : Form
    {
        // ─────────────────────────────────────────────────────
        // PRIVATE FIELDS
        // ─────────────────────────────────────────────────────

        private readonly InventoryReceiveRepository _repo = new();

        private int _orderId = 0;
        private int _supplierId = 0;
        private int _receiptId = 0;
        private bool _isPosted = false;
        private bool _isSaved = false;
        private string _invoiceFile = "";

        // [NEW] Controls thêm — khai báo ở đây vì designer chưa có


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO
        // ─────────────────────────────────────────────────────

        public frmInventoryReceive()
        {
            InitializeComponent();
            // TK Nợ
            cboTKNo.Items.Clear();
            cboTKNo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTKNo.Items.AddRange(new string[]
            {
    "152 - Nguyên liệu, vật liệu",
    "153 - Công cụ, dụng cụ",
    "156 - Hàng hóa",
    "211 - Tài sản cố định hữu hình",
    "242 - Chi phí trả trước",
    "627 - Chi phí sản xuất chung",
    "641 - Chi phí bán hàng",
    "642 - Chi phí quản lý doanh nghiệp"
            });
            cboTKNo.SelectedIndex = 0;

            // TK Có
            cboTKCo.Items.Clear();
            cboTKCo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTKCo.Items.AddRange(new string[]
            {
    "331 - Phải trả nhà cung cấp",
    "111 - Tiền mặt",
    "112 - Tiền gửi ngân hàng",
    "311 - Vay ngắn hạn",
    "341 - Vay dài hạn"
            });
            cboTKCo.SelectedIndex = 0;
            this.Load += frmInventoryReceive_Load;
        }

        public frmInventoryReceive(int orderId) : this()
        {
            _orderId = orderId;
        }

        // ─────────────────────────────────────────────────────
        // [NEW] BUILD CONTROLS THÊM (không có trong designer cũ)
        // Gọi 1 lần trong constructor — thêm vào panel phù hợp
        // ─────────────────────────────────────────────────────

        private void BuildExtraControls()
        {
            Control parent = this.Controls["pnlHeader"] ?? (Control)this;

            // bám theo control có sẵn để khỏi “nhảy lung tung”
            int baseY = dtpDocumentDate.Bottom + 12;

            nudSoNgayNoo = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 365,
                Value = 0,
                Width = 80,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(120, baseY - 3)
            };
            nudSoNgayNoo.ValueChanged += nudSoNgayNoo_ValueChanged;

            dtpNgayDenHan = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(220, baseY - 3)
            };

            cboTKNo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 280,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(160, baseY + 65)
            };
            cboTKNo.Items.AddRange(new string[]
            {
        "152 - Nguyên liệu, vật liệu",
        "153 - Công cụ, dụng cụ",
        "156 - Hàng hóa",
        "211 - Tài sản cố định hữu hình",
        "242 - Chi phí trả trước",
        "627 - Chi phí sản xuất chung",
        "641 - Chi phí bán hàng",
        "642 - Chi phí quản lý doanh nghiệp"
            });
            cboTKNo.SelectedIndex = 0;

            cboTKCo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 280,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(160, baseY + 97)
            };
            cboTKCo.Items.AddRange(new string[]
            {
        "331 - Phải trả nhà cung cấp",
        "111 - Tiền mặt",
        "112 - Tiền gửi ngân hàng",
        "311 - Vay ngắn hạn",
        "341 - Vay dài hạn"
            });
            cboTKCo.SelectedIndex = 0;

            parent.Controls.Add(nudSoNgayNoo);
            parent.Controls.Add(dtpNgayDenHan);
            parent.Controls.Add(cboTKNo);
            parent.Controls.Add(cboTKCo);
        }

        // ─────────────────────────────────────────────────────
        // [NEW] TỰ TÍNH NGÀY ĐẾN HẠN KHI THAY ĐỔI SỐ NGÀY NỢ
        // ─────────────────────────────────────────────────────

        private void nudSoNgayNoo_ValueChanged(object sender, EventArgs e)
        {
            dtpNgayDenHan.Value = dtpDocumentDate.Value.AddDays((double)nudSoNgayNoo.Value);
        }

        // ─────────────────────────────────────────────────────
        // LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmInventoryReceive_Load(object sender, EventArgs e)
        {
            cboReceiveType.Items.Clear();
            cboReceiveType.Items.AddRange(new string[]
            {
                "Mua trong nước nhập kho",
                "Mua trong nước không qua kho",
                "Nhập khẩu nhập kho",
                "Nhập khẩu không qua kho"
            });
            cboReceiveType.SelectedIndex = 0;

            cboPaymentStatus.Items.Clear();
            cboPaymentStatus.Items.AddRange(new string[] { "Chưa thanh toán", "Thanh toán ngay" });
            cboPaymentStatus.SelectedIndex = 0;

            // [FIX] Khi chọn "Thanh toán ngay" → TK Có chuyển về 111/112
            cboPaymentStatus.SelectedIndexChanged += (s, ev) =>
            {
                if (cboTKCo == null) return;
                string status = cboPaymentStatus.SelectedItem?.ToString() ?? "";
                if (status == "Thanh toán ngay")
                    cboTKCo.SelectedIndex = 1; // 111 - Tiền mặt
                else
                    cboTKCo.SelectedIndex = 0; // 331 - Phải trả NCC
            };

            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange(new string[]
                { "Tiền mặt", "Ủy nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPaymentMethod.SelectedIndex = 0;

            cboInvoiceStatus.Items.Clear();
            cboInvoiceStatus.Items.AddRange(new string[]
                { "Nhận kèm hóa đơn", "Không kèm hóa đơn", "Không có hóa đơn" });
            cboInvoiceStatus.SelectedIndex = 0;

            dtpDocumentDate.Value = DateTime.Today;
            dtpAccountingDate.Value = DateTime.Today;
            dtpInvoiceDate.Value = DateTime.Today;
            txtInvoiceForm.Text = "01GTKT";

            // [NEW] Mặc định ngày đến hạn
            if (dtpNgayDenHan != null)
                dtpNgayDenHan.Value = DateTime.Today;

            GenerateReceiptCode();
            SetupGrid();
            LoadOrderOptions();

            if (_orderId > 0)
                LoadDataFromOrder(_orderId);
        }

        // ─────────────────────────────────────────────────────
        // SINH MÃ PHIẾU NHẬP
        // ─────────────────────────────────────────────────────

        private void GenerateReceiptCode()
        {
            txtReceiptCode.Text = _repo.GenerateReceiptCode();
            txtReceiptCode.ReadOnly = true;
        }

        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        private void LoadOrderOptions()
        {
            cboOrderCode.SelectedIndexChanged -= cboOrderCode_SelectedIndexChanged;
            cboOrderCode.Items.Clear();
            cboOrderCode.Items.Add(new OrderItem());

            try
            {
                var dt = _repo.GetPendingOrders();
                foreach (DataRow row in dt.Rows)
                {
                    cboOrderCode.Items.Add(new OrderItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        OrderCode = row["Ma_Don_Hang"].ToString(),
                        SupplierName = row["Ten_NCC"].ToString(),
                        SupplierId = Convert.ToInt32(row["SupplierId"]),
                        SupplierAddress = row["SupplierAddress"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }

            cboOrderCode.SelectedIndexChanged += cboOrderCode_SelectedIndexChanged;

            if (_orderId > 0)
                foreach (OrderItem item in cboOrderCode.Items)
                    if (item.Id == _orderId) { cboOrderCode.SelectedItem = item; return; }

            cboOrderCode.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────
        // CHỌN ĐƠN HÀNG → TỰ ĐIỀN DỮ LIỆU
        // ─────────────────────────────────────────────────────

        private void cboOrderCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOrderCode.SelectedItem is OrderItem order && order.Id > 0)
                LoadDataFromOrder(order.Id);
        }

        private void LoadDataFromOrder(int orderId)
        {
            try
            {
                var ds = _repo.GetOrderReceiveData(orderId);
                if (ds.Tables.Count == 0) return;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    _orderId = orderId;
                    _supplierId = Convert.ToInt32(row["SupplierId"]);
                    txtSupplierName.Text = row["Ten_NCC"].ToString();
                    txtSupplierAddress.Text = row["Dia_Chi"].ToString();
                    txtSupplierCode.Text = row["Ma_NCC"].ToString();

                    string paymentMethod = row["Dieu_Khoan_Thanh_Toan"].ToString();
                    for (int i = 0; i < cboPaymentMethod.Items.Count; i++)
                        if (cboPaymentMethod.Items[i].ToString() == paymentMethod)
                        { cboPaymentMethod.SelectedIndex = i; break; }

                    // [NEW] Auto-fill số ngày nợ từ đơn hàng
                    if (nudSoNgayNoo != null && row.Table.Columns.Contains("So_Ngay_No"))
                    {
                        int soNgayNo = Convert.ToInt32(row["So_Ngay_No"]);
                        nudSoNgayNoo.Value = soNgayNo;
                        if (dtpNgayDenHan != null)
                            dtpNgayDenHan.Value = dtpDocumentDate.Value.AddDays(soNgayNo);
                    }
                }

                if (ds.Tables.Count > 1)
                {
                    dgvReceiveDetails.Rows.Clear();
                    int rowIndex = 1;
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        double orderedQty = Convert.ToDouble(row["OrderQty"]);
                        double daNhan = row.Table.Columns.Contains("So_Luong_Da_Nhan") && row["So_Luong_Da_Nhan"] != DBNull.Value
                            ? Convert.ToDouble(row["So_Luong_Da_Nhan"])
                            : 0;
                        double remaining = Math.Max(0, orderedQty - daNhan);
                        double unitPrice = Convert.ToDouble(row["Don_Gia"]);
                        double vatRate = Convert.ToDouble(row["Phan_Tram_Thue_GTGT"]);
                        // Thành tiền / VAT theo SL còn nhập (tránh nhập trùng khi đã có So_Luong_Da_Nhan)
                        double lineTotal = remaining * unitPrice;
                        double vatAmount = Math.Round(lineTotal * vatRate / 100.0, 0, MidpointRounding.AwayFromZero);

                        dgvReceiveDetails.Rows.Add(
                            rowIndex,
                            row["Ma_Nguyen_Lieu"].ToString(),
                            row["Ten_Nguyen_Lieu"].ToString(),
                            row["Don_Vi_Tinh"].ToString(),
                            orderedQty.ToString("N0"),
                            remaining.ToString("N0"),
                            unitPrice.ToString("N0"),
                            lineTotal.ToString("N0"),
                            vatRate,
                            vatAmount.ToString("N0"),
                            Convert.ToInt32(row["MaterialId"]),
                            Convert.ToInt32(row["id"]));

                        rowIndex++;
                    }
                }

                UpdateTotals();

                foreach (OrderItem item in cboOrderCode.Items)
                    if (item.Id == orderId) { cboOrderCode.SelectedItem = item; break; }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // SETUP DATAGRIDVIEW (giữ nguyên như cũ)
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvReceiveDetails;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.ScrollBars = ScrollBars.Both;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);

            var styleRO = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 10f)
            };
            var styleEdit = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(255, 255, 220),
                ForeColor = Color.FromArgb(30, 80, 160),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            void AddCol(string name, string header, int w, bool readOnly = true,
                        DataGridViewCellStyle style = null,
                        DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
            {
                var s = style ?? new DataGridViewCellStyle(styleRO) { Alignment = align };
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    Width = w,
                    ReadOnly = readOnly,
                    DefaultCellStyle = s
                });
            }

            AddCol("colSTT", "STT", 50, align: DataGridViewContentAlignment.MiddleCenter);
            AddCol("colItemCode", "Mã hàng", 110, align: DataGridViewContentAlignment.MiddleCenter);
            AddCol("colItemName", "Tên hàng", 250);
            AddCol("colUnit", "ĐVT", 70, align: DataGridViewContentAlignment.MiddleCenter);
            AddCol("colOrderQty", "SL đặt", 90, align: DataGridViewContentAlignment.MiddleRight);
            AddCol("colReceivedQty", "SL nhận", 90, false, styleEdit);
            AddCol("colUnitPrice", "Đơn giá", 130, align: DataGridViewContentAlignment.MiddleRight);
            AddCol("colLineTotal", "Thành tiền", 150, align: DataGridViewContentAlignment.MiddleRight);
            AddCol("colVatRate", "% VAT", 70, align: DataGridViewContentAlignment.MiddleCenter);
            AddCol("colVatAmount", "Tiền VAT", 120, align: DataGridViewContentAlignment.MiddleRight);
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaterialId", Width = 0, Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetailId", Width = 0, Visible = false });

            dgv.CellValueChanged += dgvReceiveDetails_CellValueChanged;
        }

        // ─────────────────────────────────────────────────────
        // TÍNH LẠI KHI SỬA SL NHẬN
        // ─────────────────────────────────────────────────────

        private void dgvReceiveDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvReceiveDetails.Columns[e.ColumnIndex].Name != "colReceivedQty") return;

            var row = dgvReceiveDetails.Rows[e.RowIndex];
            double.TryParse(row.Cells["colReceivedQty"].Value?.ToString().Replace(",", ""), out double receivedQty);
            double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double unitPrice);
            double.TryParse(row.Cells["colVatRate"].Value?.ToString(), out double vatRate);

            double lineTotal = receivedQty * unitPrice;
            double vatAmount = lineTotal * vatRate / 100.0;

            row.Cells["colLineTotal"].Value = lineTotal.ToString("N0");
            row.Cells["colVatAmount"].Value = vatAmount.ToString("N0");

            UpdateTotals();
        }

        // ─────────────────────────────────────────────────────
        // CẬP NHẬT TỔNG
        // ─────────────────────────────────────────────────────

        private void UpdateTotals()
        {
            double subTotal = 0, vatTotal = 0;
            foreach (DataGridViewRow row in dgvReceiveDetails.Rows)
            {
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lt);
                double.TryParse(row.Cells["colVatAmount"].Value?.ToString().Replace(",", ""), out double va);
                subTotal += lt;
                vatTotal += va;
            }
            lblSubTotal.Text = subTotal.ToString("N0") + " đ";
            lblVatAmount.Text = vatTotal.ToString("N0") + " đ";
            lblGrandTotal.Text = (subTotal + vatTotal).ToString("N0") + " đ";
        }

        // ─────────────────────────────────────────────────────
        // NÚT ĐÍNH KÈM HÓA ĐƠN
        // ─────────────────────────────────────────────────────

        private void btnAttachInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new OpenFileDialog
                {
                    Title = "Chọn file hóa đơn",
                    Filter = "Tất cả|*.*|PDF|*.pdf|Excel|*.xlsx;*.xls|Ảnh|*.jpg;*.png",
                    Multiselect = false
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _invoiceFile = dialog.FileName;
                    MessageBox.Show(
                        $"✅ Đã chọn:\n{System.IO.Path.GetFileName(_invoiceFile)}",
                        "Đính kèm hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        // ─────────────────────────────────────────────────────
        // NÚT LƯU PHIẾU (chưa ghi sổ)
        // [FIX] Truyền thêm SoNgayNo + NgayDenHanTT vào repository
        // ─────────────────────────────────────────────────────

        private void btnSaveReceipt_Click(object sender, EventArgs e)
        {
            if (_isPosted)
            { MessageBox.Show("⚠️ Phiếu đã ghi sổ!"); return; }

            if (_orderId == 0)
            { MessageBox.Show("⚠️ Vui lòng chọn đơn đặt hàng!"); return; }

            if (dgvReceiveDetails.Rows.Count == 0)
            { MessageBox.Show("⚠️ Không có hàng hóa nào!"); return; }

            try
            {
                bool hasInvoice = cboInvoiceStatus.SelectedItem?.ToString() == "Nhận kèm hóa đơn";

                int soNgayNo = (int)(nudSoNgayNoo?.Value ?? 0);
                DateTime? ngayDH = dtpNgayDenHan?.Value;

                // [FIX] Gọi repository với 2 tham số mới
                _receiptId = _repo.SaveReceipt(
                    _receiptId,
                    txtReceiptCode.Text.Trim(),
                    _orderId,
                    _supplierId,
                    dtpDocumentDate.Value.Date,
                    dtpAccountingDate.Value.Date,
                    CurrentUser.HoTen ?? CurrentUser.Username ?? "",
                    cboReceiveType.SelectedItem?.ToString() ?? "",
                    cboPaymentMethod.SelectedItem?.ToString() ?? "",
                    cboPaymentStatus.SelectedItem?.ToString() ?? "",
                    hasInvoice,
                    txtInvoiceForm.Text.Trim(),
                    txtInvoiceSymbol.Text.Trim(),
                    txtInvoiceNumber.Text.Trim(),
                    dtpInvoiceDate.Value.Date,
                    _invoiceFile ?? "",
                    soNgayNo,       // [NEW]
                    ngayDH);        // [NEW]

                _isSaved = true;
                MessageBox.Show(
                    $"✅ Đã lưu phiếu nhập!\nSố phiếu: {txtReceiptCode.Text}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        // ─────────────────────────────────────────────────────
        // NÚT GHI SỔ (giữ nguyên logic cũ)
        // ─────────────────────────────────────────────────────

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (_isPosted)
            { MessageBox.Show("Phiếu đã ghi sổ."); return; }

            if (!_isSaved || _receiptId == 0)
            { MessageBox.Show("Vui lòng LƯU PHIẾU trước khi ghi sổ."); return; }

            if (dgvReceiveDetails.Rows.Count == 0)
            { MessageBox.Show("Không có dòng hàng để ghi sổ."); return; }

            if (MessageBox.Show(
                "Ghi sổ sẽ cộng tồn kho, cập nhật công nợ và khóa phiếu.\nXác nhận?",
                "Xác nhận ghi sổ", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                _repo.PostReceipt(_receiptId, _supplierId, BuildPostDetailTable());
                _isPosted = true;
                MessageBox.Show("✅ Ghi sổ thành công!", "Thành công");
                RefreshOrderComboAfterPost();
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        /// <summary>
        /// Làm mới combo đơn hàng: đơn nhập đủ biến mất; nếu còn ở dòng trống thì xóa lưới/NCC.
        /// </summary>
        private void RefreshOrderComboAfterPost()
        {
            LoadOrderOptions();
            if (cboOrderCode.SelectedItem is OrderItem o && o.Id == 0)
            {
                _orderId = 0;
                _supplierId = 0;
                dgvReceiveDetails.Rows.Clear();
                txtSupplierCode.Clear();
                txtSupplierName.Clear();
                txtSupplierAddress.Clear();
                UpdateTotals();
            }
        }

        // ─────────────────────────────────────────────────────
        // BUILD POST DETAIL TABLE
        // ─────────────────────────────────────────────────────

        private DataTable BuildPostDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("MaterialId", typeof(int));
            dt.Columns.Add("DetailId", typeof(int));
            dt.Columns.Add("ReceivedQty", typeof(double));
            dt.Columns.Add("UnitPrice", typeof(double));
            dt.Columns.Add("VatAmount", typeof(double));
            dt.Columns.Add("LineTotal", typeof(double));

            foreach (DataGridViewRow row in dgvReceiveDetails.Rows)
            {
                int.TryParse(row.Cells["colMaterialId"].Value?.ToString(), out int matId);
                int.TryParse(row.Cells["colDetailId"].Value?.ToString(), out int detId);
                double.TryParse(row.Cells["colReceivedQty"].Value?.ToString().Replace(",", ""), out double rqty);
                double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double uprice);
                double.TryParse(row.Cells["colVatAmount"].Value?.ToString().Replace(",", ""), out double vat);
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double total);
                dt.Rows.Add(matId, detId, rqty, uprice, vat, total);
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // [FIX] NÚT IN PHIẾU — thêm hạch toán kế toán + ngày đến hạn
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvReceiveDetails.Rows.Count == 0)
            { MessageBox.Show("⚠️ Không có dữ liệu để in!"); return; }

            try
            {
                double subTotal = 0, vatTotal = 0;
                var printLines = new List<string[]>();

                foreach (DataGridViewRow row in dgvReceiveDetails.Rows)
                {
                    double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lt);
                    double.TryParse(row.Cells["colVatAmount"].Value?.ToString().Replace(",", ""), out double va);
                    subTotal += lt; vatTotal += va;

                    printLines.Add(new string[]
                    {
                        row.Cells["colSTT"].Value?.ToString()         ?? "",
                        row.Cells["colItemCode"].Value?.ToString()    ?? "",
                        row.Cells["colItemName"].Value?.ToString()    ?? "",
                        row.Cells["colUnit"].Value?.ToString()        ?? "",
                        row.Cells["colOrderQty"].Value?.ToString()    ?? "",
                        row.Cells["colReceivedQty"].Value?.ToString() ?? "",
                        row.Cells["colUnitPrice"].Value?.ToString()   ?? "",
                        row.Cells["colLineTotal"].Value?.ToString()   ?? "",
                        row.Cells["colVatRate"].Value?.ToString()     ?? ""
                    });
                }

                double grandTotal = subTotal + vatTotal;

                // Lấy thông tin tài khoản kế toán
                string tkNo = cboTKNo?.SelectedItem?.ToString() ?? "152 - Nguyên liệu, vật liệu";
                string tkCo = cboTKCo?.SelectedItem?.ToString() ?? "331 - Phải trả nhà cung cấp";
                int soNgay = (int)(nudSoNgayNoo?.Value ?? 0);
                string ngayDH = dtpNgayDenHan != null
                    ? dtpNgayDenHan.Value.ToString("dd/MM/yyyy")
                    : "--";
                string trangThaiTT = soNgay == 0 ? "Thanh toán ngay" : $"Nợ {soNgay} ngày (đến hạn: {ngayDH})";

                // ── Snapshot dữ liệu để dùng trong PrintPage ──
                string receiptCode = txtReceiptCode.Text;
                string docDate = dtpDocumentDate.Value.ToString("dd/MM/yyyy");
                string accountDate = dtpAccountingDate.Value.ToString("dd/MM/yyyy");
                string supplierName = txtSupplierName.Text;
                string supplierAddr = txtSupplierAddress.Text;
                string supplierCode = txtSupplierCode.Text;
                string payMethod = cboPaymentMethod.SelectedItem?.ToString() ?? "";
                string invoiceNo = txtInvoiceNumber.Text;
                string invoiceDate = dtpInvoiceDate.Value.ToString("dd/MM/yyyy");
                string receiveType = cboReceiveType.SelectedItem?.ToString() ?? "";
                string createdBy = CurrentUser.HoTen ?? CurrentUser.Username ?? "";

                var printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.DefaultPageSettings.Landscape = true;
                printDoc.DefaultPageSettings.Margins =
                    new System.Drawing.Printing.Margins(30, 30, 20, 20);

                printDoc.PrintPage += (s, ev) =>
                {
                    var g = ev.Graphics;

                    var fTitle = new Font("Times New Roman", 13f, FontStyle.Bold);
                    var fSubTitle = new Font("Times New Roman", 10f, FontStyle.Italic);
                    var fBold = new Font("Times New Roman", 10f, FontStyle.Bold);
                    var fNormal = new Font("Times New Roman", 10f);
                    var fSmall = new Font("Times New Roman", 9f);
                    var fTKBold = new Font("Times New Roman", 10f, FontStyle.Bold | FontStyle.Underline);

                    var bBlue = new SolidBrush(Color.FromArgb(20, 60, 160));
                    var bGray = new SolidBrush(Color.FromArgb(100, 100, 100));
                    var bRed = new SolidBrush(Color.FromArgb(180, 20, 20));

                    int x = 30, y = 20, pageW = 800;

                    // ── TIÊU ĐỀ ──────────────────────────────
                    g.DrawString("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", fSubTitle, bGray,
                        (pageW - (int)g.MeasureString("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", fSubTitle).Width) / 2, y);
                    y += 18;
                    g.DrawString("PHIẾU NHẬP KHO", fTitle, Brushes.Black,
                        (pageW - (int)g.MeasureString("PHIẾU NHẬP KHO", fTitle).Width) / 2, y);
                    y += 28;

                    // Số phiếu + ngày
                    string header2 = $"Số phiếu: {receiptCode}   |   Ngày CT: {docDate}   |   Ngày HT: {accountDate}   |   {receiveType}";
                    g.DrawString(header2, fSmall, bGray,
                        (pageW - (int)g.MeasureString(header2, fSmall).Width) / 2, y);
                    y += 22;
                    g.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), x, y, pageW - 30, y); y += 10;

                    // ── THÔNG TIN NCC ──────────────────────────
                    g.DrawString("NHÀ CUNG CẤP:", fBold, Brushes.Black, x, y);
                    g.DrawString($"{supplierCode} - {supplierName}", fNormal, Brushes.Black, x + 140, y); y += 20;
                    g.DrawString("Địa chỉ:", fBold, Brushes.Black, x, y);
                    g.DrawString(supplierAddr, fNormal, Brushes.Black, x + 140, y); y += 20;
                    g.DrawString("Phương thức TT:", fBold, Brushes.Black, x, y);
                    g.DrawString(payMethod, fNormal, Brushes.Black, x + 140, y);
                    if (!string.IsNullOrEmpty(invoiceNo))
                    {
                        g.DrawString($"Hóa đơn số: {invoiceNo}  ngày {invoiceDate}", fNormal, Brushes.Black, x + 350, y);
                    }
                    y += 20;

                    // [NEW] Điều khoản thanh toán / số ngày nợ
                    g.DrawString("Điều khoản TT:", fBold, Brushes.Black, x, y);
                    g.DrawString(trangThaiTT, fNormal, bRed, x + 140, y); y += 10;
                    g.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), x, y, pageW - 30, y); y += 10;

                    // ── BẢNG HÀNG HÓA ─────────────────────────
                    g.FillRectangle(new SolidBrush(Color.FromArgb(230, 235, 245)), x, y, pageW - 60, 26);
                    int[] xP = { 2, 35, 105, 320, 370, 420, 475, 590, 685 };
                    string[] hdrs = { "STT", "Mã hàng", "Tên hàng/Diễn giải", "ĐVT", "SL đặt", "SL nhận", "Đơn giá", "Thành tiền", "VAT%" };
                    for (int i = 0; i < hdrs.Length; i++)
                        g.DrawString(hdrs[i], fBold, Brushes.Black, x + xP[i], y + 5);
                    y += 28;

                    foreach (var line in printLines)
                    {
                        for (int i = 0; i < Math.Min(line.Length, xP.Length); i++)
                            g.DrawString(line[i], fSmall, Brushes.Black, x + xP[i], y + 2);
                        g.DrawLine(new Pen(Color.FromArgb(210, 210, 210)), x, y + 20, pageW - 30, y + 20);
                        y += 22;
                    }

                    // ── TỔNG KẾT ──────────────────────────────
                    y += 8;
                    g.DrawLine(new Pen(Color.FromArgb(150, 150, 150), 1f), x + 450, y, pageW - 30, y); y += 6;
                    g.DrawString($"Tổng tiền hàng:", fBold, Brushes.Black, x + 450, y);
                    g.DrawString($"{subTotal:N0} đ", fNormal, Brushes.Black, x + 620, y); y += 20;
                    g.DrawString($"Thuế GTGT (VAT):", fBold, Brushes.Black, x + 450, y);
                    g.DrawString($"{vatTotal:N0} đ", fNormal, Brushes.Black, x + 620, y); y += 20;
                    g.DrawString($"TỔNG CỘNG:", fBold, bBlue, x + 450, y);
                    g.DrawString($"{grandTotal:N0} đ", fTKBold, bBlue, x + 620, y); y += 30;

                    // [FIX] ── HẠCH TOÁN KẾ TOÁN ──────────────
                    g.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), x, y, pageW - 30, y); y += 10;
                    g.DrawString("ĐỊNH KHOẢN KẾ TOÁN:", fBold, bBlue, x, y); y += 22;

                    // Dòng Nợ
                    g.DrawString("Nợ", fBold, Brushes.Black, x + 10, y);
                    g.DrawString(tkNo, fTKBold, bBlue, x + 40, y);
                    g.DrawString($"{subTotal:N0} đ", fBold, Brushes.Black, x + 380, y); y += 20;

                    // Dòng Nợ 133 (VAT đầu vào) — nếu có VAT
                    if (vatTotal > 0)
                    {
                        g.DrawString("Nợ", fBold, Brushes.Black, x + 10, y);
                        g.DrawString("133 - Thuế GTGT được khấu trừ", fTKBold, bBlue, x + 40, y);
                        g.DrawString($"{vatTotal:N0} đ", fBold, Brushes.Black, x + 380, y); y += 20;
                    }

                    // Dòng Có
                    g.DrawString("Có", fBold, Brushes.Black, x + 10, y);
                    g.DrawString(tkCo, fTKBold, new SolidBrush(Color.FromArgb(160, 20, 20)), x + 40, y);
                    g.DrawString($"{grandTotal:N0} đ", fBold, Brushes.Black, x + 380, y); y += 30;

                    // ── KÝ TÊN ────────────────────────────────
                    g.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), x, y, pageW - 30, y); y += 15;
                    int col1 = x + 20, col2 = x + 230, col3 = x + 470, col4 = x + 640;
                    g.DrawString("Người lập phiếu", fBold, bGray, col1, y);
                    g.DrawString("Kế toán trưởng", fBold, bGray, col2, y);
                    g.DrawString("Thủ kho", fBold, bGray, col3, y);
                    g.DrawString("Giám đốc", fBold, bGray, col4, y);
                    y += 12;
                    g.DrawString("(Ký, ghi rõ họ tên)", fSmall, bGray, col1, y);
                    g.DrawString("(Ký, ghi rõ họ tên)", fSmall, bGray, col2, y);
                    g.DrawString("(Ký, ghi rõ họ tên)", fSmall, bGray, col3, y);
                    g.DrawString("(Ký, ghi rõ họ tên)", fSmall, bGray, col4, y);
                    y += 55;
                    g.DrawString(createdBy, fNormal, Brushes.Black, col1 + 10, y);

                    // Dispose
                    fTitle.Dispose(); fSubTitle.Dispose(); fBold.Dispose();
                    fNormal.Dispose(); fSmall.Dispose(); fTKBold.Dispose();
                    bBlue.Dispose(); bGray.Dispose(); bRed.Dispose();
                };

                new PrintPreviewDialog
                {
                    Document = printDoc,
                    WindowState = FormWindowState.Maximized,
                    UseAntiAlias = true
                }.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT LÀM MỚI
        // ─────────────────────────────────────────────────────

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!_isSaved && (_orderId > 0 || dgvReceiveDetails.Rows.Count > 0))
                if (MessageBox.Show("Dữ liệu chưa lưu sẽ mất. Tiếp tục?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    != DialogResult.Yes) return;

            _orderId = _supplierId = _receiptId = 0;
            _isPosted = _isSaved = false;
            _invoiceFile = "";

            cboOrderCode.SelectedIndex = 0;
            txtSupplierCode.Clear();
            txtSupplierName.Clear();
            txtSupplierAddress.Clear();
            cboReceiveType.SelectedIndex = 0;
            cboPaymentStatus.SelectedIndex = 0;
            cboPaymentMethod.SelectedIndex = 0;
            cboInvoiceStatus.SelectedIndex = 0;
            dtpDocumentDate.Value = DateTime.Today;
            dtpAccountingDate.Value = DateTime.Today;
            dtpInvoiceDate.Value = DateTime.Today;
            txtInvoiceForm.Text = "01GTKT";
            txtInvoiceSymbol.Text = "";
            txtInvoiceNumber.Text = "";

            if (nudSoNgayNoo != null) nudSoNgayNoo.Value = 0;
            if (dtpNgayDenHan != null) dtpNgayDenHan.Value = DateTime.Today;
            if (cboTKNo != null) cboTKNo.SelectedIndex = 0;
            if (cboTKCo != null) cboTKCo.SelectedIndex = 0;

            dgvReceiveDetails.Rows.Clear();
            lblSubTotal.Text = "0 đ";
            lblVatAmount.Text = "0 đ";
            lblGrandTotal.Text = "0 đ";

            GenerateReceiptCode();
        }

        // ─────────────────────────────────────────────────────
        // INNER CLASS
        // ─────────────────────────────────────────────────────

        private class OrderItem
        {
            public int Id { get; set; }
            public string OrderCode { get; set; } = "";
            public string SupplierName { get; set; } = "-- Chọn đơn hàng --";
            public int SupplierId { get; set; }
            public string SupplierAddress { get; set; } = "";

            public override string ToString()
                => string.IsNullOrEmpty(OrderCode) ? SupplierName : OrderCode;
        }
    }
}