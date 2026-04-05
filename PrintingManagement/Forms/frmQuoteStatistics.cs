// ═══════════════════════════════════════════════════════════════════
// ║  frmQuoteStatistics.cs - THỐNG KÊ BÁO GIÁ                     ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Hiển thị KPI tháng, biểu đồ xu hướng, biểu đồ trạng
// thái và bảng chi tiết theo khách hàng.
// Toàn bộ DB ủy thác cho QuoteStatisticsRepository.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PrintingManagement
{
    public partial class frmQuoteStatistics : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly QuoteStatisticsRepository _repo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmQuoteStatistics()
        {
            InitializeComponent();
            this.Load += frmQuoteStatistics_Load;
            this.Shown += frmQuoteStatistics_Shown;
        }

        /// <summary>
        /// Designer còn 2 lớp panel + combo/date trùng vị trí — chúng chồng lên panel3 làm cắt DateTimePicker / combo trống.
        /// </summary>
        private void HideDuplicateFilterControls()
        {
            foreach (Control c in new Control[]
                     {
                         comboBox1, comboBox2, dateTimePicker1, dateTimePicker4,
                         label1, label2, label3, label4, label5, label6,
                         pictureBox2, pictureBox3
                     })
            {
                if (c != null) c.Visible = false;
            }
        }

        /// <summary>
        /// Chỉ lọc theo tháng/năm — dùng ShowUpDown + MM/yyyy tránh chuỗi ngày dài bị cắt.
        /// </summary>
        private void ConfigureMonthPicker()
        {
            dtpMonth.Format = DateTimePickerFormat.Custom;
            dtpMonth.CustomFormat = "MM/yyyy";
            dtpMonth.ShowUpDown = true;
            dtpMonth.MinDate = new DateTime(2000, 1, 1);
        }

        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmQuoteStatistics_Load(object sender, EventArgs e)
        {
            dtpMonth.Value = DateTime.Today;
            ConfigureMonthPicker();
            HideDuplicateFilterControls();

            // Nạp tùy chọn lọc trước khi load dữ liệu
            LoadStatusOptions();
            SetupTrendChart();
            SetupStatusChart();
            SetupDetailGrid();

            // Load toàn bộ dữ liệu
            LoadStatsSummary();
            LoadTrendChart();
            LoadStatusChart();
            LoadDetailGrid();
            label12.BringToFront();
            pictureBox6.BringToFront();
            label14.BringToFront();
            pictureBox8.BringToFront();
        }

        private void frmQuoteStatistics_Shown(object sender, EventArgs e)
        {
            // Đưa panel lọc lên trên control trùng (dateTimePicker1/4…) sau khi layout xong
            panel3?.BringToFront();
            panel2?.BringToFront();
            ConfigureMonthPicker();
            chartTrend?.Refresh();
            chartStatus?.Refresh();
        }

        // ─────────────────────────────────────────────────────
        // THIẾT KẾ BIỂU ĐỒ XU HƯỚNG
        // ─────────────────────────────────────────────────────

        private void SetupTrendChart()
        {
            var chart = chartTrend;
            chart.BackColor = Color.White;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineColor = Color.FromArgb(230, 230, 230);

            if (chart.Series.Count == 0) chart.Series.Add("Số lượng báo giá");

            var series = chart.Series[0];
            series.ChartType = SeriesChartType.Column;
            series.BorderWidth = 3;
            series.Color = Color.FromArgb(80, 52, 152, 219);
            series.BorderColor = Color.FromArgb(52, 152, 219);
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.MarkerColor = Color.FromArgb(52, 152, 219);
            series.MarkerBorderColor = Color.White;
            series.MarkerBorderWidth = 2;
            series.LegendText = "Số lượng báo giá";

            var area = chart.ChartAreas[0];
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.FromArgb(200, 200, 200);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9f);
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(100, 100, 100);
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisY.LineColor = Color.FromArgb(200, 200, 200);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9f);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(100, 100, 100);
            area.AxisY.Minimum = 0;

            area.InnerPlotPosition.Auto = true;

            if (chart.Legends.Count == 0) chart.Legends.Add(new Legend());
            chart.Legends[0].Font = new Font("Segoe UI", 9f);
            chart.Legends[0].BackColor = Color.Transparent;
            chart.Legends[0].BorderColor = Color.Transparent;
            chart.Legends[0].Docking = Docking.Bottom;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ BIỂU ĐỒ TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void SetupStatusChart()
        {
            var chart = chartStatus;
            chart.BackColor = Color.White;

            if (chart.Series.Count == 0) chart.Series.Add("Trạng thái");

            var series = chart.Series[0];
            series.ChartType = SeriesChartType.Doughnut;
            series["DoughnutRadius"] = "60";
            series["PieLineColor"] = "Black";
            series["PieLabelStyle"] = "Disabled";
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX";
            series.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            var area = chart.ChartAreas[0];
            area.BackColor = Color.White;
            area.Area3DStyle.Enable3D = false;
            area.InnerPlotPosition.Auto = true;

            if (chart.Legends.Count == 0) chart.Legends.Add(new Legend());
            chart.Legends[0].Docking = Docking.Bottom;
            chart.Legends[0].Alignment = StringAlignment.Center;
            chart.Legends[0].Font = new Font("Segoe UI", 9f);
            chart.Legends[0].BackColor = Color.Transparent;
            chart.Legends[0].BorderColor = Color.Transparent;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW CHI TIẾT
        // ─────────────────────────────────────────────────────

        private void SetupDetailGrid()
        {
            var dgv = dgvDetails;

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

            // ── THÊM CÁC CỘT ──
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                Width = 55,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCustomer",
                HeaderText = "Khách hàng",
                Width = 230,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 0, 0, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQuoteCount",
                HeaderText = "Số lượng BG",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colApproved",
                HeaderText = "Đã duyệt",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPending",
                HeaderText = "Chờ duyệt",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCancelled",
                HeaderText = "Hủy",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalValue",
                HeaderText = "Tổng giá trị",
                Width = 185,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 12, 0)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCloseRate",
                HeaderText = "Tỷ lệ chốt",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Đăng ký sự kiện vẽ badge
            dgvDetails.CellPainting += dgvDetails_CellPainting;
        }


        // ─────────────────────────────────────────────────────
        // VẼ BADGE TRÒN VÀ MÀU CHỮ TỶ LỆ
        // ─────────────────────────────────────────────────────

        private void dgvDetails_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            string colName = dgvDetails.Columns[e.ColumnIndex].Name;

            // ── BADGE TRÒN MÀU CHO CÁC CỘT SỐ LƯỢNG ──
            if (colName == "colApproved" || colName == "colPending" || colName == "colCancelled")
            {
                if (!int.TryParse(e.Value.ToString(), out int count) || count == 0) return;

                Color badgeBg, badgeFg;
                if (colName == "colApproved") { badgeBg = Color.FromArgb(209, 250, 229); badgeFg = Color.FromArgb(22, 163, 74); }
                else if (colName == "colPending") { badgeBg = Color.FromArgb(254, 243, 199); badgeFg = Color.FromArgb(161, 98, 7); }
                else { badgeBg = Color.FromArgb(254, 226, 226); badgeFg = Color.FromArgb(220, 38, 38); }

                e.PaintBackground(e.CellBounds, true);

                int badgeSize = 28;
                int x = e.CellBounds.X + (e.CellBounds.Width - badgeSize) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - badgeSize) / 2;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(badgeBg))
                    e.Graphics.FillEllipse(brush, x, y, badgeSize, badgeSize);

                using (var font = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (var brush = new SolidBrush(badgeFg))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(count.ToString(), font, brush,
                        new RectangleF(x, y, badgeSize, badgeSize), sf);
                }

                e.Handled = true;
                return;
            }

            // ── MÀU CHỮ TỶ LỆ CHỐT ──
            if (colName == "colCloseRate")
            {
                e.PaintBackground(e.CellBounds, true);

                double.TryParse(e.Value.ToString().Replace("%", ""), out double rate);

                Color textColor = rate >= 70 ? Color.FromArgb(22, 163, 74)
                                : rate >= 40 ? Color.FromArgb(161, 98, 7)
                                : Color.FromArgb(220, 38, 38);

                using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var brush = new SolidBrush(textColor))
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


        // ─────────────────────────────────────────────────────
        // LOAD KPI TỔNG HỢP
        // ─────────────────────────────────────────────────────

        private void LoadStatsSummary()
        {
            try
            {
                // Toàn bộ SQL nằm trong QuoteStatisticsRepository
                var dt = _repo.GetStatsSummary(dtpMonth.Value.Month, dtpMonth.Value.Year);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    lblTotalQuotes.Text = Convert.ToInt32(row["TotalQuotes"]).ToString("N0");
                    lblApproved.Text = Convert.ToInt32(row["Approved"]).ToString("N0");
                    lblPending.Text = Convert.ToInt32(row["Pending"]).ToString("N0");
                    lblCancelled.Text = Convert.ToInt32(row["Cancelled"]).ToString("N0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // LOAD BIỂU ĐỒ XU HƯỚNG
        // ─────────────────────────────────────────────────────

        private void LoadTrendChart()
        {
            chartTrend.Series[0].Points.Clear();
            chartTrend.Titles.Clear();

            try
            {
                // Toàn bộ SQL nằm trong QuoteStatisticsRepository
                var dt = _repo.GetTrendByDay(dtpMonth.Value.Month, dtpMonth.Value.Year);

                if (dt.Rows.Count == 0)
                {
                    chartTrend.Titles.Add("Không có báo giá trong tháng đã chọn");
                    chartTrend.Refresh();
                    return;
                }

                string dayCol = dt.Columns.Cast<DataColumn>()
                    .FirstOrDefault(c => c.ColumnName.Equals("DayOfMonth", StringComparison.OrdinalIgnoreCase))?.ColumnName;
                string cntCol = dt.Columns.Cast<DataColumn>()
                    .FirstOrDefault(c => c.ColumnName.Equals("QuoteCount", StringComparison.OrdinalIgnoreCase))?.ColumnName;
                if (dayCol == null || cntCol == null)
                    throw new Exception("Thiếu cột DayOfMonth / QuoteCount — cập nhật sp_GetQuoteTrendByDay trên SQL Server.");

                foreach (DataRow row in dt.Rows)
                {
                    string label = row[dayCol] + "/" + dtpMonth.Value.Month.ToString("D2");
                    chartTrend.Series[0].Points.AddXY(label, Convert.ToDouble(row[cntCol]));
                }

                chartTrend.Series[0].IsValueShownAsLabel = chartTrend.Series[0].Points.Count <= 15;
                chartTrend.ChartAreas[0].AxisY.Maximum = double.NaN;
                chartTrend.ChartAreas[0].RecalculateAxesScale();
                chartTrend.Invalidate();
                chartTrend.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // LOAD BIỂU ĐỒ TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void LoadStatusChart()
        {
            chartStatus.Series[0].Points.Clear();

            try
            {
                // Toàn bộ SQL nằm trong QuoteStatisticsRepository
                var dt = _repo.GetStatusGroups(dtpMonth.Value.Month, dtpMonth.Value.Year);

                if (dt.Rows.Count == 0)
                {
                    chartStatus.Series[0].Points.AddXY("(Chưa có)", 1);
                    chartStatus.Refresh();
                    return;
                }

                string grpCol = dt.Columns.Cast<DataColumn>()
                    .FirstOrDefault(c => c.ColumnName.Equals("StatusGroup", StringComparison.OrdinalIgnoreCase))?.ColumnName;
                string cntCol = dt.Columns.Cast<DataColumn>()
                    .FirstOrDefault(c => c.ColumnName.Equals("QuoteCount", StringComparison.OrdinalIgnoreCase))?.ColumnName;
                if (grpCol == null || cntCol == null)
                    throw new Exception("Thiếu cột StatusGroup / QuoteCount — cập nhật sp_GetQuoteStatusGroups (@Month, @Year) trên SQL Server.");

                foreach (DataRow row in dt.Rows)
                {
                    string group = row[grpCol].ToString();
                    int quoteCount = Convert.ToInt32(row[cntCol]);
                    int idx = chartStatus.Series[0].Points.AddXY(group, quoteCount);

                    // Tô màu theo nhóm trạng thái
                    switch (group)
                    {
                        case "Đã duyệt": chartStatus.Series[0].Points[idx].Color = Color.FromArgb(52, 152, 219); break;
                        case "Chờ duyệt": chartStatus.Series[0].Points[idx].Color = Color.FromArgb(40, 180, 99); break;
                        case "Huỷ": chartStatus.Series[0].Points[idx].Color = Color.FromArgb(231, 76, 60); break;
                        default: chartStatus.Series[0].Points[idx].Color = Color.FromArgb(149, 165, 166); break;
                    }
                }

                chartStatus.ChartAreas[0].RecalculateAxesScale();
                chartStatus.Invalidate();
                chartStatus.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // LOAD BẢNG CHI TIẾT THEO KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void LoadDetailGrid(string keyword = "")
        {
            dgvDetails.Rows.Clear();

            try
            {
                // Toàn bộ SQL nằm trong QuoteStatisticsRepository
                var dt = _repo.GetDetailByCustomer(keyword, dtpMonth.Value.Month, dtpMonth.Value.Year);

                int rowIndex = 1;
                int sumQuotes = 0;
                int sumApproved = 0;
                int sumPending = 0;
                int sumCancelled = 0;
                double sumTotal = 0;

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string customerName = row["Ten_Khach_Hang"].ToString();
                    int quoteCount = Convert.ToInt32(row["QuoteCount"]);
                    int approved = Convert.ToInt32(row["Approved"]);
                    int pending = Convert.ToInt32(row["Pending"]);
                    int cancelled = Convert.ToInt32(row["Cancelled"]);
                    double totalValue = Convert.ToDouble(row["TotalValue"]);
                    double closeRate = quoteCount > 0 ? (double)approved / quoteCount * 100.0 : 0;

                    int idx = dgvDetails.Rows.Add(
                        rowIndex, customerName, quoteCount,
                        approved, pending, cancelled,
                        FormatCurrency(totalValue),
                        closeRate.ToString("0.#") + "%");

                    // Màu nền xen kẽ
                    dgvDetails.Rows[idx].DefaultCellStyle.BackColor =
                        rowIndex % 2 == 0 ? Color.FromArgb(250, 252, 255) : Color.White;

                    sumQuotes += quoteCount;
                    sumApproved += approved;
                    sumPending += pending;
                    sumCancelled += cancelled;
                    sumTotal += totalValue;
                    rowIndex++;
                }

                // Dòng tổng cộng cuối bảng
                double totalCloseRate = sumQuotes > 0 ? (double)sumApproved / sumQuotes * 100.0 : 0;
                int totalRowIdx = dgvDetails.Rows.Add(
                    "", "TỔNG CỘNG", sumQuotes,
                    sumApproved, sumPending, sumCancelled,
                    FormatCurrency(sumTotal),
                    totalCloseRate.ToString("0.#") + "%");

                var totalStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    BackColor = Color.FromArgb(245, 247, 250),
                    ForeColor = Color.FromArgb(30, 30, 30)
                };
                dgvDetails.Rows[totalRowIdx].DefaultCellStyle = totalStyle;
                dgvDetails.Rows[totalRowIdx].Cells["colTotalValue"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                dgvDetails.Rows[totalRowIdx].Cells["colCloseRate"].Style.ForeColor = Color.FromArgb(22, 163, 74);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // FORMAT SỐ TIỀN
        // ─────────────────────────────────────────────────────

        private string FormatCurrency(double value)
        {
            if (value >= 1_000_000_000)
                return (value / 1_000_000_000.0).ToString("N3").TrimEnd('0').TrimEnd('.') + " tỷđ";
            return value.ToString("#,##0") + "đ";
        }


        // ─────────────────────────────────────────────────────
        // NẠP TÙY CHỌN LỌC TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void LoadStatusOptions()
        {
            cboStatus.Items.Clear();
            cboStatus.Items.Add("Tất cả");
            cboStatus.Items.Add("Chờ duyệt");
            cboStatus.Items.Add("Đã duyệt");
            cboStatus.Items.Add("Huỷ");
            cboStatus.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LỌC DỮ LIỆU
        // ─────────────────────────────────────────────────────

        private void btnFilter_Click(object sender, EventArgs e)
        {
            // Reload toàn bộ theo tháng + từ khóa mới
            LoadStatsSummary();
            LoadTrendChart();
            LoadStatusChart();
            LoadDetailGrid(txtCustomerSearch.Text.Trim());
            label12.BringToFront();
            pictureBox6.BringToFront();
            label14.BringToFront();
            pictureBox8.BringToFront();
        }


        // ─────────────────────────────────────────────────────
        // TÌM KIẾM KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void txtCustomerSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // Nhấn Enter để tìm kiếm ngay
            if (e.KeyCode == Keys.Enter)
            {
                LoadDetailGrid(txtCustomerSearch.Text.Trim());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtCustomerSearch_TextChanged(object sender, EventArgs e)
        {
            // Xóa text thì tải lại toàn bộ
            if (string.IsNullOrWhiteSpace(txtCustomerSearch.Text))
                LoadDetailGrid();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN PAINT GIỮ NGUYÊN
        // ─────────────────────────────────────────────────────

        private void pnlTop_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}