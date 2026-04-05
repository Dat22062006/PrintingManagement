// ═══════════════════════════════════════════════════════════════════
// ║  frmInventoryIssue.cs - TỔNG HỢP TỒN KHO                       ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Hiển thị tồn kho nguyên vật liệu, cảnh báo sắp hết
// và hỗ trợ tạo đơn mua hàng, xuất báo cáo Excel.
// Toàn bộ DB ủy thác cho InventoryRepository.
// ═══════════════════════════════════════════════════════════════════

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryIssue : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly InventoryRepository _repo = new();

        private DateTimePicker _dtpPeriodFrom = null!;
        private DateTimePicker _dtpPeriodTo = null!;
        private bool _periodFilterBuilt;


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmInventoryIssue()
        {
            InitializeComponent();
            // Load chỉ trong Designer — tránh Load 2 lần → 2 hàng "Kỳ" và panel bị giãn gấp đôi.
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmInventoryIssue_Load(object sender, EventArgs e)
        {
            SetupInventoryGrid();
            SetupLowStockGrid();
            BuildInventoryPeriodFilters();
            LoadFilterOptions();
            LoadInventoryData();
        }

        private int ScaleInvGap(int designPixelsAt96) =>
            (int)Math.Round(designPixelsAt96 * (double)DeviceDpi / 96.0);

        /// <summary>
        /// Hàng chọn kỳ (từ ngày → đến ngày) để SP tính nhập/xuất trong kỳ; mặc định tháng hiện tại.
        /// Chèn dưới hàng combo lọc, đẩy nút "Tìm kiếm" / tiêu đề danh sách xuống — không chỉnh Top các panel Dock
        /// (tránh chồng hàng + lệch khi sang máy DPI khác).
        /// </summary>
        private void BuildInventoryPeriodFilters()
        {
            if (_periodFilterBuilt)
                return;
            _periodFilterBuilt = true;

            int Px(int d) => ScaleInvGap(d);

            int comboBottom = Math.Max(cboMaterialType.Bottom, Math.Max(cboStockStatus.Bottom, txtSearch.Bottom));
            int yRow = comboBottom + Px(10);
            int btnRowTopBefore = btnSearch.Top;

            // Ước lượng chỗ cần thêm; nếu chồng lên hàng nút thì đẩy toàn bộ phía dưới xuống trước khi Add control.
            int estRowBottom = yRow + Px(34);
            int overlap = estRowBottom + Px(10) - btnRowTopBefore;
            if (overlap > 0)
            {
                foreach (Control c in paneltimkiemvaloc.Controls)
                {
                    if (c.Top >= btnRowTopBefore - Px(1))
                        c.Top += overlap;
                }
            }

            var startMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endMonth = startMonth.AddMonths(1).AddDays(-1);
            Font f = labelloaivattu.Font;

            var lbl = new Label
            {
                Text = "Kỳ (nhập / xuất):",
                AutoSize = true,
                Font = f,
                Location = new Point(Px(12), yRow + Px(3))
            };
            _dtpPeriodFrom = new DateTimePicker
            {
                Location = new Point(Px(148), yRow),
                Width = Px(118),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                ShowUpDown = false,
                Value = startMonth,
                Font = f
            };
            var lblDen = new Label
            {
                Text = "đến",
                AutoSize = true,
                Font = f,
                Location = new Point(Px(275), yRow + Px(3))
            };
            _dtpPeriodTo = new DateTimePicker
            {
                Location = new Point(Px(318), yRow),
                Width = Px(118),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = endMonth,
                Font = f
            };
            var btnKy = new Button
            {
                Text = "Lọc kỳ",
                Location = new Point(Px(448), yRow - Px(2)),
                Size = new Size(Px(92), Px(28)),
                Font = f
            };
            btnKy.Click += (_, _) => LoadInventoryData();

            paneltimkiemvaloc.Controls.Add(lbl);
            paneltimkiemvaloc.Controls.Add(_dtpPeriodFrom);
            paneltimkiemvaloc.Controls.Add(lblDen);
            paneltimkiemvaloc.Controls.Add(_dtpPeriodTo);
            paneltimkiemvaloc.Controls.Add(btnKy);

            int maxB = 0;
            foreach (Control c in paneltimkiemvaloc.Controls)
                maxB = Math.Max(maxB, c.Bottom);
            paneltimkiemvaloc.Height = Math.Max(paneltimkiemvaloc.Height, maxB + Px(16));
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID TỒN KHO
        // ─────────────────────────────────────────────────────

        private void SetupInventoryGrid()
        {
            var dgv = dgvInventory;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 38;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 4, 0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);

            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.5f)
            };
            var styleRight = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5f),
                Padding = new Padding(0, 0, 8, 0)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colSTT", HeaderText = "STT", FillWeight = 5, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colItemCode",
                HeaderText = "Mã hàng",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Padding = new Padding(6, 0, 4, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colItemName",
                HeaderText = "Tên hàng",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle
                { Font = new Font("Segoe UI", 9.5f), Padding = new Padding(6, 0, 4, 0) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colUnit", HeaderText = "ĐVT", FillWeight = 6, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colOpeningStock", HeaderText = "Tồn đầu", FillWeight = 8, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStockIn", HeaderText = "Nhập trong kỳ", FillWeight = 9, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStockOut", HeaderText = "Xuất trong kỳ", FillWeight = 9, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colClosingStock",
                HeaderText = "Tồn cuối",
                FillWeight = 8,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colUnitPrice", HeaderText = "Đơn giá", FillWeight = 10, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStockValue",
                HeaderText = "Giá trị tồn",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    Padding = new Padding(0, 0, 8, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStatus", HeaderText = "Trạng thái", FillWeight = 10, DefaultCellStyle = styleCenter });

            // Cột ẩn
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colClosingStockRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockValueRaw", Visible = false });

            // Đăng ký event vẽ badge trạng thái
            dgv.CellPainting += dgvInventory_CellPainting;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID SẮP HẾT
        // ─────────────────────────────────────────────────────

        private void SetupLowStockGrid()
        {
            var dgv = dgvLowStock;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.5f)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLsItemCode",
                HeaderText = "Mã hàng",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Padding = new Padding(6, 0, 4, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLsItemName",
                HeaderText = "Tên hàng",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle
                { Font = new Font("Segoe UI", 9.5f), Padding = new Padding(6, 0, 4, 0) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLsCurrentStock",
                HeaderText = "Tồn hiện tại",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colLsUnit", HeaderText = "ĐVT", FillWeight = 8, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colLsMinStock", HeaderText = "Mức tồn tối thiểu", FillWeight = 13, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colLsSuggestQty", HeaderText = "Đề xuất nhập", FillWeight = 13, DefaultCellStyle = styleCenter });

            // Nút tạo đơn mua
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colLsCreateOrder",
                HeaderText = "Thao tác",
                Text = "🛒 Tạo đơn mua",
                UseColumnTextForButtonValue = true,
                FillWeight = 13,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(4)
                }
            });

            // Cột ẩn ID nguyên liệu
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLsMaterialId", Visible = false });

            dgv.CellContentClick += dgvLowStock_CellContentClick;
        }


        // ─────────────────────────────────────────────────────
        // NẠP TÙY CHỌN FILTER
        // ─────────────────────────────────────────────────────

        private void LoadFilterOptions()
        {
            cboMaterialType.Items.Clear();
            cboMaterialType.Items.AddRange(new string[]
                { "Tất cả", "Giấy", "Mực in", "Vật tư phụ", "Thùng đóng hàng", "Khác" });
            cboMaterialType.SelectedIndex = 0;

            cboStockStatus.Items.Clear();
            cboStockStatus.Items.AddRange(new string[]
                { "Tất cả", "Còn hàng", "Sắp hết", "Hết hàng" });
            cboStockStatus.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU TỒN KHO
        // ─────────────────────────────────────────────────────

        private void LoadInventoryData()
        {
            dgvInventory.Rows.Clear();
            dgvLowStock.Rows.Clear();

            string keyword = txtSearch.Text.Trim();
            string statusFilter = cboStockStatus.SelectedItem?.ToString() ?? "Tất cả";

            // Xác định tiền tố mã hàng theo loại vật tư
            string materialType = cboMaterialType.SelectedItem?.ToString() ?? "Tất cả";
            string codePrefix = materialType switch
            {
                "Giấy" => "G",
                "Mực in" => "M",
                "Vật tư phụ" => "P",
                "Thùng đóng hàng" => "T",
                _ => ""
            };

            int totalItems = 0;
            double totalValue = 0;
            int lowStockCount = 0;
            int oldStockCount = 0; // Dùng cho tính năng tồn quá lâu (mở rộng sau)

            try
            {
                DateTime fromD = _dtpPeriodFrom?.Value.Date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime toD = _dtpPeriodTo?.Value.Date ?? fromD.AddMonths(1).AddDays(-1);
                if (toD < fromD)
                    toD = fromD;

                // Toàn bộ SQL nằm trong InventoryRepository (@FromDate / @ToDate cần cập nhật SP)
                var dt = _repo.GetInventoryData(keyword, codePrefix, fromD, toD);

                int rowIndex = 1;

                foreach (System.Data.DataRow dataRow in dt.Rows)
                {
                    int materialId = Convert.ToInt32(dataRow["id"]);
                    string itemCode = dataRow["Ma_Nguyen_Lieu"].ToString();
                    string itemName = dataRow["Ten_Nguyen_Lieu"].ToString();
                    string unit = dataRow["Don_Vi_Tinh"].ToString();
                    double stockIn = Convert.ToDouble(dataRow["StockIn"]);
                    double stockOut = Convert.ToDouble(dataRow["StockOut"]);
                    double closingStock = Convert.ToDouble(dataRow["ClosingStock"]);
                    double unitPrice = Convert.ToDouble(dataRow["Gia_Nhap"]);
                    double minStock = Convert.ToDouble(dataRow["Ton_Kho_Toi_Thieu"]);
                    double suggestQty = Convert.ToDouble(dataRow["SuggestQty"]);

                    // Tính tồn đầu và giá trị
                    double openingStock = closingStock - stockIn + stockOut;
                    if (openingStock < 0) openingStock = 0;
                    double stockValue = closingStock * unitPrice;

                    // Xác định trạng thái
                    string statusLabel;
                    if (closingStock <= 0)
                        statusLabel = "HẾT HÀNG";
                    else if (closingStock <= minStock)
                        statusLabel = "SẮP HẾT";
                    else
                        statusLabel = "CÒN HÀNG";

                    // Lọc theo trạng thái
                    if (statusFilter != "Tất cả")
                    {
                        if (statusFilter == "Còn hàng" && statusLabel != "CÒN HÀNG") continue;
                        if (statusFilter == "Sắp hết" && statusLabel != "SẮP HẾT") continue;
                        if (statusFilter == "Hết hàng" && statusLabel != "HẾT HÀNG") continue;
                    }

                    // Thêm vào grid tồn kho
                    int idx = dgvInventory.Rows.Add(
                        rowIndex++, itemCode, itemName, unit,
                        openingStock.ToString("N0"),
                        stockIn.ToString("N0"),
                        stockOut.ToString("N0"),
                        closingStock.ToString("N0"),
                        unitPrice.ToString("N0"),
                        stockValue.ToString("N0"),
                        statusLabel,
                        materialId, closingStock, stockValue);

                    // Tô màu cột tồn cuối
                    var gridRow = dgvInventory.Rows[idx];
                    if (closingStock <= 0)
                        gridRow.Cells["colClosingStock"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                    else if (closingStock <= minStock)
                        gridRow.Cells["colClosingStock"].Style.ForeColor = Color.FromArgb(234, 88, 12);

                    // Cập nhật KPI
                    totalItems++;
                    totalValue += stockValue;
                    if (statusLabel == "SẮP HẾT" || statusLabel == "HẾT HÀNG") lowStockCount++;

                    // Thêm vào grid sắp hết / hết hàng
                    if (statusLabel != "CÒN HÀNG")
                    {
                        dgvLowStock.Rows.Add(
                            itemCode, itemName,
                            closingStock.ToString("N0") + " " + unit,
                            unit,
                            minStock.ToString("N0") + " " + unit,
                            suggestQty.ToString("N0") + " " + unit,
                            "🛒 Tạo đơn mua",
                            materialId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Cập nhật nhãn KPI
            lblTotalItems.Text = totalItems.ToString();
            lblStockValue.Text = FormatCurrency(totalValue);
            lblLowStockCount.Text = lowStockCount.ToString();
            lblOldStockCount.Text = oldStockCount.ToString();
            lblTotalValueLabel.Text = "Tổng giá trị tồn kho:";
            lblTotalValueAmount.Text = totalValue.ToString("N0") + " đ";

            // Tô màu cột tồn hiện tại trong grid sắp hết
            foreach (DataGridViewRow row in dgvLowStock.Rows)
            {
                string currentVal = row.Cells["colLsCurrentStock"].Value?.ToString() ?? "";
                row.Cells["colLsCurrentStock"].Style.ForeColor =
                    currentVal.StartsWith("0")
                        ? Color.FromArgb(220, 38, 38)
                        : Color.FromArgb(234, 88, 12);
            }
        }


        // ─────────────────────────────────────────────────────
        // FORMAT SỐ TIỀN
        // ─────────────────────────────────────────────────────

        private string FormatCurrency(double value)
        {
            if (value >= 1_000_000_000) return $"{value / 1_000_000_000:0.#} tỷ";
            if (value >= 1_000_000) return $"{value / 1_000_000:0.#} triệu";
            return value.ToString("N0");
        }


        // ─────────────────────────────────────────────────────
        // VẼ BADGE TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void dgvInventory_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvInventory.Columns[e.ColumnIndex].Name != "colStatus") return;

            e.PaintBackground(e.ClipBounds, true);

            string statusText = e.Value?.ToString() ?? "";
            Color badgeBg, badgeFg;

            switch (statusText)
            {
                case "CÒN HÀNG":
                    badgeBg = Color.FromArgb(220, 252, 231); badgeFg = Color.FromArgb(21, 128, 61); break;
                case "SẮP HẾT":
                    badgeBg = Color.FromArgb(254, 243, 199); badgeFg = Color.FromArgb(146, 64, 14); break;
                default:
                    badgeBg = Color.FromArgb(254, 226, 226); badgeFg = Color.FromArgb(185, 28, 28); break;
            }

            int badgeX = e.CellBounds.X + (e.CellBounds.Width - 70) / 2;
            int badgeY = e.CellBounds.Y + (e.CellBounds.Height - 22) / 2;
            var badgeBounds = new Rectangle(badgeX, badgeY, 70, 22);

            using (var brush = new SolidBrush(badgeBg))
            using (var pen = new Pen(badgeBg))
            {
                e.Graphics.FillRectangle(brush, badgeBounds);
                e.Graphics.DrawRectangle(pen, badgeBounds);
            }

            using (var brush = new SolidBrush(badgeFg))
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(statusText, font, brush, badgeBounds, sf);
            }

            e.Handled = true;
        }


        // ─────────────────────────────────────────────────────
        // CLICK NÚT TẠO ĐƠN MUA TRONG GRID SẮP HẾT
        // ─────────────────────────────────────────────────────

        private void dgvLowStock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvLowStock.Columns[e.ColumnIndex].Name != "colLsCreateOrder") return;

            int materialId = Convert.ToInt32(dgvLowStock.Rows[e.RowIndex].Cells["colLsMaterialId"].Value);
            string itemCode = dgvLowStock.Rows[e.RowIndex].Cells["colLsItemCode"].Value?.ToString();
            string itemName = dgvLowStock.Rows[e.RowIndex].Cells["colLsItemName"].Value?.ToString();

            var frm = new frmPurchaseOrder(materialId, itemCode, itemName);
            frm.ShowDialog(this);
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN FILTER
        // ─────────────────────────────────────────────────────

        private void btnSearch_Click(object sender, EventArgs e) => LoadInventoryData();
        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadInventoryData();
        private void cboMaterialType_SelectedIndexChanged(object sender, EventArgs e) => LoadInventoryData();
        private void cboStockStatus_SelectedIndexChanged(object sender, EventArgs e) => LoadInventoryData();


        // ─────────────────────────────────────────────────────
        // NÚT XUẤT EXCEL
        // ─────────────────────────────────────────────────────

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvInventory.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Không có dữ liệu để xuất Excel!", "Thông báo");
                return;
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("BaoCaoTonKho");
                    int r = 1;

                    // Tiêu đề
                    ws.Cells[r, 1].Value = "TỔNG HỢP TỒN KHO";
                    ws.Cells[r, 1, r, 12].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;

                    ws.Cells[r, 1].Value = "Kho: Nguyên Vật Liệu Giấy, Tháng " + DateTime.Today.ToString("MM/yyyy");
                    ws.Cells[r, 1, r, 12].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true;
                    r += 2;

                    // Header
                    string[] headers =
                    {
                        "Tên kho", "Mã hàng", "Tên hàng", "ĐVT",
                        "Đầu kỳ SL", "Đầu kỳ Giá trị",
                        "Nhập kho SL", "Nhập kho Giá trị",
                        "Xuất kho SL", "Xuất kho Giá trị",
                        "Cuối kỳ SL", "Cuối kỳ Giá trị"
                    };

                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cells[r, c + 1].Value = headers[c];
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 240, 255));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    r++;

                    // Dữ liệu
                    foreach (DataGridViewRow row in dgvInventory.Rows)
                    {
                        string itemCode = row.Cells["colItemCode"].Value?.ToString() ?? "";
                        string itemName = row.Cells["colItemName"].Value?.ToString() ?? "";
                        string unit = row.Cells["colUnit"].Value?.ToString() ?? "";

                        double.TryParse(row.Cells["colOpeningStock"].Value?.ToString().Replace(",", ""), out double openingStock);
                        double.TryParse(row.Cells["colStockIn"].Value?.ToString().Replace(",", ""), out double stockIn);
                        double.TryParse(row.Cells["colStockOut"].Value?.ToString().Replace(",", ""), out double stockOut);
                        double.TryParse(row.Cells["colClosingStock"].Value?.ToString().Replace(",", ""), out double closingStock);
                        double.TryParse(row.Cells["colUnitPrice"].Value?.ToString().Replace(",", ""), out double unitPrice);

                        double openingValue = openingStock * unitPrice;
                        double inValue = stockIn * unitPrice;
                        double outValue = stockOut * unitPrice;
                        double closingValue = closingStock * unitPrice;

                        ws.Cells[r, 1].Value = "Nguyên Vật Liệu Giấy";
                        ws.Cells[r, 2].Value = itemCode;
                        ws.Cells[r, 3].Value = itemName;
                        ws.Cells[r, 4].Value = unit;
                        ws.Cells[r, 5].Value = openingStock;
                        ws.Cells[r, 6].Value = openingValue;
                        ws.Cells[r, 7].Value = stockIn;
                        ws.Cells[r, 8].Value = inValue;
                        ws.Cells[r, 9].Value = stockOut;
                        ws.Cells[r, 10].Value = outValue;
                        ws.Cells[r, 11].Value = closingStock;
                        ws.Cells[r, 12].Value = closingValue;

                        ws.Cells[r, 5, r, 12].Style.Numberformat.Format = "#,##0";
                        for (int c = 1; c <= 12; c++)
                            ws.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        r++;
                    }

                    // Độ rộng cột
                    ws.Column(1).Width = 18;
                    ws.Column(2).Width = 12;
                    ws.Column(3).Width = 28;
                    ws.Column(4).Width = 8;
                    for (int c = 5; c <= 12; c++) ws.Column(c).Width = 14;

                    // Lưu file
                    string filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"BAO_CAO_TON_KHO_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    pkg.SaveAs(new FileInfo(filePath));

                    MessageBox.Show($"✅ Đã xuất báo cáo tồn kho ra Excel:\n{filePath}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // NÚT KIỂM KHO — THỐNG KÊ NHANH
        // ─────────────────────────────────────────────────────

        private void btnCheckStock_Click(object sender, EventArgs e)
        {
            if (dgvInventory.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu tồn kho. Vui lòng tải dữ liệu trước.", "Thông báo");
                return;
            }

            int outOfStock = 0, lowStock = 0, inStock = 0;
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                string status = row.Cells["colStatus"].Value?.ToString() ?? "";
                if (status == "HẾT HÀNG") outOfStock++;
                else if (status == "SẮP HẾT") lowStock++;
                else if (status == "CÒN HÀNG") inStock++;
            }

            MessageBox.Show(
                $"Tổng mặt hàng: {dgvInventory.Rows.Count}\n" +
                $"Còn hàng:  {inStock}\n" +
                $"Sắp hết:   {lowStock}\n" +
                $"Hết hàng:  {outOfStock}",
                "Kết quả kiểm kho",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Cuộn về đầu danh sách sắp hết nếu có
            if (outOfStock > 0 && dgvLowStock.Rows.Count > 0)
            {
                dgvLowStock.ClearSelection();
                dgvLowStock.Rows[0].Selected = true;
                dgvLowStock.FirstDisplayedScrollingRowIndex = 0;
            }
        }
    }
}