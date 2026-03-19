using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPurchaseOrder : Form
    {
        private int _idNCC = 0;
        private int _idDonHangDaLuu = 0;
        private string _fileDinhKem = "";
        private bool _daLuu = false;

        // ⭐ Preload khi mở từ frmInventoryIssue
        private int _preloadIdNL = 0;
        private string _preloadMaHang = "";
        private string _preloadTenHang = "";

        public frmPurchaseOrder()
        {
            InitializeComponent();
            this.Load += frmPurchaseOrder_Load;
        }

        // ⭐ Constructor gọi từ frmInventoryIssue (nút "Tạo đơn mua" vật tư sắp hết)
        public frmPurchaseOrder(int idNL, string maHang, string tenHang) : this()
        {
            _preloadIdNL = idNL;
            _preloadMaHang = maHang;
            _preloadTenHang = tenHang;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD FORM
        // ═══════════════════════════════════════════════════════════════
        private void frmPurchaseOrder_Load(object sender, EventArgs e)
        {
            cboTinhTrang.Items.Clear();
            cboTinhTrang.Items.AddRange(new string[] { "Chưa thực hiện", "Đang thực hiện", "Hoàn thành", "Hủy" });
            cboTinhTrang.SelectedIndex = 0;

            cboDieuKhoan.Items.Clear();
            cboDieuKhoan.Items.AddRange(new string[] { "Tiền mặt", "Ủy nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboDieuKhoan.SelectedIndex = 0;

            dtpNgayDatHang.Value = DateTime.Today;
            dtpNgayGiaoHang.Value = DateTime.Today;

            SetupGrid();
            LoadNhaCungCap();
            SinhSoDonHang();
            TinhTong();

            // ⭐ Nếu mở từ frmInventoryIssue → tự điền dòng vật tư sắp hết vào grid
            if (_preloadIdNL > 0)
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        var cmd = new SqlCommand(
                            "SELECT Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap FROM NGUYEN_LIEU WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", _preloadIdNL);
                        var rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            string ma = rd["Ma_Nguyen_Lieu"].ToString();
                            string ten = rd["Ten_Nguyen_Lieu"].ToString();
                            string dvt = rd["Don_Vi_Tinh"].ToString();
                            double gia = Convert.ToDouble(rd["Gia_Nhap"]);
                            dgvChiTiet.Rows.Add("", ma, ten, dvt, "0", gia.ToString("N0"), "0", 10, "0", _preloadIdNL);
                            TinhTong();
                        }
                    }
                }
                catch { }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SINH SỐ ĐƠN HÀNG
        // ═══════════════════════════════════════════════════════════════
        void SinhSoDonHang()
        {
            string prefix = "DH-" + DateTime.Today.Year + "-";
            int next = 1;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Don_Hang, LEN(@p)+1, 10) AS INT)), 0) + 1
