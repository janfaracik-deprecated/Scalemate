using Scalemate.ViewModels;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Scalemate.Views
{
    public sealed partial class ImportView : UserControl
    {

        public ImportView()
        {
            this.InitializeComponent();
            AnimateIn();
        }

        public void AnimateIn()
        {
            AnimateInStoryboard.Begin();
        }

        public void AnimateOut()
        {
            AnimateOutStoryboard.Begin();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TouchCapabilities touchCapabilities = new TouchCapabilities();
            if (touchCapabilities.TouchPresent == 1)
            {
                textBlockClickToBrowse.Text = "or tap to Browse";
            }
        }

        private void buttonClickToBrowse_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel mainPageViewModel = DataContext as MainPageViewModel;
            mainPageViewModel.BrowseImages();
        }

    }
}