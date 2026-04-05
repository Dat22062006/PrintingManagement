-- =====================================================
-- DATABASE: PrintingManagement — SCHEMA
-- Hệ thống Quản lý In ấn - DNT Hub
-- Phiên bản: 3.1
-- Ngày: 2026-03-28
--
-- CHẠY TRƯỚC TIÊN — tạo database, bảng, stored procedure.
-- Sau đó chạy SeedData.sql để tạo tài khoản test.
-- =====================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PrintingManagement')
BEGIN
    ALTER DATABASE PrintingManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PrintingManagement;
END
GO

CREATE DATABASE PrintingManagement COLLATE Vietnamese_CI_AS;
GO

USE PrintingManagement;
GO

-- =====================================================
-- BẢNG 1: USERS
-- =====================================================
CREATE TABLE USERS (
    id             INT           PRIMARY KEY IDENTITY(1,1),
    Ten_Nguoi_Dung NVARCHAR(100) NOT NULL,
    Email          NVARCHAR(100) UNIQUE,
    Mat_Khau       NVARCHAR(255) NOT NULL,
    Vai_Tro        NVARCHAR(50)  NOT NULL
        CHECK (Vai_Tro IN (N'Admin', N'Kinh doanh', N'Kế toán', N'Thủ kho', N'Sản xuất', N'Giám đốc')),
    Ngay_Tao       DATETIME      DEFAULT GETDATE(),
    Ngay_Cap_Nhat  DATETIME      DEFAULT GETDATE(),
    Trang_Thai     BIT           DEFAULT 1
);
GO

