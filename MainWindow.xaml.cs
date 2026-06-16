using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
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
public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();

        AlertDevicesGrid.ItemsSource = GetAlertDevices();
        AllDevicesGrid.ItemsSource = GetDevices();
        AllUsersGrid.ItemsSource = GetUsers();
        AllVerificationsGrid.ItemsSource = GetVerifications();
        LoadTypes();
        Notifications();

        switch (Session.CurrentUser.RoleId)
        {
            case 1:
                Users.Visibility = Visibility.Visible;
                DeviceType.Visibility = Visibility.Visible;
                AddButton.Visibility = Visibility.Visible;
                AddVerificationButton.Visibility = Visibility.Visible;
                break;
            case 2:
                Users.Visibility = Visibility.Collapsed;
                AddButton.Visibility = Visibility.Collapsed;
                AddVerificationButton.Visibility = Visibility.Collapsed;
                break;
            case 3:
                Users.Visibility = Visibility.Collapsed;
                AddButton.Visibility = Visibility.Visible;
                AddVerificationButton.Visibility = Visibility.Visible;
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
    private void Notifications()
    {
        using (PostgresContext db = new PostgresContext())
        {
            var expiringDevices = db.Measurementdevices
                .Where(d => d.Nextverificationdate <= DateOnly.FromDateTime(DateTime.Today.AddDays(7)))
                .ToList();
            foreach (var device in expiringDevices)
            {
                Growl.WarningGlobal(new GrowlInfo
                {
                    Message = $"{device.Name} ({device.Serialnumber}) требует поверки до {device.Nextverificationdate:dd.MM.yyyy}",
                    WaitTime = 8
                });
            }
        }
    }

    #region База приборов
    private List<MeasurementDeviceView> GetDevices()
    {
        using (PostgresContext db = new PostgresContext())
        {
            return db.Measurementdevices
                     .Include(d => d.Type)
                     .Include(d => d.Species)
                     .Select(d => new MeasurementDeviceView
                     {
                         Id = d.Id,
                         Name = d.Name,
                         TypeName = d.Type.Name,
                         SpeciesName = d.Species.Name,
                         Serialnumber = d.Serialnumber,
                         Lastverificationdate = d.Lastverificationdate,
                         Nextverificationdate = d.Nextverificationdate,
                         Suitable = d.Suitable
                     })
                     .ToList();
        }
    }

    private List<MeasurementDeviceView> GetSearchedDevices(string searchQuery, int id)
    {
        using (PostgresContext db = new PostgresContext())
        {
            if(id > 0 && !string.IsNullOrEmpty(searchQuery))
            {
                return db.Measurementdevices
                         .Include(d => d.Type)
                         .Include(d => d.Species)
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
                             SpeciesName = d.Species.Name,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate,
                             Suitable = d.Suitable
                         })
                         .ToList();
            }
            if(id == 0 && !string.IsNullOrEmpty(searchQuery))
            {
                return db.Measurementdevices
                         .Include(d => d.Type)
                         .Include(d => d.Species)
                         .Where(d =>
                            EF.Functions.ILike(d.Name, $"%{searchQuery}%") ||
                            EF.Functions.ILike(d.Serialnumber, $"%{searchQuery}%"))
                         .Select(d => new MeasurementDeviceView
                         {
                             Id = d.Id,
                             Name = d.Name,
                             TypeName = d.Type.Name,
                             Serialnumber = d.Serialnumber,
                             SpeciesName = d.Species.Name,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate,
                             Suitable = d.Suitable
                         })
                         .ToList();
            }
            if (id > 0 && !string.IsNullOrEmpty(searchQuery))
            {
                return db.Measurementdevices
                         .Where(d => d.Typeid == id)
                         .Include(d => d.Type)
                         .Include(d => d.Species)
                         .Select(d => new MeasurementDeviceView
                         {
                             Id = d.Id,
                             Name = d.Name,
                             TypeName = d.Type.Name,
                             Serialnumber = d.Serialnumber,
                             SpeciesName = d.Species.Name,
                             Lastverificationdate = d.Lastverificationdate,
                             Nextverificationdate = d.Nextverificationdate,
                             Suitable = d.Suitable
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
        addDeviceWindow.Owner = this;
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
                    window.Owner = this;
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
                   .Include(d => d.Species)
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
                           SpeciesName = d.Species.Name,
                           Lastverificationdate = d.Lastverificationdate,
                           Nextverificationdate = d.Nextverificationdate,
                           VerificationStatus = status,
                           Suitable = d.Suitable,
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
                    .Include(d => d.Species)
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
            if (!string.IsNullOrEmpty(searchQuery))
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
        UserWindow.Owner = this;
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
                    window.Owner = this;
                    window.ShowDialog();
                    AllUsersGrid.ItemsSource = GetSearchedUsers(UsersSearch.Text);
                }
            }
        }
    }
    #endregion

    #region Типы
    private ObservableCollection<Devicetype> _types;
    private List<Species> _species;

    private void LoadTypes()
    {
        using(PostgresContext db = new PostgresContext())
        {
            _types = new ObservableCollection<Devicetype>(
                db.Devicetypes.OrderBy(t => t.Name).ToList());
            _species = db.Species.ToList();

            TypesGrid.ItemsSource = _types;
        }
        ((CollectionViewSource)FindResource("SpeciesSource")).Source = _species;
    }
    private void AddTypeButton_Click(object sender, RoutedEventArgs e)
    {
        var newType = new Devicetype
        {
            Name = ""
        };
        _types.Add(newType);
        TypesGrid.SelectedItem = newType;
        TypesGrid.ScrollIntoView(newType);
    }
    private void TypesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var type = e.Row.Item as Devicetype;

            if (type == null || string.IsNullOrWhiteSpace(type.Name))
                return;

            using (PostgresContext db = new PostgresContext())
            {
                if (type.Id == 0)
                {
                    db.Devicetypes.Add(type);
                }
                else
                {
                    db.Devicetypes.Update(type);
                }

                db.SaveChanges();
            }
        }),
        System.Windows.Threading.DispatcherPriority.Background);
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Devicetype type)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var entity = db.Devicetypes.First(t => t.Id == type.Id);

                db.Devicetypes.Remove(entity);
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show("Невозможно удалить данный тип т.к. существуют зависимые записи");
                    return;
                }
            }

            _types.Remove(type);
        }
    }
    private void TypesGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
    #endregion

    #region Протоколы
    private List<VerificationsView> GetVerifications()
    {
        using (PostgresContext db = new PostgresContext())
        {
            return db.Verifications
                .Include(v => v.Measurementdevice)
                .Select(v => new VerificationsView
                {
                    Id = v.Id,
                    Organization = v.Organization,
                    Certificatenumber = v.Certificatenumber,
                    Verificationdate = v.Verificationdate,
                    Nextverificationdate = v.Nextverificationdate,
                    Suitable = v.Suitable,
                    VSearialnumber = v.Measurementdevice.Serialnumber,
                    VMeasurementdevice = v.Measurementdevice.Name
                })
                .ToList();
        }
    }
    private List<VerificationsView> GetSearchedVerifications(string searchQuery)
    {
        using(PostgresContext db = new PostgresContext())
        {
            if(!string.IsNullOrEmpty(searchQuery))
            {
                return db.Verifications
                    .Include(v => v.Measurementdevice)
                    .Where(v => 
                        EF.Functions.ILike(v.Certificatenumber, $"%{searchQuery}%") ||
                        EF.Functions.ILike(v.Organization, $"%{searchQuery}%") ||
                        EF.Functions.ILike(v.Measurementdevice.Name, $"%{searchQuery}%") ||
                        EF.Functions.ILike(v.Measurementdevice.Serialnumber, $"%{searchQuery}%")
                        )
                    .Select(v => new VerificationsView
                    {
                        Id = v.Id,
                        Organization = v.Organization,
                        Certificatenumber = v.Certificatenumber,
                        Verificationdate = v.Verificationdate,
                        Nextverificationdate = v.Nextverificationdate,
                        Suitable = v.Suitable,
                        VSearialnumber = v.Measurementdevice.Serialnumber,
                        VMeasurementdevice = v.Measurementdevice.Name
                    })
                    .ToList();
            }
            else 
                return GetVerifications();
        }
    }
    private void AddVerificationButton_Click(object sender, RoutedEventArgs e)
    {
        int count = 0;
        using(var db = new PostgresContext())
        {
            count = db.Measurementdevices.Count();
        }
        if(count == 0)
        {
            System.Windows.MessageBox.Show("Невозможно добавить протокол т.к. в базе отсутствуют приборы");
            return;
        }
        Verification verification = new Verification();
        VerificationWindow window = new VerificationWindow(verification);
        window.Owner = this;
        window.ShowDialog();
        AllVerificationsGrid.ItemsSource = GetSearchedVerifications(VerificationSearch.Text);
    }
    private void VerificationSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        AllVerificationsGrid.ItemsSource = GetSearchedVerifications(VerificationSearch.Text);
    }
    private void AllVerificationsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
    private void AllVerificationsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AllVerificationsGrid.SelectedItem is VerificationsView selectedVerify)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var verify = db.Verifications
                    .Include(d => d.Measurementdevice)
                    .FirstOrDefault(d => d.Id == selectedVerify.Id);

                if (verify != null)
                {
                    VerificationWindow window = new VerificationWindow(verify);
                    window.Owner = this;
                    window.ShowDialog();
                    AllVerificationsGrid.ItemsSource = GetSearchedVerifications(UsersSearch.Text);
                }
            }
        }
    }
    #endregion
}