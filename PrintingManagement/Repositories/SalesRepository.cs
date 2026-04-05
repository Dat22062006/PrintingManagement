// ═══════════════════════════════════════════════════════════════════
// ║  SalesRepository.cs - DỮ LIỆU BÁN HÀNG                        ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmSales.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmSales.
    /// </summary>
    public class SalesRepository
    {
        // ─────────────────────────────────────────────────────
        // SINH MÃ ĐƠN BÁN HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateSalesCode để sinh mã BH-2026-001.
        /// </summary>
        public string GenerateSalesCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GenerateSalesCode", conn)
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
                    return outputParam.Value?.ToString() ?? $"BH-{DateTime.Today.Year}-001";
                }
            }
            catch
            {
                return $"BH-{DateTime.Today.Year}-001";
            }
        }


        // ─────────────────────────────────────────────────────
        // SINH SỐ HÓA ĐƠN NỘI BỘ (HD-YYYY-######
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateSalesInvoiceNumber — định dạng HD-2026-000001.
        /// Nếu SP chưa cập nhật, fallback theo năm (000001).
        /// </summary>
        public string GenerateSalesInvoiceNumber()
        {
            int year = DateTime.Today.Year;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GenerateSalesInvoiceNumber", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Year", year);
                    var outN = new SqlParameter("@NextNumber", SqlDbType.NVarChar, 32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outN);
                    cmd.ExecuteNonQuery();
                    string n = outN.Value?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(n))
                        return n;
                }
            }
            catch
            {
                /* SP chưa có hoặc lỗi — dùng fallback */
            }

            return $"HD-{year}-000001";
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH BÁO GIÁ HOÀN THÀNH
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetCompletedQuotes để lấy báo giá đưa vào ComboBox.
        /// </summary>
        public DataTable GetCompletedQuotes()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetCompletedQuotes", conn)
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
        // PHIẾU GIAO ĐÃ GIAO — CHỌN NGUỒN CHO ĐƠN BÁN
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDeliveredNotesForSales — PGH trạng thái Đã giao, chưa lập đơn bán.
        /// </summary>
        public DataTable GetDeliveredNotesForSales()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDeliveredNotesForSales", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách phiếu giao hàng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT BÁO GIÁ
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteDetails để lấy danh sách sản phẩm của báo giá.
        /// Trả về: id, Ten_San_Pham, So_Luong, UnitPrice, LineTotal.
        /// </summary>
        public DataTable GetQuoteDetails(int quoteId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteDetails", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

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
        // LẤY THÔNG TIN KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Dùng lại sp_GetCustomerById (đã tạo từ CustomerManagement).
        /// Trả về: Ten_Khach_Hang, Dia_Chi, MST.
        /// </summary>
        public DataTable GetCustomerById(int customerId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetCustomerById", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin khách hàng: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH ĐƠN BÁN HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSalesInvoiceList để lấy tất cả đơn bán.
        /// </summary>
        public DataTable GetSalesInvoiceList()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSalesInvoiceList", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách hóa đơn: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU ĐƠN BÁN HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveSalesOrder để lưu đơn bán + chi tiết.
        /// <paramref name="phieuGiaoId"/> &gt; 0: liên kết phiếu giao (id báo giá lấy từ PGH).
        /// </summary>
        public int SaveSalesOrder(
            string documentCode, int customerId, DateTime saleDate,
            string invoiceForm, string invoiceSymbol, string invoiceNumber, DateTime invoiceDate,
            decimal subTotal, decimal vatRate, decimal totalVat, decimal grandTotal,
            int quoteId, int phieuGiaoId, DataTable details,
            DateTime? expectedPaymentDate,
            string debitAccount,
            string creditAccount)
        {
            try
            {
                try
                {
                    return ExecuteSaveSalesOrder(
                        documentCode, customerId, saleDate,
                        invoiceForm, invoiceSymbol, invoiceNumber, invoiceDate,
                        subTotal, vatRate, totalVat, grandTotal,
                        quoteId, phieuGiaoId, details,
                        expectedPaymentDate, debitAccount, creditAccount,
                        includeAccountingParams: true, includePhieuGiaoParam: true);
                }
                catch (Exception ex) when (IsSpTooManyArgumentsSpecified(ex))
                {
                    try
                    {
                        return ExecuteSaveSalesOrder(
                            documentCode, customerId, saleDate,
                            invoiceForm, invoiceSymbol, invoiceNumber, invoiceDate,
                            subTotal, vatRate, totalVat, grandTotal,
                            quoteId, phieuGiaoId, details,
                            expectedPaymentDate, debitAccount, creditAccount,
                            includeAccountingParams: false, includePhieuGiaoParam: true);
                    }
                    catch (Exception ex2) when (IsSpTooManyArgumentsSpecified(ex2))
                    {
                        if (phieuGiaoId > 0)
                            throw new Exception(
                                "Database cần chạy script MainSQL (sp_SaveSalesOrder + cột id_Phieu_Giao_Hang) để lưu theo phiếu giao hàng.",
                                ex2);
                        return ExecuteSaveSalesOrder(
                            documentCode, customerId, saleDate,
                            invoiceForm, invoiceSymbol, invoiceNumber, invoiceDate,
                            subTotal, vatRate, totalVat, grandTotal,
                            quoteId, phieuGiaoId, details,
                            expectedPaymentDate, debitAccount, creditAccount,
                            includeAccountingParams: false, includePhieuGiaoParam: false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu đơn bán hàng: {ex.Message}", ex);
            }
        }

        private static bool IsSpTooManyArgumentsSpecified(Exception ex)
        {
            for (Exception? e = ex; e != null; e = e.InnerException)
            {
                if (e.Message.Contains("too many arguments", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private int ExecuteSaveSalesOrder(
            string documentCode, int customerId, DateTime saleDate,
            string invoiceForm, string invoiceSymbol, string invoiceNumber, DateTime invoiceDate,
            decimal subTotal, decimal vatRate, decimal totalVat, decimal grandTotal,
            int quoteId, int phieuGiaoId, DataTable details,
            DateTime? expectedPaymentDate,
            string debitAccount,
            string creditAccount,
            bool includeAccountingParams,
            bool includePhieuGiaoParam)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string detailXml = BuildDetailXml(details);

            var cmd = new SqlCommand("sp_SaveSalesOrder", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DocumentCode", documentCode);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@SaleDate", saleDate.Date);
            cmd.Parameters.AddWithValue("@InvoiceForm", invoiceForm);
            cmd.Parameters.AddWithValue("@InvoiceSymbol", invoiceSymbol);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
            cmd.Parameters.AddWithValue("@InvoiceDate", invoiceDate.Date);
            cmd.Parameters.AddWithValue("@SubTotal", subTotal);
            cmd.Parameters.AddWithValue("@VatRate", vatRate);
            cmd.Parameters.AddWithValue("@TotalVat", totalVat);
            cmd.Parameters.AddWithValue("@GrandTotal", grandTotal);
            cmd.Parameters.AddWithValue("@QuoteId", quoteId);
            if (includePhieuGiaoParam)
                cmd.Parameters.AddWithValue("@PhieuGiaoId", phieuGiaoId);
            cmd.Parameters.AddWithValue("@Details", detailXml);

            if (includeAccountingParams)
            {
                cmd.Parameters.AddWithValue("@ExpectedPaymentDate",
                    expectedPaymentDate.HasValue ? expectedPaymentDate.Value.Date : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DebitAccount", debitAccount ?? "");
                cmd.Parameters.AddWithValue("@CreditAccount", creditAccount ?? "");
            }

            var outputId = new SqlParameter("@NewOrderId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            cmd.ExecuteNonQuery();
            if (outputId.Value == null || outputId.Value == DBNull.Value)
                throw new Exception("Không lấy được ID đơn hàng từ SQL Server.");
            return Convert.ToInt32(outputId.Value);
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DATATABLE SANG XML
        // ─────────────────────────────────────────────────────

        private string BuildDetailXml(DataTable details)
        {
            var root = new XElement("Details");

            foreach (DataRow row in details.Rows)
            {
                var item = new XElement("Item",
                    new XElement("ProductName", row["ProductName"]?.ToString() ?? ""),
                    new XElement("Quantity", row["Quantity"] ?? 0),
                    new XElement("UnitPrice", row["UnitPrice"] ?? 0),
                    new XElement("LineTotal", row["LineTotal"] ?? 0),
                    new XElement("VatRate", row["VatRate"] ?? 0),
                    new XElement("VatAmount", row["VatAmount"] ?? 0)
                );
                root.Add(item);
            }

            return root.ToString();
        }
    }
}