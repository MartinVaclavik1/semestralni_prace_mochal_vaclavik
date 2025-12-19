using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    class SystemovyKatalogService : ISystemovyKatalogService
    {
        private readonly Repository.ISystemovyKatalogRepository systemovyKatalogRepository;

        public SystemovyKatalogService(Repository.ISystemovyKatalogRepository repo)
        {
            this.systemovyKatalogRepository = repo;
        }

        public async Task<DataTable> GetSystemovyKatalogAsync()
        {
            return await systemovyKatalogRepository.GetSystemovyKatalogAsync();
        }
    }
}
