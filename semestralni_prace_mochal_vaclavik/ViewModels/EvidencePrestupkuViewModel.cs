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
    public partial class EvidencePrestupkuViewModel: ObservableObject
    {
        private readonly IEvidencePrestupkuService service;

        [ObservableProperty]
        public ObservableCollection<Prestupek> prestupky = new();

        [ObservableProperty]
        private List<string> typy_prestupkuSeznam = new();

        public EvidencePrestupkuViewModel(IEvidencePrestupkuService service)
        {
            this.service = service;
            NactiTypyPrestupku();
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Prestupky.Clear();
            var prestupky_nactene = await service.GetPrestupkyAsync();
            foreach (var c in prestupky_nactene)
                Prestupky.Add(c);
        }
        private void NactiTypyPrestupku()
        {
            try
            {
                Typy_prestupkuSeznam.Clear();
                foreach (var item in service.GetTypyPrestupky())
                {
                    Typy_prestupkuSeznam.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