-- =====================================================
-- BẢNG 2: NHA_CUNG_CAP
-- =====================================================
CREATE TABLE NHA_CUNG_CAP (
    id            INT           PRIMARY KEY IDENTITY(1,1),
    Ma_NCC        NVARCHAR(50)  UNIQUE NOT NULL,
    Ten_NCC       NVARCHAR(200) NOT NULL,
    Dia_Chi       NVARCHAR(500),
    Dien_Thoai    NVARCHAR(20),
    Email         NVARCHAR(100),
    MST           NVARCHAR(50),
    Nguoi_Lien_He NVARCHAR(100),
    So_Tai_Khoan  NVARCHAR(50),
    Ngan_Hang     NVARCHAR(200),
    Ghi_Chu       NVARCHAR(MAX),
    Ngay_Tao      DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 3: KHACH_HANG
-- =====================================================
CREATE TABLE KHACH_HANG (
    id             INT           PRIMARY KEY IDENTITY(1,1),
    Ma_KH          NVARCHAR(50)  UNIQUE NOT NULL,
    Ten_Khach_Hang NVARCHAR(200) NOT NULL,
    Dia_Chi        NVARCHAR(500),
    Dien_Thoai     NVARCHAR(20),
    Email          NVARCHAR(100),
    MST            NVARCHAR(50),
    Nguoi_Lien_He  NVARCHAR(100),
    Ghi_Chu        NVARCHAR(MAX),
    Trang_Thai     NVARCHAR(50)  DEFAULT N'Hoạt động'
        CHECK (Trang_Thai IN (N'Hoạt động', N'Không hoạt động', N'Nợ xấu')),
    Ngay_Tao       DATETIME      DEFAULT GETDATE(),
    Nguoi_Tao      NVARCHAR(100) NULL,
    Nguoi_Sua      NVARCHAR(100) NULL,
    Ngay_Sua       DATETIME      NULL,
    Nguoi_Xoa      NVARCHAR(100) NULL,
    Ngay_Xoa       DATETIME      NULL
);
GO

-- =====================================================
-- BẢNG 4: NGUYEN_LIEU
-- =====================================================
CREATE TABLE NGUYEN_LIEU (
    id                INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Nguyen_Lieu    NVARCHAR(50)  UNIQUE NOT NULL,
    Ten_Nguyen_Lieu   NVARCHAR(200) NOT NULL,
    Don_Vi_Tinh       NVARCHAR(50),
    Gia_Nhap          DECIMAL(18,2),
    Ton_Kho           DECIMAL(18,2) DEFAULT 0,
    Ton_Kho_Toi_Thieu DECIMAL(18,2) DEFAULT 0,
    Ton_Dau           DECIMAL(18,2) DEFAULT 0,
    Ngay_Nhap_Cuoi    DATE          NULL,
    De_Xuat_Nhap      DECIMAL(18,2) DEFAULT 0,
    Ghi_Chu           NVARCHAR(MAX)
);
GO

-- =====================================================
-- BẢNG 4b: NguyenLieu_NCC — Danh mục vật tư riêng theo từng NCC
-- Khi lưu đơn mua hàng, vật tư mới sẽ được tự động thêm vào danh mục này.
-- Sau này chỉ cần chọn vật tư từ danh sách NCC đã lưu, không cần nhập lại.
-- =====================================================
CREATE TABLE NguyenLieu_NCC (
    id                  INT           PRIMARY KEY IDENTITY(1,1),
    id_Nha_Cung_Cap    INT           FOREIGN KEY REFERENCES NHA_CUNG_CAP(id) ON DELETE CASCADE,
    id_Nguyen_Lieu      INT           FOREIGN KEY REFERENCES NGUYEN_LIEU(id) ON DELETE CASCADE,
    Don_Gia_Mac_Dinh    DECIMAL(18,2) DEFAULT 0,
    Ngay_Tao            DATETIME      DEFAULT GETDATE(),
    -- Khóa kép: mỗi vật tư chỉ xuất hiện 1 lần trong danh mục của 1 NCC
    CONSTRAINT UQ_NguyenLieu_NCC UNIQUE (id_Nha_Cung_Cap, id_Nguyen_Lieu)
);
GO

-- =====================================================
-- BẢNG 5: BAO_GIA
-- =====================================================
CREATE TABLE BAO_GIA (
    id                          INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Bao_Gia                  NVARCHAR(50)  UNIQUE NOT NULL,
    id_Khach_Hang               INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
    Ten_San_Pham                NVARCHAR(200) NOT NULL,
    Ngay_Bao_Gia                DATE          NOT NULL,
    Kich_Thuoc_Thanh_Pham       NVARCHAR(100),
    Khoi_Luong_Giay             NVARCHAR(100),
    Ten_Loai_Giay               NVARCHAR(100) NULL,
    Kho_In                      NVARCHAR(100),
    So_Mau_In                   INT,
    So_Con                      INT           DEFAULT 1,
    Bu_Hao                      INT           DEFAULT 0,
    Gia_Giay_Tan                DECIMAL(18,2),
    Loi_Nhuan_Phan_Tram         DECIMAL(5,2)  DEFAULT 20,
    Hieu_Luc_Bao_Gia_Ngay       INT           NULL DEFAULT 30,
    Thoi_Gian_Giao_Hang_Du_Kien NVARCHAR(100) NULL,
    Duong_Dan_PDF               NVARCHAR(500) NULL,
    Ghi_Chu                     NVARCHAR(MAX) NULL,
    Trang_Thai                  NVARCHAR(50)  DEFAULT N'Chờ duyệt'
        CHECK (Trang_Thai IN (
            N'Chờ duyệt', N'Đã duyệt', N'Đã gửi khách',
            N'Khách duyệt', N'Đã ký', N'Đang sản xuất',
            N'Hoàn thành', N'Từ chối', N'Hủy'
        )),
    Ngay_Tao                    DATETIME      DEFAULT GETDATE(),
    Nguoi_Tao                   NVARCHAR(100) NULL
);
GO

-- =====================================================
-- BẢNG 6: CHI_TIET_BAO_GIA
-- =====================================================
CREATE TABLE CHI_TIET_BAO_GIA (
    id                   INT           PRIMARY KEY IDENTITY(1,1),
    id_Bao_Gia           INT           FOREIGN KEY REFERENCES BAO_GIA(id) ON DELETE CASCADE,
    So_Luong             INT           NOT NULL,
    Tien_Giay            DECIMAL(18,2),
    Tien_Kem             DECIMAL(18,2),
    Tien_In              DECIMAL(18,2),
    Tien_Can_Mang        DECIMAL(18,2),
    Tien_Metalize        DECIMAL(18,2),
    Tien_UV              DECIMAL(18,2),
    Tien_Be              DECIMAL(18,2),
    Tien_Khuon_Be        DECIMAL(18,2),
    Tien_Dan             DECIMAL(18,2),
    Tien_Day             DECIMAL(18,2),
    Tien_Nut             DECIMAL(18,2),
    Tien_Thung           DECIMAL(18,2),
    Tien_Xe_Giao         DECIMAL(18,2),
    Tien_Proof           DECIMAL(18,2),
    Tong_Gia_Thanh       DECIMAL(18,2),
    Gia_Moi_Cai          DECIMAL(18,2),
    Gia_Bao_Khach        DECIMAL(18,2),
    Tong_Gia_Bao_Khach   DECIMAL(18,2) NULL,
    So_Luong_Thuc_Te     INT           NULL,
    Phi_Gia_Cong_Thuc_Te DECIMAL(18,2) NULL,
    Loi_Nhuan_Thuc_Te    DECIMAL(18,2) NULL,
    Ghi_Chu_Gia_Cong     NVARCHAR(MAX) NULL
);
GO

-- Mức số lượng hiển thị / duyệt chính (1 dòng CHI_TIET_BAO_GIA), tránh MAX(SL)+MAX(Tổng) lệch nhau
ALTER TABLE dbo.BAO_GIA ADD id_Muc_Chinh INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BAO_GIA_ID_MUC_CHINH')
BEGIN
    ALTER TABLE dbo.BAO_GIA ADD CONSTRAINT FK_BAO_GIA_ID_MUC_CHINH
        FOREIGN KEY (id_Muc_Chinh) REFERENCES dbo.CHI_TIET_BAO_GIA(id);
END
GO

-- =====================================================
-- BẢNG 7: LENH_SAN_XUAT
-- =====================================================
CREATE TABLE LENH_SAN_XUAT (
    id                     INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Lenh_SX             NVARCHAR(50)  UNIQUE NOT NULL,
    id_Bao_Gia             INT           FOREIGN KEY REFERENCES BAO_GIA(id),
    id_Khach_Hang          INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
    Ten_San_Pham           NVARCHAR(200),
    So_Luong               INT,
    Ngay_Bat_Dau           DATE,
    Ngay_Ket_Thuc          DATE,
    Ngay_Giao_Hang_Du_Kien DATE,
    Trang_Thai             NVARCHAR(50)  DEFAULT N'Chưa thực hiện'
        CHECK (Trang_Thai IN (N'Chưa thực hiện', N'Đang sản xuất', N'Hoàn thành', N'Hủy')),
    Ghi_Chu                NVARCHAR(MAX),
    Ngay_Tao               DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 8: DON_DAT_HANG_NCC
-- =====================================================
CREATE TABLE DON_DAT_HANG_NCC (
    id                    INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Don_Hang           NVARCHAR(50)  UNIQUE NOT NULL,
    id_Nha_Cung_Cap       INT           FOREIGN KEY REFERENCES NHA_CUNG_CAP(id),
    Ngay_Dat_Hang         DATE          NOT NULL,
    Ngay_Giao_Hang        DATE,
    Dieu_Khoan_Thanh_Toan NVARCHAR(200),
    So_Ngay_No            INT           DEFAULT 0,
    Trang_Thai            NVARCHAR(50)  DEFAULT N'Chưa thực hiện'
        CHECK (Trang_Thai IN (N'Chưa thực hiện', N'Đang thực hiện', N'Hoàn thành', N'Hủy')),
    Dia_Diem_Giao_Hang    NVARCHAR(500),
    File_Dinh_Kem         NVARCHAR(500),
    Ghi_Chu               NVARCHAR(MAX) NULL,
    Tong_Tien             DECIMAL(18,2) DEFAULT 0,
    Ngay_Tao              DATETIME      DEFAULT GETDATE()
);
GO

-- Bổ sung ghi chú đơn mua (DB đã tạo trước đây không có cột)
IF COL_LENGTH(N'dbo.DON_DAT_HANG_NCC', N'Ghi_Chu') IS NULL
    ALTER TABLE dbo.DON_DAT_HANG_NCC ADD Ghi_Chu NVARCHAR(MAX) NULL;
GO

-- =====================================================
-- BẢNG 9: CHI_TIET_DON_HANG_NCC
-- =====================================================
CREATE TABLE CHI_TIET_DON_HANG_NCC (
    id                  INT           PRIMARY KEY IDENTITY(1,1),
    id_Don_Dat_Hang     INT           FOREIGN KEY REFERENCES DON_DAT_HANG_NCC(id) ON DELETE CASCADE,
    id_Nguyen_Lieu      INT           FOREIGN KEY REFERENCES NGUYEN_LIEU(id),
    So_Luong            DECIMAL(18,2) NOT NULL,
    So_Luong_Da_Nhan    DECIMAL(18,2) DEFAULT 0,
    Don_Gia             DECIMAL(18,2) NOT NULL,
    Thanh_Tien          DECIMAL(18,2),
    Phan_Tram_Thue_GTGT DECIMAL(5,2)  DEFAULT 10,
    Tien_Thue_GTGT      DECIMAL(18,2)
);
GO

-- =====================================================
-- BẢNG 10: PHIEU_NHAP_KHO
-- =====================================================
CREATE TABLE PHIEU_NHAP_KHO (
    id                   INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Phieu_Nhap        NVARCHAR(50)  UNIQUE NOT NULL,
    id_Don_Dat_Hang      INT           FOREIGN KEY REFERENCES DON_DAT_HANG_NCC(id),
    id_Nha_Cung_Cap      INT           FOREIGN KEY REFERENCES NHA_CUNG_CAP(id),
    Ngay_Nhap            DATE          NOT NULL,
    Nguoi_Nhap           NVARCHAR(100),
    Loai_Nhap            NVARCHAR(50)
        CHECK (Loai_Nhap IN (
            N'Mua trong nước nhập kho', N'Mua trong nước không qua kho',
            N'Nhập khẩu nhập kho',      N'Nhập khẩu không qua kho'
        )),
    Hinh_Thuc_Thanh_Toan NVARCHAR(50),
    Co_Hoa_Don           BIT           DEFAULT 1,
    Mau_So_Hoa_Don       NVARCHAR(50),
    Ky_Hieu_Hoa_Don      NVARCHAR(50),
    So_Hoa_Don           NVARCHAR(50),
    Ngay_Hoa_Don         DATE,
    File_Hoa_Don         NVARCHAR(500),
    Trang_Thai           NVARCHAR(50)  DEFAULT N'Chưa hoàn thành',
    Ngay_Chung_Tu        DATE,
    Ngay_Hach_Toan       DATE,
    Trang_Thai_TT        NVARCHAR(50)  DEFAULT N'Chưa thanh toán',
    Ngay_Tao             DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 11: CHI_TIET_PHIEU_NHAP
-- =====================================================
CREATE TABLE CHI_TIET_PHIEU_NHAP (
    id             INT           PRIMARY KEY IDENTITY(1,1),
    id_Phieu_Nhap  INT           FOREIGN KEY REFERENCES PHIEU_NHAP_KHO(id) ON DELETE CASCADE,
    id_Nguyen_Lieu INT           FOREIGN KEY REFERENCES NGUYEN_LIEU(id),
    So_Luong_Nhap  DECIMAL(18,2) NOT NULL,
    Don_Gia_Nhap   DECIMAL(18,2) NOT NULL,
    Thanh_Tien     DECIMAL(18,2)
);
GO

-- =====================================================
-- BẢNG 12: THANH_TOAN_NCC
-- =====================================================
CREATE TABLE THANH_TOAN_NCC (
    id              INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Thanh_Toan   NVARCHAR(50)  UNIQUE NOT NULL,
    id_Nha_Cung_Cap INT           FOREIGN KEY REFERENCES NHA_CUNG_CAP(id),
    id_Don_Dat_Hang INT           FOREIGN KEY REFERENCES DON_DAT_HANG_NCC(id),
    Ngay_Thanh_Toan DATE          NOT NULL,
    So_Tien         DECIMAL(18,2) NOT NULL,
    Phuong_Thuc     NVARCHAR(50)
        CHECK (Phuong_Thuc IN (N'Tiền mặt', N'Chuyển khoản', N'Ủy nhiệm chi', N'Séc')),
    So_Chung_Tu     NVARCHAR(50),
    Ngay_Chung_Tu   DATE,
    Ghi_Chu         NVARCHAR(MAX),
    Ngay_Tao        DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 13: PHIEU_XUAT_KHO_SX
-- =====================================================
CREATE TABLE PHIEU_XUAT_KHO_SX (
    id               INT          PRIMARY KEY IDENTITY(1,1),
    Ma_Phieu_Xuat    NVARCHAR(50) UNIQUE NOT NULL,
    id_Lenh_San_Xuat INT          FOREIGN KEY REFERENCES LENH_SAN_XUAT(id),
    Ngay_Xuat        DATE         NOT NULL,
    Nguoi_Xuat       NVARCHAR(100),
    Trang_Thai       NVARCHAR(50) DEFAULT N'Chưa hoàn thành',
    Ngay_Tao         DATETIME     DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 14: CHI_TIET_XUAT_KHO_SX
-- =====================================================
CREATE TABLE CHI_TIET_XUAT_KHO_SX (
    id             INT           PRIMARY KEY IDENTITY(1,1),
    id_Phieu_Xuat  INT           FOREIGN KEY REFERENCES PHIEU_XUAT_KHO_SX(id) ON DELETE CASCADE,
    id_Nguyen_Lieu INT           FOREIGN KEY REFERENCES NGUYEN_LIEU(id),
    So_Luong_Xuat  DECIMAL(18,2) NOT NULL,
    Don_Gia        DECIMAL(18,2),
    Thanh_Tien     DECIMAL(18,2)
);
GO

-- =====================================================
-- BẢNG 15: DON_BAN_HANG
-- =====================================================
CREATE TABLE DON_BAN_HANG (
    id                   INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Don_Ban           NVARCHAR(50)  UNIQUE NOT NULL,
    id_Khach_Hang        INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
    Ngay_Ban_Hang        DATE          NOT NULL,
    Ngay_Den_Han         DATE          NULL,
    Mau_So_Hoa_Don       NVARCHAR(50),
    Ky_Hieu_Hoa_Don      NVARCHAR(50),
    So_Hoa_Don           NVARCHAR(50),
    Ngay_Hoa_Don         DATE,
    Tong_Tien_Truoc_Thue DECIMAL(18,2),
    Phan_Tram_VAT        DECIMAL(5,2)  DEFAULT 10,
    Tien_Thue_VAT        DECIMAL(18,2),
    Tong_Thanh_Toan      DECIMAL(18,2),
    Trang_Thai           NVARCHAR(50)  DEFAULT N'Chưa hoàn thành',
    Ngay_Uoc_Thu         DATE          NULL,
    TK_No                NVARCHAR(20)  NULL,
    TK_Co                NVARCHAR(20)  NULL,
    Ghi_Chu              NVARCHAR(MAX),
    Ngay_Tao             DATETIME      DEFAULT GETDATE(),
    id_Bao_Gia           INT           NULL
);
GO

-- =====================================================
-- BẢNG 16: CHI_TIET_DON_BAN_HANG
-- =====================================================
CREATE TABLE CHI_TIET_DON_BAN_HANG (
    id              INT           PRIMARY KEY IDENTITY(1,1),
    id_Don_Ban_Hang INT           FOREIGN KEY REFERENCES DON_BAN_HANG(id) ON DELETE CASCADE,
    Ten_San_Pham    NVARCHAR(200),
    So_Luong        DECIMAL(18,2) NOT NULL,
    Don_Gia         DECIMAL(18,2) NOT NULL,
    Thanh_Tien      DECIMAL(18,2),
    Phan_Tram_VAT   DECIMAL(5,2)  DEFAULT 10,
    Tien_VAT        DECIMAL(18,2)
);
GO

-- =====================================================
-- BẢNG 17: PHIEU_THU
-- =====================================================
CREATE TABLE PHIEU_THU (
    id              INT           PRIMARY KEY IDENTITY(1,1),
    Ma_Phieu_Thu    NVARCHAR(50)  UNIQUE NOT NULL,
    id_Khach_Hang   INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
    id_Don_Ban_Hang INT           FOREIGN KEY REFERENCES DON_BAN_HANG(id),
    Ngay_Thu        DATE          NOT NULL,
    So_Tien_Thu     DECIMAL(18,2) NOT NULL,
    Phuong_Thuc_Thu NVARCHAR(50)
        CHECK (Phuong_Thuc_Thu IN (N'Tiền mặt', N'Chuyển khoản', N'Séc')),
    So_Chung_Tu     NVARCHAR(50),
    Ghi_Chu         NVARCHAR(MAX),
    Ngay_Tao        DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 18: CONG_NO_KHACH_HANG
-- =====================================================
CREATE TABLE CONG_NO_KHACH_HANG (
    id            INT           PRIMARY KEY IDENTITY(1,1),
    id_Khach_Hang INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
    Tong_No       DECIMAL(18,2) DEFAULT 0,
    Da_Thu        DECIMAL(18,2) DEFAULT 0,
    Con_Lai       AS (Tong_No - Da_Thu) PERSISTED,
    Ngay_Cap_Nhat DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 19: CONG_NO_NCC
-- =====================================================
CREATE TABLE CONG_NO_NCC (
    id              INT           PRIMARY KEY IDENTITY(1,1),
    id_Nha_Cung_Cap INT           FOREIGN KEY REFERENCES NHA_CUNG_CAP(id),
    Tong_No         DECIMAL(18,2) DEFAULT 0,
    Da_Tra          DECIMAL(18,2) DEFAULT 0,
    Con_Lai         AS (Tong_No - Da_Tra) PERSISTED,
    Ngay_Cap_Nhat   DATETIME      DEFAULT GETDATE()
);
GO

-- =====================================================
-- BẢNG 20: AUDIT_LOG
-- =====================================================
CREATE TABLE AUDIT_LOG (
    id             INT           PRIMARY KEY IDENTITY(1,1),
    Ten_User       NVARCHAR(100) NOT NULL,
    Vai_Tro        NVARCHAR(50),
    Hanh_Dong      NVARCHAR(100) NOT NULL,
    Bang_Lien_Quan NVARCHAR(100),
    Ma_Doi_Tuong   NVARCHAR(100),
    Mo_Ta          NVARCHAR(MAX),
    IP_Address     NVARCHAR(50),
    Thoi_Gian      DATETIME      DEFAULT GETDATE()
);
GO

CREATE INDEX IX_AUDIT_LOG_User ON AUDIT_LOG (Ten_User, Thoi_Gian DESC);
CREATE INDEX IX_AUDIT_LOG_Time ON AUDIT_LOG (Thoi_Gian DESC);
GO

-- =====================================================
-- DỮ LIỆU MẪU: USERS (mật khẩu plaintext, đổi sau khi go-live)
-- =====================================================
INSERT INTO USERS (Ten_Nguoi_Dung, Email, Mat_Khau, Vai_Tro) VALUES
(N'admin',      N'admin@company.vn',      N'123456', N'Admin'),
(N'kinhdoanh1', N'kinhdoanh1@company.vn', N'123456', N'Kinh doanh'),
(N'ketoan1',    N'ketoan1@company.vn',    N'123456', N'Kế toán'),
(N'thukho1',    N'thukho1@company.vn',    N'123456', N'Thủ kho'),
(N'sanxuat1',   N'sanxuat1@company.vn',   N'123456', N'Sản xuất'),
(N'giamdoc1',   N'giamdoc1@company.vn',   N'123456', N'Giám đốc');
GO

-- =====================================================
-- TRIGGER 1: TỰ ĐỘNG TRỪ TỒN KHO KHI XUẤT
-- [FIX] Thêm UPDLOCK để tránh race condition khi nhiều giao dịch xảy ra đồng thời
-- =====================================================
CREATE TRIGGER trg_AfterInsertExportDetail
ON CHI_TIET_XUAT_KHO_SX AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    -- Kiểm tra đủ tồn kho với lock để tránh race condition
    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN NGUYEN_LIEU nl WITH (UPDLOCK, HOLDLOCK)
        ON nl.id = i.id_Nguyen_Lieu
        WHERE nl.Ton_Kho < i.So_Luong_Xuat
    )
    BEGIN
        RAISERROR(N'Tồn kho không đủ để xuất. Giao dịch bị hủy.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    -- Trừ tồn kho sau khi đã kiểm tra đủ
    UPDATE nl WITH (UPDLOCK)
    SET nl.Ton_Kho = nl.Ton_Kho - i.So_Luong_Xuat
    FROM NGUYEN_LIEU nl WITH (UPDLOCK)
    JOIN inserted i ON nl.id = i.id_Nguyen_Lieu;
END
GO

-- =====================================================
-- TRIGGER 2: HOÀN TRẢ TỒN KHO KHI XÓA XUẤT
-- =====================================================
CREATE TRIGGER trg_AfterDeleteExportDetail
ON CHI_TIET_XUAT_KHO_SX AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE nl SET nl.Ton_Kho = nl.Ton_Kho + d.So_Luong_Xuat
    FROM NGUYEN_LIEU nl JOIN deleted d ON nl.id = d.id_Nguyen_Lieu;
END
GO

-- =====================================================
-- SP: ĐĂNG NHẬP (sp_Login)
-- [FIX] Cải thiện email matching - chỉ khớp chính xác hoặc phần trước @
-- =====================================================
CREATE PROCEDURE sp_Login
    @Username NVARCHAR(100),
    @VaiTro   NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @u NVARCHAR(100) = LTRIM(RTRIM(@Username));
    IF @u = N'' RETURN;

    -- Khớp: tên hiển thị, email đầy đủ, hoặc phần trước @ (vd: admin → admin@anything.com)
    -- [FIX] Sửa LIKE: chỉ khớp phần trước @, không khớp nhầm "admin123@..."
    SELECT id, Ten_Nguoi_Dung, Email, Vai_Tro, Mat_Khau
    FROM USERS
    WHERE Vai_Tro    = @VaiTro
      AND Trang_Thai = 1
      AND (
          Ten_Nguoi_Dung = @u
          OR LOWER(LTRIM(RTRIM(ISNULL(Email, N'')))) = LOWER(@u)
          -- [FIX] So sánh đúng: phần trước @ của email phải khớp CHÍNH XÁC với username
          OR (LOWER(LEFT(LTRIM(RTRIM(ISNULL(Email, N''))), CHARINDEX('@', LTRIM(RTRIM(ISNULL(Email, N''))) + '@' - 1))) = LOWER(@u)
              AND CHARINDEX('@', LTRIM(RTRIM(ISNULL(Email, N'')))) > 0)
      );
END
GO

-- =====================================================
-- SP: GHI AUDIT LOG (sp_LogAction)
-- =====================================================
CREATE PROCEDURE sp_LogAction
    @TenUser      NVARCHAR(100),
    @VaiTro       NVARCHAR(50),
    @HanhDong     NVARCHAR(100),
    @BangLienQuan NVARCHAR(100) = NULL,
    @MaDoiTuong   NVARCHAR(100) = NULL,
    @MoTa         NVARCHAR(MAX) = NULL,
    @IPAddress    NVARCHAR(50)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AUDIT_LOG (Ten_User, Vai_Tro, Hanh_Dong, Bang_Lien_Quan, Ma_Doi_Tuong, Mo_Ta, IP_Address)
    VALUES (@TenUser, @VaiTro, @HanhDong, @BangLienQuan, @MaDoiTuong, @MoTa, @IPAddress);
END
GO

-- =====================================================
-- SP: QUẢN LÝ VẬT TƯ / XUẤT KHO SX
-- =====================================================
CREATE PROCEDURE sp_GetMaterials AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap, Ton_Kho
    FROM NGUYEN_LIEU ORDER BY Ma_Nguyen_Lieu;
END
GO

CREATE PROCEDURE sp_GetActiveProductionOrders AS
BEGIN
    SET NOCOUNT ON;
    SELECT lsx.id, lsx.Ma_Lenh_SX, lsx.Ten_San_Pham, lsx.So_Luong, lsx.Trang_Thai, kh.Ten_Khach_Hang
    FROM LENH_SAN_XUAT lsx
    LEFT JOIN KHACH_HANG kh ON kh.id = lsx.id_Khach_Hang
    WHERE lsx.Trang_Thai = N'Đang sản xuất'
    ORDER BY lsx.id DESC;
END
GO

CREATE PROCEDURE sp_GenerateExportCode @Year INT, @NextCode NVARCHAR(50) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(20) = N'PX-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Xuat, LEN(@prefix)+1, 10) AS INT)), 0) + 1
    FROM PHIEU_XUAT_KHO_SX WHERE Ma_Phieu_Xuat LIKE @prefix + N'%';
    SET @NextCode = @prefix + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
END
GO

CREATE PROCEDURE sp_SaveWarehouseExport
    @ExportCode NVARCHAR(50), @ProductionOrderId INT, @ExportDate DATE,
    @Receiver NVARCHAR(100), @Details XML, @NewExportId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO PHIEU_XUAT_KHO_SX (Ma_Phieu_Xuat, id_Lenh_San_Xuat, Ngay_Xuat, Nguoi_Xuat, Trang_Thai)
        VALUES (@ExportCode, @ProductionOrderId, @ExportDate, @Receiver, N'Hoàn thành');
        SET @NewExportId = SCOPE_IDENTITY();
        INSERT INTO CHI_TIET_XUAT_KHO_SX (id_Phieu_Xuat, id_Nguyen_Lieu, So_Luong_Xuat, Don_Gia, Thanh_Tien)
        SELECT @NewExportId,
            d.value('(MaterialId)[1]', 'INT'),
            d.value('(Qty)[1]', 'FLOAT'),
            d.value('(UnitPrice)[1]', 'FLOAT'),
            d.value('(LineTotal)[1]', 'FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg1 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg1, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- SP: QUẢN LÝ KHÁCH HÀNG
-- =====================================================
CREATE PROCEDURE sp_GetCustomers @Keyword NVARCHAR(200) = N'' AS
BEGIN
    SET NOCOUNT ON;
    SELECT ROW_NUMBER() OVER (ORDER BY Ma_KH) AS STT,
        Ma_KH, Ten_Khach_Hang, Dia_Chi, MST, Dien_Thoai, Nguoi_Lien_He, id
    FROM KHACH_HANG
    WHERE Trang_Thai = N'Hoạt động'
      AND (@Keyword = N'' OR Ma_KH LIKE N'%' + @Keyword + N'%'
           OR Ten_Khach_Hang LIKE N'%' + @Keyword + N'%'
           OR Dia_Chi LIKE N'%' + @Keyword + N'%')
    ORDER BY Ma_KH;
END
GO

CREATE PROCEDURE sp_DeactivateCustomer @CustomerId INT, @UpdatedBy NVARCHAR(100) AS
BEGIN
    SET NOCOUNT ON;
    UPDATE KHACH_HANG SET Trang_Thai = N'Không hoạt động', Nguoi_Xoa = @UpdatedBy, Ngay_Xoa = GETDATE()
    WHERE id = @CustomerId;
END
GO

CREATE PROCEDURE sp_GenerateCustomerCode @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_KH, 3, LEN(Ma_KH)) AS INT)), 0) + 1
    FROM KHACH_HANG WHERE Ma_KH LIKE N'KH%';
    SET @NextCode = N'KH' + RIGHT(N'0000' + CAST(@nextNum AS NVARCHAR), 4);
END
GO

CREATE PROCEDURE sp_GetCustomerById @CustomerId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT Ma_KH, Ten_Khach_Hang, Dia_Chi, MST, Dien_Thoai, Email, Nguoi_Lien_He, Ghi_Chu
    FROM KHACH_HANG WHERE id = @CustomerId;
END
GO

CREATE PROCEDURE sp_SaveCustomer
    @CustomerId INT, @Code NVARCHAR(50), @Name NVARCHAR(200),
    @Address NVARCHAR(500), @TaxCode NVARCHAR(50), @Phone NVARCHAR(20),
    @Email NVARCHAR(100), @Contact NVARCHAR(100), @Note NVARCHAR(MAX), @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF @CustomerId = 0
        INSERT INTO KHACH_HANG (Ma_KH, Ten_Khach_Hang, Dia_Chi, MST, Dien_Thoai, Email, Nguoi_Lien_He, Ghi_Chu, Trang_Thai, Nguoi_Tao, Ngay_Tao)
        VALUES (@Code, @Name, @Address, @TaxCode, @Phone, @Email, @Contact, @Note, N'Hoạt động', @UpdatedBy, GETDATE());
    ELSE
        UPDATE KHACH_HANG SET Ten_Khach_Hang=@Name, Dia_Chi=@Address, MST=@TaxCode,
            Dien_Thoai=@Phone, Email=@Email, Nguoi_Lien_He=@Contact, Ghi_Chu=@Note,
            Nguoi_Sua=@UpdatedBy, Ngay_Sua=GETDATE()
        WHERE id = @CustomerId;
END
GO
-- =====================================================
-- ⚠️ THÊM VÀO PrintingManagement_FINAL.sql
-- Vị trí: Dán vào sau phần SP 9 (sp_SaveCustomer)
--         và trước dòng SP 10 (Dashboard)
-- =====================================================
-- CÁC THAY ĐỔI:
--   [FIX]  sp_GenerateCustomerCode: mã từ KH0001 → KH_01
--   [NEW]  sp_GetAllActiveCustomers: lấy tất cả KH cho ComboBox
--   [NEW]  sp_GetAllPaperTypes: lấy danh sách loại giấy đã lưu
--   [NEW]  Bảng PAPER_TYPE_CONFIG: lưu loại giấy tùy chỉnh
-- =====================================================


-- =====================================================
-- [NEW] BẢNG PAPER_TYPE_CONFIG
-- Lưu các loại giấy người dùng tự thêm
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PAPER_TYPE_CONFIG')
BEGIN
    CREATE TABLE PAPER_TYPE_CONFIG (
        id            INT           PRIMARY KEY IDENTITY(1,1),
        Ten_Loai_Giay NVARCHAR(200) NOT NULL UNIQUE,
        Dinh_Luong    DECIMAL(10,2) NOT NULL,  -- gsm, ví dụ 250.0
        Kho_In_Mac_Dinh NVARCHAR(50),          -- ví dụ "79 x 51"
        Gia_Tan_Mac_Dinh DECIMAL(18,2),        -- đơn giá mặc định /tấn
        Bu_Hao_Mac_Dinh  INT DEFAULT 500,
        Is_System     BIT DEFAULT 0,           -- 1 = loại mặc định, 0 = do user thêm
        Ngay_Tao      DATETIME DEFAULT GETDATE()
    );
END
GO

-- Seed dữ liệu loại giấy mặc định (chỉ chạy nếu bảng mới tạo)
IF NOT EXISTS (SELECT 1 FROM PAPER_TYPE_CONFIG WHERE Is_System = 1)
BEGIN
    INSERT INTO PAPER_TYPE_CONFIG
        (Ten_Loai_Giay, Dinh_Luong, Kho_In_Mac_Dinh, Gia_Tan_Mac_Dinh, Bu_Hao_Mac_Dinh, Is_System)
    VALUES
        (N'Couche 250 gsm',  250, N'79 x 51',   26000000, 500, 1),
        (N'Ivory 400 gsm',   400, N'69.5 x 42', 22000000, 500, 1),
        (N'Duplex 300 gsm',  300, N'79 x 51',   24000000, 500, 1),
        (N'Bristol 230 gsm', 230, N'79 x 51',   20000000, 500, 1);
END
GO


-- ─────────────────────────────────────────────────────
-- [FIX] sp_GenerateCustomerCode: đổi sang KH_01 format
-- ─────────────────────────────────────────────────────
-- Mã cũ: KH0001 | Mã mới: KH_01
-- Tự động detect cả 2 format cũ để không bị lỗi nếu
-- đã có dữ liệu format cũ trong DB
-- ─────────────────────────────────────────────────────

IF OBJECT_ID('sp_GenerateCustomerCode', 'P') IS NOT NULL
    DROP PROCEDURE sp_GenerateCustomerCode;
GO

CREATE PROCEDURE sp_GenerateCustomerCode
    @NextCode NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @nextNum INT;

    -- Hỗ trợ cả format mới KH_XX và format cũ KH0000
    SELECT @nextNum = ISNULL(MAX(
        CASE
            -- Format mới: KH_01, KH_02, ...
            WHEN Ma_KH LIKE N'KH\_%' ESCAPE N'\'
                THEN CAST(SUBSTRING(Ma_KH, 4, 10) AS INT)
            -- Format cũ: KH0001, KH0002, ...
            WHEN Ma_KH LIKE N'KH[0-9]%'
                THEN CAST(SUBSTRING(Ma_KH, 3, 10) AS INT)
            ELSE 0
        END
    ), 0) + 1
    FROM KHACH_HANG
    WHERE Ma_KH LIKE N'KH%';

    -- Trả về mã dạng KH_01, KH_02, ..., KH_99, KH_100
    SET @NextCode = N'KH_' + RIGHT(N'00' + CAST(@nextNum AS NVARCHAR), 2);
    
    -- Nếu số > 99, không pad thêm 0
    IF @nextNum > 99
        SET @NextCode = N'KH_' + CAST(@nextNum AS NVARCHAR);
END
GO


-- ─────────────────────────────────────────────────────
-- [NEW] sp_GetAllActiveCustomers
-- Lấy toàn bộ KH đang hoạt động để nạp vào ComboBox
-- Trả về: id, Ma_KH, Ten_Khach_Hang, Dia_Chi, MST
-- ─────────────────────────────────────────────────────

IF OBJECT_ID('sp_GetAllActiveCustomers', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllActiveCustomers;
GO

CREATE PROCEDURE sp_GetAllActiveCustomers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        Ma_KH,
        Ten_Khach_Hang,
        ISNULL(Dia_Chi, N'')        AS Dia_Chi,
        ISNULL(MST,     N'')        AS MST,
        ISNULL(Dien_Thoai, N'')     AS Dien_Thoai,
        ISNULL(Nguoi_Lien_He, N'')  AS Nguoi_Lien_He
    FROM KHACH_HANG
    WHERE Trang_Thai = N'Hoạt động'
    ORDER BY Ma_KH;
END
GO


-- ─────────────────────────────────────────────────────
-- [NEW] sp_GetAllPaperTypes
-- Lấy danh sách loại giấy (mặc định + do user thêm)
-- ─────────────────────────────────────────────────────

IF OBJECT_ID('sp_GetAllPaperTypes', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllPaperTypes;
GO

CREATE PROCEDURE sp_GetAllPaperTypes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        Ten_Loai_Giay,
        Dinh_Luong,
        ISNULL(Kho_In_Mac_Dinh, N'')    AS Kho_In_Mac_Dinh,
        ISNULL(Gia_Tan_Mac_Dinh, 0)     AS Gia_Tan_Mac_Dinh,
        ISNULL(Bu_Hao_Mac_Dinh, 500)    AS Bu_Hao_Mac_Dinh,
        Is_System
    FROM PAPER_TYPE_CONFIG
    ORDER BY Is_System DESC, Ten_Loai_Giay;
END
GO


-- ─────────────────────────────────────────────────────
-- [NEW] sp_SavePaperType
-- Lưu loại giấy mới do user nhập tay vào ComboBox
-- Nếu đã tồn tại cùng tên thì UPDATE, không thì INSERT
-- ─────────────────────────────────────────────────────

IF OBJECT_ID('sp_SavePaperType', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavePaperType;
GO

CREATE PROCEDURE sp_SavePaperType
    @TenLoaiGiay     NVARCHAR(200),
    @DinhLuong       DECIMAL(10,2),
    @KhoIn           NVARCHAR(50)  = NULL,
    @GiaTan          DECIMAL(18,2) = NULL,
    @BuHao           INT           = 500
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM PAPER_TYPE_CONFIG WHERE Ten_Loai_Giay = @TenLoaiGiay)
    BEGIN
        -- Cập nhật nếu đã có (chỉ cập nhật loại user tự thêm)
        UPDATE PAPER_TYPE_CONFIG SET
            Dinh_Luong       = @DinhLuong,
            Kho_In_Mac_Dinh  = @KhoIn,
            Gia_Tan_Mac_Dinh = @GiaTan,
            Bu_Hao_Mac_Dinh  = @BuHao
        WHERE Ten_Loai_Giay = @TenLoaiGiay
          AND Is_System = 0;
    END
    ELSE
    BEGIN
        INSERT INTO PAPER_TYPE_CONFIG
            (Ten_Loai_Giay, Dinh_Luong, Kho_In_Mac_Dinh, Gia_Tan_Mac_Dinh, Bu_Hao_Mac_Dinh, Is_System)
        VALUES
            (@TenLoaiGiay, @DinhLuong, @KhoIn, @GiaTan, @BuHao, 0);
    END
END
GO
-- =====================================================
-- SP: DASHBOARD
-- =====================================================
CREATE PROCEDURE sp_GetDashboardStats AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM BAO_GIA) AS QuoteCount,
        (SELECT COUNT(*) FROM BAO_GIA WHERE Trang_Thai = N'Đã duyệt') AS ApprovedCount,
        (SELECT COUNT(*) FROM BAO_GIA WHERE Trang_Thai = N'Đang sản xuất') AS ProductionCount,
        (SELECT COUNT(DISTINCT id) FROM NGUYEN_LIEU WHERE Ton_Kho <= Ton_Kho_Toi_Thieu OR Ton_Kho <= 0) AS LowStockCount,
        (SELECT ISNULL(SUM(Tong_Thanh_Toan), 0) FROM DON_BAN_HANG
         WHERE MONTH(Ngay_Ban_Hang) = MONTH(GETDATE()) AND YEAR(Ngay_Ban_Hang) = YEAR(GETDATE())) AS MonthlyRevenue,
        (SELECT COUNT(*) FROM NGUYEN_LIEU WHERE Ton_Kho > 0) AS StockItemCount,
        (SELECT ISNULL(SUM(CASE WHEN Con_Lai > 0 THEN Con_Lai ELSE 0 END), 0) FROM CONG_NO_KHACH_HANG) AS TotalReceivable,
        (SELECT ISNULL(SUM(Con_Lai), 0) FROM CONG_NO_NCC WHERE Con_Lai > 0) AS TotalPayable;
END
GO

CREATE PROCEDURE sp_GetDashboardQuotes AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 4 bg.id, bg.Ma_Bao_Gia, ISNULL(kh.Ten_Khach_Hang, N'—') AS Ten_Khach_Hang,
        bg.Ten_San_Pham,
        ISNULL((SELECT TOP 1 ct.So_Luong FROM CHI_TIET_BAO_GIA ct WHERE ct.id_Bao_Gia = bg.id ORDER BY ct.id), 0) AS So_Luong,
        ISNULL((SELECT TOP 1 ct.Tong_Gia_Bao_Khach FROM CHI_TIET_BAO_GIA ct WHERE ct.id_Bao_Gia = bg.id ORDER BY ct.id), 0) AS Gia_Tri,
        bg.Trang_Thai, bg.Ngay_Bao_Gia
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    ORDER BY bg.Ngay_Tao DESC;
END
GO

CREATE PROCEDURE sp_GetDashboardProduction AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 10 lsx.id, lsx.Ma_Lenh_SX, lsx.Ten_San_Pham,
        ISNULL(kh.Ten_Khach_Hang, N'—') AS Ten_Khach_Hang,
        lsx.Ngay_Giao_Hang_Du_Kien, lsx.Trang_Thai
    FROM LENH_SAN_XUAT lsx LEFT JOIN KHACH_HANG kh ON kh.id = lsx.id_Khach_Hang
    WHERE lsx.Trang_Thai = N'Đang sản xuất' ORDER BY lsx.Ngay_Tao DESC;
END
GO

CREATE PROCEDURE sp_GetDashboardNotifications AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 3 Ma_Bao_Gia, Ngay_Tao FROM BAO_GIA
    WHERE Trang_Thai = N'Đã duyệt' AND Ngay_Tao >= DATEADD(DAY, -3, GETDATE()) ORDER BY Ngay_Tao DESC;
    SELECT TOP 3 Ten_Nguyen_Lieu, Ton_Kho FROM NGUYEN_LIEU
    WHERE Ton_Kho <= Ton_Kho_Toi_Thieu OR Ton_Kho <= 0 ORDER BY Ton_Kho ASC;
    SELECT TOP 2 Ma_Phieu_Nhap, Ngay_Tao FROM PHIEU_NHAP_KHO
    WHERE Trang_Thai = N'Hoàn thành' AND Ngay_Tao >= DATEADD(DAY, -3, GETDATE()) ORDER BY Ngay_Tao DESC;
    SELECT TOP 3 kh.Ten_Khach_Hang, DATEDIFF(DAY, dbh.Ngay_Den_Han, GETDATE()) AS SoNgay, cn.Con_Lai
    FROM CONG_NO_KHACH_HANG cn
    JOIN KHACH_HANG kh ON kh.id = cn.id_Khach_Hang
    JOIN DON_BAN_HANG dbh ON dbh.id_Khach_Hang = kh.id
    WHERE cn.Con_Lai > 0 AND dbh.Ngay_Den_Han IS NOT NULL AND dbh.Ngay_Den_Han < GETDATE()
    GROUP BY kh.Ten_Khach_Hang, dbh.Ngay_Den_Han, cn.Con_Lai ORDER BY SoNgay DESC;
END
GO

-- =====================================================
-- SP: BÁO GIÁ
-- =====================================================
CREATE PROCEDURE sp_GetQuoteById @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, bg.Ngay_Bao_Gia, bg.So_Con, bg.Gia_Giay_Tan,
        bg.Loi_Nhuan_Phan_Tram, bg.Kich_Thuoc_Thanh_Pham, bg.Khoi_Luong_Giay, bg.Kho_In,
        bg.So_Mau_In, bg.Thoi_Gian_Giao_Hang_Du_Kien, bg.Ten_Loai_Giay, bg.Hieu_Luc_Bao_Gia_Ngay,
        bg.Nguoi_Tao, bg.Trang_Thai, kh.Ten_Khach_Hang, kh.Dia_Chi,
        ct.So_Luong, ct.Tien_Khuon_Be, ct.Tien_Can_Mang
    FROM BAO_GIA bg
    LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
    WHERE bg.id = @QuoteId;
END
GO

CREATE PROCEDURE sp_GenerateQuoteCode @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @count INT;
    SELECT @count = COUNT(*) FROM BAO_GIA;
    SET @NextCode = N'BG-' + RIGHT(N'0000' + CAST(@count + 1 AS NVARCHAR), 4);
END
GO

CREATE PROCEDURE sp_SaveNewQuote
    @CustomerName NVARCHAR(200), @CustomerAddress NVARCHAR(500), @CreatedBy NVARCHAR(100),
    @QuoteCode NVARCHAR(50), @ProductName NVARCHAR(200), @QuoteDate DATE,
    @LayoutCount INT, @PaperPricePerTon DECIMAL(18,2), @ProfitPercent DECIMAL(5,2),
    @ProductSize NVARCHAR(100), @PaperGsm NVARCHAR(100), @PrintSize NVARCHAR(100),
    @ColorCount INT, @DeliveryTime NVARCHAR(100), @ValidityDays INT, @PaperType NVARCHAR(100),
    @Quantity INT, @CostPaper DECIMAL(18,2), @CostPlate DECIMAL(18,2), @CostPrint DECIMAL(18,2),
    @CostLaminate DECIMAL(18,2), @CostMetalize DECIMAL(18,2), @CostUV DECIMAL(18,2),
    @CostDie DECIMAL(18,2), @CostDieMold DECIMAL(18,2), @CostGlue DECIMAL(18,2),
    @CostRibbon DECIMAL(18,2), @CostButton DECIMAL(18,2), @CostBox DECIMAL(18,2),
    @CostDelivery DECIMAL(18,2), @CostProof DECIMAL(18,2), @TotalCost DECIMAL(18,2),
    @CostPerUnit DECIMAL(18,2), @PricePerUnit DECIMAL(18,2), @TotalQuotePrice DECIMAL(18,2),
    @NewQuoteId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @customerId INT;
        INSERT INTO KHACH_HANG (Ma_KH, Ten_Khach_Hang, Dia_Chi, Ngay_Tao, Nguoi_Tao)
        VALUES (N'KH' + CAST(NEWID() AS NVARCHAR(36)), @CustomerName, @CustomerAddress, GETDATE(), @CreatedBy);
        SET @customerId = SCOPE_IDENTITY();
        INSERT INTO BAO_GIA (Ma_Bao_Gia, id_Khach_Hang, Ten_San_Pham, Ngay_Bao_Gia, So_Con, Gia_Giay_Tan,
            Loi_Nhuan_Phan_Tram, Kich_Thuoc_Thanh_Pham, Khoi_Luong_Giay, Kho_In, So_Mau_In,
            Thoi_Gian_Giao_Hang_Du_Kien, Hieu_Luc_Bao_Gia_Ngay, Ten_Loai_Giay, Nguoi_Tao)
        VALUES (@QuoteCode, @customerId, @ProductName, @QuoteDate, @LayoutCount, @PaperPricePerTon,
            @ProfitPercent, @ProductSize, @PaperGsm, @PrintSize, @ColorCount, @DeliveryTime,
            @ValidityDays, @PaperType, @CreatedBy);
        SET @NewQuoteId = SCOPE_IDENTITY();
        INSERT INTO CHI_TIET_BAO_GIA (id_Bao_Gia, So_Luong, Tien_Giay, Tien_Kem, Tien_In,
            Tien_Can_Mang, Tien_Metalize, Tien_UV, Tien_Be, Tien_Khuon_Be, Tien_Dan, Tien_Day,
            Tien_Nut, Tien_Thung, Tien_Xe_Giao, Tien_Proof, Tong_Gia_Thanh, Gia_Moi_Cai,
            Gia_Bao_Khach, Tong_Gia_Bao_Khach)
        VALUES (@NewQuoteId, @Quantity, @CostPaper, @CostPlate, @CostPrint, @CostLaminate,
            @CostMetalize, @CostUV, @CostDie, @CostDieMold, @CostGlue, @CostRibbon, @CostButton,
            @CostBox, @CostDelivery, @CostProof, @TotalCost, @CostPerUnit, @PricePerUnit, @TotalQuotePrice);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg2 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg2, 16, 1);
    END CATCH
END
GO
ALTER PROCEDURE sp_SaveNewQuote
    @CustomerId       INT           = 0,   -- [NEW] truyền ID nếu đã có KH
    @CustomerName     NVARCHAR(200),
    @CustomerAddress  NVARCHAR(500),
    @CreatedBy        NVARCHAR(100),
    @QuoteCode        NVARCHAR(50),
    @ProductName      NVARCHAR(200),
    @QuoteDate        DATE,
    @LayoutCount      INT,
    @PaperPricePerTon DECIMAL(18,2),
    @ProfitPercent    DECIMAL(5,2),
    @ProductSize      NVARCHAR(100),
    @PaperGsm         NVARCHAR(100),
    @PrintSize        NVARCHAR(100),
    @ColorCount       INT,
    @DeliveryTime     NVARCHAR(100),
    @ValidityDays     INT,
    @PaperType        NVARCHAR(100),
    @Quantity         INT,
    @CostPaper        DECIMAL(18,2), @CostPlate    DECIMAL(18,2), @CostPrint    DECIMAL(18,2),
    @CostLaminate     DECIMAL(18,2), @CostMetalize  DECIMAL(18,2), @CostUV      DECIMAL(18,2),
    @CostDie          DECIMAL(18,2), @CostDieMold   DECIMAL(18,2), @CostGlue    DECIMAL(18,2),
    @CostRibbon       DECIMAL(18,2), @CostButton    DECIMAL(18,2), @CostBox     DECIMAL(18,2),
    @CostDelivery     DECIMAL(18,2), @CostProof     DECIMAL(18,2), @TotalCost   DECIMAL(18,2),
    @CostPerUnit      DECIMAL(18,2), @PricePerUnit  DECIMAL(18,2), @TotalQuotePrice DECIMAL(18,2),
    @NewQuoteId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- [FIX] Chỉ tạo KH mới khi @CustomerId = 0 (khách lẻ gõ tay)
        DECLARE @resolvedCustomerId INT = @CustomerId;

        IF @resolvedCustomerId = 0
        BEGIN
            DECLARE @newCode NVARCHAR(20);
            DECLARE @nextNum INT;
            SELECT @nextNum = ISNULL(MAX(
                CASE
                    WHEN Ma_KH LIKE N'KH\_%' ESCAPE N'\'
                        THEN CAST(SUBSTRING(Ma_KH, 4, 10) AS INT)
                    WHEN Ma_KH LIKE N'KH[0-9]%'
                        THEN CAST(SUBSTRING(Ma_KH, 3, 10) AS INT)
                    ELSE 0
                END), 0) + 1
            FROM KHACH_HANG WHERE Ma_KH LIKE N'KH%';

            SET @newCode = CASE WHEN @nextNum > 99
                THEN N'KH_' + CAST(@nextNum AS NVARCHAR)
                ELSE N'KH_' + RIGHT(N'00' + CAST(@nextNum AS NVARCHAR), 2)
            END;

            INSERT INTO KHACH_HANG (Ma_KH, Ten_Khach_Hang, Dia_Chi, Trang_Thai, Ngay_Tao, Nguoi_Tao)
            VALUES (@newCode, @CustomerName, @CustomerAddress, N'Hoạt động', GETDATE(), @CreatedBy);

            SET @resolvedCustomerId = SCOPE_IDENTITY();
        END

        INSERT INTO BAO_GIA (
            Ma_Bao_Gia, id_Khach_Hang, Ten_San_Pham, Ngay_Bao_Gia, So_Con,
            Gia_Giay_Tan, Loi_Nhuan_Phan_Tram, Kich_Thuoc_Thanh_Pham,
            Khoi_Luong_Giay, Kho_In, So_Mau_In, Thoi_Gian_Giao_Hang_Du_Kien,
            Hieu_Luc_Bao_Gia_Ngay, Ten_Loai_Giay, Nguoi_Tao)
        VALUES (
            @QuoteCode, @resolvedCustomerId, @ProductName, @QuoteDate, @LayoutCount,
            @PaperPricePerTon, @ProfitPercent, @ProductSize, @PaperGsm, @PrintSize,
            @ColorCount, @DeliveryTime, @ValidityDays, @PaperType, @CreatedBy);

        SET @NewQuoteId = SCOPE_IDENTITY();

        INSERT INTO CHI_TIET_BAO_GIA (
            id_Bao_Gia, So_Luong, Tien_Giay, Tien_Kem, Tien_In,
            Tien_Can_Mang, Tien_Metalize, Tien_UV, Tien_Be, Tien_Khuon_Be,
            Tien_Dan, Tien_Day, Tien_Nut, Tien_Thung, Tien_Xe_Giao, Tien_Proof,
            Tong_Gia_Thanh, Gia_Moi_Cai, Gia_Bao_Khach, Tong_Gia_Bao_Khach)
        VALUES (
            @NewQuoteId, @Quantity, @CostPaper, @CostPlate, @CostPrint,
            @CostLaminate, @CostMetalize, @CostUV, @CostDie, @CostDieMold,
            @CostGlue, @CostRibbon, @CostButton, @CostBox, @CostDelivery, @CostProof,
            @TotalCost, @CostPerUnit, @PricePerUnit, @TotalQuotePrice);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @err NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@err, 16, 1);
    END CATCH
END
GO
CREATE PROCEDURE sp_UpdateQuoteDetail
    @QuoteId INT, @ValidityDays INT, @PaperType NVARCHAR(100), @Quantity INT,
    @CostPaper DECIMAL(18,2), @CostPlate DECIMAL(18,2), @CostPrint DECIMAL(18,2),
    @CostLaminate DECIMAL(18,2), @CostMetalize DECIMAL(18,2), @CostUV DECIMAL(18,2),
    @CostDie DECIMAL(18,2), @CostDieMold DECIMAL(18,2), @CostGlue DECIMAL(18,2),
    @CostRibbon DECIMAL(18,2), @CostButton DECIMAL(18,2), @CostBox DECIMAL(18,2),
    @CostDelivery DECIMAL(18,2), @CostProof DECIMAL(18,2), @TotalCost DECIMAL(18,2),
    @CostPerUnit DECIMAL(18,2), @PricePerUnit DECIMAL(18,2), @TotalQuotePrice DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE BAO_GIA SET Hieu_Luc_Bao_Gia_Ngay = @ValidityDays, Ten_Loai_Giay = @PaperType WHERE id = @QuoteId;
        UPDATE CHI_TIET_BAO_GIA SET So_Luong=@Quantity, Tien_Giay=@CostPaper, Tien_Kem=@CostPlate,
            Tien_In=@CostPrint, Tien_Can_Mang=@CostLaminate, Tien_Metalize=@CostMetalize, Tien_UV=@CostUV,
            Tien_Be=@CostDie, Tien_Khuon_Be=@CostDieMold, Tien_Dan=@CostGlue, Tien_Day=@CostRibbon,
            Tien_Nut=@CostButton, Tien_Thung=@CostBox, Tien_Xe_Giao=@CostDelivery, Tien_Proof=@CostProof,
            Tong_Gia_Thanh=@TotalCost, Gia_Moi_Cai=@CostPerUnit, Gia_Bao_Khach=@PricePerUnit,
            Tong_Gia_Bao_Khach=@TotalQuotePrice
        WHERE id_Bao_Gia = @QuoteId;
        IF @@ROWCOUNT = 0
            INSERT INTO CHI_TIET_BAO_GIA (id_Bao_Gia, So_Luong, Tien_Giay, Tien_Kem, Tien_In, Tien_Can_Mang,
                Tien_Metalize, Tien_UV, Tien_Be, Tien_Khuon_Be, Tien_Dan, Tien_Day, Tien_Nut, Tien_Thung,
                Tien_Xe_Giao, Tien_Proof, Tong_Gia_Thanh, Gia_Moi_Cai, Gia_Bao_Khach, Tong_Gia_Bao_Khach)
            VALUES (@QuoteId, @Quantity, @CostPaper, @CostPlate, @CostPrint, @CostLaminate, @CostMetalize,
                @CostUV, @CostDie, @CostDieMold, @CostGlue, @CostRibbon, @CostButton, @CostBox, @CostDelivery,
                @CostProof, @TotalCost, @CostPerUnit, @PricePerUnit, @TotalQuotePrice);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg3 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg3, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_GetQuoteDetailById @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT So_Luong, Tien_Giay, Tien_Kem, Tien_In, Tien_Can_Mang, Tien_Metalize, Tien_UV,
        Tien_Be, Tien_Khuon_Be, Tien_Dan, Tien_Day, Tien_Nut, Tien_Thung, Tien_Xe_Giao, Tien_Proof,
        Tong_Gia_Thanh, Gia_Moi_Cai, Gia_Bao_Khach, Tong_Gia_Bao_Khach
    FROM CHI_TIET_BAO_GIA WHERE id_Bao_Gia = @QuoteId;
END
GO

CREATE PROCEDURE sp_GetQuoteHeaderById @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ma_Bao_Gia, CONVERT(NVARCHAR, bg.Ngay_Bao_Gia, 103) AS Ngay_Bao_Gia,
        bg.Ten_San_Pham, ISNULL(kh.Ten_Khach_Hang, N'') AS Ten_Khach_Hang
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    WHERE bg.id = @QuoteId;
END
GO

CREATE PROCEDURE sp_GetQuoteList
    @Search   NVARCHAR(200) = N'',
    @FromDate DATE,
    @ToDate   DATE,
    @Offset   INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ToDateNext DATE = DATEADD(DAY, 1, @ToDate);
    SELECT COUNT(DISTINCT bg.id) AS TotalRecord
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
    WHERE bg.Ngay_Bao_Gia >= @FromDate AND bg.Ngay_Bao_Gia < @ToDateNext
      AND (@Search = N'' OR bg.Ma_Bao_Gia LIKE N'%'+@Search+N'%'
           OR kh.Ten_Khach_Hang LIKE N'%'+@Search+N'%' OR bg.Ten_San_Pham LIKE N'%'+@Search+N'%');
    SELECT ROW_NUMBER() OVER (ORDER BY bg.id DESC) AS STT,
        bg.id AS IDBaoGia, bg.Ma_Bao_Gia AS [Số báo giá], bg.Ngay_Bao_Gia AS [Ngày BG],
        kh.Ten_Khach_Hang AS [Khách hàng], bg.Ten_San_Pham AS [Sản phẩm],
        ISNULL(ct.So_Luong, 0) AS [Số lượng], ISNULL(ct.Tong_Gia_Bao_Khach, 0) AS [Tổng báo khách],
        bg.Trang_Thai AS [Trạng thái]
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
    LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id
             ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.Ngay_Bao_Gia >= @FromDate AND bg.Ngay_Bao_Gia < @ToDateNext
      AND (@Search = N'' OR bg.Ma_Bao_Gia LIKE N'%'+@Search+N'%'
           OR kh.Ten_Khach_Hang LIKE N'%'+@Search+N'%' OR bg.Ten_San_Pham LIKE N'%'+@Search+N'%')
    ORDER BY bg.id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE PROCEDURE sp_UpdateQuoteStatus @QuoteId INT, @NewStatus NVARCHAR(50) AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE BAO_GIA SET Trang_Thai = @NewStatus WHERE id = @QuoteId;
        IF @NewStatus = N'Hoàn thành'
            UPDATE LENH_SAN_XUAT SET Trang_Thai = N'Hoàn thành' WHERE id_Bao_Gia = @QuoteId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg4 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg4, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_GetQuoteForExcel @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ma_Bao_Gia, CONVERT(NVARCHAR, bg.Ngay_Bao_Gia, 103) AS Ngay_Bao_Gia,
        bg.Hieu_Luc_Bao_Gia_Ngay, bg.Thoi_Gian_Giao_Hang_Du_Kien, bg.Ten_San_Pham,
        bg.Kich_Thuoc_Thanh_Pham, ISNULL(bg.Ten_Loai_Giay, bg.Khoi_Luong_Giay) AS Ten_Loai_Giay,
        ISNULL(kh.Ten_Khach_Hang, N'') AS Ten_Khach_Hang, ISNULL(kh.Dia_Chi, N'') AS Dia_Chi,
        ISNULL(kh.MST, N'') AS MST, ISNULL(kh.Nguoi_Lien_He, N'') AS Nguoi_Lien_He
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang WHERE bg.id = @QuoteId;
    SELECT So_Luong, Gia_Bao_Khach FROM CHI_TIET_BAO_GIA WHERE id_Bao_Gia = @QuoteId;
END
GO

-- =====================================================
-- SP: THỐNG KÊ BÁO GIÁ
-- =====================================================
CREATE PROCEDURE sp_GetQuoteStatsSummary @Month INT, @Year INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS TotalQuotes,
        SUM(CASE WHEN Trang_Thai = N'Chờ duyệt' THEN 1 ELSE 0 END) AS Pending,
        SUM(CASE WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành') THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN 1 ELSE 0 END) AS Cancelled
    FROM BAO_GIA WHERE MONTH(Ngay_Bao_Gia) = @Month AND YEAR(Ngay_Bao_Gia) = @Year;
END
GO

CREATE PROCEDURE sp_GetQuoteTrendByDay @Month INT, @Year INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT DAY(Ngay_Bao_Gia) AS DayOfMonth, COUNT(*) AS QuoteCount
    FROM BAO_GIA WHERE MONTH(Ngay_Bao_Gia) = @Month AND YEAR(Ngay_Bao_Gia) = @Year
    GROUP BY DAY(Ngay_Bao_Gia) ORDER BY DayOfMonth;
END
GO

IF OBJECT_ID('sp_GetQuoteStatusGroups','P') IS NOT NULL DROP PROCEDURE sp_GetQuoteStatusGroups;
GO
CREATE PROCEDURE sp_GetQuoteStatusGroups @Month INT, @Year INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký') THEN N'Đã duyệt'
                WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN N'Huỷ'
                ELSE Trang_Thai END AS StatusGroup, COUNT(*) AS QuoteCount
    FROM BAO_GIA
    WHERE MONTH(Ngay_Bao_Gia) = @Month AND YEAR(Ngay_Bao_Gia) = @Year
    GROUP BY CASE WHEN Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký') THEN N'Đã duyệt'
                  WHEN Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN N'Huỷ' ELSE Trang_Thai END
    ORDER BY QuoteCount DESC;
END
GO

IF OBJECT_ID('sp_GetQuoteDetailByCustomer','P') IS NOT NULL DROP PROCEDURE sp_GetQuoteDetailByCustomer;
GO
CREATE PROCEDURE sp_GetQuoteDetailByCustomer
    @Keyword NVARCHAR(200) = N'',
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT kh.Ten_Khach_Hang, COUNT(DISTINCT bg.id) AS QuoteCount,
        SUM(CASE WHEN bg.Trang_Thai IN (N'Đã duyệt',N'Khách duyệt',N'Đang sản xuất',N'Hoàn thành',N'Đã ký') THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN bg.Trang_Thai = N'Chờ duyệt' THEN 1 ELSE 0 END) AS Pending,
        SUM(CASE WHEN bg.Trang_Thai IN (N'Huỷ',N'Hủy',N'Từ chối') THEN 1 ELSE 0 END) AS Cancelled,
        ISNULL(SUM(ct.MaxGia), 0) AS TotalValue
    FROM BAO_GIA bg JOIN KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
    LEFT JOIN (SELECT id_Bao_Gia, MAX(Tong_Gia_Bao_Khach) AS MaxGia FROM CHI_TIET_BAO_GIA GROUP BY id_Bao_Gia) ct ON ct.id_Bao_Gia = bg.id
    WHERE MONTH(bg.Ngay_Bao_Gia) = @Month AND YEAR(bg.Ngay_Bao_Gia) = @Year
      AND (@Keyword = N'' OR kh.Ten_Khach_Hang LIKE N'%' + @Keyword + N'%')
    GROUP BY kh.Ten_Khach_Hang
    ORDER BY TotalValue DESC;
END
GO

-- =====================================================
-- SP: MUA HÀNG
-- =====================================================
CREATE PROCEDURE sp_GenerateOrderCode @Year INT, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(20) = N'DH-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Don_Hang, LEN(@prefix)+1, 10) AS INT)), 0) + 1
    FROM DON_DAT_HANG_NCC WHERE Ma_Don_Hang LIKE @prefix + N'%';
    SET @NextCode = @prefix + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
END
GO

CREATE PROCEDURE sp_GetSuppliers AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, Ma_NCC, Ten_NCC, Dia_Chi, MST, Dien_Thoai FROM NHA_CUNG_CAP ORDER BY Ma_NCC;
END
GO

CREATE PROCEDURE sp_CheckOrCreateSupplier
    @Name NVARCHAR(200), @Address NVARCHAR(500), @TaxCode NVARCHAR(50),
    @SupplierId INT OUTPUT, @SupplierCode NVARCHAR(50) OUTPUT, @IsNew BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 @SupplierId = id, @SupplierCode = Ma_NCC FROM NHA_CUNG_CAP WHERE Ten_NCC = @Name;
    IF @SupplierId IS NOT NULL BEGIN SET @IsNew = 0; RETURN; END
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_NCC, 4, 10) AS INT)), 0) + 1 FROM NHA_CUNG_CAP WHERE Ma_NCC LIKE N'NCC%';
    SET @SupplierCode = N'NCC' + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
    INSERT INTO NHA_CUNG_CAP (Ma_NCC, Ten_NCC, Dia_Chi, MST, Dien_Thoai) VALUES (@SupplierCode, @Name, @Address, @TaxCode, N'');
    SET @SupplierId = SCOPE_IDENTITY(); SET @IsNew = 1;
