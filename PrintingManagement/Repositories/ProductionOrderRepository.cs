// ═══════════════════════════════════════════════════════════════════
// ║  ProductionOrderRepository.cs - DỮ LIỆU LỆNH SẢN XUẤT         ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmProductionOrder.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmProductionOrder.
    /// </summary>
    public class ProductionOrderRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH BÁO GIÁ ĐÃ DUYỆT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetApprovedQuotes để lấy báo giá đưa vào ComboBox.
        /// </summary>
        public DataTable GetApprovedQuotes()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetApprovedQuotes", conn)
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
        // LẤY CHI TIẾT 1 BÁO GIÁ ĐỂ ĐIỀN FORM
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteForProduction để lấy thông tin hiển thị trên form.
        /// Trả về DataTable (1 dòng): Ten_San_Pham, id_Khach_Hang,
        ///                            Ten_Khach_Hang, So_Luong.
        /// </summary>
        public DataTable GetQuoteForProduction(int quoteId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteForProduction", conn)
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
        // LẤY TỒN KHO ĐỂ KIỂM TRA
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetMaterialsForCheck để lấy tồn kho nguyên liệu.
        /// Trả về: id, Ten_Nguyen_Lieu, Don_Vi_Tinh, Ton_Kho.
        /// </summary>
        public DataTable GetMaterialsForCheck()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetMaterialsForCheck", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra tồn kho: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LƯU LỆNH SẢN XUẤT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveProductionOrder để INSERT lệnh SX + UPDATE báo giá.
        /// SP tự sinh mã và xử lý transaction.
        /// Trả về mã lệnh sản xuất vừa tạo.
        /// </summary>
        public string SaveProductionOrder(
            int quoteId, int customerId, string productName,
            int quantity, DateTime startDate,
            out int productionOrderId)
        {
            productionOrderId = 0;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_SaveProductionOrder", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@StartDate", startDate.Date);

                    var outputCode = new SqlParameter("@ProductionCode", SqlDbType.NVarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var outputId = new SqlParameter("@ProductionOrderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputCode);
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();
                    // [FIX] Kiểm tra DBNull trước khi convert
                    productionOrderId = (outputId.Value != null && outputId.Value != DBNull.Value)
                        ? Convert.ToInt32(outputId.Value)
                        : 0;
                    return outputCode.Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu lệnh sản xuất: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT ĐỂ XUẤT EXCEL
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteForExcelExport để lấy đầy đủ thông tin in Excel.
        /// </summary>
        public DataTable GetQuoteForExcelExport(int quoteId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteForExcelExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải dữ liệu xuất Excel: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // SINH MÃ PHIẾU XUẤT KHO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateExportCode để sinh mã phiếu xuất kho tự động.
        /// </summary>
        public string GenerateExportCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GenerateExportCode", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Year", DateTime.Today.Year);
                    var output = new SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(output);
                    cmd.ExecuteNonQuery();
                    return output.Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi sinh mã phiếu xuất: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // TẠO PHIẾU XUẤT KHO CHO LỆNH SẢN XUẤT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Tự động tạo phiếu xuất kho nguyên liệu cho lệnh SX vừa tạo.
        /// Dùng sp_SaveWarehouseExport (cùng SP với frmWarehouseExport).
        /// details: DataTable có cột MaterialId, Qty, UnitPrice, LineTotal.
        /// Trả về mã phiếu xuất vừa tạo.
        /// </summary>
        public int CreateExportReceiptForProductionOrder(
            int productionOrderId,
            DateTime exportDate,
            string receiver,
            DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string exportCode = GenerateExportCode();
                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_SaveWarehouseExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                    cmd.Parameters.AddWithValue("@ProductionOrderId", productionOrderId);
                    cmd.Parameters.AddWithValue("@ExportDate", exportDate.Date);
                    cmd.Parameters.AddWithValue("@Receiver", receiver);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    var outputId = new SqlParameter("@NewExportId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputId.Value ?? 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo phiếu xuất kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Chuyển DataTable chi tiết xuất kho sang chuỗi XML
        /// để truyền vào sp_SaveWarehouseExport.
        /// </summary>
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
                xml.Append($"<Qty>{EscapeXml(row["Qty"])}</Qty>");
                xml.Append($"<UnitPrice>{EscapeXml(row["UnitPrice"])}</UnitPrice>");
                xml.Append($"<LineTotal>{EscapeXml(row["LineTotal"])}</LineTotal>");
                xml.Append("</Item>");
            }
            xml.Append("</Details>");
            return xml.ToString();
        }
    }
}