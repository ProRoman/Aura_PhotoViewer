using System.Windows.Controls;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel;
using Microsoft.Practices.Unity;
using System.Windows.Media;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.View
{
    /// <summary>
    /// Interaction logic for ContentAndNavigationView.xaml
    /// </summary>
    public partial class ContentAndNavigationView : UserControl
    {
        private ContentAndNavigationViewModel _contentAndNavigationViewModel;

        public ContentAndNavigationView()
        {
            InitializeComponent();
        }

        [InjectionMethod]
        public void Initialize(ContentAndNavigationViewModel contentAndNavigationViewModel)
        {
            _contentAndNavigationViewModel = contentAndNavigationViewModel;
            DataContext = _contentAndNavigationViewModel;
        }

        private void Viewbox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {            
            double zoom = e.Delta > 0 ? .2 : -.2;
            if (zoom < 0 && scaleTransform.ScaleX <= 1 && scaleTransform.ScaleY <= 1)
            {
                e.Handled = true;
                return;
            }
            scaleTransform.ScaleX += zoom;
            scaleTransform.ScaleY += zoom;
        }

    }
}
