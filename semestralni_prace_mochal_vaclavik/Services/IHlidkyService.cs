using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public interface IHlidkyService 
    {
        Task<List<Hlidka>> GetHlidkyAsync();
        List<string> GetTypyHlidky();
        Task UpravitHlidku(Hlidka hlidka);
        Task PridatHlidku(string nazev, string typ);
        Task OdebratHlidku(Hlidka hlidka);
    }
}
