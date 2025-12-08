using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using semestralni_prace_mochal_vaclavik.Tridy;

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
        private Uzivatel uzivatel = new Uzivatel();
        public MainViewModel(MainWindow window)
        {
            this.Window = window;
            //wallis45548 - policista
            //martin25922 - obcan => hesla jsou stejné číslo
            //user.Id = 80;
            //user.Opravneni = "obcan";
            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            nastavOknaPodleOpravneni(); //vše se schová kromě úvodního okna a přihlášení

        }

        /// <summary>
        /// otevře nové okno kde se nebude nic commitovat do db - půjde jen zobrazovat data
        /// </summary>
        [RelayCommand]
        public async void Emulovat()
        {
            var emulace = new MainWindow();
            /*((MainViewModel)emulace.DataContext).IdUzivatele =*/
            emulace.Show();
        }

        [RelayCommand(CanExecute = nameof(ZkontrolovatVyplneniPrihlaseni))]
        private void Prihlas((string PrihlasovaciJmeno, string Heslo) udaje)
        {
            //udělat funkci v databázi která vrátí uživatele?
            try
            {
                // SQL dotaz - Načteme uživatele pro Admin Grid
                //id ,prihlasovacijmeno, nazevopravneni, o_jmeno, o_prijmeni, p_jmeno, p_prijmeni
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
                            //uzivatel.Password = reader["heslo"].ToString();
                            uzivatel.Opravneni = reader["nazevopravneni"].ToString();
                            
                            if(uzivatel.Opravneni == "obcan")
                            {
                                uzivatel.Jmeno = reader["o_jmeno"].ToString();
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
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
            nastavOknaPodleOpravneni();
        }

        [RelayCommand]
        private void Odhlas()
        {
            uzivatel.Id = 0;
            uzivatel.Opravneni = string.Empty;
            Window.Okna.SelectedIndex = 0;
            nastavOknaPodleOpravneni();
        }


        public bool ZkontrolovatVyplneniPrihlaseni()
        {
            return Window.UsernameTextBox.Text != string.Empty && Window.PasswordBox.Text != string.Empty;
        }

        private void nastavOknaPodleOpravneni()
        {
            Window.Kontakty.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            Window.Ucet.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            //// Tlačítko Potvrdit na Můj účet (Policista a Admin mohou editovat)
            //public Visibility UcetEditVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

            Window.Okrsky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.Prestupky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.MojePrestupky.Visibility = uzivatel.Opravneni == "obcan" ? Visibility.Visible : Visibility.Collapsed;

            Window.Hlidky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

            //// Celý obsah na kartě Admin (DataGrid, Filtry, Akční tlačítka)
            Window.Admin.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            //public Visibility AdminControlsVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;

            Window.Prihlaseni.Visibility = IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;

        }
        private bool IsAtLeastRole(string requiredRole)
        {
            // Kontrola role, pokud by se v budoucnu přidaly další úrovně
            if (uzivatel.Opravneni == "administrator") return true;
            if (uzivatel.Opravneni == "policista" && (requiredRole == "policista" || requiredRole == "obcan")) return true;
            if (uzivatel.Opravneni == "obcan" && requiredRole == "obcan") return true;
            return false;
        }

        [RelayCommand]
        private void ZmenaOkna()
        {
            if (Window.Kontakty.IsSelected)
            {
                NacistKontakty();
            }
            else if (Window.Ucet.IsSelected)
            {
                NacistDetailUzivatele(uzivatel.Id);
            }
            else if (Window.Admin.IsSelected)
            {
                NacistUzivatele();
            }
            else if (Window.Prestupky.IsSelected)
            {
                NacistPrestupky();
            }
            else if (Window.MojePrestupky.IsSelected)
            {
                NacistMojePrestupky();
            }
            else if (Window.Hlidky.IsSelected)
            {
                NacistHlidky();
            }
            else if (Window.Okrsky.IsSelected)
            {
                NacistOkrsky();
            }
        }

        private void NacistKontakty()
        {
            try
            {
                // SQL dotaz - spojíme Policistu s Hodností, aby to hezky vypadalo
                string sql = @"
                        SELECT
                            p.jmeno AS Jméno,
                            p.prijmeni AS Příjmení,
                            h.nazev AS Hodnost,
                            s.nazev AS Stanice
                        FROM 
                            policiste p
                        INNER JOIN 
                            hodnosti h ON p.idhodnosti = h.idhodnosti
                        INNER JOIN 
                            policejni_stanice s ON p.idstanice = s.idstanice
                        ORDER BY 
                            p.prijmeni, h.nazev
                        ";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Použijeme DataAdapter pro naplnění tabulky
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
        // Příklad načtení detailu uživatele (pro záložku Můj účet)
        private void NacistDetailUzivatele(int idUzivatele)
        {
            try
            {
                string sql = "SELECT jmeno, prijmeni, ulice, postovnismerovacicislo, obec, zeme " +
                             "FROM obcane o JOIN adresy a ON o.idadresy = a.idadresy " +
                             "WHERE o.iduzivatele = :id";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("id", idUzivatele));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Window.JmenoTxt.Clear();
                            Window.PrijmeniTxt.Clear();
                            Window.UsernameTxt.Clear();
                            Window.HesloTxt.Clear();
                            Window.JmenoTxt.Text = "Dodělat";
                            Window.PrijmeniTxt.Text = "Dodělat";
                            Window.UsernameTxt.Text = uzivatel.Username;
                            Window.HesloTxt.Text = uzivatel.Password;
                            //Window.ZemeTxt.Text = reader["zeme"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba detailu: " + ex.Message);
            }
        }

        private void NacistUzivatele()
        {
            try
            {
                // SQL dotaz - Načteme uživatele pro Admin Grid
                string sql = @"
                        SELECT u.prihlasovacijmeno, o.nazevopravneni FROM uzivatele u
                        left join opravneni o using(idopravneni)"; // Změněno na "uzivatel" pro Admin Grid

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Použijeme DataAdapter pro naplnění tabulky
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Napojení dat do vašeho DataGridu v XAML
                    Window.UzivateleGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }

        private void NacistPrestupky()
        {
            try
            {

                // SQL dotaz - Načteme přestupky pro Přestupky Grid
                string sql = @"
                        SELECT * FROM prestupky_obcanu";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Použijeme DataAdapter pro naplnění tabulky
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Napojení dat do vašeho DataGridu v XAML
                    Window.PrestupkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání přestupků: " + ex.Message);
            }
        }

        private void NacistMojePrestupky()
        {
            //throw new NotImplementedException();
            try
            {
                string sql = @"
                        SELECT * FROM prestupky_obcanu
                        where idobcana = :id";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Použijeme DataAdapter pro naplnění tabulky
                    cmd.Parameters.Add(new OracleParameter("id", uzivatel.Id));
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Napojení dat do vašeho DataGridu v XAML
                    //Window.PrestupkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání přestupků: " + ex.Message);
            }
        }
        private void NacistHlidky()
        {
            try
            {

                // SQL dotaz - Načtení hlídek (příklad)
                string sql = @"
                SELECT * FROM hlidky";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    // Vazba na nový DataGrid
                    Window.HlidkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání hlídek: " + ex.Message);
            }
        }
        private void NacistOkrsky()
        {
            try
            {

                // SQL dotaz - Načtení okrsků
                string sql = @"
                SELECT * FROM okrsky";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    // Vazba na nový DataGrid
                    Window.OkrskyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání okrsků: " + ex.Message);
            }
        }
    }
}
