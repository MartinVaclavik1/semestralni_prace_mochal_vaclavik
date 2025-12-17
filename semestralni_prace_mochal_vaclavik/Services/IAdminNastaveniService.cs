using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public interface IAdminNastaveniService
    {
        Task<List<Uzivatel>> GetUzivateleAsync();
        List<string> GetOpravneni();
    }
}
