using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using semestralni_prace_mochal_vaclavik.Tridy;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    /// <summary>
    /// Hlavní ViewModel pro aplikaci.
    /// Spravuje přihlášení, registraci, správu uživatelů a zobrazení dat na základě rolí.
    /// </summary>
    /// <remarks>
    /// Třída obsluhuje komunikaci s databází Oracle, správu přihlášeného uživatele a dynamické zobrazení UI prvků
    /// podle uživatelských oprávnění (obcan, policista, administrator).
    /// 
    /// <note>Heslo a údaje o připojení jsou hardcodovány.</note>
    /// </remarks>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// Připojovací řetězec k databázi Oracle. (heslo by bylo jinde pokud by šlo o produkci)
        /// </summary>
        private readonly string connectionString = "User Id=st72536;" +
                                    "Password=killer12;" +
                                    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                    "(CONNECT_DATA=(SID=BDAS)));";

        /// <summary>
        /// Aktivní Oracle připojení k databázi.
        /// </summary>
        private OracleConnection conn;

        /// <summary>
        /// Referenční na hlavní okno aplikace.
        /// </summary>
        private MainWindow Window { get; set; }

        /// <summary>
        /// Zdroj dat pro DataGrid s kontakty.
        /// </summary>
        [ObservableProperty]
        public DataView kontaktyItemsSource;

        [ObservableProperty]
        public DataView okrskyItemsSource;

        [ObservableProperty]
        public DataView prestupkyItemsSource;

        /// <summary>
        /// Zdroj dat pro DataGrid se všemi uživateli.
        /// </summary>
        [ObservableProperty]
        private DataView uzivatelItemsSource;

        /// <summary>
        /// Kolekce všech načtených uživatelů pro nastavení admina.
        /// </summary>
        public ObservableCollection<Uzivatel> Users { get; set; } = new ObservableCollection<Uzivatel>();

        public ObservableCollection<Policista> Policiste { get; set; } = new ObservableCollection<Policista>();

        public ObservableCollection<Okrsek> Okrsky { get; set; } = new ObservableCollection<Okrsek>();
        public ObservableCollection<Prestupek> Prestupky { get; set; } = new ObservableCollection<Prestupek>();
        public ObservableCollection<Hlidka> Hlidky { get; set; } = new ObservableCollection<Hlidka>();

        /// <summary>
        /// Aktuálně přihlášený uživatel.
        /// </summary>
        [ObservableProperty]
        private Uzivatel uzivatel = new Uzivatel();

        /// <summary>
        /// Data pro nový účet během registrace.
        /// </summary>
        [ObservableProperty]
        private Registrace novaRegistrace = new Registrace();

        /// <summary>
        /// Tabulka s oprávněními z databáze.
        /// </summary>
        [ObservableProperty]
        private DataTable opravneniZdroj;

        /// <summary>
        /// Seznam dostupných typů oprávnění v systému.
        /// </summary>
        [ObservableProperty]
        private List<string> opravneniSeznam = new List<string>();

        [ObservableProperty]
        private List<string> hodnostiSeznam = new List<string>();

        [ObservableProperty]
        private List<string> typy_prestupkuSeznam = new List<string>();

        [ObservableProperty]
        private List<string> typy_hlidkySeznam = new List<string>();

        // Přihlášení 
        //wallis45548 - policista
        //martin25922 - obcan => hesla jsou stejné číslo
        //uzivatel.Id = 80;
        //uzivatel.Opravneni = "administrator";

        /// <summary>
        /// Inicializuje novou instanci MainViewModel.
        /// </summary>
        /// <param name="window">Referenční na hlavní okno aplikace</param>
        /// <remarks>
        /// Nastavuje inicializace databázového připojení a automaticky přihlašuje testovacího uživatele.
        /// </remarks>
        public MainViewModel(MainWindow window)
        {
            this.Window = window;
            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Prihlas(("Oli", "12345"));
            NastavComboboxy();
            NastavOknaPodleOpravneni(); //vše se schová kromě úvodního okna a přihlášení
        }

        /// <summary>
        /// Určuje viditelnost ovládacích prvků pro policisty.
        /// </summary>
        public Visibility PolicistaControlsVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Určuje viditelnost ovládacích prvků pro administrátory.
        /// </summary>
        public Visibility AdminControlsVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AdminBtnsVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// Určuje viditelnost editačních prvků účtu (viditelné pro policisty a vyšší).
        /// </summary>
        public Visibility UcetEditVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Emuluje přihlášení jiného uživatele v novém okně bez commitování do databáze.
        /// </summary>
        /// <param name="radek">Řádek z DataGridu s daty uživatele k emulaci</param>
        [RelayCommand]
        public async void Emulovat(object radek)
        {
            var uzivatelRow = radek as Uzivatel;
            if (uzivatelRow != null)
            {
                if (uzivatelRow.Id == Uzivatel.Id)
                {
                    MessageBox.Show("Nelze emulovat sám sebe!");
                    return;
                }

                try
                {
                    string jmeno = uzivatelRow.Username;
                    string heslo = uzivatelRow.Password;

                    MessageBox.Show($"Emulace uživatele: {jmeno}");

                    var emulace = new MainWindow();
                    emulace.Title = emulace.Title + " - EMULACE";
                    ((MainViewModel)emulace.DataContext).Prihlas((jmeno, heslo));
                    emulace.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při získávání dat řádku: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Aktualizuje údaje uživatele v databázi z Admin panelu.
        /// </summary>
        /// <param name="radek">Řádek s upravenými daty uživatele</param>
        /// <remarks>Volá uloženou proceduru UPRAVY_UZIVATELU.upravitUzivatele.</remarks>
        [RelayCommand]
        public async Task UpravitUzivatele(object radek)
        {
            var uzivatelRow = radek as Uzivatel;

            if (uzivatelRow != null && uzivatelRow.Zmenen)
            {
                if (uzivatelRow.Id == Uzivatel.Id)
                {
                    MessageBox.Show("Nelze editovat sám sebe! \n Pro editaci použijte \"Můj účet\"");
                    return;
                }

                try
                {

                    using (OracleCommand cmd = new OracleCommand("UPRAVY_UZIVATELU.upravitUzivatele", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = uzivatelRow.Username;
                        cmd.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatelRow.Password;

                        cmd.Parameters.Add("p_typOpravneni", OracleDbType.Varchar2).Value = uzivatelRow.Opravneni;
                        cmd.Parameters.Add("p_iduzivatele", OracleDbType.Int32).Value = uzivatelRow.Id;

                        await cmd.ExecuteNonQueryAsync();


                        using (var commitCmd = new OracleCommand("COMMIT", conn))
                        {
                            await commitCmd.ExecuteNonQueryAsync();
                        }
                        uzivatelRow.Zmenen = false;
                        MessageBox.Show("Uživatel byl úspěšně upraven.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při aktualizaci uživatele: " + ex.Message, "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Odebere uživatele z databáze po potvrzení.
        /// </summary>
        /// <param name="radek">Řádek s daty uživatele k odstranění</param>
        /// <remarks>
        /// Vyžaduje potvrzení od uživatele. Volá uloženou proceduru UPRAVY_UZIVATELU.smazUzivatele.
        /// Nelze odstranit sám sebe.
        /// </remarks>
        [RelayCommand]
        public async Task OdebratUzivatele(object radek)
        {
            var uzivatelRow = radek as Uzivatel;

            if (uzivatelRow != null)
            {
                if (uzivatelRow.Id == Uzivatel.Id)
                {
                    MessageBox.Show("Nelze odstranit sám sebe!");
                    return;
                }
                var result = MessageBox.Show(
                $"Opravdu chcete trvale smazat uživatele?",
                "Potvrzení smazání",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                try
                {
                    using (OracleCommand cmd = new OracleCommand("UPRAVY_UZIVATELU.smazUzivatele", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_iduzivatele", OracleDbType.Int32).Value = uzivatelRow.Id;

                        await cmd.ExecuteNonQueryAsync();


                        using (var commitCmd = new OracleCommand("COMMIT", conn))
                        {
                            await commitCmd.ExecuteNonQueryAsync();
                        }
                        MessageBox.Show("Uživatel byl úspěšně odebrán.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                        NacistUzivatele();
                    }
                }
                catch (OracleException oraEx)
                {

                    MessageBox.Show("Chyba Oracle: " + oraEx.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Obecná chyba: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Přihlašuje uživatele do systému na základě přihlašovacích údajů.
        /// </summary>
        /// <param name="udaje">Tuple obsahující přihlašovací jméno a heslo</param>
        /// <remarks>
        /// Komunikuje s databází, načítá uživatelské údaje a profilový obrázek.
        /// Automaticky nastavuje viditelnost UI prvků podle role uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        [RelayCommand(CanExecute = nameof(ZkontrolovatVyplneniPrihlaseni))]
        private void Prihlas((string PrihlasovaciJmeno, string Heslo) udaje)
        {
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
                            var blob = reader.GetOracleBlob(2);
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
                            Window.PrihlaseniView.UsernameTextBox.Clear();
                            Window.PrihlaseniView.PasswordBox.Clear();

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
            NastavOknaPodleOpravneni();
            OnPropertyChanged(nameof(PolicistaControlsVisible));
            OnPropertyChanged(nameof(AdminControlsVisible));
            OnPropertyChanged(nameof(UcetEditVisible));
        }

        /// <summary>
        /// Načítá binární data z Oracle BLOB objektu do pole bajtů.
        /// </summary>
        /// <param name="imgBlob">Oracle BLOB objekt s daty obrázku</param>
        /// <returns>
        /// Pole bajtů obsahující obsah BLOBu, nebo null pokud je BLOB prázdný
        /// </returns>
        private byte[] nactiByteZBLOB(OracleBlob imgBlob)
        {
            if (imgBlob == null)
                return null;

            byte[] imgBytes = new byte[imgBlob.Length];
            imgBlob.Read(imgBytes, 0, (int)imgBlob.Length);

            return imgBytes;
        }

        /// <summary>
        /// Odhlašuje aktuálně přihlášeného uživatele z systému.
        /// </summary>
        /// <remarks>
        /// Resetuje data uživatele a skrývá všechny prvky UI kromě přihlašovacího formuláře Registrace a karty Domu.
        /// </remarks>
        [RelayCommand]
        private void Odhlas()
        {
            Uzivatel.Resetuj();
            Window.Okna.SelectedIndex = 0;
            NastavOknaPodleOpravneni();
            OnPropertyChanged(nameof(PolicistaControlsVisible));
            OnPropertyChanged(nameof(AdminControlsVisible));
            OnPropertyChanged(nameof(UcetEditVisible));

        }

        /// <summary>
        /// Ověřuje, zda jsou vyplněna všechna povinná pole přihlašovacího formuláře.
        /// </summary>
        /// <returns>true pokud jsou obě pole (uživatelské jméno i heslo) vyplněna, jinak false</returns>
        public bool ZkontrolovatVyplneniPrihlaseni()
        {
            return true;
            //return Window.UsernameTextBox.Text != string.Empty && Window.PasswordBox.Text != string.Empty;
        }

        /// <summary>
        /// Ověřuje, zda jsou vyplněna všechna povinná pole registračního formuláře.
        /// </summary>
        /// <returns>Vždy vrací true (validace není plně implementována)</returns>
        private bool ZkontrolovatVyplneniRegistrace()
        {
            return true;
                //Window.jmenoTxt.Text != string.Empty
                //&& Window.prijmeniTxt.Text != string.Empty
                //&& Window.opTxt.Text != string.Empty
                //&& Window.pscTxt.Text != string.Empty
                //&& Window.uliceTxt.Text != string.Empty
                //&& Window.cpTxt.Text != string.Empty
                //&& Window.obecTxt.Text != string.Empty
                //&& Window.zemeTxt.Text != string.Empty
                //&& Window.usernameTxt.Text != string.Empty
                //&& Window.heslotxt.Text != string.Empty;
        }

        /// <summary>
        /// Nastavuje viditelnost všech oken a ovládacích prvků na základě role přihlášeného uživatele.
        /// </summary>
        /// <remarks>
        /// Tato metoda řídí zobrazení všech sekcí UI podle hierarchie oprávnění:
        /// - obcan: Vidí Kontakty, Můj účet, Moje přestupky
        /// - policista: Vidí vše pro obcana + Okrsky, Přestupky, Hlídky
        /// - administrator: Vidí vše + Admin panel, Logovací tabulka, Systémový katalog
        /// </remarks>
        private void NastavOknaPodleOpravneni()
        {
            Window.Kontakty.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            Window.Ucet.Visibility = IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;
            Window.MojePrestupky.Visibility = Uzivatel.Opravneni == "obcan" ? Visibility.Visible : Visibility.Collapsed;

            Window.Okrsky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.Prestupky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;
            Window.Hlidky.Visibility = IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

            Window.Admin.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            Window.LogovaciTabulka.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;
            Window.SystemovyKatalog.Visibility = IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;

            Window.Prihlaseni.Visibility = IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;
            Window.Registrace.Visibility = IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;

            Window.PolicisteView.KontaktyAdminGrid.IsEnabled = IsAtLeastRole("administrator") ? true : false;
        }

        /// <summary>
        /// Ověřuje, zda má uživatel požadované oprávnění nebo vyšší.
        /// </summary>
        /// <param name="requiredRole">Požadovaná role: "administrator", "policista", nebo "obcan"</param>
        /// <returns>true pokud uživatel má požadované oprávnění nebo vyšší, jinak false</returns>
        /// <remarks>
        /// Hierarchie rolí: administrator > policista > obcan
        /// </remarks>
        private bool IsAtLeastRole(string requiredRole)
        {
            // Kontrola role, pokud by se v budoucnu přidaly další úrovně
            if (Uzivatel.Opravneni == "administrator") return true;
            if (Uzivatel.Opravneni == "policista" && (requiredRole == "policista" || requiredRole == "obcan")) return true;
            if (Uzivatel.Opravneni == "obcan" && requiredRole == "obcan") return true;
            return false;
        }

        /// <summary>
        /// Registruje nového občana v systému.
        /// </summary>
        /// <remarks>
        /// Volá uloženou proceduru vytvor_uzivatele_obcana s údaji ze formuláře.
        /// Po úspěšné registraci automaticky přihlašuje nového uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě databáze</exception>
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
                Prihlas((NovaRegistrace.Username, NovaRegistrace.Heslo));
                NovaRegistrace.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualizuje údaje přihlášeného uživatele v databázi (sekce "Můj účet").
        /// </summary>
        /// <remarks>
        /// Aktualizuje heslo, uživatelské jméno a profilový obrázek voláním procedury aktualizuj_ucet.
        /// Dále aktualizuje jméno a příjmení voláním příslušné procedury (obcan/policista).
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě databáze</exception>
        [RelayCommand]
        private void AktualizovatUcet()
        {
            try
            {
                using (OracleCommand cmdUcet = new OracleCommand("aktualizuj_ucet", conn))
                {


                    cmdUcet.CommandType = CommandType.StoredProcedure;
                    cmdUcet.BindByName = true;

                    cmdUcet.Parameters.Add("p_id", OracleDbType.Int32).Value = uzivatel.Id;
                    cmdUcet.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = uzivatel.Username;
                    cmdUcet.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Password;
                    cmdUcet.Parameters.Add("p_obrazek", OracleDbType.Blob).Value = uzivatel.ObrazekBytes;


                    cmdUcet.ExecuteNonQuery();
                }
                conn.Commit();


                string volanaMetoda;
                if (Uzivatel.Opravneni == "obcan")
                {
                    volanaMetoda = "aktualizuj_jmeno_prijmeni_obcana";
                }
                else
                {
                    volanaMetoda = "aktualizuj_jmeno_prijmeni_policisty";
                }

                using (OracleCommand cmdJmenoPrijmeni = new OracleCommand(volanaMetoda, conn))
                {


                    cmdJmenoPrijmeni.CommandType = CommandType.StoredProcedure;
                    cmdJmenoPrijmeni.BindByName = true;

                    // p_idUzivatele number, p_jmeno varchar2, p_prijmeni varchar2
                    cmdJmenoPrijmeni.Parameters.Add("p_idUzivatele", OracleDbType.Int32).Value = Uzivatel.Id;
                    cmdJmenoPrijmeni.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = Uzivatel.Jmeno;
                    cmdJmenoPrijmeni.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = Uzivatel.Prijmeni;

                    cmdJmenoPrijmeni.ExecuteNonQuery();
                }
                conn.Commit();

                MessageBox.Show($"Uživatel {Uzivatel.Jmeno} Heslo {Uzivatel.Prijmeni}.", "Aktualizace", MessageBoxButton.OK, MessageBoxImage.Information);
                NovaRegistrace.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při registraci: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Načítá data z databáze při změně aktivní záložky.
        /// </summary>
        [RelayCommand]
        private void ZmenaOkna(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.TabControl)
            {


                if (Window.Admin.IsSelected)
                {
                    NacistUzivatele();
                }
                else if (Window.Kontakty.IsSelected)
                {
                    NacistKontakty();
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
                else if (Window.LogovaciTabulka.IsSelected)
                {
                    NacistLogovaciTabulku();

                }
                else if (Window.SystemovyKatalog.IsSelected)
                {
                    NacistSystemovyKatalog();
                }
            }
        }

        /// <summary>
        /// Načítá seznam všech uživatelů z databáze do kolekce.
        /// </summary>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistUzivatele()
        {
            try
            {
                string sql = @"SELECT * FROM vsichniUzivatele";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    UzivatelItemsSource = dt.DefaultView;
                    Users.Clear();
                    foreach (DataRow item in dt.Rows)
                    {

                        Users.Add(new Uzivatel
                        {
                            Id = (int)item.Field<decimal>("iduzivatele"),
                            Username = item.Field<string>("prihlasovacijmeno"),
                            Password = item.Field<string>("heslo"),
                            Opravneni = item.Field<string>("nazevopravneni")
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá systémový katalog z databáze a zobrazuje ho v dataGridu.
        /// </summary>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistSystemovyKatalog()
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
                    Window.SystemovyKatalogView.systemovyKatalogGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá logovací tabulku z databáze.
        /// </summary>
        /// <remarks>
        /// Zobrazuje historii akcí uživatelů v administračním panelu.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistLogovaciTabulku()
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
                    Window.LogovaciTabulkaView.logovaciTabulkaGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá seznam policistů z databáze.
        /// </summary>
        /// <remarks>
        /// Zobrazuje policisty na kartě Kontakty.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistKontakty()
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

                    UzivatelItemsSource = dt.DefaultView;
                    Policiste.Clear();
                    foreach (DataRow item in dt.Rows)
                    {

                        Policiste.Add(new Policista
                        {
                            Id = (int)item.Field<decimal>("idpolicisty"),
                            Jmeno = item.Field<string>("jmeno"),
                            Prijmeni = item.Field<string>("prijmeni"),
                            Hodnost = item.Field<string>("hodnost"),
                            Nadrizeny = item.Field<string>("nadrizeny"),
                            Stanice = item.Field<string>("stanice")
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá seznam všech přestupků z databáze.
        /// </summary>
        /// <remarks>
        /// Zobrazuje přestupky na kartě Přestupky (dostupné pro policisty a administrátory).
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistPrestupky()
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
                    Prestupky.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        Prestupky.Add(new Prestupek
                        {  
                            IdPrestupku = (int)item.Field<decimal>("idprestupku"),
                            IdObcana = (int)item.Field<decimal>("idobcana"),
                            TypPrestupku = item.Field<string>("prestupek"),
                            Datum = item.Field<DateTime>("datum").Date,
                            JmenoObcana = item.Field<string>("jmenoobcana"),
                            Poznamka = item.Field<string>("poznamka")
                        });
                    }
                    //Window.EvidencePrestupkuView.PrestupkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání přestupků: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá přestupky přihlášeného občana.
        /// </summary>
        /// <remarks>
        /// Dostupné pouze pro role "obcan". Filtruje data podle ID aktuálně přihlášeného uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistMojePrestupky()
        {
            try
            {
                string sql = @"
                        SELECT * FROM prestupkyview
                        where idobcana = :id";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {

                    cmd.Parameters.Add(new OracleParameter("id", Uzivatel.Id));
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

        /// <summary>
        /// Načítá seznam všech hlídek z databáze.
        /// Zobrazuje hlídky na kartě Hlídky (dostupné pro policisty a administrátory).
        /// </Summary>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistHlidky()
        {
            try
            {

                string sql = @"select * from hlidkyView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Hlidky.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        Hlidky.Add(new Hlidka
                        {
                            IdHlidky = (int)item.Field<decimal>("idhlidky"),
                            NazevHlidky = item.Field<string>("nazevhlidky"),
                            Nazev=item.Field<string>("nazev"),
                        });
                    }
                    //Window.HlidkyView.HlidkyGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání hlídek: " + ex.Message);
            }
        }

        /// <summary>
        /// Načítá seznam všech okrsků z databáze.
        /// </summary>
        /// <remarks>
        /// Zobrazuje okrsky na kartě Okrsky (dostupné pro policisty a administrátory).
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        private void NacistOkrsky()
        {
            try
            {
                string sql = @"
                SELECT * FROM okrskyView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    adapter.Fill(dt);
                    Okrsky.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        Okrsky.Add(new Okrsek
                        {
                            Id = (int)item.Field<decimal>("idokrsku"),
                            Nazev = item.Field<string>("nazev")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání okrsků: " + ex.Message);
            }
        }


        /// <summary>
        /// Otevře dialog pro výběr a nahrání profilového obrázku.
        /// </summary>
        /// <remarks>
        /// Přijímá formáty JPG, JPEG a PNG. Obrázek se uloží v paměti a bude commitován při úpravě účtu.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě čtení souboru</exception>
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

        /// <summary>
        /// Konvertuje binární data obrázku na BitmapImage pro zobrazení v UI.
        /// </summary>
        /// <param name="imageBytes">Pole bajtů s daty obrázku</param>
        /// <returns>BitmapImage objekt připravený k zobrazení</returns>
        /// <remarks>
        /// Obrázek je "zmrazen" (Freeze) pro optimalizaci výkonu v WPF.
        /// </remarks>
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

        /// <summary>
        /// Odstraňuje profilový obrázek přihlášeného uživatele.
        /// </summary>
        /// <remarks>
        /// Nastavuje vlastnosti Obrazek a ObrazekBytes na null. Změna se commituje při úpravě účtu.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě operace</exception>
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

        /// <summary>
        /// Aktualizuje údaje policisty v databázi na základě úprav v DataGridu Kontakty.
        /// </summary>
        /// <param name="radek">DataRowView s upravenými daty (Jmeno, Prijmeni, Hodnost...)</param>
        [RelayCommand]
        public void UpravitKontakty(object radek)
        {
            var policistaRow = radek as Policista;

            if (policistaRow != null)
            {
                try
                {
                    if (policistaRow.Zmenen)
                    {
                        policistaRow.Uloz(conn);
                        NacistKontakty();

                        MessageBox.Show("Úprava policisty byla úspěšně provedena.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Odstraní záznam policisty z databáze.
        /// </summary>
        /// <param name="radek">DataRowView s daty policisty k odstranění (Identifikace přes IDPOLICISTY)</param>
        [RelayCommand]
        public void OdebratKontakty(object radek)
        {
            var policistaRow = radek as Policista;

            if (policistaRow != null)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Opravdu chcete záznam smazat??",
                        "Potvrzení smazání",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes) return;

                    policistaRow.Smaz(conn);
                    NacistKontakty();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při odebírání policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        [RelayCommand]
        public void PridatKontakty()
        {
            try
            {
                var novyPolicista = new Policista();
                string jmeno = Window.PolicisteView.pridatKontaktyJmeno.Text;
                string prijmeni = Window.PolicisteView.pridatKontaktyPrijmeni.Text;
                string hodnost = Window.PolicisteView.pridatKontaktHodnost.Text;
                string nadrizeny = Window.PolicisteView.pridatKontaktyNadrizeny.Text;
                string stanice = Window.PolicisteView.pridatKontaktyStanice.Text;
                DateTime datumNarozeni = Window.PolicisteView.pridatKontaktyDatum.Text != string.Empty ? Convert.ToDateTime(Window.PolicisteView.pridatKontaktyDatum.Text) : DateTime.MinValue;
                int plat = Window.PolicisteView.pridatKontaktyPlat.Text != string.Empty ? Convert.ToInt32(Window.PolicisteView.pridatKontaktyPlat.Text) : 0;
                novyPolicista.Pridej(conn, jmeno, prijmeni, hodnost, nadrizeny, stanice, plat, datumNarozeni);
                NacistKontakty();

                MessageBox.Show("Nový policista byl úspěšně přidán.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nového policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualizuje název Okrsku v databázi.
        /// </summary>
        /// <param name="radek">DataRowView s upraveným názvem (Identifikace přes IDOKRSKU)</param>
        [RelayCommand]
        public void UpravitOkrsek(object radek)
        {
            var row = radek as Okrsek;

            if (row != null)
            {
                try
                {
                    if (row.Zmenen)
                    {
                        row.Uloz(conn);
                        NacistOkrsky();

                        MessageBox.Show("Úprava okrsku byla úspěšně provedena.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy okrsku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Odstraní záznam Okrsku z databáze.
        /// </summary>
        /// <param name="radek">DataRowView s daty Okrsku k odstranění (Identifikace přes IDOKRSKU)</param>
        [RelayCommand]
        public void OdebratOkrsek(object radek)
        {
            var row = radek as Okrsek;

            if (row != null)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Opravdu chcete záznam smazat??",
                        "Potvrzení smazání",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes) return;

                    row.Smaz(conn);
                    NacistOkrsky();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při odebírání okrsku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        [RelayCommand]
        public void PridatOkrsek()
        {
            try
            {
                Okrsek novyOkrsek = new Okrsek();
                string nazev = Window.OkrskyView.pridatOkrsekNazev.Text;
                novyOkrsek.Pridej(conn, nazev);
                
                MessageBox.Show("Nový okrsek byl úspěšně přidán.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                NacistOkrsky();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nového policisty: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualizuje záznam přestupku (typ a poznámku) v databázi.
        /// </summary>
        /// <param name="radek">DataRowView s upravenými daty (Identifikace přes IDPRESTUPKU)</param>
        [RelayCommand]
        public void UpravitPrestupek(object radek)
        {
            var row = radek as Prestupek;

            if (row != null)
            {
                try
                {
                    if (row.Zmenen)
                    {
                        row.Uloz(conn);
                        MessageBox.Show("Úprava přestupku byla úspěšně provedena.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                        NacistPrestupky();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy přestupku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Odstraní záznam Přestupku z databáze.
        /// </summary>
        /// <param name="radek">DataRowView s daty Přestupku k odstranění (Identifikace přes IDPRESTUPKU)</param>
        [RelayCommand]
        public void OdebratPrestupek(object radek)
        {
            var row = radek as Prestupek;

            if (row != null)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Opravdu chcete záznam smazat??",
                        "Potvrzení smazání",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes) return;

                    row.Smaz(conn);
                    NacistPrestupky();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při odebírání okrsku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        [RelayCommand]
        public void PridatPrestupek()
        {
            try
            {
                Prestupek novyPrestupek = new Prestupek();
         
                string typPrestupku = Window.EvidencePrestupkuView.pridatPrestupekTyp.Text;
                string popisPrestupku = Window.EvidencePrestupkuView.pridatPrestupekPopisZasahu.Text;
                string jmenoObcana = Window.EvidencePrestupkuView.pridatPrestupekObcan.Text;
                string ulice = Window.EvidencePrestupkuView.pridatPrestupekUlice.Text;
                string cisloPopisne = Window.EvidencePrestupkuView.pridatPrestupekCisloPopisne.Text;
                int cp = int.Parse(cisloPopisne);

                string psc = Window.EvidencePrestupkuView.pridatPrestupekPSC.Text;
                if (psc.Length < 5)
                {
                    MessageBox.Show("PSČ musí mít 5 znaků.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string obec = Window.EvidencePrestupkuView.pridatPrestupekObec.Text;
                novyPrestupek.Pridej(conn,ulice, cp,obec, psc, typPrestupku, popisPrestupku, jmenoObcana);
                MessageBox.Show("Nový přestupek byl úspěšně přidán.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                NacistPrestupky();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nového přestupku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualizuje záznam Hlídky (název a typ) v databázi.
        /// </summary>
        /// <param name="radek">DataRowView s upravenými daty (Identifikace přes IDHLIDKY)</param>
        [RelayCommand]
        public void UpravitHlidku(object radek)
        {
            var row = radek as Hlidka;

            if (row != null)
            {
                try
                {
                    if (row.Zmenen)
                    {
                        row.Uloz(conn);
                        NacistHlidky();

                        MessageBox.Show("Úprava okrsku byla úspěšně provedena.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při zpracování úpravy okrsku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void PridatHlidku()
        {
            try
            {
                Hlidka novaHlidka = new Hlidka();
                string nazev = Window.HlidkyView.pridatHlidkuNazev.Text;
                string typ = Window.HlidkyView.pridatHlidkuTyp.Text;
                novaHlidka.Pridej(conn, nazev, typ);
                NacistHlidky();
                MessageBox.Show("Nová hlídka byla úspěšně přidána.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nové hlídky: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Odstraní záznam Hlídky z databáze.
        /// </summary>
        /// <param name="radek">DataRowView s daty Hlídky k odstranění (Identifikace přes IDHLIDKY)</param>
        [RelayCommand]
        public async Task OdebratHlidku(object radek)
        {
            var row = radek as Hlidka;

            if (row != null)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Opravdu chcete záznam smazat??",
                        "Potvrzení smazání",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes) return;

                    row.Smaz(conn);
                    NacistHlidky();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba při odebírání okrsku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void NastavComboboxy()
        {
            NactiOpravneni();
            NacistHodnosti();
            NacistTypyPrestupku();
            NacistTypyHlidky();
        }

        private void NactiOpravneni()
        {
            try
            {
                string sql = @"select * from opravneniView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    OpravneniSeznam.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        OpravneniSeznam.Add(item.Field<string>("nazevopravneni"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }
        private void NacistHodnosti()
        {
            try
            {
                string sql = @"select * from hodnostiView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    HodnostiSeznam.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        HodnostiSeznam.Add(item.Field<string>("nazev"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }

        private void NacistTypyPrestupku()
        {
            try
            {
                string sql = @"select * from typy_prestupkuView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Typy_prestupkuSeznam.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        Typy_prestupkuSeznam.Add(item.Field<string>("prestupek"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }

        private void NacistTypyHlidky()
        {
            try
            {
                string sql = @"select * from typy_hlidkyView";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Typy_hlidkySeznam.Clear();
                    foreach (DataRow item in dt.Rows)
                    {
                        Typy_hlidkySeznam.Add(item.Field<string>("nazev"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }

        //TODO potom smazat
        private void VytvorVieNecoVDB(string sql)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatelů: " + ex.Message);
            }
        }
    }
}