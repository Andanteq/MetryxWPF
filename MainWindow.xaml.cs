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

        AllDevicesGrid.ItemsSource = GetDevices();

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

    private List<MeasurementDeviceView> GetDevices()
    {
        using (PostgresContext db = new PostgresContext())
        {
            return db.Measurementdevices
                     .Include(d => d.Type)
                     .Select(d => new MeasurementDeviceView
                     {
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

    private List<MeasurementDeviceView> GetSearchedDevices(string searchQuery)
    {
        using (PostgresContext db = new PostgresContext())
        {
            return db.Measurementdevices
                     .Include(d => d.Type)
                     .Where(d => 
                        EF.Functions.ILike(d.Name, $"%{searchQuery}%") ||
                        EF.Functions.ILike(d.Serialnumber, $"%{searchQuery}%"))
                     .Select(d => new MeasurementDeviceView
                     {
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

    private void AllDevicesGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddDeviceWindow addDeviceWindow = new AddDeviceWindow();
        addDeviceWindow.Owner = this;
        addDeviceWindow.ShowDialog();
    }

    private void filtersType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTypeId = (long)filters.SelectedValue;

        if (selectedTypeId == 0)
        {
            AllDevicesGrid.ItemsSource = GetDevices();
        }
        else
        {
            using (PostgresContext db = new PostgresContext())
            {
                AllDevicesGrid.ItemsSource = db.Measurementdevices
                                            .Where(d => d.Typeid == selectedTypeId)
                                            .Include(d => d.Type)
                                            .Select(d => new MeasurementDeviceView
                                            {
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
    }

    private void Search_TextChanged(object sender, RoutedEventArgs e)
    {
        if (Search.Text.Length > 0)
            AllDevicesGrid.ItemsSource = GetSearchedDevices(Search.Text);
        else
            AllDevicesGrid.ItemsSource = GetDevices();
    }
}