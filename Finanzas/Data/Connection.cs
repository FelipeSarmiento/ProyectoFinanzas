using System;
using System.Configuration;
using System.Data.SqlClient;
namespace Finanzas.Data
{
    public class Connection
    {
        private string serverThinkpad = "AR-LSARMIENTO\\FELIPE";
        private string serverPropio = "FELIPEPC\\FELIPE";
        private string cadenaSQL = String.Empty;

        public Connection()
        {
            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(
                    "Data Source = " + serverPropio +"; Initial Catalog = Piggy; Integrated Security = True; MultipleActiveResultSets=true"
                    );

            cadenaSQL = builder.ConnectionString;
            
        }

        public string getCadenaSQL()
        {
            return cadenaSQL;
        }
        
    }
}