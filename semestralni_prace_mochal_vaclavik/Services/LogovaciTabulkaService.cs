using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    class LogovaciTabulkaService : ILogovaciTabulkaService
    {
        private readonly Repository.ILogovaciTabulkaRepository logovaciTabulkaRepository;

        public LogovaciTabulkaService(Repository.ILogovaciTabulkaRepository repo)
        {
            this.logovaciTabulkaRepository = repo;
        }

        public async Task<System.Data.DataTable> GetLogovaciTabulkaAsync()
            => await logovaciTabulkaRepository.GetLogovaciTabulkaAsync();
    }
}
