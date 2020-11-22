using Client_Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Admin.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private static MainViewModel ViewModel = new MainViewModel();

        public MainView()
        {
            InitializeComponent();

            this.DataContext = ViewModel;

            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            ViewModel.TxtServiceOutput = TxtServiceOutput;

            TxtServiceOutput.Document.Blocks.Clear();
            ViewModel.AddOutputLine("Select a service.");

            this.LabelConnectedText.Content = "Connected to: " + AdminNetwork.IpAddress;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.ServiceItemSelected(e.NewValue);
        }

        private void txtCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = txtCommand.Text;

                if (ViewModel.CommandEntered(command))
                {
                    txtCommand.Text = "";
                }

                e.Handled = true;
            }
        }

        private void ServiceTreeView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            txtCommand.Focus();
        }
    }
}
