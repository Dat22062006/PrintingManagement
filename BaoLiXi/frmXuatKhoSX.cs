// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmXuatKhoSX.cs - XUẤT KHO SẢN XUẤT                               ║
// ╚══════════════════════════════════════════════════════════════════════╝

using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmXuatKhoSX : Form
    {
        private int _idLenhSX = 0;
        private int _idPhieuXuat = 0;   // > 0 sau khi Lưu
        private bool _daXacNhan = false;

        // Cache NGUYEN_LIEU
        private DataTable _dtNguyenLieu = new DataTable();

        public frmXuatKhoSX()
        {
            InitializeComponent();
            this.Load += frmXuatKhoSX_Load;
        }

        private void frmXuatKhoSX_Load(object sender, EventArgs e)
        {
            LoadNguyenLieuCache();
            SetupGrid();
            SetupStaticCombos();
            LoadcboLenhSX();
            txtSoPhieuXuatt.Text = SinhMaPhieuXuat();
            dtpNgayXuatt.Value = DateTime.Today;
            txtLyDoo.Text = "Xuất vật tư phục vụ sản xuất";
            UpdateTong();
            btnXacNhan.Visible = false;
        }

        // ═══════════════════════════════════════════════════════════════
        // CACHE NGUYÊN LIỆU
        // ═══════════════════════════════════════════════════════════════
        void LoadNguyenLieuCache()
        {
            _dtNguyenLieu.Clear();
            if (_dtNguyenLieu.Columns.Count > 0)
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string sql = @"
SELECT id, Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh,
       Gia_Nhap, Ton_Kho
FROM NGUYEN_LIEU
ORDER BY Ma_Nguyen_Lieu";
                        var da = new SqlDataAdapter(sql, conn);
                        da.Fill(_dtNguyenLieu);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi load nguyên liệu:\n{ex.Message}", "Lỗi");
                }
                return;
            }
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT id, Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh,
       Gia_Nhap, Ton_Kho
