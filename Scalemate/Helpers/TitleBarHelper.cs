using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Scalemate.Helpers
{

    class TitleBarHelper
    {

        //Gets Current TitleBar
        ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        public CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

        public TitleBarHelper()
        {

            //Sets TitleBar colours
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //Extends view into TitleBar
            coreTitleBar.ExtendViewIntoTitleBar = true;

            UI_ColorValuesChanged(null, null);

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(660,660));

            //Tracks System Colour
            UISettings UI = new UISettings();
            UI.ColorValuesChanged += UI_ColorValuesChanged;
        }
        
        private void UI_ColorValuesChanged(UISettings sender, object args)
        {

            //Tracks the system theme and updates the caption buttons accordingly

            if (((SolidColorBrush)Application.Current.Resources["ForegroundColor"]).Color.ToString() == "#FF000000")
            {
                formattableTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                formattableTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(50, 0, 0, 0);
            }
            else
            {
                formattableTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(35, 255, 255, 255);
                formattableTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(70, 255, 255, 255);
            }

        }

    }

}
