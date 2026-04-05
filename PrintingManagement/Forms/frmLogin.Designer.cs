namespace PrintingManagement
{
    partial class frmLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            pnlMain = new Panel();
            pnlRight = new Panel();
            btnLogin = new Guna.UI2.WinForms.Guna2GradientButton();
            guna2GroupBox1 = new Guna.UI2.WinForms.Guna2GroupBox();
            rdoDirector = new Guna.UI2.WinForms.Guna2RadioButton();
            rdoWarehouse = new Guna.UI2.WinForms.Guna2RadioButton();
            rdoProduction = new Guna.UI2.WinForms.Guna2RadioButton();
            rdoAccountant = new Guna.UI2.WinForms.Guna2RadioButton();
            rdoSales = new Guna.UI2.WinForms.Guna2RadioButton();
            rdoAdmin = new Guna.UI2.WinForms.Guna2RadioButton();
            lnkForgotPassword = new LinkLabel();
            chkRememberMe = new CheckBox();
            ptrPasswordd = new PictureBox();
            txtPassword = new TextBox();
            lblPassword = new Label();
            ptrUsername = new PictureBox();
            txtUsername = new TextBox();
            lbltendangnhap = new Label();
            lblchaomung = new Label();
            lbldangnhap = new Label();
            pnlLeft = new Panel();
            lbltich5 = new Label();
            ptrtich5 = new PictureBox();
            lbltich4 = new Label();
            ptrtich4 = new PictureBox();
            lbltich3 = new Label();
            ptrtich3 = new PictureBox();
            lbltich2 = new Label();
            ptrtich2 = new PictureBox();
            lbltich1 = new Label();
            ptrtich1 = new PictureBox();
            lblgiaiphap = new Label();
            lblTitle = new Label();
            ptrlogoteam = new PictureBox();
            pnlMain.SuspendLayout();
            pnlRight.SuspendLayout();
            guna2GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrPasswordd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrUsername).BeginInit();
            pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptrtich5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptrlogoteam).BeginInit();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(pnlRight);
            pnlMain.Controls.Add(pnlLeft);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(800, 603);
            pnlMain.TabIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(btnLogin);
            pnlRight.Controls.Add(guna2GroupBox1);
            pnlRight.Controls.Add(lnkForgotPassword);
            pnlRight.Controls.Add(chkRememberMe);
            pnlRight.Controls.Add(ptrPasswordd);
            pnlRight.Controls.Add(txtPassword);
            pnlRight.Controls.Add(lblPassword);
            pnlRight.Controls.Add(ptrUsername);
            pnlRight.Controls.Add(txtUsername);
            pnlRight.Controls.Add(lbltendangnhap);
            pnlRight.Controls.Add(lblchaomung);
            pnlRight.Controls.Add(lbldangnhap);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.ForeColor = SystemColors.ControlText;
            pnlRight.Location = new Point(400, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(400, 603);
            pnlRight.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.CustomizableEdges = customizableEdges1;
            btnLogin.DisabledState.BorderColor = Color.DarkGray;
            btnLogin.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLogin.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLogin.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            btnLogin.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLogin.FillColor = Color.Black;
            btnLogin.Font = new Font("Segoe UI", 14.25F);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(33, 377);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLogin.Size = new Size(339, 44);
            btnLogin.TabIndex = 13;
            btnLogin.Text = "Đăng Nhập";
            btnLogin.Click += btnLogin_Click;
            // 
            // guna2GroupBox1
            // 
            guna2GroupBox1.Controls.Add(rdoDirector);
            guna2GroupBox1.Controls.Add(rdoWarehouse);
            guna2GroupBox1.Controls.Add(rdoProduction);
            guna2GroupBox1.Controls.Add(rdoAccountant);
            guna2GroupBox1.Controls.Add(rdoSales);
            guna2GroupBox1.Controls.Add(rdoAdmin);
            guna2GroupBox1.CustomizableEdges = customizableEdges3;
            guna2GroupBox1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2GroupBox1.ForeColor = Color.FromArgb(125, 137, 149);
            guna2GroupBox1.Location = new Point(0, 427);
            guna2GroupBox1.Name = "guna2GroupBox1";
            guna2GroupBox1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2GroupBox1.Size = new Size(400, 176);
            guna2GroupBox1.TabIndex = 12;
            guna2GroupBox1.Text = "Vai trò";
            // 
            // rdoDirector
            // 
            rdoDirector.AutoSize = true;
            rdoDirector.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoDirector.CheckedState.BorderThickness = 0;
            rdoDirector.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoDirector.CheckedState.InnerColor = Color.White;
            rdoDirector.CheckedState.InnerOffset = -4;
            rdoDirector.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            rdoDirector.Location = new Point(217, 97);
            rdoDirector.Name = "rdoDirector";
            rdoDirector.Size = new Size(83, 21);
            rdoDirector.TabIndex = 16;
            rdoDirector.Text = "Giám đốc";
            rdoDirector.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoDirector.UncheckedState.BorderThickness = 2;
            rdoDirector.UncheckedState.FillColor = Color.Transparent;
            rdoDirector.UncheckedState.InnerColor = Color.Transparent;
            // 
            // rdoWarehouse
            // 
            rdoWarehouse.AutoSize = true;
            rdoWarehouse.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoWarehouse.CheckedState.BorderThickness = 0;
            rdoWarehouse.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoWarehouse.CheckedState.InnerColor = Color.White;
            rdoWarehouse.CheckedState.InnerOffset = -4;
            rdoWarehouse.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            rdoWarehouse.Location = new Point(217, 70);
            rdoWarehouse.Name = "rdoWarehouse";
            rdoWarehouse.Size = new Size(76, 21);
            rdoWarehouse.TabIndex = 15;
            rdoWarehouse.Text = "Thủ kho";
            rdoWarehouse.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoWarehouse.UncheckedState.BorderThickness = 2;
            rdoWarehouse.UncheckedState.FillColor = Color.Transparent;
            rdoWarehouse.UncheckedState.InnerColor = Color.Transparent;
            // 
            // rdoProduction
            // 
            rdoProduction.AutoSize = true;
            rdoProduction.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoProduction.CheckedState.BorderThickness = 0;
            rdoProduction.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoProduction.CheckedState.InnerColor = Color.White;
            rdoProduction.CheckedState.InnerOffset = -4;
            rdoProduction.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoProduction.Location = new Point(217, 43);
            rdoProduction.Name = "rdoProduction";
            rdoProduction.Size = new Size(79, 21);
            rdoProduction.TabIndex = 14;
            rdoProduction.Text = "Sản xuất";
            rdoProduction.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoProduction.UncheckedState.BorderThickness = 2;
            rdoProduction.UncheckedState.FillColor = Color.Transparent;
            rdoProduction.UncheckedState.InnerColor = Color.Transparent;
            // 
            // rdoAccountant
            // 
            rdoAccountant.AutoSize = true;
            rdoAccountant.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoAccountant.CheckedState.BorderThickness = 0;
            rdoAccountant.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoAccountant.CheckedState.InnerColor = Color.White;
            rdoAccountant.CheckedState.InnerOffset = -4;
            rdoAccountant.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            rdoAccountant.Location = new Point(40, 97);
            rdoAccountant.Name = "rdoAccountant";
            rdoAccountant.Size = new Size(73, 21);
            rdoAccountant.TabIndex = 13;
            rdoAccountant.Text = "Kế toán";
            rdoAccountant.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoAccountant.UncheckedState.BorderThickness = 2;
            rdoAccountant.UncheckedState.FillColor = Color.Transparent;
            rdoAccountant.UncheckedState.InnerColor = Color.Transparent;
            // 
            // rdoSales
            // 
            rdoSales.AutoSize = true;
            rdoSales.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoSales.CheckedState.BorderThickness = 0;
            rdoSales.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoSales.CheckedState.InnerColor = Color.White;
            rdoSales.CheckedState.InnerOffset = -4;
            rdoSales.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            rdoSales.Location = new Point(40, 70);
            rdoSales.Name = "rdoSales";
            rdoSales.Size = new Size(96, 21);
            rdoSales.TabIndex = 12;
            rdoSales.Text = "Kinh doanh";
            rdoSales.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoSales.UncheckedState.BorderThickness = 2;
            rdoSales.UncheckedState.FillColor = Color.Transparent;
            rdoSales.UncheckedState.InnerColor = Color.Transparent;
            // 
            // rdoAdmin
            // 
            rdoAdmin.AutoSize = true;
            rdoAdmin.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            rdoAdmin.CheckedState.BorderThickness = 0;
            rdoAdmin.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            rdoAdmin.CheckedState.InnerColor = Color.White;
            rdoAdmin.CheckedState.InnerOffset = -4;
            rdoAdmin.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoAdmin.Location = new Point(40, 43);
            rdoAdmin.Name = "rdoAdmin";
            rdoAdmin.Size = new Size(66, 21);
            rdoAdmin.TabIndex = 11;
            rdoAdmin.Text = "Admin";
            rdoAdmin.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            rdoAdmin.UncheckedState.BorderThickness = 2;
            rdoAdmin.UncheckedState.FillColor = Color.Transparent;
            rdoAdmin.UncheckedState.InnerColor = Color.Transparent;
            // 
            // lnkForgotPassword
            // 
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lnkForgotPassword.Location = new Point(268, 318);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new Size(102, 17);
            lnkForgotPassword.TabIndex = 9;
            lnkForgotPassword.TabStop = true;
            lnkForgotPassword.Text = "Quên mật khẩu?";
            lnkForgotPassword.LinkClicked += lnkForgotPassword_LinkClicked;
            // 
            // chkRememberMe
            // 
            chkRememberMe.AutoSize = true;
            chkRememberMe.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkRememberMe.Location = new Point(33, 318);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Size = new Size(139, 21);
            chkRememberMe.TabIndex = 8;
            chkRememberMe.Text = "Ghi nhớ đăng nhập";
            chkRememberMe.UseVisualStyleBackColor = true;
            // 
            // ptrPasswordd
            // 
            ptrPasswordd.Image = Properties.Resources.okhoa;
            ptrPasswordd.Location = new Point(33, 265);
            ptrPasswordd.Name = "ptrPasswordd";
            ptrPasswordd.Size = new Size(27, 26);
            ptrPasswordd.SizeMode = PictureBoxSizeMode.Zoom;
            ptrPasswordd.TabIndex = 7;
            ptrPasswordd.TabStop = false;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(66, 268);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(306, 23);
            txtPassword.TabIndex = 6;
            txtPassword.TabStop = false;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Enter += txtPassword_Enter;
            txtPassword.Leave += txtPassword_Leave;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPassword.Location = new Point(32, 232);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(74, 20);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Mật khẩu";
            // 
            // ptrUsername
            // 
            ptrUsername.Image = Properties.Resources.ngdung;
            ptrUsername.Location = new Point(32, 169);
            ptrUsername.Name = "ptrUsername";
            ptrUsername.Size = new Size(27, 26);
            ptrUsername.SizeMode = PictureBoxSizeMode.Zoom;
            ptrUsername.TabIndex = 4;
            ptrUsername.TabStop = false;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(65, 172);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(307, 23);
            txtUsername.TabIndex = 3;
            txtUsername.TabStop = false;
            txtUsername.Enter += txtUsername_Enter;
            txtUsername.Leave += txtUsername_Leave;
            // 
            // lbltendangnhap
            // 
            lbltendangnhap.AutoSize = true;
            lbltendangnhap.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbltendangnhap.Location = new Point(31, 136);
            lbltendangnhap.Name = "lbltendangnhap";
            lbltendangnhap.Size = new Size(111, 20);
            lbltendangnhap.TabIndex = 2;
            lbltendangnhap.Text = "Tên đăng nhập / Email";
            // 
            // lblchaomung
            // 
            lblchaomung.AutoSize = true;
            lblchaomung.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblchaomung.Location = new Point(31, 88);
            lblchaomung.Name = "lblchaomung";
            lblchaomung.Size = new Size(364, 17);
            lblchaomung.TabIndex = 1;
            lblchaomung.Text = "Chào mừng bạn quay trở lại! Vui lòng đăng nhập để tiếp tục.";
            // 
            // lbldangnhap
            // 
            lbldangnhap.AutoSize = true;
            lbldangnhap.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbldangnhap.Location = new Point(27, 40);
            lbldangnhap.Name = "lbldangnhap";
            lbldangnhap.Size = new Size(139, 32);
            lbldangnhap.TabIndex = 0;
            lbldangnhap.Text = "Đăng nhập";
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(lbltich5);
            pnlLeft.Controls.Add(ptrtich5);
            pnlLeft.Controls.Add(lbltich4);
            pnlLeft.Controls.Add(ptrtich4);
            pnlLeft.Controls.Add(lbltich3);
            pnlLeft.Controls.Add(ptrtich3);
            pnlLeft.Controls.Add(lbltich2);
            pnlLeft.Controls.Add(ptrtich2);
            pnlLeft.Controls.Add(lbltich1);
            pnlLeft.Controls.Add(ptrtich1);
            pnlLeft.Controls.Add(lblgiaiphap);
            pnlLeft.Controls.Add(lblTitle);
            pnlLeft.Controls.Add(ptrlogoteam);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(400, 603);
            pnlLeft.TabIndex = 0;
            // 
            // lbltich5
            // 
            lbltich5.AutoSize = true;
            lbltich5.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltich5.Location = new Point(56, 484);
            lbltich5.Name = "lbltich5";
            lbltich5.Size = new Size(179, 21);
            lbltich5.TabIndex = 12;
            lbltich5.Text = "Báo cáo thống kê chi tiết";
            // 
            // ptrtich5
            // 
            ptrtich5.Image = Properties.Resources.tích;
            ptrtich5.Location = new Point(29, 481);
            ptrtich5.Name = "ptrtich5";
            ptrtich5.Size = new Size(27, 26);
            ptrtich5.SizeMode = PictureBoxSizeMode.Zoom;
            ptrtich5.TabIndex = 11;
            ptrtich5.TabStop = false;
            // 
            // lbltich4
            // 
            lbltich4.AutoSize = true;
            lbltich4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltich4.Location = new Point(56, 441);
            lbltich4.Name = "lbltich4";
            lbltich4.Size = new Size(226, 21);
            lbltich4.TabIndex = 10;
            lbltich4.Text = "Theo dõi công nợ và doanh thu";
            // 
            // ptrtich4
            // 
            ptrtich4.Image = Properties.Resources.tích;
            ptrtich4.Location = new Point(29, 438);
            ptrtich4.Name = "ptrtich4";
            ptrtich4.Size = new Size(27, 26);
            ptrtich4.SizeMode = PictureBoxSizeMode.Zoom;
            ptrtich4.TabIndex = 9;
            ptrtich4.TabStop = false;
            // 
            // lbltich3
            // 
            lbltich3.AutoSize = true;
            lbltich3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltich3.Location = new Point(56, 403);
            lbltich3.Name = "lbltich3";
            lbltich3.Size = new Size(187, 21);
            lbltich3.TabIndex = 8;
            lbltich3.Text = "Quản lý kho vận hiệu quả";
            // 
            // ptrtich3
            // 
            ptrtich3.Image = Properties.Resources.tích;
            ptrtich3.Location = new Point(29, 400);
            ptrtich3.Name = "ptrtich3";
            ptrtich3.Size = new Size(27, 26);
            ptrtich3.SizeMode = PictureBoxSizeMode.Zoom;
            ptrtich3.TabIndex = 7;
            ptrtich3.TabStop = false;
            // 
            // lbltich2
            // 
            lbltich2.AutoSize = true;
            lbltich2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltich2.Location = new Point(56, 362);
            lbltich2.Name = "lbltich2";
            lbltich2.Size = new Size(234, 21);
            lbltich2.TabIndex = 6;
            lbltich2.Text = "Quản lý báo giá và lệnh sản xuất";
            // 
            // ptrtich2
            // 
            ptrtich2.Image = Properties.Resources.tích;
            ptrtich2.Location = new Point(29, 359);
            ptrtich2.Name = "ptrtich2";
            ptrtich2.Size = new Size(27, 26);
            ptrtich2.SizeMode = PictureBoxSizeMode.Zoom;
            ptrtich2.TabIndex = 5;
            ptrtich2.TabStop = false;
            // 
            // lbltich1
            // 
            lbltich1.AutoSize = true;
            lbltich1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbltich1.Location = new Point(56, 320);
            lbltich1.Name = "lbltich1";
            lbltich1.Size = new Size(263, 21);
            lbltich1.TabIndex = 4;
            lbltich1.Text = "Tính giá sản phẩm tự động chính xác";
            // 
            // ptrtich1
            // 
            ptrtich1.Image = Properties.Resources.tích;
            ptrtich1.Location = new Point(29, 317);
            ptrtich1.Name = "ptrtich1";
            ptrtich1.Size = new Size(27, 26);
            ptrtich1.SizeMode = PictureBoxSizeMode.Zoom;
            ptrtich1.TabIndex = 3;
            ptrtich1.TabStop = false;
            // 
            // lblgiaiphap
            // 
            lblgiaiphap.AutoSize = true;
            lblgiaiphap.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblgiaiphap.Location = new Point(16, 273);
            lblgiaiphap.Name = "lblgiaiphap";
            lblgiaiphap.Size = new Size(363, 20);
            lblgiaiphap.TabIndex = 2;
            lblgiaiphap.Text = "Nhom1 - Giải pháp toàn diện cho doanh nghiệp in ấn";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(16, 230);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(280, 32);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Hệ thống Quản lý In ấn";
            // 
            // ptrlogoteam
            // 
            ptrlogoteam.Image = Properties.Resources.logo_nhóm_removebg_preview;
            ptrlogoteam.InitialImage = (Image)resources.GetObject("ptrlogoteam.InitialImage");
            ptrlogoteam.Location = new Point(3, 40);
            ptrlogoteam.Name = "ptrlogoteam";
            ptrlogoteam.Size = new Size(386, 175);
            ptrlogoteam.SizeMode = PictureBoxSizeMode.Zoom;
            ptrlogoteam.TabIndex = 0;
            ptrlogoteam.TabStop = false;
            ptrlogoteam.Click += ptrlogoteam_Click;
            // 
            // frmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(800, 603);
            Controls.Add(pnlMain);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmLogin";
            Text = "Đăng Nhập";
            Load += frmLogin_Load;
            pnlMain.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            guna2GroupBox1.ResumeLayout(false);
            guna2GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrPasswordd).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrUsername).EndInit();
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptrtich5).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich4).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich3).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrtich1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptrlogoteam).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMain;
        private Panel pnlRight;
        private Panel pnlLeft;
        private PictureBox ptrlogoteam;
        private Label lbltich1;
        private PictureBox ptrtich1;
        private Label lblgiaiphap;
        private Label lblTitle;
        private Label lbltich5;
        private PictureBox ptrtich5;
        private Label lbltich4;
        private PictureBox ptrtich4;
        private Label lbltich3;
        private PictureBox ptrtich3;
        private Label lbltich2;
        private PictureBox ptrtich2;
        private TextBox txtUsername;
        private Label lbltendangnhap;
        private Label lblchaomung;
        private Label lbldangnhap;
        private LinkLabel lnkForgotPassword;
        private CheckBox chkRememberMe;
        private PictureBox ptrPasswordd;
        private TextBox txtPassword;
        private Label lblPassword;
        private PictureBox ptrUsername;
        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox1;
        private Guna.UI2.WinForms.Guna2RadioButton rdoDirector;
        private Guna.UI2.WinForms.Guna2RadioButton rdoWarehouse;
        private Guna.UI2.WinForms.Guna2RadioButton rdoProduction;
        private Guna.UI2.WinForms.Guna2RadioButton rdoAccountant;
        private Guna.UI2.WinForms.Guna2RadioButton rdoSales;
        private Guna.UI2.WinForms.Guna2RadioButton rdoAdmin;
        private Guna.UI2.WinForms.Guna2GradientButton btnLogin;
    }
}
