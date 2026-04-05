// ═══════════════════════════════════════════════════════════════════
// ║  frmSelectSupplierMaterial.cs — POPUP CHỌN VẬT TƯ ĐÃ LƯU CỦA NCC  ║
// ═══════════════════════════════════════════════════════════════════
// Mục đích: Khi ấn "Thêm dòng" trong đơn mua hàng,
// hiển thị danh sách vật tư đã lưu của NCC để chọn nhanh.
// ─────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace PrintingManagement
{
    public partial class frmSelectSupplierMaterial : Form
    {
        // Dữ liệu trả về: danh sách vật tư được chọn
        public List<MaterialPickItem> SelectedMaterials { get; private set; } = new();

        private readonly int _supplierId;
        private readonly string _supplierName;
        private readonly PurchaseOrderRepository _repo = new();

        public frmSelectSupplierMaterial(int supplierId, string supplierName)
        {
            _supplierId = supplierId;
            _supplierName = supplierName;
            InitializeComponent();
            this.Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            lblTitle.Text = $"Vật tư của: {_supplierName}";
            // Tránh ghosting / sọc khi vẽ chồng lên vùng nút (Guna2 + Dock Fill).
            foreach (Control c in panelButtons.Controls)
            {
                if (c is Guna2Button b)
                    b.ShadowDecoration.Enabled = false;
            }
            SetupGrid();
            LoadMaterials();
        }

        private void SetupGrid()
        {
            dgvMaterials.ColumnHeadersHeight = 38;
            dgvMaterials.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvMaterials.RowTemplate.Height = 36;
            dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterials.MultiSelect = true;
            dgvMaterials.ReadOnly = true;
            dgvMaterials.BorderStyle = BorderStyle.None;
            dgvMaterials.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMaterials.GridColor = Color.FromArgb(231, 229, 255);
            dgvMaterials.BackgroundColor = Color.White;
            dgvMaterials.EnableHeadersVisualStyles = false;

            dgvMaterials.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(100, 88, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvMaterials.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(71, 69, 94)
            };

            dgvMaterials.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            { BackColor = Color.FromArgb(248, 248, 255) };

            dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCode", HeaderText = "Mã hàng", Width = 110,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 100, 200)
                }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Tên vật tư",
                MinimumWidth = 240,
                FillWeight = 300,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUnit", HeaderText = "ĐVT", Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrice", HeaderText = "Đơn giá", Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId", HeaderText = "", Width = 0, Visible = false
            });

            dgvMaterials.RowHeadersVisible = false;
            dgvMaterials.AllowUserToResizeRows = false;
            dgvMaterials.AllowUserToAddRows = false;
            dgvMaterials.AllowUserToDeleteRows = false;
            dgvMaterials.AllowUserToResizeRows = false;

            // Double-click = chọn + đóng
            dgvMaterials.DoubleClick += (_, _) => ConfirmSelection();
        }

        private void LoadMaterials(string? search = null)
        {
            try
            {
                var dt = _repo.GetMaterialsBySupplier(_supplierId);
                dgvMaterials.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    string code = row["Ma_Nguyen_Lieu"]?.ToString() ?? "";
                    string name = row["Ten_Nguyen_Lieu"]?.ToString() ?? "";

                    // Lọc theo tìm kiếm
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        if (!code.Contains(search, StringComparison.OrdinalIgnoreCase)
                            && !name.Contains(search, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }

                    decimal price = Convert.ToDecimal(row["Don_Gia_Mac_Dinh"]);

                    dgvMaterials.Rows.Add(
                        code,
                        name,
                        row["Don_Vi_Tinh"]?.ToString() ?? "",
                        price.ToString("N0"),
                        Convert.ToInt32(row["MaterialId"]));
                }

                lblCount.Text = $"{dgvMaterials.RowCount} vật tư";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}", "Lỗi");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
            => LoadMaterials(txtSearch.Text.Trim());

        private void ConfirmSelection()
        {
            SelectedMaterials.Clear();
            foreach (DataGridViewRow row in dgvMaterials.SelectedRows)
            {
                SelectedMaterials.Add(new MaterialPickItem
                {
                    MaterialId = Convert.ToInt32(row.Cells["colId"].Value),
                    MaterialCode = row.Cells["colCode"].Value?.ToString() ?? "",
                    MaterialName = row.Cells["colName"].Value?.ToString() ?? "",
                    Unit = row.Cells["colUnit"].Value?.ToString() ?? "",
                    UnitPrice = Convert.ToDecimal(
                        row.Cells["colPrice"].Value?.ToString().Replace(",", "") ?? "0")
                });
            }

            if (SelectedMaterials.Count == 0)
            {
                MessageBox.Show(
                    "Bạn chưa chọn dòng nào trong danh sách.\n\n" +
                    "• Chọn một hoặc nhiều dòng rồi nhấn «Chọn», hoặc\n" +
                    "• Nhấn «Hàng mới (chưa trong danh mục)» để thêm dòng và nhập mã/tên hàng trên đơn.",
                    "Chọn vật tư",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Đóng popup với OK nhưng không chọn dòng — frmPurchaseOrder sẽ gọi AddNewRow() (mã tự sinh, nhập tay).
        /// </summary>
        private void btnManualNew_Click(object? sender, EventArgs e)
        {
            SelectedMaterials.Clear();
            DialogResult = DialogResult.OK;
        }

        private void btnOK_Click(object sender, EventArgs e) => ConfirmSelection();

        private void btnCancel_Click(object sender, EventArgs e)
            => DialogResult = DialogResult.Cancel;

        private void dgvMaterials_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            { e.Handled = true; ConfirmSelection(); }
        }


        // ═══════════════════════════════════════════════════════════
        // INNER CLASS — VẬT TƯ CHỌN TỪ POPUP
        // ═══════════════════════════════════════════════════════════

        public class MaterialPickItem
        {
            public int MaterialId { get; set; }
            public string MaterialCode { get; set; } = "";
            public string MaterialName { get; set; } = "";
            public string Unit { get; set; } = "";
            public decimal UnitPrice { get; set; }
        }
    }
}
