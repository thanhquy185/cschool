using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cschool.ViewModels;

public partial class AccountViewModel : ViewModelBase
{
    // Tiêu đề trang
    public string TitlePage { get; } = "Thông tin tài khoản";
    // Mô tả trang
    public string DescriptionPage { get; } = "Thông tin cơ bản tài khoản";
    // Danh sách tài khoản
    public ObservableCollection<Account> Accounts { get; }

    public AccountViewModel()
    {
        var account = new List<Account>
            {
                new Account(1, null, "test-username", "test-password", 1, "test-fullname", "1234567890", "test@gmail.com", "test-address", "Hoạt động"),
                new Account(2, null, "test-username", "test-password", 1, "test-fullname", "1234567890", "test@gmail.com", "test-address", "Tạm dừng"),
            };
        Accounts = new ObservableCollection<Account>(account);
    }
}