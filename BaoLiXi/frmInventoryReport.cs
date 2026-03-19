// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmInventoryReport.cs - BÁO CÁO KHO                               ║
// ╚══════════════════════════════════════════════════════════════════════╝

using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryReport : Form
    {
        public frmInventoryReport()
        {
            InitializeComponent();
            this.Load += frmInventoryReport_Load;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD
        // ═══════════════════════════════════════════════════════════════
        private void frmInventoryReport_Load(object sender, EventArgs e)
        {
            // ComboBox loại báo cáo
            cboLoaiBaoCao.Items.Clear();
            cboLoaiBaoCao.Items.Add("-- Chọn loại báo cáo --");
            cboLoaiBaoCao.Items.Add("Sổ chi tiết nhập xuất tồn");
            cboLoaiBaoCao.Items.Add("Tổng hợp tồn kho");
            cboLoaiBaoCao.Items.Add("Thẻ kho");
            cboLoaiBaoCao.SelectedIndex = 0;
            cboLoaiBaoCao.SelectedIndexChanged += cboLoaiBaoCao_SelectedIndexChanged;

            // Mặc định khoảng thời gian tháng hiện tại
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDenNgay.Value = DateTime.Today;

            // Label tiêu đề ẩn ban đầu
            lblTieuDe.Visible = false;
            lblTieuDe.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(30, 64, 175);

            // Grid
            SetupGrid();
        }

        // ═══════════════════════════════════════════════════════════════
        // COMBOBOX CHỌN LOẠI → HIỆN TIÊU ĐỀ
        // ═══════════════════════════════════════════════════════════════
        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboLoaiBaoCao.SelectedIndex;
            if (idx <= 0)
            {
                lblTieuDe.Visible = false;
                return;
            }

            string[] titles = {
                "",
                "Sổ chi tiết nhập xuất tồn",
                "Tổng hợp tồn kho",
                "Thẻ kho"
            };

            lblTieuDe.Text = titles[idx];
            lblTieuDe.Visible = true;
            dgvBaoCaoCao.Rows.Clear();
            dgvBaoCaoCao.Columns.Clear();
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID (style chung)
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvBaoCaoCao;
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

        // ═══════════════════════════════════════════════════════════════
        // LẤY TIÊU ĐỀ BÁO CÁO THEO COMBOBOX
        // ═══════════════════════════════════════════════════════════════
        string GetReportTitle()
        {
            switch (cboLoaiBaoCao.SelectedIndex)
            {
                case 1: return "SỔ CHI TIẾT VẬT TƯ HÀNG HÓA";
                case 2: return "TỔNG HỢP TỒN KHO";
                case 3: return "THẺ KHO";
                default: return "";
            }
        }

        string GetReportShortName()
        {
            switch (cboLoaiBaoCao.SelectedIndex)
            {
                case 1: return "SoChiTiet";
                case 2: return "TongHopTonKho";
                case 3: return "TheKho";
                default: return "BaoCaoKho";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT XEM BÁO CÁO
        // ═══════════════════════════════════════════════════════════════
        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            int idx = cboLoaiBaoCao.SelectedIndex;
            if (idx <= 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn loại báo cáo!", "Thiếu thông tin");
                return;
            }

            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;

            if (tuNgay > denNgay)
            {
                MessageBox.Show("⚠️ Từ ngày phải nhỏ hơn hoặc bằng Đến ngày!", "Sai khoảng thời gian");
                return;
            }

            dgvBaoCaoCao.Rows.Clear();
            dgvBaoCaoCao.Columns.Clear();

            switch (idx)
            {
                case 1: LoadSoChiTietNhapXuatTon(tuNgay, denNgay); break;
                case 2: LoadTongHopTonKho(tuNgay, denNgay); break;
                case 3: LoadTheKho(tuNgay, denNgay); break;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // XUẤT EXCEL CHUNG CHO CÁC LOẠI BÁO CÁO
        // ═══════════════════════════════════════════════════════════════
        void ExportReportToExcel()
        {
            if (dgvBaoCaoCao.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Chưa có dữ liệu để xuất Excel!", "Thông báo");
                return;
            }

            if (cboLoaiBaoCao.SelectedIndex <= 0)
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
                    string title = GetReportTitle();
                    ws.Cells[r, 1].Value = title;
                    ws.Cells[r, 1, r, dgvBaoCaoCao.Columns.Count].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;

                    // Dòng khoảng thời gian
                    ws.Cells[r, 1].Value =
                        $"Kho: Nguyên Vật Liệu Giấy, từ ngày {dtpTuNgay.Value:dd/MM/yyyy} đến ngày {dtpDenNgay.Value:dd/MM/yyyy}";
                    ws.Cells[r, 1, r, dgvBaoCaoCao.Columns.Count].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true;
                    r += 2;

                    // Header cột từ DataGridView
                    for (int c = 0; c < dgvBaoCaoCao.Columns.Count; c++)
                    {
                        ws.Cells[r, c + 1].Value = dgvBaoCaoCao.Columns[c].HeaderText;
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 240, 255));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    r++;

                    // Dữ liệu
                    foreach (DataGridViewRow row in dgvBaoCaoCao.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int c = 0; c < dgvBaoCaoCao.Columns.Count; c++)
                        {
                            object val = row.Cells[c].Value;
                            ws.Cells[r, c + 1].Value = val;

                            ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            // canh phải nếu cột trong grid đang alignment phải
                            if (dgvBaoCaoCao.Columns[c].DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                                ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        r++;
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    string fileName = $"{GetReportShortName()}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        fileName);

                    pkg.SaveAs(new FileInfo(path));

                    MessageBox.Show($"✅ Đã xuất báo cáo ra Excel:\n{path}", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT IN BÁO CÁO
        // ═══════════════════════════════════════════════════════════════
        private void btnInBaoCao_Click(object sender, EventArgs e)
        {
            // Đơn giản: xuất Excel rồi in trong Excel
            ExportReportToExcel();
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT XUẤT EXCEL
        // ═══════════════════════════════════════════════════════════════
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            ExportReportToExcel();
        }

        // ═══════════════════════════════════════════════════════════════
        // BÁO CÁO 1: SỔ CHI TIẾT NHẬP XUẤT TỒN
        // ═══════════════════════════════════════════════════════════════
        // ═══════════════════════════════════════════════════════════════
        // BÁO CÁO 1: SỔ CHI TIẾT NHẬP XUẤT TỒN  (ĐÃ FIX TỒN ÂM)
        // ═══════════════════════════════════════════════════════════════
        // ═══════════════════════════════════════════════════════════════
        // BÁO CÁO 1: SỔ CHI TIẾT NHẬP XUẤT TỒN (ĐÃ CHỈNH LẠI TỔNG CỘNG)
        // ═══════════════════════════════════════════════════════════════
        void LoadSoChiTietNhapXuatTon(DateTime tuNgay, DateTime denNgay)
        {
            var dgv = dgvBaoCaoCao;

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
            var tongStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(220, 235, 255),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 120),
                Padding = new Padding(6, 0, 6, 0)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgay", HeaderText = "Ngày", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colChungTu", HeaderText = "Chứng từ", FillWeight = 13 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDienGiai", HeaderText = "Diễn giải", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNhap", HeaderText = "Nhập", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXuat", HeaderText = "Xuất", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTon", HeaderText = "Tồn", FillWeight = 10, DefaultCellStyle = boldStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", HeaderText = "Đơn giá", FillWeight = 12, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colGiaTriTon", HeaderText = "Giá trị tồn", FillWeight = 14, DefaultCellStyle = boldStyle });

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Vật tư có phát sinh trong kỳ
                    string sqlVatTu = @"
SELECT DISTINCT nl.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu,
       nl.Don_Vi_Tinh, nl.Gia_Nhap
FROM NGUYEN_LIEU nl
WHERE nl.id IN (
    SELECT id_Nguyen_Lieu FROM CHI_TIET_PHIEU_NHAP ctn
    JOIN PHIEU_NHAP_KHO pn ON pn.id = ctn.id_Phieu_Nhap
    WHERE pn.Ngay_Nhap BETWEEN @tu AND @den
    UNION
    SELECT id_Nguyen_Lieu FROM CHI_TIET_XUAT_KHO_SX ctx
    JOIN PHIEU_XUAT_KHO_SX px ON px.id = ctx.id_Phieu_Xuat
    WHERE px.Ngay_Xuat BETWEEN @tu AND @den
)
ORDER BY nl.Ma_Nguyen_Lieu";

                    var cmdVT = new SqlCommand(sqlVatTu, conn);
                    cmdVT.Parameters.AddWithValue("@tu", tuNgay);
                    cmdVT.Parameters.AddWithValue("@den", denNgay);
                    var rdVT = cmdVT.ExecuteReader();

                    var vatTuList = new System.Collections.Generic.List<(int id, string ma, string ten, string dvt, double gia)>();
                    while (rdVT.Read())
                    {
                        vatTuList.Add((
                            Convert.ToInt32(rdVT["id"]),
                            rdVT["Ma_Nguyen_Lieu"].ToString(),
                            rdVT["Ten_Nguyen_Lieu"].ToString(),
                            rdVT["Don_Vi_Tinh"].ToString(),
                            Convert.ToDouble(rdVT["Gia_Nhap"])
                        ));
                    }
                    rdVT.Close();

                    foreach (var vt in vatTuList)
                    {
                        // 1. Tính Nhập/Xuất trong kỳ + Tồn cuối để SUY RA Tồn đầu
                        double tongNhapKy = 0, tongXuatKy = 0, tonCuoi = 0;

                        string sqlAgg = @"
SELECT 
    ISNULL(SUM(CASE WHEN pn.Ngay_Nhap BETWEEN @tu AND @den 
                    THEN ct_n.So_Luong_Nhap END), 0) AS NhapKy,
    ISNULL(SUM(CASE WHEN px.Ngay_Xuat BETWEEN @tu AND @den 
                    THEN ct_x.So_Luong_Xuat END), 0) AS XuatKy,
    nl.Ton_Kho AS TonCuoi
FROM NGUYEN_LIEU nl
LEFT JOIN CHI_TIET_PHIEU_NHAP ct_n  ON ct_n.id_Nguyen_Lieu = nl.id
LEFT JOIN PHIEU_NHAP_KHO pn         ON pn.id = ct_n.id_Phieu_Nhap
LEFT JOIN CHI_TIET_XUAT_KHO_SX ct_x ON ct_x.id_Nguyen_Lieu = nl.id
LEFT JOIN PHIEU_XUAT_KHO_SX px      ON px.id = ct_x.id_Phieu_Xuat
WHERE nl.id = @idNL
GROUP BY nl.Ton_Kho";

                        var cmdAgg = new SqlCommand(sqlAgg, conn);
                        cmdAgg.Parameters.AddWithValue("@idNL", vt.id);
                        cmdAgg.Parameters.AddWithValue("@tu", tuNgay);
                        cmdAgg.Parameters.AddWithValue("@den", denNgay);
                        var rdAgg = cmdAgg.ExecuteReader();
                        if (rdAgg.Read())
                        {
                            tongNhapKy = Convert.ToDouble(rdAgg["NhapKy"]);
                            tongXuatKy = Convert.ToDouble(rdAgg["XuatKy"]);
                            tonCuoi = Convert.ToDouble(rdAgg["TonCuoi"]);
                        }
                        rdAgg.Close();

                        double tonDau = tonCuoi - tongNhapKy + tongXuatKy;
                        if (tonDau < 0) tonDau = 0;

                        // 2. Header vật tư
                        int gIdx = dgv.Rows.Add(
                            "", "", $"{vt.ma} - {vt.ten} (ĐVT: {vt.dvt})",
                            "", "", "", "", "");
                        for (int c = 0; c < dgv.Columns.Count; c++)
                            dgv.Rows[gIdx].Cells[c].Style = groupStyle;

                        double tonChay = tonDau;
                        double tongNhap = 0, tongXuat = 0;

                        // 3. Dòng Tồn đầu kỳ
                        int dkIdx = dgv.Rows.Add(
                            tuNgay.ToString("dd/MM/yyyy"),
                            "-",
                            "Tồn đầu kỳ",
                            "-", "-",
                            tonChay > 0 ? tonChay.ToString("N2") : "-",
                            vt.gia > 0 ? vt.gia.ToString("N0") : "-",
                            tonChay > 0 ? (tonChay * vt.gia).ToString("N0") : "-");
                        dgv.Rows[dkIdx].DefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);

                        // 4. Phát sinh NHẬP
                        string sqlNhap = @"
SELECT pn.Ngay_Nhap, pn.Ma_Phieu_Nhap,
       ncc.Ten_NCC,
       ct.So_Luong_Nhap, ct.Don_Gia_Nhap
FROM CHI_TIET_PHIEU_NHAP ct
JOIN PHIEU_NHAP_KHO pn ON pn.id = ct.id_Phieu_Nhap
LEFT JOIN NHA_CUNG_CAP ncc ON ncc.id = pn.id_Nha_Cung_Cap
WHERE ct.id_Nguyen_Lieu = @idNL
  AND pn.Ngay_Nhap BETWEEN @tu AND @den
ORDER BY pn.Ngay_Nhap";
                        var cmdN = new SqlCommand(sqlNhap, conn);
                        cmdN.Parameters.AddWithValue("@idNL", vt.id);
                        cmdN.Parameters.AddWithValue("@tu", tuNgay);
                        cmdN.Parameters.AddWithValue("@den", denNgay);
                        var rdN = cmdN.ExecuteReader();
                        while (rdN.Read())
                        {
                            double sl = Convert.ToDouble(rdN["So_Luong_Nhap"]);
                            double dg = Convert.ToDouble(rdN["Don_Gia_Nhap"]);
                            tonChay += sl;
                            tongNhap += sl;
                            dgv.Rows.Add(
                                Convert.ToDateTime(rdN["Ngay_Nhap"]).ToString("dd/MM/yyyy"),
                                rdN["Ma_Phieu_Nhap"].ToString(),
                                $"Nhập từ {rdN["Ten_NCC"]}",
                                sl.ToString("N2"), "-",
                                tonChay.ToString("N2"),
                                dg.ToString("N0"),
                                (tonChay * dg).ToString("N0"));
                        }
                        rdN.Close();

                        // 5. Phát sinh XUẤT
                        string sqlXuat = @"
SELECT px.Ngay_Xuat, px.Ma_Phieu_Xuat,
       lsx.Ma_Lenh_SX,
       ct.So_Luong_Xuat, ct.Don_Gia
FROM CHI_TIET_XUAT_KHO_SX ct
JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct.id_Phieu_Xuat
LEFT JOIN LENH_SAN_XUAT lsx ON lsx.id = px.id_Lenh_San_Xuat
WHERE ct.id_Nguyen_Lieu = @idNL
  AND px.Ngay_Xuat BETWEEN @tu AND @den
ORDER BY px.Ngay_Xuat";
                        var cmdX = new SqlCommand(sqlXuat, conn);
                        cmdX.Parameters.AddWithValue("@idNL", vt.id);
                        cmdX.Parameters.AddWithValue("@tu", tuNgay);
                        cmdX.Parameters.AddWithValue("@den", denNgay);
                        var rdX = cmdX.ExecuteReader();
                        while (rdX.Read())
                        {
                            double sl = Convert.ToDouble(rdX["So_Luong_Xuat"]);
                            double dg = Convert.ToDouble(rdX["Don_Gia"]);
                            tonChay -= sl;
                            tongXuat += sl;
                            dgv.Rows.Add(
                                Convert.ToDateTime(rdX["Ngay_Xuat"]).ToString("dd/MM/yyyy"),
                                rdX["Ma_Phieu_Xuat"].ToString(),
                                $"Xuất cho {rdX["Ma_Lenh_SX"]}",
                                "-", sl.ToString("N2"),
                                tonChay.ToString("N2"),
                                dg.ToString("N0"),
                                (tonChay * dg).ToString("N0"));
                        }
                        rdX.Close();

                        // 6. Dòng TỔNG CỘNG cho mã hàng
                        // Nhập = tổng phát sinh nhập trong kỳ
                        // Xuất = tổng phát sinh xuất trong kỳ
                        // Tồn = tồn cuối (tonChay)
                        int tIdx = dgv.Rows.Add(
                            "", "", $"Tổng cộng {vt.ma}",
                            tongNhap > 0 ? tongNhap.ToString("N2") : "-",
                            tongXuat > 0 ? tongXuat.ToString("N2") : "-",
                            tonChay.ToString("N2"),
                            "-",
                            (tonChay * vt.gia).ToString("N0"));
                        for (int c = 0; c < dgv.Columns.Count; c++)
                            dgv.Rows[tIdx].Cells[c].Style = tongStyle;

                        // Dòng trống ngăn cách
                        dgv.Rows.Add("", "", "", "", "", "", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải báo cáo:\n{ex.Message}", "Lỗi");
            }
        }


        // ═══════════════════════════════════════════════════════════════
        // BÁO CÁO 2: TỔNG HỢP TỒN KHO
        // ═══════════════════════════════════════════════════════════════
        void LoadTongHopTonKho(DateTime tuNgay, DateTime denNgay)
        {
            var dgv = dgvBaoCaoCao;

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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMa", HeaderText = "Mã hàng", FillWeight = 10 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên hàng", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDVT", HeaderText = "ĐVT", FillWeight = 7 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonDau", HeaderText = "Tồn đầu", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNhapKy", HeaderText = "Nhập kỳ", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXuatKy", HeaderText = "Xuất kỳ", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonCuoi", HeaderText = "Tồn cuối", FillWeight = 10, DefaultCellStyle = boldRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", HeaderText = "Đơn giá", FillWeight = 11, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colGiaTriTon", HeaderText = "Giá trị tồn", FillWeight = 12, DefaultCellStyle = boldRight });

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT 
    nl.Ma_Nguyen_Lieu,
    nl.Ten_Nguyen_Lieu,
    nl.Don_Vi_Tinh,
    ISNULL(nl.Ton_Dau, 0)                                           AS Ton_Dau,
    ISNULL(SUM(CASE WHEN pn.Ngay_Nhap BETWEEN @tu AND @den 
                    THEN ct_n.So_Luong_Nhap END), 0)                AS Nhap_Ky,
    ISNULL(SUM(CASE WHEN px.Ngay_Xuat BETWEEN @tu AND @den 
                    THEN ct_x.So_Luong_Xuat END), 0)                AS Xuat_Ky,
    nl.Ton_Kho                                                      AS Ton_Cuoi,
    nl.Gia_Nhap
FROM NGUYEN_LIEU nl
LEFT JOIN CHI_TIET_PHIEU_NHAP ct_n  ON ct_n.id_Nguyen_Lieu = nl.id
LEFT JOIN PHIEU_NHAP_KHO pn         ON pn.id = ct_n.id_Phieu_Nhap
LEFT JOIN CHI_TIET_XUAT_KHO_SX ct_x ON ct_x.id_Nguyen_Lieu = nl.id
LEFT JOIN PHIEU_XUAT_KHO_SX px      ON px.id = ct_x.id_Phieu_Xuat
GROUP BY nl.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu,
         nl.Don_Vi_Tinh, nl.Ton_Dau, nl.Ton_Kho, nl.Gia_Nhap
ORDER BY nl.Ma_Nguyen_Lieu";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tu", tuNgay);
                    cmd.Parameters.AddWithValue("@den", denNgay);
                    var rd = cmd.ExecuteReader();

                    double tongGiaTri = 0;
                    while (rd.Read())
                    {
                        double tonDau = Convert.ToDouble(rd["Ton_Dau"]);
                        double nhapKy = Convert.ToDouble(rd["Nhap_Ky"]);
                        double xuatKy = Convert.ToDouble(rd["Xuat_Ky"]);
                        double tonCuoi = Convert.ToDouble(rd["Ton_Cuoi"]);
                        double donGia = Convert.ToDouble(rd["Gia_Nhap"]);
                        double gtTon = tonCuoi * donGia;
                        tongGiaTri += gtTon;

                        int idx = dgv.Rows.Add(
                            rd["Ma_Nguyen_Lieu"],
                            rd["Ten_Nguyen_Lieu"],
                            rd["Don_Vi_Tinh"],
                            tonDau > 0 ? tonDau.ToString("N2") : "-",
                            nhapKy > 0 ? nhapKy.ToString("N2") : "-",
                            xuatKy > 0 ? xuatKy.ToString("N2") : "-",
                            tonCuoi.ToString("N2"),
                            donGia > 0 ? donGia.ToString("N0") : "-",
                            gtTon > 0 ? gtTon.ToString("N0") : "-");

                        if (tonCuoi <= 0)
                        {
                            dgv.Rows[idx].Cells["colTonCuoi"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                            dgv.Rows[idx].Cells["colTonCuoi"].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                        }
                    }
                    rd.Close();

                    int tIdx = dgv.Rows.Add("", "TỔNG CỘNG", "", "", "", "", "", "",
                        tongGiaTri.ToString("N0"));
                    for (int c = 0; c < dgv.Columns.Count; c++)
                    {
                        dgv.Rows[tIdx].Cells[c].Style.BackColor = Color.FromArgb(220, 235, 255);
                        dgv.Rows[tIdx].Cells[c].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                        dgv.Rows[tIdx].Cells[c].Style.ForeColor = Color.FromArgb(20, 40, 120);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải báo cáo:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // BÁO CÁO 3: THẺ KHO
        // ═══════════════════════════════════════════════════════════════
        void LoadTheKho(DateTime tuNgay, DateTime denNgay)
        {
            var dgv = dgvBaoCaoCao;

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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgay", HeaderText = "Ngày", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colChungTu", HeaderText = "Chứng từ", FillWeight = 14 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDienGiai", HeaderText = "Diễn giải", FillWeight = 28 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNhap", HeaderText = "Nhập", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXuat", HeaderText = "Xuất", FillWeight = 10, DefaultCellStyle = rightStyle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonLuyKe", HeaderText = "Tồn lũy kế", FillWeight = 12, DefaultCellStyle = boldRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVatTu", HeaderText = "Vật tư", FillWeight = 16 });

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = @"
SELECT nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh,
       ISNULL(nl.Ton_Dau, 0) AS Ton_Dau,
       pn.Ngay_Nhap        AS Ngay,
       pn.Ma_Phieu_Nhap    AS ChungTu,
       N'Nhập từ ' + ISNULL(ncc.Ten_NCC, N'NCC') AS DienGiai,
       ct_n.So_Luong_Nhap  AS Nhap,
       0                   AS Xuat
FROM CHI_TIET_PHIEU_NHAP ct_n
JOIN PHIEU_NHAP_KHO pn    ON pn.id  = ct_n.id_Phieu_Nhap
JOIN NGUYEN_LIEU nl        ON nl.id  = ct_n.id_Nguyen_Lieu
LEFT JOIN NHA_CUNG_CAP ncc ON ncc.id = pn.id_Nha_Cung_Cap
WHERE pn.Ngay_Nhap BETWEEN @tu AND @den

UNION ALL

SELECT nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh,
       ISNULL(nl.Ton_Dau, 0) AS Ton_Dau,
       px.Ngay_Xuat          AS Ngay,
       px.Ma_Phieu_Xuat      AS ChungTu,
       N'Xuất cho ' + ISNULL(lsx.Ma_Lenh_SX, N'LSX') AS DienGiai,
       0                     AS Nhap,
       ct_x.So_Luong_Xuat    AS Xuat
FROM CHI_TIET_XUAT_KHO_SX ct_x
JOIN PHIEU_XUAT_KHO_SX px   ON px.id  = ct_x.id_Phieu_Xuat
JOIN NGUYEN_LIEU nl          ON nl.id  = ct_x.id_Nguyen_Lieu
LEFT JOIN LENH_SAN_XUAT lsx  ON lsx.id = px.id_Lenh_San_Xuat
WHERE px.Ngay_Xuat BETWEEN @tu AND @den

ORDER BY Ma_Nguyen_Lieu, Ngay";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tu", tuNgay);
                    cmd.Parameters.AddWithValue("@den", denNgay);
                    var rd = cmd.ExecuteReader();

                    string curMa = "";
                    double tonChay = 0;

                    while (rd.Read())
                    {
                        string ma = rd["Ma_Nguyen_Lieu"].ToString();
                        string ten = rd["Ten_Nguyen_Lieu"].ToString();
                        string dvt = rd["Don_Vi_Tinh"].ToString();

                        if (ma != curMa)
                        {
                            if (curMa != "") dgv.Rows.Add("", "", "", "", "", "", "");
                            curMa = ma;
                            tonChay = Convert.ToDouble(rd["Ton_Dau"]);

                            int gIdx = dgv.Rows.Add(
                                "", "", $"{ma} - {ten} (ĐVT: {dvt})",
                                "", "", "", "");
                            for (int c = 0; c < dgv.Columns.Count; c++)
                            {
                                dgv.Rows[gIdx].Cells[c].Style.BackColor = Color.FromArgb(235, 245, 255);
                                dgv.Rows[gIdx].Cells[c].Style.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                                dgv.Rows[gIdx].Cells[c].Style.ForeColor = Color.FromArgb(30, 64, 175);
                            }

                            dgv.Rows.Add(
                                tuNgay.ToString("dd/MM/yyyy"), "-", "Tồn đầu kỳ",
                                "-", "-",
                                tonChay > 0 ? tonChay.ToString("N2") : "0",
                                $"{ma} - {ten}");
                        }

                        double nhap = Convert.ToDouble(rd["Nhap"]);
                        double xuat = Convert.ToDouble(rd["Xuat"]);
                        tonChay += nhap - xuat;

                        dgv.Rows.Add(
                            Convert.ToDateTime(rd["Ngay"]).ToString("dd/MM/yyyy"),
                            rd["ChungTu"],
                            rd["DienGiai"],
                            nhap > 0 ? nhap.ToString("N2") : "-",
                            xuat > 0 ? xuat.ToString("N2") : "-",
                            tonChay.ToString("N2"),
                            $"{ma} - {ten}");
                    }
                    rd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải báo cáo:\n{ex.Message}", "Lỗi");
            }
        }
    }
}