using Shared.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Scalemate.Controls
{
    public sealed partial class ExportControl : UserControl
    {
        public ExportControl()
        {
            this.InitializeComponent();
        }

        public void ShowTitleBar()
        {
            gridTitleBar.Visibility = Visibility.Visible;
            AnimationHelper.ChangeObjectHeight(gridTitleBar, 0, 60);
            AnimationHelper.ChangeObjectOpacity(gridTitleBar, 0, 1);
        }

        public void HideTitleBar()
        {
            AnimationHelper.ChangeObjectHeight(gridTitleBar, 60, 0);
            AnimationHelper.FadeObjectVisibility(gridTitleBar, 1, 0, Visibility.Collapsed);
        }

    }
}
