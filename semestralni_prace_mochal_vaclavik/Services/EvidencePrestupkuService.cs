using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class EvidencePrestupkuService : IEvidencePrestupkuService
    {
        private readonly IEvidencePrestupkuRepository repo;

        public EvidencePrestupkuService(IEvidencePrestupkuRepository repo)
        {
            this.repo = repo;
        }

        public Task<List<Prestupek>> GetPrestupkyAsync() => repo.GetPrestupkyAsync();

        public List<string> GetTypyPrestupky() => repo.GetTypyPrestupky();

        public async Task OdebratPrestupekAsync(Prestupek prestupek)
        {
            await repo.OdebratPrestupekAsync(prestupek);
        }

        public async Task PridatPrestupekAsync(string ulice, int cisloPopisne, string obec, string psc, string typPrestupku, string popisZasahu, string jmenoObcana)
        {
            await repo.PridatPrestupekAsync(ulice, cisloPopisne, obec, psc, typPrestupku, popisZasahu, jmenoObcana);
        }

        public async Task UpravitPrestupekAsync(Prestupek prestupek)
        {
            await repo.UpravitPrestupekAsync(prestupek);
        }
    }
}
