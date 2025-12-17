using Microsoft.VisualBasic.ApplicationServices;
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
    public class AdminNastaveniRepository : IAdminNastaveniRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public AdminNastaveniRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public async Task<List<Uzivatel>> GetUzivateleAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM vsichniUzivatele";

            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<Uzivatel>();
            while (await reader.ReadAsync())
            {
                result.Add(new Uzivatel
                {
                    Id = reader.GetInt32("iduzivatele"),
                    Username = reader.GetString("prihlasovacijmeno"),
                    Password = reader.GetString("heslo"),
                    Opravneni = reader.GetString("nazevopravneni")
                });
            }

            return result;
        }
        
        public List<string> GetOpravneni()
        {
            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from opravneniView";

            using var reader = cmd.ExecuteReader();

            var result = new List<string>();
            while (reader.Read())
            {
                result.Add(
                    reader.GetString("nazevopravneni")
                );
            }

            return result;
        }
    }
}
