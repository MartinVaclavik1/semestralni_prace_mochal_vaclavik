using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public partial class SpravceOkenService : ObservableObject
    {
        [ObservableProperty]
        private int index;
    }
}
