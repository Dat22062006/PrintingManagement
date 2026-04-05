namespace PrintingManagement
{
    partial class frmCustomerManagement
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCustomerManagement));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelhoadongtg = new Panel();
            btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            txtSearch = new TextBox();
            pictureBox7 = new PictureBox();
            labeltrangchubanhanghoadon = new Label();
            label1 = new Label();
            labellhoadongtgt = new Label();
            ptrbtrangchu = new PictureBox();
            panel1 = new Panel();
            dgvCustomers = new Guna.UI2.WinForms.Guna2DataGridView();
            btnAddNew = new Guna.UI2.WinForms.Guna2Button();
            panelhoadongtg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).BeginInit();
            SuspendLayout();
            // 
            // panelhoadongtg
            // 
            panelhoadongtg.Controls.Add(btnAddNew);
            panelhoadongtg.Controls.Add(btnRefresh);
            panelhoadongtg.Controls.Add(txtSearch);
            panelhoadongtg.Controls.Add(pictureBox7);
            panelhoadongtg.Controls.Add(labeltrangchubanhanghoadon);
            panelhoadongtg.Controls.Add(label1);
            panelhoadongtg.Controls.Add(labellhoadongtgt);
            panelhoadongtg.Controls.Add(ptrbtrangchu);
            panelhoadongtg.Dock = DockStyle.Top;
            panelhoadongtg.Location = new Point(0, 0);
            panelhoadongtg.Name = "panelhoadongtg";
            panelhoadongtg.Size = new Size(1058, 168);
            panelhoadongtg.TabIndex = 6;
            // 
            // btnRefresh
            // 
            btnRefresh.BorderRadius = 10;
            btnRefresh.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnRefresh.CustomizableEdges = customizableEdges3;
            btnRefresh.DisabledState.BorderColor = Color.DarkGray;
            btnRefresh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRefresh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRefresh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRefresh.FillColor = Color.FromArgb(255, 128, 255);
            btnRefresh.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.ImageOffset = new Point(-1, 1);
            btnRefresh.Location = new Point(867, 126);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnRefresh.Size = new Size(151, 31);
            btnRefresh.TabIndex = 261;
            btnRefresh.Text = "Làm Mới";
            btnRefresh.TextOffset = new Point(-2, 0);
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtSearch
            // 
            txtSearch.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            txtSearch.Location = new Point(3, 128);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Tìm theo mã,tên khách hàng,....";
            txtSearch.Size = new Size(366, 29);
            txtSearch.TabIndex = 260;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // pictureBox7
            // 
            pictureBox7.ErrorImage = (Image)resources.GetObject("pictureBox7.ErrorImage");
            pictureBox7.Image = (Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.InitialImage = (Image)resources.GetObject("pictureBox7.InitialImage");
            pictureBox7.Location = new Point(3, 87);
            pictureBox7.Margin = new Padding(3, 2, 3, 2);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(34, 26);
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.TabIndex = 258;
            pictureBox7.TabStop = false;
            // 
            // labeltrangchubanhanghoadon
            // 
            labeltrangchubanhanghoadon.AutoSize = true;
            labeltrangchubanhanghoadon.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labeltrangchubanhanghoadon.Location = new Point(6, 55);
            labeltrangchubanhanghoadon.Name = "labeltrangchubanhanghoadon";
            labeltrangchubanhanghoadon.Size = new Size(359, 20);
            labeltrangchubanhanghoadon.TabIndex = 2;
            labeltrangchubanhanghoadon.Text = "Trang chủ / Tính giá và Báo giá / Quản lý Khách hàng";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(39, 90);
            label1.Name = "label1";
            label1.Size = new Size(182, 21);
            label1.TabIndex = 257;
            label1.Text = "Danh sách khách hàng";
            // 
            // labellhoadongtgt
            // 
            labellhoadongtgt.AutoSize = true;
            labellhoadongtgt.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            labellhoadongtgt.Location = new Point(56, 11);
            labellhoadongtgt.Name = "labellhoadongtgt";
            labellhoadongtgt.Size = new Size(287, 32);
            labellhoadongtgt.TabIndex = 1;
            labellhoadongtgt.Text = "QUẢN LÝ KHÁCH HÀNG";
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
            // panel1
            // 
            panel1.Controls.Add(dgvCustomers);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 168);
            panel1.Name = "panel1";
            panel1.Size = new Size(1058, 393);
            panel1.TabIndex = 7;
            // 
            // dgvCustomers
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvCustomers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvCustomers.ColumnHeadersHeight = 4;
            dgvCustomers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvCustomers.DefaultCellStyle = dataGridViewCellStyle3;
            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.GridColor = Color.FromArgb(231, 229, 255);
            dgvCustomers.Location = new Point(0, 0);
            dgvCustomers.Name = "dgvCustomers";
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.RowTemplate.Height = 25;
            dgvCustomers.Size = new Size(1058, 393);
            dgvCustomers.TabIndex = 0;
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvCustomers.ThemeStyle.BackColor = Color.White;
            dgvCustomers.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvCustomers.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvCustomers.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCustomers.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvCustomers.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvCustomers.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvCustomers.ThemeStyle.HeaderStyle.Height = 4;
            dgvCustomers.ThemeStyle.ReadOnly = false;
            dgvCustomers.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvCustomers.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCustomers.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvCustomers.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvCustomers.ThemeStyle.RowsStyle.Height = 25;
            dgvCustomers.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvCustomers.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // btnAddNew
            // 
            btnAddNew.BorderRadius = 10;
            btnAddNew.CustomImages.ImageAlign = HorizontalAlignment.Left;
            btnAddNew.CustomizableEdges = customizableEdges1;
            btnAddNew.DisabledState.BorderColor = Color.DarkGray;
            btnAddNew.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAddNew.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAddNew.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAddNew.FillColor = Color.FromArgb(192, 192, 0);
            btnAddNew.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnAddNew.ForeColor = Color.White;
            btnAddNew.ImageOffset = new Point(-1, 1);
            btnAddNew.Location = new Point(767, 126);
            btnAddNew.Name = "btnAddNew";
            btnAddNew.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAddNew.Size = new Size(94, 31);
            btnAddNew.TabIndex = 262;
            btnAddNew.Text = "Thêm";
            btnAddNew.TextOffset = new Point(-2, 0);
            btnAddNew.Click += btnAddNew_Click;
            // 
            // frmCustomerManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1058, 561);
            Controls.Add(panel1);
            Controls.Add(panelhoadongtg);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmCustomerManagement";
            Load += frmCustomerManagement_Load;
            panelhoadongtg.ResumeLayout(false);
            panelhoadongtg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrbtrangchu).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelhoadongtg;
        private Label labeltrangchubanhanghoadon;
        private Label labellhoadongtgt;
        private PictureBox ptrbtrangchu;
        private Panel panel1;
        private PictureBox pictureBox7;
        private Label label1;
        private TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2DataGridView dgvCustomers;
        private Guna.UI2.WinForms.Guna2Button btnAddNew;
    }
}