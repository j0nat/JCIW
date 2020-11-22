using Client_Admin.Controls;
using Networking.Data.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Client_Admin.Models
{
    class MainModel
    {
        public List<ServiceItem> UpdateList(ModuleList moduleList)
        {
            List<ServiceItem> newList = new List<ServiceItem>();

            if (moduleList.ModuleInfoList != null)
            {
                foreach (ModuleInfo moduleInfo in moduleList.ModuleInfoList)
                {
                    string serviceName = moduleInfo.Name + " (" + moduleInfo.Version + ")";

                    ServiceItem ServiceItem = new ServiceItem();
                    ServiceItem.ServiceTitle = serviceName;
                    ServiceItem.Tag = moduleInfo; 

                    if (moduleInfo.Enabled == 0)
                    {
                        ServiceItem.MenuEnableHeader = "Enable";
                        ServiceItem.ServiceStatusColor = (Brush)(new BrushConverter()).ConvertFromString("#FF0000");
                    }
                    else
                    {
                        ServiceItem.MenuEnableHeader = "Disable";
                        ServiceItem.ServiceStatusColor = (Brush)(new BrushConverter()).ConvertFromString("#00FF00");
                    }

                    newList.Add(ServiceItem);
                }
            }

            return newList;
        }
    }
}
