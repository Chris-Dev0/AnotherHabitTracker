using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AnotherHabitTracker.ViewModels;
using AnotherHabitTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AnotherHabitTracker;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices(); // Register all the services needed for the application to run

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        Services = collection.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Called in App.OnFrameworkInitializationCompleted(), registers all the services needed for the application to run
    /// </summary>
    /// <param name="collection"></param>
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        // collection.AddSingleton<IShell>(sp => sp.GetRequiredService<MainViewModel>());
        // collection.AddSingleton<ISshService, SshService>();
    }
}