using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class PolicisteViewModel : ObservableObject
    {
        private readonly IPolicisteService service;

        [ObservableProperty]
        public ObservableCollection<Policista> policiste = new();

        [ObservableProperty]
        private List<string> hodnostiSeznam = new List<string>();

        public PolicisteViewModel(IPolicisteService service)
        {
            this.service = service;
            NactiHodnosti();
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Policiste.Clear();
            var policiste_nacteni = await service.GetPolicisteAsync();
            foreach (var c in policiste_nacteni)
                Policiste.Add(c);
        }   

        private void NactiHodnosti()
        {
            try
            {
                HodnostiSeznam.Clear();
                foreach (var item in service.GetHodnosti())
                {
                    HodnostiSeznam.Add(item);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
