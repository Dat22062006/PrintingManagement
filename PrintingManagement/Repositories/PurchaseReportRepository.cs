// ═══════════════════════════════════════════════════════════════════
// ║  PurchaseReportRepository.cs - DỮ LIỆU BÁO CÁO MUA HÀNG      ║
// [FIX/ADD] Thêm GetPurchaseDetail — chi tiết theo NCC + ngày nhập
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class PurchaseReportRepository
    {
        // ─────────────────────────────────────────────────────
        // TỔNG HỢP THEO NCC (tab 1)
        // ─────────────────────────────────────────────────────

        public DataTable GetPurchaseSummary()
        {
            var dt = new DataTable();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("sp_GetPurchaseSummary", conn)
                { CommandType = CommandType.StoredProcedure };
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tải tổng hợp mua hàng: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // [NEW] CHI TIẾT TỪNG LẦN NHẬP (tab 2)
        // Lọc theo: tên/mã NCC, khoảng ngày
        // ─────────────────────────────────────────────────────

        public DataTable GetPurchaseDetail(
            string supplierKeyword = "",
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var dt = new DataTable();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("sp_GetPurchaseDetail", conn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@SupplierKeyword", supplierKeyword ?? "");
                cmd.Parameters.AddWithValue("@FromDate",
                    fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate",
                    toDate.HasValue ? (object)toDate.Value : DBNull.Value);

                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tải chi tiết mua hàng: {ex.Message}");
            }
            return dt;
        }
    }
}