using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;

namespace MetryxWPF
{
    /// <summary>
    /// Логика взаимодействия для SelectDeviceWindow.xaml
    /// </summary>
    public partial class SelectDeviceWindow : Window
    {
        private List<MeasurementDeviceView> _devices;
        public MeasurementDeviceView SelectedDevice { get; private set; }
        public SelectDeviceWindow()
        {
            InitializeComponent();
            LoadDevices();
        }
        private void LoadDevices()
        {
            using (PostgresContext db = new PostgresContext())
            {
                _devices = db.Measurementdevices
                    .Include(d => d.Type)
                    .Select(d => new MeasurementDeviceView
                    {
                        Id = d.Id,
                        Name = d.Name,
                        TypeName = d.Type.Name,
                        Serialnumber = d.Serialnumber
                    })
                    .ToList();

                DevicesGrid.ItemsSource = _devices;
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();

            DevicesGrid.ItemsSource = _devices
                .Where(d =>
                    d.Name.ToLower().Contains(search) ||
                    d.Serialnumber.ToLower().Contains(search))
                .ToList();
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesGrid.SelectedItem is MeasurementDeviceView device)
            {
                SelectedDevice = device;

                DialogResult = true;
            }
        }
        private void DevicesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectButton_Click(sender, e);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
