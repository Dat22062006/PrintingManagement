// ═══════════════════════════════════════════════════════
// ║  UserRepository.cs - XỬ LÝ DỮ LIỆU NGƯỜI DÙNG     ║
// ═══════════════════════════════════════════════════════
// Mục đích: Tập trung TOÀN BỘ câu lệnh SQL liên quan
// đến bảng USERS tại đây — form không được viết SQL trực
// tiếp. Mọi tham số đều dùng parameterized query để
// chống SQL Injection tuyệt đối.
// ═══════════════════════════════════════════════════════

using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác CRUD với bảng USERS.
    /// Form chỉ gọi hàm — không được phép viết SQL trực tiếp trong form.
    /// </summary>
    public class UserRepository
    {
        // ─────────────────────────────────────────────────────
        // TÌM KIẾM / LẤY DANH SÁCH NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy danh sách người dùng, có thể lọc theo từ khóa tìm kiếm.
        /// Truyền chuỗi rỗng hoặc null để lấy tất cả.
        /// </summary>
        public DataTable SearchUsers(string keyword = "")
        {
            // Dùng parameterized query — chống SQL Injection tuyệt đối
            // [FIX] Thêm @ vào trước keyword để tránh SQL injection
            string query = @"
                SELECT
                    id              AS [ID],
                    Ten_Nguoi_Dung  AS [Tên người dùng],
                    Email,
                    Vai_Tro         AS [Vai trò],
                    Trang_Thai      AS [Trạng thái],
                    Ngay_Tao        AS [Ngày tạo]
                FROM USERS";

            // Chỉ thêm WHERE khi có từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // [FIX] Escape ký tự đặc biệt trong LIKE pattern để tránh lỗi hoặc injection
                string safeKeyword = keyword.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
                query += " WHERE Ten_Nguoi_Dung LIKE @keyword ESCAPE '[' OR Email LIKE @keyword ESCAPE '['";

                var parameters = new[]
                {
                    new SqlParameter("@keyword", "%" + safeKeyword + "%")
                };

                return DatabaseHelper.ExecuteQuery(query, parameters);
            }

            return DatabaseHelper.ExecuteQuery(query, null);
        }


        // ─────────────────────────────────────────────────────
        // LẤY THÔNG TIN NGƯỜI DÙNG THEO ID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy thông tin 1 người dùng theo ID.
        /// Trả về DataTable (1 dòng) hoặc rỗng nếu không tìm thấy.
        /// </summary>
        public DataTable GetById(int userId)
        {
            // Dùng parameterized query — chống SQL Injection tuyệt đối
            string query = @"
                SELECT Ten_Nguoi_Dung, Email, Vai_Tro, Trang_Thai
                FROM   USERS
                WHERE  id = @id";

            var parameters = new[]
            {
                new SqlParameter("@id", userId)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }


        // ─────────────────────────────────────────────────────
        // THÊM MỚI NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Thêm mới 1 người dùng vào bảng USERS.
        /// Mật khẩu được hash bằng BCrypt trước khi lưu.
        /// </summary>
        public int Insert(string fullName, string email, string password, string role, bool isActive)
        {
            string query = @"
                INSERT INTO USERS
                    (Ten_Nguoi_Dung, Email, Mat_Khau, Vai_Tro, Trang_Thai, Ngay_Tao)
                VALUES
                    (@FullName, @Email, @Password, @Role, @IsActive, GETDATE())";

            // BCrypt: salt tự động, đủ mạnh
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var parameters = new[]
            {
                new SqlParameter("@FullName",  fullName),
                new SqlParameter("@Email",     email),
                new SqlParameter("@Password",  hashedPassword),
                new SqlParameter("@Role",      role),
                new SqlParameter("@IsActive",  isActive ? 1 : 0)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT THÔNG TIN — KHÔNG ĐỔI MẬT KHẨU
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Cập nhật thông tin người dùng, không thay đổi mật khẩu.
        /// Dùng khi admin chỉ sửa tên, email, vai trò hoặc trạng thái.
        /// </summary>
        public int UpdateInfo(int userId, string fullName, string email, string role, bool isActive)
        {
            string query = @"
                UPDATE USERS SET
                    Ten_Nguoi_Dung = @FullName,
                    Email          = @Email,
                    Vai_Tro        = @Role,
                    Trang_Thai     = @IsActive
                WHERE id = @id";

            var parameters = new[]
            {
                new SqlParameter("@FullName",  fullName),
                new SqlParameter("@Email",     email),
                new SqlParameter("@Role",      role),
                new SqlParameter("@IsActive",  isActive ? 1 : 0),
                new SqlParameter("@id",        userId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT THÔNG TIN KÈM ĐỔI MẬT KHẨU
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Cập nhật thông tin người dùng kèm mật khẩu mới (đã hash BCrypt).
        /// </summary>
        public int UpdateInfoWithPassword(int userId, string fullName, string email,
                                          string newPassword, string role, bool isActive)
        {
            string query = @"
                UPDATE USERS SET
                    Ten_Nguoi_Dung = @FullName,
                    Email          = @Email,
                    Mat_Khau       = @Password,
                    Vai_Tro        = @Role,
                    Trang_Thai     = @IsActive
                WHERE id = @id";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var parameters = new[]
            {
                new SqlParameter("@FullName",  fullName),
                new SqlParameter("@Email",     email),
                new SqlParameter("@Password",  hashedPassword),
                new SqlParameter("@Role",      role),
                new SqlParameter("@IsActive",  isActive ? 1 : 0),
                new SqlParameter("@id",        userId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }


        // ─────────────────────────────────────────────────────
        // XÓA NGƯỜI DÙNG THEO ID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Xóa vĩnh viễn 1 người dùng khỏi bảng USERS theo ID.
        /// Trả về số dòng bị ảnh hưởng (> 0 là thành công).
        /// Lưu ý: thao tác này không thể hoàn tác — cần xác nhận trước khi gọi.
        /// </summary>
        public int DeleteById(int userId)
        {
            // Dùng parameterized query — chống SQL Injection tuyệt đối
            string query = "DELETE FROM USERS WHERE id = @id";

            var parameters = new[]
            {
                new SqlParameter("@id", userId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters);
        }
    }
}