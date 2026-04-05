namespace PrintingManagement
{
    partial class frmDebtReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDebtReport));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            panelcongnphaithu = new Panel();
            btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            btnPayable = new Guna.UI2.WinForms.Guna2Button();
            btnReceivable = new Guna.UI2.WinForms.Guna2Button();
            label56 = new Label();
            pictureBox16 = new PictureBox();
            panelallcongnophaithu = new Panel();
            dgvDebt = new Guna.UI2.WinForms.Guna2DataGridView();
            panel4thongke = new Panel();
            guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            label3 = new Label();
            lblSupplierCount = new Label();
            lblPayableAmount = new Label();
            ptrdondaduyet = new PictureBox();
            guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblCustomerCount = new Label();
            lblReceivableAmount = new Label();
            lbcongnochu = new Label();
            ptrthongkethangnay = new PictureBox();
            labeltrangchubaocaocongno = new Label();
            labelbaocaocongno = new Label();
            ptrbtrangchu = new PictureBox();
            panelbaocaocongno = new Panel();
            panelcongnphaithu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).BeginInit();
            panelallcongnophaithu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDebt).BeginInit();
            panel4thongke.SuspendLayout();
            guna2ShadowPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdondaduyet).BeginInit();
            guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrthongkethangnay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panelbaocaocongno.SuspendLayout();
            SuspendLayout();
            // 
            // panelcongnphaithu
            // 
            panelcongnphaithu.Controls.Add(btnRefresh);
            panelcongnphaithu.Controls.Add(btnPayable);
            panelcongnphaithu.Controls.Add(btnReceivable);
            panelcongnphaithu.Controls.Add(label56);
            panelcongnphaithu.Controls.Add(pictureBox16);
            panelcongnphaithu.Dock = DockStyle.Top;
            panelcongnphaithu.Location = new Point(0, 0);
            panelcongnphaithu.Margin = new Padding(3, 2, 3, 2);
            panelcongnphaithu.Name = "panelcongnphaithu";
            panelcongnphaithu.Size = new Size(1075, 89);
            panelcongnphaithu.TabIndex = 5;
            // 
            // btnRefresh
            // 
            btnRefresh.BorderRadius = 10;
            btnRefresh.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnRefresh.CustomizableEdges = customizableEdges1;
            btnRefresh.DisabledState.BorderColor = Color.DarkGray;
            btnRefresh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRefresh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRefresh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRefresh.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Image = (Image)resources.GetObject("btnRefresh.Image");
            btnRefresh.ImageOffset = new Point(-1, 1);
            btnRefresh.Location = new Point(432, 42);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnRefresh.Size = new Size(204, 41);
            btnRefresh.TabIndex = 210;
            btnRefresh.Text = "Làm mới";
            btnRefresh.TextOffset = new Point(-2, 0);
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnPayable
            // 
            btnPayable.BorderRadius = 10;
            btnPayable.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnPayable.CustomizableEdges = customizableEdges3;
            btnPayable.DisabledState.BorderColor = Color.DarkGray;
            btnPayable.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPayable.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPayable.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPayable.FillColor = Color.Olive;
            btnPayable.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPayable.ForeColor = Color.White;
            btnPayable.Image = (Image)resources.GetObject("btnPayable.Image");
            btnPayable.ImageOffset = new Point(-1, 1);
            btnPayable.Location = new Point(808, 42);
            btnPayable.Name = "btnPayable";
            btnPayable.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnPayable.Size = new Size(264, 41);
            btnPayable.TabIndex = 209;
            btnPayable.Text = "Công nợ phải trả";
            btnPayable.TextOffset = new Point(-2, 0);
            btnPayable.Click += btnPayable_Click;
            // 
            // btnReceivable
            // 
            btnReceivable.BorderRadius = 10;
            btnReceivable.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnReceivable.CustomizableEdges = customizableEdges5;
            btnReceivable.DisabledState.BorderColor = Color.DarkGray;
            btnReceivable.DisabledState.CustomBorderColor = Color.DarkGray;
            btnReceivable.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnReceivable.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnReceivable.FillColor = Color.DarkCyan;
            btnReceivable.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnReceivable.ForeColor = Color.White;
            btnReceivable.Image = (Image)resources.GetObject("btnReceivable.Image");
            btnReceivable.ImageOffset = new Point(-1, 1);
            btnReceivable.Location = new Point(3, 42);
            btnReceivable.Name = "btnReceivable";
            btnReceivable.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnReceivable.Size = new Size(264, 41);
            btnReceivable.TabIndex = 208;
            btnReceivable.Text = "Công nợ phải thu";
            btnReceivable.TextOffset = new Point(-2, 0);
            btnReceivable.Click += btnReceivable_Click;
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label56.Location = new Point(35, 6);
            label56.Name = "label56";
            label56.Size = new Size(162, 21);
            label56.TabIndex = 142;
            label56.Text = "Thông tin thanh toán";
            // 
            // pictureBox16
            // 
            pictureBox16.Image = (Image)resources.GetObject("pictureBox16.Image");
            pictureBox16.Location = new Point(15, 6);
            pictureBox16.Name = "pictureBox16";
            pictureBox16.Size = new Size(21, 20);
            pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox16.TabIndex = 141;
            pictureBox16.TabStop = false;
            // 
            // panelallcongnophaithu
            // 
            panelallcongnophaithu.Controls.Add(dgvDebt);
            panelallcongnophaithu.Controls.Add(panelcongnphaithu);
            panelallcongnophaithu.Dock = DockStyle.Fill;
            panelallcongnophaithu.Location = new Point(0, 192);
            panelallcongnophaithu.Name = "panelallcongnophaithu";
            panelallcongnophaithu.Size = new Size(1075, 392);
            panelallcongnophaithu.TabIndex = 8;
            // 
            // dgvDebt
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvDebt.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvDebt.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvDebt.ColumnHeadersHeight = 4;
            dgvDebt.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvDebt.DefaultCellStyle = dataGridViewCellStyle3;
            dgvDebt.Dock = DockStyle.Fill;
            dgvDebt.GridColor = Color.FromArgb(231, 229, 255);
            dgvDebt.Location = new Point(0, 89);
            dgvDebt.Name = "dgvDebt";
            dgvDebt.RowHeadersVisible = false;
            dgvDebt.Size = new Size(1075, 303);
            dgvDebt.TabIndex = 6;
            dgvDebt.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvDebt.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvDebt.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvDebt.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvDebt.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvDebt.ThemeStyle.BackColor = Color.White;
            dgvDebt.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvDebt.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvDebt.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDebt.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvDebt.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvDebt.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvDebt.ThemeStyle.HeaderStyle.Height = 4;
            dgvDebt.ThemeStyle.ReadOnly = false;
            dgvDebt.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvDebt.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDebt.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvDebt.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvDebt.ThemeStyle.RowsStyle.Height = 25;
            dgvDebt.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvDebt.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // panel4thongke
            // 
            panel4thongke.Controls.Add(guna2ShadowPanel2);
            panel4thongke.Controls.Add(guna2ShadowPanel1);
            panel4thongke.Dock = DockStyle.Top;
            panel4thongke.Location = new Point(0, 78);
            panel4thongke.Name = "panel4thongke";
            panel4thongke.Size = new Size(1075, 114);
            panel4thongke.TabIndex = 7;
            // 
            // guna2ShadowPanel2
            // 
            guna2ShadowPanel2.BackColor = Color.Transparent;
            guna2ShadowPanel2.Controls.Add(label3);
            guna2ShadowPanel2.Controls.Add(lblSupplierCount);
            guna2ShadowPanel2.Controls.Add(lblPayableAmount);
            guna2ShadowPanel2.Controls.Add(ptrdondaduyet);
            guna2ShadowPanel2.FillColor = Color.White;
            guna2ShadowPanel2.Location = new Point(545, 0);
            guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            guna2ShadowPanel2.ShadowColor = Color.Black;
            guna2ShadowPanel2.Size = new Size(530, 114);
            guna2ShadowPanel2.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(74, 11);
            label3.Name = "label3";
            label3.Size = new Size(107, 17);
            label3.TabIndex = 10;
            label3.Text = "Công nợ phải trả";
            // 
            // lblSupplierCount
            // 
            lblSupplierCount.AutoSize = true;
            lblSupplierCount.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSupplierCount.Location = new Point(76, 77);
            lblSupplierCount.Name = "lblSupplierCount";
            lblSupplierCount.Size = new Size(132, 17);
            lblSupplierCount.TabIndex = 9;
            lblSupplierCount.Text = "+18% so tháng trước";
            // 
            // lblPayableAmount
            // 
            lblPayableAmount.AutoSize = true;
            lblPayableAmount.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPayableAmount.Location = new Point(74, 32);
            lblPayableAmount.Name = "lblPayableAmount";
            lblPayableAmount.Size = new Size(28, 32);
            lblPayableAmount.TabIndex = 8;
            lblPayableAmount.Text = "0";
            // 
            // ptrdondaduyet
            // 
            ptrdondaduyet.ErrorImage = null;
            ptrdondaduyet.Image = (Image)resources.GetObject("ptrdondaduyet.Image");
            ptrdondaduyet.InitialImage = null;
            ptrdondaduyet.Location = new Point(19, 3);
            ptrdondaduyet.Name = "ptrdondaduyet";
            ptrdondaduyet.Size = new Size(49, 61);
            ptrdondaduyet.SizeMode = PictureBoxSizeMode.Zoom;
            ptrdondaduyet.TabIndex = 7;
            ptrdondaduyet.TabStop = false;
            // 
            // guna2ShadowPanel1
            // 
            guna2ShadowPanel1.BackColor = Color.Transparent;
            guna2ShadowPanel1.Controls.Add(lblCustomerCount);
            guna2ShadowPanel1.Controls.Add(lblReceivableAmount);
            guna2ShadowPanel1.Controls.Add(lbcongnochu);
            guna2ShadowPanel1.Controls.Add(ptrthongkethangnay);
            guna2ShadowPanel1.Dock = DockStyle.Left;
            guna2ShadowPanel1.FillColor = Color.White;
            guna2ShadowPanel1.Location = new Point(0, 0);
            guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            guna2ShadowPanel1.ShadowColor = Color.Black;
            guna2ShadowPanel1.Size = new Size(530, 114);
            guna2ShadowPanel1.TabIndex = 9;
            // 
            // lblCustomerCount
            // 
            lblCustomerCount.AutoSize = true;
            lblCustomerCount.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCustomerCount.Location = new Point(78, 77);
            lblCustomerCount.Name = "lblCustomerCount";
            lblCustomerCount.Size = new Size(132, 17);
            lblCustomerCount.TabIndex = 9;
            lblCustomerCount.Text = "+18% so tháng trước";
            // 
            // lblReceivableAmount
            // 
            lblReceivableAmount.AutoSize = true;
            lblReceivableAmount.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblReceivableAmount.Location = new Point(76, 32);
            lblReceivableAmount.Name = "lblReceivableAmount";
            lblReceivableAmount.Size = new Size(28, 32);
            lblReceivableAmount.TabIndex = 8;
            lblReceivableAmount.Text = "0";
            // 
            // lbcongnochu
            // 
            lbcongnochu.AutoSize = true;
            lbcongnochu.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbcongnochu.Location = new Point(76, 11);
            lbcongnochu.Name = "lbcongnochu";
            lbcongnochu.Size = new Size(109, 17);
            lbcongnochu.TabIndex = 5;
            lbcongnochu.Text = "Công nợ phải thu";
            // 
            // ptrthongkethangnay
            // 
            ptrthongkethangnay.ErrorImage = null;
            ptrthongkethangnay.Image = (Image)resources.GetObject("ptrthongkethangnay.Image");
            ptrthongkethangnay.InitialImage = null;
            ptrthongkethangnay.Location = new Point(21, 3);
            ptrthongkethangnay.Name = "ptrthongkethangnay";
            ptrthongkethangnay.Size = new Size(49, 61);
            ptrthongkethangnay.SizeMode = PictureBoxSizeMode.Zoom;
            ptrthongkethangnay.TabIndex = 4;
            ptrthongkethangnay.TabStop = false;
            // 
            // labeltrangchubaocaocongno
            // 
            labeltrangchubaocaocongno.AutoSize = true;
            labeltrangchubaocaocongno.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labeltrangchubaocaocongno.Location = new Point(6, 55);
            labeltrangchubaocaocongno.Name = "labeltrangchubaocaocongno";
            labeltrangchubaocaocongno.Size = new Size(211, 20);
            labeltrangchubaocaocongno.TabIndex = 2;
            labeltrangchubaocaocongno.Text = "Trang chủ / Báo cáo / Công nợ";
            // 
            // labelbaocaocongno
            // 
            labelbaocaocongno.AutoSize = true;
            labelbaocaocongno.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelbaocaocongno.Location = new Point(56, 11);
            labelbaocaocongno.Name = "labelbaocaocongno";
            labelbaocaocongno.Size = new Size(241, 32);
            labelbaocaocongno.TabIndex = 1;
            labelbaocaocongno.Text = "BÁO CÁO CÔNG NỢ";
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
            // panelbaocaocongno
            // 
            panelbaocaocongno.Controls.Add(labeltrangchubaocaocongno);
            panelbaocaocongno.Controls.Add(labelbaocaocongno);
            panelbaocaocongno.Controls.Add(ptrbtrangchu);
            panelbaocaocongno.Dock = DockStyle.Top;
            panelbaocaocongno.Location = new Point(0, 0);
            panelbaocaocongno.Name = "panelbaocaocongno";
            panelbaocaocongno.Size = new Size(1075, 78);
            panelbaocaocongno.TabIndex = 6;
            // 
            // frmDebtReport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1075, 584);
            Controls.Add(panelallcongnophaithu);
            Controls.Add(panel4thongke);
            Controls.Add(panelbaocaocongno);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmDebtReport";
            Load += frmDebtReport_Load;
            panelcongnphaithu.ResumeLayout(false);
            panelcongnphaithu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).EndInit();
            panelallcongnophaithu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDebt).EndInit();
            panel4thongke.ResumeLayout(false);
            guna2ShadowPanel2.ResumeLayout(false);
            guna2ShadowPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrdondaduyet).EndInit();
            guna2ShadowPanel1.ResumeLayout(false);
            guna2ShadowPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrthongkethangnay).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panelbaocaocongno.ResumeLayout(false);
            panelbaocaocongno.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panelcongnphaithu;
        private Panel panelallcongnophaithu;
        private Panel panel4thongke;
        private Label labeltrangchubaocaocongno;
        private Label labelbaocaocongno;
        private PictureBox ptrbtrangchu;
        private Panel panelbaocaocongno;
        private Label label56;
        private PictureBox pictureBox16;
        private DataGridView dataGridViewbccn1;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Label lblchobnnhacungcap;
        private Label lblsocongnophaitra;
        private Label lblcongnophaitra;
        private PictureBox ptrdondaduyet;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Label lbltubaonhieukh;
        private Label lbltongcongno;
        private Label lbcongnochu;
        private PictureBox ptrthongkethangnay;
        private Guna.UI2.WinForms.Guna2DataGridView dgvDebt;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2Button btnPayable;
        private Guna.UI2.WinForms.Guna2Button btnReceivable;
        private Label label3;
        private Label lblSupplierCount;
        private Label lblPayableAmount;
        private Label lblCustomerCount;
        private Label lblReceivableAmount;
    }
}