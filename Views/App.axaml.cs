using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ViewModels;
using System.Linq;
using Services;
using System;

namespace Views;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // // Thêm FluentAvaloniaTheme
        // Styles.Add(new FluentAvaloniaTheme());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            var connectionString = "Server=localhost;Database=cschool;User ID=root;Password=123456;AllowPublicKeyRetrieval=True;SslMode=None;";
            AppService.DBService = new DBService(connectionString);
            AppService.UserService = new UserService(AppService.DBService);
            AppService.StudentService = new StudentService(AppService.DBService);
            AppService.ExamService = new ExamService(AppService.DBService);
            AppService.TuitionService = new TuitionService(AppService.DBService);
            AppService.AssignTeacherService = new AssignTeacherService(AppService.DBService);
            AppService.TeacherService = new TeacherService(AppService.DBService);
            AppService.SubjectClassService = new SubjectClassService(AppService.DBService);
            AppService.statisticalService = new StatisticalService(AppService.DBService);
            AppService.homeClassService = new HomeClassService(AppService.DBService);
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
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