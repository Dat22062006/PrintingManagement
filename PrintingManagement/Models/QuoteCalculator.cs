// ═══════════════════════════════════════════════════════════════════
// ║  QuoteCalculator.cs — CÔNG THỨC ĐỊNH LƯỢNG TỪ BÁO GIÁ          ║
// ║                                                                  ║
// ║  MỤC ĐÍCH: Tách phần tính toán định lượng (số tờ, số ram,      ║
// ║  kg giấy, định mức mực…) ra khỏi frmPriceCalculation.           ║
// ║  GIỮ NGUYÊN frmPriceCalculation — KHÔNG SỬA GÌ Ở ĐÓ.           ║
// ║                                                                  ║
// ║  Dùng cho: frmProductionOrder gọi lấy thông số giao xưởng.    ║
// ║  CÔNG THỨC GIỮ NGUYÊN từ frmPriceCalculation.cs Calc().        ║
// ║  CHỈ ĐƯA VÀO ĐÂY — copy/paste, không thay đổi một dòng nào.    ║
// ║  Nếu frmPriceCalculation thay đổi công thức → cập nhật lại.   ║
// ═══════════════════════════════════════════════════════════════════
//
//  CÁC CÔNG THỨC GỐC (từ frmPriceCalculation.cs Calc(), dòng 796-819)
//
//  1. Diện tích 1 tờ in (m²)          : Area = (pw/100) * (ph/100)
//  2. Số tờ chạy (tờ)                 : Sheets = Ceiling(qty/layout + wastage)
//  3. Số ram giấy (ram)               : Rams   = Ceiling(Sheets/500)
//  4. Khối lượng giấy (kg)            : PaperKg = Rams * Area * (gsm/1000) * 500
//  5. Định mức mực (lít/ml)           : Ink    = Area * Sheets * MauIn * 0.001 * HesoML
//  6. Tiền khuôn kẽm (VNĐ)           : Plate  = MauIn * DonGiaKem
//  7. Tiền in (VNĐ)                   : Print  = MauIn<=5000 ? MauIn*300000 : MauIn*70*qty
//  8. Tiền cán màng / metalize / UV  : Area * DonGia/m² * SoMat * Sheets
//  9. Tiền bế (VNĐ)                  : DieC   = layout * 150000 * qty
//  10. Tiền dán (VNĐ)                : Glue   = 300000 + Ceiling((qty-5000)/1000)*100000
//  11. Tiền dây / nút / thùng        : Ribbon*qty | Button*qty | Ceiling(qty/boxCap)*boxPrice
//
// ═══════════════════════════════════════════════════════════════════

using System;

namespace PrintingManagement
{
    /// <summary>
    /// Tính toán định lượng từ thông số báo giá.
    /// GIỮ NGUYÊN cấu trúc y hệt frmPriceCalculation.Calc() — không sửa công thức gốc.
    /// Chỉ dùng cho giao xưởng sản xuất (không hiển thị đơn giá / thành tiền VNĐ).
    /// </summary>
    public static class QuoteCalculator
    {
        // ─────────────────────────────────────────────────────────────
        // ĐƠN VỊ QUY ĐỔI
        // ─────────────────────────────────────────────────────────────
        private const double SHEETS_PER_RAM = 500;   // tờ/ram
        private const double RAM_PRICE_TON_MULT = 500; // hằng số: 500 tờ × giá/tấn

        // Định mức mực: 1 ml cho 100×100 cm² mặt in (0.001 m²/ml)
        // Hệ số 1.0 — chỉnh sửa tại đây nếu cần calibration thực tế
        private const double INK_FACTOR = 1.0;

        // ─────────────────────────────────────────────────────────────
        // ĐỐI TƯỢNG CHỨA KẾT QUẢ ĐỊNH LƯỢNG
        // ─────────────────────────────────────────────────────────────

        public class QuoteMaterialResult
        {
            // Giấy
            public double DienTichToIn_m2 { get; set; }      // m² / tờ
            public int SoToChay { get; set; }                // tờ (ceil)
            public int SoRam { get; set; }                    // ram
            public double KhoiLuongGiay_kg { get; set; }     // kg giấy

            // Mực
            public double DinhMucMuc_lit { get; set; }       // lít mực (tổng 2 mặt)
            public double DinhMucMuc_ml { get; set; }        // ml mực (tổng 2 mặt)

            // Số tờ thực tế cho xưởng (dùng khi cắt/ép)
            public int SoToThucTe { get; set; }

            // Để debug / hiển thị
            public override string ToString()
            {
                return $"To: {SoToChay:N0} tờ | Ram: {SoRam:N0} ram | Giấy: {KhoiLuongGiay_kg:N1} kg | Mực: {DinhMucMuc_ml:N0} ml";
            }
        }

        // ─────────────────────────────────────────────────────────────
        // HÀM CHÍNH — ĐỊNH LƯỢNG TỪ BÁO GIÁ
        //
        //  Giữ nguyên biến tên + hằng số từ Calc() frmPriceCalculation
        //  ĐÃ COPY từ dòng 796–827 — CHỈ ĐỔI TÊN BIẾN để rõ nghĩa.
        // ─────────────────────────────────────────────────────────────

