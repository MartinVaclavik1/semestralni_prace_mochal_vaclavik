using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class PolicisteViewModel : ObservableObject
    {
        private readonly IPolicisteService service;

        [ObservableProperty]
        public ObservableCollection<Policista> policiste = new();

        [ObservableProperty]
        private List<string> hodnostiSeznam = new List<string>();

        [ObservableProperty]
        private string jmeno;

        [ObservableProperty]
        private string prijmeni;

        [ObservableProperty]
        private string hodnost;

        [ObservableProperty]
        private string nadrizeny;

        [ObservableProperty]
        private string stanice;

        [ObservableProperty]
        private DateTime datumNarozeni = DateTime.Now.AddYears(-25);

        [ObservableProperty]
        private int plat;

        [ObservableProperty]
        private PrihlasenyUzivatelService prihlasenyUzivatelService;

        public PolicisteViewModel(IPolicisteService service, PrihlasenyUzivatelService prihlasenyUzivatelService)
        {
            this.service = service;
            PrihlasenyUzivatelService = prihlasenyUzivatelService;
            NactiHodnosti();
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

        [RelayCommand]
        public async Task UpravitPolicistu(object radek)
        {
            var policistaRow = radek as Policista;

            if (policistaRow != null)
            {
                try
                {
                    if (policistaRow.Zmenen)
                    {
                        await service.UpravitPolicistu(policistaRow);
                        await LoadAsync();

                        MessageBox.Show("Úprava policisty byla úspěšně provedena.", "Hotovo");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy policisty: " + ex.Message, "Chyba");
                }
            }
        }

        [RelayCommand]
        public async Task OdebratPolicistu(object radek)
        {
            var policistaRow = radek as Policista;

            if (policistaRow != null)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Opravdu chcete záznam smazat??",
                        "Potvrzení smazání",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes) return;

                    await service.OdebratPolicistu(policistaRow);
                    await LoadAsync();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při odebírání policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public async Task PridatPolicistu()
        {
            try
            {
                var novyPolicista = new Policista();

                //Jmeno;
                //string prijmeni = Prijmeni;
                //string hodnost = Hodnost;
                //string nadrizeny = Nadrizeny;
                //string stanice = Stanice;
                //DateTime datumNarozeni = DatumNarozeni;
                //int plat = Plat;
                //novyPolicista.Pridej(conn, jmeno, prijmeni, hodnost, nadrizeny, stanice, plat, datumNarozeni);
                await service.PridejPolicistu(Jmeno, Prijmeni, Hodnost, Nadrizeny, Stanice, Plat, DatumNarozeni);
                await LoadAsync();

                MessageBox.Show("Nový policista byl úspěšně přidán.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nového policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
