# Hệ Thống Tính Giá Và Quản Lý Nghiệp Vụ In Ấn

Hệ thống hỗ trợ tính giá sản phẩm in (bao lì xì, hộp, túi...), lập báo giá, quản lý lệnh sản xuất, mua hàng, kho vận và bán hàng cho doanh nghiệp in ấn.

---

## Tính năng chính

| Module | Mô tả |
|--------|-------|
| **Dashboard** | Thống kê tổng quan doanh thu, đơn hàng, công nợ |
| **Quản lý khách hàng** | Thêm, sửa, xóa thông tin khách hàng |
| **Quản lý nhân viên** | Phân quyền người dùng (Admin, Nhân viên) |
| **Tính giá in ấn** | Tính giá theo kích thước, số lượng, loại giấy, kiểu in |
| **Báo giá** | Lập và quản lý báo giá cho khách hàng |
| **Lệnh sản xuất** | Quản lý đơn hàng sản xuất, theo dõi tiến độ |
| **Quản lý kho** | Nhập/xuất nguyên liệu, thành phẩm, tồn kho |
| **Mua hàng** | Quản lý đơn mua nguyên liệu từ nhà cung cấp |
| **Bán hàng** | Xuất hóa đơn, theo dõi doanh thu |
| **Báo cáo** | Doanh thu, công nợ, tồn kho, mua hàng, tổng hợp |

---

## Công nghệ sử dụng

- **Ngôn ngữ:** C# .NET Framework / WinForms
- **Cơ sở dữ liệu:** Microsoft SQL Server
- **IDE:** Visual Studio 2022
- **Báo cáo:** RDLC Reports

---

## Yêu cầu hệ thống

Trước khi chạy chương trình, máy tính cần cài đặt:

- **Visual Studio 2022** (khuyến nghị)
- **SQL Server** (Express hoặc bản đầy đủ)
- **SQL Server Management Studio (SSMS)**

---

## Hướng dẫn cài đặt

### 1. Chuẩn bị cơ sở dữ liệu

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server của bạn
3. Mở và chạy file `MainSQL.sql` để tạo:
   - Database `PrintingManagement`
   - Các bảng dữ liệu
   - Dữ liệu mẫu ban đầu

### 2. Mở source code

1. Giải nén source code (nếu tải về dạng zip)
2. Mở **Visual Studio**
3. Mở file solution: `PrintingManagement.sln`

### 3. Cấu hình kết nối database

1. Mở file `App.config` trong project
2. Sửa `Server name` phù hợp với máy của bạn:

```xml
<connectionStrings>
    <add name="PrintingManagement" 
         connectionString="Server=.\SQLEXPRESS;Database=PrintingManagement;Trusted_Connection=True;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Lưu ý:** Thay `.\SQLEXPRESS` bằng:
- `localhost` - nếu dùng SQL Server mặc định
- `localhost\SQLEXPRESS` - nếu dùng SQL Server Express
- `.\MSSQLSERVER` - nếu dùng SQL Server mặc định (named instance)

### 4. Build và chạy

1. Trong Visual Studio: **Build → Build Solution** (Ctrl + Shift + B)
2. Nhấn **F5** để chạy chương trình

---

## Tài khoản mặc định

| Username | Password | Quyền |
|----------|----------|-------|
| admin | 123456 | Quản trị |

> **Lưu ý:** Nên đổi mật khẩu sau khi đăng nhập lần đầu.

---

## Cấu trúc dự án

```
PrintingManagement/
│
├── BaoLiXi/                          # Source code chính (WinForms)
│   ├── App.config                     # Cấu hình kết nối database
│   ├── Program.cs                     # Entry point
│   ├── DatabaseHelper.cs              # Class kết nối SQL
│   ├── CurrentUser.cs                 # Quản lý user đăng nhập
│   │
│   ├── frmLogin.cs                    # Form đăng nhập
│   ├── frmMain.cs                     # Form chính (Menu)
│   ├── frmDashBoard.cs                # Dashboard thống kê
│   ├── frmQuanLyKhachHang.cs         # Quản lý khách hàng
│   ├── frmQuanLyNguoiDung.cs          # Quản lý nhân viên
│   ├── frmPriceCalculation.cs         # Tính giá in
│   ├── frmQuoteManagement.cs          # Quản lý báo giá
│   ├── frmProductionOrder.cs          # Lệnh sản xuất
│   ├── frmPurchaseOrder.cs            # Đơn mua hàng
│   ├── frmSales.cs                    # Bán hàng
│   ├── frmInventory*.cs               # Quản lý kho (nhập/xuất/báo cáo)
│   ├── frmDebtReport.cs               # Báo cáo công nợ
│   ├── frmPurchaseReport.cs           # Báo cáo mua hàng
│   ├── frmSalesReport.cs              # Báo cáo bán hàng
│   ├── frmSummaryReport.cs            # Báo cáo tổng hợp
│   │
│   └── Resources/                     # Hình ảnh, icon
│
├── MainSQL.sql                        # Script tạo database
│
├── README.md                           # Tài liệu dự án
│
└── PrintingManagement.sln              # Solution file
```

---

## Giấy phép

Đồ án học tập - Không sử dụng cho mục đích thương mại.

---

## Nhóm

- **Nhóm:** 1
- **Đề tài:** Hệ Thống Tính Giá Và Quản Lý Nghiệp Vụ In Ấn

---
