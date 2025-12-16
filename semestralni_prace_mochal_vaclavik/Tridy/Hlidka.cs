using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Hlidka : INotifyPropertyChanged
    {
        private int idHlidky { get; set; }
        public int IdHlidky
        {
            get { return idHlidky; }
            set { idHlidky = value; }
        }
        private string nazevHlidky { get; set; }
        public string NazevHlidky
        {
            get { return nazevHlidky; }
            set { nazevHlidky = value; }
        }
        private string nazev { get; set; }
        public string Nazev
        {
            get { return nazev; }
            set { nazev = value; }
        }
        public void Resetuj()
        {
            IdHlidky = 0;
            NazevHlidky = String.Empty;
            Nazev = String.Empty;
        }

        private bool zmenen { get; set; }
        public bool Zmenen
        {
            get { return zmenen; }

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
    }
}
