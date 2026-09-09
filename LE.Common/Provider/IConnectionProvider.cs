using Npgsql;

namespace LE.Common.Provider
{
    public interface IConnectionProvider
    {
        NpgsqlConnection GetDbConnection();
    }
}