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
            panelbangdieukhien = new Panel();
            lbltrangchudashboard = new Label();
            lblbangdieukhien = new Label();
            ptrbtrangchu = new PictureBox();
            panel1 = new Panel();
            dgvMaterials = new Guna.UI2.WinForms.Guna2DataGridView();
            label2 = new Label();
            cboQuote = new ComboBox();
            label49 = new Label();
            pictureBox20 = new PictureBox();
            btnLoadData = new Guna.UI2.WinForms.Guna2Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel4 = new Panel();
            btnStartProduction = new Guna.UI2.WinForms.Guna2Button();
            btnExportExcel = new Guna.UI2.WinForms.Guna2Button();
            label3 = new Label();
            txtQuantity = new TextBox();
            label106 = new Label();
            txtCustomerName = new TextBox();
            lbKichThuocSanPham = new Label();
            txtProductName = new TextBox();
            label107 = new Label();
            label108 = new Label();
            pictureBox21 = new PictureBox();
            dtpStartDate = new DateTimePicker();
            panelbangdieukhien.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMaterials).BeginInit();
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
            lbltrangchudashboard.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbltrangchudashboard.Location = new Point(6, 55);
            lbltrangchudashboard.Name = "lbltrangchudashboard";
            lbltrangchudashboard.Size = new Size(246, 20);
            lbltrangchudashboard.TabIndex = 2;
            lbltrangchudashboard.Text = "Trang chủ / Sản xuất / Lệnh sản xuất";
            // 
            // lblbangdieukhien
            // 
            lblbangdieukhien.AutoSize = true;
            lblbangdieukhien.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
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
            panel1.Controls.Add(dgvMaterials);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(cboQuote);
            panel1.Controls.Add(label49);
            panel1.Controls.Add(pictureBox20);
            panel1.Controls.Add(btnLoadData);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 78);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1036, 398);
            panel1.TabIndex = 2;
            // 
            // dgvMaterials
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvMaterials.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvMaterials.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvMaterials.ColumnHeadersHeight = 4;
            dgvMaterials.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvMaterials.DefaultCellStyle = dataGridViewCellStyle3;
            dgvMaterials.GridColor = Color.FromArgb(231, 229, 255);
            dgvMaterials.Location = new Point(0, 178);
            dgvMaterials.Name = "dgvMaterials";
            dgvMaterials.RowHeadersVisible = false;
            dgvMaterials.RowTemplate.Height = 25;
            dgvMaterials.Size = new Size(1036, 220);
            dgvMaterials.TabIndex = 228;
            dgvMaterials.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvMaterials.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvMaterials.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvMaterials.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvMaterials.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvMaterials.ThemeStyle.BackColor = Color.White;
            dgvMaterials.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvMaterials.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvMaterials.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvMaterials.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvMaterials.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvMaterials.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvMaterials.ThemeStyle.HeaderStyle.Height = 4;
            dgvMaterials.ThemeStyle.ReadOnly = false;
            dgvMaterials.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvMaterials.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMaterials.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvMaterials.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvMaterials.ThemeStyle.RowsStyle.Height = 25;
            dgvMaterials.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvMaterials.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
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
            // cboQuote
            // 
            cboQuote.FormattingEnabled = true;
            cboQuote.Location = new Point(12, 129);
            cboQuote.Name = "cboQuote";
            cboQuote.Size = new Size(647, 23);
            cboQuote.TabIndex = 225;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
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
            // btnLoadData
            // 
            btnLoadData.BorderRadius = 10;
            btnLoadData.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnLoadData.CustomizableEdges = customizableEdges1;
            btnLoadData.DisabledState.BorderColor = Color.DarkGray;
            btnLoadData.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLoadData.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLoadData.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLoadData.FillColor = Color.FromArgb(255, 128, 255);
            btnLoadData.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnLoadData.ForeColor = Color.White;
            btnLoadData.Image = Properties.Resources._2795_color_removebg_preview__1_1;
            btnLoadData.ImageOffset = new Point(-1, 1);
            btnLoadData.Location = new Point(665, 121);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLoadData.Size = new Size(361, 31);
            btnLoadData.TabIndex = 220;
            btnLoadData.Text = "Lấy dữ liệu";
            btnLoadData.TextOffset = new Point(-2, 0);
            btnLoadData.Click += btnLoadData_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
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
            panel4.Controls.Add(btnStartProduction);
            panel4.Controls.Add(btnExportExcel);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(txtQuantity);
            panel4.Controls.Add(label106);
            panel4.Controls.Add(txtCustomerName);
            panel4.Controls.Add(lbKichThuocSanPham);
            panel4.Controls.Add(txtProductName);
            panel4.Controls.Add(label107);
            panel4.Controls.Add(label108);
            panel4.Controls.Add(pictureBox21);
            panel4.Controls.Add(dtpStartDate);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(0, 476);
            panel4.Margin = new Padding(3, 2, 3, 2);
            panel4.Name = "panel4";
            panel4.Size = new Size(1036, 288);
            panel4.TabIndex = 3;
            // 
            // btnStartProduction
            // 
            btnStartProduction.BorderRadius = 10;
            btnStartProduction.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnStartProduction.CustomizableEdges = customizableEdges3;
            btnStartProduction.DisabledState.BorderColor = Color.DarkGray;
            btnStartProduction.DisabledState.CustomBorderColor = Color.DarkGray;
            btnStartProduction.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnStartProduction.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnStartProduction.FillColor = Color.Blue;
            btnStartProduction.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnStartProduction.ForeColor = Color.White;
            btnStartProduction.Image = Properties.Resources.nhammay2;
            btnStartProduction.ImageOffset = new Point(-1, -2);
            btnStartProduction.Location = new Point(169, 116);
            btnStartProduction.Name = "btnStartProduction";
            btnStartProduction.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnStartProduction.Size = new Size(158, 31);
            btnStartProduction.TabIndex = 229;
            btnStartProduction.Text = "Sản Xuất";
            btnStartProduction.TextOffset = new Point(-2, 0);
            btnStartProduction.Click += btnStartProduction_Click;
            // 
            // btnExportExcel
            // 
            btnExportExcel.BorderRadius = 10;
            btnExportExcel.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnExportExcel.CustomizableEdges = customizableEdges5;
            btnExportExcel.DisabledState.BorderColor = Color.DarkGray;
            btnExportExcel.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExportExcel.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExportExcel.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExportExcel.FillColor = Color.FromArgb(255, 128, 255);
            btnExportExcel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.Image = Properties.Resources._1f5a8_color_removebg_preview1;
            btnExportExcel.ImageOffset = new Point(-1, 1);
            btnExportExcel.Location = new Point(12, 116);
            btnExportExcel.Name = "btnExportExcel";
            btnExportExcel.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnExportExcel.Size = new Size(151, 31);
            btnExportExcel.TabIndex = 228;
            btnExportExcel.Text = "In";
            btnExportExcel.TextOffset = new Point(-2, 0);
            btnExportExcel.Click += btnExportExcel_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(819, 44);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 117;
            label3.Text = "Ngày bắt đầu";
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(321, 61);
            txtQuantity.Margin = new Padding(3, 2, 3, 2);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(151, 23);
            txtQuantity.TabIndex = 116;
            // 
            // label106
            // 
            label106.AutoSize = true;
            label106.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label106.Location = new Point(321, 44);
            label106.Name = "label106";
            label106.Size = new Size(54, 15);
            label106.TabIndex = 115;
            label106.Text = "Số lượng";
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new Point(478, 61);
            txtCustomerName.Margin = new Padding(3, 2, 3, 2);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new Size(335, 23);
            txtCustomerName.TabIndex = 114;
            // 
            // lbKichThuocSanPham
            // 
            lbKichThuocSanPham.AutoSize = true;
            lbKichThuocSanPham.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lbKichThuocSanPham.Location = new Point(478, 44);
            lbKichThuocSanPham.Name = "lbKichThuocSanPham";
            lbKichThuocSanPham.Size = new Size(72, 15);
            lbKichThuocSanPham.TabIndex = 113;
            lbKichThuocSanPham.Text = "Khách Hàng";
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(12, 61);
            txtProductName.Margin = new Padding(3, 2, 3, 2);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(297, 23);
            txtProductName.TabIndex = 112;
            // 
            // label107
            // 
            label107.AutoSize = true;
            label107.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label107.Location = new Point(17, 44);
            label107.Name = "label107";
            label107.Size = new Size(81, 15);
            label107.TabIndex = 111;
            label107.Text = "Tên sản phẩm";
            // 
            // label108
            // 
            label108.AutoSize = true;
            label108.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
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
            // dtpStartDate
            // 
            dtpStartDate.CustomFormat = "dd/MM/yyyy";
            dtpStartDate.Format = DateTimePickerFormat.Custom;
            dtpStartDate.Location = new Point(819, 61);
            dtpStartDate.Margin = new Padding(3, 2, 3, 2);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(169, 23);
            dtpStartDate.TabIndex = 108;
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
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "frmProductionOrder";
            panelbangdieukhien.ResumeLayout(false);
            panelbangdieukhien.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMaterials).EndInit();
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
        private Guna.UI2.WinForms.Guna2Button btnLoadData;
        private ComboBox cboQuote;
        private Label label49;
        private PictureBox pictureBox20;
        private Label label2;
        private Guna.UI2.WinForms.Guna2DataGridView dgvMaterials;
        private Label label3;
        private TextBox txtQuantity;
        private Label label106;
        private TextBox txtCustomerName;
        private Label lbKichThuocSanPham;
        private TextBox txtProductName;
        private Label label107;
        private Label label108;
        private PictureBox pictureBox21;
        private DateTimePicker dtpStartDate;
        private Guna.UI2.WinForms.Guna2Button btnStartProduction;
        private Guna.UI2.WinForms.Guna2Button btnExportExcel;
    }
}