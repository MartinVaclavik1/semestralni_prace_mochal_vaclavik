using semestralni_prace_mochal_vaclavik.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }



        //// Viditelné pro Občana a vyšší
        //public Visibility KontaktyVisible => IsAtLeastRole("Občan") ? Visibility.Visible : Visibility.Collapsed;
        //// Viditelné pro všechny přihlášené (předpoklad: Občan+)
        //public Visibility UcetVisible => IsAtLeastRole("Občan") ? Visibility.Visible : Visibility.Collapsed;
        //// Tlačítko Potvrdit na Můj účet (Policista a Admin mohou editovat)
        //public Visibility UcetEditVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        //public Visibility OkrskyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        //public Visibility PrestupkyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;
        //public Visibility HlidkyVisible => IsAtLeastRole("Policista") ? Visibility.Visible : Visibility.Collapsed;

        //// Celý obsah na kartě Admin (DataGrid, Filtry, Akční tlačítka)
        //public Visibility AdminVisible => IsAtLeastRole("Admin") ? Visibility.Visible : Visibility.Collapsed;
        //public Visibility AdminControlsVisible => IsAtLeastRole("Admin") ? Visibility.Visible : Visibility.Collapsed;


        //private bool IsAtLeastRole(string requiredRole)
        //{
        //    // Kontrola role, pokud by se v budoucnu přidaly další úrovně
        //    if (_userRole == "Admin") return true;
        //    if (_userRole == "Policista" && (requiredRole == "Policista" || requiredRole == "Občan")) return true;
        //    if (_userRole == "Občan" && requiredRole == "Občan") return true;
        //    return false;
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}


        // Metody na tlačítka
        //private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) { /* Zde bude filtrování kontaktů */ }
        //private void PotvrditBtn_Click(object sender, RoutedEventArgs e) { /* Uložení změn */ }
        //private void Button_Click_1(object sender, RoutedEventArgs e) { }
        //private void Button_Click(object sender, RoutedEventArgs e) { }
        ////private void Button_Click_2(object sender, RoutedEventArgs e) { }
        //private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e){ }

        //private void HledatTxt_kontakty_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //}

        //private void KontaktyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //private void HlidkyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //}



    }
}