using Scalemate.Models;
using Shared.Helpers;
using System;
using System.Diagnostics;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            if (DataContext != null && DataContext.GetType() == typeof(ImportedImage))
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
                        AnimationHelper.ChangeObjectOpacity(imageThumbnail, 0, 1, 75);
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
                        AnimationHelper.ChangeObjectOpacity(imageFull, 0, 1, 200);
                        AnimationHelper.ChangeObjectOpacity(imageThumbnail, 1, 0, 300, 200);
                    }
                }

               // ii.ImageProps = await ii.LinkedFile.Properties.GetImagePropertiesAsync();

                using (StorageItemThumbnail dominantColour = await ii.LinkedFile.GetThumbnailAsync(ThumbnailMode.SingleItem, 1))
                {
                    if (dominantColour != null)
                    {
                        //Prepare thumbnail to display
                        var bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(dominantColour);

                        //Create a decoder for the image
                        var decoder = await BitmapDecoder.CreateAsync(dominantColour.CloneStream());

                        //Get the pixel provider
                        var pixels = await decoder.GetPixelDataAsync(
                            BitmapPixelFormat.Rgba8,
                            BitmapAlphaMode.Ignore,
                            new BitmapTransform(),
                            ExifOrientationMode.IgnoreExifOrientation,
                            ColorManagementMode.DoNotColorManage);

                        //Get the bytes of the 1x1 scaled image
                        var bytes = pixels.DetachPixelData();

                        //read the color 
                        var myDominantColor = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);

                        gradientStop1.Color = Color.FromArgb(100, bytes[0], bytes[1], bytes[2]);
                        gradientStop2.Color = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
                        ellipseRGB.Fill = new SolidColorBrush(myDominantColor);

                    }
                }

            }
        }

        private void Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(gridHoverCheck, 0, 1);
            ScaleImageUp.Begin();
        }

        private void Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(gridHoverCheck, 1, 0);
            ScaleImageDown.Begin();
        }
        
    }
}
