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

        private int idPrestupku { get; set; }
        public int IdPrestupku
        {
            get
            { return idPrestupku; }
            set
            {
                idPrestupku = value;
                this.OnPropertyChanged(nameof(IdPrestupku));
            }
        }
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
        
        private DateTime datum { get; set; }
        public DateTime Datum
        {
            get { return datum.Date; }
            set
            {
                datum = value.Date;
                this.OnPropertyChanged(nameof(Datum));
            }
        }

        private string jmenoObcana { get; set; }
        public string  JmenoObcana
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
        private string adresaZasahu { get; set; }
        public string AdresaZasahu
        {
            get { return adresaZasahu; }
            set
            {
                adresaZasahu = value;
                this.OnPropertyChanged(nameof(AdresaZasahu));
            }
        }
        private string popisZasahu { get; set; }
        public string PopisZasahu
        {
            get { return popisZasahu; }
            set
            {
                popisZasahu = value;
                this.OnPropertyChanged(nameof(PopisZasahu));
            }
        }

        public void Resetuj()
        {
            IdPrestupku = 0;
            IdObcana = 0;
            TypPrestupku = String.Empty;
            Datum = DateTime.MinValue;
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
                cmd.Parameters.Add("p_idPrestupku", OracleDbType.Int32).Value = idPrestupku;
                cmd.Parameters.Add("p_idObcana", OracleDbType.Int32).Value = idObcana;
                cmd.Parameters.Add("p_nazevPrestupku", OracleDbType.Varchar2).Value = typPrestupku;
                cmd.Parameters.Add("p_datumZasahu", OracleDbType.Date).Value = datum.Date;
                cmd.Parameters.Add("p_jmenoObcana", OracleDbType.Varchar2).Value = jmenoObcana;
                cmd.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = poznamka;
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
                cmd.Parameters.Add("p_idPrestupku", OracleDbType.Varchar2).Value = idPrestupku;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }

        // TODO : opravit ve view nebo v DB aby to šlo vložit (nefunkční) 
        // není tam políčko na popis zasahu
        public void Pridej(OracleConnection conn,string ulice, int cisloPopisne, string obec, string psc, string typPrestupku, string popisZasahu, string jmenoObcana)
        {
            string storedProcedureName = "upravy_prestupku.pridejPrestupek";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = ulice;
                cmd.Parameters.Add("p_cislopopisne", OracleDbType.Int32).Value = cisloPopisne;
                cmd.Parameters.Add("p_obec", OracleDbType.Varchar2).Value = obec;
                cmd.Parameters.Add("p_psc", OracleDbType.Char).Value = psc;
                cmd.Parameters.Add("p_popisZasahu", OracleDbType.Varchar2).Value = popisZasahu;
                cmd.Parameters.Add("p_typPrestupku", OracleDbType.Varchar2).Value = typPrestupku;
                cmd.Parameters.Add("p_jmenoObcana", OracleDbType.Varchar2).Value = jmenoObcana;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }
    }
}