END
GO

CREATE PROCEDURE sp_DeleteSupplier @SupplierId INT, @OrderCount INT OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    SELECT @OrderCount = COUNT(*) FROM DON_DAT_HANG_NCC WHERE id_Nha_Cung_Cap = @SupplierId;
    IF @OrderCount = 0 DELETE FROM NHA_CUNG_CAP WHERE id = @SupplierId;
END
GO

CREATE PROCEDURE sp_GetMaterialById @MaterialId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap FROM NGUYEN_LIEU WHERE id = @MaterialId;
END
GO

CREATE PROCEDURE sp_GenerateMaterialCode @ExtraCount INT = 0, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Nguyen_Lieu, 3, 10) AS INT)), 0) + 1 + @ExtraCount
    FROM NGUYEN_LIEU WHERE Ma_Nguyen_Lieu LIKE N'MH%';
    SET @NextCode = N'MH' + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
END
GO

CREATE PROCEDURE sp_SavePurchaseOrder
    @OrderCode NVARCHAR(50), @SupplierId INT, @OrderDate DATE, @DeliveryDate DATE,
    @PaymentMethod NVARCHAR(100), @DebtDays INT, @OrderStatus NVARCHAR(50),
    @DeliveryLocation NVARCHAR(500), @AttachedFile NVARCHAR(500), @GhiChu NVARCHAR(MAX) = NULL,
    @GrandTotal DECIMAL(18,2),
    @Details XML, @NewOrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO DON_DAT_HANG_NCC (Ma_Don_Hang, id_Nha_Cung_Cap, Ngay_Dat_Hang, Ngay_Giao_Hang,
            Dieu_Khoan_Thanh_Toan, So_Ngay_No, Trang_Thai, Dia_Diem_Giao_Hang, File_Dinh_Kem, Ghi_Chu, Tong_Tien)
        VALUES (@OrderCode, @SupplierId, @OrderDate, @DeliveryDate, @PaymentMethod, @DebtDays,
            @OrderStatus, @DeliveryLocation, @AttachedFile, @GhiChu, @GrandTotal);
        SET @NewOrderId = SCOPE_IDENTITY();

        -- [FIX] Duyệt TẤT CẢ dòng trong XML bằng cursor
        DECLARE @MaterialId INT, @MaterialCode NVARCHAR(50), @MaterialName NVARCHAR(200),
            @Unit NVARCHAR(50), @Qty FLOAT, @UnitPrice FLOAT, @LineTotal FLOAT,
            @VatRate FLOAT, @VatAmount FLOAT, @ExistingId INT;
        DECLARE item_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT d.value('(MaterialId)[1]','INT'),
                   d.value('(MaterialCode)[1]','NVARCHAR(50)'),
                   d.value('(MaterialName)[1]','NVARCHAR(200)'),
                   d.value('(Unit)[1]','NVARCHAR(50)'),
                   d.value('(Qty)[1]','FLOAT'),
                   d.value('(UnitPrice)[1]','FLOAT'),
                   d.value('(LineTotal)[1]','FLOAT'),
                   d.value('(VatRate)[1]','FLOAT'),
                   d.value('(VatAmount)[1]','FLOAT')
            FROM @Details.nodes('/Details/Item') AS T(d);
        OPEN item_cursor;
        FETCH NEXT FROM item_cursor INTO @MaterialId, @MaterialCode, @MaterialName, @Unit, @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Tìm hoặc tạo nguyên liệu mới
            SET @ExistingId = NULL;
            IF @MaterialId = 0 OR @MaterialId IS NULL
            BEGIN
                SELECT @ExistingId = id FROM NGUYEN_LIEU WHERE Ma_Nguyen_Lieu = @MaterialCode;
                IF @ExistingId IS NOT NULL
                    SET @MaterialId = @ExistingId;
                ELSE
                BEGIN
                    INSERT INTO NGUYEN_LIEU (Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap, Ton_Kho, Ton_Kho_Toi_Thieu)
                    VALUES (@MaterialCode, @MaterialName, @Unit, @UnitPrice, 0, 0);
                    SET @MaterialId = SCOPE_IDENTITY();
                END
            END
            -- Chèn chi tiết đơn hàng
            INSERT INTO CHI_TIET_DON_HANG_NCC (id_Don_Dat_Hang, id_Nguyen_Lieu, So_Luong, So_Luong_Da_Nhan,
                Don_Gia, Thanh_Tien, Phan_Tram_Thue_GTGT, Tien_Thue_GTGT)
            VALUES (@NewOrderId, @MaterialId, @Qty, 0, @UnitPrice, @LineTotal, @VatRate, @VatAmount);
            FETCH NEXT FROM item_cursor INTO @MaterialId, @MaterialCode, @MaterialName, @Unit, @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
        END
        CLOSE item_cursor;
        DEALLOCATE item_cursor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg5 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg5, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- SP: NHẬP KHO
-- =====================================================
CREATE PROCEDURE sp_GenerateReceiptCode @Year INT, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(20) = N'PN-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Nhap, LEN(@prefix)+1, 10) AS INT)), 0) + 1
    FROM PHIEU_NHAP_KHO WHERE Ma_Phieu_Nhap LIKE @prefix + N'%';
    SET @NextCode = @prefix + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
END
GO

CREATE PROCEDURE sp_GetPendingOrders AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.id, d.Ma_Don_Hang, n.Ten_NCC, n.id AS SupplierId, n.Dia_Chi AS SupplierAddress
    FROM DON_DAT_HANG_NCC d JOIN NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
    WHERE d.Trang_Thai NOT IN (N'Hoàn thành', N'Hủy') ORDER BY d.Ngay_Dat_Hang DESC;
END
GO

CREATE PROCEDURE sp_GetOrderReceiveData @OrderId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.Dieu_Khoan_Thanh_Toan, n.Ten_NCC, n.Dia_Chi, n.id AS SupplierId, n.Ma_NCC
    FROM DON_DAT_HANG_NCC d JOIN NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id WHERE d.id = @OrderId;
    SELECT ct.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh,
        ct.So_Luong AS OrderQty, ct.Don_Gia, ct.Thanh_Tien, ct.Phan_Tram_Thue_GTGT, ct.Tien_Thue_GTGT, nl.id AS MaterialId
    FROM CHI_TIET_DON_HANG_NCC ct JOIN NGUYEN_LIEU nl ON ct.id_Nguyen_Lieu = nl.id WHERE ct.id_Don_Dat_Hang = @OrderId;
END
GO

CREATE PROCEDURE sp_SaveInventoryReceipt
    @ReceiptId INT, @ReceiptCode NVARCHAR(50), @OrderId INT, @SupplierId INT,
    @DocumentDate DATE, @AccountingDate DATE, @CreatedBy NVARCHAR(100),
    @ReceiveType NVARCHAR(100), @PaymentMethod NVARCHAR(100), @PaymentStatus NVARCHAR(100),
    @HasInvoice BIT, @InvoiceForm NVARCHAR(50), @InvoiceSymbol NVARCHAR(50),
    @InvoiceNumber NVARCHAR(50), @InvoiceDate DATE, @InvoiceFile NVARCHAR(500), @NewReceiptId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ReceiptId = 0
    BEGIN
        INSERT INTO PHIEU_NHAP_KHO (Ma_Phieu_Nhap, id_Don_Dat_Hang, id_Nha_Cung_Cap, Ngay_Nhap,
            Ngay_Chung_Tu, Ngay_Hach_Toan, Nguoi_Nhap, Loai_Nhap, Hinh_Thuc_Thanh_Toan, Trang_Thai_TT,
            Co_Hoa_Don, Mau_So_Hoa_Don, Ky_Hieu_Hoa_Don, So_Hoa_Don, Ngay_Hoa_Don, File_Hoa_Don, Trang_Thai)
        VALUES (@ReceiptCode, @OrderId, @SupplierId, @DocumentDate, @DocumentDate, @AccountingDate, @CreatedBy,
            @ReceiveType, @PaymentMethod, @PaymentStatus, @HasInvoice, @InvoiceForm, @InvoiceSymbol,
            @InvoiceNumber, @InvoiceDate, @InvoiceFile, N'Chưa ghi sổ');
        SET @NewReceiptId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE PHIEU_NHAP_KHO SET Ngay_Nhap=@DocumentDate, Ngay_Chung_Tu=@DocumentDate,
            Ngay_Hach_Toan=@AccountingDate, Loai_Nhap=@ReceiveType, Hinh_Thuc_Thanh_Toan=@PaymentMethod,
            Trang_Thai_TT=@PaymentStatus, Co_Hoa_Don=@HasInvoice, Mau_So_Hoa_Don=@InvoiceForm,
            Ky_Hieu_Hoa_Don=@InvoiceSymbol, So_Hoa_Don=@InvoiceNumber, Ngay_Hoa_Don=@InvoiceDate, File_Hoa_Don=@InvoiceFile
        WHERE id = @ReceiptId;
        SET @NewReceiptId = @ReceiptId;
    END
END
GO

