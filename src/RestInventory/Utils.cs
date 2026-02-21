using Terraria;
using TShockAPI;

namespace RestInventory;

public class InventoryData
{
    public int netID { get; set; }
    public int prefix { get; set; }
    public int stack { get; set; }
    public InventoryData(int netID, int prefix, int stack)
    {
        this.netID = netID;
        this.prefix = prefix;
        this.stack = stack;
    }
}

public class Suits
{
    //饰品时装
    public InventoryData[] armor { get; set; } = Array.Empty<InventoryData>();
    //染料
    public InventoryData[] dye { get; set; } = Array.Empty<InventoryData>();
}

public class RetObject
{
    //在线状态
    public bool OnlineStatu { get; set; }
    //玩家名字
    public string Username { get; set; } = "";
    //最大生命
    public int statLifeMax { get; set; }
    //当前生命
    public int statLife { get; set; }
    //最大法力 
    public int statManaMax { get; set; }
    //当前法力
    public int statMana { get; set; }
    //buff
    public int[] buffType { get; set; } = Array.Empty<int>();
    //buff时间
    public int[] buffTime { get; set; } = Array.Empty<int>();
    //背包
    public InventoryData[] inventory { get; set; } = Array.Empty<InventoryData>();
    //宠物坐骑等
    public InventoryData[] miscEquip { get; set; } = Array.Empty<InventoryData>();
    //宠物坐骑染料
    public InventoryData[] miscDye { get; set; } = Array.Empty<InventoryData>();
    //套装
    public Suits[] Loadout { get; set; } = Array.Empty<Suits>();
    //垃圾桶
    public InventoryData[] trashItem { get; set; } = Array.Empty<InventoryData>();
    //猪猪存钱罐
    public InventoryData[] Piggiy { get; set; } = Array.Empty<InventoryData>();
    //保险箱
    public InventoryData[] safe { get; set; } = Array.Empty<InventoryData>();
    //护卫熔炉
    public InventoryData[] Forge { get; set; } = Array.Empty<InventoryData>();
    //虚空保险箱
    public InventoryData[] VoidVault { get; set; } = Array.Empty<InventoryData>();
}

public static class Utils
{
    public static InventoryData[] GetInventoryData(Item[] items, int slots)
    {
        var info = new InventoryData[slots];
        for (var i = 0; i < slots; i++)
        {
            info[i] = new InventoryData(items[i].type, items[i].prefix, items[i].stack);
        }
        return info;
    }
    //public static InventoryData[] GetInventoryData(NetItem[] items, int slots, int start = 0)
    //{
    //    InventoryData[] info = new InventoryData[slots];
    //    for (int i = start; i < slots; i++)
    //    {
    //        var item = items[i];
    //        info[i] = new InventoryData(item.NetId, item.PrefixId, item.Stack);
    //    }
    //    return info;
    //}

