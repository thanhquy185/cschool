using System.Data;

namespace Services;

public class TuitionService
{
    private readonly DBService _db;

    public TuitionService(DBService db)
    {
        _db = db;
    }

    public List<StudentModel> GetStudents()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM students WHERE status = 1");
        var list = new List<StudentModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new StudentModel
            {
                Id = (int)row["id"],
                Fullname = row["fullname"].ToString()!,
                Avatar = row["avatar"].ToString()!,
                BirthDay = row["birthday"].ToString()!,
                Gender = row["gender"].ToString()!,
                Ethnicity = row["ethnicity"].ToString()!,
                Religion = row["religion"].ToString()!,
                Phone = row["phone"].ToString()!,
                Email = row["email"].ToString()!,
                Address = row["address"].ToString()!,
                LearnYear = row["learn_year"].ToString()!,
                LearnStatus = row["learn_status"].ToString()!,
                Status = (sbyte)row["status"]
            });
        }

        return list;
    }

    public List<StudentModel> GetAllStudents()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM students");
        var list = new List<StudentModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new StudentModel
            {
                Id = (int)row["id"],
                Fullname = row["fullname"].ToString()!,
                Avatar = row["avatar"].ToString()!,
                BirthDay = row["birthday"].ToString()!,
                Gender = row["gender"].ToString()!,
                Ethnicity = row["ethnicity"].ToString()!,
                Religion = row["religion"].ToString()!,
                Phone = row["phone"].ToString()!,
                Email = row["email"].ToString()!,
                Address = row["address"].ToString()!,
                LearnYear = row["learn_year"].ToString()!,
                LearnStatus = row["learn_status"].ToString()!,
                Status = (sbyte)row["status"]
            });
        }

        return list;
    }

    public StudentModel? GetStudentById(int id)
    {
        string sql = @$"SELECT students.id, students.fullname, students.avatar, students.birthday, students.gender, students.ethnicity,
                    students.religion, students.phone, students.email, students.address, students.learn_year, students.learn_status, 
                    students.status, classes.name AS class_name, teachers.fullname AS teacher_name
                    FROM students
                    JOIN assign_class_students ON students.id = assign_class_students.student_id
                    JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                    JOIN terms ON assign_classes.term_id = terms.id
                    JOIN classes ON assign_classes.class_id = classes.id
                    JOIN teachers ON assign_classes.head_teacher_id = teachers.id
                    WHERE students.id = {id}";
        var dt = _db.ExecuteQuery(sql);

        if (dt.Rows.Count == 0)
            return null;

        var row = dt.Rows[0];

        return new StudentModel
        {
            Id = (int)row["id"],
            Fullname = row["fullname"].ToString()!,
            Avatar = row["avatar"].ToString()!,
            BirthDay = row["birthday"].ToString()!,
            Gender = row["gender"].ToString()!,
            Ethnicity = row["ethnicity"].ToString()!,
            Religion = row["religion"].ToString()!,
            Phone = row["phone"].ToString()!,
            Email = row["email"].ToString()!,
            Address = row["address"].ToString()!,
            LearnYear = row["learn_year"].ToString()!,
            LearnStatus = row["learn_status"].ToString()!,
            Status = (sbyte)row["status"],
            ClassName = row["class_name"].ToString()!,
            TeacherName = row["teacher_name"].ToString()!
        };
    }

    public int GetIdLastStudent()
    {
        // Console.WriteLine(123);
        var dt = _db.ExecuteQuery("SELECT id FROM students ORDER BY id DESC LIMIT 1");
        if (dt.Rows.Count > 0)
            return System.Convert.ToInt32(dt.Rows[0]["id"]);
        return 0;
    }

    // Thêm học sinh mới
    public bool CreateStudent(StudentModel student)
    {
        string formattedBirthDay = "NULL";
        if (!string.IsNullOrWhiteSpace(student.BirthDay) && DateTime.TryParse(student.BirthDay, out var birth))
        {
            formattedBirthDay = $"'{birth:yyyy-MM-dd}'";
        }
        string sql = @$"
            INSERT INTO students 
            (fullname, avatar, birthday, gender, ethnicity, religion, address, phone, email, learn_year, learn_status)
            VALUES 
            ('{student.Fullname}', '{student.Avatar}', {formattedBirthDay}, 
                '{student.Gender}', '{student.Ethnicity}', '{student.Religion}', 
                '{student.Address}', '{student.Phone}', '{student.Email}', 
                '{student.LearnYear}', '{student.LearnStatus}');";

        int rows = _db.ExecuteNonQuery(sql);
        return rows > 0;
    }

    // Cập nhật thông tin học sinh
    public bool UpdateStudent(StudentModel student)
    {
        string formattedBirthDay = "NULL";
        if (!string.IsNullOrWhiteSpace(student.BirthDay) && DateTime.TryParse(student.BirthDay, out var birth))
        {
            formattedBirthDay = $"'{birth:yyyy-MM-dd}'";
        }
        string sql = @$"
            UPDATE students SET 
                fullname = '{student.Fullname}',
                avatar = '{student.Avatar}',
                birthday = {formattedBirthDay},
                gender = '{student.Gender}',
                ethnicity = '{student.Ethnicity}',
                religion = '{student.Religion}',
                phone = '{student.Phone}',
                email = '{student.Email}',
                address = '{student.Address}',
                learn_year = '{student.LearnYear}',
                learn_status = '{student.LearnStatus}'
            WHERE id = {student.Id};";

        int rows = _db.ExecuteNonQuery(sql);
        return rows > 0;
    }

    // Khóa học sinh
    public bool LockStudent(StudentModel student)
    {
        string sql = @$"
            UPDATE students 
            SET status = 0
            WHERE id = {student.Id};";

        int rows = _db.ExecuteNonQuery(sql);
        return rows > 0;
    }

}
