// ═══════════════════════════════════════════════════════════════════
// ║  frmCustomerDetail.cs - THÊM / SỬA KHÁCH HÀNG                ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Form nhập liệu để thêm mới hoặc chỉnh sửa khách hàng.
// Toàn bộ DB ủy thác cho CustomerRepository — form chỉ
// xử lý UI và validate đầu vào, KHÔNG viết SQL trực tiếp.
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmCustomerDetail : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly CustomerRepository _repo = new();

        // 0 = thêm mới, > 0 = đang sửa khách hàng theo ID
        private int _customerId;


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmCustomerDetail()
        {
            InitializeComponent();
            _customerId = 0;
        }

        public frmCustomerDetail(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmCustomerDetail_Load(object sender, EventArgs e)
        {
            if (_customerId > 0)
            {
                // Chế độ sửa — load dữ liệu có sẵn
                lblTitle.Text = "✏️ Sửa khách hàng";
                LoadCustomerData();
            }
            else
            {
                // Chế độ thêm mới — sinh mã tự động
                lblTitle.Text = "➕ Thêm khách hàng mới";
                txtCode.Text = _repo.GenerateCustomerCode();
                txtCode.ReadOnly = true;
            }
        }




        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU KHÁCH HÀNG VÀO FORM (CHẾ ĐỘ SỬA)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gọi repository lấy thông tin khách hàng theo ID,
        /// sau đó điền vào các trường trên form.
        /// </summary>
        private void LoadCustomerData()
        {
            try
            {
                // Toàn bộ SQL nằm trong CustomerRepository
                var dt = _repo.GetCustomerById(_customerId);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy khách hàng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Điền dữ liệu vào các control tương ứng
                var row = dt.Rows[0];
                txtCode.Text = row["Ma_KH"].ToString();
                txtCode.ReadOnly = true;
                txtName.Text = row["Ten_Khach_Hang"].ToString();
                txtAddress.Text = row["Dia_Chi"].ToString();
                txtTaxCode.Text = row["MST"].ToString();
                txtPhone.Text = row["Dien_Thoai"].ToString();
                txtEmail.Text = row["Email"] != DBNull.Value ? row["Email"].ToString() : "";
                txtContact.Text = row["Nguoi_Lien_He"].ToString();
                txtNote.Text = row["Ghi_Chu"] != DBNull.Value ? row["Ghi_Chu"].ToString() : "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─────────────────────────────────────────────────────
        // KIỂM TRA DỮ LIỆU ĐẦU VÀO
        // ─────────────────────────────────────────────────────

        private bool ValidateInput()
        {
            // Tên khách hàng không được để trống
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("⚠️ Vui lòng nhập tên khách hàng!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            return true;
        }


        // ─────────────────────────────────────────────────────
        // LƯU DỮ LIỆU — THÊM MỚI HOẶC CẬP NHẬT
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Validate trước, sau đó gọi CustomerRepository lưu.
        /// Form không viết SQL — ủy thác hoàn toàn cho repository.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Toàn bộ SQL nằm trong CustomerRepository
                _repo.SaveCustomer(
                    _customerId,
                    txtCode.Text.Trim(),
                    txtName.Text.Trim(),
                    txtAddress.Text.Trim(),
                    txtTaxCode.Text.Trim(),
                    txtPhone.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtContact.Text.Trim(),
                    txtNote.Text.Trim(),
                    CurrentUser.HoTen);

                MessageBox.Show(
                    _customerId == 0
                        ? "✅ Thêm khách hàng thành công!"
                        : "✅ Cập nhật thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi",
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
    }
}