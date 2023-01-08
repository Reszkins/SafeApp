using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace SafeApp.DataAccess
{
    public interface ISqlDataAccess
    {
        Task<List<T>> LoadData<T>(string sql, Dictionary<string, object> parameters);
        Task SaveData(string sql, Dictionary<string, object> parameters);
    }
    public class SqlDataAccess : ISqlDataAccess
    {
        public async Task<List<T>> LoadData<T>(string sql, Dictionary<string,object> parameters)
        {
            string connectionString = "Host=postgresql_database;Username=admin;Password=admin;Database=SafeAppDatabase";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                var data = await connection.QueryAsync<T>(sql, new DynamicParameters(parameters));
                return data.ToList();
            }
        }

        public async Task SaveData(string sql, Dictionary<string, object> parameters)
        {
            string connectionString = "Host=postgresql_database;Username=admin;Password=admin;Database=SafeAppDatabase";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.ExecuteAsync(sql, new DynamicParameters(parameters));
            }
        }
    }
}
