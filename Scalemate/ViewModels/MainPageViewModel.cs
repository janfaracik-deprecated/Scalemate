using GalaSoft.MvvmLight.Messaging;
using Scalemate.Models;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using System.Collections.Generic;
using Windows.Storage.FileProperties;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Shared.Helpers;
using System.Linq;

namespace Scalemate.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {

        public ObservableCollection<ImportedImage> ImageList { get; set; }
        public ObservableCollection<ExportItem> ExportList { get; set; }

        public MainPageViewModel()
        {
            ImageList = new ObservableCollection<ImportedImage>();
            ExportList = new ObservableCollection<ExportItem>();
            ExportList.Add(new ExportItem());
        }

        #region Import

        public async void BrowseImages()
        {

            if (ImageList.Count == 0)
            {

                //Launch a FileOpenPicker and add necessary file formats

                FileOpenPicker fileOpenPicker = new FileOpenPicker()
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                fileOpenPicker.FileTypeFilter.Add(".tiff");

                //Open the files

                TryImportStorageItems(await fileOpenPicker.PickMultipleFilesAsync());

            }
            else
            {
                ImageList.Clear();
            }

        }

        private async void TryImportStorageItems(IReadOnlyList<IStorageItem> storageItems)
        {

            Debug.WriteLine("Using the new TryImportStorageItems method");

            //Checks if files is empty, if not continue

            if (storageItems.Count == 0)
            {
                return;
            }

            //Loop through the files

            foreach (IStorageItem iStorageItem in storageItems)
            {
                if (iStorageItem.IsOfType(StorageItemTypes.File))
                {
                    TryImportImage((StorageFile)iStorageItem);
                }
                if (iStorageItem.IsOfType(StorageItemTypes.Folder))
                {

                    StorageFolder s = (StorageFolder)iStorageItem;

                    foreach (IStorageItem i2 in await s.GetFilesAsync())
                    {
                        if (i2.IsOfType(StorageItemTypes.File))
                        {
                            TryImportImage((StorageFile)i2);
                        }
                    }

                }
            }

        }

        private void TryImportImage(StorageFile sf)
        {
            if (sf.Path.ToLower().EndsWith(".jpg") || sf.Path.ToLower().EndsWith(".jpeg") || sf.Path.ToLower().EndsWith(".png") || sf.Path.ToLower().EndsWith(".bmp") || sf.Path.ToLower().EndsWith(".tiff"))
            {
                ImportedImage ii = new ImportedImage { Address = sf.Path, LinkedFile = sf };
                if (ImageList.FirstOrDefault(s => s.Address == ii.Address) == null)
                {
                    ImageList.Add(ii);
                    ii.Index = ImageList.IndexOf(ii);
                }
            }
        }

        #endregion

        #region Export

