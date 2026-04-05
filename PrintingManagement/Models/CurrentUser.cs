using Microsoft.Data.SqlClient;
using PrintingManagement;
using System;
using System.Data;

namespace PrintingManagement
{
    internal class CurrentUser
    {
        // ===== THÔNG TIN NGƯỜI DÙNG HIỆN TẠI =====
        public static int UserId { get; private set; }
        public static string Username { get; private set; }
        public static string HoTen { get; private set; }
        public static string Email { get; private set; }
        public static string VaiTro { get; private set; }

        // ===== KIỂM TRA VAI TRÒ =====
        public static bool IsAdmin => VaiTro?.Equals("Admin") ?? false;
        public static bool IsKinhDoanh => VaiTro?.Equals("Kinh doanh") ?? false;
        public static bool IsKeToan => VaiTro?.Equals("Kế toán") ?? false;
        public static bool IsThuKho => VaiTro?.Equals("Thủ kho") ?? false;
        public static bool IsSanXuat => VaiTro?.Equals("Sản xuất") ?? false;
        public static bool IsGiamDoc => VaiTro?.Equals("Giám đốc") ?? false;

        // ===== ĐĂNG NHẬP — GỌI STORED PROCEDURE =====
        public static bool Login(string username, string password, string vaiTro)
        {
            try
            {
                username = username?.Trim() ?? "";
                password = password?.Trim() ?? "";
                vaiTro = vaiTro?.Trim() ?? "";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@Username", SqlDbType.NVarChar, 100) { Value = username },
                    new SqlParameter("@VaiTro",   SqlDbType.NVarChar,  50) { Value = vaiTro   }
                };

                // Gọi SP — không có query trực tiếp
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_Login", parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string storedPass = row["Mat_Khau"]?.ToString() ?? "";

                    // BCrypt.Verify — nhận cả bcrypt hash lẫn plaintext cũ (backward-compatible)
                    if (!string.IsNullOrEmpty(storedPass)
                        && (storedPass.StartsWith("$2") || storedPass.StartsWith("$sha")))
                    {
                        if (!BCrypt.Net.BCrypt.Verify(password, storedPass))
                            return false;
                    }
                    else if (!string.Equals(password, storedPass, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    UserId = Convert.ToInt32(row["id"]);
                    Username = row["Ten_Nguoi_Dung"]?.ToString() ?? "";
                    // sp_Login chỉ trả Ten_Nguoi_Dung — không có cột Ho_Ten; truy cập row["Ho_Ten"] sẽ lỗi
                    HoTen = row.Table.Columns.Contains("Ho_Ten") && row["Ho_Ten"] != DBNull.Value
                        ? row["Ho_Ten"]?.ToString() ?? ""
                        : Username;
                    Email = row["Email"]?.ToString() ?? "";
                    VaiTro = row["Vai_Tro"]?.ToString() ?? "";
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đăng nhập: " + ex.Message);
            }
        }

        // ===== ĐĂNG XUẤT =====
        public static void Logout()
        {
            UserId = 0;
            Username = null;
            HoTen = null;
            Email = null;
            VaiTro = null;
        }

        // ===== KIỂM TRA ĐÃ ĐĂNG NHẬP =====
        public static bool IsLoggedIn() => UserId > 0 && !string.IsNullOrEmpty(VaiTro);
    }
}
