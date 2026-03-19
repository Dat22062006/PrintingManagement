// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmProductionOrder.cs - LỆNH SẢN XUẤT (ĐẦY ĐỦ)                    ║
// ║  - Chọn báo giá đã duyệt                                            ║
// ║  - Kiểm tra kho                                                      ║
// ║  - Bắt đầu sản xuất → Lưu vào LENH_SAN_XUAT                        ║
// ║  - Xuất Excel lệnh SX                                               ║
// ╚══════════════════════════════════════════════════════════════════════╝

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
    public partial class frmProductionOrder : Form
    {
        private int _idBaoGia = 0;
        private int _idKhachHang = 0;
        private string _maBaoGia = "";

        public frmProductionOrder()
        {
            InitializeComponent();
            this.Load += frmProductionOrder_Load;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD FORM
        // ═══════════════════════════════════════════════════════════════
        private void frmProductionOrder_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadCboBaoGia();
            dtpNgayBatDau.Value = DateTime.Today;
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID VẬT TƯ
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvVatTu;
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

            var rightStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            var centerStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            // Vật tư
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVatTu",
                HeaderText = "Vật tư",
                FillWeight = 55,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10f),
                    Padding = new Padding(8, 0, 6, 0),
                    BackColor = Color.FromArgb(255, 251, 220)
                }
            });

            // ĐVT
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonVi",
                HeaderText = "ĐVT",
                FillWeight = 15,
                DefaultCellStyle = centerStyle
            });

            // Tồn kho
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTonKho",
                HeaderText = "Tồn kho",
                FillWeight = 20,
                DefaultCellStyle = rightStyle
            });

            // ID ẩn
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdNL",
                Visible = false
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD COMBOBOX BÁO GIÁ ĐÃ DUYỆT
        // ═══════════════════════════════════════════════════════════════
        void LoadCboBaoGia()
        {
            cboBaoGia.SelectedIndexChanged -= cboBaoGia_SelectedIndexChanged;
            cboBaoGia.Items.Clear();
            cboBaoGia.Items.Add(new BaoGiaItem());

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham,
       kh.Ten_Khach_Hang,
       ISNULL(MAX(ct.So_Luong), 0) AS So_Luong
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
WHERE bg.Trang_Thai = N'Đã duyệt'
GROUP BY bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, kh.Ten_Khach_Hang
ORDER BY bg.id DESC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        cboBaoGia.Items.Add(new BaoGiaItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Ma = rd["Ma_Bao_Gia"].ToString(),
                            TenSanPham = rd["Ten_San_Pham"].ToString(),
                            TenKH = rd["Ten_Khach_Hang"].ToString(),
                            SoLuong = Convert.ToInt32(rd["So_Luong"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load báo giá:\n{ex.Message}", "Lỗi");
            }

            cboBaoGia.SelectedIndexChanged += cboBaoGia_SelectedIndexChanged;
            cboBaoGia.SelectedIndex = 0;
        }

        private void cboBaoGia_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Có thể tự động load khi chọn (tùy chọn)
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT LẤY DỮ LIỆU
        // ═══════════════════════════════════════════════════════════════
        private void btnLayDuLieu_Click(object sender, EventArgs e)
        {
            if (!(cboBaoGia.SelectedItem is BaoGiaItem bg) || bg.Id == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn số báo giá!", "Thiếu thông tin");
                return;
            }

            _idBaoGia = bg.Id;
            _maBaoGia = bg.Ma;
            _idKhachHang = 0;

            dgvVatTu.Rows.Clear();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sqlBG = @"
SELECT bg.Ten_San_Pham, bg.id_Khach_Hang,
       kh.Ten_Khach_Hang,
       ISNULL(MAX(ct.So_Luong), 0) AS So_Luong
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
WHERE bg.id = @id
GROUP BY bg.Ten_San_Pham, bg.id_Khach_Hang, kh.Ten_Khach_Hang";

                    var cmd = new SqlCommand(sqlBG, conn);
                    cmd.Parameters.AddWithValue("@id", _idBaoGia);
                    var rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        txtTenSanPham.Text = rd["Ten_San_Pham"].ToString();
                        txtSoLuong.Text = rd["So_Luong"].ToString();
                        txtKhachHang.Text = rd["Ten_Khach_Hang"].ToString();
                        _idKhachHang = rd["id_Khach_Hang"] != DBNull.Value
                            ? Convert.ToInt32(rd["id_Khach_Hang"])
                            : 0;
                    }
                    rd.Close();

                    MessageBox.Show("✅ Đã lấy dữ liệu thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi lấy dữ liệu:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT KIỂM TRA KHO
        // ═══════════════════════════════════════════════════════════════
        private void btnKiemTraKho_Click(object sender, EventArgs e)
        {
            if (_idBaoGia == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn báo giá và bấm Lấy dữ liệu trước!",
                    "Thiếu thông tin");
                return;
            }

            dgvVatTu.Rows.Clear();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // ⭐ Query đúng tên cột
                    string sql = @"
SELECT 
    nl.id, 
    nl.Ten_Nguyen_Lieu,
    nl.Don_Vi_Tinh, 
    nl.Ton_Kho
FROM NGUYEN_LIEU nl
WHERE nl.Ton_Kho >= 0
ORDER BY nl.Ma_Nguyen_Lieu";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();

                    while (rd.Read())
                    {
                        int id = Convert.ToInt32(rd["id"]);
                        string ten = rd["Ten_Nguyen_Lieu"].ToString();
                        string dvt = rd["Don_Vi_Tinh"].ToString();
                        double tonKho = Convert.ToDouble(rd["Ton_Kho"]);

                        int idx = dgvVatTu.Rows.Add(ten, dvt, tonKho.ToString("N0"), id);

                        // Tô đỏ nếu tồn kho <= 1
                        if (tonKho <= 1)
                        {
                            dgvVatTu.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                            dgvVatTu.Rows[idx].DefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                        }
                    }

                    rd.Close();
                }

                // Đếm số vật tư hết hàng
                int hetHang = dgvVatTu.Rows.Cast<DataGridViewRow>()
                    .Count(r => {
                        double.TryParse(r.Cells["colTonKho"].Value?.ToString().Replace(",", ""),
                            out double ton);
                        return ton <= 1;
                    });

                if (hetHang > 0)
                {
                    MessageBox.Show(
                        $"⚠️ Có {hetHang} vật tư sắp hết hoặc đã hết hàng (màu đỏ)!",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "✅ Tất cả vật tư đều còn hàng trong kho!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi kiểm tra kho:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT BẮT ĐẦU SẢN XUẤT
        // ═══════════════════════════════════════════════════════════════
        private void btnBatDauSX_Click(object sender, EventArgs e)
        {
            if (_idBaoGia == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn báo giá!", "Thiếu thông tin");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
            {
                MessageBox.Show("⚠️ Vui lòng bấm 'Lấy dữ liệu' trước!", "Thiếu thông tin");
                return;
            }

            var rs = MessageBox.Show(
                $"Bạn có muốn lưu và bắt đầu sản xuất không?\n\n" +
                $"Sản phẩm: {txtTenSanPham.Text}\n" +
                $"Số lượng: {txtSoLuong.Text}\n" +
                $"Khách hàng: {txtKhachHang.Text}",
                "Xác nhận bắt đầu sản xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Sinh mã lệnh SX
                            string maLenh = SinhMaLenhSX(conn, tran);

                            int.TryParse(txtSoLuong.Text.Replace(",", ""), out int soLuong);

                            // INSERT LENH_SAN_XUAT
                            string sqlIns = @"
INSERT INTO LENH_SAN_XUAT
    (Ma_Lenh_SX, id_Bao_Gia, id_Khach_Hang, Ten_San_Pham,
     So_Luong, Ngay_Bat_Dau, Trang_Thai)
VALUES
    (@ma, @idBG, @idKH, @tenSP, @sl, @ngay, N'Đang sản xuất')";

                            var cmd = new SqlCommand(sqlIns, conn, tran);
                            cmd.Parameters.AddWithValue("@ma", maLenh);
                            cmd.Parameters.AddWithValue("@idBG", _idBaoGia);
                            cmd.Parameters.AddWithValue("@idKH",
                                _idKhachHang > 0 ? (object)_idKhachHang : DBNull.Value);
                            cmd.Parameters.AddWithValue("@tenSP", txtTenSanPham.Text.Trim());
                            cmd.Parameters.AddWithValue("@sl", soLuong);
                            cmd.Parameters.AddWithValue("@ngay", dtpNgayBatDau.Value.Date);
                            cmd.ExecuteNonQuery();

                            // Cập nhật trạng thái BAO_GIA
                            string sqlUpd = @"
UPDATE BAO_GIA 
SET Trang_Thai = N'Đang sản xuất' 
WHERE id = @id";
                            var cmdUpd = new SqlCommand(sqlUpd, conn, tran);
                            cmdUpd.Parameters.AddWithValue("@id", _idBaoGia);
                            cmdUpd.ExecuteNonQuery();

                            tran.Commit();

                            MessageBox.Show(
                                $"✅ Đã tạo lệnh sản xuất thành công!\n\nMã lệnh: {maLenh}",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            // Reset form
                            ResetForm();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi lưu lệnh sản xuất:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SINH MÃ LỆNH SX: LSX-2026-001
        // ═══════════════════════════════════════════════════════════════
        string SinhMaLenhSX(SqlConnection conn, SqlTransaction tran)
        {
            string prefix = "LSX-" + DateTime.Today.Year + "-";
            var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Lenh_SX, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM LENH_SAN_XUAT WHERE Ma_Lenh_SX LIKE @like", conn, tran);
            cmd.Parameters.AddWithValue("@p", prefix);
            cmd.Parameters.AddWithValue("@like", prefix + "%");
            int next = Convert.ToInt32(cmd.ExecuteScalar());
            return prefix + next.ToString("D3");
        }

        // ═══════════════════════════════════════════════════════════════
        // RESET FORM
        // ═══════════════════════════════════════════════════════════════
        void ResetForm()
        {
            cboBaoGia.SelectedIndex = 0;
            txtTenSanPham.Clear();
            txtSoLuong.Clear();
            txtKhachHang.Clear();
            dgvVatTu.Rows.Clear();
            _idBaoGia = 0;
            _idKhachHang = 0;
            _maBaoGia = "";
            dtpNgayBatDau.Value = DateTime.Today;
            LoadCboBaoGia();
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT IN LỆNH → XUẤT EXCEL
        // ═══════════════════════════════════════════════════════════════
        private void btnInLenh_Click(object sender, EventArgs e)
        {
            if (_idBaoGia == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn báo giá và bấm 'Lấy dữ liệu' trước khi in!",
                    "Thiếu thông tin");
                return;
            }
            try
            {
                // Lấy dữ liệu báo giá từ DB
                string maBG = "", tenSP = "", tenKH = "", khoIn = "", kichThuoc = "",
                       khoiLuongGiay = "", soMauIn = "", soCon = "", ghiChu = "";
                DateTime ngayBG = DateTime.Today;
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT bg.Ma_Bao_Gia,
       bg.Ten_San_Pham,
       bg.Ngay_Bao_Gia,
       bg.Kich_Thuoc_Thanh_Pham,
       bg.Khoi_Luong_Giay,
       bg.Kho_In,
       bg.So_Mau_In,
       bg.So_Con,
       bg.Ghi_Chu,
       ISNULL(kh.Ten_Khach_Hang,'') AS TenKH
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
WHERE bg.id = @id";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _idBaoGia);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        maBG = rd["Ma_Bao_Gia"].ToString();
                        tenSP = rd["Ten_San_Pham"].ToString();
                        tenKH = rd["TenKH"].ToString();
                        khoIn = rd["Kho_In"].ToString();
                        kichThuoc = rd["Kich_Thuoc_Thanh_Pham"].ToString();
                        khoiLuongGiay = rd["Khoi_Luong_Giay"].ToString();
                        soMauIn = rd["So_Mau_In"].ToString();
                        soCon = rd["So_Con"].ToString();
                        ghiChu = rd["Ghi_Chu"].ToString();
                        if (rd["Ngay_Bao_Gia"] != DBNull.Value)
                            ngayBG = Convert.ToDateTime(rd["Ngay_Bao_Gia"]);
                    }
                    rd.Close();
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("LenhSX");
                    int r = 1;
                    // Dòng tiêu đề công ty
                    ws.Cells[r, 1].Value = "CÔNG TY TNHH SX TM DỊCH VỤ AN LÂM";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 11;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r++;
                    ws.Cells[r, 1].Value = "51/10/3 Hòa Bình, Phường Tân Phú, Tp. HCM";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.Font.Size = 9;
                    r += 2;
                    // Tiêu đề LỆNH SẢN XUẤT
                    ws.Cells[r, 1].Value = "LỆNH SẢN XUẤT";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;
                    ws.Cells[r, 1].Value = $"Ngày {ngayBG:dd/MM/yyyy}";
                    ws.Cells[r, 1, r, 4].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[r, 5].Value = "Số LSX:";
                    ws.Cells[r, 6].Value = maBG;
                    ws.Cells[r, 6].Style.Font.Bold = true;
                    r += 2;
                    // Bảng thông tin chung (phần trên giống mẫu)
                    ws.Cells[r, 1].Value = "Khách hàng";
                    ws.Cells[r, 2, r, 4].Merge = true;
                    ws.Cells[r, 2].Value = tenKH;
                    ws.Cells[r, 2].Style.Font.Bold = true;
                    r++;
                    ws.Cells[r, 1].Value = "Tên bài in";
                    ws.Cells[r, 2, r, 4].Merge = true;
                    ws.Cells[r, 2].Value = tenSP;
                    r++;
                    ws.Cells[r, 1].Value = "Tên giấy";
                    ws.Cells[r, 2, r, 4].Merge = true;
                    ws.Cells[r, 2].Value = khoiLuongGiay;
                    r++;
                    ws.Cells[r, 1].Value = "Khổ in";
                    ws.Cells[r, 2].Value = khoIn;
                    ws.Cells[r, 3].Value = "Kích thước TP";
                    ws.Cells[r, 4].Value = kichThuoc;
                    r++;
                    ws.Cells[r, 1].Value = "Số lượng in";
                    ws.Cells[r, 2].Value = txtSoLuong.Text; // hoặc lấy lại từ BAO_GIA nếu muốn
                    ws.Cells[r, 3].Value = "Số màu in";
                    ws.Cells[r, 4].Value = soMauIn;
                    r++;
                    ws.Cells[r, 1].Value = "Số con";
                    ws.Cells[r, 2].Value = soCon;
                    ws.Cells[r, 3].Value = "Ghi chú";
                    ws.Cells[r, 4, r, 8].Merge = true;
                    ws.Cells[r, 4].Value = ghiChu;
                    r += 2;
                    // Kẻ khung đơn giản cho vùng thông tin
                    using (var range = ws.Cells[4, 1, r - 1, 8])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    // Phần chữ ký / phân công (đơn giản, bạn có thể chỉnh thêm sau)
                    ws.Cells[r, 1].Value = "Thiết kế";
                    ws.Cells[r, 3].Value = "Thủ kho";
                    ws.Cells[r, 5].Value = "KCS";
                    ws.Cells[r, 7].Value = "Người điều độ";
                    ws.Cells[r, 1, r, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;
                    ws.Cells[r, 1].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 3].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 5].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 7].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 1, r, 8].Style.Font.Italic = true;
                    ws.Cells[r, 1, r, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Định dạng cột
                    ws.Column(1).Width = 14;
                    ws.Column(2).Width = 22;
                    ws.Column(3).Width = 14;
                    ws.Column(4).Width = 18;
                    ws.Column(5).Width = 12;
                    ws.Column(6).Width = 18;
                    ws.Column(7).Width = 12;
                    ws.Column(8).Width = 12;
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"LENH_SAN_XUAT_{maBG}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    pkg.SaveAs(new FileInfo(path));
                    MessageBox.Show($"✅ Đã xuất lệnh sản xuất ra Excel:\n{path}", "Thành công",
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
                MessageBox.Show($"❌ Lỗi xuất Excel lệnh sản xuất:\n{ex.Message}", "Lỗi");
            }
        }
        // ═══════════════════════════════════════════════════════════════
        // BÁO GIÁ ITEM CLASS
        // ═══════════════════════════════════════════════════════════════
        private class BaoGiaItem
        {
            public int Id { get; set; }
            public string Ma { get; set; } = "";
            public string TenSanPham { get; set; } = "";
            public string TenKH { get; set; } = "";
            public int SoLuong { get; set; }

            public override string ToString()
            {
                if (Id == 0) return "-- Chọn số báo giá --";
                return $"{Ma} - {TenSanPham} - {SoLuong:N0} cái";
            }
        }
    }
}