using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using DataAccess.Services.Contract;

namespace DataAccess.Data
{
    public class DataContext : IDataContext
    {
        private readonly IConfiguration _config;
        public string ConnectionString { get; }

        public DataContext(IConfiguration config)
        {
            _config = config;
            ConnectionString = _config.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
    }
}