-- =====================================================
-- [FIX] Thêm kiểm tra trạng thái ĐÃ GHI SỔ trước khi post
-- =====================================================
CREATE PROCEDURE sp_PostInventoryReceipt @ReceiptId INT, @SupplierId INT, @Details XML AS
BEGIN
    SET NOCOUNT ON;

    -- [FIX] Kiểm tra phiếu đã được ghi sổ chưa
    IF EXISTS (SELECT 1 FROM PHIEU_NHAP_KHO WHERE id = @ReceiptId AND Trang_Thai = N'Đã ghi sổ')
    BEGIN
        RAISERROR(N'Phiếu nhập kho này đã được ghi sổ. Không thể ghi sổ lại.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @totalAmount FLOAT=0, @materialId INT, @detailId INT,
            @receivedQty FLOAT, @unitPrice FLOAT, @vatAmount FLOAT, @lineTotal FLOAT;
        DECLARE item_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.value('(MaterialId)[1]','INT'), d.value('(DetailId)[1]','INT'),
            d.value('(ReceivedQty)[1]','FLOAT'), d.value('(UnitPrice)[1]','FLOAT'),
            d.value('(VatAmount)[1]','FLOAT'), d.value('(LineTotal)[1]','FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);
        OPEN item_cursor;
        FETCH NEXT FROM item_cursor INTO @materialId, @detailId, @receivedQty, @unitPrice, @vatAmount, @lineTotal;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @materialId > 0 AND @receivedQty > 0
            BEGIN
                INSERT INTO CHI_TIET_PHIEU_NHAP (id_Phieu_Nhap, id_Nguyen_Lieu, So_Luong_Nhap, Don_Gia_Nhap, Thanh_Tien)
                VALUES (@ReceiptId, @materialId, @receivedQty, @unitPrice, @lineTotal);
                UPDATE NGUYEN_LIEU SET Ton_Kho = ISNULL(Ton_Kho, 0) + @receivedQty, Gia_Nhap = @unitPrice WHERE id = @materialId;
            END
            IF @detailId > 0 AND @receivedQty > 0
                UPDATE CHI_TIET_DON_HANG_NCC SET So_Luong_Da_Nhan = ISNULL(So_Luong_Da_Nhan, 0) + @receivedQty WHERE id = @detailId;
            SET @totalAmount = @totalAmount + @lineTotal + @vatAmount;
            FETCH NEXT FROM item_cursor INTO @materialId, @detailId, @receivedQty, @unitPrice, @vatAmount, @lineTotal;
        END
        CLOSE item_cursor; DEALLOCATE item_cursor;
        IF EXISTS (SELECT 1 FROM CONG_NO_NCC WHERE id_Nha_Cung_Cap = @SupplierId)
            UPDATE CONG_NO_NCC SET Tong_No = Tong_No + @totalAmount, Ngay_Cap_Nhat = GETDATE() WHERE id_Nha_Cung_Cap = @SupplierId;
        ELSE
            INSERT INTO CONG_NO_NCC (id_Nha_Cung_Cap, Tong_No, Da_Tra, Ngay_Cap_Nhat) VALUES (@SupplierId, @totalAmount, 0, GETDATE());
        UPDATE PHIEU_NHAP_KHO SET Trang_Thai = N'Đã ghi sổ' WHERE id = @ReceiptId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg6 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg6, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- SP: THANH TOÁN NCC
-- =====================================================
CREATE PROCEDURE sp_GetSuppliersForPayment AS
BEGIN SET NOCOUNT ON; SELECT id, Ma_NCC, Ten_NCC FROM NHA_CUNG_CAP ORDER BY Ma_NCC; END
GO

CREATE PROCEDURE sp_GetSupplierDebtList @SupplierId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT dh.id AS OrderId, dh.Ma_Don_Hang AS OrderCode, dh.Ngay_Dat_Hang AS OrderDate,
        N'Đơn hàng ' + dh.Ma_Don_Hang AS Description, dh.Tong_Tien AS TotalAmount,
        ISNULL((SELECT SUM(So_Tien) FROM THANH_TOAN_NCC WHERE id_Don_Dat_Hang = dh.id), 0) AS PaidAmount
    FROM DON_DAT_HANG_NCC dh
    WHERE dh.id_Nha_Cung_Cap = @SupplierId
      AND dh.Tong_Tien > ISNULL((SELECT SUM(So_Tien) FROM THANH_TOAN_NCC WHERE id_Don_Dat_Hang = dh.id), 0)
    ORDER BY dh.Ngay_Dat_Hang DESC;
END
GO

CREATE PROCEDURE sp_GeneratePaymentCode @Year INT, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(20) = N'TT-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Thanh_Toan, LEN(@prefix)+1, 10) AS INT)), 0) + 1
    FROM THANH_TOAN_NCC WHERE Ma_Thanh_Toan LIKE @prefix + N'%';
    SET @NextCode = @prefix + RIGHT(N'000' + CAST(@nextNum AS NVARCHAR), 3);
END
GO

CREATE PROCEDURE sp_SaveSupplierPayment
    @SupplierId INT, @PaymentDate DATE, @PaymentMethod NVARCHAR(100), @Details XML, @TotalPaid DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        SET @TotalPaid = 0;
        DECLARE @orderId INT, @paymentAmount DECIMAL(18,2), @paymentCode NVARCHAR(20);
        DECLARE @prefix2 NVARCHAR(20) = N'TT-' + CAST(YEAR(@PaymentDate) AS NVARCHAR) + N'-';
        DECLARE @nextNum2 INT;
        SELECT @nextNum2 = ISNULL(MAX(CAST(SUBSTRING(Ma_Thanh_Toan, LEN(@prefix2)+1, 10) AS INT)), 0)
        FROM THANH_TOAN_NCC WHERE Ma_Thanh_Toan LIKE @prefix2 + N'%';
        DECLARE payment_cursor CURSOR FOR
        SELECT d.value('(OrderId)[1]','INT'), d.value('(PaymentAmount)[1]','DECIMAL(18,2)')
        FROM @Details.nodes('/Details/Item') AS T(d)
        WHERE d.value('(PaymentAmount)[1]','DECIMAL(18,2)') > 0;
        OPEN payment_cursor; FETCH NEXT FROM payment_cursor INTO @orderId, @paymentAmount;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @nextNum2 = @nextNum2 + 1;
            SET @paymentCode = @prefix2 + RIGHT(N'000' + CAST(@nextNum2 AS NVARCHAR), 3);
            INSERT INTO THANH_TOAN_NCC (Ma_Thanh_Toan, id_Nha_Cung_Cap, id_Don_Dat_Hang, Ngay_Thanh_Toan, So_Tien, Phuong_Thuc, So_Chung_Tu, Ngay_Chung_Tu)
            VALUES (@paymentCode, @SupplierId, @orderId, @PaymentDate, @paymentAmount, @PaymentMethod, @paymentCode, @PaymentDate);
            SET @TotalPaid = @TotalPaid + @paymentAmount;
            FETCH NEXT FROM payment_cursor INTO @orderId, @paymentAmount;
        END
        CLOSE payment_cursor; DEALLOCATE payment_cursor;
        IF EXISTS (SELECT 1 FROM CONG_NO_NCC WHERE id_Nha_Cung_Cap = @SupplierId)
            UPDATE CONG_NO_NCC SET Da_Tra = Da_Tra + @TotalPaid, Ngay_Cap_Nhat = GETDATE() WHERE id_Nha_Cung_Cap = @SupplierId;
        ELSE
            INSERT INTO CONG_NO_NCC (id_Nha_Cung_Cap, Tong_No, Da_Tra, Ngay_Cap_Nhat) VALUES (@SupplierId, @TotalPaid, @TotalPaid, GETDATE());
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg7 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg7, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- SP: BÁO CÁO MUA HÀNG & KHO
-- =====================================================
CREATE PROCEDURE sp_GetPurchaseSummary AS
BEGIN
    SET NOCOUNT ON;
    SELECT ncc.Ten_NCC AS SupplierName, COUNT(DISTINCT dh.id) AS OrderCount,
        ISNULL(SUM(dh.Tong_Tien), 0) AS TotalAmount,
        ISNULL((SELECT SUM(tt.So_Tien) FROM THANH_TOAN_NCC tt WHERE tt.id_Nha_Cung_Cap = ncc.id), 0) AS PaidAmount
    FROM NHA_CUNG_CAP ncc LEFT JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
    GROUP BY ncc.id, ncc.Ten_NCC HAVING COUNT(DISTINCT dh.id) > 0 ORDER BY ncc.Ten_NCC;
END
GO

-- sp_GetInventoryData: CREATE OR ALTER ở cuối file (đồng bộ + nâng cấp DB cũ).

-- =====================================================
-- SP: CÔNG NỢ
-- =====================================================
CREATE PROCEDURE sp_GetDebtSummary AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(SUM(CASE WHEN Con_Lai > 0 THEN Con_Lai ELSE 0 END), 0) AS TotalReceivable,
        COUNT(CASE WHEN Con_Lai > 0 THEN 1 END) AS CustomerCount FROM CONG_NO_KHACH_HANG;
    SELECT ISNULL(SUM(TotalDebt), 0) AS TotalPayable, ISNULL(SUM(TotalPaid), 0) AS TotalPaid, COUNT(*) AS SupplierCount
    FROM (SELECT ncc.id, SUM(dh.Tong_Tien) AS TotalDebt,
        ISNULL((SELECT SUM(tt.So_Tien) FROM THANH_TOAN_NCC tt WHERE tt.id_Nha_Cung_Cap = ncc.id), 0) AS TotalPaid
        FROM NHA_CUNG_CAP ncc JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id GROUP BY ncc.id) X;
END
GO

-- sp_GetReceivables: CREATE OR ALTER ở cuối file.

CREATE PROCEDURE sp_GetPayables AS
BEGIN
    SET NOCOUNT ON;
    -- Bản rút gọn; định nghĩa đầy đủ: CREATE OR ALTER dbo.sp_GetPayables ở cuối file.
    ;WITH Agg AS (
        SELECT ncc.id, ncc.Ten_NCC, SUM(dh.Tong_Tien) AS TotalDebt,
            ISNULL((SELECT SUM(tt.So_Tien) FROM THANH_TOAN_NCC tt WHERE tt.id_Nha_Cung_Cap = ncc.id), 0) AS TotalPaid
        FROM NHA_CUNG_CAP ncc INNER JOIN DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
        GROUP BY ncc.id, ncc.Ten_NCC
    )
    SELECT id, Ten_NCC, TotalDebt, TotalPaid FROM Agg WHERE TotalDebt - TotalPaid > 0;
END
GO

-- =====================================================
-- SP: BÁO CÁO KHO
-- =====================================================
CREATE PROCEDURE sp_GetDetailLedgerMaterials @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT nl.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, nl.Gia_Nhap
    FROM NGUYEN_LIEU nl
    WHERE nl.id IN (
        SELECT ct.id_Nguyen_Lieu FROM CHI_TIET_PHIEU_NHAP ct JOIN PHIEU_NHAP_KHO pn ON pn.id = ct.id_Phieu_Nhap WHERE pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate
        UNION
        SELECT ct.id_Nguyen_Lieu FROM CHI_TIET_XUAT_KHO_SX ct JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct.id_Phieu_Xuat WHERE px.Ngay_Xuat BETWEEN @FromDate AND @ToDate
    ) ORDER BY nl.Ma_Nguyen_Lieu;
END
GO

CREATE PROCEDURE sp_GetDetailLedgerAgg @MaterialId INT, @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    -- [FIX] Thêm date filter vào JOIN để chỉ tính đúng trong khoảng thời gian
    SELECT ISNULL(SUM(CASE WHEN pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate THEN ct_n.So_Luong_Nhap END), 0) AS TotalStockIn,
        ISNULL(SUM(CASE WHEN px.Ngay_Xuat BETWEEN @FromDate AND @ToDate THEN ct_x.So_Luong_Xuat END), 0) AS TotalStockOut,
        nl.Ton_Kho AS ClosingStock
    FROM NGUYEN_LIEU nl
    LEFT JOIN CHI_TIET_PHIEU_NHAP ct_n ON ct_n.id_Nguyen_Lieu = nl.id
        AND ct_n.id_Phieu_Nhap IN (SELECT id FROM PHIEU_NHAP_KHO WHERE Ngay_Nhap BETWEEN @FromDate AND @ToDate)
    LEFT JOIN PHIEU_NHAP_KHO pn ON pn.id = ct_n.id_Phieu_Nhap
    LEFT JOIN CHI_TIET_XUAT_KHO_SX ct_x ON ct_x.id_Nguyen_Lieu = nl.id
        AND ct_x.id_Phieu_Xuat IN (SELECT id FROM PHIEU_XUAT_KHO_SX WHERE Ngay_Xuat BETWEEN @FromDate AND @ToDate)
    LEFT JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct_x.id_Phieu_Xuat
    WHERE nl.id = @MaterialId GROUP BY nl.Ton_Kho;
END
GO

CREATE PROCEDURE sp_GetDetailLedgerStockIn @MaterialId INT, @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    SELECT pn.Ngay_Nhap, pn.Ma_Phieu_Nhap, ncc.Ten_NCC, ct.So_Luong_Nhap, ct.Don_Gia_Nhap
    FROM CHI_TIET_PHIEU_NHAP ct JOIN PHIEU_NHAP_KHO pn ON pn.id = ct.id_Phieu_Nhap
    LEFT JOIN NHA_CUNG_CAP ncc ON ncc.id = pn.id_Nha_Cung_Cap
    WHERE ct.id_Nguyen_Lieu = @MaterialId AND pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate ORDER BY pn.Ngay_Nhap;
END
GO

CREATE PROCEDURE sp_GetDetailLedgerStockOut @MaterialId INT, @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    SELECT px.Ngay_Xuat, px.Ma_Phieu_Xuat, lsx.Ma_Lenh_SX, ct.So_Luong_Xuat, ct.Don_Gia
    FROM CHI_TIET_XUAT_KHO_SX ct JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct.id_Phieu_Xuat
    LEFT JOIN LENH_SAN_XUAT lsx ON lsx.id = px.id_Lenh_San_Xuat
    WHERE ct.id_Nguyen_Lieu = @MaterialId AND px.Ngay_Xuat BETWEEN @FromDate AND @ToDate ORDER BY px.Ngay_Xuat;
END
GO

CREATE PROCEDURE sp_GetInventorySummary @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    -- [FIX] Thêm date filter vào JOIN để chỉ tính đúng trong khoảng thời gian
    SELECT nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, ISNULL(nl.Ton_Dau, 0) AS OpeningStock,
        ISNULL(SUM(CASE WHEN pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate THEN ct_n.So_Luong_Nhap END), 0) AS StockIn,
        ISNULL(SUM(CASE WHEN px.Ngay_Xuat BETWEEN @FromDate AND @ToDate THEN ct_x.So_Luong_Xuat END), 0) AS StockOut,
        nl.Ton_Kho AS ClosingStock, nl.Gia_Nhap
    FROM NGUYEN_LIEU nl
    LEFT JOIN CHI_TIET_PHIEU_NHAP ct_n ON ct_n.id_Nguyen_Lieu = nl.id
        AND ct_n.id_Phieu_Nhap IN (SELECT id FROM PHIEU_NHAP_KHO WHERE Ngay_Nhap BETWEEN @FromDate AND @ToDate)
    LEFT JOIN PHIEU_NHAP_KHO pn ON pn.id = ct_n.id_Phieu_Nhap
    LEFT JOIN CHI_TIET_XUAT_KHO_SX ct_x ON ct_x.id_Nguyen_Lieu = nl.id
        AND ct_x.id_Phieu_Xuat IN (SELECT id FROM PHIEU_XUAT_KHO_SX WHERE Ngay_Xuat BETWEEN @FromDate AND @ToDate)
    LEFT JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct_x.id_Phieu_Xuat
    GROUP BY nl.id, nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, nl.Ton_Dau, nl.Ton_Kho, nl.Gia_Nhap
    ORDER BY nl.Ma_Nguyen_Lieu;
END
GO

CREATE PROCEDURE sp_GetStockCard @FromDate DATE, @ToDate DATE AS
BEGIN
    SET NOCOUNT ON;
    SELECT nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, ISNULL(nl.Ton_Dau, 0) AS OpeningStock,
        pn.Ngay_Nhap AS TxDate, pn.Ma_Phieu_Nhap AS VoucherCode,
        N'Nhập từ ' + ISNULL(ncc.Ten_NCC, N'NCC') AS Description, ct_n.So_Luong_Nhap AS StockIn, 0 AS StockOut
    FROM CHI_TIET_PHIEU_NHAP ct_n JOIN PHIEU_NHAP_KHO pn ON pn.id = ct_n.id_Phieu_Nhap
    JOIN NGUYEN_LIEU nl ON nl.id = ct_n.id_Nguyen_Lieu LEFT JOIN NHA_CUNG_CAP ncc ON ncc.id = pn.id_Nha_Cung_Cap
    WHERE pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate
    UNION ALL
    SELECT nl.Ma_Nguyen_Lieu, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, ISNULL(nl.Ton_Dau, 0) AS OpeningStock,
        px.Ngay_Xuat AS TxDate, px.Ma_Phieu_Xuat AS VoucherCode,
        N'Xuất cho ' + ISNULL(lsx.Ma_Lenh_SX, N'LSX') AS Description, 0 AS StockIn, ct_x.So_Luong_Xuat AS StockOut
    FROM CHI_TIET_XUAT_KHO_SX ct_x JOIN PHIEU_XUAT_KHO_SX px ON px.id = ct_x.id_Phieu_Xuat
    JOIN NGUYEN_LIEU nl ON nl.id = ct_x.id_Nguyen_Lieu LEFT JOIN LENH_SAN_XUAT lsx ON lsx.id = px.id_Lenh_San_Xuat
    WHERE px.Ngay_Xuat BETWEEN @FromDate AND @ToDate
    ORDER BY Ma_Nguyen_Lieu, TxDate;
END
GO

-- =====================================================
-- SP: THU TIỀN KHÁCH HÀNG
-- =====================================================
CREATE PROCEDURE sp_GetCustomersWithDebt AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT kh.id, kh.Ten_Khach_Hang FROM KHACH_HANG kh
    INNER JOIN DON_BAN_HANG dbh ON dbh.id_Khach_Hang = kh.id
    WHERE ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy' ORDER BY kh.Ten_Khach_Hang;
END
GO

CREATE PROCEDURE sp_GetOrderDebtByCustomer @CustomerId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT dbh.id,
        COALESCE(NULLIF(LTRIM(RTRIM(dbh.So_Hoa_Don)), N''), dbh.Ma_Don_Ban) AS OrderCode,
        dbh.Ma_Don_Ban AS DocumentCode,
        dbh.Ngay_Ban_Hang AS OrderDate,
        ISNULL(ct.Ten_San_Pham, N'(không có chi tiết)') AS ProductName,
        dbh.Tong_Thanh_Toan AS TotalAmount, ISNULL(SUM(pt.So_Tien_Thu), 0) AS Collected,
        dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) AS Remaining
    FROM DON_BAN_HANG dbh LEFT JOIN PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
    LEFT JOIN (SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham FROM CHI_TIET_DON_BAN_HANG GROUP BY id_Don_Ban_Hang) ct ON ct.id_Don_Ban_Hang = dbh.id
    WHERE dbh.id_Khach_Hang = @CustomerId AND ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy'
    GROUP BY dbh.id, dbh.Ma_Don_Ban, dbh.So_Hoa_Don, dbh.Ngay_Ban_Hang, dbh.Tong_Thanh_Toan, ct.Ten_San_Pham
    HAVING dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) > 0 ORDER BY dbh.Ngay_Ban_Hang DESC;
END
GO

CREATE PROCEDURE sp_GenerateCollectionCode @Year INT, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(20) = N'PT-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum INT;
    SELECT @nextNum = ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Thu, LEN(@prefix)+1, 10) AS INT)), 0) + 1
    FROM PHIEU_THU WHERE Ma_Phieu_Thu LIKE @prefix + N'%';
    SET @NextCode = @prefix + RIGHT(N'0000' + CAST(@nextNum AS NVARCHAR), 4);
END
GO

CREATE PROCEDURE sp_SavePaymentCollection
    @CustomerId INT, @PaymentDate DATE, @PaymentMethod NVARCHAR(100), @Details XML, @ActualTotal DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        SET @ActualTotal = 0;
        DECLARE @prefix3 NVARCHAR(20) = N'PT-' + CAST(YEAR(@PaymentDate) AS NVARCHAR) + N'-';
        DECLARE @nextNum3 INT;
        SELECT @nextNum3 = ISNULL(MAX(CAST(SUBSTRING(Ma_Phieu_Thu, LEN(@prefix3)+1, 10) AS INT)), 0)
        FROM PHIEU_THU WHERE Ma_Phieu_Thu LIKE @prefix3 + N'%';
        DECLARE @orderId2 INT, @amountToCollect DECIMAL(18,2), @receiptCode2 NVARCHAR(20);
        DECLARE collect_cursor CURSOR FOR
        SELECT d.value('(OrderId)[1]','INT'), d.value('(AmountToCollect)[1]','DECIMAL(18,2)')
        FROM @Details.nodes('/Details/Item') AS T(d)
        WHERE d.value('(AmountToCollect)[1]','DECIMAL(18,2)') > 0;
        OPEN collect_cursor; FETCH NEXT FROM collect_cursor INTO @orderId2, @amountToCollect;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @nextNum3 = @nextNum3 + 1;
            SET @receiptCode2 = @prefix3 + RIGHT(N'0000' + CAST(@nextNum3 AS NVARCHAR), 4);
            INSERT INTO PHIEU_THU (Ma_Phieu_Thu, id_Khach_Hang, id_Don_Ban_Hang, Ngay_Thu, So_Tien_Thu, Phuong_Thuc_Thu, So_Chung_Tu, Ghi_Chu, Ngay_Tao)
            VALUES (@receiptCode2, @CustomerId, @orderId2, @PaymentDate, @amountToCollect, @PaymentMethod, NULL, NULL, GETDATE());
            SET @ActualTotal = @ActualTotal + @amountToCollect;
            DECLARE @remainingAfter2 DECIMAL(18,2);
            SELECT @remainingAfter2 = dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0)
            FROM DON_BAN_HANG dbh LEFT JOIN PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
            WHERE dbh.id = @orderId2 GROUP BY dbh.Tong_Thanh_Toan;
            IF @remainingAfter2 IS NOT NULL AND @remainingAfter2 <= 0
                UPDATE DON_BAN_HANG SET Trang_Thai = N'Đã thanh toán' WHERE id = @orderId2;
            FETCH NEXT FROM collect_cursor INTO @orderId2, @amountToCollect;
        END
        CLOSE collect_cursor; DEALLOCATE collect_cursor;
        DECLARE @totalDebt2 DECIMAL(18,2), @totalCollected2 DECIMAL(18,2);
        SELECT @totalDebt2 = ISNULL(SUM(Tong_Thanh_Toan), 0) FROM DON_BAN_HANG WHERE id_Khach_Hang = @CustomerId AND ISNULL(Trang_Thai, N'') <> N'Đã hủy';
        SELECT @totalCollected2 = ISNULL(SUM(So_Tien_Thu), 0) FROM PHIEU_THU WHERE id_Khach_Hang = @CustomerId;
        IF EXISTS (SELECT 1 FROM CONG_NO_KHACH_HANG WHERE id_Khach_Hang = @CustomerId)
            UPDATE CONG_NO_KHACH_HANG SET Tong_No = @totalDebt2, Da_Thu = @totalCollected2, Ngay_Cap_Nhat = GETDATE() WHERE id_Khach_Hang = @CustomerId;
        ELSE
            INSERT INTO CONG_NO_KHACH_HANG (id_Khach_Hang, Tong_No, Da_Thu, Ngay_Cap_Nhat) VALUES (@CustomerId, @totalDebt2, @totalCollected2, GETDATE());
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg8 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg8, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- SP: LỆNH SẢN XUẤT
-- =====================================================
CREATE PROCEDURE sp_GetApprovedQuotes AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, kh.Ten_Khach_Hang, ISNULL(ct.So_Luong, 0) AS So_Luong
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.Trang_Thai = N'Đã duyệt'
    ORDER BY bg.id DESC;
END
GO

CREATE PROCEDURE sp_GetQuoteForProduction @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ten_San_Pham, bg.id_Khach_Hang, kh.Ten_Khach_Hang, ISNULL(ct.So_Luong, 0) AS So_Luong
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.id = @QuoteId;
END
GO

CREATE PROCEDURE sp_GetMaterialsForCheck AS
BEGIN
    SET NOCOUNT ON;
    SELECT nl.id, nl.Ten_Nguyen_Lieu, nl.Don_Vi_Tinh, nl.Ton_Kho
    FROM NGUYEN_LIEU nl WHERE nl.Ton_Kho >= 0 ORDER BY nl.Ma_Nguyen_Lieu;
END
GO

CREATE PROCEDURE sp_SaveProductionOrder
    @QuoteId INT, @CustomerId INT, @ProductName NVARCHAR(500), @Quantity INT,
    @StartDate DATE, @ProductionCode NVARCHAR(20) OUTPUT, @ProductionOrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @prefix4 NVARCHAR(20) = N'LSX-' + CAST(YEAR(@StartDate) AS NVARCHAR) + N'-';
        DECLARE @nextNum4 INT;
        SELECT @nextNum4 = ISNULL(MAX(CAST(SUBSTRING(Ma_Lenh_SX, LEN(@prefix4)+1, 10) AS INT)), 0) + 1
        FROM LENH_SAN_XUAT WHERE Ma_Lenh_SX LIKE @prefix4 + N'%';
        SET @ProductionCode = @prefix4 + RIGHT(N'000' + CAST(@nextNum4 AS NVARCHAR), 3);
        INSERT INTO LENH_SAN_XUAT (Ma_Lenh_SX, id_Bao_Gia, id_Khach_Hang, Ten_San_Pham, So_Luong, Ngay_Bat_Dau, Trang_Thai)
        VALUES (@ProductionCode, @QuoteId, CASE WHEN @CustomerId > 0 THEN @CustomerId ELSE NULL END, @ProductName, @Quantity, @StartDate, N'Đang sản xuất');
        SET @ProductionOrderId = SCOPE_IDENTITY();
        UPDATE BAO_GIA SET Trang_Thai = N'Đang sản xuất' WHERE id = @QuoteId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @errMsg9 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errMsg9, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE sp_GetQuoteForExcelExport @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ma_Bao_Gia, bg.Ten_San_Pham, bg.Ngay_Bao_Gia, bg.Kich_Thuoc_Thanh_Pham, bg.Khoi_Luong_Giay,
        bg.Kho_In, bg.So_Mau_In, bg.So_Con, bg.Ghi_Chu,
        bg.Gia_Giay_Tan,
        ISNULL(kh.Ten_Khach_Hang, N'') AS Ten_Khach_Hang,
        ISNULL(ct.So_Luong, 0) AS So_Luong
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.id = @QuoteId;
END
GO

-- =====================================================
-- SP: BÁN HÀNG
-- =====================================================
CREATE PROCEDURE sp_GenerateSalesCode @Year INT, @NextCode NVARCHAR(20) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix5 NVARCHAR(20) = N'BH-' + CAST(@Year AS NVARCHAR) + N'-';
    DECLARE @nextNum5 INT;
    SELECT @nextNum5 = ISNULL(MAX(CAST(SUBSTRING(Ma_Don_Ban, LEN(@prefix5)+1, 10) AS INT)), 0) + 1
    FROM DON_BAN_HANG WHERE Ma_Don_Ban LIKE @prefix5 + N'%';
    SET @NextCode = @prefix5 + RIGHT(N'000' + CAST(@nextNum5 AS NVARCHAR), 3);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetCompletedQuotes AS
