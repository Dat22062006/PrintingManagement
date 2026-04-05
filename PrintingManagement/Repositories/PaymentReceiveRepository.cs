// ═══════════════════════════════════════════════════════════════════
// ║  PaymentReceiveRepository.cs - DỮ LIỆU THU TIỀN KHÁCH HÀNG    ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmPaymentReceive.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmPaymentReceive.
    /// </summary>
    public class PaymentReceiveRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY KHÁCH HÀNG CÓ ĐƠN HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetCustomersWithDebt để lấy danh sách KH vào ComboBox.
        /// </summary>
        public DataTable GetCustomersWithDebt()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetCustomersWithDebt", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách khách hàng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY ĐƠN HÀNG CÒN NỢ
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetOrderDebtByCustomer để lấy đơn hàng còn nợ.
        /// Trả về: id, OrderCode (ưu tiên So_Hoa_Don — số HĐ thật; fallback Ma_Don_Ban),
        ///         DocumentCode (mã chứng từ đơn bán), OrderDate, DueDate, ProductName,
        ///         TotalAmount, Collected, Remaining.
        /// </summary>
        public DataTable GetOrderDebtByCustomer(int customerId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetOrderDebtByCustomer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải công nợ khách hàng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU THU TIỀN KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SavePaymentCollection để INSERT phiếu thu + UPDATE công nợ.
        /// SP xử lý toàn bộ trong 1 transaction — tự rollback nếu lỗi.
        /// Trả về tổng tiền thu thực tế.
        /// </summary>
        public decimal SaveCollection(
            int customerId, DateTime paymentDate,
            string paymentMethod, DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_SavePaymentCollection", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@PaymentDate", paymentDate.Date);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    var outputTotal = new SqlParameter("@ActualTotal", SqlDbType.Decimal)
                    {
                        Precision = 18,
                        Scale = 2,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputTotal);

                    cmd.ExecuteNonQuery();
                    return Convert.ToDecimal(outputTotal.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu thu tiền: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DATATABLE CHI TIẾT SANG XML
        // ─────────────────────────────────────────────────────

        private static string EscapeXml(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            string str = value.ToString() ?? "";
            return System.Security.SecurityElement.Escape(str);
        }

        private string BuildDetailXml(DataTable details)
        {
            var xml = new StringBuilder("<Details>");

            foreach (DataRow row in details.Rows)
            {
                xml.Append("<Item>");
                xml.Append($"<OrderId>{EscapeXml(row["OrderId"])}</OrderId>");
                xml.Append($"<AmountToCollect>{EscapeXml(row["AmountToCollect"])}</AmountToCollect>");
                xml.Append("</Item>");
            }

            xml.Append("</Details>");
            return xml.ToString();
        }
    }
}