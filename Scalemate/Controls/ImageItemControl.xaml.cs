using Scalemate.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Scalemate.Controls
{
    public sealed partial class ImageItemControl : UserControl
    {
        public ImageItemControl()
        {
            this.InitializeComponent();
        }

        private void imageThumbnail_Loaded(object sender, RoutedEventArgs e)
        {
           // Animationmate.ChangeObjectOpacity(imageThumbnail, 0, 1, 50);
        }

        private void imageFull_Loaded(object sender, RoutedEventArgs e)
        {
            Animationmate.ChangeObjectOpacity(imageFull, 0, 1, 250);
        }

    }
}
