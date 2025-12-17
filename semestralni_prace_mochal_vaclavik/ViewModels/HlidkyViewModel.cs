using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class HlidkyViewModel : ObservableObject
    {
        private readonly IHlidkyService service;

        [ObservableProperty]
        public ObservableCollection<Hlidka> hlidky = new();

        [ObservableProperty]
        private List<string> typyHlidkySeznam = new List<string>();

        public HlidkyViewModel(IHlidkyService service)
        {
            this.service = service;
            NactiTypyHlidky();
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Hlidky.Clear();
            var hlidky_nacteni = await service.GetHlidkyAsync();
            foreach (var c in hlidky_nacteni)
                Hlidky.Add(c);
        }

        private void NactiTypyHlidky()
        {
            try
            {
                TypyHlidkySeznam.Clear();
                foreach (var item in service.GetTypyHlidky())
                {
                    TypyHlidkySeznam.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
