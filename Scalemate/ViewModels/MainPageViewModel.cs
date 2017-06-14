using GalaSoft.MvvmLight.Messaging;
using Scalemate.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using Scalemate.Helpers;
using Windows.System;
using Windows.Storage.FileProperties;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;

namespace Scalemate.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {

        public ObservableCollection<ImportedImage> ImageList = new ObservableCollection<ImportedImage>();

        private bool percentageChecked;
        private bool specificSizeChecked;
        private String percentage;
        private String width;
        private String height;
        private bool constrainProportions;
        private int progressValue;
        private int progressMax;

        public bool PercentageChecked
        {
            get
            {
                return percentageChecked;
            }
            set
            {
                percentageChecked = value;
                OnPropertyChanged();
            }
        }

        public bool SpecificSizeChecked
        {
            get
            {
                return specificSizeChecked;
            }
            set
            {
                specificSizeChecked = value;
                OnPropertyChanged();
            }
        }

        public String Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = value;
                OnPropertyChanged();
            }
        }

        public String Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged();
            }
        }

        public String Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }

        public bool ConstrainProportions
        {
            get
            {
                return constrainProportions;
            }
            set
            {
                constrainProportions = value;
                OnPropertyChanged();
            }
        }

        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                OnPropertyChanged();
            }
        }

        public int ProgressMax
        {
            get
            {
                return progressMax;
            }
            set
            {
                progressMax = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            PercentageChecked = true;
            Percentage = "100";
            Width = "1280";
            Height = "720";
            ProgressValue = 0;
            ProgressMax = 0;
        }

        private void UpdateCurrentFolder(StorageFolder storageFolder)
        {
            SettingsHelper.UpdateSetting("CurrentFolder", storageFolder.Path, false);
        }

        public async void SaveImage(StorageFile linkedFile, StorageFile resizedImage, Size newSize)
        {

            var imageStream = await linkedFile.OpenReadAsync();

            //Resize and Save

            //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

            var fileEncoder = BitmapEncoder.JpegEncoderId;

            switch (linkedFile.Path.Substring(linkedFile.Path.Length - 4, 4).ToLower())
            {
                case ".png":
                    fileEncoder = BitmapEncoder.PngEncoderId;
                    break;
                case "tiff":
                    fileEncoder = BitmapEncoder.TiffEncoderId;
                    break;
                case "bmp":
                    fileEncoder = BitmapEncoder.BmpEncoderId;
                    break;
                default:
                    fileEncoder = BitmapEncoder.JpegEncoderId;
                    break;
            }

            using (var sourceStream = await linkedFile.OpenAsync(FileAccessMode.Read))
            {

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);

                ImageProperties props = await linkedFile.Properties.GetImagePropertiesAsync();

                //Debug.WriteLine(linkedFile.DisplayName + " " + props.Orientation);

                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledWidth = (uint)newSize.Width,
                    ScaledHeight = (uint)newSize.Height,
                    InterpolationMode = BitmapInterpolationMode.Fant
                };

                if (!(props.Orientation == PhotoOrientation.Normal || props.Orientation == PhotoOrientation.Unspecified || props.Orientation == PhotoOrientation.FlipVertical || props.Orientation == PhotoOrientation.Rotate180))
                {
                    transform.ScaledWidth = (uint)newSize.Height;
                    transform.ScaledHeight = (uint)newSize.Width;
                }

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

                using (var destinationStream = await resizedImage.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(fileEncoder, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width, (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }

            }

            //using (imageStream)
            //{
            //    using (var resizedStream = await resizedImage.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

            //        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
            //        encoder.BitmapTransform.ScaledWidth = (uint)newSize.Width;
            //        encoder.BitmapTransform.ScaledHeight = (uint)newSize.Height;

            //        BitmapTransform transform = new BitmapTransform()
            //        {
            //            ScaledHeight = (uint)newSize.Width,
            //            ScaledWidth = (uint)newSize.Height,
            //            InterpolationMode = BitmapInterpolationMode.Fant
            //        };

            //        PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

            //        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width, (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());

            //        await encoder.FlushAsync();
            //    }
            //}



            //using (imageStream)
            //{

            //    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

            //    BitmapTransform transform = new BitmapTransform()
            //    {
            //        ScaledWidth = (uint)newSize.Width,
            //        ScaledHeight = (uint)newSize.Height,
            //        InterpolationMode = BitmapInterpolationMode.Fant
            //    };

            //    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

            //    using (var resizedStream = await resizedImage.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(fileEncoder, resizedStream);
            //        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width, (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());
            //        await encoder.FlushAsync();
            //    }

            //}

        }

        public async void Save()
        {

            var folderPicker = new FolderPicker()
            {
                CommitButtonText = "Export",
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            var pfolder = await folderPicker.PickSingleFolderAsync();

            if (pfolder != null)
            {

                StorageApplicationPermissions.FutureAccessList.Add(pfolder);

                UpdateCurrentFolder(pfolder);

                Messenger.Default.Send("Export");
                ProgressValue = 0;
                ProgressMax = ImageList.Count;

                foreach (ImportedImage image in ImageList)
                {

                    Size newSize;

                    if (PercentageChecked)
                    {

                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(await image.LinkedFile.OpenReadAsync());

                        double correctPercentage = Convert.ToDouble(Percentage) / 100;
                        uint aspectWidth = (uint)Math.Floor(bitmapImage.PixelWidth * correctPercentage);
                        uint aspectHeight = (uint)Math.Floor(bitmapImage.PixelHeight * correctPercentage);

                        newSize = new Size(Convert.ToDouble(aspectWidth), Convert.ToDouble(aspectHeight));

                    }
                    else
                    {
                        if (ConstrainProportions)
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            await bitmapImage.SetSourceAsync(await image.LinkedFile.OpenReadAsync());

                            float scaleWidth = (float)Convert.ToDouble(Width) / bitmapImage.PixelWidth;
                            float scaleHeight = (float)Convert.ToDouble(Height) / bitmapImage.PixelHeight;
                            float scale = Math.Min(scaleHeight, scaleWidth);

                            newSize = new Size((int)(bitmapImage.PixelWidth * scale), (int)(bitmapImage.PixelHeight * scale));
                        }
                        else
                        {
                            newSize = new Size(Convert.ToDouble(Width), Convert.ToDouble(Height));
                        }
                    }

                    try
                    {
                        SaveImage(image.LinkedFile, await pfolder.CreateFileAsync(image.LinkedFile.Name, CreationCollisionOption.ReplaceExisting), newSize);

                        ProgressValue++;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                }

                Messenger.Default.Send("ExportComplete");

                NotificationHelper.ShowNotification();

            }

        }

        public async void OpenFolder()
        {
            await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(SettingsHelper.GetSetting("CurrentFolder", false).ToString()));
        }

        public void Delete(ImportedImage image)
        {
            ImageList.Remove(image);
            Messenger.Default.Send("TryShowStartUI");
        }

        public void Delete(IList<object> itemsToDelete)
        {

            foreach (ImportedImage ii in itemsToDelete)
            {
                ImageList.Remove(ii);
            }

            Messenger.Default.Send("TryShowStartUI");

        }

        #region About

        public async void About()
        {

            //Create Content

            StackPanel stackpanelContent = new StackPanel();
            TextBlock textblockTitle = new TextBlock();
            TextBlock textblockVersion = new TextBlock();

            textblockTitle.Style = Application.Current.Resources["TextBlockTitle"] as Style;
            textblockVersion.Style = Application.Current.Resources["TextBlockSubtitle"] as Style;

            stackpanelContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockTitle.HorizontalAlignment = HorizontalAlignment.Center;
            textblockVersion.HorizontalAlignment = HorizontalAlignment.Center;

            textblockVersion.Margin = new Thickness(0, 15, 0, 0);
            stackpanelContent.Padding = new Thickness(0, 90, 0, 90);

            stackpanelContent.Children.Add(textblockTitle);
            stackpanelContent.Children.Add(textblockVersion);

            PackageId packageId = Package.Current.Id;
            PackageVersion version = packageId.Version;

            textblockTitle.Text = Package.Current.DisplayName;
            textblockVersion.Text = string.Format("{0}.{1}", version.Major, version.Minor);

            var dialog = new ContentDialog()
            {
                Content = stackpanelContent,
                CloseButtonText = "Close Dialog"
            };

            //Show Dialog

            ContentDialogResult result = await dialog.ShowAsync();

        }

        #endregion

    }
}
