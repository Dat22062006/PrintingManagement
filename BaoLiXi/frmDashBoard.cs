using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public partial class frmDashBoard : Form
    {
        frmMain mainForm;
        public frmDashBoard(frmMain main)
        {
            InitializeComponent();
            mainForm = main;
        }

        void OpenForm(Form f) => mainForm.LoadForm(f);

        void Panel_MouseEnter(object sender, EventArgs e)
        {
            Panel pnl = GetParentPanel(sender as Control);
            if (pnl != null) pnl.BackColor = Color.FromArgb(46, 204, 113);
        }
        void Panel_MouseLeave(object sender, EventArgs e)
        {
            Panel pnl = GetParentPanel(sender as Control);
            if (pnl != null) pnl.BackColor = Color.Transparent;
        }
        Panel GetParentPanel(Control c)
        {
            while (c != null && !(c is Panel)) c = c.Parent;
            return c as Panel;
        }
        void AttachHover(Control parent)
        {
            parent.MouseEnter += Panel_MouseEnter;
            parent.MouseLeave += Panel_MouseLeave;
            foreach (Control c in parent.Controls)
            {
                c.MouseEnter += Panel_MouseEnter;
                c.MouseLeave += Panel_MouseLeave;
            }
        }
        void AttachClick(Control parent, EventHandler clickEvent)
        {
            parent.Click += clickEvent;
            foreach (Control c in parent.Controls)
                AttachClick(c, clickEvent);
        }

        private void frmDashBoard_Load(object sender, EventArgs e)
        {
            AttachHover(pnltinhgiamoi);
            AttachHover(pnltaodonmua);
            AttachHover(panellenhsanxuat);
            AttachHover(pnlbanhang);
            AttachClick(pnltinhgiamoi, pnltinhgiamoi_Click_1);
            AttachClick(pnltaodonmua, pnltaodonmua_Click_1);
            AttachClick(panellenhsanxuat, panellenhsanxuat_Click_1);
            AttachClick(pnlbanhang, pnlbanhang_Click_1);

            SetupDgvBaoGia();
            SetupDgvLenhSX();
            SetupDgvThongBao();
            LoadDashboardData();
        }

        void SetupDgvBaoGia()
        {
            var dgv = dgvBaoGia;
            dgv.Columns.Clear();
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 38;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 34;

            // ← FIX: Enable header visual styles để control được header
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 64, 175); // ← Giữ màu như bình thường
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Padding = new Padding(0, 0, 6, 0) };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGMa", HeaderText = "Số BG", FillWeight = 14, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGKhach", HeaderText = "Khách hàng", FillWeight = 24 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGSanPham", HeaderText = "Sản phẩm", FillWeight = 18 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGSoLuong", HeaderText = "Số lượng", FillWeight = 11, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGGiaTri", HeaderText = "Giá trị", FillWeight = 16, DefaultCellStyle = right });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGTrangThai", HeaderText = "Trạng thái", FillWeight = 13, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGNgay", HeaderText = "Ngày", FillWeight = 12, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBGId", Visible = false });
            dgv.CellPainting += DgvBaoGia_CellPainting;
        }

        void SetupDgvLenhSX()
        {
            var dgv = dgvLenhSX;
            dgv.Columns.Clear();
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 38;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 34;

            // ← FIX: Enable header visual styles để control được header
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 64, 175); // ← Giữ màu như bình thường
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXMa", HeaderText = "Số LSX", FillWeight = 16, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXSanPham", HeaderText = "Sản phẩm", FillWeight = 27 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXKhach", HeaderText = "Khách hàng", FillWeight = 27 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXNgayGiao", HeaderText = "Ngày giao", FillWeight = 15, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXTrangThai", HeaderText = "Trạng thái", FillWeight = 15, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSXId", Visible = false });
            dgv.CellPainting += DgvLenhSX_CellPainting;
        }

        void SetupDgvThongBao()
        {
            var dgv = dgvThongBao;
            dgv.Columns.Clear();
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 38;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 42;

            // ← FIX: Enable header visual styles để control được header
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 64, 175); // ← Giữ màu như bình thường
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTBIcon", HeaderText = "", FillWeight = 8, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTBNoiDung", HeaderText = "Thông báo", FillWeight = 64, DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTBThoiGian", HeaderText = "Thời gian", FillWeight = 28, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTBLoai", Visible = false });
        }

        private void LoadDashboardData()
        {
            try
            {
                lblsonguoibaogia.Text = CountBaoGia().ToString();
                lblsoluongdon.Text = CountBaoGiaDaDuyet().ToString();
                lblsoluongsanxuat.Text = CountBaoGiaDangSanXuat().ToString();
                lblsoluongvattu.Text = CountVatTuCanNhap().ToString();

                lblDoanhThu.Text = FormatTien(GetDoanhThuThangNay());
                lblTonKho.Text = GetSoMatHangTonKho() + " mặt hàng";
                lblNoPhaiThu.Text = FormatTien(GetTongNoPhaiThu());
                lblNoPhaiTra.Text = FormatTien(GetTongNoPhaiTra());

                lblNoPhaiThu.ForeColor = Color.FromArgb(76, 175, 80);
                lblNoPhaiTra.ForeColor = Color.FromArgb(244, 67, 54);

                LoadDgvBaoGia();
                LoadDgvLenhSX();
                LoadDgvThongBao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dashboard: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadDgvBaoGia()
        {
            dgvBaoGia.Rows.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT TOP 4
    bg.id, bg.Ma_Bao_Gia,
    ISNULL(kh.Ten_Khach_Hang, N'—') AS Ten_Khach_Hang,
    bg.Ten_San_Pham,
    ISNULL((SELECT TOP 1 ct.So_Luong FROM CHI_TIET_BAO_GIA ct
            WHERE ct.id_Bao_Gia = bg.id ORDER BY ct.id), 0) AS So_Luong,
    ISNULL((SELECT TOP 1 ct.Tong_Gia_Bao_Khach FROM CHI_TIET_BAO_GIA ct
            WHERE ct.id_Bao_Gia = bg.id ORDER BY ct.id), 0) AS Gia_Tri,
    bg.Trang_Thai, bg.Ngay_Bao_Gia
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
ORDER BY bg.Ngay_Tao DESC";
                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                        dgvBaoGia.Rows.Add(
                            rd["Ma_Bao_Gia"].ToString(),
                            rd["Ten_Khach_Hang"].ToString(),
                            rd["Ten_San_Pham"].ToString(),
                            Convert.ToDouble(rd["So_Luong"]).ToString("N0"),
                            Convert.ToDouble(rd["Gia_Tri"]).ToString("N0") + "đ",
                            rd["Trang_Thai"].ToString(),
                            Convert.ToDateTime(rd["Ngay_Bao_Gia"]).ToString("dd/MM/yyyy"),
                            rd["id"].ToString());
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi load báo giá: {ex.Message}"); }
        }

        void LoadDgvLenhSX()
        {
            dgvLenhSX.Rows.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT TOP 10
    lsx.id, lsx.Ma_Lenh_SX, lsx.Ten_San_Pham,
    ISNULL(kh.Ten_Khach_Hang, N'—') AS Ten_Khach_Hang,
    lsx.Ngay_Giao_Hang_Du_Kien, lsx.Trang_Thai
FROM LENH_SAN_XUAT lsx
LEFT JOIN KHACH_HANG kh ON kh.id = lsx.id_Khach_Hang
WHERE lsx.Trang_Thai = N'Đang sản xuất'
ORDER BY lsx.Ngay_Tao DESC";
                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        string ngayGiao = rd["Ngay_Giao_Hang_Du_Kien"] == DBNull.Value
                            ? "—" : Convert.ToDateTime(rd["Ngay_Giao_Hang_Du_Kien"]).ToString("dd/MM/yyyy");
                        dgvLenhSX.Rows.Add(
                            rd["Ma_Lenh_SX"].ToString(),
                            rd["Ten_San_Pham"].ToString(),
                            rd["Ten_Khach_Hang"].ToString(),
                            ngayGiao,
                            rd["Trang_Thai"].ToString(),
                            rd["id"].ToString());
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi load lệnh SX: {ex.Message}"); }
        }

        void LoadDgvThongBao()
        {
            dgvThongBao.Rows.Clear();
            var list = new List<(string Icon, string NoiDung, string ThoiGian, string Loai)>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var rd1 = new SqlCommand(@"SELECT TOP 3 Ma_Bao_Gia, Ngay_Tao FROM BAO_GIA
                        WHERE Trang_Thai=N'Đã duyệt' AND Ngay_Tao>=DATEADD(DAY,-3,GETDATE())
                        ORDER BY Ngay_Tao DESC", conn).ExecuteReader();
                    while (rd1.Read())
                        list.Add(("✅", $"Báo giá {rd1["Ma_Bao_Gia"]} đã được duyệt",
                            FormatThoiGian(Convert.ToDateTime(rd1["Ngay_Tao"])), "INFO"));
                    rd1.Close();

                    var rd2 = new SqlCommand(@"SELECT TOP 3 Ten_Nguyen_Lieu, Ton_Kho FROM NGUYEN_LIEU
                        WHERE Ton_Kho<=Ton_Kho_Toi_Thieu OR Ton_Kho<=0 ORDER BY Ton_Kho ASC", conn).ExecuteReader();
                    while (rd2.Read())
                    {
                        string tt = Convert.ToDouble(rd2["Ton_Kho"]) <= 0 ? "hết hàng" : "sắp hết";
                        list.Add(("⚠️", $"Vật tư \"{rd2["Ten_Nguyen_Lieu"]}\" {tt}, cần nhập thêm",
                            "Vừa cập nhật", "WARN"));
                    }
                    rd2.Close();

                    var rd3 = new SqlCommand(@"SELECT TOP 2 Ma_Phieu_Nhap, Ngay_Tao FROM PHIEU_NHAP_KHO
                        WHERE Trang_Thai=N'Hoàn thành' AND Ngay_Tao>=DATEADD(DAY,-3,GETDATE())
                        ORDER BY Ngay_Tao DESC", conn).ExecuteReader();
                    while (rd3.Read())
                        list.Add(("📦", $"Phiếu nhập {rd3["Ma_Phieu_Nhap"]} đã nhập kho thành công",
                            FormatThoiGian(Convert.ToDateTime(rd3["Ngay_Tao"])), "INFO"));
                    rd3.Close();

                    var rd4 = new SqlCommand(@"SELECT TOP 3 kh.Ten_Khach_Hang,
                        DATEDIFF(DAY,dbh.Ngay_Den_Han,GETDATE()) AS SoNgay, cn.Con_Lai
                        FROM CONG_NO_KHACH_HANG cn
                        JOIN KHACH_HANG kh ON kh.id=cn.id_Khach_Hang
                        JOIN DON_BAN_HANG dbh ON dbh.id_Khach_Hang=kh.id
                        WHERE cn.Con_Lai>0 AND dbh.Ngay_Den_Han IS NOT NULL
                          AND dbh.Ngay_Den_Han<GETDATE()
                        GROUP BY kh.Ten_Khach_Hang,dbh.Ngay_Den_Han,cn.Con_Lai
                        ORDER BY SoNgay DESC", conn).ExecuteReader();
                    while (rd4.Read())
                    {
                        int n = Convert.ToInt32(rd4["SoNgay"]);
                        list.Add(("🔴", $"Công nợ KH \"{rd4["Ten_Khach_Hang"]}\" đã quá hạn {n} ngày",
                            $"{n} ngày trước", "ERROR"));
                    }
                    rd4.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi load thông báo: {ex.Message}"); }

            foreach (var tb in list)
            {
                int idx = dgvThongBao.Rows.Add(tb.Icon, tb.NoiDung, tb.ThoiGian, tb.Loai);
                Color fg = tb.Loai == "ERROR" ? Color.FromArgb(211, 47, 47)
                         : tb.Loai == "WARN" ? Color.FromArgb(230, 81, 0)
                         : Color.FromArgb(30, 100, 200);
                dgvThongBao.Rows[idx].Cells["colTBNoiDung"].Style.ForeColor = fg;
                dgvThongBao.Rows[idx].Cells["colTBIcon"].Style.ForeColor = fg;
            }
        }

        private void DgvBaoGia_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvBaoGia.Columns[e.ColumnIndex].Name != "colBGTrangThai") return;
            e.PaintBackground(e.ClipBounds, true);
            string val = e.Value?.ToString() ?? "";
            Color bg, fg;
            switch (val)
            {
                case "Đã duyệt": case "Khách duyệt": bg = Color.FromArgb(220, 252, 231); fg = Color.FromArgb(21, 128, 61); break;
                case "Chờ duyệt": bg = Color.FromArgb(254, 243, 199); fg = Color.FromArgb(146, 64, 14); break;
                case "Đang sản xuất": bg = Color.FromArgb(219, 234, 254); fg = Color.FromArgb(30, 64, 175); break;
                case "Hoàn thành": bg = Color.FromArgb(209, 250, 229); fg = Color.FromArgb(6, 95, 70); break;
                case "Hủy": case "Từ chối": bg = Color.FromArgb(254, 226, 226); fg = Color.FromArgb(185, 28, 28); break;
                default: bg = Color.FromArgb(243, 244, 246); fg = Color.FromArgb(75, 85, 99); break;
            }
            int bw = Math.Min(e.CellBounds.Width - 8, 92), bh = 20;
            var rect = new Rectangle(e.CellBounds.X + (e.CellBounds.Width - bw) / 2,
                                     e.CellBounds.Y + (e.CellBounds.Height - bh) / 2, bw, bh);
            using (var b = new SolidBrush(bg)) e.Graphics.FillRectangle(b, rect);
            using (var b = new SolidBrush(fg))
            using (var f = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                e.Graphics.DrawString(val, f, b, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
            e.Handled = true;
        }

        private void DgvLenhSX_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvLenhSX.Columns[e.ColumnIndex].Name != "colSXTrangThai") return;
            e.PaintBackground(e.ClipBounds, true);
            string val = e.Value?.ToString() ?? "";
            Color bg = Color.FromArgb(219, 234, 254), fg = Color.FromArgb(30, 64, 175);
            int bw = Math.Min(e.CellBounds.Width - 8, 100), bh = 20;
            var rect = new Rectangle(e.CellBounds.X + (e.CellBounds.Width - bw) / 2,
                                     e.CellBounds.Y + (e.CellBounds.Height - bh) / 2, bw, bh);
            using (var b = new SolidBrush(bg)) e.Graphics.FillRectangle(b, rect);
            using (var b = new SolidBrush(fg))
            using (var f = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                e.Graphics.DrawString(val, f, b, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            e.Handled = true;
        }

        private int CountBaoGia() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM BAO_GIA", c).ExecuteScalar()); } } catch { return 0; } }
        private int CountBaoGiaDaDuyet() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM BAO_GIA WHERE Trang_Thai=N'Đã duyệt'", c).ExecuteScalar()); } } catch { return 0; } }
        private int CountBaoGiaDangSanXuat() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM BAO_GIA WHERE Trang_Thai=N'Đang sản xuất'", c).ExecuteScalar()); } } catch { return 0; } }
        private int CountVatTuCanNhap() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToInt32(new SqlCommand("SELECT COUNT(DISTINCT id) FROM NGUYEN_LIEU WHERE Ton_Kho<=Ton_Kho_Toi_Thieu OR Ton_Kho<=0", c).ExecuteScalar()); } } catch { return 0; } }
        private decimal GetDoanhThuThangNay() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToDecimal(new SqlCommand("SELECT ISNULL(SUM(Tong_Thanh_Toan),0) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=MONTH(GETDATE()) AND YEAR(Ngay_Ban_Hang)=YEAR(GETDATE())", c).ExecuteScalar()); } } catch { return 0; } }
        private int GetSoMatHangTonKho() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM NGUYEN_LIEU WHERE Ton_Kho>0", c).ExecuteScalar()); } } catch { return 0; } }
        private decimal GetTongNoPhaiThu() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToDecimal(new SqlCommand("SELECT ISNULL(SUM(CASE WHEN Con_Lai>0 THEN Con_Lai ELSE 0 END),0) FROM CONG_NO_KHACH_HANG", c).ExecuteScalar()); } } catch { return 0; } }
        private decimal GetTongNoPhaiTra() { try { using (var c = DatabaseHelper.GetConnection()) { c.Open(); return Convert.ToDecimal(new SqlCommand("SELECT ISNULL(SUM(Con_Lai),0) FROM CONG_NO_NCC WHERE Con_Lai>0", c).ExecuteScalar()); } } catch { return 0; } }

        string FormatTien(decimal v)
        {
            if (v >= 1_000_000_000) return $"{v / 1_000_000_000:0.#} tỷ";
            if (v >= 1_000_000) return $"{v / 1_000_000:0.#} triệu";
            return v.ToString("N0") + "đ";
        }
        string FormatThoiGian(DateTime dt)
        {
            var d = DateTime.Now - dt;
            if (d.TotalMinutes < 60) return $"{(int)d.TotalMinutes} phút trước";
            if (d.TotalHours < 24) return $"{(int)d.TotalHours} giờ trước";
            return $"{(int)d.TotalDays} ngày trước";
        }

        private void pnltinhgiamoi_Click_1(object sender, EventArgs e)
        {
            if (CurrentUser.VaiTro == "Kinh doanh" || CurrentUser.VaiTro == "Admin") OpenForm(new frmPriceCalculation());
            else MessageBox.Show("Bạn không đủ quyền truy cập chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void pnltaodonmua_Click_1(object sender, EventArgs e)
        {
            if (CurrentUser.VaiTro == "Kế toán" || CurrentUser.VaiTro == "Admin") OpenForm(new frmPurchaseOrder());
            else MessageBox.Show("Bạn không đủ quyền truy cập chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void panellenhsanxuat_Click_1(object sender, EventArgs e)
        {
            if (CurrentUser.VaiTro == "Sản xuất" || CurrentUser.VaiTro == "Admin") OpenForm(new frmProductionOrder());
            else MessageBox.Show("Bạn không đủ quyền truy cập chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void pnlbanhang_Click_1(object sender, EventArgs e)
        {
            if (CurrentUser.VaiTro == "Kinh doanh" || CurrentUser.VaiTro == "Admin") OpenForm(new frmSales());
            else MessageBox.Show("Bạn không đủ quyền truy cập chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}