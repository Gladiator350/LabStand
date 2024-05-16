using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using WpfApp23.ApplicationContexts;
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
        var angles = Angles.FromCoordinates(_vm.XCor, _vm.YCor, _vm.ZCor);
        _arduinoService.SendAngles(angles);
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