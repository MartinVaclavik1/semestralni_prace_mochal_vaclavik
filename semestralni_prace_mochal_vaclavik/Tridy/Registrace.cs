using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using semestralni_prace_mochal_vaclavik.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public partial class Registrace : ObservableValidator
    {
        public PrihlasenyUzivatelService PrihlasenyUzivatelService { get; set; }

        public Registrace(PrihlasenyUzivatelService prihlasenyUzivatelService)
        {
            ValidateAllProperties();
            PrihlasenyUzivatelService = prihlasenyUzivatelService;
        }



        [Required, RegularExpression(@"^[\p{L}\s-]+$", ErrorMessage = "Lze vložit jen písmena")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string jmeno;

        [Required, RegularExpression(@"^[\p{L}\s-]+$", ErrorMessage = "Lze vložit jen písmena")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string prijmeni;


        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage = "Lze vložit jen čísla")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public int? cisloOP;

        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage = "Lze vložit jen čísla"), Length(5, 5)]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string pSC;


        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string ulice;

        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage = "Lze vložit jen čísla")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public int? cisloPopisne;

        [Required, RegularExpression(@"^[\p{L}\s-]+$", ErrorMessage = "Lze vložit jen písmena")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string obec;

        [Required, RegularExpression(@"^[\p{L}\s-]+$", ErrorMessage = "Lze vložit jen písmena")]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string zeme;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string username;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistrovatCommand))]
        public string heslo;
        public void Clear()
        {
            Jmeno = string.Empty;
            Prijmeni = string.Empty;
            CisloOP = null;
            PSC = string.Empty;
            Ulice = string.Empty;
            CisloPopisne = null;
            Obec = string.Empty;
            Zeme = string.Empty;
            Username = string.Empty;
            Heslo = string.Empty;

        }

        [RelayCommand(CanExecute = nameof(ZkontrolujRegistraci))]
        public void Registrovat()
        {
            try
            {
                var reg = new Registrace(PrihlasenyUzivatelService)
                {
                    Jmeno = Jmeno,
                    CisloOP = CisloOP,
                    CisloPopisne = CisloPopisne,
                    Obec = Obec,
                    Ulice = Ulice,
                    Heslo = Heslo,
                    Prijmeni = Prijmeni,
                    PSC = PSC,
                    Username = Username,
                    Zeme = Zeme
                };
                PrihlasenyUzivatelService.Registrovat(reg);

                PrihlasenyUzivatelService.Prihlas(Username, Heslo);
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ZkontrolujRegistraci()
        {
            return !HasErrors;
        }
    }
}
