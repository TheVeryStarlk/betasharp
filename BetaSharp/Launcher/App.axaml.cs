using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BetaSharp.Launcher.Services;
using BetaSharp.Launcher.ViewModels;
using BetaSharp.Launcher.Views;

namespace BetaSharp.Launcher;

public class App : Application
{
    private INavigationService? _navigationService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create the Shell ViewModel
            var shellViewModel = new ShellViewModel();

            // Create navigation service that updates the Shell's CurrentViewModel
            _navigationService = new NavigationService(
                viewModelFactory: type => CreateViewModel(type),
                navigationAction: viewModel => shellViewModel.CurrentViewModel = viewModel
            );

            // Create the Shell View and set its DataContext
            var shellView = new ShellView
            {
                DataContext = shellViewModel
            };

            // Navigate to the initial view
            _navigationService.NavigateTo<LoginViewModel>();

            desktop.MainWindow = shellView;
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private ViewModelBase CreateViewModel(Type type)
    {
        // Simple factory method - in a real app, use a DI container
        if (type == typeof(LoginViewModel))
        {
            return new LoginViewModel(_navigationService!);
        }
        else if (type == typeof(JarSelectionViewModel))
        {
            return new JarSelectionViewModel(_navigationService!);
        }

        throw new InvalidOperationException($"Unknown ViewModel type: {type}");
    }
}