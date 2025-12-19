using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Registrace : ObservableValidator, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Registrace()
        {
            ValidateAllProperties();
        }

        private string _jmeno;
        
        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lze vložit jen písmena")]
        public string Jmeno
        {
            get => _jmeno;
            set { 
                SetProperty(ref _jmeno, value,true);
                OnPropertyChanged(); }
        }

        private string _prijmeni;
        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lze vložit jen písmena")]
        public string Prijmeni
        {
            get => _prijmeni;
            set { SetProperty(ref _prijmeni,value,true);
                OnPropertyChanged(); }
        }

        private int? _cisloOP;
        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage = "Lze vložit jen čísla")]
        public int? CisloOP
        {
            get => _cisloOP;
            set { SetProperty(ref _cisloOP, value, true);
                OnPropertyChanged(); }
        }

        private string _psc;
        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage ="Lze vložit jen čísla"), Length(5,5)]
        public string PSC
        {
            get => _psc;
            set {
                SetProperty(ref _psc, value, true);    
                OnPropertyChanged(); }
        }

        private string _ulice;
        [Required]
        public string Ulice
        {
            get => _ulice;
            set { SetProperty(ref _ulice, value, true);
                OnPropertyChanged(); }
        }

        private int? _cisloPopisne;
        [Required, RegularExpression(@"^[0-9]+$", ErrorMessage = "Lze vložit jen čísla")]
        public int? CisloPopisne
        {
            get => _cisloPopisne;
            set { SetProperty(ref _cisloPopisne, value, true);
                OnPropertyChanged(); }
        }

        private string _obec;
        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lze vložit jen písmena")]
        public string Obec
        {
            get => _obec;
            set { SetProperty(ref _obec, value, true);
                OnPropertyChanged(); }
        }

        private string _zeme;
        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lze vložit jen písmena")]
        public string Zeme
        {
            get => _zeme;
            set { SetProperty(ref _zeme, value, true);
                OnPropertyChanged(); }
        }

        private string _username;
        [Required]
        public string Username
        {
            get => _username;
            set { SetProperty(ref _username, value, true);
                OnPropertyChanged(); }
        }
        private string _heslo;
        [Required]
        public string Heslo
        {
            get => _heslo;
            set { SetProperty(ref _heslo, value, true); 
                OnPropertyChanged(); }
        }
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
    }
}