FROM DON_DAT_HANG_NCC WHERE Ma_Don_Hang LIKE @like", conn);
                    cmd.Parameters.AddWithValue("@p", prefix);
                    cmd.Parameters.AddWithValue("@like", prefix + "%");
                    next = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { }
            txtMaDonHang.Text = prefix + next.ToString("D3");
        }

        // ═══════════════════════════════════════════════════════════════
        // LOAD NHÀ CUNG CẤP VÀO COMBOBOX
        // ═══════════════════════════════════════════════════════════════
        void LoadNhaCungCap()
        {
            int prevId = _idNCC;
            cmbNhaCungCap.SelectedIndexChanged -= cmbNhaCungCap_SelectedIndexChanged; // tạm tắt event
            cmbNhaCungCap.Items.Clear();
            cmbNhaCungCap.Items.Add(new NccItem()); // dòng trống

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var rd = new SqlCommand(
                        "SELECT id, Ma_NCC, Ten_NCC, Dia_Chi, MST, Dien_Thoai FROM NHA_CUNG_CAP ORDER BY Ma_NCC",
                        conn).ExecuteReader();
                    while (rd.Read())
                        cmbNhaCungCap.Items.Add(new NccItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Ma = rd["Ma_NCC"].ToString(),
                            Ten = rd["Ten_NCC"].ToString(),
                            DiaChi = rd["Dia_Chi"].ToString(),
                            MST = rd["MST"].ToString(),
                            DienThoai = rd["Dien_Thoai"].ToString()
                        });
                }
            }
            catch { }

            cmbNhaCungCap.SelectedIndexChanged += cmbNhaCungCap_SelectedIndexChanged; // bật lại event

            // Chọn lại NCC đang dùng (nếu có)
            if (prevId > 0)
            {
                foreach (NccItem item in cmbNhaCungCap.Items)
                    if (item.Id == prevId) { cmbNhaCungCap.SelectedItem = item; return; }
            }
            cmbNhaCungCap.SelectedIndex = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ CHỌN NCC → TỰ ĐIỀN TÊN, ĐỊA CHỈ, MST
        // ═══════════════════════════════════════════════════════════════
        private void cmbNhaCungCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbNhaCungCap.SelectedItem is NccItem ncc && ncc.Id > 0)
            {
                _idNCC = ncc.Id;
                // ⭐ Điền thông tin vào các ô - kiểm tra tên control đúng với Designer
                txtTenNCC.Text = ncc.Ten;     // Tên nhà cung cấp
                txtMaSoThue.Text = ncc.MST;     // Mã số thuế
                txtDiaChiNCC.Text = ncc.DiaChi;  // Địa chỉ
            }
            else
            {
                _idNCC = 0;
                txtTenNCC.Clear();
                txtMaSoThue.Clear();
                txtDiaChiNCC.Clear();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NHẬP TÊN NCC MỚI → HỎI → SINH MÃ → LƯU → HIỆN MÃ → CHỌN VÀO COMBOBOX
        // ═══════════════════════════════════════════════════════════════
        private void txtTenNCC_Leave(object sender, EventArgs e)
        {
            string ten = txtTenNCC.Text.Trim();
            if (string.IsNullOrEmpty(ten) || _idNCC > 0) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Kiểm tra đã tồn tại chưa
                    var cmdCheck = new SqlCommand(
                        "SELECT id, Ma_NCC FROM NHA_CUNG_CAP WHERE Ten_NCC = @ten", conn);
                    cmdCheck.Parameters.AddWithValue("@ten", ten);
                    var rd = cmdCheck.ExecuteReader();
                    if (rd.Read())
                    {
                        _idNCC = Convert.ToInt32(rd["id"]);
                        string maCoSan = rd["Ma_NCC"].ToString();
                        rd.Close();
                        MessageBox.Show($"Nhà cung cấp đã có trong hệ thống.\nMã: {maCoSan}",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadNhaCungCap();
                        return;
                    }
                    rd.Close();

                    // Hỏi có muốn lưu không
                    if (MessageBox.Show(
                        $"Bạn có muốn lưu nhà cung cấp mới không?\n\nTên: {ten}",
                        "Lưu nhà cung cấp mới",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes) return;

                    // Sinh mã NCC001, NCC002...
                    var cmdNum = new SqlCommand(
                        "SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_NCC,4,10) AS INT)),0)+1 FROM NHA_CUNG_CAP WHERE Ma_NCC LIKE 'NCC%'",
                        conn);
                    string maNCC = "NCC" + Convert.ToInt32(cmdNum.ExecuteScalar()).ToString("D3");

                    // INSERT
                    var cmdIns = new SqlCommand(@"
INSERT INTO NHA_CUNG_CAP (Ma_NCC, Ten_NCC, Dia_Chi, MST, Dien_Thoai)
OUTPUT INSERTED.id
VALUES (@ma, @ten, @dc, @mst, '')", conn);
                    cmdIns.Parameters.AddWithValue("@ma", maNCC);
                    cmdIns.Parameters.AddWithValue("@ten", ten);
                    cmdIns.Parameters.AddWithValue("@dc", txtDiaChiNCC.Text.Trim());
                    cmdIns.Parameters.AddWithValue("@mst", txtMaSoThue.Text.Trim());
                    _idNCC = Convert.ToInt32(cmdIns.ExecuteScalar());

                    // ⭐ Thông báo mã vừa tạo
                    MessageBox.Show($"✅ Đã tạo mã: {maNCC}",
                        "Tạo nhà cung cấp thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload combobox → tự chọn NCC vừa tạo nhờ _idNCC
                    LoadNhaCungCap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ XÓA NCC - nút btnXoaNCC
        // ═══════════════════════════════════════════════════════════════
        private void btnXoaNCC_Click(object sender, EventArgs e)
        {
            if (_idNCC == 0)
            {
                MessageBox.Show("Chưa chọn nhà cung cấp nào!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tenHienTai = txtTenNCC.Text.Trim();

            if (MessageBox.Show(
                $"Bạn có chắc muốn xóa nhà cung cấp:\n\"{tenHienTai}\" không?\n\n⚠️ Chỉ xóa được nếu chưa có đơn hàng liên quan!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Kiểm tra có đơn hàng liên quan không
                    var cmdCheck = new SqlCommand(
                        "SELECT COUNT(*) FROM DON_DAT_HANG_NCC WHERE id_Nha_Cung_Cap = @id", conn);
                    cmdCheck.Parameters.AddWithValue("@id", _idNCC);
                    int sodon = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (sodon > 0)
                    {
                        MessageBox.Show(
                            $"❌ Không thể xóa!\nNhà cung cấp này đã có {sodon} đơn hàng liên quan.",
                            "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Xóa
                    var cmdDel = new SqlCommand(
                        "DELETE FROM NHA_CUNG_CAP WHERE id = @id", conn);
                    cmdDel.Parameters.AddWithValue("@id", _idNCC);
                    cmdDel.ExecuteNonQuery();

                    MessageBox.Show($"✅ Đã xóa nhà cung cấp \"{tenHienTai}\"",
                        "Xóa thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset NCC
                    _idNCC = 0;
                    txtTenNCC.Clear();
                    txtMaSoThue.Clear();
                    txtDiaChiNCC.Clear();
                    LoadNhaCungCap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvChiTiet;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", Width = 50, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMaNL", HeaderText = "Mã hàng", Width = 110, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(30, 100, 200), Font = new Font("Segoe UI", 10f, FontStyle.Bold) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTenHang", HeaderText = "Tên hàng", Width = 280 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDVT", HeaderText = "ĐVT", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSoLuong", HeaderText = "Số lượng", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", HeaderText = "Đơn giá", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colThanhTien", HeaderText = "Thành tiền", Width = 170, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle(ro) { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 80, 160) } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVAT", HeaderText = "% VAT", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdNL", Width = 0, Visible = false });
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colXoa",
                HeaderText = "",
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(254, 226, 226), ForeColor = Color.FromArgb(220, 38, 38), Font = new Font("Segoe UI", 12f, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgv.CellValueChanged -= DgvChiTiet_CellValueChanged;
            dgv.CellClick -= DgvChiTiet_CellClick;
            dgv.KeyDown -= DgvChiTiet_KeyDown;
            dgv.CellValueChanged += DgvChiTiet_CellValueChanged;
            dgv.CellClick += DgvChiTiet_CellClick;
            dgv.KeyDown += DgvChiTiet_KeyDown;

            ThemDongMoi();
        }

        void ThemDongMoi()
        {
            int soTrongDB = 0;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT ISNULL(MAX(CAST(SUBSTRING(Ma_Nguyen_Lieu,3,10) AS INT)),0) FROM NGUYEN_LIEU WHERE Ma_Nguyen_Lieu LIKE 'MH%'",
                        conn);
                    soTrongDB = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { }

            int next = soTrongDB + dgvChiTiet.Rows.Count + 1;
            string ma = "MH" + next.ToString("D3");
            int stt = dgvChiTiet.Rows.Count + 1;
            dgvChiTiet.Rows.Add(stt, ma, "", "", "1", "0", "0", "10", 0, "✕");
        }

        private void DgvChiTiet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string col = dgvChiTiet.Columns[e.ColumnIndex].Name;
            if (col == "colSoLuong" || col == "colDonGia" || col == "colVAT")
            {
                TinhThanhTienDong(e.RowIndex);
                TinhTong();
            }
        }

        void TinhThanhTienDong(int rowIdx)
        {
            var row = dgvChiTiet.Rows[rowIdx];
            double.TryParse(row.Cells["colSoLuong"].Value?.ToString().Replace(",", ""), out double sl);
            double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double dg);
            row.Cells["colThanhTien"].Value = (sl * dg).ToString("N0");
        }

        void TinhTong()
        {
            double tongTienHang = 0, tongThue = 0;
            foreach (DataGridViewRow row in dgvChiTiet.Rows)
            {
                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vat);
                tongTienHang += tt;
                tongThue += tt * vat / 100.0;
            }
            lblTongTienHanggg.Text = tongTienHang.ToString("N0") + " đ";
            lblTienGTGTT.Text = tongThue.ToString("N0") + " đ";
            lblTongThanhToann.Text = (tongTienHang + tongThue).ToString("N0") + " đ";
        }

        private void DgvChiTiet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvChiTiet.Columns[e.ColumnIndex].Name != "colXoa") return;
            if (dgvChiTiet.Rows.Count <= 1) return;
            dgvChiTiet.Rows.RemoveAt(e.RowIndex);
            for (int i = 0; i < dgvChiTiet.Rows.Count; i++)
                dgvChiTiet.Rows[i].Cells["colSTT"].Value = i + 1;
            TinhTong();
        }

        private void DgvChiTiet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = e.SuppressKeyPress = true;
            int next = dgvChiTiet.CurrentCell.RowIndex + 1;
            if (next >= dgvChiTiet.Rows.Count) ThemDongMoi();
            dgvChiTiet.CurrentCell = dgvChiTiet.Rows[Math.Min(next, dgvChiTiet.Rows.Count - 1)]
                                                .Cells[dgvChiTiet.CurrentCell.ColumnIndex];
        }

        private void btnThemDongg_Click(object sender, EventArgs e)
        {
            ThemDongMoi();
            int last = dgvChiTiet.Rows.Count - 1;
            dgvChiTiet.CurrentCell = dgvChiTiet.Rows[last].Cells["colTenHang"];
            dgvChiTiet.BeginEdit(true);
        }

        private void btnDinhKemFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Tất cả file|*.*|PDF|*.pdf|Excel|*.xlsx;*.xls|Word|*.docx;*.doc",
                Title = "Chọn file đính kèm"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _fileDinhKem = ofd.FileName;
                MessageBox.Show($"✅ Đã chọn:\n{System.IO.Path.GetFileName(_fileDinhKem)}",
                    "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT LÀM MỚI - có cờ _daLuu để không hỏi sau khi lưu xong
        // ═══════════════════════════════════════════════════════════════
        private void btnLamMoii_Click(object sender, EventArgs e)
        {
            // Nếu gọi tự động sau khi lưu thành công → không hỏi
            if (!_daLuu)
            {
                if (MessageBox.Show(
                    "Bạn có chắc muốn làm mới?\nDữ liệu chưa lưu sẽ bị mất!",
                    "Xác nhận làm mới",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) != DialogResult.OK) return;
            }
            _daLuu = false; // reset cờ
            _idDonHangDaLuu = 0; // reset id đơn hàng

            _idNCC = 0;
            cmbNhaCungCap.SelectedIndex = 0;
            txtTenNCC.Clear();
            txtMaSoThue.Clear();
            txtDiaChiNCC.Clear();

            dtpNgayDatHang.Value = DateTime.Today;
            dtpNgayGiaoHang.Value = DateTime.Today;
            cboTinhTrang.SelectedIndex = 0;
            cboDieuKhoan.SelectedIndex = 0;
            txtSoNgayNo.Text = "";
            txtDiaDiem.Text = "";
            txtDienGiai.Text = "";
            _fileDinhKem = "";

            dgvChiTiet.Rows.Clear();
            ThemDongMoi();
            TinhTong();
            SinhSoDonHang();
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT LƯU ĐƠN HÀNG → DATABASE
        // ═══════════════════════════════════════════════════════════════
        private void btnLuuu_Click(object sender, EventArgs e)
        {
            // Validate
            if (_idNCC == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn hoặc nhập Tên nhà cung cấp!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(txtMaDonHang.Text))
            {
                MessageBox.Show("⚠️ Số đơn hàng không được trống!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool coHang = false;
            foreach (DataGridViewRow row in dgvChiTiet.Rows)
                if (!string.IsNullOrWhiteSpace(row.Cells["colTenHang"].Value?.ToString()))
                { coHang = true; break; }
            if (!coHang)
            {
                MessageBox.Show("⚠️ Vui lòng nhập ít nhất 1 mặt hàng!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ⭐ ĐÃ LƯU RỒI → không INSERT lại, chỉ thông báo
            if (_daLuu && _idDonHangDaLuu > 0)
            {
                MessageBox.Show(
                    $"✅ Đơn hàng đã được lưu!\nSố đơn: {txtMaDonHang.Text}",
                    "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Tính tổng tiền
                            double tongTien = 0;
                            foreach (DataGridViewRow row in dgvChiTiet.Rows)
                            {
                                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                                double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vat);
                                tongTien += tt + tt * vat / 100.0;
                            }

                            // INSERT DON_DAT_HANG_NCC
                            var cmdDon = new SqlCommand(@"
INSERT INTO DON_DAT_HANG_NCC
    (Ma_Don_Hang, id_Nha_Cung_Cap, Ngay_Dat_Hang, Ngay_Giao_Hang,
     Dieu_Khoan_Thanh_Toan, So_Ngay_No, Trang_Thai,
     Dia_Diem_Giao_Hang, File_Dinh_Kem, Tong_Tien)
OUTPUT INSERTED.id
VALUES
    (@maDH, @idNCC, @ngayDat, @ngayGiao,
     @dieuKhoan, @soNgayNo, @trangThai,
     @diaDiem, @file, @tongTien)", conn, tran);

                            cmdDon.Parameters.AddWithValue("@maDH", txtMaDonHang.Text.Trim());
                            cmdDon.Parameters.AddWithValue("@idNCC", _idNCC);
                            cmdDon.Parameters.AddWithValue("@ngayDat", dtpNgayDatHang.Value.Date);
                            cmdDon.Parameters.AddWithValue("@ngayGiao", dtpNgayGiaoHang.Value.Date);
                            cmdDon.Parameters.AddWithValue("@dieuKhoan", cboDieuKhoan.SelectedItem?.ToString() ?? "");
                            cmdDon.Parameters.AddWithValue("@soNgayNo", int.TryParse(txtSoNgayNo.Text, out int snn) ? snn : 0);
                            cmdDon.Parameters.AddWithValue("@trangThai", cboTinhTrang.SelectedItem?.ToString() ?? "Chưa thực hiện");
                            cmdDon.Parameters.AddWithValue("@diaDiem", txtDiaDiem.Text.Trim());
                            cmdDon.Parameters.AddWithValue("@file", _fileDinhKem ?? "");
                            cmdDon.Parameters.AddWithValue("@tongTien", tongTien);

                            int idDonHang = Convert.ToInt32(cmdDon.ExecuteScalar());

                            // INSERT từng dòng hàng
                            foreach (DataGridViewRow row in dgvChiTiet.Rows)
                            {
                                string tenHang = row.Cells["colTenHang"].Value?.ToString().Trim() ?? "";
                                if (string.IsNullOrWhiteSpace(tenHang)) continue;

                                string maMH = row.Cells["colMaNL"].Value?.ToString() ?? "";
                                string dvt = row.Cells["colDVT"].Value?.ToString() ?? "";
                                double.TryParse(row.Cells["colSoLuong"].Value?.ToString().Replace(",", ""), out double sl);
                                double.TryParse(row.Cells["colDonGia"].Value?.ToString().Replace(",", ""), out double dg);
                                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                                double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vat);
                                double tienVAT = tt * vat / 100.0;

                                // Hàng mới → INSERT vào NGUYEN_LIEU trước
                                int idNL = 0;
                                int.TryParse(row.Cells["colIdNL"].Value?.ToString(), out idNL);
                                if (idNL == 0)
                                {
                                    var cmdNL = new SqlCommand(@"
INSERT INTO NGUYEN_LIEU (Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap, Ton_Kho, Ton_Kho_Toi_Thieu)
OUTPUT INSERTED.id
VALUES (@ma, @ten, @dvt, @gia, 0, 0)", conn, tran);
                                    cmdNL.Parameters.AddWithValue("@ma", maMH);
                                    cmdNL.Parameters.AddWithValue("@ten", tenHang);
                                    cmdNL.Parameters.AddWithValue("@dvt", dvt);
                                    cmdNL.Parameters.AddWithValue("@gia", dg);
                                    idNL = Convert.ToInt32(cmdNL.ExecuteScalar());
                                    row.Cells["colIdNL"].Value = idNL;
                                }

                                // INSERT CHI_TIET_DON_HANG_NCC
                                var cmdCT = new SqlCommand(@"
INSERT INTO CHI_TIET_DON_HANG_NCC
    (id_Don_Dat_Hang, id_Nguyen_Lieu, So_Luong, So_Luong_Da_Nhan,
     Don_Gia, Thanh_Tien, Phan_Tram_Thue_GTGT, Tien_Thue_GTGT)
VALUES (@idDH, @idNL, @sl, 0, @dg, @tt, @vat, @tienVAT)", conn, tran);
                                cmdCT.Parameters.AddWithValue("@idDH", idDonHang);
                                cmdCT.Parameters.AddWithValue("@idNL", idNL);
                                cmdCT.Parameters.AddWithValue("@sl", sl);
                                cmdCT.Parameters.AddWithValue("@dg", dg);
                                cmdCT.Parameters.AddWithValue("@tt", tt);
                                cmdCT.Parameters.AddWithValue("@vat", vat);
                                cmdCT.Parameters.AddWithValue("@tienVAT", tienVAT);
                                cmdCT.ExecuteNonQuery();
                            }

                            tran.Commit();

                            // ⭐ Lưu id để btnNhapKhoo dùng, KHÔNG reset form
                            _idDonHangDaLuu = idDonHang;
                            _daLuu = true;

                            MessageBox.Show(
                                $"✅ Lưu đơn hàng thành công!\n\nSố đơn: {txtMaDonHang.Text}\nTổng tiền: {tongTien:N0} đ",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex2) { tran.Rollback(); throw ex2; }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi lưu đơn hàng:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ⭐ NÚT NHẬP KHO - lưu đơn trước (nếu chưa), rồi mở frmInventoryReceive
        // ═══════════════════════════════════════════════════════════════
        private void btnNhapKhoo_Click(object sender, EventArgs e)
        {
            // Nếu chưa lưu lần nào → lưu trước
            if (!_daLuu || _idDonHangDaLuu == 0)
            {
                // Kiểm tra có đủ dữ liệu không
                if (_idNCC == 0)
                {
                    MessageBox.Show("⚠️ Vui lòng chọn nhà cung cấp trước khi nhập kho!",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool coHang = false;
                foreach (DataGridViewRow row in dgvChiTiet.Rows)
                    if (!string.IsNullOrWhiteSpace(row.Cells["colTenHang"].Value?.ToString()))
                    { coHang = true; break; }
                if (!coHang)
                {
                    MessageBox.Show("⚠️ Vui lòng nhập ít nhất 1 mặt hàng trước khi nhập kho!",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dr = MessageBox.Show(
                    "Đơn hàng chưa được lưu.\nHệ thống sẽ lưu đơn hàng trước rồi chuyển sang Phiếu Nhập Kho.\n\nBạn có đồng ý không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;

                // Lưu đơn hàng
                btnLuuu_Click(sender, e);

                // Nếu vẫn chưa lưu được (lỗi xảy ra) → thoát
                if (!_daLuu || _idDonHangDaLuu == 0) return;
            }

            // ⭐ Mở frmInventoryReceive với idDonHang đúng
            var frm = new frmInventoryReceive(_idDonHangDaLuu);
            frm.Show();
        }

        private void btnInDonHangg_Click(object sender, EventArgs e)
        {
            if (_idNCC == 0)
            {
                MessageBox.Show("⚠️ Chưa có thông tin nhà cung cấp!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double tongTienHang = 0, tongVAT = 0;
            foreach (DataGridViewRow row in dgvChiTiet.Rows)
            {
                double.TryParse(row.Cells["colThanhTien"].Value?.ToString().Replace(",", ""), out double tt);
                double.TryParse(row.Cells["colVAT"].Value?.ToString(), out double vat);
                tongTienHang += tt; tongVAT += tt * vat / 100.0;
            }
            var pd = new System.Drawing.Printing.PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var fTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
                var fHeader = new Font("Segoe UI", 10f, FontStyle.Bold);
                var fNormal = new Font("Segoe UI", 10f);
                var fSmall = new Font("Segoe UI", 9f);
                int x = 40, y = 30;
                g.DrawString("ĐƠN MUA HÀNG", fTitle, Brushes.Black, x, y); y += 30;
                g.DrawString($"Số đơn: {txtMaDonHang.Text}   |   Ngày: {dtpNgayDatHang.Value:dd/MM/yyyy}", fNormal, Brushes.Gray, x, y); y += 25;
                g.DrawLine(Pens.LightGray, x, y, 750, y); y += 12;
                g.DrawString("NHÀ CUNG CẤP", fHeader, Brushes.Black, x, y); y += 20;
                g.DrawString($"Tên: {txtTenNCC.Text}   |   MST: {txtMaSoThue.Text}   |   Địa chỉ: {txtDiaChiNCC.Text}", fNormal, Brushes.Black, x, y); y += 25;
                g.DrawLine(Pens.LightGray, x, y, 750, y); y += 15;
                g.FillRectangle(new SolidBrush(Color.FromArgb(248, 249, 250)), x, y, 710, 26);
                g.DrawString("STT", fHeader, Brushes.Black, x + 5, y + 4); g.DrawString("Tên hàng", fHeader, Brushes.Black, x + 50, y + 4);
                g.DrawString("ĐVT", fHeader, Brushes.Black, x + 280, y + 4); g.DrawString("SL", fHeader, Brushes.Black, x + 340, y + 4);
                g.DrawString("Đơn giá", fHeader, Brushes.Black, x + 400, y + 4); g.DrawString("Thành tiền", fHeader, Brushes.Black, x + 520, y + 4);
                g.DrawString("VAT", fHeader, Brushes.Black, x + 650, y + 4); y += 30;
                int stt = 1;
                foreach (DataGridViewRow row in dgvChiTiet.Rows)
                {
                    string ten = row.Cells["colTenHang"].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(ten)) continue;
                    if (stt % 2 == 0) g.FillRectangle(new SolidBrush(Color.FromArgb(250, 252, 255)), x, y, 710, 22);
                    g.DrawString(stt.ToString(), fSmall, Brushes.Black, x + 5, y + 3);
                    g.DrawString(ten, fSmall, Brushes.Black, x + 50, y + 3);
                    g.DrawString(row.Cells["colDVT"].Value?.ToString(), fSmall, Brushes.Black, x + 280, y + 3);
                    g.DrawString(row.Cells["colSoLuong"].Value?.ToString(), fSmall, Brushes.Black, x + 340, y + 3);
                    g.DrawString(row.Cells["colDonGia"].Value?.ToString(), fSmall, Brushes.Black, x + 400, y + 3);
                    g.DrawString(row.Cells["colThanhTien"].Value?.ToString(), fSmall, Brushes.Black, x + 520, y + 3);
                    g.DrawString(row.Cells["colVAT"].Value?.ToString() + "%", fSmall, Brushes.Black, x + 650, y + 3);
                    y += 24; stt++;
                }
                y += 12; g.DrawLine(Pens.Gray, x + 400, y, 750, y); y += 8;
                g.DrawString($"Tổng tiền hàng:   {tongTienHang:N0} đ", fNormal, Brushes.Black, x + 400, y); y += 22;
                g.DrawString($"Thuế GTGT:        {tongVAT:N0} đ", fNormal, Brushes.Black, x + 400, y); y += 22;
                g.DrawString($"TỔNG THANH TOÁN: {(tongTienHang + tongVAT):N0} đ", fHeader, new SolidBrush(Color.FromArgb(30, 80, 200)), x + 400, y);
                fTitle.Dispose(); fHeader.Dispose(); fNormal.Dispose(); fSmall.Dispose();
            };
            new PrintPreviewDialog { Document = pd, WindowState = FormWindowState.Maximized }.ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════
        // NCC ITEM CLASS
        // ═══════════════════════════════════════════════════════════════
        private class NccItem
        {
            public int Id { get; set; }
            public string Ma { get; set; } = "";
            public string Ten { get; set; } = "-- Chọn nhà cung cấp --";
            public string DiaChi { get; set; } = "";
            public string MST { get; set; } = "";
            public string DienThoai { get; set; } = "";
            // Combobox hiện MÃ NCC, dòng trống hiện chữ mặc định
            public override string ToString() => string.IsNullOrEmpty(Ma) ? Ten : Ma;
        }
    }
}