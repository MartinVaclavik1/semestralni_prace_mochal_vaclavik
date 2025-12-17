using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class PolicisteService : IPolicisteService
    {
        private readonly IPolicisteRepository repo;

        public PolicisteService(IPolicisteRepository repo)
        {
            this.repo = repo;
        }

        public Task<List<Policista>> GetPolicisteAsync() => repo.GetPolicisteAsync();

        public List<string> GetHodnosti() => repo.GetHodnosti();
    }
}
