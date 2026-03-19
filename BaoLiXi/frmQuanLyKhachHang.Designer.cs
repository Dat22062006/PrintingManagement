namespace PrintingManagement
{
    partial class frmQuanLyKhachHang
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuanLyKhachHang));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            panelhoadongtg = new Panel();
            btnLamMoi = new Guna.UI2.WinForms.Guna2Button();
            txtTimKiem = new TextBox();
            pictureBox7 = new PictureBox();
            labeltrangchubanhanghoadon = new Label();
            label1 = new Label();
            labellhoadongtgt = new Label();
            ptrbtrangchu = new PictureBox();
            panel1 = new Panel();
            dgvKhachHang = new Guna.UI2.WinForms.Guna2DataGridView();
            panelhoadongtg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKhachHang).BeginInit();
            SuspendLayout();
            // 
            // panelhoadongtg
            // 
            panelhoadongtg.Controls.Add(btnLamMoi);
            panelhoadongtg.Controls.Add(txtTimKiem);
            panelhoadongtg.Controls.Add(pictureBox7);
            panelhoadongtg.Controls.Add(labeltrangchubanhanghoadon);
            panelhoadongtg.Controls.Add(label1);
            panelhoadongtg.Controls.Add(labellhoadongtgt);
            panelhoadongtg.Controls.Add(ptrbtrangchu);
            panelhoadongtg.Dock = DockStyle.Top;
            panelhoadongtg.Location = new Point(0, 0);
            panelhoadongtg.Name = "panelhoadongtg";
            panelhoadongtg.Size = new Size(1058, 168);
            panelhoadongtg.TabIndex = 6;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BorderRadius = 10;
            btnLamMoi.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnLamMoi.CustomizableEdges = customizableEdges1;
            btnLamMoi.DisabledState.BorderColor = Color.DarkGray;
            btnLamMoi.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLamMoi.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLamMoi.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLamMoi.FillColor = Color.FromArgb(255, 128, 255);
            btnLamMoi.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.ImageOffset = new Point(-1, 1);
            btnLamMoi.Location = new Point(867, 126);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLamMoi.Size = new Size(151, 31);
            btnLamMoi.TabIndex = 261;
            btnLamMoi.Text = "Làm Mới";
            btnLamMoi.TextOffset = new Point(-2, 0);
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // txtTimKiem
            // 
            txtTimKiem.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTimKiem.Location = new Point(3, 128);
            txtTimKiem.Name = "txtTimKiem";
            txtTimKiem.PlaceholderText = "Tìm theo mã,tên khách hàng,....";
            txtTimKiem.Size = new Size(366, 29);
            txtTimKiem.TabIndex = 260;
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            // 
            // pictureBox7
            // 
            pictureBox7.ErrorImage = (Image)resources.GetObject("pictureBox7.ErrorImage");
            pictureBox7.Image = (Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.InitialImage = (Image)resources.GetObject("pictureBox7.InitialImage");
            pictureBox7.Location = new Point(3, 87);
            pictureBox7.Margin = new Padding(3, 2, 3, 2);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(34, 26);
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.TabIndex = 258;
            pictureBox7.TabStop = false;
            // 
            // labeltrangchubanhanghoadon
            // 
            labeltrangchubanhanghoadon.AutoSize = true;
            labeltrangchubanhanghoadon.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labeltrangchubanhanghoadon.Location = new Point(6, 55);
            labeltrangchubanhanghoadon.Name = "labeltrangchubanhanghoadon";
            labeltrangchubanhanghoadon.Size = new Size(359, 20);
            labeltrangchubanhanghoadon.TabIndex = 2;
            labeltrangchubanhanghoadon.Text = "Trang chủ / Tính giá và Báo giá / Quản lý Khách hàng";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(39, 90);
            label1.Name = "label1";
            label1.Size = new Size(182, 21);
            label1.TabIndex = 257;
            label1.Text = "Danh sách khách hàng";
            // 
            // labellhoadongtgt
            // 
            labellhoadongtgt.AutoSize = true;
            labellhoadongtgt.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labellhoadongtgt.Location = new Point(56, 11);
            labellhoadongtgt.Name = "labellhoadongtgt";
            labellhoadongtgt.Size = new Size(287, 32);
            labellhoadongtgt.TabIndex = 1;
            labellhoadongtgt.Text = "QUẢN LÝ KHÁCH HÀNG";
            // 
            // ptrbtrangchu
            // 
            ptrbtrangchu.ErrorImage = null;
            ptrbtrangchu.Image = (Image)resources.GetObject("ptrbtrangchu.Image");
            ptrbtrangchu.InitialImage = null;
            ptrbtrangchu.Location = new Point(12, 5);
            ptrbtrangchu.Name = "ptrbtrangchu";
            ptrbtrangchu.Size = new Size(43, 47);
            ptrbtrangchu.SizeMode = PictureBoxSizeMode.Zoom;
            ptrbtrangchu.TabIndex = 0;
            ptrbtrangchu.TabStop = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(dgvKhachHang);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 168);
            panel1.Name = "panel1";
            panel1.Size = new Size(1058, 393);
            panel1.TabIndex = 7;
            // 
            // dgvKhachHang
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvKhachHang.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvKhachHang.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvKhachHang.ColumnHeadersHeight = 4;
            dgvKhachHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvKhachHang.DefaultCellStyle = dataGridViewCellStyle3;
            dgvKhachHang.Dock = DockStyle.Fill;
            dgvKhachHang.GridColor = Color.FromArgb(231, 229, 255);
            dgvKhachHang.Location = new Point(0, 0);
            dgvKhachHang.Name = "dgvKhachHang";
            dgvKhachHang.RowHeadersVisible = false;
            dgvKhachHang.Size = new Size(1058, 393);
            dgvKhachHang.TabIndex = 0;
            dgvKhachHang.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvKhachHang.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvKhachHang.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvKhachHang.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvKhachHang.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvKhachHang.ThemeStyle.BackColor = Color.White;
            dgvKhachHang.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvKhachHang.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvKhachHang.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvKhachHang.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvKhachHang.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvKhachHang.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvKhachHang.ThemeStyle.HeaderStyle.Height = 4;
            dgvKhachHang.ThemeStyle.ReadOnly = false;
            dgvKhachHang.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvKhachHang.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvKhachHang.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvKhachHang.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvKhachHang.ThemeStyle.RowsStyle.Height = 25;
            dgvKhachHang.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvKhachHang.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // frmQuanLyKhachHang
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1058, 561);
            Controls.Add(panel1);
            Controls.Add(panelhoadongtg);
            MaximizeBox = false;
            Name = "frmQuanLyKhachHang";
            Text = "frmQuanLyKhachHang";
            Load += frmQuanLyKhachHang_Load;
            panelhoadongtg.ResumeLayout(false);
            panelhoadongtg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvKhachHang).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelhoadongtg;
        private Label labeltrangchubanhanghoadon;
        private Label labellhoadongtgt;
        private PictureBox ptrbtrangchu;
        private Panel panel1;
        private PictureBox pictureBox7;
        private Label label1;
        private TextBox txtTimKiem;
        private Guna.UI2.WinForms.Guna2DataGridView dgvKhachHang;
        private Guna.UI2.WinForms.Guna2Button btnLamMoi;
    }
}