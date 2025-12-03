using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly string connectionString = "User Id=st72588;" +
                                    "Password=;" +
                                    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                    "(CONNECT_DATA=(SID=BDAS)));";
        private MainWindow window {  get; set; }
        private Dispatcher _dispatcher { get; }
        [ObservableProperty]
        public DataView kontaktyItemsSource;
        public MainViewModel(MainWindow window, Dispatcher dispatcher) { 
            this.window = window;
            _dispatcher = dispatcher;
        }

        [RelayCommand]
        public async void Emulovat()
        {
            MessageBox.Show("Test");
        }

        [RelayCommand(CanExecute =nameof(ZkontrolovatHeslo))]
        private void Prihlas((string PrihlasovaciJmeno, string Heslo) udaje)
        {
            
            MessageBox.Show($"{udaje.PrihlasovaciJmeno} , {udaje.Heslo}");
        }

        
        public bool ZkontrolovatHeslo()
        {
            return true;
        }

        [RelayCommand]
        private void ZmenaOkna()
        {
            if (window.Kontakty.IsSelected)
            {
                NacistKontakty();
            }
            else if (window.Ucet.IsSelected)
            {
                NacistDetailUzivatele(1);
            }
            else if (window.Admin.IsSelected)
            {
                NacistUzivatele();
            }
            else if (window.Prestupky.IsSelected)
            {
                NacistPrestupky();
            }
            else if (window.Hlidky.IsSelected)
            {
                NacistHlidky();
            }
            else if (window.Okrsky.IsSelected)
            {
                NacistOkrsky();
            }
        }

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

                        
                        _dispatcher.Invoke(() => { 
                            window.KontaktyGrid.ItemsSource = dt.DefaultView; 
                        });
                        
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
                                window.JmenoTxt.Text = reader["jmeno"].ToString();
                                window.PrijmeniTxt.Text = reader["prijmeni"].ToString();
                                window.UliceTxt.Text = reader["ulice"].ToString();
                                window.PSCTxt.Text = reader["postovnismerovacicislo"].ToString();
                                window.ZemeTxt.Text = reader["zeme"].ToString();
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
                        window.UzivateleGrid.ItemsSource = dt.DefaultView;
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
                        window.PrestupkyGrid.ItemsSource = dt.DefaultView;
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
                        window.HlidkyGrid.ItemsSource = dt.DefaultView;
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
                        window.OkrskyGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání okrsků: " + ex.Message);
            }
        }
    }
}
