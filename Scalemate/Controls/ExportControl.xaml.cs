using Scalemate.Models;
using Shared.Helpers;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Scalemate.Controls
{
    public sealed partial class ExportControl : UserControl
    {

        public ExportControl()
        {
            this.InitializeComponent();
        }
        
        public void Flip(int delay)
        {
            AnimateInSaving.BeginTime = TimeSpan.FromMilliseconds(delay * 150);
            AnimateInSaving.Begin();
        }

        private void progressBar_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (progressBar.Value == progressBar.Maximum)
            {
                AnimationHelper.ChangeObjectTranslateY(stackPanelProgress, 0, -50, 200);
                AnimationHelper.ChangeObjectOpacity(stackPanelProgress, 1, 0, 200);
                AnimationHelper.ChangeObjectTranslateY(stackPanelExportComplete, 50, 0, 200);
                AnimationHelper.ChangeObjectOpacity(stackPanelExportComplete, 0, 1, 200);
            }
        }

        private async void buttonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            ExportItem exportItem = DataContext as ExportItem;
            await Launcher.LaunchFolderAsync(exportItem.Folder);
        }

    }
}
