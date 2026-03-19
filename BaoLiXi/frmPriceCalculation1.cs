using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPriceCalculation1 : Form
    {
        private bool isClosing = false;

        public int SoLuong { get; set; }
        public double TienGiay { get; set; }
        public double TienKem { get; set; }
        public double TienIn { get; set; }
        public double TienCanMang { get; set; }
        public double TienMetallize { get; set; }
        public double TienUV { get; set; }
        public double TienBe { get; set; }
        public double TienKhuonBe { get; set; }
        public double TienDan { get; set; }
        public double TienDay { get; set; }
        public double TienNut { get; set; }
        public double TienThung { get; set; }
        public double TienXeGiao { get; set; }
        public double TienProof { get; set; }
        public double TongChiPhi { get; set; }
        public double Gia1SP { get; set; }
        public double GiaBaoKhach { get; set; }
        public double TongGiaBaoKhach { get; set; }

        int idBaoGia;

        public frmPriceCalculation1(int _idBaoGia)
        {
            InitializeComponent();
            idBaoGia = _idBaoGia;
            this.AutoValidate = AutoValidate.Disable;
        }

        public void HienThiKetQua()
        {
            lblTienGiay.Text = TienGiay.ToString("N0") + "đ";
            lblTienKem.Text = TienKem.ToString("N0") + "đ";
            lblTienIn.Text = TienIn.ToString("N0") + "đ";
            lblTienCanMang.Text = TienCanMang.ToString("N0") + "đ";
            lblTienMetallize.Text = TienMetallize.ToString("N0") + "đ";
            lblTienUV.Text = TienUV.ToString("N0") + "đ";
            lblTienBe.Text = TienBe.ToString("N0") + "đ";
            lblTienKhuonBe.Text = TienKhuonBe.ToString("N0") + "đ";
            lblTienDan.Text = TienDan.ToString("N0") + "đ";
            lblTienDay.Text = TienDay.ToString("N0") + "đ";
            lblTienNut.Text = TienNut.ToString("N0") + "đ";
            lblTienThung.Text = TienThung.ToString("N0") + "đ";
            lblTienXeGiao.Text = TienXeGiao.ToString("N0") + "đ";
            lblTienProof.Text = TienProof.ToString("N0") + "đ";
            lblTongChiPhi.Text = TongChiPhi.ToString("N0") + "đ";
            lblGia1SP.Text = Gia1SP.ToString("N0") + "đ";
            lblGiaBaoKhach.Text = GiaBaoKhach.ToString("N0") + "đ";
            lblTongChiPhiLoiNhuan.Text = TongGiaBaoKhach.ToString("N0") + "đ";
        }

        private void frmPriceCalculation1_Load(object sender, EventArgs e)
        {
            LoadChiTietBaoGia();
            btnXuatPDF.Click -= BtnXuatPDF_Click;
            btnXuatPDF.Click += BtnXuatPDF_Click;
        }

        void LoadChiTietBaoGia()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT * FROM CHI_TIET_BAO_GIA WHERE id_Bao_Gia = @id", conn);
                    cmd.Parameters.AddWithValue("@id", idBaoGia);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        SoLuong = Convert.ToInt32(rd["So_Luong"]);
                        TienGiay = Convert.ToDouble(rd["Tien_Giay"]);
                        TienKem = Convert.ToDouble(rd["Tien_Kem"]);
                        TienIn = Convert.ToDouble(rd["Tien_In"]);
                        TienCanMang = Convert.ToDouble(rd["Tien_Can_Mang"]);
                        TienMetallize = Convert.ToDouble(rd["Tien_Metalize"]);
                        TienUV = Convert.ToDouble(rd["Tien_UV"]);
                        TienBe = Convert.ToDouble(rd["Tien_Be"]);
                        TienKhuonBe = Convert.ToDouble(rd["Tien_Khuon_Be"]);
                        TienDan = Convert.ToDouble(rd["Tien_Dan"]);
                        TienDay = Convert.ToDouble(rd["Tien_Day"]);
                        TienNut = Convert.ToDouble(rd["Tien_Nut"]);
                        TienThung = Convert.ToDouble(rd["Tien_Thung"]);
                        TienXeGiao = Convert.ToDouble(rd["Tien_Xe_Giao"]);
                        TienProof = Convert.ToDouble(rd["Tien_Proof"]);
                        TongChiPhi = Convert.ToDouble(rd["Tong_Gia_Thanh"]);
                        Gia1SP = Convert.ToDouble(rd["Gia_Moi_Cai"]);
                        GiaBaoKhach = Convert.ToDouble(rd["Gia_Bao_Khach"]);
                        TongGiaBaoKhach = GiaBaoKhach * SoLuong;
                    }
                    else
                    {
                        MessageBox.Show("⚠️ Không tìm thấy chi tiết báo giá!", "Cảnh báo");
                        this.Close(); return;
                    }
                    rd.Close();
                }
                HienThiKetQua();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi load:\n" + ex.Message, "Lỗi");
                this.Close();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnXuatPDF_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "PDF|*.pdf";
                dlg.FileName = "ChiTietGia_" + idBaoGia + "_" + DateTime.Today.ToString("ddMMyyyy") + ".pdf";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    XuatPdfChiTiet(dlg.FileName);
                    MessageBox.Show("✅ Xuất PDF thành công!\n" + dlg.FileName, "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Lỗi xuất PDF:\n" + ex.Message + "\n\n" + ex.StackTrace, "Lỗi");
                }
            }
        }

        void XuatPdfChiTiet(string outputPath)
        {
            // Lấy thông tin header từ DB
            string tenKH = "", tenSP = "", maBG = "", ngayBG = "";
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
SELECT bg.Ma_Bao_Gia,
       CONVERT(NVARCHAR,bg.Ngay_Bao_Gia,103) AS NgayBG,
       bg.Ten_San_Pham,
       ISNULL(kh.Ten_Khach_Hang,'') AS TenKH
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
WHERE bg.id = @id", conn);
                cmd.Parameters.AddWithValue("@id", idBaoGia);
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    maBG = rd["Ma_Bao_Gia"].ToString();
                    ngayBG = rd["NgayBG"].ToString();
                    tenSP = rd["Ten_San_Pham"].ToString();
                    tenKH = rd["TenKH"].ToString();
                }
                rd.Close();
            }

            // FONT UNICODE cho tiếng Việt
            string fontPath = @"C:\Windows\Fonts\arial.ttf";   // nếu không có arial.ttf bạn chọn font .ttf khác
            var bf = iTextSharp.text.pdf.BaseFont.CreateFont(
                fontPath,
                iTextSharp.text.pdf.BaseFont.IDENTITY_H,
                iTextSharp.text.pdf.BaseFont.EMBEDDED);

            iTextSharp.text.Font fNorm = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fBold = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fTitle = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(30, 58, 95));
            iTextSharp.text.Font fHead = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
            iTextSharp.text.Font fSmall = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(100, 116, 139));
            iTextSharp.text.Font fBlue = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(37, 99, 235));
            iTextSharp.text.Font fGreen = new iTextSharp.text.Font(bf, 11, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(5, 150, 105));
            iTextSharp.text.Font fWhite10 = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
            iTextSharp.text.Font fSubHeader = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(189, 213, 234));
            iTextSharp.text.Font fFooter = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.ITALIC, new iTextSharp.text.BaseColor(148, 163, 184));
            iTextSharp.text.Font fVatNote = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.ITALIC, new iTextSharp.text.BaseColor(189, 213, 234));

            iTextSharp.text.BaseColor navy = new iTextSharp.text.BaseColor(30, 58, 95);
            iTextSharp.text.BaseColor blue = new iTextSharp.text.BaseColor(37, 99, 235);
            iTextSharp.text.BaseColor gray = new iTextSharp.text.BaseColor(248, 250, 252);
            iTextSharp.text.BaseColor lgray = new iTextSharp.text.BaseColor(203, 213, 225);

            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36f, 36f, 36f, 36f);
            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                var pw = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // HEADER
                iTextSharp.text.pdf.PdfPTable hdr = new iTextSharp.text.pdf.PdfPTable(1);
                hdr.WidthPercentage = 100f;
                iTextSharp.text.pdf.PdfPCell hdrCell = new iTextSharp.text.pdf.PdfPCell();
                hdrCell.BackgroundColor = navy;
                hdrCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                hdrCell.Padding = 14f;

                hdrCell.AddElement(new iTextSharp.text.Phrase("CHI TIẾT GIÁ SẢN PHẨM", fTitle));
                hdrCell.AddElement(new iTextSharp.text.Phrase("Khách hàng: " + tenKH + "  |  " + tenSP, fWhite10));
                hdrCell.AddElement(new iTextSharp.text.Phrase("Báo giá: " + maBG + "  |  Ngày: " + ngayBG, fSubHeader));

                hdr.AddCell(hdrCell);
                doc.Add(hdr);
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // BẢNG CHI TIẾT
                iTextSharp.text.pdf.PdfPTable tbl = new iTextSharp.text.pdf.PdfPTable(2);
                tbl.WidthPercentage = 100f;
                tbl.SpacingBefore = 4f;
                tbl.SetWidths(new float[] { 55f, 45f });

                void AddHeader(string text)
                {
                    iTextSharp.text.pdf.PdfPCell c = new iTextSharp.text.pdf.PdfPCell(
                        new iTextSharp.text.Phrase(text, fHead));
                    c.Colspan = 2;
                    c.BackgroundColor = blue;
                    c.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    c.Padding = 8f;
                    c.PaddingLeft = 10f;
                    tbl.AddCell(c);
                }

                void AddRow(string label, string value, bool alt = false)
                {
                    iTextSharp.text.BaseColor bg = alt ? gray : iTextSharp.text.BaseColor.WHITE;

                    iTextSharp.text.pdf.PdfPCell cL = new iTextSharp.text.pdf.PdfPCell(
                        new iTextSharp.text.Phrase(label, fSmall));
                    cL.BackgroundColor = bg;
                    cL.BorderColor = lgray;
                    cL.Padding = 7f;
                    cL.PaddingLeft = 10f;

                    iTextSharp.text.pdf.PdfPCell cR = new iTextSharp.text.pdf.PdfPCell(
                        new iTextSharp.text.Phrase(value, alt ? fBold : fNorm));
                    cR.BackgroundColor = bg;
                    cR.BorderColor = lgray;
                    cR.Padding = 7f;
                    cR.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;

                    tbl.AddCell(cL);
                    tbl.AddCell(cR);
                }

                AddHeader("CHI PHÍ SẢN XUẤT");
                AddRow("Tiền giấy", TienGiay.ToString("N0") + " đ");
                AddRow("Tiền kẽm", TienKem.ToString("N0") + " đ", true);
                AddRow("Tiền in", TienIn.ToString("N0") + " đ");
                AddRow("Tiền cán màng", TienCanMang.ToString("N0") + " đ", true);
                AddRow("Tiền metalize", TienMetallize.ToString("N0") + " đ");
                AddRow("Tiền UV mờ", TienUV.ToString("N0") + " đ", true);
                AddRow("Tiền bế", TienBe.ToString("N0") + " đ");
                AddRow("Tiền khuôn bế", TienKhuonBe.ToString("N0") + " đ", true);
                AddRow("Tiền dán", TienDan.ToString("N0") + " đ");
                AddRow("Tiền dây", TienDay.ToString("N0") + " đ", true);
                AddRow("Tiền nút", TienNut.ToString("N0") + " đ");
                AddRow("Tiền thùng", TienThung.ToString("N0") + " đ", true);
                AddRow("Tiền xe giao", TienXeGiao.ToString("N0") + " đ");
                AddRow("Tiền in proof", TienProof.ToString("N0") + " đ", true);

                AddHeader("TỔNG KẾT");
                AddRow("Tổng chi phí SX", TongChiPhi.ToString("N0") + " đ");
                AddRow("Giá thành 1 cái", Gia1SP.ToString("N0") + " đ", true);

                // Đơn giá báo khách
                iTextSharp.text.pdf.PdfPCell cLbl1 = new iTextSharp.text.pdf.PdfPCell(
                    new iTextSharp.text.Phrase("Đơn giá báo khách (đã tính LN)", fBold));
                cLbl1.BackgroundColor = new iTextSharp.text.BaseColor(219, 234, 254);
                cLbl1.BorderColor = lgray;
                cLbl1.Padding = 9f;
                cLbl1.PaddingLeft = 10f;

                iTextSharp.text.pdf.PdfPCell cVal1 = new iTextSharp.text.pdf.PdfPCell(
                    new iTextSharp.text.Phrase(GiaBaoKhach.ToString("N0") + " đ/cái", fBlue));
                cVal1.BackgroundColor = new iTextSharp.text.BaseColor(219, 234, 254);
                cVal1.BorderColor = lgray;
                cVal1.Padding = 9f;
                cVal1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                tbl.AddCell(cLbl1);
                tbl.AddCell(cVal1);

                // Tổng tiền
                iTextSharp.text.pdf.PdfPCell cLbl2 = new iTextSharp.text.pdf.PdfPCell();
                cLbl2.BackgroundColor = navy;
                cLbl2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cLbl2.Padding = 11f;
                cLbl2.PaddingLeft = 10f;
                cLbl2.AddElement(new iTextSharp.text.Phrase(
                    "TỔNG TIỀN (" + SoLuong.ToString("N0") + " cái)", fHead));
                cLbl2.AddElement(new iTextSharp.text.Phrase("chưa VAT", fVatNote));

                iTextSharp.text.pdf.PdfPCell cVal2 = new iTextSharp.text.pdf.PdfPCell(
                    new iTextSharp.text.Phrase(TongGiaBaoKhach.ToString("N0") + " đ", fGreen));
                cVal2.BackgroundColor = navy;
                cVal2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cVal2.Padding = 11f;
                cVal2.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                tbl.AddCell(cLbl2);
                tbl.AddCell(cVal2);

                doc.Add(tbl);

                // FOOTER
                doc.Add(new iTextSharp.text.Paragraph(" "));
                iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph(
                    "Xuất ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                    "  |  Tài liệu nội bộ — không gửi khách hàng",
                    fFooter);
                footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(footer);

                doc.Close();
                fs.Close();
            }
        }

        private void pnltop_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void panel4_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }

        private void frmPriceCalculation1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isClosing) return;
            isClosing = true;
        }
    }
}