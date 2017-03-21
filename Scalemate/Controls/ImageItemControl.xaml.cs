using Scalemate.Helpers;
using Scalemate.Models;
using System;
using System.Diagnostics;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Scalemate.Controls
{
    public sealed partial class ImageItemControl : UserControl
    {
        public ImageItemControl()
        {
            this.InitializeComponent();
        }

        private async void UserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null)
            {

                ImportedImage ii = (ImportedImage)DataContext;

                if (ii.Index % 2 == 0)
                {
                    rectangleDarken.Opacity = 0.1;
                }
                else
                {
                    rectangleDarken.Opacity = 0.05;
                }

                using (StorageItemThumbnail thumbnail2 = await ii.LinkedFile.GetThumbnailAsync(ThumbnailMode.SingleItem, 1))
                {
                    if (thumbnail2 != null)
                    {
                        //Prepare thumbnail to display
                        var bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(thumbnail2);
                        imageThumbnail.Source = bitmapImage;
                        Animationmate.ChangeObjectOpacity(imageThumbnail, 0, 1, 75);
                    }
                }

                using (StorageItemThumbnail thumbnail = await ii.LinkedFile.GetThumbnailAsync(ThumbnailMode.SingleItem, 250))
                {
                    if (thumbnail != null)
                    {
                        //Prepare thumbnail to display
                        var bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(thumbnail);
                        imageFull.Source = bitmapImage;
                        imagePreview.Source = bitmapImage;
                        Animationmate.ChangeObjectOpacity(imageFull, 0, 1, 200);
                        Animationmate.ChangeObjectOpacity(imageThumbnail, 1, 0, 300, 200);
                    }
                }
                
            }
        }
    }
}
