// ═══════════════════════════════════════════════════════════════════
// ║  PriceCalculationRepository.cs                                  ║
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class PriceCalculationRepository
    {
        // ─────────────────────────────────────────────────────
        // LẤY CHI TIẾT BÁO GIÁ THEO ID
        // ─────────────────────────────────────────────────────

        public DataTable GetQuoteById(int quoteId)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GetQuoteById", conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    new SqlDataAdapter(cmd).Fill(dt);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi tải báo giá: {ex.Message}"); }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // SINH MÃ BÁO GIÁ
        // ─────────────────────────────────────────────────────

        public string GenerateQuoteCode()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_GenerateQuoteCode", conn)
                    { CommandType = CommandType.StoredProcedure };
                    var p = new SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(p);
                    cmd.ExecuteNonQuery();
                    return p.Value?.ToString() ?? "BG-0001";
                }
            }
            catch { return "BG-0001"; }
        }

        // ─────────────────────────────────────────────────────
        // LƯU BÁO GIÁ MỚI (kèm CHI_TIET đầu tiên)
        // ─────────────────────────────────────────────────────

        public int SaveNewQuote(
            int customerId,
            string customerName, string customerAddress, string createdBy,
            string quoteCode, string productName, DateTime quoteDate,
            int layoutCount, decimal paperPricePerTon, decimal profitPercent,
            string productSize, string paperGsm, string printSize,
            int colorCount, string deliveryTime, int validityDays, string paperType,
            int quantity,
            decimal costPaper, decimal costPlate, decimal costPrint,
            decimal costLaminate, decimal costMetalize, decimal costUV,
            decimal costDie, decimal costDieMold, decimal costGlue,
            decimal costRibbon, decimal costButton, decimal costBox,
            decimal costDelivery, decimal costProof, decimal totalCost,
            decimal costPerUnit, decimal pricePerUnit, decimal totalQuotePrice)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_SaveNewQuote", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@CustomerName", customerName);
                    cmd.Parameters.AddWithValue("@CustomerAddress", customerAddress);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@QuoteCode", quoteCode);
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@QuoteDate", quoteDate);
                    cmd.Parameters.AddWithValue("@LayoutCount", layoutCount);
                    cmd.Parameters.AddWithValue("@PaperPricePerTon", paperPricePerTon);
                    cmd.Parameters.AddWithValue("@ProfitPercent", profitPercent);
                    cmd.Parameters.AddWithValue("@ProductSize", productSize);
                    cmd.Parameters.AddWithValue("@PaperGsm", paperGsm);
                    cmd.Parameters.AddWithValue("@PrintSize", printSize);
                    cmd.Parameters.AddWithValue("@ColorCount", colorCount);
                    cmd.Parameters.AddWithValue("@DeliveryTime", deliveryTime);
                    cmd.Parameters.AddWithValue("@ValidityDays", validityDays);
                    cmd.Parameters.AddWithValue("@PaperType", paperType);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@CostPaper", costPaper);
                    cmd.Parameters.AddWithValue("@CostPlate", costPlate);
                    cmd.Parameters.AddWithValue("@CostPrint", costPrint);
                    cmd.Parameters.AddWithValue("@CostLaminate", costLaminate);
                    cmd.Parameters.AddWithValue("@CostMetalize", costMetalize);
                    cmd.Parameters.AddWithValue("@CostUV", costUV);
                    cmd.Parameters.AddWithValue("@CostDie", costDie);
                    cmd.Parameters.AddWithValue("@CostDieMold", costDieMold);
                    cmd.Parameters.AddWithValue("@CostGlue", costGlue);
                    cmd.Parameters.AddWithValue("@CostRibbon", costRibbon);
                    cmd.Parameters.AddWithValue("@CostButton", costButton);
                    cmd.Parameters.AddWithValue("@CostBox", costBox);
                    cmd.Parameters.AddWithValue("@CostDelivery", costDelivery);
                    cmd.Parameters.AddWithValue("@CostProof", costProof);
                    cmd.Parameters.AddWithValue("@TotalCost", totalCost);
                    cmd.Parameters.AddWithValue("@CostPerUnit", costPerUnit);
                    cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                    cmd.Parameters.AddWithValue("@TotalQuotePrice", totalQuotePrice);

                    var outId = new SqlParameter("@NewQuoteId", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outId);
                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outId.Value);
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi lưu báo giá: {ex.Message}"); }
        }

        // ─────────────────────────────────────────────────────
        // [NEW] INSERT THÊM MỨC SL (không ghi đè)
        // ─────────────────────────────────────────────────────

        public void InsertQuoteDetail(
            int quoteId, int validityDays, string paperType, int quantity,
            decimal costPaper, decimal costPlate, decimal costPrint,
            decimal costLaminate, decimal costMetalize, decimal costUV,
            decimal costDie, decimal costDieMold, decimal costGlue,
            decimal costRibbon, decimal costButton, decimal costBox,
            decimal costDelivery, decimal costProof, decimal totalCost,
            decimal costPerUnit, decimal pricePerUnit, decimal totalQuotePrice)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_InsertQuoteDetail", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    cmd.Parameters.AddWithValue("@ValidityDays", validityDays);
                    cmd.Parameters.AddWithValue("@PaperType", paperType);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@CostPaper", costPaper);
                    cmd.Parameters.AddWithValue("@CostPlate", costPlate);
                    cmd.Parameters.AddWithValue("@CostPrint", costPrint);
                    cmd.Parameters.AddWithValue("@CostLaminate", costLaminate);
                    cmd.Parameters.AddWithValue("@CostMetalize", costMetalize);
                    cmd.Parameters.AddWithValue("@CostUV", costUV);
                    cmd.Parameters.AddWithValue("@CostDie", costDie);
                    cmd.Parameters.AddWithValue("@CostDieMold", costDieMold);
                    cmd.Parameters.AddWithValue("@CostGlue", costGlue);
                    cmd.Parameters.AddWithValue("@CostRibbon", costRibbon);
                    cmd.Parameters.AddWithValue("@CostButton", costButton);
                    cmd.Parameters.AddWithValue("@CostBox", costBox);
                    cmd.Parameters.AddWithValue("@CostDelivery", costDelivery);
                    cmd.Parameters.AddWithValue("@CostProof", costProof);
                    cmd.Parameters.AddWithValue("@TotalCost", totalCost);
                    cmd.Parameters.AddWithValue("@CostPerUnit", costPerUnit);
                    cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                    cmd.Parameters.AddWithValue("@TotalQuotePrice", totalQuotePrice);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi thêm mức SL: {ex.Message}"); }
        }

        // ─────────────────────────────────────────────────────
        // CẬP NHẬT CHI TIẾT BÁO GIÁ (chế độ sửa 1 dòng)
        // ─────────────────────────────────────────────────────

        public void UpdateQuoteDetail(
            int quoteId, int validityDays, string paperType, int quantity,
            decimal costPaper, decimal costPlate, decimal costPrint,
            decimal costLaminate, decimal costMetalize, decimal costUV,
            decimal costDie, decimal costDieMold, decimal costGlue,
            decimal costRibbon, decimal costButton, decimal costBox,
            decimal costDelivery, decimal costProof, decimal totalCost,
            decimal costPerUnit, decimal pricePerUnit, decimal totalQuotePrice)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("sp_UpdateQuoteDetail", conn)
                    { CommandType = CommandType.StoredProcedure };

                    cmd.Parameters.AddWithValue("@QuoteId", quoteId);
                    cmd.Parameters.AddWithValue("@ValidityDays", validityDays);
                    cmd.Parameters.AddWithValue("@PaperType", paperType);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@CostPaper", costPaper);
                    cmd.Parameters.AddWithValue("@CostPlate", costPlate);
                    cmd.Parameters.AddWithValue("@CostPrint", costPrint);
                    cmd.Parameters.AddWithValue("@CostLaminate", costLaminate);
                    cmd.Parameters.AddWithValue("@CostMetalize", costMetalize);
                    cmd.Parameters.AddWithValue("@CostUV", costUV);
                    cmd.Parameters.AddWithValue("@CostDie", costDie);
                    cmd.Parameters.AddWithValue("@CostDieMold", costDieMold);
                    cmd.Parameters.AddWithValue("@CostGlue", costGlue);
                    cmd.Parameters.AddWithValue("@CostRibbon", costRibbon);
                    cmd.Parameters.AddWithValue("@CostButton", costButton);
                    cmd.Parameters.AddWithValue("@CostBox", costBox);
                    cmd.Parameters.AddWithValue("@CostDelivery", costDelivery);
                    cmd.Parameters.AddWithValue("@CostProof", costProof);
                    cmd.Parameters.AddWithValue("@TotalCost", totalCost);
                    cmd.Parameters.AddWithValue("@CostPerUnit", costPerUnit);
                    cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                    cmd.Parameters.AddWithValue("@TotalQuotePrice", totalQuotePrice);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception($"Lỗi khi cập nhật báo giá: {ex.Message}"); }
        }
    }
}