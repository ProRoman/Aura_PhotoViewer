using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;

namespace AuraPhotoViewer.Styles.Behaviors
{
    public class ZoomingPanningBorderBehavior : Behavior<Image>
    {
        private Point _start;
        private Grid _parentGrid;
        private Border _parentBorder;

        public static readonly DependencyProperty IsZoomingOrPanningProperty = DependencyProperty.Register(
            "IsZoomingOrPanning",
            typeof (bool),
            typeof (ZoomingPanningBorderBehavior),
            new FrameworkPropertyMetadata(false)
            );

        public bool IsZoomingOrPanning
        {
            get { return (bool) GetValue(IsZoomingOrPanningProperty); }
            set { SetValue(IsZoomingOrPanningProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _parentBorder = AssociatedObject.Parent as Border;
            if (_parentBorder != null)
            {
                _parentGrid = VisualTreeHelper.GetParent(_parentBorder) as Grid;
            }
            if (_parentGrid != null)
            {
                _parentGrid.MouseWheel += OnMouseWheel;
                _parentGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
                _parentGrid.MouseMove += OnMouseMove;
                _parentGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
                _parentGrid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            Application.Current.MainWindow.StateChanged += MainWindowOnStateChanged;
            AssociatedObject.TargetUpdated += ResetTransforms;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_parentGrid != null)
            {
                _parentGrid.MouseWheel -= OnMouseWheel;
                _parentGrid.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                _parentGrid.MouseMove -= OnMouseMove;
                _parentGrid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                _parentGrid.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            Application.Current.MainWindow.StateChanged -= MainWindowOnStateChanged;
            AssociatedObject.TargetUpdated -= ResetTransforms;
        }

        private void ResetTransforms(object sender, DataTransferEventArgs e)
        {
            if (e.Property.Name != "Source")
            {
                return;
            }
            ScaleTransform scale = (ScaleTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[0];
            TranslateTransform translate =
                (TranslateTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[1];
            RotateTransform rotate = (RotateTransform) ((TransformGroup) AssociatedObject.LayoutTransform).Children[0];
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
            ScaleTransform scale = (ScaleTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[0];
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
            scale.ScaleX += zoom;
            scale.ScaleY += zoom;
            AutoCorrectAfterScale(translate);
            IsZoomingOrPanning = true;
            CheckScaleLimit(zoom, scale, translate);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (AssociatedObject.IsMouseCaptured) return;
            _start = mouseButtonEventArgs.GetPosition(AssociatedObject);
            AssociatedObject.CaptureMouse();
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!AssociatedObject.IsMouseCaptured) return;
            TranslateTransform translate =
                (TranslateTransform) ((TransformGroup) AssociatedObject.RenderTransform).Children[1];
            Point p = mouseEventArgs.GetPosition(AssociatedObject);
            Rect bounds = GetImageBoundsRelativeToBorder();
            RotateTransform rotate = (RotateTransform) ((TransformGroup) AssociatedObject.LayoutTransform).Children[0];
            Point newStartPoint = rotate.Transform(_start);
            Point newEndPoint = rotate.Transform(p);
            double offsetX = newEndPoint.X - newStartPoint.X;
            double offsetY = newEndPoint.Y - newStartPoint.Y;
            // Check if the bounds are located inside the Border control.
            if (_parentBorder.ActualWidth < bounds.Width)
            {
                if (offsetX < 0 && bounds.Right > _parentBorder.ActualWidth) // move left
                {
                    translate.X += offsetX;
                }
                if (offsetX > 0 && bounds.Left < 0) // move right
                {
                    translate.X += offsetX;
                }
            }
            if (_parentBorder.ActualHeight < bounds.Height)
            {
                if (offsetY < 0 && bounds.Bottom > _parentBorder.ActualHeight) // move up
                {
                    translate.Y += offsetY;
                }
                if (offsetY > 0 && bounds.Top < 0) // move down
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

        private Rect GetImageBoundsRelativeToBorder()
        {
            // The bounds of the transformed Image control in coordinates relative to the Border control.
            //Rect rect = new Rect(new Size(image.ActualWidth, image.ActualHeight));
            //return image.TransformToAncestor((Border)AssociatedObject.Parent).TransformBounds(rect);
            GeneralTransform transform = AssociatedObject.TransformToVisual((Border) AssociatedObject.Parent);

            return transform.TransformBounds(new Rect(0, 0, AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));
        }

        private void AutoCorrectAfterTranslate(TranslateTransform translate, double offsetX, double offsetY)
        {
            Rect bounds = GetImageBoundsRelativeToBorder();
            if (_parentBorder.ActualWidth < bounds.Width)
            {
                if (offsetX < 0 && bounds.Right < _parentBorder.ActualWidth) // move left
                {
                    translate.X += _parentBorder.ActualWidth - bounds.Right; // move right
                }
                if (offsetX > 0 && bounds.Left > 0) // move right
                {
                    translate.X += -bounds.Left; // move left
                }
            }
            if (_parentBorder.ActualHeight < bounds.Height)
            {
                if (offsetY < 0 && bounds.Bottom < _parentBorder.ActualHeight) // move up
                {
                    translate.Y += _parentBorder.ActualHeight - bounds.Bottom; // move down
                }
                if (offsetY > 0 && bounds.Top > 0) // move down
                {
                    translate.Y += -bounds.Top; // move up
                }
            }
        }

        private void AutoCorrectAfterScale(TranslateTransform translate)
        {
            Rect bounds = GetImageBoundsRelativeToBorder();
            if (_parentBorder.ActualWidth < bounds.Width)
            {
                if (bounds.Right < _parentBorder.ActualWidth)
                {
                    translate.X += _parentBorder.ActualWidth - bounds.Right; // move right
                }
                if (bounds.Left > 0)
                {
                    translate.X += -bounds.Left; // move left
                }
            }
            if (_parentBorder.ActualHeight < bounds.Height)
            {
                if (bounds.Bottom < _parentBorder.ActualHeight)
                {
                    translate.Y += _parentBorder.ActualHeight - bounds.Bottom; // move down
                }
                if (bounds.Top > 0)
                {
                    translate.Y += -bounds.Top; // move up
                }
            }
        }

        private void OnRotateClick(object sender, RoutedEventArgs e)
        {
            string tag = (string) ((FrameworkElement) e.OriginalSource).Tag;
            RotateTransform rotate = (RotateTransform) ((TransformGroup) AssociatedObject.LayoutTransform).Children[0];
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