namespace PrintingManagement
{
    partial class frmPaymentReceive
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPaymentReceive));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelthutienkh = new Panel();
            labeltrangchuthutien = new Label();
            labelthutienkh = new Label();
            ptrbtrangchu = new PictureBox();
            paneldscongno = new Panel();
            dgvCongNo = new Guna.UI2.WinForms.Guna2DataGridView();
            label2 = new Label();
            pictureBox3 = new PictureBox();
            label1 = new Label();
            pictureBox7 = new PictureBox();
            btnThuTien = new Guna.UI2.WinForms.Guna2Button();
            btnInPhieuThu = new Guna.UI2.WinForms.Guna2Button();
            panelsotientongthu = new Panel();
            lblTongThuValue = new Label();
            paneltongthuttkh = new Panel();
            labeltongthuttkh = new Label();
            cboPhuongThuc = new ComboBox();
            dtpNgayThu = new DateTimePicker();
            lblphuongthucthu = new Label();
            lblkhachhang = new Label();
            labelkhachhangttkh = new Label();
            cboKhachHang = new ComboBox();
            panelthutienkh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            paneldscongno.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCongNo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            panelsotientongthu.SuspendLayout();
            paneltongthuttkh.SuspendLayout();
            SuspendLayout();
            // 
            // panelthutienkh
            // 
            panelthutienkh.Controls.Add(labeltrangchuthutien);
            panelthutienkh.Controls.Add(labelthutienkh);
            panelthutienkh.Controls.Add(ptrbtrangchu);
            panelthutienkh.Dock = DockStyle.Top;
            panelthutienkh.Location = new Point(0, 0);
            panelthutienkh.Name = "panelthutienkh";
            panelthutienkh.Size = new Size(1058, 78);
            panelthutienkh.TabIndex = 3;
            // 
            // labeltrangchuthutien
            // 
            labeltrangchuthutien.AutoSize = true;
            labeltrangchuthutien.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labeltrangchuthutien.Location = new Point(6, 55);
            labeltrangchuthutien.Name = "labeltrangchuthutien";
            labeltrangchuthutien.Size = new Size(216, 20);
            labeltrangchuthutien.TabIndex = 2;
            labeltrangchuthutien.Text = "Trang chủ / Bán hàng / Thu tiền";
            // 
            // labelthutienkh
            // 
            labelthutienkh.AutoSize = true;
            labelthutienkh.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelthutienkh.Location = new Point(56, 11);
            labelthutienkh.Name = "labelthutienkh";
            labelthutienkh.Size = new Size(293, 32);
            labelthutienkh.TabIndex = 1;
            labelthutienkh.Text = "THU TIỀN KHÁCH HÀNG";
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
            // paneldscongno
            // 
            paneldscongno.Controls.Add(dgvCongNo);
            paneldscongno.Controls.Add(label2);
            paneldscongno.Controls.Add(pictureBox3);
            paneldscongno.Controls.Add(label1);
            paneldscongno.Controls.Add(pictureBox7);
            paneldscongno.Controls.Add(btnThuTien);
            paneldscongno.Controls.Add(btnInPhieuThu);
            paneldscongno.Controls.Add(panelsotientongthu);
            paneldscongno.Controls.Add(paneltongthuttkh);
            paneldscongno.Controls.Add(cboPhuongThuc);
            paneldscongno.Controls.Add(dtpNgayThu);
            paneldscongno.Controls.Add(lblphuongthucthu);
            paneldscongno.Controls.Add(lblkhachhang);
            paneldscongno.Controls.Add(labelkhachhangttkh);
            paneldscongno.Controls.Add(cboKhachHang);
            paneldscongno.Dock = DockStyle.Fill;
            paneldscongno.Location = new Point(0, 78);
            paneldscongno.Name = "paneldscongno";
            paneldscongno.Size = new Size(1058, 983);
            paneldscongno.TabIndex = 5;
            // 
            // dgvCongNo
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvCongNo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvCongNo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvCongNo.ColumnHeadersHeight = 4;
            dgvCongNo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvCongNo.DefaultCellStyle = dataGridViewCellStyle3;
            dgvCongNo.GridColor = Color.FromArgb(231, 229, 255);
            dgvCongNo.Location = new Point(13, 173);
            dgvCongNo.Name = "dgvCongNo";
            dgvCongNo.RowHeadersVisible = false;
            dgvCongNo.Size = new Size(1033, 387);
            dgvCongNo.TabIndex = 268;
            dgvCongNo.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvCongNo.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvCongNo.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvCongNo.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvCongNo.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvCongNo.ThemeStyle.BackColor = Color.White;
            dgvCongNo.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvCongNo.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvCongNo.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCongNo.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvCongNo.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvCongNo.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvCongNo.ThemeStyle.HeaderStyle.Height = 4;
            dgvCongNo.ThemeStyle.ReadOnly = false;
            dgvCongNo.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvCongNo.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCongNo.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvCongNo.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvCongNo.ThemeStyle.RowsStyle.Height = 25;
            dgvCongNo.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvCongNo.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvCongNo.CurrentCellDirtyStateChanged += DgvCongNo_CurrentCellDirtyStateChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label2.Location = new Point(55, 117);
            label2.Name = "label2";
            label2.Size = new Size(126, 17);
            label2.TabIndex = 267;
            label2.Text = "Danh sách công nợ";
            // 
            // pictureBox3
            // 
            pictureBox3.ErrorImage = (Image)resources.GetObject("pictureBox3.ErrorImage");
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.InitialImage = (Image)resources.GetObject("pictureBox3.InitialImage");
            pictureBox3.Location = new Point(17, 113);
            pictureBox3.Margin = new Padding(3, 2, 3, 2);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(34, 26);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 266;
            pictureBox3.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(55, 14);
            label1.Name = "label1";
            label1.Size = new Size(148, 21);
            label1.TabIndex = 263;
            label1.Text = "Thông tin thu tiền";
            // 
            // pictureBox7
            // 
            pictureBox7.ErrorImage = (Image)resources.GetObject("pictureBox7.ErrorImage");
            pictureBox7.Image = (Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.InitialImage = (Image)resources.GetObject("pictureBox7.InitialImage");
            pictureBox7.Location = new Point(17, 12);
            pictureBox7.Margin = new Padding(3, 2, 3, 2);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(34, 26);
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.TabIndex = 262;
            pictureBox7.TabStop = false;
            // 
            // btnThuTien
            // 
            btnThuTien.BorderRadius = 10;
            btnThuTien.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnThuTien.CustomizableEdges = customizableEdges1;
            btnThuTien.DisabledState.BorderColor = Color.DarkGray;
            btnThuTien.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThuTien.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThuTien.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThuTien.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnThuTien.ForeColor = Color.White;
            btnThuTien.Image = (Image)resources.GetObject("btnThuTien.Image");
            btnThuTien.ImageOffset = new Point(-1, 1);
            btnThuTien.Location = new Point(17, 609);
            btnThuTien.Name = "btnThuTien";
            btnThuTien.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnThuTien.Size = new Size(134, 44);
            btnThuTien.TabIndex = 258;
            btnThuTien.Text = "Thu tiền";
            btnThuTien.TextOffset = new Point(-2, 0);
            btnThuTien.Click += BtnThuTien_Click;
            // 
            // btnInPhieuThu
            // 
            btnInPhieuThu.BorderRadius = 10;
            btnInPhieuThu.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnInPhieuThu.CustomizableEdges = customizableEdges3;
            btnInPhieuThu.DisabledState.BorderColor = Color.DarkGray;
            btnInPhieuThu.DisabledState.CustomBorderColor = Color.DarkGray;
            btnInPhieuThu.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnInPhieuThu.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnInPhieuThu.FillColor = Color.Olive;
            btnInPhieuThu.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInPhieuThu.ForeColor = Color.White;
            btnInPhieuThu.Image = (Image)resources.GetObject("btnInPhieuThu.Image");
            btnInPhieuThu.ImageOffset = new Point(-1, 1);
            btnInPhieuThu.Location = new Point(172, 609);
            btnInPhieuThu.Name = "btnInPhieuThu";
            btnInPhieuThu.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnInPhieuThu.Size = new Size(164, 44);
            btnInPhieuThu.TabIndex = 259;
            btnInPhieuThu.Text = "In phiếu thu";
            btnInPhieuThu.TextOffset = new Point(-2, 0);
            btnInPhieuThu.Click += BtnInPhieuThu_Click;
            // 
            // panelsotientongthu
            // 
            panelsotientongthu.Controls.Add(lblTongThuValue);
            panelsotientongthu.Location = new Point(888, 588);
            panelsotientongthu.Margin = new Padding(3, 2, 3, 2);
            panelsotientongthu.Name = "panelsotientongthu";
            panelsotientongthu.Size = new Size(158, 32);
            panelsotientongthu.TabIndex = 19;
            // 
            // lblTongThuValue
            // 
            lblTongThuValue.AutoSize = true;
            lblTongThuValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblTongThuValue.Location = new Point(43, 8);
            lblTongThuValue.Name = "lblTongThuValue";
            lblTongThuValue.Size = new Size(87, 15);
            lblTongThuValue.TabIndex = 2;
            lblTongThuValue.Text = "135,564,000 đ";
            // 
            // paneltongthuttkh
            // 
            paneltongthuttkh.Controls.Add(labeltongthuttkh);
            paneltongthuttkh.Location = new Point(718, 588);
            paneltongthuttkh.Margin = new Padding(3, 2, 3, 2);
            paneltongthuttkh.Name = "paneltongthuttkh";
            paneltongthuttkh.Size = new Size(164, 32);
            paneltongthuttkh.TabIndex = 18;
            // 
            // labeltongthuttkh
            // 
            labeltongthuttkh.AutoSize = true;
            labeltongthuttkh.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 163);
            labeltongthuttkh.Location = new Point(83, 8);
            labeltongthuttkh.Name = "labeltongthuttkh";
            labeltongthuttkh.Size = new Size(72, 15);
            labeltongthuttkh.TabIndex = 2;
            labeltongthuttkh.Text = "TỔNG THU:";
            // 
            // cboPhuongThuc
            // 
            cboPhuongThuc.FormattingEnabled = true;
            cboPhuongThuc.Location = new Point(741, 75);
            cboPhuongThuc.Name = "cboPhuongThuc";
            cboPhuongThuc.Size = new Size(305, 23);
            cboPhuongThuc.TabIndex = 16;
            // 
            // dtpNgayThu
            // 
            dtpNgayThu.CustomFormat = "dd/MM/yyyy";
            dtpNgayThu.Format = DateTimePickerFormat.Custom;
            dtpNgayThu.Location = new Point(378, 75);
            dtpNgayThu.Name = "dtpNgayThu";
            dtpNgayThu.Size = new Size(323, 23);
            dtpNgayThu.TabIndex = 15;
            // 
            // lblphuongthucthu
            // 
            lblphuongthucthu.AutoSize = true;
            lblphuongthucthu.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblphuongthucthu.Location = new Point(741, 50);
            lblphuongthucthu.Name = "lblphuongthucthu";
            lblphuongthucthu.Size = new Size(104, 17);
            lblphuongthucthu.TabIndex = 11;
            lblphuongthucthu.Text = "Phương thức thu";
            // 
            // lblkhachhang
            // 
            lblkhachhang.AutoSize = true;
            lblkhachhang.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblkhachhang.Location = new Point(378, 50);
            lblkhachhang.Name = "lblkhachhang";
            lblkhachhang.Size = new Size(86, 17);
            lblkhachhang.TabIndex = 10;
            lblkhachhang.Text = "Ngày thu tiền";
            // 
            // labelkhachhangttkh
            // 
            labelkhachhangttkh.AutoSize = true;
            labelkhachhangttkh.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelkhachhangttkh.Location = new Point(13, 50);
            labelkhachhangttkh.Name = "labelkhachhangttkh";
            labelkhachhangttkh.Size = new Size(76, 17);
            labelkhachhangttkh.TabIndex = 7;
            labelkhachhangttkh.Text = "Khách hàng";
            // 
            // cboKhachHang
            // 
            cboKhachHang.FormattingEnabled = true;
            cboKhachHang.Location = new Point(13, 75);
            cboKhachHang.Name = "cboKhachHang";
            cboKhachHang.Size = new Size(323, 23);
            cboKhachHang.TabIndex = 6;
            cboKhachHang.SelectedIndexChanged += CboKhachHang_SelectedIndexChanged;
            // 
            // frmPaymentReceive
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1058, 1061);
            Controls.Add(paneldscongno);
            Controls.Add(panelthutienkh);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmPaymentReceive";
            Text = "frmPaymentReceive";
            Load += frmPaymentReceive_Load;
            panelthutienkh.ResumeLayout(false);
            panelthutienkh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            paneldscongno.ResumeLayout(false);
            paneldscongno.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCongNo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            panelsotientongthu.ResumeLayout(false);
            panelsotientongthu.PerformLayout();
            paneltongthuttkh.ResumeLayout(false);
            paneltongthuttkh.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelthutienkh;
        private Label labeltrangchuthutien;
        private Label labelthutienkh;
        private PictureBox ptrbtrangchu;
        private Panel paneldscongno;
        private DateTimePicker dtpNgayThu;
        private Label lblphuongthucthu;
        private Label lblkhachhang;
        private Label labelkhachhangttkh;
        private ComboBox cboKhachHang;
        private ComboBox cboPhuongThuc;
        private Panel panelsotientongthu;
        private Label lblTongThuValue;
        private Panel paneltongthuttkh;
        private Label labeltongthuttkh;
        private Guna.UI2.WinForms.Guna2Button btnThuTien;
        private Guna.UI2.WinForms.Guna2Button btnInPhieuThu;
        private Label label2;
        private PictureBox pictureBox3;
        private Label label1;
        private PictureBox pictureBox7;
        private Guna.UI2.WinForms.Guna2DataGridView dgvCongNo;
    }
}