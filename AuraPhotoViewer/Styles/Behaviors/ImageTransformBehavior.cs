using System;
using System.Diagnostics;
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
    public class ImageTransformBehavior : Behavior<Image>
    {
        private Point _start;
        private double _startHorizontalOffset;
        private double _startVerticalOffset;
        private Grid _parentGrid;
        private ScrollViewer _parentScroll;

        public static readonly DependencyProperty IsImageTransformingProperty =
            DependencyProperty.Register(
                "IsImageTransforming",
                typeof (bool),
                typeof (ImageTransformBehavior),
                new FrameworkPropertyMetadata(false)
                );

        public bool IsImageTransforming
        {
            get { return (bool)GetValue(IsImageTransformingProperty); }
            set { SetValue(IsImageTransformingProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _parentScroll = AssociatedObject.Parent as ScrollViewer;
            if (_parentScroll != null)
            {
                _parentScroll.ScrollChanged += OnScrollChanged;
                _parentGrid = VisualTreeHelper.GetParent(_parentScroll) as Grid;
            }
            if (_parentGrid != null)
            {
                _parentGrid.MouseWheel += OnMouseWheel;
                _parentGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
                _parentGrid.MouseMove += OnMouseMove;
                _parentGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
                _parentGrid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            //Application.Current.MainWindow.StateChanged += MainWindowOnStateChanged;
            AssociatedObject.TargetUpdated += ResetTransforms;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _parentScroll.ScrollChanged -= OnScrollChanged;
            if (_parentGrid != null)
            {
                _parentGrid.MouseWheel -= OnMouseWheel;
                _parentGrid.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                _parentGrid.MouseMove -= OnMouseMove;
                _parentGrid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                _parentGrid.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnRotateClick));
            }
            //Application.Current.MainWindow.StateChanged -= MainWindowOnStateChanged;
            AssociatedObject.TargetUpdated -= ResetTransforms;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if (scrollChangedEventArgs.ExtentHeightChange != 0 || scrollChangedEventArgs.ExtentWidthChange != 0)
            {
                Point mousePos = Mouse.GetPosition(_parentScroll);
                double offsetX = scrollChangedEventArgs.HorizontalOffset + mousePos.X;
                double offsetY = scrollChangedEventArgs.VerticalOffset + mousePos.Y;
                double oldExtentWidth = scrollChangedEventArgs.ExtentWidth - scrollChangedEventArgs.ExtentWidthChange;
                double oldExtentHeight = scrollChangedEventArgs.ExtentHeight - scrollChangedEventArgs.ExtentHeightChange;
                double relx = offsetX/oldExtentWidth;
                double rely = offsetY/oldExtentHeight;
                offsetX = Math.Max(relx * scrollChangedEventArgs.ExtentWidth - mousePos.X, 0);
                offsetY = Math.Max(rely * scrollChangedEventArgs.ExtentHeight - mousePos.Y, 0);
                _parentScroll.ScrollToHorizontalOffset(offsetX);
                _parentScroll.ScrollToVerticalOffset(offsetY);
            }
        }

        private void ResetTransforms(object sender, DataTransferEventArgs e)
        {
            if (e.Property.Name != "Source")
            {
                return;
            }
            ScaleTransform scale = (ScaleTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[0];
            RotateTransform rotate = (RotateTransform) ((TransformGroup) AssociatedObject.LayoutTransform).Children[1];
            scale.ScaleX = 1;
            scale.ScaleY = 1;
            rotate.Angle = 0;
            IsImageTransforming = false;
            ResetScrollViewer();
        }

        private void ResetScrollViewer()
        {
            _parentScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _parentScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            AssociatedObject.Stretch = Stretch.None;
            if (AssociatedObject.Source == null)
            {
                return;
            }
            if (AssociatedObject.Source.Height > _parentScroll.ViewportHeight ||
                AssociatedObject.Source.Width > _parentScroll.ViewportWidth)
            {
                AssociatedObject.Stretch = Stretch.Uniform;
            }
        }

        private bool CheckScaleLimit(double zoom, ScaleTransform scale)
        {
            return zoom < 0 && scale.ScaleX <= 1 && scale.ScaleY <= 1;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            _parentScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _parentScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            double zoom = mouseWheelEventArgs.Delta > 0 ? .2 : -.2;
            ScaleTransform scale = (ScaleTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[0];
            if (CheckScaleLimit(zoom, scale))
            {
                ResetScrollViewer();
                return;
            }
            scale.ScaleX += zoom;
            scale.ScaleY += zoom;
            IsImageTransforming = scale.ScaleX != 1;
            mouseWheelEventArgs.Handled = true;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (AssociatedObject.IsMouseCaptured) return;
            _start = mouseButtonEventArgs.GetPosition(AssociatedObject);
            _startHorizontalOffset = _parentScroll.HorizontalOffset;
            _startVerticalOffset = _parentScroll.VerticalOffset;
            AssociatedObject.CaptureMouse();
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!AssociatedObject.IsMouseCaptured) return;
            Point p = mouseEventArgs.GetPosition(AssociatedObject);
            RotateTransform rotate = (RotateTransform)((TransformGroup)AssociatedObject.LayoutTransform).Children[1];
            Point newStartPoint = rotate.Transform(_start);
            Point newEndPoint = rotate.Transform(p);
            double offsetX = newStartPoint.X - newEndPoint.X;
            double offsetY = newStartPoint.Y - newEndPoint.Y;
            if (offsetX != 0)
            {
                _parentScroll.ScrollToHorizontalOffset(_startHorizontalOffset + offsetX);
            }
            if (offsetY != 0)
            {
                _parentScroll.ScrollToVerticalOffset(_startVerticalOffset + offsetY);
            }
            Debug.WriteLine("Sender " + sender.GetType());
            Debug.WriteLine("OnMouseMove " + offsetX + " " + offsetY);
            mouseEventArgs.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            AssociatedObject.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
        }

        private void OnRotateClick(object sender, RoutedEventArgs e)
        {
            string tag = (string) ((FrameworkElement) e.OriginalSource).Tag;
            RotateTransform rotate = (RotateTransform) ((TransformGroup) AssociatedObject.LayoutTransform).Children[1];
            if (tag == "CounterClockwiseRotateButton")
            {
                rotate.Angle = rotate.Angle - 90;
            }
            else if (tag == "ClockwiseRotateButton")
            {
                rotate.Angle = rotate.Angle + 90;
            }
        }

        /*private void MainWindowOnStateChanged(object sender, EventArgs e)
        {
            AssociatedObject.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {                    
                }));
        }*/

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