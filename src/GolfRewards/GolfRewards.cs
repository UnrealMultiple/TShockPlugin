using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace RewardSection;

[ApiVersion(2, 1)]
public class GolfRewards : TerrariaPlugin
{
    public override string Name => "高尔夫奖励";
    public override string Author => "GK 阁下 由 鸽子 定制，肝帝熙恩更新适配1449";
    public override string Description => "将高尔夫打入球洞会得到奖励.";
    public override Version Version => new Version(1, 0, 8);

    public GolfRewards(Main game) : base(game)
    {
        base.Order = 5;
    }


    public override void Initialize()
    {
        //LC.RI();
        LC.RC();
        GeneralHooks.ReloadEvent += this.CMD;
        Commands.ChatCommands.Add(new Command("物块坐标", this.CMD2, "物块坐标")
        {
            HelpText = "输入/物块坐标后敲击物块确认坐标"
        });
        ServerApi.Hooks.NetGetData.Register(this, this.GetData);

        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);

        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);

        GetDataHandlers.TileEdit += this.TileEdit;

        GetDataHandlers.LandGolfBallInCup += this.Golf;
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.CMD;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CMD2);
            ServerApi.Hooks.NetGetData.Deregister(this, this.GetData);

            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);

            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);

            GetDataHandlers.TileEdit -= this.TileEdit;

            GetDataHandlers.LandGolfBallInCup -= this.Golf;
        }
        base.Dispose(disposing);
    }


    private void CMD(ReloadEventArgs args)
    {
        LC.RC();
        args.Player.SendSuccessMessage("[高尔夫奖励] 重读配置完毕!");
    }

    private void CMD2(CommandArgs args)
    {
        if (LC.LPlayers.TryGetValue(args.Player.Index, out var player) && player != null)
        {
            player.tip = true;
        }
        args.Player.SendSuccessMessage("敲击物块以确认其坐标!");
    }

    private void OnGreetPlayer(GreetPlayerEventArgs e)
    {
        LC.LPlayers.AddOrUpdate(e.Who, new LPlayer(e.Who), (_, existingPlayer) => existingPlayer);
    }

    private void OnLeave(LeaveEventArgs e)
    {
        LC.LPlayers.TryRemove(e.Who, out _);
    }

    private void TileEdit(object? sender, GetDataHandlers.TileEditEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var tsplayer = TShock.Players[args.Player.Index];
        if (tsplayer == null)
        {
            return;
        }

        if (LC.LPlayers.TryGetValue(args.Player.Index, out var player) && player != null && player.tip)
        {
            player.tip = false;
            tsplayer.SendInfoMessage($"目标坐标为: X{args.X} Y{args.Y}");
        }
    }


    public void Golf(object? sender, GetDataHandlers.LandGolfBallInCupEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var player = args.Player;
        if (player == null)
        {
            return;
        }

        int tileX = args.TileX;
        int tileY = args.TileY;
        int hits = args.Hits;

        foreach (var RewardSection in LC.LConfig.RewardTable)
        {
            if (RewardSection.HolePositionX != tileX || RewardSection.HolePositionY != tileY)
            {
                continue;
            }

            if (hits <= RewardSection.MinSwings || hits > RewardSection.MaxSwings)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(RewardSection.HitPrompt))
            {
                TShock.Utils.Broadcast(RewardSection.HitPrompt, byte.MaxValue, 215, 0);
            }

            var randomItems = RewardSection.GetRandomItems();
            if (randomItems != null && randomItems.ItemId != 0 && randomItems.ItemStack > 0)
            {
                player.GiveItem(randomItems.ItemId, randomItems.ItemStack, randomItems.ItemPrefix);
            }

            var randomCMD = RewardSection.GetRandomCMD();
            if (!string.IsNullOrEmpty(randomCMD))
            {
                var cmdSuccess = Commands.HandleCommand(TSPlayer.Server, randomCMD.Replace("{name}", player.Name));
                if (!cmdSuccess)
                {
                    Console.WriteLine($"指令 {randomCMD} 执行失败！ ");
                }
            }

            if (RewardSection.OnlyClaimRewardAtThisLocation)
            {
                break;
            }
        }
    }

    private void GetData(GetDataEventArgs args)
    {
    }
}