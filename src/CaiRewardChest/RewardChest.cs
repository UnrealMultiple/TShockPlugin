using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using Microsoft.Xna.Framework;
using Terraria;

namespace CaiRewardChest;

[Table("CaiRewardChest")]
public class RewardChest : RecordBase<RewardChest>
{
    [Column][PrimaryKey] public int ChestId;

    [NotColumn] public List<int> HasOpenPlayer = new();

    [Column]
    // ReSharper disable once InconsistentNaming
    public string _hasOpenPlayer
    {
        get => string.Join(',', this.HasOpenPlayer.Select(x => x.ToString()));
        set => this.HasOpenPlayer = value == string.Empty ? new List<int>() : value.Split(',').Select(int.Parse).ToList();
    }

    [Column] public int X { get; set; }

    [Column] public int Y { get; set; }

    [NotColumn] public Chest Chest =>  Main.chest[Chest.FindChest(this.X, this.Y)];

    private static Context context => Db.Context<RewardChest>("CaiRewardChest");

    public static List<Point> GetAllChestId()
    {
        return context.Records.Select(c => new Point(c.X,c.Y) ).ToList();
    }


    public static RewardChest? GetChestByPos(int x, int y)
    {
        return context.Records.FirstOrDefault(i => i.X == x && i.Y == y);
    }


    public static void UpdateChest(RewardChest chest)
    {
        context.Update(chest);
    }

    public static void AddChest(int chestId, int x, int y)
    {
        CaiRewardChest.RewardChestPos.Add(new  Point(x, y));
        var chest = new RewardChest { ChestId = chestId, X = x, Y = y };
        context.Insert(chest);
    }

    public static void DelChest(int x,int y)
    {
        CaiRewardChest.RewardChestPos.RemoveAll(c => c.X == x && c.Y == y );
        context.Records.Delete(c => c.X== x && c.Y == y );
    }

    public static void ClearDb()
    {
        CaiRewardChest.RewardChestPos.Clear();
        context.Records.Drop();
    }
}