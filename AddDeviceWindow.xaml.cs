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

                DeviceType.ItemsSource = types;
            }
        }

        //Добавление записи в БД
        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var newDevice = new Measurementdevice
                {
                    Name = DeviceName.Text,
                    Typeid = (long)DeviceType.SelectedValue,
                    Serialnumber = DeviceSerialNumber.Text,
                    Releasedate = DateOnly.FromDateTime(DeviceReleaseDate.SelectedDate.Value.Date),
                    Lastverificationdate = DateOnly.FromDateTime(DeviceLastverificationDate.SelectedDate.Value.Date)
                };
                db.Measurementdevices.Add(newDevice);
                db.SaveChanges();
            }
            Close();
        }

        //Закрыть окно
        public void CancelButton_Click( object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
