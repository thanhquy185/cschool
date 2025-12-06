
namespace cschool.Models;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;


    public class ClassModel
    {
    public int Id { get; set; }

    public int ClassTypeId { get; set; }
    public int AssignClassId { get; set; }  
    public int FeeTemplateId { get; set; }


    public int Grade { get; set; }
    public string Name { get; set; }
    public string Area { get; set; }
    public string Room { get; set; }
    public string Term { get; set; }

    public String Year { get; set; }

    public string HeadTeacher { get; set; }
    public string ClassTypeName { get; set; } = "";

    public int Status
    { get; set; }
    public string Gender { get; internal set; }

    public TeacherModel TeacherHK1{ get; set; }
    
    public TeacherModel TeacherHK2{ get; set; }
    public bool IsSelected { get; set; } 

        public ClassModel(int ma, int Assign_class_Id, string ten, string phong)
    {
        Id = ma;
        this.AssignClassId = Assign_class_Id;
        Name = ten;
        Room = phong;
    }
    public ClassModel()
    {
        
    }
}
