// ═══════════════════════════════════════════════════════════════════
// ║  SupplierPaymentRepository.cs - DỮ LIỆU THANH TOÁN NCC        ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmSupplierPayment.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmSupplierPayment.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class SupplierPaymentRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH NHÀ CUNG CẤP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSuppliersForPayment để lấy danh sách NCC.
        /// </summary>
        public DataTable GetSuppliers()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSuppliersForPayment", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH ĐƠN HÀNG CÒN NỢ
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSupplierDebtList để lấy đơn hàng còn nợ của NCC.
        /// Trả về các cột: OrderId, OrderCode, OrderDate, Description,
        ///                  TotalAmount, PaidAmount.
        /// </summary>
        public DataTable GetDebtList(int supplierId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSupplierDebtList", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách công nợ: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU THANH TOÁN
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveSupplierPayment để INSERT thanh toán + UPDATE công nợ.
        /// SP xử lý toàn bộ trong 1 transaction — tự rollback nếu lỗi.
        /// Trả về tổng tiền đã thanh toán thực tế.
        /// </summary>
        public decimal SavePayment(
            int supplierId, DateTime paymentDate,
            string paymentMethod, DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_SaveSupplierPayment", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@PaymentDate", paymentDate.Date);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    var outputTotal = new SqlParameter("@TotalPaid", SqlDbType.Decimal)
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
                throw new Exception($"Lỗi khi lưu thanh toán: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DATATABLE SANG XML
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
                xml.Append($"<PaymentAmount>{EscapeXml(row["PaymentAmount"])}</PaymentAmount>");
                xml.Append("</Item>");
            }

            xml.Append("</Details>");
            return xml.ToString();
        }
    }
}