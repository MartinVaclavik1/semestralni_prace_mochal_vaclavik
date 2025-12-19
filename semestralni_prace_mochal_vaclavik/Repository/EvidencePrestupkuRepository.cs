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
    public class EvidencePrestupkuRepository : IEvidencePrestupkuRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public EvidencePrestupkuRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<Prestupek>> GetPrestupkyAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM prestupkyview";

            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Prestupek>();
            while (await reader.ReadAsync())
            {
                result.Add(new Prestupek
                {
                    IdPrestupku = reader.GetInt32("idprestupku"),
                    IdObcana = reader.GetInt32("idobcana"),
                    TypPrestupku = reader.GetString("prestupek"),
                    Datum = reader.GetDateTime("datum"),
                    JmenoObcana = reader.GetString("jmenoobcana"),
                    Poznamka = reader.IsDBNull("poznamka") ? "" : reader.GetString("poznamka")
                });
            }

            return result;
        }

        public List<string> GetTypyPrestupky()
        {
            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from typy_prestupkuView";

            using var reader = cmd.ExecuteReader();

            var result = new List<string>();
            while (reader.Read())
            {
                result.Add(
                    reader.GetString("prestupek")
                );
            }

            return result;
        }

        public async Task OdebratPrestupekAsync(Prestupek prestupek)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            prestupek.Smaz((OracleConnection)conn);
        }

        public async Task PridatPrestupekAsync(string ulice, int cisloPopisne, string obec, string psc, string typPrestupku, string popisZasahu, string jmenoObcana)
        {
            using var conn = (OracleConnection)connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            new Prestupek().Pridej(conn,ulice,cisloPopisne,obec,psc,typPrestupku,popisZasahu,jmenoObcana);
        }

        public async Task UpravitPrestupekAsync(Prestupek prestupek)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            prestupek.Uloz((OracleConnection)conn);
        }
    }
}
