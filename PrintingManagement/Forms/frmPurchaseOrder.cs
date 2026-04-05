// ═══════════════════════════════════════════════════════════════════
// ║  frmPurchaseOrder.cs - TẠO ĐƠN MUA HÀNG                       ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tạo và lưu đơn mua hàng từ nhà cung cấp.
// Hỗ trợ preload vật tư sắp hết từ frmInventoryIssue.
// Toàn bộ DB ủy thác cho PurchaseOrderRepository.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPurchaseOrder : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly PurchaseOrderRepository _repo = new();

        // Trạng thái form
        private int _supplierId = 0;
        private int _savedOrderId = 0;
        private string _attachedFile = "";
        private bool _isSaved = false;

        // Preload vật tư từ frmInventoryIssue
        private int _preloadMaterialId = 0;
        private string _preloadMaterialCode = "";
        private string _preloadMaterialName = "";

        // Dữ liệu in — lưu vào field để PrintPageHandler dùng (tránh capture lambda mỗi lần)
        private System.Drawing.Printing.PrintDocument? _currentPrintDoc;
        private string _printOrderCode = "";
        private string _printSupplierName = "";
        private string _printTaxCode = "";
        private string _printAddress = "";
        private string _printOrderDate = "";
        private List<string[]> _printPurchaseLines = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmPurchaseOrder()
        {
            InitializeComponent();
            // Load chỉ trong Designer — tránh chạy Load 2 lần.
        }

        /// <summary>
        /// Constructor gọi từ frmInventoryIssue khi muốn tạo đơn
        /// mua cho vật tư sắp hết — tự điền dòng vào grid.
        /// </summary>
        public frmPurchaseOrder(int materialId, string materialCode, string materialName) : this()
        {
            _preloadMaterialId = materialId;
            _preloadMaterialCode = materialCode;
            _preloadMaterialName = materialName;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmPurchaseOrder_Load(object sender, EventArgs e)
        {
            // Nạp tùy chọn ComboBox
            cboOrderStatus.Items.Clear();
            cboOrderStatus.Items.AddRange(new string[]
                { "Chưa thực hiện", "Đang thực hiện", "Hoàn thành", "Hủy" });
            cboOrderStatus.SelectedIndex = 0;

            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange(new string[]
                { "Tiền mặt", "Ủy nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPaymentMethod.SelectedIndex = 0;

            dtpOrderDate.Value = DateTime.Today;
            dtpDeliveryDate.Value = DateTime.Today;

            SetupGrid();
            LoadSuppliers();
            GenerateOrderCode();
            UpdateTotals();

            // Preload vật tư sắp hết nếu mở từ frmInventoryIssue
            if (_preloadMaterialId > 0)
                PreloadMaterial();

            ApplyPurchaseOrderScrollExtent();
        }

        /// <summary>Đảm bảo form cuộn xuống tới hàng nút Lưu / Nhập kho trên màn nhỏ hoặc DPI cao.</summary>
        private void ApplyPurchaseOrderScrollExtent()
        {
            int maxBottom = 0;
            foreach (Control c in Controls)
                maxBottom = Math.Max(maxBottom, c.Bottom);
            AutoScroll = true;
            AutoScrollMargin = new Size(24, 48);
            AutoScrollMinSize = new Size(Math.Max(ClientSize.Width, 1075), maxBottom + 48);
        }


        // ─────────────────────────────────────────────────────
        // PRELOAD VẬT TƯ TỪ FRINVENTORYISSUE
        // ─────────────────────────────────────────────────────

        private void PreloadMaterial()
        {
            try
            {
                // Toàn bộ SQL nằm trong PurchaseOrderRepository
                var dt = _repo.GetMaterialById(_preloadMaterialId);

                if (dt.Rows.Count == 0) return;

                var row = dt.Rows[0];
                string code = row["Ma_Nguyen_Lieu"].ToString();
                string name = row["Ten_Nguyen_Lieu"].ToString();
                string unit = row["Don_Vi_Tinh"].ToString();
                double price = Convert.ToDouble(row["Gia_Nhap"]);

                dgvOrderDetails.Rows.Add(
                    "", code, name, unit, "0",
                    price.ToString("N0"), "0", 10, _preloadMaterialId, "✕");

                UpdateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // SINH MÃ ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        private void GenerateOrderCode()
        {
            // Toàn bộ SQL nằm trong PurchaseOrderRepository
            txtOrderCode.Text = _repo.GenerateOrderCode();
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH NHÀ CUNG CẤP
        // ─────────────────────────────────────────────────────

        private void LoadSuppliers()
        {
            int prevId = _supplierId;

            // Hủy event tạm để tránh trigger khi load
            cmbSupplier.SelectedIndexChanged -= cmbSupplier_SelectedIndexChanged;
            cmbSupplier.Items.Clear();
            cmbSupplier.Items.Add(new SupplierItem()); // dòng trống

            try
            {
                // GetSuppliers giờ gắn kèm LatestOrderId + thông tin đơn cuối
                var dt = _repo.GetSuppliers();

                foreach (DataRow row in dt.Rows)
                {
                    cmbSupplier.Items.Add(new SupplierItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["Ma_NCC"].ToString(),
                        Name = row["Ten_NCC"].ToString(),
                        Address = row["Dia_Chi"].ToString(),
                        TaxCode = row["MST"].ToString(),
                        Phone = row["Dien_Thoai"].ToString(),
                        LatestOrderId = Convert.ToInt32(row["LatestOrderId"]),
                        LatestOrderCode = row["LatestOrderCode"]?.ToString() ?? "",
                        LatestOrderDate = row["LatestOrderDate"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(row["LatestOrderDate"]),
                        LatestOrderStatus = row["LatestOrderStatus"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }

            // Đăng ký lại event
            cmbSupplier.SelectedIndexChanged += cmbSupplier_SelectedIndexChanged;

            // Chọn lại NCC đang dùng nếu có
            if (prevId > 0)
            {
                foreach (SupplierItem item in cmbSupplier.Items)
                    if (item.Id == prevId) { cmbSupplier.SelectedItem = item; return; }
            }

            cmbSupplier.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // CHỌN NCC → TỰ ĐIỀN THÔNG TIN
        // ─────────────────────────────────────────────────────

        private void cmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            int prevId = _supplierId;

            if (cmbSupplier.SelectedItem is not SupplierItem supplier)
            {
                _supplierId = 0;
                txtSupplierName.Clear();
                txtTaxCode.Clear();
                txtSupplierAddress.Clear();
                if (prevId != 0)
                    ResetOrderDraftForNewEntry();
                return;
            }

            _supplierId = supplier.Id;
            txtSupplierName.Text = supplier.Name;
            txtTaxCode.Text = supplier.TaxCode;
            txtSupplierAddress.Text = supplier.Address;

            if (supplier.Id == 0)
            {
                // Dòng trống → form mới
                if (prevId != 0)
                    ResetOrderDraftForNewEntry();
            }
            else if (prevId != 0 && supplier.Id != prevId)
            {
                // Đổi sang NCC khác → form mới
                ResetOrderDraftForNewEntry();
            }
            else if (prevId == 0 && supplier.LatestOrderId > 0)
            {
                // Chọn NCC đã lưu đơn → load chi tiết đơn cuối vào lưới
                LoadOrderDetailsIntoGrid(supplier.LatestOrderId);
            }
            else if (prevId == 0 && supplier.LatestOrderId == 0)
            {
                // Chọn NCC mới chưa có đơn → form mới (giữ preload từ tồn kho)
                ResetOrderDraftForNewEntry();
            }
        }

        /// <summary>
        /// Load chi tiết đơn đã lưu vào lưới và điền thông tin đơn.
        /// </summary>
        private void LoadOrderDetailsIntoGrid(int orderId)
        {
            try
            {
                if (_repo.PurchaseOrderHasReceipt(orderId))
                {
                    MessageBox.Show(
                        "Đơn mới nhất của nhà cung cấp này đã có phiếu nhập kho — không mở để sửa.\nHệ thống chuyển sang tạo đơn mua mới.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetOrderDraftForNewEntry();
                    return;
                }

                var ds = _repo.GetPurchaseOrderById(orderId);
                if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                {
                    ResetOrderDraftForNewEntry();
                    return;
                }

                DataRow h = ds.Tables[0].Rows[0];
                _savedOrderId = orderId;
                _isSaved = true;
                _attachedFile = h["File_Dinh_Kem"]?.ToString() ?? "";

                txtOrderCode.Text = h["Ma_Don_Hang"]?.ToString() ?? "";
                dtpOrderDate.Value = h["Ngay_Dat_Hang"] == DBNull.Value
                    ? DateTime.Today
                    : Convert.ToDateTime(h["Ngay_Dat_Hang"]);
                dtpDeliveryDate.Value = h["Ngay_Giao_Hang"] == DBNull.Value
                    ? DateTime.Today
                    : Convert.ToDateTime(h["Ngay_Giao_Hang"]);

                // Chọn đúng item trong combo thanh toán / trạng thái
                SelectComboByText(cboPaymentMethod, h["Dieu_Khoan_Thanh_Toan"]?.ToString());
                SelectComboByText(cboOrderStatus, h["Trang_Thai"]?.ToString());

                txtDebtDays.Text = h["So_Ngay_No"] == DBNull.Value
                    ? "" : Convert.ToInt32(h["So_Ngay_No"]).ToString();
                txtDeliveryLocation.Text = h["Dia_Diem_Giao_Hang"]?.ToString() ?? "";
                // Ghi chú: chỉ khi SP/DB có cột Ghi_Chu (tránh lỗi "Column does not belong to table")
                txtNote.Text = h.Table.Columns.Contains("Ghi_Chu") && h["Ghi_Chu"] != DBNull.Value
                    ? h["Ghi_Chu"]?.ToString() ?? ""
                    : "";

                // Load chi tiết vào lưới
                dgvOrderDetails.Rows.Clear();
                int stt = 1;
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    decimal qty = Convert.ToDecimal(r["So_Luong"]);
                    decimal price = Convert.ToDecimal(r["Don_Gia"]);
                    decimal line = Convert.ToDecimal(r["Thanh_Tien"]);
                    decimal vat = r["Phan_Tram_Thue_GTGT"] == DBNull.Value
                        ? 10m : Convert.ToDecimal(r["Phan_Tram_Thue_GTGT"]);
                    int mid = Convert.ToInt32(r["MaterialId"]);

                    dgvOrderDetails.Rows.Add(
                        stt,
                        r["Ma_Nguyen_Lieu"]?.ToString() ?? "",
                        r["Ten_Nguyen_Lieu"]?.ToString() ?? "",
                        r["Don_Vi_Tinh"]?.ToString() ?? "",
                        qty.ToString("N0"),
                        price.ToString("N0"),
                        line.ToString("N0"),
                        vat.ToString("0.##"),
                        mid,
                        "✕");
                    stt++;
                }

                if (dgvOrderDetails.Rows.Count == 0)
                    AddNewRow();

                UpdateTotals();
                ApplyPurchaseOrderScrollExtent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi khi tải đơn cũ");
                ResetOrderDraftForNewEntry();
            }
        }

        private static void SelectComboByText(ComboBox cbo, string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            for (int i = 0; i < cbo.Items.Count; i++)
            {
                if (string.Equals(cbo.Items[i]?.ToString(), text, StringComparison.Ordinal))
                { cbo.SelectedIndex = i; return; }
            }
        }

        /// <summary>
        /// Đơn nhập mới cho NCC đang chọn: mã đơn mới, grid trống, chưa gắn bản ghi DB (Lưu = INSERT).
        /// </summary>
        private void ResetOrderDraftForNewEntry()
        {
            _savedOrderId = 0;
            _isSaved = false;
            _attachedFile = "";
            dtpOrderDate.Value = DateTime.Today;
            dtpDeliveryDate.Value = DateTime.Today;
            cboOrderStatus.SelectedIndex = 0;
            cboPaymentMethod.SelectedIndex = 0;
            txtDebtDays.Clear();
            txtDeliveryLocation.Clear();
            txtNote.Clear();

            dgvOrderDetails.Rows.Clear();
            AddNewRow();
            UpdateTotals();
            GenerateOrderCode();
        }


        // ─────────────────────────────────────────────────────
        // RỜI Ô TÊN NCC → KIỂM TRA / TẠO MỚI
        // ─────────────────────────────────────────────────────

        private void txtSupplierName_Leave(object sender, EventArgs e)
        {
            string supplierName = txtSupplierName.Text.Trim();
            if (string.IsNullOrEmpty(supplierName) || _supplierId > 0) return;

            try
            {
                // Toàn bộ SQL nằm trong PurchaseOrderRepository
                var (id, code, isNew) = _repo.CheckOrCreateSupplier(
                    supplierName,
                    txtSupplierAddress.Text.Trim(),
                    txtTaxCode.Text.Trim());

                _supplierId = id;

                if (!isNew)
                {
                    MessageBox.Show($"Nhà cung cấp đã có trong hệ thống.\nMã: {code}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSuppliers();
                    return;
                }

                // Hỏi xác nhận trước khi tạo mới
                if (MessageBox.Show(
                    $"Bạn có muốn lưu nhà cung cấp mới không?\n\nTên: {supplierName}",
                    "Lưu nhà cung cấp mới",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    _supplierId = 0;
                    return;
                }

                MessageBox.Show($"✅ Đã tạo mã: {code}",
                    "Tạo nhà cung cấp thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload ComboBox và chọn NCC vừa tạo
                LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // XÓA NHÀ CUNG CẤP
        // ─────────────────────────────────────────────────────

        private void btnDeleteSupplier_Click(object sender, EventArgs e)
        {
            if (_supplierId == 0)
            {
                MessageBox.Show("Chưa chọn nhà cung cấp nào!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentName = txtSupplierName.Text.Trim();

            if (MessageBox.Show(
                $"Bạn có chắc muốn xóa nhà cung cấp:\n\"{currentName}\" không?\n\n" +
                "⚠️ Chỉ xóa được nếu chưa có đơn hàng liên quan!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                // Toàn bộ SQL nằm trong PurchaseOrderRepository
                int orderCount = _repo.DeleteSupplier(_supplierId);

                if (orderCount > 0)
                {
                    MessageBox.Show(
                        $"❌ Không thể xóa!\nNhà cung cấp này đã có {orderCount} đơn hàng liên quan.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"✅ Đã xóa nhà cung cấp \"{currentName}\"",
                    "Xóa thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset thông tin NCC
                _supplierId = 0;
                txtSupplierName.Clear();
                txtTaxCode.Clear();
                txtSupplierAddress.Clear();
                LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvOrderDetails;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
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

            var styleReadOnly = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 10f)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                Width = 50,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMaterialCode",
                HeaderText = "Mã hàng",
                Width = 110,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colMaterialName", HeaderText = "Tên hàng", Width = 280 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUnit",
                HeaderText = "ĐVT",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQty",
                HeaderText = "Số lượng",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUnitPrice",
                HeaderText = "Đơn giá",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLineTotal",
                HeaderText = "Thành tiền",
                Width = 170,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 80, 160)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVatRate",
                HeaderText = "% VAT",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Cột ẩn lưu ID nguyên liệu
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colMaterialId", Width = 0, Visible = false });

            // Nút xóa dòng
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colDelete",
                HeaderText = "",
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(254, 226, 226),
                    ForeColor = Color.FromArgb(220, 38, 38),
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Đăng ký event 1 lần — dùng -= trước để tránh trùng
            dgv.CellValueChanged -= dgvOrderDetails_CellValueChanged;
            dgv.CellClick -= dgvOrderDetails_CellClick;
            dgv.KeyDown -= dgvOrderDetails_KeyDown;
            dgv.CellValueChanged += dgvOrderDetails_CellValueChanged;
            dgv.CellClick += dgvOrderDetails_CellClick;
            dgv.KeyDown += dgvOrderDetails_KeyDown;

            // Thêm dòng mới mặc định khi mở form
            AddNewRow();
        }


        // ─────────────────────────────────────────────────────
        // THÊM DÒNG MỚI VÀO GRID
        // ─────────────────────────────────────────────────────

        private void AddNewRow()
        {
            // Toàn bộ SQL nằm trong PurchaseOrderRepository
            string materialCode = _repo.GenerateMaterialCode(dgvOrderDetails.Rows.Count);
            int rowIndex = dgvOrderDetails.Rows.Count + 1;

            dgvOrderDetails.Rows.Add(rowIndex, materialCode, "", "", "1", "0", "0", "10", 0, "✕");
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN GRID
        // ─────────────────────────────────────────────────────

        private void dgvOrderDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvOrderDetails.Columns[e.ColumnIndex].Name;
            if (colName == "colQty" || colName == "colUnitPrice" || colName == "colVatRate")
            {
                CalculateRowTotal(e.RowIndex);
                UpdateTotals();
            }
        }

        private void CalculateRowTotal(int rowIndex)
        {
            var row = dgvOrderDetails.Rows[rowIndex];
            double.TryParse(row.Cells["colQty"].Value?.ToString().Replace(",", ""), out double qty);
            double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double unitPrice);
            row.Cells["colLineTotal"].Value = (qty * unitPrice).ToString("N0");
        }

        private void UpdateTotals()
        {
            double subTotal = 0;
            double vatTotal = 0;

            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                double.TryParse(row.Cells["colVatRate"].Value?.ToString(), out double vatRate);
                subTotal += lineTotal;
                vatTotal += lineTotal * vatRate / 100.0;
            }

            lblSubTotal.Text = subTotal.ToString("N0") + " đ";
            lblVatAmount.Text = vatTotal.ToString("N0") + " đ";
            lblGrandTotal.Text = (subTotal + vatTotal).ToString("N0") + " đ";
        }

        private void dgvOrderDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvOrderDetails.Columns[e.ColumnIndex].Name != "colDelete") return;

            // Giữ ít nhất 1 dòng
            if (dgvOrderDetails.Rows.Count <= 1) return;

            dgvOrderDetails.Rows.RemoveAt(e.RowIndex);

            // Cập nhật lại số thứ tự
            for (int i = 0; i < dgvOrderDetails.Rows.Count; i++)
                dgvOrderDetails.Rows[i].Cells["colSTT"].Value = i + 1;

            UpdateTotals();
        }

        private void dgvOrderDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.Handled = e.SuppressKeyPress = true;

            int nextRow = dgvOrderDetails.CurrentCell.RowIndex + 1;
            if (nextRow >= dgvOrderDetails.Rows.Count) AddNewRow();

            dgvOrderDetails.CurrentCell = dgvOrderDetails
                .Rows[Math.Min(nextRow, dgvOrderDetails.Rows.Count - 1)]
                .Cells[dgvOrderDetails.CurrentCell.ColumnIndex];
        }


        // ─────────────────────────────────────────────────────
        // NÚT THÊM DÒNG
        // ─────────────────────────────────────────────────────

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            if (_supplierId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn nhà cung cấp trước khi thêm vật tư!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frm = new frmSelectSupplierMaterial(_supplierId, txtSupplierName.Text.Trim());
            if (frm.ShowDialog() != DialogResult.OK)
            {
                frm.Dispose();
                return;
            }

            var picked = frm.SelectedMaterials;
            frm.Dispose();

            if (picked.Count == 0)
            {
                // Không có vật tư nào trong danh mục → thêm dòng trắng để nhập tay
                AddNewRow();
                int lastRow = dgvOrderDetails.Rows.Count - 1;
                dgvOrderDetails.CurrentCell = dgvOrderDetails.Rows[lastRow].Cells["colMaterialName"];
                dgvOrderDetails.BeginEdit(true);
                return;
            }

            // Xóa dòng trắng cuối nếu chưa có dữ liệu
            bool hasBlank = false;
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                string nm = row.Cells["colMaterialName"].Value?.ToString().Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nm) && dgvOrderDetails.Rows.Count > 1)
                { hasBlank = true; break; }
            }

            // Chèn vật tư được chọn vào grid
            foreach (var m in picked)
            {
                if (hasBlank && dgvOrderDetails.Rows.Count > 0)
                {
                    // Thay thế dòng trắng đầu tiên
                    int blankIdx = -1;
                    for (int i = 0; i < dgvOrderDetails.Rows.Count; i++)
                    {
                        string nm = dgvOrderDetails.Rows[i].Cells["colMaterialName"].Value?.ToString().Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(nm)) { blankIdx = i; break; }
                    }
                    if (blankIdx >= 0)
                    {
                        dgvOrderDetails.Rows[blankIdx].Cells["colMaterialId"].Value = m.MaterialId;
                        dgvOrderDetails.Rows[blankIdx].Cells["colMaterialCode"].Value = m.MaterialCode;
                        dgvOrderDetails.Rows[blankIdx].Cells["colMaterialName"].Value = m.MaterialName;
                        dgvOrderDetails.Rows[blankIdx].Cells["colUnit"].Value = m.Unit;
                        dgvOrderDetails.Rows[blankIdx].Cells["colQty"].Value = "1";
                        dgvOrderDetails.Rows[blankIdx].Cells["colUnitPrice"].Value = m.UnitPrice.ToString("N0");
                        dgvOrderDetails.Rows[blankIdx].Cells["colVatRate"].Value = "10";
                        CalculateRowTotal(blankIdx);
                        hasBlank = false; // đã dùng dòng trắng
                        continue;
                    }
                }

                // Thêm dòng mới
                int newIdx = dgvOrderDetails.Rows.Count;
                dgvOrderDetails.Rows.Add(
                    newIdx + 1,
                    m.MaterialCode,
                    m.MaterialName,
                    m.Unit,
                    "1",
                    m.UnitPrice.ToString("N0"),
                    m.UnitPrice.ToString("N0"),
                    "10",
                    m.MaterialId,
                    "✕");
                CalculateRowTotal(newIdx);
            }

            UpdateTotals();
        }


        // ─────────────────────────────────────────────────────
        // NÚT ĐÍNH KÈM FILE
        // ─────────────────────────────────────────────────────

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Tất cả file|*.*|PDF|*.pdf|Excel|*.xlsx;*.xls|Word|*.docx;*.doc",
                Title = "Chọn file đính kèm"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _attachedFile = dialog.FileName;
                MessageBox.Show(
                    $"✅ Đã chọn:\n{System.IO.Path.GetFileName(_attachedFile)}",
                    "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // ─────────────────────────────────────────────────────
        // NÚT LÀM MỚI
        // ─────────────────────────────────────────────────────

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Không hỏi nếu vừa lưu xong
            if (!_isSaved)
            {
                if (MessageBox.Show(
                    "Bạn có chắc muốn làm mới?\nDữ liệu chưa lưu sẽ bị mất!",
                    "Xác nhận làm mới",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) != DialogResult.OK) return;
            }

            // Reset toàn bộ form
            _isSaved = false;
            _savedOrderId = 0;
            _supplierId = 0;
            _attachedFile = "";

            cmbSupplier.SelectedIndex = 0;
            txtSupplierName.Clear();
            txtTaxCode.Clear();
            txtSupplierAddress.Clear();
            dtpOrderDate.Value = DateTime.Today;
            dtpDeliveryDate.Value = DateTime.Today;
            cboOrderStatus.SelectedIndex = 0;
            cboPaymentMethod.SelectedIndex = 0;
            txtDebtDays.Text = "";
            txtDeliveryLocation.Text = "";
            txtNote.Text = "";

            dgvOrderDetails.Rows.Clear();
            AddNewRow();
            UpdateTotals();
            GenerateOrderCode();
        }


        // ─────────────────────────────────────────────────────
        // NÚT LƯU ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            // ── VALIDATE ──
            if (_supplierId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn hoặc nhập Tên nhà cung cấp!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOrderCode.Text))
            {
                MessageBox.Show("⚠️ Số đơn hàng không được trống!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool hasItems = false;
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
                if (!string.IsNullOrWhiteSpace(row.Cells["colMaterialName"].Value?.ToString()))
                { hasItems = true; break; }

            if (!hasItems)
            {
                MessageBox.Show("⚠️ Vui lòng nhập ít nhất 1 mặt hàng!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đơn đã có phiếu nhập → không UPDATE được; chuẩn bị đơn mới (mã mới, INSERT)
            if (_savedOrderId > 0 && _repo.PurchaseOrderHasReceipt(_savedOrderId))
            {
                MessageBox.Show(
                    "Đơn đã có phiếu nhập kho — không cập nhật chi tiết trên đơn cũ.\nHệ thống đã chuẩn bị đơn mua mới (mã mới), giữ nhà cung cấp.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetOrderDraftForNewEntry();
            }

            try
            {
                // Tính tổng tiền
                double grandTotal = 0;
                foreach (DataGridViewRow row in dgvOrderDetails.Rows)
                {
                    double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                    double.TryParse(row.Cells["colVatRate"].Value?.ToString(), out double vatRate);
                    grandTotal += lineTotal + lineTotal * vatRate / 100.0;
                }

                // Đóng gói chi tiết thành DataTable để gửi vào Repository
                var details = BuildDetailTable();

                if (details.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Vui lòng nhập ít nhất 1 mặt hàng có tên!",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Đơn mới → INSERT; đơn đã mở từ combo → UPDATE (trừ khi đã có phiếu nhập)
                if (_savedOrderId > 0)
                {
                    _repo.UpdatePurchaseOrder(
                        _savedOrderId,
                        txtOrderCode.Text.Trim(),
                        _supplierId,
                        dtpOrderDate.Value.Date,
                        dtpDeliveryDate.Value.Date,
                        cboPaymentMethod.SelectedItem?.ToString() ?? "",
                        int.TryParse(txtDebtDays.Text, out int dd2) ? dd2 : 0,
                        cboOrderStatus.SelectedItem?.ToString() ?? "Chưa thực hiện",
                        txtDeliveryLocation.Text.Trim(),
                        _attachedFile ?? "",
                        txtNote.Text.Trim(),
                        (decimal)grandTotal,
                        details);
                }
                else
                {
                    _savedOrderId = _repo.SavePurchaseOrder(
                        txtOrderCode.Text.Trim(),
                        _supplierId,
                        dtpOrderDate.Value.Date,
                        dtpDeliveryDate.Value.Date,
                        cboPaymentMethod.SelectedItem?.ToString() ?? "",
                        int.TryParse(txtDebtDays.Text, out int dd) ? dd : 0,
                        cboOrderStatus.SelectedItem?.ToString() ?? "Chưa thực hiện",
                        txtDeliveryLocation.Text.Trim(),
                        _attachedFile ?? "",
                        txtNote.Text.Trim(),
                        (decimal)grandTotal,
                        details);
                }

                _isSaved = true;

                // Lưu vật tư mới vào danh mục NCC (nếu đã có trong danh mục thì SP tự bỏ qua)
                try { _repo.SaveMaterialsForSupplier(_supplierId, details); }
                catch { /* không chặn lưu đơn nếu lỗi danh mục */ }

                // Reload danh sách NCC để combobox hiện đơn vừa lưu
                int savedSupId = _supplierId;
                LoadSuppliers();
                // Chọn lại đúng NCC vừa dùng
                foreach (SupplierItem item in cmbSupplier.Items)
                {
                    if (item.Id == savedSupId)
                    {
                        cmbSupplier.SelectedItem = item;
                        break;
                    }
                }

                ApplyPurchaseOrderScrollExtent();

                MessageBox.Show(
                    $"✅ Lưu đơn hàng thành công!\n\nSố đơn: {txtOrderCode.Text}\nTổng tiền: {grandTotal:N0} đ",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string msg = ex.Message ?? "";
                if (msg.IndexOf("phiếu nhập kho", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("ghi đè chi tiết", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (MessageBox.Show(
                        "Đơn này đã có phiếu nhập kho — không sửa chi tiết trên màn này.\n\nChuyển sang đơn mua mới (giữ nhà cung cấp)?",
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        ResetOrderDraftForNewEntry();
                    return;
                }

                MessageBox.Show($"❌ {msg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DỮ LIỆU GRID THÀNH DATATABLE GỬI VÀO REPOSITORY
        // ─────────────────────────────────────────────────────

        private DataTable BuildDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("MaterialId", typeof(int));
            dt.Columns.Add("MaterialCode", typeof(string));
            dt.Columns.Add("MaterialName", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Qty", typeof(double));
            dt.Columns.Add("UnitPrice", typeof(double));
            dt.Columns.Add("LineTotal", typeof(double));
            dt.Columns.Add("VatRate", typeof(double));
            dt.Columns.Add("VatAmount", typeof(double));

            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                string itemName = row.Cells["colMaterialName"].Value?.ToString().Trim() ?? "";
                if (string.IsNullOrWhiteSpace(itemName)) continue;

                int.TryParse(row.Cells["colMaterialId"].Value?.ToString(), out int materialId);
                double.TryParse(row.Cells["colQty"].Value?.ToString().Replace(",", ""), out double qty);
                double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double unitPrice);
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                double.TryParse(row.Cells["colVatRate"].Value?.ToString(), out double vatRate);
                double vatAmount = lineTotal * vatRate / 100.0;

                dt.Rows.Add(
                    materialId,
                    row.Cells["colMaterialCode"].Value?.ToString() ?? "",
                    itemName,
                    row.Cells["colUnit"].Value?.ToString() ?? "",
                    qty, unitPrice, lineTotal, vatRate, vatAmount);
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // NÚT NHẬP KHO
        // ─────────────────────────────────────────────────────

        private void btnReceiveStock_Click(object sender, EventArgs e)
        {
            // Lưu đơn trước nếu chưa lưu
            if (!_isSaved || _savedOrderId == 0)
            {
                if (_supplierId == 0)
                {
                    MessageBox.Show("⚠️ Vui lòng chọn nhà cung cấp trước khi nhập kho!",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool hasItems = false;
                foreach (DataGridViewRow row in dgvOrderDetails.Rows)
                    if (!string.IsNullOrWhiteSpace(row.Cells["colMaterialName"].Value?.ToString()))
                    { hasItems = true; break; }

                if (!hasItems)
                {
                    MessageBox.Show("⚠️ Vui lòng nhập ít nhất 1 mặt hàng trước khi nhập kho!",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show(
                    "Đơn hàng chưa được lưu.\nHệ thống sẽ lưu đơn hàng trước rồi chuyển sang Phiếu Nhập Kho.\n\nBạn có đồng ý không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                // Lưu đơn hàng
                btnSave_Click(null, EventArgs.Empty);

                // Nếu lưu không thành công thì thoát
                if (!_isSaved || _savedOrderId == 0) return;
            }

            // Mở form nhập kho với ID đơn hàng đúng
            int orderIdForReceive = _savedOrderId;
            var frm = new frmInventoryReceive(orderIdForReceive);
            frm.FormClosed += (_, _) =>
            {
                if (orderIdForReceive > 0 && _repo.PurchaseOrderHasReceipt(orderIdForReceive))
                    ResetOrderDraftForNewEntry();
            };
            frm.Show(this);
        }


        // ─────────────────────────────────────────────────────
        // NÚT IN ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_supplierId == 0)
            {
                MessageBox.Show("⚠️ Chưa có thông tin nhà cung cấp!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double subTotal = 0;
            double vatTotal = 0;

            // Lưu dữ liệu vào field
            _printOrderCode = txtOrderCode.Text.Trim();
            _printSupplierName = txtSupplierName.Text.Trim();
            _printTaxCode = txtTaxCode.Text.Trim();
            _printAddress = txtSupplierAddress.Text.Trim();
            _printOrderDate = dtpOrderDate.Value.ToString("dd/MM/yyyy");

            _printPurchaseLines.Clear();
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                double.TryParse(row.Cells["colLineTotal"].Value?.ToString().Replace(",", ""), out double lineTotal);
                double.TryParse(row.Cells["colVatRate"].Value?.ToString(), out double vatRate);
                subTotal += lineTotal;
                vatTotal += lineTotal * vatRate / 100.0;

                string itemName = row.Cells["colMaterialName"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(itemName)) continue;

                _printPurchaseLines.Add(new[]
                {
                    row.Cells["colUnit"].Value?.ToString() ?? "",
                    row.Cells["colQty"].Value?.ToString() ?? "",
                    row.Cells["colUnitPrice"].Value?.ToString() ?? "",
                    row.Cells["colLineTotal"].Value?.ToString() ?? "",
                    row.Cells["colVatRate"].Value?.ToString() ?? "",
                    itemName
                });
            }

            // Gỡ handler cũ trước khi thêm mới
            if (_currentPrintDoc != null)
                _currentPrintDoc.PrintPage -= PrintPurchasePage;

            _currentPrintDoc = new System.Drawing.Printing.PrintDocument();
            _currentPrintDoc.PrintPage += PrintPurchasePage;

            new PrintPreviewDialog
            {
                Document = _currentPrintDoc,
                WindowState = FormWindowState.Maximized
            }.ShowDialog();
        }

        private void PrintPurchasePage(object s, System.Drawing.Printing.PrintPageEventArgs ev)
        {
            var g = ev.Graphics;
            var fTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
            var fHeader = new Font("Segoe UI", 10f, FontStyle.Bold);
            var fNormal = new Font("Segoe UI", 10f);
            var fSmall = new Font("Segoe UI", 9f);

            int x = 40, y = 30;

            g.DrawString("ĐƠN MUA HÀNG", fTitle, Brushes.Black, x, y); y += 30;
            g.DrawString($"Số đơn: {_printOrderCode}   |   Ngày: {_printOrderDate}", fNormal, Brushes.Gray, x, y); y += 25;
            g.DrawLine(Pens.LightGray, x, y, 750, y); y += 12;

            g.DrawString("NHÀ CUNG CẤP", fHeader, Brushes.Black, x, y); y += 20;
            g.DrawString($"Tên: {_printSupplierName}   |   MST: {_printTaxCode}   |   Địa chỉ: {_printAddress}", fNormal, Brushes.Black, x, y); y += 25;
            g.DrawLine(Pens.LightGray, x, y, 750, y); y += 15;

            // Header bảng
            g.FillRectangle(new SolidBrush(Color.FromArgb(248, 249, 250)), x, y, 710, 26);
            g.DrawString("STT", fHeader, Brushes.Black, x + 5, y + 4);
            g.DrawString("Tên hàng", fHeader, Brushes.Black, x + 50, y + 4);
            g.DrawString("ĐVT", fHeader, Brushes.Black, x + 280, y + 4);
            g.DrawString("SL", fHeader, Brushes.Black, x + 340, y + 4);
            g.DrawString("Đơn giá", fHeader, Brushes.Black, x + 400, y + 4);
            g.DrawString("Thành tiền", fHeader, Brushes.Black, x + 520, y + 4);
            g.DrawString("VAT", fHeader, Brushes.Black, x + 650, y + 4);
            y += 30;

            double subTotal = 0, vatTotal = 0;
            int rowIndex = 1;
            foreach (string[] line in _printPurchaseLines)
            {
                if (rowIndex % 2 == 0)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(250, 252, 255)), x, y, 710, 22);

                double.TryParse(line[3].Replace(",", ""), out double lt);
                double.TryParse(line[4], out double vr);
                subTotal += lt;
                vatTotal += lt * vr / 100.0;

                g.DrawString(rowIndex.ToString(), fSmall, Brushes.Black, x + 5, y + 3);
                g.DrawString(line[5], fSmall, Brushes.Black, x + 50, y + 3);
                g.DrawString(line[0], fSmall, Brushes.Black, x + 280, y + 3);
                g.DrawString(line[1], fSmall, Brushes.Black, x + 340, y + 3);
                g.DrawString(line[2], fSmall, Brushes.Black, x + 400, y + 3);
                g.DrawString(line[3], fSmall, Brushes.Black, x + 520, y + 3);
                g.DrawString(line[4] + "%", fSmall, Brushes.Black, x + 650, y + 3);
                y += 24;
                rowIndex++;
            }

            y += 12;
            g.DrawLine(Pens.Gray, x + 400, y, 750, y); y += 8;
            g.DrawString($"Tổng tiền hàng:   {subTotal:N0} đ", fNormal, Brushes.Black, x + 400, y); y += 22;
            g.DrawString($"Thuế GTGT:        {vatTotal:N0} đ", fNormal, Brushes.Black, x + 400, y); y += 22;
            g.DrawString($"TỔNG THANH TOÁN: {(subTotal + vatTotal):N0} đ",
                fHeader, new SolidBrush(Color.FromArgb(30, 80, 200)), x + 400, y);

            fTitle.Dispose(); fHeader.Dispose(); fNormal.Dispose(); fSmall.Dispose();
        }


        // ─────────────────────────────────────────────────────
        // INNER CLASS — ĐỐI TƯỢNG NHÀ CUNG CẤP TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Hiển thị trong combobox NCC:
        /// - Dòng trống: placeholder.
        /// - NCC chưa có đơn: mã NCC.
        /// - NCC đã có đơn: "Mã NCC | Đơn DD/MM/YYYY | Trạng thái".
        /// </summary>
        private class SupplierItem
        {
            public int Id { get; set; }
            public string Code { get; set; } = "";
            public string Name { get; set; } = "-- Chọn nhà cung cấp --";
            public string Address { get; set; } = "";
            public string TaxCode { get; set; } = "";
            public string Phone { get; set; } = "";
            public int LatestOrderId { get; set; }
            public string LatestOrderCode { get; set; } = "";
            public DateTime? LatestOrderDate { get; set; }
            public string LatestOrderStatus { get; set; } = "";

            public override string ToString()
            {
                if (Id == 0)
                    return Name; // dòng trống

                if (LatestOrderId > 0 && !string.IsNullOrEmpty(LatestOrderCode))
                    return $"{Code}  ▸  {LatestOrderCode}  |  {LatestOrderDate:dd/MM/yyyy}  |  {LatestOrderStatus}";

                return Code;
            }
        }
    }
}