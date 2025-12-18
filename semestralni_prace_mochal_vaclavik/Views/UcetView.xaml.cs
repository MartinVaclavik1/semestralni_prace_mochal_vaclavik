using semestralni_prace_mochal_vaclavik.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace semestralni_prace_mochal_vaclavik.Views
{
    /// <summary>
    /// Interakční logika pro UcetView.xaml
    /// </summary>
    public partial class UcetView : System.Windows.Controls.UserControl
    {
        public UcetView()
        {
            InitializeComponent();
        }
        private void HesloTxt_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Uzivatel.Password =
                ((PasswordBox)sender).Password;
        }
    }
}
