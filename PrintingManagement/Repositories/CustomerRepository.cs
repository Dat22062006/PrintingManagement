// ═══════════════════════════════════════════════════════════════════
// ║  CustomerRepository.cs - DỮ LIỆU KHÁCH HÀNG (CẬP NHẬT)        ║
// ═══════════════════════════════════════════════════════════════════
// Thay đổi:
//   [FIX] GenerateCustomerCode → format KH_01
//   [NEW] GetAllActiveCustomers() — cho ComboBox frmPriceCalculation
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class CustomerRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY DANH SÁCH KHÁCH HÀNG (CÓ TÌM KIẾM)
        // ─────────────────────────────────────────────────────

        public DataTable GetCustomers(string keyword = "")
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetCustomers", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@Keyword", keyword ?? "");
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
        // [NEW] LẤY TẤT CẢ KH ĐANG HOẠT ĐỘNG (CHO COMBOBOX)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GetAllActiveCustomers.
        /// Trả về: id, Ma_KH, Ten_Khach_Hang, Dia_Chi, MST, Dien_Thoai, Nguoi_Lien_He.
        /// Dùng để nạp vào ComboBox cboCustomer ở frmPriceCalculation.
        /// </summary>
        public DataTable GetAllActiveCustomers()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetAllActiveCustomers", conn)
                    { CommandType = CommandType.StoredProcedure };
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
        // VÔ HIỆU HÓA KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        public void DeactivateCustomer(int customerId, string updatedBy)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_DeactivateCustomer", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi vô hiệu hóa khách hàng: {ex.Message}");
            }
        }


        // ─────────────────────────────────────────────────────
        // [FIX] SINH MÃ KHÁCH HÀNG → FORMAT KH_01
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi sp_GenerateCustomerCode đã sửa.
        /// Trả về mã dạng KH_01, KH_02, ..., KH_99, KH_100.
        /// </summary>
        public string GenerateCustomerCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GenerateCustomerCode", conn)
                    { CommandType = CommandType.StoredProcedure };
                    var outputParam = new SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputParam);
                    cmd.ExecuteNonQuery();
                    return outputParam.Value?.ToString() ?? "KH_01";
                }
            }
            catch
            {
                return "KH_01";
            }
        }


        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT KHÁCH HÀNG THEO ID
        // ─────────────────────────────────────────────────────

        public DataTable GetCustomerById(int customerId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetCustomerById", conn)
                    { CommandType = CommandType.StoredProcedure };
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
        // LƯU KHÁCH HÀNG (THÊM MỚI HOẶC CẬP NHẬT)
        // ─────────────────────────────────────────────────────

        public void SaveCustomer(
            int customerId, string code, string name,
            string address, string taxCode, string phone,
            string email, string contact, string note, string updatedBy)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_SaveCustomer", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@Code", code ?? "");
                    cmd.Parameters.AddWithValue("@Name", name ?? "");
                    cmd.Parameters.AddWithValue("@Address", address ?? "");
                    cmd.Parameters.AddWithValue("@TaxCode", taxCode ?? "");
                    cmd.Parameters.AddWithValue("@Phone", phone ?? "");
                    cmd.Parameters.AddWithValue("@Email", email ?? "");
                    cmd.Parameters.AddWithValue("@Contact", contact ?? "");
                    cmd.Parameters.AddWithValue("@Note", note ?? "");
                    cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu khách hàng: {ex.Message}");
            }
        }
    }
}