        public async void Export()
        {

            FolderPicker folderPicker = new FolderPicker()
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

                Messenger.Default.Send("Flip");

                foreach (ExportItem exportItem in ExportList)
                {
                    ExportExportItem(exportItem, pfolder);
                }

            }

        }

        private async void ExportExportItem(ExportItem exportItem, StorageFolder storageFolder)
        {

            //Set progress value of the ExportItem

            exportItem.ProgressValue = 0;
            exportItem.ProgressMax = ImageList.Count;

            //Create a new Storage Folder for the Export

            StorageFolder storageFolder2 = await storageFolder.CreateFolderAsync(exportItem.Title, CreationCollisionOption.OpenIfExists);
            exportItem.Folder = storageFolder2;
            StorageApplicationPermissions.FutureAccessList.Add(storageFolder2);

            //Loop through the ImageList and export the image at the specified dimensions

            foreach (ImportedImage importedImage in ImageList)
            {

                Size newSize;

                if (exportItem.PercentageChecked)
                {
                    double correctPercentage = Convert.ToDouble(exportItem.Percentage) / 100;
                    uint aspectWidth = (uint)Math.Floor(importedImage.ImageProps.Width * correctPercentage);
                    uint aspectHeight = (uint)Math.Floor(importedImage.ImageProps.Height * correctPercentage);

                    newSize = new Size(Convert.ToDouble(aspectWidth), Convert.ToDouble(aspectHeight));
                }
                if (exportItem.SpecificSizeChecked)
                {
                    if (exportItem.ConstrainProportions)
                    {

                        float scaleWidth = (float)Convert.ToDouble(exportItem.Width) / importedImage.ImageProps.Width;
                        float scaleHeight = (float)Convert.ToDouble(exportItem.Height) / importedImage.ImageProps.Height;
                        float scale = Math.Min(scaleHeight, scaleWidth);

                        newSize = new Size((int)(importedImage.ImageProps.Width * scale), (int)(importedImage.ImageProps.Height * scale));
                    }
                    else
                    {
                        newSize = new Size(Convert.ToDouble(exportItem.Width), Convert.ToDouble(exportItem.Height));
                    }
                }

                try
                {
                    ExportImage(importedImage, await storageFolder2.CreateFileAsync(importedImage.LinkedFile.Name, CreationCollisionOption.ReplaceExisting), newSize);
                    exportItem.ProgressValue++;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            }

        }

        private async void ExportImage(ImportedImage importedImage, StorageFile resizedImage, Size newSize)
        {

            var imageStream = await importedImage.LinkedFile.OpenReadAsync();

            //Resize and Save

            var fileEncoder = BitmapEncoder.JpegEncoderId;

            switch (importedImage.LinkedFile.Path.Substring(importedImage.LinkedFile.Path.Length - 4, 4).ToLower())
            {
                case ".png":
                    fileEncoder = BitmapEncoder.PngEncoderId;
                    break;
                case "tiff":
                    fileEncoder = BitmapEncoder.TiffEncoderId;
                    break;
                case ".bmp":
                    fileEncoder = BitmapEncoder.BmpEncoderId;
                    break;
                default:
                    fileEncoder = BitmapEncoder.JpegEncoderId;
                    break;
            }

            using (var sourceStream = await importedImage.LinkedFile.OpenAsync(FileAccessMode.Read))
            {

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);

                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledWidth = (uint)newSize.Width,
                    ScaledHeight = (uint)newSize.Height,
                    InterpolationMode = BitmapInterpolationMode.Fant
                };

                if (!(importedImage.ImageProps.Orientation == PhotoOrientation.Normal || importedImage.ImageProps.Orientation == PhotoOrientation.Unspecified || importedImage.ImageProps.Orientation == PhotoOrientation.FlipVertical || importedImage.ImageProps.Orientation == PhotoOrientation.Rotate180))
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

        }

        public void AddExport()
        {

            //Add a new ExportItem to the ExportList

            ExportList.Add(new ExportItem());

            //Rename existing ExportItems in accordance to their index

            foreach (ExportItem exportItem in ExportList)
            {
                exportItem.Title = "Export " + (ExportList.IndexOf(exportItem) + 1);
            }

        }

        public void RemoveExport(ExportItem exportItem)
        {

            ExportList.Remove(exportItem);

            //Rename existing ExportItems in accordance to their index

            foreach (ExportItem exportItem2 in ExportList)
            {
                exportItem2.Title = "Export " + (ExportList.IndexOf(exportItem2) + 1);
            }

        }

        private void UpdateCurrentFolder(StorageFolder storageFolder)
        {
            SettingsHelper.UpdateSetting("CurrentFolder", storageFolder.Path, false);
        }

        #endregion

        #region Manage Images

        public void Delete(ImportedImage image)
        {
            ImageList.Remove(image);
            // Messenger.Default.Send("TryShowStartUI");
        }

        public void Delete(IList<object> itemsToDelete)
        {

            foreach (ImportedImage ii in itemsToDelete)
            {
                ImageList.Remove(ii);
            }

            //   Messenger.Default.Send("TryShowStartUI");

        }

        #endregion

        #region Overflow

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