using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSalesReport : Form
    {
        public frmSalesReport()
        {
            InitializeComponent();
            this.Load += frmSalesReport_Load;
        }

        private void frmSalesReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadData();
        }

        private void SetupGrid()
        {
            dgvDoanhThu.Columns.Clear();
            dgvDoanhThu.RowHeadersVisible = false;
            dgvDoanhThu.AllowUserToAddRows = false;
            dgvDoanhThu.AllowUserToDeleteRows = false;
            dgvDoanhThu.ReadOnly = true;
            dgvDoanhThu.MultiSelect = false;
            dgvDoanhThu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDoanhThu.BorderStyle = BorderStyle.None;
            dgvDoanhThu.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDoanhThu.GridColor = Color.FromArgb(230, 235, 245);
            dgvDoanhThu.BackgroundColor = Color.White;
            dgvDoanhThu.RowTemplate.Height = 40;
            dgvDoanhThu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDoanhThu.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgvDoanhThu.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 254);
            dgvDoanhThu.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 60, 120);

            dgvDoanhThu.EnableHeadersVisualStyles = false;
            dgvDoanhThu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDoanhThu.ColumnHeadersHeight = 44;
            dgvDoanhThu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgvDoanhThu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 110);
            dgvDoanhThu.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDoanhThu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var right = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 12, 0)
            };
            var center = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            var boldBlue = new DataGridViewCellStyle(right)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            var greenPct = new DataGridViewCellStyle(right)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };

            dgvDoanhThu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 5,
                MinimumWidth = 50,
                DefaultCellStyle = center,
                ReadOnly = true
            });
            dgvDoanhThu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSanPham",
                HeaderText = "Sản phẩm",
                FillWeight = 35,
                MinimumWidth = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Padding = new Padding(10, 0, 0, 0),
                    Font = new Font("Segoe UI", 10f)
                }
            });
            dgvDoanhThu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số lượng",
                FillWeight = 15,
                MinimumWidth = 100,
                DefaultCellStyle = right,
                ReadOnly = true
            });
            dgvDoanhThu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDoanhThu",
                HeaderText = "Doanh thu",
                FillWeight = 25,
                MinimumWidth = 150,
                DefaultCellStyle = boldBlue,
                ReadOnly = true
            });
            dgvDoanhThu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhanTram",
                HeaderText = "% tổng DT",
                FillWeight = 15,
                MinimumWidth = 100,
                DefaultCellStyle = greenPct,
                ReadOnly = true
            });

            dgvDoanhThu.RowPostPaint += (s, ev) =>
            {
                var row = dgvDoanhThu.Rows[ev.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor = ev.RowIndex % 2 == 0
                        ? Color.White
                        : Color.FromArgb(248, 250, 255);
            };
        }

        private void LoadData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 1. Tổng doanh thu toàn thời gian
                    var cmdDT = new SqlCommand(@"
SELECT ISNULL(SUM(Tong_Thanh_Toan), 0)
FROM DON_BAN_HANG
WHERE ISNULL(Trang_Thai, '') <> N'Đã hủy'", conn);
                    decimal doanhThu = Convert.ToDecimal(cmdDT.ExecuteScalar());

                    // 2. Doanh thu tháng này
                    var cmdThang = new SqlCommand(@"
SELECT ISNULL(SUM(Tong_Thanh_Toan), 0)
FROM DON_BAN_HANG
WHERE MONTH(Ngay_Ban_Hang) = MONTH(GETDATE())
  AND YEAR(Ngay_Ban_Hang)  = YEAR(GETDATE())
  AND ISNULL(Trang_Thai, '') <> N'Đã hủy'", conn);
                    decimal doanhThuThang = Convert.ToDecimal(cmdThang.ExecuteScalar());

                    // 3. Doanh thu tháng trước
                    var cmdThangTruoc = new SqlCommand(@"
SELECT ISNULL(SUM(Tong_Thanh_Toan), 0)
FROM DON_BAN_HANG
WHERE MONTH(Ngay_Ban_Hang) = MONTH(DATEADD(MONTH,-1,GETDATE()))
  AND YEAR(Ngay_Ban_Hang)  = YEAR(DATEADD(MONTH,-1,GETDATE()))
  AND ISNULL(Trang_Thai, '') <> N'Đã hủy'", conn);
                    decimal doanhThuThangTruoc = Convert.ToDecimal(cmdThangTruoc.ExecuteScalar());

                    double pctTang = doanhThuThangTruoc > 0
                        ? (double)((doanhThuThang - doanhThuThangTruoc) / doanhThuThangTruoc * 100)
                        : 0;

                    // 4. Lợi nhuận = tổng tiền VAT
                    var cmdLN = new SqlCommand(@"
SELECT ISNULL(SUM(Tien_Thue_VAT), 0)
FROM DON_BAN_HANG
WHERE ISNULL(Trang_Thai, '') <> N'Đã hủy'", conn);
                    decimal loiNhuan = Convert.ToDecimal(cmdLN.ExecuteScalar());
                    double bienLN = doanhThu > 0 ? (double)(loiNhuan / doanhThu * 100) : 0;

                    // 5. Số khách hàng
                    var cmdKH = new SqlCommand(@"
SELECT COUNT(DISTINCT id_Khach_Hang)
FROM DON_BAN_HANG
WHERE ISNULL(Trang_Thai, '') <> N'Đã hủy'
  AND id_Khach_Hang IS NOT NULL", conn);
                    int soKhach = Convert.ToInt32(cmdKH.ExecuteScalar());

                    CapNhatTheKPI(doanhThuThang, pctTang, loiNhuan, bienLN, soKhach);

                    // 6. Bảng theo sản phẩm
                    var cmdSP = new SqlCommand(@"
SELECT
    ct.Ten_San_Pham        AS SanPham,
    SUM(ct.So_Luong)       AS TongSoLuong,
    SUM(ct.Thanh_Tien)     AS TongDoanhThu
FROM CHI_TIET_DON_BAN_HANG ct
JOIN DON_BAN_HANG dbh ON dbh.id = ct.id_Don_Ban_Hang
WHERE ISNULL(dbh.Trang_Thai, '') <> N'Đã hủy'
GROUP BY ct.Ten_San_Pham
ORDER BY TongDoanhThu DESC", conn);

                    var rd = cmdSP.ExecuteReader();
                    var rows = new System.Collections.Generic.List<(string sp, double sl, decimal dt)>();
                    while (rd.Read())
                    {
                        rows.Add((
                            rd["SanPham"].ToString(),
                            Convert.ToDouble(rd["TongSoLuong"]),
                            Convert.ToDecimal(rd["TongDoanhThu"])
                        ));
                    }
                    rd.Close();

                    decimal tongDT = 0;
                    double tongSL = 0;
                    foreach (var r in rows) { tongDT += r.dt; tongSL += r.sl; }

                    dgvDoanhThu.Rows.Clear();
                    int stt = 1;
                    foreach (var r in rows)
                    {
                        double pct = tongDT > 0 ? (double)(r.dt / tongDT * 100) : 0;
                        int idx = dgvDoanhThu.Rows.Add();
                        var row = dgvDoanhThu.Rows[idx];
                        row.Cells["colSTT"].Value = stt++;
                        row.Cells["colSanPham"].Value = r.sp;
                        row.Cells["colSoLuong"].Value = r.sl.ToString("N0") + " cái";
                        row.Cells["colDoanhThu"].Value = r.dt.ToString("#,##0") + "đ";
                        row.Cells["colPhanTram"].Value = pct.ToString("0.0") + "%";
                    }

                    // Dòng TỔNG
                    if (rows.Count > 0)
                    {
                        int idxTong = dgvDoanhThu.Rows.Add();
                        var rowTong = dgvDoanhThu.Rows[idxTong];
                        rowTong.Cells["colSTT"].Value = "";
                        rowTong.Cells["colSanPham"].Value = "TỔNG";
                        rowTong.Cells["colSoLuong"].Value = tongSL.ToString("N0");
                        rowTong.Cells["colDoanhThu"].Value = tongDT.ToString("#,##0") + "đ";
                        rowTong.Cells["colPhanTram"].Value = "100%";

                        foreach (DataGridViewCell cell in rowTong.Cells)
                        {
                            cell.Style = new DataGridViewCellStyle
                            {
                                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                                BackColor = Color.FromArgb(235, 245, 255),
                                ForeColor = Color.FromArgb(30, 60, 120),
                                Alignment = cell.OwningColumn.Name == "colSanPham"
                                    ? DataGridViewContentAlignment.MiddleLeft
                                    : DataGridViewContentAlignment.MiddleRight,
                                Padding = cell.OwningColumn.Name == "colSanPham"
                                    ? new Padding(10, 0, 0, 0)
                                    : new Padding(0, 0, 12, 0)
                            };
                        }
                        rowTong.Cells["colPhanTram"].Style.ForeColor = Color.FromArgb(22, 163, 74);
                        rowTong.Cells["colDoanhThu"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu:\n" + ex.Message, "Lỗi");
            }
        }

        private void CapNhatTheKPI(decimal doanhThuThang, double pctTang,
                                   decimal loiNhuan, double bienLN, int soKhach)
        {
            lblDoanhThuThangNay.Text = FormatSoTien(doanhThuThang);
            string sign = pctTang >= 0 ? "+" : "";
            lblTangTruong.Text = sign + pctTang.ToString("0.#") + "% so tháng trước";
            lblTangTruong.ForeColor = pctTang >= 0
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(220, 38, 38);

            lblLoiNhuan.Text = FormatSoTien(loiNhuan);
            lblBienLoiNhuan.Text = "Biên lợi nhuận " + bienLN.ToString("0.#") + "%";

            decimal mucTieu = 3_000_000_000m;
            double pctMT = mucTieu > 0
                ? Math.Min((double)(doanhThuThang / mucTieu * 100), 100)
                : 0;
            decimal conLai = Math.Max(mucTieu - doanhThuThang, 0);
            lblMucTieuPhanTram.Text = pctMT.ToString("0.#") + "%";
            lblMucTieuConLai.Text = "Còn " + FormatSoTien(conLai) + " để đạt";

            lblKhachHang.Text = soKhach.ToString();
            lblKhachHangSub.Text = "Đã giao dịch";
        }

        private string FormatSoTien(decimal so)
        {
            if (so >= 1_000_000_000m) return (so / 1_000_000_000m).ToString("0.##") + " tỷ";
            if (so >= 1_000_000m) return (so / 1_000_000m).ToString("0.##") + " triệu";
            return so.ToString("#,##0") + "đ";
        }
    }
}