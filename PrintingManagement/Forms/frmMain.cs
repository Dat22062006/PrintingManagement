using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace PrintingManagement
{
    public partial class frmMain : Form
    {
        // Chặn maximize hoàn toàn (double-click title bar, taskbar, etc.)
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112 && m.WParam.ToInt32() == 0xF030) // SC_MAXIMIZE
                return;
            base.WndProc(ref m);
        }

        System.Windows.Forms.ContextMenuStrip currentMenu = null;

        // NAV styling
        private readonly Color NavBarDark = Color.FromArgb(24, 32, 40);
        private readonly Color NavItemHover = Color.FromArgb(43, 120, 211);   // sáng hơn khi hover
        private readonly Color NavItemActive = Color.FromArgb(28, 98, 205);   // màu active
        private readonly Color NavLabelDefault = Color.WhiteSmoke;
        private Panel activeNavPanel = null;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED - giúp paint mượt, giảm flicker
                return cp;
            }
        }
        public frmMain()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Ensure nav bar header colors are consistent (Designer may already set some)
            try
            {
                pnlcha.BackColor = NavBarDark;
                // set default label colors
                lbltrangchu.ForeColor = NavLabelDefault;
                lbltinhgiabaogia.ForeColor = NavLabelDefault;
                lblmuahang.ForeColor = NavLabelDefault;
                lblsanxuat.ForeColor = NavLabelDefault;
                lblkho.ForeColor = NavLabelDefault;
                lblbanhang.ForeColor = NavLabelDefault;
                lblbaocao.ForeColor = NavLabelDefault;
            }
            catch
            {
                // ignore if controls not yet created in designer context
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cmsBaoGia.AutoClose = true;
            cmsmuahang.AutoClose = true;
            cmssanxuat.AutoClose = true;
            cmskho.AutoClose = true;
            cmsbanhang.AutoClose = true;
            cmsbaocao.AutoClose = true;
            AttachMouseEvents(pnltinhgiabaogia);
            AttachMouseEvents(pnlmuahang);
            AttachMouseEvents(pnlsanxuat);
            AttachMouseEvents(pnlkho);
            AttachMouseEvents(pnlbanhang);
            AttachMouseEvents(pnlbaocao);
            //mở dashboard
            frmDashBoard dash = new frmDashBoard(this);
            LoadForm(dash);

            // Make Trang chủ active by default (if visible)
            if (pnltrangchu.Visible)
                SetActiveNavPanel(pnltrangchu);

            foreach (Control c in pnltrangchu.Controls)
            {
                c.Click += OpenDashboard;
            }

            pnltrangchu.Click += OpenDashboard;
            AddClickEvent(pnltrangchu);
            //
            //designer
            pnltinhgiabaogia.MouseEnter += Menu_MouseEnter;
            pnltinhgiabaogia.MouseLeave += Menu_MouseLeave;
            pnlmuahang.MouseEnter += Menu_MouseEnter;
            pnlmuahang.MouseLeave += Menu_MouseLeave;

            pnlsanxuat.MouseEnter += Menu_MouseEnter;
            pnlsanxuat.MouseLeave += Menu_MouseLeave;

            pnlkho.MouseEnter += Menu_MouseEnter;
            pnlkho.MouseLeave += Menu_MouseLeave;

            pnlbanhang.MouseEnter += Menu_MouseEnter;
            pnlbanhang.MouseLeave += Menu_MouseLeave;

            pnlbaocao.MouseEnter += Menu_MouseEnter;
            pnlbaocao.MouseLeave += Menu_MouseLeave;
            pnltrangchu.MouseEnter += Menu_MouseEnter;
            pnltrangchu.MouseLeave += Menu_MouseLeave;
            AttachHover(pnltinhgiabaogia);
            AttachHover(pnlmuahang);
            AttachHover(pnlsanxuat);
            AttachHover(pnlkho);
            AttachHover(pnlbanhang);
            AttachHover(pnlbaocao);
            AttachHover(pnltrangchu);
            //close fix delay header
            cmsBaoGia.Closed += Menu_Closed;
            cmsmuahang.Closed += Menu_Closed;
            cmssanxuat.Closed += Menu_Closed;
            cmskho.Closed += Menu_Closed;
            cmsbanhang.Closed += Menu_Closed;
            cmsbaocao.Closed += Menu_Closed;
            //test debug
            //role kinh doanh
            if (string.IsNullOrEmpty(CurrentUser.VaiTro))
            {
                new frmLogin().Show();
                this.Close();
                return;
            }

            // Ẩn hết tất cả panel + menu item nhạy cảm trước (an toàn nhất)
            pnltinhgiabaogia.Visible = false;
            pnlbanhang.Visible = false;
            pnlmuahang.Visible = false;
            pnlkho.Visible = false;
            pnlsanxuat.Visible = false;

            // Bật quyền theo vai trò
            switch (CurrentUser.VaiTro)
            {
                case "Admin":
                    // Admin thấy hết
                    pnltinhgiabaogia.Visible = true;
                    pnlbanhang.Visible = true;
                    pnlmuahang.Visible = true;
                    pnlkho.Visible = true;
                    pnlsanxuat.Visible = true;
                    pnltrangchu.Visible = true;
                    break;

                case "Kinh doanh":

                    pnltinhgiabaogia.Visible = true;
                    pnlbanhang.Visible = true;
                    cậpNhậpThôngTinKháchToolStripMenuItem.Visible = true;
                    phiếuGiaoHàngToolStripMenuItem.Visible = true;

                    break;

                case "Kế toán":
                    pnlmuahang.Visible = true;
                    // Ẩn các chức năng không thuộc kế toán (nếu cần)
                    nhậpKhoToolStripMenuItem.Visible = false;
                    break;

                case "Thủ kho":
                    pnlkho.Visible = true;
                    // Ẩn các chức năng mua hàng/thanh toán không thuộc thủ kho
                    pnlmuahang.Visible = false;
                    tạoĐơnHàngToolStripMenuItem.Visible = false;
                    thanhToánNCCToolStripMenuItem.Visible = false;
                    báoCáoMuaHàngToolStripMenuItem.Visible = false;
                    break;

                case "Sản xuất":
                    pnlsanxuat.Visible = true;
                    break;

                case "Giám đốc":
                    pnltinhgiabaogia.Visible = true;
                    pnlbanhang.Visible = true;
                    pnlmuahang.Visible = true;
                    pnlkho.Visible = true;
                    pnlsanxuat.Visible = true;
                    pnltrangchu.Visible = true;
                    //sau này update button k cho sửa
                    break;

                default:
                    MessageBox.Show($"Chào {CurrentUser.HoTen}! Vai trò {CurrentUser.VaiTro} chưa được cấp quyền truy cập đầy đủ.",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }

            // Nếu có panel chung cho tất cả (ví dụ báo cáo, dashboard)
            // panelBaoCao.Visible = true;
            // panelDashboard.Visible = true;

        }
        //hàm fix delay header
        void Menu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            currentMenu = null;
        }
        //hàm designer màu
        void AttachHover(Control parent)
        {
            parent.MouseEnter += Menu_MouseEnter;
            parent.MouseLeave += Menu_MouseLeave;

            foreach (Control c in parent.Controls)
            {
                c.MouseEnter += Menu_MouseEnter;
                c.MouseLeave += Menu_MouseLeave;
            }
        }
        void Menu_MouseEnter(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Panel pnl = GetParentPanel(c);

            if (pnl != null)
            {
                // don't override active panel color
                if (activeNavPanel != null && pnl == activeNavPanel) return;

                pnl.BackColor = NavItemHover;
                // set label to white for contrast
                SetPanelLabelForeColor(pnl, Color.White);
            }
        }

        void Menu_MouseLeave(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Panel pnl = GetParentPanel(c);

            if (pnl != null)
            {
                // if panel is active keep active color
                if (activeNavPanel != null && pnl == activeNavPanel)
                {
                    pnl.BackColor = NavItemActive;
                    SetPanelLabelForeColor(pnl, Color.White);
                    return;
                }

                // otherwise revert to nav dark and default label color
                pnl.BackColor = NavBarDark;
                SetPanelLabelForeColor(pnl, NavLabelDefault);
            }
        }

        // Set a panel as active (called when opening a page)
        void SetActiveNavPanel(Panel pnl)
        {
            try
            {
                if (activeNavPanel != null && activeNavPanel != pnl)
                {
                    // reset previous
                    activeNavPanel.BackColor = NavBarDark;
                    SetPanelLabelForeColor(activeNavPanel, NavLabelDefault);
                }

                activeNavPanel = pnl;
                if (activeNavPanel != null)
                {
                    activeNavPanel.BackColor = NavItemActive;
                    SetPanelLabelForeColor(activeNavPanel, Color.White);
                }
            }
            catch
            {
                // ignore
            }
        }

        // Helper: set label forecolor for known panels
        void SetPanelLabelForeColor(Panel pnl, Color color)
        {
            if (pnl == pnltrangchu) lbltrangchu.ForeColor = color;
            else if (pnl == pnltinhgiabaogia) lbltinhgiabaogia.ForeColor = color;
            else if (pnl == pnlmuahang) lblmuahang.ForeColor = color;
            else if (pnl == pnlsanxuat) lblsanxuat.ForeColor = color;
            else if (pnl == pnlkho) lblkho.ForeColor = color;
            else if (pnl == pnlbanhang) lblbanhang.ForeColor = color;
            else if (pnl == pnlbaocao) lblbaocao.ForeColor = color;
        }

        //hàm mở panel
        private void OpenFormInPanel(Form form)
        {
            panelcameratongquat.Controls.Clear(); // panel chứa form

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelcameratongquat.Controls.Add(form);
            form.Show();
            panelcameratongquat.Focus();
        }
        //hàm mở dashboard
        private void OpenDashboard(object sender, EventArgs e)
        {
            // make Trang chủ active
            SetActiveNavPanel(pnltrangchu);

            frmDashBoard f = new frmDashBoard(this);
            LoadForm(f);
        }
        void AddClickEvent(Control parent)
        {
            parent.Click += OpenDashboard;

            foreach (Control c in parent.Controls)
            {
                AddClickEvent(c);
            }
        }
        //
        //Tạo hàm load form vào panel
        public void LoadForm(Form f)
        {
            panelcameratongquat.Controls.Clear();   // xóa form cũ
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;                // lấp đầy panel, AutoScroll bật sẵn

            panelcameratongquat.Controls.Add(f);
            f.Show();
        }

        //hàm dành cho phần mở các trang
        void AttachMouseEvents(Control parent)
        {
            parent.MouseEnter += Panel_MouseEnter;

            foreach (Control c in parent.Controls)
            {
                c.MouseEnter += Panel_MouseEnter;
            }
        }
        Panel GetParentPanel(Control c)
        {
            while (c != null && !(c is Panel))
                c = c.Parent;

            return c as Panel;
        }
        void Panel_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = GetParentPanel(sender as Control);
            if (panel == null) return;

            if (panel == pnltinhgiabaogia)
                ShowMenu(panel, cmsBaoGia);

            else if (panel == pnlmuahang)
                ShowMenu(panel, cmsmuahang);

            else if (panel == pnlsanxuat)
                ShowMenu(panel, cmssanxuat);

            else if (panel == pnlkho)
                ShowMenu(panel, cmskho);

            else if (panel == pnlbanhang)
                ShowMenu(panel, cmsbanhang);

            else if (panel == pnlbaocao)
                ShowMenu(panel, cmsbaocao);
        }
        void HideAllMenus()
        {
            cmsBaoGia.Close();
            cmsbaocao.Close();
            cmsbanhang.Close();
            cmskho.Close();
            cmssanxuat.Close();
            cmsmuahang.Close();
        }
        //hàm mở menu
        void ShowMenu(Panel panel, ContextMenuStrip menu)
        {
            if (currentMenu == menu && menu.Visible) return;

            HideAllMenus();
            menu.Show(panel, new Point(0, panel.Height));
            currentMenu = menu;
        }
        //event
        private ContextMenuStrip currentOpenMenu = null;
        private Panel currentHoveredPanel = null;
        private void pnlcha_MouseMove(object sender, MouseEventArgs e)
        {

        }
        private void pnltinhgiabaogia_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnltinhgiabaogia, cmsBaoGia);
        }

        private void pnlmuahang_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnlmuahang, cmsmuahang);
        }

        private void pnlsanxuat_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnlsanxuat, cmssanxuat);
        }

        private void pnlkho_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnlkho, cmskho);
        }

        private void pnlbanhang_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnlbanhang, cmsbanhang);
        }
        private void pnlbaocao_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu(pnlbaocao, cmsbaocao);
        }
        private void pnltrangchu_MouseEnter(object sender, EventArgs e)
        {

        }
        private void tínhGiáSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // active -> Tính giá & Báo giá
            SetActiveNavPanel(pnltinhgiabaogia);
            frmPriceCalculation f = new frmPriceCalculation();
            OpenFormInPanel(f);
        }

        private void quảnLýBáoGiáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnltinhgiabaogia);
            OpenFormInPanel(new frmQuoteManagement());
        }

        private void thốngKêBáoGiáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnltinhgiabaogia);
            OpenFormInPanel(new frmQuoteStatistics());
        }

        private void tạoĐơnHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlmuahang);
            OpenFormInPanel(new frmPurchaseOrder());
        }

        private void nhậpKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlmuahang);
            OpenFormInPanel(new frmInventoryReceive());
        }

        private void thanhToánNCCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlmuahang);
            OpenFormInPanel(new frmSupplierPayment());
        }

        private void báoCáoMuaHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlmuahang);
            OpenFormInPanel(new frmPurchaseReport());
        }

        private void lệnhSảnXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlsanxuat);
            OpenFormInPanel(new frmProductionOrder());
        }

        private void tồnKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlkho);
            OpenFormInPanel(new frmInventoryIssue());
        }

        private void lậpChứngTừBánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbanhang);
            OpenFormInPanel(new frmSales());
        }

        private void doanhThuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbaocao);
            OpenFormInPanel(new frmSalesReport());
        }

        private void thuTiềnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbanhang);
            OpenFormInPanel(new frmPaymentReceive());
        }



        private void báoCáoKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlkho);
            OpenFormInPanel(new frmInventoryReport());
        }

        private void côngNợToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbaocao);
            OpenFormInPanel(new frmDebtReport());
        }

        private void xuấtKhoSảnXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlsanxuat);
            OpenFormInPanel(new frmWarehouseExport());
        }

        private void báoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbaocao);
            OpenFormInPanel(new frmSummaryReport());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Hiện hộp thoại xác nhận
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2  // mặc định chọn No
            );

            if (result == DialogResult.Yes)
            {
                // Xóa thông tin user hiện tại
                CurrentUser.Logout();

                // Hiện thông báo (tùy chọn)
                MessageBox.Show("Đã đăng xuất thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở lại form đăng nhập
                frmLogin loginForm = new frmLogin();
                loginForm.Show();

                // Đóng form chính (frmMain)
                this.Close();
            }
            // Nếu chọn No → không làm gì cả
        }

        private void cậpNhậpThôngTinKháchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnltinhgiabaogia);
            OpenFormInPanel(new frmCustomerManagement());
        }

        private void phiếuGiaoHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbanhang);
            OpenFormInPanel(new frmDeliveryNote());
        }

        private void báoCáoGiaoHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveNavPanel(pnlbanhang);
            OpenFormInPanel(new frmDeliveryReport());
        }
    }
}
