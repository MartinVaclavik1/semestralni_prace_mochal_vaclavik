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
    public class Hlidka : INotifyPropertyChanged
    {
        private int _idHlidky { get; set; }
        public int IdHlidky
        {
            get { return _idHlidky; }
            set {
                _idHlidky = value;
                this.OnPropertyChanged(nameof(IdHlidky));
            }
        }
        private string _nazevHlidky { get; set; }
        public string NazevHlidky
        {
            get { return _nazevHlidky; }
            set {
                _nazevHlidky = value;
                this.OnPropertyChanged(nameof(NazevHlidky));
            }
        }
        private string _nazev { get; set; }
        public string Nazev
        {
            get { return _nazev; }
            set {
                _nazev = value;
                this.OnPropertyChanged(nameof(Nazev));
            }
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
        public void Uloz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_hlidek.upravitHlidku";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("p_idHlidky", OracleDbType.Int32).Value = _idHlidky;
                cmd.Parameters.Add("p_nazevHlidky", OracleDbType.Varchar2).Value = _nazevHlidky;
                cmd.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = _nazev;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
                zmenen = false;
            }
        }

        public void Smaz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_hlidek.smazHlidku";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("p_idHlidky", OracleDbType.Int32).Value = _idHlidky;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }

        public void Pridej(OracleConnection conn, string nazev, string typ)
        {
            string storedProcedureName = "upravy_hlidek.pridatHlidku";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("p_nazevHlidky", OracleDbType.Varchar2).Value = nazev;
                cmd.Parameters.Add("p_nazevTypu", OracleDbType.Varchar2).Value = typ;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
                
            }
        }
    }
}
