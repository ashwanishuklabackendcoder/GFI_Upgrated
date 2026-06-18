using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace GFI_Upgrated.Data.Common
{
    public static class ResilientSqlExecutor
    {
        private static readonly ResiliencePipeline SqlRetryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<SqlException>(ex => IsTransient(ex)),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2)
            })
            .Build();

        private static bool IsTransient(SqlException ex)
        {
            switch (ex.Number)
            {
                case -2:
                case 10053:
                case 10054:
                case 10060:
                case 40197:
                case 40501:
                case 40613:
                case 49918:
                case 49919:
                case 49920:
                    return true;
            }
            return false;
        }

        private static SqlParameter CloneParameter(SqlParameter p)
        {
            return new SqlParameter(p.ParameterName, p.SqlDbType)
            {
                Value = p.Value,
                Direction = p.Direction,
                Size = p.Size,
                Precision = p.Precision,
                Scale = p.Scale,
                SourceColumn = p.SourceColumn,
                SourceVersion = p.SourceVersion,
                IsNullable = p.IsNullable
            };
        }

        private static void MapOutputParametersBack(IEnumerable<SqlParameter> sourceParameters, SqlCommand command)
        {
            if (sourceParameters == null) return;
            foreach (var origParam in sourceParameters)
            {
                if (origParam.Direction == ParameterDirection.Output || 
                    origParam.Direction == ParameterDirection.InputOutput || 
                    origParam.Direction == ParameterDirection.ReturnValue)
                {
                    var commandParam = command.Parameters[origParam.ParameterName];
                    if (commandParam != null)
                    {
                        origParam.Value = commandParam.Value;
                    }
                }
            }
        }

        public static async Task<DataTable> ExecuteDataTableAsync(string connectionString, string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            return await SqlRetryPipeline.ExecuteAsync(async (ct) =>
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CloneParameter(parameter));
                }

                await connection.OpenAsync(ct);
                await using var reader = await command.ExecuteReaderAsync(ct);
                var table = new DataTable();
                table.Load(reader);
                MapOutputParametersBack(parameters, command);
                return table;
            }, cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync(string connectionString, string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            return await SqlRetryPipeline.ExecuteAsync(async (ct) =>
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CloneParameter(parameter));
                }

                await connection.OpenAsync(ct);
                var result = await command.ExecuteNonQueryAsync(ct);
                MapOutputParametersBack(parameters, command);
                return result;
            }, cancellationToken);
        }

        public static async Task<DataTable> ExecuteDataTableRawAsync(string connectionString, string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            return await SqlRetryPipeline.ExecuteAsync(async (ct) =>
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text
                };

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CloneParameter(parameter));
                }

                await connection.OpenAsync(ct);
                await using var reader = await command.ExecuteReaderAsync(ct);
                var table = new DataTable();
                table.Load(reader);
                MapOutputParametersBack(parameters, command);
                return table;
            }, cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryRawAsync(string connectionString, string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            return await SqlRetryPipeline.ExecuteAsync(async (ct) =>
            {
                await using var connection = new SqlConnection(connectionString);
                await using var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text
                };

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CloneParameter(parameter));
                }

                await connection.OpenAsync(ct);
                var result = await command.ExecuteNonQueryAsync(ct);
                MapOutputParametersBack(parameters, command);
                return result;
            }, cancellationToken);
        }
    }
}
