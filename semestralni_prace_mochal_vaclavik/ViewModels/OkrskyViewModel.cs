using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class OkrskyViewModel : ObservableObject
    {
        private readonly IOkrskyService service;

        [ObservableProperty]
        public ObservableCollection<Okrsek> okrsky = new();


        public OkrskyViewModel(IOkrskyService service)
        {
            this.service = service;
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Okrsky.Clear();
            var policiste_nacteni = await service.GetOkrskyAsync();
            foreach (var c in policiste_nacteni)
                Okrsky.Add(c);
        }
    }
}
