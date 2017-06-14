using GalaSoft.MvvmLight.Messaging;
using Scalemate.Helpers;
using Scalemate.Models;
using Scalemate.ViewModels;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Scalemate
{
    public sealed partial class MainPage : Page
    {

        TitleBarHelper titleBarHelper = new TitleBarHelper();
        public MainPageViewModel mainPageViewModel { get; set; }

        Visual visualAll;
        Compositor compositor;
        bool shouldHide = true;

        #region Load

        public MainPage()
        {

            this.InitializeComponent();

            mainPageViewModel = new MainPageViewModel();

            DataContext = mainPageViewModel;

            //Adds event handler in order to set proper margins for TitleBar buttons

            visualAll = ElementCompositionPreview.GetElementVisual(gridContainer);
            compositor = visualAll.Compositor;

            Messenger.Default.Register<String>(this, (action) => ReceiveMessage(action));

        }

        internal void OnLaunchedEvent(string arguments)
        {
            switch (arguments)
            {
                case "importphotos":
                    AddImagesFromFolderAsync(null, null);
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateOOBE();
        }

        private void AnimateOOBE()
        {
            rectangleOR1.Width = 0;
            rectangleOR2.Width = 0;
            AnimateIn.Begin();
            Animationmate.ChangeObjectWidth(rectangleOR1, 0, 100, 500, 600);
            Animationmate.ChangeObjectWidth(rectangleOR2, 0, 100, 500, 600);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (e.Parameter)
            {
                case "importphotos":
                    AddImagesFromFolderAsync(null, null);
                    break;
            }
        }

        #endregion

        #region Messaging

        private void ReceiveMessage(object action)
        {
            switch (action.ToString())
            {
                case "Export":
                    gridSave.Visibility = Visibility.Visible;
                    stackpanelExporting.Visibility = Visibility.Visible;
                    stackpanelExportComplete.Visibility = Visibility.Collapsed;
                    Animationmate.ChangeObjectOpacity(gridSave, 0, 1, 150);
                    gridContainer.AllowDrop = false;
                    break;
                case "ExportComplete":
                    gridSave.Visibility = Visibility.Visible;
                    stackpanelExporting.Visibility = Visibility.Collapsed;
                    stackpanelExportComplete.Visibility = Visibility.Visible;
                    Animationmate.ChangeObjectOpacity(stackpanelExportComplete, 0, 1, 150);
                    break;
                case "TryShowStartUI":
                    TryShowStartUI();
                    break;
            }
        }

        #endregion

        #region SizeChanged

        private void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateItemSize();
        }

        private void UpdateItemSize()
        {
            ItemsWrapGrid itemsWrapGrid = (ItemsWrapGrid)gridviewImages.ItemsPanelRoot;

            if (itemsWrapGrid != null)
            {
                double optimizedWidth = 200;
                double number = (int)itemsWrapGrid.ActualWidth / (int)optimizedWidth;
                double newSize = itemsWrapGrid.ActualWidth / number;

                if (itemsWrapGrid != null)
                {
                    itemsWrapGrid.ItemWidth = (int)newSize;
                    itemsWrapGrid.ItemHeight = (int)newSize * 0.6;
                }
            }

        }

        #endregion

        #region Add Images

        private async void AddImagesFromFolderAsync(object sender, RoutedEventArgs e)
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".tiff");

            Import(await picker.PickMultipleFilesAsync());

        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                Import(await e.DataView.GetStorageItemsAsync());
            }
        }

        private void Grid_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {

        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
        }

        private async void Import(IReadOnlyList<IStorageItem> files)
        {

            Debug.WriteLine("Using the new import method!");

            //Checks if files is empty, if not continue

            if (files.Count > 0)
            {

                foreach (IStorageItem i in files)
                {

                    if (i.IsOfType(StorageItemTypes.File))
                    {
                        ImportPhotos((StorageFile)i);
                    }
                    if (i.IsOfType(StorageItemTypes.Folder))
                    {

                        StorageFolder s = (StorageFolder)i;

                        foreach (IStorageItem i2 in await s.GetFilesAsync())
                        {
                            if (i2.IsOfType(StorageItemTypes.File))
                            {
                                ImportPhotos((StorageFile)i2);
                            }
                        }

                    }
                }

                if (mainPageViewModel.ImageList.Count != 0)
                {

                    //Performs first time animation if need be

                    if (stackpanelImport.IsHitTestVisible == true)
                    {
                        stackpanelImport.IsHitTestVisible = false;

                        gridOOBE.Visibility = Visibility.Collapsed;
                        relativePanelContainer.Visibility = Visibility.Visible;
                        Animationmate.ChangeObjectOpacity(gridSidebarBG, 0, 1, 200);

                        AnimateInContent.Begin();

                    }
                }

            }
        }

        private void ImportPhotos(StorageFile sf)
        {
            if (sf.Path.ToLower().EndsWith(".jpg") || sf.Path.ToLower().EndsWith(".jpeg") || sf.Path.ToLower().EndsWith(".png") || sf.Path.ToLower().EndsWith(".bmp") || sf.Path.ToLower().EndsWith(".tiff"))
            {
                ImportedImage ii = new ImportedImage { Address = sf.Path, LinkedFile = sf };
                if (mainPageViewModel.ImageList.FirstOrDefault(s => s.Address == ii.Address) == null)
                {
                    mainPageViewModel.ImageList.Add(ii);
                    ii.Index = mainPageViewModel.ImageList.IndexOf(ii);
                }
            }
        }

        #endregion

        #region GridView Animations

        private void gridviewImages_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            var item = args.Item as ImportedImage;

            // Don't run an entrance animation if we're in recycling
            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }

            //args.Handled = true;
        }

        private Vector3 GetCenterPoint(GridViewItem itemcontainer, double itemsize)
        {
            var ttv = itemcontainer.TransformToVisual(Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            if (screenCoords.X < ActualWidth / 2)
            {
                return new Vector3((float)itemsize, (float)itemsize, 0);
            }
            else
            {
                return new Vector3(0, (float)itemsize, 0);
            }

        }

        private Vector3 GetCenterPoint2(GridViewItem itemcontainer, double itemsize)
        {
            var ttv = itemcontainer.TransformToVisual(Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            double width = ActualWidth;
            double itemX = screenCoords.X;
            double itemWidth = itemsize;

            double percentage = width / itemX;

            double percentageValue = itemsize / percentage;

            //Debug.WriteLine("---------------------------");
            //Debug.WriteLine("ITEMX: " + itemX);
            //Debug.WriteLine("itemWidth:" + itemsize);
            //Debug.WriteLine("percentage: " + percentage);
            //Debug.WriteLine("percentageValue: " + percentageValue);
            //Debug.WriteLine("center point: " + (itemsize - percentageValue));
            //Debug.WriteLine("---------------------------");

            return new Vector3((float)(itemsize - percentageValue), (float)itemsize, 0);

        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsWrapGrid)gridviewImages.ItemsPanelRoot;
            var itemContainer = (GridViewItem)sender;
            var itemIndex = gridviewImages.IndexFromContainer(itemContainer);

            var uc = itemContainer.ContentTemplateRoot as FrameworkElement;

            //Debug.WriteLine(itemContainer.ActualWidth);
            //Debug.WriteLine(itemContainer.ActualHeight);

            //Don't animate if we're not in the visible viewport
            if (itemIndex >= itemsPanel.FirstVisibleIndex && itemIndex <= itemsPanel.LastVisibleIndex)
            {
                var itemVisual = ElementCompositionPreview.GetElementVisual(uc);

                float width = (float)itemContainer.ActualWidth;
                float height = (float)itemContainer.ActualHeight;
                itemVisual.Size = new Vector2(width, height);
                itemVisual.CenterPoint = new Vector3(width / 2, height / 2, 1); //GetCenterPoint2(itemContainer, itemContainer.ActualWidth);
                itemVisual.Offset = new Vector3(0, 25, 0);
                itemVisual.Scale = new Vector3(1, 1, 0);

                var relativeIndex = itemIndex - itemsPanel.FirstVisibleIndex;

                // Create KeyFrameAnimations

                KeyFrameAnimation scaleXAnimation = compositor.CreateScalarKeyFrameAnimation();
                scaleXAnimation.InsertExpressionKeyFrame(1f, "0", compositor.CreateCubicBezierEasingFunction(new Vector2(0, 0), new Vector2(0.58f, 1)));
                scaleXAnimation.Duration = TimeSpan.FromMilliseconds(200);

                // Start animations
                itemVisual.StartAnimation("Offset.Y", scaleXAnimation);
            }

            itemContainer.Loaded -= ItemContainer_Loaded;
        }

        #endregion

        #region Select Bar

        public void ShowGridSelected()
        {
            if (stackpanelImageCount.Opacity == 1)
            {
                AnimateInSelectBar.Begin();
            }
        }

        public void HideGridSelected()
        {
            if (shouldHide)
            {
                AnimateOutSelectBar.Begin();
            }
            shouldHide = true;
        }

        private void gridviewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (ImportedImage ii in gridviewImages.Items)
            {
                if (gridviewImages.SelectedItems.Contains(ii))
                {
                    ii.IsSelected = true;
                }
                else
                {
                    ii.IsSelected = false;
                }
            }

            if (gridviewImages.SelectedItems.Count == 0)
            {
                HideGridSelected();
            }
            else
            {
                ShowGridSelected();
                textblockItemsSelected.Text = gridviewImages.SelectedItems.Count + " Selected";
            }

        }

        private void buttonDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            gridviewImages.SelectedIndex = -1;
        }

        private void buttonInverseSelection_Click(object sender, RoutedEventArgs e)
        {

            shouldHide = false;

            List<ImportedImage> newList = new List<ImportedImage>();

            foreach (ImportedImage ii in gridviewImages.Items)
            {
                if (!ii.IsSelected)
                {
                    newList.Add(ii);
                }
            }

            gridviewImages.SelectedIndex = -1;

            foreach (ImportedImage ii2 in newList)
            {
                gridviewImages.SelectedItems.Add(ii2);
            }

        }

        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            shouldHide = false;
            gridviewImages.SelectedIndex = -1;
            foreach (ImportedImage ii in gridviewImages.Items)
            {
                gridviewImages.SelectedItems.Add(ii);
            }
        }

        private void buttonRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            mainPageViewModel.Delete(gridviewImages.SelectedItems.ToList());
        }

        #endregion

        #region Save Interface

        private void TryShowStartUI()
        {
            if (mainPageViewModel.ImageList.Count == 0)
            {
                gridContainer.AllowDrop = true;
                stackpanelImport.Visibility = Visibility.Visible;
                stackpanelImport.Opacity = 1;
                gridSave.Visibility = Visibility.Collapsed;
                stackpanelImport.IsHitTestVisible = true;
                relativePanelContainer.Visibility = Visibility.Collapsed;
                gridOOBE.Visibility = Visibility.Visible;
                AnimateOOBE();
            }
        }

        private void buttonImportMoreImages_Click(object sender, RoutedEventArgs e)
        {
            mainPageViewModel.ImageList.Clear();
            TryShowStartUI();
        }

        #endregion

        #region Context Menu

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private async void menuFlyoutShowInFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            ImportedImage ii = (ImportedImage)(e.OriginalSource as FrameworkElement).DataContext;
            await Launcher.LaunchFileAsync(ii.LinkedFile);
        }

        private void menuFlyoutRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            ImportedImage ii = (ImportedImage)(e.OriginalSource as FrameworkElement).DataContext;
            mainPageViewModel.Delete(ii);
        }

        #endregion

        #region Validation

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Key.ToString(), "\\d+([.]\\d)?");
        }

        private void ValidateTextboxes(object sender, TextChangedEventArgs e)
        {

            buttonSave.IsEnabled = false;

            try
            {
                Double percentage = Convert.ToDouble(textboxPercentage.Text);
                Convert.ToDouble(textboxWidth.Text);
                Convert.ToDouble(textboxHeight.Text);

                if (percentage > 400)
                {
                    return;
                }

                buttonSave.IsEnabled = true;
            }
            catch
            {
                buttonSave.IsEnabled = false;
            }

        }

        #endregion

        #region Keyboard Shortcuts

        private void gridviewImages_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                e.Handled = true;
                mainPageViewModel.Delete(gridviewImages.SelectedItems.ToList());
            }
        }

        #endregion

        private void textboxPercentage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
