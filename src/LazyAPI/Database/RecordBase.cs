using LinqToDB;
using LinqToDB.Data;
using TShockAPI;
using TShockAPI.DB;
using SqlType = TShockAPI.DB.SqlType;

namespace LazyAPI.Database;

public abstract class RecordBase<T> where T : RecordBase<T>
{
    public class Context : DataConnection
    {
        public ITable<T> Records => this.GetTable<T>();

        private static string GetProvider()
        {
            return TShock.DB.GetSqlType() switch
            {
                SqlType.Mysql => ProviderName.MySql,
                SqlType.Sqlite => ProviderName.SQLiteMS,
                SqlType.Postgres => ProviderName.PostgreSQL,
                _ => "",
            };
        }
        
        
        public Context(string? tableName) : base(new DataOptions().UseConnectionString(GetProvider(), RecordBase<T>.ConnectionString))
        {
            this.MappingSchema.AddScalarType(typeof(string), new LinqToDB.SqlQuery.SqlDataType(DataType.NVarChar, 255));
            this.CreateTable<T>(tableName, tableOptions: TableOptions.CreateIfNotExists);
        }
    }

    internal static Context GetContext(string? tableName)
    {
        return new(tableName);
    }

    // ReSharper disable once StaticMemberInGenericType
    protected static string ConnectionString = TShock.DB.ConnectionString.Replace(",Version=3", "");
}