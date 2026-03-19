using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace PrintingManagement
{
    internal class CurrentUser
    {
        // ===== THÔNG TIN NGƯỜI DÙNG HIỆN TẠI =====
        public static int UserId { get; set; }
        public static string Username { get; set; } // ⭐ Username thay vì Email
        public static string HoTen { get; set; }
        public static string Email { get; set; }
        public static string VaiTro { get; set; }

        // ===== KIỂM TRA VAI TRÒ =====
        public static bool IsAdmin => VaiTro == "Admin";
        public static bool IsKinhDoanh => VaiTro == "Kinh doanh";
        public static bool IsKeToan => VaiTro == "Kế toán";
        public static bool IsThuKho => VaiTro == "Thủ kho";
        public static bool IsSanXuat => VaiTro == "Sản xuất";
        public static bool IsGiamDoc => VaiTro == "Giám đốc";

        // ===== ⭐ HÀM LOGIN - BẰNG USERNAME =====
        public static bool Login(string username, string password, string vaiTro)
        {
            try
            {
                // ⭐ Query bằng Ten_Nguoi_Dung (Username), Mat_Khau, Vai_Tro
                string sql = @"SELECT id, Ten_Nguoi_Dung, Email, Vai_Tro, Trang_Thai, Mat_Khau
                               FROM USERS 
                               WHERE Ten_Nguoi_Dung = @username 
                               AND Vai_Tro = @vaiTro
                               AND Trang_Thai = 1";

                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@username", username),
                    new SqlParameter("@vaiTro", vaiTro)
                };

                var dt = DatabaseHelper.ExecuteQuery(sql, parameters);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    string storedPassInDB = row["Mat_Khau"]?.ToString() ?? "";

                    // ⭐ VERIFY PASSWORD (PLAINTEXT COMPARISON)
                    // Note: storing plaintext is NOT recommended. This compares raw strings.
                    if (!string.IsNullOrEmpty(storedPassInDB) &&
                        string.Equals(password, storedPassInDB, StringComparison.Ordinal))
                    {
                        // ⭐ LƯU THÔNG TIN NGƯỜI DÙNG
                        UserId = (int)row["id"];
                        Username = row["Ten_Nguoi_Dung"].ToString();
                        HoTen = row["Ten_Nguoi_Dung"].ToString(); // Hoặc lấy từ cột HoTen nếu có
                        Email = row["Email"]?.ToString() ?? "";
                        VaiTro = row["Vai_Tro"].ToString();

                        return true; // Đăng nhập thành công
                    }
                }

                return false; // Sai username/password/vai trò
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đăng nhập: " + ex.Message);
            }
        }

        // ===== HÀM LOGOUT =====
        public static void Logout()
        {
            UserId = 0;
            Username = null;
            HoTen = null;
            Email = null;
            VaiTro = null;
        }

        // ===== KIỂM TRA ĐÃ ĐĂNG NHẬP CHƯA =====
        public static bool IsLoggedIn()
        {
            return UserId > 0 && !string.IsNullOrEmpty(VaiTro);
        }
    }
}