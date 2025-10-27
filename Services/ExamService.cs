using System;
using System.Collections.Generic;
using System.Data;

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
        string sql = @$"SELECT exams.id, exams.exam_room, teachers.fullname, exams.candidate_count, 
                        subjects.name AS subject, terms.name AS term_name, exam_details.start_time
                        FROM exams
                        JOIN exam_details ON exams.exam_detail_id = exam_details.id
                        JOIN teachers ON exams.supervisor_id = teachers.id
                        JOIN subjects ON exam_details.subject_id = subjects.id
                        JOIN terms ON exam_details.term_id = terms.id";
        var dt = _db.ExecuteQuery(sql);
        var list = new List<ExamModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new ExamModel
            {
                Id = (int)row["id"],
                Room = row["exam_room"].ToString()!,
                Subject = row["subject"].ToString()!,
                TeacherName = row["fullname"].ToString()!,
                ExamDate = row["start_time"].ToString()!,
                TermName = row["term_name"].ToString()!,
                CandidateCount = row["candidate_count"].ToString()!,
            });
        }

        return list;
    }

    public ExamModel? GetExamById(int id)
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

        return new ExamModel
        {
            Id = (int)row["id"],
            Room = row["exam_room"].ToString()!,
            Subject = row["subject"].ToString()!,
            TeacherName = row["fullname"].ToString()!,
            ExamDate = row["start_time"].ToString()!,
            TermName = row["term_name"].ToString()!,
            CandidateCount = row["candidate_count"].ToString()!
        };
    }

    // // Thêm học sinh mới
    // public bool CreateStudent(ExamModel student)
    // {
    //     string formattedBirthDay = "NULL";
    //     if (!string.IsNullOrWhiteSpace(student.BirthDay) && DateTime.TryParse(student.BirthDay, out var birth))
    //     {
    //         formattedBirthDay = $"'{birth:yyyy-MM-dd}'";
    //     }
    //     string sql = @$"
    //         INSERT INTO cschool.students 
    //         (fullname, avatar, birthday, gender, ethnicity, religion, address, phone, email, learn_year, learn_status)
    //         VALUES 
    //         ('{student.Fullname}', '{student.Avatar}', {formattedBirthDay}, 
    //             '{student.Gender}', '{student.Ethnicity}', '{student.Religion}', 
    //             '{student.Address}', '{student.Phone}', '{student.Email}', 
    //             '{student.LearnYear}', '{student.LearnStatus}');";

    //     int rows = _db.ExecuteNonQuery(sql);
    //     return rows > 0;
    // }

    // // Cập nhật thông tin học sinh
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

    // // Khóa học sinh
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
