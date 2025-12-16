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

        private int _idPrestupku { get; set; }
        public int IdPrestupku
        {
            get
            { return _idPrestupku; }
            set
            {
                _idPrestupku = value;
                this.OnPropertyChanged(nameof(IdPrestupku));
            }
        }
        private int _idObcana { get; set; }
        public int IdObcana
        {
            get
            { return _idObcana; }
            set
            {
                _idObcana = value;
                this.OnPropertyChanged(nameof(IdObcana));
            }
        }
        
        private string _typPrestupku { get; set; }
        public string TypPrestupku
        {
            get { return _typPrestupku; }
            set
            {
                _typPrestupku = value;
                this.OnPropertyChanged(nameof(TypPrestupku));
            }
        }
        
        private string _datum { get; set; }
        public string Datum
        {
            get { return _datum; }
            set
            {
                _datum = value;
                this.OnPropertyChanged(nameof(Datum));
            }
        }

        private string _jmenoObcana { get; set; }
        public string JmenoObcana
        {
            get { return _jmenoObcana; }
            set
            {
                _jmenoObcana = value;
                this.OnPropertyChanged(nameof(JmenoObcana));
            }
        }

        private string _poznamka { get; set; }
        public string Poznamka
        {
            get { return _poznamka; }
            set
            {
                _poznamka = value;
                this.OnPropertyChanged(nameof(Poznamka));
            }

        }
        private string _adresaZasahu { get; set; }
        public string AdresaZasahu
        {
            get { return _adresaZasahu; }
            set
            {
                _adresaZasahu = value;
                this.OnPropertyChanged(nameof(AdresaZasahu));
            }
        }
        private string _popisZasahu { get; set; }
        public string PopisZasahu
        {
            get { return _popisZasahu; }
            set
            {
                _popisZasahu = value;
                this.OnPropertyChanged(nameof(PopisZasahu));
            }
        }

        public void Resetuj()
        {
            IdPrestupku = 0;
            IdObcana = 0;
            TypPrestupku = String.Empty;
            Datum = String.Empty;
            JmenoObcana = String.Empty;
            Poznamka = String.Empty;
            AdresaZasahu = String.Empty;
            PopisZasahu = String.Empty;
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

        public void Uloz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_prestupku.upravitPrestupek";
            using(OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_idPrestupku", OracleDbType.Varchar2).Value = _idPrestupku;
                cmd.Parameters.Add("p_nazevPrestupku", OracleDbType.Varchar2).Value = _typPrestupku;
                cmd.Parameters.Add("p_datumZasahu", OracleDbType.Varchar2).Value = _datum;
                cmd.Parameters.Add("p_jmenoObcana", OracleDbType.Varchar2).Value = _jmenoObcana;
                cmd.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = _poznamka;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
                zmenen = false;
            }
        }
        public void Smaz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_prestupku.smazatPrestupek";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_idPrestupku", OracleDbType.Varchar2).Value = _idPrestupku;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }

        // TODO : opravit ve view nebo v DB aby to šlo vložit (nefunkční) 
        // není tam políčko na popis zasahu
        public void Pridej(OracleConnection conn, string typPrestupku, string popisZasahu, string jmenoObcana, string adresa, string poznamka)
        {
            string storedProcedureName = "upravy_prestupku.pridatPrestupek";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_adresaZasahu", OracleDbType.Varchar2).Value = adresa;
                cmd.Parameters.Add("p_popisZasahu", OracleDbType.Varchar2).Value = popisZasahu;
                cmd.Parameters.Add("p_typPrestupku", OracleDbType.Varchar2).Value = typPrestupku;
                cmd.Parameters.Add("p_jmenoObcana", OracleDbType.Varchar2).Value = jmenoObcana;
                cmd.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = poznamka;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }
    }
}
