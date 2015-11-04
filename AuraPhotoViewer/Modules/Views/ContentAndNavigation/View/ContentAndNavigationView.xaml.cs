using System.Windows.Controls;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel;
using Microsoft.Practices.Unity;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

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

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {            
            double zoom = e.Delta > 0 ? .2 : -.2;
            if (zoom < 0 && scaleTransform.ScaleX <= 1 && scaleTransform.ScaleY <= 1)
            {
                return;
            }
            var position = e.GetPosition(vb);
            vb.RenderTransformOrigin = new Point(position.X / vb.ActualWidth, position.Y / vb.ActualHeight);

            scaleTransform.ScaleX += zoom;
            scaleTransform.ScaleY += zoom;
            //scaleTransform.CenterX += position.X / vb.ActualWidth;
            //scaleTransform.CenterY += position.Y / vb.ActualHeight;
            e.Handled = true;
        }

        private void vb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(vb);
           // vb.RenderTransformOrigin = new Point(position.X / vb.ActualWidth, position.Y / vb.ActualHeight);
            if (Border.IsMouseCaptured) return;

            start = e.GetPosition(vb);
            Border.CaptureMouse();
            origin.X = translateTransform.X;
            origin.Y = translateTransform.Y;
            e.Handled = true;
        }

        private void vb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void vb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Border.IsMouseCaptured) return;
            Point p = e.GetPosition(vb);

           // Matrix m = translateTransform.Value;
           // m.OffsetX = origin.X + (p.X - start.X);
            //m.OffsetY = origin.Y + (p.Y - start.Y);

            translateTransform.X += p.X - start.X;
            translateTransform.Y += p.Y - start.Y;
            //vb.RenderTransformOrigin = new Point(p.X / vb.ActualWidth, p.Y / vb.ActualHeight);
            e.Handled = true;
        }

    }
}
