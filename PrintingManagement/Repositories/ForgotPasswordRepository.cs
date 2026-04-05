using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Xử lý logic quên mật khẩu: lưu OTP, gửi email, đặt lại mật khẩu.
    /// KHÔNG có logic gửi email hay truy vấn DB trong Form.
    /// </summary>
    public class ForgotPasswordRepository
    {
        // Cấu hình SMTP đọc từ App.config (bảo mật, không hardcode trong form)
        private static readonly string SmtpHost    = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"]    ?? "smtp.gmail.com";
        private static readonly int    SmtpPort    = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]    ?? "587");
        private static readonly string SmtpEmail   = System.Configuration.ConfigurationManager.AppSettings["SmtpEmail"]   ?? "";
        private static readonly string SmtpAppPass = System.Configuration.ConfigurationManager.AppSettings["SmtpAppPass"] ?? "";
        private static readonly string AppName      = System.Configuration.ConfigurationManager.AppSettings["AppName"]      ?? "Hệ thống quản lý in ấn";

        // ─────────────────────────────────────────────────────
        // BƯỚC 1: YÊU CẦU ĐẶT LẠI MẬT KHẨU
        // Lưu OTP vào DB, gửi email cho người dùng.
        // Trả về: email gửi thành công hay không.
        // ─────────────────────────────────────────────────────

        public bool RequestReset(string email, out string maskedEmail, out string errorMessage)
        {
            maskedEmail   = "";
            errorMessage  = "";

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                errorMessage = "Email khong hop le.";
                return false;
            }

            // Tạo mã OTP 6 số bảo mật (cryptographically secure)
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            int otpInt = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 900000 + 100000;
            string otp = otpInt.ToString();
            DateTime expiry = DateTime.Now.AddMinutes(5);

            try
            {
                // Lưu OTP hash vào database (BCrypt)
                string otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
                bool emailExists = SaveOtpToDatabase(email, otpHash, otp, expiry);

                if (!emailExists)
                {
                    errorMessage = "Email khong ton tai trong he thong.";
                    return false;
                }

                // Gửi email chứa mã OTP
                SendOtpEmail(email, otp);
                maskedEmail = MaskEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Loi gui email: " + ex.Message;
                return false;
            }
        }

        // ─────────────────────────────────────────────────────
        // BƯỚC 2: XÁC MINH OTP + ĐẶT LẠI MẬT KHẨU
        // Trả về: true = thành công, false = thất bại.
        // ─────────────────────────────────────────────────────

        public bool ResetPassword(string email, string otp, string newPassword, out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(otp) || otp.Length != 6)
            {
                errorMessage = "Ma OTP phai gom 6 chu so.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                errorMessage = "Mat khau moi phai co it nhat 6 ky tu.";
                return false;
            }

            try
            {
                // Bước 1: Lấy OTP hash từ DB
                string storedHash = GetStoredOtpHash(email);
                if (string.IsNullOrEmpty(storedHash))
                {
                    errorMessage = "Ma OTP khong dung hoac da het han.";
                    return false;
                }

                // Bước 2: Verify OTP trong C# bằng BCrypt
                if (!BCrypt.Net.BCrypt.Verify(otp, storedHash))
                {
                    errorMessage = "Ma OTP khong dung hoac da het han.";
                    return false;
                }

                // Bước 3: Hash mật khẩu mới
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Bước 4: Gọi SP — truyền OTP để verify (đã verify BCrypt ở trên)
                return SaveNewPassword(email, otp, passwordHash, out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = "Loi: " + ex.Message;
                return false;
            }
        }

        // Lấy OTP hash từ database (chưa dùng, chưa hết hạn)
        private string GetStoredOtpHash(string email)
        {
            string query = @"
                SELECT TOP 1 Otp_Hash
                FROM PASSWORD_RESET
                WHERE Email = @Email
                  AND Da_Su_Dung = 0
                  AND Het_Han > GETDATE()
                ORDER BY Ngay_Tao DESC";

            var parameters = new[]
            {
                new SqlParameter("@Email", email)
            };

            var dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Otp_Hash"]?.ToString() ?? "";
            }
            return "";
        }

        // Lưu mật khẩu mới (SP đã sửa — yêu cầu OTP hợp lệ)
        private bool SaveNewPassword(string email, string otp, string passwordHash, out string errorMessage)
        {
            errorMessage = "";

            var parameters = new[]
            {
                new SqlParameter("@Email",       email),
                new SqlParameter("@OtpCode",     otp),
                new SqlParameter("@NewPassword", passwordHash)
            };

            var result = DatabaseHelper.ExecuteStoredProcedure("sp_VerifyPasswordReset", parameters);
            if (result == null || result.Rows.Count == 0)
            {
                errorMessage = "Khong the cap nhat mat khau. Vui long thu lai.";
                return false;
            }
            return true;
        }

        // ─────────────────────────────────────────────────────
        // LƯU OTP VÀO DATABASE (gọi stored procedure)
        // ─────────────────────────────────────────────────────

        private bool SaveOtpToDatabase(string email, string otpHash, string otpCode, DateTime expiry)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email",    email),
                new SqlParameter("@OtpHash",  otpHash),
                new SqlParameter("@OtpCode",  otpCode),
                new SqlParameter("@Expiry",   expiry)
            };

            var result = DatabaseHelper.ExecuteStoredProcedure("sp_RequestPasswordReset", parameters);
            return result != null && result.Rows.Count > 0;
        }

        // ─────────────────────────────────────────────────────
        // GỬI EMAIL OTP qua SMTP (cấu hình từ App.config)
        // ─────────────────────────────────────────────────────

        private void SendOtpEmail(string toEmail, string otp)
        {
            if (string.IsNullOrEmpty(SmtpEmail) || string.IsNullOrEmpty(SmtpAppPass))
            {
                throw new Exception("Chua cau hinh SMTP. Vui long cau hinh SmtpEmail va SmtpAppPass trong App.config.");
            }

            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.EnableSsl           = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SmtpEmail, SmtpAppPass);

                var msg = new MailMessage(SmtpEmail, toEmail)
                {
                    Subject = $"[{AppName}] Mã xác nhận đặt lại mật khẩu",
                    Body = $@"Mã OTP của bạn là: {otp}

Mã có hiệu lực trong 5 phút.
Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.

Trân trọng.",
                    IsBodyHtml = false,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8
                };

                client.Send(msg);
            }
        }

        // ─────────────────────────────────────────────────────
        // CHE EMAIL — bảo mật hiển thị
        // ─────────────────────────────────────────────────────

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            int at = email.IndexOf('@');
            string local = email.Substring(0, at);
            string domain = email.Substring(at);
            if (local.Length <= 3)
                return "***" + domain;
            return local.Substring(0, 3) + new string('*', Math.Max(0, local.Length - 3)) + domain;
        }
    }
}
