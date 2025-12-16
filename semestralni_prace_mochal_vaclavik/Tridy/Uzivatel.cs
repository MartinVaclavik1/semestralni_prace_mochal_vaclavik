using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Uzivatel : INotifyPropertyChanged
    {
        private int id { get; set; }
        public int Id
        {
            get
            { return id;}
            set
            {
                id = value;
                this.OnPropertyChanged("Id");
            }
        }
        private string jmeno { get; set; }
        public string Jmeno {
            get { return jmeno; }
            set
            {
                jmeno = value;
                this.OnPropertyChanged(nameof(Jmeno));
            }
        }
        private string prijmeni { get; set; }
        public string Prijmeni
        {
            get { return prijmeni; }
            set
            {
                prijmeni = value;
                this.OnPropertyChanged(nameof(Prijmeni));
            }
        }
        private string username { get; set; }
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                this.OnPropertyChanged(nameof(Username));
            }
        }
        private string password { get; set; }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.OnPropertyChanged(nameof(Password));
            }
        }
        private string opravneni { get; set; }
        public string Opravneni
        {
            get { return opravneni; }
            set
            {
                if (opravneni == "obcan" && value != String.Empty)
                {
                    MessageBox.Show("Nelze měnit oprávnění občanovi", "CHYBA");

                }
                else if((opravneni == "administrator" || opravneni == "policista") 
                    && value == "obcan" && value != String.Empty)
                {
                    MessageBox.Show("Nelze měnit oprávnění na občana", "CHYBA");
                }
                else
                {
                    opravneni = value;
                    this.OnPropertyChanged(nameof(Opravneni));
                }

            }
        }
        private BitmapImage? obrazek { get; set; }
        public  BitmapImage? Obrazek
        {
            get { return obrazek; }
            set
            {
                obrazek = value;
                this.OnPropertyChanged("Obrazek");
            }
        }
        public byte[]? ObrazekBytes { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Resetuj()
        {
            Id= 0;
            Jmeno = String.Empty;
            Prijmeni = String.Empty;
            Username = String.Empty;
            Password = String.Empty;
            Opravneni = String.Empty;
            Obrazek = null;
            ObrazekBytes = null;
        }

        private bool zmenen { get; set; }
        public bool Zmenen
        {
            get { return zmenen; }
            set {  zmenen = value; }
            
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                zmenen = true;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
