// ═══════════════════════════════════════════════════════════════════
// ║  frmWarehouseExport.cs - XUẤT KHO SẢN XUẤT                    ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Form nhập liệu phiếu xuất kho vật tư sản xuất.
// Toàn bộ DB ủy thác cho WarehouseExportRepository.
// Trigger phía DB tự động trừ tồn kho — form không cần xử lý.
// ═══════════════════════════════════════════════════════════════════

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmWarehouseExport : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly WarehouseExportRepository _repo = new();

        // ID lệnh sản xuất đang chọn (0 = chưa chọn)
        private int _productionOrderId = 0;

        // ID phiếu xuất sau khi lưu thành công (0 = chưa lưu)
        private int _exportReceiptId = 0;

        // Trạng thái đã xác nhận chưa
        private bool _isConfirmed = false;

        // Cache nguyên liệu — tránh query lại nhiều lần
        private DataTable _dtMaterials = new DataTable();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmWarehouseExport()
        {
            InitializeComponent();
            this.Load += frmWarehouseExport_Load;
        }



        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmWarehouseExport_Load(object sender, EventArgs e)
        {
            LoadMaterialCache();
            SetupGrid();
            SetupStaticCombos();
            LoadProductionOrders();

            txtExportCode.Text = _repo.GenerateExportCode();
            dtpExportDate.Value = DateTime.Today;
            txtReason.Text = "Xuất vật tư phục vụ sản xuất";

            UpdateTotalAmount();
            btnConfirm.Visible = false;
        }


        // ─────────────────────────────────────────────────────
        // NẠP CACHE NGUYÊN LIỆU
        // ─────────────────────────────────────────────────────

        private void LoadMaterialCache()
        {
            try
            {
                _dtMaterials = _repo.GetMaterials();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvMaterials;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false; dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false; dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false; dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White; dgv.RowTemplate.Height = 38;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.EnableHeadersVisualStyles = false; dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            var styleRight = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10f), Padding = new Padding(0, 0, 8, 0) };
            var styleCenter = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10f) };
            var styleReadOnly = new DataGridViewCellStyle { BackColor = Color.FromArgb(245, 247, 250), ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 10f) };
            var styleEditable = new DataGridViewCellStyle(styleRight) { BackColor = Color.FromArgb(255, 251, 220) };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", FillWeight = 5, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(styleCenter) { BackColor = Color.FromArgb(248, 249, 250) } });
            dgv.Columns.Add(new DataGridViewComboBoxColumn { Name = "colMaterialCode", HeaderText = "Mã hàng", FillWeight = 12, DisplayMember = "Ma_Nguyen_Lieu", ValueMember = "Ma_Nguyen_Lieu", DataSource = _dtMaterials, FlatStyle = FlatStyle.Flat });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaterialName", HeaderText = "Tên hàng", FillWeight = 25, ReadOnly = true, DefaultCellStyle = styleReadOnly });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "ĐVT", FillWeight = 8, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly) { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockQty", HeaderText = "Tồn kho", FillWeight = 10, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly) { Alignment = DataGridViewContentAlignment.MiddleRight, Padding = new Padding(0, 0, 8, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExportQty", HeaderText = "SL xuất", FillWeight = 10, DefaultCellStyle = styleEditable });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnitPrice", HeaderText = "Đơn giá", FillWeight = 13, DefaultCellStyle = styleEditable });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLineTotal", HeaderText = "Thành tiền", FillWeight = 14, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(styleRight) { Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235) } });

            // Cột ẩn lưu giá trị raw để tính toán và lưu DB
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaterialId", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnitPriceRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExportQtyRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockQtyRaw", Visible = false });

            dgv.CellValueChanged += dgvMaterials_CellValueChanged;
            dgv.EditingControlShowing += dgvMaterials_EditingControlShowing;
            dgv.CellClick += dgvMaterials_CellClick;
            dgv.CurrentCellDirtyStateChanged += dgvMaterials_CurrentCellDirtyStateChanged;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT LẬP COMBOBOX TĨNH
        // ─────────────────────────────────────────────────────

        private void SetupStaticCombos()
        {
            cboExportType.Items.Clear();
            cboDepartment.Items.Clear();
            cboExportType.Items.AddRange(new object[] { "Xuất cho sản xuất", "Xuất bán hàng", "Xuất khác" });
            cboExportType.SelectedIndex = 0;
            cboDepartment.Items.AddRange(new object[] { "Sản xuất", "In ấn", "Gia công", "Đóng gói" });
            cboDepartment.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH LỆNH SẢN XUẤT
        // ─────────────────────────────────────────────────────

        private void LoadProductionOrders()
        {
            cboProductionOrder.SelectedIndexChanged -= cboProductionOrder_SelectedIndexChanged;
            cboProductionOrder.Items.Clear();
            cboProductionOrder.Items.Add(new ProductionOrderItem());

            try
            {
                DataTable dt = _repo.GetActiveProductionOrders();
                foreach (DataRow row in dt.Rows)
                {
                    cboProductionOrder.Items.Add(new ProductionOrderItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["Ma_Lenh_SX"].ToString(),
                        ProductName = row["Ten_San_Pham"].ToString(),
                        Quantity = Convert.ToInt32(row["So_Luong"]),
                        CustomerName = row["Ten_Khach_Hang"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }

            cboProductionOrder.SelectedIndexChanged += cboProductionOrder_SelectedIndexChanged;
            cboProductionOrder.SelectedIndex = 0;
        }

        private void cboProductionOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboProductionOrder.SelectedItem is ProductionOrderItem item) || item.Id == 0)
            {
                txtCustomer.Text = ""; txtProduct.Text = ""; txtQuantity.Text = "";
                _productionOrderId = 0;
                dgvMaterials.Rows.Clear();
                UpdateTotalAmount();
                return;
            }
            _productionOrderId = item.Id;
            txtCustomer.Text = item.CustomerName;
            txtProduct.Text = item.ProductName;
            txtQuantity.Text = item.Quantity.ToString("N0") + " cái";

            // Khi chọn LSX: tự nạp tất cả vật tư đang có trong kho để nhập SL xuất ngay
            LoadMaterialsForSelectedOrder();
        }

        private void LoadMaterialsForSelectedOrder()
        {
            dgvMaterials.Rows.Clear();

            if (_dtMaterials == null || _dtMaterials.Rows.Count == 0)
            {
                UpdateTotalAmount();
                return;
            }

            int stt = 1;
            foreach (DataRow material in _dtMaterials.Rows)
            {
                int materialId = Convert.ToInt32(material["id"]);
                string code = material["Ma_Nguyen_Lieu"]?.ToString() ?? "";
                string name = material["Ten_Nguyen_Lieu"]?.ToString() ?? "";
                string unit = material["Don_Vi_Tinh"]?.ToString() ?? "";
                double stockQty = Convert.ToDouble(material["Ton_Kho"]);
                double unitPrice = Convert.ToDouble(material["Gia_Nhap"]);

                int rowIdx = dgvMaterials.Rows.Add(
                    stt++,
                    code,
                    name,
                    unit,
                    stockQty.ToString("N2"),
                    "0",
                    unitPrice.ToString("N0"),
                    "",
                    materialId,
                    unitPrice,
                    0d,
                    stockQty
                );

                dgvMaterials.Rows[rowIdx].Cells["colStockQty"].Style.ForeColor =
                    stockQty <= 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(22, 163, 74);
            }

            UpdateTotalAmount();
        }


        // ─────────────────────────────────────────────────────
        // THÊM DÒNG VẬT TƯ
        // ─────────────────────────────────────────────────────

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            int rowNumber = dgvMaterials.Rows.Count + 1;
            dgvMaterials.Rows.Add(rowNumber, "", "", "", "0", "0", "0", "0", 0, 0, 0, 0);
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN GRID
        // ─────────────────────────────────────────────────────

        private void dgvMaterials_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvMaterials.CurrentCell?.OwningColumn.Name is "colExportQty" or "colUnitPrice")
                if (e.Control is TextBox tb) tb.SelectAll();
        }

        private void dgvMaterials_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvMaterials.CurrentCell?.OwningColumn.Name == "colMaterialCode")
                dgvMaterials.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvMaterials_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvMaterials.Columns[e.ColumnIndex].Name != "colMaterialCode") return;
            dgvMaterials.BeginEdit(true);
            if (dgvMaterials.EditingControl is ComboBox cbo) cbo.DroppedDown = true;
        }

        private void dgvMaterials_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvMaterials.Rows[e.RowIndex];
            string colName = dgvMaterials.Columns[e.ColumnIndex].Name;

            if (colName == "colMaterialCode")
            {
                string code = row.Cells["colMaterialCode"].Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(code)) return;

                var matched = _dtMaterials.Select($"Ma_Nguyen_Lieu = '{code}'");
                if (matched.Length == 0) return;

                var material = matched[0];
                double stockQty = Convert.ToDouble(material["Ton_Kho"]);
                double unitPrice = Convert.ToDouble(material["Gia_Nhap"]);
                int materialId = Convert.ToInt32(material["id"]);

                row.Cells["colMaterialName"].Value = material["Ten_Nguyen_Lieu"].ToString();
                row.Cells["colUnit"].Value = material["Don_Vi_Tinh"].ToString();
                row.Cells["colStockQty"].Value = stockQty.ToString("N2");
                row.Cells["colUnitPrice"].Value = unitPrice.ToString("N0");
                row.Cells["colMaterialId"].Value = materialId;
                row.Cells["colUnitPriceRaw"].Value = unitPrice;
                row.Cells["colStockQtyRaw"].Value = stockQty;
                row.Cells["colStockQty"].Style.ForeColor =
                    stockQty <= 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(22, 163, 74);
            }

            if (colName == "colExportQty" || colName == "colUnitPrice")
            {
                CalculateLineTotal(row);
                UpdateTotalAmount();
            }
        }


        // ─────────────────────────────────────────────────────
        // TÍNH THÀNH TIỀN TỪNG DÒNG
        // ─────────────────────────────────────────────────────

        private void CalculateLineTotal(DataGridViewRow row)
        {
            string qtyStr = row.Cells["colExportQty"].Value?.ToString() ?? "0";
            double.TryParse(qtyStr.Replace(",", "").Replace(".", ""), out double qty);
            double.TryParse(row.Cells["colUnitPriceRaw"].Value?.ToString(), out double unitPrice);
            double lineTotal = qty * unitPrice;
            row.Cells["colExportQtyRaw"].Value = qty;
            row.Cells["colLineTotal"].Value = lineTotal > 0 ? lineTotal.ToString("N0") : "";
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT TỔNG GIÁ TRỊ XUẤT
        // ─────────────────────────────────────────────────────

        private void UpdateTotalAmount()
        {
            double total = 0;
            foreach (DataGridViewRow row in dgvMaterials.Rows)
            {
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                total += lineTotal;
            }
            lblTotalValue.Text = $"TỔNG GIÁ TRỊ XUẤT:  {total:N0} đ";
        }


        // ─────────────────────────────────────────────────────
        // KIỂM TRA DỮ LIỆU TRƯỚC KHI LƯU
        // ─────────────────────────────────────────────────────

        private bool ValidateInput()
        {
            if (_productionOrderId == 0)
            { MessageBox.Show("⚠️ Vui lòng chọn số lệnh sản xuất!", "Thiếu thông tin"); return false; }

            if (string.IsNullOrWhiteSpace(txtReceiver.Text))
            { MessageBox.Show("⚠️ Vui lòng nhập tên người nhận!", "Thiếu thông tin"); txtReceiver.Focus(); return false; }

            if (dgvMaterials.Rows.Count == 0)
            { MessageBox.Show("⚠️ Vui lòng thêm ít nhất 1 vật tư!", "Thiếu thông tin"); return false; }

            int linesWithExport = 0;
            foreach (DataGridViewRow row in dgvMaterials.Rows)
            {
                double.TryParse(row.Cells["colExportQtyRaw"].Value?.ToString(), out double qty);
                if (qty <= 0)
                    continue;

                linesWithExport++;
                string code = row.Cells["colMaterialCode"].Value?.ToString()?.Trim() ?? "";
                if (string.IsNullOrEmpty(code))
                {
                    MessageBox.Show($"⚠️ Dòng {row.Index + 1}: Có số lượng xuất > 0 nhưng chưa chọn mã hàng!", "Thiếu thông tin");
                    return false;
                }

                int materialId = Convert.ToInt32(row.Cells["colMaterialId"].Value ?? 0);
                if (materialId <= 0)
                {
                    MessageBox.Show($"⚠️ Dòng {row.Index + 1}: Mã hàng không hợp lệ!", "Thiếu thông tin");
                    return false;
                }

                double.TryParse(row.Cells["colStockQtyRaw"].Value?.ToString(), out double stockQty);
                if (qty > stockQty)
                {
                    string name = row.Cells["colMaterialName"].Value?.ToString();
                    MessageBox.Show($"⚠️ Vật tư '{name}': Số lượng xuất ({qty:N2}) vượt quá tồn kho ({stockQty:N2})!", "Không đủ tồn kho");
                    return false;
                }
            }

            if (linesWithExport == 0)
            {
                MessageBox.Show(
                    "⚠️ Cần ít nhất một dòng có số lượng xuất > 0.\n\n" +
                    "Các dòng để 0 là không xuất vật tư đó (chỉ xuất những gì thực sự lấy từ kho).",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DỮ LIỆU GRID THÀNH DATATABLE GỬI VÀO REPOSITORY
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Đọc từng dòng grid, đóng gói vào DataTable
        /// để truyền vào WarehouseExportRepository.SaveExportReceipt().
        /// Repository sẽ chuyển DataTable này sang XML rồi gửi vào SP.
        /// </summary>
        private DataTable BuildDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("MaterialId", typeof(int));
            dt.Columns.Add("Qty", typeof(double));
            dt.Columns.Add("UnitPrice", typeof(double));
            dt.Columns.Add("LineTotal", typeof(double));

            foreach (DataGridViewRow row in dgvMaterials.Rows)
            {
                int materialId = Convert.ToInt32(row.Cells["colMaterialId"].Value ?? 0);
                double.TryParse(row.Cells["colExportQtyRaw"].Value?.ToString(), out double qty);
                if (qty <= 0 || materialId <= 0)
                    continue;
                double.TryParse(row.Cells["colUnitPriceRaw"].Value?.ToString(), out double unitPrice);
                dt.Rows.Add(materialId, qty, unitPrice, qty * unitPrice);
            }
            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU PHIẾU XUẤT KHO
        // ─────────────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvMaterials.EndEdit();
            foreach (DataGridViewRow row in dgvMaterials.Rows) CalculateLineTotal(row);
            if (!ValidateInput()) return;

            if (MessageBox.Show("Lưu phiếu xuất và trừ tồn kho ngay?\n\nThao tác này không thể hoàn tác!",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // Gọi repository — SP + Trigger phía DB xử lý transaction và trừ tồn
                _exportReceiptId = _repo.SaveExportReceipt(
                    txtExportCode.Text.Trim(),
                    _productionOrderId,
                    dtpExportDate.Value,
                    txtReceiver.Text.Trim(),
                    BuildDetailTable());

                MessageBox.Show($"✅ Lưu và xuất kho thành công!\nSố phiếu: {txtExportCode.Text}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnSave.Enabled = false; btnConfirm.Enabled = false;
                btnConfirm.Text = "✅ Đã xuất kho";

                // Làm mới cache tồn kho sau khi trigger trừ
                LoadMaterialCache();
                RefreshStockGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // XÁC NHẬN XUẤT KHO
        // ─────────────────────────────────────────────────────

        private void btnConfirm_Click(object sender, EventArgs e) { }


        // ─────────────────────────────────────────────────────
        // LÀM MỚI TỒN KHO TRÊN GRID
        // ─────────────────────────────────────────────────────

        private void RefreshStockGrid()
        {
            foreach (DataGridViewRow row in dgvMaterials.Rows)
            {
                int materialId = Convert.ToInt32(row.Cells["colMaterialId"].Value ?? 0);
                if (materialId == 0) continue;
                var matched = _dtMaterials.Select($"id = {materialId}");
                if (matched.Length > 0)
                    row.Cells["colStockQty"].Value = Convert.ToDouble(matched[0]["Ton_Kho"]).ToString("N2");
            }
        }


        // ─────────────────────────────────────────────────────
        // IN PHIẾU XUẤT KHO RA EXCEL
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvMaterials.Rows.Count == 0)
            { MessageBox.Show("⚠️ Không có dữ liệu để in!", "Thiếu dữ liệu"); return; }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("PhieuXuatKho");
                    int r = 1;

                    ws.Cells[r, 1].Value = "CÔNG TY TNHH SX THƯƠNG MẠI DỊCH VỤ AN LÂM";
                    ws.Cells[r, 1, r, 5].Merge = true; ws.Cells[r, 1].Style.Font.Bold = true; r++;
                    ws.Cells[r, 1].Value = "51/10/3 Hòa Bình, Phường Tân Phú, TP.HCM";
                    ws.Cells[r, 1, r, 5].Merge = true;
                    ws.Cells[1, 7].Value = "Mẫu số: 02 - VT"; ws.Cells[2, 7].Value = "(Theo TT 200/2014/TT-BTC)";
                    r += 2;

                    ws.Cells[r, 1].Value = "PHIẾU XUẤT KHO";
                    ws.Cells[r, 1, r, 8].Merge = true; ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; r++;

                    DateTime d = dtpExportDate.Value.Date;
                    ws.Cells[r, 1].Value = $"Ngày {d:dd} tháng {d:MM} năm {d:yyyy}";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true; r++;
                    ws.Cells[r, 5].Value = $"Số: {txtExportCode.Text}"; r++;
                    ws.Cells[r, 1].Value = "Họ và tên người nhận:"; ws.Cells[r, 2, r, 8].Merge = true; ws.Cells[r, 2].Value = txtReceiver.Text; r++;
                    ws.Cells[r, 1].Value = "Địa chỉ (bộ phận):"; ws.Cells[r, 2, r, 8].Merge = true; ws.Cells[r, 2].Value = cboDepartment.SelectedItem?.ToString(); r++;
                    ws.Cells[r, 1].Value = "Lý do xuất kho:"; ws.Cells[r, 2, r, 8].Merge = true; ws.Cells[r, 2].Value = txtReason.Text; r++;
                    ws.Cells[r, 1].Value = "Xuất tại kho:"; ws.Cells[r, 2, r, 8].Merge = true; ws.Cells[r, 2].Value = "Nguyên vật liệu giấy"; r += 2;

                    string[] headers = { "STT", "Tên vật tư", "Mã số", "ĐVT", "SL thực xuất", "Đơn giá", "Thành tiền" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cells[r, c + 1].Value = headers[c];
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 224, 180));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    r++;

                    int stt = 1; double totalAmt = 0;
                    foreach (DataGridViewRow gridRow in dgvMaterials.Rows)
                    {
                        double.TryParse(gridRow.Cells["colExportQtyRaw"].Value?.ToString(), out double qty);
                        if (qty <= 0)
                            continue;
                        double.TryParse(gridRow.Cells["colUnitPriceRaw"].Value?.ToString(), out double up);
                        double lt = qty * up; totalAmt += lt;
                        ws.Cells[r, 1].Value = stt++; ws.Cells[r, 2].Value = gridRow.Cells["colMaterialName"].Value?.ToString();
                        ws.Cells[r, 3].Value = gridRow.Cells["colMaterialCode"].Value?.ToString(); ws.Cells[r, 4].Value = gridRow.Cells["colUnit"].Value?.ToString();
                        ws.Cells[r, 5].Value = qty; ws.Cells[r, 6].Value = up; ws.Cells[r, 7].Value = lt;
                        ws.Cells[r, 5].Style.Numberformat.Format = "#,##0.###"; ws.Cells[r, 6].Style.Numberformat.Format = "#,##0"; ws.Cells[r, 7].Style.Numberformat.Format = "#,##0";
                        for (int c = 1; c <= 7; c++) ws.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        r++;
                    }

                    ws.Cells[r, 1].Value = "Tổng cộng"; ws.Cells[r, 1, r, 6].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true; ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[r, 7].Value = totalAmt; ws.Cells[r, 7].Style.Numberformat.Format = "#,##0"; ws.Cells[r, 7].Style.Font.Bold = true; r += 2;

                    int[] sigCol = { 1, 3, 5, 7 }; string[] signers = { "Người lập phiếu", "Người nhận hàng", "Thủ kho", "Kế toán trưởng" };
                    for (int i = 0; i < signers.Length; i++) { ws.Cells[r, sigCol[i]].Value = signers[i]; ws.Cells[r + 1, sigCol[i]].Value = "(Ký, ghi rõ họ tên)"; ws.Cells[r + 1, sigCol[i]].Style.Font.Italic = true; }
                    ws.Column(1).Width = 6; ws.Column(2).Width = 35; ws.Column(3).Width = 15; ws.Column(4).Width = 10; ws.Column(5).Width = 18; ws.Column(6).Width = 18; ws.Column(7).Width = 20;

                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"PHIEU_XUAT_KHO_{txtExportCode.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    pkg.SaveAs(new FileInfo(path));

                    MessageBox.Show($"✅ Đã xuất phiếu ra Excel:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true });
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi"); }
        }


        // ─────────────────────────────────────────────────────
        // HỦY / RESET FORM
        // ─────────────────────────────────────────────────────

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgvMaterials.Rows.Count > 0 || _productionOrderId > 0)
                if (MessageBox.Show("Bạn có chắc muốn hủy bỏ?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            ResetForm();
        }

        private void ResetForm()
        {
            _productionOrderId = 0; _exportReceiptId = 0; _isConfirmed = false;
            cboProductionOrder.SelectedIndex = 0;
            txtCustomer.Text = ""; txtProduct.Text = ""; txtQuantity.Text = ""; txtReceiver.Text = "";
            txtReason.Text = "Xuất vật tư phục vụ sản xuất";
            cboExportType.SelectedIndex = 0; cboDepartment.SelectedIndex = 0;
            dtpExportDate.Value = DateTime.Today;
            txtExportCode.Text = _repo.GenerateExportCode();
            dgvMaterials.Rows.Clear();
            lblTotalValue.Text = "TỔNG GIÁ TRỊ XUẤT:  0 đ";
            btnSave.Enabled = true; btnConfirm.Enabled = false; btnConfirm.Text = "✅ Xác nhận xuất kho";
            LoadMaterialCache();
            if (dgvMaterials.Columns["colMaterialCode"] is DataGridViewComboBoxColumn col) col.DataSource = _dtMaterials;
        }


        // ─────────────────────────────────────────────────────
        // INNER CLASS — LỆNH SẢN XUẤT TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class ProductionOrderItem
        {
            public int Id { get; set; }
            public string Code { get; set; } = "";
            public string ProductName { get; set; } = "";
            public string CustomerName { get; set; } = "";
            public int Quantity { get; set; }

            public override string ToString()
            {
                if (Id == 0) return "-- Chọn lệnh sản xuất --";
                return $"{Code} – {ProductName} ({Quantity:N0} cái)";
            }
        }
    }
}