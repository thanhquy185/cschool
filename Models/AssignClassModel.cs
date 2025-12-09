namespace Models
{
    public class AssignClassModel
    {
        // ID của bản assign_class
        public int Id { get; set; }

        // ID lớp gốc
        public int ClassId { get; set; }

        // ID học kỳ
        public int TermId { get; set; }

        // ID giáo viên chủ nhiệm
        public int HeadTeacherId { get; set; }

        // Tên lớp (lấy từ bảng classes)
        public string ClassName { get; set; } = string.Empty;

        // Khối lớp
        public int Grade { get; set; }

        // Tên học kỳ (lấy từ bảng terms)
        public string TermName { get; set; } = string.Empty;

        // Năm học
        public string Year { get; set; } = string.Empty;

        // Giáo viên chủ nhiệm (nếu cần)
        public string? HeadTeacherName { get; set; }
    }
}
