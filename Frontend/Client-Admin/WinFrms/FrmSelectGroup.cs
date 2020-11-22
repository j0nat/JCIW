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
    public partial class FrmSelectGroup : Form
    {
        public static Group GROUP = null;

        public FrmSelectGroup()
        {
            InitializeComponent();

            NetworkingEvents.GroupListResultEvent += NetworkingEvents_GroupListResultEvent;
            AdminNetwork.SendRequestGroupList();

            GROUP = null;

            listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (((ListView)sender).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)sender).SelectedItems[0];

                    Group group = (Group)viewItem.Tag;

                    if (group != null)
                    {
                        GROUP = group;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
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
                }));
            }
        }
    }
}
