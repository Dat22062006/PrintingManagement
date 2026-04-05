using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmForgotPassword : Form
    {
        private readonly ForgotPasswordRepository _repo = new ForgotPasswordRepository();

        public frmForgotPassword()
        {
            InitializeComponent();
            txtOtp.Enabled            = false;
            txtNewPassword.Enabled     = false;
            txtConfirmPassword.Enabled = false;
            btnReset.Enabled          = false;
        }

        // ─────────────────────────────────────────────────────
        // GỬI OTP
        // ─────────────────────────────────────────────────────

        private void btnSendOtp_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            bool ok = _repo.RequestReset(email, out string maskedEmail, out string errorMessage);

            if (ok)
            {
                lblOtpStatus.Text = "Mã OTP đã gửi đến " + maskedEmail;
                lblOtpStatus.ForeColor = Color.Green;
                txtOtp.Enabled            = true;
                txtNewPassword.Enabled     = true;
                txtConfirmPassword.Enabled = true;
                btnReset.Enabled          = true;
                txtOtp.Focus();
            }
            else
            {
                lblOtpStatus.Text = errorMessage;
                lblOtpStatus.ForeColor = Color.Red;
            }
        }

        // ─────────────────────────────────────────────────────
        // ĐẶT LẠI MẬT KHẨU
        // ─────────────────────────────────────────────────────

        private void btnReset_Click(object sender, EventArgs e)
        {
            string email   = txtEmail.Text.Trim();
            string otp     = txtOtp.Text.Trim();
            string newPass = txtNewPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (newPass != confirm)
            {
                lblResetStatus.Text = "Mật khẩu xác nhận không khớp.";
                lblResetStatus.ForeColor = Color.Red;
                return;
            }

            bool ok = _repo.ResetPassword(email, otp, newPass, out string errorMessage);

            if (ok)
            {
                MessageBox.Show(
                    "Đặt lại mật khẩu thành công!\nBạn có thể đăng nhập bằng mật khẩu mới.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                lblResetStatus.Text = errorMessage;
                lblResetStatus.ForeColor = Color.Red;
            }
        }

        // ─────────────────────────────────────────────────────
        // ĐÓNG FORM
        // ─────────────────────────────────────────────────────

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
