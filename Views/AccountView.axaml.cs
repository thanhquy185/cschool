using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using cschool.ViewModels;
using FluentAvalonia.UI.Controls;

namespace cschool.Views;

public partial class AccountView : UserControl
{
    public AccountView()
    {
        InitializeComponent();
        DataContext = new AccountViewModel();
    }

    private async void TestDialogButton_Click(object sender, RoutedEventArgs e)
    {
        // Header
        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto, *"),
            Classes = { "DialogHeader" }
        };
        // - Title
        var title = new TextBlock
        {
            Text = "Thông tin học sinh",
            Classes = { "DialogHeaderTitle" }
        };
        Grid.SetColumn(title, 1);
        header.Children.Add(title);
        // - Close Button
        var closeButton = new Button
        {
            Content = "X",
            Classes = { "DialogCloseButton" }
        };
        Grid.SetColumn(closeButton, 2);
        header.Children.Add(closeButton);

        // Form 
        var form = new StackPanel
        {
            Classes = { "DialogForm" },
        };
        // - Label, Input, Select...
        form.Children.Add(new TextBlock { Text = "Tên học sinh:" });
        var nameTextBox = new TextBox { Width = 200 };
        form.Children.Add(nameTextBox);
        form.Children.Add(new TextBlock { Text = "Tuổi:" });
        var ageTextBox = new TextBox { Width = 50 };
        form.Children.Add(ageTextBox);
        // - Submit Button
        var submitButton = new Button
        {
            Content = "Xác nhận",
            Classes = { "DialogSubmitButton" }
        };
        form.Children.Add(submitButton);
        // - Form Warper
        var formCWarper = new Border
        {
            Classes = { "DialogFormCWarper" },
            Child = form
        };

        // Nội dung dialog (vì không dùng title và các nút mặc định của control)
        var dialogContent = new StackPanel
        {
            Children =
        {
            header,
            formCWarper
        }
        };

        // Tạo dialog
        var dialog = new ContentDialog
        {
            Title = null,
            Content = dialogContent,
            PrimaryButtonText = null,
            Classes = { "Dialog", "Create" },
        };

        // Đóng dialog khi bấm X
        closeButton.Click += (_, __) => dialog.Hide();

        // Hiện dialog
        var result = await dialog.ShowAsync();
        submitButton.Click += (_, __) =>
        {
            Console.WriteLine($"Tên: {nameTextBox.Text}, Tuổi: {ageTextBox.Text}");
            dialog.Hide(ContentDialogResult.Primary);
        };
    }
}