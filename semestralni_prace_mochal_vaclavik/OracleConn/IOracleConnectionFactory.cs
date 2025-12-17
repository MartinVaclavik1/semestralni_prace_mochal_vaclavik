using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralni_prace_mochal_vaclavik.OracleConn
{
    public interface IOracleConnectionFactory
    {
        OracleConnection CreateConnection();
    }
}
