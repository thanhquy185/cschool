using Avalonia.Controls;
using MessageBox.Avalonia.Enums;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Utils;

public static class MessageBoxUtil
{
    public static async Task ShowInfo(string message, string title = "Thông tin", Window? owner = null)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Info);

        if (owner != null)
            await box.ShowWindowDialogAsync(owner);
        else
            await box.ShowAsync();
    }

    public static async Task ShowSuccess(string message, string title = "Thành công", Window? owner = null)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Success);

        if (owner != null)
            await box.ShowWindowDialogAsync(owner);
        else
            await box.ShowAsync();
    }

    public static async Task ShowWarning(string message, string title = "Cảnh báo", Window? owner = null)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Warning);

        if (owner != null)
            await box.ShowWindowDialogAsync(owner);
        else
            await box.ShowAsync();
    }

    public static async Task ShowError(string message, string title = "Lỗi", Window? owner = null)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Error);

        if (owner != null)
            await box.ShowWindowDialogAsync(owner);
        else
            await box.ShowAsync();
    }

    public static async Task<bool> ShowConfirm(string message, string title = "Xác nhận", Window? owner = null)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.YesNo,
            Icon.Question);

        var result = owner != null 
            ? await box.ShowWindowDialogAsync(owner)
            : await box.ShowAsync();
            
        return result == ButtonResult.Yes;
    }
}
