// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmSummaryReport.cs - BÁO CÁO TỔNG HỢP                           ║
// ║  Dashboard hiển thị tổng quan hoạt động kinh doanh                 ║
// ╚══════════════════════════════════════════════════════════════════════╝

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSummaryReport : Form
    {
        // ── Kích thước thiết kế gốc (đo trong Designer) ──────────────
        private const float BASE_WIDTH = 1280f;
        private const float BASE_HEIGHT = 720f;

        // ── Lưu font gốc của từng control để scale chính xác ─────────
        private readonly Dictionary<Control, float> _baseFontSizes = new Dictionary<Control, float>();

        public frmSummaryReport()
        {
            InitializeComponent();
            this.Load += frmSummaryReport_Load;
            this.Resize += frmSummaryReport_Resize;
        }

        private void frmSummaryReport_Load(object sender, EventArgs e)
        {
            // Ghi nhớ font gốc trước khi scale lần nào
            SnapBaseFonts(this.Controls);

            SetupGrid();
            LoadDashboardData();

            // Scale ngay lần đầu theo kích thước hiện tại
            ApplyScale();
        }

        // ═══════════════════════════════════════════════════════════════
        // SCALE ENGINE
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Đệ quy lưu lại font size gốc của mọi control.</summary>
        private void SnapBaseFonts(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (!_baseFontSizes.ContainsKey(c))
                    _baseFontSizes[c] = c.Font.Size;

                if (c.HasChildren)
                    SnapBaseFonts(c.Controls);
            }
        }

        /// <summary>Tính tỷ lệ scale rồi áp dụng lên toàn form.</summary>
        private void ApplyScale()
        {
            float ratioW = this.ClientSize.Width / BASE_WIDTH;
            float ratioH = this.ClientSize.Height / BASE_HEIGHT;
            float ratio = Math.Min(ratioW, ratioH); // giữ tỷ lệ đều
            ratio = Math.Max(ratio, 0.5f);    // không nhỏ hơn 50%

            ScaleFonts(this.Controls, ratio);
        }

        /// <summary>Đệ quy scale font tất cả controls theo ratio.</summary>
        private void ScaleFonts(Control.ControlCollection controls, float ratio)
        {
            foreach (Control c in controls)
            {
                if (_baseFontSizes.TryGetValue(c, out float baseSize))
                {
                    float newSize = Math.Max(7f, baseSize * ratio);
                    if (Math.Abs(c.Font.Size - newSize) > 0.1f)
                        c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                }

                // Panel, GroupBox, TableLayoutPanel... → đệ quy vào trong
                if (c.HasChildren)
                    ScaleFonts(c.Controls, ratio);
            }
        }

        private void frmSummaryReport_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            ApplyScale();
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID TỔNG HỢP
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvTongHop;
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
            dgv.RowTemplate.Height = 45;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Dock = DockStyle.Fill; // luôn fill panel chứa nó

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 45;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

            var right = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colChiTieu",
                HeaderText = "Chỉ tiêu",
                FillWeight = 30,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    Padding = new Padding(10, 0, 0, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThangNay",
                HeaderText = "Tháng này",
                FillWeight = 25,
                DefaultCellStyle = right
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThangTruoc",
                HeaderText = "Tháng trước",
                FillWeight = 25,
                DefaultCellStyle = right
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoSanh",
                HeaderText = "So sánh",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });

            // Màu xen kẽ dòng
            dgv.RowPostPaint += (s, ev) =>
            {
                var row = dgv.Rows[ev.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor = ev.RowIndex % 2 == 0
                        ? Color.White
                        : Color.FromArgb(248, 250, 255);
            };
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD DỮ LIỆU DASHBOARD
        // ═══════════════════════════════════════════════════════════════
        void LoadDashboardData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    double doanhThuThangNay = GetDoanhThu(conn, DateTime.Today);
                    double doanhThuThangTruoc = GetDoanhThu(conn, DateTime.Today.AddMonths(-1));

                    double chiPhiThangNay = GetChiPhi(conn, DateTime.Today);
                    double chiPhiThangTruoc = GetChiPhi(conn, DateTime.Today.AddMonths(-1));

                    double loiNhuanThangNay = doanhThuThangNay - chiPhiThangNay;
                    double loiNhuanThangTruoc = doanhThuThangTruoc - chiPhiThangTruoc;

                    double bienLN = doanhThuThangNay > 0
                        ? (loiNhuanThangNay / doanhThuThangNay) * 100
                        : 0;

                    // ── Cập nhật 4 thẻ KPI ──────────────────────────
                    lblDoanhThuMota.Text = FormatTien(doanhThuThangNay);
                    lblDoanhThuMota.ForeColor = Color.FromArgb(76, 175, 80);

                    lblChiPhiMota.Text = FormatTien(chiPhiThangNay);
                    lblChiPhiMota.ForeColor = Color.FromArgb(244, 67, 54);

                    lblLoiNhuanMota.Text = FormatTien(loiNhuanThangNay);
                    lblLoiNhuanMota.ForeColor = Color.FromArgb(33, 150, 243);

                    lblBienLNMota.Text = bienLN.ToString("N1") + "%";
                    lblBienLNMota.ForeColor = Color.FromArgb(156, 39, 176);

                    // ── Grid tổng hợp ────────────────────────────────
                    LoadGridTongHop(conn,
                        doanhThuThangNay, doanhThuThangTruoc,
                        loiNhuanThangNay, loiNhuanThangTruoc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load dashboard:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD GRID TỔNG HỢP
        // ═══════════════════════════════════════════════════════════════
        void LoadGridTongHop(SqlConnection conn,
            double dtNay, double dtTruoc,
            double lnNay, double lnTruoc)
        {
            dgvTongHop.Rows.Clear();

            int soDonNay = GetSoDonHang(conn, DateTime.Today);
            int soDonTruoc = GetSoDonHang(conn, DateTime.Today.AddMonths(-1));

            AddRow("Số đơn hàng",
                soDonNay.ToString(), soDonTruoc.ToString(),
                TinhPhanTram(soDonNay, soDonTruoc));

            AddRow("Doanh thu",
                FormatSoNguyen(dtNay) + "đ", FormatSoNguyen(dtTruoc) + "đ",
                TinhPhanTram(dtNay, dtTruoc));

            AddRow("Lợi nhuận",
                FormatSoNguyen(lnNay) + "đ", FormatSoNguyen(lnTruoc) + "đ",
                TinhPhanTram(lnNay, lnTruoc));

            double congNo = GetCongNoPhaiThu(conn);
            AddRow("Công nợ phải thu", FormatSoNguyen(congNo) + "đ", "-", 0);

            double tonKho = GetTonKho(conn);
            AddRow("Tồn kho", FormatSoNguyen(tonKho) + "đ", "-", 0);
        }

        void AddRow(string chiTieu, string thangNay, string thangTruoc, double phanTram)
        {
            string soSanh = (phanTram > 0 ? "+" : "") + phanTram.ToString("N1") + "%";
            Color mau = phanTram >= 0
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(244, 67, 54);

            int idx = dgvTongHop.Rows.Add(chiTieu, thangNay, thangTruoc, soSanh);
            dgvTongHop.Rows[idx].Cells["colSoSanh"].Style.ForeColor = mau;
        }

        // ═══════════════════════════════════════════════════════════════
        // QUERY DATABASE
        // ═══════════════════════════════════════════════════════════════
        double GetDoanhThu(SqlConnection conn, DateTime thang)
        {
            var cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Tong_Thanh_Toan), 0)
FROM DON_BAN_HANG
WHERE MONTH(Ngay_Ban_Hang) = @thang
  AND YEAR(Ngay_Ban_Hang)  = @nam
  AND ISNULL(Trang_Thai,'') <> N'Đã hủy'", conn);
            cmd.Parameters.AddWithValue("@thang", thang.Month);
            cmd.Parameters.AddWithValue("@nam", thang.Year);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        double GetChiPhi(SqlConnection conn, DateTime thang)
        {
            var cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Tong_Tien_Truoc_Thue), 0)
