using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Tridy;
using semestralni_prace_mochal_vaclavik.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public partial class PrihlasenyUzivatelService : ObservableObject
    {
        private readonly PrihlasenyUzivatelRepository repo;

        [ObservableProperty]
        private Uzivatel uzivatel = new Uzivatel();

        public PrihlasenyUzivatelService(PrihlasenyUzivatelRepository repo)
        {
            this.repo = repo;
        }


        /// <summary>
        /// Přihlašuje uživatele do systému na základě přihlašovacích údajů.
        /// </summary>
        /// <param name="udaje">Tuple obsahující přihlašovací jméno a heslo</param>
        /// <remarks>
        /// Komunikuje s databází, načítá uživatelské údaje a profilový obrázek.
        /// Automaticky nastavuje viditelnost UI prvků podle role uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě komunikace s databází</exception>
        public void Prihlas(string prihlasovaciJmeno, string heslo)
        {
            Uzivatel = repo.Prihlas(prihlasovaciJmeno, heslo);
            NastavOknaPodleOpravneni();
        }
        /// <summary>
        /// Odhlašuje aktuálně přihlášeného uživatele z systému.
        /// </summary>
        /// <remarks>
        /// Resetuje data uživatele a skrývá všechny prvky UI kromě přihlašovacího formuláře Registrace a karty Domu.
        /// </remarks>
        public void Odhlas()
        {
            Uzivatel.Resetuj();
            NastavOknaPodleOpravneni();

        }

        /// <summary>
        /// Registruje nového občana v systému.
        /// </summary>
        /// <remarks>
        /// Volá uloženou proceduru vytvor_uzivatele_obcana s údaji ze formuláře.
        /// Po úspěšné registraci automaticky přihlašuje nového uživatele.
        /// </remarks>
        /// <exception cref="Exception">Vyvolána při chybě databáze</exception>
        public void Registrovat(Registrace novaRegistrace)
        {
            repo.Registrovat(novaRegistrace);
        }

        public void AktualizovatUcet()
        {
            repo.AktualizujUcet(Uzivatel);
        }

        public void NastavObrazekZBytes(byte[] obrazekBytes)
        {
            Uzivatel.Obrazek = repo.vytvorObrazek(obrazekBytes);
            Uzivatel.ObrazekBytes = obrazekBytes;
        }

        public void OdeberObrazek()
        {
            Uzivatel.Obrazek = null;
            Uzivatel.ObrazekBytes = null;
        }

        public Visibility PolicistaVisible => IsAtLeastRole("policista") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AdministratorVisible => IsAtLeastRole("administrator") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ObcanVisible => IsAtLeastRole("obcan") ? Visibility.Visible : Visibility.Collapsed;

        public Visibility JenObcanVisible => Uzivatel.Opravneni == "obcan" ? Visibility.Visible : Visibility.Collapsed;

        public Visibility NeprihlasenyVisible => IsAtLeastRole("obcan") ? Visibility.Collapsed : Visibility.Visible;

        public bool AdministratorEnabled => IsAtLeastRole("administrator") ? true : false;

        /// <summary>
        /// Ověřuje, zda má uživatel požadované oprávnění nebo vyšší.
        /// </summary>
        /// <param name="requiredRole">Požadovaná role: "administrator", "policista", nebo "obcan"</param>
        /// <returns>true pokud uživatel má požadované oprávnění nebo vyšší, jinak false</returns>
        /// <remarks>
        /// Hierarchie rolí: administrator > policista > obcan
        /// </remarks>
        private bool IsAtLeastRole(string requiredRole)
        {
            // Kontrola role, pokud by se v budoucnu přidaly další úrovně
            if (Uzivatel.Opravneni == "administrator") return true;
            if (Uzivatel.Opravneni == "policista" && (requiredRole == "policista" || requiredRole == "obcan")) return true;
            if (Uzivatel.Opravneni == "obcan" && requiredRole == "obcan") return true;
            return false;
        }

        /// <summary>
        /// Nastavuje viditelnost všech oken a ovládacích prvků na základě role přihlášeného uživatele.
        /// </summary>
        /// <remarks>
        /// Tato metoda řídí zobrazení všech sekcí UI podle hierarchie oprávnění:
        /// - obcan: Vidí Kontakty, Můj účet, Moje přestupky
        /// - policista: Vidí vše pro obcana + Okrsky, Přestupky, Hlídky
        /// - administrator: Vidí vše + Admin panel, Logovací tabulka, Systémový katalog
        /// </remarks>
        private void NastavOknaPodleOpravneni()
        {
            OnPropertyChanged(nameof(PolicistaVisible));
            OnPropertyChanged(nameof(AdministratorVisible));
            OnPropertyChanged(nameof(ObcanVisible));
            OnPropertyChanged(nameof(JenObcanVisible));
            OnPropertyChanged(nameof(NeprihlasenyVisible));
            OnPropertyChanged(nameof(AdministratorEnabled));
        }
        public int GetIdUzivatele()
        {
            return Uzivatel.Id;
        }
    }
}
