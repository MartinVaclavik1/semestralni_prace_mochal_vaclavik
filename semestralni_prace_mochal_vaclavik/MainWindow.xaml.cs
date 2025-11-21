using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace semestralni_prace_mochal_vaclavik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Prestupky.IsEnabled = false;    //dokud bude uživatel odhlášen, tak některé okna budou disabeled jako toto
            Prestupky.Visibility = Visibility.Hidden;
            Okna.UseLayoutRounding = true;
            //TODO přidat popup okno s přihlášením - možnost vytvořit nový účet a pokračovat jako host bez přihlášení
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //možná budeme využívat vytváření nových oken za běhu
            var testItem = new TabItem();
            testItem.Header = "Test nového okna";
            Okna.Items.Add(testItem);

            Prestupky.Visibility = Visibility.Hidden; //nebo možnost schovávat a zobrazovat okna
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Okna_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void PotvrditBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }
    }
}