using System.Windows.Input;
using BetaSharp.Launcher.Services;

namespace BetaSharp.Launcher.ViewModels;

/// <summary>
/// ViewModel for the Microsoft login screen.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private string _statusText = "Ready to launch";

    public LoginViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateToJarSelectionCommand = new RelayCommand(NavigateToJarSelection);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public ICommand NavigateToJarSelectionCommand { get; }

    /// <summary>
    /// Navigates to the JAR selection screen.
    /// </summary>
    private void NavigateToJarSelection()
    {
        _navigationService.NavigateTo<JarSelectionViewModel>();
    }
}
