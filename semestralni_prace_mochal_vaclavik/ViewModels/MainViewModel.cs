using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using semestralni_prace_mochal_vaclavik.Views;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    /// <summary>
    /// Hlavní ViewModel pro aplikaci.
    /// Spravuje přihlášení, registraci, správu uživatelů a zobrazení dat na základě rolí.
    /// </summary>
    /// <remarks>
    /// Třída obsluhuje komunikaci s databází Oracle, správu přihlášeného uživatele a dynamické zobrazení UI prvků
    /// podle uživatelských oprávnění (obcan, policista, administrator).
    /// 
    /// <note>Heslo a údaje o připojení jsou hardcodovány.</note>
    /// </remarks>
    public partial class MainViewModel : ObservableObject
    {

        /// <summary>
        /// Data pro nový účet během registrace.
        /// </summary>
        [ObservableProperty]
        private Registrace novaRegistrace = new Registrace();

        public PolicisteView PolicisteView { get; }
        public OkrskyView OkrskyView { get; }
        public EvidencePrestupkuView EvidencePrestupkuView { get; }
        public AdminView AdminView { get; }
        public HlidkyView HlidkyView { get; }
        public PrihlaseniView PrihlaseniView { get; }
        public UcetView UcetView { get; }
        public LogovaciTabulkaView LogovaciTabulkaView { get; }
        public MojePrestupkyView MojePrestupkyView { get; }
        public SystemovyKatalogView SystemovyKatalogView { get; }

        [ObservableProperty]
        public PrihlasenyUzivatelService prihlasenyUzivatelService;

        [ObservableProperty]
        private int vybranyIndex = 0;

        [ObservableProperty]
        private string prihlasovaciJmeno;

        [ObservableProperty]
        private string heslo;

        // Přihlášení 
        //wallis45548 - policista
        //martin25922 - obcan => hesla jsou stejné číslo
        //uzivatel.Id = 80;
        //uzivatel.Opravneni = "administrator";

        /// <summary>
        /// Inicializuje novou instanci MainViewModel.
        /// </summary>
        /// <param name="window">Referenční na hlavní okno aplikace</param>
        /// <remarks>
        /// Nastavuje inicializace databázového připojení a automaticky přihlašuje testovacího uživatele.
        /// </remarks>
        /// public PolicisteViewModel PolicisteVM { get; }

        public MainViewModel(PolicisteView policisteView, OkrskyView okrskyView,
            EvidencePrestupkuView evidencePrestupkuView, AdminView adminView,
            HlidkyView hlidkyView, PrihlaseniView prihlaseniView,
            UcetView ucetView, LogovaciTabulkaView logovaciTabulkaView,
            PrihlasenyUzivatelService prihlasenyUzivatelService,
            MojePrestupkyView mojePrestupkyView, SystemovyKatalogView systemovyKatalogView)
        {
            PolicisteView = policisteView ?? throw new ArgumentNullException(nameof(policisteView));
            OkrskyView = okrskyView ?? throw new ArgumentNullException(nameof(okrskyView));
            EvidencePrestupkuView = evidencePrestupkuView ?? throw new ArgumentNullException(nameof(evidencePrestupkuView));
            AdminView = adminView ?? throw new ArgumentNullException(nameof(adminView));
            HlidkyView = hlidkyView ?? throw new ArgumentNullException(nameof(hlidkyView));
            PrihlaseniView = prihlaseniView ?? throw new ArgumentNullException(nameof(prihlaseniView));
            UcetView = ucetView ?? throw new ArgumentNullException(nameof(ucetView));
            LogovaciTabulkaView = logovaciTabulkaView ?? throw new ArgumentNullException(nameof(logovaciTabulkaView));

            PrihlasenyUzivatelService = prihlasenyUzivatelService;

            MojePrestupkyView = mojePrestupkyView ?? throw new ArgumentNullException(nameof(mojePrestupkyView));
            SystemovyKatalogView = systemovyKatalogView ?? throw new ArgumentNullException(nameof(systemovyKatalogView));
        }

        private void Prihlas(string jmeno, string heslo)
        {
            PrihlasovaciJmeno = jmeno;
            Heslo = heslo;
            Prihlas();
        }
        /// <summary>
        /// Přihlašuje uživatele do systému na základě přihlašovacích údajů.
        /// </summary>
        /// <param name="udaje">Tuple obsahující přihlašovací jméno a heslo</param>
        /// <remarks>
        /// Komunikuje s databází, načítá uživatelské údaje a profilový obrázek.
        /// Automaticky nastavuje viditelnost UI prvků podle role uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        [RelayCommand]
        private void Prihlas()
        {
            try
            {
                PrihlasenyUzivatelService.Prihlas(PrihlasovaciJmeno, Heslo);

                VybranyIndex = 0;

                PrihlasovaciJmeno = String.Empty;
                PrihlaseniView.PasswordBox.Clear();
                Heslo = String.Empty;

                Task.Run(() => { MessageBox.Show("Uživatel přihlášen"); });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatele: " + ex.Message);
            }
        }


        /// <summary>
        /// Odhlašuje aktuálně přihlášeného uživatele z systému.
        /// </summary>
        /// <remarks>
        /// Resetuje data uživatele a skrývá všechny prvky UI kromě přihlašovacího formuláře Registrace a karty Domu.
        /// </remarks>
        [RelayCommand]
        private void Odhlas()
        {
            PrihlasenyUzivatelService.Odhlas();
            VybranyIndex = 0;
        }

        /// <summary>
        /// Registruje nového občana v systému.
        /// </summary>
        /// <remarks>
        /// Volá uloženou proceduru vytvor_uzivatele_obcana s údaji ze formuláře.
        /// Po úspěšné registraci automaticky přihlašuje nového uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě databáze</exception>
        [RelayCommand(CanExecute = nameof(ZkontrolujRegistraci))]
        private void Registrovat()
        {
            try
            {
                PrihlasenyUzivatelService.Registrovat(NovaRegistrace);

                MessageBox.Show($"Uživatel {NovaRegistrace.Username} vytvořen.", "Registrace", MessageBoxButton.OK, MessageBoxImage.Information);
                Prihlas(NovaRegistrace.Username, NovaRegistrace.Heslo);
                NovaRegistrace.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ZkontrolujRegistraci()
        {
            return !NovaRegistrace.HasErrors;
        }

        /// <summary>
        /// Aktualizuje údaje přihlášeného uživatele v databázi (sekce "Můj účet").
        /// </summary>
        /// <remarks>
        /// Aktualizuje heslo, uživatelské jméno a profilový obrázek voláním procedury aktualizuj_ucet.
        /// Dále aktualizuje jméno a příjmení voláním příslušné procedury (obcan/policista).
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě databáze</exception>
        [RelayCommand]
        private void AktualizovatUcet()
        {
            try
            {
                PrihlasenyUzivatelService.AktualizovatUcet();
                UcetView.HesloTxt.Clear();
                MessageBox.Show($"Účet aktualizován.", "Aktualizace", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        /// <summary>
        /// Otevře dialog pro výběr a nahrání profilového obrázku.
        /// </summary>
        /// <remarks>
        /// Přijímá formáty JPG, JPEG a PNG. Obrázek se uloží v paměti a bude commitován při úpravě účtu.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě čtení souboru</exception>
        [RelayCommand]
        private void NahratImg()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Obrázky (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|Všechny soubory (*.*)|*.*";

            try
            {
                openFileDialog.ShowDialog();
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                PrihlasenyUzivatelService.NastavObrazekZBytes(imageBytes);

                MessageBox.Show("Profilový obrázek byl úspěšně nahrán a uložen.", "Úspěch");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při nahrávání obrázku: {ex.Message}", "Chyba");
            }
        }

        /// <summary>
        /// Odstraňuje profilový obrázek přihlášeného uživatele.
        /// </summary>
        /// <remarks>
        /// Nastavuje vlastnosti Obrazek a ObrazekBytes na null. Změna se commituje při úpravě účtu.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě operace</exception>
        [RelayCommand]
        private void OdebratImg()
        {
            if (PrihlasenyUzivatelService.Uzivatel.Obrazek != null)
            {
                try
                {

                    PrihlasenyUzivatelService.OdeberObrazek();

                    MessageBox.Show("Profilový obrázek byl úspěšně odstraněn.", "Úspěch");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při odstraňování obrázku: {ex.Message}", "Chyba");
                }
            }
        }

    }
}