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

        [ObservableProperty]
        private string novyNazevOkrsku;

        public OkrskyViewModel(IOkrskyService service)
        {
            this.service = service;
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Okrsky.Clear();
            var okrsky_nactene = await service.GetOkrskyAsync();
            foreach (var c in okrsky_nactene)
                Okrsky.Add(c);
        }
        [RelayCommand]
        public async Task PridatOkrsekAsync()
        {

            if (string.IsNullOrWhiteSpace(novyNazevOkrsku))
            {
                MessageBox.Show("Prosím vyplňte název okrsku.");
                return;
            }
            try
            {
                await service.PridatOkrsek(novyNazevOkrsku);
                await LoadAsync();
                novyNazevOkrsku = string.Empty;

                MessageBox.Show("Okrsek byla úspěšně přidán.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidávání: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OdebratOkrsekAsync(Okrsek okrsek)
        {
            if (okrsek == null)
            {
                MessageBox.Show("Prosím vyberte okrsek k odstranění.");
                return;
            }
            try
            {
                await service.OdebratOkrsek(okrsek);
                await LoadAsync();
                MessageBox.Show("Okrsek byl úspěšně odstraněn.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při odstraňování: {ex.Message}");
            }

        }

        [RelayCommand]
        public async Task UpravitOkrsekAsync(Okrsek okrsek)
        {
            if (okrsek == null)
            {
                MessageBox.Show("Prosím vyberte okrsek k úpravě.");
                return;
            }
            try
            {
                await service.UpravitOkrsek(okrsek);
                await LoadAsync();
                MessageBox.Show("Okrsek byl úspěšně upraven.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při úpravě: {ex.Message}");
            }
        }
    }
}
