using System.Data;
using Models;

namespace Services
{
    public class ClassService
    {
        private readonly DBService _db;

        public ClassService(DBService db)
        {
            _db = db;
        }

        // Lấy danh sách lớp
        public List<ClassModel> GetClasses()
        {
            var dt = _db.ExecuteQuery(@"
                SELECT 
                    c.id,
                    c.class_type_id,
                    ct.name AS class_type_name,
                    c.grade,
                    c.name,
                    c.area,
                    c.room,
                    c.status,
                    t.name AS term,
                    t.learnyear,
                    a.id AS assign_class_id,
                    tea.id AS head_teacher
                FROM cschool.classes AS c
                JOIN cschool.assign_classes AS a ON c.id = a.class_id
                JOIN cschool.terms AS t ON a.term_id = t.id
                JOIN cschool.teachers AS tea ON a.head_teacher_id = tea.id
                JOIN cschool.class_types AS ct ON c.class_type_id = ct.id
            ");

            var list = new List<ClassModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ClassModel
                {
                    Id = (int)row["id"],
                    ClassTypeId = (int)row["class_type_id"],
                    AssignClassId= (int)row["assign_class_id"],
                    Grade = (int)row["grade"],
                    Name = row["name"].ToString()!,
                    Area = row["area"].ToString()!,
                    Room = row["room"].ToString()!,
                    Status = Convert.ToInt16(row["status"]),
                    Term = row["term"].ToString()!,
                    Year = row["learnyear"].ToString()!,
                    HeadTeacher = row["head_teacher"].ToString()!,
                    ClassTypeName = row["class_type_name"].ToString()!,
                });
            }
            return list;
        }

