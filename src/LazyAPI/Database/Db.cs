using TShockAPI;

namespace LazyAPI.Database;

public static class Db
{
    public static PlayerRecordBase<T>.Context PlayerContext<T>(string? tableName = null) where T : PlayerRecordBase<T>
    {
        return PlayerRecordBase<T>.GetContext(tableName);
    }

    public static RecordBase<T>.Context Context<T>(string? tableName = null) where T : RecordBase<T>
    {
        return RecordBase<T>.GetContext(tableName);
    }

    public static DisposableQuery<T> Get<T>(string name, string? tableName = null) where T : PlayerRecordBase<T>
    {
        var context = PlayerRecordBase<T>.GetContext(tableName);
        return new DisposableQuery<T>(context.Get(name), context);
    }

    public static DisposableQuery<T> Get<T>(this TSPlayer player, string? tableName = null) where T : PlayerRecordBase<T>
    {
        return Get<T>(player.Account.Name, tableName);
    }
}