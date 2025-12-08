using System;
using System.Collections.Generic;
using System.Data;
using cschool.Models;
using System.Threading.Tasks;
using System.Linq;


namespace cschool.Services;

public class TuitionService
{
    private readonly DBService _db;

    public TuitionService(DBService db)
    {
        _db = db;
    }
 
         public List<TuitionModel> GetAllStudents()
    {
       List<TuitionModel> list = new List<TuitionModel>();

    try
    {
        string sql = @"
            SELECT 
                tm.id,
                tm.student_id,
                s.fullname AS student_name,
                
                tm.assign_class_id,
                ac.class_id,
                c.name AS class_name,
                c.grade,

                te.year AS class_year,

                tm.month_id,
                m.name AS month_name,

                tm.total_amount,
                tm.is_paid

            FROM tuition_monthly tm
            JOIN students s ON s.id = tm.student_id
            LEFT JOIN assign_classes ac ON ac.id = tm.assign_class_id
            LEFT JOIN terms te ON te.id = ac.term_id
            LEFT JOIN classes c ON c.id = ac.class_id
            LEFT JOIN months m ON m.id = tm.month_id
            ORDER BY ac.class_id, tm.student_id, tm.month_id;
        ";

        DataTable dt = _db.ExecuteQuery(sql);

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new TuitionModel
            {
                Id = Convert.ToInt32(row["id"]),
                StudentId = Convert.ToInt32(row["student_id"]),
                StudentName = row["student_name"].ToString() ?? "",

                AssignClassId = Convert.ToInt32(row["assign_class_id"]),
                ClassId = row["class_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["class_id"]),
                ClassName = row["class_name"].ToString() ?? "",
                Grade =Convert.ToInt32(row["grade"]),

                ClassYear = row["class_year"].ToString() ?? "",

                MonthId = Convert.ToInt32(row["month_id"]),
                MonthName = row["month_name"].ToString() ?? "",

                TotalAmount = Convert.ToDecimal(row["total_amount"]),
                IsPaid = Convert.ToBoolean(row["is_paid"])
            });
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi GetAllTuitionMonthly: {ex.Message}");
    }

    return list;
    }
   

        public int GetClassFeeTemplateId(int assignClassId, int feeTemplateId)
    {
        try
        {
            string sql = $@"
                SELECT id
                FROM class_fee_months
                WHERE assign_class_id = {assignClassId}
                AND fee_template_id = {feeTemplateId}
                AND is_active = 1
                LIMIT 1";

            object? result = _db.ExecuteScalar(sql);

            return result == null ? 0 : Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error GetClassFeeTemplateId: " + ex.Message);
            return 0;
        }
    }


