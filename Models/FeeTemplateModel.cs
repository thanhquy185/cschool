using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Models
{
    public class FeeTemplateModel : ObservableObject
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Type { get; set; } = "";      // ✅ sửa field 'Type' thành property

        public Decimal Amount { get; set; }        // đổi từ int sang decimal nếu cần tiền có số lẻ

        public string CreatedAt { get; set; }    // đổi từ string sang DateTime

        public string UpdatedAt { get; set; }    // đổi từ string sang DateTime
        
        private bool _isSelected;
        public bool IsReadOnly { get; set; } = true;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public UserModel CreatedBy { get; set; } = new UserModel();
        public String UpdatedBy { get;}
    }
}
