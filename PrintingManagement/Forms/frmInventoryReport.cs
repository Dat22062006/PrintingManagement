// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmInventoryReport.cs - BÁO CÁO KHO                               ║
// ╚══════════════════════════════════════════════════════════════════════╝

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryReport : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly InventoryReportRepository _repo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmInventoryReport()
        {
            InitializeComponent();
            this.Load += frmInventoryReport_Load;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmInventoryReport_Load(object sender, EventArgs e)
        {
            // Nạp loại báo cáo vào ComboBox
            cboReportType.Items.Clear();
            cboReportType.Items.Add("-- Chọn loại báo cáo --");
            cboReportType.Items.Add("Sổ chi tiết nhập xuất tồn");
            cboReportType.Items.Add("Tổng hợp tồn kho");
            cboReportType.Items.Add("Thẻ kho");
            cboReportType.SelectedIndex = 0;
            cboReportType.SelectedIndexChanged += cboReportType_SelectedIndexChanged;

            // Khoảng thời gian mặc định: tháng hiện tại
            dtpFromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpToDate.Value = DateTime.Today;

            // Ẩn tiêu đề ban đầu
            lblReportTitle.Visible = false;
            lblReportTitle.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblReportTitle.ForeColor = Color.FromArgb(30, 64, 175);

            SetupGrid();
        }


        // ─────────────────────────────────────────────────────
        // CHỌN LOẠI BÁO CÁO → HIỆN TIÊU ĐỀ
        // ─────────────────────────────────────────────────────

        private void cboReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboReportType.SelectedIndex;

            if (idx <= 0)
            {
                lblReportTitle.Visible = false;
                return;
            }

            string[] titles = { "", "Sổ chi tiết nhập xuất tồn", "Tổng hợp tồn kho", "Thẻ kho" };
            lblReportTitle.Text = titles[idx];
            lblReportTitle.Visible = true;

            // Reset grid khi đổi loại
            dgvReport.Rows.Clear();
            dgvReport.Columns.Clear();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID (STYLE CHUNG)
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvReport;
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
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }


        // ─────────────────────────────────────────────────────
        // HELPER — LẤY TIÊU ĐỀ VÀ TÊN FILE BÁO CÁO
        // ─────────────────────────────────────────────────────

        private string GetReportTitle() => cboReportType.SelectedIndex switch
        {
            1 => "SỔ CHI TIẾT VẬT TƯ HÀNG HÓA",
            2 => "TỔNG HỢP TỒN KHO",
            3 => "THẺ KHO",
            _ => ""
        };

        private string GetReportShortName() => cboReportType.SelectedIndex switch
        {
            1 => "SoChiTiet",
            2 => "TongHopTonKho",
            3 => "TheKho",
            _ => "BaoCaoKho"
        };


        // ─────────────────────────────────────────────────────
        // NÚT XEM BÁO CÁO
        // ─────────────────────────────────────────────────────

        private void btnView_Click(object sender, EventArgs e)
        {
            int idx = cboReportType.SelectedIndex;

            if (idx <= 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn loại báo cáo!", "Thiếu thông tin");
                return;
            }

            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("⚠️ Từ ngày phải nhỏ hơn hoặc bằng Đến ngày!", "Sai khoảng thời gian");
                return;
            }

            dgvReport.Rows.Clear();
            dgvReport.Columns.Clear();

            switch (idx)
            {
                case 1: LoadDetailLedger(fromDate, toDate); break;
                case 2: LoadInventorySummary(fromDate, toDate); break;
                case 3: LoadStockCard(fromDate, toDate); break;
            }
        }


        // ─────────────────────────────────────────────────────
        // NÚT IN / XUẤT EXCEL
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e) => ExportReportToExcel();
        private void btnExportExcel_Click(object sender, EventArgs e) => ExportReportToExcel();


        // ─────────────────────────────────────────────────────
        // XUẤT EXCEL CHUNG
        // ─────────────────────────────────────────────────────

        private void ExportReportToExcel()
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Chưa có dữ liệu để xuất Excel!", "Thông báo");
                return;
            }

            if (cboReportType.SelectedIndex <= 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn loại báo cáo trước!", "Thiếu thông tin");
                return;
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add(GetReportShortName());
                    int r = 1;

                    // Tiêu đề lớn
                    ws.Cells[r, 1].Value = GetReportTitle();
                    ws.Cells[r, 1, r, dgvReport.Columns.Count].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;

                    // Dòng khoảng thời gian
                    ws.Cells[r, 1].Value =
                        $"Kho: Nguyên Vật Liệu Giấy, từ ngày {dtpFromDate.Value:dd/MM/yyyy} đến ngày {dtpToDate.Value:dd/MM/yyyy}";
                    ws.Cells[r, 1, r, dgvReport.Columns.Count].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true;
                    r += 2;

                    // Header cột
                    for (int c = 0; c < dgvReport.Columns.Count; c++)
                    {
                        ws.Cells[r, c + 1].Value = dgvReport.Columns[c].HeaderText;
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 240, 255));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    r++;

                    // Dữ liệu
                    foreach (DataGridViewRow row in dgvReport.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int c = 0; c < dgvReport.Columns.Count; c++)
                        {
                            ws.Cells[r, c + 1].Value = row.Cells[c].Value;
                            ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            // Canh phải nếu cột align phải
                            if (dgvReport.Columns[c].DefaultCellStyle.Alignment ==
                                DataGridViewContentAlignment.MiddleRight)
                                ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        r++;
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    string filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"{GetReportShortName()}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    pkg.SaveAs(new FileInfo(filePath));

                    MessageBox.Show($"✅ Đã xuất báo cáo ra Excel:\n{filePath}", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // BÁO CÁO 1: SỔ CHI TIẾT NHẬP XUẤT TỒN
        // ─────────────────────────────────────────────────────

        private void LoadDetailLedger(DateTime fromDate, DateTime toDate)
        {
            var dgv = dgvReport;

            var rightStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0),
                Font = new Font("Segoe UI", 10f)
            };
            var boldStyle = new DataGridViewCellStyle(rightStyle)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            };
            var groupStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(235, 245, 255),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175),
                Padding = new Padding(6, 0, 6, 0)
            };
            var totalStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(220, 235, 255),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 120),
                Padding = new Padding(6, 0, 6, 0)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Ngày", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVoucher", HeaderText = "Chứng từ", FillWeight = 13 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescription", HeaderText = "Diễn giải", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockIn", HeaderText = "Nhập", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockOut", HeaderText = "Xuất", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBalance", HeaderText = "Tồn", FillWeight = 10, DefaultCellStyle = boldStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnitPrice", HeaderText = "Đơn giá", FillWeight = 12, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockValue", HeaderText = "Giá trị tồn", FillWeight = 14, DefaultCellStyle = boldStyle });

            try
            {
                // Toàn bộ SQL nằm trong InventoryReportRepository
                var dtMaterials = _repo.GetDetailLedgerMaterials(fromDate, toDate);

                foreach (System.Data.DataRow matRow in dtMaterials.Rows)
                {
                    int materialId = Convert.ToInt32(matRow["id"]);
                    string itemCode = matRow["Ma_Nguyen_Lieu"].ToString();
                    string itemName = matRow["Ten_Nguyen_Lieu"].ToString();
                    string unit = matRow["Don_Vi_Tinh"].ToString();
                    double unitPrice = Convert.ToDouble(matRow["Gia_Nhap"]);

                    // Lấy tổng nhập/xuất + tồn cuối để tính tồn đầu
                    double totalStockIn = 0, totalStockOut = 0, closingStock = 0;
                    var dtAgg = _repo.GetDetailLedgerAgg(materialId, fromDate, toDate);
                    if (dtAgg.Rows.Count > 0)
                    {
                        totalStockIn = Convert.ToDouble(dtAgg.Rows[0]["TotalStockIn"]);
                        totalStockOut = Convert.ToDouble(dtAgg.Rows[0]["TotalStockOut"]);
                        closingStock = Convert.ToDouble(dtAgg.Rows[0]["ClosingStock"]);
                    }

                    double openingStock = closingStock - totalStockIn + totalStockOut;
                    if (openingStock < 0) openingStock = 0;

                    // Header vật tư
                    int headerRowIdx = dgv.Rows.Add(
                        "", "", $"{itemCode} - {itemName} (ĐVT: {unit})",
                        "", "", "", "", "");
                    for (int c = 0; c < dgv.Columns.Count; c++)
                        dgv.Rows[headerRowIdx].Cells[c].Style = groupStyle;

                    double runningBalance = openingStock;
                    double totalIn = 0, totalOut = 0;

                    // Dòng tồn đầu kỳ
                    int openingRowIdx = dgv.Rows.Add(
                        fromDate.ToString("dd/MM/yyyy"), "-", "Tồn đầu kỳ",
                        "-", "-",
                        runningBalance > 0 ? runningBalance.ToString("N2") : "-",
                        unitPrice > 0 ? unitPrice.ToString("N0") : "-",
                        runningBalance > 0 ? (runningBalance * unitPrice).ToString("N0") : "-");
                    dgv.Rows[openingRowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);

                    // Phát sinh nhập
                    var dtStockIn = _repo.GetDetailLedgerStockIn(materialId, fromDate, toDate);
                    foreach (System.Data.DataRow row in dtStockIn.Rows)
                    {
                        double qty = Convert.ToDouble(row["So_Luong_Nhap"]);
                        double price = Convert.ToDouble(row["Don_Gia_Nhap"]);
                        runningBalance += qty;
                        totalIn += qty;

                        dgv.Rows.Add(
                            Convert.ToDateTime(row["Ngay_Nhap"]).ToString("dd/MM/yyyy"),
                            row["Ma_Phieu_Nhap"].ToString(),
                            $"Nhập từ {row["Ten_NCC"]}",
                            qty.ToString("N2"), "-",
                            runningBalance.ToString("N2"),
                            price.ToString("N0"),
                            (runningBalance * price).ToString("N0"));
                    }

                    // Phát sinh xuất
                    var dtStockOut = _repo.GetDetailLedgerStockOut(materialId, fromDate, toDate);
                    foreach (System.Data.DataRow row in dtStockOut.Rows)
                    {
                        double qty = Convert.ToDouble(row["So_Luong_Xuat"]);
                        double price = Convert.ToDouble(row["Don_Gia"]);
                        runningBalance -= qty;
                        totalOut += qty;

                        dgv.Rows.Add(
                            Convert.ToDateTime(row["Ngay_Xuat"]).ToString("dd/MM/yyyy"),
                            row["Ma_Phieu_Xuat"].ToString(),
                            $"Xuất cho {row["Ma_Lenh_SX"]}",
                            "-", qty.ToString("N2"),
                            runningBalance.ToString("N2"),
                            price.ToString("N0"),
                            (runningBalance * price).ToString("N0"));
                    }

                    // Dòng tổng cộng
                    int totalRowIdx = dgv.Rows.Add(
                        "", "", $"Tổng cộng {itemCode}",
                        totalIn > 0 ? totalIn.ToString("N2") : "-",
                        totalOut > 0 ? totalOut.ToString("N2") : "-",
                        runningBalance.ToString("N2"),
                        "-",
                        (runningBalance * unitPrice).ToString("N0"));
                    for (int c = 0; c < dgv.Columns.Count; c++)
                        dgv.Rows[totalRowIdx].Cells[c].Style = totalStyle;

                    // Dòng trống ngăn cách
                    dgv.Rows.Add("", "", "", "", "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // BÁO CÁO 2: TỔNG HỢP TỒN KHO
        // ─────────────────────────────────────────────────────

        private void LoadInventorySummary(DateTime fromDate, DateTime toDate)
        {
            var dgv = dgvReport;

            var rightStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0),
                Font = new Font("Segoe UI", 10f)
            };
            var boldRight = new DataGridViewCellStyle(rightStyle)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colItemCode", HeaderText = "Mã hàng", FillWeight = 10 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colItemName", HeaderText = "Tên hàng", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "ĐVT", FillWeight = 7 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOpeningStock", HeaderText = "Tồn đầu", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockIn", HeaderText = "Nhập kỳ", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockOut", HeaderText = "Xuất kỳ", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colClosingStock", HeaderText = "Tồn cuối", FillWeight = 10, DefaultCellStyle = boldRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnitPrice", HeaderText = "Đơn giá", FillWeight = 11, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockValue", HeaderText = "Giá trị tồn", FillWeight = 12, DefaultCellStyle = boldRight });

            try
            {
                // Toàn bộ SQL nằm trong InventoryReportRepository
                var dt = _repo.GetInventorySummary(fromDate, toDate);

                double grandTotal = 0;

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    double openingStock = Convert.ToDouble(row["OpeningStock"]);
                    double stockIn = Convert.ToDouble(row["StockIn"]);
                    double stockOut = Convert.ToDouble(row["StockOut"]);
                    double closingStock = Convert.ToDouble(row["ClosingStock"]);
                    double unitPrice = Convert.ToDouble(row["Gia_Nhap"]);
                    double stockValue = closingStock * unitPrice;
                    grandTotal += stockValue;

                    int rowIdx = dgv.Rows.Add(
                        row["Ma_Nguyen_Lieu"],
                        row["Ten_Nguyen_Lieu"],
                        row["Don_Vi_Tinh"],
                        openingStock > 0 ? openingStock.ToString("N2") : "-",
                        stockIn > 0 ? stockIn.ToString("N2") : "-",
                        stockOut > 0 ? stockOut.ToString("N2") : "-",
                        closingStock.ToString("N2"),
                        unitPrice > 0 ? unitPrice.ToString("N0") : "-",
                        stockValue > 0 ? stockValue.ToString("N0") : "-");

                    // Tô đỏ nếu hết hàng
                    if (closingStock <= 0)
                    {
                        dgv.Rows[rowIdx].Cells["colClosingStock"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                        dgv.Rows[rowIdx].Cells["colClosingStock"].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                    }
                }

                // Dòng tổng cộng
                int totalRowIdx = dgv.Rows.Add("", "TỔNG CỘNG", "", "", "", "", "", "", grandTotal.ToString("N0"));
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    dgv.Rows[totalRowIdx].Cells[c].Style.BackColor = Color.FromArgb(220, 235, 255);
                    dgv.Rows[totalRowIdx].Cells[c].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                    dgv.Rows[totalRowIdx].Cells[c].Style.ForeColor = Color.FromArgb(20, 40, 120);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // BÁO CÁO 3: THẺ KHO
        // ─────────────────────────────────────────────────────

        private void LoadStockCard(DateTime fromDate, DateTime toDate)
        {
            var dgv = dgvReport;

            var rightStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0),
                Font = new Font("Segoe UI", 10f)
            };
            var boldRight = new DataGridViewCellStyle(rightStyle)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Ngày", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVoucher", HeaderText = "Chứng từ", FillWeight = 14 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescription", HeaderText = "Diễn giải", FillWeight = 28 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockIn", HeaderText = "Nhập", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStockOut", HeaderText = "Xuất", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRunningBalance", HeaderText = "Tồn lũy kế", FillWeight = 12, DefaultCellStyle = boldRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaterial", HeaderText = "Vật tư", FillWeight = 16 });

            try
            {
                // Toàn bộ SQL nằm trong InventoryReportRepository
                var dt = _repo.GetStockCard(fromDate, toDate);

                string currentCode = "";
                double runningBalance = 0;

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string itemCode = row["Ma_Nguyen_Lieu"].ToString();
                    string itemName = row["Ten_Nguyen_Lieu"].ToString();
                    string unit = row["Don_Vi_Tinh"].ToString();

                    // Header mới khi gặp vật tư khác
                    if (itemCode != currentCode)
                    {
                        if (currentCode != "") dgv.Rows.Add("", "", "", "", "", "", "");

                        currentCode = itemCode;
                        runningBalance = Convert.ToDouble(row["OpeningStock"]);

                        // Dòng tiêu đề vật tư
                        int headerRowIdx = dgv.Rows.Add(
                            "", "", $"{itemCode} - {itemName} (ĐVT: {unit})",
                            "", "", "", "");
                        for (int c = 0; c < dgv.Columns.Count; c++)
                        {
                            dgv.Rows[headerRowIdx].Cells[c].Style.BackColor = Color.FromArgb(235, 245, 255);
                            dgv.Rows[headerRowIdx].Cells[c].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                            dgv.Rows[headerRowIdx].Cells[c].Style.ForeColor = Color.FromArgb(30, 64, 175);
                        }

                        // Dòng tồn đầu kỳ
                        dgv.Rows.Add(
                            fromDate.ToString("dd/MM/yyyy"), "-", "Tồn đầu kỳ",
                            "-", "-",
                            runningBalance > 0 ? runningBalance.ToString("N2") : "0",
                            $"{itemCode} - {itemName}");
                    }

                    double stockIn = Convert.ToDouble(row["StockIn"]);
                    double stockOut = Convert.ToDouble(row["StockOut"]);
                    runningBalance += stockIn - stockOut;

                    dgv.Rows.Add(
                        Convert.ToDateTime(row["TxDate"]).ToString("dd/MM/yyyy"),
                        row["VoucherCode"],
                        row["Description"],
                        stockIn > 0 ? stockIn.ToString("N2") : "-",
                        stockOut > 0 ? stockOut.ToString("N2") : "-",
                        runningBalance.ToString("N2"),
                        $"{itemCode} - {itemName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }
    }
}