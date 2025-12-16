using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Okrsek : INotifyPropertyChanged
    {
        private int id { get; set; }
        public int Id
        {
            get
            { return id; }
            set
            {
                id = value;
                this.OnPropertyChanged(nameof(Id));
            }
        }
        private string nazev { get; set; }
        public string Nazev
        {
            get { return nazev; }
            set
            {
                nazev = value;
                this.OnPropertyChanged(nameof(nazev));
            }
        }
        private bool zmenen { get; set; }
        public bool Zmenen
        {
            get { return zmenen; }

        }

        public void Resetuj()
        {
            Id = 0;
            Nazev = String.Empty;
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                zmenen = true;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Uloz()
        {
            zmenen = false;
        }
    }
}
