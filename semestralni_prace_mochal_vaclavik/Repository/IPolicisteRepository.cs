using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    public interface IPolicisteRepository
    {
        Task<List<Policista>> GetPolicisteAsync();
        List<string> GetHodnosti();
    }
}
