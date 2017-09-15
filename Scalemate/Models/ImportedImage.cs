using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;

namespace Scalemate.Models
{
    public class ImportedImage : INotifyPropertyChanged
    {

        public String Address { get; set; }
        public StorageFile LinkedFile { get; set; }
        public ImageProperties ImageProps { get; set; }
        public int Index = 0;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
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