    public List<MonthFeeItem> GetAllMonths()
    {
        List<MonthFeeItem> list = new List<MonthFeeItem>();

        try
        {
            string sql = "SELECT id, name FROM months ORDER BY id";
            var result = _db.ExecuteQuery(sql);

            foreach (DataRow row in result.Rows)
            {
                list.Add(new MonthFeeItem
                {
                    MonthId = Convert.ToInt32(row["id"]),
                    MonthName = row["name"].ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error GetAllMonths: " + ex.Message);
        }

        return list;
    }




    // Lấy danh sách lớp + học phí tháng
    public List<FeeTemplateModel> GetFeeTemplates()
    {
        List<FeeTemplateModel> list = new List<FeeTemplateModel>();
        try
        {
            string sql = @"SELECT id, name, fee_type, amount, created_at, updated_at 
                        FROM fee_templates 
                        WHERE is_active = 1";

            var result = _db.ExecuteQuery(sql); // _db là DBService

            foreach (DataRow row in result.Rows)
            {
                list.Add(new FeeTemplateModel
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString() ?? "",
                    Type = row["fee_type"].ToString() ?? "",
                    Amount = Convert.ToInt32(row["amount"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"]).ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetFeeTemplates: {ex.Message}");
            return new List<FeeTemplateModel>();
        }
    }

    public void SaveFeeTemplates(List<FeeTemplateModel> feeTemplates, List<FeeTemplateModel> deletedFees)
    {
        try
        {
            // Xử lý soft delete
            foreach (var fee in deletedFees)
            {
                string softDeleteSql = $"UPDATE fee_templates SET is_active = 0, updated_at = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' WHERE id = {fee.Id}";
                _db.ExecuteNonQuery(softDeleteSql);
            }
            deletedFees.Clear();

            // Thêm hoặc cập nhật
            foreach (var fee in feeTemplates)
            {
                if (fee.Id == 0)
                {
                    string insertSql = $@"
                        INSERT INTO fee_templates (name, fee_type, amount, is_active, created_at, updated_at)
                        VALUES ('{fee.Name}', '{fee.Type}', {fee.Amount}, 1, '{fee.CreatedAt}', '{fee.UpdatedAt}')";
                    _db.ExecuteNonQuery(insertSql);
                }
                else
                {
                    string updateSql = $@"
                        UPDATE fee_templates
                        SET name = '{fee.Name}',
                            fee_type = '{fee.Type}',
                            amount = {fee.Amount},
                            updated_at = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'
                        WHERE id = {fee.Id}";
                    _db.ExecuteNonQuery(updateSql);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveFeeTemplates: {ex.Message}");
        }
    }



    public List<FeeClassMonthModel> getClassFeeMonth()
    { 
        // Lấy của tất cả các tháng
        List <FeeClassMonthModel> list = new List<FeeClassMonthModel>();


        return list;
        
    }


    public bool SaveFeeClassMonths(List<FeeClassMonthModel> feeClassMonths)
        {
            if (feeClassMonths == null || feeClassMonths.Count == 0)
                return false;

            try
            {
                // --- LƯU class_fee_months ---
                foreach (var item in feeClassMonths)
                {
                    string sql = $@"
                        INSERT INTO class_fee_months
                            (assign_class_id, fee_template_id, month_id, term, amount, start_date, end_date, created_at, updated_at)
                        VALUES
                            ({item.AssignClassId}, {item.FeeTemplateId}, {item.MonthId}, {item.Term}, {item.Amount},
                             '{item.StartDate}', '{item.EndDate}', '{item.CreatedAt}', '{item.UpdatedAt}');";

                    _db.ExecuteNonQuery(sql);
                }

                // --- LẤY ASSIGN_CLASS_ID của các assign class khác nhau ---
                var assignClassIds = feeClassMonths
                    .Select(f => f.AssignClassId)
                    .Distinct()
                    .ToList();

                // --- TẠO tuition_monthly cho học sinh cho mỗi assign class ---
                foreach (var acId in assignClassIds)
                    GenerateTuitionMonthly(acId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi lưu FeeClassMonths: {ex.Message}");
                return false;
            }
        }

        private void GenerateTuitionMonthly(int assignClassId)
        {
            try
            {
                // 1. Lấy tổng tiền theo tháng (sử dụng ExecuteQuery -> DataTable)
                string sqlSum = $@"
                    SELECT month_id, SUM(amount) AS total_amount
                    FROM class_fee_months
                    WHERE assign_class_id = {assignClassId}
                    GROUP BY month_id;";

                DataTable monthFeesTable = _db.ExecuteQuery(sqlSum);

                // 2. Lấy học sinh thuộc assign_class_id
                string sqlStudents = $@"
                    SELECT student_id 
                    FROM assign_class_students
                    WHERE assign_class_id = {assignClassId};";

                DataTable studentsTable = _db.ExecuteQuery(sqlStudents);

                // 3. Tạo tuition_monthly
                foreach (DataRow stuRow in studentsTable.Rows)
                {
                    int studentId = Convert.ToInt32(stuRow["student_id"]);

                    foreach (DataRow mfRow in monthFeesTable.Rows)
                    {
                        int monthId = Convert.ToInt32(mfRow["month_id"]);
                        // total_amount có thể trả về int/decimal tùy database => dùng Convert.ToDecimal
                        decimal total = Convert.ToDecimal(mfRow["total_amount"]);

                        string insertSql = $@"
                            INSERT INTO tuition_monthly
                                (student_id, assign_class_id, month_id, total_amount, is_paid, created_at, updated_at)
                            VALUES
                                ({studentId}, {assignClassId}, {monthId}, {total}, 0, '{DateTime.Now:yyyy-MM-dd HH:mm:ss}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}');";

                        _db.ExecuteNonQuery(insertSql);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi GenerateTuitionMonthly: {ex.Message}");
            }
        }

public List<FeeClassMonthModel> GetSavedFeeClassMonths(int assignClassId)
{
    Console.WriteLine("===== GetSavedFeeClassMonths START =====");
    Console.WriteLine($"► assignClassId truyền vào = {assignClassId}");

    var list = new List<FeeClassMonthModel>();

    try
    {
        string sql = $@"
            SELECT cfm.id,
                   cfm.assign_class_id,
                   cfm.fee_template_id,
                   ft.name AS FeeTemplateName,
                   cfm.month_id,
                   cfm.term,
                   cfm.amount,
                   cfm.start_date,
                   cfm.end_date,
                   cfm.created_at,
                   cfm.updated_at
            FROM class_fee_months cfm
            LEFT JOIN fee_templates ft ON cfm.fee_template_id = ft.id
            WHERE cfm.assign_class_id = {assignClassId}";

        Console.WriteLine("► SQL Query:");
        Console.WriteLine(sql);

        DataTable dt = _db.ExecuteQuery(sql);

        Console.WriteLine($"► Số dòng lấy được từ DB: {dt.Rows.Count}");

        foreach (DataRow row in dt.Rows)
        {
            var model = new FeeClassMonthModel
            {
                Id = Convert.ToInt32(row["id"]),
                AssignClassId = Convert.ToInt32(row["assign_class_id"]),
                FeeTemplateId = Convert.ToInt32(row["fee_template_id"]),
                FeeTemplateName = row["FeeTemplateName"].ToString() ?? "",
                MonthId = Convert.ToInt32(row["month_id"]),
                Term = Convert.ToInt32(row["term"]),
                Amount = Convert.ToDecimal(row["amount"]),
                StartDate = row["start_date"].ToString() ?? "",
                EndDate = row["end_date"].ToString() ?? "",
                CreatedAt = row["created_at"].ToString() ?? "",
                UpdatedAt = row["updated_at"].ToString() ?? ""
            };

            list.Add(model);

            // 🔥 Log chi tiết từng row
            Console.WriteLine(
                $"  • Row: Id={model.Id}, FeeTemplateId={model.FeeTemplateId}, Name={model.FeeTemplateName}, MonthId={model.MonthId}, Term={model.Term}, Amount={model.Amount}, Start={model.StartDate}, End={model.EndDate}"
            );
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi GetSavedFeeClassMonths: {ex.Message}");
    }

    Console.WriteLine("===== GetSavedFeeClassMonths END =====");

    return list;
}


}
