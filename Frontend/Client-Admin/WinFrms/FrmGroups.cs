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
    public partial class FrmGroups : Form
    {
        public FrmGroups()
        {
            InitializeComponent();

            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            NetworkingEvents.AddRemoveGroupReceivedEvent += NetworkingEvents_AddRemoveGroupReceivedEvent;
            AdminNetwork.SendRequestGroupList();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            NetworkingEvents.GroupListResultEvent -= NetworkingEvents_GroupListResultEvent;
            NetworkingEvents.AddRemoveGroupReceivedEvent -= NetworkingEvents_AddRemoveGroupReceivedEvent;
        }

        private void NetworkingEvents_AddRemoveGroupReceivedEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            AdminNetwork.SendRequestGroupList();
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            FrmAddGroup frmAddGroup = new FrmAddGroup();
            if (frmAddGroup.ShowDialog() == DialogResult.OK)
            {
                string name = FrmAddGroup.name;
                string description = FrmAddGroup.description;
                frmAddGroup.Dispose();

                AdminNetwork.SendAddGroup(name, description);
            }
        }

        private void btnRemoveGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if ((listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = (listView1).SelectedItems[0];

                    Group group = (Group)viewItem.Tag;

                    AdminNetwork.SendDeleteGroup(group.id);
                }
            }
            catch
            {

            }
        }

        private void NetworkingEvents_GroupListResultEvent(Group[] groups)
        {
            NetworkingEvents.GroupListResultEvent -= NetworkingEvents_GroupListResultEvent;

            if (groups != null)
            {
                listView1.Invoke(new Action(() =>
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

                    btnRemoveGroup.Enabled = true;
                }));
            }
        }
    }
}
