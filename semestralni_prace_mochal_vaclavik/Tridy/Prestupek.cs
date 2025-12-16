using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Prestupek : INotifyPropertyChanged
    {
        private int idObcana { get; set; }
        public int IdObcana
        {
            get
            { return idObcana; }
            set
            {
                idObcana = value;
                this.OnPropertyChanged(nameof(IdObcana));
            }
        }
        
        private string typPrestupku { get; set; }
        public string TypPrestupku
        {
            get { return typPrestupku; }
            set
            {
                typPrestupku = value;
                this.OnPropertyChanged(nameof(TypPrestupku));
            }
        }
        
        private string datum { get; set; }
        public string Datum
        {
            get { return datum; }
            set
            {
                datum = value;
                this.OnPropertyChanged(nameof(Datum));
            }
        }

        private string jmenoObcana { get; set; }
        public string JmenoObcana
        {
            get { return jmenoObcana; }
            set
            {
                jmenoObcana = value;
                this.OnPropertyChanged(nameof(JmenoObcana));
            }
        }

        private string poznamka { get; set; }
        public string Poznamka
        {
            get { return poznamka; }
            set
            {
                poznamka = value;
                this.OnPropertyChanged(nameof(Poznamka));
            }
        }

        public void Resetuj()
        {
            IdObcana = 0;
            TypPrestupku = String.Empty;
            Datum = String.Empty;
            JmenoObcana = String.Empty;
            Poznamka = String.Empty;
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
