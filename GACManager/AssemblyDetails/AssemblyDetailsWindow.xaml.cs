using System.Windows;

namespace GACManager.AssemblyDetails
{
    /// <summary>
    /// Interaction logic for AssemblyDetailsWindow.xaml
    /// </summary>
    public partial class AssemblyDetailsWindow : Window
    {
        public AssemblyDetailsWindow()
        {
            InitializeComponent();

            Loaded += AssemblyDetailsWindow_Loaded;
        }

        void AssemblyDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            assemblyView.DataContext = AssemblyViewModel;
        }

        public GACAssemblyViewModel AssemblyViewModel { get; set; }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
