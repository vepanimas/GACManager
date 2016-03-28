using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using GACManagerApi.Fusion;
using GACManagerApi;
using System.Collections.Generic;
using System.Linq;

namespace GACManager
{
    /// <summary>
    /// Interaction logic for GACManagerView.xaml
    /// </summary>
    public partial class GACManagerView : UserControl
    {
        public GACManagerView()
        {
            InitializeComponent();

            //  If RefreshOnStartup is set, we can refresh now.
            if (Properties.Settings.Default.RefreshOnStartup)
                ViewModel.RefreshAssembliesCommand.DoExecute(null);
        }

        public GACManagerViewModel ViewModel
        {
            get { return FindResource("MainViewModel") as GACManagerViewModel; }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ShowAssemblyDetailsCommand.DoExecute(((FrameworkElement)e.OriginalSource).DataContext as GACAssemblyViewModel);
        }

        private void AssemliesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (GACAssemblyViewModel addedAssembly in e.AddedItems)
            {
                ViewModel.SelectedAssemblies.Add(addedAssembly);
            }

            foreach (GACAssemblyViewModel removedAssembly in e.RemovedItems)
            {
                ViewModel.SelectedAssemblies.Remove(removedAssembly);
            }
        }
    }
}
