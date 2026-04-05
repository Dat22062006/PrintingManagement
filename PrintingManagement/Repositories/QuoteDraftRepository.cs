using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class QuoteDraftRepository
    {
        public void SaveDraft(int customerId, string tenUser, string draftJson)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_SaveQuoteDraft", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@TenUser", tenUser ?? "");
                    cmd.Parameters.AddWithValue("@DraftJson", draftJson ?? "{}");

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu bản nháp báo giá: {ex.Message}");
            }
        }

        public string GetDraftJson(int customerId, string tenUser)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteDraft", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@TenUser", tenUser ?? "");

                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải bản nháp báo giá: {ex.Message}");
            }
        }
    }
}