using System.Windows.Controls;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel;
using Microsoft.Practices.Unity;
using System.Windows.Media;
using System.Windows;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.View
{
    /// <summary>
    /// Interaction logic for ContentAndNavigationView.xaml
    /// </summary>
    public partial class ContentAndNavigationView : UserControl
    {
        private ContentAndNavigationViewModel _contentAndNavigationViewModel;
        private Point start;
        private Point origin;

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
            var position = e.GetPosition(vb);
            vb.RenderTransformOrigin = new Point(position.X / vb.ActualWidth, position.Y / vb.ActualHeight);

            scaleTransform.ScaleX += zoom;
            scaleTransform.ScaleY += zoom;
        }

        private void vb_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (vb.IsMouseCaptured) return;
            vb.CaptureMouse();

            start = e.GetPosition(vb);
            origin = vb.RenderTransformOrigin;
        }

        private void vb_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            vb.ReleaseMouseCapture();
        }

        private void vb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!vb.IsMouseCaptured) return;
            Point p = e.GetPosition(vb);

            Matrix m = translateTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            translateTransform.X = m.OffsetX;
            translateTransform.Y = m.OffsetY;
            vb.RenderTransformOrigin = new Point(p.X, p.Y);
        }

    }
}
