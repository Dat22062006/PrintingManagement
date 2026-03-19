using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public partial class frmCapNhapNguoiDung : Form
    {
        private int _idKhachHang;

        public frmCapNhapNguoiDung()
        {
            InitializeComponent();
            _idKhachHang = 0;
        }

        public frmCapNhapNguoiDung(int idKhachHang)
        {
            InitializeComponent();
            _idKhachHang = idKhachHang;
        }

        private void frmCapNhapNguoiDung_Load(object sender, EventArgs e)
        {
            if (_idKhachHang > 0)
            {
                lblTieuDe.Text = "✏️ Sửa khách hàng";
                LoadThongTin();
            }
            else
            {
                lblTieuDe.Text = "➕ Thêm khách hàng mới";
                txtMaKH.Text = SinhMaKhachHang();
                txtMaKH.ReadOnly = true;
            }
        }

        // ─── Sinh mã tự động ──────────────────────────────────────────────────────
        private string SinhMaKhachHang()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        @"SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_KH, 3, LEN(Ma_KH)) AS INT)), 0) + 1
                          FROM KHACH_HANG
                          WHERE Ma_KH LIKE 'KH%'", conn);
                    int stt = Convert.ToInt32(cmd.ExecuteScalar());
                    return "KH" + stt.ToString("D4");
                }
            }
            catch { return "KH0001"; }
        }

        // ─── Load dữ liệu khi sửa ─────────────────────────────────────────────────
        private void LoadThongTin()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM KHACH_HANG WHERE id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", _idKhachHang);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        txtMaKH.Text = rd["Ma_KH"].ToString();
                        txtMaKH.ReadOnly = true;
                        txtTenKH.Text = rd["Ten_Khach_Hang"].ToString();
                        txtDiaChi.Text = rd["Dia_Chi"].ToString();
                        txtMST.Text = rd["MST"].ToString();
                        txtDienThoai.Text = rd["Dien_Thoai"].ToString();
                        txtEmail.Text = rd["Email"] != DBNull.Value ? rd["Email"].ToString() : "";
                        txtNguoiLienHe.Text = rd["Nguoi_Lien_He"].ToString();
                        txtGhiChu.Text = rd["Ghi_Chu"] != DBNull.Value ? rd["Ghi_Chu"].ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Nút Lưu ──────────────────────────────────────────────────────────────
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd;

                    if (_idKhachHang == 0)
                    {
                        // THÊM MỚI
                        cmd = new SqlCommand(@"
                            INSERT INTO KHACH_HANG
                                (Ma_KH, Ten_Khach_Hang, Dia_Chi, MST,
                                 Dien_Thoai, Email, Nguoi_Lien_He, Ghi_Chu,
                                 Trang_Thai, Nguoi_Tao, Ngay_Tao)
                            VALUES
                                (@Ma, @Ten, @DiaChi, @MST,
                                 @DienThoai, @Email, @NguoiLH, @GhiChu,
                                 N'Hoạt động', @NguoiTao, GETDATE())", conn);
                        cmd.Parameters.AddWithValue("@NguoiTao", CurrentUser.HoTen);
                    }
                    else
                    {
                        // SỬA
                        cmd = new SqlCommand(@"
                            UPDATE KHACH_HANG SET
                                Ten_Khach_Hang = @Ten,
                                Dia_Chi        = @DiaChi,
                                MST            = @MST,
                                Dien_Thoai     = @DienThoai,
                                Email          = @Email,
                                Nguoi_Lien_He  = @NguoiLH,
                                Ghi_Chu        = @GhiChu,
                                Nguoi_Sua      = @NguoiTao,
                                Ngay_Sua       = GETDATE()
                            WHERE id = @Id", conn);
                        cmd.Parameters.AddWithValue("@Id", _idKhachHang);
                        cmd.Parameters.AddWithValue("@NguoiTao", CurrentUser.HoTen);
                    }

                    cmd.Parameters.AddWithValue("@Ma", txtMaKH.Text.Trim());
                    cmd.Parameters.AddWithValue("@Ten", txtTenKH.Text.Trim());
                    cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());
                    cmd.Parameters.AddWithValue("@MST", txtMST.Text.Trim());
                    cmd.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@NguoiLH", txtNguoiLienHe.Text.Trim());
                    cmd.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text.Trim());

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    _idKhachHang == 0 ? "Thêm khách hàng thành công!" : "Cập nhật thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Nút Hủy ──────────────────────────────────────────────────────────────
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}