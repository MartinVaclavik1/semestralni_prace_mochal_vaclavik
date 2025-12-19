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
    public partial class EvidencePrestupkuViewModel : ObservableObject
    {
        private readonly IEvidencePrestupkuService service;

        [ObservableProperty]
        public ObservableCollection<Prestupek> prestupky = new();

        [ObservableProperty]
        private List<string> typy_prestupkuSeznam = new();

        [ObservableProperty]
        private string novyPrestupekUlice;
        [ObservableProperty]
        private int novyPrestupekCP;
        [ObservableProperty]
        private string novyPrestupekObec;
        [ObservableProperty]
        private string novyPrestupekPSC;
        [ObservableProperty]
        private string vybranyTypPrestupku;
        [ObservableProperty]
        private string novyPrestupekPopisZasahu;
        [ObservableProperty]
        private string novyPrestupekJmenoObcana;

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
        [RelayCommand]
        public async Task PridatPrestupekAsync()
        {
            if (string.IsNullOrWhiteSpace(NovyPrestupekUlice) ||
                //NovyPrestupekCP != 0 ||
                string.IsNullOrWhiteSpace(NovyPrestupekObec) ||
                string.IsNullOrWhiteSpace(NovyPrestupekPSC) ||
                string.IsNullOrWhiteSpace(VybranyTypPrestupku) ||
                string.IsNullOrWhiteSpace(NovyPrestupekPopisZasahu) ||
                string.IsNullOrWhiteSpace(NovyPrestupekJmenoObcana)
                )
            {
                MessageBox.Show("Prosím vyplňte všechna pole.");
                return;
            }

            try
            {

                await service.PridatPrestupekAsync(NovyPrestupekUlice, NovyPrestupekCP, NovyPrestupekObec, NovyPrestupekPSC, VybranyTypPrestupku, NovyPrestupekPopisZasahu, NovyPrestupekJmenoObcana);

                await LoadAsync();

                NovyPrestupekUlice = string.Empty;
                NovyPrestupekCP = 0;
                NovyPrestupekObec = string.Empty;
                NovyPrestupekPSC = string.Empty;
                VybranyTypPrestupku = string.Empty;
                NovyPrestupekPopisZasahu = string.Empty;
                NovyPrestupekJmenoObcana = string.Empty;

                MessageBox.Show("Přestupek byl úspěšně přidán.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání: {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task OdebratPrestupekAsync(Prestupek prestupek)
        {
            if (prestupek == null)
            {
                MessageBox.Show("Prosím vyberte přestupek k odebrání.");
                return;
            }
            try
            {
                await service.OdebratPrestupekAsync(prestupek);
                await LoadAsync();
                MessageBox.Show("Přestupek byl úspěšně odebrán.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při odebírání: {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task UpravitPrestupekAsync(Prestupek prestupek)
        {
            if (prestupek == null)
            {
                MessageBox.Show("Prosím vyberte přestupek k úpravě.");
                return;
            }
            try
            {
                await service.UpravitPrestupekAsync(prestupek);
                await LoadAsync();
                MessageBox.Show("Přestupek byl úspěšně upraven.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při úpravě: {ex.Message}");
            }
        }
    }
}
