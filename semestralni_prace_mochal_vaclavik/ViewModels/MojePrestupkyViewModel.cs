using CommunityToolkit.Mvvm.ComponentModel;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class MojePrestupkyViewModel : ObservableObject
    {
        private readonly IMojePrestupkyService service;

        [ObservableProperty]
        private DataTable mojePrestupky;
        
        public MojePrestupkyViewModel(IMojePrestupkyService service)
        {
            
            this.service = service;
            LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                // TODO dodělat
                int idUzivatele = 0;
                MojePrestupky = await service.GetMojePrestupkyAsync(idUzivatele);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Chyba načítání přestupků: " + ex.Message);
            }
        }
    }
}
