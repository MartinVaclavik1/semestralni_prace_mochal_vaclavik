using System;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.ManagedDataAccess.Client; // Důležité: Přidat toto using

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindow : Window
    {
        // 1. Zde nastavte připojovací řetězec (zkopírujte údaje z přihlašování)
        // Všimněte si formátu s dvojtečkou pro SID: fei-sql3.upceucebny.cz:1521:BDAS

        string connectionString = "User Id=ST72536;Password=||DejTamSvojeHeslo||;Data Source=fei-sql3.upceucebny.cz:1521:BDAS;";

        public MainWindow()
        {
            InitializeComponent();

            // Načíst data hned po spuštění (nebo to můžete dát až po kliknutí na záložku)
            //NacistKontakty();
        }

        // Metoda pro načtení dat do tabulky Kontakty
        private void NacistKontakty()
        {
            try
            {
                // Vytvoření připojení
                using (OracleConnection pripojeni = new OracleConnection(connectionString))
                {
                    pripojeni.Open();

                    // SQL dotaz - spojíme Policistu s Hodností, aby to hezky vypadalo
                    string sql = @"
                        SELECT 
                            p.jmeno, 
                            p.prijmeni, 
                            h.nazev AS hodnost, 
                            s.nazev AS stanice 
                        FROM policista p
                        JOIN hodnost h ON p.hodnost_idhodnosti = h.idhodnosti
                        JOIN policejni_stanice s ON p.policejni_stanice_idstanice = s.idstanice";

                    using (OracleCommand cmd = new OracleCommand(sql, pripojeni))
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
                System.Windows.MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }

        // Příklad načtení detailu uživatele (pro záložku Můj účet)
        // Tuto metodu zavolejte, když budete chtít načíst konkrétního člověka (např. ID 1)
        private void NacistDetailUzivatele(int idUzivatele)
        {
            try
            {
                using (OracleConnection pripojeni = new OracleConnection(connectionString))
                {
                    pripojeni.Open();

                    // Příklad dotazu pro občana
                    string sql = "SELECT jmeno, prijmeni, ulice, postovnismerovacicislo, obec, zeme " +
                                 "FROM obcan o JOIN adresa a ON o.adresa_idadresy = a.idadresy " +
                                 "WHERE o.uzivatel_iduzivatele = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, pripojeni))
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
                System.Windows.MessageBox.Show("Chyba detailu: " + ex.Message);
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
    }
}