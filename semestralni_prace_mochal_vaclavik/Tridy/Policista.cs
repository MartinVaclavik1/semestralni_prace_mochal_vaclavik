using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Policista : INotifyPropertyChanged
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
        private string jmeno { get; set; }
        public string Jmeno
        {
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
        private string hodnost { get; set; }
        public string Hodnost
        {
            get { return hodnost; }
            set
            {
                hodnost = value;
                this.OnPropertyChanged(nameof(Hodnost));
            }
        }
        private string nadrizeny { get; set; }
        public string Nadrizeny
        {
            get { return nadrizeny; }
            set
            {
                nadrizeny = value;
                this.OnPropertyChanged(nameof(Nadrizeny));
            }
        }
        private string stanice { get; set; }
        public string Stanice
        {
            get { return stanice; }
            set
            {
                stanice = value;
                this.OnPropertyChanged(nameof(Stanice));

            }
        }

        public void Resetuj()
        {
            Id = 0;
            Jmeno = String.Empty;
            Prijmeni = String.Empty;
            Hodnost = String.Empty;
            Nadrizeny = String.Empty;
            Stanice = String.Empty;
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
            string storedProcedureName = "upravy_policistu.upravitPolicistu";

            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = jmeno;
                cmd.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = prijmeni;
                cmd.Parameters.Add("p_hodnost", OracleDbType.Varchar2).Value = hodnost;
                if (nadrizeny == string.Empty)
                {
                    cmd.Parameters.Add("p_nadrizeny", OracleDbType.Varchar2).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("p_nadrizeny", OracleDbType.Varchar2).Value = nadrizeny;
                }
                cmd.Parameters.Add("p_stanice", OracleDbType.Varchar2).Value = stanice;

                cmd.Parameters.Add("p_idPolicisty", OracleDbType.Int32).Value = id;

                cmd.ExecuteNonQueryAsync();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }

            zmenen = false;
        }
    }
}
