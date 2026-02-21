using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Terraria;
using TShockAPI;

namespace PerPlayerLoot;

public class JItem
{
    public int id { get; set; }
    public int stack { get; set; }
    public byte prefix { get; set; }
}

public class FakeChestDatabase
{
    // 映射：{ 玩家 UUID: { 宝箱 ID: 宝箱对象 } }
    public static Dictionary<string, Dictionary<int, Chest>> fakeChestsMap = new Dictionary<string, Dictionary<int, Chest>> { };

    // 玩家放置的宝箱位置集合 (x, y)
    public static HashSet<(int, int)> playerPlacedChests = [];

    private static readonly string connString = "Data Source=tshock/perplayerloot.sqlite";

    public void Initialize()
    {
        this.CreateTables();
        this.LoadFakeChests();
    }

    public void CreateTables()
    {
        TSPlayer.Server.SendInfoMessage(GetString("设置每个玩家的宝箱数据库..."));
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();

            var sql = @"
                    CREATE TABLE IF NOT EXISTS chests (
                        id INTEGER NOT NULL,
                        playerUuid TEXT NOT NULL,
                        x INTEGER NOT NULL,
                        y INTEGER NOT NULL,
                        items BLOB NOT NULL,
                        PRIMARY KEY (id, playerUuid)
                    );

                    CREATE TABLE IF NOT EXISTS placed (
                        x INTEGER NOT NULL,
                        y INTEGER NOT NULL,
                        PRIMARY KEY (x, y)
                    );
                ";

            using (var cmd = new SqliteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void LoadFakeChests()
    {
        TSPlayer.Server.SendInfoMessage(GetString("加载每个玩家的战利品宝箱库存..."));
        var count = 0;

        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();

            using (var cmd = new SqliteCommand("SELECT id, playerUuid, x, y, items FROM chests;", conn))
            {
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var playerUuid = Convert.ToString(reader["playerUuid"])!;
                    var chestId = Convert.ToInt32(reader["id"]);

                    // 获取物品列表
                    var items = new List<Item?>();

                    var itemsRaw = new MemoryStream((byte[]) reader["items"]);

                    using (var br = new BsonReader(itemsRaw))
                    {
                        br.ReadRootValueAsArray = true;

                        var jItems = new JsonSerializer().Deserialize<IList<JItem>>(br)!;

                        // 将每个 JItem 转换为真实的 Item
                        foreach (var jItem in jItems)
                        {
                            if (jItem == null)
                            {
                                items.Add(null);
                                continue;
                            }

                            var item = new Item();
                            item.netDefaults(jItem.id);
                            item.stack = jItem.stack;
                            item.prefix = jItem.prefix;

                            items.Add(item);
                        }
                    }

                    // 构建一个宝箱
                    var chest = new Chest(x: Convert.ToInt32(reader["x"]),
                        y: Convert.ToInt32(reader["y"]))
                    {
                        item = items.ToArray(),
                    };
                    
                    // 保存到假宝箱映射中
                    var playerChests = fakeChestsMap!.GetValueOrDefault(playerUuid, new Dictionary<int, Chest>());
                    fakeChestsMap[playerUuid] = playerChests;

                    fakeChestsMap[playerUuid][chestId] = chest;

                    count++;
                }
            }

            // 加载被排除的物块位置
            using (var cmd = new SqliteCommand("SELECT x, y FROM placed;", conn))
            {
                var reader = cmd.ExecuteReader();

                playerPlacedChests.Clear();

                while (reader.Read())
                {
                    var x = Convert.ToInt32(reader["x"]);
                    var y = Convert.ToInt32(reader["y"]);

                    playerPlacedChests.Add((x, y));
                }
            }
        }
    }

    // 保存假宝箱到数据库
    public void SaveFakeChests(string? PlayerUuid = null, int? ChestId = null)
    {
        var count = 0;

        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();

            foreach (var playerEntry in fakeChestsMap)
            {
                var playerUuid = playerEntry.Key;
                // 如果指定了玩家UUID，并且当前玩家不符合，就跳过
                if (PlayerUuid != null && playerUuid != PlayerUuid)
                {
                    continue;
                }

                var playerChests = playerEntry.Value;

                foreach (var chestEntry in playerChests)
                {
                    var chestId = chestEntry.Key;
                    // 如果指定了宝箱ID，并且当前宝箱不符合，就跳过
                    if (ChestId != null && chestId != ChestId)
                    {
                        continue;
                    }

                    var chest = chestEntry.Value;
                    var jItems = new List<JItem>(chest.item.Length);

                    foreach (var item in chest.item)
                    {
                        var jItem = new JItem
                        {
                            id = item.type,
                            stack = item.stack,
                            prefix = item.prefix
                        };
                        jItems.Add(jItem);
                    }

                    var itemsMs = new MemoryStream();
                    using (var writer = new BsonWriter(itemsMs))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(writer, jItems);
                    }

                    // 插入或替换宝箱数据到数据库
                    var sql = @"REPLACE INTO chests (id, playerUuid, x, y, items) VALUES (@id, @playerUuid, @x, @y, @items);";

                    using (var cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", chestId);
                        cmd.Parameters.AddWithValue("@playerUuid", playerUuid);
                        cmd.Parameters.AddWithValue("@x", chest.x);
                        cmd.Parameters.AddWithValue("@y", chest.y);
                        cmd.Parameters.AddWithValue("@items", itemsMs.ToArray());

                        cmd.ExecuteNonQuery();
                    }

                    count++;
                }
            }

            foreach ((var x, var y) in playerPlacedChests)//没看懂啊这里怎么还有个循环
            {
                var sql = @"REPLACE INTO placed (x, y) VALUES (@x, @y);";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@x", x);
                    cmd.Parameters.AddWithValue("@y", y);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        TSPlayer.Server.SendSuccessMessage(GetString($"保存了 {count} 个战利品宝箱库存, {playerPlacedChests.Count} 玩家放置的宝箱."));
    }

    // 获取或创建指定玩家的假宝箱
    public Chest GetOrCreateFakeChest(int chestId, string playerUuid)
    {
        var playerChests = fakeChestsMap.GetValueOrDefault(playerUuid, new Dictionary<int, Chest>());
        fakeChestsMap[playerUuid] = playerChests;

        if (!playerChests.ContainsKey(chestId))
        {
            var realChest = Main.chest[chestId];

            // 从真实宝箱复制数据
            var fakeChest = realChest.CloneWithSeparateItems(); 
            // 保存到假宝箱列表中
            fakeChestsMap[playerUuid][chestId] = fakeChest;

            // 保存当前玩家的该宝箱
            this.SaveFakeChests(playerUuid, chestId);

            return fakeChest;
        }

        return playerChests[chestId];
    }


    // 标记某个位置为玩家放置的宝箱
    public void SetChestPlayerPlaced(int tileX, int tileY)
    {
        playerPlacedChests.Add((tileX, tileY));
    }

    // 检查某个位置是否被标记为玩家放置的宝箱
    public bool IsChestPlayerPlaced(int tileX, int tileY)
    {
        return playerPlacedChests.Contains((tileX, tileY));
    }
}