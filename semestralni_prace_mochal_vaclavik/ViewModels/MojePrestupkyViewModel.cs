using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly PrihlasenyUzivatelService prihlasenyUzivatelService;

        [ObservableProperty]
        private DataTable mojePrestupky;

        public MojePrestupkyViewModel(IMojePrestupkyService mojePrestupkyService, PrihlasenyUzivatelService prihlasenyUzivatelService)
        {
            
            this.service = mojePrestupkyService;
            this.prihlasenyUzivatelService = prihlasenyUzivatelService;
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            try
            {
                int idUzivatele = prihlasenyUzivatelService.GetIdUzivatele();
                MojePrestupky = await service.GetMojePrestupkyAsync(idUzivatele);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Chyba načítání přestupků: " + ex.Message);
            }
        }
    }
}
