using System;
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
            //Application.Current.MainWindow.StateChanged += MainWindowOnStateChanged;
        }

        [InjectionMethod]
        public void Initialize(ContentAndNavigationViewModel contentAndNavigationViewModel)
        {
            _contentAndNavigationViewModel = contentAndNavigationViewModel;
            DataContext = _contentAndNavigationViewModel;
        }

        /*private void MainWindowOnStateChanged(object sender, EventArgs e)
        {
            translateTransform.X = 0;
            translateTransform.Y = 0;
            /*Rect rect = new Rect(new Size(image.ActualWidth, image.ActualHeight));
            Rect bounds = image.TransformToAncestor(Border).TransformBounds(rect);
            if (Border.ActualWidth < bounds.Width)
            {
                if (bounds.Left > 0) // move left
                {
                    translateTransform.X += -bounds.Left;
                }
                if (bounds.Right < Border.ActualWidth) // move right
                {
                    translateTransform.X += Border.ActualWidth - bounds.Right;
                }
            }
            if (Border.ActualHeight < bounds.Height)
            {
                if (bounds.Top > 0) // move up
                {
                    translateTransform.Y += -bounds.Top;
                }
                if (bounds.Bottom < Border.ActualHeight) // move down
                {
                    translateTransform.Y += Border.ActualHeight - bounds.Bottom;
                }
                //translateTransform.Y += p.Y - start.Y;
            }*/
        /*}

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {            
            double zoom = e.Delta > 0 ? .2 : -.2;
            if (zoom < 0 && scaleTransform.ScaleX <= 1 && scaleTransform.ScaleY <= 1)
            {
                return;
            }
            //var position = e.GetPosition(vb);
           // vb.RenderTransformOrigin = new Point(position.X / vb.ActualWidth, position.Y / vb.ActualHeight);

            translateTransform.X = 0;
            translateTransform.Y = 0;
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

            // The following code gives you the bounds of the transformed Image control in coordinates relative to the Border control. 
            // You could easily check if the bounds are located inside the Border control.
            Rect rect = new Rect(new Size(image.ActualWidth, image.ActualHeight));
            Rect bounds = image.TransformToAncestor(Border).TransformBounds(rect);
            double offsetX = p.X - start.X;
            double offsetY = p.Y - start.Y;
            if (Border.ActualWidth < bounds.Width)
            {
                if (offsetX < 0 && bounds.Right > Border.ActualWidth) // move left
                {
                    translateTransform.X += offsetX;
                }
                if (offsetX > 0 && bounds.Left < 0) // move right
                {
                    translateTransform.X += offsetX;
                }
                //translateTransform.X += p.X - start.X;
            }
            if (Border.ActualHeight < bounds.Height)
            {
                if (offsetY < 0 && bounds.Bottom > Border.ActualHeight) // move up
                {
                    translateTransform.Y += offsetY;
                }
                if (offsetY > 0 && bounds.Top < 0) // move down
                {
                    translateTransform.Y += offsetY;
                }
                //translateTransform.Y += p.Y - start.Y;
            }
            e.Handled = true;
        }*/

    }
}
