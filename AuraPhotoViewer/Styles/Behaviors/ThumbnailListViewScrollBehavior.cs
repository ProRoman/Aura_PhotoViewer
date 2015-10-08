using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;

namespace AuraPhotoViewer.Styles.Behaviors
{
    public class ThumbnailListViewScrollBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
            AssociatedObject.SelectionChanged += OnSelectionChanged;

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            ListView lv = sender as ListView;
            DependencyObject child = FindVisualChild<ScrollViewer>(lv);
            if (child != null)
            {
                ScrollViewer scrollViewer = (ScrollViewer)child;
                if (mouseWheelEventArgs.Delta > 0)
                {
                    scrollViewer.LineLeft();
                }
                else
                {
                    scrollViewer.LineRight();
                }
                mouseWheelEventArgs.Handled = true;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        if (lv.SelectedItem != null)
                        {
                            lv.ScrollIntoView(lv.SelectedItem);
                        }
                    }));
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