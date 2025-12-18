using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.OracleConn;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    class MojePrestupkyRepository : IMojePrestupkyRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public MojePrestupkyRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<DataTable> GetMojePrestupkyAsync(int idUzivatele)
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                        SELECT * FROM prestupkyview
                        where idobcana = :id";
            cmd.Parameters.Add(new OracleParameter("id", idUzivatele));
            using var reader = await cmd.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }
}
