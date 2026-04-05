namespace PrintingManagement
{
    partial class frmSelectSupplierMaterial
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2DataGridView dgvMaterials = null!;
        private Guna.UI2.WinForms.Guna2Button btnManualNew = null!;
        private Guna.UI2.WinForms.Guna2Button btnOK = null!;
        private Guna.UI2.WinForms.Guna2Button btnCancel = null!;
        private TextBox txtSearch = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;
        private Label lblSearch = null!;
        private Panel panel1 = null!;
        private Panel panel2 = null!;
        private Panel panelButtons = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvMaterials = new Guna.UI2.WinForms.Guna2DataGridView();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnManualNew = new Guna.UI2.WinForms.Guna2Button();
            this.btnOK = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).BeginInit();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 46);
            this.panel1.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(120, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Vật tư của NCC";
            //
            // panel2
            //
            this.panel2.Controls.Add(this.lblCount);
            this.panel2.Controls.Add(this.lblSearch);
            this.panel2.Controls.Add(this.txtSearch);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 46);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(700, 40);
            this.panel2.TabIndex = 1;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSearch.Location = new System.Drawing.Point(12, 11);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(60, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";
            //
            // txtSearch
            //
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtSearch.Location = new System.Drawing.Point(78, 8);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Mã / tên vật tư...";
            this.txtSearch.Size = new System.Drawing.Size(320, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // lblCount
            //
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCount.ForeColor = System.Drawing.Color.Gray;
            this.lblCount.Location = new System.Drawing.Point(580, 12);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(50, 15);
            this.lblCount.TabIndex = 2;
            this.lblCount.Text = "0 vật tư";
            //
            // dgvMaterials
            //
            this.dgvMaterials.AllowUserToAddRows = false;
            this.dgvMaterials.AllowUserToDeleteRows = false;
            this.dgvMaterials.AllowUserToResizeRows = false;
            this.dgvMaterials.BackgroundColor = System.Drawing.Color.White;
            this.dgvMaterials.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvMaterials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMaterials.Location = new System.Drawing.Point(0, 86);
            this.dgvMaterials.Name = "dgvMaterials";
            this.dgvMaterials.RowTemplate.Height = 36;
            this.dgvMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterials.Size = new System.Drawing.Size(700, 360);
            this.dgvMaterials.TabIndex = 2;
            this.dgvMaterials.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvMaterials_KeyDown);
            //
            // panelButtons
            //
            this.panelButtons.BackColor = System.Drawing.Color.White;
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnOK);
            this.panelButtons.Controls.Add(this.btnManualNew);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 448);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(12, 8, 12, 12);
            this.panelButtons.Size = new System.Drawing.Size(700, 56);
            this.panelButtons.TabIndex = 3;
            //
            // btnManualNew
            //
            this.btnManualNew.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnManualNew.BorderRadius = 8;
            this.btnManualNew.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnManualNew.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnManualNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnManualNew.ForeColor = System.Drawing.Color.White;
            this.btnManualNew.Location = new System.Drawing.Point(12, 8);
            this.btnManualNew.Name = "btnManualNew";
            this.btnManualNew.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnManualNew.Size = new System.Drawing.Size(280, 36);
            this.btnManualNew.TabIndex = 0;
            this.btnManualNew.Text = "Hàng mới (chưa trong danh mục)";
            this.btnManualNew.Click += new System.EventHandler(this.btnManualNew_Click);
            //
            // btnOK
            //
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOK.BorderRadius = 8;
            this.btnOK.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnOK.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(304, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnOK.Size = new System.Drawing.Size(100, 36);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Chọn";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.BorderRadius = 8;
            this.btnCancel.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnCancel.FillColor = System.Drawing.Color.Gray;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(412, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmSelectSupplierMaterial
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(700, 504);
            this.Controls.Add(this.dgvMaterials);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectSupplierMaterial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn vật tư từ danh mục NCC";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterials)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
