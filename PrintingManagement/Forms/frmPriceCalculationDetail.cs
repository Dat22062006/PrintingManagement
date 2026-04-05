// ═══════════════════════════════════════════════════════════════════
// ║  frmPriceCalculationDetail.cs - XEM CHI TIẾT GIÁ BÁO GIÁ     ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] Nhận thêm detailId → load đúng mức SL đang được tích
//       thay vì luôn lấy dòng đầu tiên.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.IO;
using System.Windows.Forms;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

namespace PrintingManagement
{
    public partial class frmPriceCalculationDetail : Form
    {
        private readonly PriceCalculationDetailRepository _repo = new();
        private readonly int _quoteId;
        private readonly int _detailId;  // [FIX] 0 = dùng mức mặc định
        private bool _isClosing = false;

        public int Quantity { get; set; }
        public double CostPaper { get; set; }
        public double CostPlate { get; set; }
        public double CostPrint { get; set; }
        public double CostLaminate { get; set; }
        public double CostMetalize { get; set; }
        public double CostUV { get; set; }
        public double CostDie { get; set; }
        public double CostDieMold { get; set; }
        public double CostGlue { get; set; }
        public double CostRibbon { get; set; }
        public double CostButton { get; set; }
        public double CostBox { get; set; }
        public double CostDelivery { get; set; }
        public double CostProof { get; set; }
        public double TotalCost { get; set; }
        public double CostPerUnit { get; set; }
        public double PricePerUnit { get; set; }
        public double TotalQuotePrice { get; set; }

        // ─────────────────────────────────────────────────────
        // CONSTRUCTOR
        // ─────────────────────────────────────────────────────

        // [FIX] Thêm tham số detailId (mặc định = 0)
        public frmPriceCalculationDetail(int quoteId, int detailId = 0)
        {
            InitializeComponent();
            _quoteId = quoteId;
            _detailId = detailId;
            this.AutoValidate = AutoValidate.Disable;
        }

        // ─────────────────────────────────────────────────────
        // HIỂN THỊ KẾT QUẢ
        // ─────────────────────────────────────────────────────

        public void DisplayResults()
        {
            lblCostPaper.Text = CostPaper.ToString("N0") + "đ";
            lblCostPlate.Text = CostPlate.ToString("N0") + "đ";
            lblCostPrint.Text = CostPrint.ToString("N0") + "đ";
            lblCostLaminate.Text = CostLaminate.ToString("N0") + "đ";
            lblCostMetalize.Text = CostMetalize.ToString("N0") + "đ";
            lblCostUV.Text = CostUV.ToString("N0") + "đ";
            lblCostDie.Text = CostDie.ToString("N0") + "đ";
            lblCostDieMold.Text = CostDieMold.ToString("N0") + "đ";
            lblCostGlue.Text = CostGlue.ToString("N0") + "đ";
            lblCostRibbon.Text = CostRibbon.ToString("N0") + "đ";
            lblCostButton.Text = CostButton.ToString("N0") + "đ";
            lblCostBox.Text = CostBox.ToString("N0") + "đ";
            lblCostDelivery.Text = CostDelivery.ToString("N0") + "đ";
            lblCostProof.Text = CostProof.ToString("N0") + "đ";
            lblTotalCost.Text = TotalCost.ToString("N0") + "đ";
            lblCostPerUnit.Text = CostPerUnit.ToString("N0") + "đ";
            lblPricePerUnit.Text = PricePerUnit.ToString("N0") + "đ";
            lblTotalQuotePrice.Text = TotalQuotePrice.ToString("N0") + "đ";
        }

        // ─────────────────────────────────────────────────────
        // LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmPriceCalculationDetail_Load(object sender, EventArgs e)
        {
            LoadQuoteDetail();
            btnExportPDF.Click -= btnExportPDF_Click;
            btnExportPDF.Click += btnExportPDF_Click;
        }

        // ─────────────────────────────────────────────────────
        // NẠP CHI TIẾT BÁO GIÁ TỪ DB
        // ─────────────────────────────────────────────────────

