namespace PrintingManagement
{
    partial class frmProductionOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProductionOrder));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelbangdieukhien = new Panel();
            lbltrangchudashboard = new Label();
            lblbangdieukhien = new Label();
            ptrbtrangchu = new PictureBox();
            panel1 = new Panel();
            dgvVatTu = new Guna.UI2.WinForms.Guna2DataGridView();
            btnKiemTraKho = new Guna.UI2.WinForms.Guna2Button();
            label2 = new Label();
            cboBaoGia = new ComboBox();
            label49 = new Label();
            pictureBox20 = new PictureBox();
            btnLayDuLieu = new Guna.UI2.WinForms.Guna2Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel4 = new Panel();
            btnBatDauSX = new Guna.UI2.WinForms.Guna2Button();
            btnInLenh = new Guna.UI2.WinForms.Guna2Button();
            label3 = new Label();
            txtSoLuong = new TextBox();
            label106 = new Label();
            txtKhachHang = new TextBox();
            lbKichThuocSanPham = new Label();
            txtTenSanPham = new TextBox();
            label107 = new Label();
            label108 = new Label();
            pictureBox21 = new PictureBox();
            dtpNgayBatDau = new DateTimePicker();
            panelbangdieukhien.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVatTu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox20).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox21).BeginInit();
            SuspendLayout();
            // 
            // panelbangdieukhien
            // 
            panelbangdieukhien.Controls.Add(lbltrangchudashboard);
            panelbangdieukhien.Controls.Add(lblbangdieukhien);
            panelbangdieukhien.Controls.Add(ptrbtrangchu);
            panelbangdieukhien.Dock = DockStyle.Top;
            panelbangdieukhien.Location = new Point(0, 0);
            panelbangdieukhien.Name = "panelbangdieukhien";
            panelbangdieukhien.Size = new Size(1036, 78);
            panelbangdieukhien.TabIndex = 1;
            // 
            // lbltrangchudashboard
            // 
            lbltrangchudashboard.AutoSize = true;
            lbltrangchudashboard.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltrangchudashboard.Location = new Point(6, 55);
            lbltrangchudashboard.Name = "lbltrangchudashboard";
            lbltrangchudashboard.Size = new Size(246, 20);
            lbltrangchudashboard.TabIndex = 2;
            lbltrangchudashboard.Text = "Trang chủ / Sản xuất / Lệnh sản xuất";
            // 
            // lblbangdieukhien
            // 
            lblbangdieukhien.AutoSize = true;
            lblbangdieukhien.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblbangdieukhien.Location = new Point(56, 11);
            lblbangdieukhien.Name = "lblbangdieukhien";
            lblbangdieukhien.Size = new Size(203, 32);
            lblbangdieukhien.TabIndex = 1;
            lblbangdieukhien.Text = "LỆNH SẢN XUẤT";
            // 
            // ptrbtrangchu
            // 
            ptrbtrangchu.Image = (Image)resources.GetObject("ptrbtrangchu.Image");
            ptrbtrangchu.Location = new Point(12, 5);
            ptrbtrangchu.Name = "ptrbtrangchu";
            ptrbtrangchu.Size = new Size(43, 47);
            ptrbtrangchu.SizeMode = PictureBoxSizeMode.Zoom;
            ptrbtrangchu.TabIndex = 0;
            ptrbtrangchu.TabStop = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(dgvVatTu);
            panel1.Controls.Add(btnKiemTraKho);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(cboBaoGia);
            panel1.Controls.Add(label49);
            panel1.Controls.Add(pictureBox20);
            panel1.Controls.Add(btnLayDuLieu);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 78);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1036, 398);
            panel1.TabIndex = 2;
            // 
            // dgvVatTu
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvVatTu.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvVatTu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvVatTu.ColumnHeadersHeight = 4;
            dgvVatTu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvVatTu.DefaultCellStyle = dataGridViewCellStyle3;
            dgvVatTu.GridColor = Color.FromArgb(231, 229, 255);
            dgvVatTu.Location = new Point(0, 195);
            dgvVatTu.Name = "dgvVatTu";
            dgvVatTu.RowHeadersVisible = false;
            dgvVatTu.Size = new Size(1036, 203);
            dgvVatTu.TabIndex = 228;
            dgvVatTu.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvVatTu.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvVatTu.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvVatTu.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvVatTu.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvVatTu.ThemeStyle.BackColor = Color.White;
            dgvVatTu.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvVatTu.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvVatTu.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvVatTu.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvVatTu.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvVatTu.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvVatTu.ThemeStyle.HeaderStyle.Height = 4;
            dgvVatTu.ThemeStyle.ReadOnly = false;
            dgvVatTu.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvVatTu.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVatTu.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvVatTu.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvVatTu.ThemeStyle.RowsStyle.Height = 25;
            dgvVatTu.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvVatTu.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // btnKiemTraKho
            // 
            btnKiemTraKho.BorderRadius = 10;
            btnKiemTraKho.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnKiemTraKho.CustomizableEdges = customizableEdges1;
            btnKiemTraKho.DisabledState.BorderColor = Color.DarkGray;
            btnKiemTraKho.DisabledState.CustomBorderColor = Color.DarkGray;
            btnKiemTraKho.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnKiemTraKho.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnKiemTraKho.FillColor = Color.Blue;
            btnKiemTraKho.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnKiemTraKho.ForeColor = Color.White;
            btnKiemTraKho.Image = Properties.Resources._2795_color_removebg_preview__1_1;
            btnKiemTraKho.ImageOffset = new Point(-1, 1);
            btnKiemTraKho.Location = new Point(665, 158);
            btnKiemTraKho.Name = "btnKiemTraKho";
            btnKiemTraKho.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnKiemTraKho.Size = new Size(361, 31);
            btnKiemTraKho.TabIndex = 227;
            btnKiemTraKho.Text = "Kiểm tra kho";
            btnKiemTraKho.TextOffset = new Point(-2, 0);
            btnKiemTraKho.Click += btnKiemTraKho_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 107);
            label2.Name = "label2";
            label2.Size = new Size(70, 15);
            label2.TabIndex = 226;
            label2.Text = "Số báo giá *";
            // 
            // cboBaoGia
            // 
            cboBaoGia.FormattingEnabled = true;
            cboBaoGia.Location = new Point(12, 129);
            cboBaoGia.Name = "cboBaoGia";
            cboBaoGia.Size = new Size(647, 23);
            cboBaoGia.TabIndex = 225;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label49.ForeColor = Color.Blue;
            label49.Location = new Point(32, 78);
            label49.Name = "label49";
            label49.Size = new Size(104, 17);
            label49.TabIndex = 224;
            label49.Text = "Liên kết báo giá";
            // 
            // pictureBox20
            // 
            pictureBox20.Image = Properties.Resources.images_removebg_preview;
            pictureBox20.Location = new Point(12, 76);
            pictureBox20.Name = "pictureBox20";
            pictureBox20.Size = new Size(21, 20);
            pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox20.TabIndex = 223;
            pictureBox20.TabStop = false;
            // 
            // btnLayDuLieu
            // 
            btnLayDuLieu.BorderRadius = 10;
            btnLayDuLieu.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnLayDuLieu.CustomizableEdges = customizableEdges3;
            btnLayDuLieu.DisabledState.BorderColor = Color.DarkGray;
            btnLayDuLieu.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLayDuLieu.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLayDuLieu.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLayDuLieu.FillColor = Color.FromArgb(255, 128, 255);
            btnLayDuLieu.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLayDuLieu.ForeColor = Color.White;
            btnLayDuLieu.Image = Properties.Resources._2795_color_removebg_preview__1_1;
            btnLayDuLieu.ImageOffset = new Point(-1, 1);
            btnLayDuLieu.Location = new Point(665, 121);
            btnLayDuLieu.Name = "btnLayDuLieu";
            btnLayDuLieu.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnLayDuLieu.Size = new Size(361, 31);
            btnLayDuLieu.TabIndex = 220;
            btnLayDuLieu.Text = "Lấy dữ liệu";
            btnLayDuLieu.TextOffset = new Point(-2, 0);
            btnLayDuLieu.Click += btnLayDuLieu_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(56, 31);
            label1.Name = "label1";
            label1.Size = new Size(190, 21);
            label1.TabIndex = 3;
            label1.Text = "Thông tin lệnh sản xuất";
            // 
            // pictureBox1
            // 
            pictureBox1.ErrorImage = (Image)resources.GetObject("pictureBox1.ErrorImage");
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(10, 26);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(34, 26);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel4
            // 
            panel4.Controls.Add(btnBatDauSX);
            panel4.Controls.Add(btnInLenh);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(txtSoLuong);
            panel4.Controls.Add(label106);
            panel4.Controls.Add(txtKhachHang);
            panel4.Controls.Add(lbKichThuocSanPham);
            panel4.Controls.Add(txtTenSanPham);
            panel4.Controls.Add(label107);
            panel4.Controls.Add(label108);
            panel4.Controls.Add(pictureBox21);
            panel4.Controls.Add(dtpNgayBatDau);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(0, 476);
            panel4.Margin = new Padding(3, 2, 3, 2);
            panel4.Name = "panel4";
            panel4.Size = new Size(1036, 288);
            panel4.TabIndex = 3;
            // 
            // btnBatDauSX
            // 
            btnBatDauSX.BorderRadius = 10;
            btnBatDauSX.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnBatDauSX.CustomizableEdges = customizableEdges5;
            btnBatDauSX.DisabledState.BorderColor = Color.DarkGray;
            btnBatDauSX.DisabledState.CustomBorderColor = Color.DarkGray;
            btnBatDauSX.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnBatDauSX.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnBatDauSX.FillColor = Color.Blue;
            btnBatDauSX.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBatDauSX.ForeColor = Color.White;
            btnBatDauSX.ImageOffset = new Point(-1, 1);
            btnBatDauSX.Location = new Point(169, 116);
            btnBatDauSX.Name = "btnBatDauSX";
            btnBatDauSX.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnBatDauSX.Size = new Size(158, 31);
            btnBatDauSX.TabIndex = 229;
            btnBatDauSX.Text = "Sản Xuất";
            btnBatDauSX.TextOffset = new Point(-2, 0);
            btnBatDauSX.Click += btnBatDauSX_Click;
            // 
            // btnInLenh
            // 
            btnInLenh.BorderRadius = 10;
            btnInLenh.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnInLenh.CustomizableEdges = customizableEdges7;
            btnInLenh.DisabledState.BorderColor = Color.DarkGray;
            btnInLenh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnInLenh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnInLenh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnInLenh.FillColor = Color.FromArgb(255, 128, 255);
            btnInLenh.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInLenh.ForeColor = Color.White;
            btnInLenh.ImageOffset = new Point(-1, 1);
            btnInLenh.Location = new Point(12, 116);
            btnInLenh.Name = "btnInLenh";
            btnInLenh.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnInLenh.Size = new Size(151, 31);
            btnInLenh.TabIndex = 228;
            btnInLenh.Text = "In";
            btnInLenh.TextOffset = new Point(-2, 0);
            btnInLenh.Click += btnInLenh_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label3.Location = new Point(819, 44);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 117;
            label3.Text = "Ngày bắt đầu";
            // 
            // txtSoLuong
            // 
            txtSoLuong.Location = new Point(321, 61);
            txtSoLuong.Margin = new Padding(3, 2, 3, 2);
            txtSoLuong.Name = "txtSoLuong";
            txtSoLuong.Size = new Size(151, 23);
            txtSoLuong.TabIndex = 116;
            // 
            // label106
            // 
            label106.AutoSize = true;
            label106.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label106.Location = new Point(321, 44);
            label106.Name = "label106";
            label106.Size = new Size(54, 15);
            label106.TabIndex = 115;
            label106.Text = "Số lượng";
            // 
            // txtKhachHang
            // 
            txtKhachHang.Location = new Point(478, 61);
            txtKhachHang.Margin = new Padding(3, 2, 3, 2);
            txtKhachHang.Name = "txtKhachHang";
            txtKhachHang.Size = new Size(335, 23);
            txtKhachHang.TabIndex = 114;
            // 
            // lbKichThuocSanPham
            // 
            lbKichThuocSanPham.AutoSize = true;
            lbKichThuocSanPham.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lbKichThuocSanPham.Location = new Point(478, 44);
            lbKichThuocSanPham.Name = "lbKichThuocSanPham";
            lbKichThuocSanPham.Size = new Size(72, 15);
            lbKichThuocSanPham.TabIndex = 113;
            lbKichThuocSanPham.Text = "Khách Hàng";
            // 
            // txtTenSanPham
            // 
            txtTenSanPham.Location = new Point(12, 61);
            txtTenSanPham.Margin = new Padding(3, 2, 3, 2);
            txtTenSanPham.Name = "txtTenSanPham";
            txtTenSanPham.Size = new Size(297, 23);
            txtTenSanPham.TabIndex = 112;
            // 
            // label107
            // 
            label107.AutoSize = true;
            label107.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label107.Location = new Point(17, 44);
            label107.Name = "label107";
            label107.Size = new Size(81, 15);
            label107.TabIndex = 111;
            label107.Text = "Tên sản phẩm";
            // 
            // label108
            // 
            label108.AutoSize = true;
            label108.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label108.ForeColor = Color.Blue;
            label108.Location = new Point(32, 18);
            label108.Name = "label108";
            label108.Size = new Size(131, 17);
            label108.TabIndex = 110;
            label108.Text = "Thông tin sản phẩm";
            // 
            // pictureBox21
            // 
            pictureBox21.Image = Properties.Resources.hoppdo1;
            pictureBox21.Location = new Point(12, 18);
            pictureBox21.Name = "pictureBox21";
            pictureBox21.Size = new Size(21, 20);
            pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox21.TabIndex = 109;
            pictureBox21.TabStop = false;
            // 
            // dtpNgayBatDau
            // 
            dtpNgayBatDau.CustomFormat = "dd/MM/yyyy";
            dtpNgayBatDau.Format = DateTimePickerFormat.Custom;
            dtpNgayBatDau.Location = new Point(819, 61);
            dtpNgayBatDau.Margin = new Padding(3, 2, 3, 2);
            dtpNgayBatDau.Name = "dtpNgayBatDau";
            dtpNgayBatDau.Size = new Size(169, 23);
            dtpNgayBatDau.TabIndex = 108;
            // 
            // frmProductionOrder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1036, 764);
            Controls.Add(panel4);
            Controls.Add(panel1);
            Controls.Add(panelbangdieukhien);
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "frmProductionOrder";
            Text = "frmProductionOrder";
            Load += frmProductionOrder_Load;
            panelbangdieukhien.ResumeLayout(false);
            panelbangdieukhien.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVatTu).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox20).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox21).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelbangdieukhien;
        private Label lbltrangchudashboard;
        private Label lblbangdieukhien;
        private PictureBox ptrbtrangchu;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private Panel panel4;
        private Guna.UI2.WinForms.Guna2Button btnLayDuLieu;
        private ComboBox cboBaoGia;
        private Label label49;
        private PictureBox pictureBox20;
        private Guna.UI2.WinForms.Guna2Button btnKiemTraKho;
        private Label label2;
        private Guna.UI2.WinForms.Guna2DataGridView dgvVatTu;
        private Label label3;
        private TextBox txtSoLuong;
        private Label label106;
        private TextBox txtKhachHang;
        private Label lbKichThuocSanPham;
        private TextBox txtTenSanPham;
        private Label label107;
        private Label label108;
        private PictureBox pictureBox21;
        private DateTimePicker dtpNgayBatDau;
        private Guna.UI2.WinForms.Guna2Button btnBatDauSX;
        private Guna.UI2.WinForms.Guna2Button btnInLenh;
    }
}