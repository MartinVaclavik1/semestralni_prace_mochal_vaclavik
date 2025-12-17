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
    }
}
