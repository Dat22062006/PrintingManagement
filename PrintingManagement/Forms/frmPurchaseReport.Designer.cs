namespace PrintingManagement
{
    partial class frmPurchaseReport
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPurchaseReport));
            panel2 = new Panel();
            dgvPurchaseSummary = new Guna.UI2.WinForms.Guna2DataGridView();
            panel1 = new Panel();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            pnltop = new Panel();
            lblnhapkhonhapkho = new Label();
            lblnhapkho = new Label();
            pictureBox1 = new PictureBox();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPurchaseSummary).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            pnltop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvPurchaseSummary);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 146);
            panel2.Name = "panel2";
            panel2.Size = new Size(1075, 456);
            panel2.TabIndex = 187;
            // 
            // dgvPurchaseSummary
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 248, 249);
            dgvPurchaseSummary.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvPurchaseSummary.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvPurchaseSummary.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvPurchaseSummary.ColumnHeadersHeight = 4;
            dgvPurchaseSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvPurchaseSummary.DefaultCellStyle = dataGridViewCellStyle3;
            dgvPurchaseSummary.Dock = DockStyle.Fill;
            dgvPurchaseSummary.GridColor = Color.FromArgb(244, 245, 247);
            dgvPurchaseSummary.Location = new Point(0, 0);
            dgvPurchaseSummary.Name = "dgvPurchaseSummary";
            dgvPurchaseSummary.RowHeadersVisible = false;
            dgvPurchaseSummary.Size = new Size(1075, 456);
            dgvPurchaseSummary.TabIndex = 112;
            dgvPurchaseSummary.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvPurchaseSummary.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvPurchaseSummary.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvPurchaseSummary.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvPurchaseSummary.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvPurchaseSummary.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvPurchaseSummary.ThemeStyle.BackColor = Color.White;
            dgvPurchaseSummary.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvPurchaseSummary.ThemeStyle.HeaderStyle.Height = 4;
            dgvPurchaseSummary.ThemeStyle.ReadOnly = false;
            dgvPurchaseSummary.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvPurchaseSummary.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPurchaseSummary.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvPurchaseSummary.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvPurchaseSummary.ThemeStyle.RowsStyle.Height = 25;
            dgvPurchaseSummary.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvPurchaseSummary.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 94);
            panel1.Name = "panel1";
            panel1.Size = new Size(1075, 52);
            panel1.TabIndex = 186;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 163);
            label1.Location = new Point(39, 19);
            label1.Name = "label1";
            label1.Size = new Size(148, 20);
            label1.TabIndex = 150;
            label1.Text = "Tổng hợp mua hàng";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.togiaytrang_removebg_preview;
            pictureBox2.Location = new Point(19, 19);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(21, 20);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 149;
            pictureBox2.TabStop = false;
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
            pnltop.TabIndex = 185;
            // 
            // lblnhapkhonhapkho
            // 
            lblnhapkhonhapkho.AutoSize = true;
            lblnhapkhonhapkho.Location = new Point(19, 64);
            lblnhapkhonhapkho.Name = "lblnhapkhonhapkho";
            lblnhapkhonhapkho.Size = new Size(235, 15);
            lblnhapkhonhapkho.TabIndex = 2;
            lblnhapkhonhapkho.Text = "Trang chủ / Mua hàng / Báo cáo mua hàng";
            // 
            // lblnhapkho
            // 
            lblnhapkho.AutoSize = true;
            lblnhapkho.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblnhapkho.Location = new Point(70, 17);
            lblnhapkho.Name = "lblnhapkho";
            lblnhapkho.Size = new Size(262, 32);
            lblnhapkho.TabIndex = 1;
            lblnhapkho.Text = "BÁO CÁO MUA HÀNG";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.bieudo;
            pictureBox1.Location = new Point(21, 9);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(52, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // frmPurchaseReport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1075, 602);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pnltop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmPurchaseReport";
            Text = "frmPurchaseReport";
            Load += frmPurchaseReport_Load;
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPurchaseSummary).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            pnltop.ResumeLayout(false);
            pnltop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel2;
        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox2;
        private Panel pnltop;
        private Label lblnhapkhonhapkho;
        private Label lblnhapkho;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2DataGridView dgvTongHopMuaHangg;
        private Guna.UI2.WinForms.Guna2DataGridView dgvPurchaseSummary;
    }
}