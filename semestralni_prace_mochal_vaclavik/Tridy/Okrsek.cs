using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using System.Data;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Okrsek : INotifyPropertyChanged
    {
        private int _id { get; set; }
        public int Id
        {
            get
            { return _id; }
            set
            {
                _id = value;
                this.OnPropertyChanged(nameof(Id));
            }
        }
        
        private string _nazev { get; set; }
        public string Nazev
        {
            get { return _nazev; }
            set
            {
                _nazev = value;
                this.OnPropertyChanged(nameof(_nazev));
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
        private void Uloz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_okrsku.upravitOkrsek";

            using(OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = _nazev;
                cmd.Parameters.Add("p_idOkrsku", OracleDbType.Int32).Value = _id;

                cmd.ExecuteNonQueryAsync();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }    
        }
        private void Smaz(OracleConnection conn)
        {
            string storedProcedureName = "upravy_okrsku.smazOkrsek";

            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_idOkrsku", OracleDbType.Int32).Value = _id;

                cmd.ExecuteNonQueryAsync();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }
        private void Pridej(OracleConnection conn)
        {
            string storedProcedureName = "upravy_okrsku.pridatOkrsek";

            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = _nazev;

                cmd.ExecuteNonQueryAsync();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }
    }
}
