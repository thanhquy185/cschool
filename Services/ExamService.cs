using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace cschool.Services;

public class ExamService
{
    private readonly DBService _db;

    public ExamService(DBService db)
    {
        _db = db;
    }

    public List<ExamModel> GetExamSchedule()
    {
        string sql = @$"SELECT exam_details.id, subjects.name AS subject, terms.name AS term_name, terms.year AS term_year,
                        exam_details.start_time, exam_details.end_time,
                        GROUP_CONCAT(DISTINCT classes.grade ORDER BY classes.grade SEPARATOR ', ') AS grades
                        FROM exams
                        JOIN exam_details ON exams.exam_detail_id = exam_details.id
                        JOIN rooms ON exams.exam_room = rooms.id
                        JOIN student_exams ON exams.id = student_exams.exam_id
                        JOIN students ON student_exams.student_id = students.id
                        JOIN assign_class_students ON students.id = assign_class_students.student_id
                        JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                        JOIN classes ON assign_classes.class_id = classes.id
                        JOIN teachers ON exams.supervisor_id = teachers.id
                        JOIN subjects ON exam_details.subject_id = subjects.id
                        JOIN terms ON exam_details.term_id = terms.id
                        GROUP BY 
                            exam_details.id, subjects.name, terms.name, term_year,
                            exam_details.start_time, exam_details.end_time
                        ORDER BY exam_details.start_time ASC, subjects.name ASC";
        var dt = _db.ExecuteQuery(sql);
        var list = new List<ExamModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new ExamModel
            {
                Id = (int)row["id"],
                Subject = row["subject"].ToString()!,
                Grade = row["grades"].ToString()!,
                ExamDate = row["start_time"].ToString()!,
                StartTime = row["start_time"].ToString()!,
                EndTime = row["end_time"].ToString()!,
                TermName = row["term_name"].ToString()!,
                TermYear = row["term_year"].ToString()!,
            });
        }

        return list;
    }

    // Lấy chi tiết lịch thi
    public ExamModel? GetExamById(int id)
    {
        string sql = @$"SELECT exams.id, rooms.name AS exam_room, teachers.fullname, rooms.quantity AS candidate_count, 
                        subjects.name AS subject, terms.name AS term_name, terms.year AS term_year, exam_details.start_time, 
                        exam_details.end_time, classes.grade
                        FROM exams
                        JOIN exam_details ON exams.exam_detail_id = exam_details.id
                        JOIN rooms ON exams.exam_room = rooms.id
                        JOIN student_exams ON exams.id = student_exams.exam_id
                        JOIN students ON student_exams.student_id = students.id
                        JOIN assign_class_students ON students.id = assign_class_students.student_id
                        JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                        JOIN classes ON assign_classes.class_id = classes.id
                        JOIN teachers ON exams.supervisor_id = teachers.id
                        JOIN subjects ON exam_details.subject_id = subjects.id
                        JOIN terms ON exam_details.term_id = terms.id
                        WHERE exams.id = {id}";
        var dt = _db.ExecuteQuery(sql);

        if (dt.Rows.Count == 0)
            return null;

        var row = dt.Rows[0];

        return new ExamModel
        {
            Id = (int)row["id"],
            Subject = row["subject"].ToString()!,
            ExamDate = row["start_time"].ToString()!,
            TermName = row["term_name"].ToString()!,
            TermYear = row["term_year"].ToString()!,
            StartTime = row["start_time"].ToString()!,
            EndTime = row["end_time"].ToString()!,
            Grade = row["grade"].ToString()!,
        };
    }

    // Lấy danh sách học sinh mỗi phòng của chi tiết lịch thi
    public List<StudentExamModel> GetStudentExamById(int details_id, int room_id)
    {
        string sql = @$"SELECT students.id AS student_id, students.fullname AS student_name, classes.name AS class_name
                        FROM exams
                        JOIN exam_details ON exams.exam_detail_id = exam_details.id
                        JOIN rooms ON exams.exam_room = rooms.id
                        JOIN student_exams ON exams.id = student_exams.exam_id
                        JOIN students ON student_exams.student_id = students.id
                        JOIN assign_class_students ON students.id = assign_class_students.student_id
                        JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                        JOIN classes ON assign_classes.class_id = classes.id
                        WHERE exam_details.id = {details_id} 
                        AND rooms.id = {room_id}
                        ORDER BY students.id ASC";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<StudentExamModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new StudentExamModel
            {
                StudentId = (int)row["student_id"],
                StudentName = row["student_name"].ToString()!,
                ClassName = row["class_name"].ToString()!,
            });
        }

        return list;
    }

    // Lấy danh sách phòng thi của chi tiết lịch thi
    public List<RoomExamModel> GetRoomExamById(int id)
    {
        string sql = @$"SELECT rooms.id, rooms.name AS room_name, teachers.fullname, rooms.quantity AS candidate_count
                        FROM exams
                        JOIN exam_details ON exams.exam_detail_id = exam_details.id
                        JOIN rooms ON exams.exam_room = rooms.id
                        JOIN teachers ON exams.supervisor_id = teachers.id
                        WHERE exam_details.id = {id}
                        ORDER BY rooms.id ASC";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<RoomExamModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RoomExamModel
            {
                Id = (int)row["id"],
                RoomName = row["room_name"].ToString()!,
                TeacherName = row["fullname"].ToString()!,
                RoomQuantity = row["candidate_count"].ToString()!,
            });
        }

        return list;
    }

    // Đếm số lượng sinh viên của khối chưa được phân phòng
    public int GetStudentGrade(int grade)
    {
        string sql = @$"SELECT COUNT(students.id) AS StudentGrade
                    FROM students
                    JOIN assign_class_students ON students.id = assign_class_students.student_id
                    JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                    JOIN classes ON assign_classes.class_id = classes.id
                    WHERE classes.grade = {grade}
                    AND students.id NOT IN (SELECT student_id FROM student_exams);";
        var dt = _db.ExecuteQuery(sql);

        int count = 0;
        if (dt.Rows.Count > 0)
        {
            count = Convert.ToInt32(dt.Rows[0]["StudentGrade"]);
        }

        return count;
    }

    // Lấy danh sách môn học
    public List<SubjectModel> GetSubjectList()
    {
        string sql = $"SELECT * from cschool.subjects";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<SubjectModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SubjectModel
            {
                Id = (int)row["id"],
                SubjectName = row["name"].ToString()!,
            });
        }

        return list;
    }

    // Lấy ds học kỳ
    public List<TermModel> GetTermList()
    {
        string sql = @"SELECT id, CONCAT(name,' - ',year) AS study_term FROM terms";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<TermModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new TermModel
            {
                Id = (int)row["id"],
                TermName = row["study_term"].ToString()!,
            });
        }

        return list;
    }

    // Lấy danh sách phòng thi
    public List<RoomModel> GetRoomList()
    {
        string sql = $"SELECT * from cschool.rooms";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<RoomModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RoomModel
            {
                Id = (int)row["id"],
                RoomName = row["name"].ToString()!,
                Quantity = (int)row["quantity"],
            });
        }

        return list;
    }

    // Lấy danh sách giáo viên
    public List<TeacherModel> GetTeacherList()
    {
        string sql = $"SELECT * from cschool.teachers";
        var dt = _db.ExecuteQuery(sql);

        var list = new List<TeacherModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new TeacherModel
            {
                Id = (int)row["id"],
                TeacherName = row["fullname"].ToString()!,
            });
        }

        return list;
    }

    // Thêm lịch thi mới
    public bool CreateExam(ExamCreateModel exam)
    {
        try
        {
            // Thêm exam_details
            string sqlDetail = @$"
                INSERT INTO exam_details (subject_id, term_id, exam_type_id, start_time, end_time)
                VALUES (
                    {exam.SubjectId},
                    {exam.TermId},
                    NULL,
                    '{exam.StartTime}',
                    '{exam.EndTime}'
                );
                SELECT LAST_INSERT_ID();";

            int examDetailId = _db.ExecuteScalar<int>(sqlDetail);
            if (examDetailId <= 0)
                throw new Exception("Không lấy được ID ca thi.");

            // Lấy danh sách học sinh thuộc khối này
            string sqlStudents = @$"SELECT students.id
                                    FROM students
                                    JOIN assign_class_students ON students.id = assign_class_students.student_id
                                    JOIN assign_classes ON assign_class_students.assign_class_id = assign_classes.id
                                    JOIN classes ON assign_classes.class_id = classes.id
                                    WHERE classes.grade = {exam.GradeId}
                                    AND students.id NOT IN (SELECT student_id FROM student_exams);";
            var allStudents = _db.ExecuteQuery(sqlStudents)
                                .AsEnumerable()
                                .Select(r => Convert.ToInt32(r["id"]))
                                .ToList();

            if (allStudents.Count == 0)
                throw new Exception("Không có học sinh nào trong khối này.");

            // Shuffle ngẫu nhiên danh sách học sinh
            var rnd = new Random();
            allStudents = allStudents.OrderBy(x => rnd.Next()).ToList();

            int currentIndex = 0;

            // Thêm từng phòng thi và gán học sinh
            foreach (var a in exam.Assignments)
            {
                // Thêm bản ghi exam
                string sqlExam = @$"
                    INSERT INTO exams (exam_detail_id, exam_room, supervisor_id)
                    VALUES ({examDetailId}, {a.RoomId}, {a.TeacherId});
                    SELECT LAST_INSERT_ID();";

                int examId = _db.ExecuteScalar<int>(sqlExam);
                if (examId <= 0)
                    throw new Exception("Không lấy được ID phòng thi (exam_id).");

                // Lấy danh sách học sinh cho phòng này
                var assigned = allStudents.Skip(currentIndex).Take(a.AssignedStudents).ToList();
                currentIndex += a.AssignedStudents;

                // Thêm vào bảng student_exams
                foreach (var studentId in assigned)
                {
                    string sqlStudentExam = @$"
                        INSERT INTO student_exams (exam_id, student_id)
                        VALUES ({examId}, {studentId});";
                    _db.ExecuteNonQuery(sqlStudentExam);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi thêm lịch thi: {ex.Message}");
            return false;
        }
    }


    // // Cập nhật thông tin lịch thi
    // public bool UpdateStudent(ExamModel student)
    // {
    //     string formattedBirthDay = "NULL";
    //     if (!string.IsNullOrWhiteSpace(student.BirthDay) && DateTime.TryParse(student.BirthDay, out var birth))
    //     {
    //         formattedBirthDay = $"'{birth:yyyy-MM-dd}'";
    //     }
    //     string sql = @$"
    //         UPDATE cschool.students SET 
    //             fullname = '{student.Fullname}',
    //             avatar = '{student.Avatar}',
    //             birthday = {formattedBirthDay},
    //             gender = '{student.Gender}',
    //             ethnicity = '{student.Ethnicity}',
    //             religion = '{student.Religion}',
    //             phone = '{student.Phone}',
    //             email = '{student.Email}',
    //             address = '{student.Address}',
    //             learn_year = '{student.LearnYear}',
    //             learn_status = '{student.LearnStatus}'
    //         WHERE id = {student.Id};";

    //     int rows = _db.ExecuteNonQuery(sql);
    //     return rows > 0;
    // }

    // // Khóa lịch thi
    // public bool LockStudent(ExamModel student)
    // {
    //     string sql = @$"
    //         UPDATE cschool.students 
    //         SET status = 0
    //         WHERE id = {student.Id};";

    //     int rows = _db.ExecuteNonQuery(sql);
    //     return rows > 0;
    // }

}
