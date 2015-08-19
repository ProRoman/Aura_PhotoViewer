using System.Windows.Controls;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel;
using Microsoft.Practices.Unity;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.View
{
    /// <summary>
    /// Interaction logic for ContentAndNavigationView.xaml
    /// </summary>
    public partial class ContentAndNavigationView : UserControl
    {
        private IContentAndNavigationViewModel _contentAndNavigationViewModel;

        public ContentAndNavigationView()
        {
            InitializeComponent();
        }

        [InjectionMethod]
        public void Initialize(IContentAndNavigationViewModel contentAndNavigationViewModel)
        {
            _contentAndNavigationViewModel = contentAndNavigationViewModel;
            DataContext = _contentAndNavigationViewModel;
        } 
    }
}
