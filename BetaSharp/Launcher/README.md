Most of this code was written by AI. You have been warned.

## MVVM Navigation Pattern

This launcher implements a ViewModel-first MVVM navigation pattern. See:
- [MVVM_NAVIGATION.md](MVVM_NAVIGATION.md) - Complete architecture documentation
- [NAVIGATION_EXAMPLE.md](NAVIGATION_EXAMPLE.md) - Visual flow example and quick start

### Quick Overview

The launcher uses a Shell-based navigation where:
- **ShellViewModel** has a `CurrentViewModel` property (INotifyPropertyChanged)
- ViewModels receive **INavigationService** to navigate to other ViewModels
- **DataTemplates** automatically map ViewModels to Views
- Navigation is ViewModel-first - Views know nothing about other Views

Example ViewModels included:
- `LoginViewModel` - Microsoft login screen
- `JarSelectionViewModel` - JAR file selection screen

Both demonstrate bidirectional navigation using the NavigationService.