    // TODO: 可能需要更改
    public static Player ModifyData(string name, PlayerData data)
    {
        var player = new Player();
        if (data != null)
        {
            player.name = name;
            player.SpawnX = data.spawnX;
            player.SpawnY = data.spawnY;

            player.hideVisibleAccessory = data.hideVisuals;
            player.skinVariant = data.skinVariant ?? default;
            player.statLife = data.health;
            player.statLifeMax = data.maxHealth;
            player.statMana = data.mana;
            player.statManaMax = data.maxMana;
            player.extraAccessory = data.extraSlot == 1;

            player.difficulty = (byte) Main.GameMode;

            // 火把神
            player.unlockedBiomeTorches = data.unlockedBiomeTorches == 1;

            player.hairColor = data.hairColor ?? default;
            player.skinColor = data.skinColor ?? default;
            player.eyeColor = data.eyeColor ?? default;
            player.shirtColor = data.shirtColor ?? default;
            player.underShirtColor = data.underShirtColor ?? default;
            player.pantsColor = data.pantsColor ?? default;
            player.shoeColor = data.shoeColor ?? default;

            player.hair = data.hair ?? default;
            player.hairDye = data.hairDye;

            player.anglerQuestsFinished = data.questsCompleted;
            player.CurrentLoadoutIndex = data.currentLoadoutIndex;

            //player.numberOfDeathsPVE = data.numberOfDeathsPVE;
            //player.numberOfDeathsPVP = data.numberOfDeathsPVP;

            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                //  0~49 背包   5*10
                //  50、51、52、53 钱
                //  54、55、56、57 弹药
                // 59 ~68  饰品栏
                // 69 ~78  社交栏
                // 79 ~88  染料1
                // 89 ~93  宠物、照明、矿车、坐骑、钩爪
                // 94 ~98  染料2
                // 99~138 储蓄罐
                // 139~178 保险箱（商人）
                // 179 垃圾桶
                // 180~219 护卫熔炉
                // 220~259 虚空保险箱
                // 260~350 装备123
                if (i < 59)
                {
                    player.inventory[i] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 59 && i < 79)
                {
                    player.armor[i - 59] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 79 && i < 89)
                {
                    player.dye[i - 79] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 89 && i < 94)
                {
                    player.miscEquips[i - 89] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 94 && i < 99)
                {
                    player.miscDyes[i - 94] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 99 && i < 139)
                {
                    player.bank.item[i - 99] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 139 && i < 179)
                {
                    player.bank2.item[i - 139] = NetItem2Item(data.inventory[i]);
                }
                else if (i == 179)
                {
                    player.trashItem = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 180 && i < 220)
                {
                    player.bank3.item[i - 180] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 220 && i < 260)
                {
                    player.bank4.item[i - 220] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 260 && i < 280)
                {
                    player.Loadouts[0].Armor[i - 260] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 280 && i < 290)
                {
                    player.Loadouts[0].Dye[i - 280] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 290 && i < 310)
                {
                    player.Loadouts[1].Armor[i - 290] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 310 && i < 320)
                {
                    player.Loadouts[1].Dye[i - 310] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 320 && i < 340)
                {
                    player.Loadouts[2].Armor[i - 320] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 340 && i < 350)
                {
                    player.Loadouts[2].Dye[i - 340] = NetItem2Item(data.inventory[i]);
                }
            }
        }
        return player;
    }

    public static Item NetItem2Item(NetItem netItem)
    {
        var item = new Item
        {
            type = netItem.NetId,
            stack = netItem.Stack,
            prefix = netItem.PrefixId
        };
        return item;
    }

    //      public static InventoryData[] GetOfflinePlayerInv(IDbConnection db,string plrName) 
    //      {
    //          var id = TShock.UserAccounts.GetUserAccountID(plrName);
    //          using (var reader = db.QueryReader("SELECT * FROM tsCharacter WHERE Account=@0", id))
    //          {
    //              if (reader.Read())
    //              {

    //                  List<NetItem> inventory = reader.Get<string>("Inventory").Split('~').Select(NetItem.Parse).ToList();
    //                  if (inventory.Count < NetItem.MaxInventory)
    //                  {
    //                      //TODO: unhardcode this - stop using magic numbers and use NetItem numbers
    //                      //Set new armour slots empty
    //                      inventory.InsertRange(67, new NetItem[2]);
    //                      //Set new vanity slots empty
    //                      inventory.InsertRange(77, new NetItem[2]);
    //                      //Set new dye slots empty
    //                      inventory.InsertRange(87, new NetItem[2]);
    //                      //Set the rest of the new slots empty
    //                      inventory.AddRange(new NetItem[NetItem.MaxInventory - inventory.Count]);
    //                  }
    //                  return GetInventoryData(inventory.ToArray(), inventory.Count);
    //              }
    //              else {
    //                  return new InventoryData[0];
    //              }
    //	}


    //}
}