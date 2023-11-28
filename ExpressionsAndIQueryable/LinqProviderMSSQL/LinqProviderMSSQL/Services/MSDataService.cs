using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

namespace LinqProviderMSSQL.Services;

public sealed class MSDataService : IDisposable
{
    private readonly string _connectionString;
    private readonly SqlConnection _connection;

    public MSDataService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        _connectionString = connectionString;
        _connection = new SqlConnection(_connectionString);
    }

    public async Task<IEnumerable<TResult>> ReadSQL<TResult>(string whereQuery) 
        where TResult : class, new()
    {
        if (_connection.State is not ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }

        var type = typeof(TResult);
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(x => x.Name)
            .ToArray();

        var query = GenerateSelectBody(properties, type, whereQuery);

        var command = new SqlCommand(query, _connection);
        var reader = await command.ExecuteReaderAsync();

        if (reader.HasRows)
        {
            var result = new List<TResult>();

            for (;await reader.ReadAsync();)
            {
                var current = new TResult();
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i].SetValue(current, reader.GetValue(i));
                }

                result.Add(current);
            }

            return result;
        }

        return Array.Empty<TResult>();
    }

    public async Task<IEnumerable> ReadSQL(Type? type, string whereQuery)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (_connection.State is not ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(x => x.Name)
            .ToArray();

        var query = GenerateSelectBody(properties, type, whereQuery);

        var command = new SqlCommand(query, _connection);
        var reader = await command.ExecuteReaderAsync();

        var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

        if (reader.HasRows)
        {
            for (; await reader.ReadAsync();)
            {
                object? current = Activator.CreateInstance(type);
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i].SetValue(current, reader.GetValue(i));
                }

                result?.Add(current!);
            }
        }

        return result!;
    }

    public async void Dispose()
    {
        await _connection.DisposeAsync();
    }

    private static string GenerateSelectBody(PropertyInfo[] properties, Type type, string whereQuery)
    {
        StringBuilder endQueryBuilder = new("SELECT ");
        for (int i = 0; i < properties.Length - 1; i++)
        {
            endQueryBuilder.Append('[').Append(properties[i].Name).Append("], ");
        }

        endQueryBuilder
            .Append('[').Append(properties[^1].Name).Append(']')
            .Append(" FROM ")
            .Append('[')
            .Append(type.Name)
            .Append(']')
            .Append(' ')
            .Append(whereQuery);

        return endQueryBuilder.ToString();
    }
}
