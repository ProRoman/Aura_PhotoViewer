using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class Picture : INotifyPropertyChanged
    {
        #region Private fields

        private string _imageUri;

        #endregion

        #region Presentation properties

        public string ImageUri
        {
            get { return _imageUri; }
            set
            {
                _imageUri = value;
                OnPropertyChanged();
            }
        }

        public double Angle { get; set; }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}