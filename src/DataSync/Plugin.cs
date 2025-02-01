using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace DataSync;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (2025, 1, 29);
    internal static Dictionary<int, List<ProgressType>> _idmatch = new Dictionary<int, List<ProgressType>>();
    internal static Dictionary<ProgressType, Func<bool?, bool>> _flagaccessors = new Dictionary<ProgressType, Func<bool?, bool>>();
    public Plugin(Main game) : base(game)
    {
        foreach (var variant in typeof(ProgressType).GetFields().Where(f => f.FieldType == typeof(ProgressType)))
        {
            var value = (ProgressType) variant.GetValue(null)!;
            foreach (var match in variant.GetCustomAttributes<MatchAttribute>()!)
            {
                foreach (var id in match.NPCID)
                {
                    if (!_idmatch.ContainsKey(id))
                    {
                        _idmatch[id] = new List<ProgressType>();
                    }
                    _idmatch[id].Add(value);
                }
            }
            foreach (var mapping in variant.GetCustomAttributes<MappingAttribute>()!)
            {
                if (mapping.Key is not null)
                {
                    var targetField = typeof(NPC).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)
                        ?? typeof(Main).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)
                        ?? typeof(Terraria.GameContent.Events.DD2Event).GetField(mapping.Key!, BindingFlags.Public | BindingFlags.Static)!;

                    _flagaccessors.Add(value, (newvalue) =>
                    {
                        if (newvalue is not null && targetField.FieldType == typeof(bool))
                        {
                            targetField.SetValue(null, newvalue);
                        }
                        return mapping.Value.Equals(targetField.GetValue(null));
                    });
                }
            }
        }
    }

    public static event Action? OnProgressChanged;
    private void ClearProgress(CommandArgs args)
    {
        TShock.DB.Query("TRUNCATE TABLE synctable;");
    }

    public static Dictionary<ProgressType, bool> LocalProgress { get; set; } = Config.DefaultDict;
    public static Dictionary<ProgressType, bool> SyncedProgress { get; set; } = new Dictionary<ProgressType, bool>();

    private static void EnsureTable()
    {
        var db = TShock.DB;
        var sqlTable = new SqlTable("synctable", new SqlColumn[]
        {
            new SqlColumn("key", MySqlDbType.VarChar)
            {
                Primary = true,
                Length = 256
            },
            new SqlColumn("value", MySqlDbType.VarChar)
            {
                Length = 256
            }
        });

        var sqlTableCreator = new SqlTableCreator(db, (db.GetSqlType() == SqlType.Sqlite)
            ? new SqliteQueryCreator()
            : new MysqlQueryCreator());
        sqlTableCreator.EnsureTableStructure(sqlTable);

        using var result = TShock.DB.QueryReader("SELECT * FROM synctable WHERE `key`=@0", Config.GetProgressName(ProgressType.Unreachable));
        if (!result.Read())
        {
            foreach (var t in Config._default)
            {
                TShock.DB.Query("INSERT INTO synctable (`key`, `value`) VALUES (@0, @1)", Config.GetProgressName(t.Key), false);
            }
        }
    }

    private int _frameCount = 0;
    private GeneralHooks.ReloadEventD _reloadHandler = null!;
    public override void Initialize()
    {
        Config.LoadConfig();
        EnsureTable();
        this._reloadHandler = (_) => this.Reload();
        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.GamePostInitialize.Register(this, this.PostInitualize);
        Commands.ChatCommands.Add(new Command("DataSync", this.ClearProgress, "重置进度同步"));
        TShock.RestApi.Register(new SecureRestCommand("/DataSync", ProgressRest, "DataSync"));
        Commands.ChatCommands.Add(new Command("DataSync", ProgressCommand, "进度", "progress"));
    }

    private void OnUpdate(EventArgs args)
    {
        this._frameCount++;
        if (this._frameCount % 300 == 0)
        {
            LoadProgress();
        }
    }

    private void PostInitualize(EventArgs args)
    {
        LoadProgress();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NpcKilled);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.PostInitualize);
            ((List<RestCommand>) typeof(Rest).GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(TShock.RestApi)!)
            .RemoveAll(x => x.Name == "/DataSync");
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.ClearProgress || x.CommandDelegate == ProgressCommand);
        }

        base.Dispose(disposing);
    }

    private static object ProgressRest(RestRequestArgs args)
    {
        return new RestObject {
            { "local", JsonConvert.DeserializeObject<Dictionary<string, bool>>(JsonConvert.SerializeObject(LocalProgress)) },
            { "remote", JsonConvert.DeserializeObject<Dictionary<string, bool>>(JsonConvert.SerializeObject(SyncedProgress)) }
        };
    }

    private static void ProgressCommand(CommandArgs args)
    {
        switch (args.Parameters.Count)
        {
            case 2:
            {
                if (!args.Player.HasPermission("DataSync.set"))
                {
                    args.Player.SendErrorMessage(GetString("你没有权限设置进度"));
                    return;
                }
                var type = Config.GetProgressType(args.Parameters[0]);
                if (type == null)
                {
                    args.Player.SendErrorMessage(GetString($"进度 '{args.Parameters[0]}' 不存在"));
                    return;
                }
                if (!bool.TryParse(args.Parameters[1], out var result))
                {
                    args.Player.SendErrorMessage(GetString($"值 '{args.Parameters[1]}' 应为 true 或 false"));
                    return;
                }
                if (_flagaccessors.TryGetValue(type.Value, out var accessor))
                {
                    accessor(result);
                }
                UpdateProgress(type.Value, result, true);
                return;
            }
            case 1 when !string.Equals(args.Parameters[0], "local", StringComparison.OrdinalIgnoreCase):
            {
                if (string.Equals(args.Parameters[0], "remote", StringComparison.OrdinalIgnoreCase))
                {
                    args.Player.SendInfoMessage(GetString("远程进度:"));
                    var readable = GetReadableProgress(SyncedProgress);
                    if (readable.ContainsKey(true))
                    {
                        args.Player.SendInfoMessage(GetString("已完成:"));
                        args.Player.SendSuccessMessage(string.Join(", ", readable[true]));
                    }
                    if (readable.ContainsKey(false))
                    {
                        args.Player.SendInfoMessage(GetString("未完成:"));
                        args.Player.SendErrorMessage(string.Join(", ", readable[false]));
                    }
                    return;
                }
                var type = Config.GetProgressType(args.Parameters[0]);
                if (type == null)
                {
                    args.Player.SendErrorMessage(GetString($"进度 '{args.Parameters[0]}' 不存在"));
                    return;
                }
                args.Player.SendInfoMessage(GetString($"进度 '{args.Parameters[0]}' 的值为 '{LocalProgress[type.Value]}'"));
                return;
            }

            default:
            {
                args.Player.SendInfoMessage(GetString("本地进度:"));
                var readable = GetReadableProgress(LocalProgress);
                if (readable.ContainsKey(true))
                {
                    args.Player.SendInfoMessage(GetString("已完成:"));
                    args.Player.SendSuccessMessage(string.Join(", ", readable[true]));
                }
                if (readable.ContainsKey(false))
                {
                    args.Player.SendInfoMessage(GetString("未完成:"));
                    args.Player.SendErrorMessage(string.Join(", ", readable[false]));
                }
                return;
            }
        }
    }

    private static Dictionary<bool, List<string>> GetReadableProgress(Dictionary<ProgressType, bool> progress)
    {
        var result = new Dictionary<bool, List<string>>();
        foreach (var (key, value) in progress)
        {
            if (!result.TryGetValue(value, out var list))
            {
                list = new List<string>();
                result[value] = list;
            }
            list.Add(Config.GetProgressName(key));
        }
        return result;
    }

    public static void UpdateProgress(ProgressType type, bool value, bool force = false)
    {
        if (!force && LocalProgress.TryGetValue(type, out var oldValue) && oldValue == value)
        {
            return;
        }

        TSPlayer.Server.SendInfoMessage(GetString($"[DataSync]更新进度 {Config.GetProgressName(type)} {value}"));

        LocalProgress[type] = value;

        if (Config.ShouldSyncProgress.TryGetValue(type, out var progress) && progress)
        {
            if (!SyncedProgress.TryGetValue(type, out var syncedValue) || syncedValue != value)
            {
                TSPlayer.Server.SendInfoMessage(GetString($"[DataSync]上传进度 {Config.GetProgressName(type)} {value}"));
                TShock.DB.Query("UPDATE synctable SET value = @1 WHERE `key` = @0", Config.GetProgressName(type), value);
            }
        }

        OnProgressChanged?.Invoke();
    }

    private void NpcKilled(NpcKilledEventArgs args)
    {
        if (_idmatch.TryGetValue(args.npc.netID, out var types))
        {
            foreach (var type in types)
            {
                if (!_flagaccessors.TryGetValue(type, out var accessor) || accessor(null))
                {
                    UpdateProgress(type, true);
                }
            }
        }
    }

    private void Reload()
    {
        try
        {
            Config.LoadConfig();
            EnsureTable();
            LoadProgress();
            //args.Player.SendErrorMessage($"[QwRPG.Shop]重载成功！");
        }
        catch
        {
            TSPlayer.Server.SendErrorMessage(GetString($"[DataSync]配置文件读取错误"));
        }
    }

    public static void LoadProgress()
    {
        if (!Monitor.TryEnter(kg))
        {
            return;
        }

        foreach (var (pg, ac) in _flagaccessors)
        {
            if (ac(null) && (!LocalProgress.TryGetValue(pg, out var value) || !value))
            {
                UpdateProgress(pg, true);
            }
        }

        using (var reader = TShock.DB.QueryReader("SELECT * FROM synctable"))
        {
            while (reader.Read())
            {
                var key = reader.Get<string>("key");
                var value = reader.Get<bool>("value");
                if (Config.GetProgressType(key) is ProgressType type)
                {
                    SyncedProgress[type] = value;
                    if (Config.ShouldSyncProgress.TryGetValue(type, out var progress) && progress)
                    {
                        LocalProgress[type] = value;
                        if (_flagaccessors.TryGetValue(type, out var accessor))
                        {
                            accessor(value);
                        }
                    }
                }
            }
        }

        Monitor.Exit(kg);
    }

    public static object kg = new object();
}