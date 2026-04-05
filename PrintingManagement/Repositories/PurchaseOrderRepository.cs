// ═══════════════════════════════════════════════════════════════════
// ║  PurchaseOrderRepository.cs - DỮ LIỆU ĐƠN MUA HÀNG           ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmPurchaseOrder.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmPurchaseOrder.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class PurchaseOrderRepository
    {
        // ─────────────────────────────────────────────────────
        // SINH MÃ ĐƠN HÀNG TỰ ĐỘNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateOrderCode để sinh mã đơn hàng theo năm.
        /// Trả về mã dạng DH-2026-001.
        /// </summary>
        public string GenerateOrderCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GenerateOrderCode", conn)
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
                    return outputParam.Value?.ToString() ?? $"DH-{DateTime.Today.Year}-001";
                }
            }
            catch
            {
                return $"DH-{DateTime.Today.Year}-001";
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH NHÀ CUNG CẤP
        // [FIX] Sửa N+1 query — gọi SP duy nhất thay vì query trong vòng lặp
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy toàn bộ NCC từ <c>sp_GetSuppliers</c>, thêm cột đơn mới nhất (đơn NCC = bảng <c>DON_DAT_HANG_NCC</c>).
        /// </summary>
        public DataTable GetSuppliers()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetSuppliers", conn)
                    { CommandType = CommandType.StoredProcedure };
                    new SqlDataAdapter(cmd).Fill(dt);

                    dt.Columns.Add("LatestOrderId", typeof(int));
                    dt.Columns.Add("LatestOrderCode", typeof(string));
                    dt.Columns.Add("LatestOrderDate", typeof(DateTime));
                    dt.Columns.Add("LatestOrderStatus", typeof(string));

                    var latestOrders = new System.Collections.Generic.Dictionary<int, (int Id, string Code, DateTime? Date, string Status)>();
                    using (var cmd2 = new SqlCommand(@"
                        SELECT id_Nha_Cung_Cap, id, Ma_Don_Hang, Ngay_Dat_Hang, Trang_Thai
                        FROM DON_DAT_HANG_NCC
                        WHERE id IN (
                            SELECT MAX(id) FROM DON_DAT_HANG_NCC GROUP BY id_Nha_Cung_Cap
                        )", conn))
                    using (var r = cmd2.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            int supId = r.GetInt32(0);
                            DateTime? orderDate = r.IsDBNull(3) ? null : r.GetDateTime(3);
                            string status = r.IsDBNull(4) ? "" : r["Trang_Thai"]?.ToString() ?? "";
                            latestOrders[supId] = (
                                r.GetInt32(1),
                                r["Ma_Don_Hang"]?.ToString() ?? "",
                                orderDate,
                                status
                            );
                        }
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        int supId = Convert.ToInt32(row["id"]);
                        if (latestOrders.TryGetValue(supId, out var order))
                        {
                            row["LatestOrderId"] = order.Id;
                            row["LatestOrderCode"] = order.Code;
                            row["LatestOrderDate"] = order.Date.HasValue ? order.Date.Value : (object)DBNull.Value;
                            row["LatestOrderStatus"] = order.Status;
                        }
                        else
                        {
                            row["LatestOrderId"] = 0;
                            row["LatestOrderCode"] = "";
                            row["LatestOrderDate"] = DBNull.Value;
                            row["LatestOrderStatus"] = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // KIỂM TRA HOẶC TẠO NHÀ CUNG CẤP MỚI
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_CheckOrCreateSupplier để kiểm tra hoặc tạo NCC.
        /// Trả về tuple (supplierId, supplierCode, isNew).
        /// </summary>
        public (int supplierId, string supplierCode, bool isNew) CheckOrCreateSupplier(
            string name, string address, string taxCode)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_CheckOrCreateSupplier", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@TaxCode", taxCode);

                    var paramId = new SqlParameter("@SupplierId", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    var paramCode = new SqlParameter("@SupplierCode", SqlDbType.NVarChar, 50)
                    { Direction = ParameterDirection.Output };
                    var paramIsNew = new SqlParameter("@IsNew", SqlDbType.Bit)
                    { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(paramId);
                    cmd.Parameters.Add(paramCode);
                    cmd.Parameters.Add(paramIsNew);

                    cmd.ExecuteNonQuery();

                    return (
                        Convert.ToInt32(paramId.Value),
                        paramCode.Value?.ToString() ?? "",
                        Convert.ToBoolean(paramIsNew.Value)
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra nhà cung cấp: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // XÓA NHÀ CUNG CẤP
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_DeleteSupplier để xóa NCC nếu không có đơn hàng.
        /// Trả về số đơn hàng liên quan (> 0 = không thể xóa).
        /// </summary>
        public int DeleteSupplier(int supplierId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_DeleteSupplier", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);

                    var paramCount = new SqlParameter("@OrderCount", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(paramCount);

                    cmd.ExecuteNonQuery();

                    return Convert.ToInt32(paramCount.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhà cung cấp: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY THÔNG TIN NGUYÊN LIỆU THEO ID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetMaterialById để lấy thông tin preload từ frmInventoryIssue.
        /// </summary>
        public DataTable GetMaterialById(int materialId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetMaterialById", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@MaterialId", materialId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải thông tin nguyên liệu: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // SINH MÃ NGUYÊN LIỆU TỰ ĐỘNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateMaterialCode để sinh mã nguyên liệu mới.
        /// extraCount: số dòng đang có trên grid chưa lưu.
        /// </summary>
        public string GenerateMaterialCode(int extraCount = 0)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GenerateMaterialCode", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ExtraCount", extraCount);

                    var outputParam = new SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();
                    return outputParam.Value?.ToString() ?? "MH001";
                }
            }
            catch
            {
                return "MH001";
            }
        }

        /// <summary>
        /// Đơn mua đã có phiếu nhập kho — không được UPDATE chi tiết (theo sp_UpdatePurchaseOrder).
        /// </summary>
        public bool PurchaseOrderHasReceipt(int orderId)
        {
            if (orderId <= 0) return false;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_PurchaseOrderHasReceipt", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    var o = cmd.ExecuteScalar();
                    return o != null && o != DBNull.Value && Convert.ToBoolean(o);
                }
            }
            catch
            {
                return false;
            }
        }


        // ─────────────────────────────────────────────────────
        // ĐƠN ĐÃ LƯU THEO NHÀ CUNG CẤP (mở lại để mua thêm)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Đơn chưa hoàn thành / chưa hủy của NCC — hiển thị trong ComboBox trên form.
        /// </summary>
        public DataTable GetOpenPurchaseOrdersBySupplier(int supplierId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetOpenPurchaseOrdersBySupplier", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải đơn hàng của NCC: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Bảng [0] header đơn, [1] chi tiết dòng hàng.
        /// </summary>
        public DataSet GetPurchaseOrderById(int orderId)
        {
            var ds = new DataSet();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetPurchaseOrderById", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải đơn mua hàng: {ex.Message}");
            }
            return ds;
        }

        // ─────────────────────────────────────────────────────
        // LƯU ĐƠN MUA HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Cập nhật đơn đã có + thay toàn bộ chi tiết (không cho nếu đã có phiếu nhập kho).
        /// </summary>
        public int UpdatePurchaseOrder(
            int orderId,
            string orderCode, int supplierId,
            DateTime orderDate, DateTime deliveryDate,
            string paymentMethod, int debtDays, string orderStatus,
            string deliveryLocation, string attachedFile, string ghiChu, decimal grandTotal,
            DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string detailXml = BuildDetailXml(details);
                    var cmd = new SqlCommand("sp_UpdatePurchaseOrder", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@OrderCode", orderCode);
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@OrderDate", orderDate.Date);
                    cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate.Date);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@DebtDays", debtDays);
                    cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
                    cmd.Parameters.AddWithValue("@DeliveryLocation", deliveryLocation);
                    cmd.Parameters.AddWithValue("@AttachedFile", attachedFile);
                    cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(ghiChu) ? (object)DBNull.Value : ghiChu);
                    cmd.Parameters.AddWithValue("@GrandTotal", grandTotal);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    var outputId = new SqlParameter("@NewOrderId", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputId.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật đơn mua hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Gọi sp_SavePurchaseOrder để lưu đơn hàng + chi tiết trong 1 transaction.
        /// SP tự xử lý INSERT nguyên liệu mới nếu chưa có.
        /// Trả về ID đơn hàng vừa tạo.
        /// </summary>
        public int SavePurchaseOrder(
            string orderCode, int supplierId,
            DateTime orderDate, DateTime deliveryDate,
            string paymentMethod, int debtDays, string orderStatus,
            string deliveryLocation, string attachedFile, string ghiChu, decimal grandTotal,
            DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_SavePurchaseOrder", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@OrderCode", orderCode);
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@OrderDate", orderDate.Date);
                    cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate.Date);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@DebtDays", debtDays);
                    cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
                    cmd.Parameters.AddWithValue("@DeliveryLocation", deliveryLocation);
                    cmd.Parameters.AddWithValue("@AttachedFile", attachedFile);
                    cmd.Parameters.AddWithValue("@GhiChu", string.IsNullOrEmpty(ghiChu) ? (object)DBNull.Value : ghiChu);
                    cmd.Parameters.AddWithValue("@GrandTotal", grandTotal);
                    cmd.Parameters.AddWithValue("@Details", detailXml);

                    var outputId = new SqlParameter("@NewOrderId", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputId.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu đơn mua hàng: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DATATABLE CHI TIẾT SANG XML
        // ─────────────────────────────────────────────────────

        private string BuildDetailXml(DataTable details)
        {
            var root = new XElement("Details");

            foreach (DataRow row in details.Rows)
            {
                var item = new XElement("Item",
                    new XElement("MaterialId", row["MaterialId"] ?? 0),
                    new XElement("MaterialCode", row["MaterialCode"]?.ToString() ?? ""),
                    new XElement("MaterialName", row["MaterialName"]?.ToString() ?? ""),
                    new XElement("Unit", row["Unit"]?.ToString() ?? ""),
                    new XElement("Qty", row["Qty"] ?? 0),
                    new XElement("UnitPrice", row["UnitPrice"] ?? 0),
                    new XElement("LineTotal", row["LineTotal"] ?? 0),
                    new XElement("VatRate", row["VatRate"] ?? 0),
                    new XElement("VatAmount", row["VatAmount"] ?? 0)
                );
                root.Add(item);
            }

            return root.ToString();
        }


        // ─────────────────────────────────────────────────────
        // DANH MỤC VẬT TƯ THEO NCC
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lấy danh sách vật tư đã lưu của 1 NCC (từ bảng NguyenLieu_NCC).
        /// </summary>
        public DataTable GetMaterialsBySupplier(int supplierId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetMaterialsBySupplier", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh mục vật tư NCC: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// Lưu danh sách vật tư vào danh mục NCC.
        /// Những vật tư mới (chưa có trong danh mục) sẽ được tự động thêm.
        /// </summary>
        public void SaveMaterialsForSupplier(int supplierId, DataTable materials)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string xml = BuildMaterialsXml(materials);
                    var cmd = new SqlCommand("sp_SaveMaterialsForSupplier", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@Materials", xml);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu danh mục vật tư NCC: {ex.Message}");
            }
        }

        private string BuildMaterialsXml(DataTable materials)
        {
            var root = new XElement("Materials");
            foreach (DataRow row in materials.Rows)
            {
                string itemName = row["MaterialName"]?.ToString().Trim() ?? "";
                if (string.IsNullOrWhiteSpace(itemName)) continue;

                root.Add(new XElement("Item",
                    new XElement("MaterialId", row["MaterialId"] ?? 0),
                    new XElement("MaterialCode", row["MaterialCode"]?.ToString() ?? ""),
                    new XElement("MaterialName", itemName),
                    new XElement("Unit", row["Unit"]?.ToString() ?? ""),
                    new XElement("UnitPrice", row["UnitPrice"] ?? 0)));
            }
            return root.ToString();
        }
    }
}