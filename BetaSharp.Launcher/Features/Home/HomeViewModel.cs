using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BetaSharp.Launcher.Features.Accounts;
using BetaSharp.Launcher.Features.Authentication;
using BetaSharp.Launcher.Features.Playing;
using BetaSharp.Launcher.Features.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaSharp.Launcher.Features.Home;

internal sealed partial class HomeViewModel(
    NavigationService navigationService,
    AccountsService accountsService,
    SkinService skinService) : ObservableObject
{
    [ObservableProperty]
    public partial Account? Account { get; set; }

    [ObservableProperty]
    public partial CroppedBitmap? Face { get; set; }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        Account = await accountsService.GetAsync();

        ArgumentNullException.ThrowIfNull(Account);

        if (!string.IsNullOrWhiteSpace(Account.Skin))
        {
            Face = await skinService.GetFaceAsync(Account.Skin);
        }
    }

    [RelayCommand]
    private void Play()
    {
        navigationService.Navigate<PlayingViewModel>();
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        navigationService.Navigate<AuthenticationViewModel>();
        await accountsService.DeleteAsync();
    }
}
