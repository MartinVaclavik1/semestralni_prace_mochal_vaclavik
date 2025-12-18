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

        public async Task UpravitPolicistu(Policista policista) => await repo.UpravitPolicistu(policista);

        public async Task OdebratPolicistu(Policista policista) => await repo.OdebratPolicistu(policista);

        public async Task PridejPolicistu(string jmeno, string prijmeni, string hodnost, string nadrizeny, string stanice, int plat, DateTime datumNarozeni)
        => await repo.PridejPolicistu(jmeno, prijmeni, hodnost, nadrizeny, stanice, plat, datumNarozeni);
    }
}
