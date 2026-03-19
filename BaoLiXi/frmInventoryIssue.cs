using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryIssue : Form
    {
        public frmInventoryIssue()
        {
            InitializeComponent();
            this.Load += frmInventoryIssue_Load;
        }

        private void frmInventoryIssue_Load(object sender, EventArgs e)
        {
            SetupGridTonKho();
            SetupGridSapHet();
            LoadComboBox();
            LoadData();
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID TỒN KHO
        // ═══════════════════════════════════════════════════════════════
        void SetupGridTonKho()
        {
            var dgv = dgvTonKho;
            dgv.Columns.Clear();
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
            dgv.RowTemplate.Height = 38;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 4, 0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f) };
            var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9.5f), Padding = new Padding(0, 0, 8, 0) };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", FillWeight = 5, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaHang", HeaderText = "Mã hàng", FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 100, 200), Padding = new Padding(6, 0, 4, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTenHang", HeaderText = "Tên hàng", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f), Padding = new Padding(6, 0, 4, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDVT", HeaderText = "ĐVT", FillWeight = 6, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonDau", HeaderText = "Tồn đầu", FillWeight = 8, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNhapKy", HeaderText = "Nhập trong kỳ", FillWeight = 9, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colXuatKy", HeaderText = "Xuất trong kỳ", FillWeight = 9, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonCuoi", HeaderText = "Tồn cuối", FillWeight = 8, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", HeaderText = "Đơn giá", FillWeight = 10, DefaultCellStyle = right });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colGiaTriTon", HeaderText = "Giá trị tồn", FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Padding = new Padding(0, 0, 8, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTrangThai", HeaderText = "Trạng thái", FillWeight = 10, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonCuoiRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colGiaTriRaw", Visible = false });

            dgv.CellPainting += DgvTonKho_CellPainting;
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID SẮP HẾT
        // ═══════════════════════════════════════════════════════════════
        void SetupGridSapHet()
        {
            var dgv = dgvSapHet;
            dgv.Columns.Clear();
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
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f) };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2MaHang", HeaderText = "Mã hàng", FillWeight = 15, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 100, 200), Padding = new Padding(6, 0, 4, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2TenHang", HeaderText = "Tên hàng", FillWeight = 25, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f), Padding = new Padding(6, 0, 4, 0) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2TonHienTai", HeaderText = "Tồn hiện tại", FillWeight = 13, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2DVT", HeaderText = "ĐVT", FillWeight = 8, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2MucTonToiThieu", HeaderText = "Mức tồn tối thiểu", FillWeight = 13, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2DeXuatNhap", HeaderText = "Đề xuất nhập", FillWeight = 13, DefaultCellStyle = center });

            var btnCol = new DataGridViewButtonColumn
            {
                Name = "col2TaoDonh",
                HeaderText = "Thao tác",
                Text = "🛒 Tạo đơn mua",
                UseColumnTextForButtonValue = true,
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(4) },
                FlatStyle = FlatStyle.Flat
            };
            dgv.Columns.Add(btnCol);
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "col2IdNL", Visible = false });
            dgv.CellContentClick += DgvSapHet_CellContentClick;
        }

        // ═══════════════════════════════════════════════════════════════
        // COMBOBOX
        // ═══════════════════════════════════════════════════════════════
        void LoadComboBox()
        {
            cboLoaiVatTu.Items.Clear();
            cboLoaiVatTu.Items.AddRange(new string[] { "Tất cả", "Giấy", "Mực in", "Vật tư phụ", "Thùng đóng hàng", "Khác" });
            cboLoaiVatTu.SelectedIndex = 0;

            cboTrangThaiTon.Items.Clear();
            cboTrangThaiTon.Items.AddRange(new string[] { "Tất cả", "Còn hàng", "Sắp hết", "Hết hàng" });
            cboTrangThaiTon.SelectedIndex = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD DATA TỒN KHO
        // ═══════════════════════════════════════════════════════════════
        void LoadData()
        {
            dgvTonKho.Rows.Clear();
            dgvSapHet.Rows.Clear();

            string tuKhoa = txtTimKiem.Text.Trim();
            string trangThai = cboTrangThaiTon.SelectedItem?.ToString() ?? "Tất cả";

            // lọc theo loại vật tư bằng chữ cái đầu Mã hàng
            string loai = cboLoaiVatTu.SelectedItem?.ToString() ?? "Tất cả";
            string prefix = "";
            switch (loai)
            {
                case "Giấy": prefix = "G"; break;
                case "Mực in": prefix = "M"; break;
                case "Vật tư phụ": prefix = "P"; break;
                case "Thùng đóng hàng": prefix = "T"; break;
                case "Khác": prefix = ""; break;
                default: prefix = ""; break;
            }

            int tongMatHang = 0;
            double tongGiaTri = 0;
            int sapHet = 0;
            int tonQuaLau = 0;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = @"
SELECT
    nl.id,
    nl.Ma_Nguyen_Lieu,
    nl.Ten_Nguyen_Lieu,
    nl.Don_Vi_Tinh,
    ISNULL((SELECT SUM(ct.So_Luong_Da_Nhan)
            FROM CHI_TIET_DON_HANG_NCC ct
            WHERE ct.id_Nguyen_Lieu = nl.id
              AND ct.So_Luong_Da_Nhan > 0), 0)  AS Nhap_Ky,
    0                                            AS Xuat_Ky,
    ISNULL(nl.Ton_Kho, 0)                        AS Ton_Cuoi,
    nl.Gia_Nhap,
    nl.Ton_Kho_Toi_Thieu,
    ISNULL(nl.De_Xuat_Nhap, nl.Ton_Kho_Toi_Thieu * 2) AS De_Xuat_Nhap
FROM NGUYEN_LIEU nl
WHERE
    (@tuKhoa = '' OR nl.Ma_Nguyen_Lieu LIKE @tuKhoa OR nl.Ten_Nguyen_Lieu LIKE @tuKhoa)
    AND (@prefix = '' OR nl.Ma_Nguyen_Lieu LIKE @prefix + '%')
ORDER BY nl.Ma_Nguyen_Lieu";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tuKhoa", tuKhoa == "" ? "" : "%" + tuKhoa + "%");
                    cmd.Parameters.AddWithValue("@prefix", prefix ?? "");

                    var rd = cmd.ExecuteReader();
                    int stt = 1;

                    while (rd.Read())
                    {
                        int id = Convert.ToInt32(rd["id"]);
                        string ma = rd["Ma_Nguyen_Lieu"].ToString();
                        string ten = rd["Ten_Nguyen_Lieu"].ToString();
                        string dvt = rd["Don_Vi_Tinh"].ToString();
                        double nhapKy = Convert.ToDouble(rd["Nhap_Ky"]);
                        double xuatKy = Convert.ToDouble(rd["Xuat_Ky"]);
                        double tonCuoi = Convert.ToDouble(rd["Ton_Cuoi"]);
                        double donGia = Convert.ToDouble(rd["Gia_Nhap"]);
                        double tonToiThieu = Convert.ToDouble(rd["Ton_Kho_Toi_Thieu"]);
                        double deXuat = Convert.ToDouble(rd["De_Xuat_Nhap"]);

                        double tonDau = tonCuoi - nhapKy + xuatKy;
                        if (tonDau < 0) tonDau = 0;
                        double giaTriTon = tonCuoi * donGia;

                        string trangThaiItem;
                        if (tonCuoi <= 0)
                            trangThaiItem = "HẾT HÀNG";
                        else if (tonCuoi <= tonToiThieu)
                            trangThaiItem = "SẮP HẾT";
                        else
                            trangThaiItem = "CÒN HÀNG";

                        if (trangThai != "Tất cả")
                        {
                            if (trangThai == "Còn hàng" && trangThaiItem != "CÒN HÀNG") continue;
                            if (trangThai == "Sắp hết" && trangThaiItem != "SẮP HẾT") continue;
                            if (trangThai == "Hết hàng" && trangThaiItem != "HẾT HÀNG") continue;
                        }

                        int idx = dgvTonKho.Rows.Add(
                            stt++, ma, ten, dvt,
                            tonDau.ToString("N0"),
                            nhapKy.ToString("N0"),
                            xuatKy.ToString("N0"),
                            tonCuoi.ToString("N0"),
                            donGia.ToString("N0"),
                            giaTriTon.ToString("N0"),
                            trangThaiItem,
                            id, tonCuoi, giaTriTon
                        );

                        var row = dgvTonKho.Rows[idx];
                        if (tonCuoi <= 0)
                            row.Cells["colTonCuoi"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                        else if (tonCuoi <= tonToiThieu)
                            row.Cells["colTonCuoi"].Style.ForeColor = Color.FromArgb(234, 88, 12);

                        tongMatHang++;
                        tongGiaTri += giaTriTon;
                        if (trangThaiItem == "SẮP HẾT") sapHet++;
                        if (trangThaiItem == "HẾT HÀNG") sapHet++;

                        if (trangThaiItem != "CÒN HÀNG")
                        {
                            dgvSapHet.Rows.Add(
                                ma, ten,
                                tonCuoi.ToString("N0") + " " + dvt,
                                dvt,
                                tonToiThieu.ToString("N0") + " " + dvt,
                                deXuat.ToString("N0") + " " + dvt,
                                "🛒 Tạo đơn mua",
                                id
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblTongMatHang.Text = tongMatHang.ToString();
            lblGiaTriTonKho.Text = FormatTien(tongGiaTri);
            lblSapHetHang.Text = sapHet.ToString();
            lblTonQuaLau.Text = tonQuaLau.ToString();
            lblTongGiaTri.Text = "Tổng giá trị tồn kho:";
            lblTongGiaTriSo.Text = tongGiaTri.ToString("N0") + " đ";

            foreach (DataGridViewRow row in dgvSapHet.Rows)
            {
                string val = row.Cells["col2TonHienTai"].Value?.ToString() ?? "";
                if (val.StartsWith("0"))
                    row.Cells["col2TonHienTai"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                else
                    row.Cells["col2TonHienTai"].Style.ForeColor = Color.FromArgb(234, 88, 12);
            }
        }

        string FormatTien(double value)
        {
            if (value >= 1_000_000_000) return $"{value / 1_000_000_000:0.#} tỷ";
            if (value >= 1_000_000) return $"{value / 1_000_000:0.#} triệu";
            return value.ToString("N0");
        }

        // ═══════════════════════════════════════════════════════════════
        // VẼ BADGE TRẠNG THÁI
        // ═══════════════════════════════════════════════════════════════
        private void DgvTonKho_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvTonKho.Columns[e.ColumnIndex].Name != "colTrangThai") return;

            e.PaintBackground(e.ClipBounds, true);
            string val = e.Value?.ToString() ?? "";
            Color bgColor, fgColor;
            switch (val)
            {
                case "CÒN HÀNG": bgColor = Color.FromArgb(220, 252, 231); fgColor = Color.FromArgb(21, 128, 61); break;
                case "SẮP HẾT": bgColor = Color.FromArgb(254, 243, 199); fgColor = Color.FromArgb(146, 64, 14); break;
                default: bgColor = Color.FromArgb(254, 226, 226); fgColor = Color.FromArgb(185, 28, 28); break;
            }
            int px = e.CellBounds.X + (e.CellBounds.Width - 70) / 2;
            int py = e.CellBounds.Y + (e.CellBounds.Height - 22) / 2;
            var badgeRect = new Rectangle(px, py, 70, 22);
            using (var brush = new SolidBrush(bgColor))
            using (var pen = new Pen(bgColor))
            {
                e.Graphics.FillRectangle(brush, badgeRect);
                e.Graphics.DrawRectangle(pen, badgeRect);
            }
            using (var fBrush = new SolidBrush(fgColor))
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(val, font, fBrush, badgeRect, sf);
            }
            e.Handled = true;
        }

        // ═══════════════════════════════════════════════════════════════
        // GRID SẮP HẾT: CLICK TẠO ĐƠN
        // ═══════════════════════════════════════════════════════════════
        private void DgvSapHet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvSapHet.Columns[e.ColumnIndex].Name != "col2TaoDonh") return;
            int idNL = Convert.ToInt32(dgvSapHet.Rows[e.RowIndex].Cells["col2IdNL"].Value);
            string maHang = dgvSapHet.Rows[e.RowIndex].Cells["col2MaHang"].Value?.ToString();
            string tenHang = dgvSapHet.Rows[e.RowIndex].Cells["col2TenHang"].Value?.ToString();
            var frm = new frmPurchaseOrder(idNL, maHang, tenHang);
            frm.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT TÌM KIẾM / FILTER
        // ═══════════════════════════════════════════════════════════════
        private void btnTimKiem_Click(object sender, EventArgs e) => LoadData();
        private void txtTimKiem_TextChanged(object sender, EventArgs e) => LoadData();
        private void cboLoaiVatTu_SelectedIndexChanged(object sender, EventArgs e) => LoadData();
        private void cboTrangThaiTon_SelectedIndexChanged(object sender, EventArgs e) => LoadData();

        // ═══════════════════════════════════════════════════════════════
        // NÚT XUẤT EXCEL – BÁO CÁO TỒN KHO
        // ═══════════════════════════════════════════════════════════════
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvTonKho.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Không có dữ liệu để xuất Excel!", "Thông báo");
                return;
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("BaoCaoTonKho");
                    int r = 1;

                    ws.Cells[r, 1].Value = "TỔNG HỢP TỒN KHO";
                    ws.Cells[r, 1, r, 11].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;

                    ws.Cells[r, 1].Value = "Kho: Nguyên Vật Liệu Giấy, Tháng " + DateTime.Today.ToString("MM/yyyy");
                    ws.Cells[r, 1, r, 11].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true;
                    r += 2;

                    string[] headers = {
                        "Tên kho", "Mã hàng", "Tên hàng", "ĐVT",
                        "Đầu kỳ SL", "Đầu kỳ Giá trị",
                        "Nhập kho SL", "Nhập kho Giá trị",
                        "Xuất kho SL", "Xuất kho Giá trị",
                        "Cuối kỳ SL", "Cuối kỳ Giá trị"
                    };

                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cells[r, c + 1].Value = headers[c];
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 240, 255));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    r++;

                    foreach (DataGridViewRow row in dgvTonKho.Rows)
                    {
                        string ma = row.Cells["colMaHang"].Value?.ToString() ?? "";
                        string ten = row.Cells["colTenHang"].Value?.ToString() ?? "";
                        string dvt = row.Cells["colDVT"].Value?.ToString() ?? "";
                        double.TryParse(row.Cells["colTonDau"].Value?.ToString().Replace(",", ""), out double tonDau);
                        double.TryParse(row.Cells["colNhapKy"].Value?.ToString().Replace(",", ""), out double nhapKy);
                        double.TryParse(row.Cells["colXuatKy"].Value?.ToString().Replace(",", ""), out double xuatKy);
                        double.TryParse(row.Cells["colTonCuoi"].Value?.ToString().Replace(",", ""), out double tonCuoi);
                        double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double donGia);

                        double dauKyGT = tonDau * donGia;
                        double nhapGT = nhapKy * donGia;
                        double xuatGT = xuatKy * donGia;
                        double cuoiKyGT = tonCuoi * donGia;

                        ws.Cells[r, 1].Value = "Nguyên Vật Liệu Giấy";
                        ws.Cells[r, 2].Value = ma;
                        ws.Cells[r, 3].Value = ten;
                        ws.Cells[r, 4].Value = dvt;

                        ws.Cells[r, 5].Value = tonDau;
                        ws.Cells[r, 6].Value = dauKyGT;
                        ws.Cells[r, 7].Value = nhapKy;
                        ws.Cells[r, 8].Value = nhapGT;
                        ws.Cells[r, 9].Value = xuatKy;
                        ws.Cells[r, 10].Value = xuatGT;
                        ws.Cells[r, 11].Value = tonCuoi;
                        ws.Cells[r, 12].Value = cuoiKyGT;

                        ws.Cells[r, 5, r, 12].Style.Numberformat.Format = "#,##0";
                        for (int c = 1; c <= 12; c++)
                        {
                            ws.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                        r++;
                    }

                    ws.Column(1).Width = 18; // Tên kho
                    ws.Column(2).Width = 12; // Mã hàng
                    ws.Column(3).Width = 28; // Tên hàng
                    ws.Column(4).Width = 8;  // ĐVT
                    for (int c = 5; c <= 12; c++) ws.Column(c).Width = 14;

                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"BAO_CAO_TON_KHO_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    pkg.SaveAs(new FileInfo(path));

                    MessageBox.Show($"✅ Đã xuất báo cáo tồn kho ra Excel:\n{path}", "Thành công",
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
                MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT KIỂM KHO – THỐNG KÊ NHANH
        // ═══════════════════════════════════════════════════════════════
        private void btnKiemKho_Click(object sender, EventArgs e)
        {
            if (dgvTonKho.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu tồn kho. Vui lòng tải dữ liệu trước.", "Thông báo");
                return;
            }

            int hetHang = 0, sapHet = 0, conHang = 0;
            foreach (DataGridViewRow row in dgvTonKho.Rows)
            {
                string tt = row.Cells["colTrangThai"].Value?.ToString() ?? "";
                if (tt == "HẾT HÀNG") hetHang++;
                else if (tt == "SẮP HẾT") sapHet++;
                else if (tt == "CÒN HÀNG") conHang++;
            }

            MessageBox.Show(
                $"Tổng mặt hàng: {dgvTonKho.Rows.Count}\n" +
                $"Còn hàng: {conHang}\n" +
                $"Sắp hết: {sapHet}\n" +
                $"Hết hàng: {hetHang}",
                "Kết quả kiểm kho",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            if (hetHang > 0 && dgvSapHet.Rows.Count > 0)
            {
                dgvSapHet.ClearSelection();
                dgvSapHet.Rows[0].Selected = true;
                dgvSapHet.FirstDisplayedScrollingRowIndex = 0;
            }
        }
    }
}