using Microsoft.VisualBasic;
using Networking.Data.Packets;
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
    public partial class FrmAccount : Form
    {
        private Account account;

        public FrmAccount(Account account)
        {
            InitializeComponent();

            txtUsername.Text = account.username;
            txtLastname.Text = account.lastname;
            txtFirstname.Text = account.firstname;

            this.account = account;

            this.Text = account.firstname + " " + account.lastname;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            NetworkingEvents.GroupListResultEvent -= NetworkingEvents_GroupListResultEvent;
            NetworkingEvents.GroupAddedToUserResultEvent -= NetworkingEvents_GroupAddedToUserResultEvent;
            NetworkingEvents.UserGroupDeletedResultEvent -= NetworkingEvents_UserGroupDeletedResultEvent;
            NetworkingEvents.PasswordChangeResultEvent -= NetworkingEvents_PasswordChangeResultEvent;
            NetworkingEvents.AccountUpdatedResultEvent -= NetworkingEvents_AccountUpdatedResultEvent;

            base.OnClosing(e);
        }

        private void FrmAccount_Load(object sender, EventArgs e)
        {
            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            NetworkingEvents.GroupAddedToUserResultEvent += NetworkingEvents_GroupAddedToUserResultEvent;
            NetworkingEvents.UserGroupDeletedResultEvent += NetworkingEvents_UserGroupDeletedResultEvent;
            NetworkingEvents.PasswordChangeResultEvent += NetworkingEvents_PasswordChangeResultEvent;
            NetworkingEvents.AccountUpdatedResultEvent += NetworkingEvents_AccountUpdatedResultEvent;
            AdminNetwork.SendRequestGroupUserList(account.id);
        }

        private void NetworkingEvents_GroupAddedToUserResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            AdminNetwork.SendRequestGroupUserList(account.id);
        }

        private void NetworkingEvents_AccountUpdatedResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Account updated " + response.ToString());
            }));
        }

        private void NetworkingEvents_PasswordChangeResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Password changed " + response.ToString());
            }));
        }

        private void NetworkingEvents_UserGroupDeletedResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            AdminNetwork.SendRequestGroupUserList(account.id);
        }

        private void NetworkingEvents_GroupListResultEvent(Group[] groups)
        {
            NetworkingEvents.GroupListResultEvent -= NetworkingEvents_GroupListResultEvent;

            if (groups != null)
            {
                this.Invoke(new Action(() =>
                {
                    listView1.Clear();

                    listView1.Columns.AddRange(new ColumnHeader[] {
                    columnHeader1,
                    columnHeader2});

                    ImageList imageList = new ImageList();
                    imageList.ImageSize = new Size(1, 18);      // 'hack' to increase row height
                    listView1.SmallImageList = imageList;

                    foreach (Group group in groups)
                    {
                        ListViewItem viewItem = new ListViewItem(new string[] {
                        group.name,
                        group.description}, -1);

                        viewItem.Font = new Font("Arial", 12);

                        viewItem.Tag = group;

                        listView1.Items.AddRange(new ListViewItem[] {
                            viewItem});
                    }

                    btnGroupDelete.Enabled = true;
                }));
            }
        }

        private void btnSetPassword_Click(object sender, EventArgs e)
        {
           string newPassword = Interaction.InputBox("Type in a new password for " + account.firstname + " " + account.lastname, "Change password", "");

            if (newPassword.Length > 3)
            {
                AdminNetwork.SendUpdateAccountPassword(account.id, newPassword);
            }
        }

        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string firstname = txtFirstname.Text;
            string lastname = txtLastname.Text;

            if (username != account.username || lastname != account.lastname || firstname != account.firstname)
            {
                AdminNetwork.SendUpdateAccountInformation(account.id, username, firstname, lastname);
            }
        }

        private void btnGroupDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if ((listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = (listView1).SelectedItems[0];

                    Group group = (Group)viewItem.Tag;

                    AdminNetwork.SendDeleteGroupFromUser(account.id, group.id);
                }
            }
            catch
            {

            }
        }

        private void btnGroupAdd_Click(object sender, EventArgs e)
        {
            FrmSelectGroup frmSelectGroup = new FrmSelectGroup();
            if (frmSelectGroup.ShowDialog() == DialogResult.OK)
            {
                if (FrmSelectGroup.GROUP != null)
                {
                    long groupId = FrmSelectGroup.GROUP.id;
                    frmSelectGroup.Dispose();

                    AdminNetwork.SendAddGroupToUser(account.id, groupId);
                }
            }
        }
    }
}
