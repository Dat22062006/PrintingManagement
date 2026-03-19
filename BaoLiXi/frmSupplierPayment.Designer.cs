namespace PrintingManagement
{
    partial class frmSupplierPayment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSupplierPayment));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblnhapkhonhapkho = new Label();
            lblnhapkho = new Label();
            pictureBox1 = new PictureBox();
            pnltop = new Panel();
            panel1 = new Panel();
            cboNhaCungCap = new ComboBox();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            cboPhuongThuc = new ComboBox();
            dtpNgayTraTien = new DateTimePicker();
            label56 = new Label();
            pictureBox16 = new PictureBox();
            label57 = new Label();
            label58 = new Label();
            label59 = new Label();
            panel2 = new Panel();
            dgvChiTietThanhToan = new Guna.UI2.WinForms.Guna2DataGridView();
            lblTongThanhToan = new Label();
            label76 = new Label();
            panel19 = new Panel();
            btnInPhieuChi = new Guna.UI2.WinForms.Guna2Button();
            btnThanhToan = new Guna.UI2.WinForms.Guna2Button();
            btnLamMoi = new Guna.UI2.WinForms.Guna2Button();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnltop.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTietThanhToan).BeginInit();
            panel19.SuspendLayout();
            SuspendLayout();
            // 
            // lblnhapkhonhapkho
            // 
            lblnhapkhonhapkho.AutoSize = true;
            lblnhapkhonhapkho.Location = new Point(19, 64);
            lblnhapkhonhapkho.Name = "lblnhapkhonhapkho";
            lblnhapkhonhapkho.Size = new Size(225, 15);
            lblnhapkhonhapkho.TabIndex = 2;
            lblnhapkhonhapkho.Text = "Trang chủ / Mua hàng / Thanh toán NCC";
            // 
            // lblnhapkho
            // 
            lblnhapkho.AutoSize = true;
            lblnhapkho.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblnhapkho.Location = new Point(70, 17);
            lblnhapkho.Name = "lblnhapkho";
            lblnhapkho.Size = new Size(364, 32);
            lblnhapkho.TabIndex = 1;
            lblnhapkho.Text = "THANH TOÁN NHÀ CUNG CẤP";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(21, 9);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(52, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pnltop
            // 
            pnltop.Controls.Add(lblnhapkhonhapkho);
            pnltop.Controls.Add(lblnhapkho);
            pnltop.Controls.Add(pictureBox1);
            pnltop.Dock = DockStyle.Top;
            pnltop.Location = new Point(0, 0);
            pnltop.Margin = new Padding(3, 2, 3, 2);
            pnltop.Name = "pnltop";
            pnltop.Size = new Size(1075, 94);
            pnltop.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.Controls.Add(cboNhaCungCap);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(cboPhuongThuc);
            panel1.Controls.Add(dtpNgayTraTien);
            panel1.Controls.Add(label56);
            panel1.Controls.Add(pictureBox16);
            panel1.Controls.Add(label57);
            panel1.Controls.Add(label58);
            panel1.Controls.Add(label59);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 94);
            panel1.Name = "panel1";
            panel1.Size = new Size(1075, 121);
            panel1.TabIndex = 4;
            // 
            // cboNhaCungCap
            // 
            cboNhaCungCap.FormattingEnabled = true;
            cboNhaCungCap.Location = new Point(19, 56);
            cboNhaCungCap.Name = "cboNhaCungCap";
            cboNhaCungCap.Size = new Size(289, 23);
            cboNhaCungCap.TabIndex = 151;
            cboNhaCungCap.SelectedIndexChanged += cboNhaCungCap_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(42, 91);
            label1.Name = "label1";
            label1.Size = new Size(125, 17);
            label1.TabIndex = 150;
            label1.Text = "Danh sách công nợ";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(22, 89);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(21, 20);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 149;
            pictureBox2.TabStop = false;
            // 
            // cboPhuongThuc
            // 
            cboPhuongThuc.FormattingEnabled = true;
            cboPhuongThuc.Items.AddRange(new object[] { "Tiền mặt", "Uỷ nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPhuongThuc.Location = new Point(512, 55);
            cboPhuongThuc.Name = "cboPhuongThuc";
            cboPhuongThuc.Size = new Size(551, 23);
            cboPhuongThuc.TabIndex = 144;
            // 
            // dtpNgayTraTien
            // 
            dtpNgayTraTien.CustomFormat = "dd/MM/yyyy";
            dtpNgayTraTien.Format = DateTimePickerFormat.Custom;
            dtpNgayTraTien.Location = new Point(341, 56);
            dtpNgayTraTien.Name = "dtpNgayTraTien";
            dtpNgayTraTien.Size = new Size(165, 23);
            dtpNgayTraTien.TabIndex = 141;
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label56.Location = new Point(42, 11);
            label56.Name = "label56";
            label56.Size = new Size(162, 21);
            label56.TabIndex = 140;
            label56.Text = "Thông tin thanh toán";
            // 
            // pictureBox16
            // 
            pictureBox16.Image = (Image)resources.GetObject("pictureBox16.Image");
            pictureBox16.Location = new Point(22, 11);
            pictureBox16.Name = "pictureBox16";
            pictureBox16.Size = new Size(21, 20);
            pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox16.TabIndex = 139;
            pictureBox16.TabStop = false;
            // 
            // label57
            // 
            label57.AutoSize = true;
            label57.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label57.Location = new Point(341, 37);
            label57.Name = "label57";
            label57.Size = new Size(75, 15);
            label57.TabIndex = 138;
            label57.Text = "Ngày trả tiền";
            // 
            // label58
            // 
            label58.AutoSize = true;
            label58.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label58.Location = new Point(512, 37);
            label58.Name = "label58";
            label58.Size = new Size(137, 15);
            label58.TabIndex = 137;
            label58.Text = "Phương thức thanh toán";
            // 
            // label59
            // 
            label59.AutoSize = true;
            label59.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label59.Location = new Point(21, 38);
            label59.Name = "label59";
            label59.Size = new Size(81, 15);
            label59.TabIndex = 135;
            label59.Text = "Nhà cung cấp";
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvChiTietThanhToan);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 215);
            panel2.Name = "panel2";
            panel2.Size = new Size(1075, 212);
            panel2.TabIndex = 5;
            // 
            // dgvChiTietThanhToan
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 248, 249);
            dgvChiTietThanhToan.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvChiTietThanhToan.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvChiTietThanhToan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvChiTietThanhToan.ColumnHeadersHeight = 4;
            dgvChiTietThanhToan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvChiTietThanhToan.DefaultCellStyle = dataGridViewCellStyle3;
            dgvChiTietThanhToan.Dock = DockStyle.Fill;
            dgvChiTietThanhToan.GridColor = Color.FromArgb(244, 245, 247);
            dgvChiTietThanhToan.Location = new Point(0, 0);
            dgvChiTietThanhToan.Name = "dgvChiTietThanhToan";
            dgvChiTietThanhToan.RowHeadersVisible = false;
            dgvChiTietThanhToan.Size = new Size(1075, 212);
            dgvChiTietThanhToan.TabIndex = 112;
            dgvChiTietThanhToan.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvChiTietThanhToan.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvChiTietThanhToan.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvChiTietThanhToan.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvChiTietThanhToan.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvChiTietThanhToan.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvChiTietThanhToan.ThemeStyle.BackColor = Color.White;
            dgvChiTietThanhToan.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvChiTietThanhToan.ThemeStyle.HeaderStyle.Height = 4;
            dgvChiTietThanhToan.ThemeStyle.ReadOnly = false;
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.Height = 25;
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvChiTietThanhToan.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvChiTietThanhToan.CellEnter += dgvChiTietThanhToan_CellEnter;
            dgvChiTietThanhToan.CurrentCellDirtyStateChanged += dgvChiTietThanhToan_CurrentCellDirtyStateChanged;
            // 
            // lblTongThanhToan
            // 
            lblTongThanhToan.AutoSize = true;
            lblTongThanhToan.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTongThanhToan.Location = new Point(254, 12);
            lblTongThanhToan.Name = "lblTongThanhToan";
            lblTongThanhToan.Size = new Size(35, 25);
            lblTongThanhToan.TabIndex = 3;
            lblTongThanhToan.Text = "0d";
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label76.Location = new Point(13, 13);
            label76.Name = "label76";
            label76.Size = new Size(202, 25);
            label76.TabIndex = 2;
            label76.Text = "TỔNG THANH TOÁN:";
            // 
            // panel19
            // 
            panel19.Controls.Add(lblTongThanhToan);
            panel19.Controls.Add(label76);
            panel19.Location = new Point(641, 433);
            panel19.Name = "panel19";
            panel19.Size = new Size(422, 51);
            panel19.TabIndex = 181;
            // 
            // btnInPhieuChi
            // 
            btnInPhieuChi.BorderRadius = 10;
            btnInPhieuChi.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnInPhieuChi.CustomizableEdges = customizableEdges1;
            btnInPhieuChi.DisabledState.BorderColor = Color.DarkGray;
            btnInPhieuChi.DisabledState.CustomBorderColor = Color.DarkGray;
            btnInPhieuChi.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnInPhieuChi.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnInPhieuChi.FillColor = Color.Olive;
            btnInPhieuChi.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInPhieuChi.ForeColor = Color.White;
            btnInPhieuChi.Image = Properties.Resources._1f5a8_color_removebg_preview;
            btnInPhieuChi.ImageOffset = new Point(-1, 1);
            btnInPhieuChi.Location = new Point(175, 456);
            btnInPhieuChi.Name = "btnInPhieuChi";
            btnInPhieuChi.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnInPhieuChi.Size = new Size(150, 41);
            btnInPhieuChi.TabIndex = 212;
            btnInPhieuChi.Text = "In phiếu chi";
            btnInPhieuChi.TextOffset = new Point(-2, 0);
            btnInPhieuChi.Click += btnInPhieuChi_Click;
            // 
            // btnThanhToan
            // 
            btnThanhToan.BorderRadius = 10;
            btnThanhToan.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnThanhToan.CustomizableEdges = customizableEdges3;
            btnThanhToan.DisabledState.BorderColor = Color.DarkGray;
            btnThanhToan.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThanhToan.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThanhToan.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThanhToan.FillColor = Color.Orange;
            btnThanhToan.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnThanhToan.ForeColor = Color.White;
            btnThanhToan.Image = (Image)resources.GetObject("btnThanhToan.Image");
            btnThanhToan.ImageOffset = new Point(-1, 1);
            btnThanhToan.Location = new Point(19, 456);
            btnThanhToan.Name = "btnThanhToan";
            btnThanhToan.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnThanhToan.Size = new Size(150, 41);
            btnThanhToan.TabIndex = 208;
            btnThanhToan.Text = "Thanh toán";
            btnThanhToan.TextOffset = new Point(-2, 0);
            btnThanhToan.Click += btnThanhToan_Click;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BorderRadius = 10;
            btnLamMoi.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnLamMoi.CustomizableEdges = customizableEdges5;
            btnLamMoi.DisabledState.BorderColor = Color.DarkGray;
            btnLamMoi.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLamMoi.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLamMoi.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLamMoi.FillColor = Color.Olive;
            btnLamMoi.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.ImageOffset = new Point(-1, 1);
            btnLamMoi.Location = new Point(331, 456);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnLamMoi.Size = new Size(150, 41);
            btnLamMoi.TabIndex = 213;
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.TextOffset = new Point(-2, 0);
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // frmSupplierPayment
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1075, 791);
            Controls.Add(btnLamMoi);
            Controls.Add(btnInPhieuChi);
            Controls.Add(btnThanhToan);
            Controls.Add(panel19);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pnltop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmSupplierPayment";
            Text = "frmSupplierPayment";
            Load += frmSupplierPayment_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnltop.ResumeLayout(false);
            pnltop.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvChiTietThanhToan).EndInit();
            panel19.ResumeLayout(false);
            panel19.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblnhapkhonhapkho;
        private Label lblnhapkho;
        private PictureBox pictureBox1;
        private Panel pnltop;
        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox2;
        private ComboBox cboPhuongThuc;
        private DateTimePicker dtpNgayTraTien;
        private Label label56;
        private PictureBox pictureBox16;
        private Label label57;
        private Label label58;
        private Label label59;
        private Panel panel2;
        private Label lblTongThanhToan;
        private Label label76;
        private Panel panel19;
        private Guna.UI2.WinForms.Guna2Button btnInPhieuChi;
        private Guna.UI2.WinForms.Guna2Button btnThanhToan;
        private ComboBox cboNhaCungCap;
        private Guna.UI2.WinForms.Guna2Button btnLamMoi;
        private Guna.UI2.WinForms.Guna2DataGridView dgvChiTietThanhToan;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
    }
}