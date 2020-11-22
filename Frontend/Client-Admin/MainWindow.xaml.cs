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

using Networking.Data.ResponseCodes;

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NetworkingEvents.LoginResultEvent += NetworkingEvents_LoginResultEvent;
        }

        private void NetworkingEvents_LoginResultEvent(LoginResponse response)
        {
            MainFrame.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                if (response == LoginResponse.Success)
                {
                    MainFrame.NavigationService.Navigate(new Uri(@"Views\MainView.xaml", UriKind.Relative));
                }
                else
                if (response == LoginResponse.WrongUsernamePassword)
                {
                    MessageBox.Show("Wrong username / password.");
                }
                else
                if (response == LoginResponse.NoAdminAccess)
                {
                    MessageBox.Show("This user does not have administrator acccess.");
                }
            }));
        }
    }
}
