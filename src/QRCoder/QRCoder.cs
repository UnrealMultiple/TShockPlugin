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
using System.Reflection;
using LazyAPI;

namespace QRCoder;

[ApiVersion(2, 1)]
public class QRCoder(Main game) : LazyPlugin(game)
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "Jonesn，熙恩，Radix.";
    public override string Description => "生成二维码";
    public override Version Version => new (1, 1, 0, 0);

    private readonly Dictionary<int, QRPosition> _playerPositions = new ();

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new ("qr.add", this.QREncoder, "qr")
        {
            HelpText = GetString("生成二维码，用法：/qr <内容> [尺寸(不指定则为自适应)]")
        });

        Commands.ChatCommands.Add(new ("qr.add", this.SetQRPosition, "qrpos")
        {
            HelpText = GetString("设置二维码位置，用法：/qrpos <tl|bl|tr|br>，tl=左上角，bl=左下角，tr=右上角，br=右下角")
        });

        Commands.ChatCommands.Add(new ("qr.add", this.SetQRConfig, "qrconf")
        {
            HelpText = GetString("设置二维码配置内容，用法：/qrconf <键> <值>")
        });

        TileEdit += this.OnTileEdit;
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        TShock.RestApi.Register(new SecureRestCommand("/tool/qrcoder", this.QRtest, "tool.rest.qrcoder"));
    }
    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.QREncoder);
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.SetQRPosition);
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.SetQRConfig);
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
            TileEdit -= this.OnTileEdit;
            ((List<RestCommand>) typeof(Rest)
                .GetField("commands", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(TShock.RestApi)!)
                .RemoveAll(x => x.UriTemplate == "/tool/qrcoder");
        }
        base.Dispose(Disposing);
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName =
            $"embedded.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        var assemblyData = new byte[stream.Length];
        _ = stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
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
            position = new ();
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
    public Ecc QRErrorCorrectionLevel(int level)
    {
        // 根据配置返回对应的纠错级别
        return Config.Instance.QRLevel switch
        {
            1 => Ecc.Low,
            2 => Ecc.Medium,
            3 => Ecc.Quartile,
            4 => Ecc.High,
            _ => Ecc.Medium,// 默认返回中等纠错级别或抛出异常
        };
    }

    private void GenerateQRCode(TSPlayer player, string content, QRPosition position, int size)
    {
        var list = QrSegment.MakeSegments(content);
        var eccLevel = this.QRErrorCorrectionLevel(Config.Instance.QRLevel);
        var qrCode = size > 0
            ? QrCode.EncodeSegments(list, eccLevel, size, size, -1, false)
            : QrCode.EncodeSegments(list, eccLevel, 1, 40, -1, false);

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

                Main.tile[tileX, tileY].wall = (ushort)Config.Instance.BaseWall; // 底墙

                if (qrCode.GetModule(j, i))
                {
                    Main.tile[tileX, tileY].wall = (ushort)Config.Instance.CodeWall; // 码墙
                    WorldGen.paintWall(tileX, tileY, (byte) Config.Instance.CodeColor,true); // 码漆
                }
                else
                {
                    WorldGen.paintWall(tileX, tileY, (byte)Config.Instance.BaseColor, true); // 底漆
                }
                if (Config.Instance.isGlowPaintApplied)
                {
                    WorldGen.paintCoatWall(tileX, tileY, 1, true); // 夜明漆
                }
                else
                {
                    WorldGen.paintCoatWall(tileX, tileY, 0, true); // 无漆
                }
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
    private void SetQRConfig(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage(GetString("用法：/qrconf <键> <值>"));
            return;
        }

        var key = args.Parameters[0];
        var value = args.Parameters[1];

        try
        {
            switch (key)
            {
                case "BaseWall":
                case "bw":
                case "底墙":
                    Config.Instance.BaseWall = Convert.ToInt32(value);
                    break;
                case "BaseColor":
                case "bc":
                case "底墙颜色":
                    Config.Instance.BaseColor = Convert.ToInt32(value);
                    break;
                case "CodeWall":
                case "cw":
                case "码墙":
                    Config.Instance.CodeWall = Convert.ToInt32(value);
                    break;
                case "CodeColor":
                case "cc":
                case "码墙颜色":
                    Config.Instance.CodeColor = Convert.ToInt32(value);
                    break;
                case "IlluminantCoating":
                case "ic":
                case "夜明涂料":
                    Config.Instance.isGlowPaintApplied = Convert.ToBoolean(value);
                    break;
                case "QRErrorCorrectionLevel":
                case "qrlevel":
                case "纠错等级":
                    var level = Convert.ToInt32(value);
                    if (level < 1 || level > 4)
                    {
                        args.Player.SendErrorMessage("纠错等级必须是 1 到 4 之间的整数。");
                        return;
                    }
                    Config.Instance.QRLevel = level;
                    break;
                default:
                    args.Player.SendErrorMessage($"未知的配置项：{key}");
                    return;
            }

            Config.Save();

            args.Player.SendSuccessMessage(GetString($"已设置二维码配置：{key}={value}"));
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage("设置配置时发生错误：" + ex.Message);
        }
    }
}