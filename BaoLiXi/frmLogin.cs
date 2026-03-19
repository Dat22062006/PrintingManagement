using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using BCrypt.Net;

namespace PrintingManagement
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void ptrlogoteam_Click(object sender, EventArgs e)
        {

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            // Placeholder cho textbox
            txtUsername.Text = "Nhập tên đăng nhập";
            txtUsername.ForeColor = Color.Gray;
            txtPassword.Text = "Nhập mật khẩu";
            txtPassword.ForeColor = Color.Gray;
            txtPassword.PasswordChar = '\0';
            this.ActiveControl = null;
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Nhập tên đăng nhập")
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Nhập tên đăng nhập";
                txtUsername.ForeColor = Color.Gray;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Nhập mật khẩu")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.Black;
                txtPassword.PasswordChar = '*';
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.PasswordChar = '\0';
                txtPassword.Text = "Nhập mật khẩu";
                txtPassword.ForeColor = Color.Gray;
            }
        }

        // ===== ⭐ NÚT ĐĂNG NHẬP - BẰNG USERNAME + VAI TRÒ =====
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtUsername.Text.Trim();
            string matKhau = txtPassword.Text.Trim();

            // ===== VALIDATE INPUT =====
            if (string.IsNullOrEmpty(tenDangNhap) ||
                tenDangNhap == "Nhập tên đăng nhập" ||
                string.IsNullOrEmpty(matKhau) ||
                matKhau == "Nhập mật khẩu")
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ===== LẤY VAI TRÒ TỪ RADIO BUTTON =====
            string vaiTroChon = "";
            if (rdoAdmin.Checked) vaiTroChon = "Admin";
            else if (rdoKinhDoanh.Checked) vaiTroChon = "Kinh doanh";
            else if (rdoKeToan.Checked) vaiTroChon = "Kế toán";
            else if (rdoThuKho.Checked) vaiTroChon = "Thủ kho";
            else if (rdoSanXuat.Checked) vaiTroChon = "Sản xuất";
            else if (rdoGiamDoc.Checked) vaiTroChon = "Giám đốc";
            else
            {
                MessageBox.Show("⚠️ Vui lòng chọn vai trò!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ===== ⭐ GỌI HÀM LOGIN =====
                bool success = CurrentUser.Login(tenDangNhap, matKhau, vaiTroChon);

                if (success)
                {
                    MessageBox.Show(
                        $"✅ Đăng nhập thành công!\n\n" +
                        $"👤 Tên: {CurrentUser.HoTen}\n" +
                        $"🔑 Vai trò: {CurrentUser.VaiTro}",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // ===== MỞ FORM TƯƠNG ỨNG =====
                    this.Hide();

                    if (CurrentUser.IsAdmin)
                    {
                        // Mở form Quản lý người dùng cho Admin
                        frmQuanLyNguoiDung frmAdmin = new frmQuanLyNguoiDung();
                        frmAdmin.Show();
                    }
                    else
                    {
                        // Mở form Main cho các vai trò khác
                        frmMain frmMain = new frmMain();
                        frmMain.Show();
                    }
                }
                else
                {
                    MessageBox.Show(
                        "❌ Đăng nhập thất bại!\n\n" +
                        "Kiểm tra lại:\n" +
                        "• Tên đăng nhập\n" +
                        "• Mật khẩu\n" +
                        "• Vai trò đã chọn\n" +
                        "• Tài khoản có bị khóa không?",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtPassword.Clear();
                    txtPassword.Text = "Nhập mật khẩu";
                    txtPassword.ForeColor = Color.Gray;
                    txtPassword.PasswordChar = '\0';
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "❌ Lỗi kết nối cơ sở dữ liệu:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}