using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Scalemate.Models
{
    public class ImportedImage : INotifyPropertyChanged
    {

        private String address { get; set; }
        private BitmapImage thumbnail { get; set; }
        private BitmapImage includedImage { get; set; }
        private bool isSelected { get; set; }

        public String Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
            }
        }

        public BitmapImage IncludedImage
        {
            get { return includedImage; }
            set
            {
                includedImage = value;
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public async Task SetIncludedImageAsync(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                IncludedImage = new BitmapImage();
                IncludedImage.SetSource(await file.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView, 300));
                OnPropertyChanged("IncludedImage");

            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
