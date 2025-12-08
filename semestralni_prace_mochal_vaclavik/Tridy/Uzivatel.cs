using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                this.OnPropertyChanged("Jmeno");
            }
        }
        private string prijmeni { get; set; }
        public string Prijmeni
        {
            get { return prijmeni; }
            set
            {
                prijmeni = value;
                this.OnPropertyChanged("Prijmeni");
            }
        }
        private string username { get; set; }
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                this.OnPropertyChanged("Username");
            }
        }
        private string password { get; set; }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.OnPropertyChanged("Password");
            }
        }
        private string opravneni { get; set; }
        public string Opravneni
        {
            get { return opravneni; }
            set
            {
                opravneni = value;
                this.OnPropertyChanged("Opravneni");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
