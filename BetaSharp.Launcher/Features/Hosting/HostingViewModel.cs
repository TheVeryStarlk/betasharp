using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Hosting;

internal sealed partial class HostingViewModel(NavigationService navigationService) : ObservableObject
{
    [ObservableProperty]
    public partial int Selected { get; set; }

    public ObservableCollection<string> Logs { get; } = [];

    [ObservableProperty]
    public partial string Message { get; set; } = "Start";

    [ObservableProperty]
    public partial bool Started { get; set; }

    [RelayCommand]
    private async Task StartAsync()
    {
        Selected = -1;
        Logs.Clear();

        if (Started)
        {
            Started = false;

            Message = "Stopping";

            await Task.Delay(1000);

            Message = "Start";

            return;
        }

        Started = true;

        Message = "Starting";

        await Task.Delay(1000);

        Message = "Stop";
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.Navigate<HomeViewModel>();
    }
}
