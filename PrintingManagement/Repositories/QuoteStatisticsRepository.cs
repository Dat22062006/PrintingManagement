// ═══════════════════════════════════════════════════════════════════
// ║  QuoteStatisticsRepository.cs - DỮ LIỆU THỐNG KÊ BÁO GIÁ     ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmQuoteStatistics.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmQuoteStatistics.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class QuoteStatisticsRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY KPI TỔNG HỢP THEO THÁNG/NĂM
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteStatsSummary để lấy các số liệu KPI.
        /// Trả về DataTable (1 dòng) với TotalQuotes, Pending, Approved, Cancelled.
        /// </summary>
        public DataTable GetStatsSummary(int month, int year)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteStatsSummary", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thống kê tháng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY SỐ LƯỢNG BÁO GIÁ THEO TỪNG NGÀY
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteTrendByDay để lấy dữ liệu cho biểu đồ xu hướng.
        /// Trả về DataTable với cột DayOfMonth và QuoteCount.
        /// </summary>
        public DataTable GetTrendByDay(int month, int year)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteTrendByDay", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu xu hướng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY NHÓM TRẠNG THÁI CHO BIỂU ĐỒ TRÒN
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteStatusGroups để lấy dữ liệu cho biểu đồ donut.
        /// Trả về DataTable với cột StatusGroup và QuoteCount.
        /// </summary>
        public DataTable GetStatusGroups(int month, int year)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteStatusGroups", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu trạng thái: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT THEO KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteDetailByCustomer để lấy chi tiết theo từng KH.
        /// Truyền keyword để lọc, để trống để lấy tất cả.
        /// </summary>
        public DataTable GetDetailByCustomer(string keyword, int month, int year)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteDetailByCustomer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Keyword", keyword ?? "");
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải chi tiết theo khách hàng: {ex.Message}");
            }

            return dt;
        }
    }
}