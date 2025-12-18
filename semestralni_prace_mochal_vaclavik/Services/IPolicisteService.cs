using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public interface IPolicisteService
    {
        Task<List<Policista>> GetPolicisteAsync();
        List<string> GetHodnosti();
        Task UpravitPolicistu(Policista policista);

        Task OdebratPolicistu(Policista policista);

        Task PridejPolicistu(string jmeno, string prijmeni, string hodnost, string nadrizeny, string stanice, int plat, DateTime datumNarozeni);

    }
}
