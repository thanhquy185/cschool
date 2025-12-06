    using System;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using System.IO;
    namespace cschool.Models;

    public class ClassTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ClassTypeModel() { }
        public ClassTypeModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
