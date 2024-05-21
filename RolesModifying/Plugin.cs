using Org.BouncyCastle.Asn1.Microsoft;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RolesModifying;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    class ModifyData
    {
        public TSPlayer Player { get; set; }

        //public PlayerData LastPlayerData { get; set; }

        public PlayerData SourcePlayerData { get; set; }

        public ModifyData(TSPlayer player, PlayerData sourcePlayerData)
        {
            Player = player;
            //LastPlayerData = LastData;
            SourcePlayerData = sourcePlayerData;
        }
    }

    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    private Dictionary<TSPlayer, ModifyData> TempData = new();
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        Commands.ChatCommands.Add(new Command("tshock.user.rolesmodifying", Roles, "rm", "修改", "查背包"));
    }

    private void Roles(CommandArgs args)
    {
        if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "save")
        {
            if (TempData.TryGetValue(args.Player, out var modify))
            {
                var target = TShock.Players.FirstOrDefault(ply => ply != null && ply.Active && ply.Name == modify.Player.TPlayer.name);
                if (target != null)
                {
                    args.Player.PlayerData.RestoreCharacter(target);
                    TShock.CharacterDB.InsertPlayerData(target);
                }
                else
                {
                    var old_id = args.Player.Account.ID;
                    args.Player.Account.ID = modify.Player.Account.ID;
                    TShock.CharacterDB.InsertPlayerData(args.Player);
                    args.Player.Account.ID = old_id;
                }
                //恢复
                args.Player.PlayerData = modify.SourcePlayerData;
                modify.SourcePlayerData.RestoreCharacter(args.Player);
                TShock.CharacterDB.InsertPlayerData(args.Player);
                TempData.Remove(args.Player);
                args.Player.SendSuccessMessage("修改保存成功！");
            }
            else
            {
                args.Player.SendErrorMessage("你没有对玩家的信息进行修改，无需保存！");
            }
        }
        else if (args.Parameters.Count > 0)
        {
            var target = TShock.Players.FirstOrDefault(ply => ply != null && ply.Active && ply.Name == args.Parameters[0]);
            //在线
            if (target != null)
            {
                var targetPlayerData = CopyCharacter(target);
                TempData[args.Player] = new(target, CopyCharacter(args.Player));
                //保存自己背包物品
                TShock.CharacterDB.InsertPlayerData(args.Player);
                //复制
                target.PlayerData.RestoreCharacter(args.Player);
                args.Player.SendSuccessMessage("正在查看{0}的人物信息，可以对背包进行修改，使用/rm save 进行保存。", args.Parameters[0]);
            }
            else
            {
                var account = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
                if (account != null)
                {
                    var TempPlayer = new TempPlayer(account.Name)
                    {
                        Account = account
                    };
                    TempPlayer.PlayerData = TShock.CharacterDB.GetPlayerData(TempPlayer, account.ID);
                    var targetPlayerData = CopyCharacter(TempPlayer);
                    TempData[args.Player] = new(TempPlayer, CopyCharacter(args.Player));
                    //保存自己背包物品
                    TShock.CharacterDB.InsertPlayerData(args.Player);
                    //复制
                    TempPlayer.PlayerData.RestoreCharacter(args.Player);
                    args.Player.SendSuccessMessage("正在查看{0}的人物信息，可以对背包进行修改，使用/rm save 进行保存。", args.Parameters[0]);
                }
                else
                {
                    args.Player.SendErrorMessage("目标玩家不存在!");
                }

            }
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && TempData.TryGetValue(player, out var modify))
        {
            modify.SourcePlayerData.RestoreCharacter(player);
            TempData.Remove(player);
        }
    }


    /// <summary>
    /// 复制PlayerData对象
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public PlayerData CopyCharacter(TSPlayer player)
    {
        var PlayerData = new PlayerData(player);
        PlayerData.health = player.TPlayer.statLife > 0 ? player.TPlayer.statLife : 1;
        PlayerData.maxHealth = player.TPlayer.statLifeMax;
        PlayerData.mana = player.TPlayer.statMana;
        PlayerData.maxMana = player.TPlayer.statManaMax;
        if (player.sX > 0 && player.sY > 0)
        {
            PlayerData.spawnX = player.sX;
            PlayerData.spawnY = player.sY;
        }
        else
        {
            PlayerData.spawnX = player.TPlayer.SpawnX;
            PlayerData.spawnY = player.TPlayer.SpawnY;
        }
        PlayerData.extraSlot = player.TPlayer.extraAccessory ? 1 : 0;
        PlayerData.skinVariant = player.TPlayer.skinVariant;
        PlayerData.hair = player.TPlayer.hair;
        PlayerData.hairDye = player.TPlayer.hairDye;
        PlayerData.hairColor = player.TPlayer.hairColor;
        PlayerData.pantsColor = player.TPlayer.pantsColor;
        PlayerData.shirtColor = player.TPlayer.shirtColor;
        PlayerData.underShirtColor = player.TPlayer.underShirtColor;
        PlayerData.shoeColor = player.TPlayer.shoeColor;
        PlayerData.hideVisuals = player.TPlayer.hideVisibleAccessory;
        PlayerData.skinColor = player.TPlayer.skinColor;
        PlayerData.eyeColor = player.TPlayer.eyeColor;
        PlayerData.questsCompleted = player.TPlayer.anglerQuestsFinished;
        PlayerData.usingBiomeTorches = player.TPlayer.UsingBiomeTorches ? 1 : 0;
        PlayerData.happyFunTorchTime = player.TPlayer.happyFunTorchTime ? 1 : 0;
        PlayerData.unlockedBiomeTorches = player.TPlayer.unlockedBiomeTorches ? 1 : 0;
        PlayerData.currentLoadoutIndex = player.TPlayer.CurrentLoadoutIndex;
        PlayerData.ateArtisanBread = player.TPlayer.ateArtisanBread ? 1 : 0;
        PlayerData.usedAegisCrystal = player.TPlayer.usedAegisCrystal ? 1 : 0;
        PlayerData.usedAegisFruit = player.TPlayer.usedAegisFruit ? 1 : 0;
        PlayerData.usedArcaneCrystal = player.TPlayer.usedArcaneCrystal ? 1 : 0;
        PlayerData.usedGalaxyPearl = player.TPlayer.usedGalaxyPearl ? 1 : 0;
        PlayerData.usedGummyWorm = player.TPlayer.usedGummyWorm ? 1 : 0;
        PlayerData.usedAmbrosia = player.TPlayer.usedAmbrosia ? 1 : 0;
        PlayerData.unlockedSuperCart = player.TPlayer.unlockedSuperCart ? 1 : 0;
        PlayerData.enabledSuperCart = player.TPlayer.enabledSuperCart ? 1 : 0;

        Item[] inventory = player.TPlayer.inventory;
        Item[] armor = player.TPlayer.armor;
        Item[] dye = player.TPlayer.dye;
        Item[] miscEqups = player.TPlayer.miscEquips;
        Item[] miscDyes = player.TPlayer.miscDyes;
        Item[] piggy = player.TPlayer.bank.item;
        Item[] safe = player.TPlayer.bank2.item;
        Item[] forge = player.TPlayer.bank3.item;
        Item[] voidVault = player.TPlayer.bank4.item;
        Item trash = player.TPlayer.trashItem;
        Item[] loadout1Armor = player.TPlayer.Loadouts[0].Armor;
        Item[] loadout1Dye = player.TPlayer.Loadouts[0].Dye;
        Item[] loadout2Armor = player.TPlayer.Loadouts[1].Armor;
        Item[] loadout2Dye = player.TPlayer.Loadouts[1].Dye;
        Item[] loadout3Armor = player.TPlayer.Loadouts[2].Armor;
        Item[] loadout3Dye = player.TPlayer.Loadouts[2].Dye;

        for (int i = 0; i < NetItem.MaxInventory; i++)
        {
            if (i < NetItem.InventoryIndex.Item2)
            {
                //0-58
                PlayerData.inventory[i] = (NetItem)inventory[i];
            }
            else if (i < NetItem.ArmorIndex.Item2)
            {
                //59-78
                var index = i - NetItem.ArmorIndex.Item1;
                PlayerData.inventory[i] = (NetItem)armor[index];
            }
            else if (i < NetItem.DyeIndex.Item2)
            {
                //79-88
                var index = i - NetItem.DyeIndex.Item1;
                PlayerData.inventory[i] = (NetItem)dye[index];
            }
            else if (i < NetItem.MiscEquipIndex.Item2)
            {
                //89-93
                var index = i - NetItem.MiscEquipIndex.Item1;
                PlayerData.inventory[i] = (NetItem)miscEqups[index];
            }
            else if (i < NetItem.MiscDyeIndex.Item2)
            {
                //93-98
                var index = i - NetItem.MiscDyeIndex.Item1;
                PlayerData.inventory[i] = (NetItem)miscDyes[index];
            }
            else if (i < NetItem.PiggyIndex.Item2)
            {
                //98-138
                var index = i - NetItem.PiggyIndex.Item1;
                PlayerData.inventory[i] = (NetItem)piggy[index];
            }
            else if (i < NetItem.SafeIndex.Item2)
            {
                //138-178
                var index = i - NetItem.SafeIndex.Item1;
                PlayerData.inventory[i] = (NetItem)safe[index];
            }
            else if (i < NetItem.TrashIndex.Item2)
            {
                //179-219
                PlayerData.inventory[i] = (NetItem)trash;
            }
            else if (i < NetItem.ForgeIndex.Item2)
            {
                //220
                var index = i - NetItem.ForgeIndex.Item1;
                PlayerData.inventory[i] = (NetItem)forge[index];
            }
            else if (i < NetItem.VoidIndex.Item2)
            {
                //220
                var index = i - NetItem.VoidIndex.Item1;
                PlayerData.inventory[i] = (NetItem)voidVault[index];
            }
            else if (i < NetItem.Loadout1Armor.Item2)
            {
                var index = i - NetItem.Loadout1Armor.Item1;
                PlayerData.inventory[i] = (NetItem)loadout1Armor[index];
            }
            else if (i < NetItem.Loadout1Dye.Item2)
            {
                var index = i - NetItem.Loadout1Dye.Item1;
                PlayerData.inventory[i] = (NetItem)loadout1Dye[index];
            }
            else if (i < NetItem.Loadout2Armor.Item2)
            {
                var index = i - NetItem.Loadout2Armor.Item1;
                PlayerData.inventory[i] = (NetItem)loadout2Armor[index];
            }
            else if (i < NetItem.Loadout2Dye.Item2)
            {
                var index = i - NetItem.Loadout2Dye.Item1;
                PlayerData.inventory[i] = (NetItem)loadout2Dye[index];
            }
            else if (i < NetItem.Loadout3Armor.Item2)
            {
                var index = i - NetItem.Loadout3Armor.Item1;
                PlayerData.inventory[i] = (NetItem)loadout3Armor[index];
            }
            else if (i < NetItem.Loadout3Dye.Item2)
            {
                var index = i - NetItem.Loadout3Dye.Item1;
                PlayerData.inventory[i] = (NetItem)loadout3Dye[index];
            }
        }
        return PlayerData;
    }

}