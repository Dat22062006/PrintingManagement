// ═══════════════════════════════════════════════════════════════════
// ║  frmUserDetail.cs - THÊM / SỬA NGƯỜI DÙNG                ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Form nhập liệu để thêm mới hoặc chỉnh sửa người dùng.
// Toàn bộ thao tác DB được ủy thác cho UserRepository — form chỉ
// xử lý UI và validate đầu vào, KHÔNG viết SQL trực tiếp tại đây.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmUserDetail : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Form cha cần được làm mới sau khi lưu thành công
        private readonly frmUserManagement _parentForm;

        // null = đang thêm mới; có giá trị = đang sửa người dùng theo ID
        private readonly int? _userId;

        // Repository xử lý toàn bộ truy vấn DB — tách biệt khỏi form
        private readonly UserRepository _userRepo = new();


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmUserDetail(frmUserManagement parent, int? id = null)
        {
            InitializeComponent();

            _parentForm = parent;
            _userId = id;

            // Nạp danh sách vai trò vào ComboBox
            LoadRoleOptions();

            // Quyết định chế độ form: thêm mới hay chỉnh sửa
            if (_userId.HasValue)
            {
                this.Text = "SỬA NGƯỜI DÙNG";
                LoadUserData(_userId.Value);
            }
            else
            {
                this.Text = "+ THÊM NGƯỜI DÙNG MỚI";
                chkIsActive.Checked = true;
            }

            // Ẩn ký tự khi nhập mật khẩu
            txtPasswordd.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH VAI TRÒ VÀO COMBOBOX
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Nạp danh sách vai trò vào cmbRole.
        /// Tập trung tại 1 chỗ — dễ bổ sung/bớt vai trò về sau.
        /// </summary>
        private void LoadRoleOptions()
        {
            cmbRole.Items.AddRange(new string[]
            {
                "Admin", "Kinh doanh", "Kế toán", "Thủ kho", "Sản xuất", "Giám đốc"
            });
            cmbRole.SelectedIndex = 0;
        }


        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU NGƯỜI DÙNG VÀO FORM (CHẾ ĐỘ SỬA)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi repository lấy thông tin người dùng theo ID,
        /// sau đó điền vào các trường trên form.
        /// Không hiển thị mật khẩu — chỉ đổi khi người dùng nhập mới.
        /// </summary>
        private void LoadUserData(int userId)
        {
            try
            {
                var dt = _userRepo.GetById(userId);

                // Không tìm thấy thì đóng form luôn
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Điền dữ liệu vào các control tương ứng
                txtFullName.Text = dt.Rows[0]["Ten_Nguoi_Dung"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                cmbRole.SelectedItem = dt.Rows[0]["Vai_Tro"].ToString();
                chkIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["Trang_Thai"]);

                // Không load mật khẩu lên form — để trống, chỉ đổi khi nhập mới
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // KIỂM TRA DỮ LIỆU ĐẦU VÀO
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Kiểm tra toàn bộ điều kiện hợp lệ trước khi lưu.
        /// Trả về true nếu hợp lệ, false nếu có lỗi (đã hiện thông báo).
        /// </summary>
        private bool ValidateInput()
        {
            // Tên và email không được để trống
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ Tên người dùng và Email!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Thêm mới bắt buộc phải nhập mật khẩu
            if (!_userId.HasValue && string.IsNullOrWhiteSpace(txtPasswordd.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập mật khẩu cho người dùng mới!",
                    "Thiếu mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Mật khẩu và xác nhận phải khớp nhau (chỉ kiểm tra khi có nhập mới)
            if (!string.IsNullOrEmpty(txtPasswordd.Text) &&
                txtPasswordd.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("⚠️ Mật khẩu xác nhận không khớp!",
                    "Mật khẩu sai", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        // ─────────────────────────────────────────────────────
        // LƯU DỮ LIỆU — THÊM MỚI HOẶC CẬP NHẬT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Xử lý sự kiện nhấn nút Save.
        /// Validate trước, sau đó gọi UserRepository thực hiện INSERT hoặc UPDATE.
        /// Form không tự viết SQL — ủy thác hoàn toàn cho repository.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Dừng lại nếu dữ liệu không hợp lệ
            if (!ValidateInput()) return;

            try
            {
                // Lấy giá trị từ các control trên form
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string role = cmbRole.SelectedItem.ToString();
                bool isActive = chkIsActive.Checked;
                string newPassword = txtPasswordd.Text.Trim();

                int rowsAffected;

                if (!_userId.HasValue)
                {
                    // ── CHẾ ĐỘ THÊM MỚI ──
                    rowsAffected = _userRepo.Insert(fullName, email, newPassword, role, isActive);
                }
                else if (!string.IsNullOrEmpty(newPassword))
                {
                    // ── CHẾ ĐỘ SỬA — CÓ ĐỔI MẬT KHẨU ──
                    rowsAffected = _userRepo.UpdateInfoWithPassword(
                        _userId.Value, fullName, email, newPassword, role, isActive);
                }
                else
                {
                    // ── CHẾ ĐỘ SỬA — KHÔNG ĐỔI MẬT KHẨU ──
                    rowsAffected = _userRepo.UpdateInfo(
                        _userId.Value, fullName, email, role, isActive);
                }

                // Thông báo kết quả cho người dùng
                if (rowsAffected > 0)
                {
                    string message = _userId.HasValue
                        ? "✅ Cập nhật người dùng thành công!"
                        : "✅ Thêm người dùng thành công!";

                    MessageBox.Show(message, "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Làm mới danh sách ở form cha rồi đóng form này
                    _parentForm.LoadDataToGrid();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ Không thể lưu dữ liệu. Vui lòng kiểm tra lại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // HỦY — ĐÓNG FORM KHÔNG LƯU
        // ─────────────────────────────────────────────────────

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmUserDetail_Load(object sender, EventArgs e) { }
    }

}