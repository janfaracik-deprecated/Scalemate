using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scalemate.Models
{
    public class ImportedImages : INotifyPropertyChanged
    {

        public ObservableCollection<ImportedImage> ImageList = new ObservableCollection<ImportedImage>();
        
        public ImportedImages()
        {
            ImageList = new ObservableCollection<ImportedImage>();
        }

        public void Add(ImportedImage image)
        {
            if (!ImageList.Contains(image))
            {
                ImageList.Add(image);
            }
        }

        public void Delete(ImportedImage image)
        {
            if (ImageList.Contains(image))
            {
                ImageList.Remove(image);
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
