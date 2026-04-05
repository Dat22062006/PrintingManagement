// ═══════════════════════════════════════════════════════════════════
// ║  frmSalesReport.cs - BÁO CÁO DOANH THU                         ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Hiển thị KPI doanh thu tháng, lợi nhuận, khách hàng
// và bảng doanh thu chi tiết theo từng sản phẩm.
// Toàn bộ DB ủy thác cho SalesReportRepository.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSalesReport : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly SalesReportRepository _repo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmSalesReport()
        {
            InitializeComponent();
            this.Load += frmSalesReport_Load;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmSalesReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadRevenueData();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvRevenue;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(230, 235, 245);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 60, 120);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 110);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var styleRight = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 12, 0)
            };
            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            var styleBoldBlue = new DataGridViewCellStyle(styleRight)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            var styleGreenPct = new DataGridViewCellStyle(styleRight)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 5,
                MinimumWidth = 50,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colProduct",
                HeaderText = "Sản phẩm",
                FillWeight = 35,
                MinimumWidth = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                { Padding = new Padding(10, 0, 0, 0), Font = new Font("Segoe UI", 10f) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQuantity",
                HeaderText = "Số lượng",
                FillWeight = 15,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleRight
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRevenue",
                HeaderText = "Doanh thu",
                FillWeight = 25,
                MinimumWidth = 150,
                ReadOnly = true,
                DefaultCellStyle = styleBoldBlue
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPercent",
                HeaderText = "% tổng DT",
                FillWeight = 15,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleGreenPct
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
        // NẠP DỮ LIỆU BÁO CÁO
        // ─────────────────────────────────────────────────────

        private void LoadRevenueData()
        {
            try
            {
                // ── KPI TỔNG HỢP ──
                // Toàn bộ SQL nằm trong SalesReportRepository
                var dtSummary = _repo.GetSummary();

                decimal totalRevenue = 0;
                decimal monthlyRevenue = 0;
                decimal prevMonthRevenue = 0;
                decimal profit = 0;
                int customerCount = 0;

                if (dtSummary.Rows.Count > 0)
                {
                    var row = dtSummary.Rows[0];
                    totalRevenue = Convert.ToDecimal(row["TotalRevenue"]);
                    monthlyRevenue = Convert.ToDecimal(row["MonthlyRevenue"]);
                    prevMonthRevenue = Convert.ToDecimal(row["PrevMonthRevenue"]);
                    profit = Convert.ToDecimal(row["Profit"]);
                    customerCount = Convert.ToInt32(row["CustomerCount"]);
                }

                double growthRate = prevMonthRevenue > 0
                    ? (double)((monthlyRevenue - prevMonthRevenue) / prevMonthRevenue * 100)
                    : 0;
                double profitMargin = totalRevenue > 0
                    ? (double)(profit / totalRevenue * 100) : 0;

                UpdateKpiCards(monthlyRevenue, growthRate, profit, profitMargin, customerCount);

                // ── BẢNG DOANH THU THEO SẢN PHẨM ──
                // Toàn bộ SQL nằm trong SalesReportRepository
                var dtProducts = _repo.GetRevenueByProduct();

                // Tính tổng để tính %
                decimal grandTotal = 0;
                double totalQty = 0;
                foreach (System.Data.DataRow r in dtProducts.Rows)
                {
                    grandTotal += Convert.ToDecimal(r["TotalRevenue"]);
                    totalQty += Convert.ToDouble(r["TotalQty"]);
                }

                dgvRevenue.Rows.Clear();
                int rowIndex = 1;

                foreach (System.Data.DataRow dataRow in dtProducts.Rows)
                {
                    double qty = Convert.ToDouble(dataRow["TotalQty"]);
                    decimal revenue = Convert.ToDecimal(dataRow["TotalRevenue"]);
                    double percent = grandTotal > 0 ? (double)(revenue / grandTotal * 100) : 0;

                    int idx = dgvRevenue.Rows.Add();
                    var row = dgvRevenue.Rows[idx];
                    row.Cells["colSTT"].Value = rowIndex++;
                    row.Cells["colProduct"].Value = dataRow["ProductName"].ToString();
                    row.Cells["colQuantity"].Value = qty.ToString("N0") + " cái";
                    row.Cells["colRevenue"].Value = revenue.ToString("#,##0") + "đ";
                    row.Cells["colPercent"].Value = percent.ToString("0.0") + "%";
                }

                // Dòng TỔNG CỘNG
                if (dtProducts.Rows.Count > 0)
                {
                    int totalRowIdx = dgvRevenue.Rows.Add();
                    var totalRow = dgvRevenue.Rows[totalRowIdx];
                    totalRow.Cells["colSTT"].Value = "";
                    totalRow.Cells["colProduct"].Value = "TỔNG";
                    totalRow.Cells["colQuantity"].Value = totalQty.ToString("N0");
                    totalRow.Cells["colRevenue"].Value = grandTotal.ToString("#,##0") + "đ";
                    totalRow.Cells["colPercent"].Value = "100%";

                    foreach (DataGridViewCell cell in totalRow.Cells)
                    {
                        cell.Style = new DataGridViewCellStyle
                        {
                            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                            BackColor = Color.FromArgb(235, 245, 255),
                            ForeColor = Color.FromArgb(30, 60, 120),
                            Alignment = cell.OwningColumn.Name == "colProduct"
                                ? DataGridViewContentAlignment.MiddleLeft
                                : DataGridViewContentAlignment.MiddleRight,
                            Padding = cell.OwningColumn.Name == "colProduct"
                                ? new Padding(10, 0, 0, 0)
                                : new Padding(0, 0, 12, 0)
                        };
                    }
                    totalRow.Cells["colPercent"].Style.ForeColor = Color.FromArgb(22, 163, 74);
                    totalRow.Cells["colRevenue"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT THẺ KPI
        // ─────────────────────────────────────────────────────

        private void UpdateKpiCards(
            decimal monthlyRevenue, double growthRate,
            decimal profit, double profitMargin, int customerCount)
        {
            // Thẻ doanh thu tháng
            lblMonthlyRevenue.Text = FormatCurrency(monthlyRevenue);
            string sign = growthRate >= 0 ? "+" : "";
            lblGrowthRate.Text = sign + growthRate.ToString("0.#") + "% so tháng trước";
            lblGrowthRate.ForeColor = growthRate >= 0
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(220, 38, 38);

            // Thẻ lợi nhuận
            lblProfit.Text = FormatCurrency(profit);
            lblProfitMargin.Text = "Biên lợi nhuận " + profitMargin.ToString("0.#") + "%";

            // Thẻ mục tiêu
            decimal target = 3_000_000_000m;
            double targetPercent = target > 0
                ? Math.Min((double)(monthlyRevenue / target * 100), 100) : 0;
            decimal remaining = Math.Max(target - monthlyRevenue, 0);

            lblTargetPercent.Text = targetPercent.ToString("0.#") + "%";
            lblTargetRemaining.Text = "Còn " + FormatCurrency(remaining) + " để đạt";

            // Thẻ khách hàng
            lblCustomerCount.Text = customerCount.ToString();
            lblCustomerSub.Text = "Đã giao dịch";
        }


        // ─────────────────────────────────────────────────────
        // FORMAT SỐ TIỀN
        // ─────────────────────────────────────────────────────

        private string FormatCurrency(decimal value)
        {
            if (value >= 1_000_000_000m) return (value / 1_000_000_000m).ToString("0.##") + " tỷ";
            if (value >= 1_000_000m) return (value / 1_000_000m).ToString("0.##") + " triệu";
            return value.ToString("#,##0") + "đ";
        }
    }
}