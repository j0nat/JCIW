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
    public partial class FrmAccountAdmin : Form
    {
        public FrmAccountAdmin()
        {
            InitializeComponent();

            AdminNetwork.SendRequestAccountList();
            NetworkingEvents.AccountListResultEvent += NetworkingEvents_AccountListResultEvent;
            NetworkingEvents.DeleteAccountResultEvent += NetworkingEvents_DeleteAccountResultEvent;

            listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (((ListView)sender).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)sender).SelectedItems[0];

                    Account account = (Account)viewItem.Tag;

                    if (account != null)
                    {
                        FrmAccount frmAccount = new FrmAccount(account);
                        frmAccount.ShowDialog();
                    }
                }
            }
            catch
            {

            }
        }

        private void NetworkingEvents_AccountListResultEvent(Account[] accounts)
        {
            NetworkingEvents.AccountListResultEvent -= NetworkingEvents_AccountListResultEvent;
            if (accounts != null)
            {
                listView1.Invoke(new Action(() =>
                {
                    listView1.Clear();

                    listView1.Columns.AddRange(new ColumnHeader[] {
                    Username,
                    Firstname,
                    Lastname});

                    ImageList imageList = new ImageList();
                    imageList.ImageSize = new Size(1, 18);      // 'hack' to increase row height
                    listView1.SmallImageList = imageList;

                    foreach (Account account in accounts)
                    {
                        ListViewItem viewItem = new ListViewItem(new string[] {
                        account.username,
                        account.firstname,
                        account.lastname}, -1);

                        viewItem.Font = new Font("Arial", 12);

                        viewItem.Tag = account;

                        listView1.Items.AddRange(new ListViewItem[] {
                            viewItem});
                    }
                }));
            }
        }


        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister();

            if (frmRegister.ShowDialog() == DialogResult.OK)
            {
                NetworkingEvents.AccountListResultEvent += NetworkingEvents_AccountListResultEvent;
                AdminNetwork.SendRequestAccountList();
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (((ListView)listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)listView1).SelectedItems[0];

                    Account account = (Account)viewItem.Tag;

                    if (account != null)
                    {
                        AdminNetwork.SendRequestDeleteAccount(account.id);
                    }
                }
                else
                {
                    MessageBox.Show("No account selected.");
                }
            }
            catch
            {

            }
        }

        private void NetworkingEvents_DeleteAccountResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.AccountListResultEvent += NetworkingEvents_AccountListResultEvent;
            AdminNetwork.SendRequestAccountList();
        }
    }
}
