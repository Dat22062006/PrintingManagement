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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelthutienkh = new Panel();
            labeltrangchuthutien = new Label();
            labelthutienkh = new Label();
            ptrbtrangchu = new PictureBox();
            paneldscongno = new Panel();
            dgvDebtList = new Guna.UI2.WinForms.Guna2DataGridView();
            label2 = new Label();
            pictureBox3 = new PictureBox();
            label1 = new Label();
            pictureBox7 = new PictureBox();
            btnCollect = new Guna.UI2.WinForms.Guna2Button();
            btnPrint = new Guna.UI2.WinForms.Guna2Button();
            panelsotientongthu = new Panel();
            lblTotalCollected = new Label();
            paneltongthuttkh = new Panel();
            labeltongthuttkh = new Label();
            cboPaymentMethod = new ComboBox();
            dtpPaymentDate = new DateTimePicker();
            lblphuongthucthu = new Label();
            lblkhachhang = new Label();
            labelkhachhangttkh = new Label();
            cboCustomer = new ComboBox();
            panelthutienkh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            paneldscongno.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDebtList).BeginInit();
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
            labeltrangchuthutien.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labeltrangchuthutien.Location = new Point(6, 55);
            labeltrangchuthutien.Name = "labeltrangchuthutien";
            labeltrangchuthutien.Size = new Size(216, 20);
            labeltrangchuthutien.TabIndex = 2;
            labeltrangchuthutien.Text = "Trang chủ / Bán hàng / Thu tiền";
            // 
            // labelthutienkh
            // 
            labelthutienkh.AutoSize = true;
            labelthutienkh.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
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
            paneldscongno.Controls.Add(dgvDebtList);
            paneldscongno.Controls.Add(label2);
            paneldscongno.Controls.Add(pictureBox3);
            paneldscongno.Controls.Add(label1);
            paneldscongno.Controls.Add(pictureBox7);
            paneldscongno.Controls.Add(btnCollect);
            paneldscongno.Controls.Add(btnPrint);
            paneldscongno.Controls.Add(panelsotientongthu);
            paneldscongno.Controls.Add(paneltongthuttkh);
            paneldscongno.Controls.Add(cboPaymentMethod);
            paneldscongno.Controls.Add(dtpPaymentDate);
            paneldscongno.Controls.Add(lblphuongthucthu);
            paneldscongno.Controls.Add(lblkhachhang);
            paneldscongno.Controls.Add(labelkhachhangttkh);
            paneldscongno.Controls.Add(cboCustomer);
            paneldscongno.AutoScroll = true;
            paneldscongno.Dock = DockStyle.Fill;
            paneldscongno.Location = new Point(0, 78);
            paneldscongno.Name = "paneldscongno";
            paneldscongno.Size = new Size(1058, 983);
            paneldscongno.TabIndex = 5;
            // 
            // dgvDebtList
            // 
            dataGridViewCellStyle4.BackColor = Color.White;
            dgvDebtList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvDebtList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvDebtList.ColumnHeadersHeight = 4;
            dgvDebtList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvDebtList.DefaultCellStyle = dataGridViewCellStyle6;
            dgvDebtList.GridColor = Color.FromArgb(231, 229, 255);
            dgvDebtList.Location = new Point(13, 173);
            dgvDebtList.Name = "dgvDebtList";
            dgvDebtList.RowHeadersVisible = false;
            dgvDebtList.RowTemplate.Height = 25;
            dgvDebtList.Size = new Size(1033, 387);
            dgvDebtList.TabIndex = 268;
            dgvDebtList.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvDebtList.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvDebtList.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvDebtList.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvDebtList.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvDebtList.ThemeStyle.BackColor = Color.White;
            dgvDebtList.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvDebtList.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvDebtList.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDebtList.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvDebtList.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvDebtList.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvDebtList.ThemeStyle.HeaderStyle.Height = 4;
            dgvDebtList.ThemeStyle.ReadOnly = false;
            dgvDebtList.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvDebtList.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDebtList.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvDebtList.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvDebtList.ThemeStyle.RowsStyle.Height = 25;
            dgvDebtList.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvDebtList.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvDebtList.CurrentCellDirtyStateChanged += dgvDebtList_CurrentCellDirtyStateChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
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
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
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
            // btnCollect
            // 
            btnCollect.BorderRadius = 10;
            btnCollect.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnCollect.CustomizableEdges = customizableEdges5;
            btnCollect.DisabledState.BorderColor = Color.DarkGray;
            btnCollect.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCollect.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCollect.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCollect.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnCollect.ForeColor = Color.White;
            btnCollect.Image = (Image)resources.GetObject("btnCollect.Image");
            btnCollect.ImageOffset = new Point(-1, 1);
            btnCollect.Location = new Point(17, 655);
            btnCollect.Name = "btnCollect";
            btnCollect.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnCollect.Size = new Size(134, 44);
            btnCollect.TabIndex = 258;
            btnCollect.Text = "Thu tiền";
            btnCollect.TextOffset = new Point(-2, 0);
            btnCollect.Click += btnCollect_Click;
            // 
            // btnPrint
            // 
            btnPrint.BorderRadius = 10;
            btnPrint.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnPrint.CustomizableEdges = customizableEdges7;
            btnPrint.DisabledState.BorderColor = Color.DarkGray;
            btnPrint.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPrint.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPrint.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPrint.FillColor = Color.Olive;
            btnPrint.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnPrint.ForeColor = Color.White;
            btnPrint.Image = (Image)resources.GetObject("btnPrint.Image");
            btnPrint.ImageOffset = new Point(-1, 1);
            btnPrint.Location = new Point(172, 655);
            btnPrint.Name = "btnPrint";
            btnPrint.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnPrint.Size = new Size(164, 44);
            btnPrint.TabIndex = 259;
            btnPrint.Text = "In phiếu thu";
            btnPrint.TextOffset = new Point(-2, 0);
            btnPrint.Click += btnPrint_Click;
            // 
            // panelsotientongthu
            // 
            panelsotientongthu.Controls.Add(lblTotalCollected);
            panelsotientongthu.Location = new Point(875, 655);
            panelsotientongthu.Margin = new Padding(3, 2, 3, 2);
            panelsotientongthu.Name = "panelsotientongthu";
            panelsotientongthu.Size = new Size(158, 32);
            panelsotientongthu.TabIndex = 19;
            // 
            // lblTotalCollected
            // 
            lblTotalCollected.AutoSize = true;
            lblTotalCollected.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblTotalCollected.Location = new Point(43, 8);
            lblTotalCollected.Name = "lblTotalCollected";
            lblTotalCollected.Size = new Size(87, 15);
            lblTotalCollected.TabIndex = 2;
            lblTotalCollected.Text = "135,564,000 đ";
            // 
            // paneltongthuttkh
            // 
            paneltongthuttkh.Controls.Add(labeltongthuttkh);
            paneltongthuttkh.Location = new Point(705, 655);
            paneltongthuttkh.Margin = new Padding(3, 2, 3, 2);
            paneltongthuttkh.Name = "paneltongthuttkh";
            paneltongthuttkh.Size = new Size(164, 32);
            paneltongthuttkh.TabIndex = 18;
            // 
            // labeltongthuttkh
            // 
            labeltongthuttkh.AutoSize = true;
            labeltongthuttkh.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labeltongthuttkh.Location = new Point(83, 8);
            labeltongthuttkh.Name = "labeltongthuttkh";
            labeltongthuttkh.Size = new Size(72, 15);
            labeltongthuttkh.TabIndex = 2;
            labeltongthuttkh.Text = "TỔNG THU:";
            // 
            // cboPaymentMethod
            // 
            cboPaymentMethod.FormattingEnabled = true;
            cboPaymentMethod.Location = new Point(741, 75);
            cboPaymentMethod.Name = "cboPaymentMethod";
            cboPaymentMethod.Size = new Size(305, 23);
            cboPaymentMethod.TabIndex = 16;
            // 
            // dtpPaymentDate
            // 
            dtpPaymentDate.CustomFormat = "dd/MM/yyyy";
            dtpPaymentDate.Format = DateTimePickerFormat.Custom;
            dtpPaymentDate.Location = new Point(378, 75);
            dtpPaymentDate.Name = "dtpPaymentDate";
            dtpPaymentDate.Size = new Size(323, 23);
            dtpPaymentDate.TabIndex = 15;
            // 
            // lblphuongthucthu
            // 
            lblphuongthucthu.AutoSize = true;
            lblphuongthucthu.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblphuongthucthu.Location = new Point(741, 50);
            lblphuongthucthu.Name = "lblphuongthucthu";
            lblphuongthucthu.Size = new Size(104, 17);
            lblphuongthucthu.TabIndex = 11;
            lblphuongthucthu.Text = "Phương thức thu";
            // 
            // lblkhachhang
            // 
            lblkhachhang.AutoSize = true;
            lblkhachhang.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblkhachhang.Location = new Point(378, 50);
            lblkhachhang.Name = "lblkhachhang";
            lblkhachhang.Size = new Size(86, 17);
            lblkhachhang.TabIndex = 10;
            lblkhachhang.Text = "Ngày thu tiền";
            // 
            // labelkhachhangttkh
            // 
            labelkhachhangttkh.AutoSize = true;
            labelkhachhangttkh.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            labelkhachhangttkh.Location = new Point(13, 50);
            labelkhachhangttkh.Name = "labelkhachhangttkh";
            labelkhachhangttkh.Size = new Size(76, 17);
            labelkhachhangttkh.TabIndex = 7;
            labelkhachhangttkh.Text = "Khách hàng";
            // 
            // cboCustomer
            // 
            cboCustomer.FormattingEnabled = true;
            cboCustomer.Location = new Point(13, 75);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(323, 23);
            cboCustomer.TabIndex = 6;
            cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;
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
            Load += frmPaymentReceive_Load;
            panelthutienkh.ResumeLayout(false);
            panelthutienkh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            paneldscongno.ResumeLayout(false);
            paneldscongno.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDebtList).EndInit();
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
        private DateTimePicker dtpPaymentDate;
        private Label lblphuongthucthu;
        private Label lblkhachhang;
        private Label labelkhachhangttkh;
        private ComboBox cboCustomer;
        private ComboBox cboPaymentMethod;
        private Panel panelsotientongthu;
        private Label lblTotalCollected;
        private Panel paneltongthuttkh;
        private Label labeltongthuttkh;
        private Guna.UI2.WinForms.Guna2Button btnCollect;
        private Guna.UI2.WinForms.Guna2Button btnPrint;
        private Label label2;
        private PictureBox pictureBox3;
        private Label label1;
        private PictureBox pictureBox7;
        private Guna.UI2.WinForms.Guna2DataGridView dgvDebtList;
    }
}