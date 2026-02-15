# MVVM Navigation Pattern Implementation

This document describes the ViewModel-first MVVM navigation pattern implemented in the BetaSharp Launcher.

## Overview

The implementation provides a simple, clean way to implement navigation in an Avalonia MVVM application where:
- ViewModels can navigate to other ViewModels without knowing about Views
- A Shell ViewModel hosts all other ViewModels
- Navigation is handled through a service that ViewModels can use

## Architecture

### Core Components

#### 1. ViewModelBase
Base class for all ViewModels implementing `INotifyPropertyChanged`:
- Located in: `BetaSharp/Launcher/ViewModels/ViewModelBase.cs`
- Provides `OnPropertyChanged()` and `SetProperty<T>()` helper methods
- All ViewModels should inherit from this class

#### 2. INavigationService
Interface defining navigation capabilities:
- Located in: `BetaSharp/Launcher/Services/INavigationService.cs`
- Method: `NavigateTo<TViewModel>()` - navigates to a specific ViewModel type
- Injected into ViewModels that need navigation capabilities

#### 3. NavigationService
Implementation of the navigation service:
- Located in: `BetaSharp/Launcher/Services/NavigationService.cs`
- Takes two delegates:
  - `viewModelFactory`: Creates ViewModels by type
  - `navigationAction`: Updates the Shell's CurrentViewModel

#### 4. ShellViewModel
The main container ViewModel:
- Located in: `BetaSharp/Launcher/ViewModels/ShellViewModel.cs`
- Has a `CurrentViewModel` property (implements INPC)
- The Shell displays whatever ViewModel is set to `CurrentViewModel`

#### 5. ShellView
The main window that hosts all views:
- Located in: `BetaSharp/Launcher/Views/ShellView.axaml`
- Uses a `ContentControl` bound to `CurrentViewModel`
- DataTemplates automatically resolve ViewModels to Views

#### 6. RelayCommand
Simple `ICommand` implementation:
- Located in: `BetaSharp/Launcher/ViewModels/RelayCommand.cs`
- Used to bind actions to buttons in XAML

## How It Works

### 1. Setup (in App.axaml.cs)

```csharp
// Create the Shell ViewModel
var shellViewModel = new ShellViewModel();

// Create navigation service
var navigationService = new NavigationService(
    viewModelFactory: type => CreateViewModel(type),
    navigationAction: viewModel => shellViewModel.CurrentViewModel = viewModel
);

// Navigate to initial view
navigationService.NavigateTo<LoginViewModel>();
```

### 2. ViewModel with Navigation

```csharp
public class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public LoginViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateToJarSelectionCommand = new RelayCommand(NavigateToJarSelection);
    }

    public ICommand NavigateToJarSelectionCommand { get; }

    private void NavigateToJarSelection()
    {
        _navigationService.NavigateTo<JarSelectionViewModel>();
    }
}
```

### 3. View with Navigation Button

```xml
<Button Content="Provide b1.7.3.jar" 
        Command="{Binding NavigateToJarSelectionCommand}"/>
```

### 4. DataTemplates (in App.axaml)

```xml
<Application.DataTemplates>
    <DataTemplate DataType="vm:LoginViewModel">
        <views:LoginView />
    </DataTemplate>
    <DataTemplate DataType="vm:JarSelectionViewModel">
        <views:JarSelectionView />
    </DataTemplate>
</Application.DataTemplates>
```

## Example Implementation

The launcher includes two example ViewModels demonstrating bidirectional navigation:

1. **LoginViewModel** - Can navigate to JarSelectionViewModel
2. **JarSelectionViewModel** - Can navigate back to LoginViewModel

Both ViewModels:
- Inherit from `ViewModelBase`
- Receive `INavigationService` via constructor injection
- Expose `ICommand` properties for navigation
- Call `_navigationService.NavigateTo<T>()` to navigate

## Benefits

1. **ViewModel-First**: ViewModels control navigation, not Views
2. **Decoupled**: ViewModels don't reference Views directly
3. **Testable**: Easy to unit test ViewModels and navigation logic
4. **Flexible**: Easy to add new ViewModels and navigation paths
5. **INPC Support**: Shell's CurrentViewModel property notifies UI of changes

## Adding New Views

To add a new navigable view:

1. Create a new ViewModel inheriting from `ViewModelBase`:
   ```csharp
   public class MyNewViewModel : ViewModelBase
   {
       private readonly INavigationService _navigationService;
       
       public MyNewViewModel(INavigationService navigationService)
       {
           _navigationService = navigationService;
       }
   }
   ```

2. Create a corresponding View (UserControl):
   ```xml
   <UserControl x:Class="BetaSharp.Launcher.Views.MyNewView"
                x:DataType="vm:MyNewViewModel">
       <!-- Your UI here -->
   </UserControl>
   ```

3. Add DataTemplate in `App.axaml`:
   ```xml
   <DataTemplate DataType="vm:MyNewViewModel">
       <views:MyNewView />
   </DataTemplate>
   ```

4. Register in the factory method in `App.axaml.cs`:
   ```csharp
   private ViewModelBase CreateViewModel(Type type)
   {
       // ... existing code ...
       else if (type == typeof(MyNewViewModel))
       {
           return new MyNewViewModel(_navigationService!);
       }
       // ...
   }
   ```

5. Navigate to it from any ViewModel:
   ```csharp
   _navigationService.NavigateTo<MyNewViewModel>();
   ```

## Advanced Scenarios

### Passing Parameters During Navigation

For passing data between ViewModels, you can:
- Use shared services
- Store data in the ViewModels themselves
- Extend the NavigationService to accept parameters

### Using a DI Container

For larger applications, replace the simple factory with a DI container like:
- Microsoft.Extensions.DependencyInjection
- Autofac
- Ninject

Example with Microsoft.Extensions.DependencyInjection:
```csharp
var services = new ServiceCollection();
services.AddSingleton<ShellViewModel>();
services.AddSingleton<INavigationService, NavigationService>();
services.AddTransient<LoginViewModel>();
services.AddTransient<JarSelectionViewModel>();

var serviceProvider = services.BuildServiceProvider();
```

## Notes

- The current implementation uses a simple factory pattern for ViewModel creation
- For production applications, consider using a proper DI container
- The NavigationService is created once and shared across all ViewModels
- DataTemplates in App.axaml provide the ViewModel-to-View mapping
