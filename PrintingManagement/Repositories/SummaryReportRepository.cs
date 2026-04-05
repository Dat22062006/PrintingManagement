// ═══════════════════════════════════════════════════════════════════
// ║  SummaryReportRepository.cs - DỮ LIỆU BÁO CÁO TỔNG HỢP        ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmSummaryReport.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// DTO chứa toàn bộ dữ liệu dashboard cần hiển thị.
    /// </summary>
    public class SummaryReportData
    {
        public double RevenueThisMonth { get; set; }
        public double RevenuePrevMonth { get; set; }
        public double CostThisMonth { get; set; }
        public double CostPrevMonth { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersPrevMonth { get; set; }
        public double ReceivableDebt { get; set; }
        public double StockValue { get; set; }
    }

    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmSummaryReport.
    /// </summary>
    public class SummaryReportRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY TOÀN BỘ DỮ LIỆU TRONG 1 LẦN GỌI DB
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSummaryReportData để lấy tất cả chỉ số dashboard.
        /// Trả về DTO đã map sẵn — form chỉ dùng properties.
        /// </summary>
        public SummaryReportData GetSummaryData()
        {
            var result = new SummaryReportData();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSummaryReportData", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        result.RevenueThisMonth = Convert.ToDouble(row["RevenueThisMonth"]);
                        result.RevenuePrevMonth = Convert.ToDouble(row["RevenuePrevMonth"]);
                        result.CostThisMonth = Convert.ToDouble(row["CostThisMonth"]);
                        result.CostPrevMonth = Convert.ToDouble(row["CostPrevMonth"]);
                        result.OrdersThisMonth = Convert.ToInt32(row["OrdersThisMonth"]);
                        result.OrdersPrevMonth = Convert.ToInt32(row["OrdersPrevMonth"]);
                        result.ReceivableDebt = Convert.ToDouble(row["ReceivableDebt"]);
                        result.StockValue = Convert.ToDouble(row["StockValue"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu tổng hợp: {ex.Message}");
            }

            return result;
        }
    }
}