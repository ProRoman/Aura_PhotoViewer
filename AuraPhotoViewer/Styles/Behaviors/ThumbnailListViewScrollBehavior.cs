using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using AuraPhotoViewer.Styles.CustomControls;

namespace AuraPhotoViewer.Styles.Behaviors
{
    public class ThumbnailListViewScrollBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
            AssociatedObject.SelectionChanged += OnSelectionChanged;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        }

        private void OnPreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs mouseButtonEventArgs)
        {
            ListView lv = sender as ListView;
            DependencyObject scroll = FindVisualChild<ScrollViewer>(lv);
            ScrollViewer scrollViewer = (ScrollViewer) scroll;
            DependencyObject button = FindVisualChilds<AuraCircleIconButton>(lv).Find(b => b.IsMouseOver);
            if (button == null || scroll == null) return;
            AuraCircleIconButton scrollButton = (AuraCircleIconButton)button;

            if (scrollButton.Name == "NavigateBefore")
            {
                scrollViewer.LineLeft();
            }
            else
            {
                scrollViewer.LineRight();
            }
            mouseButtonEventArgs.Handled = true;
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
            Debug.Assert(lv != null, "lv != null");
            lv.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        if (lv.SelectedItem != null)
                        {
                            lv.ScrollIntoView(lv.SelectedItem);
                        }
                    }));
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
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

        private List<T> FindVisualChilds<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> visualChilds = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                T visualChild = child as T;
                if (visualChild != null)
                {
                    visualChilds.Add(visualChild);
                }
                List<T> childOfChild = FindVisualChilds<T>(child);
                if (childOfChild != null)
                {
                    visualChilds.AddRange(childOfChild);
                }
            }
            return visualChilds;
        }
    }
}