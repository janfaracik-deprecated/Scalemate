using GalaSoft.MvvmLight.Messaging;
using Scalemate.ViewModels;
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

namespace Scalemate.Views
{
    public sealed partial class ImagesView : UserControl
    {

        MainPageViewModel mainPageViewModel;

        public ImagesView()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainPageViewModel = DataContext as MainPageViewModel;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send("ShowExportView");
        }

        private void gridviewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void gridviewImages_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
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

        private void gridviewImages_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

    }
}
