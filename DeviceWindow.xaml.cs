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

            DeviceType.SelectedIndex = device.Typeid;
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
