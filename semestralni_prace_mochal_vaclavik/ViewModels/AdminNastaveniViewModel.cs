using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.ApplicationServices;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using semestralni_prace_mochal_vaclavik.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.ViewModels
{
    public partial class AdminNastaveniViewModel : ObservableObject
    {
        private readonly IAdminNastaveniService service;

        [ObservableProperty]
        public ObservableCollection<Uzivatel> uzivatele = new();

        [ObservableProperty]
        private List<string> opravneniSeznam = new List<string>();

        private PrihlasenyUzivatelService prihlasenyUzivatelService;

        public AdminNastaveniViewModel(IAdminNastaveniService service, PrihlasenyUzivatelService prihlasenyUzivatelService)
        {
            this.service = service;
            this.prihlasenyUzivatelService = prihlasenyUzivatelService;
            NactiHodnosti();
            LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Uzivatele.Clear();
            var uzivatele_nacteni = await service.GetUzivateleAsync();
            foreach (var c in uzivatele_nacteni)
                Uzivatele.Add(c);
        }

        private void NactiHodnosti()
        {
            try
            {
                OpravneniSeznam.Clear();
                foreach (var item in service.GetOpravneni())
                {
                    OpravneniSeznam.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [RelayCommand]
        private void Emulovat(object radek)
        {
            var uzivatelRow = radek as Uzivatel;
            if (uzivatelRow != null)
            {
                if (uzivatelRow.Id == prihlasenyUzivatelService.Uzivatel.Id)
                {
                    MessageBox.Show("Nelze emulovat sám sebe!");
                    return;
                }


                string jmeno = uzivatelRow.Username;
                string heslo = uzivatelRow.Password;

                MessageBox.Show($"Emulace uživatele: {jmeno}");

                var exe = Process.GetCurrentProcess().MainModule!.FileName;

                Process.Start(new ProcessStartInfo
                {
                    FileName = exe,
                    ArgumentList = { "--emulace", jmeno, heslo },
                    UseShellExecute = true
                });
            }
        }
    }
}
