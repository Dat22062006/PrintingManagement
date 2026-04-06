# Hệ Thống Tính Giá Và Quản Lý Nghiệp Vụ In Ấn

Hệ thống hỗ trợ tính giá sản phẩm in (bao lì xì, hộp, túi, ...), lập báo giá, quản lý lệnh sản xuất, mua hàng, kho vận và bán hàng cho doanh nghiệp in ấn.

## Tính năng chính
- **Dashboard** - Thống kê tổng quan
- **Quản lý khách hàng & nhân viên**
- **Tính giá in ấn** - Tính giá theo kích thước, số lượng, loại giấy
- **Báo giá, Lệnh sản xuất, Mua hàng, Kho vận, Bán hàng**
- **Báo cáo** - Doanh thu, công nợ, tồn kho

## Công nghệ
- C# .NET Framework / WinForms
- SQL Server
- RDLC Reports

## Yêu cầu hệ thống
- Visual Studio 2022
- SQL Server + SSMS

## Tài khoản mặc định
- Username: `admin`
- Password: `123456`

## Hướng dẫn cài đặt

### 1. Chuẩn bị database
- Mở SSMS, chạy file `MainSQL.sql`

### 2. Mở source code
- Mở `PrintingManagement.sln` bằng Visual Studio

### 3. Cấu hình kết nối
- Sửa `Server name` trong `App.config` cho phù hợp

### 4. Build & Chạy
- Build → Build Solution (Ctrl+Shift+B)
- Chạy phím F5

## Cấu trúc dự án
PrintingManagement/
├── BaoLiXi/           # Source code
├── MainSQL.sql        # Script database
└── README.md
