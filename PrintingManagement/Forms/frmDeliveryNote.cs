// ═══════════════════════════════════════════════════════════════════
// ║  frmDeliveryNote.cs - LẬP PHIẾU GIAO HÀNG                    ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] LoadQuoteDetailsForGrid: sửa thứ tự cột khi add row
// [FIX] Giờ giao: dtpDeliveryTime (Designer, cạnh Ngày giao)
// [FIX] cboShippingMethod: thêm "Khác" cho phép tự gõ
// [FIX] Nút Lưu: btnCalculate (Designer), handler btnCalculate_Click
// [FIX] dgvItems chỉ có: STT | Tên SP | ĐVT | SL giao | Tổng cộng
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PrintingManagement
{
    public partial class frmDeliveryNote : Form
    {
        // ─────────────────────────────────────────────────────
        // FIELDS
        // ─────────────────────────────────────────────────────

        private readonly DeliveryNoteRepository _repo = new();

        private int _phieuGiaoId = 0;
        private int _baoGiaId = 0;
        private int _khachHangId = 0;
        private bool _isSaved = false;

        // ─────────────────────────────────────────────────────
        // KHỞI TẠO
        // ─────────────────────────────────────────────────────

        public frmDeliveryNote()
        {
            InitializeComponent();
            this.Load += frmDeliveryNote_Load;
        }

        // ─────────────────────────────────────────────────────
        // LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmDeliveryNote_Load(object sender, EventArgs e)
        {
            SetupGrid();
            SetupCboShippingMethod();
            SetupCboStatus();
            SetupTimePicker();
            // Phải nạp báo giá trước: LoadSavedDeliveryCodes gán SelectedIndex → có thể gọi ResetForm()
            LoadCompletedQuotes();
            LoadSavedDeliveryCodes();
            GenerateDeliveryCode();

            SetPlaceholder(txtDeliveryStaff, "Họ tên tài xế / nhân viên giao hàng...");
            txtDeliveryCode.ReadOnly = true;
            dtpDeliveryDate.Value = DateTime.Today;

            btnCalculate.Click -= btnCalculate_Click;
            btnCalculate.Click += btnCalculate_Click;
            btnConfirmDelivery.Click -= btnConfirmDelivery_Click;
            btnConfirmDelivery.Click += btnConfirmDelivery_Click;
            btnCancel.Click -= btnCancel_Click;
            btnCancel.Click += btnCancel_Click;
            btnExport.Click -= btnExport_Click;
            btnExport.Click += btnExport_Click;

            UpdateButtonStates();
        }

        // ─────────────────────────────────────────────────────
        // SETUP: TIME PICKER (giờ giao)
        // [FIX] Dùng DateTimePicker ở chế độ Time thay vì TextBox
        // ─────────────────────────────────────────────────────

        private void SetupTimePicker()
        {
            dtpDeliveryTime.Format = DateTimePickerFormat.Custom;
            dtpDeliveryTime.CustomFormat = "HH:mm";
            dtpDeliveryTime.ShowUpDown = true;
            dtpDeliveryTime.Value = DateTime.Today.AddHours(12);
        }

        // ─────────────────────────────────────────────────────
        // SETUP: DATAGRIDVIEW
        // [FIX] Chỉ có 5 cột: STT | Tên SP | ĐVT | SL giao | Tổng cộng
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvItems;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
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

            // Cột hiển thị
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                FillWeight = 6,
                MinimumWidth = 50,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleReadOnly)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTenSanPham",
                HeaderText = "Tên sản phẩm",
                FillWeight = 42,
                MinimumWidth = 180,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Padding = new Padding(6, 0, 4, 0) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDVT",
                HeaderText = "ĐVT",
                FillWeight = 8,
                MinimumWidth = 60,
                ReadOnly = true,
                DefaultCellStyle = styleCenter
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                // [FIX] Chỉ có SL giao — không có SL báo giá riêng (gộp vào)
                Name = "colSLGiao",
                HeaderText = "Số lượng giao",
                FillWeight = 18,
                MinimumWidth = 120,
                DefaultCellStyle = styleRight
            });

            // Cột ẩn
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetailId", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDonGia", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSLBaoGia", Visible = false });

            dgv.CellValueChanged -= dgvItems_CellValueChanged;
            dgv.CellValueChanged += dgvItems_CellValueChanged;
        }

        // ─────────────────────────────────────────────────────
        // SETUP: COMBOBOX HÌNH THỨC GIAO
        // [FIX] Thêm "Khác" và cho phép tự gõ khi chọn "Khác"
        // ─────────────────────────────────────────────────────

        private void SetupCboShippingMethod()
        {
            cboShippingMethod.Items.Clear();
            cboShippingMethod.Items.AddRange(new string[]
            {
                "Xe công ty",
                "Thuê ngoài",
                "Khách tự lấy",
                "Gửi xe khách",
                "Khác"
            });

            // [FIX] DropDown (không phải DropDownList) để "Khác" cho phép tự gõ
            cboShippingMethod.DropDownStyle = ComboBoxStyle.DropDown;
            cboShippingMethod.SelectedIndex = 0;

            cboShippingMethod.SelectedIndexChanged -= cboShippingMethod_SelectedIndexChanged;
            cboShippingMethod.SelectedIndexChanged += cboShippingMethod_SelectedIndexChanged;
        }

        private void cboShippingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboShippingMethod.SelectedItem?.ToString() == "Khác")
            {
                // Xóa text để user tự gõ lý do
                cboShippingMethod.Text = "";
                cboShippingMethod.ForeColor = Color.FromArgb(71, 69, 94);
            }
        }

        // ─────────────────────────────────────────────────────
        // SETUP: COMBOBOX TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void SetupCboStatus()
        {
            cboStatus.Items.Clear();
            cboStatus.Items.AddRange(new string[]
            { "Mới lập", "Đang giao", "Đã giao", "Hủy" });
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.SelectedIndex = 0;

            cboStatus.SelectedIndexChanged -= cboStatus_SelectedIndexChanged;
            cboStatus.SelectedIndexChanged += cboStatus_SelectedIndexChanged;
        }

        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH PHIẾU GIAO ĐÃ LƯU (cboDeliveryCodeSave)
        // ─────────────────────────────────────────────────────

        private void LoadSavedDeliveryCodes()
        {
            cboDeliveryCodeSave.SelectedIndexChanged -= cboDeliveryCodeSave_SelectedIndexChanged;
            cboDeliveryCodeSave.Items.Clear();
            cboDeliveryCodeSave.Items.Add(new DeliveryNoteComboItem()); // dòng trống

            try
            {
                var dt = _repo.GetSavedDeliveryCodes();
                foreach (DataRow row in dt.Rows)
                {
                    cboDeliveryCodeSave.Items.Add(new DeliveryNoteComboItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        MaPhieu = row["Ma_Phieu_Giao"].ToString(),
                        NgayGiao = row["Ngay_Giao"] != DBNull.Value
                                    ? Convert.ToDateTime(row["Ngay_Giao"]).ToString("dd/MM/yyyy") : "",
                        TrangThai = row["Trang_Thai"].ToString(),
                        TenKH = row["Ten_KH"].ToString()
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }

            cboDeliveryCodeSave.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDeliveryCodeSave.SelectedIndexChanged += cboDeliveryCodeSave_SelectedIndexChanged;
            cboDeliveryCodeSave.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────
        // NẠP BÁO GIÁ HOÀN THÀNH (cboSourceOrder)
        // ─────────────────────────────────────────────────────

        private void LoadCompletedQuotes()
        {
            cboSourceOrder.SelectedIndexChanged -= cboSourceOrder_SelectedIndexChanged;
            cboSourceOrder.Items.Clear();
            cboSourceOrder.Items.Add(new QuoteDeliveryItem()); // dòng trống

            try
            {
                var dt = _repo.GetCompletedQuotes();
                foreach (DataRow row in dt.Rows)
                {
                    cboSourceOrder.Items.Add(new QuoteDeliveryItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        MaBaoGia = row["Ma_Bao_Gia"].ToString(),
                        TenSanPham = row["Ten_San_Pham"].ToString(),
                        CustomerId = row["id_Khach_Hang"] != DBNull.Value
                                       ? Convert.ToInt32(row["id_Khach_Hang"]) : 0,
                        MaKH = row["Ma_KH"]?.ToString() ?? "",
                        TenKH = row["Ten_Khach_Hang"]?.ToString() ?? "",
                        DiaChi = row["Dia_Chi"]?.ToString() ?? "",
                        NguoiLienHe = row["Nguoi_Lien_He"]?.ToString() ?? "",
                        DienThoai = row["Dien_Thoai"]?.ToString() ?? "",
                        DonGia = row["Don_Gia"] != DBNull.Value
                                       ? Convert.ToDecimal(row["Don_Gia"]) : 0
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }

            cboSourceOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSourceOrder.SelectedIndexChanged += cboSourceOrder_SelectedIndexChanged;
            cboSourceOrder.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────
        // SINH MÃ PHIẾU GIAO HÀNG (txtDeliveryCode)
        // ─────────────────────────────────────────────────────

        private void GenerateDeliveryCode()
        {
            txtDeliveryCode.Text = _repo.GenerateDeliveryCode();
        }

        // ─────────────────────────────────────────────────────
        // SỰ KIỆN: PICK PHIẾU GIAO ĐÃ LƯU
        // ─────────────────────────────────────────────────────

        private void cboDeliveryCodeSave_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDeliveryCodeSave.SelectedItem is DeliveryNoteComboItem saved && saved.Id > 0)
                LoadDeliveryNoteById(saved.Id);
            else
                ResetForm();
        }

        private void LoadDeliveryNoteById(int phieuGiaoId)
        {
            try
            {
                var ds = _repo.GetDeliveryNoteById(phieuGiaoId);
                if (ds.Tables.Count < 1 || ds.Tables[0].Rows.Count == 0) return;

                var pgh = ds.Tables[0].Rows[0];
                _phieuGiaoId = phieuGiaoId;
                _baoGiaId = pgh["id_Bao_Gia"] != DBNull.Value ? Convert.ToInt32(pgh["id_Bao_Gia"]) : 0;
                _khachHangId = pgh["id_Khach_Hang"] != DBNull.Value ? Convert.ToInt32(pgh["id_Khach_Hang"]) : 0;
                _isSaved = true;

                txtDeliveryCode.Text = pgh["Ma_Phieu_Giao"].ToString();
                txtCustomerCode.Text = pgh["Ma_KH"]?.ToString() ?? "";
                txtCustomerName.Text = pgh["Ten_KH"]?.ToString() ?? "";
                txtDeliveryAddress.Text = pgh["Dia_Chi_Giao_Hang"]?.ToString() ?? "";
                txtReceiverName.Text = pgh["Nguoi_Nhan"]?.ToString() ?? "";
                txtReceiverPhone.Text = pgh["SDT_Nguoi_Nhan"]?.ToString() ?? "";

                if (pgh["Ngay_Giao"] != DBNull.Value)
                    dtpDeliveryDate.Value = Convert.ToDateTime(pgh["Ngay_Giao"]);

                // [FIX] Load giờ giao vào DateTimePicker
                if (pgh["Gio_Giao"] != DBNull.Value)
                {
                    var gioStr = pgh["Gio_Giao"].ToString();
                    if (TimeSpan.TryParse(gioStr, out var ts))
                        dtpDeliveryTime.Value = DateTime.Today.Add(ts);
                }
                else
                {
                    dtpDeliveryTime.Value = DateTime.Today.AddHours(12);
                }

                // Hình thức giao
                string hinhThuc = pgh["Hinh_Thuc_Giao"]?.ToString() ?? "";
                bool matched = false;
                for (int i = 0; i < cboShippingMethod.Items.Count; i++)
                {
                    if (cboShippingMethod.Items[i]?.ToString() == hinhThuc)
                    {
                        cboShippingMethod.SelectedIndex = i;
                        matched = true;
                        break;
                    }
                }
                if (!matched) cboShippingMethod.Text = hinhThuc; // trường hợp "Khác - tự gõ"

                // Trạng thái
                string trangThai = pgh["Trang_Thai"]?.ToString() ?? "Mới lập";
                for (int i = 0; i < cboStatus.Items.Count; i++)
                {
                    if (cboStatus.Items[i]?.ToString() == trangThai)
                    { cboStatus.SelectedIndex = i; break; }
                }

                txtDeliveryStaff.Text = pgh["Ten_Tai_Xe"]?.ToString() ?? "";

                // PGH lưu snapshot KH; ô nào trống (dữ liệu cũ / lưu thiếu) thì lấy từ danh mục KHACH_HANG
                EnrichCustomerFieldsFromMaster();

                // Load chi tiết sản phẩm
                if (ds.Tables.Count > 1)
                {
                    dgvItems.Rows.Clear();
                    int stt = 1;
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        int slGiao = row["So_Luong_Giao"] != DBNull.Value ? Convert.ToInt32(row["So_Luong_Giao"]) : 0;
                        decimal donGia = row["Don_Gia"] != DBNull.Value ? Convert.ToDecimal(row["Don_Gia"]) : 0;
                        int slBaoGia = row["So_Luong_Bao_Gia"] != DBNull.Value ? Convert.ToInt32(row["So_Luong_Bao_Gia"]) : 0;

                        dgvItems.Rows.Add(
                            stt,                                    // colSTT
                            row["Ten_San_Pham"]?.ToString() ?? "",  // colTenSanPham
                            "Cái",                                  // colDVT
                            slGiao.ToString("N0"),                  // colSLGiao
                            Convert.ToInt32(row["id"]),             // colDetailId (ẩn)
                            donGia,                                 // colDonGia (ẩn)
                            slBaoGia                                // colSLBaoGia (ẩn)
                        );
                        stt++;
                    }
                }

                // Hiển thị đúng báo giá nguồn (không gọi SelectedIndexChanged — tránh ghi đè lưới từ báo giá)
                SyncSourceQuoteComboWithoutReload(_baoGiaId);

                UpdateButtonStates();
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        /// <summary>
        /// Điền từng ô khách hàng còn trống từ KHACH_HANG khi phiếu có id_Khach_Hang
        /// (tránh màn hình chỉ có mã KH mà thiếu tên/địa chỉ → chứng từ bán lỗi tiếp theo).
        /// </summary>
        private void EnrichCustomerFieldsFromMaster()
        {
            if (_khachHangId <= 0) return;

            try
            {
                var dt = _repo.GetCustomerById(_khachHangId);
                if (dt.Rows.Count == 0) return;
                var kh = dt.Rows[0];
                if (string.IsNullOrWhiteSpace(txtCustomerCode.Text))
                    txtCustomerCode.Text = kh["Ma_KH"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                    txtCustomerName.Text = kh["Ten_Khach_Hang"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(txtDeliveryAddress.Text))
                    txtDeliveryAddress.Text = kh["Dia_Chi"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(txtReceiverName.Text))
                    txtReceiverName.Text = kh["Nguoi_Lien_He"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(txtReceiverPhone.Text))
                    txtReceiverPhone.Text = kh["Dien_Thoai"]?.ToString() ?? "";
            }
            catch
            {
                // Không chặn mở phiếu nếu lỗi tải danh mục
            }
        }

        /// <summary>
        /// Chọn đúng dòng báo giá trên combo mà không kích hoạt load lại lưới từ báo giá.
        /// </summary>
        private void SyncSourceQuoteComboWithoutReload(int baoGiaId)
        {
            cboSourceOrder.SelectedIndexChanged -= cboSourceOrder_SelectedIndexChanged;
            try
            {
                if (baoGiaId <= 0)
                {
                    cboSourceOrder.SelectedIndex = 0;
                    return;
                }

                for (int i = 0; i < cboSourceOrder.Items.Count; i++)
                {
                    if (cboSourceOrder.Items[i] is QuoteDeliveryItem q && q.Id == baoGiaId)
                    {
                        cboSourceOrder.SelectedIndex = i;
                        return;
                    }
                }

                cboSourceOrder.SelectedIndex = 0;
            }
            finally
            {
                cboSourceOrder.SelectedIndexChanged += cboSourceOrder_SelectedIndexChanged;
            }
        }

        // ─────────────────────────────────────────────────────
        // SỰ KIỆN: CHỌN BÁO GIÁ (cboSourceOrder)
        // → Tự điền thông tin KH + load grid sản phẩm
        // ─────────────────────────────────────────────────────

        private void cboSourceOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboSourceOrder.SelectedItem is QuoteDeliveryItem quote) || quote.Id == 0)
            {
                _baoGiaId = 0;
                _khachHangId = 0;
                txtCustomerCode.Clear();
                txtCustomerName.Clear();
                txtDeliveryAddress.Clear();
                txtReceiverName.Clear();
                txtReceiverPhone.Clear();
                dgvItems.Rows.Clear();
                return;
            }

            _baoGiaId = quote.Id;
            _khachHangId = quote.CustomerId;

            // [FIX] Điền thông tin khách hàng từ báo giá
            txtCustomerCode.Text = quote.MaKH;
            txtCustomerName.Text = quote.TenKH;
            txtDeliveryAddress.Text = quote.DiaChi;
            txtReceiverName.Text = quote.NguoiLienHe;
            txtReceiverPhone.Text = quote.DienThoai;

            LoadQuoteDetailsForGrid(_baoGiaId, quote.DonGia);
        }

        // ─────────────────────────────────────────────────────
        // NẠP CHI TIẾT SẢN PHẨM VÀO GRID
        // [FIX] Sửa thứ tự cột add đúng với SetupGrid()
        // ─────────────────────────────────────────────────────

        private void LoadQuoteDetailsForGrid(int quoteId, decimal defaultDonGia = 0)
        {
            dgvItems.Rows.Clear();

            try
            {
                var dt = _repo.GetQuoteDetailsForDelivery(quoteId);
                int stt = 1;

                foreach (DataRow row in dt.Rows)
                {
                    int soLuong = row["So_Luong_Bao_Gia"] != DBNull.Value
                                  ? Convert.ToInt32(row["So_Luong_Bao_Gia"]) : 0;
                    decimal donGia = row["Don_Gia"] != DBNull.Value
                                     ? Convert.ToDecimal(row["Don_Gia"]) : defaultDonGia;
                    int detailId = row["DetailId"] != DBNull.Value
                                        ? Convert.ToInt32(row["DetailId"]) : 0;

                    // colSTT | colTenSanPham | colDVT | colSLGiao | colDetailId | colDonGia | colSLBaoGia
                    dgvItems.Rows.Add(
                        stt,                                          // colSTT
                        row["Ten_San_Pham"]?.ToString() ?? "",        // colTenSanPham
                        "Cái",                                        // colDVT
                        soLuong.ToString("N0"),                       // colSLGiao (mặc định = SL báo giá)
                        detailId,                                     // colDetailId (ẩn)
                        donGia,                                       // colDonGia (ẩn)
                        soLuong                                       // colSLBaoGia (ẩn)
                    );
                    stt++;
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        // ─────────────────────────────────────────────────────
        // SỰ KIỆN GRID: CHO PHÉP SỬA SL GIAO TRỰC TIẾP
        // ─────────────────────────────────────────────────────

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ theo dõi thay đổi, không cần tính tổng cộng
        }

        // ─────────────────────────────────────────────────────
        // CẬP NHẬT TRẠNG THÁI NÚT
        // ─────────────────────────────────────────────────────

        private void UpdateButtonStates()
        {
            string ts = cboStatus.SelectedItem?.ToString() ?? "Mới lập";

            btnCalculate.Enabled = ts != "Đã giao" && ts != "Hủy";
            btnConfirmDelivery.Enabled = ts == "Mới lập" || ts == "Đang giao";
            btnCancel.Enabled = ts != "Đã giao" && ts != "Hủy";
            btnExport.Enabled = _isSaved;
        }

        // ─────────────────────────────────────────────────────
        // NÚT LƯU PHIẾU GIAO HÀNG (btnCalculate, text "Lưu")
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Đọc số nguyên từ ô lưới: hỗ trợ 1.000 / 1,000 (phân cách hàng nghìn).
        /// </summary>
        private static bool TryParseGridInt(string? text, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var s = text.Trim().Replace("\u00A0", "").Replace(" ", "")
                .Replace(".", "").Replace(",", "");
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (_baoGiaId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn đơn hàng (báo giá) trước khi lưu!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSourceOrder.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDeliveryCode.Text))
            {
                MessageBox.Show("⚠️ Mã phiếu giao hàng không được trống!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvItems.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn đơn hàng có sản phẩm!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra SL giao > 0
            bool hasValidItem = false;
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;
                if (TryParseGridInt(row.Cells["colSLGiao"].Value?.ToString(), out int sl)
                    && sl > 0)
                { hasValidItem = true; break; }
            }
            if (!hasValidItem)
            {
                MessageBox.Show("⚠️ Vui lòng nhập Số lượng giao lớn hơn 0!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                $"Xác nhận lưu phiếu giao hàng?\n\n" +
                $"Mã phiếu: {txtDeliveryCode.Text}\n" +
                $"Khách hàng: {txtCustomerName.Text}\n" +
                $"Số sản phẩm: {dgvItems.Rows.Count}",
                "Xác nhận lưu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // Lưu không cần tổng cộng
                string gioGiao = dtpDeliveryTime.Value.ToString("HH:mm");

                var details = BuildDetailTable();

                int newId = _repo.SaveDeliveryNote(
                    _phieuGiaoId,
                    txtDeliveryCode.Text.Trim(),
                    _baoGiaId,
                    _khachHangId,
                    txtCustomerCode.Text.Trim(),
                    txtCustomerName.Text.Trim(),
                    txtDeliveryAddress.Text.Trim(),
                    txtReceiverName.Text.Trim(),
                    txtReceiverPhone.Text.Trim(),
                    dtpDeliveryDate.Value.Date,
                    gioGiao,
                    cboShippingMethod.Text.Trim(),
                    null,
                    txtDeliveryStaff.Text.Trim(),
                    0m,
                    null,
                    CurrentUser.Username,
                    details);

                _phieuGiaoId = newId;
                _isSaved = true;

                MessageBox.Show(
                    $"✅ Lưu phiếu giao hàng thành công!\n\nMã phiếu: {txtDeliveryCode.Text}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cboStatus.SelectedIndex = 0; // Mới lập
                UpdateButtonStates();
                LoadSavedDeliveryCodes();

                // Chọn đúng phiếu vừa lưu trong cboDeliveryCodeSave
                for (int i = 0; i < cboDeliveryCodeSave.Items.Count; i++)
                {
                    if (cboDeliveryCodeSave.Items[i] is DeliveryNoteComboItem item
                        && item.Id == _phieuGiaoId)
                    { cboDeliveryCodeSave.SelectedIndex = i; break; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT XÁC NHẬN ĐÃ GIAO (btnConfirmDelivery)
        // ─────────────────────────────────────────────────────

        private void btnConfirmDelivery_Click(object sender, EventArgs e)
        {
            if (_phieuGiaoId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng lưu phiếu giao hàng trước!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ts = cboStatus.SelectedItem?.ToString() ?? "";
            if (ts == "Đã giao" || ts == "Hủy")
            {
                MessageBox.Show("⚠️ Phiếu đang ở trạng thái không thể xác nhận!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                $"Xác nhận ĐÃ GIAO HÀNG?\n\nMã phiếu: {txtDeliveryCode.Text}\nKhách hàng: {txtCustomerName.Text}",
                "Xác nhận giao hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _repo.UpdateDeliveryStatus(_phieuGiaoId, "Đã giao", CurrentUser.Username);

                for (int i = 0; i < cboStatus.Items.Count; i++)
                    if (cboStatus.Items[i]?.ToString() == "Đã giao") { cboStatus.SelectedIndex = i; break; }

                UpdateButtonStates();
                LoadSavedDeliveryCodes();
                MessageBox.Show("✅ Đã xác nhận giao hàng thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT HUỶ PHIẾU GIAO (btnCancel) → trạng thái = "Hủy"
        // ─────────────────────────────────────────────────────

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_phieuGiaoId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng lưu phiếu giao hàng trước!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ts = cboStatus.SelectedItem?.ToString() ?? "";
            if (ts == "Đã giao" || ts == "Hủy")
            {
                MessageBox.Show("⚠️ Phiếu đang ở trạng thái không thể hủy!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                $"Xác nhận HỦY phiếu giao hàng?\n\nMã phiếu: {txtDeliveryCode.Text}",
                "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _repo.UpdateDeliveryStatus(_phieuGiaoId, "Hủy", CurrentUser.Username);

                for (int i = 0; i < cboStatus.Items.Count; i++)
                    if (cboStatus.Items[i]?.ToString() == "Hủy") { cboStatus.SelectedIndex = i; break; }

                UpdateButtonStates();
                LoadSavedDeliveryCodes();
                MessageBox.Show("✅ Đã hủy phiếu giao hàng!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT XUẤT EXCEL (btnExport) — Phiếu giao hàng
        // ─────────────────────────────────────────────────────

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!_isSaved || _phieuGiaoId == 0)
            {
                MessageBox.Show("⚠️ Vui lòng lưu phiếu giao hàng trước khi xuất!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"PhieuGiaoHang_{txtDeliveryCode.Text.Trim()}_{DateTime.Now:yyyyMMdd}.xlsx",
                Title = "Xuất Phiếu Giao Hàng"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                ExportToExcel(dialog.FileName);
                MessageBox.Show($"✅ Đã xuất file:\n{dialog.FileName}",
                    "Xuất Excel thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                { FileName = dialog.FileName, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xuất Excel:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────
        // XUẤT EXCEL — PHIẾU GIAO HÀNG
        // ─────────────────────────────────────────────────────

        private void ExportToExcel(string outputPath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pkg = new ExcelPackage();
            var ws = pkg.Workbook.Worksheets.Add("Phiếu Giao Hàng");

            ws.View.ShowGridLines = false;
            ws.Column(1).Width = 5;
            ws.Column(2).Width = 28;
            ws.Column(3).Width = 10;
            ws.Column(4).Width = 18;
            ws.Column(5).Width = 20;

            var navy = Color.FromArgb(30, 58, 95);
            var indigo = Color.FromArgb(100, 88, 255);
            var lightBg = Color.FromArgb(245, 247, 255);
            var gray = Color.FromArgb(100, 116, 139);
            var white = Color.White;

            // Helper: set style
            void S(ExcelRange r, Color bg, Color fg, bool bold = false, int size = 10,
                   string ha = "left", bool wrap = false, bool brd = false)
            {
                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(bg);
                r.Style.Font.Color.SetColor(fg);
                r.Style.Font.Bold = bold;
                r.Style.Font.Size = size;
                r.Style.Font.Name = "Segoe UI";
                r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                r.Style.HorizontalAlignment = ha == "center" ? ExcelHorizontalAlignment.Center
                                            : ha == "right" ? ExcelHorizontalAlignment.Right
                                            : ExcelHorizontalAlignment.Left;
                r.Style.WrapText = wrap;
                if (brd)
                {
                    var bStyle = ExcelBorderStyle.Thin;
                    var bColor = Color.FromArgb(203, 213, 225);
                    r.Style.Border.Top.Style = bStyle; r.Style.Border.Top.Color.SetColor(bColor);
                    r.Style.Border.Bottom.Style = bStyle; r.Style.Border.Bottom.Color.SetColor(bColor);
                    r.Style.Border.Left.Style = bStyle; r.Style.Border.Left.Color.SetColor(bColor);
                    r.Style.Border.Right.Style = bStyle; r.Style.Border.Right.Color.SetColor(bColor);
                }
            }

            // Helper: dòng thông tin
            void InfoRow(int row, string label, string value)
            {
                ws.Row(row).Height = 20;
                ws.Cells[row, 1, row, 2].Merge = true;
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.Font.Name = "Segoe UI";
                ws.Cells[row, 1].Style.Font.Size = 10;
                ws.Cells[row, 1].Style.Font.Color.SetColor(navy);
                ws.Cells[row, 1].Value = label;
                ws.Cells[row, 3, row, 5].Merge = true;
                ws.Cells[row, 3].Style.Font.Name = "Segoe UI";
                ws.Cells[row, 3].Style.Font.Size = 10;
                ws.Cells[row, 3].Style.Font.Color.SetColor(Color.Black);
                ws.Cells[row, 3].Value = value;
            }

            // ── HEADER ──
            ws.Row(1).Height = 30;
            ws.Cells[1, 1, 1, 4].Merge = true;
            ws.Cells[1, 1].Value = "PHIẾU GIAO HÀNG";
            S(ws.Cells[1, 1, 1, 4], indigo, white, bold: true, size: 16, ha: "center");

            ws.Row(2).Height = 18;
            ws.Cells[2, 1, 2, 4].Merge = true;
            ws.Cells[2, 1].Value = "(Kiểm tra kỹ hàng hóa trước khi ký xác nhận)";
            S(ws.Cells[2, 1, 2, 4], lightBg, navy, size: 9, ha: "center");

            // ── THÔNG TIN PHIẾU ──
            ws.Row(4).Height = 18;
            ws.Cells[4, 1, 4, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[4, 1, 4, 4].Style.Fill.BackgroundColor.SetColor(lightBg);
            ws.Cells[4, 1].Style.Font.Bold = true;
            ws.Cells[4, 1].Style.Font.Name = "Segoe UI";
            ws.Cells[4, 1].Style.Font.Color.SetColor(navy);
            ws.Cells[4, 1].Value = "THÔNG TIN PHIẾU GIAO HÀNG";

            InfoRow(5, "Mã phiếu:", txtDeliveryCode.Text.Trim());
            InfoRow(6, "Ngày giao:", dtpDeliveryDate.Value.ToString("dd/MM/yyyy"));
            InfoRow(7, "Giờ giao:", dtpDeliveryTime.Value.ToString("HH:mm"));
            InfoRow(8, "Trạng thái:", cboStatus.SelectedItem?.ToString() ?? "");

            // ── THÔNG TIN KHÁCH HÀNG ──
            ws.Row(10).Height = 18;
            ws.Cells[10, 1, 10, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[10, 1, 10, 4].Style.Fill.BackgroundColor.SetColor(lightBg);
            ws.Cells[10, 1].Style.Font.Bold = true;
            ws.Cells[10, 1].Style.Font.Name = "Segoe UI";
            ws.Cells[10, 1].Style.Font.Color.SetColor(navy);
            ws.Cells[10, 1].Value = "THÔNG TIN KHÁCH HÀNG";

            InfoRow(11, "Mã KH:", txtCustomerCode.Text.Trim());
            InfoRow(12, "Tên khách hàng:", txtCustomerName.Text.Trim());
            InfoRow(13, "Địa chỉ giao:", txtDeliveryAddress.Text.Trim());
            InfoRow(14, "Người nhận:", txtReceiverName.Text.Trim());
            InfoRow(15, "Điện thoại:", txtReceiverPhone.Text.Trim());

            // ── THÔNG TIN VẬN CHUYỂN ──
            ws.Row(17).Height = 18;
            ws.Cells[17, 1, 17, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[17, 1, 17, 4].Style.Fill.BackgroundColor.SetColor(lightBg);
            ws.Cells[17, 1].Style.Font.Bold = true;
            ws.Cells[17, 1].Style.Font.Name = "Segoe UI";
            ws.Cells[17, 1].Style.Font.Color.SetColor(navy);
            ws.Cells[17, 1].Value = "THÔNG TIN VẬN CHUYỂN";

            InfoRow(18, "Hình thức giao:", cboShippingMethod.Text.Trim());
            InfoRow(19, "Tài xế / NV giao:", txtDeliveryStaff.Text.Trim());

            // ── BẢNG SẢN PHẨM ──
            int headerRow = 21;
            ws.Row(headerRow).Height = 24;

            string[] headers = { "STT", "Tên sản phẩm", "ĐVT", "Số lượng giao" };
            string[] hAligns = { "center", "left", "center", "center" };

            S(ws.Cells[headerRow, 1, headerRow, 4], navy, white, bold: true, brd: true);
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[headerRow, i + 1].Value = headers[i];
                ws.Cells[headerRow, i + 1].Style.HorizontalAlignment =
                    hAligns[i] == "center" ? ExcelHorizontalAlignment.Center
                    : hAligns[i] == "right" ? ExcelHorizontalAlignment.Right
                    : ExcelHorizontalAlignment.Left;
            }

            int dataStart = headerRow + 1;
            int stt = 1;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;

                int rowIdx = dataStart + stt - 1;
                ws.Row(rowIdx).Height = 22;
                Color rowBg = stt % 2 == 0 ? lightBg : white;

                S(ws.Cells[rowIdx, 1], rowBg, navy, brd: true, ha: "center");
                S(ws.Cells[rowIdx, 2], rowBg, navy, brd: true);
                S(ws.Cells[rowIdx, 3], rowBg, navy, brd: true, ha: "center");
                S(ws.Cells[rowIdx, 4], rowBg, navy, brd: true, ha: "center");

                ws.Cells[rowIdx, 1].Value = stt;
                ws.Cells[rowIdx, 2].Value = row.Cells["colTenSanPham"].Value?.ToString() ?? "";
                ws.Cells[rowIdx, 3].Value = row.Cells["colDVT"].Value?.ToString() ?? "Cái";
                ws.Cells[rowIdx, 4].Value = row.Cells["colSLGiao"].Value?.ToString() ?? "0";

                stt++;
            }

            // ── CHỮ KÝ ──
            int signRow = dataStart + stt - 1 + 2;
            ws.Row(signRow).Height = 16;
            ws.Row(signRow + 1).Height = 16;
            ws.Row(signRow + 2).Height = 16;
            ws.Row(signRow + 3).Height = 18;

            string[] signers = { "Người lập phiếu", "Tài xế / NV giao", "Khách hàng" };
            // Chia 4 cột thành 3 vùng: [1-1], [2-2], [3-4]
            int[][] ranges = { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 3, 4 } };

            for (int i = 0; i < 3; i++)
            {
                int c1 = ranges[i][0], c2 = ranges[i][1];
                ws.Cells[signRow + 1, c1, signRow + 1, c2].Merge = true;
                ws.Cells[signRow + 1, c1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[signRow + 1, c1].Style.Font.Italic = true;
                ws.Cells[signRow + 1, c1].Style.Font.Size = 9;
                ws.Cells[signRow + 1, c1].Style.Font.Color.SetColor(gray);
                ws.Cells[signRow + 1, c1].Value = "(Ký, ghi rõ họ tên)";

                ws.Cells[signRow + 3, c1, signRow + 3, c2].Merge = true;
                ws.Cells[signRow + 3, c1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[signRow + 3, c1].Style.Font.Bold = true;
                ws.Cells[signRow + 3, c1].Style.Font.Size = 10;
                ws.Cells[signRow + 3, c1].Style.Font.Color.SetColor(navy);
                ws.Cells[signRow + 3, c1].Value = signers[i];
            }

            ws.PrinterSettings.Orientation = eOrientation.Portrait;
            ws.PrinterSettings.PaperSize = ePaperSize.A4;
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;

            pkg.SaveAs(new System.IO.FileInfo(outputPath));
        }

        // ─────────────────────────────────────────────────────
        // BUILD DETAIL TABLE
        // ─────────────────────────────────────────────────────

        private DataTable BuildDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("DetailId", typeof(int));
            dt.Columns.Add("TenSanPham", typeof(string));
            dt.Columns.Add("DonViTinh", typeof(string));
            dt.Columns.Add("SoLuongBaoGia", typeof(int));
            dt.Columns.Add("SoLuongGiao", typeof(int));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("ThanhTien", typeof(decimal));

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;
                string tenSP = row.Cells["colTenSanPham"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(tenSP)) continue;

                int.TryParse(row.Cells["colDetailId"].Value?.ToString(), out int detailId);
                TryParseGridInt(row.Cells["colSLGiao"].Value?.ToString(), out int slGiao);
                TryParseGridInt(row.Cells["colSLBaoGia"].Value?.ToString(), out int slBg);
                decimal.TryParse(row.Cells["colDonGia"].Value?.ToString()?.Replace(",", ""), out decimal donGia);
                decimal thanhTien = slGiao * donGia;

                dt.Rows.Add(detailId, tenSP,
                    row.Cells["colDVT"].Value?.ToString() ?? "",
                    slBg, slGiao, donGia, thanhTien);
            }

            return dt;
        }

        // ─────────────────────────────────────────────────────
        // RESET FORM
        // ─────────────────────────────────────────────────────

        private void ResetForm()
        {
            _phieuGiaoId = 0;
            _baoGiaId = 0;
            _khachHangId = 0;
            _isSaved = false;

            if (cboSourceOrder.Items.Count > 0)
                cboSourceOrder.SelectedIndex = 0;
            txtCustomerCode.Clear();
            txtCustomerName.Clear();
            txtDeliveryAddress.Clear();
            txtReceiverName.Clear();
            txtReceiverPhone.Clear();
            txtDeliveryStaff.Clear();
            dtpDeliveryDate.Value = DateTime.Today;
            dtpDeliveryTime.Value = DateTime.Today.AddHours(12);
            if (cboStatus.Items.Count > 0)
                cboStatus.SelectedIndex = 0;
            if (cboShippingMethod.Items.Count > 0)
                cboShippingMethod.SelectedIndex = 0;
            dgvItems.Rows.Clear();

            GenerateDeliveryCode();
            UpdateButtonStates();
        }

        // ─────────────────────────────────────────────────────
        // SỰ KIỆN: COMBOBOX TRẠNG THÁI
        // ─────────────────────────────────────────────────────

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        // ─────────────────────────────────────────────────────
        // PLACEHOLDER TEXT
        // ─────────────────────────────────────────────────────

        private static void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                { txt.Text = ""; txt.ForeColor = Color.Black; }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                { txt.Text = placeholder; txt.ForeColor = Color.Gray; }
            };
        }

        // ─────────────────────────────────────────────────────
        // INNER CLASS: PHIẾU GIAO TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class DeliveryNoteComboItem
        {
            public int Id { get; set; }
            public string MaPhieu { get; set; } = "";
            public string NgayGiao { get; set; } = "";
            public string TrangThai { get; set; } = "";
            public string TenKH { get; set; } = "";

            public override string ToString() =>
                Id == 0 ? "-- Chọn phiếu giao --"
                        : $"{MaPhieu}  |  {NgayGiao}  |  {TrangThai}  —  {TenKH}";
        }

        // ─────────────────────────────────────────────────────
        // INNER CLASS: BÁO GIÁ TRONG COMBOBOX
        // ─────────────────────────────────────────────────────

        private class QuoteDeliveryItem
        {
            public int Id { get; set; }
            public string MaBaoGia { get; set; } = "";
            public string TenSanPham { get; set; } = "";
            public int CustomerId { get; set; }
            public string MaKH { get; set; } = "";
            public string TenKH { get; set; } = "";
            public string DiaChi { get; set; } = "";
            public string NguoiLienHe { get; set; } = "";
            public string DienThoai { get; set; } = "";
            public decimal DonGia { get; set; }

            public override string ToString() =>
                Id == 0 ? "-- Chọn đơn hàng (báo giá hoàn thành) --"
                        : $"{MaBaoGia}  -  {TenSanPham}  ({TenKH})";
        }

        // ─────────────────────────────────────────────────────
        // HELPER: TÌM CONTROL THEO TÊN
        // ─────────────────────────────────────────────────────

        private static T? FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Name == name && c is T t) return t;
                if (c.HasChildren) { var f = FindControl<T>(c, name); if (f != null) return f; }
            }
            return null;
        }
    }
}