// ═══════════════════════════════════════════════════════════════════
// ║  frmPurchaseReport.cs - BÁO CÁO MUA HÀNG  [FIX v3]            ║
// ═══════════════════════════════════════════════════════════════════
// FIX 1: BuildTabLayout() phải chạy TRƯỚC SetupSummaryGrid/LoadSummary
//         vì dgvPurchaseSummary cần được move vào tab trước khi setup cột
// FIX 2: Thứ tự Controls.Add cho DockStyle phải là:
//         Footer (Bottom) → Toolbar (Top) → Grid (Fill)
//         WinForms xử lý Dock theo thứ tự add NGƯỢC lại
// FIX 3: Tìm kiếm — event Click gán đúng sau khi _btnSearch tạo xong
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPurchaseReport : Form
    {
        // ─────────────────────────────────────────────────────
        // FIELDS
        // ─────────────────────────────────────────────────────

        private readonly PurchaseReportRepository _repo = new();

        private TabControl _tabMain;
        private TabPage _tabSummary;
        private TabPage _tabDetail;
        private TextBox _txtSearchNCC;
        private DateTimePicker _dtpFrom;
        private DateTimePicker _dtpTo;
        private Button _btnSearch;
        private Button _btnClear;
        private DataGridView _dgvDetail;
        private Label _lblSummary;

        // ─────────────────────────────────────────────────────
        // KHỞI TẠO
        // ─────────────────────────────────────────────────────

        public frmPurchaseReport()
        {
            InitializeComponent();
            // Load đã gắn trong frmPurchaseReport.Designer.cs — KHÔNG đăng ký thêm ở đây
            // (nếu gắn 2 lần: BuildTabLayout chạy 2 lần → 2 TabControl, tab tổng hợp trống, dock/nút lỗi).
        }

        // ─────────────────────────────────────────────────────
        // LOAD — thứ tự quan trọng:
        //   1. BuildTabLayout  (move dgvPurchaseSummary vào tab)
        //   2. SetupSummaryGrid (setup cột SAU KHI đã trong tab)
        //   3. SetupDetailGrid
        //   4. Load dữ liệu
        // ─────────────────────────────────────────────────────

        private void frmPurchaseReport_Load(object sender, EventArgs e)
        {
            BuildTabLayout();       // [FIX] phải chạy đầu tiên
            SetupSummaryGrid();     // [FIX] chạy SAU BuildTabLayout
            SetupDetailGrid();
            LoadSummary();
            LoadDetail();
        }

        // ─────────────────────────────────────────────────────
        // BUILD TAB LAYOUT
        // ─────────────────────────────────────────────────────

        private void BuildTabLayout()
        {
            if (_tabMain != null)
                return;

            // Tách dgvPurchaseSummary ra khỏi panel2 (designer đang để nó ở đây)
            panel2.Controls.Remove(dgvPurchaseSummary);
            dgvPurchaseSummary.Dock = DockStyle.Fill;

            // Tab 1 — Tổng hợp
            _tabSummary = new TabPage
            {
                Text = "📊  Tổng hợp theo NCC",
                Name = "tabSummary",
                Padding = new Padding(0)
            };
            _tabSummary.Controls.Add(dgvPurchaseSummary);

            // Tab 2 — Chi tiết (thứ tự add Controls quan trọng với Dock)
            _tabDetail = new TabPage
            {
                Text = "📋  Chi tiết nhập kho",
                Name = "tabDetail",
                Padding = new Padding(0)
            };

            // [FIX] Thứ tự add: Bottom trước → Top → Fill (WinForms dock ngược)
            _tabDetail.Controls.Add(CreateDetailGrid());      // Fill — add trước
            _tabDetail.Controls.Add(CreateDetailFooter());    // Bottom
            _tabDetail.Controls.Add(CreateDetailToolbar());   // Top — add sau cùng

            // TabControl
            _tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Name = "tabMain",
                Font = new Font("Segoe UI", 10f),
                Padding = new Point(14, 5)
            };
            _tabMain.TabPages.Add(_tabSummary);
            _tabMain.TabPages.Add(_tabDetail);

            panel2.Controls.Add(_tabMain);

            // Cập nhật label section cũ (giữ nguyên, không xóa)
            label1.Text = "Tổng hợp / Chi tiết mua hàng";
        }

        // ─────────────────────────────────────────────────────
        // TẠO TOOLBAR TÌM KIẾM
        // ─────────────────────────────────────────────────────

        private Control CreateDetailToolbar()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var lblNCC = new Label
            {
                Text = "Tìm NCC:",
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(12, 15)
            };

            _txtSearchNCC = new TextBox
            {
                Name = "txtSearchNCC",
                Width = 200,
                Height = 28,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(82, 12),
                PlaceholderText = "Tên hoặc mã NCC..."
            };
            // [FIX] Enter trong textbox cũng tìm được
            _txtSearchNCC.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    LoadDetail();
                }
            };

            var lblFrom = new Label
            {
                Text = "Từ:",
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(298, 15)
            };

            _dtpFrom = new DateTimePicker
            {
                Name = "dtpFrom",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 142,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(332, 12),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            var lblTo = new Label
            {
                Text = "Đến:",
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(486, 15)
            };

            _dtpTo = new DateTimePicker
            {
                Name = "dtpTo",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 142,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(524, 12),
                Value = DateTime.Today
            };

            _btnSearch = new Button
            {
                Text = "🔍  Tìm",
                Name = "btnSearchDetail",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Width = 90,
                Height = 28,
                Location = new Point(680, 12),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnSearch.FlatAppearance.BorderSize = 0;
            // [FIX] gán event đúng cách
            _btnSearch.Click += btnSearchDetail_Click;

            _btnClear = new Button
            {
                Text = "✕  Xóa lọc",
                Name = "btnClearDetail",
                Font = new Font("Segoe UI", 10f),
                Width = 90,
                Height = 28,
                Location = new Point(778, 12),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnClear.FlatAppearance.BorderSize = 0;
            _btnClear.Click += btnClearDetail_Click;

            pnl.Controls.AddRange(new Control[]
            {
                lblNCC, _txtSearchNCC,
                lblFrom, _dtpFrom,
                lblTo, _dtpTo,
                _btnSearch, _btnClear
            });

            return pnl;
        }

        // ─────────────────────────────────────────────────────
        // EVENT HANDLERS NÚT TÌM / XÓA LỌC
        // [FIX] Tách thành method riêng thay vì lambda lồng nhau
        // ─────────────────────────────────────────────────────

        private void btnSearchDetail_Click(object sender, EventArgs e)
        {
            LoadDetail();
        }

        private void btnClearDetail_Click(object sender, EventArgs e)
        {
            _txtSearchNCC.Clear();
            _dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _dtpTo.Value = DateTime.Today;
            LoadDetail();
        }

        // ─────────────────────────────────────────────────────
        // TẠO FOOTER
        // ─────────────────────────────────────────────────────

        private Control CreateDetailFooter()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 34,
                BackColor = Color.FromArgb(239, 246, 255)
            };

            _lblSummary = new Label
            {
                Name = "lblDetailSummary",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 16, 0),
                Text = "Chưa có dữ liệu"
            };

            pnl.Controls.Add(_lblSummary);
            return pnl;
        }

        // ─────────────────────────────────────────────────────
        // TẠO GRID CHI TIẾT
        // ─────────────────────────────────────────────────────

        private Control CreateDetailGrid()
        {
            _dgvDetail = new DataGridView
            {
                Name = "dgvDetail",
                Dock = DockStyle.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(229, 231, 235),
                BackgroundColor = Color.White,
                ScrollBars = ScrollBars.Both,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            };
            _dgvDetail.RowTemplate.Height = 34;
            _dgvDetail.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            _dgvDetail.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            _dgvDetail.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            _dgvDetail.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            _dgvDetail.EnableHeadersVisualStyles = false;
            _dgvDetail.ColumnHeadersHeight = 40;
            _dgvDetail.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _dgvDetail.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 99, 235);
            _dgvDetail.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            return _dgvDetail;
        }

        // ─────────────────────────────────────────────────────
        // SETUP CỘT dgvDetail
        // ─────────────────────────────────────────────────────

        private void SetupDetailGrid()
        {
            if (_dgvDetail == null)
                return;

            _dgvDetail.Columns.Clear();

            var sRO = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f) };
            var sCenter = new DataGridViewCellStyle(sRO)
            { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var sRight = new DataGridViewCellStyle(sRO)
            { Alignment = DataGridViewContentAlignment.MiddleRight };

            void AddFixed(string name, string header, int w, DataGridViewCellStyle style = null)
            {
                _dgvDetail.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = w,
                    ReadOnly = true,
                    DefaultCellStyle = style ?? sRO
                });
            }

            AddFixed("colTenNCC", "Nhà cung cấp", 200);
            AddFixed("colNgayNhap", "Ngày nhập", 95, sCenter);
            AddFixed("colSoHoaDon", "Số hóa đơn", 120, sCenter);
            AddFixed("colMaDonHang", "Mã đơn hàng", 110, sCenter);
            AddFixed("colTenHang", "Tên hàng", 220);
            AddFixed("colDVT", "ĐVT", 70, sCenter);
            AddFixed("colSoLuong", "Số lượng", 95, sRight);

            _dgvDetail.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTrangThai",
                HeaderText = "Trạng thái",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 120,
                FillWeight = 100,
                ReadOnly = true,
                DefaultCellStyle = sCenter
            });
        }

        // ─────────────────────────────────────────────────────
        // SETUP CỘT dgvPurchaseSummary (Tab 1)
        // ─────────────────────────────────────────────────────

        private void SetupSummaryGrid()
        {
            var dgv = dgvPurchaseSummary;
            dgv.Columns.Clear();
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Dock = DockStyle.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 99, 235);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            var sRO = new DataGridViewCellStyle { Font = new Font("Segoe UI", 10f) };
            var sNum = new DataGridViewCellStyle(sRO)
            { Alignment = DataGridViewContentAlignment.MiddleRight };
            var sCtr = new DataGridViewCellStyle(sRO)
            { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var sRed = new DataGridViewCellStyle(sNum)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colSupplier", HeaderText = "Nhà cung cấp", FillWeight = 35, DefaultCellStyle = sRO });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colOrderCount", HeaderText = "Số phiếu NK", FillWeight = 12, DefaultCellStyle = sCtr });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colTotalAmount", HeaderText = "Tổng tiền hàng", FillWeight = 20, DefaultCellStyle = sNum });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPaidAmount", HeaderText = "Đã thanh toán", FillWeight = 18, DefaultCellStyle = sNum });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colRemainingDebt", HeaderText = "Còn nợ", FillWeight = 15, DefaultCellStyle = sRed });

            dgv.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgv.Columns[e.ColumnIndex].Name != "colRemainingDebt") return;
                if (dgv.Rows[e.RowIndex].Tag is double debt)
                    e.CellStyle.ForeColor = debt > 0
                        ? Color.FromArgb(220, 38, 38)
                        : Color.FromArgb(22, 163, 74);
            };

            // Guna2DataGridView: designer để ThemeStyle.HeaderStyle.Height = 4 → header/cột dễ “mất”.
            // Đồng bộ ThemeStyle với ColumnHeadersHeight / RowTemplate sau khi add cột.
            if (dgv is Guna.UI2.WinForms.Guna2DataGridView guna)
            {
                guna.ThemeStyle.HeaderStyle.Height = dgv.ColumnHeadersHeight;
                guna.ThemeStyle.RowsStyle.Height = dgv.RowTemplate.Height;
                guna.ThemeStyle.HeaderStyle.Font = dgv.ColumnHeadersDefaultCellStyle.Font;
                guna.ThemeStyle.HeaderStyle.BackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
                guna.ThemeStyle.HeaderStyle.ForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
            }
        }

        // ─────────────────────────────────────────────────────
        // LOAD DỮ LIỆU — TAB 1: TỔNG HỢP
        // ─────────────────────────────────────────────────────

        private void LoadSummary()
        {
            dgvPurchaseSummary.Rows.Clear();
            try
            {
                var dt = _repo.GetPurchaseSummary();
                foreach (DataRow row in dt.Rows)
                {
                    double total = Convert.ToDouble(row["TotalAmount"]);
                    double paid = Convert.ToDouble(row["PaidAmount"]);
                    double remaining = total - paid;

                    int idx = dgvPurchaseSummary.Rows.Add(
                        row["SupplierName"].ToString(),
                        Convert.ToInt32(row["OrderCount"]),
                        total.ToString("N0") + " đ",
                        paid.ToString("N0") + " đ",
                        remaining.ToString("N0") + " đ");

                    dgvPurchaseSummary.Rows[idx].Tag = remaining;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // LOAD DỮ LIỆU — TAB 2: CHI TIẾT
        // ─────────────────────────────────────────────────────

        private void LoadDetail()
        {
            if (_dgvDetail == null) return;

            _dgvDetail.Rows.Clear();
            if (_lblSummary != null) _lblSummary.Text = "Đang tải...";

            try
            {
                string keyword = _txtSearchNCC?.Text.Trim() ?? "";
                DateTime? from = _dtpFrom?.Value.Date;
                DateTime? to = _dtpTo?.Value.Date;

                var dt = _repo.GetPurchaseDetail(keyword, from, to);

                double totalSoLuong = 0;

                foreach (DataRow row in dt.Rows)
                {
                    double soLuong = TryDouble(row, "SoLuong");
                    totalSoLuong += soLuong;

                    string ngayNhap = row["NgayNhap"] != DBNull.Value
                        ? Convert.ToDateTime(row["NgayNhap"]).ToString("dd/MM/yyyy") : "";

                    _dgvDetail.Rows.Add(
                        row["TenNCC"]?.ToString() ?? "",
                        ngayNhap,
                        row["SoHoaDon"]?.ToString() ?? "",
                        row["MaDonHang"] != DBNull.Value ? row["MaDonHang"].ToString() : "",
                        row["TenHang"]?.ToString() ?? "",
                        row["DVT"]?.ToString() ?? "",
                        soLuong.ToString("N2"),
                        row["TrangThai"]?.ToString() ?? "");
                }

                if (_lblSummary != null)
                    _lblSummary.Text =
                        $"Tổng số lượng: {totalSoLuong:N2}    |    Số dòng: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                if (_lblSummary != null) _lblSummary.Text = "❌ Lỗi tải dữ liệu";
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // HELPER
        // ─────────────────────────────────────────────────────

        private static double TryDouble(DataRow row, string col)
            => row[col] != DBNull.Value ? Convert.ToDouble(row[col]) : 0;
    }
}