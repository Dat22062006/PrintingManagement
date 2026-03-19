using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmPurchaseReport : Form
    {
        public frmPurchaseReport()
        {
            InitializeComponent();
            this.Load += frmPurchaseReport_Load;
        }

        private void frmPurchaseReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadData();
        }

        void SetupGrid()
        {
            var dgv = dgvTongHopMuaH;
            dgv.Columns.Clear();
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 220, 220);
            dgv.BackgroundColor = Color.White;
            dgv.RowTemplate.Height = 40;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 6, 0);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);

            var ro = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 10f)
            };

            // Cột NCC
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNCC",
                HeaderText = "NCC",
                FillWeight = 30,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Padding = new Padding(8, 0, 6, 0)
                }
            });

            // Cột Số đơn
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSoDon",
                HeaderText = "Số đơn",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Cột Tổng tiền
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTongTien",
                HeaderText = "Tổng tiền",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 10, 0)
                }
            });

            // Cột Đã trả
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDaTra",
                HeaderText = "Đã trả",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 10, 0)
                }
            });

            // Cột Còn nợ
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConNo",
                HeaderText = "Còn nợ",
                FillWeight = 18,
                DefaultCellStyle = new DataGridViewCellStyle(ro)
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Padding = new Padding(0, 0, 10, 0)
                }
            });

            // Tô màu cột Còn nợ
            dgv.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgv.Columns[e.ColumnIndex].Name != "colConNo") return;
                if (dgv.Rows[e.RowIndex].Tag is double v)
                    e.CellStyle.ForeColor = v > 0
                        ? Color.FromArgb(220, 38, 38)   // đỏ - còn nợ
                        : Color.FromArgb(22, 163, 74);  // xanh - hết nợ
            };
        }

        void LoadData()
        {
            dgvTongHopMuaH.Rows.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT
    ncc.Ten_NCC                                      AS TenNCC,
    COUNT(DISTINCT dh.id)                            AS SoDon,
    ISNULL(SUM(dh.Tong_Tien), 0)                     AS TongTien,
    ISNULL((SELECT SUM(tt.So_Tien)
            FROM THANH_TOAN_NCC tt
            WHERE tt.id_Nha_Cung_Cap = ncc.id), 0)   AS DaTra
FROM NHA_CUNG_CAP ncc
LEFT JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
GROUP BY ncc.id, ncc.Ten_NCC
HAVING COUNT(DISTINCT dh.id) > 0
ORDER BY ncc.Ten_NCC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        double tongTien = Convert.ToDouble(rd["TongTien"]);
                        double daTra = Convert.ToDouble(rd["DaTra"]);
                        double conNo = tongTien - daTra;

                        int idx = dgvTongHopMuaH.Rows.Add(
                            rd["TenNCC"].ToString(),
                            Convert.ToInt32(rd["SoDon"]),
                            tongTien.ToString("N0") + "đ",
                            daTra.ToString("N0") + "đ",
                            conNo.ToString("N0") + "đ"
                        );
                        dgvTongHopMuaH.Rows[idx].Tag = conNo;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi tải báo cáo:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}