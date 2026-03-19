using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using BCrypt.Net;  // Đảm bảo đã cài NuGet BCrypt.Net-Next

namespace PrintingManagement
{
    public partial class FormQuanLyNguoiDung1 : Form
    {
        private readonly frmQuanLyNguoiDung parentForm;  // Form cha để refresh DataGridView
        private readonly int? userId;  // null = thêm mới, có giá trị = sửa

        public FormQuanLyNguoiDung1(frmQuanLyNguoiDung parent, int? id = null)
        {
            InitializeComponent();
            parentForm = parent;
            userId = id;

            // Load ComboBox Vai trò
            cmbVaiTro.Items.AddRange(new string[]
            {
                "Admin", "Kinh doanh", "Kế toán", "Thủ kho", "Sản xuất", "Giám đốc"
            });
            cmbVaiTro.SelectedIndex = 0;

            if (userId.HasValue)
            {
                this.Text = "SỬA NGƯỜI DÙNG";
                LoadUserData(userId.Value);
            }
            else
            {
                this.Text = "+ THÊM NGƯỜI DÙNG MỚI";
                chkTaiKhoanHoatDong.Checked = true;  // Mặc định hoạt động khi thêm mới
            }

            // Hiện/ẩn mật khẩu mặc định
            txtMatKhau.UseSystemPasswordChar = true;
            txtXacNhanMatKhau.UseSystemPasswordChar = true;
        }

        // Load dữ liệu khi sửa
        private void LoadUserData(int id)
        {
            string query = @"
                SELECT Ten_Nguoi_Dung, Email, Vai_Tro, Trang_Thai 
                FROM USERS 
                WHERE id = @id";

            var parameters = new[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@id", id)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                txtTenNguoiDung.Text = dt.Rows[0]["Ten_Nguoi_Dung"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                cmbVaiTro.SelectedItem = dt.Rows[0]["Vai_Tro"].ToString();
                chkTaiKhoanHoatDong.Checked = Convert.ToBoolean(dt.Rows[0]["Trang_Thai"]);
            }
            else
            {
                MessageBox.Show("Không tìm thấy người dùng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // Nút LƯU (thêm mới hoặc sửa)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra bắt buộc
            if (string.IsNullOrWhiteSpace(txtTenNguoiDung.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên người dùng và Email!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!userId.HasValue && string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu cho người dùng mới!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMatKhau.Text != txtXacNhanMatKhau.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Hash mật khẩu nếu có nhập mới
                string hashedPass = string.IsNullOrEmpty(txtMatKhau.Text)
                    ? null  // Giữ pass cũ nếu sửa mà không đổi pass
                    : BCrypt.Net.BCrypt.HashPassword(txtMatKhau.Text.Trim());

                string query;
                List<Microsoft.Data.SqlClient.SqlParameter> parameters = new();

                if (!userId.HasValue) // Thêm mới
                {
                    query = @"
                        INSERT INTO USERS 
                        (Ten_Nguoi_Dung, Email, Mat_Khau, Vai_Tro, Trang_Thai, Ngay_Tao)
                        VALUES 
                        (@Ten, @Email, @MatKhau, @VaiTro, @TrangThai, GETDATE())";

                    parameters.AddRange(new[]
                    {
                        new Microsoft.Data.SqlClient.SqlParameter("@Ten", txtTenNguoiDung.Text.Trim()),
                        new Microsoft.Data.SqlClient.SqlParameter("@Email", txtEmail.Text.Trim()),
                        new Microsoft.Data.SqlClient.SqlParameter("@MatKhau", hashedPass),
                        new Microsoft.Data.SqlClient.SqlParameter("@VaiTro", cmbVaiTro.SelectedItem.ToString()),
                        new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", chkTaiKhoanHoatDong.Checked ? 1 : 0)
                    });
                }
                else // Sửa
                {
                    query = @"
                        UPDATE USERS SET 
                            Ten_Nguoi_Dung = @Ten,
                            Email = @Email,
                            Vai_Tro = @VaiTro,
                            Trang_Thai = @TrangThai";

                    parameters.AddRange(new[]
                    {
                        new Microsoft.Data.SqlClient.SqlParameter("@Ten", txtTenNguoiDung.Text.Trim()),
                        new Microsoft.Data.SqlClient.SqlParameter("@Email", txtEmail.Text.Trim()),
                        new Microsoft.Data.SqlClient.SqlParameter("@VaiTro", cmbVaiTro.SelectedItem.ToString()),
                        new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", chkTaiKhoanHoatDong.Checked ? 1 : 0),
                        new Microsoft.Data.SqlClient.SqlParameter("@id", userId.Value)
                    });

                    if (!string.IsNullOrEmpty(txtMatKhau.Text))
                    {
                        query += ", Mat_Khau = @MatKhau";
                        parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MatKhau", hashedPass));
                    }

                    query += " WHERE id = @id";
                }

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters.ToArray());

                if (rowsAffected > 0)
                {
                    MessageBox.Show(userId.HasValue ? "Cập nhật thành công!" : "Thêm người dùng thành công!",
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh DataGridView ở form cha
                    parentForm.LoadDataToGrid();  // ← Đúng tên hàm public ở form cha

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể lưu dữ liệu. Vui lòng kiểm tra lại!",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút HỦY
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();  // Chỉ đóng form, không lưu gì
        }

        // Hiện/ẩn mật khẩu (tùy chọn, nếu bạn có checkbox hiện mật khẩu)
/*        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
            txtXacNhanMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
        }
*/
        private void FormQuanLyNguoiDung1_Load(object sender, EventArgs e)
        {
            // Nếu cần load thêm gì khi form mở, thêm ở đây
        }
    }
}