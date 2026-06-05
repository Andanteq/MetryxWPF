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
    /// Логика взаимодействия для VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : Window
    {
        public VerificationWindow(Verification verification)
        {
            InitializeComponent();

            DataContext = verification;

            using (PostgresContext db = new PostgresContext())
            {
                var types = db.Verificationtypes.ToList();
                VType.ItemSource = types;
            }
            ValueType.SelectedValue = verification.Verificationtypeid;
        }
    }
}
