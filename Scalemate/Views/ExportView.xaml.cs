using Scalemate.Controls;
using Scalemate.Models;
using Scalemate.ViewModels;
using Shared.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Scalemate.Views
{
    public sealed partial class ExportView : UserControl
    {

        MainPageViewModel mainPageViewModel;

        public ExportView()
        {
            this.InitializeComponent();
        }

        #region Public Methods

        public void AnimateIn()
        {
            AnimateInStoryboard.Begin();
        }

        public void AnimateOut()
        {
            AnimateOutStoryboard.Begin();
        }
        
        public void FlipCards()
        {
            foreach (ExportItem exportItem in gridViewExportItems.Items)
            {
                if (gridViewExportItems.ContainerFromItem(exportItem) is GridViewItem container)
                {
                    var templateRoot = (FrameworkElement)container.ContentTemplateRoot;
                    ExportControl exportControl = templateRoot.FindName("exportControl") as ExportControl;
                    Button buttonRemoveExportItem = templateRoot.FindName("buttonRemoveExportItem") as Button;
                    exportControl.Flip(gridViewExportItems.Items.IndexOf(exportItem));
                    buttonRemoveExportItem.Visibility = Visibility.Collapsed;
                }
            }

            scrollViewerExportItems.ChangeView(0, null, null);
            buttonAddExport.IsEnabled = false;
            AnimationHelper.ChangeObjectOpacity(buttonExportAll, 1, 0);
            buttonExportAll.IsHitTestVisible = false;
        }

        #endregion

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainPageViewModel = DataContext as MainPageViewModel;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double xPadding = (ActualWidth - 304) / 2;
            stackPanelExportItems.Padding = new Thickness(xPadding, 0, xPadding, 0);
        }

        private void buttonAddExport_Click(object sender, RoutedEventArgs e)
        {
            scrollViewerExportItems.ChangeView(scrollViewerExportItems.ScrollableWidth, null, null);
            mainPageViewModel.AddExport();
        }

        private void scrollViewerExportItems_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            // Checks if the scrollviewer is still scrolling, if not, then continue

            if (!e.IsIntermediate)
            {

                // Gets the horizontal offset of the scrollviewer

                double targetNumber = scrollViewerExportItems.HorizontalOffset;

                // Create an array based on multiples of 334 (the ExportControl width + margin)

                int[] array = new int[10];

                for (int i = 0; i < 10; i++)
                {
                    array[i] = 336 * i;
                }

                // Find the nearest value in the array to the horizontal offset of the scrollviewer and scroll to that position

                var nearest = array.OrderBy(v => Math.Abs(v - targetNumber)).First();

                scrollViewerExportItems.ChangeView(nearest, null, null);

            }

        }

        private void buttonRemoveExportItem_Click(object sender, RoutedEventArgs e)
        {
            if (mainPageViewModel.ExportList.Count == 1)
            {
                AnimateOut();
            }
            else
            {
                Button button = sender as Button;
                mainPageViewModel.RemoveExport(button.DataContext as ExportItem);
                scrollViewerExportItems.ChangeView(scrollViewerExportItems.ScrollableWidth - 666, null, null);
            }
        }

        private void buttonExportAll_Click(object sender, RoutedEventArgs e)
        {
            mainPageViewModel.Export();
        }

    }
}