using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
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
    private readonly MainViewModel _vm = new();
    private Command? _selectedItem = null;
    public MainWindow(IArduinoService arduinoService)
    {
        _arduinoService = arduinoService;
        InitializeComponent();
        DataContext = _vm;
        _vm.ExecuteButtonCommand = new RelayCommand(ExecuteButtonClick);
        _vm.CoordinateButtonCommand = new RelayCommand(CoordinateButtonCommand);
        CommandsList.SelectedItem = _vm.Commands.FirstOrDefault();
        _vm.Commands.Add(new Command(){Id = 2, Uid = 3, Timestamp = 17000003}); ;

        async Task PerformTaskAsync()
        {
            // Вызов метода из класса 'Program'
            await WebServer.Start(new string[0]);
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
}