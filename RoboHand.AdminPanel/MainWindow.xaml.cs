using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using WpfApp23.ApplicationContexts;
using WpfApp23.Calculate;
using WpfApp23.Models;
using WpfApp23.Services.Interfaces;
using WpfApp23.ViewModels;

namespace WpfApp23;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IArduinoService _arduinoService;
    private readonly ApplicationContext _context;
    private readonly MainViewModel _vm = new();
    private Command? _selectedItem = null;
    public MainWindow(IArduinoService arduinoService, WebServer server, ApplicationContext context)
    {
        _arduinoService = arduinoService;
        _context = context;
        InitializeComponent();
        DataContext = _vm;
        _vm.ExecuteButtonCommand = new RelayCommand(ExecuteButtonClick);
        _vm.CoordinateButtonCommand = new RelayCommand(CoordinateButtonCommand);
        _vm.DeleteButtonCommand = new RelayCommand(DeleteButtonClick);
        _vm.DemoButtonCommand = new RelayCommand(DemoButtonCommand);
        CommandsList.SelectedItem = _vm.Commands.FirstOrDefault();
        foreach (var command in context.Commands)
        {   
            _vm.Commands.Add(command);
        }

        ;

        async Task PerformTaskAsync()
        {
            // Вызов метода из класса 'Program'
            await server.Start(x =>
            {
                _vm.Commands.Add(x);
            });
            Console.WriteLine("Запущен сервер");
        }

        _ = PerformTaskAsync();
    }

    private void ExecuteButtonClick()
    {
        if(_selectedItem is null)
            return;
        _arduinoService.SendCommand(_selectedItem);
        //_arduinoService.SendAngles(AnglesCalculator.Recalculate(_selectedItem.Angles, _context.Variants.First(x => x.Id == _selectedItem.Uid)));
        
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CommandsList.SelectedItem is Command command)
        {
            _selectedItem = command;
        }
    }
    private void CoordinateButtonCommand()
    {
        var anglescord = Angles.FromCoordinates(_vm.XCor, _vm.YCor, _vm.ZCor);
        _arduinoService.SendAngles(anglescord);
    }
    private void DemoButtonCommand()
    {
        System.Net.WebClient htmlClient = new System.Net.WebClient();
        string htmlString = htmlClient.DownloadString("http://127.0.0.1:8070/data.html");
        if (htmlString.Contains("marker22"))
        {
            string[] values = htmlString.Split(' ');
            double x = double.Parse(values[values.Length - 3], CultureInfo.InvariantCulture);
            double y = Double.Parse(values[values.Length - 2], CultureInfo.InvariantCulture);
            double z = 0;
            var anglescord = Angles.FromCoordinates(x, y, z);
            _arduinoService.SendAngles(anglescord);
            anglescord.Alpha = 0;
            anglescord.Beta = 0;
            anglescord.Gamma = 0;
            anglescord.Theta = 0;
            anglescord.Omega = 90;
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(x, y, -7);
            _arduinoService.SendAngles(anglescord);
            anglescord.Alpha = 0;
            anglescord.Beta = 0;
            anglescord.Gamma = 0;
            anglescord.Theta = 0;
            anglescord.Omega = 180;
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(x, y, 2);
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(18, 0, -7);
            _arduinoService.SendAngles(anglescord);
            anglescord.Alpha = 0;
            anglescord.Beta = 0;
            anglescord.Gamma = 0;
            anglescord.Theta = 0;
            anglescord.Omega = 90;
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(18, 0, 2);
            _arduinoService.SendAngles(anglescord);
            x = Double.Parse(values[values.Length - 6], CultureInfo.InvariantCulture);
            y = Double.Parse(values[values.Length - 5], CultureInfo.InvariantCulture);
            anglescord = Angles.FromCoordinates(x, y, 0);
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(x, y, -7);
            _arduinoService.SendAngles(anglescord);
            anglescord.Alpha = 0;
            anglescord.Beta = 0;
            anglescord.Gamma = 0;
            anglescord.Theta = 0;
            anglescord.Omega = 180;
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(x, y, 0);
            _arduinoService.SendAngles(anglescord);
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(18, 0, -2);
            _arduinoService.SendAngles(anglescord);
            anglescord.Alpha = 0;
            anglescord.Beta = 0;
            anglescord.Gamma = 0;
            anglescord.Theta = 0;
            anglescord.Omega = 90;
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(18, 0, 5);
            _arduinoService.SendAngles(anglescord);
            anglescord = Angles.FromCoordinates(10, 0, 0);
            _arduinoService.SendAngles(anglescord);
        }
        
    }

    private void DeleteButtonClick()
    {
        if (_selectedItem != null)
        {
            _vm.Commands.Remove(_selectedItem);
            _context.Commands.Remove(_selectedItem);
            _context.SaveChanges();
        }
    }
    
}