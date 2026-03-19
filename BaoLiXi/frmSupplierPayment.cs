using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSupplierPayment : Form
    {
        private int _idNCC = 0;

        public frmSupplierPayment()
        {
            InitializeComponent();
            this.Load += frmSupplierPayment_Load;
        }

        private void frmSupplierPayment_Load(object sender, EventArgs e)
        {
            // Load phương thức thanh toán
            cboPhuongThuc.Items.Clear();
            cboPhuongThuc.Items.AddRange(new string[]
            {
                "Tiền mặt",
                "Ủy nhiệm chi",
                "Séc chuyển khoản",
                "Séc tiền mặt"
            });
            cboPhuongThuc.SelectedIndex = 0;

            dtpNgayTraTien.Value = DateTime.Today;

            SetupGrid();
            LoadNhaCungCap();
            TinhTongThanhToan();
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD NHÀ CUNG CẤP VÀO COMBOBOX
        // ═══════════════════════════════════════════════════════════════
        void LoadNhaCungCap()
        {
            cboNhaCungCap.SelectedIndexChanged -= cboNhaCungCap_SelectedIndexChanged;
            cboNhaCungCap.Items.Clear();
            cboNhaCungCap.Items.Add(new NCCItem()); // Dòng trống

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var rd = new SqlCommand(
                        "SELECT id, Ma_NCC, Ten_NCC FROM NHA_CUNG_CAP ORDER BY Ma_NCC",
                        conn).ExecuteReader();

                    while (rd.Read())
                        cboNhaCungCap.Items.Add(new NCCItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Ma = rd["Ma_NCC"].ToString(),
                            Ten = rd["Ten_NCC"].ToString()
                        });
                }
            }
            catch { }

            cboNhaCungCap.SelectedIndexChanged += cboNhaCungCap_SelectedIndexChanged;
            cboNhaCungCap.SelectedIndex = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // CHỌN NCC → LOAD DANH SÁCH ĐƠN HÀNG CÒN NỢ
        // ═══════════════════════════════════════════════════════════════
        private void cboNhaCungCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNhaCungCap.SelectedItem is NCCItem ncc && ncc.Id > 0)
            {
                _idNCC = ncc.Id;
                LoadDanhSachCongNo(ncc.Id);
            }
            else
            {
                _idNCC = 0;
                dgvChiTietThanhToan.Rows.Clear();
                TinhTongThanhToan();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ LOAD DANH SÁCH ĐƠN HÀNG CÒN NỢ CỦA NCC
        // ═══════════════════════════════════════════════════════════════
        void LoadDanhSachCongNo(int idNCC)
        {
            dgvChiTietThanhToan.Rows.Clear();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // ⭐ Query: Lấy đơn hàng còn nợ
                    string sql = @"
SELECT 
    dh.id AS idDonHang,
    dh.Ma_Don_Hang AS SoHoaDon,
    dh.Ngay_Dat_Hang AS NgayHD,
    N'Đơn hàng ' + dh.Ma_Don_Hang AS NoiDung,
    dh.Tong_Tien AS TongTien,
    ISNULL(
        (SELECT SUM(So_Tien) FROM THANH_TOAN_NCC WHERE id_Don_Dat_Hang = dh.id),
        0
    ) AS DaTra
FROM DON_DAT_HANG_NCC dh
WHERE dh.id_Nha_Cung_Cap = @idNCC
  AND dh.Tong_Tien > ISNULL(
      (SELECT SUM(So_Tien) FROM THANH_TOAN_NCC WHERE id_Don_Dat_Hang = dh.id),
      0
  )
ORDER BY dh.Ngay_Dat_Hang DESC";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@idNCC", idNCC);
                    var rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        int idDH = Convert.ToInt32(rd["idDonHang"]);
                        string soHD = rd["SoHoaDon"].ToString();
                        DateTime ngayHD = Convert.ToDateTime(rd["NgayHD"]);
                        string noiDung = rd["NoiDung"].ToString();
                        double tongTien = Convert.ToDouble(rd["TongTien"]);
                        double daTra = Convert.ToDouble(rd["DaTra"]);
                        double conNo = tongTien - daTra;

                        dgvChiTietThanhToan.Rows.Add(
                            false,                      // Checkbox
                            soHD,                       // Số HĐ
                            ngayHD.ToString("dd/MM/yyyy"), // Ngày
                            noiDung,                    // Nội dung
                            tongTien.ToString("N0"),    // Tổng tiền
                            daTra.ToString("N0"),       // Đã trả
                            conNo.ToString("N0"),       // Còn nợ
                            "0",                        // Số tiền trả (user nhập)
                            idDH,                       // Hidden
                            idNCC                       // Hidden
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load công nợ:\n{ex.Message}", "Lỗi");
            }

            TinhTongThanhToan();
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvChiTietThanhToan;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
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

            var ro = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 10f)
            };

            var editable = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(255, 255, 220), // Vàng nhạt
                ForeColor = Color.FromArgb(30, 80, 160),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            // Checkbox
            dgv.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colCheck",
                HeaderText = "✓",
                Width = 40,
                TrueValue = true,
                FalseValue = false
            });

            // Số hóa đơn
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoHD",
                HeaderText = "Số hóa đơn",
                Width = 110,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });

            // Ngày HĐ
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNgayHD",
                HeaderText = "Ngày HĐ",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Nội dung
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNoiDung",
                HeaderText = "Nội dung",
                Width = 200,
                ReadOnly = true,
                DefaultCellStyle = ro
            });

            // Tổng tiền
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Đã trả
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDaTra",
                HeaderText = "Đã trả",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(0, 120, 0)
                }
            });

            // Còn nợ (màu đỏ)
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConNo",
                HeaderText = "Còn nợ",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(220, 38, 38),
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });

            // Số tiền trả (editable)
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoTienTra",
                HeaderText = "Số tiền trả",
                Width = 140,
                ReadOnly = false,
                DefaultCellStyle = editable
            });

            // Hidden columns
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdDonHang",
                Width = 0,
                Visible = false
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdNCC",
                Width = 0,
                Visible = false
            });

            // Events
            dgv.CellValueChanged -= dgvChiTietThanhToan_CellValueChanged;
            dgv.CurrentCellDirtyStateChanged -= dgvChiTietThanhToan_CurrentCellDirtyStateChanged;

            dgv.CellValueChanged += dgvChiTietThanhToan_CellValueChanged;
            dgv.CurrentCellDirtyStateChanged += dgvChiTietThanhToan_CurrentCellDirtyStateChanged;
            dgv.CellEnter -= dgvChiTietThanhToan_CellEnter;
            dgv.CellEnter += dgvChiTietThanhToan_CellEnter;
        }
        private void dgvChiTietThanhToan_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvChiTietThanhToan.Columns[e.ColumnIndex].Name != "colSoTienTra") return;

            BeginInvoke(new Action(() =>
            {
                if (dgvChiTietThanhToan.EditingControl is TextBox tb)
                    tb.SelectAll();
            }));
        }
        // ═══════════════════════════════════════════════════════════════
        // ⭐ KHI THAY ĐỔI CHECKBOX → COMMIT NGAY
        // ═══════════════════════════════════════════════════════════════
        private void dgvChiTietThanhToan_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvChiTietThanhToan.IsCurrentCellDirty)
                dgvChiTietThanhToan.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ KHI THAY ĐỔI GIÁ TRỊ → TÍNH LẠI TỔNG
        // ═══════════════════════════════════════════════════════════════
        private void dgvChiTietThanhToan_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvChiTietThanhToan.Columns[e.ColumnIndex].Name;

            if (colName == "colCheck")
            {
                var row = dgvChiTietThanhToan.Rows[e.RowIndex];
                bool isChecked = row.Cells["colCheck"].Value != null &&
                                 (bool)row.Cells["colCheck"].Value;

                if (isChecked)
                {
                    string conNoStr = row.Cells["colConNo"].Value?
                                        .ToString().Replace(",", "") ?? "0";
                    row.Cells["colSoTienTra"].Value = conNoStr;
                    dgvChiTietThanhToan.CurrentCell = row.Cells["colSoTienTra"];
                }
                else
                {
                    row.Cells["colSoTienTra"].Value = "0";
                }
            }

            TinhTongThanhToan();
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ TÍNH TỔNG THANH TOÁN
        // ═══════════════════════════════════════════════════════════════
        void TinhTongThanhToan()
        {
            double tong = 0;

            foreach (DataGridViewRow row in dgvChiTietThanhToan.Rows)
            {
                bool isChecked = row.Cells["colCheck"].Value != null &&
                                 (bool)row.Cells["colCheck"].Value;

                if (isChecked)
                {
                    double.TryParse(
                        row.Cells["colSoTienTra"].Value?.ToString().Replace(",", ""),
                        out double soTienTra);

                    tong += soTienTra;
                }
            }

            lblTongThanhToan.Text = tong.ToString("N0") + " đ";
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT THANH TOÁN
        // ═══════════════════════════════════════════════════════════════
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (_idNCC == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn nhà cung cấp!", "Thiếu thông tin");
                return;
            }

            // Đếm số dòng được chọn
            int soDong = 0;
            foreach (DataGridViewRow row in dgvChiTietThanhToan.Rows)
            {
                bool isChecked = row.Cells["colCheck"].Value != null &&
                                 (bool)row.Cells["colCheck"].Value;
                if (isChecked) soDong++;
            }

            if (soDong == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn ít nhất 1 đơn hàng để thanh toán!",
                    "Thiếu thông tin");
                return;
            }

            // Xác nhận
            double tongTra = 0;
            double.TryParse(lblTongThanhToan.Text.Replace(" đ", "").Replace(",", ""),
                out tongTra);

            var rs = MessageBox.Show(
                $"Xác nhận thanh toán {soDong} đơn hàng?\n\n" +
                $"Tổng tiền: {tongTra:N0} đ\n" +
                $"Phương thức: {cboPhuongThuc.Text}",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs != DialogResult.Yes) return;

            // Thực hiện thanh toán
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int soThanhCong = 0;

                            foreach (DataGridViewRow row in dgvChiTietThanhToan.Rows)
                            {
                                bool isChecked = row.Cells["colCheck"].Value != null &&
                                                 (bool)row.Cells["colCheck"].Value;
                                if (!isChecked) continue;

                                int idDH = Convert.ToInt32(row.Cells["colIdDonHang"].Value);
                                double.TryParse(
                                    row.Cells["colSoTienTra"].Value?.ToString().Replace(",", ""),
                                    out double soTienTra);

                                if (soTienTra <= 0) continue;

                                // Sinh mã thanh toán
                                string maTT = SinhMaThanhToan(conn, tran);

                                // INSERT THANH_TOAN_NCC
                                string sqlIns = @"
INSERT INTO THANH_TOAN_NCC
    (Ma_Thanh_Toan, id_Nha_Cung_Cap, id_Don_Dat_Hang, 
     Ngay_Thanh_Toan, So_Tien, Phuong_Thuc, So_Chung_Tu, Ngay_Chung_Tu)
VALUES
    (@ma, @idNCC, @idDH, @ngay, @soTien, @phuongThuc, @soCT, @ngayCT)";

                                var cmdIns = new SqlCommand(sqlIns, conn, tran);
                                cmdIns.Parameters.AddWithValue("@ma", maTT);
                                cmdIns.Parameters.AddWithValue("@idNCC", _idNCC);
                                cmdIns.Parameters.AddWithValue("@idDH", idDH);
                                cmdIns.Parameters.AddWithValue("@ngay", dtpNgayTraTien.Value.Date);
                                cmdIns.Parameters.AddWithValue("@soTien", soTienTra);
                                cmdIns.Parameters.AddWithValue("@phuongThuc", cboPhuongThuc.Text);
                                cmdIns.Parameters.AddWithValue("@soCT", maTT);
                                cmdIns.Parameters.AddWithValue("@ngayCT", dtpNgayTraTien.Value.Date);
                                cmdIns.ExecuteNonQuery();

                                // ⭐ UPDATE CONG_NO_NCC
                                CapNhatCongNo(conn, tran, _idNCC, soTienTra);

                                soThanhCong++;
                            }

                            tran.Commit();

                            MessageBox.Show(
                                $"✅ Đã thanh toán thành công {soThanhCong} đơn hàng!\n\n" +
                                $"Tổng tiền: {tongTra:N0} đ",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            // Refresh
                            LoadDanhSachCongNo(_idNCC);
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
                MessageBox.Show($"❌ Lỗi thanh toán:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SINH MÃ THANH TOÁN: TT-2026-001
        // ═══════════════════════════════════════════════════════════════
        string SinhMaThanhToan(SqlConnection conn, SqlTransaction tran)
        {
            string prefix = "TT-" + DateTime.Today.Year + "-";
            int next = 1;

            var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Thanh_Toan, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM THANH_TOAN_NCC WHERE Ma_Thanh_Toan LIKE @like", conn, tran);
            cmd.Parameters.AddWithValue("@p", prefix);
            cmd.Parameters.AddWithValue("@like", prefix + "%");
            next = Convert.ToInt32(cmd.ExecuteScalar());

            return prefix + next.ToString("D3");
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ CẬP NHẬT CÔNG NỢ (UPSERT)
        // ═══════════════════════════════════════════════════════════════
        void CapNhatCongNo(SqlConnection conn, SqlTransaction tran, int idNCC, double soTienTra)
        {
            var cmdCheck = new SqlCommand(
                "SELECT id FROM CONG_NO_NCC WHERE id_Nha_Cung_Cap = @idNCC", conn, tran);
            cmdCheck.Parameters.AddWithValue("@idNCC", idNCC);
            var idCN = cmdCheck.ExecuteScalar();

            if (idCN == null)
            {
                // Nếu chưa có, tạo record mới: Tong_No = soTienTra, Da_Tra = soTienTra
                var cmdIns = new SqlCommand(@"
INSERT INTO CONG_NO_NCC (id_Nha_Cung_Cap, Tong_No, Da_Tra, Ngay_Cap_Nhat)
VALUES (@idNCC, @tongNo, @daTra, GETDATE())", conn, tran);
                cmdIns.Parameters.AddWithValue("@idNCC", idNCC);
                cmdIns.Parameters.AddWithValue("@tongNo", soTienTra);
                cmdIns.Parameters.AddWithValue("@daTra", soTienTra);
                cmdIns.ExecuteNonQuery();
            }
            else
            {
                var cmdUpd = new SqlCommand(@"
UPDATE CONG_NO_NCC
SET Da_Tra = Da_Tra + @soTienTra,
    Ngay_Cap_Nhat = GETDATE()
WHERE id_Nha_Cung_Cap = @idNCC", conn, tran);
                cmdUpd.Parameters.AddWithValue("@soTienTra", soTienTra);
                cmdUpd.Parameters.AddWithValue("@idNCC", idNCC);
                cmdUpd.ExecuteNonQuery();
            }
        }
        // ═══════════════════════════════════════════════════════════════
        // NÚT IN PHIẾU CHI
        // ═══════════════════════════════════════════════════════════════
        private void btnInPhieuChi_Click(object sender, EventArgs e)
        {
            if (_idNCC == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn nhà cung cấp trước khi in phiếu.", "Thiếu thông tin");
                return;
            }

            // Gom các dòng được chọn
            var rows = new System.Collections.Generic.List<DataGridViewRow>();
            double tongThanhToan = 0;

            foreach (DataGridViewRow row in dgvChiTietThanhToan.Rows)
            {
                bool isChecked = row.Cells["colCheck"].Value != null &&
                                 (bool)row.Cells["colCheck"].Value;
                if (!isChecked) continue;

                double.TryParse(
                    row.Cells["colSoTienTra"].Value?.ToString().Replace(",", ""),
                    out double soTienTra);

                if (soTienTra <= 0) continue;

                tongThanhToan += soTienTra;
                rows.Add(row);
            }

            if (rows.Count == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn ít nhất 1 hóa đơn và nhập Số tiền trả > 0.", "Thiếu dữ liệu");
                return;
            }

            // Tiêu đề theo phương thức thanh toán
            string phuongThuc = cboPhuongThuc.Text;
            string tieuDeChung;
            string tieuDePhu;

            if (phuongThuc == "Tiền mặt")
            {
                tieuDeChung = "PHIẾU CHI TIỀN MẶT";
                tieuDePhu = "Chi tiền cho nhà cung cấp";
            }
            else if (phuongThuc == "Ủy nhiệm chi")
            {
                tieuDeChung = "ỦY NHIỆM CHI TRẢ TIỀN NHÀ CUNG CẤP";
                tieuDePhu = "Thanh toán công nợ theo các hóa đơn đính kèm";
            }
            else
            {
                tieuDeChung = "PHIẾU CHI THANH TOÁN BẰNG SÉC";
                tieuDePhu = "Thanh toán nhà cung cấp";
            }

            // Lấy tên NCC từ combobox
            string tenNCC = "";
            if (cboNhaCungCap.SelectedItem is NCCItem ncc && ncc.Id > 0)
                tenNCC = ncc.Ten;
            string ngayTT = dtpNgayTraTien.Value.ToString("dd/MM/yyyy");

            // Chuẩn bị dữ liệu dòng chi tiết để in
            var lines = new System.Collections.Generic.List<string[]>();
            foreach (var row in rows)
            {
                lines.Add(new string[]
                {
            row.Cells["colSoHD"].Value?.ToString() ?? "",
            row.Cells["colNgayHD"].Value?.ToString() ?? "",
            row.Cells["colNoiDung"].Value?.ToString() ?? "",
            row.Cells["colConNo"].Value?.ToString() ?? "0",
            row.Cells["colSoTienTra"].Value?.ToString() ?? "0"
                });
            }

            // In bằng PrintPreviewDialog
            var pd = new System.Drawing.Printing.PrintDocument();
            pd.DefaultPageSettings.Landscape = true;

            pd.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var fTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
                var fSub = new Font("Segoe UI", 9.5f, FontStyle.Italic);
                var fH = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fN = new Font("Segoe UI", 9f);
                var fS = new Font("Segoe UI", 8f);

                int x = 50, y = 40;

                // Tiêu đề
                g.DrawString(tieuDeChung, fTitle, Brushes.Black, x, y);
                y += 26;
                g.DrawString(tieuDePhu, fSub, Brushes.Gray, x, y);
                y += 22;

                // Thông tin chung
                g.DrawString($"Nhà cung cấp: {tenNCC}", fN, Brushes.Black, x, y); y += 18;
                g.DrawString($"Ngày thanh toán: {ngayTT}", fN, Brushes.Black, x, y); y += 18;
                g.DrawString($"Phương thức thanh toán: {phuongThuc}", fN, Brushes.Black, x, y); y += 18;

                g.DrawLine(Pens.LightGray, x, y, x + 720, y);
                y += 12;

                // Header bảng chi tiết
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 235, 245)), x, y, 720, 22);
                int[] colX = { 4, 90, 170, 460, 580 };
                string[] headers = { "Số HĐ", "Ngày HĐ", "Nội dung", "Còn nợ", "Số tiền trả" };

                for (int i = 0; i < headers.Length; i++)
                    g.DrawString(headers[i], fH, Brushes.Black, x + colX[i], y + 3);

                y += 24;

                // Dòng chi tiết
                foreach (var r in lines)
                {
                    for (int i = 0; i < r.Length && i < colX.Length; i++)
                    {
                        var text = r[i];
                        if (i >= 3)
                            g.DrawString(text, fS, Brushes.Black,
                                x + colX[i], y + 2,
                                new StringFormat { Alignment = StringAlignment.Far });
                        else
                            g.DrawString(text, fS, Brushes.Black, x + colX[i], y + 2);
                    }
                    g.DrawLine(Pens.Gainsboro, x, y + 18, x + 720, y + 18);
                    y += 20;
                }

                y += 10;
                g.DrawLine(Pens.Gray, x + 400, y, x + 720, y);
                y += 6;

                g.DrawString("TỔNG THANH TOÁN:", fH, Brushes.Black, x + 400, y);
                g.DrawString(tongThanhToan.ToString("N0") + " đ",
                    fH, new SolidBrush(Color.FromArgb(30, 80, 200)),
                    x + 720, y,
                    new StringFormat { Alignment = StringAlignment.Far });
                y += 30;

                // Chữ ký
                g.DrawString("Người lập phiếu", fN, Brushes.Black, x + 80, y);
                g.DrawString("Kế toán trưởng", fN, Brushes.Black, x + 300, y);
                g.DrawString("Giám đốc", fN, Brushes.Black, x + 550, y);
                y += 14;
                g.DrawString("(Ký, ghi rõ họ tên)", fS, Brushes.Gray, x + 80, y);
                g.DrawString("(Ký, ghi rõ họ tên)", fS, Brushes.Gray, x + 300, y);
                g.DrawString("(Ký, ghi rõ họ tên)", fS, Brushes.Gray, x + 550, y);

                // Giải phóng font
                fTitle.Dispose(); fSub.Dispose(); fH.Dispose(); fN.Dispose(); fS.Dispose();
            };

            var preview = new PrintPreviewDialog
            {
                Document = pd,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            };
            preview.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT LÀM MỚI
        // ═══════════════════════════════════════════════════════════════
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            _idNCC = 0;
            cboNhaCungCap.SelectedIndex = 0;
            cboPhuongThuc.SelectedIndex = 0;
            dtpNgayTraTien.Value = DateTime.Today;
            dgvChiTietThanhToan.Rows.Clear();
            lblTongThanhToan.Text = "0 đ";
        }

        // ═══════════════════════════════════════════════════════════════
        // NCC ITEM CLASS
        // ═══════════════════════════════════════════════════════════════
        private class NCCItem
        {
            public int Id { get; set; }
            public string Ma { get; set; } = "";
            public string Ten { get; set; } = "-- Chọn nhà cung cấp --";
            public override string ToString() => string.IsNullOrEmpty(Ma) ? Ten : $"{Ma} - {Ten}";
        }
    }
}