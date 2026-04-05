namespace PrintingManagement
{
    partial class frmForgotPassword
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmForgotPassword));
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblEmail = new Label();
            txtEmail = new TextBox();
            btnSendOtp = new Guna.UI2.WinForms.Guna2Button();
            lblOtpStatus = new Label();
            lblOtp = new Label();
            txtOtp = new TextBox();
            lblNewPassword = new Label();
            txtNewPassword = new TextBox();
            lblConfirmPassword = new Label();
            txtConfirmPassword = new TextBox();
            btnReset = new Guna.UI2.WinForms.Guna2Button();
            lblResetStatus = new Label();
            btnClose = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(192, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Quên Mật Khẩu";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(30, 60);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(353, 17);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Nhap email cua ban de nhan ma xac nhan dat lai mat khau.";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblEmail.Location = new Point(30, 100);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            txtEmail.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtEmail.Location = new Point(30, 125);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(360, 27);
            txtEmail.TabIndex = 3;
            // 
            // btnSendOtp
            // 
            btnSendOtp.CustomizableEdges = customizableEdges1;
            btnSendOtp.DisabledState.BorderColor = Color.DarkGray;
            btnSendOtp.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSendOtp.FillColor = Color.Black;
            btnSendOtp.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnSendOtp.ForeColor = Color.White;
            btnSendOtp.Location = new Point(30, 165);
            btnSendOtp.Name = "btnSendOtp";
            btnSendOtp.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnSendOtp.Size = new Size(360, 40);
            btnSendOtp.TabIndex = 4;
            btnSendOtp.Text = "Gửi mã";
            btnSendOtp.Click += btnSendOtp_Click;
            // 
            // lblOtpStatus
            // 
            lblOtpStatus.AutoSize = true;
            lblOtpStatus.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblOtpStatus.Location = new Point(30, 215);
            lblOtpStatus.Name = "lblOtpStatus";
            lblOtpStatus.Size = new Size(0, 17);
            lblOtpStatus.TabIndex = 5;
            // 
            // lblOtp
            // 
            lblOtp.AutoSize = true;
            lblOtp.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblOtp.Location = new Point(30, 250);
            lblOtp.Name = "lblOtp";
            lblOtp.Size = new Size(36, 20);
            lblOtp.TabIndex = 6;
            lblOtp.Text = "OTP";
            // 
            // txtOtp
            // 
            txtOtp.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            txtOtp.Location = new Point(30, 275);
            txtOtp.MaxLength = 6;
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(200, 32);
            txtOtp.TabIndex = 7;
            // 
            // lblNewPassword
            // 
            lblNewPassword.AutoSize = true;
            lblNewPassword.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblNewPassword.Location = new Point(30, 320);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(106, 20);
            lblNewPassword.TabIndex = 8;
            lblNewPassword.Text = "Mật Khẩu Mới";
            // 
            // txtNewPassword
            // 
            txtNewPassword.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtNewPassword.Location = new Point(30, 345);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '*';
            txtNewPassword.Size = new Size(360, 27);
            txtNewPassword.TabIndex = 9;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblConfirmPassword.Location = new Point(30, 380);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(145, 20);
            lblConfirmPassword.TabIndex = 10;
            lblConfirmPassword.Text = "Xác Nhận Mật Khẩu";
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            txtConfirmPassword.Location = new Point(30, 405);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PasswordChar = '*';
            txtConfirmPassword.Size = new Size(360, 27);
            txtConfirmPassword.TabIndex = 11;
            // 
            // btnReset
            // 
            btnReset.CustomizableEdges = customizableEdges3;
            btnReset.DisabledState.BorderColor = Color.DarkGray;
            btnReset.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnReset.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(30, 445);
            btnReset.Name = "btnReset";
            btnReset.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnReset.Size = new Size(360, 40);
            btnReset.TabIndex = 12;
            btnReset.Text = "Đặt Lại Mật Khẩu";
            btnReset.Click += btnReset_Click;
            // 
            // lblResetStatus
            // 
            lblResetStatus.AutoSize = true;
            lblResetStatus.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblResetStatus.Location = new Point(30, 495);
            lblResetStatus.Name = "lblResetStatus";
            lblResetStatus.Size = new Size(0, 17);
            lblResetStatus.TabIndex = 13;
            // 
            // btnClose
            // 
            btnClose.CustomizableEdges = customizableEdges5;
            btnClose.FillColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnClose.ForeColor = Color.Gray;
            btnClose.Location = new Point(345, 20);
            btnClose.Name = "btnClose";
            btnClose.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnClose.Size = new Size(45, 35);
            btnClose.TabIndex = 14;
            btnClose.Text = "X";
            btnClose.Click += btnClose_Click;
            // 
            // frmForgotPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(420, 540);
            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(btnSendOtp);
            Controls.Add(lblOtpStatus);
            Controls.Add(lblOtp);
            Controls.Add(txtOtp);
            Controls.Add(lblNewPassword);
            Controls.Add(txtNewPassword);
            Controls.Add(lblConfirmPassword);
            Controls.Add(txtConfirmPassword);
            Controls.Add(btnReset);
            Controls.Add(lblResetStatus);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmForgotPassword";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2Button btnSendOtp;
        private System.Windows.Forms.Label lblOtpStatus;
        private System.Windows.Forms.Label lblOtp;
        private System.Windows.Forms.TextBox txtOtp;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private Guna.UI2.WinForms.Guna2Button btnReset;
        private System.Windows.Forms.Label lblResetStatus;
        private Guna.UI2.WinForms.Guna2Button btnClose;
    }
}
