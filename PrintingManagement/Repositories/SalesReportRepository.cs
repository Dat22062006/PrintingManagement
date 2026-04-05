// ═══════════════════════════════════════════════════════════════════
// ║  SalesReportRepository.cs - DỮ LIỆU BÁO CÁO DOANH THU         ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmSalesReport.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmSalesReport.
    /// </summary>
    public class SalesReportRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY KPI TỔNG HỢP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSalesReportSummary để lấy tất cả KPI trong 1 lần gọi.
        /// Trả về DataTable (1 dòng) với:
        ///   TotalRevenue, MonthlyRevenue, PrevMonthRevenue, Profit, CustomerCount.
        /// </summary>
        public DataTable GetSummary()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSalesReportSummary", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải KPI doanh thu: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // DOANH THU THEO SẢN PHẨM
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetRevenueByProduct để lấy doanh thu nhóm theo sản phẩm.
        /// Trả về: ProductName, TotalQty, TotalRevenue.
        /// </summary>
        public DataTable GetRevenueByProduct()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetRevenueByProduct", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải doanh thu theo sản phẩm: {ex.Message}");
            }

            return dt;
        }
    }
}