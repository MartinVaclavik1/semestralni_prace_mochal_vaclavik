using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public interface IEvidencePrestupkuService
    {
        Task<List<Prestupek>> GetPrestupkyAsync();
        List<string> GetTypyPrestupky();
        Task OdebratPrestupekAsync(Prestupek prestupek);
        Task UpravitPrestupekAsync(Prestupek prestupek);
        Task PridatPrestupekAsync(string ulice, int cisloPopisne, string obec, string psc, string typPrestupku, string popisZasahu, string jmenoObcana);
    }
}
