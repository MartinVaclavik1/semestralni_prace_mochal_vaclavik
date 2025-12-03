using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.ViewModels;
using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // 1. Zde nastavte připojovací řetězec (zkopírujte údaje z přihlašování)
        string connectionString =   "User Id=st72588;" +
                                    "Password=;" +
                                    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                    "(CONNECT_DATA=(SID=BDAS)));";

        private string _userRole = "Admin";
        private int idPrihlasenehoUzivatele = 0;

        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = new MainViewModel(this, Dispatcher);
        }


        // Viditelné pro Občana a vyšší
        public Visibility KontaktyVisible => IsAtLeastRole("Občan") ? Visibility.Visible : Visibility.Collapsed;
        // Viditelné pro všechny přihlášené (předpoklad: Občan+)
        public Visibility UcetVisible => IsAtLeastRole("Občan") ? Visibility.Visible : Visibility.Collapsed;
        // Tlačítko Potvrdit na Můj účet (Policista a Admin mohou editovat)
        public Visibility UcetEditVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility OkrskyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PrestupkyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility HlidkyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;

        // Celý obsah na kartě Admin (DataGrid, Filtry, Akční tlačítka)
        public Visibility AdminVisible => IsAtLeastRole("Admin") ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdminControlsVisible => IsAtLeastRole("Admin") ? Visibility.Visible : Visibility.Collapsed;


        private bool IsAtLeastRole(string requiredRole)
        {
            // Kontrola role, pokud by se v budoucnu přidaly další úrovně
            if (_userRole == "Admin") return true;
            if (_userRole == "Policista" && (requiredRole == "Policista" || requiredRole == "Občan")) return true;
            if (_userRole == "Občan" && requiredRole == "Občan") return true;
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Metoda pro načtení dat do tabulky Kontakty
        private void NacistKontakty()
        {
            try
            {
                // Vytvoření připojení
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

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

                        // Napojení dat do vašeho DataGridu v XAML
                        KontaktyGrid.ItemsSource = dt.DefaultView;
                    }
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
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

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
                                // Naplnění vašich TextBoxů z XAML
                                JmenoTxt.Text = reader["jmeno"].ToString();
                                PrijmeniTxt.Text = reader["prijmeni"].ToString();
                                UliceTxt.Text = reader["ulice"].ToString();
                                PSCTxt.Text = reader["postovnismerovacicislo"].ToString();
                                ZemeTxt.Text = reader["zeme"].ToString();
                            }
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
                // Vytvoření připojení
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // SQL dotaz - Načteme uživatele pro Admin Grid
                    string sql = @"
                        SELECT * FROM uzivatele"; // Změněno na "uzivatel" pro Admin Grid

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        // Použijeme DataAdapter pro naplnění tabulky
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Napojení dat do vašeho DataGridu v XAML
                        UzivateleGrid.ItemsSource = dt.DefaultView;
                    }
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
                // Vytvoření připojení
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

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
                        PrestupkyGrid.ItemsSource = dt.DefaultView;
                    }
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
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // SQL dotaz - Načtení hlídek (příklad)
                    string sql = @"
                SELECT * FROM hlidky";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        // Vazba na nový DataGrid
                        HlidkyGrid.ItemsSource = dt.DefaultView;
                    }
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
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // SQL dotaz - Načtení okrsků
                    string sql = @"
                SELECT * FROM okrsky";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        // Vazba na nový DataGrid
                        OkrskyGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání okrsků: " + ex.Message);
            }
        }

        // Obsluha přepínání záložek (máte ji v XAML jako Okna_SelectionChanged)
        private void Okna_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                if (Kontakty.IsSelected)
                {
                    NacistKontakty();
                }
                else if (Ucet.IsSelected)
                {
                    NacistDetailUzivatele(1);
                }
                else if (Admin.IsSelected)
                {
                    NacistUzivatele();
                }
                else if (Prestupky.IsSelected)
                {
                    NacistPrestupky();
                }
                else if (Hlidky.IsSelected)
                {
                    NacistHlidky();
                }
                else if (Okrsky.IsSelected)
                {
                    NacistOkrsky();
                }
            }
        }

        // Metody na tlačítka
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) { /* Zde bude filtrování kontaktů */ }
        private void PotvrditBtn_Click(object sender, RoutedEventArgs e) { /* Uložení změn */ }
        private void Button_Click_1(object sender, RoutedEventArgs e) { }
        private void Button_Click(object sender, RoutedEventArgs e) { }
        //private void Button_Click_2(object sender, RoutedEventArgs e) { }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void HledatTxt_kontakty_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}