using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPaymentReceive : Form
    {
        bool dangThuTien = false;

        public frmPaymentReceive()
        {
            InitializeComponent();
            this.Load += frmPaymentReceive_Load;
        }

        private void frmPaymentReceive_Load(object sender, EventArgs e)
        {
            SetupGrid();

            cboPhuongThuc.Items.Clear();
            cboPhuongThuc.Items.AddRange(new object[]
            {
                "Tiền mặt",
                "Chuyển khoản",
                "Séc"
            });
            cboPhuongThuc.SelectedIndex = 0;

            dtpNgayThu.Value = DateTime.Today;

            LoadKhachHang();

            cboKhachHang.SelectedIndexChanged -= CboKhachHang_SelectedIndexChanged;
            cboKhachHang.SelectedIndexChanged += CboKhachHang_SelectedIndexChanged;

            dgvCongNo.CurrentCellDirtyStateChanged -= DgvCongNo_CurrentCellDirtyStateChanged;
            dgvCongNo.CurrentCellDirtyStateChanged += DgvCongNo_CurrentCellDirtyStateChanged;

            dgvCongNo.CellValueChanged -= DgvCongNo_CellValueChanged;
            dgvCongNo.CellValueChanged += DgvCongNo_CellValueChanged;

            dgvCongNo.CellEndEdit -= DgvCongNo_CellEndEdit;
            dgvCongNo.CellEndEdit += DgvCongNo_CellEndEdit;

            btnThuTien.Click -= BtnThuTien_Click;
            btnThuTien.Click += BtnThuTien_Click;

            btnInPhieuThu.Click -= BtnInPhieuThu_Click;
            btnInPhieuThu.Click += BtnInPhieuThu_Click;
        }

        private void SetupGrid()
        {
            dgvCongNo.Columns.Clear();
            dgvCongNo.RowHeadersVisible = false;
            dgvCongNo.AllowUserToAddRows = false;
            dgvCongNo.AllowUserToDeleteRows = false;
            dgvCongNo.MultiSelect = false;
            dgvCongNo.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvCongNo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCongNo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            var right = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0)
            };
            var boldRed = new DataGridViewCellStyle(right)
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };
            var yellowRight = new DataGridViewCellStyle(right)
            {
                BackColor = Color.FromArgb(255, 251, 220)
            };
            var center = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvCongNo.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colChon",
                HeaderText = "Chọn",
                FillWeight = 6,
                DefaultCellStyle = center
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoHD",
                HeaderText = "Số HĐ",
                FillWeight = 14
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNgayHD",
                HeaderText = "Ngày HĐ",
                FillWeight = 11,
                DefaultCellStyle = center
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSanPham",
                HeaderText = "Sản phẩm",
                FillWeight = 20
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền",
                FillWeight = 13,
                ReadOnly = true,
                DefaultCellStyle = right
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDaThu",
                HeaderText = "Đã thu",
                FillWeight = 12,
                ReadOnly = true,
                DefaultCellStyle = right
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConNo",
                HeaderText = "Còn nợ",
                FillWeight = 13,
                ReadOnly = true,
                DefaultCellStyle = boldRed
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoTienThu",
                HeaderText = "Số tiền thu",
                FillWeight = 14,
                DefaultCellStyle = yellowRight
            });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn { Name = "colConNoRaw", Visible = false });
            dgvCongNo.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdDonBan", Visible = false });

            dgvCongNo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCongNo.ColumnHeadersHeight = 45;
            dgvCongNo.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            dgvCongNo.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCongNo.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);

            foreach (DataGridViewColumn col in dgvCongNo.Columns)
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadKhachHang()
        {
            try
            {
                cboKhachHang.Items.Clear();
                cboKhachHang.Items.Add(new KhachHangItem { Id = 0, Ten = "-- Chọn khách hàng --" });

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
SELECT DISTINCT kh.id, kh.Ten_Khach_Hang
FROM KHACH_HANG kh
INNER JOIN DON_BAN_HANG dbh ON dbh.id_Khach_Hang = kh.id
WHERE dbh.Trang_Thai <> N'Đã hủy'
ORDER BY kh.Ten_Khach_Hang", conn);
                    var rd = cmd.ExecuteReader();
                    while (rd.Read())
                        cboKhachHang.Items.Add(new KhachHangItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Ten = rd["Ten_Khach_Hang"].ToString()
                        });
                }
                cboKhachHang.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load khách hàng:\n" + ex.Message, "Lỗi");
            }
        }

        private void CboKhachHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dangThuTien) return;
            dgvCongNo.Rows.Clear();
            lblTongThuValue.Text = "0 đ";
            var kh = cboKhachHang.SelectedItem as KhachHangItem;
            if (kh == null || kh.Id == 0) return;
            LoadDonBanCongNo(kh.Id);
        }

        private void LoadDonBanCongNo(int idKhachHang)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT
    dbh.id,
    dbh.Ma_Don_Ban                                        AS SoHD,
    dbh.Ngay_Ban_Hang                                     AS NgayHD,
    ISNULL(ct.Ten_San_Pham, N'(không có chi tiết)')       AS SanPham,
    dbh.Tong_Thanh_Toan                                   AS TongTien,
    ISNULL(SUM(pt.So_Tien_Thu), 0)                        AS DaThu,
    dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) AS ConNo
