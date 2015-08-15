using System.Windows;
using System.Windows.Input;

namespace AuraPhotoViewer
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : ResizeableWindow
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow((Window)sender);
        }

        private void MaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            if (window.WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(window);
            }
            else
            {
                SystemCommands.MaximizeWindow(window);
            }
        }

        private void MinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow((Window)sender);
        }
    }
}
