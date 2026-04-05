// ═══════════════════════════════════════════════════════════════════
// ║  WarehouseExportRepository.cs - DỮ LIỆU XUẤT KHO SẢN XUẤT    ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung TOÀN BỘ thao tác DB liên quan đến xuất kho.
// Toàn bộ SQL gọi qua Stored Procedure — form không viết SQL trực tiếp.
// Trigger phía DB tự động trừ tồn kho sau mỗi lần INSERT chi tiết.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho màn hình Xuất kho SX.
    /// Mọi SQL đều gọi qua Stored Procedure — không viết inline SQL.
    /// </summary>
    public class WarehouseExportRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH NGUYÊN LIỆU
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetMaterials để lấy toàn bộ nguyên liệu trong kho.
        /// Dùng để nạp cache và ComboBox chọn mã hàng.
        /// </summary>
        public DataTable GetMaterials()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Gọi Stored Procedure — không viết SELECT trực tiếp
                    var cmd = new SqlCommand("sp_GetMaterials", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nguyên liệu: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH LỆNH SẢN XUẤT ĐANG CHẠY
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetActiveProductionOrders để lấy lệnh SX đang sản xuất.
        /// Chỉ trả về lệnh có trạng thái "Đang sản xuất".
        /// </summary>
        public DataTable GetActiveProductionOrders()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Gọi Stored Procedure — không viết SELECT trực tiếp
                    var cmd = new SqlCommand("sp_GetActiveProductionOrders", conn)
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
        // SINH MÃ PHIẾU XUẤT TỰ ĐỘNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateExportCode để sinh mã phiếu xuất theo năm hiện tại.
        /// Trả về mã dạng PX-2026-001.
        /// </summary>
        public string GenerateExportCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Gọi Stored Procedure — không viết SELECT trực tiếp
                    var cmd = new SqlCommand("sp_GenerateExportCode", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Year", DateTime.Today.Year);

                    // Tham số OUTPUT để nhận mã phiếu trả về từ SP
                    var outputParam = new SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();

                    return outputParam.Value?.ToString() ?? $"PX-{DateTime.Today.Year}-001";
                }
            }
            catch
            {
                // Trả về mã mặc định nếu lỗi kết nối
                return $"PX-{DateTime.Today.Year}-001";
            }
        }


        // ─────────────────────────────────────────────────────
        // LƯU PHIẾU XUẤT KHO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveWarehouseExport để lưu phiếu xuất và chi tiết.
        /// SP tự quản lý transaction — nếu lỗi sẽ rollback toàn bộ.
        /// Trigger trg_AfterInsertExportDetail tự trừ tồn kho sau khi INSERT.
        ///
        /// Tham số details: danh sách vật tư cần xuất.
        /// Trả về ID phiếu xuất vừa tạo (> 0 là thành công).
        /// </summary>
        public int SaveExportReceipt(
            string exportCode,
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

                    // Chuyển danh sách chi tiết sang XML để truyền vào SP
                    string detailXml = BuildDetailXml(details);

                    // Gọi Stored Procedure — toàn bộ transaction nằm trong SP
                    var cmd = new SqlCommand("sp_SaveWarehouseExport", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                    cmd.Parameters.AddWithValue("@ProductionOrderId", productionOrderId);
                    cmd.Parameters.AddWithValue("@ExportDate", exportDate.Date);
                    cmd.Parameters.AddWithValue("@Receiver", receiver);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    // Tham số OUTPUT nhận ID phiếu vừa tạo từ SP
                    var outputId = new SqlParameter("@NewExportId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();

                    // [FIX] Kiểm tra DBNull trước khi convert
                    return (outputId.Value != null && outputId.Value != DBNull.Value)
                        ? Convert.ToInt32(outputId.Value)
                        : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu phiếu xuất kho: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DANH SÁCH CHI TIẾT SANG XML
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Chuyển DataTable chi tiết xuất kho sang chuỗi XML
        /// để truyền vào tham số @Details của sp_SaveWarehouseExport.
        ///
        /// Định dạng XML:
        /// <Details>
        ///   <Item>
        ///     <MaterialId>1</MaterialId>
        ///     <Qty>100</Qty>
        ///     <UnitPrice>50000</UnitPrice>
        ///     <LineTotal>5000000</LineTotal>
        ///   </Item>
        /// </Details>
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