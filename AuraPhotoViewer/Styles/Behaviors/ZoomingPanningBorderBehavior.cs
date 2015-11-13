using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace AuraPhotoViewer.Styles.Behaviors
{
    public class ZoomingPanningBorderBehavior : Behavior<Border>
    {
        private Point start;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseWheel += OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }
        
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseWheel -= OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            double zoom = mouseWheelEventArgs.Delta > 0 ? .2 : -.2;
            ScaleTransform scale = (ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[0];
            TranslateTransform translate =
                (TranslateTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1];
            if (zoom < 0 && scale.ScaleX <= 1 && scale.ScaleY <= 1)
            {
                translate.X = 0;
                translate.Y = 0;
                return;
            }
            Point position = mouseWheelEventArgs.GetPosition(AssociatedObject);
            AssociatedObject.RenderTransformOrigin = new Point(position.X / AssociatedObject.ActualWidth, position.Y / AssociatedObject.ActualHeight);
            scale.ScaleX += zoom;
            scale.ScaleY += zoom;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (AssociatedObject.IsMouseCaptured) return;
            start = mouseButtonEventArgs.GetPosition(AssociatedObject);
            AssociatedObject.CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!AssociatedObject.IsMouseCaptured) return;
            TranslateTransform translate =
                (TranslateTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1];
            Point p = mouseEventArgs.GetPosition(AssociatedObject);
            DependencyObject child = FindVisualChild<Image>(AssociatedObject);
            if (child != null)
            {
                Image image = (Image)child;
                // The bounds of the transformed Image control in coordinates relative to the Border control. 
                // Check if the bounds are located inside the Border control.
                Rect rect = new Rect(new Size(image.ActualWidth, image.ActualHeight));
                Rect bounds = image.TransformToAncestor(AssociatedObject).TransformBounds(rect);
                double offsetX = p.X - start.X;
                double offsetY = p.Y - start.Y;
                if (AssociatedObject.ActualWidth < bounds.Width)
                {
                    if (offsetX < 0 && bounds.Right > AssociatedObject.ActualWidth) // move left
                    {
                        translate.X += offsetX;
                    }
                    if (offsetX > 0 && bounds.Left < 0) // move right
                    {
                        translate.X += offsetX;
                    }
                }
                if (AssociatedObject.ActualHeight < bounds.Height)
                {
                    if (offsetY < 0 && bounds.Bottom > AssociatedObject.ActualHeight) // move up
                    {
                        translate.Y += offsetY;
                    }
                    if (offsetY > 0 && bounds.Top < 0) // move down
                    {
                        translate.Y += offsetY;
                    }
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            AssociatedObject.ReleaseMouseCapture();
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
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