        private void LoadQuoteDetail()
        {
            try
            {
                System.Data.DataTable dt;

                if (_detailId > 0)
                    // [FIX] Load đúng mức SL đang được tích theo detailId
                    dt = _repo.GetQuoteDetailById(_detailId);
                else
                    // Fallback: lấy theo quoteId (mức đầu tiên hoặc mức primary)
                    dt = _repo.GetQuoteDetail(_quoteId);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy chi tiết báo giá!", "Cảnh báo");
                    this.Close();
                    return;
                }

                var row = dt.Rows[0];

                Quantity = Convert.ToInt32(row["So_Luong"]);
                CostPaper = Convert.ToDouble(row["Tien_Giay"]);
                CostPlate = Convert.ToDouble(row["Tien_Kem"]);
                CostPrint = Convert.ToDouble(row["Tien_In"]);
                CostLaminate = Convert.ToDouble(row["Tien_Can_Mang"]);
                CostMetalize = Convert.ToDouble(row["Tien_Metalize"]);
                CostUV = Convert.ToDouble(row["Tien_UV"]);
                CostDie = Convert.ToDouble(row["Tien_Be"]);
                CostDieMold = Convert.ToDouble(row["Tien_Khuon_Be"]);
                CostGlue = Convert.ToDouble(row["Tien_Dan"]);
                CostRibbon = Convert.ToDouble(row["Tien_Day"]);
                CostButton = Convert.ToDouble(row["Tien_Nut"]);
                CostBox = Convert.ToDouble(row["Tien_Thung"]);
                CostDelivery = Convert.ToDouble(row["Tien_Xe_Giao"]);
                CostProof = Convert.ToDouble(row["Tien_Proof"]);
                TotalCost = Convert.ToDouble(row["Tong_Gia_Thanh"]);
                CostPerUnit = Convert.ToDouble(row["Gia_Moi_Cai"]);
                PricePerUnit = Convert.ToDouble(row["Gia_Bao_Khach"]);
                TotalQuotePrice = PricePerUnit * Quantity;

                DisplayResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
                this.Close();
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT XÁC NHẬN
        // ─────────────────────────────────────────────────────

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ─────────────────────────────────────────────────────
        // NÚT XUẤT PDF
        // ─────────────────────────────────────────────────────

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PDF|*.pdf";
                dialog.FileName = $"ChiTietGia_{_quoteId}_{DateTime.Today:ddMMyyyy}.pdf";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    ExportDetailPdf(dialog.FileName);
                    MessageBox.Show($"✅ Xuất PDF thành công!\n{dialog.FileName}", "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    { FileName = dialog.FileName, UseShellExecute = true });
                }
                catch (Exception ex) { MessageBox.Show($"❌ Lỗi xuất PDF:\n{ex.Message}", "Lỗi"); }
            }
        }

        // ─────────────────────────────────────────────────────
        // XUẤT PDF CHI TIẾT GIÁ
        // ─────────────────────────────────────────────────────

