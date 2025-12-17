using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class OkrskyService : IOkrskyService
    {
        private readonly IOkrskyRepository repo;

        public OkrskyService(IOkrskyRepository repo)
        {
            this.repo = repo;

        }
        public Task<List<Okrsek>> GetOkrskyAsync()
        {
            return repo.GetOkrskyAsync();
        }
    }
}
