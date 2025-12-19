using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class AdminNastaveniService : IAdminNastaveniService
    {
        private readonly IAdminNastaveniRepository repo;

        public AdminNastaveniService(IAdminNastaveniRepository repo)
        {
            this.repo = repo;
        }
        public Task<List<Uzivatel>> GetUzivateleAsync() => repo.GetUzivateleAsync();
        
        public List<string> GetOpravneni() => repo.GetOpravneni();

        public async Task UpravitUzivateleAsync(Uzivatel uzivatel)
        {
            await repo.UpravitUzivateleAsync(uzivatel);
        }

        public async Task OdebratUzivateleAsync(Uzivatel uzivatel)
        {
            await repo.OdebratUzivateleAsync(uzivatel);
        }

        public async Task PridatUzivateleAsync(string prihlasovaciJmeno, string heslo, string jmenoPolicisty, string jmenoObcana, string opravneni)
        {
            await repo.PridatUzivateleAsync(prihlasovaciJmeno, heslo, jmenoPolicisty, jmenoObcana, opravneni);
        }
    }
}
