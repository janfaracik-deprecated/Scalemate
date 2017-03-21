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

namespace Scalemate.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {

        public ImportedImages Images = new ImportedImages();

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

            //using (imageStream)
            //{
            //    using (var resizedStream = await resizedImage.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

            //        BitmapTransform transform = new BitmapTransform()
            //        {
            //            InterpolationMode = BitmapInterpolationMode.Fant,
            //            ScaledWidth = (uint)newSize.Width,
            //            ScaledHeight = (uint)newSize.Height
            //        };

            //        PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

            //        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width, (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());

            //        await encoder.FlushAsync();
            //    }
            //}

            var fileEncoder = BitmapEncoder.JpegEncoderId;

            Debug.WriteLine(linkedFile.Path.Substring(linkedFile.Path.Length - 4, 4));
            
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

            using (imageStream)
            {

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledHeight = (uint)newSize.Width,
                    ScaledWidth = (uint)newSize.Height,
                    InterpolationMode = BitmapInterpolationMode.Fant
                };

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

                using (var resizedStream = await resizedImage.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(fileEncoder, resizedStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width, (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }

            }

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
                ProgressMax = Images.ImageList.Count;

                foreach (ImportedImage image in Images.ImageList)
                {

                    Size newSize;

                    if (PercentageChecked)
                    {

                        BitmapImage bitmapImage = new BitmapImage();

                        await bitmapImage.SetSourceAsync(await image.LinkedFile.OpenReadAsync());

                        double correctPercentage = Convert.ToDouble(Percentage) / 100;
                        uint aspectWidth = (uint)Math.Floor(bitmapImage.PixelWidth * correctPercentage);
                        uint aspectHeight = (uint)Math.Floor(bitmapImage.PixelHeight * correctPercentage);

                        Debug.WriteLine("EXPORT");
                        Debug.WriteLine("original width: " + bitmapImage.PixelWidth);
                        Debug.WriteLine("original height: " + bitmapImage.PixelHeight);
                        Debug.WriteLine("multiply by: " + correctPercentage);
                        Debug.WriteLine("aspectWidth: " + aspectWidth);
                        Debug.WriteLine("aspectHeight: " + aspectHeight);

                        newSize = new Size(Convert.ToDouble(aspectWidth), Convert.ToDouble(aspectHeight));

                    }
                    else
                    {
                        if (ConstrainProportions == true)
                        {
                            newSize = new Size(Convert.ToDouble(Width), Convert.ToDouble(Height));
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

    }
}
