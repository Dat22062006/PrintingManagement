// ═══════════════════════════════════════════════════════════════════
// ║  PaperTypeRepository.cs - DỮ LIỆU LOẠI GIẤY                   ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Lấy và lưu danh sách loại giấy cho ComboBox
// frmPriceCalculation. Cho phép user tự thêm tên giấy mới.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class PaperTypeRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH LOẠI GIẤY
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetAllPaperTypes — trả về tất cả loại giấy
        /// (mặc định + do user thêm), sắp xếp mặc định lên đầu.
        /// </summary>
        public DataTable GetAllPaperTypes()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetAllPaperTypes", conn)
                    { CommandType = CommandType.StoredProcedure };
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách loại giấy: {ex.Message}");
            }
            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU LOẠI GIẤY MỚI DO USER TỰ NHẬP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SavePaperType để lưu loại giấy mới.
        /// Nếu đã tồn tại thì UPDATE, chưa có thì INSERT.
        /// </summary>
        public void SavePaperType(
            string paperName, decimal gsm,
            string printSize, decimal pricePerTon, int wastage = 500)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_SavePaperType", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@TenLoaiGiay", paperName);
                    cmd.Parameters.AddWithValue("@DinhLuong", gsm);
                    cmd.Parameters.AddWithValue("@KhoIn", printSize.Length > 0 ? (object)printSize : DBNull.Value);
                    cmd.Parameters.AddWithValue("@GiaTan", pricePerTon > 0 ? (object)pricePerTon : DBNull.Value);
                    cmd.Parameters.AddWithValue("@BuHao", wastage);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu loại giấy: {ex.Message}");
            }
        }
    }
}