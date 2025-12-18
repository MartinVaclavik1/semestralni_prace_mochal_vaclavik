using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.OracleConn;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    public class OkrskyRepository : IOkrskyRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public OkrskyRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<Okrsek>> GetOkrskyAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM okrskyView";

            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<Okrsek>();
            while (await reader.ReadAsync())
            {
                result.Add(new Okrsek
                {
                    Id = reader.GetInt32("idokrsku"),
                    Nazev = reader.GetString("nazev")
                });
            }

            return result;
        }
        public async Task OdebratOkrsek(Okrsek okrsek)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            okrsek.Smaz(conn);

        }

        public async Task UpravitOkrsek(Okrsek okrsek)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            okrsek.Uloz(conn);
        }

        public async Task PridatOkrsek(string nazev)
        {
            using var conn = (OracleConnection)connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            new Okrsek().Pridej(conn, nazev);
        }
    }
}
