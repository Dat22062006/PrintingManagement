using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PrintingManagement
{
    public partial class frmQuoteManagement : Form
    {
        int currentPage = 1;
        int pageSize = 6;
        int totalRecord = 0;
        int totalPage = 0;

        public frmQuoteManagement()
        {
            InitializeComponent();
        }

        private void frmQuoteManagement_Load(object sender, EventArgs e)
        {
            // ⭐ FIX BUG 1: SetupBaoGiaColumns chỉ gọi 1 LẦN duy nhất ở đây
            // Không gọi lại trong LoadBaoGiaGrid nữa
            SetupBaoGiaColumns();
            LoadBaoGiaGrid();
        }

        // ═════════════════════════════════════════════════════════════════
        // LOAD GRID — KHÔNG gọi SetupBaoGiaColumns ở đây nữa
        // ═════════════════════════════════════════════════════════════════
        public void LoadBaoGiaGrid(string search = "")
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);

                string countQuery = @"
SELECT COUNT(DISTINCT bg.id)
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
WHERE bg.Ngay_Bao_Gia BETWEEN @from AND @to
  AND (@search='' OR
       bg.Ma_Bao_Gia       LIKE @like OR
       kh.Ten_Khach_Hang   LIKE @like OR
       bg.Ten_San_Pham     LIKE @like)";

                SqlCommand cmdCount = new SqlCommand(countQuery, conn);
                cmdCount.Parameters.AddWithValue("@search", search);
                cmdCount.Parameters.AddWithValue("@like", "%" + search + "%");
                cmdCount.Parameters.AddWithValue("@from", fromDate);
                cmdCount.Parameters.AddWithValue("@to", toDate);

                totalRecord = (int)cmdCount.ExecuteScalar();
                totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);

                int offset = (currentPage - 1) * pageSize;

                string query = @"
SELECT 
    ROW_NUMBER() OVER (ORDER BY bg.id DESC) AS [STT],
    bg.id                               AS [IDBaoGia],
    bg.Ma_Bao_Gia                       AS [Số báo giá],
    bg.Ngay_Bao_Gia                     AS [Ngày BG],
    kh.Ten_Khach_Hang                   AS [Khách hàng],
    bg.Ten_San_Pham                     AS [Sản phẩm],
    ISNULL(ct.So_Luong, 0)              AS [Số lượng],
    ISNULL(ct.Tong_Gia_Bao_Khach, 0)   AS [Tổng báo khách],
    bg.Trang_Thai                       AS [Trạng thái]
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
LEFT JOIN (
    SELECT id_Bao_Gia,
           MAX(So_Luong)            AS So_Luong,
           MAX(Tong_Gia_Bao_Khach)  AS Tong_Gia_Bao_Khach
    FROM CHI_TIET_BAO_GIA
    GROUP BY id_Bao_Gia
) ct ON ct.id_Bao_Gia = bg.id
WHERE bg.Ngay_Bao_Gia BETWEEN @from AND @to
  AND (@search='' OR
       bg.Ma_Bao_Gia       LIKE @like OR
       kh.Ten_Khach_Hang   LIKE @like OR
       bg.Ten_San_Pham     LIKE @like)
