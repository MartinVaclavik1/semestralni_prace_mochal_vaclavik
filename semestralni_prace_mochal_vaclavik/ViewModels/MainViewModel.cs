using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using semestralni_prace_mochal_vaclavik.Tridy;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly string connectionString = "User Id=st72536;" +
                                    "Password=killer12;" +
                                    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                    "(CONNECT_DATA=(SID=BDAS)));";
        private OracleConnection conn;
        private MainWindow Window { get; set; }

        [ObservableProperty]
        public DataView kontaktyItemsSource;
        [ObservableProperty]
        private DataView uzivatelItemsSource;
        [ObservableProperty]
        private Uzivatel uzivatel = new Uzivatel();
        [ObservableProperty]
        private Registrace novaRegistrace = new Registrace();
        [ObservableProperty]
        private DataTable opravneniZdroj;
        [ObservableProperty]
        private List<string> opravneniSeznamy = new List<string> { "administrator", "policista", "obcan" };


        public MainViewModel(MainWindow window)
        {
            this.Window = window;
            
            //wallis45548 - policista
            //martin25922 - obcan => hesla jsou stejné číslo
            //uzivatel.Id = 80;
            //uzivatel.Opravneni = "administrator";
            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Prihlas(("Oli","12345"));
            nastavOknaPodleOpravneni(); //vše se schová kromě úvodního okna a přihlášení

        }
        public Visibility PolicistaControlsVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdminControlsVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility UcetEditVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// otevře nové okno kde se nebude nic commitovat do db - půjde jen zobrazovat data
        /// </summary>
        [RelayCommand]
        public async void Emulovat(object radek)
        {
            // převod na DataRowView
            var uzivatelRow = radek as DataRowView;

            if (uzivatelRow != null)
            {
                try
                {
                    string jmeno = uzivatelRow["PrihlasovaciJmeno"].ToString();
                    string heslo = uzivatelRow["Heslo"].ToString();

                    MessageBox.Show($"Emulace uživatele: {jmeno}");

                    var emulace = new MainWindow();
                    emulace.Title = emulace.Title + " - EMULACE";
                    ((MainViewModel)emulace.DataContext).Prihlas((jmeno, heslo));
                    emulace.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání dat řádku: " + ex.Message);
                }
            }
        }

        // Manipulace s uživately
        [RelayCommand]
        public async Task UpravitUzivatele(object radek)
        {
            var uzivatelRow = radek as DataRowView;
            
            if (uzivatelRow != null)
            {
                try
                {
                    uzivatelRow.EndEdit();

                    int id = Convert.ToInt32(uzivatelRow["iduzivatele"]);
                    string jmeno = uzivatelRow["prihlasovacijmeno"].ToString();
                    string heslo = uzivatelRow["heslo"].ToString();
                    string nazevOpravneni = (uzivatelRow["nazevopravneni"].ToString()).ToLower();

                    int typOpr = 0;
                    if (nazevOpravneni.Equals("administrator", StringComparison.OrdinalIgnoreCase)) typOpr = 3;
                    else if (nazevOpravneni.Equals("policista", StringComparison.OrdinalIgnoreCase)) typOpr = 1;
                    else if (nazevOpravneni.Equals("obcan", StringComparison.OrdinalIgnoreCase)) typOpr = 2;
                    else
                    {
                        MessageBox.Show($"Neznámý typ oprávnění: '{nazevOpravneni}'. Změny nebyly uloženy.", "Chyba");
                        return;
                    }

                    string sql = @"
                        UPDATE UZIVATELE 
                        SET PRIHLASOVACIJMENO = :jmeno, 
                            HESLO = :heslo,
                            IDOPRAVNENI = :typOpravneni
                        WHERE IDUZIVATELE = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;

                        cmd.Parameters.Add("jmeno", OracleDbType.Varchar2).Value = jmeno;
                        cmd.Parameters.Add("heslo", OracleDbType.Varchar2).Value = heslo;
                        cmd.Parameters.Add("typOpravneni", OracleDbType.Int32).Value = typOpr;
                        cmd.Parameters.Add("id", OracleDbType.Int32).Value = id;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            using (var commitCmd = new OracleCommand("COMMIT", conn))
                            {
                                await commitCmd.ExecuteNonQueryAsync();
                            }

                            await NacistUzivatele();
                            MessageBox.Show("Uživatel byl úspěšně upraven.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Záznam nebyl v databázi nalezen (ID se neshoduje).", "Chyba");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při aktualizaci uživatele: " + ex.Message, "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        [RelayCommand]
        public async Task OdebratUzivatele(object radek)
        {
            var uzivatelRow = radek as DataRowView;

            if (uzivatelRow != null)
            {
                int id = Convert.ToInt32(uzivatelRow["IDUZIVATELE"]);

                var result = MessageBox.Show(
                    $"Opravdu chcete trvale smazat uživatele?",
                    "Potvrzení smazání",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                try
                {
                    string sql = "DELETE FROM UZIVATELE WHERE IDUZIVATELE = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("id", OracleDbType.Int32).Value = id;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            using (var commitCmd = new OracleCommand("COMMIT", conn))
                            {
                                await commitCmd.ExecuteNonQueryAsync();
                            }

                            NacistUzivatele();

                            MessageBox.Show("Uživatel byl úspěšně odstraněn.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Záznam nebyl v databázi nalezen (možná byl již smazán).", "Chyba");
                        }
                    }
                }
                catch (OracleException oraEx)
                {
                    if (oraEx.Number == 2292) 
                    {
                        MessageBox.Show("Nelze smazat uživatele, protože je propojen s Občanem nebo Policistou. Nejdříve musíte smazat záznam tam.", "Chyba integrity", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Chyba Oracle: " + oraEx.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Obecná chyba: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(ZkontrolovatVyplneniPrihlaseni))]
        private void Prihlas((string PrihlasovaciJmeno, string Heslo) udaje)
        {
            //udělat funkci v databázi která vrátí uživatele?
            try
            {
                string sql = @"
                        select * from datauctuview where id = 
                        (select prihlaseni(:prihlJmeni,:heslo) from Dual)";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("prihlJmeno", udaje.PrihlasovaciJmeno));
                    cmd.Parameters.Add(new OracleParameter("heslo", udaje.Heslo.ToString()));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uzivatel.Id = int.Parse(reader["id"].ToString());
                            uzivatel.Username = reader["prihlasovacijmeno"].ToString();
                            var blob = reader.GetOracleBlob(2); //2 = pozice blob z view (začíná 0)
                            if (!blob.IsNull)
                            {
                                uzivatel.ObrazekBytes = nactiByteZBLOB(blob);
                                uzivatel.Obrazek = vytvorObrazek(uzivatel.ObrazekBytes);
                            }
                            
                              
                            uzivatel.Opravneni = reader["nazevopravneni"].ToString();

                            if (uzivatel.Opravneni == "obcan")
                            {
                                Uzivatel.Jmeno = reader["o_jmeno"].ToString();
                                uzivatel.Prijmeni = reader["o_prijmeni"].ToString();

                            }
                            else
                            {
                                uzivatel.Jmeno = reader["p_jmeno"].ToString();
                                uzivatel.Prijmeni = reader["p_prijmeni"].ToString();
                            }
                            Window.Okna.SelectedIndex = 0;
                            Window.UsernameTextBox.Clear();
                            Window.PasswordBox.Clear();

                            Task.Run(() => { MessageBox.Show("Uživatel přihlášen"); });


                        }
                        else
                        {
                            MessageBox.Show("Špatné přihlašovací údaje");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatele: " + ex.Message);
            }
            nastavOknaPodleOpravneni();
            OnPropertyChanged(nameof(PolicistaControlsVisible));
            OnPropertyChanged(nameof(AdminControlsVisible));
            OnPropertyChanged(nameof(UcetEditVisible));
        }

        private byte[] nactiByteZBLOB(OracleBlob imgBlob)
        {
            if (imgBlob == null)
                return null;
            // Create byte array to read the blob into
            byte[] imgBytes = new byte[imgBlob.Length];
            // Read the blob into the byte array
            imgBlob.Read(imgBytes, 0, (int)imgBlob.Length);

            return imgBytes;
        }

        [RelayCommand]
        private void Odhlas()
        {
            Uzivatel.Resetuj();
            Window.Okna.SelectedIndex = 0;
            nastavOknaPodleOpravneni();
            OnPropertyChanged(nameof(PolicistaControlsVisible));
            OnPropertyChanged(nameof(AdminControlsVisible));
            OnPropertyChanged(nameof(UcetEditVisible));

        }


        public bool ZkontrolovatVyplneniPrihlaseni()
        {
            return Window.UsernameTextBox.Text != string.Empty && Window.PasswordBox.Text != string.Empty;
        }

        private bool ZkontrolovatVyplneniRegistrace()
        {
            return true;
            return Window.jmenoTxt.Text != string.Empty
                && Window.prijmeniTxt.Text != string.Empty
                && Window.opTxt.Text != string.Empty
                && Window.pscTxt.Text != string.Empty
                && Window.uliceTxt.Text != string.Empty
                && Window.cpTxt.Text != string.Empty
                && Window.obecTxt.Text != string.Empty
                && Window.zemeTxt.Text != string.Empty;
        }

        private void nastavOknaPodleOpravneni()
        {
            Window.Kontakty.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            Window.Ucet.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            Window.MojePrestupky.Visibility = uzivatel.Opravneni == "obcan" ? Visibility.Visible : Visibility.Collapsed;
            //// Tlačítko Potvrdit na Můj účet (Policista a Admin mohou editovat)
            //public Visibility UcetEditVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

            Window.Okrsky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.Prestupky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.Hlidky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

            //// Celý obsah na kartě Admin (DataGrid, Filtry, Akční tlačítka)
            Window.Admin.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            Window.LogovaciTabulka.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            Window.SystemovyKatalog.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            //public Visibility AdminControlsVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;

            Window.Prihlaseni.Visibility = IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;
            Window.Registrace.Visibility = IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;

        }
        private bool IsAtLeastRole(string requiredRole)
        {
            // Kontrola role, pokud by se v budoucnu přidaly další úrovně
            if (uzivatel.Opravneni == "administrator") return true;
            if (uzivatel.Opravneni == "policista" && (requiredRole == "policista" || requiredRole == "obcan")) return true;
            if (uzivatel.Opravneni == "obcan" && requiredRole == "obcan") return true;
            return false;
        }

        [RelayCommand(CanExecute = nameof(ZkontrolovatVyplneniRegistrace))]
        private void Registrovat()
        {

            try
            {
                using (OracleCommand cmdAdresa = new OracleCommand("vytvor_uzivatele_obcana", conn))
                {
                    cmdAdresa.CommandType = CommandType.StoredProcedure;
                    cmdAdresa.BindByName = true;

                    cmdAdresa.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = novaRegistrace.Username;
                    cmdAdresa.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = novaRegistrace.Heslo;
                    cmdAdresa.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = novaRegistrace.Jmeno;
                    cmdAdresa.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = novaRegistrace.Prijmeni;

                    cmdAdresa.Parameters.Add("p_cisloop", OracleDbType.Decimal).Value = Convert.ToInt32(novaRegistrace.CisloOP);
                    cmdAdresa.Parameters.Add("p_psc", OracleDbType.Char, 5).Value = novaRegistrace.PSC;
                    cmdAdresa.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = novaRegistrace.Ulice;
                    cmdAdresa.Parameters.Add("p_cislopopisne", OracleDbType.Decimal).Value = Convert.ToInt32(novaRegistrace.CisloPopisne);

                    cmdAdresa.Parameters.Add("p_obec", OracleDbType.Varchar2).Value = novaRegistrace.Obec;
                    cmdAdresa.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = novaRegistrace.Zeme;
                    
                    cmdAdresa.ExecuteNonQuery();
                }
                conn.Commit();

                MessageBox.Show($"Uživatel {NovaRegistrace.Username} Heslo {novaRegistrace.Heslo}.", "Registrace", MessageBoxButton.OK, MessageBoxImage.Information);
                Prihlas((novaRegistrace.Username, novaRegistrace.Heslo));
                novaRegistrace.Clear();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task AktualizovatUcet()
        {
            try
            {
                using (OracleCommand cmdAdresa = new OracleCommand("aktualizuj_ucet", conn))
                {


                        cmdAdresa.CommandType = CommandType.StoredProcedure;
                        cmdAdresa.BindByName = true;

                        cmdAdresa.Parameters.Add("p_id", OracleDbType.Int32).Value = uzivatel.Id;
                        cmdAdresa.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = uzivatel.Username;
                        cmdAdresa.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Password;
                        cmdAdresa.Parameters.Add("p_obrazek", OracleDbType.Blob).Value = uzivatel.ObrazekBytes;


                    cmdAdresa.ExecuteNonQuery();
                }
                conn.Commit();

                MessageBox.Show($"Uživatel {Uzivatel.Username} Heslo {Uzivatel.Password}.", "Aktualizace", MessageBoxButton.OK, MessageBoxImage.Information);
                novaRegistrace.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //try
            //{
            //    using (OracleCommand cmdAdresa = new OracleCommand("aktualizuj_ucet", conn))
            //    {


            //        cmdAdresa.CommandType = CommandType.StoredProcedure;
            //        cmdAdresa.BindByName = true;

            //        cmdAdresa.Parameters.Add("p_id", OracleDbType.Int32).Value = uzivatel.Id;
            //        cmdAdresa.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = uzivatel.Username;
            //        cmdAdresa.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Password;
            //        cmdAdresa.Parameters.Add("p_obrazek", OracleDbType.Blob).Value = uzivatel.ObrazekBytes;


            //        cmdAdresa.ExecuteNonQuery();
            //    }
            //    conn.Commit();

            //    MessageBox.Show($"Uživatel {Uzivatel.Username} Heslo {Uzivatel.Password}.", "Aktualizace", MessageBoxButton.OK, MessageBoxImage.Information);
            //    novaRegistrace.Clear();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        [RelayCommand]
        private async Task ZmenaOkna()
        {
            if (Window.Admin.IsSelected)
            {
                await NacistUzivatele();
            }
            else if (Window.Kontakty.IsSelected)
            {
                await NacistKontakty();
            }
            else if (Window.Prestupky.IsSelected)
            {
                await NacistPrestupky();
            }
            else if (Window.MojePrestupky.IsSelected)
            {
                await NacistMojePrestupky();
            }
            else if (Window.Hlidky.IsSelected)
            {
                await NacistHlidky();
            }
            else if (Window.Okrsky.IsSelected)
            {
                await NacistOkrsky();
            }
            else if (Window.LogovaciTabulka.IsSelected)
            {
                await NacistLogovaciTabulku();
            }
            else if (Window.SystemovyKatalog.IsSelected)
            {
                await NacistSystemovyKatalog();
            }
        }

        private async Task NacistUzivatele()
        {
            try
            {
                string sql = @"
                    SELECT u.iduzivatele, u.prihlasovacijmeno, u.heslo, o.nazevopravneni 
                    FROM uzivatele u
                    LEFT JOIN opravneni o ON u.idopravneni = o.idopravneni";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    await Task.Run(() => adapter.Fill(dt));

                    UzivatelItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }
        private async Task NacistSystemovyKatalog()
        {
            try
            {
                string sql = @"
                        SELECT * FROM systemovy_katalogview
                        ";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Window.systemovyKatalogGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        private async Task NacistLogovaciTabulku()
        {
            try
            {
                string sql = @"
                        SELECT * FROM logovaci_tabulkaview
                        ";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Window.logovaciTabulkaGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        private async Task NacistKontakty()
        {
            try
            {
                string sql = @"
                        SELECT
                            *
                        FROM 
                            kontaktyView
                        ";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Window.KontaktyGrid.ItemsSource = dt.DefaultView;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        //private void NacistUzivatele()
        //{
        //    try
        //    {
        //        string sql = @"
        //            SELECT u.iduzivatele, u.prihlasovacijmeno, u.heslo, u.idopravneni, o.nazevopravneni 
        //            FROM uzivatele u
        //            LEFT JOIN opravneni o ON u.idopravneni = o.idopravneni";

        //        using (OracleCommand cmd = new OracleCommand(sql, conn))
        //        {
        //            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
        //            DataTable dt = new DataTable();
        //            adapter.Fill(dt);

        //            Window.UzivateleGrid.ItemsSource = dt.DefaultView;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
        //    }
        //}

        private async Task NacistPrestupky()
        {
            try
            {


                string sql = @"
                        SELECT * FROM prestupkyview";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Window.PrestupkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání přestupků: " + ex.Message);
            }
        }

        private async Task NacistMojePrestupky()
        {
            try
            {
                string sql = @"
                        SELECT * FROM prestupkyview
                        where idobcana = :id";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {

                    cmd.Parameters.Add(new OracleParameter("id", uzivatel.Id));
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání přestupků: " + ex.Message);
            }
        }
        private async Task NacistHlidky()
        {
            try
            {

                string sql = @"select h.nazevhlidky, t.nazev from hlidky h
                               join typy_hlidky t using(idtypu)";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Window.HlidkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání hlídek: " + ex.Message);
            }
        }
        private async Task NacistOkrsky()
        {
            try
            {
                string sql = @"
                SELECT * FROM okrsky";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Window.OkrskyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání okrsků: " + ex.Message);
            }
        }
        [RelayCommand]
        public void UpravitKontakty(object radek)
        {

            var policistaRow = radek as DataRowView;

            if (policistaRow != null)
            {
                try
                {
                   


                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("Chyba: Sloupec s ID policisty nebyl nalezen v datovém zdroji. Ujistěte se, že SQL dotaz vrací ID!", "Chyba datového klíče");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy kontaktu: " + ex.Message);
                }
            }
        }
        [RelayCommand]
        private void NahratImg()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Obrázky (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|Všechny soubory (*.*)|*.*";

                try
                {
                    openFileDialog.ShowDialog();
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    Uzivatel.Obrazek = vytvorObrazek(imageBytes);
                    Uzivatel.ObrazekBytes = imageBytes;

                    MessageBox.Show("Profilový obrázek byl úspěšně nahrán a uložen.", "Úspěch");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při nahrávání obrázku: {ex.Message}", "Chyba");
                }
        }
        private BitmapImage vytvorObrazek(byte[] imageBytes)
        {
            BitmapImage img = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = stream;
                img.EndInit();
                img.Freeze();
            }
            return img;
        }
        [RelayCommand]
        private void OdebratImg()
        {
            if (Uzivatel.Obrazek != null || true) 
            {
                try
                {
 
                    Uzivatel.Obrazek = null; 
                    Uzivatel.ObrazekBytes = null;

                    MessageBox.Show("Profilový obrázek byl úspěšně odstraněn.", "Úspěch");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při odstraňování obrázku: {ex.Message}", "Chyba");
                }
            }
        }
    }
}
