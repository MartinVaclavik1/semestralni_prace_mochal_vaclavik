using semestralni_prace_mochal_vaclavik.ViewModels;
using System.Windows;

namespace semestralni_prace_mochal_vaclavik
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }
    }
}