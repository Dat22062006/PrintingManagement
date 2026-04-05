// ═══════════════════════════════════════════════════════════════════
// ║  frmCustomerManagement.cs - DANH SÁCH QUẢN LÝ KHÁCH HÀNG     ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích:
// - Hiển thị danh sách khách hàng đang hoạt động
// - Hỗ trợ tìm kiếm, thêm mới, sửa và vô hiệu hóa
// - Toàn bộ SQL ủy thác cho CustomerRepository
// - Sau khi thêm/sửa/vô hiệu hóa sẽ notify frmPriceCalculation
//   để ComboBox khách hàng reload ngay nếu form đang mở
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmCustomerManagement : Form
    {
// ─────────────────────────────────────────────────────
// KHAI BÁO PRIVATE FIELD
// ─────────────────────────────────────────────────────

    // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
    private readonly CustomerRepository _repo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmCustomerManagement()
        {
            InitializeComponent();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmCustomerManagement_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadCustomers();

            // Đảm bảo grid hiển thị trên cùng
            dgvCustomers.BringToFront();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Cấu hình các cột và style cho dgvCustomers.
        /// Gọi 1 lần duy nhất khi load form.
        /// </summary>
        private void SetupGrid()
        {
            dgvCustomers.Columns.Clear();
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;

            dgvCustomers.DefaultCellStyle.Font = new Font("Segoe UI", 10.5f);
            dgvCustomers.RowTemplate.Height = 36;

            // ── STYLE HEADER ──
            dgvCustomers.EnableHeadersVisualStyles = false;
            dgvCustomers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCustomers.ColumnHeadersHeight = 44;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 175);
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            dgvCustomers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgvCustomers.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor;
            dgvCustomers.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor;

            // ── THÊM CÁC CỘT ──
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 50,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCode",
                HeaderText = "Mã KH",
                DataPropertyName = "Ma_KH",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Tên khách hàng",
                DataPropertyName = "Ten_Khach_Hang",
                Width = 250,
                ReadOnly = true
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAddress",
                HeaderText = "Địa chỉ",
                DataPropertyName = "Dia_Chi",
                Width = 300,
                ReadOnly = true
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTaxCode",
                HeaderText = "MST",
                DataPropertyName = "MST",
                Width = 130,
                ReadOnly = true
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhone",
                HeaderText = "Điện thoại",
                DataPropertyName = "Dien_Thoai",
                Width = 130,
                ReadOnly = true
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colContact",
                HeaderText = "Người liên hệ",
                DataPropertyName = "Nguoi_Lien_He",
                Width = 150,
                ReadOnly = true
            });

            // Cột ẩn lưu ID để dùng khi click nút
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = "id",
                DataPropertyName = "id",
                Visible = false
            });

            // Nút Sửa — mở form chi tiết với dữ liệu có sẵn
            dgvCustomers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEdit",
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

            // Nút Không hoạt động — vô hiệu hóa khách hàng
            dgvCustomers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnDeactivate",
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

            // Tắt sort để tránh xáo trộn thứ tự STT
            foreach (DataGridViewColumn col in dgvCustomers.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvCustomers.CellContentClick += dgvCustomers_CellContentClick;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi repository lấy danh sách khách hàng rồi bind vào dgvCustomers.
        /// Truyền keyword để lọc, để trống để lấy tất cả.
        /// </summary>
        private void LoadCustomers(string keyword = "")
        {
            try
            {
                DataTable dt = _repo.GetCustomers(keyword);
                dgvCustomers.DataSource = dt;

                // Đảm bảo cột id luôn ẩn sau khi bind
                if (dgvCustomers.Columns.Contains("colId"))
                    dgvCustomers.Columns["colId"].Visible = false;

                // Bỏ chọn dòng mặc định sau khi load
                if (dgvCustomers.Rows.Count > 0)
                {
                    dgvCustomers.ClearSelection();
                    dgvCustomers.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // XỬ LÝ CLICK NÚT SỬA / KHÔNG HOẠT ĐỘNG TRÊN GRID
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Xử lý sự kiện click vào nút Sửa hoặc Không HĐ trên từng dòng.
        /// </summary>
        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Lấy ID và thông tin khách hàng từ dòng được click
            int customerId = 0;
            string customerCode = dgvCustomers.Rows[e.RowIndex].Cells["colCode"].Value?.ToString() ?? "";
            string customerName = dgvCustomers.Rows[e.RowIndex].Cells["colName"].Value?.ToString() ?? "";

            if (dgvCustomers.Rows[e.RowIndex].Cells["colId"].Value != null)
                int.TryParse(dgvCustomers.Rows[e.RowIndex].Cells["colId"].Value.ToString(), out customerId);

            string columnName = dgvCustomers.Columns[e.ColumnIndex].Name;

            // ── NÚT SỬA — mở form chi tiết với dữ liệu có sẵn ──
            if (columnName == "btnEdit")
            {
                if (customerId == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy ID khách hàng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var frm = new frmCustomerDetail(customerId);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers(txtSearch.Text.Trim());

                    // Notify frmPriceCalculation reload ComboBox khách hàng
                    frmPriceCalculation.NotifyCustomerChanged();
                }

                return;
            }

            // ── NÚT KHÔNG HOẠT ĐỘNG — vô hiệu hóa khách hàng ──
            if (columnName == "btnDeactivate")
            {
                if (customerId == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy ID khách hàng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Chuyển khách hàng \"{customerName}\" ({customerCode}) sang trạng thái Không hoạt động?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                try
                {
                    _repo.DeactivateCustomer(customerId, CurrentUser.HoTen);

                    MessageBox.Show(
                        $"✅ Đã chuyển \"{customerName}\" sang Không hoạt động.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadCustomers(txtSearch.Text.Trim());

                    // Notify để bỏ khách hàng vừa vô hiệu hóa khỏi ComboBox
                    frmPriceCalculation.NotifyCustomerChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // ─────────────────────────────────────────────────────
        // THÊM MỚI KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Mở frmCustomerDetail ở chế độ thêm mới (truyền 0).
        /// Sau khi lưu thành công sẽ reload grid và notify frmPriceCalculation.
        /// </summary>
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            var frm = new frmCustomerDetail(0);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers(txtSearch.Text.Trim());

                // Nếu frmPriceCalculation đang mở → ComboBox KH reload ngay
                frmPriceCalculation.NotifyCustomerChanged();
            }
        }


        // ─────────────────────────────────────────────────────
        // TÌM KIẾM KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers(txtSearch.Text.Trim());
        }


        // ─────────────────────────────────────────────────────
        // LÀM MỚI — XÓA TÌM KIẾM VÀ TẢI LẠI TOÀN BỘ
        // ─────────────────────────────────────────────────────

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }
    }
}
