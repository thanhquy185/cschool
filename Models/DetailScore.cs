using System.Collections.Generic;

namespace cschool.Models;

public class DetailScore
{
    public string NameSubject { get; set; } = "";
    public List<float> Diem15p { get; set; } = new List<float>();
    public List<float> DiemMieng { get; set; } = new List<float>();
    public float DiemGK { get; set; }
    public float DiemCK { get; set; }
    public float DiemTrungBinh{get; set;}
    // Property để hiển thị điểm dạng chuỗi
    public string Diem15pDisplay => string.Join("\t", Diem15p);
    public string DiemMiengDisplay => string.Join("\t", DiemMieng);

}