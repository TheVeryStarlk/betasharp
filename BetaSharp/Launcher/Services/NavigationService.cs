using BetaSharp.Launcher.ViewModels;

namespace BetaSharp.Launcher.Services;

/// <summary>
/// Service implementation for navigating between ViewModels.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private readonly Action<ViewModelBase> _navigationAction;

    public NavigationService(
        Func<Type, ViewModelBase> viewModelFactory,
        Action<ViewModelBase> navigationAction)
    {
        _viewModelFactory = viewModelFactory;
        _navigationAction = navigationAction;
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _viewModelFactory(typeof(TViewModel));
        _navigationAction(viewModel);
    }
}
