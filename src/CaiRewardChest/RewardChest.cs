using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using System.Data;
using Terraria;

namespace CaiRewardChest;

[Table("CaiRewardChest")]
public class RewardChest : RecordBase<RewardChest>
{
    [Column]
    [PrimaryKey]
    public int ChestId;

    [NotColumn]
    public List<int> _HasOpenPlayer = new();

    [Column]
    public int X { get; set; }

    [Column]
    public int Y { get; set; }

    [Column]
    public string HasOpenPlayer
    {
        get => string.Join(",", this._HasOpenPlayer);

        set => this._HasOpenPlayer = value?.Split(',').Select(int.Parse).ToList() ?? new();
    }

    [NotColumn]
    public Chest Chest => Main.chest[this.ChestId];

    internal static Context context => Db.Context<RewardChest>("CaiRewardChest");

    public static RewardChest? GetChestByPos(int x, int y)
    {
        return context.Records.FirstOrDefault(i => i.X == x && i.Y == y);
    }

    public static RewardChest? GetChestById(int chestId)
    {
        return context.Records.FirstOrDefault(i => i.ChestId == chestId);
    }


    public static void AddChest(int chestId, int x, int y)
    {
        context.Insert<RewardChest>(new()
        {
            ChestId = chestId,
            X = x,
            Y = y
        });
    }

    public static void UpdateChest(RewardChest chest)
    {
        context.Update(chest);
    }

    public static void DelChest(int chestId)
    {
        context.Records.Delete(x => x.ChestId == chestId);
    }

    public static void ClearDb()
    {
        context.Records.Delete();
    }
}