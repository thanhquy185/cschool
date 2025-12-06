using System;

namespace cschool.Models
{
    public class FeeClassMonthModel
    {
        public int Id { get; set; }                  // Nếu dùng DB tự tăng
        public int AssignClassId { get; set; }       // Mã lớp
        public int FeeTemplateId { get; set; }       // Mã loại phí
        public int MonthId { get; set; }             // Tháng 1-12
        public int Term { get; set; }                // Học kỳ 1 hoặc 2
        public decimal Amount { get; set; }          // Số tiền áp dụng
        public String StartDate { get; set; }      // Ngày bắt đầu học kỳ
        public String EndDate { get; set; }        // Ngày kết thúc học kỳ
        public String CreatedAt { get; set; }      // Thời gian tạo
        public String UpdatedAt { get; set; }      // Thời gian cập nhật
    }
}
