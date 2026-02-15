namespace BetaSharp.Launcher.Services;

/// <summary>
/// Service for navigating between ViewModels in the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel to navigate to.</typeparam>
    void NavigateTo<TViewModel>() where TViewModel : class;
}
