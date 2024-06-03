using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoboHand.Aruco;
using RoboHand.Streaming;
using WpfApp23.ApplicationContexts;
using WpfApp23.Models;
using WpfApp23.Services;
using WpfApp23.Services.Interfaces;


namespace WpfApp23;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Обрабатывайте исключение здесь
        MessageBox.Show("Произошла серьезная ошибка: " + (e.ExceptionObject as Exception)?.Message, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            
        // e.IsTerminating указывает, завершается ли приложение
    }//
    public IServiceProvider? ServiceProvider { get; private set; }
    public IConfiguration? Configuration { get; private set; }
    
    public ImageStreamingServer Server { get; private set; }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true);
        Configuration = builder.Build();
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
        /*Server = new ImageStreamingServer(ArucoDetection.Start());
        Server.Start();*/
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        if (Configuration != null) 
            services.Configure<ArduinoSettings>(Configuration.GetSection(nameof(ArduinoSettings)));
        services.AddSingleton<IArduinoService, ArduinoService>();
        services.AddTransient(typeof(MainWindow));//
        services.AddSingleton<WebServer>();
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlite("Data Source = File.db");
        });
    }
}