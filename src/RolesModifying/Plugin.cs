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
            this.Player = player;
            //LastPlayerData = LastData;
            this.SourcePlayerData = sourcePlayerData;
        }
    }

    public override string Author => "少司命";

    public override string Description => GetString("复制然后修改其他玩家的背包");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 0, 5);

    private readonly Dictionary<TSPlayer, ModifyData> TempData = new();
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        Commands.ChatCommands.Add(new Command("tshock.user.rolesmodifying", this.Roles, "rm", "修改", "查背包"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.Roles);
        }
        base.Dispose(disposing);
    }

    private void Roles(CommandArgs args)
    {
        if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "save")
        {
            if (this.TempData.TryGetValue(args.Player, out var modify))
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
                this.TempData.Remove(args.Player);
                args.Player.SendSuccessMessage(GetString("修改保存成功！"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("你没有对玩家的信息进行修改，无需保存！"));
            }
        }
        else if (args.Parameters.Count > 0)
        {
            var target = TShock.Players.FirstOrDefault(ply => ply != null && ply.Active && ply.Name == args.Parameters[0]);
            //在线
            if (target != null)
            {
                var targetPlayerData = this.CopyCharacter(target);
                this.TempData[args.Player] = new(target, this.CopyCharacter(args.Player));
                //保存自己背包物品
                TShock.CharacterDB.InsertPlayerData(args.Player);
                //复制
                target.PlayerData.RestoreCharacter(args.Player);
                args.Player.SendSuccessMessage(GetString("正在查看{0}的人物信息，可以对背包进行修改，使用/rm save 进行保存。"), args.Parameters[0]);
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
                    var targetPlayerData = this.CopyCharacter(TempPlayer);
                    this.TempData[args.Player] = new(TempPlayer, this.CopyCharacter(args.Player));
                    //保存自己背包物品
                    TShock.CharacterDB.InsertPlayerData(args.Player);
                    //复制
                    TempPlayer.PlayerData.RestoreCharacter(args.Player);
                    args.Player.SendSuccessMessage(GetString("正在查看{0}的人物信息，可以对背包进行修改，使用/rm save 进行保存。"), args.Parameters[0]);
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("目标玩家不存在!"));
                }

            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("用法:\n/rm <玩家名> - 查看玩家信息\n/rm save - 保存修改"));
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && this.TempData.TryGetValue(player, out var modify))
        {
            modify.SourcePlayerData.RestoreCharacter(player);
            this.TempData.Remove(player);
        }
    }


    /// <summary>
    /// 复制PlayerData对象
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public PlayerData CopyCharacter(TSPlayer player)
    {
        var PlayerData = new PlayerData(true)
        {
            health = player.TPlayer.statLife > 0 ? player.TPlayer.statLife : 1,
            maxHealth = player.TPlayer.statLifeMax,
            mana = player.TPlayer.statMana,
            maxMana = player.TPlayer.statManaMax,
            spawnX = player.TPlayer.SpawnX,
            spawnY = player.TPlayer.SpawnY,
            extraSlot = player.TPlayer.extraAccessory ? 1 : 0,
            skinVariant = player.TPlayer.skinVariant,
            hair = player.TPlayer.hair,
            hairDye = player.TPlayer.hairDye,
            hairColor = player.TPlayer.hairColor,
            pantsColor = player.TPlayer.pantsColor,
            shirtColor = player.TPlayer.shirtColor,
            underShirtColor = player.TPlayer.underShirtColor,
            shoeColor = player.TPlayer.shoeColor,
            hideVisuals = player.TPlayer.hideVisibleAccessory,
            skinColor = player.TPlayer.skinColor,
            eyeColor = player.TPlayer.eyeColor,
            questsCompleted = player.TPlayer.anglerQuestsFinished,
            usingBiomeTorches = player.TPlayer.UsingBiomeTorches ? 1 : 0,
            happyFunTorchTime = player.TPlayer.happyFunTorchTime ? 1 : 0,
            unlockedBiomeTorches = player.TPlayer.unlockedBiomeTorches ? 1 : 0,
            currentLoadoutIndex = player.TPlayer.CurrentLoadoutIndex,
            ateArtisanBread = player.TPlayer.ateArtisanBread ? 1 : 0,
            usedAegisCrystal = player.TPlayer.usedAegisCrystal ? 1 : 0,
            usedAegisFruit = player.TPlayer.usedAegisFruit ? 1 : 0,
            usedArcaneCrystal = player.TPlayer.usedArcaneCrystal ? 1 : 0,
            usedGalaxyPearl = player.TPlayer.usedGalaxyPearl ? 1 : 0,
            usedGummyWorm = player.TPlayer.usedGummyWorm ? 1 : 0,
            usedAmbrosia = player.TPlayer.usedAmbrosia ? 1 : 0,
            unlockedSuperCart = player.TPlayer.unlockedSuperCart ? 1 : 0,
            enabledSuperCart = player.TPlayer.enabledSuperCart ? 1 : 0
        };

        var inventory = player.TPlayer.inventory;
        var armor = player.TPlayer.armor;
        var dye = player.TPlayer.dye;
        var miscEqups = player.TPlayer.miscEquips;
        var miscDyes = player.TPlayer.miscDyes;
        var piggy = player.TPlayer.bank.item;
        var safe = player.TPlayer.bank2.item;
        var forge = player.TPlayer.bank3.item;
        var voidVault = player.TPlayer.bank4.item;
        var trash = player.TPlayer.trashItem;
        var loadout1Armor = player.TPlayer.Loadouts[0].Armor;
        var loadout1Dye = player.TPlayer.Loadouts[0].Dye;
        var loadout2Armor = player.TPlayer.Loadouts[1].Armor;
        var loadout2Dye = player.TPlayer.Loadouts[1].Dye;
        var loadout3Armor = player.TPlayer.Loadouts[2].Armor;
        var loadout3Dye = player.TPlayer.Loadouts[2].Dye;

        for (var i = 0; i < NetItem.MaxInventory; i++)
        {
            if (i < NetItem.InventoryIndex.Item2)
            {
                //0-58
                PlayerData.inventory[i] = (NetItem) inventory[i];
            }
            else if (i < NetItem.ArmorIndex.Item2)
            {
                //59-78
                var index = i - NetItem.ArmorIndex.Item1;
                PlayerData.inventory[i] = (NetItem) armor[index];
            }
            else if (i < NetItem.DyeIndex.Item2)
            {
                //79-88
                var index = i - NetItem.DyeIndex.Item1;
                PlayerData.inventory[i] = (NetItem) dye[index];
            }
            else if (i < NetItem.MiscEquipIndex.Item2)
            {
                //89-93
                var index = i - NetItem.MiscEquipIndex.Item1;
                PlayerData.inventory[i] = (NetItem) miscEqups[index];
            }
            else if (i < NetItem.MiscDyeIndex.Item2)
            {
                //93-98
                var index = i - NetItem.MiscDyeIndex.Item1;
                PlayerData.inventory[i] = (NetItem) miscDyes[index];
            }
            else if (i < NetItem.PiggyIndex.Item2)
            {
                //98-138
                var index = i - NetItem.PiggyIndex.Item1;
                PlayerData.inventory[i] = (NetItem) piggy[index];
            }
            else if (i < NetItem.SafeIndex.Item2)
            {
                //138-178
                var index = i - NetItem.SafeIndex.Item1;
                PlayerData.inventory[i] = (NetItem) safe[index];
            }
            else if (i < NetItem.TrashIndex.Item2)
            {
                //179-219
                PlayerData.inventory[i] = (NetItem) trash;
            }
            else if (i < NetItem.ForgeIndex.Item2)
            {
                //220
                var index = i - NetItem.ForgeIndex.Item1;
                PlayerData.inventory[i] = (NetItem) forge[index];
            }
            else if (i < NetItem.VoidIndex.Item2)
            {
                //220
                var index = i - NetItem.VoidIndex.Item1;
                PlayerData.inventory[i] = (NetItem) voidVault[index];
            }
            else if (i < NetItem.Loadout1Armor.Item2)
            {
                var index = i - NetItem.Loadout1Armor.Item1;
                PlayerData.inventory[i] = (NetItem) loadout1Armor[index];
            }
            else if (i < NetItem.Loadout1Dye.Item2)
            {
                var index = i - NetItem.Loadout1Dye.Item1;
                PlayerData.inventory[i] = (NetItem) loadout1Dye[index];
            }
            else if (i < NetItem.Loadout2Armor.Item2)
            {
                var index = i - NetItem.Loadout2Armor.Item1;
                PlayerData.inventory[i] = (NetItem) loadout2Armor[index];
            }
            else if (i < NetItem.Loadout2Dye.Item2)
            {
                var index = i - NetItem.Loadout2Dye.Item1;
                PlayerData.inventory[i] = (NetItem) loadout2Dye[index];
            }
            else if (i < NetItem.Loadout3Armor.Item2)
            {
                var index = i - NetItem.Loadout3Armor.Item1;
                PlayerData.inventory[i] = (NetItem) loadout3Armor[index];
            }
            else if (i < NetItem.Loadout3Dye.Item2)
            {
                var index = i - NetItem.Loadout3Dye.Item1;
                PlayerData.inventory[i] = (NetItem) loadout3Dye[index];
            }
        }
        return PlayerData;
    }

}