BEGIN
    SET NOCOUNT ON;
    -- Chỉ báo giá SX xong và chưa có đơn bán (tránh lập nhiều HĐ cho cùng một báo giá)
    SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, bg.id_Khach_Hang, ISNULL(kh.Ten_Khach_Hang, N'') AS Ten_Khach_Hang
    FROM BAO_GIA bg
    LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    WHERE bg.Trang_Thai = N'Hoàn thành'
      AND NOT EXISTS (
          SELECT 1 FROM dbo.DON_BAN_HANG d
          WHERE d.id_Bao_Gia = bg.id AND ISNULL(d.Trang_Thai, N'') <> N'Đã hủy')
    ORDER BY bg.Ngay_Bao_Gia DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetQuoteDetails @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    -- Chỉ 1 mức: mức đã chọn (id_Muc_Chinh) hoặc fallback mức SL lớn nhất — giống sp_GetQuoteList / màn Quản lý báo giá
    SELECT ct.id, bg.Ten_San_Pham, ct.So_Luong, ct.Gia_Bao_Khach AS UnitPrice, ct.So_Luong * ct.Gia_Bao_Khach AS LineTotal
    FROM dbo.CHI_TIET_BAO_GIA ct
    INNER JOIN dbo.BAO_GIA bg ON bg.id = ct.id_Bao_Gia
    WHERE ct.id_Bao_Gia = @QuoteId
      AND ct.id = COALESCE(
          bg.id_Muc_Chinh,
          (SELECT TOP 1 z.id FROM dbo.CHI_TIET_BAO_GIA z
           WHERE z.id_Bao_Gia = @QuoteId ORDER BY z.So_Luong DESC, z.id DESC));
END
GO

CREATE PROCEDURE sp_GetSalesInvoiceList AS
BEGIN
    SET NOCOUNT ON;
    SELECT dbh.id, dbh.Ma_Don_Ban, dbh.So_Hoa_Don,
        COALESCE(NULLIF(LTRIM(RTRIM(dbh.So_Hoa_Don)), N''), dbh.Ma_Don_Ban) AS So_Hien_Thi_Hoa_Don,
        dbh.Ngay_Ban_Hang, ISNULL(kh.Ten_Khach_Hang, N'(Khách lẻ)') AS Ten_Khach_Hang,
        ISNULL(ct.Ten_San_Pham, N'') AS Ten_San_Pham, ISNULL(ct.Tong_SL, 0) AS So_Luong, dbh.Tong_Thanh_Toan, dbh.Trang_Thai
    FROM DON_BAN_HANG dbh LEFT JOIN KHACH_HANG kh ON kh.id = dbh.id_Khach_Hang
    LEFT JOIN (SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham, SUM(So_Luong) AS Tong_SL FROM CHI_TIET_DON_BAN_HANG GROUP BY id_Don_Ban_Hang) ct ON ct.id_Don_Ban_Hang = dbh.id
    ORDER BY dbh.Ngay_Ban_Hang DESC, dbh.id DESC;
END
GO

-- sp_SaveSalesOrder + sp_GenerateSalesInvoiceNumber: CREATE OR ALTER ở cuối file.

