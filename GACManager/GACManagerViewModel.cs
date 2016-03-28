using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Apex;
using Apex.MVVM;
using GACManager.Models;
using GACManager.Properties;
using GACManagerApi;
using GACManagerApi.Fusion;
using Microsoft.Win32;

namespace GACManager
{
    [ViewModel]
    public class GACManagerViewModel : ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GACManagerViewModel"/> class.
        /// </summary>
        public GACManagerViewModel()
        {
            InitCommands();

            //  Update Gac Util Status info.
            GacUtilStatusInfo = GacUtil.CanFindExecutable() ? Resources.Msg_GacUtilFound :
                Resources.Msg_GacUtilNotFound;
        }

        private void InitCommands()
        {
            RefreshAssembliesCommand = new AsynchronousCommand(DoRefreshAssembliesCommand);
            CopyDisplayNameCommand = new Command(DoCopyDisplayNameCommand, false);
            ShowFilePropertiesCommand = new Command(DoShowFilePropertiesCommand, false);

            UninstallAssembliesCommand = new Command(DoUninstallAssembliesCommand, false);
            UninstallAssembliesCommand.Executing += UninstallAssembliesCommand_Executing;

            OpenAssemblyLocationCommand = new Command(DoOpenAssemblyLocationCommand, false);
            InstallAssemblyCommand = new Command(DoInstallAssemblyCommand);
            HelpCommand = new Command(DoHelpCommand);
            ShowAssemblyDetailsCommand = new Command(DoShowAssemblyDetailsCommand, false);
        }


        /// <summary>
        /// The Assemblies observable collection.
        /// </summary>
        private readonly SafeObservableCollection<GACAssemblyViewModel> _assembliesProperty =
          new SafeObservableCollection<GACAssemblyViewModel>();

        /// <summary>
        /// Gets the Assemblies observable collection.
        /// </summary>
        /// <value>The Assemblies observable collection.</value>
        public SafeObservableCollection<GACAssemblyViewModel> Assemblies
        {
            get { return _assembliesProperty; }
        }


        /// <summary>
        /// The NotifyingProperty for the StatusInfo property.
        /// </summary>
        private readonly NotifyingProperty _statusInfoProperty =
          new NotifyingProperty("StatusInfo", typeof(string), default(string));

