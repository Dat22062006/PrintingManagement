// ═══════════════════════════════════════════════════════════════════
// ║  frmSupplierPayment.cs - THANH TOÁN NHÀ CUNG CẤP               ║
// [FIX] colOrderCode hiển thị SỐ HÓA ĐƠN (So_Hoa_Don từ PHIEU_NHAP_KHO)
//       thay vì số đơn đặt hàng (Ma_Don_Hang).
//       SP đã sửa: sp_GetSupplierDebtList trả về OrderCode = So_Hoa_Don
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmSupplierPayment : Form
    {
        private readonly SupplierPaymentRepository _repo = new();
        private int _supplierId = 0;

        public frmSupplierPayment()
        {
            InitializeComponent();
            this.Load += frmSupplierPayment_Load;
        }

        private void frmSupplierPayment_Load(object sender, EventArgs e)
        {
            // Phương thức thanh toán
            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange(new string[]
                { "Tiền mặt", "Ủy nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPaymentMethod.SelectedIndex = 0;

            // TK hạch toán (chỉ dùng khi in phiếu)
            cboDebitAccount.Items.Clear();
            cboDebitAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDebitAccount.Items.Add("331 - Phải trả nhà cung cấp");
            cboDebitAccount.SelectedIndex = 0;

            cboCreditAccount.Items.Clear();
            cboCreditAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCreditAccount.Items.AddRange(new string[]
                { "111 - Tiền mặt", "112 - Tiền gửi ngân hàng" });
            cboCreditAccount.SelectedIndex = 0;

            // Tự đổi TK Có theo phương thức
            cboPaymentMethod.SelectedIndexChanged += (s, ev) =>
                cboCreditAccount.SelectedIndex = cboPaymentMethod.Text == "Tiền mặt" ? 0 : 1;

            dtpPaymentDate.Value = DateTime.Today;

            SetupGrid();
            LoadSuppliers();
            UpdateTotal();
        }

        // ─────────────────────────────────────────────────────
        // LOAD NCC
        // ─────────────────────────────────────────────────────

        private void LoadSuppliers()
        {
            cboSupplier.SelectedIndexChanged -= cboSupplier_SelectedIndexChanged;
            cboSupplier.Items.Clear();
            cboSupplier.Items.Add(new SupplierItem());

            try
            {
                var dt = _repo.GetSuppliers();
                foreach (DataRow row in dt.Rows)
                    cboSupplier.Items.Add(new SupplierItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["Ma_NCC"].ToString(),
                        Name = row["Ten_NCC"].ToString()
                    });
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }

            cboSupplier.SelectedIndexChanged += cboSupplier_SelectedIndexChanged;
            cboSupplier.SelectedIndex = 0;
        }

        private void cboSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSupplier.SelectedItem is SupplierItem s && s.Id > 0)
            {
                _supplierId = s.Id;
                LoadDebtList(s.Id);
            }
            else
            {
                _supplierId = 0;
                dgvPaymentDetails.Rows.Clear();
                UpdateTotal();
            }
        }

        // ─────────────────────────────────────────────────────
        // LOAD DANH SÁCH HÓA ĐƠN CÒN NỢ
        // [FIX] OrderCode = So_Hoa_Don từ phiếu nhập (đã fix trong SP)
        // ─────────────────────────────────────────────────────

        private void LoadDebtList(int supplierId)
        {
            dgvPaymentDetails.Rows.Clear();

            try
            {
                var dt = _repo.GetDebtList(supplierId);

                foreach (DataRow row in dt.Rows)
                {
                    int orderId = Convert.ToInt32(row["OrderId"]);
                    string invoiceCode = row["OrderCode"].ToString();     // [FIX] số HĐ
                    string description = row["Description"].ToString();
                    double totalAmount = Convert.ToDouble(row["TotalAmount"]);
                    double paidAmount = Convert.ToDouble(row["PaidAmount"]);
                    double remaining = totalAmount - paidAmount;

                    // Ngày HĐ — có thể NULL nếu chưa nhập
                    string invoiceDate = "";
                    if (row["OrderDate"] != DBNull.Value)
                        invoiceDate = Convert.ToDateTime(row["OrderDate"]).ToString("dd/MM/yyyy");

                    // OriginalOrderId để link đúng sang THANH_TOAN_NCC
                    int originalOrderId = row.Table.Columns.Contains("OriginalOrderId")
                        ? Convert.ToInt32(row["OriginalOrderId"])
                        : orderId;

                    dgvPaymentDetails.Rows.Add(
                        false,
                        invoiceCode,    // [FIX] cột "Số hóa đơn" — đúng rồi
                        invoiceDate,
                        description,
                        totalAmount.ToString("N0"),
                        paidAmount.ToString("N0"),
                        remaining.ToString("N0"),
                        "0",
                        originalOrderId,    // [FIX] link đúng sang đơn hàng gốc để thanh toán
                        supplierId);
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }

            UpdateTotal();
        }

        // ─────────────────────────────────────────────────────
        // SETUP GRID
        // ─────────────────────────────────────────────────────

        private void SetupGrid()
        {
            var dgv = dgvPaymentDetails;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 36;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.ScrollBars = ScrollBars.Both;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);

            var styleRO = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 10f)
            };
            var styleEdit = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(255, 255, 220),
                ForeColor = Color.FromArgb(30, 80, 160),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };
            var styleRed = new DataGridViewCellStyle(styleRO)
            {
                ForeColor = Color.FromArgb(220, 38, 38),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };
            var styleGreen = new DataGridViewCellStyle(styleRO)
            {
                ForeColor = Color.FromArgb(22, 163, 74),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            dgv.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colSelect",
                HeaderText = "✓",
                Width = 40,
                TrueValue = true,
                FalseValue = false
            });

            // [FIX] Header rõ ràng là "Số hóa đơn"
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colOrderCode",
                HeaderText = "Số hóa đơn",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRO)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(30, 100, 200),
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colOrderDate",
                HeaderText = "Ngày HĐ",
                Width = 95,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRO)
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDescription",
                HeaderText = "Diễn giải",
                Width = 230,
                ReadOnly = true,
                DefaultCellStyle = styleRO
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalAmount",
                HeaderText = "Tổng tiền HĐ",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle(styleRO)
                { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaidAmount",
                HeaderText = "Đã trả",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = styleGreen
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRemainingDebt",
                HeaderText = "Còn nợ",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = styleRed
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaymentAmount",
                HeaderText = "Số tiền trả lần này",
                Width = 150,
                ReadOnly = false,
                DefaultCellStyle = styleEdit
            });

            // Cột ẩn
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrderId", Width = 0, Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSupplierId", Width = 0, Visible = false });

            dgv.CellValueChanged -= dgvPaymentDetails_CellValueChanged;
            dgv.CurrentCellDirtyStateChanged -= dgvPaymentDetails_CurrentCellDirtyStateChanged;
            dgv.CellEnter -= dgvPaymentDetails_CellEnter;
            dgv.CellValueChanged += dgvPaymentDetails_CellValueChanged;
            dgv.CurrentCellDirtyStateChanged += dgvPaymentDetails_CurrentCellDirtyStateChanged;
            dgv.CellEnter += dgvPaymentDetails_CellEnter;
        }

        private void dgvPaymentDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPaymentDetails.Columns[e.ColumnIndex].Name != "colPaymentAmount") return;
            BeginInvoke(new Action(() =>
            {
                if (dgvPaymentDetails.EditingControl is TextBox tb) tb.SelectAll();
            }));
        }

        private void dgvPaymentDetails_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPaymentDetails.IsCurrentCellDirty)
                dgvPaymentDetails.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvPaymentDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvPaymentDetails.Columns[e.ColumnIndex].Name == "colSelect")
            {
                var row = dgvPaymentDetails.Rows[e.RowIndex];
                bool chk = row.Cells["colSelect"].Value is bool b && b;
                if (chk)
                {
                    string rem = row.Cells["colRemainingDebt"].Value?.ToString().Replace(",", "") ?? "0";
                    row.Cells["colPaymentAmount"].Value = rem;
                    dgvPaymentDetails.CurrentCell = row.Cells["colPaymentAmount"];
                }
                else row.Cells["colPaymentAmount"].Value = "0";
            }
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            double total = 0;
            foreach (DataGridViewRow row in dgvPaymentDetails.Rows)
            {
                if (!(row.Cells["colSelect"].Value is bool chk && chk)) continue;
                double.TryParse(row.Cells["colPaymentAmount"].Value?.ToString().Replace(",", ""),
                    out double amt);
                total += amt;
            }
            lblTotalPayment.Text = total.ToString("N0") + " đ";
        }

        // ─────────────────────────────────────────────────────
        // NÚT THANH TOÁN
        // ─────────────────────────────────────────────────────

        private void btnPay_Click(object sender, EventArgs e)
        {
            if (_supplierId == 0)
            { MessageBox.Show("⚠️ Chọn nhà cung cấp!", "Thiếu"); return; }

            int cnt = 0;
            foreach (DataGridViewRow row in dgvPaymentDetails.Rows)
                if (row.Cells["colSelect"].Value is bool b && b) cnt++;

            if (cnt == 0)
            { MessageBox.Show("⚠️ Chọn ít nhất 1 hóa đơn!"); return; }

            double.TryParse(lblTotalPayment.Text.Replace(" đ", "").Replace(",", ""), out double total);

            if (MessageBox.Show(
                $"Xác nhận thanh toán {cnt} hóa đơn?\nTổng: {total:N0} đ\nPhương thức: {cboPaymentMethod.Text}",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var details = BuildPaymentDetailTable();
                decimal paid = _repo.SavePayment(_supplierId, dtpPaymentDate.Value.Date,
                    cboPaymentMethod.Text, details);
                MessageBox.Show($"✅ Đã thanh toán {cnt} hóa đơn!\nTổng: {paid:N0} đ",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDebtList(_supplierId);
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "Lỗi"); }
        }

        private DataTable BuildPaymentDetailTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("OrderId", typeof(int));
            dt.Columns.Add("PaymentAmount", typeof(decimal));

            foreach (DataGridViewRow row in dgvPaymentDetails.Rows)
            {
                if (!(row.Cells["colSelect"].Value is bool b && b)) continue;
                int.TryParse(row.Cells["colOrderId"].Value?.ToString(), out int oid);
                double.TryParse(row.Cells["colPaymentAmount"].Value?.ToString().Replace(",", ""), out double amt);
                if (amt <= 0) continue;
                dt.Rows.Add(oid, (decimal)amt);
            }
            return dt;
        }

        // ─────────────────────────────────────────────────────
        // NÚT IN PHIẾU
        // ─────────────────────────────────────────────────────

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_supplierId == 0)
            { MessageBox.Show("⚠️ Chọn nhà cung cấp!"); return; }

            var selectedRows = new List<DataGridViewRow>();
            double totalPay = 0;

            foreach (DataGridViewRow row in dgvPaymentDetails.Rows)
            {
                if (!(row.Cells["colSelect"].Value is bool b && b)) continue;
                double.TryParse(row.Cells["colPaymentAmount"].Value?.ToString().Replace(",", ""), out double amt);
                if (amt <= 0) continue;
                totalPay += amt;
                selectedRows.Add(row);
            }

            if (selectedRows.Count == 0)
            { MessageBox.Show("⚠️ Chọn ít nhất 1 hóa đơn và nhập số tiền > 0!"); return; }

            string payMethod = cboPaymentMethod.Text;
            string mainTitle = payMethod == "Tiền mặt" ? "PHIẾU CHI TIỀN MẶT" :
                                payMethod == "Ủy nhiệm chi" ? "ỦY NHIỆM CHI TRẢ TIỀN NHÀ CUNG CẤP" :
                                                               "PHIẾU CHI THANH TOÁN BẰNG SÉC";
            string supplierName = (cboSupplier.SelectedItem as SupplierItem)?.Name ?? "";
            string payDate = dtpPaymentDate.Value.ToString("dd/MM/yyyy");
            string tkNo = cboDebitAccount?.SelectedItem?.ToString() ?? "331 - Phải trả nhà cung cấp";
            string tkCo = cboCreditAccount?.SelectedItem?.ToString() ??
                                  (payMethod == "Tiền mặt" ? "111 - Tiền mặt" : "112 - Tiền gửi ngân hàng");

            var lines = new List<string[]>();
            foreach (var row in selectedRows)
                lines.Add(new[]
                {
                    row.Cells["colOrderCode"].Value?.ToString()      ?? "",
                    row.Cells["colOrderDate"].Value?.ToString()      ?? "",
                    row.Cells["colDescription"].Value?.ToString()    ?? "",
                    row.Cells["colRemainingDebt"].Value?.ToString()  ?? "0",
                    row.Cells["colPaymentAmount"].Value?.ToString()  ?? "0"
                });

            var printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.DefaultPageSettings.Landscape = true;

            printDoc.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var fTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
                var fBold = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fNormal = new Font("Segoe UI", 9f);
                var fSmall = new Font("Segoe UI", 8f);
                var bBlue = new SolidBrush(Color.FromArgb(30, 80, 200));

                int x = 50, y = 40;

                g.DrawString(mainTitle, fTitle, Brushes.Black, x, y); y += 28;
                g.DrawString($"Nhà cung cấp: {supplierName}", fNormal, Brushes.Black, x, y); y += 18;
                g.DrawString($"Ngày thanh toán: {payDate}", fNormal, Brushes.Black, x, y); y += 18;
                g.DrawString($"Phương thức: {payMethod}", fNormal, Brushes.Black, x, y); y += 18;
                g.DrawLine(Pens.LightGray, x, y, x + 720, y); y += 12;

                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 235, 245)), x, y, 720, 22);
                int[] cx = { 4, 100, 185, 430, 565 };
                var hdrs = new[] { "Số hóa đơn", "Ngày HĐ", "Diễn giải", "Còn nợ", "Số tiền trả" };
                for (int i = 0; i < hdrs.Length; i++)
                    g.DrawString(hdrs[i], fBold, Brushes.Black, x + cx[i], y + 3);
                y += 24;

                foreach (var line in lines)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        var fmt = i >= 3 ? new StringFormat { Alignment = StringAlignment.Far } : null;
                        if (fmt != null)
                            g.DrawString(line[i], fSmall, Brushes.Black, x + cx[i], y + 2, fmt);
                        else
                            g.DrawString(line[i], fSmall, Brushes.Black, x + cx[i], y + 2);
                    }
                    g.DrawLine(Pens.Gainsboro, x, y + 18, x + 720, y + 18);
                    y += 20;
                }

                y += 10;
                g.DrawLine(Pens.Gray, x + 400, y, x + 720, y); y += 6;
                g.DrawString("TỔNG THANH TOÁN:", fBold, Brushes.Black, x + 400, y);
                g.DrawString($"{totalPay:N0} đ", fBold, bBlue, x + 720, y,
                    new StringFormat { Alignment = StringAlignment.Far }); y += 24;

                g.DrawLine(Pens.LightGray, x, y, x + 720, y); y += 10;
                g.DrawString("ĐỊNH KHOẢN KẾ TOÁN:", fBold, bBlue, x, y); y += 18;
                g.DrawString($"Nợ  {tkNo}    {totalPay:N0} đ", fNormal, Brushes.Black, x, y); y += 18;
                g.DrawString($"Có  {tkCo}    {totalPay:N0} đ", fNormal, Brushes.Black, x, y); y += 30;

                int s1 = x + 60, s2 = x + 290, s3 = x + 540;
                g.DrawString("Người lập phiếu", fNormal, Brushes.Black, s1, y);
                g.DrawString("Kế toán trưởng", fNormal, Brushes.Black, s2, y);
                g.DrawString("Giám đốc", fNormal, Brushes.Black, s3, y); y += 14;
                g.DrawString("(Ký, ghi rõ họ tên)", fSmall, Brushes.Gray, s1, y);
                g.DrawString("(Ký, ghi rõ họ tên)", fSmall, Brushes.Gray, s2, y);
                g.DrawString("(Ký, ghi rõ họ tên)", fSmall, Brushes.Gray, s3, y);

                fTitle.Dispose(); fBold.Dispose(); fNormal.Dispose();
                fSmall.Dispose(); bBlue.Dispose();
            };

            new PrintPreviewDialog
            {
                Document = printDoc,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            }.ShowDialog(this);
        }

        // ─────────────────────────────────────────────────────
        // NÚT LÀM MỚI
        // ─────────────────────────────────────────────────────

        private void btnReset_Click(object sender, EventArgs e)
        {
            _supplierId = 0;
            cboSupplier.SelectedIndex = 0;
            cboPaymentMethod.SelectedIndex = 0;
            dtpPaymentDate.Value = DateTime.Today;
            dgvPaymentDetails.Rows.Clear();
            lblTotalPayment.Text = "0 đ";
            if (cboDebitAccount.Items.Count > 0) cboDebitAccount.SelectedIndex = 0;
            if (cboCreditAccount.Items.Count > 0) cboCreditAccount.SelectedIndex = 0;
        }

        private class SupplierItem
        {
            public int Id { get; set; }
            public string Code { get; set; } = "";
            public string Name { get; set; } = "-- Chọn nhà cung cấp --";

            public override string ToString()
                => string.IsNullOrEmpty(Code) ? Name : $"{Code} - {Name}";
        }
    }
}