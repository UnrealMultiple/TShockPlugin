using LinqToDB;
using LinqToDB.Mapping;
using TShockAPI;

namespace LazyAPI.Database;

public abstract class PlayerRecordBase<T> : RecordBase<T> where T : PlayerRecordBase<T>
{
    public new class Context : RecordBase<T>.Context
    {
        public IQueryable<T> Get(string name)
        {
            if (!this.Records.Any(t => t.Name == name))
            {
                var t = Activator.CreateInstance<T>();
                t.Name = name;
                this.Insert(t);
            }

            return this.Records.Where(t => t.Name == name);
        }
        public IQueryable<T> Get(TSPlayer player)
        {
            return this.Get(player.Account.Name);
        }

        public Context(string? tableName) : base(tableName)
        {
        }
    }

    internal static new Context GetContext(string? tableName)
    {
        return new Context(tableName);
    }

    [PrimaryKey, NotNull]
    [Column("name")]
    public string Name { get; set; } = "";
}