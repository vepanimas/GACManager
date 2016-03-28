using System;
using System.Collections.ObjectModel;
using Apex.MVVM;
using GACManagerApi;

namespace GACManager
{
    [ViewModel]
    public class GACAssemblyViewModel : ViewModel<AssemblyDescription>
    {
        public override void FromModel(AssemblyDescription model)
        {
            InternalAssemblyDescription = model;

            DisplayName = model.Name;
            Path = model.Path;
            Version = model.Version;
            Custom = model.Custom;
            ProcessorArchitecture = model.ProcessorArchitecture;
            Culture = model.Culture;
            if (model.PublicKeyToken != null)
                PublicKeyToken = BitConverter.ToString(model.PublicKeyToken).Replace("-", string.Empty);

            //  Load the reserved fusion properties.
            //ReservedHash = BitConverter.ToString(model.FusionProperties.ReservedHashValue).Replace("-", string.Empty);
            //ReservedHashAlgorithm = model.FusionProperties.ReservedHashAlgorithmId;

            LoadExtendedPropertiesCommand = new AsynchronousCommand(DoLoadExtendedPropertiesCommand);
        }

        public override void ToModel(AssemblyDescription model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the internal assembly details, which is wht we're displaying data for.
        /// </summary>
        public AssemblyDescription InternalAssemblyDescription { get; private set; }

        /// <summary>
        /// The NotifyingProperty for the FullName property.
        /// </summary>
        private readonly NotifyingProperty _displayNameProperty =
          new NotifyingProperty("DisplayName", typeof(string), default(string));

        /// <summary>
        /// Gets or sets FullName.
        /// </summary>
        /// <value>The value of FullName.</value>
        public string DisplayName
        {
            get { return (string)GetValue(_displayNameProperty); }
            set { SetValue(_displayNameProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the Name property.
        /// </summary>
        private readonly NotifyingProperty _nameProperty =
          new NotifyingProperty("Name", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value>The value of Name.</value>
        public string Name
        {
            get { return (string)GetValue(_nameProperty); }
            set { SetValue(_nameProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the Culture property.
        /// </summary>
        private readonly NotifyingProperty _cultureProperty =
          new NotifyingProperty("Culture", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        /// <value>The value of Culture.</value>
        public string Culture
        {
            get { return (string)GetValue(_cultureProperty); }
            set { SetValue(_cultureProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the FullName property.
        /// </summary>
        private readonly NotifyingProperty _fullNameProperty =
          new NotifyingProperty("FullName", typeof(string), default(string));

        /// <summary>
        /// Gets or sets FullName.
        /// </summary>
        /// <value>The value of FullName.</value>
        public string FullName
        {
            get { return (string)GetValue(_fullNameProperty); }
            set { SetValue(_fullNameProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the PublicKeyToken property.
        /// </summary>
        private readonly NotifyingProperty _publicKeyTokenProperty =
          new NotifyingProperty("PublicKeyToken", typeof(string), default(string));

        /// <summary>
        /// Gets or sets PublicKeyToken.
        /// </summary>
        /// <value>The value of PublicKeyToken.</value>
        public string PublicKeyToken
        {
            get { return (string)GetValue(_publicKeyTokenProperty); }
            set { SetValue(_publicKeyTokenProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the Version property.
        /// </summary>
        private readonly NotifyingProperty _versionProperty =
          new NotifyingProperty("Version", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        /// <value>The value of Version.</value>
        public string Version
        {
            get { return (string)GetValue(_versionProperty); }
            set { SetValue(_versionProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the FrameworkVersion property.
        /// </summary>
        private readonly NotifyingProperty _frameworkVersionProperty =
          new NotifyingProperty("FrameworkVersion", typeof(string), default(string));

        /// <summary>
        /// Gets or sets FrameworkVersion.
        /// </summary>
        /// <value>The value of FrameworkVersion.</value>
        public string FrameworkVersion
        {
            get { return (string)GetValue(_frameworkVersionProperty); }
            set { SetValue(_frameworkVersionProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the Path property.
        /// </summary>
        private readonly NotifyingProperty _pathProperty =
          new NotifyingProperty("Path", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Path.
        /// </summary>
        /// <value>The value of Path.</value>
        public string Path
        {
            get { return (string)GetValue(_pathProperty); }
            set { SetValue(_pathProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the ProcessorArchitecture property.
        /// </summary>
        private readonly NotifyingProperty _processorArchitectureProperty =
          new NotifyingProperty("ProcessorArchitecture", typeof(string), default(string));

        /// <summary>
        /// Gets or sets ProcessorArchitecture.
        /// </summary>
        /// <value>The value of ProcessorArchitecture.</value>
        public string ProcessorArchitecture
        {
            get { return (string)GetValue(_processorArchitectureProperty); }
            set { SetValue(_processorArchitectureProperty, value); }
        }

        /// <summary>
        /// The NotifyingProperty for the Custom property.
        /// </summary>
        private readonly NotifyingProperty _customProperty =
          new NotifyingProperty("Custom", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Custom.
        /// </summary>
        /// <value>The value of Custom.</value>
        public string Custom
        {
            get { return (string)GetValue(_customProperty); }
            set { SetValue(_customProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the RuntimeVersion property.
        /// </summary>
        private readonly NotifyingProperty _runtimeVersionProperty =
          new NotifyingProperty("RuntimeVersion", typeof(string), default(string));

        /// <summary>
        /// Gets or sets RuntimeVersion.
        /// </summary>
        /// <value>The value of RuntimeVersion.</value>
        public string RuntimeVersion
        {
            get { return (string)GetValue(_runtimeVersionProperty); }
            set { SetValue(_runtimeVersionProperty, value); }
        }



        /// <summary>
        /// Performs the LoadExtendedProperties command.
        /// </summary>
        /// <param name="parameter">The LoadExtendedProperties command parameter.</param>
        private void DoLoadExtendedPropertiesCommand(object parameter)
        {
            var fps = InternalAssemblyDescription.FusionProperties;
            var rps = InternalAssemblyDescription.ReflectionProperties;

            //  Set the extended properties.
            LoadExtendedPropertiesCommand.ReportProgress(
                ()
                =>
                {
                    RuntimeVersion = rps.RuntimeVersion;

                    InstallReferences.Clear();
                    foreach (var installReference in InternalAssemblyDescription.FusionProperties.InstallReferences)
                        InstallReferences.Add(new InstallReferenceViewModel()
                        {
                            Identifier = installReference.Identifier,
                            Description = installReference.Description
                        });
                });
        }

        /// <summary>
        /// Gets the LoadExtendedProperties command.
        /// </summary>
        /// <value>The value of .</value>
        public AsynchronousCommand LoadExtendedPropertiesCommand
        {
            get;
            private set;
        }


        /// <summary>
        /// The InstallReferences observable collection.
        /// </summary>
        private readonly ObservableCollection<InstallReferenceViewModel> _installReferencesProperty =
          new ObservableCollection<InstallReferenceViewModel>();

        /// <summary>
        /// Gets the InstallReferences observable collection.
        /// </summary>
        /// <value>The InstallReferences observable collection.</value>
        public ObservableCollection<InstallReferenceViewModel> InstallReferences
        {
            get { return _installReferencesProperty; }
        }


        /// <summary>
        /// The NotifyingProperty for the ReservedHash property.
        /// </summary>
        private readonly NotifyingProperty _reservedHashProperty =
          new NotifyingProperty("ReservedHash", typeof(string), default(string));

        /// <summary>
        /// Gets or sets ReservedHash.
        /// </summary>
        /// <value>The value of ReservedHash.</value>
        public string ReservedHash
        {
            get { return (string)GetValue(_reservedHashProperty); }
            set { SetValue(_reservedHashProperty, value); }
        }


        /// <summary>
        /// The NotifyingProperty for the ReservedHashAlgorithm property.
        /// </summary>
        private readonly NotifyingProperty _reservedHashAlgorithmProperty =
          new NotifyingProperty("ReservedHashAlgorithm", typeof(uint), default(uint));

        /// <summary>
        /// Gets or sets ReservedHashAlgorithm.
        /// </summary>
        /// <value>The value of ReservedHashAlgorithm.</value>
        public uint ReservedHashAlgorithm
        {
            get { return (uint)GetValue(_reservedHashAlgorithmProperty); }
            set { SetValue(_reservedHashAlgorithmProperty, value); }
        }

    }
}
