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
        string htmlString = htmlClient.DownloadString("http://127.0.0.1:8000/data.html");
        if (htmlString.Contains("marker22"))
        {
            string[] values = htmlString.Split(' ');
            double x = Double.Parse(values[values.Length - 3]);
            double y = Double.Parse(values[values.Length - 2]);
            double z = 0;
            var anglescord = Angles.FromCoordinates(x, y, z);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            anglescord = Angles.FromCoordinates(x, y, -6);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            // добавить захват
            anglescord = Angles.FromCoordinates(x, y, z);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            anglescord = Angles.FromCoordinates(18, 0, -4);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            // разжать захват
            x = Double.Parse(values[values.Length - 6]);
            y = Double.Parse(values[values.Length - 5]);
            anglescord = Angles.FromCoordinates(x, y, 0);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            anglescord = Angles.FromCoordinates(x, y, -5);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            // добавить захват
            anglescord = Angles.FromCoordinates(18, 0, 0);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
            // разжать захват
            anglescord = Angles.FromCoordinates(18, 0, 5);
            _arduinoService.SendAngles(anglescord);
            Thread.Sleep(2000);
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