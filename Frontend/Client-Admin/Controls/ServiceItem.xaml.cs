using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Admin.Controls
{
    /// <summary>
    /// Interaction logic for ServiceItem.xaml
    /// </summary>
    public partial class ServiceItem : UserControl
    {
        public delegate void EnableClick(ServiceItem serviceItem);
        public event EnableClick EnableClickEvent;

        public delegate void DeleteClick(ServiceItem serviceItem);
        public event DeleteClick DeleteClickEvent;

        public ICommand DeleteCommand { get; set; }
        public ICommand EnableCommand { get; set; }

        public string ServiceTitle
        {
            get { return (string)GetValue(ServiceTitleProperty); }
            set { SetValue(ServiceTitleProperty, value); }
        }

        public string MenuEnableHeader
        {
            get { return (string)GetValue(MenuEnableHeaderProperty); }
            set { SetValue(MenuEnableHeaderProperty, value); }
        }

        public Brush ServiceStatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public static readonly DependencyProperty ServiceTitleProperty =
            DependencyProperty.Register("ServiceTitle", typeof(string), typeof(ServiceItem), new UIPropertyMetadata(""));

        public static readonly DependencyProperty MenuEnableHeaderProperty =
            DependencyProperty.Register("MenuEnableHeader", typeof(string), typeof(ServiceItem), new UIPropertyMetadata(""));

        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("ServiceStatusColor", typeof(Brush), typeof(ServiceItem), new UIPropertyMetadata(Brushes.Cyan));


        public ServiceItem()
        {
            InitializeComponent();

            DataContext = this;

            DeleteCommand = new RelayCommand(o => DeleteClicked());
            EnableCommand = new RelayCommand(o => EnableClicked());
        }

        private void EnableClicked()
        {
            if (EnableClickEvent != null)
            {
                EnableClickEvent.Invoke(this);
            }
        }

        private void DeleteClicked()
        {
            if (DeleteClickEvent != null)
            {
                DeleteClickEvent.Invoke(this);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).ContextMenu.DataContext = ((Button)sender).DataContext;
            MenuContextMenu.IsOpen = true;
        }
    }
}
