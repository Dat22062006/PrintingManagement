// ═══════════════════════════════════════════════════════
// ║  frmLogin.cs - ĐĂNG NHẬP HỆ THỐNG                  ║
// ═══════════════════════════════════════════════════════
// Mục đích: Xác thực người dùng bằng tên đăng nhập,
// mật khẩu và vai trò. Sau khi đăng nhập thành công
// sẽ mở form tương ứng theo vai trò.
// Hỗ trợ ghi nhớ đăng nhập (Remember Me) và quên mật khẩu.
// ═══════════════════════════════════════════════════════

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmLogin : Form
    {
        private const string PlaceholderUsername = "Email, tên rút gọn (vd: admin) hoặc tên hiển thị";
        private const string PlaceholderPassword = "Nhập mật khẩu";

        // ─────────────────────────────────────────────────────
        // FILE LƯU TÀI KHOẢN GHI NHỚ
        // ─────────────────────────────────────────────────────

        private static readonly string RememberFile =
            Path.Combine(Application.StartupPath, "remember.dat");

        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmLogin()
        {
            InitializeComponent();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM — điền lại tài khoản đã lưu
        // ─────────────────────────────────────────────────────

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtUsername.Text = PlaceholderUsername;
            txtUsername.ForeColor = Color.Gray;
            txtPassword.Text = PlaceholderPassword;
            txtPassword.ForeColor = Color.Gray;
            txtPassword.PasswordChar = '\0';
            this.ActiveControl = null;

            // Tự động điền username + vai trò nếu đã lưu
            LoadSavedCredentials();
        }


        // ─────────────────────────────────────────────────────
        // GHI NHỚ ĐĂNG NHẬP — LƯU/ĐỌC FILE
        // ─────────────────────────────────────────────────────

        private void LoadSavedCredentials()
        {
            try
            {
                if (!File.Exists(RememberFile)) return;

                var lines = File.ReadAllLines(RememberFile);
                if (lines.Length < 2) return;

                string savedUsername = lines[0].Trim();
                string savedRole     = lines[1].Trim();

                if (string.IsNullOrEmpty(savedUsername) ||
                    savedUsername == PlaceholderUsername) return;

                // Điền username
                txtUsername.Text = savedUsername;
                txtUsername.ForeColor = Color.Black;

                // Chọn đúng vai trò
                SelectRoleByName(savedRole);
                chkRememberMe.Checked = true;
            }
            catch { /* bỏ qua lỗi đọc file */ }
        }

        private void SaveCredentials(string username, string role)
        {
            try
            {
                if (chkRememberMe.Checked)
                {
                    File.WriteAllLines(RememberFile, new[] { username, role });
                }
                else
                {
                    if (File.Exists(RememberFile))
                        File.Delete(RememberFile);
                }
            }
            catch { /* bỏ qua lỗi ghi file */ }
        }

        private void SelectRoleByName(string role)
        {
            rdoAdmin.Checked      = role == "Admin";
            rdoSales.Checked      = role == "Kinh doanh";
            rdoAccountant.Checked = role == "Kế toán";
            rdoWarehouse.Checked  = role == "Thủ kho";
            rdoProduction.Checked = role == "Sản xuất";
            rdoDirector.Checked   = role == "Giám đốc";
        }

        private string GetSelectedRoleName()
        {
            if (rdoAdmin.Checked)      return "Admin";
            if (rdoSales.Checked)      return "Kinh doanh";
            if (rdoAccountant.Checked) return "Kế toán";
            if (rdoWarehouse.Checked)  return "Thủ kho";
            if (rdoProduction.Checked) return "Sản xuất";
            if (rdoDirector.Checked)   return "Giám đốc";
            return "";
        }


        // ─────────────────────────────────────────────────────
        // PLACEHOLDER — Ô TÊN ĐĂNG NHẬP
        // ─────────────────────────────────────────────────────

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            // Xóa placeholder khi người dùng click vào ô
            if (txtUsername.Text == PlaceholderUsername)
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            // Hiển thị lại placeholder nếu người dùng bỏ trống
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = PlaceholderUsername;
                txtUsername.ForeColor = Color.Gray;
            }
        }


        // ─────────────────────────────────────────────────────
        // PLACEHOLDER — Ô MẬT KHẨU
        // ─────────────────────────────────────────────────────

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            // Bật ẩn ký tự và xóa placeholder khi click vào ô
            if (txtPassword.Text == PlaceholderPassword)
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.Black;
                txtPassword.PasswordChar = '*';
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            // Hiển thị lại placeholder nếu người dùng bỏ trống
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.PasswordChar = '\0';
                txtPassword.Text = PlaceholderPassword;
                txtPassword.ForeColor = Color.Gray;
            }
        }


        // ─────────────────────────────────────────────────────
        // ĐĂNG NHẬP — XÁC THỰC USERNAME + MẬT KHẨU + VAI TRÒ
        // ─────────────────────────────────────────────────────

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Kiểm tra placeholder
            if (string.IsNullOrEmpty(username) || username == PlaceholderUsername ||
                string.IsNullOrEmpty(password) || password == PlaceholderPassword)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy vai trò đã chọn
            string selectedRole = GetSelectedRoleName();
            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Vui lòng chọn vai trò!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = CurrentUser.Login(username, password, selectedRole);

                if (success)
                {
                    // Lưu tài khoản nếu check Remember Me
                    SaveCredentials(username, selectedRole);

                    MessageBox.Show(
                        "Dang nhap thanh cong!\n\n" +
                        "Nguoi dung: " + CurrentUser.HoTen + "\n" +
                        "Vai tro  : " + CurrentUser.VaiTro,
                        "Thanh cong",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.Hide();
                    if (CurrentUser.IsAdmin)
                    {
                        frmUserManagement frmAdmin = new frmUserManagement();
                        frmAdmin.Show();
                    }
                    else
                    {
                        frmMain frmMain = new frmMain();
                        frmMain.Show();
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Dang nhap that bai!\n\n" +
                        "Kiem tra lai:\n" +
                        "- Ten dang nhap / email\n" +
                        "- Mat khau\n" +
                        "- Vai tro da chon\n" +
                        "- Tai khoan co bi khoa khong?",
                        "Loi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtPassword.Clear();
                    txtPassword.PasswordChar = '\0';
                    txtPassword.Text = PlaceholderPassword;
                    txtPassword.ForeColor = Color.Gray;
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi ket noi co so du lieu:\n\n" + ex.Message,
                    "Loi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // QUÊN MẬT KHẨU — MỞ FORM ĐẶT LẠI
        // ─────────────────────────────────────────────────────

        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmForgotPassword frm = new frmForgotPassword();
            frm.ShowDialog();
        }


        // ─────────────────────────────────────────────────────
        // THOÁT ỨNG DỤNG
        // ─────────────────────────────────────────────────────

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOGO (GIỮ NGUYÊN)
        // ─────────────────────────────────────────────────────

        private void ptrlogoteam_Click(object sender, EventArgs e) { }
    }
}