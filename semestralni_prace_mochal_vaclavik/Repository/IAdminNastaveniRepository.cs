using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    public interface IAdminNastaveniRepository
    {
        Task<List<Uzivatel>> GetUzivateleAsync();
        List<string> GetOpravneni();

        Task UpravitUzivateleAsync(Uzivatel uzivatel);
        Task OdebratUzivateleAsync(Uzivatel uzivatel);
        Task PridatUzivateleAsync(string prihlasovaciJmeno, string heslo, string jmenoPolicisty, string jmenoObcana, string opravneni);
    }
}
