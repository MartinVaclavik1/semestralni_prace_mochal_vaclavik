using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindowViewController : ObservableObject
    {
        [RelayCommand]
        public void Emulovat()
        {
            MessageBox.Show("Test");
        }
    }
}
