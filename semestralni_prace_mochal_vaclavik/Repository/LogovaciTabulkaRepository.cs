using semestralni_prace_mochal_vaclavik.OracleConn;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    class LogovaciTabulkaRepository : ILogovaciTabulkaRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public LogovaciTabulkaRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<DataTable> GetLogovaciTabulkaAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM logovaci_tabulkaview";
            using var reader = await cmd.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }
}
