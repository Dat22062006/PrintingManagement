// ═══════════════════════════════════════════════════════════════════
// ║  frmUserManagement.cs - DANH SÁCH QUẢN LÝ NGƯỜI DÙNG         ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Hiển thị danh sách người dùng, tìm kiếm, thêm mới,
// sửa và xóa. Toàn bộ SQL ủy thác cho UserRepository — form
// KHÔNG viết SQL trực tiếp, chống SQL Injection tuyệt đối.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmUserManagement : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ truy vấn DB — form không chạm SQL trực tiếp
        private readonly UserRepository _userRepo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmUserManagement()
        {
            InitializeComponent();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmUserManagement_Load(object sender, EventArgs e)
        {
            SetupGridColumns();
            LoadDataToGrid();

            // Ép giãn cột và dòng theo nội dung
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Tăng chiều cao header để chữ không bị cắt
            dgvUsers.ColumnHeadersHeight = 35;

            // Làm mới và cuộn về dòng đầu
            dgvUsers.Refresh();
            if (dgvUsers.Rows.Count > 0)
            {
                dgvUsers.FirstDisplayedScrollingRowIndex = 0;
                dgvUsers.CurrentCell = dgvUsers.Rows[0].Cells[0];
            }

            // Chỉ hiển thị thanh cuộn dọc — tắt ngang nếu cột vừa màn hình
            dgvUsers.ScrollBars = ScrollBars.Vertical;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU VÀO GRID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi repository lấy danh sách người dùng rồi bind vào dgvUsers.
        /// Truyền keyword để lọc, để trống để lấy tất cả.
        /// </summary>
        public void LoadDataToGrid(string keyword = "")
        {
            // Toàn bộ SQL nằm trong UserRepository — form không viết SQL
            DataTable dt = _userRepo.SearchUsers(keyword);

            dgvUsers.DataSource = dt;

            // Cập nhật nhãn tổng số người dùng
            lblTotalCount.Text = $"Tổng: {dt.Rows.Count} người dùng";
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ CỘT DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Cấu hình các cột hiển thị cho dgvUsers.
        /// Gọi 1 lần duy nhất khi load form.
        /// </summary>
        public void SetupGridColumns()
        {
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Clear();
            dgvUsers.ReadOnly = true;

            // ── CỘT ID ──
            var colId = new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                DataPropertyName = "ID",
                Name = "colId",
                Width = 30,
                MinimumWidth = 30
            };
            dgvUsers.Columns.Add(colId);

            // ── CỘT TÊN NGƯỜI DÙNG ──
            var colFullName = new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên người dùng",
                DataPropertyName = "Tên người dùng",
                Width = 180,
                MinimumWidth = 150
            };
            dgvUsers.Columns.Add(colFullName);

            // ── CỘT EMAIL ──
            var colEmail = new DataGridViewTextBoxColumn
            {
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 220,
                MinimumWidth = 200
            };
            dgvUsers.Columns.Add(colEmail);

            // ── CỘT VAI TRÒ ──
            var colRole = new DataGridViewTextBoxColumn
            {
                HeaderText = "Vai trò",
                DataPropertyName = "Vai trò",
                Width = 120,
                MinimumWidth = 100
            };
            dgvUsers.Columns.Add(colRole);

            // ── CỘT TRẠNG THÁI ──
            var colStatus = new DataGridViewTextBoxColumn
            {
                HeaderText = "Trạng thái",
                DataPropertyName = "Trạng thái",
                Name = "colStatus",
                Width = 120,
                MinimumWidth = 80
            };
            dgvUsers.Columns.Add(colStatus);

            // ── CỘT NGÀY TẠO ──
            var colCreatedDate = new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày tạo",
                DataPropertyName = "Ngày tạo",
                Width = 110,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            };
            dgvUsers.Columns.Add(colCreatedDate);

            // ── NÚT SỬA ──
            var colBtnEdit = new DataGridViewButtonColumn
            {
                HeaderText = "Sửa",
                Name = "btnEdit",
                Text = "Sửa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dgvUsers.Columns.Add(colBtnEdit);

            // ── NÚT XÓA ──
            var colBtnDelete = new DataGridViewButtonColumn
            {
                HeaderText = "Xóa",
                Name = "btnDelete",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dgvUsers.Columns.Add(colBtnDelete);

            // Đăng ký sự kiện tô màu và xử lý click
            dgvUsers.CellFormatting += dgvUsers_CellFormatting;
            dgvUsers.CellClick += dgvUsers_CellClick;
        }


        // ─────────────────────────────────────────────────────
        // TÔ MÀU VAI TRÒ VÀ TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Tô màu nền cột Vai trò và đổi text cột Trạng thái.
        /// </summary>
        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvUsers.Columns[e.ColumnIndex].Name;

            // ── TÔ MÀU CỘT VAI TRÒ ──
            if (columnName == "Vai trò")
            {
                // Lấy giá trị vai trò từ cột index 3
                string roleName = dgvUsers.Rows[e.RowIndex].Cells[3].Value?.ToString();

                switch (roleName)
                {
                    case "Admin":
                        e.CellStyle.BackColor = Color.FromArgb(185, 28, 28);
                        e.CellStyle.ForeColor = Color.White;
                        break;
                    case "Kinh doanh":
                        e.CellStyle.BackColor = Color.FromArgb(29, 78, 216);
                        e.CellStyle.ForeColor = Color.White;
                        break;
                    case "Kế toán":
                        e.CellStyle.BackColor = Color.FromArgb(22, 163, 74);
                        e.CellStyle.ForeColor = Color.White;
                        break;
                    case "Thủ kho":
                        e.CellStyle.BackColor = Color.FromArgb(234, 179, 8);
                        e.CellStyle.ForeColor = Color.Black;
                        break;
                    case "Sản xuất":
                        e.CellStyle.BackColor = Color.FromArgb(126, 34, 206);
                        e.CellStyle.ForeColor = Color.White;
                        break;
                    case "Giám đốc":
                        e.CellStyle.BackColor = Color.FromArgb(14, 116, 144);
                        e.CellStyle.ForeColor = Color.White;
                        break;
                }
            }

            // ── HIỂN THỊ TRẠNG THÁI BẰNG CHỮ VÀ MÀU ──
            if (columnName == "colStatus" && e.Value != null)
            {
                int status = Convert.ToInt32(e.Value);

                if (status == 1)
                {
                    e.Value = "Hoạt động";
                    e.CellStyle.ForeColor = Color.Green;
                }
                else
                {
                    e.Value = "Không hoạt động";
                    e.CellStyle.ForeColor = Color.Red;
                }
            }
        }


        // ─────────────────────────────────────────────────────
        // XỬ LÝ CLICK NÚT SỬA / XÓA TRÊN GRID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Xử lý sự kiện click vào nút Sửa hoặc Xóa trên từng dòng.
        /// Gọi EditUser() hoặc DeleteUser() tương ứng.
        /// </summary>
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvUsers.Columns[e.ColumnIndex].Name;

            // Lấy ID từ cột index 0
            int userId = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells[0].Value);

            // Lấy tên người dùng từ cột index 1 để hiển thị trong hộp thoại xác nhận
            string userName = dgvUsers.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";

            if (columnName == "btnEdit")
            {
                EditUser(userId);
            }
            else if (columnName == "btnDelete")
            {
                DeleteUser(userId, userName);
            }
        }


        // ─────────────────────────────────────────────────────
        // MỞ FORM SỬA NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Mở frmUserManagement1 ở chế độ sửa với ID người dùng được chọn.
        /// </summary>
        private void EditUser(int userId)
        {
            // Mở form con ở chế độ sửa — truyền ID để load dữ liệu
            frmUserDetail frm = new frmUserDetail(this, userId);
            frm.ShowDialog();
        }


        // ─────────────────────────────────────────────────────
        // XÓA NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Xác nhận rồi gọi repository xóa người dùng theo ID.
        /// Thao tác xóa vĩnh viễn — không thể hoàn tác.
        /// </summary>
        private void DeleteUser(int userId, string userName)
        {
            // Yêu cầu xác nhận trước khi xóa vĩnh viễn
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn XÓA HOÀN TOÀN người dùng '{userName}'?\n(Dữ liệu sẽ bị xóa vĩnh viễn, không khôi phục được)",
                "Xác nhận XÓA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                // Toàn bộ SQL nằm trong UserRepository — form không viết SQL
                int rowsAffected = _userRepo.DeleteById(userId);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("✅ Đã xóa người dùng thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Làm mới lại danh sách sau khi xóa
                    LoadDataToGrid();
                }
                else
                {
                    MessageBox.Show("❌ Không thể xóa người dùng. Vui lòng thử lại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xóa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // TÌM KIẾM NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Lọc danh sách theo từ khóa nhập vào txtSearch.
        /// Tự động cập nhật khi người dùng gõ.
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDataToGrid(txtSearch.Text.Trim());
        }


        // ─────────────────────────────────────────────────────
        // THÊM MỚI NGƯỜI DÙNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Mở frmUserManagement1 ở chế độ thêm mới (không truyền ID).
        /// </summary>
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            // Mở form con ở chế độ thêm mới — không truyền ID
            frmUserDetail frm = new frmUserDetail(this);
            frm.ShowDialog();
        }


        // ─────────────────────────────────────────────────────
        // QUAY VỀ DASHBOARD
        // ─────────────────────────────────────────────────────

        private void btnBackToMain_Click(object sender, EventArgs e)
        {
            // Đóng form này và mở lại dashboard
            this.Close();
            frmMain main = new frmMain();
            main.Show();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN PAINT PANEL (GIỮ NGUYÊN)
        // ─────────────────────────────────────────────────────

        private void panel1_Paint(object sender, PaintEventArgs e) { }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LABEL (GIỮ NGUYÊN)
        // ─────────────────────────────────────────────────────

        private void guna2HtmlLabel12_Click(object sender, EventArgs e) { }
    }
}