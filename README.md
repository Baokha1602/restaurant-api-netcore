RESTAURANT MANAGEMENT SYSTEM

Dự án cung cấp giải pháp Backend toàn diện cho hệ thống quản lý nhà hàng hiện đại, tập trung vào việc tự động hóa quy trình phục vụ thông qua công nghệ quét mã QR, thanh toán điện tử và cập nhật trạng thái đơn hàng thời gian thực. Hệ thống giúp tối ưu hóa vận hành giữa các bộ phận Khách hàng - Bếp - Thu ngân và nâng cao trải nghiệm người dùng

1. CÔNG NGHỆ SỬ DỤNG
   
   - Framework: ASP.NET Core 7.0/8.0 (Web API)
   - Cơ sở dữ liệu: SQL Server với Entity Framework Core (Code First)
   - Real-time: SignalR (WebSockets) cho hệ thống thông báo tức thời giữa các bộ phận
   - Bảo mật: JWT (JSON Web Token) cho xác thực và phân quyền truy cập
   - Thanh toán: Tích hợp trực tiếp API của cổng thanh toán Momo và VNPAY
   - Kiến trúc: Repository & Service Pattern đảm bảo tính mở rộng và dễ bảo trì
     
2. CÁC CHỨC NĂNG CỐT LÕI
   
   - QR Ordering: Xử lý logic định danh bàn thông qua QR Code, cho phép khách hàng đặt món trực tiếp từ điện thoại cá nhân
   - Real-time Order Flow: Sử dụng OrderHub để đồng bộ và cập nhật trạng thái đơn hàng (đang chế biến, đã xong, đã thanh toán) ngay lập tức
   - Payment Integration: Xử lý thanh toán trực tuyến an toàn và tự động cập nhật trạng thái hóa đơn sau khi giao dịch thành công
   - Inventory Management: Quản lý xuất nhập tồn, định lượng nguyên liệu và kiểm soát trạng thái kho hàng theo thời gian thực
   - Revenue Reports: Hệ thống tự động tổng hợp dữ liệu và xuất báo cáo doanh thu chi tiết theo ngày/tháng
     
3. CẤU TRÚC THƯ MỤC DỰ ÁN
   
   - Controllers/Api: Định nghĩa các RESTful API endpoints phục vụ Mobile App và Web Front-end
   - HubSocket: Triển khai SignalR Hub phục vụ các kết nối dữ liệu thời gian thực
   - Services: Lớp xử lý logic nghiệp vụ chính như Payment, JWT, QR Image và Mail
   - Repositories: Tầng giao tiếp cơ sở dữ liệu, áp dụng Repository Pattern để tối ưu hóa truy vấn
   - DTO/ViewModels: Quản lý cấu trúc dữ liệu truyền tải giữa Client và Server