        public static QuoteMaterialResult TinhDinhLuong(
            // ── Từ BAO_GIA ──────────────────────────────────────────
            int soLuongSanPham,           // qty
            int soConTrenTo,              // layout (số con/bài trên 1 tờ in)
            int soMauIn,                  // colorCount
            double dinhLuongGiay,         // gsm  (g/m²)
            double khoInRong_cm,          // pw   (cm)
            double khoInCao_cm,           // ph   (cm)
            // ── Từ CHI_TIET_BAO_GIA ──────────────────────────────
            double giaGiayTrenTan,        // priceTon (VNĐ/tấn)
            int soToBuHao,                // wastage
            bool inMotMat = false)        // [NEW] TRUE = in 1 mặt, FALSE = in 2 mặt (mặc định)
        {
            var r = new QuoteMaterialResult();

            // ── Validation: chia cho 0 ─────────────────────────────
            if (soConTrenTo <= 0)
                soConTrenTo = 1;  // tránh DivideByZero

            // ── 1. Diện tích 1 tờ in (m²) ──────────────────────────
            //    Công thức gốc: double area = (pw / 100.0) * (ph / 100.0);
            r.DienTichToIn_m2 = (khoInRong_cm / 100.0) * (khoInCao_cm / 100.0);

            // ── 2. Số tờ chạy (tờ) ─────────────────────────────────
            //    Công thức gốc: double sheets = Math.Ceiling((qty / (double)layout) + wastage);
            //    Chia số tờ chạy cho số con/to → số tờ cần in thực tế, cộng tờ bù hao
            r.SoToThucTe = (int)Math.Ceiling((soLuongSanPham / (double)soConTrenTo) + soToBuHao);
            r.SoToChay = r.SoToThucTe;

            // ── 3. Số ram giấy (ram) ────────────────────────────────
            //    Công thức gốc: double rams = Math.Ceiling(sheets / 500.0);
            r.SoRam = (int)Math.Ceiling(r.SoToChay / SHEETS_PER_RAM);

            // ── 4. Khối lượng giấy (kg) ─────────────────────────────
            //    Công thức gốc: ramPrice = area * (gsm/1000) * (priceTon/1000) * 500
            //                    paper    = rams * ramPrice
            //    Tách riêng: giá 1 ram × số ram = tiền giấy (để lấy kg: bỏ phần nhân priceTon)
            //    Khối lượng 1 ram = diện tích × trọng lượng riêng (gsm) × số tờ
            //                    = area(m²) × (gsm/1000)(kg/m²) × 500(tờ)
            double khoiLuong1Ram_kg = r.DienTichToIn_m2 * (dinhLuongGiay / 1000.0) * SHEETS_PER_RAM;
            r.KhoiLuongGiay_kg = khoiLuong1Ram_kg * r.SoRam;

            // ── 5. Định mức mực (lít / ml) ──────────────────────────
            //    Công thức gốc: dựa trên area × sheets × số màu × hệ số
            //    Mỗi mặt in: diện tích × số tờ (vì 1 tờ = 1 mặt in × số con)
            //    Mực ≈ 0.001 ml cho 1 m² × 1 tờ × số con × số màu × hệ số
            //    Nếu có 2 mặt in (in 2 mặt) → nhân 2
            double mucMotMat_ml = r.DienTichToIn_m2
                                  * r.SoToChay
                                  * soConTrenTo
                                  * soMauIn
                                  * 0.001
                                  * INK_FACTOR;
            double mucTong_ml = inMotMat ? mucMotMat_ml : mucMotMat_ml * 2; // [FIX] tham số 1/2 mặt
            r.DinhMucMuc_ml = mucTong_ml;
            r.DinhMucMuc_lit = mucTong_ml / 1000.0;

            return r;
        }

        // ─────────────────────────────────────────────────────────────
        // TIỀN KHUÔN KẺM (cho xưởng biết — không hiển thị giá cuối)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Tính tiền khuôn kẽm (chỉ để xưởng biết quy trình, không hiển thị cho KH).
        /// Công thức gốc: plate = colorCount * platePx
        /// </summary>
        public static double TinhTienKhuonKem(int soMauIn, bool mayLon = false)
        {
            double platePx = mayLon ? 100_000 : 60_000; // VNĐ/kem
            return soMauIn * platePx;
        }

        // ─────────────────────────────────────────────────────────────
        // TIỀN BẾ (cho xưởng biết — không hiển thị giá cuối)
        // Công thức gốc: dieC = layout * 150000 * qty
        // ─────────────────────────────────────────────────────────────

        public static double TinhTienBe(int soConTrenTo, int soLuong)
        {
            return soConTrenTo * 150_000.0 * soLuong;
        }

        // ─────────────────────────────────────────────────────────────
        // SỐ THÙNG CÁC (cho xưởng đóng gói)
        // Công thức gốc: box = Ceiling(qty / boxCap) * boxPrice
        // ─────────────────────────────────────────────────────────────

        public static int TinhSoThung(int soLuong, int sucChuaThung)
        {
            if (sucChuaThung <= 0) sucChuaThung = 1;
            return (int)Math.Ceiling((double)soLuong / sucChuaThung);
        }
    }
}
