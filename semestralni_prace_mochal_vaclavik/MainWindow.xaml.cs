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
    }
}