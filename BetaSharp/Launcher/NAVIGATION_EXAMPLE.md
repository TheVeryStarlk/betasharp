# MVVM Navigation Pattern - Quick Start Example

This example demonstrates how to use the ViewModel-first MVVM navigation pattern implemented in BetaSharp Launcher.

## Simple Navigation Flow

```
┌─────────────────────────────────────────────────────────┐
│                    ShellViewModel                        │
│  ┌──────────────────────────────────────────────────┐   │
│  │ CurrentViewModel (INotifyPropertyChanged)        │   │
│  └──────────────────────────────────────────────────┘   │
│                         │                                │
│                         ▼                                │
│              Displays Current View                       │
└─────────────────────────────────────────────────────────┘
           │                              │
           │                              │
           ▼                              ▼
   ┌──────────────┐              ┌──────────────────┐
   │ LoginViewModel│◄─────────────│JarSelectionVM   │
   │              │ NavigateTo()  │                 │
   │              ├──────────────►│                 │
   └──────────────┘              └──────────────────┘
```

## Code Flow Example

### 1. User clicks "Provide b1.7.3.jar" button in LoginView

**LoginView.axaml:**
```xml
<Button Content="Provide b1.7.3.jar" 
        Command="{Binding NavigateToJarSelectionCommand}"/>
```

### 2. Command executes in LoginViewModel

**LoginViewModel.cs:**
```csharp
public ICommand NavigateToJarSelectionCommand { get; }

private void NavigateToJarSelection()
{
    _navigationService.NavigateTo<JarSelectionViewModel>();
}
```

### 3. NavigationService creates and navigates to JarSelectionViewModel

**NavigationService.cs:**
```csharp
public void NavigateTo<TViewModel>() where TViewModel : class
{
    var viewModel = _viewModelFactory(typeof(TViewModel));
    _navigationAction(viewModel); // Sets ShellViewModel.CurrentViewModel
}
```

### 4. ShellViewModel.CurrentViewModel changes

**ShellViewModel.cs:**
```csharp
public ViewModelBase? CurrentViewModel
{
    get => _currentViewModel;
    set => SetProperty(ref _currentViewModel, value); // Fires PropertyChanged
}
```

### 5. ShellView updates via data binding

**ShellView.axaml:**
```xml
<ContentControl Content="{Binding CurrentViewModel}" />
```

### 6. DataTemplate resolves ViewModel to View

**App.axaml:**
```xml
<DataTemplate DataType="vm:JarSelectionViewModel">
    <views:JarSelectionView />
</DataTemplate>
```

### Result: JarSelectionView is now displayed!

## Key Benefits

✅ **Decoupled**: LoginViewModel doesn't know about JarSelectionViewModel's implementation  
✅ **Testable**: Can test navigation logic without UI  
✅ **Flexible**: Easy to add new views and navigation paths  
✅ **ViewModel-First**: ViewModels control the flow, not Views  
✅ **MVVM Compliant**: Full separation of concerns  

## Try It Yourself

1. Build and run the launcher: `dotnet run -c ClientDebug`
2. Click "Provide b1.7.3.jar" to navigate to JarSelectionView
3. Click "Back to Login" to navigate back to LoginView

The navigation happens entirely through ViewModels - no View knows about any other View!
