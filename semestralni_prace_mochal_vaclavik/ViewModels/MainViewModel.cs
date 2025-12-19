using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using semestralni_prace_mochal_vaclavik.Views;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        /// Zdroj dat pro DataGrid s kontakty.
        /// </summary>
        [ObservableProperty]
        public DataView kontaktyItemsSource;

        //[ObservableProperty]
        //public DataView okrskyItemsSource;

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

        public ObservableCollection<Policista> Polic { get; set; } = new ObservableCollection<Policista>();

        public ObservableCollection<Okrsek> Okrsky { get; set; } = new ObservableCollection<Okrsek>();
        public ObservableCollection<Prestupek> Prestupky { get; set; } = new ObservableCollection<Prestupek>();
        public ObservableCollection<Hlidka> Hlidky { get; set; } = new ObservableCollection<Hlidka>();

        /// <summary>
        /// Aktuálně přihlášený uživatel.
        /// </summary>
        //[ObservableProperty]
        //private Uzivatel uzivatel;

        /// <summary>
        /// Data pro nový účet během registrace.
        /// </summary>
        [ObservableProperty]
        private Registrace novaRegistrace = new Registrace();

        /// <summary>
        /// Seznam dostupných typů oprávnění v systému.
        /// </summary>
        [ObservableProperty]
        private List<string> opravneniSeznam = new List<string>();

        [ObservableProperty]
        private List<string> typy_hlidkySeznam = new List<string>();

        public PolicisteView PolicisteView { get; }
        public OkrskyView OkrskyView { get; }
        public EvidencePrestupkuView EvidencePrestupkuView { get; }
        public AdminView AdminView { get; }
        public HlidkyView HlidkyView { get; }
        public PrihlaseniView PrihlaseniView { get; }
        public UcetView UcetView { get; }
        public LogovaciTabulkaView LogovaciTabulkaView { get; }
        public MojePrestupkyView MojePrestupkyView { get; }
        public SystemovyKatalogView SystemovyKatalogView { get; }

        [ObservableProperty]
        public PrihlasenyUzivatelService prihlasenyUzivatelService;

        [ObservableProperty]
        private int vybranyIndex = 0;

        [ObservableProperty]
        private string prihlasovaciJmeno;

        [ObservableProperty]
        private string heslo;

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
        /// public PolicisteViewModel PolicisteVM { get; }

        public MainViewModel(PolicisteView policisteView, OkrskyView okrskyView,
            EvidencePrestupkuView evidencePrestupkuView, AdminView adminView,
            HlidkyView hlidkyView, PrihlaseniView prihlaseniView,
            UcetView ucetView, LogovaciTabulkaView logovaciTabulkaView,
            PrihlasenyUzivatelService prihlasenyUzivatelService,
            MojePrestupkyView mojePrestupkyView, SystemovyKatalogView systemovyKatalogView)
        {
            PolicisteView = policisteView ?? throw new ArgumentNullException(nameof(policisteView));
            OkrskyView = okrskyView ?? throw new ArgumentNullException(nameof(okrskyView));
            EvidencePrestupkuView = evidencePrestupkuView ?? throw new ArgumentNullException(nameof(evidencePrestupkuView));
            AdminView = adminView ?? throw new ArgumentNullException(nameof(adminView));
            HlidkyView = hlidkyView ?? throw new ArgumentNullException(nameof(hlidkyView));
            PrihlaseniView = prihlaseniView ?? throw new ArgumentNullException(nameof(prihlaseniView));
            UcetView = ucetView ?? throw new ArgumentNullException(nameof(ucetView));
            LogovaciTabulkaView = logovaciTabulkaView ?? throw new ArgumentNullException(nameof(logovaciTabulkaView));

            PrihlasenyUzivatelService = prihlasenyUzivatelService;

            MojePrestupkyView = mojePrestupkyView ?? throw new ArgumentNullException(nameof(mojePrestupkyView));
            SystemovyKatalogView = systemovyKatalogView ?? throw new ArgumentNullException(nameof(systemovyKatalogView));


            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Prihlas("Oli", "12345");
        }

        /// <summary>
        /// Určuje viditelnost ovládacích prvků pro policisty.
        /// </summary>


        //public Visibility KontaktyVisible => Visibility.Collapsed;

        /// <summary>
        /// Emuluje přihlášení jiného uživatele v novém okně bez commitování do databáze.
        /// </summary>
        /// <param name = "radek" > Řádek z DataGridu s daty uživatele k emulaci</param>
        [RelayCommand]
        public async void Emulovat(object radek)
        {
            var uzivatelRow = radek as Uzivatel;
            if (uzivatelRow != null)
            {
                if (uzivatelRow.Id == PrihlasenyUzivatelService.Uzivatel.Id)
                {
                    MessageBox.Show("Nelze emulovat sám sebe!");
                    return;
                }

                try
                {
                    string jmeno = uzivatelRow.Username;
                    string heslo = uzivatelRow.Password;

                    MessageBox.Show($"Emulace uživatele: {jmeno}");

                    //var emulace = new MainWindow(new MainViewModel());
                    //emulace.Title = emulace.Title + " - EMULACE";
                    //((MainViewModel)emulace.DataContext).Prihlas((jmeno, heslo));
                    //emulace.ShowDialog();
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
                if (uzivatelRow.Id == PrihlasenyUzivatelService.Uzivatel.Id)
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
                if (uzivatelRow.Id == PrihlasenyUzivatelService.Uzivatel.Id)
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

        private void Prihlas(string jmeno, string heslo)
        {
            PrihlasovaciJmeno = jmeno;
            Heslo = heslo;
            Prihlas();
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
        [RelayCommand]
        private void Prihlas()
        {
            try
            {
                PrihlasenyUzivatelService.Prihlas(PrihlasovaciJmeno, Heslo);

                VybranyIndex = 0;

                PrihlasovaciJmeno = String.Empty;
                PrihlaseniView.PasswordBox.Clear();
                Heslo = String.Empty;

                Task.Run(() => { MessageBox.Show("Uživatel přihlášen"); });

                //    string sql = @"
                //            select * from datauctuview where id = 
                //            (select prihlaseni(:prihlJmeni,:heslo) from Dual)";

                //    using (OracleCommand cmd = new OracleCommand(sql, conn))
                //    {
                //        cmd.Parameters.Add(new OracleParameter("prihlJmeno", PrihlasovaciJmeno));
                //        cmd.Parameters.Add(new OracleParameter("heslo", Heslo));

                //        using (OracleDataReader reader = cmd.ExecuteReader())
                //        {
                //            if (reader.Read())
                //            {
                //                Uzivatel.Id = int.Parse(reader["id"].ToString());
                //                Uzivatel.Username = reader["prihlasovacijmeno"].ToString();
                //                var blob = reader.GetOracleBlob(2);
                //                if (!blob.IsNull)
                //                {
                //                    Uzivatel.ObrazekBytes = nactiByteZBLOB(blob);
                //                    Uzivatel.Obrazek = vytvorObrazek(Uzivatel.ObrazekBytes);
                //                }


                //                Uzivatel.Opravneni = reader["nazevopravneni"].ToString();

                //                if (Uzivatel.Opravneni == "obcan")
                //                {
                //                    Uzivatel.Jmeno = reader["o_jmeno"].ToString();
                //                    Uzivatel.Prijmeni = reader["o_prijmeni"].ToString();

                //                }
                //                else
                //                {
                //                    Uzivatel.Jmeno = reader["p_jmeno"].ToString();
                //                    Uzivatel.Prijmeni = reader["p_prijmeni"].ToString();
                //                }
                //                VybranyIndex = 0;

                //                PrihlasovaciJmeno = String.Empty;
                //                PrihlaseniView.PasswordBox.Clear();
                //                Heslo = String.Empty;


                //                Task.Run(() => { MessageBox.Show("Uživatel přihlášen"); });


                //            }
                //            else
                //            {
                //                MessageBox.Show("Špatné přihlašovací údaje");
                //            }
                //        }
                //    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání uživatele: " + ex.Message);
            }
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
            PrihlasenyUzivatelService.Odhlas();
            VybranyIndex = 0;
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
                PrihlasenyUzivatelService.Registrovat(NovaRegistrace);

                MessageBox.Show($"Uživatel {NovaRegistrace.Username} vytvořen.", "Registrace", MessageBoxButton.OK, MessageBoxImage.Information);
                Prihlas(NovaRegistrace.Username, NovaRegistrace.Heslo);
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
                PrihlasenyUzivatelService.AktualizovatUcet();
                UcetView.HesloTxt.Clear();
                MessageBox.Show($"Účet aktualizován.", "Aktualizace", MessageBoxButton.OK, MessageBoxImage.Information);

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
        private async Task ZmenaOkna(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.TabControl)
            {
                //if (Window.Admin.IsSelected)
                //{
                //    NacistUzivatele();
                //}
                //else if (Window.Kontakty.IsSelected)
                //{
                //    NacistKontakty();
                //}
                //else if (Window.Prestupky.IsSelected)
                //{
                //    NacistPrestupky();
                //}
                //else if (Window.MojePrestupky.IsSelected)
                //{
                //    NacistMojePrestupky();
                //}
                //else if (Window.Hlidky.IsSelected)
                //{
                //    NacistHlidky();

                //}
                //else if (Window.Okrsky.IsSelected)
                //{
                //    NacistOkrsky();
                //}
                //else if (Window.LogovaciTabulka.IsSelected)
                //{
                //    NacistLogovaciTabulku();

                //}
                //else if (Window.SystemovyKatalog.IsSelected)
                //{
                //    NacistSystemovyKatalog();
                //}
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
                    //Window.SystemovyKatalogView.systemovyKatalogGrid.ItemsSource = dt.DefaultView;
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
                    Polic.Clear();
                    foreach (DataRow item in dt.Rows)
                    {

                        Polic.Add(new Policista
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

                    cmd.Parameters.Add(new OracleParameter("id", PrihlasenyUzivatelService.Uzivatel.Id));
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
                PrihlasenyUzivatelService.NastavObrazekZBytes(imageBytes);
                //PrihlasenyUzivatelService.Uzivatel.Obrazek = vytvorObrazek(imageBytes);
                //Uzivatel.ObrazekBytes = imageBytes;

                MessageBox.Show("Profilový obrázek byl úspěšně nahrán a uložen.", "Úspěch");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při nahrávání obrázku: {ex.Message}", "Chyba");
            }
        }

        ///// <summary>
        ///// Konvertuje binární data obrázku na BitmapImage pro zobrazení v UI.
        ///// </summary>
        ///// <param name="imageBytes">Pole bajtů s daty obrázku</param>
        ///// <returns>BitmapImage objekt připravený k zobrazení</returns>
        ///// <remarks>
        ///// Obrázek je "zmrazen" (Freeze) pro optimalizaci výkonu v WPF.
        ///// </remarks>
        //private BitmapImage vytvorObrazek(byte[] imageBytes)
        //{
        //    BitmapImage img = new BitmapImage();
        //    using (MemoryStream stream = new MemoryStream(imageBytes))
        //    {
        //        img.BeginInit();
        //        img.CacheOption = BitmapCacheOption.OnLoad;
        //        img.StreamSource = stream;
        //        img.EndInit();
        //        img.Freeze();
        //    }
        //    return img;
        //}

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
            if (PrihlasenyUzivatelService.Uzivatel.Obrazek != null)
            {
                try
                {

                    PrihlasenyUzivatelService.OdeberObrazek();

                    MessageBox.Show("Profilový obrázek byl úspěšně odstraněn.", "Úspěch");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při odstraňování obrázku: {ex.Message}", "Chyba");
                }
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
                        //NacistPrestupky();
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
                    //NacistPrestupky();

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
                //string typPrestupku = Window.EvidencePrestupkuView.pridatPrestupekTyp.Text;
                //string popisPrestupku = Window.EvidencePrestupkuView.pridatPrestupekPopisZasahu.Text;
                //string jmenoObcana = Window.EvidencePrestupkuView.pridatPrestupekObcan.Text;
                //string adresa = Window.EvidencePrestupkuView.pridatPrestupekAdresa.Text;
                //string poznamka = Window.EvidencePrestupkuView.pridatPrestupekPoznamka.Text;
                //novyPrestupek.Pridej(conn, typPrestupku, popisPrestupku, jmenoObcana, adresa, poznamka);

                //string typPrestupku = Window.EvidencePrestupkuView.pridatPrestupekTyp.Text;
                //string popisPrestupku = Window.EvidencePrestupkuView.pridatPrestupekPopisZasahu.Text;
                //string jmenoObcana = Window.EvidencePrestupkuView.pridatPrestupekObcan.Text;
                ////string adresa = Window.EvidencePrestupkuView.pridatPrestupekAdresa.Text;
                //string ulice = Window.EvidencePrestupkuView.pridatPrestupekUlice.Text;
                //string cisloPopisne = Window.EvidencePrestupkuView.pridatPrestupekCisloPopisne.Text;
                //int cp = int.Parse(cisloPopisne);

                //string psc = Window.EvidencePrestupkuView.pridatPrestupekPSC.Text;
                //if (psc.Length < 5)
                //{
                //    MessageBox.Show("PSČ musí mít 5 znaků.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}
                //string obec = Window.EvidencePrestupkuView.pridatPrestupekObec.Text;
                //novyPrestupek.Pridej(conn,ulice, cp,obec, psc, typPrestupku, popisPrestupku, jmenoObcana);
                //NacistPrestupky();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidávání nového přestupku: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}