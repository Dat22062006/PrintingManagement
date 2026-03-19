using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmInventoryReceive : Form
    {
        private int _idDonHang = 0;
        private int _idNCC = 0;
        private int _idPhieu = 0;
        private bool _daGhiSo = false;
        private bool _daLuu = false;
        private string _fileHoaDon = "";

        public frmInventoryReceive()
        {
            InitializeComponent();
            this.Load += frmInventoryReceive_Load;
        }

        public frmInventoryReceive(int idDonHang) : this()
        {
            _idDonHang = idDonHang;
        }

        private void frmInventoryReceive_Load(object sender, EventArgs e)
        {
            cboLoaiNhap.Items.Clear();
            cboLoaiNhap.Items.AddRange(new string[]
            {
                "Mua trong nước nhập kho",
                "Mua trong nước không qua kho",
                "Nhập khẩu nhập kho",
                "Nhập khẩu không qua kho"
            });
            cboLoaiNhap.SelectedIndex = 0;

            cboTrangThaiTT.Items.Clear();
            cboTrangThaiTT.Items.AddRange(new string[] { "Chưa thanh toán", "Thanh toán ngay" });
            cboTrangThaiTT.SelectedIndex = 0;

            cboPhuongThucTT.Items.Clear();
            cboPhuongThucTT.Items.AddRange(new string[] { "Tiền mặt", "Ủy nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPhuongThucTT.SelectedIndex = 0;

            cboTrangThaiHD.Items.Clear();
            cboTrangThaiHD.Items.AddRange(new string[] { "Nhận kèm hóa đơn", "Không kèm hóa đơn", "Không có hóa đơn" });
            cboTrangThaiHD.SelectedIndex = 0;

            dtpNgayChungTu.Value = DateTime.Today;
            dtpNgayHachToan.Value = DateTime.Today;
            dtpNgayHoaDon.Value = DateTime.Today;
            txtMauSoHD.Text = "01GTKT";

            SinhSoPhieuNhap();
            SetupGrid();
            LoadDonHangCombobox();

            if (_idDonHang > 0)
                LoadDuLieuTuDonHang(_idDonHang);
        }

        void SinhSoPhieuNhap()
        {
            string prefix = "PN-" + DateTime.Today.Year + "-";
            int next = 1;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Nhap, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM PHIEU_NHAP_KHO WHERE Ma_Phieu_Nhap LIKE @like", conn);
                    cmd.Parameters.AddWithValue("@p", prefix);
                    cmd.Parameters.AddWithValue("@like", prefix + "%");
                    next = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { }
            txtSoPhieuNhap.Text = prefix + next.ToString("D3");
            txtSoPhieuNhap.ReadOnly = true;
        }

        void LoadDonHangCombobox()
        {
            cboSoDonHang.SelectedIndexChanged -= cboSoDonHang_SelectedIndexChanged;
            cboSoDonHang.Items.Clear();
            cboSoDonHang.Items.Add(new DonHangItem());
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var rd = new SqlCommand(@"
SELECT d.id, d.Ma_Don_Hang, n.Ten_NCC, n.id AS idNCC, n.Dia_Chi
FROM DON_DAT_HANG_NCC d
JOIN NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
WHERE d.Trang_Thai NOT IN (N'Hoàn thành', N'Hủy')
ORDER BY d.Ngay_Dat_Hang DESC", conn).ExecuteReader();
                    while (rd.Read())
                        cboSoDonHang.Items.Add(new DonHangItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            MaDH = rd["Ma_Don_Hang"].ToString(),
                            TenNCC = rd["Ten_NCC"].ToString(),
                            IdNCC = Convert.ToInt32(rd["idNCC"]),
                            DiaChiNCC = rd["Dia_Chi"].ToString()
                        });
                }
            }
            catch { }
            cboSoDonHang.SelectedIndexChanged += cboSoDonHang_SelectedIndexChanged;

            if (_idDonHang > 0)
            {
                foreach (DonHangItem item in cboSoDonHang.Items)
                    if (item.Id == _idDonHang) { cboSoDonHang.SelectedItem = item; return; }
                cboSoDonHang.SelectedIndex = 0;
            }
            else
            {
                cboSoDonHang.SelectedIndex = 0;
            }
        }

        private void cboSoDonHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSoDonHang.SelectedItem is DonHangItem dh && dh.Id > 0)
                LoadDuLieuTuDonHang(dh.Id);
        }

        void LoadDuLieuTuDonHang(int idDH)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmdDH = new SqlCommand(@"
SELECT d.Dieu_Khoan_Thanh_Toan, n.Ten_NCC, n.Dia_Chi, n.id AS idNCC, n.Ma_NCC
FROM DON_DAT_HANG_NCC d
JOIN NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
WHERE d.id = @id", conn);
                    cmdDH.Parameters.AddWithValue("@id", idDH);
                    var rd = cmdDH.ExecuteReader();
                    if (rd.Read())
                    {
                        _idDonHang = idDH;
                        _idNCC = Convert.ToInt32(rd["idNCC"]);
                        txtTenNhaCungCap.Text = rd["Ten_NCC"].ToString();
                        txtDiaChiNhaCungCap.Text = rd["Dia_Chi"].ToString();
                        txtMaNCCThuCong.Text = rd["Ma_NCC"].ToString();
                        string pttt = rd["Dieu_Khoan_Thanh_Toan"].ToString();
                        for (int i = 0; i < cboPhuongThucTT.Items.Count; i++)
                            if (cboPhuongThucTT.Items[i].ToString() == pttt)
                            { cboPhuongThucTT.SelectedIndex = i; break; }
                    }
                    rd.Close();

                    var cmdCT = new SqlCommand(@"
SELECT ct.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh,
       ct.So_Luong AS SL_Dat, ct.Don_Gia, ct.Thanh_Tien,
       ct.Phan_Tram_Thue_GTGT, ct.Tien_Thue_GTGT, nl.id AS idNL
FROM CHI_TIET_DON_HANG_NCC ct
JOIN NGUYEN_LIEU nl ON ct.id_Nguyen_Lieu = nl.id
WHERE ct.id_Don_Dat_Hang = @idDH", conn);
                    cmdCT.Parameters.AddWithValue("@idDH", idDH);
                    var rdCT = cmdCT.ExecuteReader();
                    dgvChiTietNhapKho.Rows.Clear();
                    int stt = 1;
                    while (rdCT.Read())
                    {
                        double sl = Convert.ToDouble(rdCT["SL_Dat"]);
                        double dg = Convert.ToDouble(rdCT["Don_Gia"]);
                        double tt = Convert.ToDouble(rdCT["Thanh_Tien"]);
                        double vat = Convert.ToDouble(rdCT["Phan_Tram_Thue_GTGT"]);
                        double tVAT = Convert.ToDouble(rdCT["Tien_Thue_GTGT"]);
                        dgvChiTietNhapKho.Rows.Add(stt,
                            rdCT["Ma_Nguyen_Lieu"].ToString(),
                            rdCT["Ten_Nguyen_Lieu"].ToString(),
                            rdCT["Don_Vi_Tinh"].ToString(),
                            sl.ToString("N0"), sl.ToString("N0"),
                            dg.ToString("N0"), tt.ToString("N0"),
                            vat, tVAT.ToString("N0"),
                            Convert.ToInt32(rdCT["idNL"]),
                            Convert.ToInt32(rdCT["id"]));
                        stt++;
                    }
                    rdCT.Close();
                }
                TinhTong();
                foreach (DonHangItem item in cboSoDonHang.Items)
                    if (item.Id == idDH) { cboSoDonHang.SelectedItem = item; break; }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void SetupGrid()
        {
            var dgv = dgvChiTietNhapKho;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false; dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false; dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false; dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);

            var ro = new DataGridViewCellStyle { BackColor = Color.FromArgb(245, 247, 250), ForeColor = Color.FromArgb(80, 80, 80), Font = new Font("Segoe UI", 10f) };
            var ed = new DataGridViewCellStyle { BackColor = Color.FromArgb(255, 255, 220), ForeColor = Color.FromArgb(30, 80, 160), Font = new Font("Segoe UI", 10f, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleRight };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", Width = 50, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaHang", HeaderText = "Mã hàng", Width = 110, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(30, 100, 200), Font = new Font("Segoe UI", 10f, FontStyle.Bold) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTenHang", HeaderText = "Tên hàng", Width = 250, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDVT", HeaderText = "ĐVT", Width = 70, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSLDat", HeaderText = "SL đặt", Width = 90, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSLNhan", HeaderText = "SL nhận", Width = 90, ReadOnly = false, DefaultCellStyle = ed });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", HeaderText = "Đơn giá", Width = 130, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colThanhTien", HeaderText = "Thành tiền", Width = 150, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 80, 160) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVAT", HeaderText = "% VAT", Width = 70, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTienVAT", HeaderText = "Tiền VAT", Width = 120, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdNL", Width = 0, Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdCT", Width = 0, Visible = false });

            dgv.CellValueChanged += DgvNhapKho_CellValueChanged;
        }

        private void DgvNhapKho_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvChiTietNhapKho.Columns[e.ColumnIndex].Name != "colSLNhan") return;
            var row = dgvChiTietNhapKho.Rows[e.RowIndex];
            double.TryParse(row.Cells["colSLNhan"].Value?.ToString().Replace(",", ""), out double sl);
            double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double dg);
            double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vat);
            double tt = sl * dg, tVAT = tt * vat / 100.0;
            row.Cells["colThanhTien"].Value = tt.ToString("N0");
            row.Cells["colTienVAT"].Value = tVAT.ToString("N0");
            TinhTong();
        }

        void TinhTong()
        {
            double t = 0, v = 0;
            foreach (DataGridViewRow row in dgvChiTietNhapKho.Rows)
            {
                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                double.TryParse(row.Cells["colTienVAT"].Value?.ToString().Replace(",", ""), out double tv);
                t += tt; v += tv;
            }
            lblTongTienHang.Text = t.ToString("N0") + " đ";
            lblThueGTGT.Text = v.ToString("N0") + " đ";
            lblTongNhapKho.Text = (t + v).ToString("N0") + " đ";
        }

        // ======================== ĐÍNH KÈM HÓA ĐƠN ========================
        private void btnDinhKemHD_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = "Chọn file hóa đơn";
                    ofd.Filter = "Tất cả file|*.*|PDF|*.pdf|Excel|*.xlsx;*.xls|Ảnh|*.jpg;*.png";
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog(this) == DialogResult.OK)
                    {
                        _fileHoaDon = ofd.FileName;
                        MessageBox.Show(
                            $"✅ Đã chọn file:\n{System.IO.Path.GetFileName(_fileHoaDon)}",
                            "Đính kèm hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi chọn file:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== LƯU PHIẾU NHẬP (chưa ghi sổ) ======================
        private void btnLuuPhieuNhap_Click(object sender, EventArgs e)
        {
            if (_daGhiSo)
            {
                MessageBox.Show("⚠️ Phiếu đã ghi sổ, không thể chỉnh sửa!", "Đã ghi sổ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (_idDonHang == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn đơn đặt hàng!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (dgvChiTietNhapKho.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Không có hàng hóa nào trong phiếu!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            bool coHoaDon = cboTrangThaiHD.SelectedItem?.ToString() == "Nhận kèm hóa đơn";

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            if (_idPhieu == 0)
                            {
                                // INSERT phiếu mới
                                var cmd = new SqlCommand(@"
INSERT INTO PHIEU_NHAP_KHO
    (Ma_Phieu_Nhap,id_Don_Dat_Hang,id_Nha_Cung_Cap,
     Ngay_Nhap,Ngay_Chung_Tu,Ngay_Hach_Toan,Nguoi_Nhap,
     Loai_Nhap,Hinh_Thuc_Thanh_Toan,Trang_Thai_TT,
     Co_Hoa_Don,Mau_So_Hoa_Don,Ky_Hieu_Hoa_Don,
     So_Hoa_Don,Ngay_Hoa_Don,File_Hoa_Don,Trang_Thai)
OUTPUT INSERTED.id
VALUES(@maPN,@idDH,@idNCC,@ngayNhap,@ngayCT,@ngayHT,@nguoiNhap,
       @loai,@ptTT,@ttTT,@coHD,@mauSo,@kyHieu,
       @soHD,@ngayHD,@fileHD,N'Chưa ghi sổ')", conn, tran);

                                cmd.Parameters.AddWithValue("@maPN", txtSoPhieuNhap.Text.Trim());
                                cmd.Parameters.AddWithValue("@idDH", _idDonHang);
                                cmd.Parameters.AddWithValue("@idNCC", _idNCC);
                                cmd.Parameters.AddWithValue("@ngayNhap", dtpNgayChungTu.Value.Date);
                                cmd.Parameters.AddWithValue("@ngayCT", dtpNgayChungTu.Value.Date);
                                cmd.Parameters.AddWithValue("@ngayHT", dtpNgayHachToan.Value.Date);
                                cmd.Parameters.AddWithValue("@nguoiNhap", CurrentUser.HoTen ?? CurrentUser.Username ?? "");
                                cmd.Parameters.AddWithValue("@loai", cboLoaiNhap.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@ptTT", cboPhuongThucTT.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@ttTT", cboTrangThaiTT.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@coHD", coHoaDon ? 1 : 0);
                                cmd.Parameters.AddWithValue("@mauSo", txtMauSoHD.Text.Trim());
                                cmd.Parameters.AddWithValue("@kyHieu", txtKyHieuHD.Text.Trim());
                                cmd.Parameters.AddWithValue("@soHD", txtSoHoaDon.Text.Trim());
                                cmd.Parameters.AddWithValue("@ngayHD", dtpNgayHoaDon.Value.Date);
                                cmd.Parameters.AddWithValue("@fileHD", _fileHoaDon ?? "");

                                _idPhieu = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                // UPDATE phiếu cũ
                                var cmd = new SqlCommand(@"
UPDATE PHIEU_NHAP_KHO SET
    Ngay_Nhap=@ngayNhap,Ngay_Chung_Tu=@ngayCT,Ngay_Hach_Toan=@ngayHT,
    Loai_Nhap=@loai,Hinh_Thuc_Thanh_Toan=@ptTT,Trang_Thai_TT=@ttTT,
    Co_Hoa_Don=@coHD,Mau_So_Hoa_Don=@mauSo,Ky_Hieu_Hoa_Don=@kyHieu,
    So_Hoa_Don=@soHD,Ngay_Hoa_Don=@ngayHD,File_Hoa_Don=@fileHD
WHERE id=@id", conn, tran);

                                cmd.Parameters.AddWithValue("@ngayNhap", dtpNgayChungTu.Value.Date);
                                cmd.Parameters.AddWithValue("@ngayCT", dtpNgayChungTu.Value.Date);
                                cmd.Parameters.AddWithValue("@ngayHT", dtpNgayHachToan.Value.Date);
                                cmd.Parameters.AddWithValue("@loai", cboLoaiNhap.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@ptTT", cboPhuongThucTT.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@ttTT", cboTrangThaiTT.SelectedItem?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@coHD", coHoaDon ? 1 : 0);
                                cmd.Parameters.AddWithValue("@mauSo", txtMauSoHD.Text.Trim());
                                cmd.Parameters.AddWithValue("@kyHieu", txtKyHieuHD.Text.Trim());
                                cmd.Parameters.AddWithValue("@soHD", txtSoHoaDon.Text.Trim());
                                cmd.Parameters.AddWithValue("@ngayHD", dtpNgayHoaDon.Value.Date);
                                cmd.Parameters.AddWithValue("@fileHD", _fileHoaDon ?? "");
                                cmd.Parameters.AddWithValue("@id", _idPhieu);
                                cmd.ExecuteNonQuery();
                            }

                            // LƯU xong nhưng CHƯA đụng tồn kho, CHƯA ghi sổ
                            tran.Commit();
                            _daLuu = true;
                            MessageBox.Show(
                                $"✅ Đã lưu phiếu nhập (chưa ghi sổ).\nSố phiếu: {txtSoPhieuNhap.Text}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex2)
                        {
                            tran.Rollback();
                            throw ex2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi lưu phiếu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CongNoNCC_PhatSinhNo(SqlConnection conn, SqlTransaction tran, int idNCC, double soTienNo)
        {
            var cmdCheck = new SqlCommand(
                "SELECT id FROM CONG_NO_NCC WHERE id_Nha_Cung_Cap = @idNCC", conn, tran);
            cmdCheck.Parameters.AddWithValue("@idNCC", idNCC);
            var idCN = cmdCheck.ExecuteScalar();
            if (idCN == null)
            {
                // Con_Lai là computed (Tong_No - Da_Tra) nên KHÔNG set trực tiếp
                var cmdIns = new SqlCommand(@"
INSERT INTO CONG_NO_NCC (id_Nha_Cung_Cap, Tong_No, Da_Tra, Ngay_Cap_Nhat)
VALUES (@idNCC, @tongNo, 0, GETDATE())", conn, tran);
                cmdIns.Parameters.AddWithValue("@idNCC", idNCC);
                cmdIns.Parameters.AddWithValue("@tongNo", soTienNo);
                cmdIns.ExecuteNonQuery();
            }
            else
            {
                var cmdUpd = new SqlCommand(@"
UPDATE CONG_NO_NCC
SET Tong_No = Tong_No + @tangNo,
    Ngay_Cap_Nhat = GETDATE()
WHERE id_Nha_Cung_Cap = @idNCC", conn, tran);
                cmdUpd.Parameters.AddWithValue("@tangNo", soTienNo);
                cmdUpd.Parameters.AddWithValue("@idNCC", idNCC);
                cmdUpd.ExecuteNonQuery();
            }
        }

        // ========================== GHI SỔ (cập nhật tồn kho) ==========================
        // GHI SỔ (cập nhật tồn kho + công nợ NCC)
        private void btnGhiSo_Click(object sender, EventArgs e)
        {
            if (_daGhiSo)
            {
                MessageBox.Show("Phiếu này đã được ghi sổ rồi, không ghi lại lần nữa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_daLuu || _idPhieu == 0)
            {
                MessageBox.Show("Vui lòng bấm LƯU PHIẾU trước khi ghi sổ.", "Chưa lưu phiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvChiTietNhapKho.Rows.Count == 0)
            {
                MessageBox.Show("Không có dòng hàng nào để ghi sổ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                "Ghi sổ sẽ cộng số lượng vào tồn kho nguyên liệu, cập nhật công nợ nhà cung cấp và khóa phiếu này.\nBạn có chắc chắn muốn ghi sổ?",
                "Xác nhận ghi sổ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            double tongTienNhap = 0; // tiền hàng + VAT

                            // 1. Cập nhật tồn kho & So_Luong_Da_Nhan, đồng thời tính tổng tiền phiếu
                            // 1. Cập nhật tồn kho & So_Luong_Da_Nhan, đồng thời tính tổng tiền phiếu
                            // 1. Cập nhật tồn kho & So_Luong_Da_Nhan, đồng thời tính tổng tiền phiếu
                            foreach (DataGridViewRow row in dgvChiTietNhapKho.Rows)
                            {
                                int.TryParse(row.Cells["colIdNL"].Value?.ToString(), out int idNL);
                                int.TryParse(row.Cells["colIdCT"].Value?.ToString(), out int idCT);
                                double.TryParse(row.Cells["colSLNhan"].Value?.ToString().Replace(",", ""), out double slNhan);
                                double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double donGia);
                                double.TryParse(row.Cells["colTienVAT"].Value?.ToString().Replace(",", ""), out double tienVAT);

                                if (idNL > 0 && slNhan > 0)
                                {
                                    // 1.1. Ghi chi tiết phiếu nhập (KHÔNG dùng cột VAT)
                                    double thanhTienHang = slNhan * donGia;

                                    var cmdInsCTPN = new SqlCommand(@"
INSERT INTO CHI_TIET_PHIEU_NHAP
    (id_Phieu_Nhap, id_Nguyen_Lieu, So_Luong_Nhap, Don_Gia_Nhap, Thanh_Tien)
VALUES
    (@idPhieu, @idNL, @sl, @dg, @tt)", conn, tran);

                                    cmdInsCTPN.Parameters.AddWithValue("@idPhieu", _idPhieu);
                                    cmdInsCTPN.Parameters.AddWithValue("@idNL", idNL);
                                    cmdInsCTPN.Parameters.AddWithValue("@sl", slNhan);
                                    cmdInsCTPN.Parameters.AddWithValue("@dg", donGia);
                                    cmdInsCTPN.Parameters.AddWithValue("@tt", thanhTienHang);
                                    cmdInsCTPN.ExecuteNonQuery();

                                    // 1.2. Cộng tồn kho, cập nhật giá nhập
                                    var cmdTon = new SqlCommand(@"
UPDATE NGUYEN_LIEU SET
    Ton_Kho  = ISNULL(Ton_Kho, 0) + @slNhan,
    Gia_Nhap = @donGia
WHERE id = @idNL", conn, tran);
                                    cmdTon.Parameters.AddWithValue("@slNhan", slNhan);
                                    cmdTon.Parameters.AddWithValue("@donGia", donGia);
                                    cmdTon.Parameters.AddWithValue("@idNL", idNL);
                                    cmdTon.ExecuteNonQuery();
                                }

                                if (idCT > 0 && slNhan > 0)
                                {
                                    // 1.3. Cập nhật số lượng đã nhận trong chi tiết đơn hàng
                                    var cmdCT = new SqlCommand(@"
UPDATE CHI_TIET_DON_HANG_NCC SET
    So_Luong_Da_Nhan = ISNULL(So_Luong_Da_Nhan, 0) + @slNhan
WHERE id = @idCT", conn, tran);
                                    cmdCT.Parameters.AddWithValue("@slNhan", slNhan);
                                    cmdCT.Parameters.AddWithValue("@idCT", idCT);
                                    cmdCT.ExecuteNonQuery();
                                }

                                // 1.4. Cộng dồn tổng tiền phiếu (thành tiền + VAT)
                                double thanhTien = 0;
                                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out thanhTien);
                                tongTienNhap += thanhTien + tienVAT;
                            }
                            // 2. Cập nhật công nợ NCC
                            CongNoNCC_PhatSinhNo(conn, tran, _idNCC, tongTienNhap);

                            // 3. Đánh dấu phiếu đã ghi sổ
                            var cmdPhieu = new SqlCommand(@"
UPDATE PHIEU_NHAP_KHO
SET Trang_Thai = N'Đã ghi sổ'
WHERE id = @id", conn, tran);
                            cmdPhieu.Parameters.AddWithValue("@id", _idPhieu);
                            cmdPhieu.ExecuteNonQuery();

                            tran.Commit();
                            _daGhiSo = true;

                            MessageBox.Show(
                                "✅ Đã ghi sổ phiếu nhập, cập nhật tồn kho và công nợ nhà cung cấp!",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex2)
                        {
                            tran.Rollback();
                            throw ex2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi ghi sổ:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================= IN PHIẾU NHẬP =============================
        private void btnInPhieuNhap_Click(object sender, EventArgs e)
        {
            if (dgvChiTietNhapKho.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Không có dữ liệu để in!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            try
            {
                double tongTienHang = 0, tongVAT = 0;
                var lines = new List<string[]>();
                foreach (DataGridViewRow row in dgvChiTietNhapKho.Rows)
                {
                    double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                    double.TryParse(row.Cells["colTienVAT"].Value?.ToString().Replace(",", ""), out double tv);
                    tongTienHang += tt; tongVAT += tv;
                    lines.Add(new string[] {
                        row.Cells["colSTT"].Value?.ToString()       ?? "",
                        row.Cells["colMaHang"].Value?.ToString()    ?? "",
                        row.Cells["colTenHang"].Value?.ToString()   ?? "",
                        row.Cells["colDVT"].Value?.ToString()       ?? "",
                        row.Cells["colSLDat"].Value?.ToString()     ?? "",
                        row.Cells["colSLNhan"].Value?.ToString()    ?? "",
                        row.Cells["colDonGia"].Value?.ToString()    ?? "",
                        row.Cells["colThanhTien"].Value?.ToString() ?? "",
                        row.Cells["colVAT"].Value?.ToString()       ?? ""
                    });
                }
                string soPhieu = txtSoPhieuNhap.Text;
                string ngayCT = dtpNgayChungTu.Value.ToString("dd/MM/yyyy");
                string ngayHT = dtpNgayHachToan.Value.ToString("dd/MM/yyyy");
                string tenNCC = txtTenNhaCungCap.Text;
                string diaChiNCC = txtDiaChiNhaCungCap.Text;
                double tongCuoi = tongTienHang + tongVAT;

                var pd = new System.Drawing.Printing.PrintDocument();
                pd.DefaultPageSettings.Landscape = true;
                pd.PrintPage += (s, ev) =>
                {
                    var g = ev.Graphics;
                    var fT = new Font("Segoe UI", 13f, FontStyle.Bold);
                    var fH = new Font("Segoe UI", 9f, FontStyle.Bold);
                    var fN = new Font("Segoe UI", 9f);
                    var fS = new Font("Segoe UI", 8f);
                    int x = 30, y = 20;

                    g.DrawString("PHIẾU NHẬP KHO", fT, Brushes.Black, x, y); y += 26;
                    g.DrawString($"Số phiếu: {soPhieu}   |   Ngày CT: {ngayCT}   |   Ngày HT: {ngayHT}",
                        fN, Brushes.Gray, x, y); y += 22;
                    g.DrawLine(Pens.LightGray, x, y, 800, y); y += 10;
                    g.DrawString("NHÀ CUNG CẤP:", fH, Brushes.Black, x, y); y += 18;
                    g.DrawString($"Tên: {tenNCC}   |   Địa chỉ: {diaChiNCC}", fN, Brushes.Black, x, y); y += 22;
                    g.DrawLine(Pens.LightGray, x, y, 800, y); y += 10;

                    g.FillRectangle(new SolidBrush(Color.FromArgb(230, 235, 245)), x, y, 770, 24);
                    int[] xp = { 2, 35, 100, 310, 358, 408, 458, 568, 660 };
                    string[] hdrs = { "STT", "Mã hàng", "Tên hàng", "ĐVT", "SL đặt", "SL nhận", "Đơn giá", "Thành tiền", "VAT%" };
                    for (int i = 0; i < hdrs.Length; i++)
                        g.DrawString(hdrs[i], fH, Brushes.Black, x + xp[i], y + 4);
                    y += 26;
                    foreach (var r in lines)
                    {
                        for (int i = 0; i < r.Length && i < xp.Length; i++)
                            g.DrawString(r[i], fS, Brushes.Black, x + xp[i], y + 2);
                        g.DrawLine(Pens.LightGray, x, y + 20, x + 770, y + 20);
                        y += 22;
                    }
                    y += 10;
                    g.DrawLine(Pens.Gray, x + 450, y, x + 770, y); y += 6;
                    g.DrawString($"Tổng tiền hàng:  {tongTienHang:N0} đ", fN, Brushes.Black, x + 450, y); y += 20;
                    g.DrawString($"Thuế GTGT:       {tongVAT:N0} đ", fN, Brushes.Black, x + 450, y); y += 20;
                    g.DrawString($"TỔNG NHẬP KHO:  {tongCuoi:N0} đ", fH, new SolidBrush(Color.FromArgb(30, 80, 200)), x + 450, y);
                    fT.Dispose(); fH.Dispose(); fN.Dispose(); fS.Dispose();
                };
                new PrintPreviewDialog { Document = pd, WindowState = FormWindowState.Maximized, UseAntiAlias = true }
                    .ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi in phiếu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================== LÀM MỚI ==============================
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            if (!_daLuu && (_idDonHang > 0 || dgvChiTietNhapKho.Rows.Count > 0))
            {
                if (MessageBox.Show(
                    "Bạn chưa lưu phiếu nhập.\nBạn có chắc muốn làm mới không?\nDữ liệu chưa lưu sẽ bị mất!",
                    "Xác nhận làm mới", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            _idDonHang = 0; _idNCC = 0; _idPhieu = 0; _daGhiSo = false; _daLuu = false; _fileHoaDon = "";

            cboSoDonHang.SelectedIndex = 0;
            txtMaNCCThuCong.Clear(); txtTenNhaCungCap.Clear(); txtDiaChiNhaCungCap.Clear();
            cboLoaiNhap.SelectedIndex = 0; cboTrangThaiTT.SelectedIndex = 0;
            cboPhuongThucTT.SelectedIndex = 0; cboTrangThaiHD.SelectedIndex = 0;
            dtpNgayChungTu.Value = DateTime.Today; dtpNgayHachToan.Value = DateTime.Today;
            dtpNgayHoaDon.Value = DateTime.Today;
            txtMauSoHD.Text = "01GTKT"; txtKyHieuHD.Text = ""; txtSoHoaDon.Text = "";
            dgvChiTietNhapKho.Rows.Clear();
            lblTongTienHang.Text = "0 đ"; lblThueGTGT.Text = "0 đ"; lblTongNhapKho.Text = "0 đ";

            SinhSoPhieuNhap();
        }

        private class DonHangItem
        {
            public int Id { get; set; }
            public string MaDH { get; set; } = "";
            public string TenNCC { get; set; } = "-- Chọn đơn hàng --";
            public int IdNCC { get; set; }
            public string DiaChiNCC { get; set; } = "";
            public override string ToString() => string.IsNullOrEmpty(MaDH) ? TenNCC : MaDH;
        }
    }
}