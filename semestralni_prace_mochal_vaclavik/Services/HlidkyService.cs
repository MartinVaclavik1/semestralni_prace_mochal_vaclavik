using semestralni_prace_mochal_vaclavik.Tridy;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public class HlidkyService : IHlidkyService
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

        public async Task OdebratHlidku(Hlidka hlidka)
        {
            await hlidkaRepository.OdebratHlidku(hlidka);
        }

        public async Task UpravitHlidku(Hlidka hlidka)
        {
            await hlidkaRepository.UpravitHlidku(hlidka);
        }

        public async Task PridatHlidku(string nazev, string typ)
        {
            await hlidkaRepository.PridatHlidku(nazev, typ);
        }
    }
}