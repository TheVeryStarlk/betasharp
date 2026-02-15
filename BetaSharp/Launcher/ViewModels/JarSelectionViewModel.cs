using System.Windows.Input;
using BetaSharp.Launcher.Services;

namespace BetaSharp.Launcher.ViewModels;

/// <summary>
/// ViewModel for the JAR file selection screen.
/// </summary>
public class JarSelectionViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private string _statusText = "Please provide b1.7.3.jar";

    public JarSelectionViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateToLoginCommand = new RelayCommand(NavigateToLogin);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public ICommand NavigateToLoginCommand { get; }

    /// <summary>
    /// Navigates back to the login screen.
    /// </summary>
    private void NavigateToLogin()
    {
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
