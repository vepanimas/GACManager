using System.Windows;
using System.Windows.Controls;
using Apex;

namespace GACManager
{
    /// <summary>
    /// Interaction logic for AssemblyView.xaml
    /// </summary>
    public partial class AssemblyView : UserControl
    {
        public AssemblyView()
        {
            InitializeComponent();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            ApexBroker.GetShell().ClosePopup(this, null);
        }
    }
}