        // Lấy học sinh theo lớp & học kỳ
        public List<StudentModel> GetStudentsByClassId(int classId, int term)
        {
            var list = new List<StudentModel>();
            string termName = $"Học kỳ {term}";

            var dt = _db.ExecuteQuery($@"
                SELECT
                    s.id, s.fullname, s.avatar, s.birthday, s.gender, s.learn_year, s.learn_status, acs.role
                FROM cschool.students s
                JOIN cschool.assign_class_students acs ON s.id = acs.student_id
                JOIN cschool.assign_classes ac ON acs.assign_class_id = ac.id
                JOIN cschool.terms t ON ac.term_id = t.id
                WHERE ac.class_id = {classId} AND t.name = '{termName}'
            ");

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new StudentModel
                {
                    Id = (int)row["id"],
                    Fullname = row["fullname"].ToString()!,
                    Avatar = row["avatar"].ToString()!,
                    BirthDay = row["birthday"].ToString()!,
                    Role = row["role"].ToString()!,
                    Gender = row["gender"].ToString()!,
                    LearnYear = row["learn_year"].ToString()!,
                    LearnStatus = row["learn_status"].ToString()!,
                });
            }
            return list;
        }

        // Lấy giáo viên theo lớp + học kỳ
        public TeacherModel? GetTeacherByClassAndTerm(int classId, int termId)
        {
            var dt = _db.ExecuteQuery($@"
                SELECT
                    t.id, t.fullname, t.birthday, t.gender, t.address, t.phone, t.email
                FROM cschool.teachers t
                JOIN cschool.assign_classes ac ON ac.head_teacher_id = t.id
                WHERE ac.class_id = {classId} AND ac.term_id = {termId}
            ");

            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];

            return new TeacherModel
            {
                Id = (int)row["id"],
                Name = row["fullname"].ToString()!,
                Birthday = row["birthday"].ToString()!,
                Gender = row["gender"].ToString()!,
                Address = row["address"].ToString()!,
                Phone = row["phone"].ToString()!,
                Email = row["email"].ToString()!,
                DepartmentName = ""
            };
        }

        // Lấy map term theo class
        public Dictionary<string, int> GetTermMapByClass(int classId)
        {
            var dict = new Dictionary<string, int>();
            var dt = _db.ExecuteQuery($@"
                SELECT tm.name, tm.id
                FROM cschool.assign_classes ac
                JOIN cschool.terms tm ON tm.id = ac.term_id
                WHERE ac.class_id = {classId}
            ");

            foreach (DataRow row in dt.Rows)
                dict[row["name"].ToString()!] = (int)row["id"];

            return dict;
        }

        // Lấy danh sách loại lớp
        public List<ClassTypeModel> GetClasstype()
        {
            var list = new List<ClassTypeModel>();
            var dt = _db.ExecuteQuery(@"
                SELECT id, name
                FROM cschool.class_types
                WHERE status=1
            ");

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ClassTypeModel
                {
                    Id = (int)row["id"],
                    Name = row["name"].ToString()!
                });
            }
            return list;
        }

        // Lấy hoặc tạo term
        public List<TermClassModel> GetOrCreateTerm(string year)
        {
            System.Console.WriteLine("Có vào GetOrCreateTerm không");
            System.Console.WriteLine(year);
            var terms = new List<TermClassModel>();
            var dt = _db.ExecuteQuery($@"SELECT * FROM cschool.terms WHERE learnyear = '{year}'");

            foreach (DataRow row in dt.Rows)
            {
                terms.Add(new TermClassModel
                {
                    Id = (int)row["id"],
                    TermName = row["name"].ToString()!,
                    Year = row["learnyear"].ToString()!,
                    StartDate = row["start_date"].ToString()!,
                    EndDate = row["end_date"].ToString()!,
                    Status = Convert.ToInt16(row["status"])
                });
            }

            if (terms.Count == 0)
            {
                var startYear = int.Parse(year.Split('-')[0]);
                var endYear = int.Parse(year.Split('-')[1]);

                var hk1Start = new DateTime(startYear, 9, 1);
                var hk1End = new DateTime(endYear, 1, 31);
                var hk2Start = new DateTime(endYear, 2, 1);
                var hk2End = new DateTime(endYear, 6, 30);

                _db.ExecuteNonQuery($@"
                    INSERT INTO cschool.terms(name, year, learnyear, start_date, end_date, status)
                    VALUES('Học kỳ 1','{startYear}', '{year}', '{hk1Start:yyyy-MM-dd}', '{hk1End:yyyy-MM-dd}', 1)
                ");

                _db.ExecuteNonQuery($@"
                    INSERT INTO cschool.terms(name, year, learnyear, start_date, end_date, status)
                    VALUES('Học kỳ 2', '{startYear}', '{year}', '{hk2Start:yyyy-MM-dd}', '{hk2End:yyyy-MM-dd}', 1)
                ");

                dt = _db.ExecuteQuery($@"SELECT * FROM cschool.terms WHERE learnyear = '{year}'");
                terms.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    terms.Add(new TermClassModel
                    {
                        Id = (int)row["id"],
                        TermName = row["name"].ToString()!,
                        Year = row["learnyear"].ToString()!,
                        StartDate = row["start_date"].ToString()!,
                        EndDate = row["end_date"].ToString()!,
                        Status = Convert.ToInt16(row["status"])
                    });
                }
            }




            return terms;
        }

        // Lấy danh sách học sinh chưa được phân lớp
     public List<StudentModel> GetUnassignedStudents(string currentYear, int currentGrade)
{
    var result = new List<StudentModel>();
    var parts = currentYear.Split('-');
    var prevStart = int.Parse(parts[0]) - 1;
    var prevEnd = int.Parse(parts[1]) - 1;
    string previousYear = $"{prevStart}-{prevEnd}";

    string sql = $@"
        SELECT DISTINCT
            s.id, s.fullname, s.gender, s.birthday, s.learn_status
        FROM students s
        LEFT JOIN term_gpa tq ON tq.student_id = s.id
        LEFT JOIN assign_classes ac ON ac.id = tq.assign_class_id
        LEFT JOIN classes c ON c.id = ac.class_id
        LEFT JOIN terms t ON t.id = ac.term_id
        WHERE 
            (
                -- CASE 1: CÓ ĐIỂM NĂM TRƯỚC
                (
                    t.learnyear = '{previousYear}'
                    AND (
                        -- LÊN LỚP
                        (tq.academic IN ('Giỏi','Khá','Trung bình') 
                         AND tq.gpa >= 5.0 
                         AND c.grade + 1 = {currentGrade})

                        OR

                        -- Ở LẠI LỚP
                        ((tq.academic = 'Yếu' OR tq.gpa < 5.0)
                         AND c.grade = {currentGrade})
                    )
                )

                OR

                -- CASE 2: HỌC SINH MỚI – không có GPA năm trước
                NOT EXISTS (
                    SELECT 1 FROM term_gpa tq2
                    JOIN assign_classes ac2 ON ac2.id = tq2.assign_class_id
                    JOIN terms t2 ON t2.id = ac2.term_id
                    WHERE tq2.student_id = s.id
                    AND t2.learnyear = '{previousYear}'
                )
            )
        AND s.id NOT IN (
            SELECT acs.student_id
            FROM assign_class_students acs
            JOIN assign_classes ac3 ON ac3.id = acs.assign_class_id
            JOIN terms t3 ON t3.id = ac3.term_id
            WHERE t3.learnyear = '{currentYear}'
        )
        AND s.learn_status NOT IN ('Nghỉ học', 'Bảo lưu');
    ";

    var dt = _db.ExecuteQuery(sql);
    foreach (DataRow row in dt.Rows)
    {
        result.Add(new StudentModel
        {
            Id = (int)row["id"],
            Fullname = row["fullname"].ToString()!,
            Gender = row["gender"].ToString()!,
            BirthDay = row["birthday"].ToString()!,
            LearnStatus = row["learn_status"].ToString()!,
        });
    }

    return result;
}

        // Lưu lớp mới và gán giáo viên
       public async Task<int> SaveClassAsync(ClassModel cls, string year)
{
    var terms = GetOrCreateTerm(year);

    if (cls.Id == 0)
    {
        // Gộp INSERT + SELECT LAST_INSERT_ID()
        var sql = $@"
            INSERT INTO classes (name, class_type_id, grade, area, room, status)
            VALUES('{cls.Name}', {cls.ClassTypeId}, {cls.Grade}, '{cls.Area}', '{cls.Room}', 1);
            SELECT LAST_INSERT_ID();
        ";

        // ExecuteScalar sẽ trả về ID vừa insert
        cls.Id = Convert.ToInt32(_db.ExecuteScalar(sql));
        Console.WriteLine($"Inserted Class Id: {cls.Id}");
    }
    else
    {
        _db.ExecuteNonQuery($@"
            UPDATE classes 
            SET name='{cls.Name}', class_type_id={cls.ClassTypeId}, grade={cls.Grade}, area='{cls.Area}', room='{cls.Room}'
            WHERE id={cls.Id}
        ");
    }

    foreach (var term in terms)
    {
        int teacherId = 0;
        if (term.TermName == "Học kỳ 1" && cls.TeacherHK1 != null) teacherId = cls.TeacherHK1.Id;
        if (term.TermName == "Học kỳ 2" && cls.TeacherHK2 != null) teacherId = cls.TeacherHK2.Id;

        var exists = _db.ExecuteScalar($@"
            SELECT COUNT(*) FROM assign_classes
            WHERE class_id={cls.Id} AND term_id={term.Id}
        ");
        if (Convert.ToInt32(exists) == 0)
        {
            _db.ExecuteNonQuery($@"
                INSERT INTO assign_classes (class_id, term_id, head_teacher_id)
                VALUES({cls.Id}, {term.Id}, {teacherId})
            ");
        }
        else
        {
            _db.ExecuteNonQuery($@"
                UPDATE assign_classes
                SET head_teacher_id={teacherId}
                WHERE class_id={cls.Id} AND term_id={term.Id}
            ");
        }
    }

    return cls.Id;
}

        // Gán học sinh vào lớp theo học kỳ
        public async Task AssignStudentsToClassAsync(int classId, int termNumber, string year, List<StudentModel> students)
        {
            var term = GetOrCreateTerm(year).FirstOrDefault(t => t.TermName == $"Học kỳ {termNumber}");
            if (term == null)
            {
                Console.WriteLine("[AssignStudents] Không tìm thấy học kỳ!");
                return;
            }

            var acIdObj = _db.ExecuteScalar($@"
                SELECT id FROM assign_classes
                WHERE class_id={classId} AND term_id={term.Id}
            ");
            if (acIdObj == null)
            {
                Console.WriteLine("[AssignStudents] Không tìm thấy assign_class!");
                return;
            }

            int assignClassId = Convert.ToInt32(acIdObj);

            Console.WriteLine($"\n========== [ASSIGN STUDENTS HK{termNumber}] ==========");
            Console.WriteLine($"ClassId = {classId}, TermId = {term.Id}, AssignClassId = {assignClassId}");

            // Lấy danh sách học sinh hiện tại trong DB
            var dt = _db.ExecuteQuery($@"
                SELECT student_id 
                FROM assign_class_students
                WHERE assign_class_id={assignClassId}
            ");

            var currentStudentIds = dt.Rows.Cast<DataRow>()
                                        .Select(r => Convert.ToInt32(r["student_id"]))
                                        .ToList();

            Console.WriteLine("Current students in DB: " + string.Join(", ", currentStudentIds));

            var newStudentIds = students.Select(s => s.Id).ToList();
            Console.WriteLine("New students in UI: " + string.Join(", ", newStudentIds));

            // Học sinh cần thêm
            var toAdd = newStudentIds.Except(currentStudentIds).ToList();
            Console.WriteLine("To Add: " + string.Join(", ", toAdd));

            // Học sinh cần xóa
            var toRemove = currentStudentIds.Except(newStudentIds).ToList();
            Console.WriteLine("To Remove: " + string.Join(", ", toRemove));

            // Thêm mới
            foreach (var id in toAdd)
            {
                _db.ExecuteNonQuery($@"
                    INSERT INTO assign_class_students (assign_class_id, student_id, role)
                    VALUES ({assignClassId}, {id}, 'Student')
                ");
               
                AppService.TuitionService.AddTuitionMonthlyForStudent(id,assignClassId);
                Console.WriteLine($"Added student: {id}");
            }

            // Xóa học sinh bị gỡ
            foreach (var id in toRemove)
            {
                _db.ExecuteNonQuery($@"
                    DELETE FROM assign_class_students
                    WHERE assign_class_id={assignClassId} AND student_id={id}
                ");
                Console.WriteLine($"Removed student: {id}");
            }

            

            Console.WriteLine($"====== DONE HK{termNumber}: +{toAdd.Count}, -{toRemove.Count} ======\n");
        }

        public (bool success, string message) DeleteClass(int classId)
        {
            // 1. Kiểm tra lớp có học sinh không
            var countObj = _db.ExecuteScalar($@"
                SELECT COUNT(*) 
                FROM assign_class_students acs
                JOIN assign_classes ac ON acs.assign_class_id = ac.id
                WHERE ac.class_id = {classId}
            ");

            int count = Convert.ToInt32(countObj);

            if (count > 0)
            {
                return (false, "Lớp này đã có học sinh, không thể xóa!");
            }

            // 2. Xóa mềm
            int rows = _db.ExecuteNonQuery($@"
                UPDATE classes 
                SET status = 0 
                WHERE id = {classId}
            ");

            if (rows > 0)
                return (true, "Xóa lớp thành công!");

            return (false, "Không thể xóa lớp! Vui lòng thử lại.");
        }


        public TermClassModel? GetTermById(int termId)
        {
            var dt = _db.ExecuteQuery($@"
                SELECT * 
                FROM cschool.terms
                WHERE id = {termId}
            ");

            if (dt.Rows.Count == 0)
                return null; // Không tìm thấy term

            var row = dt.Rows[0];
            return new TermClassModel
            {
                Id = (int)row["id"],
                TermName = row["name"].ToString()!,
                Year = row["learnyear"].ToString()!,
                StartDate = row["start_date"].ToString()!,
                EndDate = row["end_date"].ToString()!,
                Status = Convert.ToInt16(row["status"])
            };
        }
        // Thêm vào ClassService
        public AssignClassModel? GetAssignClassById(int assignClassId)
        {
            var dt = _db.ExecuteQuery($@"
                SELECT 
                    ac.id AS assign_class_id,
                    ac.class_id,
                    ac.term_id,
                    ac.head_teacher_id,
                    c.name AS class_name,
                    c.grade,
                    t.name AS term_name,
                    t.learnyear AS term_year
                FROM cschool.assign_classes ac
                JOIN cschool.classes c ON ac.class_id = c.id
                JOIN cschool.terms t ON ac.term_id = t.id
                WHERE ac.id = {assignClassId}
            ");

            if (dt.Rows.Count == 0)
                return null; // Không tìm thấy

            var row = dt.Rows[0];

            return new AssignClassModel
            {
                Id = (int)row["assign_class_id"],
                ClassId = (int)row["class_id"],
                TermId = (int)row["term_id"],
                HeadTeacherId = row["head_teacher_id"] != DBNull.Value ? (int)row["head_teacher_id"] : 0,
                ClassName = row["class_name"].ToString()!,
                Grade = row["grade"] != DBNull.Value ? Convert.ToInt32(row["grade"]) : 0,
                TermName = row["term_name"].ToString()!,
                Year = row["term_year"].ToString()!
            };
        }
    }
}
