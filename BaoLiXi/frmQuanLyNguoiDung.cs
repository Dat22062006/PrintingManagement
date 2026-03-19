using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmQuanLyNguoiDung : Form
    {
        public frmQuanLyNguoiDung()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmQuanLyNguoiDung_Load(object sender, EventArgs e)
        {
            SetupGridColumns();
            LoadDataToGrid();
            // Ép giãn cột + dòng
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Ép chiều cao header lớn hơn để chữ không bị cắt
            dgvUsers.ColumnHeadersHeight = 35;  // Tăng lên nếu chữ header vẫn bị cắt

            // Refresh và cuộn về đầu
            dgvUsers.Refresh();
            if (dgvUsers.Rows.Count > 0)
            {
                dgvUsers.FirstDisplayedScrollingRowIndex = 0;
                dgvUsers.CurrentCell = dgvUsers.Rows[0].Cells[0];
            }

            // Nếu có thanh cuộn ngang không cần thiết, tắt tự động
            dgvUsers.ScrollBars = ScrollBars.Vertical;  // Chỉ cuộn dọc, tắt ngang nếu cột fit
        }
        public void LoadDataToGrid(string search = "")
        {
            string query = @"
                SELECT 
                    id AS [ID],
                    Ten_Nguoi_Dung AS [Tên người dùng],
                    Email,
                    Vai_Tro AS [Vai trò],
                    Trang_Thai AS [Trạng thái],
                    Ngay_Tao AS [Ngày tạo]
                FROM USERS";

            if (!string.IsNullOrEmpty(search))
            {
                query += " WHERE Ten_Nguoi_Dung LIKE @search OR Email LIKE @search";
            }

            var parameters = string.IsNullOrEmpty(search)
                ? null
                : new[] { new Microsoft.Data.SqlClient.SqlParameter("@search", "%" + search + "%") };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvUsers.DataSource = dt;

            lblTongSo.Text = $"Tổng: {dt.Rows.Count} người dùng";
        }

        // ==================== THIẾT KẾ CỘT ĐẸP ====================
        public void SetupGridColumns()
        {
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Clear();
            dgvUsers.ReadOnly = true;

            // Cột ID
            var colID = new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                DataPropertyName = "ID",
                Name = "ID",  // ← Thêm dòng này để tên cột nội bộ là "ID"
                Width = 30
            };
            dgvUsers.Columns.Add(colID);

            // Cột Tên người dùng
            var colTen = new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên người dùng",
                DataPropertyName = "Tên người dùng",
                Width = 180
            };
            dgvUsers.Columns.Add(colTen);

            // Cột Email
            var colEmail = new DataGridViewTextBoxColumn
            {
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 220
            };
            dgvUsers.Columns.Add(colEmail);

            // Cột Vai trò
            var colVaiTro = new DataGridViewTextBoxColumn
            {
                HeaderText = "Vai trò",
                DataPropertyName = "Vai trò",
                Width = 120
            };
            dgvUsers.Columns.Add(colVaiTro);

            // Cột Trạng thái
            var colTrangThai = new DataGridViewTextBoxColumn
            {
                HeaderText = "Trạng thái",
                DataPropertyName = "Trạng thái",
                Width = 120,
                Name = "TrangThai"
            };
            dgvUsers.Columns.Add(colTrangThai);

            // Cột Ngày tạo
            var colNgayTao = new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày tạo",
                DataPropertyName = "Ngày tạo",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            };
            dgvUsers.Columns.Add(colNgayTao);

            // Cột Thao tác
            var colEdit = new DataGridViewButtonColumn
            {
                HeaderText = "Sửa",
                Name = "btnEdit",
                Text = "Sửa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dgvUsers.Columns.Add(colEdit);

            // Nút Xóa
            var colDelete = new DataGridViewButtonColumn
            {
                HeaderText = "Xóa",
                Name = "btnDelete",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dgvUsers.Columns.Add(colDelete);


            // Set MinimumWidth (dùng biến cột vừa tạo, không dùng tên string)
            colID.MinimumWidth = 30;
            colTen.MinimumWidth = 150;
            colEmail.MinimumWidth = 200;
            colVaiTro.MinimumWidth = 100;
            colTrangThai.MinimumWidth = 80;
            colNgayTao.MinimumWidth = 100;


            // Tô màu và sự kiện
            dgvUsers.CellFormatting += dgvUsers_CellFormatting;
            dgvUsers.CellClick += dgvUsers_CellClick;
            //hàm trạng thái

        }

        // ==================== TÔ MÀU VAI TRÒ NHƯ ẢNH ====================
        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Lấy giá trị Vai trò từ cột chỉ số 3 (điều chỉnh nếu thứ tự cột khác)
            string vaiTro = dgvUsers.Rows[e.RowIndex].Cells[3].Value?.ToString();  // 3 = cột Vai trò

            if (dgvUsers.Columns[e.ColumnIndex].Name == "Vai trò")  // Giữ kiểm tra cột Vai trò
            {
                switch (vaiTro)
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
            //hiển thị trạng thía
            // Hiển thị trạng thái
            if (dgvUsers.Columns[e.ColumnIndex].Name == "TrangThai")
            {
                if (e.Value != null)
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
        }


        // ==================== XỬ LÝ CLICK NÚT SỬA / XÓA ====================
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvUsers.Columns[e.ColumnIndex].Name;

            // Lấy ID từ cột index 0 (ID)
            int userId = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells[0].Value);

            // Lấy tên user từ cột index 1 (Tên người dùng)
            string tenUser = dgvUsers.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";

            if (columnName == "btnEdit")  // Nút Sửa
            {
                FormQuanLyNguoiDung1 frm = new FormQuanLyNguoiDung1(this, userId);
                frm.ShowDialog();
            }
            else if (columnName == "btnDelete")  // Nút Xóa
            {
                DialogResult dr = MessageBox.Show(
                    $"Bạn có chắc muốn **XÓA HOÀN TOÀN** người dùng '{tenUser}'?\n(Dữ liệu sẽ bị xóa vĩnh viễn, không khôi phục được).",
                    "Xác nhận XÓA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    string query = "DELETE FROM USERS WHERE id = @id";
                    var param = new[] { new Microsoft.Data.SqlClient.SqlParameter("@id", userId) };
                    int rows = DatabaseHelper.ExecuteNonQuery(query, param);

                    if (rows > 0)
                    {
                        MessageBox.Show("Đã xóa người dùng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataToGrid();  // Refresh danh sách
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa người dùng. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void SuaUser(int id) { /* Code sau */ }
        private void XoaUser(int id) { /* Code sau */ }

        // ==================== TÌM KIẾM ====================
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadDataToGrid(txtTimKiem.Text.Trim());
        }

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            FormQuanLyNguoiDung1 frm = new FormQuanLyNguoiDung1(this);
            frm.ShowDialog();
        }

        private void guna2HtmlLabel12_Click(object sender, EventArgs e)
        {

        }

        private void btnQuayVeMain_Click(object sender, EventArgs e)
        {
            this.Close();  // Đóng form quản lý
            frmMain main = new frmMain();
            main.Show();   // Mở dashboard
        }
    }
}