FROM DON_BAN_HANG
WHERE MONTH(Ngay_Ban_Hang) = @thang
  AND YEAR(Ngay_Ban_Hang)  = @nam
  AND ISNULL(Trang_Thai,'') <> N'Đã hủy'", conn);
            cmd.Parameters.AddWithValue("@thang", thang.Month);
            cmd.Parameters.AddWithValue("@nam", thang.Year);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        int GetSoDonHang(SqlConnection conn, DateTime thang)
        {
            var cmd = new SqlCommand(@"
SELECT COUNT(*)
FROM DON_BAN_HANG
WHERE MONTH(Ngay_Ban_Hang) = @thang
  AND YEAR(Ngay_Ban_Hang)  = @nam
  AND ISNULL(Trang_Thai,'') <> N'Đã hủy'", conn);
            cmd.Parameters.AddWithValue("@thang", thang.Month);
            cmd.Parameters.AddWithValue("@nam", thang.Year);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        double GetCongNoPhaiThu(SqlConnection conn)
        {
            return Convert.ToDouble(new SqlCommand(@"
SELECT ISNULL(SUM(Tong_No - Da_Thu), 0)
FROM CONG_NO_KHACH_HANG
WHERE Tong_No > Da_Thu", conn).ExecuteScalar());
        }

        double GetTonKho(SqlConnection conn)
        {
            return Convert.ToDouble(new SqlCommand(@"
SELECT ISNULL(SUM(Ton_Kho * ISNULL(Gia_Nhap, 0)), 0)
FROM NGUYEN_LIEU", conn).ExecuteScalar());
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER
        // ═══════════════════════════════════════════════════════════════
        double TinhPhanTram(double moi, double cu)
        {
            if (cu == 0) return moi > 0 ? 100 : 0;
            return ((moi - cu) / cu) * 100;
        }

        string FormatTien(double tien)
        {
            if (tien >= 1_000_000_000) return (tien / 1_000_000_000).ToString("N2") + " tỷ";
            if (tien >= 1_000_000) return (tien / 1_000_000).ToString("N0") + " triệu";
            return tien.ToString("N0") + "đ";
        }

        string FormatSoNguyen(double so) => so.ToString("N0");
    }
}