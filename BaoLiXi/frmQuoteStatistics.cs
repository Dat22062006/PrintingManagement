using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PrintingManagement
{
    public partial class frmQuoteStatistics : Form
    {
        public frmQuoteStatistics()
        {
            InitializeComponent();
            this.Load += frmQuoteStatistics_Load;
        }

        private void frmQuoteStatistics_Load(object sender, EventArgs e)
        {
            dtpThang.Value = DateTime.Today;

            LoadTrangThai();
            SetupChartXuHuong();
            SetupChartTrangThai();
            SetupDgvChiTiet();

            LoadThongKe();
            LoadChartXuHuong();
            LoadChartTrangThai();
            LoadDgvChiTiet();
        }

        // =====================================================================
        // SETUP CHART XU HƯỚNG
        // =====================================================================
        void SetupChartXuHuong()
        {
            var chart = chartXuHuong;
            chart.BackColor = Color.White;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineColor = Color.FromArgb(230, 230, 230);

            if (chart.Series.Count == 0) chart.Series.Add("Số lượng báo giá");
            var s = chart.Series[0];
            s.ChartType = SeriesChartType.Area;
            s.BorderWidth = 3;
            s.Color = Color.FromArgb(80, 52, 152, 219);
            s.BorderColor = Color.FromArgb(52, 152, 219);
            s.MarkerStyle = MarkerStyle.Circle;
            s.MarkerSize = 8;
            s.MarkerColor = Color.FromArgb(52, 152, 219);
            s.MarkerBorderColor = Color.White;
            s.MarkerBorderWidth = 2;
            s.LegendText = "Số lượng báo giá";

            var area = chart.ChartAreas[0];
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.FromArgb(200, 200, 200);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9f);
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(100, 100, 100);
            area.AxisX.Interval = 5;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisY.LineColor = Color.FromArgb(200, 200, 200);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9f);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(100, 100, 100);
            area.AxisY.Minimum = 0;

            if (chart.Legends.Count == 0) chart.Legends.Add(new Legend());
            chart.Legends[0].Font = new Font("Segoe UI", 9f);
            chart.Legends[0].BackColor = Color.Transparent;
            chart.Legends[0].BorderColor = Color.Transparent;
            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        // =====================================================================
        // SETUP CHART TRẠNG THÁI
        // =====================================================================
        void SetupChartTrangThai()
        {
            var chart = chartTrangThai;
            chart.BackColor = Color.White;

            if (chart.Series.Count == 0) chart.Series.Add("Trạng thái");
            var s = chart.Series[0];
            s.ChartType = SeriesChartType.Doughnut;
            s["DoughnutRadius"] = "35";
            s["PieLineColor"] = "Black";
            s["PieLabelStyle"] = "Outside";
            s.Label = "#PERCENT{P0}";
            s.LegendText = "#VALX";
            s.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            var area = chart.ChartAreas[0];
            area.BackColor = Color.White;
            area.Area3DStyle.Enable3D = false;

            if (chart.Legends.Count == 0) chart.Legends.Add(new Legend());
            chart.Legends[0].Docking = Docking.Bottom;
            chart.Legends[0].Alignment = StringAlignment.Center;
            chart.Legends[0].Font = new Font("Segoe UI", 9f);
            chart.Legends[0].BackColor = Color.Transparent;
            chart.Legends[0].BorderColor = Color.Transparent;
        }

        // =====================================================================
        // ⭐ SETUP DATAGRIDVIEW CHI TIẾT
        // =====================================================================
        void SetupDgvChiTiet()
        {
            var dgv = dgvChiTiet;

            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.GridColor = Color.White;
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 40;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 245, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersHeight = 42;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ScrollBars = ScrollBars.Vertical;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // STT
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                Width = 55,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Khách hàng
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colKhachHang",
                HeaderText = "Khách hàng",
                Width = 230,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 0, 0, 0)
                }
            });

            // Số lượng BG
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoBG",
                HeaderText = "Số lượng BG",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Đã duyệt
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDaDuyet",
                HeaderText = "Đã duyệt",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Chờ duyệt
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colChoDuyet",
                HeaderText = "Chờ duyệt",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Hủy
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHuy",
                HeaderText = "Hủy",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Tổng giá trị
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongGia",
                HeaderText = "Tổng giá trị",
                Width = 185,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 12, 0)
                }
            });

            // Tỷ lệ chốt
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTyLe",
                HeaderText = "Tỷ lệ chốt",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgv.CellPainting += Dgv_CellPainting;
        }

        // =====================================================================
        // ⭐ VẼ BADGE TRÒN MÀU + MÀU CHỮ TỶ LỆ
        // =====================================================================
        private void Dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            string colName = dgvChiTiet.Columns[e.ColumnIndex].Name;

            // Badge tròn cho 3 cột số lượng
            if (colName == "colDaDuyet" || colName == "colChoDuyet" || colName == "colHuy")
            {
                if (!int.TryParse(e.Value.ToString(), out int val) || val == 0)
                    return; // vẽ mặc định nếu = 0

                Color badgeBg, badgeFg;
                if (colName == "colDaDuyet") { badgeBg = Color.FromArgb(209, 250, 229); badgeFg = Color.FromArgb(22, 163, 74); }
                else if (colName == "colChoDuyet") { badgeBg = Color.FromArgb(254, 243, 199); badgeFg = Color.FromArgb(161, 98, 7); }
                else { badgeBg = Color.FromArgb(254, 226, 226); badgeFg = Color.FromArgb(220, 38, 38); }

                e.PaintBackground(e.CellBounds, true);

                int size = 28;
                int x = e.CellBounds.X + (e.CellBounds.Width - size) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - size) / 2;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(badgeBg))
                    e.Graphics.FillEllipse(brush, x, y, size, size);

                using (var font = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (var brush = new SolidBrush(badgeFg))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(val.ToString(), font, brush,
                        new RectangleF(x, y, size, size), sf);
                }
                e.Handled = true;
                return;
            }

            // Màu chữ tỷ lệ chốt
            if (colName == "colTyLe")
            {
                e.PaintBackground(e.CellBounds, true);
                double rate = 0;
                double.TryParse(e.Value.ToString().Replace("%", ""), out rate);

                Color fgColor = rate >= 70
                    ? Color.FromArgb(22, 163, 74)
                    : rate >= 40
                        ? Color.FromArgb(161, 98, 7)
                        : Color.FromArgb(220, 38, 38);

                using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var brush = new SolidBrush(fgColor))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(e.Value.ToString(), font, brush, e.CellBounds, sf);
                }
                e.Handled = true;
            }
        }

        // =====================================================================
        // ⭐ LOAD DATAGRIDVIEW TỪ DATABASE
        // =====================================================================
        void LoadDgvChiTiet(string search = "")
        {
            var dgv = dgvChiTiet;
            dgv.Rows.Clear();

            string sql = @"
SELECT
    KH.Ten_Khach_Hang,
    COUNT(DISTINCT BG.id)  AS SoBG,
    SUM(CASE WHEN BG.Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký')
             THEN 1 ELSE 0 END) AS DaDuyet,
    SUM(CASE WHEN BG.Trang_Thai = N'Chờ duyệt' THEN 1 ELSE 0 END) AS ChoDuyet,
    SUM(CASE WHEN BG.Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN 1 ELSE 0 END) AS DaHuy,
    ISNULL(SUM(ct.MaxGia), 0) AS TongGia
FROM BAO_GIA BG
JOIN KHACH_HANG KH ON BG.id_Khach_Hang = KH.id
LEFT JOIN (
    SELECT id_Bao_Gia, MAX(Tong_Gia_Bao_Khach) AS MaxGia
    FROM CHI_TIET_BAO_GIA GROUP BY id_Bao_Gia
) ct ON ct.id_Bao_Gia = BG.id
GROUP BY KH.Ten_Khach_Hang
HAVING (@search = '' OR KH.Ten_Khach_Hang LIKE @like)
ORDER BY TongGia DESC";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd2 = new SqlCommand(sql, conn);
                    cmd2.Parameters.AddWithValue("@search", search);
                    cmd2.Parameters.AddWithValue("@like", "%" + search + "%");
                    SqlDataReader rd = cmd2.ExecuteReader();

                    int stt = 1;
                    int tSoBG = 0, tDaDuyet = 0, tChoDuyet = 0, tHuy = 0;
                    double tTongGia = 0;

                    while (rd.Read())
                    {
                        string ten = rd["Ten_Khach_Hang"].ToString();
                        int soBG = Convert.ToInt32(rd["SoBG"]);
                        int daDuyet = Convert.ToInt32(rd["DaDuyet"]);
                        int choDuyet = Convert.ToInt32(rd["ChoDuyet"]);
                        int daHuy = Convert.ToInt32(rd["DaHuy"]);
                        double tong = Convert.ToDouble(rd["TongGia"]);
                        double tyLe = soBG > 0 ? (double)daDuyet / soBG * 100.0 : 0;

                        int rowIdx = dgv.Rows.Add(stt, ten, soBG, daDuyet, choDuyet, daHuy,
                            FormatTien(tong), tyLe.ToString("0.#") + "%");

                        dgv.Rows[rowIdx].DefaultCellStyle.BackColor =
                            stt % 2 == 0 ? Color.FromArgb(250, 252, 255) : Color.White;

                        tSoBG += soBG; tDaDuyet += daDuyet;
                        tChoDuyet += choDuyet; tHuy += daHuy; tTongGia += tong;
                        stt++;
                    }
                    rd.Close();

                    // Dòng TỔNG CỘNG
                    double tongTyLe = tSoBG > 0 ? (double)tDaDuyet / tSoBG * 100.0 : 0;
                    int totalRow = dgv.Rows.Add("", "TỔNG CỘNG", tSoBG, tDaDuyet, tChoDuyet, tHuy,
                        FormatTien(tTongGia), tongTyLe.ToString("0.#") + "%");

                    var totalStyle = new DataGridViewCellStyle
                    {
                        Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                        BackColor = Color.FromArgb(245, 247, 250),
                        ForeColor = Color.FromArgb(30, 30, 30)
                    };
                    dgv.Rows[totalRow].DefaultCellStyle = totalStyle;
                    dgv.Rows[totalRow].Cells["colTongGia"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                    dgv.Rows[totalRow].Cells["colTyLe"].Style.ForeColor = Color.FromArgb(22, 163, 74);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi bảng chi tiết:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string FormatTien(double tien)
        {
            if (tien >= 1_000_000_000)
                return (tien / 1_000_000_000.0).ToString("N3").TrimEnd('0').TrimEnd('.') + " tỷđ";
            if (tien >= 1_000_000)
                return tien.ToString("#,##0") + "đ";
            return tien.ToString("#,##0") + "đ";
        }

        // =====================================================================
        // LOAD THỐNG KÊ + CHART
        // =====================================================================
        void LoadThongKe()
        {
            int thang = dtpThang.Value.Month;
            int nam = dtpThang.Value.Year;

            string sql = @"
SELECT
    COUNT(*) AS TongBaoGia,
    SUM(CASE WHEN Trang_Thai = N'Chờ duyệt' THEN 1 ELSE 0 END) AS ChoDuyet,
    SUM(CASE WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành') THEN 1 ELSE 0 END) AS DaDuyet,
    SUM(CASE WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN 1 ELSE 0 END) AS DaHuy
FROM BAO_GIA
WHERE MONTH(Ngay_Bao_Gia) = @thang AND YEAR(Ngay_Bao_Gia) = @nam";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@thang", thang);
                    cmd.Parameters.AddWithValue("@nam", nam);
                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        lblTongBaoGia.Text = Convert.ToInt32(rd["TongBaoGia"]).ToString("N0");
                        lblDaDuyet.Text = Convert.ToInt32(rd["DaDuyet"]).ToString("N0");
                        lblChoDuyet.Text = Convert.ToInt32(rd["ChoDuyet"]).ToString("N0");
                        lblDaHuy.Text = Convert.ToInt32(rd["DaHuy"]).ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi LoadThongKe:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadChartXuHuong()
        {
            chartXuHuong.Series[0].Points.Clear();
            string sql = @"
SELECT DAY(Ngay_Bao_Gia) AS Ngay, COUNT(*) AS SoLuong
FROM BAO_GIA
WHERE MONTH(Ngay_Bao_Gia) = @thang AND YEAR(Ngay_Bao_Gia) = @nam
GROUP BY DAY(Ngay_Bao_Gia) ORDER BY Ngay";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@thang", dtpThang.Value.Month);
                    cmd.Parameters.AddWithValue("@nam", dtpThang.Value.Year);
                    SqlDataReader rd = cmd.ExecuteReader();
                    bool hasData = false;
                    while (rd.Read())
                    {
                        hasData = true;
                        string label = rd["Ngay"] + "/" + dtpThang.Value.Month.ToString("D2");
                        chartXuHuong.Series[0].Points.AddXY(label, Convert.ToDouble(rd["SoLuong"]));
                    }
                    if (!hasData) chartXuHuong.Series[0].Points.AddXY("(Chưa có)", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi ChartXuHuong:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadChartTrangThai()
        {
            chartTrangThai.Series[0].Points.Clear();
            string sql = @"
SELECT
    CASE
        WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký') THEN N'Đã duyệt'
        WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN N'Huỷ'
        ELSE Trang_Thai
    END AS Nhom,
    COUNT(*) AS SoLuong
FROM BAO_GIA
GROUP BY
    CASE
        WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký') THEN N'Đã duyệt'
        WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN N'Huỷ'
        ELSE Trang_Thai
    END
ORDER BY SoLuong DESC";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlDataReader rd = new SqlCommand(sql, conn).ExecuteReader();
                    bool hasData = false;
                    while (rd.Read())
                    {
                        hasData = true;
                        string nhom = rd["Nhom"].ToString();
                        int soLuong = Convert.ToInt32(rd["SoLuong"]);
                        int idx = chartTrangThai.Series[0].Points.AddXY(nhom, soLuong);
                        switch (nhom)
                        {
                            case "Đã duyệt": chartTrangThai.Series[0].Points[idx].Color = Color.FromArgb(52, 152, 219); break;
                            case "Chờ duyệt": chartTrangThai.Series[0].Points[idx].Color = Color.FromArgb(40, 180, 99); break;
                            case "Huỷ": chartTrangThai.Series[0].Points[idx].Color = Color.FromArgb(231, 76, 60); break;
                            default: chartTrangThai.Series[0].Points[idx].Color = Color.FromArgb(149, 165, 166); break;
                        }
                    }
                    if (!hasData) chartTrangThai.Series[0].Points.AddXY("(Chưa có)", 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi ChartTrangThai:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================================
        // EVENTS
        // =====================================================================
        void LoadTrangThai()
        {
            cbTrangThai.Items.Clear();
            cbTrangThai.Items.Add("Tất cả");
            cbTrangThai.Items.Add("Chờ duyệt");
            cbTrangThai.Items.Add("Đã duyệt");
            cbTrangThai.Items.Add("Huỷ");
            cbTrangThai.SelectedIndex = 0;
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            LoadThongKe();
            LoadChartXuHuong();
            LoadChartTrangThai();
            LoadDgvChiTiet(txtKhachHang.Text.Trim());
        }

        // ⭐ Tìm kiếm khi nhấn Enter
        private void txtKhachHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDgvChiTiet(txtKhachHang.Text.Trim());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // ⭐ Xóa text thì load lại toàn bộ
        private void txtKhachHang_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKhachHang.Text))
                LoadDgvChiTiet();
        }

        private void label2_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void pnltop_Paint(object sender, PaintEventArgs e) { }
    }
}