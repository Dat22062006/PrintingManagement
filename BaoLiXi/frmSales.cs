using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSales : Form
    {
        private int _idBaoGia = 0;
        private int _idKhachHang = 0;

        public frmSales()
        {
            InitializeComponent();
            this.Load += frmSales_Load;
        }

        private void frmSales_Load(object sender, EventArgs e)
        {
            cboLoaiChungTu.Items.Clear();
            cboLoaiChungTu.Items.AddRange(new string[] { "Đơn đặt hàng của khách" });
            cboLoaiChungTu.SelectedIndex = 0;
            cboLoaiChungTu.SelectedIndexChanged += cboLoaiChungTu_SelectedIndexChanged;

            txtMaChungTu.Text = SinhMaChungTu();
            txtMaChungTu.ReadOnly = true;

            SetupGridChiTiet();
            SetupGridHoaDon();
            LoadDanhSachHoaDon();

            txtMauSoHoaDon.TextChanged += (s, _) => txtMauSoHoaDon.BackColor = Color.White;
            txtKyHieuHoaDon.TextChanged += (s, _) => txtKyHieuHoaDon.BackColor = Color.White;
            txtSoHoaDon.TextChanged += (s, _) => txtSoHoaDon.BackColor = Color.White;
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID CHI TIẾT — FillWeight, không bị cắt cột cuối
        // ═══════════════════════════════════════════════════════════════
        void SetupGridChiTiet()
        {
            var dgv = dgvChiTiet;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // ← Fill để tự căn
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            var ro = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            // STT: nhỏ cố định
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 5,
                MinimumWidth = 45,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Tên SP: chiếm phần lớn
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTenSP",
                HeaderText = "Tên sản phẩm",
                FillWeight = 35,
                MinimumWidth = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                { Font = new Font("Segoe UI", 10f), Padding = new Padding(6, 0, 4, 0) }
            });

            // Số lượng
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số lượng",
                FillWeight = 10,
                MinimumWidth = 80,
                ReadOnly = true,
                DefaultCellStyle = ro
            });

            // Đơn giá
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonGia",
                HeaderText = "Đơn giá",
                FillWeight = 13,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = ro
            });

            // Thành tiền
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThanhTien",
                HeaderText = "Thành tiền",
                FillWeight = 15,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 80, 160)
                }
            });

            // VAT %
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVAT",
                HeaderText = "VAT (%)",
                FillWeight = 7,
                MinimumWidth = 65,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Tiền VAT — FillWeight 15, đủ rộng không bị cắt
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTienVAT",
                HeaderText = "Tiền VAT",
                FillWeight = 15,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = ro
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdCT", Visible = false });
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID HÓA ĐƠN — FillWeight đồng bộ với ChiTiet
        // ═══════════════════════════════════════════════════════════════
        void SetupGridHoaDon()
        {
            var dgv = dgvHoaDon;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(76, 175, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoHD",
                HeaderText = "Số HĐ",
                FillWeight = 12,
                MinimumWidth = 100,
                DefaultCellStyle = center
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNgayHD",
                HeaderText = "Ngày HĐ",
                FillWeight = 10,
                MinimumWidth = 90,
                DefaultCellStyle = center
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colKhachHang",
                HeaderText = "Khách hàng",
                FillWeight = 28,
                MinimumWidth = 130
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSanPham",
                HeaderText = "Sản phẩm",
                FillWeight = 25,
                MinimumWidth = 120
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuongHD",
                HeaderText = "Số lượng",
                FillWeight = 10,
                MinimumWidth = 80,
                DefaultCellStyle = right
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền",
                FillWeight = 15,
                MinimumWidth = 120,
                DefaultCellStyle = new DataGridViewCellStyle(right)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(76, 175, 80)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTrangThai",
                HeaderText = "Trạng thái",
                FillWeight = 10,
                MinimumWidth = 85,
                DefaultCellStyle = center
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdDB", Visible = false });
        }

        private void cboLoaiChungTu_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loai = cboLoaiChungTu.SelectedItem?.ToString() ?? "";
            if (loai == "Đơn đặt hàng của khách")
            {
                cboDonHang.Enabled = true;
                LoadDonHangBaoGia();
            }
            else
            {
                cboDonHang.Enabled = false;
                cboDonHang.Items.Clear();
                cboDonHang.Text = "";
            }
        }

        void LoadDonHangBaoGia()
        {
            cboDonHang.SelectedIndexChanged -= cboDonHang_SelectedIndexChanged;
            cboDonHang.Items.Clear();
            cboDonHang.Items.Add(new BaoGiaItem());
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, bg.id_Khach_Hang, kh.Ten_Khach_Hang
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
WHERE bg.Trang_Thai = N'Hoàn thành'
ORDER BY bg.Ngay_Bao_Gia DESC";
                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        cboDonHang.Items.Add(new BaoGiaItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            MaBaoGia = rd["Ma_Bao_Gia"].ToString(),
                            TenSanPham = rd["Ten_San_Pham"].ToString(),
                            IdKhachHang = rd["id_Khach_Hang"] != DBNull.Value ? Convert.ToInt32(rd["id_Khach_Hang"]) : 0,
                            TenKhachHang = rd["Ten_Khach_Hang"]?.ToString() ?? ""
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi load báo giá:\n{ex.Message}", "Lỗi"); }

            cboDonHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDonHang.SelectedIndexChanged += cboDonHang_SelectedIndexChanged;
            cboDonHang.SelectedIndex = 0;
        }

        private void cboDonHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboDonHang.SelectedItem is BaoGiaItem bg) || bg.Id == 0)
            {
                _idBaoGia = 0; _idKhachHang = 0;
                txtKhachHang.Clear(); txtDiaChi.Clear(); txtMaSoThue.Clear();
                dgvChiTiet.Rows.Clear();
                lblTongCong.Text = "0đ";
                return;
            }
            _idBaoGia = bg.Id;
            _idKhachHang = bg.IdKhachHang;
            LoadThongTinKhachHang(_idKhachHang);
            LoadChiTietBaoGia(_idBaoGia);
        }

        void LoadThongTinKhachHang(int idKH)
        {
            if (idKH == 0) { txtKhachHang.Text = "(Khách lẻ)"; txtDiaChi.Clear(); txtMaSoThue.Clear(); return; }
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Ten_Khach_Hang, Dia_Chi, MST FROM KHACH_HANG WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", idKH);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        txtKhachHang.Text = rd["Ten_Khach_Hang"].ToString();
                        txtDiaChi.Text = rd["Dia_Chi"]?.ToString() ?? "";
                        txtMaSoThue.Text = rd["MST"]?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi:\n{ex.Message}", "Lỗi"); }
        }

        void LoadChiTietBaoGia(int idBG)
        {
            dgvChiTiet.Rows.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT ct.id, bg.Ten_San_Pham, ct.So_Luong,
       ct.Gia_Bao_Khach                  AS DonGia,
       (ct.So_Luong * ct.Gia_Bao_Khach)  AS ThanhTien
FROM CHI_TIET_BAO_GIA ct
JOIN BAO_GIA bg ON bg.id = ct.id_Bao_Gia
WHERE ct.id_Bao_Gia = @id";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", idBG);
                    var rd = cmd.ExecuteReader();

                    int stt = 1;
                    double tongTruocThue = 0;
                    double vatPct = 10;

                    while (rd.Read())
                    {
                        double soLuong = Convert.ToDouble(rd["So_Luong"]);
                        double donGia = Convert.ToDouble(rd["DonGia"]);
                        double thanhTien = Convert.ToDouble(rd["ThanhTien"]); // chưa VAT
                        double tienVAT = Math.Round(thanhTien * vatPct / 100, 0);

                        dgvChiTiet.Rows.Add(
                            stt, rd["Ten_San_Pham"],
                            soLuong.ToString("N0"),
                            donGia.ToString("N0"),
                            thanhTien.ToString("N0"),
                            vatPct,
                            tienVAT.ToString("N0"),
                            Convert.ToInt32(rd["id"]));

                        tongTruocThue += thanhTien;
                        stt++;
                    }

                    // Tổng cuối đúng công thức
                    double tongVAT = Math.Round(tongTruocThue * vatPct / 100, 0);
                    double tongThanhToan = tongTruocThue + tongVAT;
                    lblTongCong.Text = tongThanhToan.ToString("N0") + "đ";
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi:\n{ex.Message}", "Lỗi"); }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT LƯU CHỨNG TỪ
        // ═══════════════════════════════════════════════════════════════
        private void btnLuuChungTu_Click(object sender, EventArgs e)
        {
            if (_idBaoGia == 0)
            { MessageBox.Show("⚠️ Vui lòng chọn đơn hàng!", "Thiếu thông tin"); return; }

            if (dgvChiTiet.Rows.Count == 0)
            { MessageBox.Show("⚠️ Không có sản phẩm nào!", "Thiếu thông tin"); return; }

            // Validate 3 ô hóa đơn bắt buộc
            txtMauSoHoaDon.BackColor = Color.White;
            txtKyHieuHoaDon.BackColor = Color.White;
            txtSoHoaDon.BackColor = Color.White;

            bool hdOk = true;
            string hdMsg = "";

            if (string.IsNullOrWhiteSpace(txtMauSoHoaDon.Text))
            {
                txtMauSoHoaDon.BackColor = Color.FromArgb(255, 235, 235);
                txtMauSoHoaDon.Focus();
                hdMsg += "• Mẫu số hóa đơn\n";
                hdOk = false;
            }
            if (string.IsNullOrWhiteSpace(txtKyHieuHoaDon.Text))
            {
                txtKyHieuHoaDon.BackColor = Color.FromArgb(255, 235, 235);
                if (hdOk) txtKyHieuHoaDon.Focus();
                hdMsg += "• Ký hiệu hóa đơn\n";
                hdOk = false;
            }
            if (string.IsNullOrWhiteSpace(txtSoHoaDon.Text))
            {
                txtSoHoaDon.BackColor = Color.FromArgb(255, 235, 235);
                if (hdOk) txtSoHoaDon.Focus();
                hdMsg += "• Số hóa đơn\n";
                hdOk = false;
            }

            if (!hdOk)
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin hóa đơn:\n\n" + hdMsg,
                    "Thiếu thông tin hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rs = MessageBox.Show(
                $"Xác nhận lưu đơn bán hàng?\n\nKhách hàng: {txtKhachHang.Text}\nTổng tiền: {lblTongCong.Text}",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Tính lại tổng từ grid
                            double tongTruocThue = 0;
                            double vatPhanTram = 10;
                            foreach (DataGridViewRow row in dgvChiTiet.Rows)
                            {
                                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                                tongTruocThue += tt;
                            }
                            double tongTienVAT = Math.Round(tongTruocThue * vatPhanTram / 100, 0);
                            double tongThanhToan = tongTruocThue + tongTienVAT;

                            // INSERT DON_BAN_HANG — đủ tất cả cột
                            string sqlIns = @"
INSERT INTO DON_BAN_HANG
    (Ma_Don_Ban, id_Khach_Hang, Ngay_Ban_Hang,
     Mau_So_Hoa_Don, Ky_Hieu_Hoa_Don, So_Hoa_Don, Ngay_Hoa_Don,
     Tong_Tien_Truoc_Thue, Phan_Tram_VAT, Tien_Thue_VAT, Tong_Thanh_Toan,
     Trang_Thai)
OUTPUT INSERTED.id
VALUES
    (@ma, @idKH, @ngayBan,
     @mauSo, @kyHieu, @soHD, @ngayHD,
     @truocThue, @vatPct, @tienVAT, @thanhToan,
     N'Đã xuất')";

                            var cmd = new SqlCommand(sqlIns, conn, tran);
                            cmd.Parameters.AddWithValue("@ma", txtMaChungTu.Text.Trim());
                            cmd.Parameters.AddWithValue("@idKH", _idKhachHang > 0 ? (object)_idKhachHang : DBNull.Value);
                            cmd.Parameters.AddWithValue("@ngayBan", DateTime.Today);
                            cmd.Parameters.AddWithValue("@mauSo", txtMauSoHoaDon.Text.Trim());
                            cmd.Parameters.AddWithValue("@kyHieu", txtKyHieuHoaDon.Text.Trim());
                            cmd.Parameters.AddWithValue("@soHD", txtSoHoaDon.Text.Trim());
                            cmd.Parameters.AddWithValue("@ngayHD", dtpNgayHoaDon.Value.Date);
                            cmd.Parameters.AddWithValue("@truocThue", tongTruocThue);
                            cmd.Parameters.AddWithValue("@vatPct", vatPhanTram);
                            cmd.Parameters.AddWithValue("@tienVAT", tongTienVAT);
                            cmd.Parameters.AddWithValue("@thanhToan", tongThanhToan);

                            int idDonBan = Convert.ToInt32(cmd.ExecuteScalar());

                            // INSERT CHI_TIET_DON_BAN_HANG
                            foreach (DataGridViewRow row in dgvChiTiet.Rows)
                            {
                                string tenSP = row.Cells["colTenSP"].Value?.ToString() ?? "";
                                double.TryParse(row.Cells["colSoLuong"].Value?.ToString().Replace(",", ""), out double sl);
                                double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double dg);
                                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                                double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vatRow);
                                double.TryParse(row.Cells["colTienVAT"].Value?.ToString().Replace(",", ""), out double tienVAT);

                                var cmdCT = new SqlCommand(@"
INSERT INTO CHI_TIET_DON_BAN_HANG
    (id_Don_Ban_Hang, Ten_San_Pham, So_Luong,
     Don_Gia, Thanh_Tien, Phan_Tram_VAT, Tien_VAT)
VALUES (@idDB, @tenSP, @sl, @dg, @tt, @vatRow, @tienVAT)", conn, tran);

                                cmdCT.Parameters.AddWithValue("@idDB", idDonBan);
                                cmdCT.Parameters.AddWithValue("@tenSP", tenSP);
                                cmdCT.Parameters.AddWithValue("@sl", sl);
                                cmdCT.Parameters.AddWithValue("@dg", dg);
                                cmdCT.Parameters.AddWithValue("@tt", tt);
                                cmdCT.Parameters.AddWithValue("@vatRow", vatRow);
                                cmdCT.Parameters.AddWithValue("@tienVAT", tienVAT);
                                cmdCT.ExecuteNonQuery();
                            }

                            // Cập nhật BAO_GIA → Hoàn thành
                            var cmdUpd = new SqlCommand(
                                "UPDATE BAO_GIA SET Trang_Thai = N'Hoàn thành' WHERE id = @id",
                                conn, tran);
                            cmdUpd.Parameters.AddWithValue("@id", _idBaoGia);
                            cmdUpd.ExecuteNonQuery();

                            tran.Commit();
                            MessageBox.Show($"✅ Lưu thành công!\nMã đơn: {txtMaChungTu.Text}", "Thành công");
                            LoadDanhSachHoaDon();
                            ResetForm();
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi lưu:\n{ex.Message}", "Lỗi"); }
        }

        void LoadDanhSachHoaDon()
        {
            dgvHoaDon.Rows.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT
    dbh.id,
    dbh.Ma_Don_Ban,
    dbh.Ngay_Ban_Hang,
    ISNULL(kh.Ten_Khach_Hang, N'(Khách lẻ)') AS Ten_Khach_Hang,
    ISNULL(ct.Ten_San_Pham,   N'')             AS Ten_San_Pham,
    ISNULL(ct.Tong_SL,        0)               AS So_Luong,
    dbh.Tong_Thanh_Toan,
    dbh.Trang_Thai
FROM DON_BAN_HANG dbh
LEFT JOIN KHACH_HANG kh ON kh.id = dbh.id_Khach_Hang
LEFT JOIN (
    SELECT id_Don_Ban_Hang,
           MAX(Ten_San_Pham) AS Ten_San_Pham,
           SUM(So_Luong)     AS Tong_SL
    FROM CHI_TIET_DON_BAN_HANG
    GROUP BY id_Don_Ban_Hang
) ct ON ct.id_Don_Ban_Hang = dbh.id
ORDER BY dbh.Ngay_Ban_Hang DESC, dbh.id DESC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        string ngay = rd["Ngay_Ban_Hang"] != DBNull.Value
                            ? Convert.ToDateTime(rd["Ngay_Ban_Hang"]).ToString("dd/MM/yyyy") : "";
                        double tongTT = rd["Tong_Thanh_Toan"] != DBNull.Value
                            ? Convert.ToDouble(rd["Tong_Thanh_Toan"]) : 0;

                        dgvHoaDon.Rows.Add(
                            rd["Ma_Don_Ban"], ngay,
                            rd["Ten_Khach_Hang"], rd["Ten_San_Pham"],
                            Convert.ToDouble(rd["So_Luong"]).ToString("N0"),
                            tongTT.ToString("N0") + "đ",
                            rd["Trang_Thai"],
                            rd["id"]);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ Lỗi load hóa đơn:\n{ex.Message}", "Lỗi"); }
        }

        string SinhMaChungTu()
        {
            string prefix = "BH-" + DateTime.Today.Year + "-";
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Don_Ban, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM DON_BAN_HANG WHERE Ma_Don_Ban LIKE @like", conn);
                    cmd.Parameters.AddWithValue("@p", prefix);
                    cmd.Parameters.AddWithValue("@like", prefix + "%");
                    return prefix + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D3");
                }
            }
            catch { return prefix + "001"; }
        }

        void ResetForm()
        {
            cboLoaiChungTu.SelectedIndex = 0;
            cboDonHang.SelectedIndex = 0;
            txtKhachHang.Clear(); txtDiaChi.Clear(); txtMaSoThue.Clear();
            txtMauSoHoaDon.Clear(); txtMauSoHoaDon.BackColor = Color.White;
            txtKyHieuHoaDon.Clear(); txtKyHieuHoaDon.BackColor = Color.White;
            txtSoHoaDon.Clear(); txtSoHoaDon.BackColor = Color.White;
            dtpNgayHoaDon.Value = DateTime.Today;
            dgvChiTiet.Rows.Clear();
            lblTongCong.Text = "0đ";
            txtMaChungTu.Text = SinhMaChungTu();
            _idBaoGia = 0; _idKhachHang = 0;
            LoadDonHangBaoGia();
        }

        private class BaoGiaItem
        {
            public int Id { get; set; }
            public string MaBaoGia { get; set; } = "";
            public string TenSanPham { get; set; } = "";
            public int IdKhachHang { get; set; }
            public string TenKhachHang { get; set; } = "";

            public override string ToString()
                => Id == 0 ? "-- Chọn đơn hàng --" : $"{MaBaoGia} - {TenSanPham} ({TenKhachHang})";
        }
    }
}