        /// <summary>
        /// Gets or sets StatusInfo.
        /// </summary>
        /// <value>The value of StatusInfo.</value>
        public string StatusInfo
        {
            get { return (string)GetValue(_statusInfoProperty); }
            set { SetValue(_statusInfoProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the GacUtilStatusInfo property.
        /// </summary>
        private readonly NotifyingProperty _gacUtilStatusInfoProperty =
          new NotifyingProperty("GacUtilStatusInfo", typeof(string), default(string));

        /// <summary>
        /// Gets or sets GacUtilStatusInfo.
        /// </summary>
        /// <value>The value of GacUtilStatusInfo.</value>
        public string GacUtilStatusInfo
        {
            get { return (string)GetValue(_gacUtilStatusInfoProperty); }
            set { SetValue(_gacUtilStatusInfoProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the SelectedAssembly property.
        /// </summary>
        private readonly NotifyingProperty _selectedAssemblyProperty =
          new NotifyingProperty("SelectedAssembly", typeof(GACAssemblyViewModel), default(GACAssemblyViewModel));

        /// <summary>
        /// Gets or sets SelectedAssembly.
        /// </summary>
        /// <value>The value of SelectedAssembly.</value>
        public GACAssemblyViewModel SelectedAssembly
        {
            get { return (GACAssemblyViewModel)GetValue(_selectedAssemblyProperty); }
            set
            {
                SetValue(_selectedAssemblyProperty, value);
                UninstallAssembliesCommand.CanExecute = value != null;
                OpenAssemblyLocationCommand.CanExecute = value != null;
                ShowFilePropertiesCommand.CanExecute = value != null;
                CopyDisplayNameCommand.CanExecute = value != null;
                ShowAssemblyDetailsCommand.CanExecute = value != null;
            }
        }


        /// <summary>
        /// The selected assemblies observable collection
        /// </summary>
        private readonly SafeObservableCollection<GACAssemblyViewModel> _selectedAssemblies
            = new SafeObservableCollection<GACAssemblyViewModel>();

        /// <summary>
        /// Gets the selected assemblies observable collection
        /// </summary>
        public SafeObservableCollection<GACAssemblyViewModel> SelectedAssemblies
        {
            get { return _selectedAssemblies; }
        }


        /// <summary>
        /// The NotifyingProperty for the AssembliesCollectionView property.
        /// </summary>
        private readonly NotifyingProperty _assembliesCollectionViewProperty =
          new NotifyingProperty("AssembliesCollectionView", typeof(CollectionView), default(CollectionView));

        /// <summary>
        /// Gets or sets AssembliesCollectionView.
        /// </summary>
        /// <value>The value of AssembliesCollectionView.</value>
        public CollectionView AssembliesCollectionView
        {
            get { return (CollectionView)GetValue(_assembliesCollectionViewProperty); }
            set { SetValue(_assembliesCollectionViewProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the SearchText property.
        /// </summary>
        private readonly NotifyingProperty _searchTextProperty =
          new NotifyingProperty("SearchText", typeof(string), default(string));

        /// <summary>
        /// Gets or sets SearchText.
        /// </summary>
        /// <value>The value of SearchText.</value>
        public string SearchText
        {
            get { return (string)GetValue(_searchTextProperty); }
            set
            {
                SetValue(_searchTextProperty, value);
                AssembliesCollectionView?.Refresh();
            }
        }


        #region Commands

        /// <summary>
        /// Gets the RefreshAssemblies command.
        /// </summary>
        /// <value>The value of the RefreshAssemblies Command.</value>
        public AsynchronousCommand RefreshAssembliesCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the UninstallAssembly command.
        /// </summary>
        /// <value>The value of .</value>
        public Command UninstallAssembliesCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the InstallAssembly command.
        /// </summary>
        /// <value>The value of .</value>
        public Command InstallAssemblyCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the open assembly location command.
        /// </summary>
        public Command OpenAssemblyLocationCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the CopyDisplayName command.
        /// </summary>
        /// <value>The value of .</value>
        public Command CopyDisplayNameCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the Help command.
        /// </summary>
        /// <value>The value of .</value>
        public Command HelpCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the ShowFileProperties command.
        /// </summary>
        /// <value>The value of .</value>
        public Command ShowFilePropertiesCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the ShowAssemblyDetails command.
        /// </summary>
        /// <value>The value of .</value>
        public Command ShowAssemblyDetailsCommand
        {
            get;
            private set;
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// Performs the RefreshAssemblies command.
        /// </summary>
        /// <param name="parameter">The RefreshAssemblies command parameter.</param>
        private void DoRefreshAssembliesCommand(object parameter)
        {
            //  Clear the assemblies.
            Assemblies.Clear();

            //  Set the status text.
            RefreshAssembliesCommand.ReportProgress(
                () =>
                {
                    StatusInfo = Resources.Msg_RefreshAssemblies_Progress;
                });

            //  Start the enumeration.
            var timeTaken = ApexBroker.GetModel<IGACManagerModel>().EnumerateAssemblies(
                assemblyDetails =>
                {
                    //  Create an assembly view model from the detials.
                    var viewModel = new GACAssemblyViewModel();
                    viewModel.FromModel(assemblyDetails);

                    //  Add it to the collection.
                    Assemblies.Add(viewModel);

                });

            //  Set the resulting status info.
            RefreshAssembliesCommand.ReportProgress(
                () =>
                {
                    AssembliesCollectionView = new ListCollectionView(Assemblies.ToList());

                    AssembliesCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
                    AssembliesCollectionView.Filter += Filter;
                    StatusInfo = string.Format(Resources.Msg_RefreshAssemblies_Success, Assemblies.Count, timeTaken.TotalMilliseconds);
                });
        }


        /// <summary>
        /// Performs UninstallAssemblies command
        /// </summary>
        /// <param name="parameter">The UninstallAssembliesCommand parameter</param>
        private void DoUninstallAssembliesCommand(object parameter)
        {
            // Check if user is an administrator
            try
            {
                //  The parameter must be an assemblies collection.
                var assemblies = SelectedAssemblies;
                if (assemblies.Count == 0) return;

                var shouldReload = false;
                var messages = new List<string>();
                
                foreach (var assemly in assemblies)
                {
                    //  Create an assembly cache.
                    IASSEMBLYCACHE_UNINSTALL_DISPOSITION disposition = IASSEMBLYCACHE_UNINSTALL_DISPOSITION.Unknown;
                    AssemblyCache.UninstallAssembly(assemly.InternalAssemblyDescription.DisplayName,
                        null, out disposition);

                    //  Depending on the result, show the appropriate message.
                    string message = string.Empty;
                    switch (disposition)
                    {
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.Unknown:
                            message = string.Format(Resources.ErrorMsg_Uninstall_Unknown, assemly.DisplayName);
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_UNINSTALLED:
                            message = string.Format(Resources.Msg_Uninstall_Success, assemly.DisplayName);
                            
                            // if any removed, reload assemblies
                            shouldReload = true;
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_STILL_IN_USE:
                            message = string.Format(Resources.ErrorMsg_Uninstall_InUse, assemly.DisplayName);
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_ALREADY_UNINSTALLED:
                            message = string.Format(Resources.ErrorMsg_Uninstall_AlreadyUninstalled, assemly.DisplayName);
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_DELETE_PENDING:
                            message = string.Format(Resources.ErrorMsg_Uninstall_Deleting, assemly.DisplayName);
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_HAS_INSTALL_REFERENCES:
                            message = string.Format(Resources.ErrorMsg_Uninstall_PartOfAnotherProduct, assemly.DisplayName);
                            break;
                        case IASSEMBLYCACHE_UNINSTALL_DISPOSITION.IASSEMBLYCACHE_UNINSTALL_DISPOSITION_REFERENCE_NOT_FOUND:
                            message = string.Format(Resources.ErrorMsg_Uninstall_NotFound, assemly.DisplayName);
                            break;
                    }
                    messages.Add(message);

                    //  Remove the assembly from the vm.
                    Assemblies.Remove(assemly);
                }

                // Show the message box.
                MessageBox.Show(string.Join("\n", messages), Resources.Title_Uninstall);

                // Reload assemblies
                if (shouldReload) RefreshAssembliesCommand.DoExecute();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(Resources.ErrorMsg_Uninstall_UnauthorizedError, Resources.Title_Uninstall);
            }
        }


        private void UninstallAssembliesCommand_Executing(object sender, Apex.MVVM.CancelCommandEventArgs args)
        {
            //  Double check with the user.
            args.Cancel = MessageBox.Show(Resources.Prompt_Uninstall, Resources.Title_Uninstall,
                                          MessageBoxButton.YesNoCancel)
                          != MessageBoxResult.Yes;
        }


        /// <summary>
        /// Performs the CopyDisplayName command.
        /// </summary>
        /// <param name="parameter">The CopyDisplayName command parameter.</param>
        private void DoCopyDisplayNameCommand(object parameter)
        {
            var assembly = (GACAssemblyViewModel)parameter;
            Clipboard.SetText(assembly.InternalAssemblyDescription.DisplayName);
        }


        /// <summary>
        /// Performs the ShowAssemblyDetails command.
        /// </summary>
        /// <param name="parameter">The ShowAssemblyDetails command parameter.</param>
        private void DoShowAssemblyDetailsCommand(object parameter)
        {
            //  Get the assembly view model.
            var assemblyViewModel = parameter as GACAssemblyViewModel;

            //  If we don't have one, bail.
            if (assemblyViewModel == null)
                return;

            //  Create a new assembly details window.
            var assemblyDetailsWindow = new AssemblyDetails.AssemblyDetailsWindow
            {
                AssemblyViewModel = assemblyViewModel
            };

            //  Show the window.
            assemblyDetailsWindow.ShowDialog();
        }


        /// <summary>
        /// Performs the Help command.
        /// </summary>
        /// <param name="parameter">The Help command parameter.</param>
        private void DoHelpCommand(object parameter)
        {
            Process.Start(Properties.Resources.ProjectHomePageUrl);
        }


        /// <summary>
        /// Performs the ShowFileProperties command.
        /// </summary>
        /// <param name="parameter">The ShowFileProperties command parameter.</param>
        private void DoShowFilePropertiesCommand(object parameter)
        {
            var assembly = (GACAssemblyViewModel)parameter;
            Interop.Shell.Shell32.ShowFileProperties(assembly.InternalAssemblyDescription.Path);
        }


        /// <summary>
        /// Handles the Executed event of the OpenAssemblyLocationCommand control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Apex.MVVM.CommandEventArgs"/> instance containing the event data.</param>
        void DoOpenAssemblyLocationCommand(object parameter)
        {
            //  Get the assembly.
            var assemblyViewModel = (GACAssemblyViewModel)parameter;

            //  Try and open it's path.
            try
            {
                Process.Start(System.IO.Path.GetDirectoryName(assemblyViewModel.Path));
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Failed to open directory: " + exception);
            }
        }

        private void DoInstallAssemblyCommand(object parameter)
        {
            //  Create an open file dialog.
            var openFileDialog = new OpenFileDialog
            {
                Title = Resources.Prompt_Installation_SelectAssembly,
                Filter = "Assemblies (*.dll)|*.dll"
            };

            if (openFileDialog.ShowDialog() != true) return;

            //  Get the assembly path.
            var assemblyPath = openFileDialog.FileName;

            //  Install the assembly.
            try
            {
                AssemblyCache.InstallAssembly(assemblyPath, null, AssemblyCommitFlags.Force);
                
                MessageBox.Show(Resources.Msg_InstallationCompleted_Success, Resources.Title_Install);
                // Reload assemblies after proper installation
                RefreshAssembliesCommand.DoExecute(null);
            }
            catch (AssemblyMustBeStronglyNamedException)
            {
                MessageBox.Show(Resources.ErrorMsg_InstallationFailed_AssemblyIsNotStrongNamed, Resources.Title_Install);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(Resources.ErrorMsg_InstallationFailed_UnauthorizedError, Resources.Title_Install);
            }
            catch
            {
                MessageBox.Show(Resources.ErrorMsg_InstallationFailed_GenericError, Resources.Title_Install);
            }
        }


        #endregion

        #region Helpers

        private bool Filter(object o)
        {
            var assemblyViewModel = o as GACAssemblyViewModel;
            if (assemblyViewModel == null)
                return false;

            return string.IsNullOrEmpty(SearchText) ||
                assemblyViewModel.DisplayName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) != -1 ||
                assemblyViewModel.PublicKeyToken.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) != -1;
        }

        #endregion
    }
}
