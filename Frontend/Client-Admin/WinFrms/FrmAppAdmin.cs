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
    public partial class FrmAppAdmin : Form
    {
        public FrmAppAdmin()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            NetworkingEvents.AccountUpdatedResultEvent -= NetworkingEvents_AccountUpdatedResultEvent;
            NetworkingEvents.AppListReceivedEvent -= NetworkingEvents_AppListReceivedEvent;
        }

        private void NetworkingEvents_AccountUpdatedResultEvent(Networking.Data.ResponseCodes.GenericResponse response)
        {
            NetworkingEvents.AppListReceivedEvent += NetworkingEvents_AppListReceivedEvent;
            AdminNetwork.SendRequestAllAppList();
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                if (((ListView)listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)listView1).SelectedItems[0];

                    ModuleInfo moduleInfo = (ModuleInfo)viewItem.Tag;

                    if (moduleInfo.Enabled == 0)
                    {
                        btnPublish.Text = "Publish";
                    }
                    else
                    {
                        btnPublish.Text = "Un-Publish";
                    }

                    btnPublish.Enabled = true;
                }
            }
            catch
            {
                btnPublish.Enabled = false;
            }
        }

        private void NetworkingEvents_AppListReceivedEvent(ModuleList moduleList)
        {
            NetworkingEvents.AppListReceivedEvent -= NetworkingEvents_AppListReceivedEvent;

            if (moduleList.ModuleInfoList != null)
            {
                this.Invoke(new Action(() =>
                {
                    btnPublish.Enabled = false;
                    listView1.Clear();

                    listView1.Columns.AddRange(new ColumnHeader[] {
                    ColumnName,
                    Version,
                    Published});

                    ImageList imageList = new ImageList();
                    imageList.ImageSize = new Size(1, 18);      // 'hack' to increase row height
                    listView1.SmallImageList = imageList;

                    foreach (ModuleInfo moduleInfo in moduleList.ModuleInfoList)
                    {
                        string enabled = "";

                        if (moduleInfo.Enabled == 0)
                        {
                            enabled = "False";
                        }
                        else
                        {
                            enabled = "True";
                        }

                        ListViewItem viewItem = new ListViewItem(new string[] {
                        moduleInfo.Name,
                        moduleInfo.Version,
                        enabled}, -1);

                        viewItem.Font = new Font("Arial", 12);

                        viewItem.Tag = moduleInfo;

                        listView1.Items.AddRange(new ListViewItem[] {
                            viewItem});
                    }
                }));
            }
        }

        private void FrmAppAdmin_Load(object sender, EventArgs e)
        {
            NetworkingEvents.AccountUpdatedResultEvent += NetworkingEvents_AccountUpdatedResultEvent;
            NetworkingEvents.AppListReceivedEvent += NetworkingEvents_AppListReceivedEvent;
            AdminNetwork.SendRequestAllAppList();

            listView1.ItemSelectionChanged += ListView1_ItemSelectionChanged;
            listView1.DoubleClick += ListView1_DoubleClick;

            btnPublish.Enabled = false;
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (((ListView)listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)listView1).SelectedItems[0];

                    ModuleInfo moduleInfo = (ModuleInfo)viewItem.Tag;

                    if (moduleInfo != null)
                    {
                        FrmAppGroups frmAppGroups = new FrmAppGroups(moduleInfo);
                        frmAppGroups.ShowDialog();

                        frmAppGroups.Dispose();
                    }
                }
            }
            catch
            {

            }
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                if (((ListView)listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)listView1).SelectedItems[0];

                    ModuleInfo moduleInfo = (ModuleInfo)viewItem.Tag;

                    if (moduleInfo != null)
                    {
                        if (moduleInfo.Enabled == 0)
                        {
                            AdminNetwork.SendEnableApp(moduleInfo.Id);
                        }
                        else
                        {
                            AdminNetwork.SendDisableApp(moduleInfo.Id);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void btnDeleteApp_Click(object sender, EventArgs e)
        {
            try
            {
                if (((ListView)listView1).SelectedItems.Count > 0)
                {
                    ListViewItem viewItem = ((ListView)listView1).SelectedItems[0];

                    ModuleInfo moduleInfo = (ModuleInfo)viewItem.Tag;

                    if (moduleInfo != null)
                    {
                        AdminNetwork.SendDeleteService(moduleInfo.Id);
                        listView1.Items.Remove(viewItem);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
