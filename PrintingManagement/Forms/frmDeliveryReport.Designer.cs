namespace PrintingManagement
{
    partial class frmDeliveryReport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeliveryReport));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            dgvReport = new Guna.UI2.WinForms.Guna2DataGridView();
            panel1 = new Panel();
            labeltrangchukhobaocaokho = new Label();
            labelbaocaokho = new Label();
            ptrbtrangchu = new PictureBox();
            dtpToDate = new DateTimePicker();
            dtpFromDate = new DateTimePicker();
            btnView = new Guna.UI2.WinForms.Guna2Button();
            lbldenngay = new Label();
            lbltungay = new Label();
            lblTongCong = new Label();
            panelchonloaivabaocaoBCK = new Panel();
            panelbaocaokho = new Panel();
            panelBottom = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panelchonloaivabaocaoBCK.SuspendLayout();
            panelbaocaokho.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // dgvReport
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvReport.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvReport.ColumnHeadersHeight = 38;
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvReport.DefaultCellStyle = dataGridViewCellStyle3;
            dgvReport.Dock = DockStyle.Fill;
            dgvReport.GridColor = Color.FromArgb(231, 229, 255);
            dgvReport.Location = new Point(0, 0);
            dgvReport.Name = "dgvReport";
            dgvReport.RowHeadersVisible = false;
            dgvReport.RowTemplate.Height = 32;
            dgvReport.Size = new Size(1075, 452);
            dgvReport.TabIndex = 0;
            dgvReport.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvReport.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvReport.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvReport.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvReport.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvReport.ThemeStyle.BackColor = Color.White;
            dgvReport.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvReport.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvReport.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvReport.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvReport.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvReport.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReport.ThemeStyle.HeaderStyle.Height = 38;
            dgvReport.ThemeStyle.ReadOnly = false;
            dgvReport.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvReport.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReport.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvReport.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvReport.ThemeStyle.RowsStyle.Height = 32;
            dgvReport.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvReport.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // panel1
            // 
            panel1.Controls.Add(dgvReport);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 168);
            panel1.Name = "panel1";
            panel1.Size = new Size(1075, 452);
            panel1.TabIndex = 8;
            // 
            // labeltrangchukhobaocaokho
            // 
            labeltrangchukhobaocaokho.AutoSize = true;
            labeltrangchukhobaocaokho.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labeltrangchukhobaocaokho.Location = new Point(6, 55);
            labeltrangchukhobaocaokho.Name = "labeltrangchukhobaocaokho";
            labeltrangchukhobaocaokho.Size = new Size(288, 20);
            labeltrangchukhobaocaokho.TabIndex = 2;
            labeltrangchukhobaocaokho.Text = "Trang chủ / Bán hàng / Báo cáo giao hàng";
            // 
            // labelbaocaokho
            // 
            labelbaocaokho.AutoSize = true;
            labelbaocaokho.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            labelbaocaokho.Location = new Point(56, 11);
            labelbaocaokho.Name = "labelbaocaokho";
            labelbaocaokho.Size = new Size(266, 32);
            labelbaocaokho.TabIndex = 1;
            labelbaocaokho.Text = "BÁO CÁO GIAO HÀNG";
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
            // dtpToDate
            // 
            dtpToDate.CustomFormat = "dd/MM/yyyy";
            dtpToDate.Format = DateTimePickerFormat.Custom;
            dtpToDate.Location = new Point(300, 39);
            dtpToDate.Name = "dtpToDate";
            dtpToDate.Size = new Size(220, 23);
            dtpToDate.TabIndex = 16;
            // 
            // dtpFromDate
            // 
            dtpFromDate.CustomFormat = "dd/MM/yyyy";
            dtpFromDate.Format = DateTimePickerFormat.Custom;
            dtpFromDate.Location = new Point(12, 39);
            dtpFromDate.Name = "dtpFromDate";
            dtpFromDate.Size = new Size(220, 23);
            dtpFromDate.TabIndex = 15;
            // 
            // btnView
            // 
            btnView.BorderRadius = 10;
            btnView.CustomizableEdges = customizableEdges1;
            btnView.DisabledState.BorderColor = Color.DarkGray;
            btnView.DisabledState.CustomBorderColor = Color.DarkGray;
            btnView.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnView.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnView.FillColor = Color.Orange;
            btnView.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnView.ForeColor = Color.White;
            btnView.Location = new Point(560, 30);
            btnView.Name = "btnView";
            btnView.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnView.Size = new Size(140, 36);
            btnView.TabIndex = 234;
            btnView.Text = "Tìm";
            // 
            // lbldenngay
            // 
            lbldenngay.AutoSize = true;
            lbldenngay.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbldenngay.Location = new Point(300, 20);
            lbldenngay.Name = "lbldenngay";
            lbldenngay.Size = new Size(63, 17);
            lbldenngay.TabIndex = 11;
            lbldenngay.Text = "Đến ngày";
            // 
            // lbltungay
            // 
            lbltungay.AutoSize = true;
            lbltungay.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbltungay.Location = new Point(12, 20);
            lbltungay.Name = "lbltungay";
            lbltungay.Size = new Size(55, 17);
            lbltungay.TabIndex = 10;
            lbltungay.Text = "Từ ngày";
            // 
            // lblTongCong
            // 
            lblTongCong.AutoSize = true;
            lblTongCong.Dock = DockStyle.Left;
            lblTongCong.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblTongCong.ForeColor = Color.FromArgb(100, 88, 255);
            lblTongCong.Location = new Point(0, 0);
            lblTongCong.Margin = new Padding(12, 0, 0, 0);
            lblTongCong.Name = "lblTongCong";
            lblTongCong.Padding = new Padding(0, 8, 0, 8);
            lblTongCong.Size = new Size(137, 35);
            lblTongCong.TabIndex = 9;
            lblTongCong.Text = "Tổng cộng: 0 phiếu";
            lblTongCong.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelchonloaivabaocaoBCK
            // 
            panelchonloaivabaocaoBCK.Controls.Add(lbltungay);
            panelchonloaivabaocaoBCK.Controls.Add(dtpFromDate);
            panelchonloaivabaocaoBCK.Controls.Add(lbldenngay);
            panelchonloaivabaocaoBCK.Controls.Add(dtpToDate);
            panelchonloaivabaocaoBCK.Controls.Add(btnView);
            panelchonloaivabaocaoBCK.Dock = DockStyle.Top;
            panelchonloaivabaocaoBCK.Location = new Point(0, 78);
            panelchonloaivabaocaoBCK.Name = "panelchonloaivabaocaoBCK";
            panelchonloaivabaocaoBCK.Size = new Size(1075, 90);
            panelchonloaivabaocaoBCK.TabIndex = 7;
            // 
            // panelbaocaokho
            // 
            panelbaocaokho.Controls.Add(labeltrangchukhobaocaokho);
            panelbaocaokho.Controls.Add(labelbaocaokho);
            panelbaocaokho.Controls.Add(ptrbtrangchu);
            panelbaocaokho.Dock = DockStyle.Top;
            panelbaocaokho.Location = new Point(0, 0);
            panelbaocaokho.Name = "panelbaocaokho";
            panelbaocaokho.Size = new Size(1075, 78);
            panelbaocaokho.TabIndex = 6;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(lblTongCong);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 620);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1075, 40);
            panelBottom.TabIndex = 9;
            // 
            // frmDeliveryReport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1075, 660);
            Controls.Add(panel1);
            Controls.Add(panelBottom);
            Controls.Add(panelchonloaivabaocaoBCK);
            Controls.Add(panelbaocaokho);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmDeliveryReport";
            Text = "Báo cáo giao hàng";
            ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panelchonloaivabaocaoBCK.ResumeLayout(false);
            panelchonloaivabaocaoBCK.PerformLayout();
            panelbaocaokho.ResumeLayout(false);
            panelbaocaokho.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // ── [FIX] Khai báo đầy đủ tất cả fields ─────────────
        private Guna.UI2.WinForms.Guna2DataGridView dgvReport;
        private Panel panel1;
        private Label labeltrangchukhobaocaokho;
        private Label labelbaocaokho;
        private PictureBox ptrbtrangchu;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private Guna.UI2.WinForms.Guna2Button btnView;
        private Label lbldenngay;
        private Label lbltungay;
        private Label lblTongCong;   // [FIX] thiếu khai báo field này
        private Panel panelchonloaivabaocaoBCK;
        private Panel panelbaocaokho;
        private Panel panelBottom;
    }
}