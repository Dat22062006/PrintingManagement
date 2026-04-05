// ═══════════════════════════════════════════════════════════════════
// ║  PriceCalculationDetailRepository.cs                           ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] Thêm GetQuoteDetailById(detailId) — lấy đúng 1 dòng
//       CHI_TIET_BAO_GIA theo id cụ thể (mức SL đang được tích).
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class PriceCalculationDetailRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT THEO QUOTEID (mức đầu tiên / primary)
        // Giữ nguyên — dùng làm fallback khi detailId = 0
        // ─────────────────────────────────────────────────────

        public DataTable GetQuoteDetail(int quoteId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteDetailById", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải chi tiết báo giá: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // [FIX] LẤY CHI TIẾT THEO DETAILID CỤ THỂ
        // Dùng khi mở frmPriceCalculationDetail từ mức SL
        // đang được tích trong dgvLevels
        // ─────────────────────────────────────────────────────

        public DataTable GetQuoteDetailById(int detailId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
                        SELECT
                            So_Luong,
                            ISNULL(Tien_Giay,     0) AS Tien_Giay,
                            ISNULL(Tien_Kem,      0) AS Tien_Kem,
                            ISNULL(Tien_In,       0) AS Tien_In,
                            ISNULL(Tien_Can_Mang, 0) AS Tien_Can_Mang,
                            ISNULL(Tien_Metalize, 0) AS Tien_Metalize,
                            ISNULL(Tien_UV,       0) AS Tien_UV,
                            ISNULL(Tien_Be,       0) AS Tien_Be,
                            ISNULL(Tien_Khuon_Be, 0) AS Tien_Khuon_Be,
                            ISNULL(Tien_Dan,      0) AS Tien_Dan,
                            ISNULL(Tien_Day,      0) AS Tien_Day,
                            ISNULL(Tien_Nut,      0) AS Tien_Nut,
                            ISNULL(Tien_Thung,    0) AS Tien_Thung,
                            ISNULL(Tien_Xe_Giao,  0) AS Tien_Xe_Giao,
                            ISNULL(Tien_Proof,    0) AS Tien_Proof,
                            ISNULL(Tong_Gia_Thanh,     0) AS Tong_Gia_Thanh,
                            ISNULL(Gia_Moi_Cai,        0) AS Gia_Moi_Cai,
                            ISNULL(Gia_Bao_Khach,      0) AS Gia_Bao_Khach,
                            ISNULL(Tong_Gia_Bao_Khach, 0) AS Tong_Gia_Bao_Khach
                        FROM CHI_TIET_BAO_GIA
                        WHERE id = @DetailId", conn);
                    cmd.Parameters.AddWithValue("@DetailId", detailId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải chi tiết mức SL: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // LẤY HEADER BÁO GIÁ (dùng để in PDF)
        // ─────────────────────────────────────────────────────

        public DataTable GetQuoteHeader(int quoteId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteHeaderById", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải header báo giá: {ex.Message}");
            }
            return dt;
        }
    }
}