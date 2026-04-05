// ═══════════════════════════════════════════════════════════════════
// ║  DashboardRepository.cs - DỮ LIỆU DASHBOARD                   ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho dashboard.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmDashBoard.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class DashboardRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY SỐ LIỆU KPI TỔNG HỢP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDashboardStats để lấy toàn bộ số liệu KPI
        /// trong 1 lần gọi DB duy nhất — tối ưu hiệu năng.
        /// </summary>
        public DataRow GetDashboardStats()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDashboardStats", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    // Trả về dòng đầu tiên chứa toàn bộ số liệu
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải số liệu dashboard: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY TOP 4 BÁO GIÁ MỚI NHẤT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDashboardQuotes để lấy 4 báo giá gần nhất.
        /// </summary>
        public DataTable GetRecentQuotes()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDashboardQuotes", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách báo giá: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY TOP 10 LỆNH SẢN XUẤT ĐANG CHẠY
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDashboardProduction để lấy lệnh SX đang sản xuất.
        /// </summary>
        public DataTable GetActiveProduction()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDashboardProduction", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải lệnh sản xuất: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY THÔNG BÁO DASHBOARD
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDashboardNotifications để lấy thông báo.
        /// Trả về DataSet gồm 4 bảng:
        ///   [0] Báo giá mới duyệt
        ///   [1] Vật tư sắp hết
        ///   [2] Phiếu nhập hoàn thành
        ///   [3] Công nợ quá hạn
        /// </summary>
        public DataSet GetNotifications()
        {
            var ds = new DataSet();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDashboardNotifications", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông báo: {ex.Message}");
            }

            return ds;
        }
    }
}