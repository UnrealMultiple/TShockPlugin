using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using ServerTools.DB;
using System.Text.RegularExpressions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ServerTools;
public partial class Plugin
{
    private static long TimerCount = 0;

    private readonly Dictionary<string, DateTime> PlayerDeath = [];

    public event Action<EventArgs>? OnTimer;

    public static readonly List<TSPlayer> Deads = [];

    public static readonly List<TSPlayer> ActivePlayers = [];

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null)
        {
            ActivePlayers.Add(ply);
            ply.RespawnTimer = 0;
        }
    }

    private void OnUpdatePlayerOnline(EventArgs args)
    {
        for (var i = ActivePlayers.Count - 1; i >= 0; i--)
        {
            var p = ActivePlayers[i];
            if (p != null && p.Active)
            {
                PlayerOnline.Add(p.Name, 1);
            }
        }
    }

    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        if (e.Player == null)
        {
            return;
        }

        DB.PlayerDeath.Add(e.Player.Name);
        Deads.Add(e.Player);
    }

    private void OnUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (!Config.Instance.KeepArmor || e.Player.HasPermission("servertool.armor.white"))
        {
            return;
        }

        var ArmorGroup = e.Player.TPlayer.armor
            .Take(10)
            .Where(x => x.type != 0)
            .GroupBy(x => x.type)
            .Where(x => x.Count() > 1)
            .Select(x => x.First());

        foreach (var keepArmor in ArmorGroup)
        {
            e.Player.SetBuff(156, 180, true);
            TShock.Utils.Broadcast(GetString($"[ServerTools] 玩家 [{e.Player.Name}] 因多饰品被冻结3秒，自动施行清理多饰品装备[i:{keepArmor.type}]"), Color.DarkRed);
        }
        if (TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            if (ArmorGroup.Any())
            {
                Utils.ClearItem(ArmorGroup.ToArray(), e.Player);
            }

            if (Config.Instance.KeepArmor2 && !Main.hardMode)
            {
                Utils.Clear7Item(e.Player);
            }
        }
    }

    private bool MessageBuffer_InvokeGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {

        if (packetId == (byte) PacketTypes.LoadNetModule)
        {
            using var ms = new MemoryStream(instance.readBuffer);
            ms.Position = readOffset;
            using var reader = new BinaryReader(ms);
            var id = reader.ReadUInt16();
            if (id == Terraria.Net.NetManager.Instance.GetId<Terraria.GameContent.NetModules.NetTextModule>())
            {
                var msg = Terraria.Chat.ChatMessage.Deserialize(reader);
                if (msg.Text.Length > Config.Instance.ChatLength)
                {
                    return false;
                }

                if (Config.Instance.DisableEmojiMessage && Regex.IsMatch(msg.Text, @"[\uD800-\uDBFF][\uDC00-\uDFFF]"))
                {
                    return false;
                }
            }
        }
        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }


    private static void OnStrike(NpcStrikeEventArgs args)
    {
        if (!Config.Instance.NpcProtect || TShock.Players[args.Player.whoAmI].HasPermission("servertool.npc.strike"))
        {
            return;
        }

        if (Config.Instance.NpcProtectList.Contains(args.Npc.netID))
        {
            args.Handled = true;
            TShock.Players[args.Player.whoAmI].SendInfoMessage(GetString($"[ServerTools] {args.Npc.FullName} 被系统保护"));
        }
    }

    private static void OnNPCUpdate(NpcAiUpdateEventArgs args)
    {
        if (!Config.Instance.NpcProtect)
        {
            return;
        }

        if (Config.Instance.NpcProtectList.Contains(args.Npc.netID) && args.Npc.active)
        {
            args.Npc.life = args.Npc.lifeMax;
            args.Npc.active = true;
            TSPlayer.All.SendData((PacketTypes) 23, "", args.Npc.whoAmI, 0f, 0f, 0f, 0);
        }
    }

    private static void ViewAccountInfo(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("语法错误，正确语法: {0}accountinfo <username>."), Commands.Specifier);
            return;
        }

        var username = string.Join(" ", args.Parameters);
        if (!string.IsNullOrWhiteSpace(username))
        {
            var account = TShock.UserAccounts.GetUserAccountByName(username);
            if (account != null)
            {
                var Timezone = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString("+#;-#");


                if (DateTime.TryParse(account.LastAccessed, out var LastSeen))
                {
                    LastSeen = DateTime.Parse(account.LastAccessed).ToLocalTime();
                    args.Player.SendSuccessMessage(GetString("{0} 最后的登录时间为 {1} {2} UTC{3}."), account.Name, LastSeen.ToShortDateString(),
                        LastSeen.ToShortTimeString(), Timezone);
                }

                if (args.Player.Group.HasPermission(Permissions.advaccountinfo))
                {
                    var KnownIps = JsonConvert.DeserializeObject<List<string>>(account.KnownIps?.ToString() ?? string.Empty);
                    var ip = KnownIps?[^1] ?? "N/A";
                    var Registered = DateTime.Parse(account.Registered).ToLocalTime();
                    args.Player.SendSuccessMessage(GetString("{0} 账户ID为 {1}."), account.Name, account.ID);
                    args.Player.SendSuccessMessage(GetString("{0} 权限组为 {1}."), account.Name, account.Group);
                    args.Player.SendSuccessMessage(GetString("{0} 最后登录使用的IP为 {1}."), account.Name, ip);
                    args.Player.SendSuccessMessage(GetString("{0} 注册时间为 {1} {2} UTC{3}."), account.Name, Registered.ToShortDateString(), Registered.ToShortTimeString(), Timezone);
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("用户 {0} 不存在."), username);
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("语法错误，正确语法: {0}accountinfo <username>."), Commands.Specifier);
        }
    }

    public static void RestPlayerCtor(Action<TSRestPlayer, string, TShockAPI.Group> orig, TSRestPlayer self, string name, TShockAPI.Group group)
    {
        self.Account = new()
        {
            Name = name,
            Group = group.Name,
            ID = self.Index
        };
        orig(self, name, group);
    }

    private void OnPlayerSpawn(object? sender, GetDataHandlers.SpawnEventArgs e)
    {
        if (e.Player != null)
        {
            Deads.Remove(e.Player);
        }
    }

    private void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
    {
        if (e.Player != null && e.Player.Dead && Config.Instance.ClearDrop)
        {
            Main.item[e.ID].TurnToAir();
            e.Handled = true;
            NetMessage.SendData(21, -1, -1, null, e.ID, 0, 0);
        }
    }

    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (Config.Instance.PreventsDeathStateJoin && Main.player[args.Who].dead)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                TShock.Players[args.Who].SendSuccessMessage(GetString("请在单人模式中结束死亡状态重新进入服务器!"));
                var count = 0;
                while (count < 3)
                {
                    TShock.Players[args.Who].SendSuccessMessage(GetString($"你将在{3 - count}秒后被踢出!"));
                    await Task.Delay(1000);
                    count++;
                }
                TShock.Players[args.Who].Disconnect(GetString("请在单人模式中结束死亡状态重新进入服务器!"));
            });
        }
    }

    private void OnUpdate(EventArgs e)
    {
        TimerCount++;
        if (TimerCount % 60 == 0)
        {
            OnTimer?.Invoke(e);
            if (Config.Instance.DeadTimer)
            {
                foreach (var ply in Deads)
                {
                    if (ply != null && ply.Active && ply.Dead && ply.RespawnTimer > 0)
                    {
                        ply.SendInfoMessage(Config.Instance.DeadFormat, ply.RespawnTimer);
                    }
                }
            }

        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply == null)
        {
            return;
        }
        ActivePlayers.Remove(TShock.Players[args.Who]);
        Deads.Remove(ply);
        if (ply.Dead)
        {
            this.PlayerDeath[ply.Name] = DateTime.Now.AddSeconds(ply.RespawnTimer);
        }
    }

    private void NewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (Main.projectile.Where(x => x != null && x.owner == e.Owner && x.sentry && x.active).Count() > Config.Instance.sentryLimit)
        {
            e.Player.Disconnect(GetString($"你因哨兵数量超过{Config.Instance.sentryLimit}被踢出"));
        }
        if (e.Player.TPlayer.slotsMinions > Config.Instance.summonLimit)
        {
            e.Player.Disconnect(GetString($"你因召唤物数量超过{Config.Instance.summonLimit}被踢出"));
        }
        if (Main.projectile[e.Index].bobber && Config.Instance.MultipleFishingRodsAreProhibited && Config.Instance.ForbiddenBuoys.FindAll(f => f == e.Type).Count > 0)
        {
            var bobber = Main.projectile.Where(f => f != null && f.owner == e.Owner && f.active && f.type == e.Type);
            if (bobber.Count() > 2)
            {
                e.Player.SendErrorMessage(GetString("你因多鱼线被石化3秒钟!"));
                e.Player.SetBuff(156, 180, true);
            }
        }
    }

    private static void HandleCommandLine(string[] param)
    {
        var args = Terraria.Utils.ParseArguements(param);
        foreach (var key in args)
        {
            switch (key.Key.ToLower())
            {
                case "-seed":
                    Main.AutogenSeedName = key.Value;
                    break;
            }
        }
    }


    private void PostInitialize(EventArgs args)
    {
        //设置世界模式
        if (Config.Instance.SetWorldMode)
        {
            if (Config.Instance.WorldMode > 1 && Config.Instance.WorldMode < 4)
            {
                Main.GameMode = Config.Instance.WorldMode;
                TSPlayer.All.SendData(PacketTypes.WorldInfo);
            }
        }
        //旅途难度
        if (Main.IsJourneyMode && Config.Instance.SetJourneyDifficult)
        {
            Utils.SetJourneyDiff(Config.Instance.JourneyMode);
        }
    }

    private void GetData(GetDataEventArgs args)
    {
        var ply = TShock.Players[args.Msg.whoAmI];
        if (args.Handled || ply == null)
        {
            return;
        }

        if (Config.Instance.PickUpMoney && args.MsgID == PacketTypes.SyncExtraValue)
        {
            using BinaryReader reader = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
            var npcid = reader.ReadInt16();
            var money = reader.ReadInt32();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            ply.SendData(PacketTypes.SyncExtraValue, "", npcid, 0, x, y);
            args.Handled = true;
            return;
        }


        if (Config.Instance.KeepOpenChest && args.MsgID == PacketTypes.ChestOpen)
        {
            using BinaryReader binaryReader5 = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
            var ChestId = binaryReader5.ReadInt16();
            if (ChestId != -1 && ply.ActiveChest != -1)
            {
                ply.ActiveChest = -1;
                ply.SendData(PacketTypes.ChestOpen, "", -1);
                ply.SendErrorMessage(GetString("禁止双箱!"));
                args.Handled = true;
            }
        }

        if (args.MsgID == PacketTypes.ChestGetContents && Config.Instance.KeepOpenChest)
        {
            if (ply.ActiveChest != -1)
            {
                ply.ActiveChest = -1;
                ply.SendData(PacketTypes.ChestOpen, "", -1);
                ply.SendErrorMessage(GetString("禁止双箱!"));
                args.Handled = true;
            }
        }
    }

    private void OnInitialize(EventArgs args)
    {
        if (TShock.UserAccounts.GetUserAccounts().Count == 0 && Config.Instance.ResetExecCommands.Length > 0)
        {
            for (var i = 0; i < Config.Instance.ResetExecCommands.Length; i++)
            {
                Commands.HandleCommand(TSPlayer.Server, Config.Instance.ResetExecCommands[i]);
            }
        }
    }

    private void OnJoin(JoinEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        if (Config.Instance.OnlySoftCoresAreAllowed)
        {
            if (TShock.Players[args.Who].Difficulty != 0)
            {
                TShock.Players[args.Who].Disconnect(GetString("仅允许软核进入!"));
            }
        }
        if (Config.Instance.BlockUnregisteredEntry)
        {
            if (args.Who == -1 || TShock.Players[args.Who] == null || TShock.UserAccounts.GetUserAccountsByName(TShock.Players[args.Who].Name).Count == 0)
            {
                TShock.Players[args.Who].Disconnect(Config.Instance.BlockEntryStatement);
            }
        }
        if (Config.Instance.DeathLast && TShock.Players[args.Who] != null)
        {
            if (this.PlayerDeath.TryGetValue(TShock.Players[args.Who].Name, out var time))
            {
                var respawn = time - DateTime.Now;
                if (respawn.TotalSeconds > 0)
                {
                    TShock.Players[args.Who].Disconnect(GetString($"退出服务器时处于死亡状态！\n请等待死亡结束，还有{respawn.TotalSeconds:0}秒结束！"));
                }
            }
        }
    }
}
