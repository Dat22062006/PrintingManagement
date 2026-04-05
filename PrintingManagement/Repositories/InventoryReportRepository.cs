// ═══════════════════════════════════════════════════════════════════
// ║  InventoryReportRepository.cs - DỮ LIỆU BÁO CÁO KHO           ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmInventoryReport.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmInventoryReport.
    /// </summary>
    public class InventoryReportRepository
    {
        // ─────────────────────────────────────────────────────
        // BÁO CÁO 1 — SỔ CHI TIẾT NHẬP XUẤT TỒN
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy danh sách vật tư có phát sinh trong kỳ.
        /// </summary>
        public DataTable GetDetailLedgerMaterials(DateTime fromDate, DateTime toDate)
        {
            return ExecuteSP("sp_GetDetailLedgerMaterials", fromDate, toDate);
        }

        /// <summary>
        /// Lấy tổng nhập/xuất + tồn cuối của 1 vật tư trong kỳ.
        /// Trả về DataTable (1 dòng): TotalStockIn, TotalStockOut, ClosingStock.
        /// </summary>
        public DataTable GetDetailLedgerAgg(int materialId, DateTime fromDate, DateTime toDate)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDetailLedgerAgg", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@MaterialId", materialId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải tổng hợp vật tư: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// Lấy phát sinh nhập của 1 vật tư trong kỳ.
        /// </summary>
        public DataTable GetDetailLedgerStockIn(int materialId, DateTime fromDate, DateTime toDate)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDetailLedgerStockIn", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@MaterialId", materialId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải phát sinh nhập: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// Lấy phát sinh xuất của 1 vật tư trong kỳ.
        /// </summary>
        public DataTable GetDetailLedgerStockOut(int materialId, DateTime fromDate, DateTime toDate)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDetailLedgerStockOut", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@MaterialId", materialId);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải phát sinh xuất: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // BÁO CÁO 2 — TỔNG HỢP TỒN KHO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy tổng hợp tồn kho tất cả vật tư trong kỳ.
        /// </summary>
        public DataTable GetInventorySummary(DateTime fromDate, DateTime toDate)
        {
            return ExecuteSP("sp_GetInventorySummary", fromDate, toDate);
        }


        // ─────────────────────────────────────────────────────
        // BÁO CÁO 3 — THẺ KHO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy toàn bộ phát sinh nhập + xuất trong kỳ (UNION) cho thẻ kho.
        /// </summary>
        public DataTable GetStockCard(DateTime fromDate, DateTime toDate)
        {
            return ExecuteSP("sp_GetStockCard", fromDate, toDate);
        }


        // ─────────────────────────────────────────────────────
        // HELPER — GỌI SP CÓ 2 THAM SỐ NGÀY
        // ─────────────────────────────────────────────────────

        private DataTable ExecuteSP(string spName, DateTime fromDate, DateTime toDate)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand(spName, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gọi {spName}: {ex.Message}");
            }

            return dt;
        }
    }
}