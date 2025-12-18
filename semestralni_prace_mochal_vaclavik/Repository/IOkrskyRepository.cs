using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    public interface IOkrskyRepository
    {
        Task<List<Okrsek>> GetOkrskyAsync();

        Task UpravitOkrsek(Okrsek okrsek);
        Task PridatOkrsek(string nazev);
        Task OdebratOkrsek(Okrsek okrsek);
    }
}
