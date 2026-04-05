// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmSummaryReport.cs - BÁO CÁO TỔNG HỢP                           ║
// ║  Dashboard hiển thị tổng quan hoạt động kinh doanh                 ║
// ╚══════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSummaryReport : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly SummaryReportRepository _repo = new();

        // Kích thước thiết kế gốc để tính tỷ lệ scale
        private const float BASE_WIDTH = 1280f;
        private const float BASE_HEIGHT = 720f;

        // Lưu font gốc của từng control để scale chính xác
        private readonly Dictionary<Control, float> _baseFontSizes = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmSummaryReport()
        {
            InitializeComponent();
            this.Load += frmSummaryReport_Load;
            this.Resize += frmSummaryReport_Resize;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmSummaryReport_Load(object sender, EventArgs e)
        {
            // Ghi nhớ font gốc trước khi scale lần đầu
            SnapBaseFonts(this.Controls);

            SetupGrid();
            LoadSummaryData();

            // Scale ngay theo kích thước hiện tại
            ApplyScale();
        }


        // ─────────────────────────────────────────────────────
        // SCALE ENGINE — GIỮ NGUYÊN LOGIC, SỬA TÊN BIẾN
        // ─────────────────────────────────────────────────────

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
            float ratio = Math.Max(Math.Min(ratioW, ratioH), 0.5f);

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

                if (c.HasChildren)
                    ScaleFonts(c.Controls, ratio);
            }
        }

        private void frmSummaryReport_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            ApplyScale();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW TỔNG HỢP
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvSummary;
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
            dgv.Dock = DockStyle.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 45;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

            var styleRight = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMetric",
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
            { Name = "colCurrentMonth", HeaderText = "Tháng này", FillWeight = 25, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPrevMonth", HeaderText = "Tháng trước", FillWeight = 25, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colComparison",
                HeaderText = "So sánh",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });

            // Màu nền xen kẽ
            dgv.RowPostPaint += (s, ev) =>
            {
                var row = dgv.Rows[ev.RowIndex];
                if (!row.Selected)
                    row.DefaultCellStyle.BackColor = ev.RowIndex % 2 == 0
                        ? Color.White
                        : Color.FromArgb(248, 250, 255);
            };
        }


        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU DASHBOARD
        // ─────────────────────────────────────────────────────

        private void LoadSummaryData()
        {
            try
            {
                // Toàn bộ SQL nằm trong SummaryReportRepository — 1 lần gọi DB duy nhất
                SummaryReportData data = _repo.GetSummaryData();

                double profitThisMonth = data.RevenueThisMonth - data.CostThisMonth;
                double profitPrevMonth = data.RevenuePrevMonth - data.CostPrevMonth;
                double profitMargin = data.RevenueThisMonth > 0
                    ? (profitThisMonth / data.RevenueThisMonth) * 100 : 0;

                // Cập nhật 4 thẻ KPI
                lblRevenueValue.Text = FormatCurrency(data.RevenueThisMonth);
                lblRevenueValue.ForeColor = Color.FromArgb(76, 175, 80);

                lblCostValue.Text = FormatCurrency(data.CostThisMonth);
                lblCostValue.ForeColor = Color.FromArgb(244, 67, 54);

                lblProfitValue.Text = FormatCurrency(profitThisMonth);
                lblProfitValue.ForeColor = Color.FromArgb(33, 150, 243);

                lblMarginValue.Text = profitMargin.ToString("N1") + "%";
                lblMarginValue.ForeColor = Color.FromArgb(156, 39, 176);

                // Nạp grid tổng hợp
                PopulateSummaryGrid(data, profitThisMonth, profitPrevMonth);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // NẠP GRID TỔNG HỢP
        // ─────────────────────────────────────────────────────

        private void PopulateSummaryGrid(
            SummaryReportData data,
            double profitThis, double profitPrev)
        {
            dgvSummary.Rows.Clear();

            AddRow("Số đơn hàng",
                data.OrdersThisMonth.ToString(),
                data.OrdersPrevMonth.ToString(),
                CalcGrowthRate(data.OrdersThisMonth, data.OrdersPrevMonth));

            AddRow("Doanh thu",
                FormatNumber(data.RevenueThisMonth) + "đ",
                FormatNumber(data.RevenuePrevMonth) + "đ",
                CalcGrowthRate(data.RevenueThisMonth, data.RevenuePrevMonth));

            AddRow("Lợi nhuận",
                FormatNumber(profitThis) + "đ",
                FormatNumber(profitPrev) + "đ",
                CalcGrowthRate(profitThis, profitPrev));

            AddRow("Công nợ phải thu",
                FormatNumber(data.ReceivableDebt) + "đ",
                "-", 0);

            AddRow("Tồn kho",
                FormatNumber(data.StockValue) + "đ",
                "-", 0);
        }

        private void AddRow(string metric, string currentMonth, string prevMonth, double growthRate)
        {
            string comparison = (growthRate > 0 ? "+" : "") + growthRate.ToString("N1") + "%";
            Color compColor = growthRate >= 0
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(244, 67, 54);

            int idx = dgvSummary.Rows.Add(metric, currentMonth, prevMonth, comparison);
            dgvSummary.Rows[idx].Cells["colComparison"].Style.ForeColor = compColor;
        }


        // ─────────────────────────────────────────────────────
        // HELPER
        // ─────────────────────────────────────────────────────

        private double CalcGrowthRate(double current, double previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return ((current - previous) / previous) * 100;
        }

        private string FormatCurrency(double value)
        {
            if (value >= 1_000_000_000) return (value / 1_000_000_000).ToString("N2") + " tỷ";
            if (value >= 1_000_000) return (value / 1_000_000).ToString("N0") + " triệu";
            return value.ToString("N0") + "đ";
        }

        private string FormatNumber(double number) => number.ToString("N0");
    }
}