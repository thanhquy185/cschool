using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using cschool.ViewModels;
using cschool.Views;
using System.Linq;
using cschool.Services;
using System;

namespace cschool;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
{
    try
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            var connectionString = "Server=localhost;Database=cschool;User ID=root;Password=123456;AllowPublicKeyRetrieval=True;SslMode=None";
            AppService.DBService = new DBService(connectionString);
            AppService.UserService = new UserService(AppService.DBService);
            AppService.AssignTeacherService = new AssignTeacherService(AppService.DBService);
            AppService.TeacherService = new TeacherService(AppService.DBService);
            AppService.DepartmentService = new DepartmentService(AppService.DBService);
            AppService.TermService = new TermService(AppService.DBService);
            AppService.statisticalService = new StatisticalService(AppService.DBService);
            AppService.homeClassService = new HomeClassService(AppService.DBService);
            Console.WriteLine("Creating MainWindow...");
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
            Console.WriteLine("MainWindow created successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fatal error: {ex}");
    }

    base.OnFrameworkInitializationCompleted();
}

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
