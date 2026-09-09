using LE.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LE.Common.Provider
{
    public class DbConnectionProvider : IConnectionProvider
    {
        private readonly IConfiguration _configuration;

        public DbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection GetDbConnection()
        {
            var defaultConnectionString = _configuration.GetDefaultConnectionString();
            return new NpgsqlConnection(defaultConnectionString);
        }
    }
}