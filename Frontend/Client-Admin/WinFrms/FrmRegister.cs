using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_Admin.WinFrms
{
    public partial class FrmRegister : Form
    {
        private bool isBusy;

        public FrmRegister()
        {
            this.isBusy = false;

            InitializeComponent();


        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string firstname = txtFirstname.Text;
            string lastname = txtLastname.Text;

            if (username.Length > 1 && password.Length > 1 && firstname.Length > 1 && lastname.Length > 1)
            {
                btnRegister.Enabled = false;
                isBusy = true;

                AdminNetwork.SendRegisterRequest(username, password, firstname, lastname);

                NetworkingEvents.RegisterResultEvent += NetworkingEvents_RegisterResultEvent;
            }
            else
            {
                MessageBox.Show("Information entered was too short.");
            }
        }

        private void NetworkingEvents_RegisterResultEvent(Networking.Data.ResponseCodes.RegisterResponse response)
        {
            NetworkingEvents.RegisterResultEvent -= NetworkingEvents_RegisterResultEvent;

            if (response == Networking.Data.ResponseCodes.RegisterResponse.Success)
            {
                MessageBox.Show("User successfully registered.");
            }
            else
            {
                if (response == Networking.Data.ResponseCodes.RegisterResponse.UsernameTaken)
                {
                    MessageBox.Show("Username is already taken.");
                }
                else
                {
                    MessageBox.Show("Could not register. Check username / password.");
                }
            }

            btnRegister.Invoke(new Action(() =>{
                btnRegister.Enabled = true;
                isBusy = false;

                if (response == Networking.Data.ResponseCodes.RegisterResponse.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }));
        }

        private void FrmRegister_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isBusy)
            {
                e.Cancel = true;
            }
        }
    }
}
