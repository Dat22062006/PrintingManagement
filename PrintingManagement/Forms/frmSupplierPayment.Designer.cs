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
            cboSupplier = new ComboBox();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            cboPaymentMethod = new ComboBox();
            dtpPaymentDate = new DateTimePicker();
            label56 = new Label();
            pictureBox16 = new PictureBox();
            label57 = new Label();
            label58 = new Label();
            label59 = new Label();
            panel2 = new Panel();
            dgvPaymentDetails = new Guna.UI2.WinForms.Guna2DataGridView();
            lblTotalPayment = new Label();
            label76 = new Label();
            panel19 = new Panel();
            btnPrint = new Guna.UI2.WinForms.Guna2Button();
            btnPay = new Guna.UI2.WinForms.Guna2Button();
            btnReset = new Guna.UI2.WinForms.Guna2Button();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            cboDebitAccount = new ComboBox();
            label2 = new Label();
            cboCreditAccount = new ComboBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnltop.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPaymentDetails).BeginInit();
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
            lblnhapkho.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
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
            panel1.Controls.Add(cboCreditAccount);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(cboDebitAccount);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(cboSupplier);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(cboPaymentMethod);
            panel1.Controls.Add(dtpPaymentDate);
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
            // cboSupplier
            // 
            cboSupplier.FormattingEnabled = true;
            cboSupplier.Location = new Point(19, 56);
            cboSupplier.Name = "cboSupplier";
            cboSupplier.Size = new Size(289, 23);
            cboSupplier.TabIndex = 151;
            cboSupplier.SelectedIndexChanged += cboSupplier_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
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
            // cboPaymentMethod
            // 
            cboPaymentMethod.FormattingEnabled = true;
            cboPaymentMethod.Items.AddRange(new object[] { "Tiền mặt", "Uỷ nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboPaymentMethod.Location = new Point(512, 55);
            cboPaymentMethod.Name = "cboPaymentMethod";
            cboPaymentMethod.Size = new Size(187, 23);
            cboPaymentMethod.TabIndex = 144;
            // 
            // dtpPaymentDate
            // 
            dtpPaymentDate.CustomFormat = "dd/MM/yyyy";
            dtpPaymentDate.Format = DateTimePickerFormat.Custom;
            dtpPaymentDate.Location = new Point(341, 56);
            dtpPaymentDate.Name = "dtpPaymentDate";
            dtpPaymentDate.Size = new Size(165, 23);
            dtpPaymentDate.TabIndex = 141;
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
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
            label57.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label57.Location = new Point(341, 37);
            label57.Name = "label57";
            label57.Size = new Size(75, 15);
            label57.TabIndex = 138;
            label57.Text = "Ngày trả tiền";
            // 
            // label58
            // 
            label58.AutoSize = true;
            label58.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label58.Location = new Point(512, 37);
            label58.Name = "label58";
            label58.Size = new Size(137, 15);
            label58.TabIndex = 137;
            label58.Text = "Phương thức thanh toán";
            // 
            // label59
            // 
            label59.AutoSize = true;
            label59.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label59.Location = new Point(21, 38);
            label59.Name = "label59";
            label59.Size = new Size(81, 15);
            label59.TabIndex = 135;
            label59.Text = "Nhà cung cấp";
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvPaymentDetails);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 215);
            panel2.Name = "panel2";
            panel2.Size = new Size(1075, 212);
            panel2.TabIndex = 5;
            // 
            // dgvPaymentDetails
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 248, 249);
            dgvPaymentDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvPaymentDetails.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvPaymentDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvPaymentDetails.ColumnHeadersHeight = 4;
            dgvPaymentDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvPaymentDetails.DefaultCellStyle = dataGridViewCellStyle3;
            dgvPaymentDetails.Dock = DockStyle.Fill;
            dgvPaymentDetails.GridColor = Color.FromArgb(244, 245, 247);
            dgvPaymentDetails.Location = new Point(0, 0);
            dgvPaymentDetails.Name = "dgvPaymentDetails";
            dgvPaymentDetails.RowHeadersVisible = false;
            dgvPaymentDetails.RowTemplate.Height = 25;
            dgvPaymentDetails.Size = new Size(1075, 212);
            dgvPaymentDetails.TabIndex = 112;
            dgvPaymentDetails.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvPaymentDetails.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvPaymentDetails.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvPaymentDetails.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvPaymentDetails.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvPaymentDetails.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvPaymentDetails.ThemeStyle.BackColor = Color.White;
            dgvPaymentDetails.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvPaymentDetails.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvPaymentDetails.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPaymentDetails.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvPaymentDetails.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvPaymentDetails.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvPaymentDetails.ThemeStyle.HeaderStyle.Height = 4;
            dgvPaymentDetails.ThemeStyle.ReadOnly = false;
            dgvPaymentDetails.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvPaymentDetails.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPaymentDetails.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvPaymentDetails.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvPaymentDetails.ThemeStyle.RowsStyle.Height = 25;
            dgvPaymentDetails.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvPaymentDetails.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvPaymentDetails.CellEnter += dgvPaymentDetails_CellEnter;
            dgvPaymentDetails.CurrentCellDirtyStateChanged += dgvPaymentDetails_CurrentCellDirtyStateChanged;
            // 
            // lblTotalPayment
            // 
            lblTotalPayment.AutoSize = true;
            lblTotalPayment.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblTotalPayment.Location = new Point(254, 12);
            lblTotalPayment.Name = "lblTotalPayment";
            lblTotalPayment.Size = new Size(35, 25);
            lblTotalPayment.TabIndex = 3;
            lblTotalPayment.Text = "0d";
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label76.Location = new Point(13, 13);
            label76.Name = "label76";
            label76.Size = new Size(202, 25);
            label76.TabIndex = 2;
            label76.Text = "TỔNG THANH TOÁN:";
            // 
            // panel19
            // 
            panel19.Controls.Add(lblTotalPayment);
            panel19.Controls.Add(label76);
            panel19.Location = new Point(641, 433);
            panel19.Name = "panel19";
            panel19.Size = new Size(422, 51);
            panel19.TabIndex = 181;
            // 
            // btnPrint
            // 
            btnPrint.BorderRadius = 10;
            btnPrint.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnPrint.CustomizableEdges = customizableEdges1;
            btnPrint.DisabledState.BorderColor = Color.DarkGray;
            btnPrint.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPrint.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPrint.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPrint.FillColor = Color.Olive;
            btnPrint.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnPrint.ForeColor = Color.White;
            btnPrint.Image = Properties.Resources._1f5a8_color_removebg_preview;
            btnPrint.ImageOffset = new Point(-1, 1);
            btnPrint.Location = new Point(175, 456);
            btnPrint.Name = "btnPrint";
            btnPrint.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnPrint.Size = new Size(150, 41);
            btnPrint.TabIndex = 212;
            btnPrint.Text = "In phiếu chi";
            btnPrint.TextOffset = new Point(-2, 0);
            btnPrint.Click += btnPrint_Click;
            // 
            // btnPay
            // 
            btnPay.BorderRadius = 10;
            btnPay.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnPay.CustomizableEdges = customizableEdges3;
            btnPay.DisabledState.BorderColor = Color.DarkGray;
            btnPay.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPay.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPay.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPay.FillColor = Color.Orange;
            btnPay.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnPay.ForeColor = Color.White;
            btnPay.Image = (Image)resources.GetObject("btnPay.Image");
            btnPay.ImageOffset = new Point(-1, 1);
            btnPay.Location = new Point(19, 456);
            btnPay.Name = "btnPay";
            btnPay.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnPay.Size = new Size(150, 41);
            btnPay.TabIndex = 208;
            btnPay.Text = "Thanh toán";
            btnPay.TextOffset = new Point(-2, 0);
            btnPay.Click += btnPay_Click;
            // 
            // btnReset
            // 
            btnReset.BorderRadius = 10;
            btnReset.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnReset.CustomizableEdges = customizableEdges5;
            btnReset.DisabledState.BorderColor = Color.DarkGray;
            btnReset.DisabledState.CustomBorderColor = Color.DarkGray;
            btnReset.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnReset.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnReset.FillColor = Color.Olive;
            btnReset.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnReset.ForeColor = Color.White;
            btnReset.ImageOffset = new Point(-1, 1);
            btnReset.Location = new Point(331, 456);
            btnReset.Name = "btnReset";
            btnReset.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnReset.Size = new Size(150, 41);
            btnReset.TabIndex = 213;
            btnReset.Text = "Làm mới";
            btnReset.TextOffset = new Point(-2, 0);
            btnReset.Click += btnReset_Click;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // cboDebitAccount
            // 
            cboDebitAccount.FormattingEnabled = true;
            cboDebitAccount.Items.AddRange(new object[] { "Tiền mặt", "Uỷ nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboDebitAccount.Location = new Point(709, 55);
            cboDebitAccount.Name = "cboDebitAccount";
            cboDebitAccount.Size = new Size(180, 23);
            cboDebitAccount.TabIndex = 153;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(709, 37);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 152;
            label2.Text = "TK Nợ";
            // 
            // cboCreditAccount
            // 
            cboCreditAccount.FormattingEnabled = true;
            cboCreditAccount.Items.AddRange(new object[] { "Tiền mặt", "Uỷ nhiệm chi", "Séc chuyển khoản", "Séc tiền mặt" });
            cboCreditAccount.Location = new Point(895, 55);
            cboCreditAccount.Name = "cboCreditAccount";
            cboCreditAccount.Size = new Size(168, 23);
            cboCreditAccount.TabIndex = 155;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(895, 37);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 154;
            label3.Text = "TK Có";
            // 
            // frmSupplierPayment
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1075, 791);
            Controls.Add(btnReset);
            Controls.Add(btnPrint);
            Controls.Add(btnPay);
            Controls.Add(panel19);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pnltop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmSupplierPayment";
            Load += frmSupplierPayment_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnltop.ResumeLayout(false);
            pnltop.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPaymentDetails).EndInit();
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
        private ComboBox cboPaymentMethod;
        private DateTimePicker dtpPaymentDate;
        private Label label56;
        private PictureBox pictureBox16;
        private Label label57;
        private Label label58;
        private Label label59;
        private Panel panel2;
        private Label lblTotalPayment;
        private Label label76;
        private Panel panel19;
        private Guna.UI2.WinForms.Guna2Button btnPrint;
        private Guna.UI2.WinForms.Guna2Button btnPay;
        private ComboBox cboSupplier;
        private Guna.UI2.WinForms.Guna2Button btnReset;
        private Guna.UI2.WinForms.Guna2DataGridView dgvPaymentDetails;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private ComboBox cboCreditAccount;
        private Label label3;
        private ComboBox cboDebitAccount;
        private Label label2;
    }
}