using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.OracleConn
{
    public class OracleConnectionFactory : IOracleConnectionFactory
    {
        private readonly string connectionString;
        public OracleConnectionFactory(string connectionString) { 
            this.connectionString = connectionString;
        }

        public OracleConnection CreateConnection()  //connection pooling
        => new OracleConnection(connectionString);
    }
}
