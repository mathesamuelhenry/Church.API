using Church.API.Data.DBContext;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Church.API.HealthCheck
{
    public class MySQLServerHealthCheck : IHealthCheck
    {
        string _connectionString;

        public string Name => "sql";

        public MySQLServerHealthCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            MySqlConnection myConnection = new MySqlConnection(_connectionString);

            try
            {
                await myConnection.OpenAsync();
            }
            catch(MySqlException)
            {
                return HealthCheckResult.Unhealthy();
            }

            return HealthCheckResult.Healthy();
        }
    }
}
