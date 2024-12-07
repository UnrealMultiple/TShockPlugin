using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using NuGet.Protocol.Plugins;
using System.Data;
using Terraria;

namespace CaiRewardChest;

[Table("CaiRewardChest")]
public class RewardChest : RecordBase<RewardChest>
{
    [Column]
    [PrimaryKey]
    public int ChestId;

    [NotColumn] public List<int> HasOpenPlayer = new ();

    [Column]
    // ReSharper disable once InconsistentNaming
    public string _hasOpenPlayer 
    {
        get => string.Join(',', this.HasOpenPlayer.Select(x => x.ToString()));
        set => this.HasOpenPlayer = value==string.Empty?new List<int>():value.Split(',').Select(int.Parse).ToList();
    }

    [Column]
    public int X { get; set; }

    [Column]
    public int Y { get; set; }

    [NotColumn]
    public Chest Chest => Main.chest[this.ChestId];

    private static Context context => Db.Context<RewardChest>("CaiRewardChest");

    public static List<int> GetAllChestId()
    {
        return context.Records.Select(x=>x.ChestId).ToList();
    }
    
    
    public static RewardChest? GetChestByPos(int x, int y)
    {
        return context.Records.FirstOrDefault(i => i.X == x && i.Y == y);
    }

    public static RewardChest? GetChestById(int chestId)
    {
        return context.Records.FirstOrDefault(i => i.ChestId == chestId);
    }
    
    
    public static void UpdateChest(RewardChest chest)
    {
        context.Update(chest);
    }
    public static void AddChest(int chestId, int x, int y)
    {
        CaiRewardChest.RewardChestId.Add(chestId);
        var chest = new RewardChest { ChestId = chestId, X = x, Y = y };
        context.Insert(chest);
    }
    public static void DelChest(int chestId)
    {
        CaiRewardChest.RewardChestId.Remove(chestId);
        context.Records.Delete(x => x.ChestId == chestId);
    }

    public static void ClearDb()
    {
        CaiRewardChest.RewardChestId.Clear();
        context.Records.Drop();
    }
}