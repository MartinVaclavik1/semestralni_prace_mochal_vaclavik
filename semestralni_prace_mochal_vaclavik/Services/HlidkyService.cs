using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class HlidkyService: IHlidkyService
    {
        private readonly Repository.IHlidkyRepository hlidkaRepository;
        public HlidkyService(Repository.IHlidkyRepository repo)
        {
            this.hlidkaRepository = repo;
        }

        public Task<List<Hlidka>> GetHlidkyAsync()
            => hlidkaRepository.GetHlidkyAsync();

        public List<string> GetTypyHlidky()
        {
            return hlidkaRepository.GetTypyHlidky();
        }
    }
}
