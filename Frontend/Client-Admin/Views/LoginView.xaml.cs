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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private static LoginViewModel ViewModel = new LoginViewModel();

        public LoginView()
        {
            InitializeComponent();

            this.DataContext = ViewModel;
        }

        private void TxtPasswordInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Enter
            if (e.Key == Key.Enter)
            {
                ViewModel.LoginClicked(TxtPasswordInput.Password);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoginClicked(TxtPasswordInput.Password);
        }
    }
}
