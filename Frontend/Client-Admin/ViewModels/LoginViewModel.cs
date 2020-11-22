using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Net;

namespace Client_Admin.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            txtHost = "127.0.0.1";

            LoginCommand = new RelayCommand(o => LoginClicked("LoginClicked"));
        }

        public void LoginClicked(string password)
        {
            IPAddress ipAddress = ParseHost(txtHost);
            if (ipAddress != null)
            {
                AdminNetwork.IpAddress = ipAddress.ToString();

                AdminNetwork.Initialize();

                AdminNetwork.SendLoginRequest(txtUsername, password);
            }
            else
            {
                MessageBox.Show("Invalid host.");
            }
        }

        private IPAddress ParseHost(string ipAddrInput)
        {
            IPAddress ipAddrOutput = null;

            if (!IPAddress.TryParse(ipAddrInput, out ipAddrOutput))
            {
                try
                {
                    IPHostEntry dnsInfo = Dns.GetHostEntry(ipAddrInput);
                    ipAddrOutput = dnsInfo.AddressList[0];
                }
                catch
                {
                    // Neither IP parsing or DNS parsing worked.
                }
            }

            return ipAddrOutput;
        }

        private string txtHost;
        public string TxtHost
        {
            get { return txtHost; }
            set
            {
                txtHost = value;
                RaisePropertyChanged("TxtHost");
            }
        }

        private string txtUsername;
        public string TxtUsername
        {
            get { return txtUsername; }
            set
            {
                txtUsername = value;
                RaisePropertyChanged("TxtUsername");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
