// ═══════════════════════════════════════════════════════════════════
// ║  InventoryRepository.cs - DỮ LIỆU TỒN KHO                     ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmInventoryIssue.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý thao tác DB cho frmInventoryIssue.
    /// </summary>
    public class InventoryRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DỮ LIỆU TỒN KHO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetInventoryData để lấy danh sách tồn kho.
        /// keyword: tìm theo mã/tên — để trống để lấy tất cả.
        /// codePrefix: lọc theo ký tự đầu mã hàng (G, M, P, T...).
        /// fromDate/toDate: kỳ tính Nhập/Xuất trong kỳ (cần SP có tham số @FromDate, @ToDate).
        /// </summary>
        public DataTable GetInventoryData(string keyword = "", string codePrefix = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            bool wantPeriod = fromDate.HasValue && toDate.HasValue;

            try
            {
                return FillInventory(keyword, codePrefix, wantPeriod ? fromDate!.Value.Date : (DateTime?)null, wantPeriod ? toDate!.Value.Date : (DateTime?)null, includePeriodParams: wantPeriod);
            }
            catch (SqlException ex) when (wantPeriod && ex.Message.Contains("too many arguments", StringComparison.OrdinalIgnoreCase))
            {
                // SP cũ chưa có @FromDate / @ToDate — chạy lại không gửi kỳ
                return FillInventory(keyword, codePrefix, null, null, includePeriodParams: false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu tồn kho: {ex.Message}");
            }
        }

        private static DataTable FillInventory(string keyword, string codePrefix, DateTime? fromDate, DateTime? toDate, bool includePeriodParams)
        {
            var dt = new DataTable();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand("sp_GetInventoryData", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Keyword", keyword ?? "");
                cmd.Parameters.AddWithValue("@CodePrefix", codePrefix ?? "");
                if (includePeriodParams && fromDate.HasValue && toDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
                }

                new SqlDataAdapter(cmd).Fill(dt);
            }

            return dt;
        }
    }
}