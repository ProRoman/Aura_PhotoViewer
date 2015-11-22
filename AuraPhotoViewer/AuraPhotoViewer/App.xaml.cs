using AuraPhotoViewer.Modules.Common.Events;
using Microsoft.Practices.Unity;
using Prism.Events;
using System.Windows;
using log4net;
using System;
using System.Windows.Threading;

namespace AuraPhotoViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Log4net

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Info("App starts");
            base.OnStartup(e);
            Log.Info("Bootstrapper starts");
            // Start Bootstrapper            
            var bootstrapper = new AuraBootstrapper();
            bootstrapper.Run();
            Application.Current.Exit += AppExitHandler;
            // On start up publish opened image
            if (e.Args.Length >= 1)
            {
                bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<OpenedImageEvent>().Publish(e.Args[0]);
            }
            // Unhandled by code running on the main UI thread
            Current.DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;
            // In any thread
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledExceptionHandler;
        }

        private void AppExitHandler(object sender, ExitEventArgs exitEventArgs)
        {
            Log.Info("App shuts down");
        }

        private void DomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var ex = unhandledExceptionEventArgs.ExceptionObject as Exception;
            Log.Fatal("Unhandled Thread Exception", ex);
        }

        private void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Log.Fatal("Dispatcher Unhandled Exception", dispatcherUnhandledExceptionEventArgs.Exception);
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }
    }
}
