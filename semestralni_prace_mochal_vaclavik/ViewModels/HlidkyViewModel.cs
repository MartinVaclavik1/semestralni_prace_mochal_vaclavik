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

        [ObservableProperty]
        private string novyNazevHlidky;

        [ObservableProperty]
        private string vybranyTypHlidky;

        public HlidkyViewModel(IHlidkyService service)
        {
            this.service = service;
            NactiTypyHlidky();
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
        [RelayCommand]
        public async Task PridatHlidkuAsync()
        {

            if (string.IsNullOrWhiteSpace(NovyNazevHlidky) || string.IsNullOrWhiteSpace(VybranyTypHlidky))
            {
                MessageBox.Show("Prosím vyplňte název i typ hlídky.");
                return;
            }

            try
            {

                await service.PridatHlidku(NovyNazevHlidky, VybranyTypHlidky);

                await LoadAsync();

                NovyNazevHlidky = string.Empty;
                VybranyTypHlidky = string.Empty;

                MessageBox.Show("Hlídka byla úspěšně přidána.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OdebratHlidkuAsync(Hlidka hlidka)
        {
            if (hlidka == null)
            {
                MessageBox.Show("Prosím vyberte hlídku k odstranění.");
                return;
            }
            try
            {
                await service.OdebratHlidku(hlidka);
                await LoadAsync();
                MessageBox.Show("Hlídka byla úspěšně odstraněna.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při odstraňování: {ex.Message}");
            }

        }

        [RelayCommand]
        public async Task UpravitHlidkuAsync(Hlidka hlidka)
        {
            if (hlidka == null)
            {
                MessageBox.Show("Prosím vyberte hlídku k úpravě.");
                return;
            }
            try
            {
                await service.UpravitHlidku(hlidka);
                await LoadAsync();
                MessageBox.Show("Hlídka byla úspěšně upravena.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při úpravě: {ex.Message}");
            }
        }
    }
}