ORDER BY bg.id DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@like", "%" + search + "%");
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@from", fromDate);
                cmd.Parameters.AddWithValue("@to", toDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvBaoGia.DataSource = dt;
            }

            // Ẩn cột ID
            if (dgvBaoGia.Columns.Contains("IDBaoGia"))
                dgvBaoGia.Columns["IDBaoGia"].Visible = false;

            // ⭐ FIX BUG NÚT: Dùng DataBindingComplete — chạy SAU khi rows đã render xong
            dgvBaoGia.DataBindingComplete -= DgvBaoGia_DataBindingComplete;
            dgvBaoGia.DataBindingComplete += DgvBaoGia_DataBindingComplete;

            // ⭐ FIX BUG 2: BỎ dgvBaoGia.ReadOnly = true
            // ReadOnly = true chặn click vào ButtonColumn → nút không phản hồi
            // Thay bằng chỉ khóa các cột text, không khóa cột nút
            foreach (DataGridViewColumn col in dgvBaoGia.Columns)
            {
                if (col is DataGridViewButtonColumn)
                    col.ReadOnly = false;  // nút phải để false mới click được
                else
                    col.ReadOnly = true;
            }

            dgvBaoGia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvBaoGia.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvBaoGia.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBaoGia.ColumnHeadersHeight = 35;
            dgvBaoGia.ScrollBars = ScrollBars.Vertical;

            if (dgvBaoGia.Rows.Count > 0)
            {
                dgvBaoGia.FirstDisplayedScrollingRowIndex = 0;
                dgvBaoGia.ClearSelection();
                dgvBaoGia.CurrentCell = null;
            }

            if (dgvBaoGia.Columns.Contains("STT"))
                dgvBaoGia.Columns["STT"].Width = 40;

            foreach (DataGridViewColumn col in dgvBaoGia.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            CapNhatTrang();
        }

        void CapNhatTrang()
        {
            int start = (currentPage - 1) * pageSize + 1;
            int end = Math.Min(currentPage * pageSize, totalRecord);
            lblTrangg.Text = $"Hiển thị {start}-{end} / {totalRecord} báo giá";
            btnPage.Text = currentPage.ToString();
        }

        // ═════════════════════════════════════════════════════════════════
        // SETUP CỘT — chỉ gọi 1 lần duy nhất trong Load
        // ═════════════════════════════════════════════════════════════════
        public void SetupBaoGiaColumns()
        {
            dgvBaoGia.Columns.Clear();
            dgvBaoGia.RowHeadersVisible = false;
            dgvBaoGia.AllowUserToAddRows = false;
            dgvBaoGia.AllowUserToResizeRows = false;
            dgvBaoGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBaoGia.MultiSelect = false;
            dgvBaoGia.RowTemplate.Height = 32;
            dgvBaoGia.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvBaoGia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 40,
                MinimumWidth = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Số báo giá",
                DataPropertyName = "Số báo giá",
                Width = 130
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày BG",
                DataPropertyName = "Ngày BG",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Khách hàng",
                DataPropertyName = "Khách hàng",
                Width = 200
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Sản phẩm",
                DataPropertyName = "Sản phẩm",
                Width = 160
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Số lượng",
                DataPropertyName = "Số lượng",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tổng báo khách",
                DataPropertyName = "Tổng báo khách",
                Width = 280,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "#,##0 'VNĐ'",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvBaoGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Trạng thái",
                DataPropertyName = "Trạng thái",
                Name = "TrangThai",
                Width = 120
            });

            var btnStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 8),
                Padding = new Padding(0),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvBaoGia.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnView",
                HeaderText = "Thao tác",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = btnStyle
            });
            dgvBaoGia.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEdit",
                HeaderText = "",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = btnStyle
            });
            dgvBaoGia.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnApprove",
                HeaderText = "",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = btnStyle
            });
            dgvBaoGia.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnCancel",
                HeaderText = "",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = btnStyle
            });

            // ⭐ Nút Hoàn thành SX
            dgvBaoGia.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnComplete",
                HeaderText = "",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(0),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(22, 163, 74)
                }
            });

            // ⭐ FIX BUG 1: Đăng ký event 1 lần duy nhất, dùng -= trước += để chắc chắn
            dgvBaoGia.CellFormatting -= dgvBaoGia_CellFormatting;
            dgvBaoGia.CellFormatting += dgvBaoGia_CellFormatting;
            dgvBaoGia.CellClick -= dgvBaoGia_CellClick;
            dgvBaoGia.CellClick += dgvBaoGia_CellClick;

            // ⭐ Nút xuất Excel báo giá
            btnXuatExcel.Click -= btnXuatExcel_Click;
            btnXuatExcel.Click += btnXuatExcel_Click;

            // ⭐ TOOLTIP: dùng CellToolTipTextNeeded — cách duy nhất đúng cho cell
            dgvBaoGia.ShowCellToolTips = true;
            dgvBaoGia.CellToolTipTextNeeded -= DgvBaoGia_CellToolTipTextNeeded;
            dgvBaoGia.CellToolTipTextNeeded += DgvBaoGia_CellToolTipTextNeeded;
        }

        // ═════════════════════════════════════════════════════════════════
        // FORMAT TRẠNG THÁI
        // ═════════════════════════════════════════════════════════════════
        private void dgvBaoGia_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBaoGia.Columns[e.ColumnIndex].HeaderText == "Tổng báo khách" && e.Value != null)
            {
                if (double.TryParse(e.Value.ToString(), out double money))
                {
                    e.Value = money.ToString("N0") + " vnđ";
                    e.FormattingApplied = true;
                }
            }
        }

        // ═════════════════════════════════════════════════════════════════
        // CELL CLICK
        // ═════════════════════════════════════════════════════════════════
        private void dgvBaoGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvBaoGia.Columns[e.ColumnIndex] == null) return;

            try
            {
                int idBaoGia = Convert.ToInt32(dgvBaoGia.Rows[e.RowIndex].Cells["IDBaoGia"].Value);

                if (dgvBaoGia.Columns[e.ColumnIndex].Name == "btnView")
                {
                    using (frmPriceCalculation1 f = new frmPriceCalculation1(idBaoGia))
                        f.ShowDialog();
                }
                else if (dgvBaoGia.Columns[e.ColumnIndex].Name == "btnEdit")
                {
                    using (frmPriceCalculation frm = new frmPriceCalculation(idBaoGia))
                        frm.ShowDialog();
                    LoadBaoGiaGrid();
                }
                else if (dgvBaoGia.Columns[e.ColumnIndex].Name == "btnApprove")
                {
                    if (MessageBox.Show("Bạn có chắc muốn duyệt báo giá này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE BAO_GIA SET Trang_Thai = N'Đã duyệt' WHERE id = @id",
                            new[] { new SqlParameter("@id", idBaoGia) });
                        MessageBox.Show("✅ Đã duyệt báo giá!", "Thành công");
                        LoadBaoGiaGrid();
                    }
                }
                else if (dgvBaoGia.Columns[e.ColumnIndex].Name == "btnCancel")
                {
                    if (MessageBox.Show("Bạn có chắc muốn hủy báo giá này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            DatabaseHelper.ExecuteNonQuery(
                                "UPDATE BAO_GIA SET Trang_Thai = N'Huỷ' WHERE id = @id",
                                new[] { new SqlParameter("@id", idBaoGia) });
                            MessageBox.Show("✅ Đã hủy báo giá!", "Thành công");
                            LoadBaoGiaGrid();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Lỗi khi hủy:\n{ex.Message}", "Lỗi");
                        }
                    }
                }
                // ⭐ NÚT HOÀN THÀNH SX
                else if (dgvBaoGia.Columns[e.ColumnIndex].Name == "btnComplete")
                {
                    string trangThai = dgvBaoGia.Rows[e.RowIndex].Cells["TrangThai"].Value?.ToString() ?? "";
                    if (trangThai != "Đang sản xuất")
                    {
                        MessageBox.Show(
                            $"⚠️ Chỉ có thể hoàn thành khi trạng thái là 'Đang sản xuất'!\n" +
                            $"Trạng thái hiện tại: {trangThai}",
                            "Không thể thực hiện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show(
                        "Xác nhận hoàn thành sản xuất?\n\n" +
                        "✅ BAO_GIA → 'Hoàn thành'\n" +
                        "✅ LENH_SAN_XUAT → 'Hoàn thành'",
                        "Xác nhận hoàn thành SX",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            using (var conn = DatabaseHelper.GetConnection())
                            {
                                conn.Open();
                                using (var tran = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        // Cập nhật BAO_GIA
                                        var cmd1 = new SqlCommand(
                                            "UPDATE BAO_GIA SET Trang_Thai = N'Hoàn thành' WHERE id = @id",
                                            conn, tran);
                                        cmd1.Parameters.AddWithValue("@id", idBaoGia);
                                        cmd1.ExecuteNonQuery();

                                        // Cập nhật LENH_SAN_XUAT liên kết
                                        var cmd2 = new SqlCommand(
                                            "UPDATE LENH_SAN_XUAT SET Trang_Thai = N'Hoàn thành' WHERE id_Bao_Gia = @id",
                                            conn, tran);
                                        cmd2.Parameters.AddWithValue("@id", idBaoGia);
                                        cmd2.ExecuteNonQuery();

                                        tran.Commit();
                                        MessageBox.Show("✅ Đã cập nhật hoàn thành sản xuất!\n\nBạn có thể lập chứng từ bán hàng.", "Thành công");
                                        LoadBaoGiaGrid();
                                    }
                                    catch { tran.Rollback(); throw; }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Lỗi:\n{ex.Message}", "Lỗi");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi");
            }
        }

        // ═════════════════════════════════════════════════════════════════
        // PHÂN TRANG & TÌM KIẾM
        // ═════════════════════════════════════════════════════════════════
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadBaoGiaGrid(txtSearch.Text); }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPage) { currentPage++; LoadBaoGiaGrid(txtSearch.Text); }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadBaoGiaGrid();
            dgvBaoGia.ClearSelection();
            dgvBaoGia.CurrentCell = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadBaoGiaGrid(txtSearch.Text.Trim());
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { currentPage = 1; LoadBaoGiaGrid(txtSearch.Text.Trim()); }
        }

        private void btnTaoBaoGiaMoi_Click(object sender, EventArgs e)
        {
            using (frmPriceCalculation frm = new frmPriceCalculation())
                frm.ShowDialog();
            LoadBaoGiaGrid();
        }

        // ═════════════════════════════════════════════════════════════════
        // TOOLTIP CHO TỪNG CELL NÚT THAO TÁC
        // ═════════════════════════════════════════════════════════════════
        private void DgvBaoGia_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dgvBaoGia.Columns.Contains("btnView")) return;
            foreach (DataGridViewRow row in dgvBaoGia.Rows)
            {
                row.Cells["btnView"].Value = "👁";
                row.Cells["btnEdit"].Value = "✏";
                row.Cells["btnApprove"].Value = "✔";
                row.Cells["btnCancel"].Value = "✖";
                row.Cells["btnComplete"].Value = "✅";
            }
        }

        private void DgvBaoGia_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            switch (dgvBaoGia.Columns[e.ColumnIndex].Name)
            {
                case "btnView": e.ToolTipText = "Xem chi tiết báo giá"; break;
                case "btnEdit": e.ToolTipText = "Chỉnh sửa báo giá"; break;
                case "btnApprove": e.ToolTipText = "Duyệt báo giá"; break;
                case "btnCancel": e.ToolTipText = "Hủy báo giá"; break;
                case "btnComplete": e.ToolTipText = "Hoàn thành sản xuất"; break;
            }
        }

        // ═════════════════════════════════════════════════════════════════
        // XUẤT EXCEL BÁO GIÁ
        // ═════════════════════════════════════════════════════════════════
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            // Lấy id của dòng đang được chọn trong grid
            if (dgvBaoGia.CurrentRow == null || dgvBaoGia.CurrentRow.Index < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn một báo giá để xuất Excel!",
                    "Chưa chọn báo giá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idBaoGia = 0;
            try
            {
                idBaoGia = Convert.ToInt32(dgvBaoGia.CurrentRow.Cells["IDBaoGia"].Value);
            }
            catch
            {
                MessageBox.Show("❌ Không lấy được ID báo giá!", "Lỗi");
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel|*.xlsx";
                dlg.FileName = $"BaoGia_{DateTime.Today:ddMMyyyy}.xlsx";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    XuatExcelBaoGia(idBaoGia, dlg.FileName);
                    MessageBox.Show($"✅ Xuất Excel thành công!\n{dlg.FileName}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở file luôn
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi");
                }
            }
        }

        private void XuatExcelBaoGia(int idBaoGia, string outputPath)
        {
            // ── Lấy dữ liệu từ DB ──────────────────────────────────────────
            string tenKH = "", diaChi = "", mst = "", nguoiLH = "";
            string maBG = "", ngayBG = "", hieuluc = "", tgGiao = "";
            string tenSP = "", kichThuoc = "", chatLieu = "";
            double soLuong = 0, donGia = 0;

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Thông tin báo giá + khách hàng
                var cmdBG = new SqlCommand(@"
SELECT
    bg.Ma_Bao_Gia,
    CONVERT(NVARCHAR,bg.Ngay_Bao_Gia,103)       AS NgayBG,
    bg.Hieu_Luc_Bao_Gia_Ngay,
    bg.Thoi_Gian_Giao_Hang_Du_Kien,
    bg.Ten_San_Pham,
    bg.Kich_Thuoc_Thanh_Pham,
    bg.Khoi_Luong_Giay,
    ISNULL(bg.Ten_Loai_Giay, bg.Khoi_Luong_Giay) AS Ten_Loai_Giay,
    ISNULL(kh.Ten_Khach_Hang,'')                AS TenKH,
    ISNULL(kh.Dia_Chi,'')                       AS DiaChi,
    ISNULL(kh.MST,'')                           AS MST,
    ISNULL(kh.Nguoi_Lien_He,'')                 AS NguoiLH
FROM BAO_GIA bg
LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
WHERE bg.id = @id", conn);
                cmdBG.Parameters.AddWithValue("@id", idBaoGia);
                var rd = cmdBG.ExecuteReader();
                if (rd.Read())
                {
                    maBG = rd["Ma_Bao_Gia"].ToString();
                    ngayBG = rd["NgayBG"].ToString();
                    hieuluc = rd["Hieu_Luc_Bao_Gia_Ngay"] != DBNull.Value
                                ? Convert.ToInt32(rd["Hieu_Luc_Bao_Gia_Ngay"]).ToString() + " ngày"
                                : "30 ngày";
                    tgGiao = rd["Thoi_Gian_Giao_Hang_Du_Kien"].ToString();
                    tenSP = rd["Ten_San_Pham"].ToString();
                    kichThuoc = rd["Kich_Thuoc_Thanh_Pham"].ToString();
                    chatLieu = rd["Ten_Loai_Giay"].ToString(); // ⭐ Lấy tên loại giấy thay vì định lượng
                    tenKH = rd["TenKH"].ToString();
                    diaChi = rd["DiaChi"].ToString();
                    mst = rd["MST"].ToString();
                    nguoiLH = rd["NguoiLH"].ToString();
                }
                rd.Close();

                // Chi tiết: số lượng + đơn giá báo khách
                var cmdCT = new SqlCommand(@"
SELECT So_Luong, Gia_Bao_Khach
FROM CHI_TIET_BAO_GIA
WHERE id_Bao_Gia = @id", conn);
                cmdCT.Parameters.AddWithValue("@id", idBaoGia);
                var rd2 = cmdCT.ExecuteReader();
                if (rd2.Read())
                {
                    soLuong = Convert.ToDouble(rd2["So_Luong"]);
                    donGia = Convert.ToDouble(rd2["Gia_Bao_Khach"]);
                }
                rd2.Close();
            }

            // ── Xuất Excel bằng EPPlus ──────────────────────────────────────
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var pkg = new OfficeOpenXml.ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Báo Giá");
                ws.View.ShowGridLines = false;

                // Column widths
                ws.Column(1).Width = 6;   // STT
                ws.Column(2).Width = 28;  // Tên SP
                ws.Column(3).Width = 22;  // Kích thước
                ws.Column(4).Width = 20;  // Chất liệu
                ws.Column(5).Width = 12;  // Số lượng
                ws.Column(6).Width = 16;  // Đơn giá
                ws.Column(7).Width = 18;  // Thành tiền
                ws.Column(8).Width = 14;  // (dự phòng)

                // ── Màu sắc ──────────────────────────────────────────────
                var navy = System.Drawing.Color.FromArgb(30, 58, 95);
                var blue = System.Drawing.Color.FromArgb(37, 99, 235);
                var lblue = System.Drawing.Color.FromArgb(219, 234, 254);
                var gray = System.Drawing.Color.FromArgb(248, 250, 252);
                var dgray = System.Drawing.Color.FromArgb(100, 116, 139);
                var white = System.Drawing.Color.White;
                var green = System.Drawing.Color.FromArgb(5, 150, 105);

                // ── Helper styles ─────────────────────────────────────────
                void StyleCell(OfficeOpenXml.ExcelRange r,
                               System.Drawing.Color bg, System.Drawing.Color fg,
                               bool bold = false, int size = 10,
                               string hAlign = "left", string vAlign = "center",
                               bool wrap = false, bool border = false)
                {
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(bg);
                    r.Style.Font.Color.SetColor(fg);
                    r.Style.Font.Bold = bold;
                    r.Style.Font.Size = size;
                    r.Style.Font.Name = "Arial";
                    r.Style.VerticalAlignment = vAlign == "top"
                        ? OfficeOpenXml.Style.ExcelVerticalAlignment.Top
                        : OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    r.Style.HorizontalAlignment = hAlign == "center"
                        ? OfficeOpenXml.Style.ExcelHorizontalAlignment.Center
                        : hAlign == "right"
                            ? OfficeOpenXml.Style.ExcelHorizontalAlignment.Right
                            : OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    r.Style.WrapText = wrap;
                    if (border)
                    {
                        var bs = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        var bc = System.Drawing.Color.FromArgb(203, 213, 225);
                        r.Style.Border.Top.Style = bs; r.Style.Border.Top.Color.SetColor(bc);
                        r.Style.Border.Bottom.Style = bs; r.Style.Border.Bottom.Color.SetColor(bc);
                        r.Style.Border.Left.Style = bs; r.Style.Border.Left.Color.SetColor(bc);
                        r.Style.Border.Right.Style = bs; r.Style.Border.Right.Color.SetColor(bc);
                    }
                }

                // ════════════════════════════════════════════════════════
                // HEADER (rows 1-4)
                // ════════════════════════════════════════════════════════
                ws.Row(1).Height = 8;
                ws.Row(2).Height = 32;
                ws.Row(3).Height = 18;
                ws.Row(4).Height = 18;

                // Nền navy toàn header
                StyleCell(ws.Cells["A1:H4"], navy, white);

                // Tên công ty (cột trái)
                ws.Cells["A2:D2"].Merge = true;
                ws.Cells["A2"].Value = "CÔNG TY IN ẤN BAO LÌ XÌ";
                StyleCell(ws.Cells["A2:D2"], navy, white, true, 14);

                ws.Cells["A3:D3"].Merge = true;
                ws.Cells["A3"].Value = "TP. Hồ Chí Minh  |  Tel: 0901 234 567";
                StyleCell(ws.Cells["A3:D3"], navy, System.Drawing.Color.FromArgb(189, 213, 234), false, 9);

                ws.Cells["A4:D4"].Merge = true;
                ws.Cells["A4"].Value = "Email: info@company.vn";
                StyleCell(ws.Cells["A4:D4"], navy, System.Drawing.Color.FromArgb(189, 213, 234), false, 9);

                // BÁO GIÁ + số (cột phải)
                ws.Cells["E2:H2"].Merge = true;
                ws.Cells["E2"].Value = "BÁO GIÁ";
                StyleCell(ws.Cells["E2:H2"], navy, white, true, 22, "right");

                ws.Cells["E3:H3"].Merge = true;
                ws.Cells["E3"].Value = $"Số: {maBG}";
                StyleCell(ws.Cells["E3:H3"], navy, System.Drawing.Color.FromArgb(147, 197, 253), false, 10, "right");

                ws.Cells["E4:H4"].Merge = true;
                ws.Cells["E4"].Value = $"Ngày: {ngayBG}";
                StyleCell(ws.Cells["E4:H4"], navy, System.Drawing.Color.FromArgb(189, 213, 234), false, 9, "right");

                // ════════════════════════════════════════════════════════
                // INFO BLOCK (rows 6-10)
                // ════════════════════════════════════════════════════════
                ws.Row(5).Height = 8;

                string[] lblKH = { "THÔNG TIN KHÁCH HÀNG", "Tên khách hàng:", "Địa chỉ:", "Mã số thuế:", "Người liên hệ:" };
                string[] valKH = { "", tenKH, diaChi, mst, nguoiLH };
                string[] lblBG = { "THÔNG TIN BÁO GIÁ", "Số báo giá:", "Ngày báo giá:", "Hiệu lực:", "Thời gian giao hàng:" };
                string[] valBG = { "", maBG, ngayBG, hieuluc, tgGiao };

                for (int i = 0; i < 5; i++)
                {
                    int r = 6 + i;
                    ws.Row(r).Height = 20;
                    bool isHeader = i == 0;

                    // Cột trái KH
                    ws.Cells[r, 1, r, 2].Merge = true;
                    ws.Cells[r, 1].Value = lblKH[i];
                    StyleCell(ws.Cells[r, 1, r, 2],
                        isHeader ? navy : gray,
                        isHeader ? white : dgray,
                        isHeader, isHeader ? 10 : 9,
                        isHeader ? "center" : "left",
                        "center", false, !isHeader);

                    ws.Cells[r, 3, r, 4].Merge = true;
                    ws.Cells[r, 3].Value = valKH[i];
                    StyleCell(ws.Cells[r, 3, r, 4],
                        isHeader ? navy : white,
                        isHeader ? white : System.Drawing.Color.FromArgb(30, 41, 59),
                        isHeader, 10, "left", "center", true, !isHeader);

                    // Cột phải BG
                    ws.Cells[r, 5, r, 6].Merge = true;
                    ws.Cells[r, 5].Value = lblBG[i];
                    StyleCell(ws.Cells[r, 5, r, 6],
                        isHeader ? navy : gray,
                        isHeader ? white : dgray,
                        isHeader, isHeader ? 10 : 9,
                        isHeader ? "center" : "left",
                        "center", false, !isHeader);

                    ws.Cells[r, 7, r, 8].Merge = true;
                    ws.Cells[r, 7].Value = valBG[i];
                    StyleCell(ws.Cells[r, 7, r, 8],
                        isHeader ? navy : white,
                        isHeader ? white : System.Drawing.Color.FromArgb(30, 41, 59),
                        false, 10, "left", "center", false, !isHeader);
                }

                // ════════════════════════════════════════════════════════
                // TABLE HEADER (rows 12-13)
                // ════════════════════════════════════════════════════════
                ws.Row(11).Height = 10;
                ws.Row(12).Height = 14;
                ws.Row(13).Height = 28;

                ws.Cells["A12:H12"].Merge = true;
                ws.Cells["A12"].Value = "CHI TIẾT SẢN PHẨM BÁO GIÁ";
                StyleCell(ws.Cells["A12:H12"], blue, white, true, 11, "center");

                string[] headers = { "STT", "Tên sản phẩm", "Kích thước", "Chất liệu", "Số lượng", "Đơn giá (đ)", "Thành tiền (đ)" };
                string[] hAligns = { "center", "left", "center", "center", "right", "right", "right" };
                for (int ci = 0; ci < headers.Length; ci++)
                {
                    var cell = ws.Cells[13, ci + 1];
                    cell.Value = headers[ci];
                    StyleCell(ws.Cells[13, ci + 1, 13, ci + 1], navy, white, true, 10, hAligns[ci], "center", true, true);
                }

                // ════════════════════════════════════════════════════════
                // DATA ROW (row 14) — 1 dòng sản phẩm
                // ════════════════════════════════════════════════════════
                ws.Row(14).Height = 28;
                int dataRow = 14;

                ws.Cells[dataRow, 1].Value = 1;
                ws.Cells[dataRow, 2].Value = tenSP;
                ws.Cells[dataRow, 3].Value = kichThuoc;
                ws.Cells[dataRow, 4].Value = chatLieu;
                ws.Cells[dataRow, 5].Value = soLuong;
                ws.Cells[dataRow, 6].Value = donGia;
                ws.Cells[dataRow, 7].Formula = $"=E{dataRow}*F{dataRow}";

                string[] dAligns = { "center", "left", "center", "center", "right", "right", "right" };
                for (int ci = 1; ci <= 7; ci++)
                {
                    StyleCell(ws.Cells[dataRow, ci], white,
                        System.Drawing.Color.FromArgb(30, 41, 59),
                        ci == 2, 10, dAligns[ci - 1], "center", true, true);
                    if (ci >= 5)
                        ws.Cells[dataRow, ci].Style.Numberformat.Format = "#,##0";
                }
                ws.Cells[dataRow, 8].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[dataRow, 8].Style.Fill.BackgroundColor.SetColor(white);

                // ════════════════════════════════════════════════════════
                // TỔNG CỘNG (rows 15-17)
                // ════════════════════════════════════════════════════════
                int tongRow = dataRow + 1;
                int vatRow = tongRow + 1;
                int ttRow = vatRow + 1;

                void SummaryRow(int r, string label, string formula,
                                bool bold, System.Drawing.Color bg, System.Drawing.Color fg)
                {
                    ws.Row(r).Height = 22;
                    ws.Cells[r, 1, r, 5].Merge = true;
                    StyleCell(ws.Cells[r, 1, r, 5], bg, bg);

                    ws.Cells[r, 6].Value = label;
                    StyleCell(ws.Cells[r, 6], bg, dgray, bold, 10, "right", "center", false, true);

                    ws.Cells[r, 7].Formula = formula;
                    ws.Cells[r, 7].Style.Numberformat.Format = "#,##0";
                    StyleCell(ws.Cells[r, 7], bg, fg, bold, bold ? 12 : 11, "right", "center", false, true);

                    ws.Cells[r, 8].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[r, 8].Style.Fill.BackgroundColor.SetColor(bg);
                }

                SummaryRow(tongRow, "Tổng tiền chưa VAT:",
                    $"=SUM(G{dataRow}:G{dataRow})", false, gray,
                    System.Drawing.Color.FromArgb(30, 41, 59));

                SummaryRow(vatRow, "VAT (10%):",
                    $"=G{tongRow}*10%", false, gray,
                    System.Drawing.Color.FromArgb(30, 41, 59));

                SummaryRow(ttRow, "TỔNG THANH TOÁN:",
                    $"=G{tongRow}+G{vatRow}", true, navy, white);

                // ════════════════════════════════════════════════════════
                // GHI CHÚ (row ttRow+2)
                // ════════════════════════════════════════════════════════
                int noteRow = ttRow + 2;
                ws.Row(noteRow).Height = 14;
                ws.Row(noteRow + 1).Height = 50;

                ws.Cells[noteRow, 1, noteRow, 8].Merge = true;
                ws.Cells[noteRow, 1].Value = "GHI CHÚ & ĐIỀU KHOẢN";
                StyleCell(ws.Cells[noteRow, 1, noteRow, 8], blue, white, true, 10, "left", "center");

                ws.Cells[noteRow + 1, 1, noteRow + 1, 8].Merge = true;
                ws.Cells[noteRow + 1, 1].Value =
                    "• Giá trên chưa bao gồm VAT 10%.\n" +
                    $"• Báo giá có hiệu lực trong {hieuluc} kể từ ngày phát hành.\n" +
                    $"• Thời gian giao hàng: {tgGiao} sau khi nhận đặt cọc.\n" +
                    "• Đặt cọc 50% giá trị đơn hàng để tiến hành sản xuất.";
                StyleCell(ws.Cells[noteRow + 1, 1, noteRow + 1, 8], gray, dgray, false, 9,
                    "left", "top", true, true);

                // ════════════════════════════════════════════════════════
                // CHỮ KÝ (row noteRow+3)
                // ════════════════════════════════════════════════════════
                int signRow = noteRow + 3;
                ws.Row(signRow).Height = 18;
                ws.Row(signRow + 1).Height = 55;

                ws.Cells[signRow, 1, signRow, 4].Merge = true;
                ws.Cells[signRow, 1].Value = "ĐẠI DIỆN KHÁCH HÀNG";
                StyleCell(ws.Cells[signRow, 1, signRow, 4], navy, white, true, 10, "center");

                ws.Cells[signRow, 5, signRow, 8].Merge = true;
                ws.Cells[signRow, 5].Value = "ĐẠI DIỆN CÔNG TY";
                StyleCell(ws.Cells[signRow, 5, signRow, 8], navy, white, true, 10, "center");

                ws.Cells[signRow + 1, 1, signRow + 1, 4].Merge = true;
                ws.Cells[signRow + 1, 1].Value = "(Ký, ghi rõ họ tên)";
                StyleCell(ws.Cells[signRow + 1, 1, signRow + 1, 4], gray, dgray, false, 9,
                    "center", "bottom", false, true);

                ws.Cells[signRow + 1, 5, signRow + 1, 8].Merge = true;
                ws.Cells[signRow + 1, 5].Value = "(Ký, ghi rõ họ tên)";
                StyleCell(ws.Cells[signRow + 1, 5, signRow + 1, 8], gray, dgray, false, 9,
                    "center", "bottom", false, true);

                // ════════════════════════════════════════════════════════
                // PAGE SETUP
                // ════════════════════════════════════════════════════════
                ws.PrinterSettings.Orientation = OfficeOpenXml.eOrientation.Landscape;
                ws.PrinterSettings.PaperSize = OfficeOpenXml.ePaperSize.A4;
                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.FitToWidth = 1;
                ws.PrinterSettings.FitToHeight = 0;

                pkg.SaveAs(new System.IO.FileInfo(outputPath));
            }
        }
    }
}