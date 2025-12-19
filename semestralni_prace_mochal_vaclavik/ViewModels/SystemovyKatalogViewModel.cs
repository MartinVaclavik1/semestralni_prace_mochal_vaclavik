using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class SystemovyKatalogViewModel : ObservableObject
    {
        private readonly Services.ISystemovyKatalogService service;

        [ObservableProperty]
        private DataTable systemovyKatalog;

        public SystemovyKatalogViewModel(Services.ISystemovyKatalogService service)
        {
            this.service = service;
        }
        [RelayCommand]
        private async Task LoadAsync()
        {
            try
            {
                SystemovyKatalog = await service.GetSystemovyKatalogAsync();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Chyba načítání systémového katalogu: " + ex.Message);
            }
        }
    }
}
