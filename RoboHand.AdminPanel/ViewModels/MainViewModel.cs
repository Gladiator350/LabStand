using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp23.Models;

namespace WpfApp23.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<Command> Commands { get; set; } = 
    [
    ];

    public string XCorTxt { get; set ; }
    public double XCor => Convert.ToDouble(XCorTxt);
    public string YCorTxt { get; set; }
    public double YCor => Convert.ToDouble(YCorTxt);
    public String ZCorTxt { get; set; }
    public double ZCor => Convert.ToDouble(ZCorTxt);
    public MainViewModel()
    {
        XCorTxt = "5";
        YCorTxt = "5";
        ZCorTxt = "6";
    }


    [ObservableProperty]
    private RelayCommand? _executeButtonCommand;
    [ObservableProperty]
    private RelayCommand? _coordinateButtonCommand;
    [ObservableProperty]
    private RelayCommand? _demoButtonCommand;

    [ObservableProperty] private RelayCommand? _deleteButtonCommand;
}