using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Client;
using BetaSharp.Launcher.Features.Home;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Playing;

internal sealed partial class PlayingViewModel(NavigationService navigationService, AccountsService accountsService, ClientService clientService) : ObservableObject
{
    [ObservableProperty]
    public partial int Selected { get; set; }

    public ObservableCollection<string> Logs { get; } = [];

    private Process? _process;
    private TaskCompletionSource? _completion;

    private readonly ProcessStartInfo _info = new()
    {
        FileName = Path.Combine(AppContext.BaseDirectory, "Client", "BetaSharp.Client"),
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    [RelayCommand]
    private async Task InitializeAsync()
    {
        Selected = -1;
        Logs.Clear();

        _completion = new TaskCompletionSource();

        var getting = accountsService.GetAsync();
        var downloading = clientService.DownloadAsync();

        await Task.WhenAll(getting, downloading);

        var account = getting.Result;

        // Check if account's token has expired.
        ArgumentNullException.ThrowIfNull(account);

        _info.Arguments = $"{account.Name} {account.Token}";
        _process = Process.Start(_info);

        // ArgumentNullException.ThrowIfNull(_process);

        await Task.WhenAll(_completion.Task, ReadingAsync());

        navigationService.Navigate<HomeViewModel>();
    }

    [RelayCommand]
    private void Stop()
    {
        ArgumentNullException.ThrowIfNull(_process);
        ArgumentNullException.ThrowIfNull(_completion);

        _completion.SetResult();
        _completion = null;

        _process.Kill(entireProcessTree: true);
        _process.Dispose();
        _process = null;
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
