namespace BetaSharp.Launcher.ViewModels;

/// <summary>
/// Main shell ViewModel that hosts other ViewModels and controls navigation.
/// </summary>
public class ShellViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;

    /// <summary>
    /// Gets or sets the currently displayed ViewModel.
    /// </summary>
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }
}
