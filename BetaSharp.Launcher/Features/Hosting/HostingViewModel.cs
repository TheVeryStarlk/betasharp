using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Client;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Hosting;

internal sealed partial class HostingViewModel(NavigationService navigationService, ClientService clientService) : ObservableObject
{
    [ObservableProperty]
    public partial int Selected { get; set; }

    public ObservableCollection<string> Logs { get; } = [];

    [ObservableProperty]
    public partial string Message { get; set; } = "Start";

    [ObservableProperty]
    public partial bool Started { get; set; }

    private Task? _hosting;
    private Process? _process;
    private TaskCompletionSource? _completion;

    private readonly ProcessStartInfo _info = new()
    {
        FileName = Path.Combine(AppContext.BaseDirectory, "Server", "BetaSharp.Server"),
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    [RelayCommand]
    private async Task ToggleAsync()
    {
        if (Started)
        {
            Started = false;
            Message = "Stopping";

            ArgumentNullException.ThrowIfNull(_completion);
            ArgumentNullException.ThrowIfNull(_process);
            ArgumentNullException.ThrowIfNull(_hosting);

            _completion.SetResult();
            _completion = null;

            _process.Kill(entireProcessTree: true);
            _process.Dispose();
            _process = null;

            await _hosting;

            _hosting = null;

            Message = "Start";

            return;
        }

        Selected = -1;
        Logs.Clear();

        Message = "Starting";

        _completion ??= new TaskCompletionSource();

        await clientService.DownloadAsync();

        _process = Process.Start(_info);
        _hosting = Task.WhenAll(_completion.Task, ReadingAsync());

        Started = true;
        Message = "Stop";
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.Navigate<HomeViewModel>();
    }

    private async Task ReadingAsync()
    {
        ArgumentNullException.ThrowIfNull(_process);

        while (await _process.StandardOutput.ReadLineAsync() is { } line)
        {
            Logs.Add(line);
            Selected = Logs.Count - 1;
        }
    }
}
