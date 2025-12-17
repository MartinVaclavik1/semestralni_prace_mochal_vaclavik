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
    public class PolicisteRepository : IPolicisteRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public PolicisteRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<Policista>> GetPolicisteAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM kontaktyView";

            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<Policista>();
            while (await reader.ReadAsync())
            {
                result.Add(new Policista
                {
                    Id = reader.GetInt32("idpolicisty"),
                    Jmeno = reader.GetString("jmeno"),
                    Prijmeni = reader.GetString("prijmeni"),
                    Hodnost = reader.GetString("hodnost"),
                    Nadrizeny = reader.IsDBNull("nadrizeny")? "": reader.GetString("nadrizeny"),
                    Stanice = reader.GetString("stanice")
                });
            }

            return result;
        }

        public List<string> GetHodnosti()
        {
            //using var conn = connectionFactory.CreateConnection();

            //string sql = @"select * from opravneniView";

            //using (OracleCommand cmd = new OracleCommand(sql, conn))
            //{
            //    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            //    DataTable dt = new DataTable();
            //    adapter.Fill(dt);

            //    OpravneniSeznam.Clear();
            //    foreach (DataRow item in dt.Rows)
            //    {
            //        OpravneniSeznam.Add(item.Field<string>("nazevopravneni"));
            //    }
            //}

            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from hodnostiView";

            using var reader = cmd.ExecuteReader();

            var result = new List<string>(); 
            while (reader.Read())
            {
                result.Add(
                    reader.GetString("nazev")
                );
            }

            return result;
        }
    }
}
