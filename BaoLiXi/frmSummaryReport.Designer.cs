namespace PrintingManagement
{
    partial class frmSummaryReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSummaryReport));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            paneltonghopkinhdoanh = new Panel();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panelalltonghopkinhdoanh = new Panel();
            dgvTongHop = new Guna.UI2.WinForms.Guna2DataGridView();
            panel4thongke = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            guna2ShadowPanel4 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblBienLNMota = new Label();
            lblBienLoiNhuan = new Label();
            ptrvattucannhap = new PictureBox();
            guna2ShadowPanel3 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblLoiNhuanMota = new Label();
            lblLoiNhuan = new Label();
            ptrdangsanxuat = new PictureBox();
            guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblChiPhiMota = new Label();
            lblChiPhi = new Label();
            ptrdondaduyet = new PictureBox();
            guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblDoanhThuMota = new Label();
            lblDoanhThu = new Label();
            ptrthongkethangnay = new PictureBox();
            labeltrangchubaocaotonghop = new Label();
            labelbaocaotonghop = new Label();
            ptrbtrangchu = new PictureBox();
            panelbaocaotonghop = new Panel();
            paneltonghopkinhdoanh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelalltonghopkinhdoanh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTongHop).BeginInit();
            panel4thongke.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            guna2ShadowPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrvattucannhap).BeginInit();
            guna2ShadowPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdangsanxuat).BeginInit();
            guna2ShadowPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdondaduyet).BeginInit();
            guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrthongkethangnay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panelbaocaotonghop.SuspendLayout();
            SuspendLayout();
            // 
            // paneltonghopkinhdoanh
            // 
            paneltonghopkinhdoanh.Controls.Add(label1);
            paneltonghopkinhdoanh.Controls.Add(pictureBox1);
            paneltonghopkinhdoanh.Dock = DockStyle.Top;
            paneltonghopkinhdoanh.Location = new Point(0, 0);
            paneltonghopkinhdoanh.Margin = new Padding(3, 2, 3, 2);
            paneltonghopkinhdoanh.Name = "paneltonghopkinhdoanh";
            paneltonghopkinhdoanh.Size = new Size(1058, 77);
            paneltonghopkinhdoanh.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(56, 7);
            label1.Name = "label1";
            label1.Size = new Size(257, 21);
            label1.TabIndex = 5;
            label1.Text = "Tổng hợp hoạt động kinh doanh";
            // 
            // pictureBox1
            // 
            pictureBox1.ErrorImage = (Image)resources.GetObject("pictureBox1.ErrorImage");
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(10, 2);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(34, 26);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // panelalltonghopkinhdoanh
            // 
            panelalltonghopkinhdoanh.Controls.Add(dgvTongHop);
            panelalltonghopkinhdoanh.Controls.Add(paneltonghopkinhdoanh);
            panelalltonghopkinhdoanh.Dock = DockStyle.Fill;
            panelalltonghopkinhdoanh.Location = new Point(0, 223);
            panelalltonghopkinhdoanh.Name = "panelalltonghopkinhdoanh";
            panelalltonghopkinhdoanh.Size = new Size(1058, 438);
            panelalltonghopkinhdoanh.TabIndex = 8;
            // 
            // dgvTongHop
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvTongHop.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvTongHop.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvTongHop.ColumnHeadersHeight = 4;
            dgvTongHop.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvTongHop.DefaultCellStyle = dataGridViewCellStyle3;
            dgvTongHop.Dock = DockStyle.Fill;
            dgvTongHop.GridColor = Color.FromArgb(231, 229, 255);
            dgvTongHop.Location = new Point(0, 77);
            dgvTongHop.Name = "dgvTongHop";
            dgvTongHop.RowHeadersVisible = false;
            dgvTongHop.Size = new Size(1058, 361);
            dgvTongHop.TabIndex = 6;
            dgvTongHop.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvTongHop.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvTongHop.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvTongHop.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvTongHop.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvTongHop.ThemeStyle.BackColor = Color.White;
            dgvTongHop.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvTongHop.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvTongHop.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTongHop.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvTongHop.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvTongHop.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvTongHop.ThemeStyle.HeaderStyle.Height = 4;
            dgvTongHop.ThemeStyle.ReadOnly = false;
            dgvTongHop.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvTongHop.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTongHop.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvTongHop.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvTongHop.ThemeStyle.RowsStyle.Height = 25;
            dgvTongHop.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvTongHop.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // panel4thongke
            // 
            panel4thongke.Controls.Add(tableLayoutPanel1);
            panel4thongke.Dock = DockStyle.Top;
            panel4thongke.Location = new Point(0, 109);
            panel4thongke.Name = "panel4thongke";
            panel4thongke.Size = new Size(1058, 114);
            panel4thongke.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(guna2ShadowPanel4, 3, 0);
            tableLayoutPanel1.Controls.Add(guna2ShadowPanel3, 2, 0);
            tableLayoutPanel1.Controls.Add(guna2ShadowPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(guna2ShadowPanel1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1058, 114);
            tableLayoutPanel1.TabIndex = 12;
            // 
            // guna2ShadowPanel4
            // 
            guna2ShadowPanel4.BackColor = Color.Transparent;
            guna2ShadowPanel4.Controls.Add(lblBienLNMota);
            guna2ShadowPanel4.Controls.Add(lblBienLoiNhuan);
            guna2ShadowPanel4.Controls.Add(ptrvattucannhap);
            guna2ShadowPanel4.Dock = DockStyle.Fill;
            guna2ShadowPanel4.FillColor = Color.White;
            guna2ShadowPanel4.Location = new Point(795, 3);
            guna2ShadowPanel4.Name = "guna2ShadowPanel4";
            guna2ShadowPanel4.ShadowColor = Color.Black;
            guna2ShadowPanel4.Size = new Size(260, 108);
            guna2ShadowPanel4.TabIndex = 12;
            // 
            // lblBienLNMota
            // 
            lblBienLNMota.AutoSize = true;
            lblBienLNMota.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBienLNMota.Location = new Point(69, 38);
            lblBienLNMota.Name = "lblBienLNMota";
            lblBienLNMota.Size = new Size(28, 32);
            lblBienLNMota.TabIndex = 8;
            lblBienLNMota.Text = "0";
            // 
            // lblBienLoiNhuan
            // 
            lblBienLoiNhuan.AutoSize = true;
            lblBienLoiNhuan.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBienLoiNhuan.Location = new Point(71, 11);
            lblBienLoiNhuan.Name = "lblBienLoiNhuan";
            lblBienLoiNhuan.Size = new Size(89, 17);
            lblBienLoiNhuan.TabIndex = 7;
            lblBienLoiNhuan.Text = "Biên lợi nhuận";
            // 
            // ptrvattucannhap
            // 
            ptrvattucannhap.ErrorImage = null;
            ptrvattucannhap.Image = (Image)resources.GetObject("ptrvattucannhap.Image");
            ptrvattucannhap.InitialImage = null;
            ptrvattucannhap.Location = new Point(16, 3);
            ptrvattucannhap.Name = "ptrvattucannhap";
            ptrvattucannhap.Size = new Size(49, 61);
            ptrvattucannhap.SizeMode = PictureBoxSizeMode.Zoom;
            ptrvattucannhap.TabIndex = 6;
            ptrvattucannhap.TabStop = false;
            // 
            // guna2ShadowPanel3
            // 
            guna2ShadowPanel3.BackColor = Color.Transparent;
            guna2ShadowPanel3.Controls.Add(lblLoiNhuanMota);
            guna2ShadowPanel3.Controls.Add(lblLoiNhuan);
            guna2ShadowPanel3.Controls.Add(ptrdangsanxuat);
            guna2ShadowPanel3.Dock = DockStyle.Fill;
            guna2ShadowPanel3.FillColor = Color.White;
            guna2ShadowPanel3.Location = new Point(531, 3);
            guna2ShadowPanel3.Name = "guna2ShadowPanel3";
            guna2ShadowPanel3.ShadowColor = Color.Black;
            guna2ShadowPanel3.Size = new Size(258, 108);
            guna2ShadowPanel3.TabIndex = 11;
            // 
            // lblLoiNhuanMota
            // 
            lblLoiNhuanMota.AutoSize = true;
            lblLoiNhuanMota.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLoiNhuanMota.ForeColor = Color.Black;
            lblLoiNhuanMota.Location = new Point(69, 38);
            lblLoiNhuanMota.Name = "lblLoiNhuanMota";
            lblLoiNhuanMota.Size = new Size(28, 32);
            lblLoiNhuanMota.TabIndex = 8;
            lblLoiNhuanMota.Text = "0";
            // 
            // lblLoiNhuan
            // 
            lblLoiNhuan.AutoSize = true;
            lblLoiNhuan.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLoiNhuan.Location = new Point(71, 11);
            lblLoiNhuan.Name = "lblLoiNhuan";
            lblLoiNhuan.Size = new Size(64, 17);
            lblLoiNhuan.TabIndex = 7;
            lblLoiNhuan.Text = "Lợi nhuận";
            // 
            // ptrdangsanxuat
            // 
            ptrdangsanxuat.ErrorImage = null;
            ptrdangsanxuat.Image = (Image)resources.GetObject("ptrdangsanxuat.Image");
            ptrdangsanxuat.InitialImage = null;
            ptrdangsanxuat.Location = new Point(16, 3);
            ptrdangsanxuat.Name = "ptrdangsanxuat";
            ptrdangsanxuat.Size = new Size(49, 61);
            ptrdangsanxuat.SizeMode = PictureBoxSizeMode.Zoom;
            ptrdangsanxuat.TabIndex = 6;
            ptrdangsanxuat.TabStop = false;
            // 
            // guna2ShadowPanel2
            // 
            guna2ShadowPanel2.BackColor = Color.Transparent;
            guna2ShadowPanel2.Controls.Add(lblChiPhiMota);
            guna2ShadowPanel2.Controls.Add(lblChiPhi);
            guna2ShadowPanel2.Controls.Add(ptrdondaduyet);
            guna2ShadowPanel2.Dock = DockStyle.Fill;
            guna2ShadowPanel2.FillColor = Color.White;
            guna2ShadowPanel2.Location = new Point(267, 3);
            guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            guna2ShadowPanel2.ShadowColor = Color.Black;
            guna2ShadowPanel2.Size = new Size(258, 108);
            guna2ShadowPanel2.TabIndex = 10;
            // 
            // lblChiPhiMota
            // 
            lblChiPhiMota.AutoSize = true;
            lblChiPhiMota.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblChiPhiMota.Location = new Point(67, 38);
            lblChiPhiMota.Name = "lblChiPhiMota";
            lblChiPhiMota.Size = new Size(28, 32);
            lblChiPhiMota.TabIndex = 8;
            lblChiPhiMota.Text = "0";
            // 
            // lblChiPhi
            // 
            lblChiPhi.AutoSize = true;
            lblChiPhi.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblChiPhi.Location = new Point(69, 11);
            lblChiPhi.Name = "lblChiPhi";
            lblChiPhi.Size = new Size(48, 17);
            lblChiPhi.TabIndex = 7;
            lblChiPhi.Text = "Chi phí";
            // 
            // ptrdondaduyet
            // 
            ptrdondaduyet.ErrorImage = null;
            ptrdondaduyet.Image = (Image)resources.GetObject("ptrdondaduyet.Image");
            ptrdondaduyet.InitialImage = null;
            ptrdondaduyet.Location = new Point(14, 3);
            ptrdondaduyet.Name = "ptrdondaduyet";
            ptrdondaduyet.Size = new Size(49, 61);
            ptrdondaduyet.SizeMode = PictureBoxSizeMode.Zoom;
            ptrdondaduyet.TabIndex = 6;
            ptrdondaduyet.TabStop = false;
            // 
            // guna2ShadowPanel1
            // 
            guna2ShadowPanel1.BackColor = Color.Transparent;
            guna2ShadowPanel1.Controls.Add(lblDoanhThuMota);
            guna2ShadowPanel1.Controls.Add(lblDoanhThu);
            guna2ShadowPanel1.Controls.Add(ptrthongkethangnay);
            guna2ShadowPanel1.Dock = DockStyle.Fill;
            guna2ShadowPanel1.FillColor = Color.White;
            guna2ShadowPanel1.Location = new Point(3, 3);
            guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            guna2ShadowPanel1.ShadowColor = Color.Black;
            guna2ShadowPanel1.Size = new Size(258, 108);
            guna2ShadowPanel1.TabIndex = 9;
            // 
            // lblDoanhThuMota
            // 
            lblDoanhThuMota.AutoSize = true;
            lblDoanhThuMota.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDoanhThuMota.Location = new Point(69, 38);
            lblDoanhThuMota.Name = "lblDoanhThuMota";
            lblDoanhThuMota.Size = new Size(28, 32);
            lblDoanhThuMota.TabIndex = 5;
            lblDoanhThuMota.Text = "0";
            // 
            // lblDoanhThu
            // 
            lblDoanhThu.AutoSize = true;
            lblDoanhThu.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDoanhThu.Location = new Point(71, 11);
            lblDoanhThu.Name = "lblDoanhThu";
            lblDoanhThu.Size = new Size(68, 17);
            lblDoanhThu.TabIndex = 4;
            lblDoanhThu.Text = "Doanh thu";
            // 
            // ptrthongkethangnay
            // 
            ptrthongkethangnay.ErrorImage = null;
            ptrthongkethangnay.Image = (Image)resources.GetObject("ptrthongkethangnay.Image");
            ptrthongkethangnay.InitialImage = null;
            ptrthongkethangnay.Location = new Point(16, 3);
            ptrthongkethangnay.Name = "ptrthongkethangnay";
            ptrthongkethangnay.Size = new Size(49, 61);
            ptrthongkethangnay.SizeMode = PictureBoxSizeMode.Zoom;
            ptrthongkethangnay.TabIndex = 3;
            ptrthongkethangnay.TabStop = false;
            // 
            // labeltrangchubaocaotonghop
            // 
            labeltrangchubaocaotonghop.AutoSize = true;
            labeltrangchubaocaotonghop.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labeltrangchubaocaotonghop.Location = new Point(6, 55);
            labeltrangchubaocaotonghop.Name = "labeltrangchubaocaotonghop";
            labeltrangchubaocaotonghop.Size = new Size(274, 20);
            labeltrangchubaocaotonghop.TabIndex = 2;
            labeltrangchubaocaotonghop.Text = "Trang chủ / Báo cáo / Báo cáo tổng hợp";
            // 
            // labelbaocaotonghop
            // 
            labelbaocaotonghop.AutoSize = true;
            labelbaocaotonghop.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelbaocaotonghop.Location = new Point(56, 11);
            labelbaocaotonghop.Name = "labelbaocaotonghop";
            labelbaocaotonghop.Size = new Size(255, 32);
            labelbaocaotonghop.TabIndex = 1;
            labelbaocaotonghop.Text = "BÁO CÁO TỔNG HỢP";
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
            // panelbaocaotonghop
            // 
            panelbaocaotonghop.Controls.Add(labeltrangchubaocaotonghop);
            panelbaocaotonghop.Controls.Add(labelbaocaotonghop);
            panelbaocaotonghop.Controls.Add(ptrbtrangchu);
            panelbaocaotonghop.Dock = DockStyle.Top;
            panelbaocaotonghop.Location = new Point(0, 0);
            panelbaocaotonghop.Name = "panelbaocaotonghop";
            panelbaocaotonghop.Size = new Size(1058, 109);
            panelbaocaotonghop.TabIndex = 6;
            // 
            // frmSummaryReport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1058, 661);
            Controls.Add(panelalltonghopkinhdoanh);
            Controls.Add(panel4thongke);
            Controls.Add(panelbaocaotonghop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmSummaryReport";
            Text = "frmSummaryReport";
            Load += frmSummaryReport_Load;
            paneltonghopkinhdoanh.ResumeLayout(false);
            paneltonghopkinhdoanh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelalltonghopkinhdoanh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTongHop).EndInit();
            panel4thongke.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            guna2ShadowPanel4.ResumeLayout(false);
            guna2ShadowPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrvattucannhap).EndInit();
            guna2ShadowPanel3.ResumeLayout(false);
            guna2ShadowPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdangsanxuat).EndInit();
            guna2ShadowPanel2.ResumeLayout(false);
            guna2ShadowPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdondaduyet).EndInit();
            guna2ShadowPanel1.ResumeLayout(false);
            guna2ShadowPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrthongkethangnay).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panelbaocaotonghop.ResumeLayout(false);
            panelbaocaotonghop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel paneltonghopkinhdoanh;
        private Panel panelalltonghopkinhdoanh;
        private Label lblphantramgsothangtruoc;
        private Label lblconsotiendedat;
        private Label lblbienloinhuan;
        private Label lblkhdagdtrongthang;
        private Panel panel4thongke;
        private Label labeltrangchubaocaotonghop;
        private Label labelbaocaotonghop;
        private PictureBox ptrbtrangchu;
        private Panel panelbaocaotonghop;
        private Label label1;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2DataGridView dgvTongHop;
        private TableLayoutPanel tableLayoutPanel1;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel4;
        private Label lblBienLNMota;
        private Label lblBienLoiNhuan;
        private PictureBox ptrvattucannhap;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel3;
        private Label lblLoiNhuanMota;
        private Label lblLoiNhuan;
        private PictureBox ptrdangsanxuat;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Label lblChiPhiMota;
        private Label lblChiPhi;
        private PictureBox ptrdondaduyet;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Label lblDoanhThuMota;
        private Label lblDoanhThu;
        private PictureBox ptrthongkethangnay;
    }
}