FROM DON_BAN_HANG dbh
LEFT JOIN PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
LEFT JOIN (
    SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham
    FROM CHI_TIET_DON_BAN_HANG
    GROUP BY id_Don_Ban_Hang
) ct ON ct.id_Don_Ban_Hang = dbh.id
WHERE dbh.id_Khach_Hang = @idKH
  AND ISNULL(dbh.Trang_Thai, '') <> N'Đã hủy'
GROUP BY
    dbh.id, dbh.Ma_Don_Ban, dbh.Ngay_Ban_Hang,
    dbh.Tong_Thanh_Toan, ct.Ten_San_Pham
HAVING
    dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) > 0
ORDER BY dbh.Ngay_Ban_Hang DESC";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@idKH", idKhachHang);
                    var rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        decimal tongTien = Convert.ToDecimal(rd["TongTien"]);
                        decimal daThu = Convert.ToDecimal(rd["DaThu"]);
                        decimal conNo = Convert.ToDecimal(rd["ConNo"]);

                        int idx = dgvCongNo.Rows.Add();
                        var row = dgvCongNo.Rows[idx];

                        row.Cells["colChon"].Value = false;
                        row.Cells["colSoHD"].Value = rd["SoHD"].ToString();
                        row.Cells["colNgayHD"].Value = Convert.ToDateTime(rd["NgayHD"]).ToString("dd/MM/yyyy");
                        row.Cells["colSanPham"].Value = rd["SanPham"].ToString();
                        row.Cells["colTongTien"].Value = tongTien.ToString("N0") + " đ";
                        row.Cells["colDaThu"].Value = daThu.ToString("N0") + " đ";
                        row.Cells["colConNo"].Value = conNo.ToString("N0") + " đ";
                        row.Cells["colSoTienThu"].Value = conNo.ToString("N0");
                        row.Cells["colConNoRaw"].Value = conNo.ToString();
                        row.Cells["colIdDonBan"].Value = rd["id"].ToString();
                        row.Tag = new decimal[] { tongTien, daThu, conNo };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load công nợ:\n" + ex.Message, "Lỗi");
            }
        }

        private void DgvCongNo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCongNo.IsCurrentCellDirty)
                dgvCongNo.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvCongNo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == dgvCongNo.Columns["colChon"].Index)
            {
                bool chk = Convert.ToBoolean(dgvCongNo.Rows[e.RowIndex].Cells["colChon"].Value ?? false);
                decimal[] vals = dgvCongNo.Rows[e.RowIndex].Tag as decimal[];
                if (chk && vals != null)
                    dgvCongNo.Rows[e.RowIndex].Cells["colSoTienThu"].Value = vals[2].ToString("N0");
                else if (!chk)
                    dgvCongNo.Rows[e.RowIndex].Cells["colSoTienThu"].Value = "0";
            }
            TinhTongThu();
        }

        private void DgvCongNo_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvCongNo.Columns["colSoTienThu"].Index) return;

            var row = dgvCongNo.Rows[e.RowIndex];
            string raw = (row.Cells["colSoTienThu"].Value ?? "0").ToString()
                         .Replace(",", "").Replace(" đ", "").Trim();

            if (!decimal.TryParse(raw, out decimal soTienThu) || soTienThu < 0)
            {
                MessageBox.Show("Số tiền thu không hợp lệ!", "Cảnh báo");
                row.Cells["colSoTienThu"].Value = "0";
                return;
            }

            decimal[] vals = row.Tag as decimal[];
            if (vals != null && soTienThu > vals[2])
            {
                MessageBox.Show("Số tiền thu không được vượt quá còn nợ (" +
                    vals[2].ToString("N0") + " đ)!", "Cảnh báo");
                row.Cells["colSoTienThu"].Value = vals[2].ToString("N0");
                soTienThu = vals[2];
            }

            if (soTienThu > 0) row.Cells["colChon"].Value = true;
            TinhTongThu();
        }

        private void TinhTongThu()
        {
            decimal tong = 0;
            foreach (DataGridViewRow row in dgvCongNo.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colChon"].Value ?? false)) continue;
                string raw = (row.Cells["colSoTienThu"].Value ?? "0").ToString()
                             .Replace(",", "").Replace(" đ", "").Trim();
                if (decimal.TryParse(raw, out decimal st)) tong += st;
            }
            lblTongThuValue.Text = tong.ToString("N0") + " đ";
        }

        private void BtnThuTien_Click(object sender, EventArgs e)
        {
            if (dangThuTien) return;

            var kh = cboKhachHang.SelectedItem as KhachHangItem;
            if (kh == null || kh.Id == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thiếu thông tin");
                return;
            }

            bool coChon = false;
            foreach (DataGridViewRow row in dgvCongNo.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colChon"].Value ?? false)) continue;
                string raw = (row.Cells["colSoTienThu"].Value ?? "0").ToString()
                             .Replace(",", "").Replace(" đ", "").Trim();
                if (decimal.TryParse(raw, out decimal st) && st > 0) { coChon = true; break; }
            }
            if (!coChon)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 đơn hàng và nhập số tiền thu!", "Thiếu thông tin");
                return;
            }

            if (MessageBox.Show("Xác nhận thu tiền khách hàng?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            dangThuTien = true;
            int idKhachHang = kh.Id;
            DateTime ngayThu = dtpNgayThu.Value.Date;
            string phuongThuc = cboPhuongThuc.Text;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    try
                    {
                        decimal tongThuThucTe = 0;

                        foreach (DataGridViewRow row in dgvCongNo.Rows)
                        {
                            if (!Convert.ToBoolean(row.Cells["colChon"].Value ?? false)) continue;
                            string rawSt = (row.Cells["colSoTienThu"].Value ?? "0").ToString()
                                           .Replace(",", "").Replace(" đ", "").Trim();
                            if (!decimal.TryParse(rawSt, out decimal soTienThu) || soTienThu <= 0) continue;

                            int idDonBan = int.Parse(row.Cells["colIdDonBan"].Value.ToString());
                            string maPhieuThu = SinhMaPhieuThu(conn, tran);

                            var cmdIns = new SqlCommand(@"
INSERT INTO PHIEU_THU
    (Ma_Phieu_Thu, id_Khach_Hang, id_Don_Ban_Hang,
     Ngay_Thu, So_Tien_Thu, Phuong_Thuc_Thu,
     So_Chung_Tu, Ghi_Chu, Ngay_Tao)
VALUES
    (@MaPT, @idKH, @idDBH,
     @NgayThu, @SoTien, @PhuongThuc,
     NULL, NULL, GETDATE())", conn, tran);
                            cmdIns.Parameters.AddWithValue("@MaPT", maPhieuThu);
                            cmdIns.Parameters.AddWithValue("@idKH", idKhachHang);
                            cmdIns.Parameters.AddWithValue("@idDBH", idDonBan);
                            cmdIns.Parameters.AddWithValue("@NgayThu", ngayThu);
                            cmdIns.Parameters.AddWithValue("@SoTien", soTienThu);
                            cmdIns.Parameters.AddWithValue("@PhuongThuc", phuongThuc);
                            cmdIns.ExecuteNonQuery();
                            tongThuThucTe += soTienThu;
                        }

                        var cmdTongNo = new SqlCommand(
                            "SELECT ISNULL(SUM(Tong_Thanh_Toan),0) FROM DON_BAN_HANG " +
                            "WHERE id_Khach_Hang=@idKH AND Trang_Thai<>N'Đã hủy'", conn, tran);
                        cmdTongNo.Parameters.AddWithValue("@idKH", idKhachHang);
                        decimal tongNo = Convert.ToDecimal(cmdTongNo.ExecuteScalar());

                        var cmdDaThu = new SqlCommand(
                            "SELECT ISNULL(SUM(So_Tien_Thu),0) FROM PHIEU_THU WHERE id_Khach_Hang=@idKH",
                            conn, tran);
                        cmdDaThu.Parameters.AddWithValue("@idKH", idKhachHang);
                        decimal daThuTong = Convert.ToDecimal(cmdDaThu.ExecuteScalar());
                        decimal conLai = tongNo - daThuTong;

                        var cmdCheck = new SqlCommand(
                            "SELECT COUNT(*) FROM CONG_NO_KHACH_HANG WHERE id_Khach_Hang=@idKH",
                            conn, tran);
                        cmdCheck.Parameters.AddWithValue("@idKH", idKhachHang);
                        int exists = (int)cmdCheck.ExecuteScalar();

                        if (exists > 0)
                        {
                            var cmdUpd = new SqlCommand(@"
UPDATE CONG_NO_KHACH_HANG
SET Tong_No=@TongNo, Da_Thu=@DaThu, Ngay_Cap_Nhat=GETDATE()
WHERE id_Khach_Hang=@idKH", conn, tran);
                            cmdUpd.Parameters.AddWithValue("@TongNo", tongNo);
                            cmdUpd.Parameters.AddWithValue("@DaThu", daThuTong);
                            cmdUpd.Parameters.AddWithValue("@idKH", idKhachHang);
                            cmdUpd.ExecuteNonQuery();
                        }
                        else
                        {
                            var cmdInsCN = new SqlCommand(@"
INSERT INTO CONG_NO_KHACH_HANG (id_Khach_Hang, Tong_No, Da_Thu, Ngay_Cap_Nhat)
VALUES (@idKH, @TongNo, @DaThu, GETDATE())", conn, tran);
                            cmdInsCN.Parameters.AddWithValue("@idKH", idKhachHang);
                            cmdInsCN.Parameters.AddWithValue("@TongNo", tongNo);
                            cmdInsCN.Parameters.AddWithValue("@DaThu", daThuTong);
                            cmdInsCN.ExecuteNonQuery();
                        }

                        foreach (DataGridViewRow row in dgvCongNo.Rows)
                        {
                            if (!Convert.ToBoolean(row.Cells["colChon"].Value ?? false)) continue;
                            int idDonBan = int.Parse(row.Cells["colIdDonBan"].Value.ToString());

                            var cmdConLai = new SqlCommand(@"
SELECT dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu),0)
FROM DON_BAN_HANG dbh
LEFT JOIN PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
WHERE dbh.id = @idDBH
GROUP BY dbh.Tong_Thanh_Toan", conn, tran);
                            cmdConLai.Parameters.AddWithValue("@idDBH", idDonBan);
                            object res = cmdConLai.ExecuteScalar();
                            if (res != null && Convert.ToDecimal(res) <= 0)
                            {
                                var cmdSt = new SqlCommand(
                                    "UPDATE DON_BAN_HANG SET Trang_Thai=N'Đã thanh toán' WHERE id=@idDBH",
                                    conn, tran);
                                cmdSt.Parameters.AddWithValue("@idDBH", idDonBan);
                                cmdSt.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        MessageBox.Show(
                            "Thu tiền thành công!\n" +
                            "Tổng thu: " + tongThuThucTe.ToString("N0") + " đ\n" +
                            "Còn lại công nợ: " + conLai.ToString("N0") + " đ",
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDonBanCongNo(idKhachHang);
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thu tiền:\n" + ex.Message, "Lỗi");
            }
            finally
            {
                dangThuTien = false;
            }
        }

        private void BtnInPhieuThu_Click(object sender, EventArgs e)
        {
            var kh = cboKhachHang.SelectedItem as KhachHangItem;
            if (kh == null || kh.Id == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng trước khi in phiếu.", "Thiếu thông tin");
                return;
            }

            var printRows = new System.Collections.Generic.List<string[]>();
            decimal tongInRa = 0;

            foreach (DataGridViewRow row in dgvCongNo.Rows)
            {
                if (!Convert.ToBoolean(row.Cells["colChon"].Value ?? false)) continue;
                string rawSt = (row.Cells["colSoTienThu"].Value ?? "0").ToString()
                               .Replace(",", "").Replace(" đ", "").Trim();
                if (!decimal.TryParse(rawSt, out decimal soTienThu) || soTienThu <= 0) continue;

                decimal.TryParse((row.Cells["colConNoRaw"].Value ?? "0").ToString(), out decimal conNo);
                tongInRa += soTienThu;

                printRows.Add(new string[]
                {
                    row.Cells["colSoHD"].Value?.ToString()    ?? "",
                    row.Cells["colNgayHD"].Value?.ToString()  ?? "",
                    row.Cells["colSanPham"].Value?.ToString() ?? "",
                    conNo.ToString("N0") + " đ",
                    soTienThu.ToString("N0") + " đ"
                });
            }

            if (printRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 hóa đơn và nhập Số tiền thu > 0.", "Thiếu dữ liệu");
                return;
            }

            string phuongThuc = cboPhuongThuc.Text;
            string tieuDeChung, tieuDePhu;
            switch (phuongThuc)
            {
                case "Tiền mặt":
                    tieuDeChung = "PHIẾU THU TIỀN MẶT";
                    tieuDePhu = "Thu tiền mặt từ khách hàng";
                    break;
                case "Séc":
                    tieuDeChung = "PHIẾU THU BẰNG SÉC";
                    tieuDePhu = "Thu tiền bằng séc từ khách hàng";
                    break;
                default:
                    tieuDeChung = "PHIẾU THU CHUYỂN KHOẢN";
                    tieuDePhu = "Thu công nợ theo các hóa đơn đính kèm";
                    break;
            }

            string tenKH = kh.Ten;
            string ngayTT = dtpNgayThu.Value.ToString("dd/MM/yyyy");
            decimal tongSnapshot = tongInRa;

            var pd = new PrintDocument();
            pd.DefaultPageSettings.Landscape = true;
            pd.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);

            pd.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var fTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
                var fSub = new Font("Segoe UI", 10f, FontStyle.Italic);
                var fLabel = new Font("Segoe UI", 10f);
                var fBold = new Font("Segoe UI", 10f, FontStyle.Bold);
                var fCell = new Font("Segoe UI", 9.5f);
                var fTotal = new Font("Segoe UI", 12f, FontStyle.Bold);
                var fSign = new Font("Segoe UI", 9.5f);
                var fSignSub = new Font("Segoe UI", 8.5f, FontStyle.Italic);

                float pageW = ev.PageBounds.Width - 80f;
                float x = 40f, y = 40f;

                // Tiêu đề
                g.DrawString(tieuDeChung, fTitle, Brushes.Black, x, y); y += 32;
                g.DrawString(tieuDePhu, fSub, Brushes.Gray, x, y); y += 28;

                // Thông tin
                g.DrawString("Khách hàng:", fLabel, Brushes.Gray, x, y);
                g.DrawString(tenKH, fBold, Brushes.Black, x + 120, y); y += 20;
                g.DrawString("Ngày thu:", fLabel, Brushes.Gray, x, y);
                g.DrawString(ngayTT, fBold, Brushes.Black, x + 120, y); y += 20;
                g.DrawString("Phương thức:", fLabel, Brushes.Gray, x, y);
                g.DrawString(phuongThuc, fBold, Brushes.Black, x + 120, y); y += 26;

                g.DrawLine(new Pen(Color.FromArgb(180, 180, 210), 1f), x, y, x + pageW, y);
                y += 10;

                // Cột bảng
                float[] cX = { x, x + 105, x + 180, x + 430, x + 610 };
                float[] cW = { 105, 75, 250, 180, 160 };
                bool[] isRight = { false, false, false, true, true };
                string[] hd = { "Số HĐ", "Ngày HĐ", "Sản phẩm", "Còn nợ (đ)", "Số tiền thu (đ)" };

                // Header
                g.FillRectangle(new SolidBrush(Color.FromArgb(220, 230, 255)), x, y, pageW, 26);
                for (int i = 0; i < hd.Length; i++)
                {
                    var sf = new StringFormat
                    {
                        Alignment = isRight[i] ? StringAlignment.Far : StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(hd[i], fBold, Brushes.Black,
                        new RectangleF(cX[i] + 4, y, cW[i] - 8, 26), sf);
                }
                y += 26;

                // Dòng dữ liệu
                var blueBrush = new SolidBrush(Color.FromArgb(30, 80, 200));
                var altBG = new SolidBrush(Color.FromArgb(245, 248, 255));
                bool alt = false;

                foreach (var r in printRows)
                {
                    if (alt) g.FillRectangle(altBG, x, y, pageW, 22);
                    for (int i = 0; i < r.Length && i < cX.Length; i++)
                    {
                        var sf = new StringFormat
                        {
                            Alignment = isRight[i] ? StringAlignment.Far : StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        Brush br = (i == 4) ? blueBrush : Brushes.Black;
                        g.DrawString(r[i], fCell, br,
                            new RectangleF(cX[i] + 4, y, cW[i] - 8, 22), sf);
                    }
                    g.DrawLine(new Pen(Color.FromArgb(210, 215, 230), 0.5f),
                        x, y + 22, x + pageW, y + 22);
                    y += 22;
                    alt = !alt;
                }

                y += 12;
                g.DrawLine(new Pen(Color.FromArgb(100, 120, 200), 1.5f), x + 350, y, x + pageW, y);
                y += 8;

                g.DrawString("TỔNG TIỀN THU:", fTotal, Brushes.Black,
                    new RectangleF(x + 350, y, 200, 28),
                    new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                g.DrawString(tongSnapshot.ToString("N0") + " đ", fTotal, blueBrush,
                    new RectangleF(x + 350, y, pageW - 350, 28),
                    new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
                y += 38;

                // Chữ ký
                string[] signNames = { "Người lập phiếu", "Kế toán trưởng", "Giám đốc" };
                float signW = pageW / 3f;
                for (int i = 0; i < 3; i++)
                {
                    float sx = x + i * signW + signW / 2f - 55;
                    g.DrawString(signNames[i], fSign, Brushes.Black, sx, y);
                    g.DrawString("(Ký, ghi rõ họ tên)", fSignSub, Brushes.Gray, sx, y + 16);
                }

                fTitle.Dispose(); fSub.Dispose(); fLabel.Dispose(); fBold.Dispose();
                fCell.Dispose(); fTotal.Dispose(); fSign.Dispose(); fSignSub.Dispose();
                blueBrush.Dispose(); altBG.Dispose();
            };

            var preview = new PrintPreviewDialog
            {
                Document = pd,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            };
            preview.ShowDialog(this);
        }

        private string SinhMaPhieuThu(SqlConnection conn, SqlTransaction tran)
        {
            string prefix = "PT-" + DateTime.Today.Year + "-";
            var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Thu, LEN(@p)+1, 10) AS INT)), 0) + 1 " +
                "FROM PHIEU_THU WHERE Ma_Phieu_Thu LIKE @like", conn, tran);
            cmd.Parameters.AddWithValue("@p", prefix);
            cmd.Parameters.AddWithValue("@like", prefix + "%");
            return prefix + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D4");
        }

        private class KhachHangItem
        {
            public int Id { get; set; }
            public string Ten { get; set; }
            public override string ToString() { return Ten; }
        }
    }
}