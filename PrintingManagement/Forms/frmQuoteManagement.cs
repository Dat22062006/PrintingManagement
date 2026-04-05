// ═══════════════════════════════════════════════════════════════════
// ║  frmQuoteManagement.cs                                          ║
// ═══════════════════════════════════════════════════════════════════
// [FIX] Panel mức SL luôn trống:
//   NGUYÊN NHÂN: SelectionChanged chạy trước DataBindingComplete
//                nên IDBaoGia chưa có giá trị → LoadQuoteLevels
//                gọi với quoteId = 0 → không load được.
//   SỬA: Dùng CellClick (click vào ô/dòng bất kỳ) để load mức SL.
//        SelectionChanged chỉ giữ lại để sync _currentQuoteId,
//        KHÔNG gọi LoadQuoteLevels nữa.
// ═══════════════════════════════════════════════════════════════════

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmQuoteManagement : Form
    {
        private readonly QuoteManagementRepository _repo = new();

        private int _currentPage = 1;
        private int _pageSize = 6;
        private int _totalRecord = 0;
        private int _totalPage = 0;
        private int _currentQuoteId = 0;

        /// <summary>
        /// True khi đang nạp dgvLevels — tránh CellValueChanged gọi UpdateGridTotalFromLevel(null)
        /// (gán chkSel từng dòng lúc ReadOnly vẫn false làm tổng dòng báo giá nhảy về 0).
        /// </summary>
        private bool _loadingQuoteLevels;

        private DataGridView dgvLevels;
        private Label lblLevels;

        /// <summary>
        /// Các trạng thái đã chốt mức giá — không cho đổi ô tích (tránh lệch lệnh SX / phiếu giao).
        /// </summary>
        private static bool IsQuoteTierSelectionLocked(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            return status.Trim() switch
            {
                "Đã duyệt" => true,
                "Khách duyệt" => true,
                "Đã ký" => true,
                "Đang sản xuất" => true,
                "Hoàn thành" => true,
                _ => false
            };
        }

        public frmQuoteManagement() { InitializeComponent(); }

        // ─────────────────────────────────────────────────────
        // LOAD FORM
        // ─────────────────────────────────────────────────────

        private void frmQuoteManagement_Load(object sender, EventArgs e)
        {
            SetupGridColumns();
            SetupLevelsPanel();
            LoadQuoteGrid();
        }

        // ─────────────────────────────────────────────────────
        // PANEL MỨC SỐ LƯỢNG
        // ─────────────────────────────────────────────────────

        private void SetupLevelsPanel()
        {
            lblLevels = new Label
            {
                Text = "📋  Mức số lượng — tích một mức khi duyệt; có thể tích nhiều khi xuất Excel:",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 0, 0)
            };

            dgvLevels = new DataGridView
            {
                Dock = DockStyle.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(226, 232, 240),
                RowTemplate = { Height = 30 },
                DefaultCellStyle = { Font = new Font("Segoe UI", 10) },
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(30, 64, 175),
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 34,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };

            dgvLevels.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "chkSel",
                HeaderText = "✓",
                Width = 44,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLvlId", Visible = false });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số lượng",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGiaThanh",
                HeaderText = "Giá thành / cái (đ)",
                Width = 180,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDonGia",
                HeaderText = "Đơn giá báo KH (đ/cái)",
                Width = 200,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền báo KH (đ)",
                Width = 210,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Raw values (ẩn, dùng khi xuất Excel)
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "rawQty", Visible = false });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "rawDonGia", Visible = false });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "rawGiaThanh", Visible = false });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "rawTongTien", Visible = false });

            var pnl = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 190,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(8, 4, 8, 6)
            };
            pnl.Controls.Add(dgvLevels);
            pnl.Controls.Add(lblLevels);
            this.Controls.Add(pnl);
            pnl.BringToFront();

            // Checkbox sync
            dgvLevels.CurrentCellDirtyStateChanged += dgvLevels_CurrentCellDirtyStateChanged;
            dgvLevels.CellValueChanged += dgvLevels_CellValueChanged;
            dgvLevels.CellBeginEdit += dgvLevels_CellBeginEdit;
        }

        private void dgvLevels_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            if (dgvLevels.Columns[e.ColumnIndex].Name != "chkSel") return;
            if (dgvLevels.Columns["chkSel"].ReadOnly)
                e.Cancel = true;
        }

        // ─────────────────────────────────────────────────────
        // CHECKBOX SYNC — commit ngay + cập nhật grid
        // ─────────────────────────────────────────────────────

        private void dgvLevels_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvLevels.IsCurrentCellDirty
                && dgvLevels.CurrentCell?.OwningColumn.Name == "chkSel")
                dgvLevels.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvLevels_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loadingQuoteLevels) return;
            if (e.RowIndex < 0 || dgvLevels.Columns[e.ColumnIndex].Name != "chkSel") return;
            if (dgvLevels.Columns["chkSel"].ReadOnly) return;

            var changedRow = dgvLevels.Rows[e.RowIndex];
            bool isChecked = Convert.ToBoolean(changedRow.Cells["chkSel"].Value ?? false);

            DataGridViewRow selectedRow = null;
            if (isChecked)
            {
                selectedRow = changedRow;
            }
            else
            {
                foreach (DataGridViewRow row in dgvLevels.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (Convert.ToBoolean(row.Cells["chkSel"].Value ?? false))
                    { selectedRow = row; break; }
                }
            }
            UpdateGridTotalFromLevel(selectedRow);
        }

        private void UpdateGridTotalFromLevel(DataGridViewRow levelRow)
        {
            DataGridViewRow quoteRow = null;
            foreach (DataGridViewRow row in dgvQuotes.Rows)
            {
                if (row.IsNewRow) continue;
                if (Convert.ToInt32(row.Cells["IDBaoGia"].Value ?? 0) == _currentQuoteId)
                { quoteRow = row; break; }
            }
            if (quoteRow == null) return;

            if (levelRow == null)
            {
                quoteRow.Cells["colSoLuong"].Value = 0;
                quoteRow.Cells["colTongBaoKhach"].Value = 0;
                return;
            }

            int.TryParse(levelRow.Cells["rawQty"].Value?.ToString(), out int qty);
            double.TryParse(levelRow.Cells["rawTongTien"].Value?.ToString(), out double total);

            quoteRow.Cells["colSoLuong"].Value = qty;
            quoteRow.Cells["colTongBaoKhach"].Value = total;

            // Highlight vàng nhạt 1.5 giây
            quoteRow.DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195);
            quoteRow.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            var timer = new System.Windows.Forms.Timer { Interval = 1500 };
            timer.Tick += (s, ev) =>
            {
                timer.Stop(); timer.Dispose();
                try
                {
                    if (quoteRow != null && !quoteRow.IsNewRow)
                    {
                        quoteRow.DefaultCellStyle.BackColor = Color.Empty;
                        quoteRow.DefaultCellStyle.ForeColor = Color.Empty;
                        dgvQuotes.InvalidateRow(quoteRow.Index);
                    }
                }
                catch { }
            };
            timer.Start();
        }

        // ─────────────────────────────────────────────────────
        // LẤY DETAIL ID CỦA MỨC ĐANG ĐƯỢC TÍCH
        // ─────────────────────────────────────────────────────

        private int GetSelectedDetailId()
        {
            if (dgvLevels == null) return 0;
            foreach (DataGridViewRow row in dgvLevels.Rows)
            {
                if (row.IsNewRow) continue;
                if (Convert.ToBoolean(row.Cells["chkSel"].Value ?? false))
                    if (int.TryParse(row.Cells["colLvlId"].Value?.ToString(), out int id))
                        return id;
            }
            // Fallback: dòng đầu tiên
            if (dgvLevels.Rows.Count > 0 && !dgvLevels.Rows[0].IsNewRow)
                if (int.TryParse(dgvLevels.Rows[0].Cells["colLvlId"].Value?.ToString(), out int firstId))
                    return firstId;
            return 0;
        }

        // ─────────────────────────────────────────────────────
        // NẠP MỨC SL VÀO PANEL
        // ─────────────────────────────────────────────────────

        private void LoadQuoteLevels(int quoteId)
        {
            if (dgvLevels == null || quoteId <= 0) return;

            // Gỡ handler để mọi gán chkSel lúc nạp dòng không kích hoạt UpdateGridTotalFromLevel(null) → 0đ
            dgvLevels.CellValueChanged -= dgvLevels_CellValueChanged;
            _loadingQuoteLevels = true;
            try
            {
                dgvLevels.Rows.Clear();
                if (dgvLevels.Columns.Contains("chkSel"))
                    dgvLevels.Columns["chkSel"].ReadOnly = false;

                string quoteStatus = "";
                foreach (DataGridViewRow gr in dgvQuotes.Rows)
                {
                    if (gr.IsNewRow) continue;
                    if (Convert.ToInt32(gr.Cells["IDBaoGia"].Value ?? 0) != quoteId) continue;
                    quoteStatus = gr.Cells["colStatus"].Value?.ToString() ?? "";
                    break;
                }
                bool tierLocked = IsQuoteTierSelectionLocked(quoteStatus);

                try
                {
                    var dt = _repo.GetQuoteDetailFull(quoteId);

                    int? primaryId = null;
                    try { primaryId = _repo.GetQuotePrimaryTierId(quoteId); }
                    catch
                    {
                        // DB chưa chạy script thêm BAO_GIA.id_Muc_Chinh → không chặn toàn bộ lưới mức
                    }
                    if ((!primaryId.HasValue || primaryId.Value <= 0) && dt.Rows.Count > 0)
                    {
                        int maxSl = int.MinValue, maxDetailId = 0;
                        foreach (DataRow r in dt.Rows)
                        {
                            int sl = Convert.ToInt32(r["So_Luong"]);
                            int did = Convert.ToInt32(r["id"]);
                            if (sl > maxSl) { maxSl = sl; maxDetailId = did; }
                        }
                        primaryId = maxDetailId;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        int sl = Convert.ToInt32(dr["So_Luong"]);
                        double donGia = dr["Gia_Bao_Khach"] != DBNull.Value ? Convert.ToDouble(dr["Gia_Bao_Khach"]) : 0;
                        double giaThanh = dr["Gia_Moi_Cai"] != DBNull.Value ? Convert.ToDouble(dr["Gia_Moi_Cai"]) : 0;
                        double tongTien = dr["Tong_Gia_Bao_Khach"] != DBNull.Value ? Convert.ToDouble(dr["Tong_Gia_Bao_Khach"]) : 0;
                        int detailId = Convert.ToInt32(dr["id"]);

                        int i = dgvLevels.Rows.Add();
                        var row = dgvLevels.Rows[i];
                        row.Cells["chkSel"].Value = primaryId.HasValue && detailId == primaryId.Value;
                        row.Cells["colLvlId"].Value = dr["id"];
                        row.Cells["colSoLuong"].Value = sl.ToString("N0");
                        row.Cells["colGiaThanh"].Value = giaThanh.ToString("N0");
                        row.Cells["colDonGia"].Value = donGia.ToString("N0");
                        row.Cells["colTongTien"].Value = tongTien.ToString("N0");
                        row.Cells["rawQty"].Value = sl;
                        row.Cells["rawDonGia"].Value = donGia;
                        row.Cells["rawGiaThanh"].Value = giaThanh;
                        row.Cells["rawTongTien"].Value = tongTien;
                    }

                    if (dgvLevels.Columns.Contains("chkSel"))
                        dgvLevels.Columns["chkSel"].ReadOnly = tierLocked;

                    if (lblLevels != null)
                    {
                        if (tierLocked)
                            lblLevels.Text = dt.Rows.Count > 1
                                ? $"📋  {dt.Rows.Count} mức — báo giá đã chốt ({quoteStatus}): không đổi ô tích."
                                : "📋  Mức số lượng — báo giá đã chốt: không đổi ô tích.";
                        else if (dt.Rows.Count > 1)
                            lblLevels.Text =
                                $"📋  {dt.Rows.Count} mức — tích một mức trước khi duyệt ✔; xuất Excel có thể tích nhiều:";
                        else
                            lblLevels.Text = "📋  Mức số lượng — tích chọn rồi bấm Xuất Excel:";
                    }
                }
                catch (Exception ex)
                {
                    _currentQuoteId = 0;
                    if (dgvLevels.Columns.Contains("chkSel"))
                        dgvLevels.Columns["chkSel"].ReadOnly = false;
                    MessageBox.Show(
                        "Không tải được mức số lượng cho báo giá này.\n\n" + ex.Message
                        + "\n\nKiểm tra: đã chạy script SQL (sp_GetQuoteDetailFull, CHI_TIET_BAO_GIA) và kết nối CSDL.",
                        "Lỗi tải mức báo giá",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _loadingQuoteLevels = false;
                dgvLevels.CellValueChanged += dgvLevels_CellValueChanged;
            }

            // Đồng bộ hiển thị dòng báo giá với mức đã tick (không đổi logic duyệt / DB)
            SyncMainGridTotalsFromCheckedTier();
        }

        /// <summary>
        /// Cập nhật cột số lượng / tổng báo khách trên lưới chính theo mức đang tick ở panel dưới.
        /// </summary>
        private void SyncMainGridTotalsFromCheckedTier()
        {
            if (dgvLevels == null) return;
            DataGridViewRow tierRow = null;
            foreach (DataGridViewRow row in dgvLevels.Rows)
            {
                if (row.IsNewRow) continue;
                if (Convert.ToBoolean(row.Cells["chkSel"].Value ?? false))
                { tierRow = row; break; }
            }
            if (tierRow == null) return;
            UpdateGridTotalFromLevel(tierRow);
        }

        // ─────────────────────────────────────────────────────
        // THIẾT KẾ CỘT DATAGRIDVIEW CHÍNH
        // ─────────────────────────────────────────────────────

        public void SetupGridColumns()
        {
            dgvQuotes.AutoGenerateColumns = false;
            dgvQuotes.Columns.Clear();
            dgvQuotes.RowHeadersVisible = false;
            dgvQuotes.AllowUserToAddRows = false;
            dgvQuotes.AllowUserToResizeRows = false;
            dgvQuotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQuotes.MultiSelect = false;
            dgvQuotes.RowTemplate.Height = 32;
            dgvQuotes.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvQuotes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSTT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 40,
                MinimumWidth = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Số báo giá", DataPropertyName = "Số báo giá", Width = 130 });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày BG",
                DataPropertyName = "Ngày BG",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Khách hàng", DataPropertyName = "Khách hàng", Width = 200 });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Sản phẩm", DataPropertyName = "Sản phẩm", Width = 160 });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "Số lượng",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongBaoKhach",
                HeaderText = "Tổng báo khách",
                DataPropertyName = "Tổng báo khách",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "#,##0 'VNĐ'",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Trạng thái", DataPropertyName = "Trạng thái", Name = "colStatus", Width = 120 });

            var btnStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 8),
                Padding = new Padding(0),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvQuotes.Columns.Add(new DataGridViewButtonColumn
            { Name = "btnView", HeaderText = "Thao tác", Width = 32, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FlatStyle = FlatStyle.Flat, DefaultCellStyle = btnStyle });
            dgvQuotes.Columns.Add(new DataGridViewButtonColumn
            { Name = "btnEdit", HeaderText = "", Width = 32, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FlatStyle = FlatStyle.Flat, DefaultCellStyle = btnStyle });
            dgvQuotes.Columns.Add(new DataGridViewButtonColumn
            { Name = "btnApprove", HeaderText = "", Width = 32, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FlatStyle = FlatStyle.Flat, DefaultCellStyle = btnStyle });
            dgvQuotes.Columns.Add(new DataGridViewButtonColumn
            { Name = "btnCancel", HeaderText = "", Width = 32, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FlatStyle = FlatStyle.Flat, DefaultCellStyle = btnStyle });
            dgvQuotes.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnComplete",
                HeaderText = "",
                Width = 32,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(0),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(22, 163, 74)
                }
            });
            dgvQuotes.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "IDBaoGia", DataPropertyName = "IDBaoGia", Visible = false });

            dgvQuotes.CellFormatting -= dgvQuotes_CellFormatting;
            dgvQuotes.CellFormatting += dgvQuotes_CellFormatting;
            dgvQuotes.CellClick -= dgvQuotes_CellClick;
            dgvQuotes.CellClick += dgvQuotes_CellClick;
            dgvQuotes.DataBindingComplete -= dgvQuotes_DataBindingComplete;
            dgvQuotes.DataBindingComplete += dgvQuotes_DataBindingComplete;
            dgvQuotes.ShowCellToolTips = true;
            dgvQuotes.CellToolTipTextNeeded -= dgvQuotes_CellToolTipTextNeeded;
            dgvQuotes.CellToolTipTextNeeded += dgvQuotes_CellToolTipTextNeeded;

            // [FIX] BỎ SelectionChanged — dùng CellClick để load mức SL
            // SelectionChanged bị trigger trước DataBindingComplete nên
            // IDBaoGia chưa có giá trị → quoteId = 0 → không load được.

            btnExportExcel.Click -= btnExportExcel_Click;
            btnExportExcel.Click += btnExportExcel_Click;
        }

        // ─────────────────────────────────────────────────────
        // NẠP DANH SÁCH BÁO GIÁ
        // ─────────────────────────────────────────────────────

        public void LoadQuoteGrid(string search = "")
        {
            try
            {
                int offset = (_currentPage - 1) * _pageSize;
                var ds = _repo.GetQuoteList(search, dtpFrom.Value.Date, dtpTo.Value.Date, offset, _pageSize);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _totalRecord = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRecord"]);
                    _totalPage = (int)Math.Ceiling((double)_totalRecord / _pageSize);
                }
                if (ds.Tables.Count > 1)
                    dgvQuotes.DataSource = ds.Tables[1];
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); return; }

            if (dgvQuotes.Columns.Contains("IDBaoGia"))
                dgvQuotes.Columns["IDBaoGia"].Visible = false;

            foreach (DataGridViewColumn col in dgvQuotes.Columns)
                col.ReadOnly = !(col is DataGridViewButtonColumn);

            dgvQuotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQuotes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvQuotes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvQuotes.ColumnHeadersHeight = 35;
            dgvQuotes.ScrollBars = ScrollBars.Vertical;

            if (dgvQuotes.Rows.Count > 0)
            {
                dgvQuotes.FirstDisplayedScrollingRowIndex = 0;
                dgvQuotes.ClearSelection();
                dgvQuotes.CurrentCell = null;
            }

            if (dgvQuotes.Columns.Contains("colSTT"))
                dgvQuotes.Columns["colSTT"].Width = 40;

            foreach (DataGridViewColumn col in dgvQuotes.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            UpdatePaging();

            // Reset panel mức SL
            if (dgvLevels != null)
            {
                dgvLevels.Rows.Clear();
                if (dgvLevels.Columns.Contains("chkSel"))
                    dgvLevels.Columns["chkSel"].ReadOnly = false;
            }
            _currentQuoteId = 0;

            if (lblLevels != null)
                lblLevels.Text = "📋  Mức số lượng — click vào dòng báo giá để xem các mức:";
        }

        // ─────────────────────────────────────────────────────
        // PHÂN TRANG
        // ─────────────────────────────────────────────────────

        private void UpdatePaging()
        {
            int start = (_currentPage - 1) * _pageSize + 1;
            int end = Math.Min(_currentPage * _pageSize, _totalRecord);
            lblPaging.Text = $"Hiển thị {start}-{end} / {_totalRecord} báo giá";
            btnPageNumber.Text = _currentPage.ToString();
        }

        // ─────────────────────────────────────────────────────
        // FORMAT SỐ TIỀN
        // ─────────────────────────────────────────────────────

        private void dgvQuotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvQuotes.Columns[e.ColumnIndex].Name == "colTongBaoKhach" && e.Value != null)
            {
                // Đã format rồi (VD: "500,000 vnđ") → bỏ qua
                if (e.Value.ToString().EndsWith("vnđ", StringComparison.OrdinalIgnoreCase)
                 || e.Value.ToString().EndsWith("đ", StringComparison.OrdinalIgnoreCase))
                { e.FormattingApplied = true; return; }

                // Raw number → format lại
                if (double.TryParse(e.Value.ToString(), out double money))
                { e.Value = money.ToString("N0") + " vnđ"; e.FormattingApplied = true; }
            }
        }

        // ─────────────────────────────────────────────────────
        // [FIX] CELL CLICK — load mức SL + xử lý nút thao tác
        // ─────────────────────────────────────────────────────

        private void dgvQuotes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // [FIX] Lấy quoteId trực tiếp từ dòng được click
            // → DataBinding đã xong ở thời điểm này (CellClick xảy ra SAU DataBindingComplete)
            int quoteId = 0;
            try { quoteId = Convert.ToInt32(dgvQuotes.Rows[e.RowIndex].Cells["IDBaoGia"].Value); }
            catch { return; }
            if (quoteId <= 0) return;

            // Phải load mức TRƯỚC khi đọc Columns[e.ColumnIndex]: click vào tiêu đề hàng có ColumnIndex = -1 → ném lỗi và không bao giờ vào LoadQuoteLevels.
            bool levelsEmpty = dgvLevels == null || dgvLevels.Rows.Cast<DataGridViewRow>().All(r => r.IsNewRow);
            if (quoteId != _currentQuoteId || levelsEmpty)
            {
                _currentQuoteId = quoteId;
                LoadQuoteLevels(quoteId);
            }

            string columnName = (e.ColumnIndex >= 0 && e.ColumnIndex < dgvQuotes.Columns.Count)
                ? (dgvQuotes.Columns[e.ColumnIndex].Name ?? "")
                : "";

            // Xử lý nút thao tác
            try
            {
                if (columnName == "btnView")
                {
                    int detailId = GetSelectedDetailId();
                    using (var frm = new frmPriceCalculationDetail(quoteId, detailId))
                        frm.ShowDialog();
                }
                else if (columnName == "btnEdit")
                {
                    string editStatus = dgvQuotes.Rows[e.RowIndex].Cells["colStatus"].Value?.ToString() ?? "";
                    if (editStatus == "Đã duyệt" || editStatus == "Khách duyệt" || editStatus == "Đã ký"
                     || editStatus == "Đang sản xuất" || editStatus == "Hoàn thành")
                    {
                        MessageBox.Show(
                            $"Không thể sửa báo giá ở trạng thái \"{editStatus}\"!",
                            "Không thể sửa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    using (var frm = new frmPriceCalculation(quoteId)) frm.ShowDialog();
                    LoadQuoteGrid();
                }
                else if (columnName == "btnApprove")
                {
                    string currentStatus = dgvQuotes.Rows[e.RowIndex].Cells["colStatus"].Value?.ToString() ?? "";
                    if (currentStatus != "Chờ duyệt")
                    {
                        MessageBox.Show(
                            $"Không thể duyệt báo giá ở trạng thái \"{currentStatus}\"!\nChỉ báo giá đang \"Chờ duyệt\" mới được duyệt.",
                            "Không thể thực hiện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Nếu vừa mới load levels lần đầu → thông báo chọn mức
                    int tierCount = dgvLevels?.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) ?? 0;

                    if (tierCount > 1)
                    {
                        var checkedTierIds = CollectCheckedTierIds();
                        if (checkedTierIds.Count != 1)
                        {
                            MessageBox.Show(
                                "Báo giá có nhiều mức số lượng.\nVui lòng tích chọn đúng một mức cần duyệt rồi bấm ✔ lại.",
                                "Chọn mức giá", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (MessageBox.Show("Bạn có chắc muốn duyệt báo giá này?", "Xác nhận",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        _repo.SetQuotePrimaryTier(quoteId, checkedTierIds[0]);
                    }
                    else if (tierCount == 1)
                    {
                        if (MessageBox.Show("Bạn có chắc muốn duyệt báo giá này?", "Xác nhận",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        var checkedTierIds = CollectCheckedTierIds();
                        _repo.SetQuotePrimaryTier(quoteId,
                            checkedTierIds.Count >= 1
                                ? checkedTierIds[0]
                                : Convert.ToInt32(dgvLevels!.Rows[0].Cells["colLvlId"].Value));
                    }
                    else
                    {
                        if (MessageBox.Show("Bạn có chắc muốn duyệt báo giá này?", "Xác nhận",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    }

                    _repo.UpdateQuoteStatus(quoteId, "Đã duyệt");
                    MessageBox.Show("Đã duyệt báo giá!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQuoteGrid();
                }
                else if (columnName == "btnCancel")
                {
                    if (MessageBox.Show("Bạn có chắc muốn hủy báo giá này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                    _repo.UpdateQuoteStatus(quoteId, "Huỷ");
                    MessageBox.Show("✅ Đã hủy báo giá!", "Thành công");
                    LoadQuoteGrid();
                }
                else if (columnName == "btnComplete")
                {
                    string status = dgvQuotes.Rows[e.RowIndex].Cells["colStatus"].Value?.ToString() ?? "";
                    if (status != "Đang sản xuất")
                    {
                        MessageBox.Show($"⚠️ Chỉ có thể hoàn thành khi đang sản xuất!\nTrạng thái: {status}",
                            "Không thể thực hiện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (MessageBox.Show("Xác nhận hoàn thành sản xuất?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    _repo.UpdateQuoteStatus(quoteId, "Hoàn thành");
                    MessageBox.Show("✅ Hoàn thành sản xuất!", "Thành công");
                    LoadQuoteGrid();
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        private void dgvQuotes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dgvQuotes.Columns.Contains("btnView")) return;
            foreach (DataGridViewRow row in dgvQuotes.Rows)
            {
                row.Cells["btnView"].Value = "👁";
                row.Cells["btnEdit"].Value = "✏";
                row.Cells["btnApprove"].Value = "✔";
                row.Cells["btnCancel"].Value = "✖";
                row.Cells["btnComplete"].Value = "✅";
            }
        }

        private void dgvQuotes_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            switch (dgvQuotes.Columns[e.ColumnIndex].Name)
            {
                case "btnView": e.ToolTipText = "Xem chi tiết báo giá"; break;
                case "btnEdit": e.ToolTipText = "Chỉnh sửa báo giá"; break;
                case "btnApprove": e.ToolTipText = "Duyệt báo giá"; break;
                case "btnCancel": e.ToolTipText = "Hủy báo giá"; break;
                case "btnComplete": e.ToolTipText = "Hoàn thành sản xuất"; break;
            }
        }

        // ─────────────────────────────────────────────────────
        // PHÂN TRANG / TÌM KIẾM / RESET
        // ─────────────────────────────────────────────────────

        private void btnPrev_Click(object sender, EventArgs e)
        { if (_currentPage > 1) { _currentPage--; LoadQuoteGrid(txtSearch.Text); } }

        private void btnNext_Click(object sender, EventArgs e)
        { if (_currentPage < _totalPage) { _currentPage++; LoadQuoteGrid(txtSearch.Text); } }

        private void btnSearch_Click(object sender, EventArgs e)
        { _currentPage = 1; LoadQuoteGrid(txtSearch.Text.Trim()); }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) { _currentPage = 1; LoadQuoteGrid(txtSearch.Text.Trim()); } }

        private void btnReset_Click(object sender, EventArgs e)
        { txtSearch.Text = ""; LoadQuoteGrid(); dgvQuotes.ClearSelection(); dgvQuotes.CurrentCell = null; }

        private void btnAddNew_Click(object sender, EventArgs e)
        { using (var frm = new frmPriceCalculation()) frm.ShowDialog(); LoadQuoteGrid(); }

        // ─────────────────────────────────────────────────────
        // XUẤT EXCEL
        // ─────────────────────────────────────────────────────

        private List<int> CollectCheckedTierIds()
        {
            var ids = new List<int>();
            if (dgvLevels == null) return ids;
            foreach (DataGridViewRow row in dgvLevels.Rows)
            {
                if (row.IsNewRow) continue;
                if (!Convert.ToBoolean(row.Cells["chkSel"].Value ?? false)) continue;
                if (int.TryParse(row.Cells["colLvlId"].Value?.ToString(), out int id))
                    ids.Add(id);
            }
            return ids;
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentQuoteId == 0)
            { MessageBox.Show("⚠️ Vui lòng click vào dòng báo giá trước!", "Chưa chọn báo giá"); return; }

            var selected = new List<(int qty, double unitPrice, double costPerUnit, double total)>();
            if (dgvLevels != null)
            {
                foreach (DataGridViewRow row in dgvLevels.Rows)
                {
                    if (row.IsNewRow || !Convert.ToBoolean(row.Cells["chkSel"].Value ?? false)) continue;
                    if (int.TryParse(row.Cells["rawQty"].Value?.ToString(), out int qty)
                     && double.TryParse(row.Cells["rawDonGia"].Value?.ToString(), out double unit)
                     && double.TryParse(row.Cells["rawGiaThanh"].Value?.ToString(), out double cost)
                     && double.TryParse(row.Cells["rawTongTien"].Value?.ToString(), out double tot))
                        selected.Add((qty, unit, cost, tot));
                }
            }

            if (selected.Count == 0)
            { MessageBox.Show("⚠️ Chưa tích chọn mức số lượng nào!\nHãy tích vào ô ✓ bên dưới.", "Chưa chọn mức SL"); return; }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel|*.xlsx";
                dialog.FileName = $"BaoGia_{DateTime.Today:ddMMyyyy}.xlsx";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    ExportQuoteToExcel(_currentQuoteId, dialog.FileName, selected);
                    MessageBox.Show($"✅ Xuất Excel thành công!\n{dialog.FileName}", "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    { FileName = dialog.FileName, UseShellExecute = true });
                }
                catch (Exception ex) { MessageBox.Show($"❌ Lỗi xuất Excel:\n{ex.Message}", "Lỗi"); }
            }
        }

        private void ExportQuoteToExcel(
            int quoteId, string outputPath,
            List<(int qty, double unitPrice, double costPerUnit, double total)> levels)
        {
            var ds = _repo.GetQuoteForExcel(quoteId);
            if (ds.Tables.Count < 1) throw new Exception("Không lấy được dữ liệu báo giá!");

            string quoteCode = "", quoteDate = "", validity = "30 ngày", deliveryTime = "";
            string productName = "", productSize = "", material = "";
            string customerName = "", address = "", taxCode = "", contact = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                quoteCode = r["Ma_Bao_Gia"].ToString();
                quoteDate = r["Ngay_Bao_Gia"].ToString();
                validity = r["Hieu_Luc_Bao_Gia_Ngay"] != DBNull.Value
                               ? Convert.ToInt32(r["Hieu_Luc_Bao_Gia_Ngay"]) + " ngày" : "30 ngày";
                deliveryTime = r["Thoi_Gian_Giao_Hang_Du_Kien"].ToString();
                productName = r["Ten_San_Pham"].ToString();
                productSize = r["Kich_Thuoc_Thanh_Pham"].ToString();
                material = r["Ten_Loai_Giay"].ToString();
                customerName = r["Ten_Khach_Hang"].ToString();
                address = r["Dia_Chi"].ToString();
                taxCode = r["MST"].ToString();
                contact = r["Nguoi_Lien_He"].ToString();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Báo Giá");
                ws.View.ShowGridLines = false;
                ws.Column(1).Width = 6; ws.Column(2).Width = 28; ws.Column(3).Width = 22;
                ws.Column(4).Width = 20; ws.Column(5).Width = 14; ws.Column(6).Width = 18;
                ws.Column(7).Width = 20; ws.Column(8).Width = 16;

                Color colorNavy = Color.FromArgb(30, 58, 95);
                Color colorBlue = Color.FromArgb(37, 99, 235);
                Color colorGray = Color.FromArgb(248, 250, 252);
                Color colorDGray = Color.FromArgb(100, 116, 139);
                Color colorWhite = Color.White;

                void S(ExcelRange r, Color bg, Color fg, bool bold = false, int size = 10,
                       string ha = "left", string va = "center", bool wrap = false, bool brd = false)
                {
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(bg);
                    r.Style.Font.Color.SetColor(fg);
                    r.Style.Font.Bold = bold; r.Style.Font.Size = size; r.Style.Font.Name = "Arial";
                    r.Style.VerticalAlignment = va == "top" ? ExcelVerticalAlignment.Top : ExcelVerticalAlignment.Center;
                    r.Style.HorizontalAlignment = ha == "center" ? ExcelHorizontalAlignment.Center
                                                : ha == "right" ? ExcelHorizontalAlignment.Right
                                                                  : ExcelHorizontalAlignment.Left;
                    r.Style.WrapText = wrap;
                    if (brd)
                    {
                        var bs = ExcelBorderStyle.Thin; var bc = Color.FromArgb(203, 213, 225);
                        r.Style.Border.Top.Style = bs; r.Style.Border.Top.Color.SetColor(bc);
                        r.Style.Border.Bottom.Style = bs; r.Style.Border.Bottom.Color.SetColor(bc);
                        r.Style.Border.Left.Style = bs; r.Style.Border.Left.Color.SetColor(bc);
                        r.Style.Border.Right.Style = bs; r.Style.Border.Right.Color.SetColor(bc);
                    }
                }

                ws.Row(1).Height = 8; ws.Row(2).Height = 32; ws.Row(3).Height = 18; ws.Row(4).Height = 18;
                S(ws.Cells["A1:H4"], colorNavy, colorWhite);
                ws.Cells["A2:D2"].Merge = true; ws.Cells["A2"].Value = "CÔNG TY IN ẤN BAO LÌ XÌ";
                S(ws.Cells["A2:D2"], colorNavy, colorWhite, true, 14);
                ws.Cells["A3:D3"].Merge = true; ws.Cells["A3"].Value = "TP. Hồ Chí Minh  |  Tel: 0901 234 567";
                S(ws.Cells["A3:D3"], colorNavy, Color.FromArgb(189, 213, 234), false, 9);
                ws.Cells["A4:D4"].Merge = true; ws.Cells["A4"].Value = "Email: info@company.vn";
                S(ws.Cells["A4:D4"], colorNavy, Color.FromArgb(189, 213, 234), false, 9);
                ws.Cells["E2:H2"].Merge = true; ws.Cells["E2"].Value = "BÁO GIÁ";
                S(ws.Cells["E2:H2"], colorNavy, colorWhite, true, 22, "right");
                ws.Cells["E3:H3"].Merge = true; ws.Cells["E3"].Value = $"Số: {quoteCode}";
                S(ws.Cells["E3:H3"], colorNavy, Color.FromArgb(147, 197, 253), false, 10, "right");
                ws.Cells["E4:H4"].Merge = true; ws.Cells["E4"].Value = $"Ngày: {quoteDate}";
                S(ws.Cells["E4:H4"], colorNavy, Color.FromArgb(189, 213, 234), false, 9, "right");

                ws.Row(5).Height = 8;
                string[] lKH = { "THÔNG TIN KHÁCH HÀNG", "Tên khách hàng:", "Địa chỉ:", "Mã số thuế:", "Người liên hệ:" };
                string[] vKH = { "", customerName, address, taxCode, contact };
                string[] lBG = { "THÔNG TIN BÁO GIÁ", "Số báo giá:", "Ngày báo giá:", "Hiệu lực:", "Thời gian giao:" };
                string[] vBG = { "", quoteCode, quoteDate, validity, deliveryTime };
                for (int i = 0; i < 5; i++)
                {
                    int rr = 6 + i; bool h = i == 0; ws.Row(rr).Height = 20;
                    ws.Cells[rr, 1, rr, 2].Merge = true; ws.Cells[rr, 1].Value = lKH[i];
                    S(ws.Cells[rr, 1, rr, 2], h ? colorNavy : colorGray, h ? colorWhite : colorDGray, h, h ? 10 : 9, h ? "center" : "left", "center", false, !h);
                    ws.Cells[rr, 3, rr, 4].Merge = true; ws.Cells[rr, 3].Value = vKH[i];
                    S(ws.Cells[rr, 3, rr, 4], h ? colorNavy : colorWhite, h ? colorWhite : Color.FromArgb(30, 41, 59), h, 10, "left", "center", true, !h);
                    ws.Cells[rr, 5, rr, 6].Merge = true; ws.Cells[rr, 5].Value = lBG[i];
                    S(ws.Cells[rr, 5, rr, 6], h ? colorNavy : colorGray, h ? colorWhite : colorDGray, h, h ? 10 : 9, h ? "center" : "left", "center", false, !h);
                    ws.Cells[rr, 7, rr, 8].Merge = true; ws.Cells[rr, 7].Value = vBG[i];
                    S(ws.Cells[rr, 7, rr, 8], h ? colorNavy : colorWhite, h ? colorWhite : Color.FromArgb(30, 41, 59), false, 10, "left", "center", false, !h);
                }

                ws.Row(11).Height = 10; ws.Row(12).Height = 14; ws.Row(13).Height = 28;
                ws.Cells["A12:H12"].Merge = true; ws.Cells["A12"].Value = "CHI TIẾT SẢN PHẨM BÁO GIÁ";
                S(ws.Cells["A12:H12"], colorBlue, colorWhite, true, 11, "center");
                string[] hdrs = { "STT", "Tên sản phẩm", "Kích thước", "Chất liệu", "Số lượng", "Giá thành/cái (đ)", "Đơn giá KH (đ)", "Thành tiền (đ)" };
                string[] hAligns = { "center", "left", "center", "center", "right", "right", "right", "right" };
                for (int ci = 0; ci < hdrs.Length; ci++)
                {
                    ws.Cells[13, ci + 1].Value = hdrs[ci];
                    S(ws.Cells[13, ci + 1, 13, ci + 1], colorNavy, colorWhite, true, 10, hAligns[ci], "center", true, true);
                }

                int startRow = 14;
                for (int i = 0; i < levels.Count; i++)
                {
                    int r2 = startRow + i; var lv = levels[i]; Color rowBg = i % 2 == 0 ? colorWhite : colorGray;
                    ws.Row(r2).Height = 28;
                    ws.Cells[r2, 1].Value = i + 1; ws.Cells[r2, 2].Value = productName;
                    ws.Cells[r2, 3].Value = productSize; ws.Cells[r2, 4].Value = material;
                    ws.Cells[r2, 5].Value = lv.qty; ws.Cells[r2, 6].Value = lv.costPerUnit;
                    ws.Cells[r2, 7].Value = lv.unitPrice; ws.Cells[r2, 8].Value = lv.total;
                    string[] dAligns = { "center", "left", "center", "center", "right", "right", "right", "right" };
                    for (int ci = 1; ci <= 8; ci++)
                    {
                        S(ws.Cells[r2, ci], rowBg, Color.FromArgb(30, 41, 59), ci == 7, 10, dAligns[ci - 1], "center", ci == 2, true);
                        if (ci >= 5) ws.Cells[r2, ci].Style.Numberformat.Format = "#,##0";
                    }
                }

                int sumStart = startRow + levels.Count; ws.Row(sumStart).Height = 8;
                int row15 = sumStart + 1, row16 = sumStart + 2, row17 = sumStart + 3;
                string dataRange = $"H{startRow}:H{startRow + levels.Count - 1}";
                void SumRow(int r2, string label, string formula, bool bold, Color bg, Color fg)
                {
                    ws.Row(r2).Height = 22; ws.Cells[r2, 1, r2, 6].Merge = true; S(ws.Cells[r2, 1, r2, 6], bg, bg);
                    ws.Cells[r2, 7].Value = label; S(ws.Cells[r2, 7], bg, colorDGray, bold, 10, "right", "center", false, true);
                    ws.Cells[r2, 8].Formula = formula; ws.Cells[r2, 8].Style.Numberformat.Format = "#,##0";
                    S(ws.Cells[r2, 8], bg, fg, bold, bold ? 12 : 11, "right", "center", false, true);
                }
                SumRow(row15, "Tổng tiền chưa VAT:", $"=SUM({dataRange})", false, colorGray, Color.FromArgb(30, 41, 59));
                SumRow(row16, "VAT (10%):", $"=H{row15}*10%", false, colorGray, Color.FromArgb(30, 41, 59));
                SumRow(row17, "TỔNG THANH TOÁN:", $"=H{row15}+H{row16}", true, colorNavy, colorWhite);

                int noteRow = row17 + 2; ws.Row(noteRow).Height = 14; ws.Row(noteRow + 1).Height = 50;
                ws.Cells[noteRow, 1, noteRow, 8].Merge = true; ws.Cells[noteRow, 1].Value = "GHI CHÚ & ĐIỀU KHOẢN";
                S(ws.Cells[noteRow, 1, noteRow, 8], colorBlue, colorWhite, true, 10, "left", "center");
                ws.Cells[noteRow + 1, 1, noteRow + 1, 8].Merge = true;
                ws.Cells[noteRow + 1, 1].Value =
                    $"• Giá trên chưa bao gồm VAT 10%.\n• Báo giá có hiệu lực trong {validity} kể từ ngày phát hành.\n" +
                    $"• Thời gian giao hàng: {deliveryTime} sau khi nhận đặt cọc.\n• Đặt cọc 50% giá trị đơn hàng để tiến hành sản xuất.";
                S(ws.Cells[noteRow + 1, 1, noteRow + 1, 8], colorGray, colorDGray, false, 9, "left", "top", true, true);

                int sigRow = noteRow + 3; ws.Row(sigRow).Height = 18; ws.Row(sigRow + 1).Height = 55;
                ws.Cells[sigRow, 1, sigRow, 4].Merge = true; ws.Cells[sigRow, 1].Value = "ĐẠI DIỆN KHÁCH HÀNG";
                S(ws.Cells[sigRow, 1, sigRow, 4], colorNavy, colorWhite, true, 10, "center");
                ws.Cells[sigRow, 5, sigRow, 8].Merge = true; ws.Cells[sigRow, 5].Value = "ĐẠI DIỆN CÔNG TY";
                S(ws.Cells[sigRow, 5, sigRow, 8], colorNavy, colorWhite, true, 10, "center");
                ws.Cells[sigRow + 1, 1, sigRow + 1, 4].Merge = true; ws.Cells[sigRow + 1, 1].Value = "(Ký, ghi rõ họ tên)";
                S(ws.Cells[sigRow + 1, 1, sigRow + 1, 4], colorGray, colorDGray, false, 9, "center", "bottom", false, true);
                ws.Cells[sigRow + 1, 5, sigRow + 1, 8].Merge = true; ws.Cells[sigRow + 1, 5].Value = "(Ký, ghi rõ họ tên)";
                S(ws.Cells[sigRow + 1, 5, sigRow + 1, 8], colorGray, colorDGray, false, 9, "center", "bottom", false, true);

                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.PaperSize = ePaperSize.A4;
                ws.PrinterSettings.FitToPage = true; ws.PrinterSettings.FitToWidth = 1; ws.PrinterSettings.FitToHeight = 0;
                pkg.SaveAs(new FileInfo(outputPath));
            }
        }
    }
}