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
    public partial class FrmAppGroups : Form
    {
        private ModuleInfo moduleInfo;

        public FrmAppGroups(ModuleInfo moduleInfo)
        {
            InitializeComponent();

            this.moduleInfo = moduleInfo;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            NetworkingEvents.AppGroupListResultEvent -= NetworkingEvents_AppGroupListResultEvent;
            NetworkingEvents.AccountUpdatedResultEvent -= NetworkingEvents_AccountUpdatedResultEvent;
        }

        private void FrmAppGroups_Load(object sender, EventArgs e)
        {
            NetworkingEvents.AppGroupListResultEvent += NetworkingEvents_AppGroupListResultEvent;
            NetworkingEvents.AccountUpdatedResultEvent += NetworkingEvents_AccountUpdatedResultEvent;
            AdminNetwork.SendRequestAppGroupList(moduleInfo.Id);
        }

        private void NetworkingEvents_AccountUpdatedResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.AppGroupListResultEvent += NetworkingEvents_AppGroupListResultEvent;
            AdminNetwork.SendRequestAppGroupList(moduleInfo.Id);
        }

        private void NetworkingEvents_AppGroupListResultEvent(Group[] groups)
        {
            NetworkingEvents.AppGroupListResultEvent -= NetworkingEvents_AppGroupListResultEvent;

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

        private void btnGroupDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if ((listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = (listView1).SelectedItems[0];

                    Group group = (Group)viewItem.Tag;

                    AdminNetwork.SendRemoveGroupFromApp(moduleInfo.Id, group.id);

                    listView1.Items.Remove(viewItem);
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

                    AdminNetwork.SendAddGroupToApp(moduleInfo.Id, groupId);
                }
            }
        }
    }
}
