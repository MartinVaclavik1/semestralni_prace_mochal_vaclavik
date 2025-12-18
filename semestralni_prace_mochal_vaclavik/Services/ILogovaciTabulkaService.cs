using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Services
{
    public interface ILogovaciTabulkaService
    {
        Task<DataTable> GetLogovaciTabulkaAsync();
    }
}
