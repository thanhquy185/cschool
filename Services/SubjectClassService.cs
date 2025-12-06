using cschool.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.IO;


namespace cschool.Services;

public class SubjectClassService
{
    private readonly DBService _db;
    public SubjectClassService(DBService dbService)
    {
        _db = dbService;
    }
    
    public List<SubjectClassModel> GetSubjectClassesByTeacherId(int teacherId = 1)
    {
        try
        {
            List<SubjectClassModel> subjectClasses = new List<SubjectClassModel>();

            var conn = _db.GetConnection();
            string query = @"SELECT act.assign_class_id, act.subject_id, ac.class_id, s.name AS subject_name, c.name AS class_name, 
                                terms.name AS term_name, terms.year, act.oral_count, act.quiz_count
                            FROM teachers t
                            JOIN assign_class_teachers act ON t.id = act.teacher_id
                            JOIN assign_classes ac ON act.assign_class_id = ac.id
                            JOIN subjects s ON act.subject_id = s.id
                            JOIN classes c ON ac.class_id = c.id
                            JOIN terms ON ac.term_id = terms.id
                            WHERE t.id = @TeacherId;";

            MySqlCommand command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@TeacherId", teacherId);

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                SubjectClassModel subjectClass = new SubjectClassModel
                {
                    Assign_class_id = (int)reader["assign_class_id"],
                    SubjectId = (int)reader["subject_id"],
                    ClassId = (int)reader["class_id"],
                    SubjectName = reader["subject_name"].ToString()!,
                    ClassName = reader["class_name"].ToString()!,
                    TermName = $"{reader["term_name"]} - Năm {reader["year"]}",
                    OralCount = (int)reader["oral_count"],
                    QuizCount = (int)reader["quiz_count"]
                };
                Console.WriteLine($"Loaded SubjectClass: ID={subjectClass.Assign_class_id}, Subject={subjectClass.SubjectName}, Class={subjectClass.ClassName}, Term={subjectClass.TermName}");
                subjectClasses.Add(subjectClass);
            }

            return subjectClasses;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching subject classes: " + ex.Message);
        }
    }

    public List<SubjectClassModel> SearchSubjectClasses(string keyword, int teacherId = 1)
    {
        try
        {
            List<SubjectClassModel> subjectClasses = new List<SubjectClassModel>();

            var conn = _db.GetConnection();
            string query = @"SELECT act.assign_class_id, act.subject_id, ac.class_id, s.name AS subject_name, c.name AS class_name, 
                                terms.name AS term_name, terms.year
                            FROM assign_class_teachers act
                            JOIN assign_classes ac ON act.assign_class_id = ac.id
                            JOIN subjects s ON act.subject_id = s.id
                            JOIN classes c ON ac.class_id = c.id
                            JOIN terms ON ac.term_id = terms.id
                            WHERE s.name LIKE @Keyword OR c.name LIKE @Keyword;";

            MySqlCommand command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                SubjectClassModel subjectClass = new SubjectClassModel
                {
                    Assign_class_id = (int)reader["assign_class_id"],
                    SubjectId = (int)reader["subject_id"],
                    ClassId = (int)reader["class_id"],
                    SubjectName = reader["subject_name"].ToString()!,
                    ClassName = reader["class_name"].ToString()!,
                    TermName = $"{reader["term_name"]} - Năm {reader["year"]}",
                };
                subjectClasses.Add(subjectClass);
            }

            return subjectClasses;
        }
        catch (Exception ex)
        {
            throw new Exception("Error searching subject classes: " + ex.Message);
        }
    }

    public List<StudentScoreModel> GetStudentsWithScores(SubjectClassModel subjectClass)
    {
        try
        {
            var students = new List<StudentScoreModel>();
            var conn = _db.GetConnection();

            // 1. Lấy hết score vào dictionary trước
            var scores = new Dictionary<int, List<(int examType, int attempt, double score)>>();

            var scQuery = @"SELECT student_id, exam_type_id, attempt, score
                            FROM score_details
                            WHERE subject_id=@SubjectId 
                            AND assign_class_id=@AssignClassId
                            ORDER BY student_id ASC, exam_type_id ASC, attempt ASC";

            using (var scCmd = new MySqlCommand(scQuery, conn))
            {
                scCmd.Parameters.AddWithValue("@SubjectId", subjectClass.SubjectId);
                scCmd.Parameters.AddWithValue("@AssignClassId", subjectClass.Assign_class_id);

                using (var scReader = scCmd.ExecuteReader())
                {
                    while (scReader.Read())
                    {
                        int studentId = (int)scReader["student_id"];
                        int examType = (int)scReader["exam_type_id"];
                        int attempt = (int)scReader["attempt"];
                        double score = Convert.ToDouble(scReader["score"]);

                        if (!scores.ContainsKey(studentId))
                            scores[studentId] = new List<(int, int, double)>();

                        scores[studentId].Add((examType, attempt, score));
                    }
                }
            }

            // 2. Lấy danh sách học sinh
            var studentQuery = @"SELECT s.id AS studentId, s.fullName
                            FROM assign_class_students acs
                            JOIN students s ON acs.student_id = s.id
                            WHERE acs.assign_class_id = @AssignClassId
                            ORDER BY s.fullName ASC";

            using (var cmd = new MySqlCommand(studentQuery, conn))
            {
                cmd.Parameters.AddWithValue("@AssignClassId", subjectClass.Assign_class_id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["studentId"];

                        var student = new StudentScoreModel
                        {
                            StudentId = id,
                            FullName = reader["fullName"].ToString()!,
                            OralScores =  Enumerable.Repeat<double?>(null, subjectClass.OralCount).ToList(),
                            Quizzes = Enumerable.Repeat<double?>(null, subjectClass.QuizCount).ToList(),
                            MidtermScore = null,
                            FinalScore = null
                        };

                        // 3. Áp điểm vào
                        if (scores.ContainsKey(id))
                        {
                            foreach (var (examType, attempt, score) in scores[id])
                            {
                                switch (examType)
                                {
                                    case 1:
                                        if (attempt >= 1 && attempt <= subjectClass.OralCount)
                                            student.OralScores[attempt - 1] = score;
                                        break;

                                    case 2:
                                        if (attempt >= 1 && attempt <= subjectClass.QuizCount)
                                            student.Quizzes[attempt - 1] = score;
                                        break;

                                    case 3:
                                        student.MidtermScore = score;
                                        break;

                                    case 4:
                                        student.FinalScore = score;
                                        break;
                                }
                            }
                        }

                        students.Add(student);
                    }
                }
            }

            return students;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching students with scores: " + ex.Message);
        }
    }

    public bool SaveStudentScores(SubjectClassModel subjectClass, List<StudentScoreModel> studentScores)
    {
        var conn = _db.GetConnection();
        try 
        {   
            int subjectId = subjectClass.SubjectId;
            int assignClassId = subjectClass.Assign_class_id;
            // xóa điểm cũ
            string deleteQuery = @"DELETE FROM score_details 
                                   WHERE subject_id=@SubjectId 
                                   AND assign_class_id=@AssignClassId;";
            var deleteCmd = new MySqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@SubjectId", subjectId);
            deleteCmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
            deleteCmd.ExecuteNonQuery();

            var insertQuery = @"INSERT INTO score_details (student_id, subject_id, assign_class_id, exam_type_id, attempt, score)
                                VALUES (@StudentId, @SubjectId, @AssignClassId, @ExamTypeId, @Attempt, @Score);";

            var cmd = new MySqlCommand(insertQuery, conn);
            foreach (var student in studentScores)
            {
                for (int i = 0; i < subjectClass.OralCount; i++)
                {
                    if (student.OralScores[i].HasValue)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                        cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                        cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                        cmd.Parameters.AddWithValue("@ExamTypeId", 1); // Điểm miệng
                        cmd.Parameters.AddWithValue("@Attempt", i + 1);
                        cmd.Parameters.AddWithValue("@Score", student.OralScores[i].Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                for (int i = 0; i < subjectClass.QuizCount; i++)
                {
                    if (student.Quizzes[i].HasValue)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                        cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                        cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                        cmd.Parameters.AddWithValue("@ExamTypeId", 2); // Điểm quiz
                        cmd.Parameters.AddWithValue("@Attempt", i + 1);
                        cmd.Parameters.AddWithValue("@Score", student.Quizzes[i].Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                if (student.MidtermScore.HasValue)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                    cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                    cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                    cmd.Parameters.AddWithValue("@ExamTypeId", 3); // Điểm giữa kỳ
                    cmd.Parameters.AddWithValue("@Attempt", 1);
                    cmd.Parameters.AddWithValue("@Score", student.MidtermScore.Value);
                    cmd.ExecuteNonQuery();
                }
                if (student.FinalScore.HasValue)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                    cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                    cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                    cmd.Parameters.AddWithValue("@ExamTypeId", 4); // Điểm cuối kỳ
                    cmd.Parameters.AddWithValue("@Attempt", 1);
                    cmd.Parameters.AddWithValue("@Score", student.FinalScore.Value);
                    cmd.ExecuteNonQuery();
                }
                double totalScore = 0;
                int totalWeight = 0;

                // Điểm miệng (oral) - hệ số 1
                for (int i = 0; i < subjectClass.OralCount; i++)
                {
                    if (student.OralScores[i].HasValue)
                    {
                        totalScore += student.OralScores[i].Value;
                        totalWeight ++;
                    }
                }

                // Điểm quiz - hệ số 1
                for (int i = 0; i < subjectClass.QuizCount; i++)
                {
                    if (student.Quizzes[i].HasValue)
                    {
                        totalScore += student.Quizzes[i].Value;
                        totalWeight ++;
                    }
                }

                // Điểm giữa kỳ - hệ số 2
                if (student.MidtermScore.HasValue)
                {
                    totalScore += student.MidtermScore.Value * 2;
                    totalWeight += 2;
                }

                // Điểm cuối kỳ - hệ số 3
                if (student.FinalScore.HasValue)
                {
                    totalScore += student.FinalScore.Value * 3;
                    totalWeight += 3;
                }

                double score = totalWeight > 0 ? totalScore / totalWeight : 0;
                Console.WriteLine($"Calculated GPA for StudentID={student.StudentId}: {score:N2}");
            }
            return true;

        }
        catch (Exception ex)
        {
            throw new Exception("Error saving student scores: " + ex.Message);
        }
    }

    public bool UpdateScoreColumns(SubjectClassModel subjectClass)
    {
        try
        {
            var conn = _db.GetConnection();
            string updateQuery = @"UPDATE assign_class_teachers 
                                   SET oral_count=@OralCount, quiz_count=@QuizCount
                                   WHERE assign_class_id=@AssignClassId 
                                   AND subject_id=@SubjectId;";
            var cmd = new MySqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@OralCount", subjectClass.OralCount);
            cmd.Parameters.AddWithValue("@QuizCount", subjectClass.QuizCount);
            cmd.Parameters.AddWithValue("@AssignClassId", subjectClass.Assign_class_id);
            cmd.Parameters.AddWithValue("@SubjectId", subjectClass.SubjectId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating score columns: " + ex.Message);
        }
    }

    public (bool success, string message) ImportStudentScoresFromExcel(SubjectClassModel subjectClass, string excelFilePath)
    {
        try
        {
            var conn = _db.GetConnection();
            int subjectId = subjectClass.SubjectId;
            int assignClassId = subjectClass.Assign_class_id;

        // 1️⃣ Lấy danh sách studentId hợp lệ
        var validStudentIds = new HashSet<int>();
        string studentQuery = @"SELECT student_id 
                                FROM assign_class_students 
                                WHERE assign_class_id = @AssignClassId";

        using (var cmd = new MySqlCommand(studentQuery, conn))
        {
            cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    validStudentIds.Add(reader.GetInt32("student_id"));
                }
            }
        }

        // 2️⃣ Đọc Excel và validate trước
        ExcelPackage.License.SetNonCommercialOrganization("cschool");

        using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
        {
            var ws = package.Workbook.Worksheets[0];
            int rowCount = ws.Dimension.Rows;

            int oralStart = 3;
            int quizStart = oralStart + subjectClass.OralCount;
            int midCol = quizStart + subjectClass.QuizCount;
            int finalCol = midCol + 1;

            for (int row = 2; row <= rowCount; row++)
            {
                int studentId = Convert.ToInt32(ws.Cells[row, 1].Value);

                if (!validStudentIds.Contains(studentId))
                {
                    return (false, "StudentId không hợp lệ");
                }

                for (int i = 0; i < subjectClass.OralCount; i++)
                {
                    var v = ws.Cells[row, oralStart + i].Value;
                    if (v != null && !double.TryParse(v.ToString(), out _))
                    {
                        return (false, "Điểm nhập không hợp lệ");
                    }
                }

                for (int i = 0; i < subjectClass.QuizCount; i++)
                {
                    var v = ws.Cells[row, quizStart + i].Value;
                    if (v != null && !double.TryParse(v.ToString(), out _))
                    {
                        return (false, "Điểm nhập không hợp lệ");
                    }
                }

                var mid = ws.Cells[row, midCol].Value;
                if (mid != null && !double.TryParse(mid.ToString(), out _))
                {
                    return (false, "Điểm nhập không hợp lệ");
                }

                var final = ws.Cells[row, finalCol].Value;
                if (final != null && !double.TryParse(final.ToString(), out _))
                {
                    return (false, "Điểm nhập không hợp lệ");
                }
            }
        }
            string deleteQuery = @"DELETE FROM score_details 
                                WHERE subject_id=@SubjectId 
                                AND assign_class_id=@AssignClassId;";
            using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
            {
                deleteCmd.Parameters.AddWithValue("@SubjectId", subjectId);
                deleteCmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                deleteCmd.ExecuteNonQuery();
            }

            // Prepare insert command
            var insertQuery = @"INSERT INTO score_details (student_id, subject_id, assign_class_id, exam_type_id, attempt, score)
                                VALUES (@StudentId, @SubjectId, @AssignClassId, @ExamTypeId, @Attempt, @Score);";
            using (var cmd = new MySqlCommand(insertQuery, conn))
            {
                // Read Excel file using EPPlus
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    ExcelPackage.License.SetNonCommercialOrganization("cschool");
                    var worksheet = package.Workbook.Worksheets[0]; // Assume first sheet
                    int rowCount = worksheet.Dimension.Rows;

                    // Assume header in row 1: StudentId, FullName, Oral1, Oral2, ..., Quiz1, Quiz2, ..., Midterm, Final
                    // Columns: 1=StudentId, 2=FullName, 3 to 3+OralCount-1=OralScores, then Quizzes, then Midterm, Final
                    int oralStartCol = 3;
                    int quizStartCol = oralStartCol + subjectClass.OralCount;
                    int midtermCol = quizStartCol + subjectClass.QuizCount;
                    int finalCol = midtermCol + 1;

                    for (int row = 2; row <= rowCount; row++) 
                    {
                        int studentId = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                        // FullName is ignored for import, as it's display-only

                        // Import Oral Scores
                        for (int i = 0; i < subjectClass.OralCount; i++)
                        {
                            var cellValue = worksheet.Cells[row, oralStartCol + i].Value;
                            if (cellValue != null && double.TryParse(cellValue.ToString(), out double score))
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@StudentId", studentId);
                                cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                                cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                                cmd.Parameters.AddWithValue("@ExamTypeId", 1); 
                                cmd.Parameters.AddWithValue("@Attempt", i + 1);
                                cmd.Parameters.AddWithValue("@Score", score);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Import Quizzes
                        for (int i = 0; i < subjectClass.QuizCount; i++)
                        {
                            var cellValue = worksheet.Cells[row, quizStartCol + i].Value;
                            if (cellValue != null && double.TryParse(cellValue.ToString(), out double score))
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@StudentId", studentId);
                                cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                                cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                                cmd.Parameters.AddWithValue("@ExamTypeId", 2);
                                cmd.Parameters.AddWithValue("@Attempt", i + 1);
                                cmd.Parameters.AddWithValue("@Score", score);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Midterm
                        var midtermValue = worksheet.Cells[row, midtermCol].Value;
                        if (midtermValue != null && double.TryParse(midtermValue.ToString(), out double midtermScore))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@StudentId", studentId);
                            cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                            cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                            cmd.Parameters.AddWithValue("@ExamTypeId", 3); 
                            cmd.Parameters.AddWithValue("@Attempt", 1);
                            cmd.Parameters.AddWithValue("@Score", midtermScore);
                            cmd.ExecuteNonQuery();
                        }

                        // Final
                        var finalValue = worksheet.Cells[row, finalCol].Value;
                        if (finalValue != null && double.TryParse(finalValue.ToString(), out double finalScore))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@StudentId", studentId);
                            cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                            cmd.Parameters.AddWithValue("@AssignClassId", assignClassId);
                            cmd.Parameters.AddWithValue("@ExamTypeId", 4); 
                            cmd.Parameters.AddWithValue("@Attempt", 1);
                            cmd.Parameters.AddWithValue("@Score", finalScore);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return (true, "");
        }
        catch (Exception ex)
        {
            throw new Exception("Error importing student scores from Excel: " + ex.Message);
        }
    }

    public void ExportStudentScoresToExcel(SubjectClassModel subjectClass, List<StudentScoreModel> studentScores, string excelFilePath)
    {
        try
        {
            ExcelPackage.License.SetNonCommercialOrganization("cschool");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Scores");

                // Add headers
                worksheet.Cells[1, 1].Value = "StudentId";
                worksheet.Cells[1, 2].Value = "FullName";

                int oralStartCol = 3;
                for (int i = 1; i <= subjectClass.OralCount; i++)
                {
                    worksheet.Cells[1, oralStartCol + i - 1].Value = $"Oral{i}";
                }

                int quizStartCol = oralStartCol + subjectClass.OralCount;
                for (int i = 1; i <= subjectClass.QuizCount; i++)
                {
                    worksheet.Cells[1, quizStartCol + i - 1].Value = $"Quiz{i}";
                }

                int midtermCol = quizStartCol + subjectClass.QuizCount;
                worksheet.Cells[1, midtermCol].Value = "Midterm";

                int finalCol = midtermCol + 1;
                worksheet.Cells[1, finalCol].Value = "Final";

                // Add data rows
                for (int row = 0; row < studentScores.Count; row++)
                {
                    var student = studentScores[row];
                    worksheet.Cells[row + 2, 1].Value = student.StudentId;
                    worksheet.Cells[row + 2, 2].Value = student.FullName;

                    // Oral Scores
                    for (int i = 0; i < subjectClass.OralCount; i++)
                    {
                        if (student.OralScores[i].HasValue)
                        {
                            worksheet.Cells[row + 2, oralStartCol + i].Value = student.OralScores[i].Value;
                        }
                    }

                    // Quizzes
                    for (int i = 0; i < subjectClass.QuizCount; i++)
                    {
                        if (student.Quizzes[i].HasValue)
                        {
                            worksheet.Cells[row + 2, quizStartCol + i].Value = student.Quizzes[i].Value;
                        }
                    }

                    // Midterm
                    if (student.MidtermScore.HasValue)
                    {
                        worksheet.Cells[row + 2, midtermCol].Value = student.MidtermScore.Value;
                    }

                    // Final
                    if (student.FinalScore.HasValue)
                    {
                        worksheet.Cells[row + 2, finalCol].Value = student.FinalScore.Value;
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.SaveAs(new FileInfo(excelFilePath));
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error exporting student scores to Excel: " + ex.Message);
        }
    }
    
}
