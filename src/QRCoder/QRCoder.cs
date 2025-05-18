using System;
using System.Collections.Generic;
using Net.Codecrete.QrCodeGenerator;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static Net.Codecrete.QrCodeGenerator.QrCode;
using System.Linq;
using static TShockAPI.GetDataHandlers;

namespace QRCoder;

[ApiVersion(2, 1)]
public class QRCoder : TerrariaPlugin
{
    public override string Author => "Jonesn，熙恩，Radix.";
    public override string Description => "生成二维码";
    public override string Name => "QRCoder";
    public override Version Version => new Version(1, 0, 0, 0);

    private readonly Dictionary<int, QRPosition> _playerPositions = new Dictionary<int, QRPosition>();

    public QRCoder(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("qr.add", this.QREncoder, "qr")
        {
            HelpText = GetString("生成二维码，用法：/qr <内容> [尺寸(不指定则为自适应)]")
        });

        Commands.ChatCommands.Add(new Command("qr.add", this.SetQRPosition, "qrpos")
        {
            HelpText = GetString("设置二维码位置，用法：/qrpos <tl|bl|tr|br>，tl=左上角，bl=左下角，tr=右上角，br=右下角")
        });

        GetDataHandlers.TileEdit += this.OnTileEdit;

        TShock.RestApi.Register(new SecureRestCommand("/tool/qrcoder", this.QRtest, "tool.rest.qrcoder"));
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.QREncoder);
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.SetQRPosition);
            GetDataHandlers.TileEdit -= this.OnTileEdit;
((List<RestCommand>) typeof(Rest)
                .GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .GetValue(TShock.RestApi)!)
                .RemoveAll(x => x.Name == "/tool/qrcoder");
        }
        base.Dispose(Disposing);
    }

    private void OnTileEdit(object? sender, TileEditEventArgs args)
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

        // 检查该玩家是否有待处理的二维码生成请求
        if (this._playerPositions.TryGetValue(args.Player.Index, out var position) && position.WaitingForSelection)
        {
            position.WaitingForSelection = false;
            position.X = args.X;
            position.Y = args.Y;

            tsplayer.SendInfoMessage(GetString($"位置已确认: X={args.X} Y={args.Y}"));

            // 提示玩家现在可以使用/qr命令生成二维码了
            tsplayer.SendInfoMessage(GetString("位置已设置完成，现在可以使用 /qr <内容> 生成二维码了"));

            args.Handled = true;
        }
    }

    private void SetQRPosition(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 1)
        {
            player.SendErrorMessage(GetString("用法：/qrpos <tl|bl|tr|br> - tl=左上角，bl=左下角，tr=右上角，br=右下角"));
            return;
        }

        var posType = args.Parameters[0];
        if (posType != "tl" && posType != "bl" && posType != "tr" && posType != "br")
        {
            player.SendErrorMessage(GetString("位置参数无效。请使用：tl(左上角)、bl(左下角)、tr(右上角)、br(右下角)"));
            return;
        }

        if (!this._playerPositions.TryGetValue(player.Index, out var position))
        {
            position = new QRPosition();
            this._playerPositions[player.Index] = position;
        }

        position.PositionType = posType;
        position.WaitingForSelection = true;

        player.SendSuccessMessage(GetString($"请点击一个方块来设置二维码的{this.GetPositionName(posType)}位置！"));
    }

    private string GetPositionName(string posType)
    {
        return posType switch
        {
            "tl" => GetString("左上角"),
            "bl" => GetString("左下角"),
            "tr" => GetString("右上角"),
            "br" => GetString("右下角"),
            _ => GetString("未知")
        };
    }

    private object QRtest(RestRequestArgs args)
    {
        return new RestObject();
    }

    private void QREncoder(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 1)
        {
            player.SendErrorMessage(GetString("用法：/qr <内容> [尺寸(不指定则为自适应)]"));
            return;
        }

        var content = string.Join(" ", args.Parameters[0]);
        var size = -1;
        if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out var parsed))
        {
            size = parsed;
        }

        // 检查是否已经设置了位置
        if (!this._playerPositions.TryGetValue(player.Index, out var position) ||
            position.X == -1 || position.Y == -1)
        {
            player.SendErrorMessage(GetString("请先使用 /qrpos <tl|bl|tr|br> 设置二维码位置！"));
            return;
        }

        // 如果正在等待玩家选择位置，提示需要先完成位置选择
        if (position.WaitingForSelection)
        {
            player.SendInfoMessage(GetString("请先点击一个方块确认位置，然后再次输入 /qr 命令生成二维码。"));
            return;
        }

        // 生成二维码
        this.GenerateQRCode(player, content, position, size);
    }

    private void GenerateQRCode(TSPlayer player, string content, QRPosition position, int size)
    {
        var list = QrSegment.MakeSegments(content);
        var qrCode = size > 0
            ? QrCode.EncodeSegments(list, Ecc.Low, size, size, -1, false)
            : QrCode.EncodeSegments(list, Ecc.Low, 1, 40, -1, false);

        // 根据选择的位置类型计算起始坐标
        int startX, startY;

        switch (position.PositionType)
        {
            case "tl": // 左上角
                startX = position.X;
                startY = position.Y;
                break;
            case "bl": // 左下角
                startX = position.X;
                startY = position.Y - qrCode.Size + 1;
                break;
            case "tr": // 右上角
                startX = position.X - qrCode.Size + 1;
                startY = position.Y;
                break;
            case "br": // 右下角
                startX = position.X - qrCode.Size + 1;
                startY = position.Y - qrCode.Size + 1;
                break;
            default:
                // 默认使用左上角
                startX = position.X;
                startY = position.Y;
                break;
        }

        player.SendInfoMessage(GetString($"正在 X={startX}, Y={startY} 生成二维码，大小: {qrCode.Size}x{qrCode.Size}"));

        // 先绘制边框（围绕二维码）
        for (var i = -1; i <= qrCode.Size; i++)
        {
            for (var j = -1; j <= qrCode.Size; j++)
            {
                // 如果是边缘格子（边框）
                if (i == -1 || i == qrCode.Size || j == -1 || j == qrCode.Size)
                {
                    var tileX = startX + j;
                    var tileY = startY + i;

                    Main.tile[tileX, tileY].wall = Terraria.ID.WallID.EchoWall;
                    TSPlayer.All.SendTileSquareCentered(tileX, tileY, 1);
                }
            }
        }

        // 生成二维码本体
        for (var i = 0; i < qrCode.Size; i++)
        {
            for (var j = 0; j < qrCode.Size; j++)
            {
                var tileX = startX + j;
                var tileY = startY + i;

                Main.tile[tileX, tileY].wall = Terraria.ID.WallID.DiamondGemspark; // 宝石墙

                if (qrCode.GetModule(j, i))
                {
                    Main.tile[tileX, tileY].wallColor(29); // 暗影漆
                }
                else
                {
                    Main.tile[tileX, tileY].wallColor(26); // 普通漆
                }

                WorldGen.paintCoatWall(tileX, tileY, 1, true); // 夜明漆
                TSPlayer.All.SendTileSquareCentered(tileX, tileY, 1);
            }
        }

        player.SendSuccessMessage(GetString($"二维码已生成! 内容: {content}"));
    }

    // 存储玩家选择的二维码位置
    private class QRPosition
    {
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public string PositionType { get; set; } = "tl"; // 默认左上角
        public bool WaitingForSelection { get; set; } = false;
    }
}