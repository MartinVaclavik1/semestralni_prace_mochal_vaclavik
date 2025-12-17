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
    public partial class AdminNastaveniViewModel : ObservableObject
    {
        private readonly IAdminNastaveniService service;

        [ObservableProperty]
        public ObservableCollection<Uzivatel> uzivatele = new();

        [ObservableProperty]
        private List<string> opravneniSeznam = new List<string>();

        public AdminNastaveniViewModel(IAdminNastaveniService service)
        {
            this.service = service;
            NactiHodnosti();
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Uzivatele.Clear();
            var uzivatele_nacteni = await service.GetUzivateleAsync();
            foreach (var c in uzivatele_nacteni)
                Uzivatele.Add(c);
        }

        private void NactiHodnosti()
        {
            try
            {
                OpravneniSeznam.Clear();
                foreach (var item in service.GetOpravneni())
                {
                    OpravneniSeznam.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
