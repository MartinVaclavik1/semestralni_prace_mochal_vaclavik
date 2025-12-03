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
        string connectionString = "User Id=st72588;" +
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
                            kontaktyItemsSource = dt.DefaultView; 
                        });
                        
                        foreach (var x in dt.DefaultView)
                        {
                            MessageBox.Show(x.ToString());
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontaktů: " + ex.Message);
            }
        }
    }
}
