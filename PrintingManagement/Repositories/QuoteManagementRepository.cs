// ═══════════════════════════════════════════════════════════════════
// ║  QuoteManagementRepository.cs                                   ║
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class QuoteManagementRepository
    {
        public DataSet GetQuoteList(string search, DateTime fromDate, DateTime toDate,
                                    int offset, int pageSize)
        {
            var ds = new DataSet();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteList", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@Search", search ?? "");
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi tải danh sách báo giá: {ex.Message}"); }
            return ds;
        }

        public void UpdateQuoteStatus(int quoteId, string newStatus)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_UpdateQuoteStatus", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}"); }
        }

        /// <summary>
        /// id dòng CHI_TIET_BAO_GIA là mức giá chính (hiển thị danh sách / duyệt). NULL = dùng mức SL lớn nhất trong SP.
        /// </summary>
        public int? GetQuotePrimaryTierId(int quoteId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT id_Muc_Chinh FROM BAO_GIA WHERE id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", quoteId);
                    object s = cmd.ExecuteScalar();
                    if (s == null || s == DBNull.Value)
                        return null;
                    return Convert.ToInt32(s);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi đọc mức giá chính: {ex.Message}"); }
        }

        public void SetQuotePrimaryTier(int quoteId, int chiTietId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
UPDATE BAO_GIA SET id_Muc_Chinh = @C
WHERE id = @Q
  AND EXISTS (SELECT 1 FROM CHI_TIET_BAO_GIA WHERE id = @C AND id_Bao_Gia = @Q)", conn);
                    cmd.Parameters.AddWithValue("@Q", quoteId);
                    cmd.Parameters.AddWithValue("@C", chiTietId);
                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException("Không cập nhật được mức giá — kiểm tra báo giá và dòng chi tiết.");
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi lưu mức giá chọn: {ex.Message}"); }
        }

        public DataSet GetQuoteForExcel(int quoteId)
        {
            var ds = new DataSet();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteForExcel", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi tải dữ liệu xuất Excel: {ex.Message}"); }
            return ds;
        }

        // [NEW] Lấy tất cả mức SL của 1 báo giá (cho panel chi tiết)
        public DataTable GetQuoteDetailFull(int quoteId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteDetailFull", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi tải chi tiết mức SL: {ex.Message}"); }
            return dt;
        }
    }
}