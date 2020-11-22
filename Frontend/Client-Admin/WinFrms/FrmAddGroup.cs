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
    public partial class FrmAddGroup : Form
    {
        public static string name = "";
        public static string description = "";

        public FrmAddGroup()
        {
            InitializeComponent();

            name = "";
            description = "";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length > 3 || txtDescription.Text.Length > 3)
            {
                name = txtName.Text;
                description = txtDescription.Text;

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
        }
    }
}