FROM NGUYEN_LIEU
ORDER BY Ma_Nguyen_Lieu";
                    var da = new SqlDataAdapter(sql, conn);
                    da.Fill(_dtNguyenLieu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load nguyên liệu:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvVatTuu;
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
            dgv.RowTemplate.Height = 38;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 4, 0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            var right = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f),
                Padding = new Padding(0, 0, 8, 0)
            };
            var center = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f)
            };
            var readonlyStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 10f)
            };
            var readonlyRight = new DataGridViewCellStyle(readonlyStyle)
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 5,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(center) { BackColor = Color.FromArgb(248, 249, 250) }
            });

            var colMa = new DataGridViewComboBoxColumn
            {
                Name = "colMaHang",
                HeaderText = "Mã hàng",
                FillWeight = 12,
                DisplayMember = "Ma_Nguyen_Lieu",
                ValueMember = "Ma_Nguyen_Lieu",
                DataSource = _dtNguyenLieu,
                FlatStyle = FlatStyle.Flat
            };
            dgv.Columns.Add(colMa);

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTenHang",
                HeaderText = "Tên hàng",
                FillWeight = 25,
                ReadOnly = true,
                DefaultCellStyle = readonlyStyle
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDVT",
                HeaderText = "ĐVT",
                FillWeight = 8,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(readonlyStyle)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTonKho",
                HeaderText = "Tồn kho",
                FillWeight = 10,
                ReadOnly = true,
                DefaultCellStyle = readonlyRight
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSLXuat",
                HeaderText = "SL xuất",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle(right)
                {
                    BackColor = Color.FromArgb(255, 251, 220)
                }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonGia",
                HeaderText = "Đơn giá",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle(right)
                {
                    BackColor = Color.FromArgb(255, 251, 220)
                }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colThanhTien",
                HeaderText = "Thành tiền",
                FillWeight = 14,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(right)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(37, 99, 235)
                }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdNL", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGiaRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSLRaw", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTonRaw", Visible = false });

            dgv.CellValueChanged += dgv_CellValueChanged;
            dgv.EditingControlShowing += dgv_EditingControlShowing;
            dgv.CellClick += dgv_CellClick;
            dgv.CurrentCellDirtyStateChanged += dgv_CurrentCellDirtyStateChanged;
        }

        // ═══════════════════════════════════════════════════════════════
        // STATIC COMBOS (đã fix KHÔNG nhân đôi)
        // ═══════════════════════════════════════════════════════════════
        void SetupStaticCombos()
        {
            cboLoaiXuatt.Items.Clear();
            cboBoPhann.Items.Clear();

            cboLoaiXuatt.Items.AddRange(new object[]
            {
                "Xuất cho sản xuất",
                "Xuất bán hàng",
                "Xuất khác"
            });
            cboLoaiXuatt.SelectedIndex = 0;

            cboBoPhann.Items.AddRange(new object[]
            {
                "Sản xuất",
                "In ấn",
                "Gia công",
                "Đóng gói"
            });
            cboBoPhann.SelectedIndex = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD COMBOBOX LỆNH SX
        // ═══════════════════════════════════════════════════════════════
        void LoadcboLenhSX()
        {
            cboLenhSXX.SelectedIndexChanged -= cboLenhSXXXX_SelectedIndexChanged;
            cboLenhSXX.Items.Clear();
            cboLenhSXX.Items.Add(new LenhSXItem());

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT lsx.id, lsx.Ma_Lenh_SX, lsx.Ten_San_Pham,
       lsx.So_Luong, lsx.Trang_Thai,
       kh.Ten_Khach_Hang
FROM LENH_SAN_XUAT lsx
LEFT JOIN KHACH_HANG kh ON kh.id = lsx.id_Khach_Hang
WHERE lsx.Trang_Thai = N'Đang sản xuất'
ORDER BY lsx.id DESC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        cboLenhSXX.Items.Add(new LenhSXItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Ma = rd["Ma_Lenh_SX"].ToString(),
                            TenSanPham = rd["Ten_San_Pham"].ToString(),
                            SoLuong = Convert.ToInt32(rd["So_Luong"]),
                            TenKH = rd["Ten_Khach_Hang"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi load lệnh SX:\n{ex.Message}", "Lỗi");
            }

            cboLenhSXX.SelectedIndexChanged += cboLenhSXXXX_SelectedIndexChanged;
            cboLenhSXX.SelectedIndex = 0;
        }

        private void cboLenhSXXXX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboLenhSXX.SelectedItem is LenhSXItem lsx) || lsx.Id == 0)
            {
                txtKhachHangg.Text = "";
                txtSanPhamm.Text = "";
                txtSoLuongSXX.Text = "";
                _idLenhSX = 0;
                return;
            }

            _idLenhSX = lsx.Id;
            txtKhachHangg.Text = lsx.TenKH;
            txtSanPhamm.Text = lsx.TenSanPham;
            txtSoLuongSXX.Text = lsx.SoLuong.ToString("N0") + " cái";
        }

        // ═══════════════════════════════════════════════════════════════
        // THÊM DÒNG
        // ═══════════════════════════════════════════════════════════════
        private void btnThemVatTuu_Click(object sender, EventArgs e)
        {
            int stt = dgvVatTuu.Rows.Count + 1;
            dgvVatTuu.Rows.Add(stt, "", "", "", "0", "0", "0", "0", 0, 0, 0, 0);
        }

        // ═══════════════════════════════════════════════════════════════
        // GRID EVENTS
        // ═══════════════════════════════════════════════════════════════
        private void dgv_EditingControlShowing(object sender,
            DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvVatTuu.CurrentCell?.OwningColumn.Name is "colSLXuat" or "colDonGia")
            {
                if (e.Control is TextBox tb)
                    tb.SelectAll();
            }
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvVatTuu.CurrentCell?.OwningColumn.Name == "colMaHang")
                dgvVatTuu.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvVatTuu.Columns[e.ColumnIndex].Name != "colMaHang") return;

            dgvVatTuu.BeginEdit(true);
            if (dgvVatTuu.EditingControl is ComboBox cbo)
                cbo.DroppedDown = true;
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvVatTuu.Rows[e.RowIndex];
            string col = dgvVatTuu.Columns[e.ColumnIndex].Name;

            if (col == "colMaHang")
            {
                string ma = row.Cells["colMaHang"].Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(ma)) return;

                var nlRows = _dtNguyenLieu.Select($"Ma_Nguyen_Lieu = '{ma}'");
                if (nlRows.Length == 0) return;

                var nl = nlRows[0];
                double tonKho = Convert.ToDouble(nl["Ton_Kho"]);
                double donGia = Convert.ToDouble(nl["Gia_Nhap"]);
                int idNL = Convert.ToInt32(nl["id"]);

                row.Cells["colTenHang"].Value = nl["Ten_Nguyen_Lieu"].ToString();
                row.Cells["colDVT"].Value = nl["Don_Vi_Tinh"].ToString();
                row.Cells["colTonKho"].Value = tonKho.ToString("N2");
                row.Cells["colDonGia"].Value = donGia.ToString("N0");
                row.Cells["colIdNL"].Value = idNL;
                row.Cells["colDonGiaRaw"].Value = donGia;
                row.Cells["colTonRaw"].Value = tonKho;

                row.Cells["colTonKho"].Style.ForeColor =
                    tonKho <= 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(22, 163, 74);
            }

            if (col == "colSLXuat" || col == "colDonGia")
            {
                TinhThanhTien(row);
                UpdateTong();
            }
        }

        void TinhThanhTien(DataGridViewRow row)
        {
            string slStr = row.Cells["colSLXuat"].Value?.ToString() ?? "0";
            double.TryParse(slStr.Replace(",", "").Replace(".", ""), out double sl);

            double.TryParse(row.Cells["colDonGiaRaw"].Value?.ToString(), out double dg);

            double tt = sl * dg;
            row.Cells["colSLRaw"].Value = sl;
            row.Cells["colThanhTien"].Value = tt > 0 ? tt.ToString("N0") : "";
        }

        void UpdateTong()
        {
            double tong = 0;
            foreach (DataGridViewRow row in dgvVatTuu.Rows)
            {
                double.TryParse(
                    row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""),
                    out double tt);
                tong += tt;
            }
            lblTongGiaTri.Text = $"TỔNG GIÁ TRỊ XUẤT:  {tong:N0} đ";
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDATE
        // ═══════════════════════════════════════════════════════════════
        bool Validate()
        {
            if (_idLenhSX == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn số lệnh sản xuất!", "Thiếu thông tin");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNguoiNhann.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập tên người nhận!", "Thiếu thông tin");
                txtNguoiNhann.Focus();
                return false;
            }
            if (dgvVatTuu.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Vui lòng thêm ít nhất 1 vật tư!", "Thiếu thông tin");
                return false;
            }
            foreach (DataGridViewRow row in dgvVatTuu.Rows)
            {
                string ma = row.Cells["colMaHang"].Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(ma))
                {
                    MessageBox.Show($"⚠️ Dòng {row.Index + 1}: Chưa chọn mã hàng!", "Thiếu thông tin");
                    return false;
                }
                double.TryParse(row.Cells["colSLRaw"].Value?.ToString(), out double sl);
                if (sl <= 0)
                {
                    MessageBox.Show($"⚠️ Dòng {row.Index + 1}: Số lượng xuất phải > 0!", "Thiếu thông tin");
                    return false;
                }
                double.TryParse(row.Cells["colTonRaw"].Value?.ToString(), out double ton);
                if (sl > ton)
                {
                    string ten = row.Cells["colTenHang"].Value?.ToString();
                    MessageBox.Show(
                        $"⚠️ Vật tư '{ten}': Số lượng xuất ({sl:N2}) vượt quá tồn kho ({ton:N2})!",
                        "Không đủ tồn kho");
                    return false;
                }
            }
            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT LƯU PHIẾU XUẤT
        // ═══════════════════════════════════════════════════════════════
        private void btnLuuu_Click(object sender, EventArgs e)
        {
            dgvVatTuu.EndEdit();

            foreach (DataGridViewRow row in dgvVatTuu.Rows)
                TinhThanhTien(row);

            if (!Validate()) return;

            var rs = MessageBox.Show(
                "Lưu phiếu xuất và trừ tồn kho ngay?\n\nThao tác này không thể hoàn tác!",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
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
                            string sqlIns = @"
INSERT INTO PHIEU_XUAT_KHO_SX
    (Ma_Phieu_Xuat, id_Lenh_San_Xuat, Ngay_Xuat, Nguoi_Xuat, Trang_Thai)
VALUES
    (@ma, @idLSX, @ngay, @nguoi, N'Hoàn thành');
SELECT SCOPE_IDENTITY();";

                            var cmd = new SqlCommand(sqlIns, conn, tran);
                            cmd.Parameters.AddWithValue("@ma", txtSoPhieuXuatt.Text.Trim());
                            cmd.Parameters.AddWithValue("@idLSX", _idLenhSX);
                            cmd.Parameters.AddWithValue("@ngay", dtpNgayXuatt.Value.Date);
                            cmd.Parameters.AddWithValue("@nguoi", txtNguoiNhann.Text.Trim());
                            _idPhieuXuat = Convert.ToInt32(cmd.ExecuteScalar());

                            foreach (DataGridViewRow row in dgvVatTuu.Rows)
                            {
                                int idNL = Convert.ToInt32(row.Cells["colIdNL"].Value ?? 0);
                                double.TryParse(row.Cells["colSLRaw"].Value?.ToString(), out double sl);
                                double.TryParse(row.Cells["colDonGiaRaw"].Value?.ToString(), out double dg);
                                double tt = sl * dg;

                                var cmdCT = new SqlCommand(@"
INSERT INTO CHI_TIET_XUAT_KHO_SX
    (id_Phieu_Xuat, id_Nguyen_Lieu, So_Luong_Xuat, Don_Gia, Thanh_Tien)
VALUES (@idPX, @idNL, @sl, @dg, @tt)", conn, tran);
                                cmdCT.Parameters.AddWithValue("@idPX", _idPhieuXuat);
                                cmdCT.Parameters.AddWithValue("@idNL", idNL);
                                cmdCT.Parameters.AddWithValue("@sl", sl);
                                cmdCT.Parameters.AddWithValue("@dg", dg);
                                cmdCT.Parameters.AddWithValue("@tt", tt);
                                cmdCT.ExecuteNonQuery();

                                var cmdUpd = new SqlCommand(@"
UPDATE NGUYEN_LIEU
SET Ton_Kho = Ton_Kho - @sl
WHERE id = @id AND Ton_Kho >= @sl", conn, tran);
                                cmdUpd.Parameters.AddWithValue("@sl", sl);
                                cmdUpd.Parameters.AddWithValue("@id", idNL);
                                int affected = cmdUpd.ExecuteNonQuery();

                                if (affected == 0)
                                {
                                    tran.Rollback();
                                    string ten = row.Cells["colTenHang"].Value?.ToString();
                                    MessageBox.Show(
                                        $"❌ Vật tư '{ten}' không đủ tồn kho!\nGiao dịch đã bị hủy.",
                                        "Lỗi tồn kho");
                                    return;
                                }
                            }

                            tran.Commit();

                            MessageBox.Show(
                                $"✅ Lưu và xuất kho thành công!\nSố phiếu: {txtSoPhieuXuatt.Text}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            btnLuuu.Enabled = false;
                            btnXacNhan.Enabled = false;
                            btnXacNhan.Text = "✅ Đã xuất kho";

                            LoadNguyenLieuCache();
                            RefreshTonKhoGrid();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT XÁC NHẬN (nếu dùng riêng)
        // ═══════════════════════════════════════════════════════════════
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // (giữ nguyên như bạn đang dùng, hoặc có thể bỏ nếu đã trừ tồn ở Lưu)
        }

        void RefreshTonKhoGrid()
        {
            foreach (DataGridViewRow row in dgvVatTuu.Rows)
            {
                int idNL = Convert.ToInt32(row.Cells["colIdNL"].Value ?? 0);
                if (idNL == 0) continue;

                var nlRows = _dtNguyenLieu.Select($"id = {idNL}");
                if (nlRows.Length > 0)
                {
                    double newTon = Convert.ToDouble(nlRows[0]["Ton_Kho"]);
                    row.Cells["colTonKho"].Value = newTon.ToString("N2");
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT IN PHIẾU XUẤT (EXCEL) – đã chỉnh theo mẫu
        // ═══════════════════════════════════════════════════════════════
        private void btnInn_Click(object sender, EventArgs e)
        {
            if (dgvVatTuu.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Không có dữ liệu để in phiếu xuất kho!", "Thiếu dữ liệu");
                return;
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage())
                {
                    var ws = pkg.Workbook.Worksheets.Add("PhieuXuatKho");
                    int r = 1;

                    ws.Cells[r, 1].Value = "CÔNG TY TNHH SX THƯƠNG MẠI DỊCH VỤ AN LÂM";
                    ws.Cells[r, 1, r, 5].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 11;
                    r++;
                    ws.Cells[r, 1].Value = "51/10/3 Hòa Bình, Phường Tân Phú, TP.HCM";
                    ws.Cells[r, 1, r, 5].Merge = true;
                    ws.Cells[r, 1].Style.Font.Size = 9;

                    ws.Cells[1, 7].Value = "Mẫu số: 02 - VT";
                    ws.Cells[1, 7].Style.Font.Size = 9;
                    ws.Cells[2, 7].Value = "(Theo TT 200/2014/TT-BTC)";
                    ws.Cells[2, 7].Style.Font.Size = 8;
                    r += 2;

                    ws.Cells[r, 1].Value = "PHIẾU XUẤT KHO";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.Font.Size = 14;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;

                    DateTime d = dtpNgayXuatt.Value.Date;
                    ws.Cells[r, 1].Value = $"Ngày {d:dd} tháng {d:MM} năm {d:yyyy}";
                    ws.Cells[r, 1, r, 8].Merge = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[r, 1].Style.Font.Italic = true;
                    r++;

                    ws.Cells[r, 5].Value = $"Số: {txtSoPhieuXuatt.Text}";
                    r++;

                    ws.Cells[r, 1].Value = "Họ và tên người nhận hàng:";
                    ws.Cells[r, 2, r, 8].Merge = true;
                    ws.Cells[r, 2].Value = txtNguoiNhann.Text;
                    r++;

                    ws.Cells[r, 1].Value = "Địa chỉ (bộ phận):";
                    ws.Cells[r, 2, r, 8].Merge = true;
                    ws.Cells[r, 2].Value = cboBoPhann.SelectedItem?.ToString();
                    r++;

                    ws.Cells[r, 1].Value = "Lý do xuất kho:";
                    ws.Cells[r, 2, r, 8].Merge = true;
                    ws.Cells[r, 2].Value = txtLyDoo.Text;
                    r++;

                    ws.Cells[r, 1].Value = "Xuất tại kho (bộ phận kho):";
                    ws.Cells[r, 2, r, 8].Merge = true;
                    ws.Cells[r, 2].Value = "Nguyên vật liệu giấy";
                    r += 2;

                    string[] headers = {
                        "STT",
                        "Tên, nhãn hiệu, quy cách vật tư",
                        "Mã số",
                        "ĐVT",
                        "Số lượng thực xuất",
                        "Đơn giá",
                        "Thành tiền"
                    };

                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cells[r, c + 1].Value = headers[c];
                        ws.Cells[r, c + 1].Style.Font.Bold = true;
                        ws.Cells[r, c + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 224, 180));
                        ws.Cells[r, c + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[r, c + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[r, c + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    r++;

                    int stt = 1;
                    double tongTien = 0;

                    foreach (DataGridViewRow gridRow in dgvVatTuu.Rows)
                    {
                        string ten = gridRow.Cells["colTenHang"].Value?.ToString() ?? "";
                        string ma = gridRow.Cells["colMaHang"].Value?.ToString() ?? "";
                        string dvt = gridRow.Cells["colDVT"].Value?.ToString() ?? "";

                        double.TryParse(gridRow.Cells["colSLRaw"].Value?.ToString(), out double sl);
                        double.TryParse(gridRow.Cells["colDonGiaRaw"].Value?.ToString(), out double dg);
                        double thanhTien = sl * dg;
                        tongTien += thanhTien;

                        ws.Cells[r, 1].Value = stt++;
                        ws.Cells[r, 2].Value = ten;
                        ws.Cells[r, 3].Value = ma;
                        ws.Cells[r, 4].Value = dvt;
                        ws.Cells[r, 5].Value = sl;
                        ws.Cells[r, 6].Value = dg;
                        ws.Cells[r, 7].Value = thanhTien;

                        ws.Cells[r, 5].Style.Numberformat.Format = "#,##0.###";
                        ws.Cells[r, 6].Style.Numberformat.Format = "#,##0";
                        ws.Cells[r, 7].Style.Numberformat.Format = "#,##0";

                        for (int c = 1; c <= 7; c++)
                        {
                            ws.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells[r, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }

                        r++;
                    }

                    ws.Cells[r, 1].Value = "Tổng cộng";
                    ws.Cells[r, 1, r, 6].Merge = true;
                    ws.Cells[r, 1].Style.Font.Bold = true;
                    ws.Cells[r, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[r, 7].Value = tongTien;
                    ws.Cells[r, 7].Style.Numberformat.Format = "#,##0";
                    ws.Cells[r, 7].Style.Font.Bold = true;
                    ws.Cells[r, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    r += 2;

                    ws.Cells[r, 1].Value = "Người lập phiếu";
                    ws.Cells[r, 3].Value = "Người nhận hàng";
                    ws.Cells[r, 5].Value = "Thủ kho";
                    ws.Cells[r, 7].Value = "Kế toán trưởng";
                    ws.Cells[r, 1, r, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r++;
                    ws.Cells[r, 1].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 3].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 5].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 7].Value = "(Ký, ghi rõ họ tên)";
                    ws.Cells[r, 1, r, 7].Style.Font.Italic = true;
                    ws.Cells[r, 1, r, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Column(1).Width = 6;
                    ws.Column(2).Width = 35;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 10;
                    ws.Column(5).Width = 18;
                    ws.Column(6).Width = 18;
                    ws.Column(7).Width = 20;

                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        $"PHIEU_XUAT_KHO_{txtSoPhieuXuatt.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    pkg.SaveAs(new FileInfo(path));

                    MessageBox.Show($"✅ Đã xuất phiếu xuất kho ra Excel:\n{path}", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // HỦY / RESET
        // ═══════════════════════════════════════════════════════════════
        private void btnHuyyy_Click(object sender, EventArgs e)
        {
            if (dgvVatTuu.Rows.Count > 0 || _idLenhSX > 0)
            {
                if (MessageBox.Show("Bạn có chắc muốn hủy bỏ?", "Xác nhận",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }
            ResetForm();
        }

        void ResetForm()
        {
            _idLenhSX = 0;
            _idPhieuXuat = 0;
            _daXacNhan = false;

            cboLenhSXX.SelectedIndex = 0;
            txtKhachHangg.Text = "";
            txtSanPhamm.Text = "";
            txtSoLuongSXX.Text = "";
            txtNguoiNhann.Text = "";
            txtLyDoo.Text = "Xuất vật tư phục vụ sản xuất";
            cboLoaiXuatt.SelectedIndex = 0;
            cboBoPhann.SelectedIndex = 0;
            dtpNgayXuatt.Value = DateTime.Today;
            txtSoPhieuXuatt.Text = SinhMaPhieuXuat();
            dgvVatTuu.Rows.Clear();
            lblTongGiaTri.Text = "TỔNG GIÁ TRỊ XUẤT:  0 đ";

            btnLuuu.Enabled = true;
            btnXacNhan.Enabled = false;
            btnXacNhan.Text = "✅ Xác nhận xuất kho";

            LoadNguyenLieuCache();
            if (dgvVatTuu.Columns["colMaHang"] is DataGridViewComboBoxColumn col)
                col.DataSource = _dtNguyenLieu;
        }

        // ═══════════════════════════════════════════════════════════════
        // SINH MÃ PHIẾU XUẤT
        // ═══════════════════════════════════════════════════════════════
        string SinhMaPhieuXuat()
        {
            string prefix = "PX-" + DateTime.Today.Year + "-";
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Xuat, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM PHIEU_XUAT_KHO_SX WHERE Ma_Phieu_Xuat LIKE @like", conn);
                    cmd.Parameters.AddWithValue("@p", prefix);
                    cmd.Parameters.AddWithValue("@like", prefix + "%");
                    int next = Convert.ToInt32(cmd.ExecuteScalar());
                    return prefix + next.ToString("D3");
                }
            }
            catch { return prefix + "001"; }
        }

        private class LenhSXItem
        {
            public int Id { get; set; }
            public string Ma { get; set; } = "";
            public string TenSanPham { get; set; } = "";
            public string TenKH { get; set; } = "";
            public int SoLuong { get; set; }
            public override string ToString()
            {
                if (Id == 0) return "-- Chọn lệnh sản xuất --";
                return $"{Ma} – {TenSanPham} ({SoLuong:N0} cái)";
            }
        }
    }
}