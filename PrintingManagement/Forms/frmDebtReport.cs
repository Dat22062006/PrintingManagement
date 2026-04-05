// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmDebtReport.cs - BÁO CÁO CÔNG NỢ                                ║
// ║  - Công nợ phải thu (khách hàng) — quá hạn dựa vào DueDate         ║
// ║  - Công nợ phải trả (nhà cung cấp)                                  ║
// ╚══════════════════════════════════════════════════════════════════════╝

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmDebtReport : Form
    {
        // ─────────────────────────────────────────────────────
        // KHAI BÁO PRIVATE FIELD
        // ─────────────────────────────────────────────────────

        // Repository xử lý toàn bộ DB — form không viết SQL trực tiếp
        private readonly DebtReportRepository _repo = new();

        // Loại công nợ đang hiển thị
        private string _debtType = "Receivable";

        /// <summary>Vùng phải thu: một bảng chứng từ + tìm kiếm (không còn lưới tóm tắt phía trên).</summary>
        private Panel? _pnlReceivableMain;
        private Panel? _topReceivableCard;
        private DataGridView? _dgvReceivableLines;
        private TextBox? _txtSearchReceivable;
        private CheckBox? _chkReceivableMonth;
        private ComboBox? _cmbReceivableMonth;
        private NumericUpDown? _numReceivableYear;
        private Label? _lblReceivableHint;
        private bool _receivableSplitBuilt;
        private bool _loadingReceivables;


        // ─────────────────────────────────────────────────────
        // KHỞI TẠO FORM
        // ─────────────────────────────────────────────────────

        public frmDebtReport()
        {
            InitializeComponent();
            this.Load += frmDebtReport_Load;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmDebtReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            BuildReceivableSingleTableUi();
            LoadDebtSummary();
            LoadReceivables();

            // Trạng thái ban đầu — nút Phải thu được chọn
            HighlightButton(btnReceivable, Color.FromArgb(76, 175, 80));
            ResetButton(btnPayable);
            Text = "Báo cáo công nợ";
            UpdateReceivableDetailPanel();
            ScheduleReceivableDetailRefresh();
        }


        // ─────────────────────────────────────────────────────
        // CHI TIẾT NỢ + TÌM KIẾM (PHẢI THU)
        // ─────────────────────────────────────────────────────

        /// <summary>Nạp bảng chứng từ sau khi form đã layout.</summary>
        private void ScheduleReceivableDetailRefresh()
        {
            if (_debtType != "Receivable")
                return;

            void Work()
            {
                if (_debtType != "Receivable")
                    return;
                RefreshReceivableDetailGrid();
            }

            if (IsHandleCreated)
                BeginInvoke(new Action(Work));
            else
                Work();
        }

        private void BuildReceivableSingleTableUi()
        {
            if (_receivableSplitBuilt)
                return;
            _receivableSplitBuilt = true;

            var parent = panelallcongnophaithu;
            parent.SuspendLayout();
            parent.Controls.Remove(dgvDebt);

            _pnlReceivableMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(243, 246, 249),
                Padding = new Padding(12, 10, 12, 12)
            };

            _topReceivableCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 132,
                BackColor = Color.White,
                Padding = new Padding(16, 12, 16, 10)
            };
            _topReceivableCard.Paint += (_, pe) =>
            {
                var g = pe.Graphics;
                using var pen = new Pen(Color.FromArgb(228, 232, 238), 1);
                g.DrawLine(pen, 0, _topReceivableCard.Height - 1, _topReceivableCard.Width, _topReceivableCard.Height - 1);
            };

            var lblHead = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Chi tiết từng hóa đơn còn nợ — gõ tên khách / số HĐ vào ô bên dưới",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(48, 63, 159),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblReceivableHint = new Label
            {
                Dock = DockStyle.Top,
                Height = 0,
                Visible = false,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(198, 40, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0)
            };

            var pnlMonth = new Panel { Dock = DockStyle.Top, Height = 36 };
            var flowMonth = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(0, 2, 0, 0),
                Margin = new Padding(0),
                BackColor = Color.White
            };
            _chkReceivableMonth = new CheckBox
            {
                Text = "Lọc theo tháng bán hàng:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 4, 10, 0)
            };
            _cmbReceivableMonth = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 56,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 2, 6, 0)
            };
            for (int i = 1; i <= 12; i++)
                _cmbReceivableMonth.Items.Add(i.ToString("00"));
            _cmbReceivableMonth.SelectedIndex = Math.Clamp(DateTime.Today.Month - 1, 0, 11);

            var lblNam = new Label
            {
                Text = "Năm",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 6, 6, 0)
            };
            _numReceivableYear = new NumericUpDown
            {
                Minimum = 2000,
                Maximum = 2100,
                Value = DateTime.Today.Year,
                Width = 76,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 2, 0, 0)
            };
            var lblMonthHint = new Label
            {
                Text = "(theo ngày bán, không phải ngày đến hạn)",
                AutoSize = true,
                Font = new Font("Segoe UI", 8.25f),
                ForeColor = Color.FromArgb(120, 125, 135),
                Margin = new Padding(12, 8, 0, 0)
            };

            void OnMonthFilterUiChanged()
            {
                if (_debtType == "Receivable" && _chkReceivableMonth != null && _chkReceivableMonth.Checked)
                    LoadReceivables();
            }
            _cmbReceivableMonth.SelectedIndexChanged += (_, _) => OnMonthFilterUiChanged();
            _numReceivableYear.ValueChanged += (_, _) => OnMonthFilterUiChanged();
            _chkReceivableMonth.CheckedChanged += (_, _) =>
            {
                if (_debtType == "Receivable")
                    LoadReceivables();
            };

            flowMonth.Controls.Add(_chkReceivableMonth);
            flowMonth.Controls.Add(_cmbReceivableMonth);
            flowMonth.Controls.Add(lblNam);
            flowMonth.Controls.Add(_numReceivableYear);
            flowMonth.Controls.Add(lblMonthHint);
            pnlMonth.Controls.Add(flowMonth);

            // Ô tìm cố định chiều ngang + nút sát bên — tránh Fill kéo dài tận mép phải (bị lệch)
            var pnlFind = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 6) };
            var flowSearch = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 2, 0, 2)
            };

            _txtSearchReceivable = new TextBox
            {
                Name = "txtSearch",
                Width = 360,
                Height = 30,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(252, 252, 254),
                ForeColor = Color.FromArgb(33, 37, 41),
                Margin = new Padding(0, 2, 10, 2)
            };
            _txtSearchReceivable.PlaceholderText = "txtSearch — tên khách, số HĐ, mã đơn…";
            _txtSearchReceivable.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    RunReceivableInvoiceSearch();
                }
            };

            var btnFind = new Button
            {
                Width = 112,
                Height = 30,
                Text = "Tìm kiếm",
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += (_, _) => RunReceivableInvoiceSearch();

            flowSearch.Controls.Add(_txtSearchReceivable);
            flowSearch.Controls.Add(btnFind);
            pnlFind.Controls.Add(flowSearch);

            // Tiêu đề → tìm kiếm → lọc tháng → gợi ý lỗi
            _topReceivableCard.Controls.Add(lblHead);
            _topReceivableCard.Controls.Add(pnlFind);
            _topReceivableCard.Controls.Add(pnlMonth);
            _topReceivableCard.Controls.Add(_lblReceivableHint);

            var gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 12, 0, 0),
                BackColor = Color.FromArgb(243, 246, 249)
            };
            var gridCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(0)
            };

            _dgvReceivableLines = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 239, 244),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvReceivableLines.Dock = DockStyle.Fill;
            SetupReceivableLinesGrid();

            gridCard.Controls.Add(_dgvReceivableLines);
            gridHost.Controls.Add(gridCard);
            _pnlReceivableMain.Controls.Add(gridHost);
            _pnlReceivableMain.Controls.Add(_topReceivableCard);

            parent.Controls.Add(dgvDebt);
            parent.Controls.Add(_pnlReceivableMain);
            parent.ResumeLayout(false);
        }

        private void SetupReceivableLinesGrid()
        {
            if (_dgvReceivableLines == null) return;
            var dgv = _dgvReceivableLines;
            dgv.Columns.Clear();

            var right = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleRight };
            var center = new DataGridViewCellStyle
            { Alignment = DataGridViewContentAlignment.MiddleCenter };

            // Bố cục giống bản thiết kế: 1 cột chứng từ/HĐ, không tách số HĐ riêng
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRCustomer",
                HeaderText = "Khách hàng",
                FillWeight = 18,
                MinimumWidth = 100
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRVoucher",
                HeaderText = "Số chứng từ / HĐ",
                FillWeight = 14,
                MinimumWidth = 110,
                DefaultCellStyle = new DataGridViewCellStyle(center)
                {
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRDate",
                HeaderText = "Ngày bán (HĐ)",
                FillWeight = 9,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle(center)
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRDue",
                HeaderText = "Đến hạn TT",
                FillWeight = 9,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle(center)
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRProduct",
                HeaderText = "Tên hàng (dòng)",
                FillWeight = 16,
                MinimumWidth = 110
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRTotal",
                HeaderText = "Tổng tiền",
                FillWeight = 10,
                MinimumWidth = 90,
                DefaultCellStyle = right
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRPaid",
                HeaderText = "Đã thu",
                FillWeight = 10,
                MinimumWidth = 88,
                DefaultCellStyle = right
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRRem",
                HeaderText = "Còn nợ",
                FillWeight = 11,
                MinimumWidth = 88,
                DefaultCellStyle = new DataGridViewCellStyle(right)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(211, 47, 47)
                }
            });

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(57, 73, 171);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.75f);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.Padding = new Padding(10, 6, 10, 6);
            dgv.RowTemplate.Height = 36;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
        }

        private void UpdateReceivableDetailPanel()
        {
            ApplyDebtViewLayout();
        }

        /// <summary>Phải thu: một panel bảng chứng từ. Phải trả: chỉ dgvDebt (giữ như cũ).</summary>
        private void ApplyDebtViewLayout()
        {
            if (panelallcongnophaithu == null || _pnlReceivableMain == null)
                return;

            if (_debtType == "Payable")
            {
                Text = "Báo cáo công nợ — Phải trả";
                _pnlReceivableMain.Visible = false;
                dgvDebt.Visible = true;
                if (dgvDebt.Parent != panelallcongnophaithu)
                {
                    dgvDebt.Parent?.Controls.Remove(dgvDebt);
                    panelallcongnophaithu.Controls.Add(dgvDebt);
                }
                dgvDebt.Dock = DockStyle.Fill;
                if (panelallcongnophaithu.Controls.Contains(dgvDebt))
                    panelallcongnophaithu.Controls.SetChildIndex(dgvDebt, 0);
                if (panelallcongnophaithu.Controls.Contains(panelcongnphaithu))
                    panelallcongnophaithu.Controls.SetChildIndex(panelcongnphaithu, 1);
            }
            else
            {
                Text = "Báo cáo công nợ — Phải thu";
                dgvDebt.Visible = false;
                _pnlReceivableMain.Visible = true;
                if (_pnlReceivableMain.Parent != panelallcongnophaithu)
                {
                    panelallcongnophaithu.Controls.Add(_pnlReceivableMain);
                }
                _pnlReceivableMain.Dock = DockStyle.Fill;
                if (panelallcongnophaithu.Controls.Contains(_pnlReceivableMain))
                    panelallcongnophaithu.Controls.SetChildIndex(_pnlReceivableMain, 0);
                if (panelallcongnophaithu.Controls.Contains(panelcongnphaithu))
                    panelallcongnophaithu.Controls.SetChildIndex(panelcongnphaithu, 1);
            }
        }

        private (int? Year, int? Month) GetReceivableMonthFilter()
        {
            if (_chkReceivableMonth == null || !_chkReceivableMonth.Checked
                || _cmbReceivableMonth == null || _numReceivableYear == null)
                return (null, null);
            int y = (int)_numReceivableYear.Value;
            int m = _cmbReceivableMonth.SelectedIndex + 1;
            if (m < 1 || m > 12) m = DateTime.Today.Month;
            return (y, m);
        }

        /// <summary>Nạp toàn bộ dòng công nợ mở (theo tháng + ô tìm) — không phụ thuộc dòng được chọn trên lưới KH.</summary>
        private void RefreshReceivableDetailGrid()
        {
            if (_debtType != "Receivable" || _dgvReceivableLines == null) return;

            var (y, m) = GetReceivableMonthFilter();
            string kw = _txtSearchReceivable?.Text?.Trim() ?? "";

            // Một bảng duy nhất — không lọc theo dòng trên lưới tóm tắt (đã bỏ lưới đó)
            int? custId = null;

            try
            {
                var dt = _repo.GetOpenReceivableInvoiceLines(kw, y, m, custId);
                if (dt.Rows.Count == 0 && !string.IsNullOrEmpty(kw))
                {
                    var all = _repo.GetOpenReceivableInvoiceLines("", y, m, null);
                    dt = DebtReportRepository.FilterReceivableDetailTable(all, kw, null);
                }
                // Vẫn trống: quét từng KH có nợ (sp_GetReceivables) + chi tiết đơn (sp_GetOrderDebtByCustomer)
                if (dt.Rows.Count == 0 && !string.IsNullOrEmpty(kw))
                {
                    DataTable? acc = null;
                    var sum = _repo.GetReceivables();
                    foreach (DataRow r in sum.Rows)
                    {
                        if (!ReceivableDebtColumns.TryGetInt32(r, out int cid, "id", "ID", "KhachHangId"))
                            continue;
                        string ten = ReceivableDebtColumns.GetString(r, "Ten_Khach_Hang", "TenKhachHang", "CustomerName", "HoTen", "TenKH", "Ten_KH");
                        if (string.IsNullOrWhiteSpace(ten)) continue;
                        var od = _repo.GetOrderDebtByCustomer(cid);
                        var wrapped = WrapOrderDebtRowsForDetailGrid(od, ten.Trim(), cid);
                        var hit = DebtReportRepository.FilterReceivableDetailTable(wrapped, kw, null);
                        if (hit.Rows.Count == 0) continue;
                        if (acc == null)
                            acc = hit.Clone();
                        foreach (DataRow row in hit.Rows)
                            acc!.ImportRow(row);
                    }
                    if (acc != null && acc.Rows.Count > 0)
                        dt = acc;
                }
                FillReceivableLinesFromSearch(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        private static DataTable WrapOrderDebtRowsForDetailGrid(DataTable od, string tenKhachHang, int customerId)
        {
            var t = new DataTable();
            t.Columns.Add("CustomerId", typeof(int));
            t.Columns.Add("Ten_Khach_Hang", typeof(string));
            t.Columns.Add("OrderCode", typeof(string));
            t.Columns.Add("SoHoaDon", typeof(string));
            t.Columns.Add("OrderDate", typeof(DateTime));
            t.Columns.Add("DueDate", typeof(object));
            t.Columns.Add("ProductName", typeof(string));
            t.Columns.Add("TotalAmount", typeof(decimal));
            t.Columns.Add("Collected", typeof(decimal));
            t.Columns.Add("Remaining", typeof(decimal));

            foreach (DataRow r in od.Rows)
            {
                string oc = od.Columns.Contains("OrderCode") && r["OrderCode"] != DBNull.Value
                    ? r["OrderCode"].ToString() ?? ""
                    : "";
                object? due = od.Columns.Contains("DueDate") ? r["DueDate"] : DBNull.Value;
                t.Rows.Add(
                    customerId,
                    tenKhachHang,
                    oc,
                    oc,
                    od.Columns.Contains("OrderDate") && r["OrderDate"] != DBNull.Value
                        ? Convert.ToDateTime(r["OrderDate"])
                        : DateTime.MinValue,
                    due,
                    od.Columns.Contains("ProductName") ? r["ProductName"] : "",
                    od.Columns.Contains("TotalAmount") && r["TotalAmount"] != DBNull.Value
                        ? Convert.ToDecimal(r["TotalAmount"])
                        : 0m,
                    od.Columns.Contains("Collected") && r["Collected"] != DBNull.Value
                        ? Convert.ToDecimal(r["Collected"])
                        : 0m,
                    od.Columns.Contains("Remaining") && r["Remaining"] != DBNull.Value
                        ? Convert.ToDecimal(r["Remaining"])
                        : 0m);
            }
            return t;
        }

        private void FillReceivableLinesFromSearch(DataTable dt)
        {
            if (_dgvReceivableLines == null) return;
            _dgvReceivableLines.Rows.Clear();

            if (_lblReceivableHint != null && _topReceivableCard != null)
            {
                if (dt.Rows.Count == 0)
                {
                    _lblReceivableHint.Visible = true;
                    _lblReceivableHint.Height = 44;
                    _lblReceivableHint.ForeColor = Color.FromArgb(183, 28, 28);
                    _lblReceivableHint.Text =
                        "Không có dòng nào. Gõ tên khách / số HĐ trong txtSearch, hoặc bỏ tick lọc tháng (lọc theo ngày bán).";
                    _topReceivableCard.Height = 176;
                }
                else
                {
                    _lblReceivableHint.Visible = false;
                    _lblReceivableHint.Height = 0;
                    _lblReceivableHint.Text = "";
                    _topReceivableCard.Height = 132;
                }
            }

            foreach (DataRow r in dt.Rows)
            {
                string orderCode = r.Table.Columns.Contains("OrderCode") ? r["OrderCode"]?.ToString() ?? "" : "";
                string soHD = "";
                if (r.Table.Columns.Contains("SoHoaDon") && r["SoHoaDon"] != DBNull.Value)
                    soHD = r["SoHoaDon"]?.ToString()?.Trim() ?? "";

                string voucherTxt;
                if (!string.IsNullOrWhiteSpace(soHD))
                    voucherTxt = !string.IsNullOrEmpty(orderCode) && !soHD.Equals(orderCode, StringComparison.OrdinalIgnoreCase)
                        ? $"{soHD} / {orderCode}"
                        : soHD;
                else
                    voucherTxt = orderCode;

                string orderDateTxt = "—";
                if (r.Table.Columns.Contains("OrderDate") && r["OrderDate"] != DBNull.Value)
                {
                    var od = Convert.ToDateTime(r["OrderDate"]);
                    if (od.Year > 1900)
                        orderDateTxt = od.ToString("dd/MM/yyyy");
                }

                string dueTxt = r.Table.Columns.Contains("DueDate") && r["DueDate"] != DBNull.Value
                    ? Convert.ToDateTime(r["DueDate"]).ToString("dd/MM/yyyy")
                    : "—";

                int rowIdx = _dgvReceivableLines.Rows.Add(
                    ReceivableDebtColumns.GetString(r, "Ten_Khach_Hang", "TenKhachHang", "CustomerName", "HoTen", "TenKH"),
                    voucherTxt,
                    orderDateTxt,
                    dueTxt,
                    r.Table.Columns.Contains("ProductName") ? r["ProductName"]?.ToString() ?? "" : "",
                    Convert.ToDecimal(r["TotalAmount"]).ToString("N0") + " đ",
                    Convert.ToDecimal(r["Collected"]).ToString("N0") + " đ",
                    Convert.ToDecimal(r["Remaining"]).ToString("N0") + " đ");

                if (r.Table.Columns.Contains("DueDate") && r["DueDate"] != DBNull.Value)
                {
                    int days = (Convert.ToDateTime(r["DueDate"]).Date - DateTime.Today).Days;
                    var remCell = _dgvReceivableLines.Rows[rowIdx].Cells["colRRem"];
                    remCell.Style.ForeColor = days < 0
                        ? Color.FromArgb(211, 47, 47)
                        : days <= 3
                            ? Color.FromArgb(230, 81, 0)
                            : Color.FromArgb(211, 47, 47);
                }
            }
        }

        private void RunReceivableInvoiceSearch()
        {
            if (_debtType != "Receivable" || _txtSearchReceivable == null) return;

            // [FIX] Gọi thẳng RefreshReceivableDetailGrid — tìm kiếm server-side qua SP
            RefreshReceivableDetailGrid();
        }


        // ─────────────────────────────────────────────────────
        // THIẾT KẾ DATAGRIDVIEW
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvDebt;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(63, 81, 181);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            if (dgv is Guna.UI2.WinForms.Guna2DataGridView g2)
            {
                g2.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(40, 40, 40);
                g2.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.FromArgb(40, 40, 40);
            }

            var styleRight = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f)
            };
            var styleCenter = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colParty", HeaderText = "Khách hàng", FillWeight = 26 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colTotalDebt", HeaderText = "Tổng nợ", Width = 130, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPaid", HeaderText = "Đã thu", Width = 130, DefaultCellStyle = styleRight });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRemaining",
                HeaderText = "Còn nợ",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle(styleRight)
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(244, 67, 54)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colDueStatus", HeaderText = "Quá hạn", Width = 120, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colDueDate", HeaderText = "Ngày đến hạn", Width = 110, DefaultCellStyle = styleCenter });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colId", Visible = false });

            dgv.SelectionChanged += dgvDebt_SelectionChanged;
        }

        private void dgvDebt_SelectionChanged(object? sender, EventArgs e)
        {
            if (_debtType != "Receivable" || _loadingReceivables) return;
            // Phải thu: không còn lưới tóm tắt trên dgvDebt — không làm gì
        }


        // ─────────────────────────────────────────────────────
        // KPI TỔNG CÔNG NỢ
        // ─────────────────────────────────────────────────────

        private void LoadDebtSummary()
        {
            try
            {
                // Toàn bộ SQL nằm trong DebtReportRepository
                var ds = _repo.GetDebtSummary();

                // Bảng 0: Công nợ phải thu
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    double totalReceivable = Convert.ToDouble(row["TotalReceivable"]);
                    int customerCount = Convert.ToInt32(row["CustomerCount"]);

                    lblReceivableAmount.Text = (totalReceivable / 1_000_000).ToString("N0") + " triệu";
                    lblReceivableAmount.ForeColor = Color.FromArgb(76, 175, 80);
                    lblCustomerCount.Text = $"Từ {customerCount} khách hàng";
                }

                // Bảng 1: Công nợ phải trả
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var row = ds.Tables[1].Rows[0];
                    double totalPayable = Convert.ToDouble(row["TotalPayable"]);
                    double totalPaid = Convert.ToDouble(row["TotalPaid"]);
                    double remainingPayable = Math.Max(totalPayable - totalPaid, 0);
                    int supplierCount = Convert.ToInt32(row["SupplierCount"]);

                    lblPayableAmount.Text = (remainingPayable / 1_000_000).ToString("N0") + " triệu";
                    lblPayableAmount.ForeColor = Color.FromArgb(244, 67, 54);
                    lblSupplierCount.Text = $"Cho {supplierCount} nhà cung cấp";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // CÔNG NỢ PHẢI THU
        // ─────────────────────────────────────────────────────

        private void LoadReceivables()
        {
            _loadingReceivables = true;
            try
            {
                ScheduleReceivableDetailRefresh();
            }
            finally
            {
                _loadingReceivables = false;
            }
        }


        // ─────────────────────────────────────────────────────
        // CÔNG NỢ PHẢI TRẢ
        // ─────────────────────────────────────────────────────

        private void LoadPayables()
        {
            dgvDebt.Rows.Clear();
            dgvDebt.Columns["colParty"].HeaderText = "Nhà cung cấp";
            dgvDebt.Columns["colPaid"].HeaderText = "Đã trả";
            dgvDebt.Columns["colDueDate"].HeaderText = "Ngày đến hạn (ước)";

            try
            {
                // Toàn bộ SQL nằm trong DebtReportRepository
                var dt = _repo.GetPayables();

                foreach (System.Data.DataRow dataRow in dt.Rows)
                {
                    int id = Convert.ToInt32(dataRow["id"]);
                    string supplierName = dataRow["Ten_NCC"].ToString();
                    double totalDebt = Convert.ToDouble(dataRow["TotalDebt"]);
                    double paid = Convert.ToDouble(dataRow["TotalPaid"]);
                    double remaining = Math.Max(totalDebt - paid, 0);

                    string dueStatusText;
                    Color statusBg, statusFg;
                    string dueDateText = "—";

                    DateTime? dueNcc = null;
                    if (dataRow.Table.Columns.Contains("DueDate") && dataRow["DueDate"] != DBNull.Value)
                        dueNcc = Convert.ToDateTime(dataRow["DueDate"]).Date;

                    if (remaining <= 0)
                    {
                        dueStatusText = "Đã trả đủ";
                        statusBg = Color.FromArgb(232, 245, 233);
                        statusFg = Color.FromArgb(56, 142, 60);
                    }
                    else
                    {
                        dueStatusText = "Còn nợ";
                        statusBg = Color.FromArgb(245, 245, 245);
                        statusFg = Color.FromArgb(100, 100, 100);
                        if (dueNcc.HasValue)
                        {
                            dueDateText = dueNcc.Value.ToString("dd/MM/yyyy");
                            int days = (dueNcc.Value - DateTime.Today).Days;
                            if (days < 0)
                            {
                                dueStatusText = $"Quá hạn {Math.Abs(days)} ngày";
                                statusBg = Color.FromArgb(255, 235, 238);
                                statusFg = Color.FromArgb(211, 47, 47);
                            }
                            else if (days <= 7)
                            {
                                dueStatusText = days == 0 ? "Đến hạn hôm nay" : $"Còn {days} ngày";
                                statusBg = Color.FromArgb(255, 249, 196);
                                statusFg = Color.FromArgb(245, 124, 0);
                            }
                        }
                    }

                    int rowIdx = dgvDebt.Rows.Add(
                        supplierName,
                        totalDebt.ToString("N0") + "đ",
                        paid.ToString("N0") + "đ",
                        remaining.ToString("N0") + "đ",
                        dueStatusText,
                        dueDateText,
                        id);

                    dgvDebt.Rows[rowIdx].Cells["colDueStatus"].Style.BackColor = statusBg;
                    dgvDebt.Rows[rowIdx].Cells["colDueStatus"].Style.ForeColor = statusFg;
                    dgvDebt.Rows[rowIdx].Cells["colRemaining"].Style.ForeColor =
                        remaining <= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }


        // ─────────────────────────────────────────────────────
        // HELPER — TÔ MÀU NÚT
        // ─────────────────────────────────────────────────────

        private void HighlightButton(Guna.UI2.WinForms.Guna2Button btn, Color fillColor)
        {
            btn.FillColor = fillColor;
            btn.ForeColor = Color.White;
        }

        private void ResetButton(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = Color.FromArgb(240, 240, 240);
            btn.ForeColor = Color.Black;
        }


        // ─────────────────────────────────────────────────────
        // SỰ KIỆN NÚT BẤM
        // ─────────────────────────────────────────────────────

        private void btnReceivable_Click(object sender, EventArgs e)
        {
            _debtType = "Receivable";
            LoadReceivables();
            HighlightButton(btnReceivable, Color.FromArgb(76, 175, 80));
            ResetButton(btnPayable);
            UpdateReceivableDetailPanel();
        }

        private void btnPayable_Click(object sender, EventArgs e)
        {
            _debtType = "Payable";
            LoadPayables();
            HighlightButton(btnPayable, Color.FromArgb(244, 67, 54));
            ResetButton(btnReceivable);
            UpdateReceivableDetailPanel();
            _dgvReceivableLines?.Rows.Clear();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDebtSummary();

            if (_debtType == "Receivable")
            {
                LoadReceivables();
                UpdateReceivableDetailPanel();
            }
            else
            {
                LoadPayables();
                UpdateReceivableDetailPanel();
            }

            MessageBox.Show("✅ Đã làm mới dữ liệu!", "Thông báo");
        }
    }
}