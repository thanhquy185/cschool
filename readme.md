# CSchool - Phần mềm Quản Lý Học Sinh

## Giới thiệu

Đây là một ứng dụng desktop được phát triển bằng **C#** với **Avalonia** và theo kiến trúc **MVVM (Model-View-ViewModel)**. Ứng dụng hỗ trợ **quản lý học sinh**, với các chức năng chính như quản lý thông tin học sinh, lớp, điểm số, và các báo cáo liên quan, với giao diện trực quan, dễ sử dụng và dễ mở rộng.

## Thành viên nhóm

| Thành viên       | MSSV       |
| ---------------- | ---------- |
| Lê Thị Trúc Ly   | 3123410210 |
| Lê Đoàn Kim Ngân | 3123410231 |
| Lâm Tú Nhi       | 3123410250 |
| Trương Văn Thiện | 3123410356 |
| Trần Thanh Quy   | 3123560072 |

## Công nghệ sử dụng

- **Ngôn ngữ**: C#
- **Framework GUI**: Avalonia
- **Kiến trúc**: MVVM
- **Thư viện chính**:
  - ReactiveUI
  - CommunityToolkit.Mvvm
  - Nhiều thư viện khác...

## Cấu trúc dự án

Dự án được tổ chức theo mô hình **MVVM** như sau:

- **/Assets:** Chứa hình ảnh, icon, media, các tài nguyên khác
- **/Styles:** Chứa các style, theme cho giao diện
- **/Common:** Chứa các lớp/chức năng dùng chung cho toàn dự án
- **/Utils:** Chứa các tiện ích, helper, extension
- **/Services:**
  - AppService: Lớp giúp triển khai các dịch vụ để sử dụng chung
  - DBService: Lớp giúp kết nối csdl (sử dụng MySQL)
  - UploadService: Lớp giúp lưu ảnh
  - Model-Services: Sử dụng DBService để thao tác với cơ sở dữ liệu theo từng model
- **/Views:** Chứa các giao diện XAML
- **/ViewModels:** Chứa các ViewModel xử lý logic, binding dữ liệu
- **/Models:** Chứa các lớp dữ liệu (Model)
- **App.xaml:**
  - Import Fluent theme của thư viện
  - Import các file style, màu và icon dùng chung
- **App.xaml.cs:**
  - Khởi tạo ứng dụng Avalonia
  - Cấu hình DBService và các service khác để sử dụng cho phần mềm

## Hướng dẫn sử dụng dự án

```
# 1. Clone repository về máy
git clone https://github.com/thanhquy185/cschool.git

# 2. Cài đặt .NET SDK (nếu chưa có)
Truy cập https://dotnet.microsoft.com/en-us/download để tải và cài đặt .NET SDK >= 9.0

# 3. Restore các package NuGet
dotnet restore

# 4. Build dự án
dotnet build

# 5. Chạy ứng dụng
dotnet run

# 6. Nếu muốn tự động reload khi code thay đổi
dotnet watch run
```
