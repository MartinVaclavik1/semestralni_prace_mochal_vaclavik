using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace semestralni_prace_mochal_vaclavik.Tridy
{
    public class Uzivatel : ObservableObject, INotifyPropertyChanged
    {
        private int id { get; set; }
        public int Id
        {
            get
            { return id; }
            set
            {
                id = value;
                this.OnPropertyChanged("Id");
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
                else if ((opravneni == "administrator" || opravneni == "policista")
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
        public BitmapImage? Obrazek
        {
            get { return obrazek != null ? obrazek : LoadBitmap(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Fotky\\no-image-icon.png"); } //do db se neuloží protože se tam dává jen bytes, který se nastaví při uložení, nebo načtení
            set
            {
                obrazek = value;
                this.OnPropertyChanged("Obrazek");
            }
        }

        BitmapImage LoadBitmap(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Relative);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public byte[]? ObrazekBytes { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Resetuj()
        {
            Id = 0;
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
            set { zmenen = value; }

        }

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
            string storedProcedureName = "UPRAVY_UZIVATELU.upravitUzivatele";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = Username;
                cmd.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = Password;

                cmd.Parameters.Add("p_typOpravneni", OracleDbType.Varchar2).Value = Opravneni;
                cmd.Parameters.Add("p_iduzivatele", OracleDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
                zmenen = false;
            }
        }

        public void Smaz(OracleConnection conn)
        {
            string storedProcedureName = "UPRAVY_UZIVATELU.smazUzivatele";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.Parameters.Add("p_iduzivatele", OracleDbType.Int32).Value = Id;
                    cmd.ExecuteNonQuery();
                    new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
         }

        public void Pridej(OracleConnection conn, string prihlasovaciJmeno, string heslo, string jmenoPolicisty, string jmenoObcana, string opravneni)
        {
            string storedProcedureName = "UPRAVY_UZIVATELU.pridejUzivatele";
            using (OracleCommand cmd = new OracleCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("p_prihlasovaciJmeno", OracleDbType.Varchar2).Value = prihlasovaciJmeno;
                cmd.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = heslo;
                cmd.Parameters.Add("p_jmenoPolicisty", OracleDbType.Varchar2).Value = jmenoPolicisty;
                cmd.Parameters.Add("p_jmenoObcana", OracleDbType.Varchar2).Value = jmenoObcana;
                cmd.Parameters.Add("p_opravneni", OracleDbType.Varchar2).Value = opravneni;

                cmd.ExecuteNonQuery();
                new OracleCommand("COMMIT", conn).ExecuteNonQuery();
            }
        }
    }
}
