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
using Microsoft.EntityFrameworkCore;

namespace MetryxWPF
{
    /// <summary>
    /// Логика взаимодействия для AddDeviceWindow.xaml
    /// </summary>
    public partial class AddDeviceWindow : Window
    {
        public AddDeviceWindow()
        {
            InitializeComponent();

            using (PostgresContext db = new PostgresContext())
            {
                var types = db.Devicetypes
                            .Select(t => new Devicetype { Id = t.Id, Name = t.Name })
                            .ToList();
                var species = db.Species
                                .Select(s => new Species { Id = s.Id, Name = s.Name })
                                .ToList();

                DeviceSpecies.ItemsSource = species;
                DeviceType.ItemsSource = types;
            }
        }

        //Добавление записи в БД
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceName.Text == "" || DeviceName.Text == " " ||
                DeviceSerialNumber.Text == "" || DeviceSerialNumber.Text == " " ||
                DeviceType.SelectedValue == null || DeviceReleaseDate.SelectedDate == null ||
                DeviceLastverificationDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            Measurementdevice newDevice;
            using (PostgresContext db = new PostgresContext())
            {
                newDevice = new Measurementdevice
                {
                    Name = DeviceName.Text,
                    Typeid = (int)DeviceType.SelectedValue,
                    Speciesid = (int)DeviceSpecies.SelectedValue,
                    Serialnumber = DeviceSerialNumber.Text,
                    Releasedate = DateOnly.FromDateTime(DeviceReleaseDate.SelectedDate.Value.Date),
                    Lastverificationdate = DateOnly.FromDateTime(DeviceLastverificationDate.SelectedDate.Value.Date)
                };
                db.Measurementdevices.Add(newDevice);
                db.SaveChanges();
            }
            var deviceWindow = new DeviceWindow(newDevice);
            deviceWindow.Owner = this.Owner;
            deviceWindow.Show();
            DialogResult = true;
        }

        //Закрыть окно
        public void CancelButton_Click( object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private bool _isUpdatingControls;
        private void DeviceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingControls)
                return;
            if (DeviceType.SelectedValue == null)
                return;

            int typeId = (int)DeviceType.SelectedValue;

            using (var db = new PostgresContext())
            {
                var type = db.Devicetypes
                            .FirstOrDefault(t => t.Id == typeId);
                if (type != null)
                    _isUpdatingControls = true;
                    DeviceSpecies.SelectedValue = type.Speciesid;
                    _isUpdatingControls = false;
            }
        }

        private void DeviceSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingControls)
                return;

            if (DeviceSpecies.SelectedValue == null)
                return;
            using (var db = new PostgresContext())
            {
                var types = db.Devicetypes
                            .Where(s => s.Speciesid == (int)DeviceSpecies.SelectedValue)
                            .ToList();
                DeviceType.ItemsSource = types;
            }
        }
    }
}
