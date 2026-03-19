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
            dgvTongHopMuaH = new Guna.UI2.WinForms.Guna2DataGridView();
            panel1 = new Panel();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            pnltop = new Panel();
            lblnhapkhonhapkho = new Label();
            lblnhapkho = new Label();
            pictureBox1 = new PictureBox();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTongHopMuaH).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            pnltop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvTongHopMuaH);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 146);
            panel2.Name = "panel2";
            panel2.Size = new Size(1075, 456);
            panel2.TabIndex = 187;
            // 
            // dgvTongHopMuaH
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 248, 249);
            dgvTongHopMuaH.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvTongHopMuaH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvTongHopMuaH.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvTongHopMuaH.ColumnHeadersHeight = 4;
            dgvTongHopMuaH.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvTongHopMuaH.DefaultCellStyle = dataGridViewCellStyle3;
            dgvTongHopMuaH.Dock = DockStyle.Fill;
            dgvTongHopMuaH.GridColor = Color.FromArgb(244, 245, 247);
            dgvTongHopMuaH.Location = new Point(0, 0);
            dgvTongHopMuaH.Name = "dgvTongHopMuaH";
            dgvTongHopMuaH.RowHeadersVisible = false;
            dgvTongHopMuaH.Size = new Size(1075, 456);
            dgvTongHopMuaH.TabIndex = 112;
            dgvTongHopMuaH.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvTongHopMuaH.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvTongHopMuaH.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvTongHopMuaH.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvTongHopMuaH.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvTongHopMuaH.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvTongHopMuaH.ThemeStyle.BackColor = Color.White;
            dgvTongHopMuaH.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvTongHopMuaH.ThemeStyle.HeaderStyle.Height = 4;
            dgvTongHopMuaH.ThemeStyle.ReadOnly = false;
            dgvTongHopMuaH.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvTongHopMuaH.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTongHopMuaH.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvTongHopMuaH.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvTongHopMuaH.ThemeStyle.RowsStyle.Height = 25;
            dgvTongHopMuaH.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvTongHopMuaH.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
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
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTongHopMuaH).EndInit();
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
        private Guna.UI2.WinForms.Guna2DataGridView dgvTongHopMuaH;
    }
}