using System.Data;
using Models;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System.Globalization;

namespace Services;

public class TeacherService
{
    private readonly DBService _db;
    public TeacherService(DBService dbService)
    {
        _db = dbService;
    }

    public List<TeacherModel> GetTeachers()
    {
        try
        {
            List<TeacherModel> ds = new List<TeacherModel>();
            string sql = @" SELECT DISTINCT t.id, t.avatar, t.fullname, t.birthday, t.gender, t.address, t.phone, t.email, 
                            COALESCE(dd.department_id, 0) AS department_id, u.username, u.password, u.id AS user_id,
                            COALESCE(d.name, 'Chưa có bộ môn') AS department_name, 
                            t.status
                            FROM teachers t
                            LEFT JOIN department_details dd ON dd.teacher_id = t.id
                            LEFT JOIN departments d ON d.id = dd.department_id
                            JOIN users u ON u.id = t.user_id
                            WHERE t.status = 1";

            var result = _db.ExecuteQuery(sql);
            foreach (DataRow data in result.Rows)
            {
                ds.Add(new TeacherModel{
                    Id = Convert.ToInt32(data["id"]),
                    Avatar = data["avatar"].ToString()!,
                    Name = data["fullname"].ToString()!,
                    Gender = data["gender"].ToString()!,
                    Birthday = Convert.ToDateTime(data["birthday"]).ToString("dd/MM/yyyy")!,
                    Email = data["email"].ToString()!,
                    Phone = data["phone"].ToString()!,
                    Address = data["address"].ToString()!,
                    DepartmentId = Convert.ToInt32(data["department_id"]),
                    DepartmentName = data["department_name"].ToString()!,
                    Username = data["username"].ToString()!,
                    Password = data["password"].ToString()!,
                    UserId = Convert.ToInt32(data["user_id"]),
                });
                Console.WriteLine($" Teacher Loaded: ID={data["id"]}, Name={data["fullname"]}");

            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTeachers: {ex.Message}");
            return new List<TeacherModel>();
        }

    }

    public List<DepartmentModel> GetDepartments()
    {
        try
        {
            var departments = new List<DepartmentModel>();
            string query = @"SELECT id, subject_id, name, description, status 
                            FROM departments
                            WHERE status = 1";

            var results = _db.ExecuteQuery(query);

            foreach (DataRow data in results.Rows)
            {
                var dept = new DepartmentModel(
                    Convert.ToInt32(data["id"]),
                    Convert.ToInt32(data["subject_id"]),
                    data["name"].ToString()!,
                    data["description"].ToString()!,
                    Convert.ToInt32(data["status"])
                );
                departments.Add(dept);
            }

            return departments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi lấy danh sách bộ môn: {ex.Message}");
            return new List<DepartmentModel>();
        }
    }

    // Return latest term as Tuple<id, name> or null if none
    public TermModel? GetLatestTerm()
    {
        try
        {
            string query = @"SELECT id, name, start_date, end_date, year, learnyear
                             FROM terms     
                             WHERE status = 1 ORDER BY start_date DESC LIMIT 1";

            var results = _db.ExecuteQuery(query);

            if (results.Rows.Count > 0)
            {
                DataRow data = results.Rows[0];
                // return Tuple.Create(Convert.ToInt32(data["id"]), data["name"].ToString()!);
                return new TermModel
                {
                    Id = (int)data["id"],
                    Name = data["name"].ToString()!,
                    Year = (int)data["year"],
                    Start = Convert.ToDateTime(data["start_date"]),
                    End = Convert.ToDateTime(data["end_date"]),
                    LearnYear = data["learnyear"].ToString()!
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Lỗi lấy học kỳ gần nhất: {ex.Message}");
            return null;
        }
    }
    public TermModel? GetTermByDate(DateTime date)
    {
        try
        {
            string query = @"SELECT id, name, year,start_date, end_date, status, learnyear
                            FROM terms
                            WHERE status = 1
                            AND @date BETWEEN start_date AND end_date
                            LIMIT 1";

            using var connection = _db.GetConnection();
            // connection.Open();
            Console.WriteLine($"🔍 GetTermByDate: Executing query for date {date:dd/MM/yyyy}");
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", date.ToString("yyyy/MM/dd"));

            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new TermModel
                {
                    Id = reader.GetInt32("id"),
                    Year = reader.GetInt32("year"),
                    Name = reader.GetString("name"),
                    Start = reader.GetDateTime("start_date"),
                    End = reader.GetDateTime("end_date"),
                    Status = reader.GetInt32("status"),
                    LearnYear = reader.GetString("learnyear")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi lấy học kỳ theo ngày: {ex.Message}");
            return null;
        }
    }
    public int GetIDLastTeacher()
    {
        // Console.WriteLine(123);
        var dt = _db.ExecuteQuery("SELECT id FROM cschool.teachers ORDER BY id DESC LIMIT 1");
        if (dt.Rows.Count > 0)
            return System.Convert.ToInt32(dt.Rows[0]["id"]);
        return 0;
    }

    

    public bool CreateTeacher(TeacherModel t)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"INSERT INTO teachers (avatar, fullname, birthday, gender, address, phone, email, user_id) 
                       VALUES (@avatar, @fullname, @birthday, @gender, @address, @phone, @email, @user_id);
                       ";
            
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@avatar", t.Avatar);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday", t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@user_id", t.UserId);
            // command.Parameters.AddWithValue("@status", t.Status);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0) return false;

            if (t.DepartmentId > 0)
            {
                string department_sql = @"INSERT INTO department_details (department_id, teacher_id) 
                                          VALUES (@department_id, LAST_INSERT_ID())";
                var departCommand = new MySqlCommand(department_sql, connection);
                departCommand.Parameters.AddWithValue("@department_id", t.DepartmentId);
                int deptRows = departCommand.ExecuteNonQuery();
                if (deptRows == 0) return false;
            }
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"lỗi không thể thêm giáo viên: {ex.Message}");
            return false;
        }
    }


    public bool LockTeacher(int id)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"UPDATE teachers SET status = 0 where id = @id";

            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể xóa giáo viên: " + e);
            return false;
        }
    }

    public bool UpdateTeacher(TeacherModel t)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"UPDATE teachers SET fullname = @fullname, birthday = @birthday, gender = @gender, address = @address,
                                  phone = @phone, email = @email, avatar = @avatar, status = @status
                           where id = @id";

            string dept_sql = @"UPDATE department_details SET department_id = @department_id WHERE teacher_id = @id";

            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday", t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@avatar", t.Avatar ?? "");
            command.Parameters.AddWithValue("@status", t.Status);
            command.Parameters.AddWithValue("@id", t.Id);
            
            Console.WriteLine($"🔍 UpdateTeacher: ID={t.Id}, Name={t.Name}, Avatar={t.Avatar}, Status={t.Status}");
            
            if (command.ExecuteNonQuery() > 0)
            {
                var deptCommand = new MySqlCommand(dept_sql, connection);
                deptCommand.Parameters.AddWithValue("@department_id", t.DepartmentId);
                deptCommand.Parameters.AddWithValue("@id", t.Id);
                return deptCommand.ExecuteNonQuery() > 0;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể cập nhật giáo viên: " + e);
            return false;
        }
    }
    
    public TeacherModel? GetTeacherById(int id)
    {
        try
        {
            var term = AppService.TeacherService.GetTermByDate(DateTime.Now);
            if (term == null)
            {
                term = AppService.TeacherService.GetLatestTerm();
            }
            Console.WriteLine($"🔍 GetTeacherById: Looking up term ID={term?.Id ?? 0} for term {term?.DisplayTextTermYear ?? "N/A"}");
            var connection = _db.GetConnection();
            string sql = @$" SELECT t.id, t.avatar, t.fullname, t.birthday, t.gender, t.address, t.phone, t.email, t.status,
                                   COALESCE(dd.department_id, 0) AS department_id, 
                                   COALESCE(d.name, 'Chưa có bộ môn') AS department_name,
                                   COALESCE(ac.class_id, 0) AS class_id,
                                   COALESCE(c.name, 'Chưa có lớp') AS class_name
                            FROM teachers t
                            LEFT JOIN department_details dd ON dd.teacher_id = t.id
                            LEFT JOIN departments d ON d.id = dd.department_id
                            LEFT JOIN assign_classes ac ON t.id = ac.head_teacher_id AND ac.term_id = @termId
                            LEFT JOIN classes c ON c.id = ac.class_id
                            WHERE t.id = @id 
                            LIMIT 1";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@termId", term?.Id ?? 0);

            var adapter = new MySqlDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);

            DataRow data = dt.Rows[0];
            return new TeacherModel
            {
                Id = Convert.ToInt32(data["id"]),
                Avatar = data["avatar"].ToString()!,
                Name = data["fullname"].ToString()!,
                Birthday = Convert.ToDateTime(data["birthday"]).ToString("dd/MM/yyyy"),
                Gender = data["gender"].ToString()!,
                Address = data["address"].ToString() ?? "",
                Phone = data["phone"].ToString() ?? "",
                Email = data["email"].ToString() ?? "",
                Status = Convert.ToInt32(data["status"]),
                DepartmentId = Convert.ToInt32(data["department_id"]),
                DepartmentName = data["department_name"].ToString()!,
                ClassId = Convert.ToInt32(data["class_id"]),
                ClassName = data["class_name"].ToString()!,    
                TermName = term?.DisplayTextTermYear ?? ""           
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể tìm giáo viên: " + e);
            return null;
        }
    }
    
    public void ExportTeachersToExcel(List<TeacherModel> teachers, string excelFilePath)
    {
        try
        {
            // Set EPPlus license context for non-commercial use
            ExcelPackage.License.SetNonCommercialOrganization("cschool");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Teachers");

                // Add headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Birthday";
                worksheet.Cells[1, 4].Value = "Gender";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Phone";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "DepartmentName";
                worksheet.Cells[1, 9].Value = "Status";

                // Add data rows
                for (int row = 0; row < teachers.Count; row++)
                {
                    var teacher = teachers[row];
                    worksheet.Cells[row + 2, 1].Value = teacher.Id;
                    worksheet.Cells[row + 2, 2].Value = teacher.Name;
                    worksheet.Cells[row + 2, 3].Value = teacher.Birthday;
                    worksheet.Cells[row + 2, 4].Value = teacher.Gender;
                    worksheet.Cells[row + 2, 5].Value = teacher.Address;
                    worksheet.Cells[row + 2, 6].Value = teacher.Phone;
                    worksheet.Cells[row + 2, 7].Value = teacher.Email;
                    worksheet.Cells[row + 2, 8].Value = teacher.DepartmentName;
                    worksheet.Cells[row + 2, 9].Value = teacher.Status;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the file
                package.SaveAs(new FileInfo(excelFilePath));
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error exporting teachers to Excel: " + ex.Message);
        }
    }

    public bool ImportTeachersFromExcel(string excelFilePath)
    {
        try
        {
            ExcelPackage.License.SetNonCommercialOrganization("cschool");

            // Get all departments for mapping DepartmentName to Id
            var departments = GetDepartments();
            var deptMap = departments.ToDictionary(d => d.Name.ToLowerInvariant(), d => d.Id);

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assume first sheet
                int rowCount = worksheet.Dimension.Rows;

                // Assume header in row 1: Name, Birthday, Gender, Address, Phone, Email, DepartmentName, Status
                // Columns: 1=Name, 2=Birthday (dd/MM/yyyy), 3=Gender (Nam/Nữ), 4=Address, 5=Phone, 6=Email, 7=DepartmentName, 8=Status (1/0)

                for (int row = 2; row <= rowCount; row++) // Start from row 2 (data rows)
                {
                    string birthday = "";
                    var cellVal = worksheet.Cells[row, 2].Value;

                    DateTime parsedDate = default;
                    bool gotDate = false;

                    if (cellVal is DateTime dt)
                    {
                        parsedDate = dt;
                        gotDate = true;
                    }
                    else if (cellVal is double d)
                    {
                        parsedDate = DateTime.FromOADate(d);
                        gotDate = true;
                    }
                    else if (cellVal is int i)
                    {
                        parsedDate = DateTime.FromOADate(i);
                        gotDate = true;
                    }
                    else if (cellVal != null)
                    {
                        var s = cellVal.ToString()!.Trim();
                        // numeric string (Excel serial)
                        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dv) ||
                            double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out dv))
                        {
                            parsedDate = DateTime.FromOADate(dv);
                            gotDate = true;
                        }
                        else
                        {
                            // try exact dd/MM/yyyy then general parse
                            if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate) ||
                                DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate) ||
                                DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                gotDate = true;
                            }
                        }
                    }

                    if (!gotDate)
                    {
                        var birthdayRaw = cellVal?.ToString() ?? "";
                        continue;
                    }

                    birthday = parsedDate.ToString("yyyy-MM-dd");
                    var teacher = new TeacherModel
                    {
                        Name = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Birthday = birthday,
                        Gender = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Address = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Phone = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Email = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Avatar = "", 
                        AvatarFile = "",
                    };

                    // Map DepartmentName to Id
                    var deptName = worksheet.Cells[row, 7].Value?.ToString()?.ToLowerInvariant() ?? "";
                    if (deptMap.TryGetValue(deptName, out int deptId))
                    {
                        teacher.DepartmentId = deptId;
                    }
                    else
                    {
                        Console.WriteLine($"Department '{deptName}' not found for teacher '{teacher.Name}'. Skipping.");
                        continue; 
                    }

                    Console.WriteLine($"Importing Teacher: Name={teacher.Name}, DeptId={teacher.DepartmentId}, Birthday={teacher.Birthday}, Gender={teacher.Gender}");
                    // Create the teacher
                    bool success = CreateTeacher(teacher);
                    if (!success)
                    {
                        Console.WriteLine($"Failed to create teacher '{teacher.Name}'.");
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error importing teachers from Excel: " + ex.Message);
        }
    }


}

