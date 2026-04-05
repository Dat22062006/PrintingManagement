// ═══════════════════════════════════════════════════════════════════
// ║  DeliveryNoteRepository.cs - DỮ LIỆU PHIẾU GIAO HÀNG           ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Tập trung toàn bộ thao tác DB cho frmDeliveryNote.
// Mọi SQL đều gọi qua Stored Procedure — form không viết SQL.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    /// <summary>
    /// Repository xử lý toàn bộ thao tác DB cho frmDeliveryNote.
    /// Mọi SQL đều gọi qua Stored Procedure.
    /// </summary>
    public class DeliveryNoteRepository
    {
        // ─────────────────────────────────────────────────────
        // SINH MÃ PHIẾU GIAO HÀNG TỰ ĐỘNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateDeliveryCode sinh mã PGH-YYYY-NNN.
        /// </summary>
        public string GenerateDeliveryCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GenerateDeliveryCode", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Year", DateTime.Today.Year);

                    var outputParam = new SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();
                    return outputParam.Value?.ToString() ?? $"PGH-{DateTime.Today.Year}-001";
                }
            }
            catch
            {
                return $"PGH-{DateTime.Today.Year}-001";
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH BÁO GIÁ HOÀN THÀNH CHO COMBOBOX
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetCompletedQuotesForDelivery — chỉ lấy báo giá Trang_Thai = Hoàn thành.
        /// </summary>
        public DataTable GetCompletedQuotes()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetCompletedQuotesForDelivery", conn)
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
        // LẤY CHI TIẾT BÁO GIÁ ĐỂ TẠO PHIẾU GIAO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetQuoteDetailsForDelivery để lấy sản phẩm từ báo giá.
        /// </summary>
        public DataTable GetQuoteDetailsForDelivery(int quoteId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetQuoteDetailsForDelivery", conn)
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
        /// Gọi sp_GetCustomerForDelivery để lấy thông tin khách hàng.
        /// </summary>
        public DataTable GetCustomerById(int customerId)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetCustomerForDelivery", conn)
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
        // LẤY DANH SÁCH PHIẾU GIAO ĐÃ LƯU (CHO COMBOBOX PICK LẠI)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetSavedDeliveryCodes — phiếu còn xử lý (không gồm Đã giao / Hủy).
        /// </summary>
        public DataTable GetSavedDeliveryCodes()
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetSavedDeliveryCodes", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách phiếu giao: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // LẤY PHIẾU GIAO HÀNG THEO ID (LOAD LẠI KHI PICK COMBOBOX)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDeliveryNoteById — trả về DataSet:
        ///   [0] Thông tin phiếu giao hàng
        ///   [1] Chi tiết sản phẩm đã giao
        /// </summary>
        public DataSet GetDeliveryNoteById(int phieuGiaoId)
        {
            var ds = new DataSet();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDeliveryNoteById", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@PhieuGiaoId", phieuGiaoId);
                    new SqlDataAdapter(cmd).Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải phiếu giao hàng: {ex.Message}");
            }

            return ds;
        }


        // ─────────────────────────────────────────────────────
        // LƯU / CẬP NHẬT PHIẾU GIAO HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_SaveDeliveryNote để INSERT hoặc UPDATE phiếu giao hàng + chi tiết.
        /// phieuGiaoId = 0 → INSERT mới, > 0 → UPDATE.
        /// Trả về ID phiếu vừa tạo/cập nhật.
        /// </summary>
        public int SaveDeliveryNote(
            int phieuGiaoId,
            string maPhieuGiao,
            int baoGiaId,
            int khachHangId,
            string maKH,
            string tenKH,
            string diaChiGiao,
            string nguoiNhan,
            string sdtNguoiNhan,
            DateTime ngayGiao,
            string? gioGiao,
            string? hinhThucGiao,
            string? ghiChuHinhThuc,
            string? tenTaiXe,
            decimal tongTien,
            string? ghiChu,
            string nguoiTao,
            DataTable details)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string detailXml = BuildDetailXml(details);

                    var cmd = new SqlCommand("sp_SaveDeliveryNote", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@PhieuGiaoId", phieuGiaoId);
                    cmd.Parameters.AddWithValue("@MaPhieuGiao", maPhieuGiao);
                    cmd.Parameters.AddWithValue("@BaoGiaId", baoGiaId);
                    cmd.Parameters.AddWithValue("@KhachHangId", khachHangId);
                    cmd.Parameters.AddWithValue("@MaKH", maKH ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenKH", tenKH ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChiGiao", diaChiGiao ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NguoiNhan", nguoiNhan ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDTNguoiNhan", sdtNguoiNhan ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayGiao", ngayGiao.Date);
                    cmd.Parameters.AddWithValue("@GioGiao", gioGiao ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@HinhThucGiao", hinhThucGiao ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChuHinhThuc", ghiChuHinhThuc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenTaiXe", tenTaiXe ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TongTien", tongTien);
                    cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NguoiTao", nguoiTao);
                    cmd.Parameters.AddWithValue("@DetailsXml", detailXml);

                    var outParam = new SqlParameter("@NewPhieuGiaoId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outParam.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu phiếu giao hàng: {ex.Message}", ex);
            }
        }


        // ─────────────────────────────────────────────────────
        // CẬP NHẬT TRẠNG THÁI PHIẾU GIAO HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_UpdateDeliveryStatus để cập nhật trạng thái phiếu giao.
        /// </summary>
        public void UpdateDeliveryStatus(int phieuGiaoId, string trangThai, string nguoiSua)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_UpdateDeliveryStatus", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@PhieuGiaoId", phieuGiaoId);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    cmd.Parameters.AddWithValue("@NguoiSua", nguoiSua);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}", ex);
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH PHIẾU GIAO HÀNG CHO BÁO CÁO (Xuất Excel)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetDeliveryNoteList để lấy toàn bộ phiếu giao (có filter).
        /// </summary>
        public DataTable GetDeliveryNoteList(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? trangThai = null)
        {
            var dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var cmd = new SqlCommand("sp_GetDeliveryNoteList", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@FromDate",
                        fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate",
                        toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai",
                        string.IsNullOrWhiteSpace(trangThai) ? (object)DBNull.Value : trangThai);

                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải danh sách phiếu giao: {ex.Message}");
            }

            return dt;
        }


        // ─────────────────────────────────────────────────────
        // CHUYỂN DATATABLE CHI TIẾT SANG XML
        // [FIX] Escape XML special characters trong TenSanPham
        // ─────────────────────────────────────────────────────

        private static string EscapeXml(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            string str = value.ToString() ?? "";
            return System.Security.SecurityElement.Escape(str);
        }

        private string BuildDetailXml(DataTable details)
        {
            var root = new XElement("Details");

            foreach (DataRow row in details.Rows)
            {
                // [FIX] Escape các giá trị text để tránh XML lỗi
                string tenSP = EscapeXml(row["TenSanPham"]);
                string donVi = EscapeXml(row["DonViTinh"]);

                var item = new XElement("Item",
                    new XElement("DetailId", row["DetailId"] ?? 0),
                    new XElement("TenSanPham", tenSP),
                    new XElement("DonViTinh", donVi),
                    new XElement("SoLuongBaoGia", row["SoLuongBaoGia"] ?? 0),
                    new XElement("SoLuongGiao", row["SoLuongGiao"] ?? 0),
                    new XElement("DonGia", row["DonGia"] ?? 0),
                    new XElement("ThanhTien", row["ThanhTien"] ?? 0)
                );
                root.Add(item);
            }

            return root.ToString();
        }
    }
}