        private void ExportDetailPdf(string outputPath)
        {
            string quoteCode = "", quoteDate = "", productName = "", customerName = "";
            try
            {
                var dt = _repo.GetQuoteHeader(_quoteId);
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    quoteCode = row["Ma_Bao_Gia"].ToString();
                    quoteDate = row["Ngay_Bao_Gia"].ToString();
                    productName = row["Ten_San_Pham"].ToString();
                    customerName = row["Ten_Khach_Hang"].ToString();
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi tải header báo giá: {ex.Message}"); }

            string fontPath = @"C:\Windows\Fonts\arial.ttf";
            var baseFont = iTextPdf.BaseFont.CreateFont(
                fontPath, iTextPdf.BaseFont.IDENTITY_H, iTextPdf.BaseFont.EMBEDDED);

            iText.Font fontNormal = new iText.Font(baseFont, 10, iText.Font.NORMAL, iText.BaseColor.BLACK);
            iText.Font fontBold = new iText.Font(baseFont, 10, iText.Font.BOLD, iText.BaseColor.BLACK);
            iText.Font fontTitle = new iText.Font(baseFont, 14, iText.Font.BOLD, new iText.BaseColor(30, 58, 95));
            iText.Font fontHeader = new iText.Font(baseFont, 10, iText.Font.BOLD, iText.BaseColor.WHITE);
            iText.Font fontSmall = new iText.Font(baseFont, 9, iText.Font.NORMAL, new iText.BaseColor(100, 116, 139));
            iText.Font fontBlue = new iText.Font(baseFont, 10, iText.Font.BOLD, new iText.BaseColor(37, 99, 235));
            iText.Font fontGreen = new iText.Font(baseFont, 11, iText.Font.BOLD, new iText.BaseColor(5, 150, 105));
            iText.Font fontWhite = new iText.Font(baseFont, 10, iText.Font.NORMAL, iText.BaseColor.WHITE);
            iText.Font fontSubHeader = new iText.Font(baseFont, 9, iText.Font.NORMAL, new iText.BaseColor(189, 213, 234));
            iText.Font fontFooter = new iText.Font(baseFont, 8, iText.Font.ITALIC, new iText.BaseColor(148, 163, 184));
            iText.Font fontVatNote = new iText.Font(baseFont, 8, iText.Font.ITALIC, new iText.BaseColor(189, 213, 234));

            iText.BaseColor colorNavy = new iText.BaseColor(30, 58, 95);
            iText.BaseColor colorBlue = new iText.BaseColor(37, 99, 235);
            iText.BaseColor colorGray = new iText.BaseColor(248, 250, 252);
            iText.BaseColor colorLGray = new iText.BaseColor(203, 213, 225);

            var doc = new iText.Document(iText.PageSize.A4, 36f, 36f, 36f, 36f);
            using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                iTextPdf.PdfWriter.GetInstance(doc, fileStream);
                doc.Open();

                // Header
                var headerTable = new iTextPdf.PdfPTable(1) { WidthPercentage = 100f };
                var headerCell = new iTextPdf.PdfPCell
                { BackgroundColor = colorNavy, Border = iText.Rectangle.NO_BORDER, Padding = 14f };
                headerCell.AddElement(new iText.Phrase("CHI TIẾT GIÁ SẢN PHẨM", fontTitle));
                headerCell.AddElement(new iText.Phrase($"Khách hàng: {customerName}  |  {productName}", fontWhite));
                headerCell.AddElement(new iText.Phrase($"Báo giá: {quoteCode}  |  Ngày: {quoteDate}", fontSubHeader));
                headerTable.AddCell(headerCell);
                doc.Add(headerTable);
                doc.Add(new iText.Paragraph(" "));

                // Bảng chi tiết
                var table = new iTextPdf.PdfPTable(2) { WidthPercentage = 100f, SpacingBefore = 4f };
                table.SetWidths(new float[] { 55f, 45f });

                void AddSectionHeader(string text)
                {
                    var cell = new iTextPdf.PdfPCell(new iText.Phrase(text, fontHeader))
                    { Colspan = 2, BackgroundColor = colorBlue, Border = iText.Rectangle.NO_BORDER, Padding = 8f, PaddingLeft = 10f };
                    table.AddCell(cell);
                }

                void AddDataRow(string label, string value, bool isAlt = false)
                {
                    iText.BaseColor bg = isAlt ? colorGray : iText.BaseColor.WHITE;
                    var cellLabel = new iTextPdf.PdfPCell(new iText.Phrase(label, fontSmall))
                    { BackgroundColor = bg, BorderColor = colorLGray, Padding = 7f, PaddingLeft = 10f };
                    var cellValue = new iTextPdf.PdfPCell(new iText.Phrase(value, isAlt ? fontBold : fontNormal))
                    { BackgroundColor = bg, BorderColor = colorLGray, Padding = 7f, HorizontalAlignment = iText.Element.ALIGN_RIGHT };
                    table.AddCell(cellLabel);
                    table.AddCell(cellValue);
                }

                AddSectionHeader("CHI PHÍ SẢN XUẤT");
                AddDataRow("Tiền giấy", CostPaper.ToString("N0") + " đ");
                AddDataRow("Tiền kẽm", CostPlate.ToString("N0") + " đ", true);
                AddDataRow("Tiền in", CostPrint.ToString("N0") + " đ");
                AddDataRow("Tiền cán màng", CostLaminate.ToString("N0") + " đ", true);
                AddDataRow("Tiền metalize", CostMetalize.ToString("N0") + " đ");
                AddDataRow("Tiền UV mờ", CostUV.ToString("N0") + " đ", true);
                AddDataRow("Tiền bế", CostDie.ToString("N0") + " đ");
                AddDataRow("Tiền khuôn bế", CostDieMold.ToString("N0") + " đ", true);
                AddDataRow("Tiền dán", CostGlue.ToString("N0") + " đ");
                AddDataRow("Tiền dây", CostRibbon.ToString("N0") + " đ", true);
                AddDataRow("Tiền nút", CostButton.ToString("N0") + " đ");
                AddDataRow("Tiền thùng", CostBox.ToString("N0") + " đ", true);
                AddDataRow("Tiền xe giao", CostDelivery.ToString("N0") + " đ");
                AddDataRow("Tiền in proof", CostProof.ToString("N0") + " đ", true);

                AddSectionHeader("TỔNG KẾT");
                AddDataRow("Tổng chi phí SX", TotalCost.ToString("N0") + " đ");
                AddDataRow("Giá thành 1 cái", CostPerUnit.ToString("N0") + " đ", true);

                var cellLabelPrice = new iTextPdf.PdfPCell(new iText.Phrase("Đơn giá báo khách (đã tính LN)", fontBold))
                { BackgroundColor = new iText.BaseColor(219, 234, 254), BorderColor = colorLGray, Padding = 9f, PaddingLeft = 10f };
                var cellValuePrice = new iTextPdf.PdfPCell(new iText.Phrase(PricePerUnit.ToString("N0") + " đ/cái", fontBlue))
                { BackgroundColor = new iText.BaseColor(219, 234, 254), BorderColor = colorLGray, Padding = 9f, HorizontalAlignment = iText.Element.ALIGN_RIGHT };
                table.AddCell(cellLabelPrice);
                table.AddCell(cellValuePrice);

                var cellLabelTotal = new iTextPdf.PdfPCell
                { BackgroundColor = colorNavy, Border = iText.Rectangle.NO_BORDER, Padding = 11f, PaddingLeft = 10f };
                cellLabelTotal.AddElement(new iText.Phrase($"TỔNG TIỀN ({Quantity:N0} cái)", fontHeader));
                cellLabelTotal.AddElement(new iText.Phrase("chưa VAT", fontVatNote));
                var cellValueTotal = new iTextPdf.PdfPCell(new iText.Phrase(TotalQuotePrice.ToString("N0") + " đ", fontGreen))
                { BackgroundColor = colorNavy, Border = iText.Rectangle.NO_BORDER, Padding = 11f, HorizontalAlignment = iText.Element.ALIGN_RIGHT };
                table.AddCell(cellLabelTotal);
                table.AddCell(cellValueTotal);

                doc.Add(table);
                doc.Add(new iText.Paragraph(" "));
                doc.Add(new iText.Paragraph(
                    $"Xuất ngày {DateTime.Now:dd/MM/yyyy HH:mm}  |  Tài liệu nội bộ — không gửi khách hàng",
                    fontFooter)
                { Alignment = iText.Element.ALIGN_CENTER });
                doc.Close();
            }
        }

        private void frmPriceCalculationDetail_FormClosing(object sender, FormClosingEventArgs e)
        { if (_isClosing) return; _isClosing = true; }

        private void pnlTop_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panel4_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
    }
}