using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Scalemate.Models
{
    public class ImportedImages : INotifyPropertyChanged
    {

        private ObservableCollection<ImportedImage> imageList = new ObservableCollection<ImportedImage>();

        public ObservableCollection<ImportedImage> ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
                OnPropertyChanged();
            }
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
            ImageList.Remove(image);
        }

        public void Delete(IList<object> itemsToDelete)
        {
            foreach (ImportedImage ii in itemsToDelete)
            {
                ImageList.Remove(ii);
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
