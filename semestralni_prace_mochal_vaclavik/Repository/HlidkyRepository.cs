using Oracle.ManagedDataAccess.Client;
using semestralni_prace_mochal_vaclavik.OracleConn;
using semestralni_prace_mochal_vaclavik.Tridy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    class HlidkyRepository : IHlidkyRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public HlidkyRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public async Task<List<Hlidka>> GetHlidkyAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM hlidkyview";

            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<Hlidka>();
            while (await reader.ReadAsync())
            {
                result.Add(new Hlidka
                {
                    IdHlidky = reader.GetInt32("idhlidky"),
                    NazevHlidky = reader.GetString("nazevhlidky"),
                    Nazev = reader.GetString("nazev")
                });
            }
            return result;
        }

        public List<string> GetTypyHlidky()
        {
            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM typy_hlidkyview";

            using var reader = cmd.ExecuteReader();

            var result = new List<string>();
            while (reader.Read())
            {
                result.Add(reader.GetString("nazev"));
            }
            return result;
        }

        public async Task OdebratHlidku(Hlidka hlidka)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            hlidka.Smaz((OracleConnection)conn);
        }

        public async Task UpravitHlidku(Hlidka hlidka)
        {
            using var conn = connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            hlidka.Uloz((OracleConnection)conn);
        }

        public async Task PridatHlidku(string nazev, string typ)
        {
            using var conn = (OracleConnection)connectionFactory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            new Hlidka().Pridej(conn, nazev, typ);
        }
    }
}
