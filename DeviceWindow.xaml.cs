using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace MetryxWPF
{
    /// <summary>
    /// Логика взаимодействия для DeviceWindow.xaml
    /// </summary>
    public partial class DeviceWindow : Window
    {
        public DeviceWindow(Measurementdevice device)
        {
            InitializeComponent();
            DataContext = device;

            switch (Session.CurrentUser.RoleId)
            {
                case 1:
                    SaveButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                    break;
                case 2:
                    SaveButton.Visibility = Visibility.Collapsed;
                    DeleteButton.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    SaveButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            using (PostgresContext db = new PostgresContext())
            {
                var types = db.Devicetypes.ToList();
                var species = db.Species.ToList();

                DeviceSpecies.ItemsSource = species;
                DeviceType.ItemsSource = types;
            }

            DeviceSpecies.SelectedValue = device.Speciesid;
            DeviceType.SelectedValue = device.Typeid;

            DeviceReleaseDate.SelectedDate = device.Releasedate.ToDateTime(TimeOnly.MinValue);
            DeviceLastVerificationDate.SelectedDate = device.Lastverificationdate.ToDateTime(TimeOnly.MinValue);
            DeviceNextVerificationDate.SelectedDate = device.Nextverificationdate.HasValue ? device.Nextverificationdate.Value.ToDateTime(TimeOnly.MinValue) : null;
            LoadDocuments();
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(DeviceName.Text) ||
                DeviceType.SelectedValue == null || string.IsNullOrWhiteSpace(DeviceSerialnumber.Text) ||
                DeviceReleaseDate.SelectedDate == null ||
                DeviceLastVerificationDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            var device = DataContext as Measurementdevice;

            using (PostgresContext db = new PostgresContext())
            {
                var existingDevice = db.Measurementdevices
                    .First(d => d.Id == device.Id);

                existingDevice.Name = DeviceName.Text;
                existingDevice.Typeid = (int)DeviceType.SelectedValue;
                existingDevice.Speciesid = (int)DeviceSpecies.SelectedValue;
                existingDevice.Serialnumber = DeviceSerialnumber.Text;

                existingDevice.Releasedate =
                    DateOnly.FromDateTime(DeviceReleaseDate.SelectedDate.Value);

                existingDevice.Lastverificationdate =
                    DateOnly.FromDateTime(DeviceLastVerificationDate.SelectedDate.Value);

                existingDevice.Verificationinterval =
                    Convert.ToInt32(DeviceVerificationInterval.Text);

                existingDevice.Nextverificationdate =
                    DateOnly.FromDateTime(DeviceNextVerificationDate.SelectedDate.Value);

                existingDevice.Installationlocation =
                    DeviceInstallationLocation.Text;

                existingDevice.Responsible =
                    DeviceResponsible.Text;

                existingDevice.Note =
                    DeviceNote.Text;

                existingDevice.Suitable = Suitable.IsChecked.Value;

                db.SaveChanges();
            }
        }
        public void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new ConfirmationWindow();
            if (deleteWindow.ShowDialog() == true)
            {
                var device = DataContext as Measurementdevice;
                using (PostgresContext db = new PostgresContext())
                {
                    var deletingDevice = db.Measurementdevices.First(d => d.Id == device.Id);

                    db.Measurementdevices.Remove(deletingDevice);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно удалить данный прибор тип т.к. существуют зависимые записи");
                        return;
                    }
                    Close();
                }
            }
            else return;
        }
        public void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var device = DataContext as Measurementdevice;

                string documentsFolder =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                 "Documents",
                                 device.Id.ToString());

                Directory.CreateDirectory(documentsFolder);

                string fileName = Path.GetFileName(dialog.FileName);

                string destinationPath =
                    Path.Combine(documentsFolder, fileName);

                File.Copy(dialog.FileName, destinationPath, true);

                using (PostgresContext db = new PostgresContext())
                {
                    Document document = new Document()
                    {
                        Measurementdeviceid = device.Id,
                        Filename = fileName,
                        Filepath = destinationPath
                    };
                    db.Documents.Add(document);
                    db.SaveChanges();
                }

                LoadDocuments();
            }
        }
        private void LoadDocuments()
        {
            var device = DataContext as Measurementdevice;

            using (PostgresContext db = new PostgresContext())
            {
                DocumentsListBox.ItemsSource = db.Documents
                    .Where(d => d.Measurementdeviceid == device.Id)
                    .ToList();
            }
        }
        private void DownloadDocument_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Document document = button.Tag as Document;

            Process.Start(new ProcessStartInfo
            {
                FileName = document.Filepath,
                UseShellExecute = true
            });
        }
        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Document document = button.Tag as Document;

            if (File.Exists(document.Filepath))
            {
                File.Delete(document.Filepath);
            }

            using (PostgresContext db = new PostgresContext())
            {
                db.Documents.Remove(document);

                db.SaveChanges();
            }

            LoadDocuments();
        }

        private void DeviceNextVerificationDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DeviceVerificationInterval.Text != "" && DeviceVerificationInterval.Text != " ")
                DeviceNextVerificationDate.SelectedDate = DeviceLastVerificationDate.SelectedDate.Value.AddMonths(Convert.ToInt32(DeviceVerificationInterval.Text));
            else
                MessageBox.Show("Введите межповерочный интервал");
        }

        private void DeviceSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new PostgresContext())
            {
                var types = db.Devicetypes
                            .Where(s => s.Speciesid == (int)DeviceSpecies.SelectedValue)
                            .ToList();
                DeviceType.ItemsSource = types;
            }
        }

        private void DeviceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using(var db = new PostgresContext())
            {
                Devicetype type = db.Devicetypes
                            .Where(t => t.Id == DeviceType.SelectedIndex) as Devicetype;

                DeviceSpecies.SelectedValue = type.Speciesid;
            }
        }
    }
}
