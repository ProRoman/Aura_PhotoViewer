using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;

namespace AuraPhotoViewer.Styles.Behaviors
{
    public class ZoomingPanningBorderBehavior : Behavior<Border>
    {
        private Point start;
        private Image image;
        private Grid parentGrid;

        public static readonly DependencyProperty IsZoomingOrPanningProperty = DependencyProperty.Register(
            "IsZoomingOrPanning",
            typeof (bool),
            typeof(ZoomingPanningBorderBehavior),
            new FrameworkPropertyMetadata(false)
            );        
        
        public bool IsZoomingOrPanning
        {
            get { return (bool)GetValue(IsZoomingOrPanningProperty); }
            set { SetValue(IsZoomingOrPanningProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            parentGrid = VisualTreeHelper.GetParent(AssociatedObject.Parent) as Grid;
            if (parentGrid != null)
            {
                parentGrid.MouseWheel += OnMouseWheel;
                parentGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
                parentGrid.MouseMove += OnMouseMove;
                parentGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
                parentGrid.AddHandler(Button.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            Application.Current.MainWindow.StateChanged += MainWindowOnStateChanged;
            image = GetImage();
            if (image != null)
            {
                image.TargetUpdated += ResetTransforms;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (parentGrid != null)
            {
                parentGrid.MouseWheel -= OnMouseWheel;
                parentGrid.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                parentGrid.MouseMove -= OnMouseMove;
                parentGrid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                parentGrid.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            Application.Current.MainWindow.StateChanged -= MainWindowOnStateChanged;
            image.TargetUpdated -= ResetTransforms;
        }

        private Image GetImage()
        {
            DependencyObject child = FindVisualChild<Image>(AssociatedObject);
            return (Image) child;
        }

        private void ResetTransforms(object sender, DataTransferEventArgs e)
        {
            if (e.Property.Name != "Source")
            {
                return;
            }
            ScaleTransform scale = (ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[0];
            TranslateTransform translate =
                (TranslateTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1];
            RotateTransform rotate = (RotateTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[0];
            translate.X = 0;
            translate.Y = 0;
            scale.ScaleX = 1;
            scale.ScaleY = 1;
            rotate.Angle = 0;
            IsZoomingOrPanning = false;
        }

        private bool CheckScaleLimit(double zoom, ScaleTransform scale, TranslateTransform translate)
        {
            if (zoom < 0 && scale.ScaleX <= 1 && scale.ScaleY <= 1)
            {
                translate.X = 0;
                translate.Y = 0;
                IsZoomingOrPanning = false;
                return true;
            }
            return false;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            double zoom = mouseWheelEventArgs.Delta > 0 ? .2 : -.2;
            ScaleTransform scale = (ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[0];
            TranslateTransform translate =
                (TranslateTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[1];
            if (CheckScaleLimit(zoom, scale, translate))
            {                
                return;
            }
            if (scale.ScaleX > 1 && (scale.ScaleX - 1) < .2)
            {
                zoom = zoom > 0 ? (scale.ScaleX - 1) : (1 - scale.ScaleX);
            }
            //Point position = mouseWheelEventArgs.GetPosition(AssociatedObject);
            //AssociatedObject.RenderTransformOrigin = new Point(position.X / AssociatedObject.ActualWidth, position.Y / AssociatedObject.ActualHeight);
            scale.ScaleX += zoom;
            scale.ScaleY += zoom;
            //AutoCorrectAfterScale(translate);
            IsZoomingOrPanning = true;
            CheckScaleLimit(zoom, scale, translate);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (AssociatedObject.IsMouseCaptured) return;
            start = mouseButtonEventArgs.GetPosition(AssociatedObject);
            AssociatedObject.CaptureMouse();
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!AssociatedObject.IsMouseCaptured) return;
            TranslateTransform translate =
                (TranslateTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[1];
            Point p = mouseEventArgs.GetPosition(AssociatedObject);
            Rect? bounds = GetImageBoundsRelativeToBorder();
            if (!bounds.HasValue)
            {
                return;
            }
            Rect boundsValue = bounds.Value;
            RotateTransform rotate = (RotateTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[0];
            Point newStartPoint = rotate.Transform(start);
            Point newEndPoint = rotate.Transform(p);
            double offsetX = newEndPoint.X - newStartPoint.X;
            double offsetY = newEndPoint.Y - newStartPoint.Y;
            // Check if the bounds are located inside the Border control.
            if (AssociatedObject.ActualWidth < boundsValue.Width)
            {
                if (offsetX < 0 && boundsValue.Right > AssociatedObject.ActualWidth) // move left
                {
                    translate.X += offsetX;
                }
                if (offsetX > 0 && boundsValue.Left < 0) // move right
                {
                    translate.X += offsetX;
                }
            }
            if (AssociatedObject.ActualHeight < boundsValue.Height)
            {
                if (offsetY < 0 && boundsValue.Bottom > AssociatedObject.ActualHeight) // move up
                {
                    translate.Y += offsetY;
                }
                if (offsetY > 0 && boundsValue.Top < 0) // move down
                {
                    translate.Y += offsetY;
                }
            }
            AutoCorrectAfterTranslate(translate, offsetX, offsetY);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            AssociatedObject.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
        }
        
        private Rect? GetImageBoundsRelativeToBorder()
        {            
            if (image != null)
            {
                // The bounds of the transformed Image control in coordinates relative to the Border control.
                Rect rect = new Rect(new Size(image.ActualWidth, image.ActualHeight));
                return image.TransformToAncestor((Border)AssociatedObject.Parent).TransformBounds(rect);
            }
            return null;
        }

        private void AutoCorrectAfterTranslate(TranslateTransform translate, double offsetX, double offsetY)
        {
            Rect? bounds = GetImageBoundsRelativeToBorder();
            if (!bounds.HasValue)
            {
                return;
            }
            Rect boundsValue = bounds.Value;
            if (AssociatedObject.ActualWidth <= boundsValue.Width)
            {
                if (offsetX <= 0 && boundsValue.Right <= AssociatedObject.ActualWidth) // move left
                {
                    translate.X += AssociatedObject.ActualWidth - boundsValue.Right; // move right
                }
                if (offsetX >= 0 && boundsValue.Left >= 0) // move right
                {
                    translate.X += -boundsValue.Left; // move left
                }
            }
            if (AssociatedObject.ActualHeight <= boundsValue.Height)
            {
                if (offsetY <= 0 && boundsValue.Bottom <= AssociatedObject.ActualHeight) // move up
                {
                    translate.Y += AssociatedObject.ActualHeight - boundsValue.Bottom; // move down
                }
                if (offsetY >= 0 && boundsValue.Top >= 0) // move down
                {
                    translate.Y += -boundsValue.Top; // move up
                }
            }
        }

        private void AutoCorrectAfterScale(TranslateTransform translate)
        {
            Rect? bounds = GetImageBoundsRelativeToBorder();
            if (!bounds.HasValue)
            {
                return;
            }
            Rect boundsValue = bounds.Value;
            if (AssociatedObject.ActualWidth <= boundsValue.Width)
            {
                if (boundsValue.Right < AssociatedObject.ActualWidth)
                {
                    translate.X += AssociatedObject.ActualWidth - boundsValue.Right; // move right
                }
                if (boundsValue.Left > 0)
                {
                    translate.X += -boundsValue.Left; // move left
                }
            }
            if (AssociatedObject.ActualHeight <= boundsValue.Height)
            {
                if (boundsValue.Bottom < AssociatedObject.ActualHeight)
                {
                    translate.Y += AssociatedObject.ActualHeight - boundsValue.Bottom; // move down
                }
                if (boundsValue.Top > 0)
                {
                    translate.Y += -boundsValue.Top; // move up
                }
            }
        }

        private void OnRotateClick(object sender, RoutedEventArgs e)
        {
            string tag = (string)((FrameworkElement)e.OriginalSource).Tag;
            RotateTransform rotate = (RotateTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[0];
            if (tag == "CounterClockwiseRotateButton")
            {
                rotate.Angle = rotate.Angle - 90;
            }
            else if (tag == "ClockwiseRotateButton")
            {
                rotate.Angle = rotate.Angle + 90;
            }
        }

        private void MainWindowOnStateChanged(object sender, EventArgs e)
        {
            AssociatedObject.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    TranslateTransform translate =
                        (TranslateTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[1];
                    AutoCorrectAfterScale(translate);
                }));
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                T visualChild = child as T;
                if (visualChild != null)
                {
                    return visualChild;
                }
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}