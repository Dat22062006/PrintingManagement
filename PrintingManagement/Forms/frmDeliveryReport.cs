using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmDeliveryReport : Form
    {
        private readonly DeliveryNoteRepository _repo = new();

        public frmDeliveryReport()
        {
            InitializeComponent();
            this.Load += frmDeliveryReport_Load;
        }

        // ─────────────────────────────────────────────────────
        // LOAD
        // ─────────────────────────────────────────────────────

        private void frmDeliveryReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            SetupFilters();

            // Mặc định: lọc 30 ngày gần nhất
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;

            LoadData();
        }

        // ─────────────────────────────────────────────────────
        // SETUP: DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvReport;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(231, 229, 255);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 32;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.Both;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var styleReadOnly = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 10f),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            var styleRight = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleRight };
            var styleCenter = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter };

            // Cột: STT
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 6,
                MinimumWidth = 50,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Mã phiếu giao
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMaPhieuGiao",
                HeaderText = "Mã phiếu giao",
                FillWeight = 16,
                MinimumWidth = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Mã báo giá
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMaBaoGia",
                HeaderText = "Mã báo giá",
                FillWeight = 14,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Ngày giao
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNgayGiao",
                HeaderText = "Ngày giao",
                FillWeight = 12,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Giờ giao
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGioGiao",
                HeaderText = "Giờ giao",
                FillWeight = 8,
                MinimumWidth = 70,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Khách hàng
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colKhachHang",
                HeaderText = "Khách hàng",
                FillWeight = 22,
                MinimumWidth = 160,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(6, 0, 4, 0) }
            });

            // Địa chỉ giao
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDiaChi",
                HeaderText = "Địa chỉ giao",
                FillWeight = 22,
                MinimumWidth = 160,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(6, 0, 4, 0) }
            });

            // Người nhận
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNguoiNhan",
                HeaderText = "Người nhận",
                FillWeight = 14,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Điện thoại
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSDT",
                HeaderText = "Điện thoại",
                FillWeight = 12,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Hình thức giao
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHinhThuc",
                HeaderText = "Hình thức",
                FillWeight = 12,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Tài xế
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTaiXe",
                HeaderText = "Tài xế",
                FillWeight = 14,
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Trạng thái
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTrangThai",
                HeaderText = "Trạng thái",
                FillWeight = 12,
                MinimumWidth = 100,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });

            // Tổng tiền
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền",
                FillWeight = 14,
                MinimumWidth = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 80, 200)
                }
            });

            // Ghi chú
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGhiChu",
                HeaderText = "Ghi chú",
                FillWeight = 16,
                MinimumWidth = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(6, 0, 4, 0) }
            });

            // Ẩn: Ngày tạo
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgayTao", Visible = false });
        }

        // ─────────────────────────────────────────────────────
        // SETUP: BỘ LỌC (DateTime + Status)
        // ─────────────────────────────────────────────────────

        private void SetupFilters()
        {
            dtpFromDate.Format = DateTimePickerFormat.Custom;
            dtpFromDate.CustomFormat = "dd/MM/yyyy";
            dtpToDate.Format = DateTimePickerFormat.Custom;
            dtpToDate.CustomFormat = "dd/MM/yyyy";

            btnView.Click -= btnView_Click;
            btnView.Click += btnView_Click;
        }

        // ─────────────────────────────────────────────────────
        // NÚT TÌM
        // ─────────────────────────────────────────────────────

        private void btnView_Click(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                MessageBox.Show("⚠️ Ngày bắt đầu không được lớn hơn ngày kết thúc!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadData();
        }

        // ─────────────────────────────────────────────────────
        // NẠP DỮ LIỆU TỪ SP
        // ─────────────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                var dt = _repo.GetDeliveryNoteList(
                    dtpFromDate.Value.Date,
                    dtpToDate.Value.Date,
                    null);

                dgvReport.Rows.Clear();

                if (dt == null || dt.Rows.Count == 0)
                {
                    lblTongCong.Text = "Tổng cộng: 0 phiếu";
                    return;
                }

                int stt = 1;
                foreach (DataRow row in dt.Rows)
                {
                    string ngayGiao = row["Ngay_Giao"] != DBNull.Value
                        ? Convert.ToDateTime(row["Ngay_Giao"]).ToString("dd/MM/yyyy") : "";

                    decimal tongTien = row["Tong_Tien"] != DBNull.Value
                        ? Convert.ToDecimal(row["Tong_Tien"]) : 0;

                    string trangThai = row["Trang_Thai"]?.ToString() ?? "";
                    Color trangThaiColor = GetStatusColor(trangThai);

                    int idx = dgvReport.Rows.Add(
                        stt,
                        row["Ma_Phieu_Giao"]?.ToString() ?? "",
                        row["Ma_Bao_Gia"]?.ToString() ?? "",
                        ngayGiao,
                        row["Gio_Giao"]?.ToString() ?? "",
                        row["Ten_KH"]?.ToString() ?? "",
                        row["Dia_Chi_Giao_Hang"]?.ToString() ?? "",
                        row["Nguoi_Nhan"]?.ToString() ?? "",
                        row["SDT_Nguoi_Nhan"]?.ToString() ?? "",
                        row["Hinh_Thuc_Giao"]?.ToString() ?? "",
                        row["Ten_Tai_Xe"]?.ToString() ?? "",
                        trangThai,
                        tongTien.ToString("N0"),
                        row["Ghi_Chu"]?.ToString() ?? ""
                    );

                    // Tô màu trạng thái
                    dgvReport.Rows[idx].Cells["colTrangThai"].Style.BackColor = trangThaiColor;
                    dgvReport.Rows[idx].Cells["colTrangThai"].Style.ForeColor = Color.White;
                    dgvReport.Rows[idx].Cells["colTrangThai"].Style.SelectionBackColor = trangThaiColor;
                    dgvReport.Rows[idx].Cells["colTrangThai"].Style.SelectionForeColor = Color.White;
                    dgvReport.Rows[idx].Cells["colTrangThai"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Tô màu dòng theo trạng thái nhạt
                    if (trangThai == "Đã giao")
                        dgvReport.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                    else if (trangThai == "Hủy")
                        dgvReport.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                    else if (trangThai == "Đang giao")
                        dgvReport.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);

                    stt++;
                }

                lblTongCong.Text = $"Tổng cộng: {stt - 1} phiếu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải báo cáo: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // MÀU TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private static Color GetStatusColor(string trangThai)
        {
            return trangThai switch
            {
                "Mới lập" => Color.FromArgb(33, 150, 243),   // Xanh dương
                "Đang giao" => Color.FromArgb(255, 152, 0),   // Cam
                "Đã giao" => Color.FromArgb(76, 175, 80),     // Xanh lá
                "Hủy" => Color.FromArgb(244, 67, 54),         // Đỏ
                _ => Color.Gray
            };
        }
    }
}
