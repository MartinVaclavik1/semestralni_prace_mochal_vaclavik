using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Registrace : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _jmeno;
        public string Jmeno
        {
            get => _jmeno;
            set { _jmeno = value; OnPropertyChanged(); }
        }

        private string _prijmeni;
        public string Prijmeni
        {
            get => _prijmeni;
            set { _prijmeni = value; OnPropertyChanged(); }
        }

        private int? _cisloOP;
        public int? CisloOP
        {
            get => _cisloOP;
            set { _cisloOP = value; OnPropertyChanged(); }
        }

        private string _psc;
        public string PSC
        {
            get => _psc;
            set { _psc = value; OnPropertyChanged(); }
        }

        private string _ulice;
        public string Ulice
        {
            get => _ulice;
            set { _ulice = value; OnPropertyChanged(); }
        }

        private int? _cisloPopisne;
        public int? CisloPopisne
        {
            get => _cisloPopisne;
            set { _cisloPopisne = value; OnPropertyChanged(); }
        }

        private string _obec;
        public string Obec
        {
            get => _obec;
            set { _obec = value; OnPropertyChanged(); }
        }

        private string _zeme;
        public string Zeme
        {
            get => _zeme;
            set { _zeme = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
        private string _heslo;
        public string Heslo
        {
            get => _heslo;
            set { _heslo = value; OnPropertyChanged(); }
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
