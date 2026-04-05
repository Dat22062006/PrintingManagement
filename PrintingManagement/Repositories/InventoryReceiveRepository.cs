// ═══════════════════════════════════════════════════════════════════
// ║  InventoryReceiveRepository.cs - DỮ LIỆU PHIẾU NHẬP KHO       ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmInventoryReceive.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmInventoryReceive.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class InventoryReceiveRepository
    {
        // ─────────────────────────────────────────────────────
        // SINH MÃ PHIẾU NHẬP TỰ ĐỘNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateReceiptCode để sinh mã phiếu nhập theo năm.
        /// Trả về mã dạng PN-2026-001.
        /// </summary>
        public string GenerateReceiptCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GenerateReceiptCode", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Year", DateTime.Today.Year);

                    var outputParam = new SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();
                    return outputParam.Value?.ToString() ?? $"PN-{DateTime.Today.Year}-001";
                }
            }
            catch
            {
                return $"PN-{DateTime.Today.Year}-001";
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH ĐƠN HÀNG CHƯA HOÀN THÀNH
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetPendingOrders để lấy đơn hàng chờ nhập kho.
        /// </summary>
        public DataTable GetPendingOrders()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetPendingOrders", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách đơn hàng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY DỮ LIỆU ĐƠN HÀNG ĐỂ ĐIỀN VÀO PHIẾU NHẬP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetOrderReceiveData để lấy thông tin NCC và chi tiết hàng.
        /// Trả về DataSet:
        ///   [0] Thông tin NCC + điều khoản
        ///   [1] Chi tiết hàng hóa
        /// </summary>
        public DataSet GetOrderReceiveData(int orderId)
        {
            var ds = new DataSet();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetOrderReceiveData", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu đơn hàng: {ex.Message}");
            }

            return ds;
        }


        // ─────────────────────────────────────────────────────
        // LƯU PHIẾU NHẬP (CHƯA GHI SỔ)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveInventoryReceipt để INSERT hoặc UPDATE phiếu nhập.
        /// receiptId = 0 → INSERT mới, > 0 → UPDATE.
        /// Trả về ID phiếu vừa tạo/cập nhật.
        /// </summary>
        public int SaveReceipt(
    int receiptId,
    string receiptCode,
    int orderId,
    int supplierId,
    DateTime documentDate,
    DateTime accountingDate,
    string createdBy,
    string receiveType,
    string paymentMethod,
    string paymentStatus,
    bool hasInvoice,
    string invoiceForm,
    string invoiceSymbol,
    string invoiceNumber,
    DateTime invoiceDate,
    string invoiceFile,
    int soNgayNo = 0,       // [NEW]
    DateTime? ngayDenHan = null)    // [NEW]
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_SaveInventoryReceipt", conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                    cmd.Parameters.AddWithValue("@ReceiptCode", receiptCode);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@DocumentDate", documentDate);
                    cmd.Parameters.AddWithValue("@AccountingDate", accountingDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@ReceiveType", receiveType);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    cmd.Parameters.AddWithValue("@HasInvoice", hasInvoice);
                    cmd.Parameters.AddWithValue("@InvoiceForm", invoiceForm);
                    cmd.Parameters.AddWithValue("@InvoiceSymbol", invoiceSymbol);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                    cmd.Parameters.AddWithValue("@InvoiceDate", invoiceDate);
                    cmd.Parameters.AddWithValue("@InvoiceFile", invoiceFile ?? "");

                    // [NEW] 2 tham số bổ sung
                    cmd.Parameters.AddWithValue("@SoNgayNo", soNgayNo);
                    cmd.Parameters.AddWithValue("@NgayDenHanTT",
                        ngayDenHan.HasValue ? (object)ngayDenHan.Value : DBNull.Value);

                    var outParam = new Microsoft.Data.SqlClient.SqlParameter(
                        "@NewReceiptId", System.Data.SqlDbType.Int)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(outParam);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outParam.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu phiếu nhập: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // GHI SỔ PHIẾU NHẬP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_PostInventoryReceipt để cập nhật tồn kho, công nợ
        /// và đánh dấu phiếu đã ghi sổ — tất cả trong 1 transaction.
        /// </summary>
        public void PostReceipt(int receiptId, int supplierId, DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_PostInventoryReceipt", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi ghi sổ phiếu nhập: {ex.Message}");
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
                xml.Append($"<MaterialId>{EscapeXml(row["MaterialId"])}</MaterialId>");
                xml.Append($"<DetailId>{EscapeXml(row["DetailId"])}</DetailId>");
                xml.Append($"<ReceivedQty>{EscapeXml(row["ReceivedQty"])}</ReceivedQty>");
                xml.Append($"<UnitPrice>{EscapeXml(row["UnitPrice"])}</UnitPrice>");
                xml.Append($"<VatAmount>{EscapeXml(row["VatAmount"])}</VatAmount>");
                xml.Append($"<LineTotal>{EscapeXml(row["LineTotal"])}</LineTotal>");
                xml.Append("</Item>");
            }

            xml.Append("</Details>");
            return xml.ToString();
        }
    }
}