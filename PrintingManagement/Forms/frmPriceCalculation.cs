// ═══════════════════════════════════════════════════════════════════
// ║  frmPriceCalculation.cs - TÍNH GIÁ SẢN PHẨM IN                ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] Lỗi tạo KH trùng: _selectedCustomerId phải > 0 khi đã chọn
//       từ ComboBox thì KHÔNG hỏi lưu KH mới.
//       sp_SaveNewQuote giờ nhận customerId thay vì tên KH để tránh
//       tạo bản ghi trùng trong DB.
// [FIX] ComboBox loại giấy: thêm dòng "✏ Tự chỉnh..." ở cuối,
//       khi chọn thì cmbPaperType trống để user gõ tên mới vào,
//       ParseGsmFromName gọi theo TextChanged thay vì chỉ Leave.
// [FIX] btnQuickCalc: lưu nhiều CHI_TIET_BAO_GIA cho 1 BAO_GIA,
//       sau đó mở frmQuoteManagement và reload grid.
// ═══════════════════════════════════════════════════════════════════
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPriceCalculation : Form
    {
        // ─────────────────────────────────────────────────────
        // PRIVATE FIELDS
        // ─────────────────────────────────────────────────────

        private readonly PriceCalculationRepository _repo = new();
        private readonly CustomerRepository _customerRepo = new();
        private readonly PaperTypeRepository _paperRepo = new();
        private readonly QuoteDraftRepository _draftRepo = new();

        private int _quoteId = 0;
        private int _selectedCustomerId = 0;   // > 0 = đã chọn KH có sẵn
        private DataTable _customerTable = new DataTable();
        private DataTable _paperTypeTable = new DataTable();

        // ─────────────────────────────────────────────────────
        // STATIC EVENT — đồng bộ KH với frmCustomerManagement
        // ─────────────────────────────────────────────────────

        public static event EventHandler CustomerListChanged;

        public static void NotifyCustomerChanged()
            => CustomerListChanged?.Invoke(null, EventArgs.Empty);

        // ─────────────────────────────────────────────────────
        // KHỞI TẠO
        // ─────────────────────────────────────────────────────

        public frmPriceCalculation() { InitializeComponent(); }

        public frmPriceCalculation(int quoteId)
        {
            InitializeComponent();
            _quoteId = quoteId;
        }

        // ─────────────────────────────────────────────────────
        // LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmPriceCalculation_Load(object sender, EventArgs e)
        {
            CustomerListChanged -= OnCustomerListChanged;
            CustomerListChanged += OnCustomerListChanged;

            LoadCustomerComboBox();
            LoadPaperTypeComboBox();
            LoadLaminateOptions();
            LoadPrintMachineOptions();

            if (_quoteId > 0) LoadQuoteData();
            else { ResetFormDefaults(); SetNewQuoteDefaults(); }

            dtpQuoteDate.Value = DateTime.Now;
            UpdatePlatePrice();
        }

        private void frmPriceCalculation_FormClosed(object sender, FormClosedEventArgs e)
        {
            CustomerListChanged -= OnCustomerListChanged;
        }

        private void OnCustomerListChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired) this.Invoke(new Action(LoadCustomerComboBox));
            else LoadCustomerComboBox();
        }

        // ─────────────────────────────────────────────────────
        // COMBOBOX KHÁCH HÀNG
        // ─────────────────────────────────────────────────────

        private void LoadCustomerComboBox()
        {
            try
            {
                // Lưu lại code đang chọn để restore sau khi reload
                string currentCode = (cboCustomer.SelectedItem is CustomerItem cur) ? cur.Code : "";

                cboCustomer.SelectedIndexChanged -= cboCustomer_SelectedIndexChanged;

                _customerTable = _customerRepo.GetAllActiveCustomers();

                cboCustomer.Items.Clear();
                cboCustomer.Items.Add(new CustomerItem());  // "(-- Khách lẻ --)"

                foreach (DataRow row in _customerTable.Rows)
                {
                    cboCustomer.Items.Add(new CustomerItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["Ma_KH"].ToString(),
                        Name = row["Ten_Khach_Hang"].ToString(),
                        Address = row["Dia_Chi"].ToString(),
                        TaxCode = row["MST"].ToString()
                    });
                }

                cboCustomer.DropDownStyle = ComboBoxStyle.DropDown;
                cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;

                // Restore lựa chọn cũ
                bool restored = false;
                if (!string.IsNullOrEmpty(currentCode))
                {
                    for (int i = 0; i < cboCustomer.Items.Count; i++)
                    {
                        if (cboCustomer.Items[i] is CustomerItem ci && ci.Code == currentCode)
                        {
                            cboCustomer.SelectedIndex = i;
                            restored = true;
                            break;
                        }
                    }
                }
                if (!restored) cboCustomer.SelectedIndex = 0;

                cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Không tải được danh sách KH: {ex.Message}", "Cảnh báo");
            }
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // lưu draft cho KH đang làm trước khi đổi KH
            SaveDraftForCurrentCustomer();

            if (!(cboCustomer.SelectedItem is CustomerItem ci)) return;

            _selectedCustomerId = ci.Id;

            if (ci.Id == 0)
            {
                txtCustomerName.Clear();
                txtAddress.Clear();
            }
            else
            {
                txtCustomerName.Text = ci.Name;
                txtAddress.Text = ci.Address;

                // NEW: restore draft của KH vừa chọn (kể cả tắt app mở lại)
                RestoreDraftForSelectedCustomer();
            }
        }
        private class QuoteDraft
        {
            // Header / sản phẩm
            public string QuoteDate { get; set; } = "";        // dtpQuoteDate (yyyy-MM-dd)
            public string ProductName { get; set; } = "";
            public string ProductSize { get; set; } = "";
            public string Quantity { get; set; } = "";
            public string Quantities { get; set; } = "";
            public string LayoutCount { get; set; } = "";
            public string ColorCount { get; set; } = "";
            public string DeliveryTime { get; set; } = "";
            public string ValidityDays { get; set; } = "";
            public string ProfitPercent { get; set; } = "";

            // Giấy / khổ in
            public string PaperTypeText { get; set; } = "";    // cmbPaperType.Text
            public string PaperGsm { get; set; } = "";
            public string PaperPricePerTon { get; set; } = "";
            public string WastageSheets { get; set; } = "";
            public string PrintSizeW { get; set; } = "";
            public string PrintSizeH { get; set; } = "";

            // Máy in / kẽm
            public string PrintMachineText { get; set; } = ""; // cmbPrintMachine.Text
            public string PlatePrice { get; set; } = "";       // txtPlatePrice.Text

            // Cán màng / gia công
            public string LaminateText { get; set; } = "";     // cmbLaminate.Text
            public string LaminatePrice { get; set; } = "";    // txtLaminatePrice.Text
            public string LaminateSides { get; set; } = "";    // txtLaminateSides.Text

            public string MetalizePrice { get; set; } = "";
            public string MetalizeSides { get; set; } = "";

            public string UvPrice { get; set; } = "";
            public string UvSides { get; set; } = "";

            public string DieCost { get; set; } = "";          // txtDieCost (tiền khuôn bế)

            // Phụ kiện / chi phí khác
            public string RibbonPrice { get; set; } = "";
            public string ButtonPrice { get; set; } = "";
            public string BoxPrice { get; set; } = "";
            public string BoxCapacity { get; set; } = "";
            public string DeliveryCost { get; set; } = "";
            public string ProofCost { get; set; } = "";
        }
        private void SaveDraftForCurrentCustomer()
        {
            if (_selectedCustomerId <= 0) return;
            if (_quoteId > 0) return; // đang sửa báo giá thật thì không lưu draft

            var d = new QuoteDraft
            {
                QuoteDate = dtpQuoteDate.Value.Date.ToString("yyyy-MM-dd"),

                ProductName = txtProductName.Text,
                ProductSize = txtProductSize.Text,
                Quantity = txtQuantity.Text,
                Quantities = txtQuantities.Text,
                LayoutCount = txtLayoutCount.Text,
                ColorCount = txtColorCount.Text,
                DeliveryTime = txtDeliveryTime.Text,
                ValidityDays = txtValidityDays.Text,
                ProfitPercent = txtProfitPercent.Text,

                PaperTypeText = cmbPaperType.Text,
                PaperGsm = txtPaperGsm.Text,
                PaperPricePerTon = txtPaperPricePerTon.Text,
                WastageSheets = txtWastageSheets.Text,
                PrintSizeW = txtPrintSizeW.Text,
                PrintSizeH = txtPrintSizeH.Text,

                PrintMachineText = cmbPrintMachine.Text,
                PlatePrice = txtPlatePrice.Text,

                LaminateText = cmbLaminate.Text,
                LaminatePrice = txtLaminatePrice.Text,
                LaminateSides = txtLaminateSides.Text,

                MetalizePrice = txtMetalizePrice.Text,
                MetalizeSides = txtMetalizeSides.Text,

                UvPrice = txtUvPrice.Text,
                UvSides = txtUvSides.Text,

                DieCost = txtDieCost.Text,

                RibbonPrice = txtRibbonPrice.Text,
                ButtonPrice = txtButtonPrice.Text,
                BoxPrice = txtBoxPrice.Text,
                BoxCapacity = txtBoxCapacity.Text,
                DeliveryCost = txtDeliveryCost.Text,
                ProofCost = txtProofCost.Text
            };

            string json = JsonSerializer.Serialize(d);
            _draftRepo.SaveDraft(_selectedCustomerId, CurrentUser.HoTen, json);
        }

        private void RestoreDraftForSelectedCustomer()
        {
            if (_selectedCustomerId <= 0) return;
            if (_quoteId > 0) return;

            string json = _draftRepo.GetDraftJson(_selectedCustomerId, CurrentUser.HoTen);
            if (string.IsNullOrWhiteSpace(json)) return;

            QuoteDraft d;
            try
            {
                d = JsonSerializer.Deserialize<QuoteDraft>(json) ?? new QuoteDraft();
            }
            catch
            {
                return;
            }

            static void SetIfNotEmptyText(TextBox tb, string v)
            {
                if (!string.IsNullOrWhiteSpace(v)) tb.Text = v;
            }

            static void SetIfNotEmptyCombo(ComboBox cb, string v)
            {
                if (!string.IsNullOrWhiteSpace(v)) cb.Text = v;
            }

            if (DateTime.TryParse(d.QuoteDate, out var qd))
                dtpQuoteDate.Value = qd;

            // TextBox
            SetIfNotEmptyText(txtProductName, d.ProductName);
            SetIfNotEmptyText(txtProductSize, d.ProductSize);
            SetIfNotEmptyText(txtQuantity, d.Quantity);
            SetIfNotEmptyText(txtQuantities, d.Quantities);
            SetIfNotEmptyText(txtLayoutCount, d.LayoutCount);
            SetIfNotEmptyText(txtColorCount, d.ColorCount);
            SetIfNotEmptyText(txtDeliveryTime, d.DeliveryTime);
            SetIfNotEmptyText(txtValidityDays, d.ValidityDays);
            SetIfNotEmptyText(txtProfitPercent, d.ProfitPercent);

            SetIfNotEmptyText(txtPaperGsm, d.PaperGsm);
            SetIfNotEmptyText(txtPaperPricePerTon, d.PaperPricePerTon);
            SetIfNotEmptyText(txtWastageSheets, d.WastageSheets);
            SetIfNotEmptyText(txtPrintSizeW, d.PrintSizeW);
            SetIfNotEmptyText(txtPrintSizeH, d.PrintSizeH);

            SetIfNotEmptyText(txtPlatePrice, d.PlatePrice);
            SetIfNotEmptyText(txtLaminatePrice, d.LaminatePrice);
            SetIfNotEmptyText(txtLaminateSides, d.LaminateSides);

            SetIfNotEmptyText(txtMetalizePrice, d.MetalizePrice);
            SetIfNotEmptyText(txtMetalizeSides, d.MetalizeSides);

            SetIfNotEmptyText(txtUvPrice, d.UvPrice);
            SetIfNotEmptyText(txtUvSides, d.UvSides);

            SetIfNotEmptyText(txtDieCost, d.DieCost);

            SetIfNotEmptyText(txtRibbonPrice, d.RibbonPrice);
            SetIfNotEmptyText(txtButtonPrice, d.ButtonPrice);
            SetIfNotEmptyText(txtBoxPrice, d.BoxPrice);
            SetIfNotEmptyText(txtBoxCapacity, d.BoxCapacity);
            SetIfNotEmptyText(txtDeliveryCost, d.DeliveryCost);
            SetIfNotEmptyText(txtProofCost, d.ProofCost);

            // ComboBox
            SetIfNotEmptyCombo(cmbPaperType, d.PaperTypeText);
            SetIfNotEmptyCombo(cmbPrintMachine, d.PrintMachineText);
            SetIfNotEmptyCombo(cmbLaminate, d.LaminateText);
        }
        private void cboCustomer_TextChanged(object sender, EventArgs e)
        {
            // Nếu đang chọn từ list thì không coi là gõ tay
            if (cboCustomer.SelectedItem is CustomerItem)
                return;

            // Tránh reset khi đang mở dropdown/autocomplete
            if (cboCustomer.DroppedDown)
                return;

            // Chỉ coi là gõ tay khi không có SelectedItem thật sự
            _selectedCustomerId = 0;
            txtCustomerName.Text = cboCustomer.Text;
        }

        // ─────────────────────────────────────────────────────
        // [FIX] COMBOBOX LOẠI GIẤY — thêm "✏ Tự chỉnh..."
        // ─────────────────────────────────────────────────────

        private const string CUSTOM_PAPER = "✏ Tự chỉnh...";

        private void LoadPaperTypeComboBox()
        {
            try
            {
                _paperTypeTable = _paperRepo.GetAllPaperTypes();

                cmbPaperType.SelectedIndexChanged -= cmbPaperType_SelectedIndexChanged;
                string currentText = cmbPaperType.Text;

                cmbPaperType.Items.Clear();

                foreach (DataRow row in _paperTypeTable.Rows)
                    cmbPaperType.Items.Add(row["Ten_Loai_Giay"].ToString());

                // Luôn có dòng "Tự chỉnh..." ở cuối
                cmbPaperType.Items.Add(CUSTOM_PAPER);

                cmbPaperType.DropDownStyle = ComboBoxStyle.DropDown;
                cmbPaperType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbPaperType.AutoCompleteSource = AutoCompleteSource.ListItems;

                // Restore
                if (!string.IsNullOrEmpty(currentText) && currentText != CUSTOM_PAPER)
                    cmbPaperType.Text = currentText;
                else if (cmbPaperType.Items.Count > 1)
                    cmbPaperType.SelectedIndex = 0;

                cmbPaperType.SelectedIndexChanged += cmbPaperType_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Không tải được loại giấy: {ex.Message}", "Cảnh báo");
            }
        }

        private void cmbPaperType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbPaperType.Text.Trim();

            // [FIX] Nếu chọn "Tự chỉnh..." → xóa text để user gõ tên giấy mới
            if (selected == CUSTOM_PAPER)
            {
                cmbPaperType.Text = "";
                txtPaperGsm.Text = "";
                cmbPaperType.Focus();
                return;
            }

            if (string.IsNullOrEmpty(selected)) return;

            // Tìm trong bảng giấy đã load
            DataRow[] rows = _paperTypeTable.Select(
                $"Ten_Loai_Giay = '{selected.Replace("'", "''")}'");

            if (rows.Length > 0)
            {
                var row = rows[0];
                txtPaperGsm.Text = row["Dinh_Luong"].ToString();
                txtPaperPricePerTon.Text = row["Gia_Tan_Mac_Dinh"].ToString();
                txtWastageSheets.Text = row["Bu_Hao_Mac_Dinh"].ToString();

                string khoIn = row["Kho_In_Mac_Dinh"].ToString();
                if (!string.IsNullOrEmpty(khoIn) && khoIn.Contains(" x "))
                {
                    var parts = khoIn.Split('x');
                    if (parts.Length == 2)
                    {
                        txtPrintSizeW.Text = parts[0].Trim();
                        txtPrintSizeH.Text = parts[1].Trim();
                    }
                }
            }
        }

        // [FIX] Parse gsm khi user gõ xong (TextChanged + Leave)
        private void cmbPaperType_TextChanged(object sender, EventArgs e)
        {
            string text = cmbPaperType.Text.Trim();
            if (string.IsNullOrEmpty(text) || text == CUSTOM_PAPER) return;

            // Chỉ parse nếu text KHÔNG khớp với item trong list (là tên giấy mới)
            bool isInList = cmbPaperType.Items.Cast<string>()
                .Any(item => string.Equals(item, text, StringComparison.OrdinalIgnoreCase)
                             && item != CUSTOM_PAPER);

            if (!isInList)
                ParseGsmFromName(text);
        }

        private void cmbPaperType_Leave(object sender, EventArgs e)
        {
            string text = cmbPaperType.Text.Trim();
            if (!string.IsNullOrEmpty(text) && text != CUSTOM_PAPER
                && !cmbPaperType.Items.Cast<string>().Any(
                    i => string.Equals(i, text, StringComparison.OrdinalIgnoreCase)))
            {
                ParseGsmFromName(text);
            }
        }

        /// <summary>
        /// Parse gsm từ tên giấy.
        /// "Quario 250 gsm" → txtPaperGsm = "250"
        /// "Art Paper 300gsm" → txtPaperGsm = "300"
        /// </summary>
        private void ParseGsmFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            string lower = name.ToLower().Replace("gsm", " gsm ");
            string[] parts = lower.Split(
                new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i + 1] == "gsm" && double.TryParse(parts[i], out double gsm))
                {
                    txtPaperGsm.Text = gsm.ToString();
                    return;
                }
            }

            // Fallback: số trong khoảng gsm hợp lệ
            foreach (var part in parts)
                if (double.TryParse(part, out double gsm) && gsm >= 80 && gsm <= 600)
                {
                    txtPaperGsm.Text = gsm.ToString();
                    return;
                }
        }

        // ─────────────────────────────────────────────────────
        // COMBOBOX LAMINATE VÀ MÁY IN
        // ─────────────────────────────────────────────────────

        private void LoadLaminateOptions()
        {
            cmbLaminate.Items.AddRange(new string[] {
                "Không cán",
                "Cán màng bóng (1,900đ/m²)",
                "Cán màng mờ (2,000đ/m²)",
                "Tự chỉnh - cán màng bóng",
                "Tự chỉnh - cán màng mờ"
            });
            cmbLaminate.SelectedIndex = 0;
            txtLaminatePrice.Text = "0";
            txtLaminatePrice.ReadOnly = true;
        }

        private void LoadPrintMachineOptions()
        {
            cmbPrintMachine.Items.AddRange(new string[] {
                "Máy lớn (100.000đ/kẽm)",
                "Máy nhỏ (60.000đ/kẽm)",
                "Tự chỉnh - Máy Lớn",
                "Tự chỉnh - Máy Nhỏ"
            });
            cmbPrintMachine.SelectedIndex = 0;
        }

        private void UpdatePlatePrice()
        {
            string s = cmbPrintMachine.SelectedItem?.ToString() ?? "";
            if (s.Contains("Máy lớn")) { txtPlatePrice.Text = "100000"; txtPlatePrice.ReadOnly = true; }
            else if (s.Contains("Máy nhỏ")) { txtPlatePrice.Text = "60000"; txtPlatePrice.ReadOnly = true; }
            else if (s.Contains("Tự chỉnh")) { txtPlatePrice.Text = ""; txtPlatePrice.ReadOnly = false; }
            else { txtPlatePrice.Text = "0"; txtPlatePrice.ReadOnly = true; }
        }

        private void cmbPrintMachine_SelectedIndexChanged(object sender, EventArgs e)
            => UpdatePlatePrice();

        private void cmbLaminate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = cmbLaminate.SelectedItem?.ToString() ?? "";
            if (s == "Cán màng bóng (1,900đ/m²)") { txtLaminatePrice.Text = "1900"; txtLaminatePrice.ReadOnly = true; }
            else if (s == "Cán màng mờ (2,000đ/m²)") { txtLaminatePrice.Text = "2000"; txtLaminatePrice.ReadOnly = true; }
            else if (s.Contains("Tự chỉnh")) { txtLaminatePrice.Text = ""; txtLaminatePrice.ReadOnly = false; }
            else { txtLaminatePrice.Text = "0"; txtLaminatePrice.ReadOnly = true; }
        }

        // ─────────────────────────────────────────────────────
        // LOAD DỮ LIỆU BÁO GIÁ (CHẾ ĐỘ SỬA)
        // ─────────────────────────────────────────────────────

        private void LoadQuoteData()
        {
            try
            {
                var dt = _repo.GetQuoteById(_quoteId);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ Không tìm thấy báo giá!");
                    this.Close(); return;
                }

                var row = dt.Rows[0];

                // NEW: đồng bộ customerId + chọn đúng KH trên ComboBox
                _selectedCustomerId =
                    row.Table.Columns.Contains("id_Khach_Hang") && row["id_Khach_Hang"] != DBNull.Value
                        ? Convert.ToInt32(row["id_Khach_Hang"])
                        : 0;

                // Tạm ngắt event để tránh TextChanged/SelectedIndexChanged làm lệch trạng thái
                cboCustomer.SelectedIndexChanged -= cboCustomer_SelectedIndexChanged;

                if (_selectedCustomerId > 0)
                {
                    bool found = false;
                    for (int i = 0; i < cboCustomer.Items.Count; i++)
                    {
                        if (cboCustomer.Items[i] is CustomerItem ci && ci.Id == _selectedCustomerId)
                        {
                            cboCustomer.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        cboCustomer.SelectedIndex = 0;
                        _selectedCustomerId = 0;
                        txtCustomerName.Text = row["Ten_Khach_Hang"]?.ToString() ?? "";
                        txtAddress.Text = row["Dia_Chi"]?.ToString() ?? "";
                    }
                }
                else
                {
                    cboCustomer.SelectedIndex = 0;
                    txtCustomerName.Text = row["Ten_Khach_Hang"]?.ToString() ?? "";
                    txtAddress.Text = row["Dia_Chi"]?.ToString() ?? "";
                }

                cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;

                // Các field báo giá
                txtProductName.Text = row["Ten_San_Pham"]?.ToString() ?? "";
                txtProductSize.Text = row["Kich_Thuoc_Thanh_Pham"]?.ToString() ?? "";
                txtPaperGsm.Text = row["Khoi_Luong_Giay"]?.ToString() ?? "";
                txtLayoutCount.Text = row["So_Con"]?.ToString() ?? "1";
                txtPaperPricePerTon.Text = row["Gia_Giay_Tan"]?.ToString() ?? "";
                txtProfitPercent.Text = row["Loi_Nhuan_Phan_Tram"]?.ToString() ?? "20";
                txtColorCount.Text = row["So_Mau_In"]?.ToString() ?? "";
                txtDeliveryTime.Text = row["Thoi_Gian_Giao_Hang_Du_Kien"]?.ToString() ?? "";
                txtValidityDays.Text = row["Hieu_Luc_Bao_Gia_Ngay"]?.ToString() ?? "30";

                string paperType = row["Ten_Loai_Giay"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(paperType))
                    cmbPaperType.Text = paperType;

                string printSize = row["Kho_In"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(printSize) && printSize.Contains(" x "))
                {
                    var parts = printSize.Split('x');
                    if (parts.Length == 2)
                    {
                        txtPrintSizeW.Text = parts[0].Trim();
                        txtPrintSizeH.Text = parts[1].Trim();
                    }
                }

                if (row["Ngay_Bao_Gia"] != DBNull.Value)
                    dtpQuoteDate.Value = Convert.ToDateTime(row["Ngay_Bao_Gia"]);

                if (row.Table.Columns.Contains("So_Luong") && row["So_Luong"] != DBNull.Value)
                    txtQuantity.Text = row["So_Luong"].ToString();

                if (row.Table.Columns.Contains("Tien_Khuon_Be") && row["Tien_Khuon_Be"] != DBNull.Value)
                    txtDieCost.Text = row["Tien_Khuon_Be"].ToString();

                if (row.Table.Columns.Contains("Tien_Can_Mang") && row["Tien_Can_Mang"] != DBNull.Value)
                    txtLaminatePrice.Text = row["Tien_Can_Mang"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
                this.Close();
            }
        }

        private void ResetFormDefaults()
        {
            txtCustomerName.Clear(); txtProductName.Clear();
            txtAddress.Clear(); txtProductSize.Clear();
            txtQuantity.Clear(); txtLayoutCount.Clear();
        }

        private void SetNewQuoteDefaults()
        {
            txtValidityDays.Text = "30";
            txtDeliveryTime.Text = "7-10 ngày làm việc";
            txtWastageSheets.Text = "500";
            txtProfitPercent.Text = "20";
            txtLaminateSides.Text = "1";
            txtMetalizePrice.Text = "5000";
            txtMetalizeSides.Text = "1";
            txtUvPrice.Text = "5500";
            txtUvSides.Text = "1";
            txtRibbonPrice.Text = "550";
            txtButtonPrice.Text = "360";
            txtBoxPrice.Text = "20000";
            txtBoxCapacity.Text = "500";
            txtProofCost.Text = "100000";
            txtDeliveryCost.Text = "500000";
        }

        // ─────────────────────────────────────────────────────
        // VALIDATE INPUT
        // ─────────────────────────────────────────────────────

        // ═══════════════════════════════════════════════════════════════════
        // [FIX] PATCH cho frmPriceCalculation.cs
        // VẤN ĐỀ: btnCalculate_Click không lưu vì TryParseInputs() return false
        //          khi các field như txtDieCost, txtRibbonPrice... bị rỗng.
        //          Hàm dùng && liên tiếp → 1 field lỗi là dừng hết, không báo field nào sai.
        // GIẢI PHÁP: Tách validate từng field, thông báo rõ field nào bị lỗi.
        // ═══════════════════════════════════════════════════════════════════

        // ── THAY TOÀN BỘ hàm TryParseInputs() bằng hàm dưới đây ──────────

        private bool TryParseInputs(
            out int qty, out int layout, out double wastage,
            out double gsm, out double priceTon,
            out double pw, out double ph,
            out double laminateP, out double laminateS,
            out double metalizeP, out double metalizeS,
            out double uvP, out double uvS, out double die,
            out double ribbon, out double button,
            out double boxP, out double boxCap,
            out double delivery, out double proof, out double profit)
        {
            // Khởi tạo out params
            qty = layout = 0;
            wastage = gsm = priceTon = pw = ph = 0;
            laminateP = laminateS = metalizeP = metalizeS = 0;
            uvP = uvS = die = ribbon = button = boxP = boxCap = delivery = proof = profit = 0;

            var errors = new System.Text.StringBuilder();

            // ── Helper parse: nếu rỗng thì dùng defaultVal, nếu có text mà sai format thì báo lỗi
            bool TryGet(string text, string fieldName, out double val, double defaultVal = 0)
            {
                text = text?.Trim() ?? "";
                if (string.IsNullOrEmpty(text)) { val = defaultVal; return true; }
                if (double.TryParse(text, out val)) return true;
                errors.AppendLine($"  • {fieldName}: \"{text}\" không phải số hợp lệ");
                return false;
            }
            bool TryGetInt(string text, string fieldName, out int val, int defaultVal = 0)
            {
                text = text?.Trim() ?? "";
                if (string.IsNullOrEmpty(text)) { val = defaultVal; return true; }
                if (int.TryParse(text, out val)) return true;
                errors.AppendLine($"  • {fieldName}: \"{text}\" không phải số nguyên hợp lệ");
                return false;
            }

            bool ok = true;

            // Bắt buộc — không cho phép rỗng
            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
                errors.AppendLine("  • Số lượng: không được bỏ trống");
            else if (!int.TryParse(txtQuantity.Text.Trim(), out qty) || qty <= 0)
                errors.AppendLine("  • Số lượng: phải là số nguyên dương");

            if (!TryGetInt(txtLayoutCount.Text, "Số con", out layout, 1)) ok = false;
            if (layout <= 0) { layout = 1; }

            if (!TryGet(txtWastageSheets.Text, "Bù hao", out wastage, 500)) ok = false;
            if (!TryGet(txtPaperGsm.Text, "Định lượng giấy (gsm)", out gsm)) ok = false;
            if (!TryGet(txtPaperPricePerTon.Text, "Giá giấy/tấn", out priceTon)) ok = false;
            if (!TryGet(txtPrintSizeW.Text, "Khổ in W", out pw)) ok = false;
            if (!TryGet(txtPrintSizeH.Text, "Khổ in H", out ph)) ok = false;

            // Có thể rỗng → mặc định 0
            if (!TryGet(txtLaminatePrice.Text, "Đơn giá cán màng", out laminateP, 0)) ok = false;
            if (!TryGet(txtLaminateSides.Text, "Số mặt cán", out laminateS, 1)) ok = false;
            if (!TryGet(txtMetalizePrice.Text, "Đơn giá metalize", out metalizeP, 0)) ok = false;
            if (!TryGet(txtMetalizeSides.Text, "Số mặt metalize", out metalizeS, 1)) ok = false;
            if (!TryGet(txtUvPrice.Text, "Đơn giá UV mờ", out uvP, 0)) ok = false;
            if (!TryGet(txtUvSides.Text, "Số mặt UV", out uvS, 1)) ok = false;
            if (!TryGet(txtDieCost.Text, "Tiền khuôn bế", out die, 0)) ok = false;   // [FIX] defaultVal=0 thay vì fail
            if (!TryGet(txtRibbonPrice.Text, "Giá dây", out ribbon, 0)) ok = false;
            if (!TryGet(txtButtonPrice.Text, "Giá nút", out button, 0)) ok = false;
            if (!TryGet(txtBoxPrice.Text, "Giá thùng/cái", out boxP, 0)) ok = false;
            if (!TryGet(txtBoxCapacity.Text, "Sức chứa thùng", out boxCap, 500)) ok = false;
            if (!TryGet(txtDeliveryCost.Text, "Tiền xe giao", out delivery, 0)) ok = false;
            if (!TryGet(txtProofCost.Text, "Tiền proof", out proof, 0)) ok = false;
            if (!TryGet(txtProfitPercent.Text, "% Lợi nhuận", out profit, 20)) ok = false;

            if (!ok || errors.Length > 0)
            {
                MessageBox.Show(
                    "⚠️ Vui lòng kiểm tra lại các trường sau:\n" + errors.ToString(),
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ──────────────────────────────────────────────────────────────────
        // LƯU Ý khi áp dụng:
        //   1. Xóa hàm TryParseInputs() cũ trong frmPriceCalculation.cs
        //   2. Paste hàm trên vào thay thế (trong class frmPriceCalculation)
        //   3. Rebuild và test lại nút "Lưu báo giá" + "Tính nhanh"
        // ──────────────────────────────────────────────────────────────────

        // ─────────────────────────────────────────────────────
        // CÔNG THỨC TÍNH GIÁ
        // ─────────────────────────────────────────────────────

        private (double paper, double plate, double print,
                 double lam, double metal, double uv,
                 double dieC, double glue, double ribbon,
                 double button, double box, double delivery,
                 double proof, double total,
                 double perUnit, double price, double totalPrice)
        Calc(int qty, int layout, double wastage,
             double gsm, double priceTon, double pw, double ph,
             double laminateP, double laminateS,
             double metalizeP, double metalizeS,
             double uvP, double uvS, double die,
             double ribbon, double button,
             double boxP, double boxCap,
             double delivery, double proof, double profit)
        {
            double area = (pw / 100.0) * (ph / 100.0);
            double ramPrice = area * (gsm / 1000.0) * (priceTon / 1000.0) * 500;
            double sheets = Math.Ceiling((qty / (double)layout) + wastage);
            double rams = Math.Ceiling(sheets / 500.0);
            double paper = rams * ramPrice;

            int colorCount = int.TryParse(txtColorCount.Text, out int cc) ? cc : 0;
            string machine = cmbPrintMachine.SelectedItem?.ToString() ?? "";
            double platePx = machine.Contains("Máy lớn") ? 100000 : 60000;
            double plate = colorCount * platePx;
            double print = qty <= 5000 ? colorCount * 300000.0 : colorCount * 70.0 * qty;

            double lam = area * laminateP * laminateS * qty / layout;
            double metal = area * metalizeP * metalizeS * qty / layout;
            double uv = area * uvP * uvS * qty / layout;
            double dieC = layout * 150.0 * qty;

            double glue;
            if (qty <= 5000) glue = 300000;
            else { int extra = (int)Math.Ceiling((qty - 5000) / 1000.0); glue = 300000 + extra * 100000; }

            double rib = ribbon * qty;
            double btn = button * qty;
            double box = Math.Ceiling((double)qty / boxCap) * boxP;

            double total = paper + plate + print + lam + metal + uv + dieC + die + glue + rib + btn + box + delivery + proof;
            double perUnit = total / qty;
            double price = perUnit * (1 + profit / 100.0);
            double totalPr = price * qty;

            return (paper, plate, print, lam, metal, uv, dieC, glue, rib, btn, box, delivery, proof, total, perUnit, price, totalPr);
        }

        // ─────────────────────────────────────────────────────
        // [FIX] NÚT TÍNH GIÁ NHANH → lưu DB + mở frmQuoteManagement
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Đọc txtQuantities (nhiều SL cách nhau bởi dấu phẩy),
        /// lưu 1 BAO_GIA với nhiều dòng CHI_TIET_BAO_GIA (mỗi SL 1 dòng),
        /// rồi mở frmQuoteManagement để người dùng xem, tích và xuất Excel.
        /// </summary>
        private void btnQuickCalc_Click(object sender, EventArgs e)
        {
            if (!TryParseInputs(
                out int qty, out int layout, out double wastage,
                out double gsm, out double priceTon, out double pw, out double ph,
                out double laminateP, out double laminateS, out double metalizeP,
                out double metalizeS, out double uvP, out double uvS, out double die,
                out double ribbon, out double button, out double boxP, out double boxCap,
                out double delivery, out double proof, out double profit))
            {
                MessageBox.Show("⚠️ Kiểm tra lại dữ liệu nhập!", "Lỗi nhập liệu");
                return;
            }

            string rawQty = txtQuantities.Text.Trim();
            if (string.IsNullOrEmpty(rawQty)) rawQty = txtQuantity.Text.Trim();

            var quantities = new List<int>();
            foreach (var token in rawQty.Split(
                new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(token.Trim().Replace(".", ""), out int q) && q > 0)
                    quantities.Add(q);
            }

            if (quantities.Count == 0)
            {
                MessageBox.Show("⚠️ Nhập ít nhất 1 mức số lượng!");
                return;
            }

            string customerName = _selectedCustomerId > 0
                ? (cboCustomer.SelectedItem as CustomerItem)?.Name ?? txtCustomerName.Text.Trim()
                : txtCustomerName.Text.Trim();
            string customerAddress = txtAddress.Text.Trim();

            try
            {
                string quoteCode = _repo.GenerateQuoteCode();
                int newQuoteId = 0;

                var r0 = Calc(quantities[0], layout, wastage, gsm, priceTon, pw, ph,
                              laminateP, laminateS, metalizeP, metalizeS,
                              uvP, uvS, die, ribbon, button, boxP, boxCap, delivery, proof, profit);

                // Lưu BAO_GIA + CHI_TIET đầu tiên
                newQuoteId = _repo.SaveNewQuote(
                    _selectedCustomerId,
                    customerName, customerAddress, CurrentUser.HoTen,
                    quoteCode, txtProductName.Text.Trim(), dtpQuoteDate.Value.Date,
                    layout, (decimal)priceTon, (decimal)profit,
                    txtProductSize.Text.Trim(), txtPaperGsm.Text.Trim(),
                    $"{txtPrintSizeW.Text} x {txtPrintSizeH.Text}",
                    int.TryParse(txtColorCount.Text, out int colorCC) ? colorCC : 0,
                    txtDeliveryTime.Text.Trim(),
                    int.TryParse(txtValidityDays.Text, out int vd) ? vd : 30,
                    cmbPaperType.Text.Trim(), quantities[0],
                    (decimal)r0.paper, (decimal)r0.plate,
                    (decimal)r0.print, (decimal)r0.lam,
                    (decimal)r0.metal, (decimal)r0.uv,
                    (decimal)r0.dieC, (decimal)die,
                    (decimal)r0.glue, (decimal)r0.ribbon,
                    (decimal)r0.button, (decimal)r0.box,
                    (decimal)r0.delivery, (decimal)r0.proof,
                    (decimal)r0.total, (decimal)r0.perUnit,
                    (decimal)r0.price, (decimal)r0.totalPrice);

                // [FIX] INSERT thêm dòng CHI_TIET cho các mức SL còn lại (KHÔNG ghi đè)
                for (int i = 1; i < quantities.Count; i++)
                {
                    var ri = Calc(quantities[i], layout, wastage, gsm, priceTon, pw, ph,
                                  laminateP, laminateS, metalizeP, metalizeS,
                                  uvP, uvS, die, ribbon, button, boxP, boxCap, delivery, proof, profit);

                    _repo.InsertQuoteDetail(                      // ← InsertQuoteDetail, không phải UpdateQuoteDetail
                        newQuoteId,
                        int.TryParse(txtValidityDays.Text, out int vd2) ? vd2 : 30,
                        cmbPaperType.Text.Trim(), quantities[i],
                        (decimal)ri.paper, (decimal)ri.plate,
                        (decimal)ri.print, (decimal)ri.lam,
                        (decimal)ri.metal, (decimal)ri.uv,
                        (decimal)ri.dieC, (decimal)die,
                        (decimal)ri.glue, (decimal)ri.ribbon,
                        (decimal)ri.button, (decimal)ri.box,
                        (decimal)ri.delivery, (decimal)ri.proof,
                        (decimal)ri.total, (decimal)ri.perUnit,
                        (decimal)ri.price, (decimal)ri.totalPrice);
                }

                // Hỏi lưu loại giấy mới nếu cần
                string paperName = cmbPaperType.Text.Trim();
                bool paperIsNew = !string.IsNullOrEmpty(paperName)
                                    && paperName != CUSTOM_PAPER
                                    && !cmbPaperType.Items.Cast<string>().Any(
                                        i => string.Equals(i, paperName, StringComparison.OrdinalIgnoreCase)
                                             && i != CUSTOM_PAPER);
                if (paperIsNew)
                {
                    if (MessageBox.Show(
                        $"Loại giấy \"{paperName}\" chưa có trong danh sách.\nLưu để dùng lại lần sau?",
                        "Lưu loại giấy mới?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        double.TryParse(txtPaperGsm.Text, out double gsmV);
                        double.TryParse(txtPaperPricePerTon.Text, out double priceTonV);
                        int.TryParse(txtWastageSheets.Text, out int buHao);
                        _paperRepo.SavePaperType(paperName, (decimal)gsmV,
                            $"{txtPrintSizeW.Text} x {txtPrintSizeH.Text}",
                            (decimal)priceTonV, buHao);
                        LoadPaperTypeComboBox();
                    }
                }

                MessageBox.Show(
                    $"✅ Đã lưu báo giá {quoteCode} với {quantities.Count} mức số lượng!\n" +
                    "Chuyển sang quản lý báo giá để xem và xuất Excel.",
                    "Tính giá nhanh thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var frmQM = Application.OpenForms["frmQuoteManagement"] as frmQuoteManagement;
                if (frmQM != null) { frmQM.LoadQuoteGrid(); frmQM.BringToFront(); }
                else new frmQuoteManagement().Show();

                cboCustomer.SelectedIndex = 0;   // về "(-- Khách lẻ --)"
                txtCustomerName.Clear();
                txtAddress.Clear();
                _selectedCustomerId = 0;
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }
        // ─────────────────────────────────────────────────────
        // [FIX] NÚT LƯU BÁO GIÁ — không tạo KH trùng
        // ─────────────────────────────────────────────────────

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (!TryParseInputs(
                out int qty, out int layout, out double wastage,
                out double gsm, out double priceTon, out double pw, out double ph,
                out double laminateP, out double laminateS, out double metalizeP,
                out double metalizeS, out double uvP, out double uvS, out double die,
                out double ribbon, out double button, out double boxP, out double boxCap,
                out double delivery, out double proof, out double profit))
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ và đúng định dạng số!", "Lỗi nhập liệu");
                return;
            }

            var r = Calc(qty, layout, wastage, gsm, priceTon, pw, ph,
                         laminateP, laminateS, metalizeP, metalizeS,
                         uvP, uvS, die, ribbon, button, boxP, boxCap,
                         delivery, proof, profit);
            try
            {
                string customerName = _selectedCustomerId > 0
                    ? (cboCustomer.SelectedItem as CustomerItem)?.Name ?? txtCustomerName.Text.Trim()
                    : txtCustomerName.Text.Trim();
                string customerAddress = txtAddress.Text.Trim();

                string quoteCode = _repo.GenerateQuoteCode();
                int newQuoteId = _repo.SaveNewQuote(
                    _selectedCustomerId,                // [FIX] truyền ID — SP tự xử lý tạo KH mới hay không
                    customerName, customerAddress, CurrentUser.HoTen,
                    quoteCode, txtProductName.Text.Trim(), dtpQuoteDate.Value.Date,
                    layout, (decimal)priceTon, (decimal)profit,
                    txtProductSize.Text.Trim(), txtPaperGsm.Text.Trim(),
                    $"{txtPrintSizeW.Text} x {txtPrintSizeH.Text}",
                    int.TryParse(txtColorCount.Text, out int colorCC) ? colorCC : 0,
                    txtDeliveryTime.Text.Trim(),
                    int.TryParse(txtValidityDays.Text, out int vd) ? vd : 30,
                    cmbPaperType.Text.Trim(), qty,
                    (decimal)r.paper, (decimal)r.plate,
                    (decimal)r.print, (decimal)r.lam,
                    (decimal)r.metal, (decimal)r.uv,
                    (decimal)r.dieC, (decimal)die,
                    (decimal)r.glue, (decimal)r.ribbon,
                    (decimal)r.button, (decimal)r.box,
                    (decimal)r.delivery, (decimal)r.proof,
                    (decimal)r.total, (decimal)r.perUnit,
                    (decimal)r.price, (decimal)r.totalPrice);

                // [FIX] Bỏ hoàn toàn hộp thoại hỏi lưu KH mới —
                //       quản lý KH chỉ thực hiện qua frmCustomerManagement

                // Hỏi lưu loại giấy mới nếu người dùng gõ tay tên giấy chưa có trong danh sách
                string paperName = cmbPaperType.Text.Trim();
                bool paperIsNew = !string.IsNullOrEmpty(paperName)
                                    && paperName != CUSTOM_PAPER
                                    && !cmbPaperType.Items.Cast<string>().Any(
                                        i => string.Equals(i, paperName, StringComparison.OrdinalIgnoreCase)
                                             && i != CUSTOM_PAPER);

                if (paperIsNew)
                {
                    if (MessageBox.Show(
                        $"Loại giấy \"{paperName}\" chưa có trong danh sách.\nLưu để dùng lại lần sau?",
                        "Lưu loại giấy mới?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        double.TryParse(txtPaperGsm.Text, out double gsmV);
                        double.TryParse(txtPaperPricePerTon.Text, out double priceTonV);
                        int.TryParse(txtWastageSheets.Text, out int buHao);
                        _paperRepo.SavePaperType(paperName, (decimal)gsmV,
                            $"{txtPrintSizeW.Text} x {txtPrintSizeH.Text}",
                            (decimal)priceTonV, buHao);
                        LoadPaperTypeComboBox();
                    }
                }

                MessageBox.Show(
                    $"✅ Đã lưu báo giá!\n\nMã: {quoteCode}\n" +
                    $"Tổng giá thành: {r.total:N0} đ\n" +
                    $"Giá báo khách:  {r.price:N0} đ/cái",
                    "Lưu thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                cboCustomer.SelectedIndex = 0;   // về "(-- Khách lẻ --)"
                txtCustomerName.Clear();
                txtAddress.Clear();
                _selectedCustomerId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        // ─────────────────────────────────────────────────────
        // NÚT CẬP NHẬT BÁO GIÁ
        // ─────────────────────────────────────────────────────

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!TryParseInputs(
                out int qty, out int layout, out double wastage,
                out double gsm, out double priceTon, out double pw, out double ph,
                out double laminateP, out double laminateS, out double metalizeP,
                out double metalizeS, out double uvP, out double uvS, out double die,
                out double ribbon, out double button, out double boxP, out double boxCap,
                out double delivery, out double proof, out double profit))
            {
                MessageBox.Show("⚠️ Kiểm tra lại dữ liệu nhập!");
                return;
            }

            var r = Calc(qty, layout, wastage, gsm, priceTon, pw, ph,
                         laminateP, laminateS, metalizeP, metalizeS,
                         uvP, uvS, die, ribbon, button, boxP, boxCap,
                         delivery, proof, profit);
            try
            {
                _repo.UpdateQuoteDetail(
                    _quoteId,
                    int.TryParse(txtValidityDays.Text, out int vd) ? vd : 30,
                    cmbPaperType.Text.Trim(), qty,
                    (decimal)r.paper, (decimal)r.plate,
                    (decimal)r.print, (decimal)r.lam,
                    (decimal)r.metal, (decimal)r.uv,
                    (decimal)r.dieC, (decimal)die,
                    (decimal)r.glue, (decimal)r.ribbon,
                    (decimal)r.button, (decimal)r.box,
                    (decimal)r.delivery, (decimal)r.proof,
                    (decimal)r.total, (decimal)r.perUnit,
                    (decimal)r.price, (decimal)r.totalPrice);

                MessageBox.Show("✅ Cập nhật báo giá thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        // ─────────────────────────────────────────────────────
        // INNER CLASS
        // ─────────────────────────────────────────────────────

        private class CustomerItem
        {
            public int Id { get; set; } = 0;
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string TaxCode { get; set; } = "";

            public override string ToString()
                => Id == 0 ? "(-- Khách lẻ --)" : $"{Code} - {Name}";
        }

        private void frmPriceCalculation_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            SaveDraftForCurrentCustomer();
            CustomerListChanged -= OnCustomerListChanged;
        }
    }
}