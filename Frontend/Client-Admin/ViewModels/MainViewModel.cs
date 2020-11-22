using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;
using System.Windows.Input;
using System.Windows;
using Client_Admin.Models;
using Client_Admin.WinFrms;
using Client_Admin.Controls;
using System.Windows.Media;
using Networking.Data.Packets;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Client_Admin.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public RichTextBox TxtServiceOutput { get; set; }
        public ICommand InstallCommand { get; set; }
        public ICommand AppCommand { get; set; }
        public ICommand ArchiveCommand { get; set; }
        public ICommand LogsCommand { get; set; }
        public ICommand UsersCommand { get; set; }
        public ICommand GroupsCommand { get; set; }
        private MainModel mainModel { get; set; }
        private List<ServiceItem> services;
        private ServiceItem selectedService;
        private long lastLogReceived;

        // If user sends a manual update we want to skip the next thread tick to avoid duplication.
        private bool ignoreNextPullTick;

        public MainViewModel()
        {
            InstallCommand = new RelayCommand(o => InstallClicked());
            AppCommand = new RelayCommand(o => AppClicked());
            ArchiveCommand = new RelayCommand(o => ArchiveClicked());
            LogsCommand = new RelayCommand(o => LogsClicked());
            UsersCommand = new RelayCommand(o => UsersClicked());
            GroupsCommand = new RelayCommand(o => GroupsClicked());

            mainModel = new MainModel();
            services = new List<ServiceItem>();
            selectedService = null;
            lastLogReceived = -1; // -1 is default
            ignoreNextPullTick = false;

            NetworkingEvents.ModuleListReceivedEvent += AdminNetwork_ModuleListReceivedEvent;
            NetworkingEvents.ServiceCommandResultEvent += NetworkingEvents_ServiceCommandResultEvent;
            NetworkingEvents.ServiceLogListReceivedEvent += NetworkingEvents_ServiceLogListReceivedEvent;

            Thread serverPullThread = new Thread(ServerPullThreadWorker);
            serverPullThread.IsBackground = true;
            serverPullThread.Start();

            ManualUpdate();
        }

        private void ClearOutput()
        {
            TxtServiceOutput.Dispatcher.Invoke(new Action(() =>
            {
                TxtServiceOutput.Document.Blocks.Clear();
            }));
        }

        public void AddOutputLine(string line)
        {
            TxtServiceOutput.Dispatcher.Invoke(new Action(() =>
            {
                Paragraph paragraph = new Paragraph(new Run(line));
                paragraph.Margin = new Thickness(0, 5, 0, 0);

                TxtServiceOutput.Document.Blocks.Add(paragraph);

                TxtServiceOutput.ScrollToEnd();
            }));
        }

        public void AddOutputLine(string from, string line, Brush color)
        {
            TxtServiceOutput.Dispatcher.Invoke(new Action(() =>
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0, 5, 0, 0);

                paragraph.Inlines.Add(new Bold(new Run(from + ": "))
                {
                    Foreground = color
                });

                paragraph.Inlines.Add(line);

                TxtServiceOutput.Document.Blocks.Add(paragraph);

                TxtServiceOutput.ScrollToEnd();
            }));
        }

        public void ServerPullThreadWorker()
        {
            while (true)
            {
                if (!ignoreNextPullTick)
                {
                    if (selectedService != null)
                    {
                        lock (selectedService)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                ModuleInfo item = (ModuleInfo)selectedService.Tag;
                                AdminNetwork.SendServiceLogRequest(item.Id, lastLogReceived);
                            }));
                        }
                    }

                    AdminNetwork.SendModuleListRequest();
                }
                else
                {
                    // Could reset this value either in the receive event or here.
                    ignoreNextPullTick = false;
                }

                Thread.Sleep(2000);
            }
        }

        private void NetworkingEvents_ServiceLogListReceivedEvent(ServiceLogList serviceLogList)
        {
            foreach (ServiceLogItem item in serviceLogList.ServiceLogItems)
            {
                AddOutputLine("SERVER", item.text, Brushes.OrangeRed);
                lastLogReceived = item.date;
            }
        }

        private void NetworkingEvents_ServiceCommandResultEvent(ServiceCommand result)
        {
            AddOutputLine("SERVER", result.value, Brushes.OrangeRed);
        }

        private void AdminNetwork_ModuleListReceivedEvent(ModuleList moduleList)
        {
            Thread thread = new Thread(() => UpdateServices(moduleList));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public bool CommandEntered(string command)
        {
            bool entered = false;

            if (command.Length > 0)
            {
               if (selectedService != null)
                {
                    if (selectedService.Tag != null)
                    {
                        ModuleInfo item = (ModuleInfo)selectedService.Tag;

                        AdminNetwork.SendServiceCommand(item.Id, command);

                   //     OutputText += "CLIENT: " + command + "\n";
                        AddOutputLine("CLIENT", command, Brushes.LightGreen);

                        entered = true;
                    }
                }
            }

            return entered;
        }

        public void ServiceItemSelected(object obj)
        {
            if (obj.GetType() == typeof(ServiceItem))
            {
                ClearOutput();
                lastLogReceived = -1; // Reset log

                ServiceItem listItem = obj as ServiceItem;
                ModuleInfo item = (ModuleInfo)listItem.Tag;

                selectedService = listItem;

                ManualUpdate();
            }
        }

        private void UpdateServices(ModuleList moduleList)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                List<ServiceItem> services = mainModel.UpdateList(moduleList);

                bool updateList = false;
                bool foundDifference = false;
                ServiceItem newSelected = null;

                foreach (ServiceItem newServiceItem in services)
                {
                    bool foundEntry = false;

                    ModuleInfo newItem = (ModuleInfo)newServiceItem.Tag;
                    foreach (ServiceItem oldServiceItem in Services)
                    {
                        ModuleInfo oldItem = (ModuleInfo)oldServiceItem.Tag;

                        if (newItem.Path == oldItem.Path && newItem.Enabled == oldItem.Enabled)
                        {
                            foundEntry = true;

                            if (oldServiceItem == selectedService)
                            {
                                newSelected = newServiceItem;
                            }
                        }
                    }

                    if (!foundEntry)
                    {
                        foundDifference = true;
                    }
                }

                if (Services.Count != services.Count || foundDifference)
                {
                    updateList = true;

                    if (selectedService != null && newSelected == null)
                    {
                        // Currently selected item has been deleted
                        ClearOutput();
                        AddOutputLine("Select a service.");
                        lastLogReceived = -1;
                        selectedService = null;
                    }
                }

                if (updateList)
                {
                    foreach (ServiceItem serviceItem in services)
                    {
                        serviceItem.EnableClickEvent += ServiceItem_EnableClickEvent;
                        serviceItem.DeleteClickEvent += ServiceItem_DeleteClickEvent;
                    }

                    Services = services;
                }
            }));
        }

        private void ServiceItem_DeleteClickEvent(ServiceItem serviceItem)
        {
            ModuleInfo item = (ModuleInfo)serviceItem.Tag;

            AdminNetwork.SendDeleteService(item.Id);
        }

        private void ServiceItem_EnableClickEvent(ServiceItem serviceItem)
        {
            ModuleInfo item = (ModuleInfo)serviceItem.Tag;

            if (item.Enabled == 0) 
            {
                // ENABLE
                AdminNetwork.SendEnableService(item.Id);
            }
            else
            {
                AdminNetwork.SendDisableService(item.Id);
            }
        }

        private void GroupsClicked()
        {
            FrmGroups frmGroups = new FrmGroups();
            frmGroups.ShowDialog();

            frmGroups.Dispose();
        }

        private void UsersClicked()
        {
            FrmAccountAdmin frmAccountAdmin = new FrmAccountAdmin();
            frmAccountAdmin.ShowDialog();

            frmAccountAdmin.Dispose();
        }

        private void LogsClicked()
        {
        }

        private void ArchiveClicked()
        {
        }

        private void AppClicked()
        {
            FrmAppAdmin frmAppAdmin = new FrmAppAdmin();
            frmAppAdmin.ShowDialog();

            frmAppAdmin.Dispose();
        }

        private void InstallClicked()
        {
            FrmUpload frmUpload = new FrmUpload();
            frmUpload.ShowDialog();

            ManualUpdate();
        }

        public List<ServiceItem> Services
        {
            get
            {
                return services;
            }
            set
            {
                services = value;
                RaisePropertyChanged("Services");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            { 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ManualUpdate()
        {
            ignoreNextPullTick = true;

            if (selectedService != null)
            {
                lock (selectedService)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ModuleInfo item = (ModuleInfo)selectedService.Tag;
                        AdminNetwork.SendServiceLogRequest(item.Id, lastLogReceived);
                    }));
                }
            }

            AdminNetwork.SendModuleListRequest();
        }
    }
}
