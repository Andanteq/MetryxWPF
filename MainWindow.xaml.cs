using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetryxWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AlertDevicesGrid.ItemsSource = GetAlertDevices();
        AllDevicesGrid.ItemsSource = GetDevices();
        AllUsersGrid.ItemsSource = GetUsers();

        switch (Session.CurrentUser.RoleId)
        {
            case 1:
                Users.Visibility = Visibility.Visible;
                AddButton.Visibility = Visibility.Visible;
                break;
            case 3:
                AddButton.Visibility = Visibility.Visible;
                break;
            default:
                break;
        }

        using (PostgresContext db = new PostgresContext())
        {
            var types = db.Devicetypes
                        .Select(t => new Devicetype {Id = t.Id, Name = t.Name})
                        .ToList();
            
            types.Insert(0, new Devicetype { Id = 0, Name = "Все типы" });

            filters.ItemsSource = types;
            filters.SelectedIndex = 0;
        }
    }

    #region База приборов
    private List<MeasurementDeviceView> GetDevices()
    {
        using (PostgresContext db = new PostgresContext())
        {
            return db.Measurementdevices
                     .Include(d => d.Type)
                     .Select(d => new MeasurementDeviceView
                     {
                         Id = d.Id,
                         Name = d.Name,
                         TypeName = d.Type.Name,
                         Serialnumber = d.Serialnumber,
                         Verificationinterval = d.Verificationinterval,
                         Lastverificationdate = d.Lastverificationdate,
                         Nextverificationdate = d.Nextverificationdate
                     })
                     .ToList();
        }
    }

    private List<MeasurementDeviceView> GetSearchedDevices(string searchQuery, int id)
    {
        using (PostgresContext db = new PostgresContext())
        {
            if(id > 0 && searchQuery.Length > 0)
            {
                return db.Measurementdevices
                         .Include(d => d.Type)
                         .Where(d =>
                            (EF.Functions.ILike(d.Name, $"%{searchQuery}%") ||
                            EF.Functions.ILike(d.Serialnumber, $"%{searchQuery}%")) &&
                            d.Typeid == id)
                         .Select(d => new MeasurementDeviceView
                         {
                             Id = d.Id,
                             Name = d.Name,
                             TypeName = d.Type.Name,
                             Serialnumber = d.Serialnumber,
                             Verificationinterval = d.Verificationinterval,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate
                         })
                         .ToList();
            }
            if(id == 0 && searchQuery.Length > 0)
            {
                return db.Measurementdevices
                         .Include(d => d.Type)
                         .Where(d =>
                            EF.Functions.ILike(d.Name, $"%{searchQuery}%") ||
                            EF.Functions.ILike(d.Serialnumber, $"%{searchQuery}%"))
                         .Select(d => new MeasurementDeviceView
                         {
                             Id = d.Id,
                             Name = d.Name,
                             TypeName = d.Type.Name,
                             Serialnumber = d.Serialnumber,
                             Verificationinterval = d.Verificationinterval,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate
                         })
                         .ToList();
            }
            if (id > 0 && searchQuery.Length == 0)
            {
                return db.Measurementdevices
                         .Where(d => d.Typeid == id)
                         .Include(d => d.Type)
                         .Select(d => new MeasurementDeviceView
                         {
                             Id = d.Id,
                             Name = d.Name,
                             TypeName = d.Type.Name,
                             Serialnumber = d.Serialnumber,
                             Verificationinterval = d.Verificationinterval,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate
                         })
                         .ToList();
            }
            else
                return GetDevices();
        }
    }

    private void AllDevicesGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddDeviceWindow addDeviceWindow = new AddDeviceWindow();
        if (addDeviceWindow.ShowDialog() == true)
        {
            AllDevicesGrid.ItemsSource = GetSearchedDevices(Search.Text, (int)filters.SelectedValue);
        }
    }

    private void filtersType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AllDevicesGrid.ItemsSource = GetSearchedDevices(Search.Text, (int)filters.SelectedValue);
    }

    private void Search_TextChanged(object sender, RoutedEventArgs e)
    {
        AllDevicesGrid.ItemsSource = GetSearchedDevices(Search.Text, (int)filters.SelectedValue);
    }

    private void AllDevicesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AllDevicesGrid.SelectedItem is MeasurementDeviceView selectedDevice)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var device = db.Measurementdevices
                    .Include(d => d.Type)
                    .FirstOrDefault(d => d.Id == selectedDevice.Id);

                if (device != null)
                {
                    DeviceWindow window = new DeviceWindow(device);
                    window.ShowDialog();
                    AllDevicesGrid.ItemsSource = GetSearchedDevices(Search.Text, (int)filters.SelectedValue);
                }
            }
        }
    }
    #endregion

    #region Главная страница
    private List<MeasurementDeviceView> GetAlertDevices()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        using (var db = new PostgresContext())
        {
            return db.Measurementdevices
                   .Include(d => d.Type)
                   .AsEnumerable()
                   .Select(d =>
                   {
                       var status = VerificationStatus.Normal;
                       if (d.Nextverificationdate.HasValue) {
                           var days = d.Nextverificationdate.Value.DayNumber - today.DayNumber;

                           if (days <= 30)
                               status = VerificationStatus.Critical;
                           else if (days <= 183)
                               status = VerificationStatus.Warning;
                       }

                       return new MeasurementDeviceView
                       {
                           Id = d.Id,
                           Name = d.Name,
                           TypeName = d.Type.Name,
                           Serialnumber = d.Serialnumber,
                           Verificationinterval = d.Verificationinterval,
                           Lastverificationdate = d.Lastverificationdate,
                           Nextverificationdate = d.Nextverificationdate,
                           VerificationStatus = status,
                       }; 
                   })
                   .ToList();
        }
    }
    private void AlertDevicesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AlertDevicesGrid.SelectedItem is MeasurementDeviceView selectedDevice)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var device = db.Measurementdevices
                    .Include(d => d.Type)
                    .FirstOrDefault(d => d.Id == selectedDevice.Id);

                if (device != null)
                {
                    DeviceWindow window = new DeviceWindow(device);
                    window.ShowDialog();
                    AlertDevicesGrid.ItemsSource = GetAlertDevices();
                }
            }
        }
    }
    #endregion

    #region Пользователи
    private List<UsersView> GetUsers()
    {
        using(PostgresContext db = new PostgresContext())
        {
            return db.Users
                   .Include(u => u.Role)
                   .Select(u => new UsersView
                   {
                       Id = u.Id,
                       Lastname = u.Lastname,
                       Firstname = u.Firstname,
                       Middlename = u.Middlename,
                       RoleName = u.Role.Name,
                       Phonenumber = u.Phonenumber
                   })
                   .ToList();
        }
    }
    private List<UsersView> GetSearchedUsers(string searchQuery)
    {
        using(var db = new PostgresContext())
        {
            if (searchQuery.Length > 0)
            {
                return db.Users
                       .Include(u => u.Role)
                       .Where(u =>
                            EF.Functions.ILike(u.Fullname, $"%{searchQuery}%") ||
                            EF.Functions.ILike(u.Role.Name, $"%{searchQuery}%") ||
                            EF.Functions.ILike(u.Phonenumber, $"%{searchQuery}%")
                       )
                       .Select(u => new UsersView
                       {
                           Id = u.Id,
                           Lastname = u.Lastname,
                           Firstname = u.Firstname,
                           Middlename = u.Middlename,
                           RoleName = u.Role.Name,
                           Phonenumber = u.Phonenumber
                       })
                       .ToList();
            }
            else
                return GetUsers();
        }
    }
    private void AllUsersGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
    private void AddUserButton_Click(object sender, RoutedEventArgs e)
    {
        User user = new User();
        UserWindow UserWindow = new UserWindow(user);
        UserWindow.ShowDialog();
        AllUsersGrid.ItemsSource = GetSearchedUsers(UsersSearch.Text);
    }
    private void UsersSearch_TextChanged(object sender, TextChangedEventArgs e) 
    {
        AllUsersGrid.ItemsSource = GetSearchedUsers(UsersSearch.Text);
    }
    private void AllUsersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AllUsersGrid.SelectedItem is UsersView selectedUser)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var user = db.Users
                    .Include(d => d.Role)
                    .FirstOrDefault(d => d.Id == selectedUser.Id);

                if (user != null)
                {
                    UserWindow window = new UserWindow(user);
                    window.ShowDialog();
                    AllUsersGrid.ItemsSource = GetSearchedUsers(UsersSearch.Text);
                }
            }
        }
    }
    #endregion
}