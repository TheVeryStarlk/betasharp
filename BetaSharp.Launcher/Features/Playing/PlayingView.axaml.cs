using Avalonia.Controls;

namespace BetaSharp.Launcher.Features.Playing;

internal sealed partial class PlayingView : UserControl
{
    public PlayingView(PlayingViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
