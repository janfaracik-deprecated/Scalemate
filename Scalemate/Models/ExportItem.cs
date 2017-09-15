using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace Scalemate.Models
{
    public class ExportItem : INotifyPropertyChanged
    {
        private string _title = "Export";
        private bool _percentageChecked = true;
        private bool _specificSizeChecked = false;
        private int _percentage = 100;
        private int _width = 1280;
        private int _height = 720;
        private bool _constrainProportions = false;
        private int _progressValue = 0;
        private int _progressMax = 0;
        private StorageFolder _folder;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool PercentageChecked
        {
            get
            {
                return _percentageChecked;
            }
            set
            {
                _percentageChecked = value;
                OnPropertyChanged();
            }
        }

        public bool SpecificSizeChecked
        {
            get
            {
                return _specificSizeChecked;
            }
            set
            {
                _specificSizeChecked = value;
                OnPropertyChanged();
            }
        }

        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public bool ConstrainProportions
        {
            get
            {
                return _constrainProportions;
            }
            set
            {
                _constrainProportions = value;
                OnPropertyChanged();
            }
        }

        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public int ProgressMax
        {
            get
            {
                return _progressMax;
            }
            set
            {
                _progressMax = value;
                OnPropertyChanged();
            }
        }

        public StorageFolder Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
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