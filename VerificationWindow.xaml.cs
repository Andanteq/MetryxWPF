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
using System.Windows.Shapes;

namespace MetryxWPF
{
    /// <summary>
    /// Логика взаимодействия для VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : Window
    {
        private int? _selectedDeviceId;
        public VerificationWindow(Verification verification)
        {
            InitializeComponent();

            DataContext = verification;

            using (PostgresContext db = new PostgresContext())
            {
                var types = db.Verificationtypes.ToList();
                VType.ItemsSource = types;
            }
            _selectedDeviceId = verification.Measurementdeviceid;
            DeviceTextBox.Text = verification.Measurementdevice.Name;
            DeviceSerialNumber.Text = verification.Measurementdevice.Serialnumber;
            VType.SelectedValue = verification.Verificationtypeid;
            Verificationdate.SelectedDate = verification.Verificationdate.ToDateTime(TimeOnly.MinValue);
            Nextverificationdate.SelectedDate = verification.Nextverificationdate.ToDateTime(TimeOnly.MinValue);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Organization.Text) || string.IsNullOrEmpty(Certificatenumber.Text) ||
                Verificationdate.SelectedDate == null || Nextverificationdate.SelectedDate == null)
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }
            if (Nextverificationdate.SelectedDate <= Verificationdate.SelectedDate) {
                MessageBox.Show("Дата следующей поверки не может быть меньше или равна текущей");
                return;
            }
            var verify = DataContext as Verification;

            using (PostgresContext db = new PostgresContext())
            {
                if(verify.Id == 0)
                {
                    if(!CertificateNumberValidation(Certificatenumber.Text))
                    {
                        MessageBox.Show("Поверка с таким номером свидетельства/извещения уже существует");
                        return;
                    }
                    db.Verifications.Add(InsertVerificationData(verify));
                    db.SaveChanges();
                }
                else
                {
                    var existingVerify = db.Verifications.First(v => v.Id == verify.Id);
                    if (!CertificateNumberValidation(Certificatenumber.Text) && existingVerify.Certificatenumber != Certificatenumber.Text)
                    {
                        MessageBox.Show("Поверка с таким номером свидетельства/извещения уже существует");
                        return;
                    }
                    InsertVerificationData(existingVerify);
                    db.SaveChanges();
                }
            }
        }
        private void DeleteButton_Click(Object sender, RoutedEventArgs e)
        {
            var deleteWindow = new ConfirmationWindow();
            if (deleteWindow.ShowDialog() == true)
            {
                var verify = DataContext as Verification;
                using (PostgresContext db = new PostgresContext())
                {
                    var deletingVerify = db.Verifications.First(d => d.Id == verify.Id);

                    db.Verifications.Remove(deletingVerify);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно удалить данную поверку т.к. существуют зависимые записи");
                        return;
                    }
                    Close();
                }
            }
            else return;
        }
        private bool CertificateNumberValidation(string certificateNumber)
        {
            using(PostgresContext db = new PostgresContext())
            {
                var verify = db.Verifications.FirstOrDefault(x => x.Certificatenumber == certificateNumber);
                if (verify != null) return false;
                else return true;
            }
        }
        private Verification InsertVerificationData(Verification verify)
        {
            verify.Organization = Organization.Text;
            verify.Certificatenumber = Certificatenumber.Text;
            verify.Verificationdate = DateOnly.FromDateTime(Verificationdate.SelectedDate.Value);
            verify.Nextverificationdate = DateOnly.FromDateTime(Nextverificationdate.SelectedDate.Value);
            verify.Suitable = Suitable.IsChecked.Value;
            verify.Verificationtypeid = (int)VType.SelectedValue;
            verify.Measurementdeviceid = (int)_selectedDeviceId;

            return verify;
        }
        private void SelectDevice_Click(object sender, RoutedEventArgs e)
        {
            SelectDeviceWindow window = new SelectDeviceWindow();

            if (window.ShowDialog() == true)
            {
                DeviceTextBox.Text = window.SelectedDevice.Name;
                DeviceSerialNumber.Text = window.SelectedDevice.Serialnumber;
                _selectedDeviceId = window.SelectedDevice.Id;
            }
        }
        public void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var verify = DataContext as Verification;

                string documentsFolder =
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                 "Documents",
                                 verify.Id.ToString());

                Directory.CreateDirectory(documentsFolder);

                string fileName = System.IO.Path.GetFileName(dialog.FileName);

                string destinationPath =
                    System.IO.Path.Combine(documentsFolder, fileName);

                File.Copy(dialog.FileName, destinationPath, true);

                using (PostgresContext db = new PostgresContext())
                {
                    Document document = new Document()
                    {
                        Verificationid = verify.Id,
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
            var verify = DataContext as Verification;

            using (PostgresContext db = new PostgresContext())
            {
                DocumentsListBox.ItemsSource = db.Documents
                    .Where(d => d.Verificationid == verify.Id)
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
