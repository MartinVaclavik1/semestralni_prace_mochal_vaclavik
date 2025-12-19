using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using semestralni_prace_mochal_vaclavik.Services;
using System.ComponentModel;
using System.Data;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class LogovaciTabulkaViewModel : ObservableObject
    {
        private readonly ILogovaciTabulkaService service;

        [ObservableProperty]
        private DataTable logovaciTabulka;

        public LogovaciTabulkaViewModel(ILogovaciTabulkaService service)
        {
            this.service = service;
        }
        [RelayCommand]
        public async Task LoadAsync()
        {
            try
            {
                LogovaciTabulka = await service.GetLogovaciTabulkaAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Chyba načítání logů: " + ex.Message);
            }
        }
    }
}