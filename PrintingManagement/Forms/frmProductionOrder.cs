// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmProductionOrder.cs - LỆNH SẢN XUẤT                              ║
// ║  - Chọn báo giá đã duyệt                                            ║
// ║  - Lấy dữ liệu → lưới thông tin giao xưởng (không giá)               ║
// ║  - Bắt đầu sản xuất → Lưu vào LENH_SAN_XUAT                        ║
// ║  - Xuất Excel lệnh SX                                               ║
// ╚══════════════════════════════════════════════════════════════════════╝

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmProductionOrder : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly ProductionOrderRepository _repo = new();

        private int _quoteId = 0;
        private int _customerId = 0;
        private string _quoteCode = "";


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmProductionOrder()
        {
            InitializeComponent();
            this.Load += frmProductionOrder_Load;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmProductionOrder_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadQuoteOptions();
            dtpStartDate.Value = DateTime.Today;
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ GRID VẬT TƯ
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvMaterials;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.Both;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);

            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            // Lưới giao xưởng: thông số từ báo giá — không cột giá, không tồn kho
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 8,
                MinimumWidth = 48,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHangMuc",
                HeaderText = "Hạng mục",
                FillWeight = 28,
                MinimumWidth = 140,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Padding = new Padding(8, 0, 6, 0),
                    BackColor = Color.FromArgb(248, 250, 252)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNoiDung",
                HeaderText = "Nội dung / yêu cầu",
                FillWeight = 64,
                MinimumWidth = 220,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f),
                    Padding = new Padding(8, 0, 6, 0)
                }
            });
        }


        // ─────────────────────────────────────────────────────
        // NẠP COMBOBOX BÁO GIÁ ĐÃ DUYỆT
        // ─────────────────────────────────────────────────────

        private void LoadQuoteOptions()
        {
            cboQuote.SelectedIndexChanged -= cboQuote_SelectedIndexChanged;
            cboQuote.Items.Clear();
            cboQuote.Items.Add(new QuoteItem()); // dòng trống

            try
            {
                // Toàn bộ SQL nằm trong ProductionOrderRepository
                var dt = _repo.GetApprovedQuotes();

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    cboQuote.Items.Add(new QuoteItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["Ma_Bao_Gia"].ToString(),
                        ProductName = row["Ten_San_Pham"].ToString(),
                        CustomerName = row["Ten_Khach_Hang"].ToString(),
                        Quantity = Convert.ToInt32(row["So_Luong"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }

            cboQuote.SelectedIndexChanged += cboQuote_SelectedIndexChanged;
            cboQuote.SelectedIndex = 0;
        }

        private void cboQuote_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Placeholder — có thể mở rộng auto-load sau
        }


        // ─────────────────────────────────────────────────────
        // NÚT LẤY DỮ LIỆU
        // ─────────────────────────────────────────────────────

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (!(cboQuote.SelectedItem is QuoteItem quote) || quote.Id == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn số báo giá!", "Thiếu thông tin");
                return;
            }

            _quoteId = quote.Id;
            _quoteCode = quote.Code;
            _customerId = 0;

            dgvMaterials.Rows.Clear();

            try
            {
                var dt = _repo.GetQuoteForProduction(_quoteId);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtProductName.Text = row["Ten_San_Pham"].ToString();
                    txtQuantity.Text = row["So_Luong"].ToString();
                    txtCustomerName.Text = row["Ten_Khach_Hang"].ToString();
                    _customerId = row["id_Khach_Hang"] != System.DBNull.Value
                        ? Convert.ToInt32(row["id_Khach_Hang"]) : 0;
                }

                int sheetRows = FillWorkshopSheetFromQuote(_quoteId);
                if (sheetRows == 0)
                    MessageBox.Show(
                        "⚠️ Đã lấy thông tin sản phẩm nhưng không có dòng thông số xưởng (kiểm tra báo giá / SP xuất Excel).",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(
                        "✅ Đã lấy dữ liệu. Lưới bên dưới là thông tin giao xưởng (giấy, khổ in, màu/mực, SL…), không hiển thị giá.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        /// <summary>
        /// Điền lưới từ thông số kỹ thuật báo giá (BAO_GIA + mức chính) — phục vụ xưởng, không có đơn giá.
        /// </summary>
        private int FillWorkshopSheetFromQuote(int quoteId)
        {
            dgvMaterials.Rows.Clear();

            var dt = _repo.GetQuoteForExcelExport(quoteId);
            if (dt.Rows.Count == 0)
                return 0;

            var r = dt.Rows[0];
            int stt = 1;

            void AddRow(string hangMuc, object? cellVal, bool highlight = false)
            {
                string s = cellVal == null || cellVal == System.DBNull.Value
                    ? ""
                    : cellVal.ToString()!.Trim();
                if (string.IsNullOrEmpty(s)) s = "—";
                int idx = dgvMaterials.Rows.Add(stt++, hangMuc, s);
                if (highlight)
                {
                    dgvMaterials.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
                    dgvMaterials.Rows[idx].DefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                }
            }

            // ── Phần A: Thông tin báo giá gốc (chỉ đọc) ─────────────
            AddRow("Mã báo giá", r["Ma_Bao_Gia"]);
            AddRow("Tên sản phẩm", r["Ten_San_Pham"]);
            AddRow("Khách hàng", r["Ten_Khach_Hang"]);
            AddRow("Kích thước thành phẩm", r["Kich_Thuoc_Thanh_Pham"]);
            AddRow("Khổ in", r["Kho_In"]);
            AddRow("Khối lượng / loại giấy", r["Khoi_Luong_Giay"]);
            AddRow("Số màu in", r["So_Mau_In"]);
            AddRow("Số con / bài", r["So_Con"]);
            AddRow("Số lượng in", r["So_Luong"]);

            // ── Phần B: Tính toán định lượng từ QuoteCalculator ────
            // Đọc thông số từ báo giá
            int soLuong = ToInt(r["So_Luong"]);
            int soCon = ToInt(r["So_Con"]);
            int soMau = ToInt(r["So_Mau_In"]);
            double gsm = ToDouble(r["Khoi_Luong_Giay"]);
            double giaGiayTan = ToDouble(r["Gia_Giay_Tan"]);

            // Parse khổ in "W x H" hoặc "W × H"
            double pw = 0, ph = 0;
            string khoIn = r["Kho_In"]?.ToString() ?? "";
            var parts = khoIn.Split('x', '×');
            if (parts.Length == 2)
            {
                double.TryParse(parts[0].Trim(), out pw);
                double.TryParse(parts[1].Trim(), out ph);
            }

            // Bù hao mặc định: 500 tờ (theo LOAI_GIAY mặc định)
            int buHao = 500;

            // Gọi QuoteCalculator — GIỮ NGUYÊN CÔNG THỨC frmPriceCalculation
            var calc = QuoteCalculator.TinhDinhLuong(
                soLuong, soCon, soMau, gsm, pw, ph, giaGiayTan, buHao);

            // Thêm dòng phân cách "ĐỊNH LƯỢNG GIAO XƯỞNG"
            int sepIdx = dgvMaterials.Rows.Add("▼", "ĐỊNH LƯỢNG GIAO XƯỞNG", "");
            dgvMaterials.Rows[sepIdx].DefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvMaterials.Rows[sepIdx].DefaultCellStyle.ForeColor = Color.White;
            dgvMaterials.Rows[sepIdx].DefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgvMaterials.Rows[sepIdx].Cells["colSTT"].Value = "";
            stt++;

            // Các dòng định lượng — tô cam/highlight
            AddRow("Số tờ chạy (có bù hao)", $"{calc.SoToChay:N0} tờ", highlight: true);
            AddRow("Số ram giấy (500 tờ/ram)", $"{calc.SoRam:N0} ram", highlight: true);
            AddRow("Khối lượng giấy dự kiến", $"{calc.KhoiLuongGiay_kg:N1} kg", highlight: true);
            AddRow("Định mức mực (2 mặt)", $"{calc.DinhMucMuc_ml:N0} ml ({calc.DinhMucMuc_lit:N2} lít)", highlight: true);
            AddRow("Số thùng (dự kiến)", $"{QuoteCalculator.TinhSoThung(soLuong, 50):N0} thùng", highlight: true);

            // Ghi chú
            string gc = r["Ghi_Chu"]?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(gc))
                AddRow("Ghi chú", gc);

            return stt - 1;
        }

        // ─────────────────────────────────────────────────────────────
        // HELPER: Parse số an toàn
        // ─────────────────────────────────────────────────────────────
        private static int ToInt(object? val)
        {
            if (val == null || val == System.DBNull.Value) return 0;
            return int.TryParse(val.ToString(), out int v) ? v : 0;
        }

        private static double ToDouble(object? val)
        {
            if (val == null || val == System.DBNull.Value) return 0;
            return double.TryParse(val.ToString(), out double v) ? v : 0;
        }


        // ─────────────────────────────────────────────────────
        // NÚT BẮT ĐẦU SẢN XUẤT
        // ─────────────────────────────────────────────────────

        private void btnStartProduction_Click(object sender, EventArgs e)
        {
            if (_quoteId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn báo giá!", "Thiếu thông tin");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("⚠️ Vui lòng bấm 'Lấy dữ liệu' trước!", "Thiếu thông tin");
                return;
            }

            if (MessageBox.Show(
                $"Bạn có muốn lưu và bắt đầu sản xuất không?\n\n" +
                $"Sản phẩm: {txtProductName.Text}\n" +
                $"Số lượng: {txtQuantity.Text}\n" +
                $"Khách hàng: {txtCustomerName.Text}",
                "Xác nhận bắt đầu sản xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                int.TryParse(txtQuantity.Text.Replace(",", ""), out int quantity);

                // Bước 1: Lưu lệnh sản xuất
                string productionCode = _repo.SaveProductionOrder(
                    _quoteId, _customerId,
                    txtProductName.Text.Trim(),
                    quantity,
                    dtpStartDate.Value.Date,
                    out int productionOrderId);

                MessageBox.Show(
                    $"✅ Đã tạo lệnh sản xuất thành công!\n\nMã lệnh: {productionCode}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        // ─────────────────────────────────────────────────────
        // RESET FORM
        // ─────────────────────────────────────────────────────

        private void ResetForm()
        {
            cboQuote.SelectedIndex = 0;
            txtProductName.Clear();
            txtQuantity.Clear();
            txtCustomerName.Clear();
            dgvMaterials.Rows.Clear();
            _quoteId = 0;
            _customerId = 0;
            _quoteCode = "";
            dtpStartDate.Value = DateTime.Today;
            LoadQuoteOptions();
        }


        // ─────────────────────────────────────────────────────
        // NÚT XUẤT EXCEL LỆNH SẢN XUẤT
        // ─────────────────────────────────────────────────────

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_quoteId == 0 || dgvMaterials.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn báo giá và bấm 'Lấy dữ liệu' trước khi in!",
                    "Thiếu thông tin");
                return;
            }

            try
            {
                var dtQuote = _repo.GetQuoteForExcelExport(_quoteId);
                if (dtQuote.Rows.Count == 0)
                {
                    MessageBox.Show("❌ Không tìm thấy dữ liệu báo giá!", "Lỗi");
                    return;
                }
                var qRow = dtQuote.Rows[0];
                string productionCode = qRow["Ma_Bao_Gia"].ToString();
                DateTime orderDate = dtpStartDate.Value.Date;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("LenhSX");
                    int row = 1;

                    void BorderCell(int r, int c)
                    {
                        ws.Cells[r, c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[r, c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        ws.Cells[r, c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[r, c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    // ── Tiêu đề công ty ───────────────────────────
                    ws.Cells[row, 1, row, 6].Merge = true;
                    ws.Cells[row, 1].Value = "CÔNG TY TNHH SX TM DỊCH VỤ AN LÂM";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 12;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Row(row).Height = 22; row++;

                    ws.Cells[row, 1, row, 6].Merge = true;
                    ws.Cells[row, 1].Value = "51/10/3 Hòa Bình, Phường Tân Phú, Tp. HCM";
                    ws.Cells[row, 1].Style.Font.Size = 9;
                    ws.Row(row).Height = 14; row++; row++;

                    // ── Tiêu đề LỆNH SẢN XUẤT ───────────────────
                    ws.Cells[row, 1, row, 6].Merge = true;
                    ws.Cells[row, 1].Value = "LỆNH SẢN XUẤT";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 16;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 1].Style.Font.Color.SetColor(Color.FromArgb(0, 100, 200));
                    ws.Row(row).Height = 28; row++;

                    // Ngày + Mã lệnh
                    ws.Cells[row, 1].Value = $"Ngày: {orderDate:dd/MM/yyyy}";
                    ws.Cells[row, 1].Style.Font.Size = 10;
                    ws.Cells[row, 4].Value = "Số lệnh:";
                    ws.Cells[row, 4].Style.Font.Size = 10;
                    ws.Cells[row, 5, row, 6].Merge = true;
                    ws.Cells[row, 5].Value = productionCode;
                    ws.Cells[row, 5].Style.Font.Bold = true;
                    ws.Cells[row, 5].Style.Font.Size = 12;
                    ws.Row(row).Height = 20; row++; row++;

                    // ── BẢNG THÔNG TIN GIAO XƯỞNG ──────────────────
                    // Header bảng
                    ws.Cells[row, 1, row, 6].Merge = true;
                    ws.Cells[row, 1].Value = "PHIẾU GIAO XƯỞNG - THÔNG TIN SẢN XUẤT";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 11;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 100, 200));
                    ws.Cells[row, 1].Style.Font.Color.SetColor(Color.White);
                    ws.Row(row).Height = 22; row++;

                    // Header cột
                    ws.Cells[row, 1].Value = "STT";
                    ws.Cells[row, 2].Value = "Hạng mục";
                    ws.Cells[row, 3, row, 6].Merge = true;
                    ws.Cells[row, 3].Value = "Nội dung / yêu cầu";
                    for (int c = 1; c <= 6; c++)
                    {
                        ws.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, c].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(232, 238, 248));
                        ws.Cells[row, c].Style.Font.Bold = true;
                        ws.Cells[row, c].Style.Font.Size = 9;
                        ws.Cells[row, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        BorderCell(row, c);
                    }
                    ws.Row(row).Height = 20; row++;

                    // ── Duyệt tất cả dòng dgvMaterials ─────────────
                    foreach (DataGridViewRow gRow in dgvMaterials.Rows)
                    {
                        string sttVal = gRow.Cells["colSTT"].Value?.ToString() ?? "";
                        string hangMuc = gRow.Cells["colHangMuc"].Value?.ToString() ?? "";
                        string noiDung = gRow.Cells["colNoiDung"].Value?.ToString() ?? "";

                        // Phát hiện dòng phân cách
                        bool isSeparator = hangMuc == "ĐỊNH LƯỢNG GIAO XƯỞNG";
                        bool isHighlight = !isSeparator &&
                            gRow.DefaultCellStyle.BackColor != Color.Empty &&
                            gRow.DefaultCellStyle.BackColor != Color.White;

                        ws.Cells[row, 1].Value = sttVal;
                        ws.Cells[row, 2].Value = hangMuc;
                        ws.Cells[row, 3, row, 6].Merge = true;
                        ws.Cells[row, 3].Value = noiDung;

                        if (isSeparator)
                        {
                            ws.Cells[row, 1, row, 6].Merge = true;
                            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 100, 200));
                            ws.Cells[row, 1].Style.Font.Color.SetColor(Color.White);
                            ws.Cells[row, 1].Style.Font.Bold = true;
                            ws.Cells[row, 1].Style.Font.Size = 10;
                            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Row(row).Height = 20;
                        }
                        else if (isHighlight)
                        {
                            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 243, 224));
                            ws.Cells[row, 1].Style.Font.Bold = true;
                            ws.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 243, 224));
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 3, row, 6].Merge = true;
                            ws.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 243, 224));
                            ws.Cells[row, 3].Style.Font.Bold = true;
                            ws.Row(row).Height = 18;
                        }
                        else
                        {
                            ws.Row(row).Height = 18;
                        }

                        // Căn chỉnh
                        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        ws.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Font mặc định
                        ws.Cells[row, 1].Style.Font.Size = 9;
                        ws.Cells[row, 2].Style.Font.Size = 9;
                        ws.Cells[row, 3].Style.Font.Size = 9;

                        // Border
                        for (int c = 1; c <= 6; c++)
                            BorderCell(row, c);
                        row++;
                    }

                    row++;

                    // ── Ký tên ──────────────────────────────────
                    int signRow = row;
                    ws.Cells[signRow, 1].Value = "Người lập";
                    ws.Cells[signRow, 1, signRow, 2].Merge = true;
                    ws.Cells[signRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[signRow, 1].Style.Font.Bold = true;

                    ws.Cells[signRow, 3].Value = "Thiết kế";
                    ws.Cells[signRow, 3, signRow, 4].Merge = true;
                    ws.Cells[signRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[signRow, 3].Style.Font.Bold = true;

                    ws.Cells[signRow, 5].Value = "Thủ kho";
                    ws.Cells[signRow, 5, signRow, 6].Merge = true;
                    ws.Cells[signRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[signRow, 5].Style.Font.Bold = true;
                    signRow++;

                    ws.Cells[signRow, 1, signRow, 2].Merge = true;
                    ws.Cells[signRow, 1].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[signRow, 1].Style.Font.Italic = true;
                    ws.Cells[signRow, 1].Style.Font.Size = 8;
                    ws.Cells[signRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells[signRow, 3, signRow, 4].Merge = true;
                    ws.Cells[signRow, 3].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[signRow, 3].Style.Font.Italic = true;
                    ws.Cells[signRow, 3].Style.Font.Size = 8;
                    ws.Cells[signRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells[signRow, 5, signRow, 6].Merge = true;
                    ws.Cells[signRow, 5].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[signRow, 5].Style.Font.Italic = true;
                    ws.Cells[signRow, 5].Style.Font.Size = 8;
                    ws.Cells[signRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Cột rộng
                    ws.Column(1).Width = 6;
                    ws.Column(2).Width = 28;
                    ws.Column(3).Width = 16;
                    ws.Column(4).Width = 16;
                    ws.Column(5).Width = 16;
                    ws.Column(6).Width = 16;

                    string filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"LENH_SAN_XUAT_{productionCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    pkg.SaveAs(new FileInfo(filePath));

                    MessageBox.Show($"✅ Đã xuất lệnh sản xuất ra Excel:\n{filePath}",
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
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // INNER CLASS — ĐỐI TƯỢNG BÁO GIÁ TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class QuoteItem
        {
            public int Id { get; set; }
            public string Code { get; set; } = "";
            public string ProductName { get; set; } = "";
            public string CustomerName { get; set; } = "";
            public int Quantity { get; set; }

            public override string ToString()
            {
                if (Id == 0) return "-- Chọn số báo giá --";
                return $"{Code} - {ProductName} - {Quantity:N0} cái";
            }
        }
    }
}