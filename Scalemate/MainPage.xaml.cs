using GalaSoft.MvvmLight.Messaging;
using Scalemate.Helpers;
using Scalemate.Models;
using Scalemate.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        #region Load

        public MainPage()
        {

            this.InitializeComponent();

            mainPageViewModel = new MainPageViewModel();

            DataContext = mainPageViewModel;

            Window.Current.CoreWindow.Activated += (sender, args) =>
            {
                if (args.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                {
                    // buttonHelp.Opacity = .5;
                    textblockLogo.Opacity = .5;
                    checkboxDone.Opacity = .5;
                }
                else
                {
                    //buttonHelp.Opacity = 1;
                    textblockLogo.Opacity = 1;
                    checkboxDone.Opacity = 1;
                }
            };

            //Adds event handler in order to set proper margins for TitleBar buttons

            titleBarHelper.coreTitleBar.LayoutMetricsChanged += OnLayoutMetricsChanged;

            Window.Current.SetTitleBar(gridTitleBarBackground);

            dropdownMenu.isOpenChanged += DropdownMenu_isOpenChanged;

            visualAll = ElementCompositionPreview.GetElementVisual(gridTitleBar);
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
            AnimateIn.Begin();
            Page_SizeChanged(this, null);
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
                    dropdownMenu.Close();
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

        private void OnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {

            //Sets margin on buttons in TitleBar

            checkboxOverflow.Margin = new Thickness(titleBarHelper.coreTitleBar.SystemOverlayLeftInset, 0, 0, 0);
            checkboxDone.Margin = new Thickness(0, 0, titleBarHelper.coreTitleBar.SystemOverlayRightInset, 0);
            rectangleTitleBarSeparator.Margin = new Thickness(0, 0, titleBarHelper.coreTitleBar.SystemOverlayRightInset - 25, 0);
            gridSelected.Margin = new Thickness(0, 0, titleBarHelper.coreTitleBar.SystemOverlayRightInset - 25, 0);

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth < 1200)
            {
                textblockLogo.Margin = new Thickness(0, 0, Math.Min((1200 - ActualWidth) / 4, titleBarHelper.coreTitleBar.SystemOverlayRightInset), 0);
            }
            else if (ActualWidth >= 1200)
            {
                textblockLogo.Margin = new Thickness(0);
            }
        }

        private void sliderImageSize_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            UpdateItemSize();
        }

        private void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateItemSize();
        }

        private void UpdateItemSize()
        {
            ItemsWrapGrid itemsWrapGrid = (ItemsWrapGrid)gridviewImages.ItemsPanelRoot;

            if (itemsWrapGrid != null)
            {
                double optimizedWidth = sliderImageSize.Value * 10;
                double number = (int)itemsWrapGrid.ActualWidth / (int)optimizedWidth;
                double newSize = itemsWrapGrid.ActualWidth / number;

                if (itemsWrapGrid != null)
                {
                    itemsWrapGrid.ItemWidth = (int)newSize;
                    itemsWrapGrid.ItemHeight = (int)newSize;
                }
            }

        }

        #endregion

        #region Add Images

        private async void AddImagesFromFolderAsync(object sender, RoutedEventArgs e)
        {

            dropdownMenu.Close();

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
            var files = await picker.PickMultipleFilesAsync();

            if (files.Count > 0)
            {

                if (stackpanelImport.IsHitTestVisible == true)
                {
                    stackpanelImport.IsHitTestVisible = false;
                    Animationmate.ChangeObjectOpacity(stackpanelImport, 1, 0, 100);
                    gridTitleBarButtons.Visibility = Visibility.Visible;
                    stackpanelImport.Visibility = Visibility.Collapsed;
                    Animationmate.ChangeObjectOpacity(gridTitleBarButtons, 0, 1);
                    gridTitleBarButtons.IsHitTestVisible = true;
                    Animationmate.ChangeObjectOpacity(gridTitleBarBackgroundInner, 0, 1);
                    gridviewImages.Visibility = Visibility.Visible;
                    Animationmate.ChangeObjectOpacity(imageTitleBarShadow, 0, 0.2);
                }

                foreach (StorageFile file in files)
                {
                    ImportedImage ii = new ImportedImage { Address = file.Path, LinkedFile = file};
                    if (mainPageViewModel.Images.ImageList.FirstOrDefault(s => s.Address == ii.Address) == null)
                    {
                        mainPageViewModel.Images.ImageList.Add(ii);
                        ii.Index = mainPageViewModel.Images.ImageList.IndexOf(ii);
                    }
                }

                Animationmate.ChangeObjectOpacity(stackpanelImageCount, stackpanelImageCount.Opacity, 0.7, 250, 150);

            }

        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            FadeOutDrop.Begin();

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var files = await e.DataView.GetStorageItemsAsync();
                if (files.Count > 0)
                {
                    if (stackpanelImport.IsHitTestVisible == true)
                    {
                        stackpanelImport.IsHitTestVisible = false;
                        Animationmate.ChangeObjectOpacity(stackpanelImport, 1, 0, 100);
                        gridTitleBarButtons.Visibility = Visibility.Visible;
                        stackpanelImport.Visibility = Visibility.Collapsed;
                        Animationmate.ChangeObjectOpacity(gridTitleBarButtons, 0, 1);
                        gridTitleBarButtons.IsHitTestVisible = true;
                        Animationmate.ChangeObjectOpacity(gridTitleBarBackgroundInner, 0, 1);
                        Animationmate.ChangeObjectOpacity(imageTitleBarShadow, 0, 0.2);
                        gridviewImages.Visibility = Visibility.Visible;
                    }

                    foreach (StorageFile file in files)
                    {
                        if (file.Path.ToLower().EndsWith(".jpg") || file.Path.ToLower().EndsWith(".jpeg") || file.Path.ToLower().EndsWith(".png") || file.Path.ToLower().EndsWith(".bmp") || file.Path.ToLower().EndsWith(".tiff"))
                        {
                            ImportedImage ii = new ImportedImage { Address = file.Path, LinkedFile = file };
                            if (mainPageViewModel.Images.ImageList.FirstOrDefault(s => s.Address == ii.Address) == null)
                            {
                                mainPageViewModel.Images.ImageList.Add(ii);
                                ii.Index = mainPageViewModel.Images.ImageList.IndexOf(ii);
                            }
                        }
                    }

                    Animationmate.ChangeObjectOpacity(stackpanelImageCount, stackpanelImageCount.Opacity, 0.7, 250, 150);
                }
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

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            FadeInDrop.Begin();
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            FadeOutDrop.Begin();
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
            AnimateInSelectBar.Begin();
        }

        public void HideGridSelected()
        {
            AnimateOutSelectBar.Begin();
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
            gridviewImages.SelectedIndex = -1;
            foreach (ImportedImage ii in gridviewImages.Items)
            {
                gridviewImages.SelectedItems.Add(ii);
            }
        }

        private void buttonRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            mainPageViewModel.Images.Delete(gridviewImages.SelectedItems.ToList());
        }

        #endregion

        #region Popups

        private void DropdownMenu_isOpenChanged(object sender, EventArgs e)
        {
            if (!dropdownMenu.IsOpen)
            {
                checkboxOverflow.IsChecked = false;
                checkboxDone.IsChecked = false;
            }
        }

        private void checkboxOverflow_Click(object sender, RoutedEventArgs e)
        {
            stackpanelOverflow.Visibility = Visibility.Visible;
            stackpanelSave.Visibility = Visibility.Collapsed;
            dropdownMenu.InvertOpenState(checkboxOverflow, new Thickness(0, 48, 0, 0));
        }

        private void checkboxDone_Click(object sender, RoutedEventArgs e)
        {
            stackpanelOverflow.Visibility = Visibility.Collapsed;
            stackpanelSave.Visibility = Visibility.Visible;
            dropdownMenu.InvertOpenState(checkboxDone, new Thickness(0, 48, 0, 0));
        }

        #endregion

        #region Overflow

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            popupControl.Show();
            dropdownMenu.Close();
        }

        #endregion

        #region Save Interface

        private void TryShowStartUI()
        {
            if (mainPageViewModel.Images.ImageList.Count == 0)
            {
                gridContainer.AllowDrop = true;
                stackpanelImport.Visibility = Visibility.Visible;
                stackpanelImport.Opacity = 1;
                gridSave.Visibility = Visibility.Collapsed;
                gridTitleBarBackgroundInner.Opacity = 0;
                gridviewImages.Visibility = Visibility.Collapsed;
                stackpanelImport.IsHitTestVisible = true;
                imageTitleBarShadow.Opacity = 0;
                AnimateIn.Begin();
                Animationmate.ChangeObjectOpacity(gridTitleBarButtons, 1, 0, 100);
                gridTitleBarButtons.IsHitTestVisible = false;
            }
        }

        private void buttonImportMoreImages_Click(object sender, RoutedEventArgs e)
        {
            mainPageViewModel.Images.ImageList.Clear();
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
            mainPageViewModel.Images.Delete(ii);
        }

        #endregion

        #region Validation

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+([.]\\d)?"))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void ValidateTextboxes(object sender, TextChangedEventArgs e)
        {

            buttonSave.IsEnabled = false;

            try
            {
                Convert.ToDouble(textboxPercentage.Text);
                Convert.ToDouble(textboxWidth.Text);
                Convert.ToDouble(textboxHeight.Text);

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
                mainPageViewModel.Images.Delete(gridviewImages.SelectedItems.ToList());
            }
        }

        #endregion

    }
}
