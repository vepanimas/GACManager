using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace GACManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SetDebugProps();
        }

        private void SetDebugProps()
        {
#if DEBUG
            // Show all unhadled exceptions
            Current.DispatcherUnhandledException +=
                (o, args) =>
                {
                    MessageBox.Show(args.Exception.ToString());
                    args.Handled = true;
                };
#endif
        }
    }
}
