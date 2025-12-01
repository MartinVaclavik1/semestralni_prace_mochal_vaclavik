using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Oracle.ManagedDataAccess.Client;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindow : Window
    {
        // 1. Zde nastavte připojovací řetězec (zkopírujte údaje z přihlašování)
        // Všimněte si formátu s dvojtečkou pro SID: fei-sql3.upceucebny.cz:1521:BDAS
        string connectionString = "User Id=st72536;Password=killer12;" +
                                  "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                  "(CONNECT_DATA=(SID=BDAS)));";
        public MainWindow()
        {
            InitializeComponent();

            // Načíst data hned po spuštění (nebo to můžete dát až po kliknutí na záložku)
            NacistKontakty();
            NacistUzivatele();
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
                        SELECT jmeno, prijmeni, idstanice, idhodnosti FROM policiste";

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
        // Tuto metodu zavolejte, když budete chtít načíst konkrétního člověka (např. ID 1)
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
                                // AdresaTxt asi chcete složit z ulice a čísla, pokud to v DB máte
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

                    // SQL dotaz - Načteme uživatele
                    string sql = @"
                        SELECT * FROM uzivatel";

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
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
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
                    // Tady byste normálně předal ID přihlášeného uživatele
                    // Pro test dáme natvrdo ID 1, pokud v DB existuje
                    NacistDetailUzivatele(1);
                }
            }
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            // Zde bude filtrování kontaktů
        }

        private void PotvrditBtn_Click(object sender, RoutedEventArgs e)
        {
            // Uložení změn
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}