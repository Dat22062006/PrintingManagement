// ╔══════════════════════════════════════════════════════════════════════╗
// ║  frmDebtReport.cs - BÁO CÁO CÔNG NỢ                                ║
// ║  - Công nợ phải thu (khách hàng) — quá hạn dựa vào Ngay_Den_Han    ║
// ║  - Công nợ phải trả (nhà cung cấp)                                  ║
// ╚══════════════════════════════════════════════════════════════════════╝

using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmDebtReport : Form
    {
        private string _loaiCongNo = "PhaiThu";

        public frmDebtReport()
        {
            InitializeComponent();
            this.Load += frmDebtReport_Load;
        }

        private void frmDebtReport_Load(object sender, EventArgs e)
        {
            SetupGrid();
            TinhTongCongNo();
            LoadCongNoPhaiThu();

            btnPhaiThu.Click += btnPhaiThu_Click;
            btnPhaiTra.Click += btnPhaiTra_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            btnPhaiThu.BackColor = Color.FromArgb(76, 175, 80);
            btnPhaiThu.ForeColor = Color.White;
        }

        // ═══════════════════════════════════════════════════════════════
        // TỔNG CÔNG NỢ
        // ═══════════════════════════════════════════════════════════════
        void TinhTongCongNo()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sqlPT = @"
SELECT
    ISNULL(SUM(CASE WHEN Con_Lai > 0 THEN Con_Lai ELSE 0 END), 0) AS TongConLai,
    COUNT(CASE WHEN Con_Lai > 0 THEN 1 END) AS SoKhachHang
FROM CONG_NO_KHACH_HANG";
                    var rdPT = new SqlCommand(sqlPT, conn).ExecuteReader();
                    if (rdPT.Read())
                    {
                        double tongPT = Convert.ToDouble(rdPT["TongConLai"]);
                        int soKH = Convert.ToInt32(rdPT["SoKhachHang"]);
                        lblCongNoPhaiThu.Text = (tongPT / 1_000_000).ToString("N0") + " triệu";
                        lblCongNoPhaiThu.ForeColor = Color.FromArgb(76, 175, 80);
                        lblSoKhachHang.Text = $"Từ {soKH} khách hàng";
                    }
                    rdPT.Close();

                    string sqlPTr = @"
SELECT ISNULL(SUM(TongNo),0) AS TongNo, ISNULL(SUM(DaTra),0) AS DaTra, COUNT(*) AS SoNCC
FROM (
    SELECT ncc.id,
        SUM(dh.Tong_Tien) AS TongNo,
        ISNULL((SELECT SUM(tt.So_Tien) FROM THANH_TOAN_NCC tt
                WHERE tt.id_Nha_Cung_Cap = ncc.id),0) AS DaTra
    FROM NHA_CUNG_CAP ncc
    JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
    GROUP BY ncc.id
) X";
                    var rdPTr = new SqlCommand(sqlPTr, conn).ExecuteReader();
                    if (rdPTr.Read())
                    {
                        double conLai = Math.Max(Convert.ToDouble(rdPTr["TongNo"]) - Convert.ToDouble(rdPTr["DaTra"]), 0);
                        int soNCC = Convert.ToInt32(rdPTr["SoNCC"]);
                        lblCongNoPhaiTraTra.Text = (conLai / 1_000_000).ToString("N0") + " triệu";
                        lblCongNoPhaiTraTra.ForeColor = Color.FromArgb(244, 67, 54);
                        lblSoNhaCungCap.Text = $"Cho {soNCC} nhà cung cấp";
                    }
                    rdPTr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tính tổng:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SETUP GRID
        // ═══════════════════════════════════════════════════════════════
        void SetupGrid()
        {
            var dgv = dgvCongNo;
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

            var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10f) };
            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colKhachHang", HeaderText = "Khách hàng", FillWeight = 26 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTongNo", HeaderText = "Tổng nợ", Width = 130, DefaultCellStyle = right });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDaThu", HeaderText = "Đã thu", Width = 130, DefaultCellStyle = right });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colConNo",
                HeaderText = "Còn nợ",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle(right) { Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(244, 67, 54) }
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQuaHan", HeaderText = "Quá hạn", Width = 120, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgayDenHan", HeaderText = "Ngày đến hạn", Width = 110, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", Visible = false });
        }

        // ═══════════════════════════════════════════════════════════════
        // CÔNG NỢ PHẢI THU — tính số ngày quá hạn từ Ngay_Den_Han
        // Nếu Ngay_Den_Han = NULL → mặc định = MAX(Ngay_Ban_Hang) + 7 ngày
        // Báo "hối" khi còn 1–2 ngày trước hạn
        // ═══════════════════════════════════════════════════════════════
        void LoadCongNoPhaiThu()
        {
            dgvCongNo.Rows.Clear();
            dgvCongNo.Columns["colKhachHang"].HeaderText = "Khách hàng";
            dgvCongNo.Columns["colDaThu"].HeaderText = "Đã thu";
            dgvCongNo.Columns["colNgayDenHan"].HeaderText = "Ngày đến hạn";

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Lấy ngày đến hạn nếu có, nếu không có thì mặc định = MAX(Ngay_Ban_Hang) + 7 ngày
                    string sql = @"
SELECT
    cn.id,
    kh.Ten_Khach_Hang,
    cn.Tong_No,
    cn.Da_Thu,
    cn.Con_Lai,
    ISNULL(
        (SELECT TOP 1 dbh.Ngay_Den_Han
         FROM DON_BAN_HANG dbh
         WHERE dbh.id_Khach_Hang = kh.id AND dbh.Ngay_Den_Han IS NOT NULL
         ORDER BY dbh.Ngay_Den_Han ASC),
        (SELECT DATEADD(DAY, 7, MAX(db2.Ngay_Ban_Hang)) FROM DON_BAN_HANG db2 WHERE db2.id_Khach_Hang = kh.id)
    ) AS Ngay_Den_Han
FROM CONG_NO_KHACH_HANG cn
JOIN KHACH_HANG kh ON kh.id = cn.id_Khach_Hang
WHERE cn.Con_Lai > 0
ORDER BY cn.Con_Lai DESC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        int id = Convert.ToInt32(rd["id"]);
                        string tenKH = rd["Ten_Khach_Hang"].ToString();
                        double tongNo = Convert.ToDouble(rd["Tong_No"]);
                        double daThu = Convert.ToDouble(rd["Da_Thu"]);
                        double conNo = Convert.ToDouble(rd["Con_Lai"]);

                        string quaHanText, ngayDenHanText;
                        Color bgQuaHan, fgQuaHan;

                        if (rd["Ngay_Den_Han"] == DBNull.Value)
                        {
                            // Nếu vẫn không có thông tin đơn hàng => không biết ngày đến hạn
                            quaHanText = "Chưa rõ";
                            ngayDenHanText = "—";
                            bgQuaHan = Color.FromArgb(243, 244, 246);
                            fgQuaHan = Color.FromArgb(107, 114, 128);
                        }
                        else
                        {
                            DateTime ngayDenHan = Convert.ToDateTime(rd["Ngay_Den_Han"]).Date;
                            ngayDenHanText = ngayDenHan.ToString("dd/MM/yyyy");

                            int daysUntil = (ngayDenHan - DateTime.Today).Days; // dương = còn x ngày, 0 = hôm nay, âm = quá hạn

                            if (daysUntil > 2)
                            {
                                // Còn nhiều ngày
                                quaHanText = $"Còn {daysUntil} ngày";
                                bgQuaHan = Color.FromArgb(232, 245, 233);
                                fgQuaHan = Color.FromArgb(56, 142, 60);
                            }
                            else if (daysUntil > 0 && daysUntil <= 2)
                            {
                                // Còn 1-2 ngày → bắt đầu hối (nhắc)
                                quaHanText = $"Sắp đến hạn ({daysUntil} ngày)";
                                bgQuaHan = Color.FromArgb(255, 249, 196); // nhẹ vàng
                                fgQuaHan = Color.FromArgb(245, 124, 0);
                            }
                            else if (daysUntil == 0)
                            {
                                // Đến hạn hôm nay
                                quaHanText = "Đến hạn hôm nay";
                                bgQuaHan = Color.FromArgb(255, 243, 205);
                                fgQuaHan = Color.FromArgb(230, 81, 0);
                            }
                            else
                            {
                                int overdue = Math.Abs(daysUntil);
                                if (overdue <= 7)
                                {
                                    // Quá hạn nhẹ (1–7 ngày)
                                    quaHanText = $"Quá {overdue} ngày";
                                    bgQuaHan = Color.FromArgb(255, 243, 205);
                                    fgQuaHan = Color.FromArgb(230, 81, 0);
                                }
                                else
                                {
                                    // Quá hạn nặng (>7 ngày)
                                    quaHanText = $"Quá {overdue} ngày";
                                    bgQuaHan = Color.FromArgb(255, 235, 238);
                                    fgQuaHan = Color.FromArgb(211, 47, 47);
                                }
                            }
                        }

                        int idx = dgvCongNo.Rows.Add(
                            tenKH,
                            tongNo.ToString("N0") + "đ",
                            daThu.ToString("N0") + "đ",
                            conNo.ToString("N0") + "đ",
                            quaHanText,
                            ngayDenHanText,
                            id
                        );

                        // Áp style cho ô Quá hạn
                        dgvCongNo.Rows[idx].Cells["colQuaHan"].Style.BackColor = bgQuaHan;
                        dgvCongNo.Rows[idx].Cells["colQuaHan"].Style.ForeColor = fgQuaHan;

                        // Làm nổi phần số nợ nếu cần
                        dgvCongNo.Rows[idx].Cells["colConNo"].Style.ForeColor =
                            conNo <= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                    }
                    rd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load công nợ phải thu:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CÔNG NỢ PHẢI TRẢ (NCC)
        // ═══════════════════════════════════════════════════════════════
        void LoadCongNoPhaiTra()
        {
            dgvCongNo.Rows.Clear();
            dgvCongNo.Columns["colKhachHang"].HeaderText = "Nhà cung cấp";
            dgvCongNo.Columns["colDaThu"].HeaderText = "Đã trả";
            dgvCongNo.Columns["colNgayDenHan"].HeaderText = "Ghi chú";

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
SELECT
    ncc.id,
    ncc.Ten_NCC,
    SUM(dh.Tong_Tien) AS TongNo,
    ISNULL((SELECT SUM(tt.So_Tien) FROM THANH_TOAN_NCC tt
            WHERE tt.id_Nha_Cung_Cap = ncc.id),0) AS DaTra
FROM NHA_CUNG_CAP ncc
JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
GROUP BY ncc.id, ncc.Ten_NCC
ORDER BY ncc.Ten_NCC";

                    var rd = new SqlCommand(sql, conn).ExecuteReader();
                    while (rd.Read())
                    {
                        int id = Convert.ToInt32(rd["id"]);
                        string tenNCC = rd["Ten_NCC"].ToString();
                        double tongNo = Convert.ToDouble(rd["TongNo"]);
                        double daTra = Convert.ToDouble(rd["DaTra"]);
                        double conNo = Math.Max(tongNo - daTra, 0);

                        string quaHanText;
                        Color bgQuaHan, fgQuaHan;

                        if (conNo <= 0)
                        {
                            quaHanText = "Đã trả đủ";
                            bgQuaHan = Color.FromArgb(232, 245, 233);
                            fgQuaHan = Color.FromArgb(56, 142, 60);
                        }
                        else
                        {
                            quaHanText = "Còn nợ";
                            bgQuaHan = Color.FromArgb(245, 245, 245);
                            fgQuaHan = Color.FromArgb(100, 100, 100);
                        }

                        int idx = dgvCongNo.Rows.Add(
                            tenNCC,
                            tongNo.ToString("N0") + "đ",
                            daTra.ToString("N0") + "đ",
                            conNo.ToString("N0") + "đ",
                            quaHanText,
                            "—",
                            id
                        );

                        dgvCongNo.Rows[idx].Cells["colQuaHan"].Style.BackColor = bgQuaHan;
                        dgvCongNo.Rows[idx].Cells["colQuaHan"].Style.ForeColor = fgQuaHan;
                        dgvCongNo.Rows[idx].Cells["colConNo"].Style.ForeColor =
                            conNo <= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                    }
                    rd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load công nợ phải trả:\n{ex.Message}", "Lỗi");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NÚT BẤM
        // ═══════════════════════════════════════════════════════════════
        private void btnPhaiThu_Click(object sender, EventArgs e)
        {
            _loaiCongNo = "PhaiThu";
            LoadCongNoPhaiThu();
            btnPhaiThu.BackColor = Color.FromArgb(76, 175, 80); btnPhaiThu.ForeColor = Color.White;
            btnPhaiTra.BackColor = Color.FromArgb(240, 240, 240); btnPhaiTra.ForeColor = Color.Black;
        }
        private void btnPhaiTra_Click(object sender, EventArgs e)
        {
            _loaiCongNo = "PhaiTra";
            LoadCongNoPhaiTra();
            btnPhaiTra.BackColor = Color.FromArgb(244, 67, 54); btnPhaiTra.ForeColor = Color.White;
            btnPhaiThu.BackColor = Color.FromArgb(240, 240, 240); btnPhaiThu.ForeColor = Color.Black;
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            TinhTongCongNo();
            if (_loaiCongNo == "PhaiThu") LoadCongNoPhaiThu(); else LoadCongNoPhaiTra();
            MessageBox.Show("✅ Đã làm mới dữ liệu!", "Thông báo");
        }
    }
}