using semestralni_prace_mochal_vaclavik.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    class MojePrestupkyService : IMojePrestupkyService
    {
        private readonly IMojePrestupkyRepository mojePrestupkyRepository;

        public MojePrestupkyService(IMojePrestupkyRepository repo)
        {
            this.mojePrestupkyRepository = repo;
        }
        public async Task<DataTable> GetMojePrestupkyAsync(int idUzivatele)
            => await mojePrestupkyRepository.GetMojePrestupkyAsync(idUzivatele);
    }
}
