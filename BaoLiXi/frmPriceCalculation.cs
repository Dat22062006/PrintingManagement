using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace PrintingManagement
{
    public partial class frmPriceCalculation : Form
    {
        DatabaseHelper db = new DatabaseHelper();
        int idBaoGia = 0;
        public frmPriceCalculation()
        {
            InitializeComponent();
        }

        public frmPriceCalculation(int id)
        {
            InitializeComponent();
            idBaoGia = id;
        }

        private void frmPriceCalculation_Load(object sender, EventArgs e)
        {
            if (idBaoGia > 0)
            {
                LoadBaoGia(); // sửa -> giữ dữ liệu
            }
            else
            {
                txtTenKhachHangg.Text = "";
                txtTenSanPhamm.Text = "";
                txtDiaChii.Text = "";
                txtKichThuocSanPham.Text = "";
                txtSoLuong.Text = "";
                txtSoCon.Text = "";

                dtpNgayBaoGiaa.Value = DateTime.Now;

                txtHieuLucBaoGia.Text = "30";
                txtThoiGianGiaoHangg.Text = "7-10 ngày làm việc";
                txtBuHaoTo.Text = "500";
                txtTyLeLoiNhuanPhanTram.Text = "20";
            }
            // === Tự ghi mẫu mặc định (chỉ khi tạo mới, không ghi đè khi sửa) ===
            dtpNgayBaoGia.Value = DateTime.Now;
            if (idBaoGia == 0)  // ⭐ FIX: chỉ set default khi idBaoGia == 0
            {
                txtHieuLucBaoGia.Text = "30";
                txtThoiGianGiaoHang.Text = "7-10 ngày làm việc";
                txtBuHaoTo.Text = "500";
                txtTyLeLoiNhuanPhanTram.Text = "20";
                txtSoMatCanMangg.Text = "1";
                txtMetallizeDongM2.Text = "5000";
                txtSoMatMetallize.Text = "1";
                txtUVMoDongM2.Text = "5500";
                txtSoMatUVMo.Text = "1";
                txtGiaDayDongCai.Text = "550";
                txtGiaNutDongCai.Text = "360";
                txtGiaThungDongThung.Text = "20000";
                txtQuyCachDongCaiThung.Text = "500";
                txtTienInProofDong.Text = "100000";
                txtTienXeGiaoDong.Text = "500000";
            }




            // === ComboBox Loại giấy ===
            cmbLoaiGiay.Items.AddRange(new string[]
            {
        "Couche 250 gsm",
        "Ivory 400 gsm",
        "Duplex 300 gsm",
        "Bristol 230 gsm"
            });
            cmbLoaiGiay.SelectedIndex = 0;

            // === ComboBox Cán màng ===
            cmbCanMang.Items.AddRange(new string[]
            {
        "Không cán",
        "Cán màng bóng (1,900đ/m²)",
        "Cán màng mờ (2,000đ/m²)",
        "Tự chỉnh - cán màng bóng",
        "Tự chỉnh - cán màng mờ"
            });
            cmbCanMang.SelectedIndex = 0;
            txtGiaCanMangDongM2.Text = "0";
            txtGiaCanMangDongM2.ReadOnly = true;

            // === ComboBox Loại máy in ===
            cmbLoaiMayIn.Items.AddRange(new string[]
            {
        "Máy lớn (100.000đ/kém)",
        "Máy nhỏ (60.000đ/kém)",
        "Tự chỉnh-Máy Lớn",
        "Tự chỉnh-Máy Nhỏ"
            });
            cmbLoaiMayIn.SelectedIndex = 0; // Mặc định Máy lớn

            // FIX: Gọi hàm ngay để giá kẽm nhảy luôn khi load
            CapNhatGiaKem();
        }


        //hàmloadbaogia
        // ===== SỬA HÀM LOADBAOGIA - frmPriceCalculation.cs =====

        void LoadBaoGia()
        {
            try
            {
                string sql = @"
SELECT 
    bg.id,
    bg.Ma_Bao_Gia,
    bg.Ten_San_Pham,
    bg.Ngay_Bao_Gia,
    bg.So_Con,
    bg.Gia_Giay_Tan,
    bg.Loi_Nhuan_Phan_Tram,
    bg.Kich_Thuoc_Thanh_Pham,
    bg.Khoi_Luong_Giay,
    bg.Kho_In,
    bg.So_Mau_In,
    bg.Thoi_Gian_Giao_Hang_Du_Kien,
    bg.Ten_Loai_Giay,
    bg.Nguoi_Tao,
    bg.Trang_Thai,
    kh.Ten_Khach_Hang,
    kh.Dia_Chi,
    ct.So_Luong,
    ct.Tien_Khuon_Be,
    ct.Tien_Can_Mang
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
WHERE bg.id = @id";

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", idBaoGia);

                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        // ===== THÔNG TIN KHÁCH HÀNG =====
                        txtTenKhachHangg.Text = rd["Ten_Khach_Hang"]?.ToString() ?? "";
                        txtDiaChii.Text = rd["Dia_Chi"]?.ToString() ?? "";

                        // ===== THÔNG TIN SẢN PHẨM =====
                        txtTenSanPhamm.Text = rd["Ten_San_Pham"]?.ToString() ?? "";
                        txtKichThuocSanPham.Text = rd["Kich_Thuoc_Thanh_Pham"]?.ToString() ?? "";

                        // ===== THÔNG TIN GIẤY =====
                        txtDinhLuongGiay.Text = rd["Khoi_Luong_Giay"]?.ToString() ?? "";
                        txtSoCon.Text = rd["So_Con"]?.ToString() ?? "1";
                        txtGiaGiayTan.Text = rd["Gia_Giay_Tan"]?.ToString() ?? "";

                        // ⭐ Load tên loại giấy vào ComboBox
                        string tenLoaiGiay = rd["Ten_Loai_Giay"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(tenLoaiGiay))
                        {
                            int idxGiay = cmbLoaiGiay.Items.IndexOf(tenLoaiGiay);
                            if (idxGiay >= 0)
                                cmbLoaiGiay.SelectedIndex = idxGiay;
                        }

                        // ===== KHỔ IN =====
                        string khoIn = rd["Kho_In"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(khoIn) && khoIn.Contains(" x "))
                        {
                            string[] parts = khoIn.Split('x');
                            if (parts.Length == 2)
                            {
                                txtKhoInDai.Text = parts[0].Trim();
                                txtKhoInRong.Text = parts[1].Trim();
                            }
                        }

                        // ===== LỢI NHUẬN =====
                        txtTyLeLoiNhuanPhanTram.Text = rd["Loi_Nhuan_Phan_Tram"]?.ToString() ?? "20";

                        // ===== SỐ MÀU IN =====
                        txtSoMauIn.Text = rd["So_Mau_In"]?.ToString() ?? "";

                        // ===== THỜI GIAN GIAO HÀNG =====
                        txtThoiGianGiaoHangg.Text = rd["Thoi_Gian_Giao_Hang_Du_Kien"]?.ToString() ?? "";

                        // ===== NGÀY BÁO GIÁ =====
                        if (rd["Ngay_Bao_Gia"] != DBNull.Value)
                        {
                            dtpNgayBaoGiaa.Value = Convert.ToDateTime(rd["Ngay_Bao_Gia"]);
                        }

                        // ===== CHI TIẾT BÁO GIÁ =====
                        if (rd["So_Luong"] != DBNull.Value)
                        {
                            txtSoLuong.Text = rd["So_Luong"].ToString();
                        }

                        if (rd["Tien_Khuon_Be"] != DBNull.Value)
                        {
                            txtTienKhuonBe.Text = rd["Tien_Khuon_Be"].ToString();
                        }

                        if (rd["Tien_Can_Mang"] != DBNull.Value)
                        {
                            txtGiaCanMangDongM2.Text = rd["Tien_Can_Mang"].ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"⚠️ Không tìm thấy báo giá ID: {idBaoGia}", "Cảnh báo");
                        this.Close();
                    }

                    rd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Lỗi load báo giá:\n\n{ex.Message}\n\nStack:\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        // ====================== SỰ KIỆN COMBOBOX LOẠI GIẤY ======================
        private void cmbLoaiGiay_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbLoaiGiay.SelectedItem?.ToString() ?? "";

            if (selected == "Couche 250 gsm")
            {
                txtDinhLuongGiay.Text = "250";
                txtKhoInDai.Text = "79";
                txtKhoInRong.Text = "51";
                txtGiaGiayTan.Text = "26000000";   // mẫu Couches
                txtBuHaoTo.Text = "500";
            }
            else if (selected == "Ivory 400 gsm")
            {
                txtDinhLuongGiay.Text = "400";
                txtKhoInDai.Text = "69.5";
                txtKhoInRong.Text = "42";
                txtGiaGiayTan.Text = "22000000";
                txtBuHaoTo.Text = "500";
            }
            else if (selected == "Duplex 300 gsm")
            {
                txtDinhLuongGiay.Text = "300";
                txtKhoInDai.Text = "79";
                txtKhoInRong.Text = "51";
                txtGiaGiayTan.Text = "24000000";
                txtBuHaoTo.Text = "500";
            }
            else if (selected == "Bristol 230 gsm")
            {
                txtDinhLuongGiay.Text = "230";
                txtKhoInDai.Text = "79";
                txtKhoInRong.Text = "51";
                txtGiaGiayTan.Text = "20000000";
                txtBuHaoTo.Text = "500";
            }
        }

        // ====================== SỰ KIỆN COMBOBOX CÁN MÀNG ======================
        private void cmbCanMang_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbCanMang.SelectedItem?.ToString() ?? "";

            if (selected == "Cán màng bóng (1,900đ/m²)")
            {
                txtGiaCanMangDongM2.Text = "1900";
                txtGiaCanMangDongM2.ReadOnly = true;
            }
            else if (selected == "Cán màng mờ (2,000đ/m²)")
            {
                txtGiaCanMangDongM2.Text = "2000";
                txtGiaCanMangDongM2.ReadOnly = true;
            }
            else if (selected.Contains("Tự chỉnh"))
            {
                txtGiaCanMangDongM2.Text = "";
                txtGiaCanMangDongM2.ReadOnly = false;
            }
            else
            {
                txtGiaCanMangDongM2.Text = "0";
                txtGiaCanMangDongM2.ReadOnly = true;
            }
        }
        //sukienloaimayin
        //hàm máy in
        private void CapNhatGiaKem()
        {
            string selected = cmbLoaiMayIn.SelectedItem?.ToString() ?? "";
            if (selected == "Máy lớn (100.000đ/kém)")
            {
                txtGiaInTinhThem.Text = "100000";
                txtGiaInTinhThem.ReadOnly = true;
            }
            else if (selected == "Máy nhỏ (60.000đ/kém)")
            {
                txtGiaInTinhThem.Text = "60000";
                txtGiaInTinhThem.ReadOnly = true;
            }
            else if (selected == "Tự chỉnh-Máy Lớn" || selected == "Tự chỉnh-Máy Nhỏ")
            {
                txtGiaInTinhThem.Text = "";
                txtGiaInTinhThem.ReadOnly = false;
            }
            else
            {
                txtGiaInTinhThem.Text = "0";
                txtGiaInTinhThem.ReadOnly = true;
            }
        }
        private void cmbLoaiMayIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatGiaKem();
        }
        private void textBox44_TextChanged(object sender, EventArgs e)
        {

        }
        //hàm sinh mã
        // ===== HÀM SINH MÃ BÁO GIÁ =====
        string GenerateMaBaoGia(SqlConnection conn, SqlTransaction tran)
        {
            // ⭐ FIX: Dùng COUNT để mã đi từ BG-0001 liên tục
            // không bị nhảy số theo auto-increment id của DB
            string sql = "SELECT COUNT(*) FROM BAO_GIA";
            SqlCommand cmd = new SqlCommand(sql, conn, tran);
            int count = (int)cmd.ExecuteScalar();
            return "BG-" + (count + 1).ToString("D4");
        }

        private void btnTinhGia_Click(object sender, EventArgs e)
        {
            // ===== LẤY DỮ LIỆU AN TOÀN =====
            int soLuong = 0;
            int soCon = 0;
            double buHao = 0;
            double gsm = 0;
            double giaTan = 0;
            double dai = 0;
            double rong = 0;
            double giaCan = 0;
            double soMatCan = 0;
            double giaMetalize = 0;
            double soMatMetalize = 0;
            double giaUV = 0;
            double soMatUV = 0;
            double tienKhuonBe = 0;

            // Kiểm tra tất cả input cần thiết
            if (!int.TryParse(txtSoLuong.Text, out soLuong) ||
                !int.TryParse(txtSoCon.Text, out soCon) ||
                !double.TryParse(txtBuHaoTo.Text, out buHao) ||
                !double.TryParse(txtDinhLuongGiay.Text, out gsm) ||
                !double.TryParse(txtGiaGiayTan.Text, out giaTan) ||
                !double.TryParse(txtKhoInDai.Text, out dai) ||
                !double.TryParse(txtKhoInRong.Text, out rong) ||
                !double.TryParse(txtSoMatCanMangg.Text, out soMatCan) ||
                !double.TryParse(txtGiaCanMangDongM2.Text, out giaCan) ||
                !double.TryParse(txtSoMatMetallize.Text, out soMatMetalize) ||
                !double.TryParse(txtMetallizeDongM2.Text, out giaMetalize) ||
                !double.TryParse(txtSoMatUVMo.Text, out soMatUV) ||
                !double.TryParse(txtUVMoDongM2.Text, out giaUV) ||
                !double.TryParse(txtTienKhuonBe.Text, out tienKhuonBe))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ và đúng định dạng số!", "Lỗi nhập liệu");
                return;
            }

            // ===== TÍNH GIÁ =====
            double khoInM2 = (dai / 100.0) * (rong / 100.0);
            double gia1Ram = khoInM2 * (gsm / 1000.0) * (giaTan / 1000.0) * 500;
            double soToCanMua = Math.Ceiling((soLuong / (double)soCon) + buHao);
            double soRam = Math.Ceiling(soToCanMua / 500);
            double tienGiay = soRam * gia1Ram;

            string loaiMay = cmbLoaiMayIn.SelectedItem?.ToString() ?? "Máy lớn";
            int soMau = 0;
            int.TryParse(txtSoMauIn.Text, out soMau);

            double giaKem = loaiMay.Contains("Máy lớn") ? 100000 : 60000;
            double tienKem = soMau * giaKem;

            double tienIn = soLuong <= 5000 ? soMau * 300000 : soMau * 70 * soLuong;

            double dienTichTo = (dai / 100.0) * (rong / 100.0);
            double tienCanMang = dienTichTo * giaCan * soMatCan * soLuong / soCon;
            double tienMetalize = dienTichTo * giaMetalize * soMatMetalize * soLuong / soCon;
            double tienUV = dienTichTo * giaUV * soMatUV * soLuong / soCon;
            double tienBe = soCon * 150 * soLuong;

            double tienDan = 0;
            if (soLuong <= 5000)
                tienDan = 300000;
            else
            {
                int soLuongThem = soLuong - 5000;
                int soLanTang = (int)Math.Ceiling((double)soLuongThem / 1000);
                tienDan = 300000 + soLanTang * 100000;
            }

            double giaDay = double.Parse(txtGiaDayDongCai.Text);
            double tienDay = giaDay * soLuong;

            double giaNut = double.Parse(txtGiaNutDongCai.Text);
            double tienNut = giaNut * soLuong;

            double giaThung = double.Parse(txtGiaThungDongThung.Text);
            double quyCachDong = double.Parse(txtQuyCachDongCaiThung.Text);
            double soThung = Math.Ceiling((double)soLuong / quyCachDong);
            double tienThung = soThung * giaThung;

            double tienXeGiao = double.Parse(txtTienXeGiaoDong.Text);
            double tienProof = double.Parse(txtTienInProofDong.Text);

            double tongChiPhi = tienGiay + tienKem + tienIn + tienCanMang + tienMetalize +
                                tienUV + tienBe + tienKhuonBe + tienDan + tienDay +
                                tienNut + tienThung + tienXeGiao + tienProof;

            double loiNhuanPhanTram = double.Parse(txtTyLeLoiNhuanPhanTram.Text) / 100;
            double gia1SP = tongChiPhi / soLuong;
            double giaBaoKhach = gia1SP * (1 + loiNhuanPhanTram);
            double tongGiaBaoKhach = giaBaoKhach * soLuong; // ⭐ THÊM MỚI

            // ═════════════════════════════════════════════════════════════════
            // ⭐ LƯU VÀO DATABASE LUÔN
            // ═════════════════════════════════════════════════════════════════
            int idKhachHang = 0;
            int newIdBaoGia = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // ===== INSERT KHACH_HANG =====
                    string sqlKH = @"INSERT INTO KHACH_HANG
                        (Ma_KH, Ten_Khach_Hang, Dia_Chi, Ngay_Tao, Nguoi_Tao)
                        OUTPUT INSERTED.id
                        VALUES (@MaKH, @TenKH, @DiaChi, GETDATE(), @NguoiTao)";

                    SqlCommand cmdKH = new SqlCommand(sqlKH, conn, tran);
                    cmdKH.Parameters.AddWithValue("@MaKH", "KH" + DateTime.Now.Ticks);
                    cmdKH.Parameters.AddWithValue("@TenKH", txtTenKhachHangg.Text);
                    cmdKH.Parameters.AddWithValue("@DiaChi", txtDiaChii.Text);
                    cmdKH.Parameters.AddWithValue("@NguoiTao", CurrentUser.HoTen);

                    idKhachHang = (int)cmdKH.ExecuteScalar();

                    // ===== INSERT BAO_GIA =====
                    string maBaoGia = GenerateMaBaoGia(conn, tran);
                    string sqlBG = @"INSERT INTO BAO_GIA
                        (Ma_Bao_Gia, id_Khach_Hang, Ten_San_Pham, Ngay_Bao_Gia, So_Con, 
                         Gia_Giay_Tan, Loi_Nhuan_Phan_Tram, Kich_Thuoc_Thanh_Pham, 
                         Khoi_Luong_Giay, Kho_In, So_Mau_In, Thoi_Gian_Giao_Hang_Du_Kien,
                         Hieu_Luc_Bao_Gia_Ngay, Ten_Loai_Giay, Nguoi_Tao)
                        OUTPUT INSERTED.id
                        VALUES (@MaBG, @KH, @SP, @Ngay, @SoCon, @GiaGiay, @LoiNhuan, 
                                @KichThuoc, @KhoiLuongGiay, @KhoIn, @SoMauIn, @ThoiGianGiaoHangDuKien,
                                @HieuLuc, @TenLoaiGiay, @NguoiTao)";

                    SqlCommand cmdBG = new SqlCommand(sqlBG, conn, tran);
                    cmdBG.Parameters.AddWithValue("@MaBG", maBaoGia);
                    cmdBG.Parameters.AddWithValue("@KH", idKhachHang);
                    cmdBG.Parameters.AddWithValue("@SP", txtTenSanPhamm.Text);
                    cmdBG.Parameters.AddWithValue("@Ngay", DateTime.Now);
                    cmdBG.Parameters.AddWithValue("@SoCon", soCon);
                    cmdBG.Parameters.AddWithValue("@GiaGiay", giaTan);
                    cmdBG.Parameters.AddWithValue("@LoiNhuan", loiNhuanPhanTram * 100);
                    cmdBG.Parameters.AddWithValue("@KichThuoc", txtKichThuocSanPham.Text);
                    cmdBG.Parameters.AddWithValue("@KhoiLuongGiay", txtDinhLuongGiay.Text);
                    cmdBG.Parameters.AddWithValue("@KhoIn", $"{txtKhoInDai.Text} x {txtKhoInRong.Text}");
                    cmdBG.Parameters.AddWithValue("@SoMauIn", txtSoMauIn.Text);
                    cmdBG.Parameters.AddWithValue("@ThoiGianGiaoHangDuKien", txtThoiGianGiaoHangg.Text);
                    cmdBG.Parameters.AddWithValue("@HieuLuc",
                        int.TryParse(txtHieuLucBaoGia.Text, out int hl) ? hl : 30);
                    cmdBG.Parameters.AddWithValue("@TenLoaiGiay",  // ⭐ THÊM
                        cmbLoaiGiay.SelectedItem?.ToString() ?? "");
                    cmdBG.Parameters.AddWithValue("@NguoiTao", CurrentUser.HoTen);

                    newIdBaoGia = (int)cmdBG.ExecuteScalar();

                    // ===== ⭐ INSERT CHI_TIET_BAO_GIA LUÔN =====
                    string sqlCT = @"INSERT INTO CHI_TIET_BAO_GIA
                        (id_Bao_Gia, So_Luong, Tien_Giay, Tien_Kem, Tien_In, Tien_Can_Mang,
                         Tien_Metalize, Tien_UV, Tien_Be, Tien_Khuon_Be, Tien_Dan, Tien_Day,
                         Tien_Nut, Tien_Thung, Tien_Xe_Giao, Tien_Proof, Tong_Gia_Thanh,
                         Gia_Moi_Cai, Gia_Bao_Khach, Tong_Gia_Bao_Khach)
                        VALUES (@BG, @SL, @Giay, @Kem, @In, @Can, @Metal, @UV, @Be, @Khuon,
                                @Dan, @Day, @Nut, @Thung, @Xe, @Proof, @Tong, @GiaMoi, @GiaBao,
                                @TongGiaBao)";

                    SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran);
                    cmdCT.Parameters.AddWithValue("@BG", newIdBaoGia);
                    cmdCT.Parameters.AddWithValue("@SL", soLuong);
                    cmdCT.Parameters.AddWithValue("@Giay", tienGiay);
                    cmdCT.Parameters.AddWithValue("@Kem", tienKem);
                    cmdCT.Parameters.AddWithValue("@In", tienIn);
                    cmdCT.Parameters.AddWithValue("@Can", tienCanMang);
                    cmdCT.Parameters.AddWithValue("@Metal", tienMetalize);
                    cmdCT.Parameters.AddWithValue("@UV", tienUV);
                    cmdCT.Parameters.AddWithValue("@Be", tienBe);
                    cmdCT.Parameters.AddWithValue("@Khuon", tienKhuonBe);
                    cmdCT.Parameters.AddWithValue("@Dan", tienDan);
                    cmdCT.Parameters.AddWithValue("@Day", tienDay);
                    cmdCT.Parameters.AddWithValue("@Nut", tienNut);
                    cmdCT.Parameters.AddWithValue("@Thung", tienThung);
                    cmdCT.Parameters.AddWithValue("@Xe", tienXeGiao);
                    cmdCT.Parameters.AddWithValue("@Proof", tienProof);
                    cmdCT.Parameters.AddWithValue("@Tong", tongChiPhi);
                    cmdCT.Parameters.AddWithValue("@GiaMoi", gia1SP);
                    cmdCT.Parameters.AddWithValue("@GiaBao", giaBaoKhach);
                    cmdCT.Parameters.AddWithValue("@TongGiaBao", tongGiaBaoKhach); // ⭐ THÊM MỚI

                    cmdCT.ExecuteNonQuery();

                    tran.Commit();

                    // ⭐ THÔNG BÁO THÀNH CÔNG
                    MessageBox.Show(
                        $"✅ Đã lưu báo giá thành công!\n\n" +
                        $"Mã báo giá: {maBaoGia}\n" +
                        $"Tổng giá thành: {tongChiPhi:N0} VNĐ\n" +
                        $"Giá báo khách: {giaBaoKhach:N0} VNĐ/cái\n\n" +
                        $"Vui lòng chuyển qua trang Quản lý báo giá để xem chi tiết.",
                        "Lưu thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // ⭐ ĐÓNG FORM
                    this.Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show($"❌ Lỗi lưu dữ liệu:\n\n{ex.Message}", "Lỗi");
                }
            }
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            int soLuong = 0;
            int soCon = 0;
            double buHao = 0;
            double gsm = 0;
            double giaTan = 0;
            double dai = 0;
            double rong = 0;
            double giaCan = 0;
            double soMatCan = 0;
            double giaMetalize = 0;
            double soMatMetalize = 0;
            double giaUV = 0;
            double soMatUV = 0;
            double tienKhuonBe = 0;
            double giaDay = 0;
            double giaNut = 0;
            double giaThung = 0;
            double quyCachDong = 0;
            double tienXeGiao = 0;
            double tienProof = 0;

            if (!int.TryParse(txtSoLuong.Text, out soLuong) ||
                !int.TryParse(txtSoCon.Text, out soCon) ||
                !double.TryParse(txtBuHaoTo.Text, out buHao) ||
                !double.TryParse(txtDinhLuongGiay.Text, out gsm) ||
                !double.TryParse(txtGiaGiayTan.Text, out giaTan) ||
                !double.TryParse(txtKhoInDai.Text, out dai) ||
                !double.TryParse(txtKhoInRong.Text, out rong) ||
                !double.TryParse(txtGiaCanMangDongM2.Text, out giaCan) ||
                !double.TryParse(txtSoMatCanMangg.Text, out soMatCan) ||
                !double.TryParse(txtMetallizeDongM2.Text, out giaMetalize) ||
                !double.TryParse(txtSoMatMetallize.Text, out soMatMetalize) ||
                !double.TryParse(txtUVMoDongM2.Text, out giaUV) ||
                !double.TryParse(txtSoMatUVMo.Text, out soMatUV) ||
                !double.TryParse(txtTienKhuonBe.Text, out tienKhuonBe) ||
                !double.TryParse(txtGiaDayDongCai.Text, out giaDay) ||
                !double.TryParse(txtGiaNutDongCai.Text, out giaNut) ||
                !double.TryParse(txtGiaThungDongThung.Text, out giaThung) ||
                !double.TryParse(txtQuyCachDongCaiThung.Text, out quyCachDong) ||
                !double.TryParse(txtTienXeGiaoDong.Text, out tienXeGiao) ||
                !double.TryParse(txtTienInProofDong.Text, out tienProof) ||
                !double.TryParse(txtTyLeLoiNhuanPhanTram.Text, out double loiNhuanTemp))
            {
                MessageBox.Show("Vui lòng kiểm tra dữ liệu nhập!");
                return;
            }

            double loiNhuan = loiNhuanTemp / 100;

            // ===== PHẦN TÍNH GIÁ =====

            double khoInM2 = (dai / 100.0) * (rong / 100.0);
            double gia1Ram = khoInM2 * (gsm / 1000.0) * (giaTan / 1000.0) * 500;

            double soToCanMua = Math.Ceiling((soLuong / (double)soCon) + buHao);
            double soRam = Math.Ceiling(soToCanMua / 500);
            double tienGiay = soRam * gia1Ram;

            string loaiMay = cmbLoaiMayIn.SelectedItem?.ToString() ?? "Máy lớn";
            int soMau = 0;
            int.TryParse(txtSoMauIn.Text, out soMau);

            double giaKem = 0;
            if (loaiMay.Contains("Máy lớn")) giaKem = 100000;
            else if (loaiMay.Contains("Máy nhỏ")) giaKem = 60000;

            double tienKem = soMau * giaKem;

            double tienIn = 0;
            if (soLuong <= 5000)
                tienIn = soMau * 300000;
            else
                tienIn = soMau * 70 * soLuong;

            double dienTichTo = (dai / 100.0) * (rong / 100.0);  // m²/tờ (khớp mẫu: dài/100 × rộng/100)
            double tienCanMang = dienTichTo * giaCan * soMatCan * soLuong / soCon;
            double tienMetalize = dienTichTo * giaMetalize * soMatMetalize * soLuong / soCon;
            double tienUV = dienTichTo * giaUV * soMatUV * soLuong / soCon;

            double tienBe = soCon * 150 * soLuong;

            double tienDan = 0;
            if (soLuong <= 5000)
                tienDan = 300000;
            else
            {
                int soLuongThem = soLuong - 5000;
                int soLanTang = (int)Math.Ceiling((double)soLuongThem / 1000);
                tienDan = 300000 + soLanTang * 100000;
            }

            giaDay = double.Parse(txtGiaDayDongCai.Text);
            giaNut = double.Parse(txtGiaNutDongCai.Text);
            giaThung = double.Parse(txtGiaThungDongThung.Text);
            quyCachDong = double.Parse(txtQuyCachDongCaiThung.Text);

            double tienDay = giaDay * soLuong;
            double tienNut = giaNut * soLuong;

            double soThung = Math.Ceiling((double)soLuong / quyCachDong);
            double tienThung = soThung * giaThung;

            double tongChiPhi = tienGiay + tienKem + tienIn + tienCanMang + tienMetalize +
                                tienUV + tienBe + tienKhuonBe + tienDan + tienDay +
                                tienNut + tienThung + tienXeGiao + tienProof;

            double gia1SP = tongChiPhi / soLuong;
            double giaBaoKhach = gia1SP * (1 + loiNhuan);
            double tongGiaBaoKhach = giaBaoKhach * soLuong; // ⭐ THÊM MỚI

            // ===== LƯU DATABASE =====

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string sqlUpdate = @"UPDATE CHI_TIET_BAO_GIA SET
                So_Luong=@SL,
                Tien_Giay=@Giay,
                Tien_Kem=@Kem,
                Tien_In=@In,
                Tien_Can_Mang=@Can,
                Tien_Metalize=@Metal,
                Tien_UV=@UV,
                Tien_Be=@Be,
                Tien_Khuon_Be=@Khuon,
                Tien_Dan=@Dan,
                Tien_Day=@Day,
                Tien_Nut=@Nut,
                Tien_Thung=@Thung,
                Tien_Xe_Giao=@Xe,
                Tien_Proof=@Proof,
                Tong_Gia_Thanh=@Tong,
                Gia_Moi_Cai=@GiaMoi,
                Gia_Bao_Khach=@GiaBao,
                Tong_Gia_Bao_Khach=@TongGiaBao
                WHERE id_Bao_Gia=@id";

                    SqlCommand cmd = new SqlCommand(sqlUpdate, conn, tran);

                    cmd.Parameters.AddWithValue("@id", idBaoGia);
                    cmd.Parameters.AddWithValue("@SL", soLuong);
                    cmd.Parameters.AddWithValue("@Giay", tienGiay);
                    cmd.Parameters.AddWithValue("@Kem", tienKem);
                    cmd.Parameters.AddWithValue("@In", tienIn);
                    cmd.Parameters.AddWithValue("@Can", tienCanMang);
                    cmd.Parameters.AddWithValue("@Metal", tienMetalize);
                    cmd.Parameters.AddWithValue("@UV", tienUV);
                    cmd.Parameters.AddWithValue("@Be", tienBe);
                    cmd.Parameters.AddWithValue("@Khuon", tienKhuonBe);
                    cmd.Parameters.AddWithValue("@Dan", tienDan);
                    cmd.Parameters.AddWithValue("@Day", tienDay);
                    cmd.Parameters.AddWithValue("@Nut", tienNut);
                    cmd.Parameters.AddWithValue("@Thung", tienThung);
                    cmd.Parameters.AddWithValue("@Xe", tienXeGiao);
                    cmd.Parameters.AddWithValue("@Proof", tienProof);
                    cmd.Parameters.AddWithValue("@Tong", tongChiPhi);
                    cmd.Parameters.AddWithValue("@GiaMoi", gia1SP);
                    cmd.Parameters.AddWithValue("@GiaBao", giaBaoKhach);
                    cmd.Parameters.AddWithValue("@TongGiaBao", tongGiaBaoKhach); // ⭐ THÊM MỚI

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        string sqlInsert = @"INSERT INTO CHI_TIET_BAO_GIA
                (id_Bao_Gia,So_Luong,Tien_Giay,Tien_Kem,Tien_In,Tien_Can_Mang,
                Tien_Metalize,Tien_UV,Tien_Be,Tien_Khuon_Be,Tien_Dan,Tien_Day,
                Tien_Nut,Tien_Thung,Tien_Xe_Giao,Tien_Proof,Tong_Gia_Thanh,
                Gia_Moi_Cai,Gia_Bao_Khach,Tong_Gia_Bao_Khach)
                VALUES
                (@id,@SL,@Giay,@Kem,@In,@Can,@Metal,@UV,@Be,@Khuon,@Dan,@Day,
                @Nut,@Thung,@Xe,@Proof,@Tong,@GiaMoi,@GiaBao,@TongGiaBao)"; // ⭐ THÊM MỚI

                        SqlCommand cmdInsert = new SqlCommand(sqlInsert, conn, tran);

                        foreach (SqlParameter p in cmd.Parameters)
                            cmdInsert.Parameters.AddWithValue(p.ParameterName, p.Value);

                        cmdInsert.ExecuteNonQuery();
                    }

                    // ⭐ Cập nhật Hieu_Luc_Bao_Gia_Ngay trong BAO_GIA
                    var cmdUpdHL = new SqlCommand(
                        "UPDATE BAO_GIA SET Hieu_Luc_Bao_Gia_Ngay = @hl, Ten_Loai_Giay = @tlg WHERE id = @id",
                        conn, tran);
                    cmdUpdHL.Parameters.AddWithValue("@hl",
                        int.TryParse(txtHieuLucBaoGia.Text, out int hl2) ? hl2 : 30);
                    cmdUpdHL.Parameters.AddWithValue("@tlg",
                        cmbLoaiGiay.SelectedItem?.ToString() ?? ""); // ⭐ THÊM
                    cmdUpdHL.Parameters.AddWithValue("@id", idBaoGia);
                    cmdUpdHL.ExecuteNonQuery();

                    tran.Commit();
                    MessageBox.Show(
                "✅ Cập nhật báo giá thành công!\n\n" +
                "Vui lòng chuyển qua trang Quản lý báo giá để xem chi tiết.",
                "Cập nhật thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                    this.Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }



    }
}