using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmQuanLyKhachHang : Form
    {
        public frmQuanLyKhachHang()
        {
            InitializeComponent();
        }

        private void frmQuanLyKhachHang_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadDanhSachKhachHang();
            dgvKhachHang.BringToFront();
        }

        private void SetupGrid()
        {
            dgvKhachHang.Columns.Clear();
            dgvKhachHang.RowHeadersVisible = false;
            dgvKhachHang.AllowUserToAddRows = false;
            dgvKhachHang.AllowUserToDeleteRows = false;
            dgvKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKhachHang.AutoGenerateColumns = false;
            dgvKhachHang.MultiSelect = false;
            dgvKhachHang.ReadOnly = true;

            dgvKhachHang.DefaultCellStyle.Font = new Font("Segoe UI", 10.5f);
            dgvKhachHang.RowTemplate.Height = 36;

            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvKhachHang.ColumnHeadersHeight = 44;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor;

            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 50,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ma_KH",
                HeaderText = "Mã KH",
                DataPropertyName = "Ma_KH",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ten_Khach_Hang",
                HeaderText = "Tên khách hàng",
                DataPropertyName = "Ten_Khach_Hang",
                Width = 250,
                ReadOnly = true
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Dia_Chi",
                HeaderText = "Địa chỉ",
                DataPropertyName = "Dia_Chi",
                Width = 300,
                ReadOnly = true
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MST",
                HeaderText = "MST",
                DataPropertyName = "MST",
                Width = 130,
                ReadOnly = true
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Dien_Thoai",
                HeaderText = "Điện thoại",
                DataPropertyName = "Dien_Thoai",
                Width = 130,
                ReadOnly = true
            });
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nguoi_Lien_He",
                HeaderText = "Người liên hệ",
                DataPropertyName = "Nguoi_Lien_He",
                Width = 150,
                ReadOnly = true
            });

            // Hidden id
            dgvKhachHang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                HeaderText = "id",
                DataPropertyName = "id",
                Visible = false
            });

            // Nút Sửa — truyền idKH để load dữ liệu có sẵn lên form
            dgvKhachHang.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnSua",
                HeaderText = "Sửa",
                Text = "✏️ Sửa",
                UseColumnTextForButtonValue = true,
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                }
            });

            // Nút Không hoạt động
            dgvKhachHang.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnKhongHD",
                HeaderText = "Không HĐ",
                Text = "🚫 Không HĐ",
                UseColumnTextForButtonValue = true,
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(239, 68, 68),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                }
            });

            foreach (DataGridViewColumn col in dgvKhachHang.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvKhachHang.CellContentClick += dgvKhachHang_CellContentClick;
        }

        private void LoadDanhSachKhachHang(string timKiem = "")
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT 
    ROW_NUMBER() OVER (ORDER BY Ma_KH) AS STT,
    Ma_KH,
    Ten_Khach_Hang,
    Dia_Chi,
    MST,
    Dien_Thoai,
    Nguoi_Lien_He,
    id
FROM KHACH_HANG
WHERE Trang_Thai = N'Hoạt động'
  AND (@tim = '' OR 
       Ma_KH          LIKE @like OR 
       Ten_Khach_Hang LIKE @like OR 
       Dia_Chi        LIKE @like)
ORDER BY Ma_KH";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tim", timKiem);
                    cmd.Parameters.AddWithValue("@like", "%" + timKiem + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvKhachHang.DataSource = dt;

                    if (dgvKhachHang.Columns.Contains("id"))
                        dgvKhachHang.Columns["id"].Visible = false;

                    if (dgvKhachHang.Rows.Count > 0)
                    {
                        dgvKhachHang.ClearSelection();
                        dgvKhachHang.CurrentCell = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh sách khách hàng:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvKhachHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Lấy id từ cột ẩn
            int idKH = 0;
            if (dgvKhachHang.Rows[e.RowIndex].Cells["id"].Value != null)
                int.TryParse(dgvKhachHang.Rows[e.RowIndex].Cells["id"].Value.ToString(), out idKH);

            string maKH = dgvKhachHang.Rows[e.RowIndex].Cells["Ma_KH"].Value?.ToString() ?? "";
            string tenKH = dgvKhachHang.Rows[e.RowIndex].Cells["Ten_Khach_Hang"].Value?.ToString() ?? "";

            // ── Nút Sửa: truyền idKH → form tự load dữ liệu dòng đó ────────────────
            if (dgvKhachHang.Columns[e.ColumnIndex].Name == "btnSua")
            {
                if (idKH == 0)
                {
                    MessageBox.Show("Không tìm thấy id khách hàng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var frm = new frmCapNhapNguoiDung(idKH); // load dữ liệu có sẵn
                frm.ShowDialog();
                LoadDanhSachKhachHang(txtTimKiem.Text.Trim());
                return;
            }

            // ── Nút Không hoạt động ───────────────────────────────────────────────
            if (dgvKhachHang.Columns[e.ColumnIndex].Name == "btnKhongHD")
            {
                if (idKH == 0)
                {
                    MessageBox.Show("Không tìm thấy id khách hàng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Chuyển khách hàng \"{tenKH}\" ({maKH}) sang trạng thái Không hoạt động?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                try
                {
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        var cmd = new SqlCommand(@"
                            UPDATE KHACH_HANG 
                            SET Trang_Thai = N'Không hoạt động',
                                Nguoi_Xoa  = @NguoiXoa,
                                Ngay_Xoa   = GETDATE()
                            WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@NguoiXoa", CurrentUser.HoTen);
                        cmd.Parameters.AddWithValue("@id", idKH);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Đã chuyển \"{tenKH}\" sang Không hoạt động.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachKhachHang(txtTimKiem.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── Nút Thêm mới bên ngoài grid (btnThemMoi trong Designer) ───────────
        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            var frm = new frmCapNhapNguoiDung(0); // 0 = form trống, thêm mới
            frm.ShowDialog();
            LoadDanhSachKhachHang(txtTimKiem.Text.Trim());
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadDanhSachKhachHang(txtTimKiem.Text.Trim());
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadDanhSachKhachHang();
        }
    }
}