-- =====================================================
-- SP: BÁO CÁO BÁN HÀNG
-- =====================================================
CREATE PROCEDURE sp_GetSalesReportSummary AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ISNULL((SELECT SUM(Tong_Thanh_Toan) FROM DON_BAN_HANG WHERE ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS TotalRevenue,
        ISNULL((SELECT SUM(Tong_Thanh_Toan) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=MONTH(GETDATE()) AND YEAR(Ngay_Ban_Hang)=YEAR(GETDATE()) AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS MonthlyRevenue,
        ISNULL((SELECT SUM(Tong_Thanh_Toan) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=MONTH(DATEADD(MONTH,-1,GETDATE())) AND YEAR(Ngay_Ban_Hang)=YEAR(DATEADD(MONTH,-1,GETDATE())) AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS PrevMonthRevenue,
        ISNULL((SELECT SUM(Tien_Thue_VAT) FROM DON_BAN_HANG WHERE ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS Profit,
        ISNULL((SELECT COUNT(DISTINCT id_Khach_Hang) FROM DON_BAN_HANG WHERE ISNULL(Trang_Thai,N'')<>N'Đã hủy' AND id_Khach_Hang IS NOT NULL),0) AS CustomerCount;
END
GO

CREATE PROCEDURE sp_GetRevenueByProduct AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.Ten_San_Pham AS ProductName, SUM(ct.So_Luong) AS TotalQty, SUM(ct.Thanh_Tien) AS TotalRevenue
    FROM CHI_TIET_DON_BAN_HANG ct JOIN DON_BAN_HANG dbh ON dbh.id = ct.id_Don_Ban_Hang
    WHERE ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy' GROUP BY ct.Ten_San_Pham ORDER BY TotalRevenue DESC;
END
GO

CREATE PROCEDURE sp_GetSummaryReportData AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @thisMonth INT=MONTH(GETDATE()), @thisYear INT=YEAR(GETDATE()),
            @prevMonth INT=MONTH(DATEADD(MONTH,-1,GETDATE())), @prevYear INT=YEAR(DATEADD(MONTH,-1,GETDATE()));
    SELECT
        ISNULL((SELECT SUM(Tong_Thanh_Toan) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@thisMonth AND YEAR(Ngay_Ban_Hang)=@thisYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS RevenueThisMonth,
        ISNULL((SELECT SUM(Tong_Thanh_Toan) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@prevMonth AND YEAR(Ngay_Ban_Hang)=@prevYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS RevenuePrevMonth,
        ISNULL((SELECT SUM(Tong_Tien_Truoc_Thue) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@thisMonth AND YEAR(Ngay_Ban_Hang)=@thisYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS CostThisMonth,
        ISNULL((SELECT SUM(Tong_Tien_Truoc_Thue) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@prevMonth AND YEAR(Ngay_Ban_Hang)=@prevYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS CostPrevMonth,
        ISNULL((SELECT COUNT(*) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@thisMonth AND YEAR(Ngay_Ban_Hang)=@thisYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS OrdersThisMonth,
        ISNULL((SELECT COUNT(*) FROM DON_BAN_HANG WHERE MONTH(Ngay_Ban_Hang)=@prevMonth AND YEAR(Ngay_Ban_Hang)=@prevYear AND ISNULL(Trang_Thai,N'')<>N'Đã hủy'),0) AS OrdersPrevMonth,
        ISNULL((SELECT SUM(Tong_No-Da_Thu) FROM CONG_NO_KHACH_HANG WHERE Tong_No>Da_Thu),0) AS ReceivableDebt,
        ISNULL((SELECT SUM(Ton_Kho*ISNULL(Gia_Nhap,0)) FROM NGUYEN_LIEU),0) AS StockValue;
END
GO

IF OBJECT_ID('sp_InsertQuoteDetail','P') IS NOT NULL DROP PROCEDURE sp_InsertQuoteDetail;
GO
CREATE PROCEDURE sp_InsertQuoteDetail
    @QuoteId INT, @ValidityDays INT, @PaperType NVARCHAR(100), @Quantity INT,
    @CostPaper DECIMAL(18,2), @CostPlate DECIMAL(18,2), @CostPrint DECIMAL(18,2),
    @CostLaminate DECIMAL(18,2), @CostMetalize DECIMAL(18,2), @CostUV DECIMAL(18,2),
    @CostDie DECIMAL(18,2), @CostDieMold DECIMAL(18,2), @CostGlue DECIMAL(18,2),
    @CostRibbon DECIMAL(18,2), @CostButton DECIMAL(18,2), @CostBox DECIMAL(18,2),
    @CostDelivery DECIMAL(18,2), @CostProof DECIMAL(18,2), @TotalCost DECIMAL(18,2),
    @CostPerUnit DECIMAL(18,2), @PricePerUnit DECIMAL(18,2), @TotalQuotePrice DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE BAO_GIA
        SET Hieu_Luc_Bao_Gia_Ngay = @ValidityDays, Ten_Loai_Giay = @PaperType
        WHERE id = @QuoteId;

        INSERT INTO CHI_TIET_BAO_GIA (
            id_Bao_Gia, So_Luong, Tien_Giay, Tien_Kem, Tien_In,
            Tien_Can_Mang, Tien_Metalize, Tien_UV, Tien_Be, Tien_Khuon_Be,
            Tien_Dan, Tien_Day, Tien_Nut, Tien_Thung, Tien_Xe_Giao, Tien_Proof,
            Tong_Gia_Thanh, Gia_Moi_Cai, Gia_Bao_Khach, Tong_Gia_Bao_Khach)
        VALUES (
            @QuoteId, @Quantity, @CostPaper, @CostPlate, @CostPrint,
            @CostLaminate, @CostMetalize, @CostUV, @CostDie, @CostDieMold,
            @CostGlue, @CostRibbon, @CostButton, @CostBox, @CostDelivery, @CostProof,
            @TotalCost, @CostPerUnit, @PricePerUnit, @TotalQuotePrice);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @err NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@err, 16, 1);
    END CATCH
END
GO

-- Sửa sp_GetQuoteForExcel trả về TẤT CẢ CHI_TIET (nhiều mức SL)
ALTER PROCEDURE sp_GetQuoteForExcel @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ma_Bao_Gia, CONVERT(NVARCHAR, bg.Ngay_Bao_Gia, 103) AS Ngay_Bao_Gia,
        bg.Hieu_Luc_Bao_Gia_Ngay, bg.Thoi_Gian_Giao_Hang_Du_Kien, bg.Ten_San_Pham,
        bg.Kich_Thuoc_Thanh_Pham, ISNULL(bg.Ten_Loai_Giay, bg.Khoi_Luong_Giay) AS Ten_Loai_Giay,
        ISNULL(kh.Ten_Khach_Hang, N'') AS Ten_Khach_Hang, ISNULL(kh.Dia_Chi, N'') AS Dia_Chi,
        ISNULL(kh.MST, N'') AS MST, ISNULL(kh.Nguoi_Lien_He, N'') AS Nguoi_Lien_He
    FROM BAO_GIA bg LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    WHERE bg.id = @QuoteId;

    SELECT id, So_Luong, Gia_Bao_Khach, Tong_Gia_Bao_Khach, Gia_Moi_Cai
    FROM CHI_TIET_BAO_GIA
    WHERE id_Bao_Gia = @QuoteId
    ORDER BY So_Luong;
END
GO

-- SP lấy chi tiết mức SL (dùng cho panel bên dưới grid)
IF OBJECT_ID('sp_GetQuoteDetailFull','P') IS NOT NULL DROP PROCEDURE sp_GetQuoteDetailFull;
GO
CREATE PROCEDURE sp_GetQuoteDetailFull @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, So_Luong, Gia_Bao_Khach, Tong_Gia_Bao_Khach, Gia_Moi_Cai, Tong_Gia_Thanh
    FROM CHI_TIET_BAO_GIA
    WHERE id_Bao_Gia = @QuoteId
    ORDER BY So_Luong;
END
GO
USE PrintingManagement;
GO
 
-- ─────────────────────────────────────────────────────
-- SP mới: sp_GenerateCustomerCode
-- Trả về mã KH tiếp theo dạng KH_01, KH_02, ...
-- Form frmCustomerDetail gọi SP này khi mở để set default
-- ─────────────────────────────────────────────────────
IF OBJECT_ID('sp_GenerateCustomerCode', 'P') IS NOT NULL
    DROP PROCEDURE sp_GenerateCustomerCode;
GO
 
CREATE PROCEDURE sp_GenerateCustomerCode
    @NextCode NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
 
    DECLARE @nextNum INT;
 
    -- Lấy số lớn nhất từ các mã dạng KH_01, KH_02, KH_1, KH_2...
    SELECT @nextNum = ISNULL(MAX(
        CASE
            WHEN Ma_KH LIKE N'KH[_][0-9]%'          -- KH_01, KH_02
                THEN TRY_CAST(SUBSTRING(Ma_KH, 4, 10) AS INT)
            WHEN Ma_KH LIKE N'KH[0-9]%'             -- KH01, KH02 (format cũ)
                THEN TRY_CAST(SUBSTRING(Ma_KH, 3, 10) AS INT)
            ELSE 0
        END
    ), 0) + 1
    FROM KHACH_HANG
    WHERE Ma_KH LIKE N'KH%';
 
    -- Format: KH_01 đến KH_99, sau đó KH_100, KH_101...
    SET @NextCode = CASE
        WHEN @nextNum > 99
            THEN N'KH_' + CAST(@nextNum AS NVARCHAR)
        ELSE
            N'KH_' + RIGHT(N'00' + CAST(@nextNum AS NVARCHAR), 2)
    END;
END
GO
 
ALTER PROCEDURE sp_GetQuoteById @QuoteId INT AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bg.id,
        bg.Ma_Bao_Gia,
        bg.id_Khach_Hang,
        bg.Ten_San_Pham,
        bg.Ngay_Bao_Gia,
        bg.So_Con,
        bg.Gia_Giay_Tan,
        bg.Loi_Nhuan_Phan_Tram,
        bg.Kich_Thuoc_Thanh_Pham,
        bg.Khoi_Luong_Giay,
        bg.Kho_In,
        bg.So_Mau_In,
        bg.Thoi_Gian_Giao_Hang_Du_Kien,
        bg.Ten_Loai_Giay,
        bg.Hieu_Luc_Bao_Gia_Ngay,
        bg.Nguoi_Tao,
        bg.Trang_Thai,
        kh.Ten_Khach_Hang,
        kh.Dia_Chi,
        ct.So_Luong,
        ct.Tien_Khuon_Be,
        ct.Tien_Can_Mang
    FROM BAO_GIA bg
    LEFT JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    OUTER APPLY (
        SELECT TOP 1
            So_Luong,
            Tien_Khuon_Be,
            Tien_Can_Mang
        FROM CHI_TIET_BAO_GIA
        WHERE id_Bao_Gia = bg.id
        ORDER BY id DESC
    ) ct
    WHERE bg.id = @QuoteId;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QUOTE_DRAFT')
BEGIN
    CREATE TABLE QUOTE_DRAFT (
        id INT IDENTITY(1,1) PRIMARY KEY,
        id_Khach_Hang INT NOT NULL,
        Ten_User NVARCHAR(100) NOT NULL,
        DraftJson NVARCHAR(MAX) NOT NULL,
        Ngay_Cap_Nhat DATETIME DEFAULT GETDATE(),
        CONSTRAINT UQ_QUOTE_DRAFT UNIQUE (id_Khach_Hang, Ten_User)
    );
END
GO

IF OBJECT_ID('sp_SaveQuoteDraft', 'P') IS NOT NULL
    DROP PROCEDURE sp_SaveQuoteDraft;
GO

CREATE PROCEDURE sp_SaveQuoteDraft
    @CustomerId INT,
    @TenUser NVARCHAR(100),
    @DraftJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @CustomerId IS NULL OR @CustomerId <= 0 RETURN;
    IF @TenUser IS NULL OR LTRIM(RTRIM(@TenUser)) = N'' RETURN;

    IF EXISTS (SELECT 1 FROM QUOTE_DRAFT WHERE id_Khach_Hang = @CustomerId AND Ten_User = @TenUser)
    BEGIN
        UPDATE QUOTE_DRAFT
        SET DraftJson = @DraftJson, Ngay_Cap_Nhat = GETDATE()
        WHERE id_Khach_Hang = @CustomerId AND Ten_User = @TenUser;
    END
    ELSE
    BEGIN
        INSERT INTO QUOTE_DRAFT (id_Khach_Hang, Ten_User, DraftJson)
        VALUES (@CustomerId, @TenUser, @DraftJson);
    END
END
GO

IF OBJECT_ID('sp_GetQuoteDraft', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetQuoteDraft;
GO

CREATE PROCEDURE sp_GetQuoteDraft
    @CustomerId INT,
    @TenUser NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 DraftJson
    FROM QUOTE_DRAFT
    WHERE id_Khach_Hang = @CustomerId AND Ten_User = @TenUser;
END
GO
 
-- ─────────────────────────────────────────────────────────────────
-- BƯỚC 1: Thêm 2 cột mới vào PHIEU_NHAP_KHO (nếu chưa có)
-- ─────────────────────────────────────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('PHIEU_NHAP_KHO') AND name = 'So_Ngay_No'
)
    ALTER TABLE PHIEU_NHAP_KHO ADD So_Ngay_No INT DEFAULT 0;
GO
 
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('PHIEU_NHAP_KHO') AND name = 'Ngay_Den_Han_TT'
)
    ALTER TABLE PHIEU_NHAP_KHO ADD Ngay_Den_Han_TT DATE NULL;
GO
 
PRINT N'✅ Đã thêm cột So_Ngay_No và Ngay_Den_Han_TT vào PHIEU_NHAP_KHO';
GO
 
-- ─────────────────────────────────────────────────────────────────
-- BƯỚC 2: Sửa sp_SaveInventoryReceipt để nhận thêm 2 tham số mới
-- ─────────────────────────────────────────────────────────────────
ALTER PROCEDURE sp_SaveInventoryReceipt
    @ReceiptId        INT,
    @ReceiptCode      NVARCHAR(50),
    @OrderId          INT,
    @SupplierId       INT,
    @DocumentDate     DATE,
    @AccountingDate   DATE,
    @CreatedBy        NVARCHAR(100),
    @ReceiveType      NVARCHAR(100),
    @PaymentMethod    NVARCHAR(100),
    @PaymentStatus    NVARCHAR(100),
    @HasInvoice       BIT,
    @InvoiceForm      NVARCHAR(50),
    @InvoiceSymbol    NVARCHAR(50),
    @InvoiceNumber    NVARCHAR(50),
    @InvoiceDate      DATE,
    @InvoiceFile      NVARCHAR(500),
    @SoNgayNo         INT           = 0,      -- [NEW] số ngày được nợ
    @NgayDenHanTT     DATE          = NULL,   -- [NEW] ngày đến hạn thanh toán
    @NewReceiptId     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
 
    IF @ReceiptId = 0
    BEGIN
        INSERT INTO PHIEU_NHAP_KHO (
            Ma_Phieu_Nhap, id_Don_Dat_Hang, id_Nha_Cung_Cap, Ngay_Nhap,
            Ngay_Chung_Tu, Ngay_Hach_Toan, Nguoi_Nhap, Loai_Nhap,
            Hinh_Thuc_Thanh_Toan, Trang_Thai_TT, Co_Hoa_Don,
            Mau_So_Hoa_Don, Ky_Hieu_Hoa_Don, So_Hoa_Don, Ngay_Hoa_Don,
            File_Hoa_Don, Trang_Thai,
            So_Ngay_No, Ngay_Den_Han_TT)          -- [NEW]
        VALUES (
            @ReceiptCode, @OrderId, @SupplierId, @DocumentDate,
            @DocumentDate, @AccountingDate, @CreatedBy, @ReceiveType,
            @PaymentMethod, @PaymentStatus, @HasInvoice,
            @InvoiceForm, @InvoiceSymbol, @InvoiceNumber, @InvoiceDate,
            @InvoiceFile, N'Chưa ghi sổ',
            @SoNgayNo, @NgayDenHanTT);            -- [NEW]
 
        SET @NewReceiptId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE PHIEU_NHAP_KHO
        SET Ngay_Nhap              = @DocumentDate,
            Ngay_Chung_Tu          = @DocumentDate,
            Ngay_Hach_Toan         = @AccountingDate,
            Loai_Nhap              = @ReceiveType,
            Hinh_Thuc_Thanh_Toan   = @PaymentMethod,
            Trang_Thai_TT          = @PaymentStatus,
            Co_Hoa_Don             = @HasInvoice,
            Mau_So_Hoa_Don         = @InvoiceForm,
            Ky_Hieu_Hoa_Don        = @InvoiceSymbol,
            So_Hoa_Don             = @InvoiceNumber,
            Ngay_Hoa_Don           = @InvoiceDate,
            File_Hoa_Don           = @InvoiceFile,
            So_Ngay_No             = @SoNgayNo,    -- [NEW]
            Ngay_Den_Han_TT        = @NgayDenHanTT -- [NEW]
        WHERE id = @ReceiptId;
 
        SET @NewReceiptId = @ReceiptId;
    END
END
GO
 
-- ─────────────────────────────────────────────────────────────────
-- BƯỚC 3: Sửa sp_GetOrderReceiveData để trả thêm So_Ngay_No
--         (dùng để auto-fill số ngày nợ khi chọn đơn hàng)
-- ─────────────────────────────────────────────────────────────────
ALTER PROCEDURE sp_GetOrderReceiveData @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Bảng 0: Thông tin NCC + số ngày nợ từ đơn hàng
    SELECT
        d.Dieu_Khoan_Thanh_Toan,
        d.So_Ngay_No,                    -- [NEW] lấy từ đơn đặt hàng
        n.Ten_NCC,
        n.Dia_Chi,
        n.id    AS SupplierId,
        n.Ma_NCC
    FROM DON_DAT_HANG_NCC d
    JOIN NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
    WHERE d.id = @OrderId;
 
    -- Bảng 1: Chi tiết hàng hóa (giữ nguyên)
    SELECT
        ct.id,
        nl.Ma_Nguyen_Lieu,
        nl.Ten_Nguyen_Lieu,
        nl.Don_Vi_Tinh,
        ct.So_Luong   AS OrderQty,
        ct.Don_Gia,
        ct.Thanh_Tien,
        ct.Phan_Tram_Thue_GTGT,
        ct.Tien_Thue_GTGT,
        nl.id AS MaterialId
    FROM CHI_TIET_DON_HANG_NCC ct
    JOIN NGUYEN_LIEU nl ON ct.id_Nguyen_Lieu = nl.id
    WHERE ct.id_Don_Dat_Hang = @OrderId;
END
GO
 ALTER PROCEDURE sp_GetSupplierDebtList @SupplierId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        dh.id AS OrderId,
        -- [FIX] Lấy So_Hoa_Don từ phiếu nhập, fallback về Ma_Don_Hang
        ISNULL(
            NULLIF(pn.So_Hoa_Don, N''), 
            dh.Ma_Don_Hang
        ) AS OrderCode,
        -- [FIX] Lấy ngày hóa đơn thay vì ngày đặt hàng
        ISNULL(pn.Ngay_Hoa_Don, dh.Ngay_Dat_Hang) AS OrderDate,
        N'Đơn hàng ' + dh.Ma_Don_Hang AS Description,
        dh.Tong_Tien AS TotalAmount,
        ISNULL((
            SELECT SUM(So_Tien) FROM THANH_TOAN_NCC 
            WHERE id_Don_Dat_Hang = dh.id
        ), 0) AS PaidAmount
    FROM DON_DAT_HANG_NCC dh
    -- [FIX] JOIN lấy số hóa đơn từ phiếu nhập kho gần nhất đã ghi sổ
    LEFT JOIN (
        SELECT id_Don_Dat_Hang,
               MAX(So_Hoa_Don)   AS So_Hoa_Don,
               MAX(Ngay_Hoa_Don) AS Ngay_Hoa_Don
        FROM PHIEU_NHAP_KHO
        WHERE Trang_Thai = N'Đã ghi sổ'
        GROUP BY id_Don_Dat_Hang
    ) pn ON pn.id_Don_Dat_Hang = dh.id
    WHERE dh.id_Nha_Cung_Cap = @SupplierId
      AND dh.Tong_Tien > ISNULL((
          SELECT SUM(So_Tien) FROM THANH_TOAN_NCC 
          WHERE id_Don_Dat_Hang = dh.id
      ), 0)
    ORDER BY dh.Ngay_Dat_Hang DESC;
END
GO
-- =====================================================================
-- [PATCH] Thanh toán NCC theo số HĐ + Báo cáo mua hàng chi tiết
-- Chạy trên database PrintingManagement
-- =====================================================================
USE PrintingManagement;
GO

-- ─────────────────────────────────────────────────────────────────
-- 1. Sửa sp_GetSupplierDebtList:
--    Thay vì lấy từ DON_DAT_HANG_NCC (mã đơn hàng),
--    lấy từ PHIEU_NHAP_KHO (số HĐ thực tế người dùng nhập tay)
-- ─────────────────────────────────────────────────────────────────
ALTER PROCEDURE sp_GetSupplierDebtList @SupplierId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy các phiếu nhập kho đã ghi sổ của NCC, nhóm theo số HĐ
    -- Mỗi dòng = 1 phiếu nhập (1 số HĐ), tính tổng tiền từ CHI_TIET_PHIEU_NHAP
    SELECT
        pn.id                                           AS OrderId,       -- dùng id phiếu nhập
        ISNULL(pn.So_Hoa_Don, pn.Ma_Phieu_Nhap)        AS OrderCode,     -- ƯU TIÊN số HĐ, fallback mã phiếu
        pn.Ngay_Hoa_Don                                 AS OrderDate,
        N'HĐ: ' + ISNULL(pn.So_Hoa_Don, pn.Ma_Phieu_Nhap)
            + N' - Ngày nhập: ' + CONVERT(NVARCHAR, pn.Ngay_Nhap, 103)  AS Description,
        -- Tổng tiền = tổng thành tiền + VAT của phiếu nhập
        ISNULL(SUM(ct.Thanh_Tien), 0)
            + ISNULL(pn_vat.TongVAT, 0)                AS TotalAmount,
        -- Đã trả: lấy từ THANH_TOAN_NCC tham chiếu theo id_Don_Dat_Hang = id đơn hàng gốc
        ISNULL((
            SELECT SUM(tt.So_Tien)
            FROM THANH_TOAN_NCC tt
            WHERE tt.id_Don_Dat_Hang = pn.id_Don_Dat_Hang
        ), 0)                                           AS PaidAmount,
        pn.id_Don_Dat_Hang                              AS OriginalOrderId  -- để thanh toán link đúng
    FROM PHIEU_NHAP_KHO pn
    JOIN CHI_TIET_PHIEU_NHAP ct ON ct.id_Phieu_Nhap = pn.id
    -- Tính VAT từ CHI_TIET_DON_HANG_NCC theo đơn hàng gốc
    LEFT JOIN (
        SELECT id_Don_Dat_Hang, SUM(Tien_Thue_GTGT) AS TongVAT
        FROM CHI_TIET_DON_HANG_NCC
        GROUP BY id_Don_Dat_Hang
    ) pn_vat ON pn_vat.id_Don_Dat_Hang = pn.id_Don_Dat_Hang
    WHERE pn.id_Nha_Cung_Cap = @SupplierId
      AND pn.Trang_Thai = N'Đã ghi sổ'    -- chỉ lấy phiếu đã ghi sổ
    GROUP BY
        pn.id, pn.So_Hoa_Don, pn.Ma_Phieu_Nhap,
        pn.Ngay_Hoa_Don, pn.Ngay_Nhap,
        pn.id_Don_Dat_Hang, pn_vat.TongVAT
    HAVING
        -- Chỉ hiện những HĐ còn nợ
        ISNULL(SUM(ct.Thanh_Tien), 0) + ISNULL(pn_vat.TongVAT, 0)
        > ISNULL((
            SELECT SUM(tt.So_Tien)
            FROM THANH_TOAN_NCC tt
            WHERE tt.id_Don_Dat_Hang = pn.id_Don_Dat_Hang
        ), 0)
    ORDER BY pn.Ngay_Nhap DESC;
END
GO

PRINT N'✅ sp_GetSupplierDebtList đã sửa — lấy theo số HĐ phiếu nhập';
GO

-- ─────────────────────────────────────────────────────────────────
-- 2. SP MỚI: sp_GetPurchaseDetail
--    Báo cáo mua hàng chi tiết: lọc theo NCC, hiển thị từng lần nhập
--    Tên NCC / Ngày nhập / Số HĐ / Mã phiếu / Tên hàng / SL / Đơn giá / Thành tiền
-- ─────────────────────────────────────────────────────────────────
IF OBJECT_ID('sp_GetPurchaseDetail', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetPurchaseDetail;
GO

CREATE PROCEDURE sp_GetPurchaseDetail
    @SupplierKeyword NVARCHAR(200) = N'',   -- tìm theo tên NCC (để trống = tất cả)
    @FromDate        DATE          = NULL,
    @ToDate          DATE          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Mặc định: 3 tháng gần nhất nếu không truyền
    IF @FromDate IS NULL SET @FromDate = DATEADD(MONTH, -3, CAST(GETDATE() AS DATE));
    IF @ToDate   IS NULL SET @ToDate   = CAST(GETDATE() AS DATE);

    SELECT
        ncc.Ten_NCC                                             AS TenNCC,
        ncc.Ma_NCC                                              AS MaNCC,
        pn.Ngay_Nhap                                            AS NgayNhap,
        ISNULL(pn.So_Hoa_Don, N'(chưa có HĐ)')                AS SoHoaDon,
        pn.Ma_Phieu_Nhap                                        AS MaPhieuNhap,
        dh.Ma_Don_Hang                                          AS MaDonHang,
        nl.Ma_Nguyen_Lieu                                       AS MaHang,
        nl.Ten_Nguyen_Lieu                                      AS TenHang,
        nl.Don_Vi_Tinh                                          AS DVT,
        ct.So_Luong_Nhap                                        AS SoLuong,
        ct.Don_Gia_Nhap                                         AS DonGia,
        ct.Thanh_Tien                                           AS ThanhTien,
        ISNULL(ctdh.Phan_Tram_Thue_GTGT, 0)                   AS PhanTramVAT,
        ISNULL(ctdh.Tien_Thue_GTGT, 0)                        AS TienVAT,
        ct.Thanh_Tien + ISNULL(ctdh.Tien_Thue_GTGT, 0)       AS TongCong,
        pn.Trang_Thai                                           AS TrangThai
    FROM CHI_TIET_PHIEU_NHAP ct
    JOIN PHIEU_NHAP_KHO pn       ON pn.id  = ct.id_Phieu_Nhap
    JOIN NHA_CUNG_CAP ncc        ON ncc.id = pn.id_Nha_Cung_Cap
    JOIN NGUYEN_LIEU nl          ON nl.id  = ct.id_Nguyen_Lieu
    LEFT JOIN DON_DAT_HANG_NCC dh ON dh.id = pn.id_Don_Dat_Hang
    LEFT JOIN CHI_TIET_DON_HANG_NCC ctdh
        ON ctdh.id_Don_Dat_Hang = pn.id_Don_Dat_Hang
        AND ctdh.id_Nguyen_Lieu = ct.id_Nguyen_Lieu
    WHERE
        pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate
        AND (
            @SupplierKeyword = N''
            OR ncc.Ten_NCC   LIKE N'%' + @SupplierKeyword + N'%'
            OR ncc.Ma_NCC    LIKE N'%' + @SupplierKeyword + N'%'
        )
    ORDER BY ncc.Ten_NCC, pn.Ngay_Nhap DESC, nl.Ten_Nguyen_Lieu;
END
GO

PRINT N'✅ sp_GetPurchaseDetail đã tạo';
GO

-- ─────────────────────────────────────────────────────────────────
-- 3. SP tổng hợp theo NCC (giữ cho tab tổng hợp, sửa thêm PaidAmount)
--    Sửa PaidAmount lấy từ THANH_TOAN_NCC cho chính xác
-- ─────────────────────────────────────────────────────────────────
ALTER PROCEDURE sp_GetPurchaseSummary
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ncc.id                                          AS SupplierId,
        ncc.Ma_NCC                                      AS MaNCC,
        ncc.Ten_NCC                                     AS SupplierName,
        COUNT(DISTINCT pn.id)                           AS OrderCount,
        ISNULL(SUM(ct.Thanh_Tien), 0)                  AS TotalAmount,
        ISNULL((
            SELECT SUM(tt.So_Tien)
            FROM THANH_TOAN_NCC tt
            WHERE tt.id_Nha_Cung_Cap = ncc.id
        ), 0)                                           AS PaidAmount
    FROM NHA_CUNG_CAP ncc
    LEFT JOIN PHIEU_NHAP_KHO pn  ON pn.id_Nha_Cung_Cap = ncc.id
                                 AND pn.Trang_Thai = N'Đã ghi sổ'
    LEFT JOIN CHI_TIET_PHIEU_NHAP ct ON ct.id_Phieu_Nhap = pn.id
    GROUP BY ncc.id, ncc.Ma_NCC, ncc.Ten_NCC
    HAVING COUNT(DISTINCT pn.id) > 0
    ORDER BY ncc.Ten_NCC;
END
GO

-- =====================================================
-- ĐỒNG BỘ CUỐI FILE (idempotent): cột đơn bán + 4 SP
-- Chạy cả khối này sau cài mới; hoặc chỉ khối này trên DB cũ (đã USE đúng catalog).
-- =====================================================
IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'Ngay_Uoc_Thu') IS NULL
    ALTER TABLE dbo.DON_BAN_HANG ADD Ngay_Uoc_Thu DATE NULL;
IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'TK_No') IS NULL
    ALTER TABLE dbo.DON_BAN_HANG ADD TK_No NVARCHAR(20) NULL;
IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'TK_Co') IS NULL
    ALTER TABLE dbo.DON_BAN_HANG ADD TK_Co NVARCHAR(20) NULL;
IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'id_Bao_Gia') IS NULL
    ALTER TABLE dbo.DON_BAN_HANG ADD id_Bao_Gia INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DON_BAN_HANG_BAO_GIA')
    ALTER TABLE dbo.DON_BAN_HANG ADD CONSTRAINT FK_DON_BAN_HANG_BAO_GIA
        FOREIGN KEY (id_Bao_Gia) REFERENCES dbo.BAO_GIA(id);
GO

IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'id_Phieu_Giao_Hang') IS NULL
    ALTER TABLE dbo.DON_BAN_HANG ADD id_Phieu_Giao_Hang INT NULL;
GO
-- FK tới PHIEU_GIAO_HANG: thêm sau khi bảng phiếu giao đã tồn tại (xem khối sau CREATE TABLE CHI_TIET_PGH)

CREATE OR ALTER PROCEDURE dbo.sp_GenerateSalesInvoiceNumber
    @Year INT,
    @NextNumber NVARCHAR(32) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @prefix NVARCHAR(16) = CONCAT(N'HD-', CONVERT(NVARCHAR(4), @Year), N'-');
    DECLARE @maxSeq INT = 0;
    SELECT @maxSeq = ISNULL(MAX(
        TRY_CONVERT(INT,
            SUBSTRING(LTRIM(RTRIM(So_Hoa_Don)), LEN(@prefix) + 1, 12))), 0)
    FROM dbo.DON_BAN_HANG
    WHERE So_Hoa_Don LIKE @prefix + N'%';
    SET @NextNumber = @prefix + RIGHT(N'000000' + CONVERT(NVARCHAR(12), @maxSeq + 1), 6);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_SaveSalesOrder
    @DocumentCode NVARCHAR(50),
    @CustomerId INT,
    @SaleDate DATE,
    @InvoiceForm NVARCHAR(50),
    @InvoiceSymbol NVARCHAR(50),
    @InvoiceNumber NVARCHAR(50),
    @InvoiceDate DATE,
    @SubTotal DECIMAL(18,2),
    @VatRate DECIMAL(5,2),
    @TotalVat DECIMAL(18,2),
    @GrandTotal DECIMAL(18,2),
    @QuoteId INT,
    @PhieuGiaoId INT = 0,
    @Details XML,
    @NewOrderId INT OUTPUT,
    @ExpectedPaymentDate DATE = NULL,
    @DebitAccount NVARCHAR(20) = NULL,
    @CreditAccount NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ResolvedQuoteId INT = NULL;
    DECLARE @ResolvedPhieuId INT = NULL;

    IF @PhieuGiaoId > 0
    BEGIN
        IF EXISTS (
            SELECT 1 FROM dbo.DON_BAN_HANG
            WHERE id_Phieu_Giao_Hang = @PhieuGiaoId AND ISNULL(Trang_Thai, N'') <> N'Đã hủy')
        BEGIN
            RAISERROR(N'Phiếu giao hàng này đã được lập đơn bán hàng. Mỗi phiếu chỉ lập đơn một lần.', 16, 1);
            RETURN;
        END

        SELECT @ResolvedQuoteId = id_Bao_Gia, @ResolvedPhieuId = id
        FROM dbo.PHIEU_GIAO_HANG WHERE id = @PhieuGiaoId;

        IF @ResolvedPhieuId IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy phiếu giao hàng.', 16, 1);
            RETURN;
        END
    END
    ELSE IF @QuoteId > 0 AND EXISTS (
        SELECT 1 FROM dbo.DON_BAN_HANG
        WHERE id_Bao_Gia = @QuoteId AND ISNULL(Trang_Thai, N'') <> N'Đã hủy')
    BEGIN
        RAISERROR(N'Báo giá này đã được lập đơn bán hàng. Mỗi báo giá chỉ lập đơn một lần.', 16, 1);
        RETURN;
    END

    IF @PhieuGiaoId <= 0 AND @QuoteId > 0
        SET @ResolvedQuoteId = @QuoteId;

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.DON_BAN_HANG (
            Ma_Don_Ban, id_Khach_Hang, Ngay_Ban_Hang, Ngay_Den_Han,
            Mau_So_Hoa_Don, Ky_Hieu_Hoa_Don, So_Hoa_Don, Ngay_Hoa_Don,
            Tong_Tien_Truoc_Thue, Phan_Tram_VAT, Tien_Thue_VAT, Tong_Thanh_Toan, Trang_Thai,
            Ngay_Uoc_Thu, TK_No, TK_Co, id_Bao_Gia, id_Phieu_Giao_Hang)
        VALUES (
            @DocumentCode,
            CASE WHEN @CustomerId > 0 THEN @CustomerId ELSE NULL END,
            @SaleDate,
            @ExpectedPaymentDate,          -- Ngay_Den_Han: hạn thanh toán chính thức
            @InvoiceForm, @InvoiceSymbol, @InvoiceNumber, @InvoiceDate,
            @SubTotal, @VatRate, @TotalVat, @GrandTotal, N'Đã xuất',
            NULL,                           -- Ngay_Uoc_Thu: cập nhật sau khi thu tiền thật
            NULLIF(LTRIM(RTRIM(@DebitAccount)), N''),
            NULLIF(LTRIM(RTRIM(@CreditAccount)), N''),
            @ResolvedQuoteId,
            @ResolvedPhieuId);
        SET @NewOrderId = SCOPE_IDENTITY();
        INSERT INTO dbo.CHI_TIET_DON_BAN_HANG (id_Don_Ban_Hang, Ten_San_Pham, So_Luong, Don_Gia, Thanh_Tien, Phan_Tram_VAT, Tien_VAT)
        SELECT @NewOrderId,
            d.value('(ProductName)[1]', 'NVARCHAR(500)'),
            d.value('(Quantity)[1]', 'FLOAT'),
            d.value('(UnitPrice)[1]', 'FLOAT'),
            d.value('(LineTotal)[1]', 'FLOAT'),
            d.value('(VatRate)[1]', 'FLOAT'),
            d.value('(VatAmount)[1]', 'FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @errUpd NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errUpd, 16, 1);
    END CATCH
END;
GO

-- Phiếu giao trạng thái "Đã giao" chưa lập đơn bán (cho frmSales)
CREATE OR ALTER PROCEDURE dbo.sp_GetDeliveredNotesForSales
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pgh.id,
        pgh.Ma_Phieu_Giao,
        pgh.Ten_KH,
        pgh.id_Khach_Hang,
        pgh.Ngay_Giao,
        bg.Ma_Bao_Gia
    FROM dbo.PHIEU_GIAO_HANG pgh
    LEFT JOIN dbo.BAO_GIA bg ON bg.id = pgh.id_Bao_Gia
    WHERE pgh.Trang_Thai = N'Đã giao'
      AND NOT EXISTS (
          SELECT 1 FROM dbo.DON_BAN_HANG dbh
          WHERE dbh.id_Phieu_Giao_Hang = pgh.id
            AND ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy')
    ORDER BY pgh.Ngay_Tao DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetInventoryData
    @Keyword NVARCHAR(200) = N'',
    @CodePrefix NVARCHAR(10) = N'',
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @FromDate IS NULL SET @FromDate = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    IF @ToDate IS NULL SET @ToDate = EOMONTH(GETDATE());
    SELECT
        nl.id,
        nl.Ma_Nguyen_Lieu,
        nl.Ten_Nguyen_Lieu,
        nl.Don_Vi_Tinh,
        ISNULL((
            SELECT SUM(ct.So_Luong_Nhap)
            FROM dbo.CHI_TIET_PHIEU_NHAP ct
            INNER JOIN dbo.PHIEU_NHAP_KHO pn ON pn.id = ct.id_Phieu_Nhap
            WHERE ct.id_Nguyen_Lieu = nl.id
              AND pn.Ngay_Nhap BETWEEN @FromDate AND @ToDate
        ), 0) AS StockIn,
        ISNULL((
            SELECT SUM(ct.So_Luong_Xuat)
            FROM dbo.CHI_TIET_XUAT_KHO_SX ct
            INNER JOIN dbo.PHIEU_XUAT_KHO_SX px ON px.id = ct.id_Phieu_Xuat
            WHERE ct.id_Nguyen_Lieu = nl.id
              AND px.Ngay_Xuat BETWEEN @FromDate AND @ToDate
        ), 0) AS StockOut,
        ISNULL(nl.Ton_Kho, 0) AS ClosingStock,
        nl.Gia_Nhap,
        nl.Ton_Kho_Toi_Thieu,
        ISNULL(nl.De_Xuat_Nhap, nl.Ton_Kho_Toi_Thieu * 2) AS SuggestQty
    FROM dbo.NGUYEN_LIEU nl
    WHERE (@Keyword = N''
        OR nl.Ma_Nguyen_Lieu LIKE N'%' + @Keyword + N'%'
        OR nl.Ten_Nguyen_Lieu LIKE N'%' + @Keyword + N'%')
      AND (@CodePrefix = N'' OR nl.Ma_Nguyen_Lieu LIKE @CodePrefix + N'%')
    ORDER BY nl.Ma_Nguyen_Lieu;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetReceivables
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        kh.id,
        kh.Ten_Khach_Hang,
        cn.Tong_No,
        cn.Da_Thu,
        cn.Con_Lai,
        ISNULL((
            SELECT TOP 1 COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han)
            FROM dbo.DON_BAN_HANG dbh
            WHERE dbh.id_Khach_Hang = kh.id
              AND ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy'
              AND COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han) IS NOT NULL
            ORDER BY COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han) ASC
        ),
        (
            SELECT DATEADD(DAY, 7, MAX(db2.Ngay_Ban_Hang))
            FROM dbo.DON_BAN_HANG db2
            WHERE db2.id_Khach_Hang = kh.id
              AND ISNULL(db2.Trang_Thai, N'') <> N'Đã hủy'
        )) AS DueDate
    FROM dbo.CONG_NO_KHACH_HANG cn
    INNER JOIN dbo.KHACH_HANG kh ON kh.id = cn.id_Khach_Hang
    WHERE cn.Con_Lai > 0
    ORDER BY cn.Con_Lai DESC;
END;
GO

PRINT N'✅ Đồng bộ: DON_BAN_HANG + sp_GenerateSalesInvoiceNumber + sp_SaveSalesOrder + sp_GetInventoryData + sp_GetReceivables';
GO

IF COL_LENGTH(N'dbo.BAO_GIA', N'id_Muc_Chinh') IS NULL
    ALTER TABLE dbo.BAO_GIA ADD id_Muc_Chinh INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BAO_GIA_ID_MUC_CHINH')
BEGIN
    ALTER TABLE dbo.BAO_GIA ADD CONSTRAINT FK_BAO_GIA_ID_MUC_CHINH
        FOREIGN KEY (id_Muc_Chinh) REFERENCES dbo.CHI_TIET_BAO_GIA(id);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetQuoteList
    @Search   NVARCHAR(200) = N'',
    @FromDate DATE,
    @ToDate   DATE,
    @Offset   INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ToDateNext DATE = DATEADD(DAY, 1, @ToDate);
    SELECT COUNT(DISTINCT bg.id) AS TotalRecord
    FROM dbo.BAO_GIA bg LEFT JOIN dbo.KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
    WHERE bg.Ngay_Bao_Gia >= @FromDate AND bg.Ngay_Bao_Gia < @ToDateNext
      AND (@Search = N'' OR bg.Ma_Bao_Gia LIKE N'%' + @Search + N'%'
           OR kh.Ten_Khach_Hang LIKE N'%' + @Search + N'%' OR bg.Ten_San_Pham LIKE N'%' + @Search + N'%');
    SELECT ROW_NUMBER() OVER (ORDER BY bg.id DESC) AS STT,
        bg.id AS IDBaoGia, bg.Ma_Bao_Gia AS [Số báo giá], bg.Ngay_Bao_Gia AS [Ngày BG],
        kh.Ten_Khach_Hang AS [Khách hàng], bg.Ten_San_Pham AS [Sản phẩm],
        ISNULL(ct.So_Luong, 0) AS [Số lượng], ISNULL(ct.Tong_Gia_Bao_Khach, 0) AS [Tổng báo khách],
        bg.Trang_Thai AS [Trạng thái]
    FROM dbo.BAO_GIA bg LEFT JOIN dbo.KHACH_HANG kh ON bg.id_Khach_Hang = kh.id
    LEFT JOIN dbo.CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM dbo.CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id
             ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.Ngay_Bao_Gia >= @FromDate AND bg.Ngay_Bao_Gia < @ToDateNext
      AND (@Search = N'' OR bg.Ma_Bao_Gia LIKE N'%' + @Search + N'%'
           OR kh.Ten_Khach_Hang LIKE N'%' + @Search + N'%' OR bg.Ten_San_Pham LIKE N'%' + @Search + N'%')
    ORDER BY bg.id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'✅ Đồng bộ: BAO_GIA.id_Muc_Chinh + sp_GetQuoteList';
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetApprovedQuotes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.id, bg.Ma_Bao_Gia, bg.Ten_San_Pham, kh.Ten_Khach_Hang, ISNULL(ct.So_Luong, 0) AS So_Luong
    FROM dbo.BAO_GIA bg LEFT JOIN dbo.KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN dbo.CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM dbo.CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.Trang_Thai = N'Đã duyệt'
    ORDER BY bg.id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetQuoteForProduction @QuoteId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT bg.Ten_San_Pham, bg.id_Khach_Hang, kh.Ten_Khach_Hang, ISNULL(ct.So_Luong, 0) AS So_Luong
    FROM dbo.BAO_GIA bg LEFT JOIN dbo.KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN dbo.CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM dbo.CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    WHERE bg.id = @QuoteId;
END;
GO

-- Tất cả đơn bán còn nợ (dùng cho lưới chi tiết báo cáo — không phụ thuộc CONG_NO_KHACH_HANG)
CREATE OR ALTER PROCEDURE dbo.sp_GetOpenReceivableInvoiceLines
    @FilterYear  INT = NULL,
    @FilterMonth INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        kh.id AS CustomerId,
        kh.Ten_Khach_Hang,
        dbh.id AS OrderId,
        dbh.Ma_Don_Ban AS OrderCode,
        dbh.Ngay_Ban_Hang AS OrderDate,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han) AS DueDate,
        ISNULL(ct.Ten_San_Pham, N'(không có chi tiết)') AS ProductName,
        dbh.Tong_Thanh_Toan AS TotalAmount,
        ISNULL(SUM(pt.So_Tien_Thu), 0) AS Collected,
        dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) AS Remaining
    FROM dbo.DON_BAN_HANG dbh
    INNER JOIN dbo.KHACH_HANG kh ON kh.id = dbh.id_Khach_Hang
    LEFT JOIN dbo.PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
    LEFT JOIN (
        SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham
        FROM dbo.CHI_TIET_DON_BAN_HANG GROUP BY id_Don_Ban_Hang
    ) ct ON ct.id_Don_Ban_Hang = dbh.id
    WHERE ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy'
      AND (
          @FilterYear IS NULL OR @FilterMonth IS NULL
          OR (YEAR(dbh.Ngay_Ban_Hang) = @FilterYear AND MONTH(dbh.Ngay_Ban_Hang) = @FilterMonth)
      )
    GROUP BY kh.id, kh.Ten_Khach_Hang, dbh.id, dbh.Ma_Don_Ban, dbh.Ngay_Ban_Hang, dbh.Tong_Thanh_Toan, ct.Ten_San_Pham,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han)
    HAVING dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) > 0
    ORDER BY kh.Ten_Khach_Hang, dbh.Ngay_Ban_Hang DESC;
END;
GO

-- Đồng bộ: chi tiết nợ KH (ngày đến hạn), NCC (ngày đến hạn ước tính), dashboard báo giá (mức id_Muc_Chinh)
CREATE OR ALTER PROCEDURE dbo.sp_GetOrderDebtByCustomer @CustomerId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT dbh.id,
        COALESCE(NULLIF(LTRIM(RTRIM(dbh.So_Hoa_Don)), N''), dbh.Ma_Don_Ban) AS OrderCode,
        dbh.Ma_Don_Ban AS DocumentCode,
        dbh.Ngay_Ban_Hang AS OrderDate,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han) AS DueDate,
        ISNULL(ct.Ten_San_Pham, N'(không có chi tiết)') AS ProductName,
        dbh.Tong_Thanh_Toan AS TotalAmount, ISNULL(SUM(pt.So_Tien_Thu), 0) AS Collected,
        dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) AS Remaining
    FROM dbo.DON_BAN_HANG dbh
    LEFT JOIN dbo.PHIEU_THU pt ON pt.id_Don_Ban_Hang = dbh.id
    LEFT JOIN (
        SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham
        FROM dbo.CHI_TIET_DON_BAN_HANG GROUP BY id_Don_Ban_Hang
    ) ct ON ct.id_Don_Ban_Hang = dbh.id
    WHERE dbh.id_Khach_Hang = @CustomerId AND ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy'
    GROUP BY dbh.id, dbh.Ma_Don_Ban, dbh.So_Hoa_Don, dbh.Ngay_Ban_Hang, dbh.Tong_Thanh_Toan, ct.Ten_San_Pham,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han)
    HAVING dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) > 0
    ORDER BY dbh.Ngay_Ban_Hang DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetSalesInvoiceList AS
BEGIN
    SET NOCOUNT ON;
    SELECT dbh.id, dbh.Ma_Don_Ban, dbh.So_Hoa_Don,
        COALESCE(NULLIF(LTRIM(RTRIM(dbh.So_Hoa_Don)), N''), dbh.Ma_Don_Ban) AS So_Hien_Thi_Hoa_Don,
        dbh.Ngay_Ban_Hang, ISNULL(kh.Ten_Khach_Hang, N'(Khách lẻ)') AS Ten_Khach_Hang,
        ISNULL(ct.Ten_San_Pham, N'') AS Ten_San_Pham, ISNULL(ct.Tong_SL, 0) AS So_Luong, dbh.Tong_Thanh_Toan, dbh.Trang_Thai
    FROM dbo.DON_BAN_HANG dbh LEFT JOIN dbo.KHACH_HANG kh ON kh.id = dbh.id_Khach_Hang
    LEFT JOIN (
        SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham, SUM(So_Luong) AS Tong_SL
        FROM dbo.CHI_TIET_DON_BAN_HANG GROUP BY id_Don_Ban_Hang
    ) ct ON ct.id_Don_Ban_Hang = dbh.id
    ORDER BY dbh.Ngay_Ban_Hang DESC, dbh.id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetPayables AS
BEGIN
    SET NOCOUNT ON;
    ;WITH Agg AS (
        SELECT ncc.id, ncc.Ten_NCC,
            SUM(dh.Tong_Tien) AS TotalDebt,
            ISNULL((
                SELECT SUM(tt.So_Tien) FROM dbo.THANH_TOAN_NCC tt WHERE tt.id_Nha_Cung_Cap = ncc.id), 0) AS TotalPaid
        FROM dbo.NHA_CUNG_CAP ncc
        INNER JOIN dbo.DON_DAT_HANG_NCC dh ON dh.id_Nha_Cung_Cap = ncc.id
        GROUP BY ncc.id, ncc.Ten_NCC
    ),
    L AS (
        SELECT a.id, a.Ten_NCC, a.TotalDebt, a.TotalPaid,
            (
                SELECT MIN(DATEADD(DAY, ISNULL(d2.So_Ngay_No, 0), d2.Ngay_Dat_Hang))
                FROM dbo.DON_DAT_HANG_NCC d2
                WHERE d2.id_Nha_Cung_Cap = a.id
                  AND ISNULL(d2.Trang_Thai, N'') <> N'Hủy'
                  AND d2.Tong_Tien - ISNULL((
                      SELECT SUM(t2.So_Tien) FROM dbo.THANH_TOAN_NCC t2 WHERE t2.id_Don_Dat_Hang = d2.id), 0) > 0
            ) AS DueDate
        FROM Agg a
        WHERE a.TotalDebt > a.TotalPaid
        UNION ALL
        SELECT ncc.id, ncc.Ten_NCC, cn.Tong_No, cn.Da_Tra,
            (
                SELECT MIN(DATEADD(DAY, ISNULL(d2.So_Ngay_No, 0), d2.Ngay_Dat_Hang))
                FROM dbo.DON_DAT_HANG_NCC d2
                WHERE d2.id_Nha_Cung_Cap = ncc.id
                  AND ISNULL(d2.Trang_Thai, N'') <> N'Hủy'
                  AND d2.Tong_Tien - ISNULL((
                      SELECT SUM(t2.So_Tien) FROM dbo.THANH_TOAN_NCC t2 WHERE t2.id_Don_Dat_Hang = d2.id), 0) > 0
            )
        FROM dbo.NHA_CUNG_CAP ncc
        INNER JOIN dbo.CONG_NO_NCC cn ON cn.id_Nha_Cung_Cap = ncc.id
        WHERE cn.Con_Lai > 0
          AND NOT EXISTS (SELECT 1 FROM Agg x WHERE x.id = ncc.id)
    )
    SELECT id, Ten_NCC, TotalDebt, TotalPaid, DueDate FROM L ORDER BY Ten_NCC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetDashboardQuotes AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 4 bg.id, bg.Ma_Bao_Gia, ISNULL(kh.Ten_Khach_Hang, N'—') AS Ten_Khach_Hang,
        bg.Ten_San_Pham,
        ISNULL(ct.So_Luong, 0) AS So_Luong,
        ISNULL(ct.Tong_Gia_Bao_Khach, 0) AS Gia_Tri,
        bg.Trang_Thai, bg.Ngay_Bao_Gia
    FROM dbo.BAO_GIA bg
    LEFT JOIN dbo.KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN dbo.CHI_TIET_BAO_GIA ct ON ct.id_Bao_Gia = bg.id
        AND ct.id = COALESCE(
            bg.id_Muc_Chinh,
            (SELECT TOP 1 z.id FROM dbo.CHI_TIET_BAO_GIA z
             WHERE z.id_Bao_Gia = bg.id ORDER BY z.So_Luong DESC, z.id DESC))
    ORDER BY bg.Ngay_Tao DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetOpenReceivableInvoiceLines
    @Keyword     NVARCHAR(200) = N'',   -- lọc tên KH / số HĐ / tên hàng
    @FilterYear  INT           = NULL,
    @FilterMonth INT           = NULL,
    @CustomerId  INT           = NULL   -- chỉ lấy 1 KH (null = tất cả)
AS
BEGIN
    SET NOCOUNT ON;
 
    SELECT
        kh.id                                                   AS CustomerId,
        kh.Ten_Khach_Hang,
        dbh.id                                                  AS OrderId,
        dbh.Ma_Don_Ban                                          AS OrderCode,
        ISNULL(dbh.So_Hoa_Don, dbh.Ma_Don_Ban)                AS SoHoaDon,     -- [NEW] số HĐ
        dbh.Ngay_Ban_Hang                                       AS OrderDate,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han)           AS DueDate,
        ISNULL(ct.Ten_San_Pham, N'(không có chi tiết)')        AS ProductName,
        dbh.Tong_Thanh_Toan                                     AS TotalAmount,
        ISNULL(SUM(pt.So_Tien_Thu), 0)                         AS Collected,
        dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0)  AS Remaining
    FROM dbo.DON_BAN_HANG dbh
    INNER JOIN dbo.KHACH_HANG kh ON kh.id = dbh.id_Khach_Hang
    LEFT  JOIN dbo.PHIEU_THU pt  ON pt.id_Don_Ban_Hang = dbh.id
    LEFT  JOIN (
        SELECT id_Don_Ban_Hang, MAX(Ten_San_Pham) AS Ten_San_Pham
        FROM dbo.CHI_TIET_DON_BAN_HANG
        GROUP BY id_Don_Ban_Hang
    ) ct ON ct.id_Don_Ban_Hang = dbh.id
    WHERE ISNULL(dbh.Trang_Thai, N'') <> N'Đã hủy'
      -- [FIX] Lọc tháng (theo ngày bán) - sửa logic OR
      -- Nếu cả Year và Month đều có giá trị -> lọc theo cả hai
      -- Nếu chỉ có Year -> lọc theo Year
      -- Nếu chỉ có Month -> lọc theo Month
      AND (
          (@FilterYear IS NOT NULL AND @FilterMonth IS NOT NULL
           AND YEAR(dbh.Ngay_Ban_Hang) = @FilterYear
           AND MONTH(dbh.Ngay_Ban_Hang) = @FilterMonth)
          OR (@FilterYear IS NOT NULL AND @FilterMonth IS NULL
              AND YEAR(dbh.Ngay_Ban_Hang) = @FilterYear)
          OR (@FilterYear IS NULL AND @FilterMonth IS NOT NULL
              AND MONTH(dbh.Ngay_Ban_Hang) = @FilterMonth)
          OR (@FilterYear IS NULL AND @FilterMonth IS NULL)
      )
      -- Lọc 1 KH cụ thể (chọn từ lưới trên)
      AND (@CustomerId IS NULL OR kh.id = @CustomerId)
      -- [NEW] Lọc keyword: tên KH, mã chứng từ, số HĐ, tên hàng
      AND (
          @Keyword = N''
          OR kh.Ten_Khach_Hang              LIKE N'%' + @Keyword + N'%'
          OR dbh.Ma_Don_Ban                 LIKE N'%' + @Keyword + N'%'
          OR ISNULL(dbh.So_Hoa_Don, N'')   LIKE N'%' + @Keyword + N'%'
          OR ISNULL(ct.Ten_San_Pham, N'')  LIKE N'%' + @Keyword + N'%'
      )
    GROUP BY
        kh.id, kh.Ten_Khach_Hang,
        dbh.id, dbh.Ma_Don_Ban, dbh.So_Hoa_Don,
        dbh.Ngay_Ban_Hang, dbh.Tong_Thanh_Toan, ct.Ten_San_Pham,
        COALESCE(dbh.Ngay_Uoc_Thu, dbh.Ngay_Den_Han)
    HAVING dbh.Tong_Thanh_Toan - ISNULL(SUM(pt.So_Tien_Thu), 0) > 0
    ORDER BY kh.Ten_Khach_Hang, dbh.Ngay_Ban_Hang DESC;
END;
GO
 
PRINT N'✅ sp_GetOpenReceivableInvoiceLines đã cập nhật — thêm @Keyword + SoHoaDon';
GO
PRINT N'✅ sp_GetPurchaseSummary đã sửa';
GO
PRINT N'✅ Patch hoàn thành! Đã thêm So_Ngay_No + Ngay_Den_Han_TT.';
GO
PRINT N'✅ sp_GenerateCustomerCode đã tạo thành công!';
GO

-- =====================================================
-- BẢNG: PASSWORD_RESET — Lưu OTP đặt lại mật khẩu
-- [FIX] Thêm cột Otp_Code để verify ở SP
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PASSWORD_RESET')
BEGIN
    CREATE TABLE PASSWORD_RESET (
        id          INT IDENTITY(1,1) PRIMARY KEY,
        Email       NVARCHAR(100)  NOT NULL,
        Otp_Hash    NVARCHAR(255)  NOT NULL,
        Otp_Code    NVARCHAR(10)   NOT NULL,  -- [NEW] Lưu OTP thuần để verify ở SP
        Het_Han     DATETIME       NOT NULL,
        Da_Su_Dung  BIT            NOT NULL DEFAULT 0,
        Ngay_Tao    DATETIME       NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Thêm cột nếu bảng đã tồn tại nhưng thiếu Otp_Code
IF COL_LENGTH(N'PASSWORD_RESET', N'Otp_Code') IS NULL
    ALTER TABLE PASSWORD_RESET ADD Otp_Code NVARCHAR(10) NULL;
GO

-- =====================================================
-- SP: YÊU CẦU ĐẶT LẠI MẬT KHẨU
-- =====================================================
CREATE PROCEDURE sp_RequestPasswordReset
    @Email    NVARCHAR(100),
    @OtpHash  NVARCHAR(255),
    @OtpCode  NVARCHAR(10),  -- [NEW] Thêm tham số OTP code
    @Expiry   DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra email có tồn tại trong hệ thống không
    IF NOT EXISTS (SELECT 1 FROM USERS WHERE Email = @Email AND Trang_Thai = 1)
    BEGIN
        -- Không trả về gì để báo email không tồn tại
        RETURN;
    END

    -- Vô hiệu hóa các OTP cũ chưa dùng của email này
    UPDATE PASSWORD_RESET
    SET Da_Su_Dung = 1
    WHERE Email = @Email AND Da_Su_Dung = 0;

    -- Lưu OTP mới (bao gồm Otp_Code để verify ở sp_VerifyPasswordReset)
    INSERT INTO PASSWORD_RESET (Email, Otp_Hash, Otp_Code, Het_Han, Da_Su_Dung)
    VALUES (@Email, @OtpHash, @OtpCode, @Expiry, 0);

    -- Trả về email để C# xác nhận thành công
    SELECT @Email AS Email;
END
GO

-- =====================================================
-- SP: ĐÁNH DẤU OTP ĐÃ DÙNG + ĐẶT LẠI MẬT KHẨU
-- [FIX] BẮT BUỘC phải có OTP hợp lệ CHƯA DÙNG mới cho đặt lại mật khẩu
-- =====================================================
CREATE PROCEDURE sp_VerifyPasswordReset
    @Email        NVARCHAR(100),
    @OtpCode      NVARCHAR(10),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- [FIX] Kiểm tra OTP hợp lệ trước khi đổi mật khẩu
    DECLARE @otpValid BIT = 0;
    SELECT @otpValid = 1
    FROM PASSWORD_RESET
    WHERE Email = @Email
      AND Otp_Code = @OtpCode
      AND Da_Su_Dung = 0
      AND Het_Han > GETDATE();

    IF @otpValid = 0
    BEGIN
        SELECT -1 AS Result, N'Mã OTP không hợp lệ hoặc đã hết hạn.' AS ErrorMessage;
        RETURN;
    END

    -- Đánh dấu OTP đã dùng
    UPDATE PASSWORD_RESET
    SET Da_Su_Dung = 1
    WHERE Email = @Email AND Otp_Code = @OtpCode AND Da_Su_Dung = 0;

    -- Cập nhật mật khẩu mới (BCrypt hash được tạo ở C# rồi truyền vào đây)
    UPDATE USERS
    SET Mat_Khau = @NewPassword
    WHERE Email = @Email;

    SELECT 1 AS Result, N'Đặt lại mật khẩu thành công.' AS ErrorMessage;
END
GO

-- =====================================================
-- KIỂM TRA
-- =====================================================
-- ╔══════════════════════════════════════════════════════════════════════╗
-- ║  PHIẾU GIAO HÀNG – PHIEU_GIAO_HANG & CHI_TIET_PGH                  ║
-- ╚══════════════════════════════════════════════════════════════════════╝

-- =====================================================
-- BẢNG 23: PHIEU_GIAO_HANG
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PHIEU_GIAO_HANG')
BEGIN
    CREATE TABLE PHIEU_GIAO_HANG (
        id                   INT           PRIMARY KEY IDENTITY(1,1),
        Ma_Phieu_Giao        NVARCHAR(50)  UNIQUE NOT NULL,
        id_Bao_Gia           INT           FOREIGN KEY REFERENCES BAO_GIA(id),
        id_Khach_Hang        INT           FOREIGN KEY REFERENCES KHACH_HANG(id),
        Ma_KH                NVARCHAR(50)  NULL,
        Ten_KH               NVARCHAR(200) NULL,
        Dia_Chi_Giao_Hang    NVARCHAR(500) NULL,
        Nguoi_Nhan            NVARCHAR(100) NULL,
        SDT_Nguoi_Nhan        NVARCHAR(20)  NULL,
        Ngay_Giao             DATE          NOT NULL,
        Gio_Giao              TIME          NULL,
        Hinh_Thuc_Giao        NVARCHAR(100) NULL,
        Ghi_Chu_Hinh_Thuc     NVARCHAR(500) NULL,
        Ten_Tai_Xe           NVARCHAR(100) NULL,
        Trang_Thai            NVARCHAR(50)  DEFAULT N'Mới lập'
            CHECK (Trang_Thai IN (N'Mới lập', N'Đang giao', N'Đã giao', N'Hủy')),
        Tong_Tien             DECIMAL(18,2) DEFAULT 0,
        Ghi_Chu               NVARCHAR(MAX) NULL,
        Ngay_Tao              DATETIME      DEFAULT GETDATE(),
        Nguoi_Tao             NVARCHAR(100) NULL,
        Ngay_Sua              DATETIME      NULL,
        Nguoi_Sua             NVARCHAR(100) NULL
    );
END
GO

CREATE INDEX IX_PGH_Ma ON PHIEU_GIAO_HANG (Ma_Phieu_Giao);
CREATE INDEX IX_PGH_TrangThai ON PHIEU_GIAO_HANG (Trang_Thai);
CREATE INDEX IX_PGH_KH ON PHIEU_GIAO_HANG (id_Khach_Hang);
GO

-- =====================================================
-- BẢNG 24: CHI_TIET_PGH
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CHI_TIET_PGH')
BEGIN
    CREATE TABLE CHI_TIET_PGH (
        id                   INT           PRIMARY KEY IDENTITY(1,1),
        id_Phieu_Giao_Hang   INT           FOREIGN KEY REFERENCES PHIEU_GIAO_HANG(id) ON DELETE CASCADE,
        id_Chi_Tiet_Bao_Gia  INT           FOREIGN KEY REFERENCES CHI_TIET_BAO_GIA(id) NULL,
        Ten_San_Pham         NVARCHAR(200) NULL,
        Don_Vi_Tinh          NVARCHAR(50)  NULL,
        So_Luong_Bao_Gia     INT           DEFAULT 0,
        So_Luong_Giao        INT           NOT NULL,
        Don_Gia              DECIMAL(18,2) DEFAULT 0,
        Thanh_Tien           DECIMAL(18,2) DEFAULT 0
    );
END
GO

IF COL_LENGTH(N'dbo.DON_BAN_HANG', N'id_Phieu_Giao_Hang') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DON_BAN_HANG_PHIEU_GIAO_HANG')
BEGIN
    ALTER TABLE dbo.DON_BAN_HANG ADD CONSTRAINT FK_DON_BAN_HANG_PHIEU_GIAO_HANG
        FOREIGN KEY (id_Phieu_Giao_Hang) REFERENCES dbo.PHIEU_GIAO_HANG(id);
END
GO

-- =====================================================
-- SP: SINH MÃ PHIẾU GIAO HÀNG
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GenerateDeliveryCode')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GenerateDeliveryCode
            @Year       INT,
            @NextCode   NVARCHAR(50) OUTPUT
        AS
        BEGIN
            SET NOCOUNT ON;

            DECLARE @Prefix  NVARCHAR(10) = ''PGH-'' + CAST(@Year AS NVARCHAR(4)) + ''-'';
            DECLARE @MaxNum  INT;

            SELECT @MaxNum = MAX(CAST(SUBSTRING(Ma_Phieu_Giao, LEN(@Prefix)+1, 10) AS INT))
            FROM PHIEU_GIAO_HANG
            WHERE Ma_Phieu_Giao LIKE @Prefix + ''%''
              AND ISNUMERIC(SUBSTRING(Ma_Phieu_Giao, LEN(@Prefix)+1, 10)) = 1;

            SET @MaxNum = ISNULL(@MaxNum, 0) + 1;
            SET @NextCode = @Prefix + RIGHT(''000'' + CAST(@MaxNum AS NVARCHAR(10)), 3);
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY BÁO GIÁ HOÀN THÀNH CHO COMBOBOX
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetCompletedQuotesForDelivery')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetCompletedQuotesForDelivery
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                bg.id,
                bg.Ma_Bao_Gia,
                bg.Ten_San_Pham,
                bg.id_Khach_Hang,
                kh.Ma_KH,
                kh.Ten_Khach_Hang,
                kh.Dia_Chi,
                kh.Nguoi_Lien_He,
                kh.Dien_Thoai,
                bg.Ngay_Bao_Gia,
                ISNULL(ctbg.Gia_Bao_Khach, 0) AS Don_Gia
            FROM BAO_GIA bg
            INNER JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
            LEFT JOIN CHI_TIET_BAO_GIA ctbg ON ctbg.id = bg.id_Muc_Chinh
            WHERE bg.Trang_Thai = N''Hoàn thành''
            ORDER BY bg.Ngay_Tao DESC;
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY CHI TIẾT BÁO GIÁ ĐỂ TẠO PHIẾU GIAO
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetQuoteDetailsForDelivery')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetQuoteDetailsForDelivery
            @QuoteId    INT
        AS
        BEGIN
            SET NOCOUNT ON;

            -- Lấy đúng 1 dòng: ưu tiên id_Muc_Chinh đã tích ở Quản lý báo giá,
            -- fallback về mức SL lớn nhất (nếu chưa tích hoặc tích null).
            SELECT TOP 1
                ctb.id                AS DetailId,
                bg.Ten_San_Pham       AS Ten_San_Pham,
                NULL                  AS Don_Vi_Tinh,
                ctb.So_Luong          AS So_Luong_Bao_Gia,
                ctb.So_Luong          AS So_Luong_Giao,
                ISNULL(ctb.Gia_Bao_Khach, 0) AS Don_Gia,
                ISNULL(ctb.Gia_Bao_Khach, 0) * ctb.So_Luong AS Thanh_Tien
            FROM CHI_TIET_BAO_GIA ctb
            INNER JOIN BAO_GIA bg ON bg.id = ctb.id_Bao_Gia
            WHERE ctb.id_Bao_Gia = @QuoteId
            ORDER BY
                CASE WHEN ctb.id = bg.id_Muc_Chinh THEN 0 ELSE 1 END,
                ctb.So_Luong DESC;
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY THÔNG TIN KHÁCH HÀNG THEO ID
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetCustomerForDelivery')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetCustomerForDelivery
            @CustomerId     INT
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                id,
                Ma_KH,
                Ten_Khach_Hang,
                Dia_Chi,
                Nguoi_Lien_He,
                Dien_Thoai
            FROM KHACH_HANG
            WHERE id = @CustomerId;
        END
    ');
END
GO

-- =====================================================
-- SP: LƯU / CẬP NHẬT PHIẾU GIAO HÀNG
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_SaveDeliveryNote')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_SaveDeliveryNote
            @PhieuGiaoId       INT,
            @MaPhieuGiao       NVARCHAR(50),
            @BaoGiaId          INT,
            @KhachHangId       INT,
            @MaKH              NVARCHAR(50),
            @TenKH             NVARCHAR(200),
            @DiaChiGiao        NVARCHAR(500),
            @NguoiNhan         NVARCHAR(100),
            @SDTNguoiNhan      NVARCHAR(20),
            @NgayGiao          DATE,
            @GioGiao           NVARCHAR(8),
            @HinhThucGiao      NVARCHAR(100),
            @GhiChuHinhThuc   NVARCHAR(500),
            @TenTaiXe          NVARCHAR(100),
            @TongTien          DECIMAL(18,2),
            @GhiChu           NVARCHAR(MAX),
            @NguoiTao         NVARCHAR(100),
            @DetailsXml        NVARCHAR(MAX),
            @NewPhieuGiaoId    INT OUTPUT
        AS
        BEGIN
            SET NOCOUNT ON;
            BEGIN TRANSACTION;

            BEGIN TRY
                DECLARE @NewId INT;

                IF @PhieuGiaoId = 0
                BEGIN
                    INSERT INTO PHIEU_GIAO_HANG (
                        Ma_Phieu_Giao, id_Bao_Gia, id_Khach_Hang,
                        Ma_KH, Ten_KH, Dia_Chi_Giao_Hang,
                        Nguoi_Nhan, SDT_Nguoi_Nhan, Ngay_Giao, Gio_Giao,
                        Hinh_Thuc_Giao, Ghi_Chu_Hinh_Thuc, Ten_Tai_Xe,
                        Trang_Thai, Tong_Tien, Ghi_Chu, Ngay_Tao, Nguoi_Tao
                    )
                    VALUES (
                        @MaPhieuGiao, @BaoGiaId, @KhachHangId,
                        @MaKH, @TenKH, @DiaChiGiao,
                        @NguoiNhan, @SDTNguoiNhan, @NgayGiao,
                        CASE WHEN @GioGiao IS NOT NULL THEN CAST(@GioGiao AS TIME) ELSE NULL END,
                        @HinhThucGiao, @GhiChuHinhThuc, @TenTaiXe,
                        N''Mới lập'', @TongTien, @GhiChu, GETDATE(), @NguoiTao
                    );

                    SET @NewId = SCOPE_IDENTITY();
                END
                ELSE
                BEGIN
                    UPDATE PHIEU_GIAO_HANG SET
                        Ma_Phieu_Giao     = @MaPhieuGiao,
                        id_Bao_Gia        = @BaoGiaId,
                        id_Khach_Hang     = @KhachHangId,
                        Ma_KH             = @MaKH,
                        Ten_KH            = @TenKH,
                        Dia_Chi_Giao_Hang = @DiaChiGiao,
                        Nguoi_Nhan        = @NguoiNhan,
                        SDT_Nguoi_Nhan    = @SDTNguoiNhan,
                        Ngay_Giao         = @NgayGiao,
                        Gio_Giao          = CASE WHEN @GioGiao IS NOT NULL THEN CAST(@GioGiao AS TIME) ELSE NULL END,
                        Hinh_Thuc_Giao    = @HinhThucGiao,
                        Ghi_Chu_Hinh_Thuc = @GhiChuHinhThuc,
                        Ten_Tai_Xe        = @TenTaiXe,
                        Tong_Tien         = @TongTien,
                        Ghi_Chu          = @GhiChu,
                        Ngay_Sua          = GETDATE(),
                        Nguoi_Sua         = @NguoiTao
                    WHERE id = @PhieuGiaoId;

                    DELETE FROM CHI_TIET_PGH WHERE id_Phieu_Giao_Hang = @PhieuGiaoId;
                    SET @NewId = @PhieuGiaoId;
                END

                -- Parse XML chi tiết
                DECLARE @Xml XML = CAST(@DetailsXml AS XML);
                INSERT INTO CHI_TIET_PGH (
                    id_Phieu_Giao_Hang, id_Chi_Tiet_Bao_Gia,
                    Ten_San_Pham, Don_Vi_Tinh,
                    So_Luong_Bao_Gia, So_Luong_Giao, Don_Gia, Thanh_Tien
                )
                SELECT
                    @NewId,
                    t.c.value(''(DetailId)[1]'',        ''INT''),
                    t.c.value(''(TenSanPham)[1]'',     ''NVARCHAR(200)''),
                    t.c.value(''(DonViTinh)[1]'',      ''NVARCHAR(50)''),
                    t.c.value(''(SoLuongBaoGia)[1]'',  ''INT''),
                    t.c.value(''(SoLuongGiao)[1]'',    ''INT''),
                    t.c.value(''(DonGia)[1]'',         ''DECIMAL(18,2)''),
                    t.c.value(''(ThanhTien)[1]'',      ''DECIMAL(18,2)'')
                FROM @Xml.nodes(''/Details/Item'') AS t(c);

                SET @NewPhieuGiaoId = @NewId;
                COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                THROW;
            END CATCH
        END
    ');
END
GO

-- =====================================================
-- SP: CẬP NHẬT TRẠNG THÁI PHIẾU GIAO HÀNG
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_UpdateDeliveryStatus')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_UpdateDeliveryStatus
            @PhieuGiaoId   INT,
            @TrangThai     NVARCHAR(50),
            @NguoiSua      NVARCHAR(100)
        AS
        BEGIN
            SET NOCOUNT ON;

            UPDATE PHIEU_GIAO_HANG
            SET Trang_Thai = @TrangThai,
                Ngay_Sua   = GETDATE(),
                Nguoi_Sua  = @NguoiSua
            WHERE id = @PhieuGiaoId;

            SELECT 1 AS Result;
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY DANH SÁCH PHIẾU GIAO HÀNG ĐÃ LƯU (CHO COMBOBOX PICK LẠI)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetSavedDeliveryCodes')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetSavedDeliveryCodes
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                id,
                Ma_Phieu_Giao,
                Ngay_Giao,
                Trang_Thai,
                Ten_KH
            FROM PHIEU_GIAO_HANG
            ORDER BY Ngay_Tao DESC;
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY PHIẾU GIAO HÀNG THEO ID (LOAD LẠI KHI PICK COMBOBOX)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetDeliveryNoteById')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetDeliveryNoteById
            @PhieuGiaoId     INT
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                pgh.*,
                bg.Ma_Bao_Gia
            FROM PHIEU_GIAO_HANG pgh
            LEFT JOIN BAO_GIA bg ON bg.id = pgh.id_Bao_Gia
            WHERE pgh.id = @PhieuGiaoId;

            SELECT
                ct.*
            FROM CHI_TIET_PGH ct
            WHERE ct.id_Phieu_Giao_Hang = @PhieuGiaoId;
        END
    ');
END
GO

-- =====================================================
-- SP: LẤY DANH SÁCH PHIẾU GIAO CHO BÁO CÁO (Xuất Excel)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_GetDeliveryNoteList')
BEGIN
    EXEC('
        CREATE PROCEDURE sp_GetDeliveryNoteList
            @FromDate   DATE = NULL,
            @ToDate     DATE = NULL,
            @TrangThai  NVARCHAR(50) = NULL
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                pgh.Ma_Phieu_Giao,
                pgh.Ngay_Giao,
                pgh.Ten_KH,
                pgh.Dia_Chi_Giao_Hang,
                pgh.Nguoi_Nhan,
                pgh.SDT_Nguoi_Nhan,
                pgh.Gio_Giao,
                pgh.Hinh_Thuc_Giao,
                pgh.Ten_Tai_Xe,
                pgh.Trang_Thai,
                pgh.Tong_Tien,
                pgh.Ghi_Chu,
                bg.Ma_Bao_Gia,
                pgh.Ngay_Tao
            FROM PHIEU_GIAO_HANG pgh
            LEFT JOIN BAO_GIA bg ON bg.id = pgh.id_Bao_Gia
            WHERE (@FromDate IS NULL OR pgh.Ngay_Giao >= @FromDate)
              AND (@ToDate   IS NULL OR pgh.Ngay_Giao <= @ToDate)
              AND (@TrangThai IS NULL OR pgh.Trang_Thai = @TrangThai)
            ORDER BY pgh.Ngay_Tao DESC;
        END
    ');
END
GO

-- =====================================================
-- PATCH: Phiếu giao — ẩn báo giá đã có phiếu trạng thái "Đã giao"
-- Đơn mua — danh sách đơn theo NCC, cập nhật đơn, lưu nhiều dòng chi tiết
-- =====================================================
CREATE OR ALTER PROCEDURE dbo.sp_GetCompletedQuotesForDelivery
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bg.id,
        bg.Ma_Bao_Gia,
        bg.Ten_San_Pham,
        bg.id_Khach_Hang,
        kh.Ma_KH,
        kh.Ten_Khach_Hang,
        kh.Dia_Chi,
        kh.Nguoi_Lien_He,
        kh.Dien_Thoai,
        bg.Ngay_Bao_Gia,
        ISNULL(ctbg.Gia_Bao_Khach, 0) AS Don_Gia
    FROM BAO_GIA bg
    INNER JOIN KHACH_HANG kh ON kh.id = bg.id_Khach_Hang
    LEFT JOIN CHI_TIET_BAO_GIA ctbg ON ctbg.id = bg.id_Muc_Chinh
    WHERE bg.Trang_Thai = N'Hoàn thành'
      AND NOT EXISTS (
          SELECT 1 FROM dbo.PHIEU_GIAO_HANG pgh
          WHERE pgh.id_Bao_Gia = bg.id AND pgh.Trang_Thai = N'Đã giao'
      )
    ORDER BY bg.Ngay_Tao DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetOpenPurchaseOrdersBySupplier
    @SupplierId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @SupplierId IS NULL OR @SupplierId <= 0 RETURN;

    SELECT
        d.id,
        d.Ma_Don_Hang,
        d.Ngay_Dat_Hang,
        d.Trang_Thai,
        d.Tong_Tien
    FROM dbo.DON_DAT_HANG_NCC d
    WHERE d.id_Nha_Cung_Cap = @SupplierId
      AND d.Trang_Thai NOT IN (N'Hoàn thành', N'Hủy')
    ORDER BY d.Ngay_Dat_Hang DESC, d.id DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetPurchaseOrderById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.id,
        d.Ma_Don_Hang,
        d.id_Nha_Cung_Cap,
        d.Ngay_Dat_Hang,
        d.Ngay_Giao_Hang,
        d.Dieu_Khoan_Thanh_Toan,
        d.So_Ngay_No,
        d.Trang_Thai,
        d.Dia_Diem_Giao_Hang,
        d.File_Dinh_Kem,
        d.Tong_Tien,
        d.Ghi_Chu
    FROM dbo.DON_DAT_HANG_NCC d
    WHERE d.id = @OrderId;

    SELECT
        nl.id AS MaterialId,
        nl.Ma_Nguyen_Lieu,
        nl.Ten_Nguyen_Lieu,
        nl.Don_Vi_Tinh,
        ct.So_Luong,
        ct.Don_Gia,
        ct.Thanh_Tien,
        ct.Phan_Tram_Thue_GTGT,
        ct.Tien_Thue_GTGT
    FROM dbo.CHI_TIET_DON_HANG_NCC ct
    INNER JOIN dbo.NGUYEN_LIEU nl ON nl.id = ct.id_Nguyen_Lieu
    WHERE ct.id_Don_Dat_Hang = @OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdatePurchaseOrder
    @OrderId INT,
    @OrderCode NVARCHAR(50),
    @SupplierId INT,
    @OrderDate DATE,
    @DeliveryDate DATE,
    @PaymentMethod NVARCHAR(100),
    @DebtDays INT,
    @OrderStatus NVARCHAR(50),
    @DeliveryLocation NVARCHAR(500),
    @AttachedFile NVARCHAR(500),
    @GhiChu NVARCHAR(MAX) = NULL,
    @GrandTotal DECIMAL(18,2),
    @Details XML,
    @NewOrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewOrderId = @OrderId;

    IF NOT EXISTS (SELECT 1 FROM dbo.DON_DAT_HANG_NCC WHERE id = @OrderId)
    BEGIN
        RAISERROR(N'Không tìm thấy đơn hàng.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.PHIEU_NHAP_KHO WHERE id_Don_Dat_Hang = @OrderId)
    BEGIN
        RAISERROR(N'Đơn đã có phiếu nhập kho — không ghi đè chi tiết. Vui lòng tạo đơn mua mới để thêm hàng.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.DON_DAT_HANG_NCC SET
            Ma_Don_Hang = @OrderCode,
            id_Nha_Cung_Cap = @SupplierId,
            Ngay_Dat_Hang = @OrderDate,
            Ngay_Giao_Hang = @DeliveryDate,
            Dieu_Khoan_Thanh_Toan = @PaymentMethod,
            So_Ngay_No = @DebtDays,
            Trang_Thai = @OrderStatus,
            Dia_Diem_Giao_Hang = @DeliveryLocation,
            File_Dinh_Kem = @AttachedFile,
            Ghi_Chu = @GhiChu,
            Tong_Tien = @GrandTotal
        WHERE id = @OrderId;

        DELETE FROM dbo.CHI_TIET_DON_HANG_NCC WHERE id_Don_Dat_Hang = @OrderId;

        DECLARE @MaterialId INT, @MaterialCode NVARCHAR(50), @MaterialName NVARCHAR(200),
            @Unit NVARCHAR(50), @Qty FLOAT, @UnitPrice FLOAT, @LineTotal FLOAT,
            @VatRate FLOAT, @VatAmount FLOAT, @ExistingId INT;

        DECLARE item_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            d.value('(MaterialId)[1]', 'INT'),
            d.value('(MaterialCode)[1]', 'NVARCHAR(50)'),
            d.value('(MaterialName)[1]', 'NVARCHAR(200)'),
            d.value('(Unit)[1]', 'NVARCHAR(50)'),
            d.value('(Qty)[1]', 'FLOAT'),
            d.value('(UnitPrice)[1]', 'FLOAT'),
            d.value('(LineTotal)[1]', 'FLOAT'),
            d.value('(VatRate)[1]', 'FLOAT'),
            d.value('(VatAmount)[1]', 'FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);

        OPEN item_cur;
        FETCH NEXT FROM item_cur INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
            @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @MaterialName IS NULL OR LTRIM(RTRIM(@MaterialName)) = N''
            BEGIN
                FETCH NEXT FROM item_cur INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
                    @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
                CONTINUE;
            END

            IF @MaterialId IS NULL OR @MaterialId = 0
            BEGIN
                SELECT @ExistingId = id FROM dbo.NGUYEN_LIEU WHERE Ma_Nguyen_Lieu = @MaterialCode;
                IF @ExistingId IS NOT NULL SET @MaterialId = @ExistingId;
                ELSE BEGIN
                    INSERT INTO dbo.NGUYEN_LIEU (Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap, Ton_Kho, Ton_Kho_Toi_Thieu)
                    VALUES (@MaterialCode, @MaterialName, @Unit, @UnitPrice, 0, 0);
                    SET @MaterialId = SCOPE_IDENTITY();
                END
            END

            INSERT INTO dbo.CHI_TIET_DON_HANG_NCC (id_Don_Dat_Hang, id_Nguyen_Lieu, So_Luong, So_Luong_Da_Nhan,
                Don_Gia, Thanh_Tien, Phan_Tram_Thue_GTGT, Tien_Thue_GTGT)
            VALUES (@OrderId, @MaterialId, @Qty, 0, @UnitPrice, @LineTotal, @VatRate, @VatAmount);

            FETCH NEXT FROM item_cur INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
                @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
        END

        CLOSE item_cur;
        DEALLOCATE item_cur;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'item_cur') >= 0
        BEGIN
            CLOSE item_cur;
            DEALLOCATE item_cur;
        END
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @em NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@em, 16, 1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_SavePurchaseOrder
    @OrderCode NVARCHAR(50), @SupplierId INT, @OrderDate DATE, @DeliveryDate DATE,
    @PaymentMethod NVARCHAR(100), @DebtDays INT, @OrderStatus NVARCHAR(50),
    @DeliveryLocation NVARCHAR(500), @AttachedFile NVARCHAR(500), @GhiChu NVARCHAR(MAX) = NULL,
    @GrandTotal DECIMAL(18,2),
    @Details XML, @NewOrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewOrderId = 0;

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.DON_DAT_HANG_NCC (Ma_Don_Hang, id_Nha_Cung_Cap, Ngay_Dat_Hang, Ngay_Giao_Hang,
            Dieu_Khoan_Thanh_Toan, So_Ngay_No, Trang_Thai, Dia_Diem_Giao_Hang, File_Dinh_Kem, Ghi_Chu, Tong_Tien)
        VALUES (@OrderCode, @SupplierId, @OrderDate, @DeliveryDate, @PaymentMethod, @DebtDays,
            @OrderStatus, @DeliveryLocation, @AttachedFile, @GhiChu, @GrandTotal);
        SET @NewOrderId = SCOPE_IDENTITY();

        DECLARE @MaterialId INT, @MaterialCode NVARCHAR(50), @MaterialName NVARCHAR(200),
            @Unit NVARCHAR(50), @Qty FLOAT, @UnitPrice FLOAT, @LineTotal FLOAT,
            @VatRate FLOAT, @VatAmount FLOAT, @ExistingId INT;

        DECLARE item_cur2 CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            d.value('(MaterialId)[1]', 'INT'),
            d.value('(MaterialCode)[1]', 'NVARCHAR(50)'),
            d.value('(MaterialName)[1]', 'NVARCHAR(200)'),
            d.value('(Unit)[1]', 'NVARCHAR(50)'),
            d.value('(Qty)[1]', 'FLOAT'),
            d.value('(UnitPrice)[1]', 'FLOAT'),
            d.value('(LineTotal)[1]', 'FLOAT'),
            d.value('(VatRate)[1]', 'FLOAT'),
            d.value('(VatAmount)[1]', 'FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);

        OPEN item_cur2;
        FETCH NEXT FROM item_cur2 INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
            @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @MaterialName IS NULL OR LTRIM(RTRIM(@MaterialName)) = N''
            BEGIN
                FETCH NEXT FROM item_cur2 INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
                    @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
                CONTINUE;
            END

            IF @MaterialId IS NULL OR @MaterialId = 0
            BEGIN
                SELECT @ExistingId = id FROM dbo.NGUYEN_LIEU WHERE Ma_Nguyen_Lieu = @MaterialCode;
                IF @ExistingId IS NOT NULL SET @MaterialId = @ExistingId;
                ELSE BEGIN
                    INSERT INTO dbo.NGUYEN_LIEU (Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap, Ton_Kho, Ton_Kho_Toi_Thieu)
                    VALUES (@MaterialCode, @MaterialName, @Unit, @UnitPrice, 0, 0);
                    SET @MaterialId = SCOPE_IDENTITY();
                END
            END

            INSERT INTO dbo.CHI_TIET_DON_HANG_NCC (id_Don_Dat_Hang, id_Nguyen_Lieu, So_Luong, So_Luong_Da_Nhan,
                Don_Gia, Thanh_Tien, Phan_Tram_Thue_GTGT, Tien_Thue_GTGT)
            VALUES (@NewOrderId, @MaterialId, @Qty, 0, @UnitPrice, @LineTotal, @VatRate, @VatAmount);

            FETCH NEXT FROM item_cur2 INTO @MaterialId, @MaterialCode, @MaterialName, @Unit,
                @Qty, @UnitPrice, @LineTotal, @VatRate, @VatAmount;
        END

        CLOSE item_cur2;
        DEALLOCATE item_cur2;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'item_cur2') >= 0
        BEGIN
            CLOSE item_cur2;
            DEALLOCATE item_cur2;
        END
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @em2 NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@em2, 16, 1);
    END CATCH
END
GO

-- =====================================================
-- KIỂM TRA
-- =====================================================
-- ╔══════════════════════════════════════════════════════════════════════════╗
-- ║  NguyenLieu_NCC — Danh mục vật tư theo NCC                              ║
-- ╚══════════════════════════════════════════════════════════════════════════╝

-- =====================================================
-- SP: sp_GetMaterialsBySupplier
-- Lấy danh sách vật tư đã lưu của 1 NCC
-- =====================================================
CREATE OR ALTER PROCEDURE dbo.sp_GetMaterialsBySupplier
    @SupplierId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT nl.id AS MaterialId,
           nl.Ma_Nguyen_Lieu,
           nl.Ten_Nguyen_Lieu,
           nl.Don_Vi_Tinh,
           ISNULL(mncc.Don_Gia_Mac_Dinh, 0) AS Don_Gia_Mac_Dinh,
           ISNULL(nl.Gia_Nhap, 0) AS Gia_Nhap
    FROM dbo.NguyenLieu_NCC mncc
    INNER JOIN dbo.NGUYEN_LIEU nl ON nl.id = mncc.id_Nguyen_Lieu
    WHERE mncc.id_Nha_Cung_Cap = @SupplierId
    ORDER BY nl.Ten_Nguyen_Lieu;
END
GO

-- =====================================================
-- SP: sp_SaveMaterialsForSupplier
-- Lưu danh sách vật tư vào danh mục NCC (INSERT OR IGNORE).
-- Gọi sau khi lưu đơn mua — những vật tư chưa có trong danh mục NCC sẽ được tự động thêm.
-- =====================================================
CREATE OR ALTER PROCEDURE dbo.sp_SaveMaterialsForSupplier
    @SupplierId INT,
    @Materials XML
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaterialId INT, @MaterialCode NVARCHAR(50), @MaterialName NVARCHAR(200),
            @Unit NVARCHAR(50), @UnitPrice FLOAT;
    DECLARE @NewMaterialId INT;

    DECLARE mat_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.value('(MaterialId)[1]', 'INT'),
               d.value('(MaterialCode)[1]', 'NVARCHAR(50)'),
               d.value('(MaterialName)[1]', 'NVARCHAR(200)'),
               d.value('(Unit)[1]', 'NVARCHAR(50)'),
               d.value('(UnitPrice)[1]', 'FLOAT')
        FROM @Materials.nodes('/Materials/Item') AS T(d);

    OPEN mat_cur;
    FETCH NEXT FROM mat_cur INTO @MaterialId, @MaterialCode, @MaterialName, @Unit, @UnitPrice;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @MaterialName IS NOT NULL AND LTRIM(RTRIM(@MaterialName)) <> N''
        BEGIN
            -- Nếu MaterialId = 0 → nguyên liệu mới, tạo trong NGUYEN_LIEU trước
            IF @MaterialId = 0 OR @MaterialId IS NULL
            BEGIN
                -- Kiểm tra đã tồn tại trong NGUYEN_LIEU chưa
                SELECT @NewMaterialId = id FROM dbo.NGUYEN_LIEU WHERE Ma_Nguyen_Lieu = @MaterialCode;
                IF @NewMaterialId IS NULL
                BEGIN
                    INSERT INTO dbo.NGUYEN_LIEU (Ma_Nguyen_Lieu, Ten_Nguyen_Lieu, Don_Vi_Tinh, Gia_Nhap)
                    VALUES (@MaterialCode, @MaterialName, @Unit, ISNULL(@UnitPrice, 0));
                    SET @NewMaterialId = SCOPE_IDENTITY();
                END
            END
            ELSE
            BEGIN
                SET @NewMaterialId = @MaterialId;
            END

            -- Thêm vào danh mục NCC (không báo lỗi nếu đã tồn tại)
            IF NOT EXISTS (
                SELECT 1 FROM dbo.NguyenLieu_NCC
                WHERE id_Nha_Cung_Cap = @SupplierId AND id_Nguyen_Lieu = @NewMaterialId
            )
            BEGIN
                INSERT INTO dbo.NguyenLieu_NCC (id_Nha_Cung_Cap, id_Nguyen_Lieu, Don_Gia_Mac_Dinh)
                VALUES (@SupplierId, @NewMaterialId,
                        CASE WHEN @UnitPrice IS NOT NULL AND @UnitPrice > 0
                             THEN CAST(@UnitPrice AS DECIMAL(18,2)) ELSE 0 END);
            END
        END

        FETCH NEXT FROM mat_cur INTO @MaterialId, @MaterialCode, @MaterialName, @Unit, @UnitPrice;
    END

    CLOSE mat_cur;
    DEALLOCATE mat_cur;
END
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- PATCH: sp_GetDeliveryNoteById — chi tiết PGH kèm giá chuẩn từ mức báo giá
-- (Tong_Gia_Bao_Khach / So_Luong) để lập chứng từ bán không phụ thuộc Don_Gia
-- lưu sai trên CHI_TIET_PGH.
-- ═══════════════════════════════════════════════════════════════════════════
CREATE OR ALTER PROCEDURE dbo.sp_GetDeliveryNoteById
    @PhieuGiaoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pgh.*,
        bg.Ma_Bao_Gia
    FROM dbo.PHIEU_GIAO_HANG pgh
    LEFT JOIN dbo.BAO_GIA bg ON bg.id = pgh.id_Bao_Gia
    WHERE pgh.id = @PhieuGiaoId;

    SELECT
        ct.*,
        ISNULL(ctbg.So_Luong, 0) AS So_Luong_Muc_Bao_Gia,
        ISNULL(ctbg.Gia_Bao_Khach, 0) AS Don_Gia_Bao_Gia,
        ISNULL(ctbg.Tong_Gia_Bao_Khach, 0) AS Tong_Bao_Gia_Muc
    FROM dbo.CHI_TIET_PGH ct
    LEFT JOIN dbo.CHI_TIET_BAO_GIA ctbg ON ctbg.id = ct.id_Chi_Tiet_Bao_Gia
    WHERE ct.id_Phieu_Giao_Hang = @PhieuGiaoId;
END
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- PATCH: Nhập kho — đơn đã nhập đủ SL không còn trong combo; ghi sổ xong cập nhật
-- Trang_Thai đơn = Hoàn thành; chi tiết đơn trả thêm So_Luong_Da_Nhan (SL còn lại).
-- ═══════════════════════════════════════════════════════════════════════════
CREATE OR ALTER PROCEDURE dbo.sp_GetPendingOrders
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.id, d.Ma_Don_Hang, n.Ten_NCC, n.id AS SupplierId, n.Dia_Chi AS SupplierAddress
    FROM dbo.DON_DAT_HANG_NCC d
    INNER JOIN dbo.NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
    WHERE d.Trang_Thai NOT IN (N'Hoàn thành', N'Hủy')
      AND EXISTS (
          SELECT 1
          FROM dbo.CHI_TIET_DON_HANG_NCC ct
          WHERE ct.id_Don_Dat_Hang = d.id
            AND ISNULL(ct.So_Luong_Da_Nhan, 0) < ct.So_Luong
      )
    ORDER BY d.Ngay_Dat_Hang DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetOrderReceiveData
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.Dieu_Khoan_Thanh_Toan, n.Ten_NCC, n.Dia_Chi, n.id AS SupplierId, n.Ma_NCC
    FROM dbo.DON_DAT_HANG_NCC d
    INNER JOIN dbo.NHA_CUNG_CAP n ON d.id_Nha_Cung_Cap = n.id
    WHERE d.id = @OrderId;

    SELECT
        ct.id,
        nl.Ma_Nguyen_Lieu,
        nl.Ten_Nguyen_Lieu,
        nl.Don_Vi_Tinh,
        ct.So_Luong AS OrderQty,
        ISNULL(ct.So_Luong_Da_Nhan, 0) AS So_Luong_Da_Nhan,
        ct.Don_Gia,
        ct.Thanh_Tien,
        ct.Phan_Tram_Thue_GTGT,
        ct.Tien_Thue_GTGT,
        nl.id AS MaterialId
    FROM dbo.CHI_TIET_DON_HANG_NCC ct
    INNER JOIN dbo.NGUYEN_LIEU nl ON ct.id_Nguyen_Lieu = nl.id
    WHERE ct.id_Don_Dat_Hang = @OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_PostInventoryReceipt
    @ReceiptId INT,
    @SupplierId INT,
    @Details XML
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.PHIEU_NHAP_KHO WHERE id = @ReceiptId AND Trang_Thai = N'Đã ghi sổ')
    BEGIN
        RAISERROR(N'Phiếu nhập kho này đã được ghi sổ. Không thể ghi sổ lại.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @totalAmount FLOAT = 0;
        DECLARE @materialId INT, @detailId INT;
        DECLARE @receivedQty FLOAT, @unitPrice FLOAT, @vatAmount FLOAT, @lineTotal FLOAT;
        DECLARE @OrderIdForComplete INT;

        SELECT @OrderIdForComplete = id_Don_Dat_Hang FROM dbo.PHIEU_NHAP_KHO WHERE id = @ReceiptId;

        DECLARE item_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.value('(MaterialId)[1]', 'INT'), d.value('(DetailId)[1]', 'INT'),
               d.value('(ReceivedQty)[1]', 'FLOAT'), d.value('(UnitPrice)[1]', 'FLOAT'),
               d.value('(VatAmount)[1]', 'FLOAT'), d.value('(LineTotal)[1]', 'FLOAT')
        FROM @Details.nodes('/Details/Item') AS T(d);

        OPEN item_cursor;
        FETCH NEXT FROM item_cursor INTO @materialId, @detailId, @receivedQty, @unitPrice, @vatAmount, @lineTotal;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @materialId > 0 AND @receivedQty > 0
            BEGIN
                INSERT INTO dbo.CHI_TIET_PHIEU_NHAP (id_Phieu_Nhap, id_Nguyen_Lieu, So_Luong_Nhap, Don_Gia_Nhap, Thanh_Tien)
                VALUES (@ReceiptId, @materialId, @receivedQty, @unitPrice, @lineTotal);
                UPDATE dbo.NGUYEN_LIEU SET Ton_Kho = ISNULL(Ton_Kho, 0) + @receivedQty, Gia_Nhap = @unitPrice WHERE id = @materialId;
            END
            IF @detailId > 0 AND @receivedQty > 0
                UPDATE dbo.CHI_TIET_DON_HANG_NCC
                SET So_Luong_Da_Nhan = ISNULL(So_Luong_Da_Nhan, 0) + @receivedQty
                WHERE id = @detailId;
            SET @totalAmount = @totalAmount + @lineTotal + @vatAmount;
            FETCH NEXT FROM item_cursor INTO @materialId, @detailId, @receivedQty, @unitPrice, @vatAmount, @lineTotal;
        END
        CLOSE item_cursor;
        DEALLOCATE item_cursor;

        IF @OrderIdForComplete IS NOT NULL AND @OrderIdForComplete > 0
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.CHI_TIET_DON_HANG_NCC
                WHERE id_Don_Dat_Hang = @OrderIdForComplete
                  AND ISNULL(So_Luong_Da_Nhan, 0) < So_Luong
            )
            AND EXISTS (SELECT 1 FROM dbo.CHI_TIET_DON_HANG_NCC WHERE id_Don_Dat_Hang = @OrderIdForComplete)
            BEGIN
                UPDATE dbo.DON_DAT_HANG_NCC
                SET Trang_Thai = N'Hoàn thành'
                WHERE id = @OrderIdForComplete;
            END
        END

        IF EXISTS (SELECT 1 FROM dbo.CONG_NO_NCC WHERE id_Nha_Cung_Cap = @SupplierId)
            UPDATE dbo.CONG_NO_NCC SET Tong_No = Tong_No + @totalAmount, Ngay_Cap_Nhat = GETDATE() WHERE id_Nha_Cung_Cap = @SupplierId;
        ELSE
            INSERT INTO dbo.CONG_NO_NCC (id_Nha_Cung_Cap, Tong_No, Da_Tra, Ngay_Cap_Nhat) VALUES (@SupplierId, @totalAmount, 0, GETDATE());

        UPDATE dbo.PHIEU_NHAP_KHO SET Trang_Thai = N'Đã ghi sổ' WHERE id = @ReceiptId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @errPostInv NVARCHAR(500) = ERROR_MESSAGE();
        RAISERROR(@errPostInv, 16, 1);
    END CATCH
END
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- PATCH: Combo «Mã PGH» — chỉ hiện phiếu còn làm việc (Mới lập / Đang giao…),
-- ẩn Đã giao và Hủy (đúng nghiệp vụ: không chọn lại phiếu đã kết thúc).
-- ═══════════════════════════════════════════════════════════════════════════
CREATE OR ALTER PROCEDURE dbo.sp_GetSavedDeliveryCodes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        Ma_Phieu_Giao,
        Ngay_Giao,
        Trang_Thai,
        Ten_KH
    FROM dbo.PHIEU_GIAO_HANG
    WHERE ISNULL(Trang_Thai, N'') NOT IN (N'Đã giao', N'Hủy')
    ORDER BY Ngay_Tao DESC;
END
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- PATCH: Kiểm tra đơn mua đã có phiếu nhập kho (không cho UPDATE chi tiết)
-- ═══════════════════════════════════════════════════════════════════════════
CREATE OR ALTER PROCEDURE dbo.sp_PurchaseOrderHasReceipt
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @OrderId IS NULL OR @OrderId <= 0
        SELECT CAST(0 AS BIT) AS HasReceipt;
    ELSE
        SELECT CAST(
            CASE WHEN EXISTS (
                SELECT 1 FROM dbo.PHIEU_NHAP_KHO WHERE id_Don_Dat_Hang = @OrderId
            ) THEN 1 ELSE 0 END AS BIT) AS HasReceipt;
END
GO

PRINT N'PHIEU_GIAO_HANG & Stored Procedures da tao thanh cong!';
GO

PRINT N'Schema da chay thanh cong! Tiep theo chay SeedData.sql (xoa @test.com) hoac SeedTestUsers.sql (tao lai TK test).';
GO

-- ╔══════════════════════════════════════════════════════════════════════╗
-- ║  SeedData (gan cuoi MainSQL): khong chen them 6 user @test.com        ║
-- ║  (trung vai tro voi 6 user @company.vn). De co TK test: SeedTestUsers ║
-- ╚══════════════════════════════════════════════════════════════════════╝

USE PrintingManagement;
GO

PRINT N'Dang xoa tai khoan @test.com (neu co)...';
GO

DELETE FROM USERS WHERE Email LIKE N'%@test.com';
GO

PRINT N'Xong. User mau: 6 tai @company.vn. Can test @test.com: chay Setup/SeedTestUsers.sql';
GO
