// ═══════════════════════════════════════════════════════════════════
// ║  DebtReportRepository.cs - DỮ LIỆU BÁO CÁO CÔNG NỢ            ║
// [FIX] Bỏ toàn bộ SQL inline → gọi SP dbo.sp_GetOpenReceivableInvoiceLines
// [FIX] Thêm @Keyword vào SP call — tìm kiếm server-side (đúng dấu tiếng Việt)
// [FIX] GetOpenReceivableInvoiceLines trả thêm cột SoHoaDon
// Giữ nguyên: GetDebtSummary, GetReceivables, GetPayables, GetOrderDebtByCustomer
//             BuildReceivableSummaryFromLines, ReceivableDebtColumns, ReceivableTextSearch
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public class DebtReportRepository
    {
        // ─────────────────────────────────────────────────────
        // KPI TỔNG CÔNG NỢ
        // ─────────────────────────────────────────────────────

        public DataSet GetDebtSummary()
        {
            var ds = new DataSet();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("sp_GetDebtSummary", conn)
                { CommandType = CommandType.StoredProcedure };
                new SqlDataAdapter(cmd).Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải tổng hợp công nợ: {ex.Message}");
            }
            return ds;
        }

        // ─────────────────────────────────────────────────────
        // CÔNG NỢ PHẢI THU (lưới trên)
        // ─────────────────────────────────────────────────────

        public DataTable GetReceivables()
        {
            var dt = new DataTable();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("sp_GetReceivables", conn)
                { CommandType = CommandType.StoredProcedure };
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải công nợ phải thu: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // CÔNG NỢ PHẢI TRẢ
        // ─────────────────────────────────────────────────────

        public DataTable GetPayables()
        {
            var dt = new DataTable();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                    var cmd = new SqlCommand("sp_GetPayables", conn)
                { CommandType = CommandType.StoredProcedure };
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải công nợ phải trả: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // CHI TIẾT NỢ THEO ĐƠN (dùng cho frmPaymentReceive)
        // ─────────────────────────────────────────────────────

        public DataTable GetOrderDebtByCustomer(int customerId)
        {
            var dt = new DataTable();
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                var cmd = new SqlCommand("sp_GetOrderDebtByCustomer", conn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải chi tiết công nợ khách: {ex.Message}");
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // [FIX] CHI TIẾT HÓA ĐƠN CÒN NỢ — GỌI SP (không còn SQL inline)
        // keyword    : tìm tên KH / số CT / số HĐ / tên hàng (để trống = tất cả)
        // filterYear/Month: lọc tháng theo ngày bán (null = mọi thời gian)
        // customerId : chỉ 1 KH (null = tất cả, dùng khi click lưới trên)
        // ─────────────────────────────────────────────────────

        /// <summary>Lọc bảng chi tiết (tên KH, mã CT, số HĐ, tên hàng) + optional CustomerId — dùng chung form + fallback SP cũ.</summary>
        public static DataTable FilterReceivableDetailTable(DataTable source, string keyword, int? customerId)
        {
            if (source == null) return new DataTable();
            keyword = keyword?.Trim() ?? "";
            if (source.Rows.Count == 0)
                return source;

            if (string.IsNullOrEmpty(keyword) && !customerId.HasValue)
                return source;

            var dt = source.Clone();
            foreach (DataRow r in source.Rows)
            {
                if (customerId.HasValue)
                {
                    if (!r.Table.Columns.Contains("CustomerId") || r["CustomerId"] == DBNull.Value)
                        continue;
                    if (Convert.ToInt32(r["CustomerId"], CultureInfo.InvariantCulture) != customerId.Value)
                        continue;
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    var sb = new StringBuilder(128);
                    void App(string col)
                    {
                        if (!r.Table.Columns.Contains(col) || r[col] == DBNull.Value) return;
                        var s = r[col]?.ToString();
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (sb.Length > 0) sb.Append(' ');
                            sb.Append(s);
                        }
                    }
                    App("Ten_Khach_Hang");
                    App("OrderCode");
                    App("SoHoaDon");
                    App("ProductName");

                    if (!ReceivableTextSearch.Matches(sb.ToString(), keyword))
                        continue;
                }

                dt.ImportRow(r);
            }
            return dt;
        }

        private static DataTable FillOpenReceivableFromNewSp(
            string keyword,
            int? filterYear,
            int? filterMonth,
            int? customerId)
        {
            var dt = new DataTable();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("dbo.sp_GetOpenReceivableInvoiceLines", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Keyword",
                string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim());
            cmd.Parameters.Add("@FilterYear", SqlDbType.Int).Value =
                filterYear.HasValue ? filterYear.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@FilterMonth", SqlDbType.Int).Value =
                filterMonth.HasValue ? filterMonth.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value =
                customerId.HasValue ? customerId.Value : (object)DBNull.Value;

            new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>SP cũ trên DB chỉ có @FilterYear / @FilterMonth (không có Keyword, CustomerId).</summary>
        private static DataTable FillOpenReceivableFromLegacySp(int? filterYear, int? filterMonth)
        {
            var dt = new DataTable();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            var cmd = new SqlCommand("dbo.sp_GetOpenReceivableInvoiceLines", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@FilterYear", SqlDbType.Int).Value =
                filterYear.HasValue ? filterYear.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@FilterMonth", SqlDbType.Int).Value =
                filterMonth.HasValue ? filterMonth.Value : (object)DBNull.Value;
            new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public DataTable GetOpenReceivableInvoiceLines(
            string keyword = "",
            int? filterYear = null,
            int? filterMonth = null,
            int? customerId = null)
        {
            try
            {
                return FillOpenReceivableFromNewSp(keyword, filterYear, filterMonth, customerId);
            }
            catch (SqlException ex) when (
                ex.Number == 8144
                || (ex.Message != null && ex.Message.Contains("too many arguments", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var dt = FillOpenReceivableFromLegacySp(filterYear, filterMonth);
                    return FilterReceivableDetailTable(dt, keyword ?? "", customerId);
                }
                catch (Exception inner)
                {
                    throw new Exception(
                        $"Lỗi khi tải chi tiết công nợ (fallback SP cũ): {inner.Message}. Gốc: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải chi tiết công nợ: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────
        // TỔng HỢP KH TỪ CÁC DÒNG ĐÃ LỌC (dùng khi tick lọc tháng)
        // ─────────────────────────────────────────────────────

        public DataTable BuildReceivableSummaryFromLines(DataTable lines)
        {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("Ten_Khach_Hang", typeof(string));
            dt.Columns.Add("Tong_No", typeof(decimal));
            dt.Columns.Add("Da_Thu", typeof(decimal));
            dt.Columns.Add("Con_Lai", typeof(decimal));
            dt.Columns.Add("DueDate", typeof(DateTime));

            if (lines.Rows.Count == 0) return dt;

            var groups = new System.Collections.Generic.Dictionary<int,
                (string Name, decimal Tot, decimal Col, decimal Rem, DateTime? MinDue)>();

            foreach (DataRow r in lines.Rows)
            {
                // [FIX] Kiểm tra tồn tại cột trước khi truy cập
                if (!r.Table.Columns.Contains("CustomerId"))
                    continue;

                int cid = Convert.ToInt32(r["CustomerId"], CultureInfo.InvariantCulture);
                string name = r.Table.Columns.Contains("Ten_Khach_Hang") ? r["Ten_Khach_Hang"]?.ToString() ?? "" : "";
                decimal tot = r.Table.Columns.Contains("TotalAmount")
                    ? Convert.ToDecimal(r["TotalAmount"], CultureInfo.InvariantCulture) : 0;
                decimal col = r.Table.Columns.Contains("Collected")
                    ? Convert.ToDecimal(r["Collected"], CultureInfo.InvariantCulture) : 0;
                decimal rem = r.Table.Columns.Contains("Remaining")
                    ? Convert.ToDecimal(r["Remaining"], CultureInfo.InvariantCulture) : 0;
                DateTime? due = null;
                if (r.Table.Columns.Contains("DueDate") && r["DueDate"] != DBNull.Value)
                    due = Convert.ToDateTime(r["DueDate"], CultureInfo.InvariantCulture).Date;

                if (!groups.TryGetValue(cid, out var acc)) acc = (name, 0, 0, 0, null);
                DateTime? newMin = acc.MinDue;
                if (due.HasValue && (!newMin.HasValue || due.Value < newMin.Value)) newMin = due;
                groups[cid] = (string.IsNullOrEmpty(acc.Name) ? name : acc.Name,
                               acc.Tot + tot, acc.Col + col, acc.Rem + rem, newMin);
            }

            foreach (var kv in groups)
            {
                var v = kv.Value;
                dt.Rows.Add(kv.Key, v.Name, v.Tot, v.Col, v.Rem,
                    v.MinDue.HasValue ? (object)v.MinDue.Value : DBNull.Value);
            }
            return dt;
        }
    }

    // ─────────────────────────────────────────────────────
    // HELPER: TÌM KIẾM TIẾNG VIỆT (client-side fallback nếu cần)
    // ─────────────────────────────────────────────────────

    internal static class ReceivableTextSearch
    {
        private static readonly CompareInfo ViCompare =
            CultureInfo.GetCultureInfo("vi-VN").CompareInfo;

        private static string RemoveDiacritics(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var norm = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(norm.Length);
            foreach (char c in norm)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>Đ/đ không tách dấu kiểu Latin — phải gộp về D/d trước khi bỏ dấu, không thì &quot;đạt&quot; không khớp &quot;ĐẠT&quot;.</summary>
        private static string FoldVietnameseForSearch(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new StringBuilder(s.Length);
            foreach (var c in s.Normalize(NormalizationForm.FormC))
            {
                if (c == 'Đ') sb.Append('D');
                else if (c == 'đ') sb.Append('d');
                else sb.Append(c);
            }
            return RemoveDiacritics(sb.ToString());
        }

        internal static bool Matches(string haystack, string needle)
        {
            if (string.IsNullOrEmpty(needle)) return true;
            if (string.IsNullOrEmpty(haystack)) return false;
            if (ViCompare.IndexOf(haystack, needle, CompareOptions.IgnoreCase) >= 0) return true;
            if (haystack.Contains(needle, StringComparison.OrdinalIgnoreCase)) return true;

            var ha = FoldVietnameseForSearch(haystack);
            var ne = FoldVietnameseForSearch(needle);
            if (ha.IndexOf(ne, StringComparison.OrdinalIgnoreCase) >= 0) return true;

            return RemoveDiacritics(haystack)
                .IndexOf(RemoveDiacritics(needle), StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    // ─────────────────────────────────────────────────────
    // HELPER: MAP CỘT (tên cột DB không thống nhất giữa các SP)
    // ─────────────────────────────────────────────────────

    internal static class ReceivableDebtColumns
    {
        internal static bool TryFind(DataTable table, out DataColumn? column, params string[] candidates)
        {
            column = null;
            foreach (var want in candidates)
                foreach (DataColumn c in table.Columns)
                    if (string.Equals(c.ColumnName, want, StringComparison.OrdinalIgnoreCase))
                    { column = c; return true; }
            return false;
        }

        internal static string GetString(DataRow row, params string[] candidates)
        {
            if (!TryFind(row.Table, out var col, candidates)) return "";
            return row[col!] == DBNull.Value ? "" : row[col!]?.ToString()?.Trim() ?? "";
        }

        internal static bool TryGetInt32(DataRow row, out int value, params string[] candidates)
        {
            value = 0;
            if (!TryFind(row.Table, out var col, candidates) || row[col!] == DBNull.Value) return false;
            value = Convert.ToInt32(row[col!], CultureInfo.InvariantCulture);
            return true;
        }

        internal static double GetDouble(DataRow row, params string[] candidates)
        {
            if (!TryFind(row.Table, out var col, candidates) || row[col!] == DBNull.Value) return 0d;
            return Convert.ToDouble(row[col!], CultureInfo.InvariantCulture);
        }

        internal static bool TryGetDate(DataRow row, out DateTime? date, params string[] candidates)
        {
            date = null;
            if (!TryFind(row.Table, out var col, candidates) || row[col!] == DBNull.Value) return false;
            date = Convert.ToDateTime(row[col!], CultureInfo.InvariantCulture).Date;
            return true;
        }
    }
}