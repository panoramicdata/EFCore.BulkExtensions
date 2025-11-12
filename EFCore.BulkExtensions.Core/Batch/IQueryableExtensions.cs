using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using EFCore.BulkExtensions.SqlAdapters;
using Microsoft.EntityFrameworkCore;

namespace EFCore.BulkExtensions;

/// <summary>
/// Contains a list of IQuerable extensions
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Extension method to paramatize sql query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="context">The database context.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static (string, IEnumerable<DbParameter>) ToParametrizedSql(this IQueryable query, DbContext context)
    {
        string relationalQueryContextText = "_relationalQueryContext";
        string relationalCommandCacheText = "_relationalCommandCache"; // used with EF 8
        string relationalCommandResolverText = "_relationalCommandResolver"; // used with EF 9

        string cannotGetText = "Cannot get";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var enumerator = query.Provider.Execute<IEnumerable>(query.Expression).GetEnumerator();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var queryContext = enumerator.Private<RelationalQueryContext>(relationalQueryContextText) ?? throw new InvalidOperationException($"{cannotGetText} {relationalQueryContextText}");
        
        // In EF Core 10, ParameterValues access may have changed - try to get it via reflection first
        IReadOnlyDictionary<string, object?>? parameterValues = null;
        
        // Try to get ParameterValues property (works for EF Core 8/9)
        var parameterValuesProperty = typeof(RelationalQueryContext).GetProperty("ParameterValues", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (parameterValuesProperty != null)
        {
            parameterValues = (IReadOnlyDictionary<string, object?>?)parameterValuesProperty.GetValue(queryContext);
        }
        
        // If not found, try the base QueryContext class
        if (parameterValues == null)
        {
            var baseProperty = typeof(QueryContext).GetProperty("ParameterValues", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseProperty != null)
            {
                parameterValues = (IReadOnlyDictionary<string, object?>?)baseProperty.GetValue(queryContext);
            }
        }
        
        if (parameterValues == null)
        {
            throw new InvalidOperationException($"{cannotGetText} ParameterValues from QueryContext");
        }

#pragma warning disable EF1001 // Internal EF Core API usage.
        var relationalCommandCache = (RelationalCommandCache?)enumerator.Private(relationalCommandCacheText);
        var relationalCommandResolver = enumerator.Private<Delegate>(relationalCommandResolverText);
#pragma warning restore EF1001

        IRelationalCommand? command = null;
        if (relationalCommandCache != null)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            // Convert IReadOnlyDictionary to Dictionary if needed for EF Core 10
            var paramDict = parameterValues as Dictionary<string, object?> ?? new Dictionary<string, object?>(parameterValues);
            command = (IRelationalCommand?)relationalCommandCache?.GetRelationalCommandTemplate(paramDict);
#pragma warning restore EF1001
        }
        if (command == null && relationalCommandResolver != null)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var paramDict = parameterValues as Dictionary<string, object?> ?? new Dictionary<string, object?>(parameterValues);
            command = (IRelationalCommand?)relationalCommandResolver.DynamicInvoke(paramDict);
#pragma warning restore EF1001
        }
        if (command == null)
        {
            string selectExpressionText = "_selectExpression";
            string querySqlGeneratorFactoryText = "_querySqlGeneratorFactory";
            SelectExpression selectExpression = enumerator.Private<SelectExpression>(selectExpressionText) ?? throw new InvalidOperationException($"{cannotGetText} {selectExpressionText}");
            IQuerySqlGeneratorFactory factory = enumerator.Private<IQuerySqlGeneratorFactory>(querySqlGeneratorFactoryText) ?? throw new InvalidOperationException($"{cannotGetText} {querySqlGeneratorFactoryText}");
            command = factory.Create().GetCommand(selectExpression);
        }
        string sql = command.CommandText;

        IList<DbParameter> parameters;
        try
        {
            using var dbCommand = SqlAdaptersMapping.DbServer(context).QueryBuilder.CreateCommand(); // Use a DbCommand to convert parameter values using ValueConverters to the correct type.
            foreach (var param in command.Parameters)
            {
                var values = parameterValues[param.InvariantName];
                param.AddDbParameter(dbCommand, values);
            }
            parameters = new List<DbParameter>(dbCommand.Parameters.OfType<DbParameter>());
            dbCommand.Parameters.Clear();
        }
        catch (Exception ex) // Fix for BatchDelete with 'uint' param on Sqlite. TEST: RunBatchUint
        {
            var npgsqlSpecParamMessage = "Npgsql-specific type mapping ";
            // Full Msg:
            // "Npgsql-specific type mapping Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping.NpgsqlArrayListTypeMapping being used with non-Npgsql parameter type SqlParameter"
            // "Npgsql-specific type mapping NpgsqlTimestampTzTypeMapping being used with non-Npgsql parameter type SqlParameter" // for Batch (a < date)

            if (ex.Message.StartsWith("No mapping exists from DbType") && ex.Message.EndsWith("to a known SqlDbType.") || // example: "No mapping exists from DbType UInt32 to a known SqlDbType."
                ex.Message.StartsWith(npgsqlSpecParamMessage)) // Fix for BatchDelete with Contains on PostgreSQL
            {
                var parameterNames = new HashSet<string>(command.Parameters.Select(p => p.InvariantName));
                parameters = parameterValues.Where(pv => parameterNames.Contains(pv.Key)).Select(pv => SqlAdaptersMapping.DbServer(context).QueryBuilder.CreateParameter("@" + pv.Key, pv.Value)).ToList();
            }
            else
            {
                throw;
            }
        }
        return (sql, parameters);
    }

    private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    private static object? Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, bindingFlags)?.GetValue(obj);

    private static T? Private<T>(this object obj, string privateField) => (T?)obj?.GetType().GetField(privateField, bindingFlags)?.GetValue(obj);
}
