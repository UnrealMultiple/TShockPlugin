using Microsoft.Data.Sqlite;
using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Localization;
using WorldEdit.Commands;
using WorldEdit.Commands.Biomes;
using WorldEdit.Expressions;
using WorldEdit.Extensions;

namespace WorldEdit;

[ApiVersion(2, 1)]
public class WorldEdit : TerrariaPlugin
{
    public const string WorldEditFolderName = "worldedit";

    public static readonly string ConfigPath;

    public static Config Config;

    public static Dictionary<string, global::WorldEdit.Commands.Biomes.Biome> Biomes;

    public static Dictionary<string, int> Colors;

    public static IDbConnection Database;

    public static Dictionary<string, Selection> Selections;

    public static Dictionary<string, int> Tiles;

    public static Dictionary<string, int> Walls;

    public static Dictionary<string, int> Slopes;

    public static readonly HandlerCollection<CanEditEventArgs> CanEdit;

    private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

    private readonly BlockingCollection<WECommand> _commandQueue = new BlockingCollection<WECommand>();

    public override string Author => "Nyx Studios, massive upgrade by Anzhelika";

    public override string Description => "Adds commands for mass editing of blocks.";

    public override string Name => "WorldEdit";

    public override Version Version => new Version(2025, 06, 05);

    static WorldEdit()
    {
        ConfigPath = Path.Combine("worldedit", "config.json");
        Config = new Config();
        Biomes = new Dictionary<string, global::WorldEdit.Commands.Biomes.Biome>();
        Colors = new Dictionary<string, int>();
        Selections = new Dictionary<string, Selection>();
        Tiles = new Dictionary<string, int>();
        Walls = new Dictionary<string, int>();
        Slopes = new Dictionary<string, int>();
        CanEdit = Activator.CreateInstance(typeof(HandlerCollection<CanEditEventArgs>), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { "CanEditHook" }, null) as HandlerCollection<CanEditEventArgs>;
    }

    public WorldEdit(Main game)
        : base(game)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            GeneralHooks.ReloadEvent -= OnReload;
            _cancel.Cancel();
        }
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        GeneralHooks.ReloadEvent += OnReload;
    }

    private static void OnReload(ReloadEventArgs e)
    {
        Config = Config.Read(ConfigPath);
        Tools.MAX_UNDOS = Config.MaxUndoCount;
        MagicWand.MaxPointCount = Config.MagicWandTileLimit;
        e?.Player?.SendSuccessMessage("[WorldEdit] Successfully reloaded config.");
        if (!Directory.Exists(Config.SchematicFolderPath))
        {
            Directory.CreateDirectory(Config.SchematicFolderPath);
        }
    }

    private void OnGetData(GetDataEventArgs e)
    {
        if (e.Handled) return;

        int msgID = (int) e.MsgID;
        if (msgID != 17 && msgID != 109) return;

        var player = TShock.Players[e.Msg.whoAmI];
        if (player == null) return;

        var playerInfo = player.GetPlayerInfo();
        if (playerInfo.Point == 0) return;

        using var reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length));

        int x1, y1, x2, y2;
        if (msgID == 17)
        {
            x1 = reader.ReadInt16();
            y1 = reader.ReadInt16();
            x2 = reader.ReadInt16();
            y2 = reader.ReadInt16();

            if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 ||
                x1 >= Main.maxTilesX || y1 >= Main.maxTilesY ||
                x2 >= Main.maxTilesX || y2 >= Main.maxTilesY) return;
        }
        else
        {
            reader.ReadByte();
            x1 = x2 = reader.ReadInt16();
            y1 = y2 = reader.ReadInt16();

            if (x1 < 0 || y1 < 0 || x1 >= Main.maxTilesX || y1 >= Main.maxTilesY) return;
        }

        var whoAmI = e.Msg.whoAmI;
        var currentPlayer = TShock.Players[whoAmI];

        if (playerInfo.Point == 1)
        {
            playerInfo.X = x1;
            playerInfo.Y = y1;
            currentPlayer.SendInfoMessage("Set point 1.");
        }
        else if (playerInfo.Point == 2)
        {
            playerInfo.X2 = x2;
            playerInfo.Y2 = y2;
            currentPlayer.SendInfoMessage("Set point 2.");
        }
        else if (playerInfo.Point == 3)
        {
            var regionNames = TShock.Regions.InAreaRegionName(x1, y1).ToList();
            if (regionNames.Count == 0)
            {
                currentPlayer.SendErrorMessage("No region exists there.");
            }
            else
            {
                var region = TShock.Regions.GetRegionByName(regionNames[0]);
                var rect = region.Area;
                playerInfo.X = rect.Left;
                playerInfo.Y = rect.Top;
                playerInfo.X2 = rect.Right;
                playerInfo.Y2 = rect.Bottom;
                currentPlayer.SendInfoMessage("Set region.");
            }
        }
        else if (playerInfo.Point == 4 && playerInfo.SavedExpression != null)
        {
            if (MagicWand.GetMagicWandSelection(x1, y1, playerInfo.SavedExpression, currentPlayer, out var magicWand))
            {
                playerInfo.MagicWand = magicWand;
                currentPlayer.SendSuccessMessage("Set magic wand selection.");
                playerInfo.SavedExpression = null;
            }
            else
            {
                currentPlayer.SendErrorMessage("Can't start counting magic wand selection from this tile.");
            }
        }

        playerInfo.Point = 0;
        e.Handled = true;

        if (msgID == 109) // TileSquare
        {
            currentPlayer.SendTileSquare(x1, y1, 3);
        }
    }

    private void OnInitialize(EventArgs e)
    {
        string path = Path.Combine("worldedit", "deleted.lock");
        if (!Directory.Exists("worldedit"))
        {
            Directory.CreateDirectory("worldedit");
            File.Create(path).Close();
        }
        OnReload(null);
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.admin", EditConfig, "/worldedit", "/wedit")
        {
            HelpText = "Edits config options."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.activate", Activate, "/activate")
        {
            HelpText = "Activates non-working signs, chests or item frames."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.all", All, "/all")
        {
            HelpText = "Sets the worldedit selection to the entire world."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.biome", Biome, "/biome")
        {
            HelpText = "Converts biomes in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.copy", Copy, "/copy", "/c")
        {
            HelpText = "Copies the worldedit selection to the clipboard."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.cut", Cut, "/cut")
        {
            HelpText = "Copies the worldedit selection to the clipboard, then deletes it."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.drain", Drain, "/drain")
        {
            HelpText = "Drains liquids in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.fill", Fill, "/fill")
        {
            HelpText = "Fills the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.fillwall", FillWall, "/fillwall", "/fillw")
        {
            HelpText = "Fills the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.fixghosts", FixGhosts, "/fixghosts")
        {
            HelpText = "Fixes invisible signs, chests and item frames."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.fixgrass", FixGrass, "/fixgrass")
        {
            HelpText = "Fixes suffocated grass in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.fixhalves", FixHalves, "/fixhalves")
        {
            HelpText = "Fixes half blocks in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.fixslopes", FixSlopes, "/fixslopes")
        {
            HelpText = "Fixes covered slopes in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.flip", Flip, "/flip")
        {
            HelpText = "Flips the worldedit clipboard."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.flood", Flood, "/flood")
        {
            HelpText = "Floods liquids in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.magic.wand", MagicWandTool, "/magicwand", "/mwand")
        {
            HelpText = "Creates worldedit selection from contiguous tiles that are matching boolean expression."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.killempty", KillEmpty, "/killempty")
        {
            HelpText = "Deletes empty signs and/or chests (only entities, doesn't remove tiles)."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.move", Move, "/move")
        {
            HelpText = "Moves tiles from the worldedit selection to new area."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.mow", Mow, "/mow")
        {
            HelpText = "Mows grass, thorns, and vines in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.near", Near, "/near")
        {
            AllowServer = false,
            HelpText = "Sets the worldedit selection to a radius around you."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.outline", Outline, "/outline", "/ol")
        {
            HelpText = "Sets block outline around blocks in area."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.outlinewall", OutlineWall, "/outlinewall", "/olw")
        {
            HelpText = "Sets wall outline around walls in area."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.paint", Paint, "/paint", "/pa")
        {
            HelpText = "Paints tiles in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.paintwall", PaintWall, "/paintwall", "/paw")
        {
            HelpText = "Paints walls in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.paste", Paste, "/paste", "/p")
        {
            HelpText = "Pastes the clipboard to the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.spaste", SPaste, "/spaste", "/sp")
        {
            HelpText = "Pastes the clipboard to the worldedit selection with certain conditions."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.point", Point1, "/point1", "/p1", "p1")
        {
            HelpText = "Sets the positions of the worldedit selection's first point."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.point", Point2, "/point2", "/p2", "p2")
        {
            HelpText = "Sets the positions of the worldedit selection's second point."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.history.redo", Redo, "/redo")
        {
            HelpText = "Redoes a number of worldedit actions."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.region", RegionCmd, "/region")
        {
            HelpText = "Selects a region as a worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.replace", Replace, "/replace", "/rep")
        {
            HelpText = "Replaces tiles in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.replacewall", ReplaceWall, "/replacewall", "/repw")
        {
            HelpText = "Replaces walls in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.resize", Resize, "/resize")
        {
            HelpText = "Resizes the worldedit selection in a direction."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.rotate", Rotate, "/rotate")
        {
            HelpText = "Rotates the worldedit clipboard."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.schematic", Schematic, "/schematic", "/schem", "/sc", "sc")
        {
            HelpText = "Manages worldedit schematics."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.selecttype", Select, "/select")
        {
            HelpText = "Sets the worldedit selection function."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.set", Set, "/set")
        {
            HelpText = "Sets tiles in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.setgrass", SetGrass, "/setgrass")
        {
            HelpText = "Sets certain grass in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.setwall", SetWall, "/setwall", "/swa")
        {
            HelpText = "Sets walls in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.setwire", SetWire, "/setwire", "/swi")
        {
            HelpText = "Sets wires in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.shape", Shape, "/shape")
        {
            HelpText = "Draws line/rectangle/ellipse/triangle in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.shape", Shape, "/shapefill", "/shapef")
        {
            HelpText = "Draws line/rectangle/ellipse/triangle in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.shape", Shape, "/shapewall", "/shapew")
        {
            HelpText = "Draws line/rectangle/ellipse/triangle in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.shape", Shape, "/shapewallfill", "/shapewf")
        {
            HelpText = "Draws line/rectangle/ellipse/triangle in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.utils.size", Size, "/size")
        {
            HelpText = "Shows size of clipboard or schematic."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.slope", Slope, "/slope")
        {
            HelpText = "Slopes tiles in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.delslope", SlopeDelete, "/delslope", "/delslopes", "/dslope", "/dslopes")
        {
            HelpText = "Removes slopes in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.smooth", Smooth, "/smooth")
        {
            HelpText = "Smooths blocks in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.inactive", Inactive, "/inactive", "/ia")
        {
            HelpText = "Sets the inactive status in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.shift", Shift, "/shift")
        {
            HelpText = "Shifts the worldedit selection in a direction."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.selection.text", Text, "/text")
        {
            HelpText = "Creates text with alphabet statues in the worldedit selection."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.history.undo", Undo, "/undo")
        {
            HelpText = "Undoes a number of worldedit actions."
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.clipboard.scale", Scale, "/scale")
        {
            HelpText = "Scale the clipboard"
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("worldedit.region.actuator", Actuator, "/actuator")
        {
            HelpText = "Sets actuators in the worldedit selection."
        });
        string text = TShock.Config.Settings.StorageType.ToLowerInvariant();
        string text2 = text;
        if (!(text2 == "mysql"))
        {
            if (text2 == "sqlite")
            {
                string arg = Path.Combine(TShock.SavePath, "worldedit.sqlite");
                Database = new SqliteConnection($"Data Source={arg}");
            }
        }
        else
        {
            string[] hostParts = TShock.Config.Settings.MySqlHost.Split(':');
            string server = hostParts[0];
            string port = hostParts.Length == 1 ? "3306" : hostParts[1];

            var connectionString = $"Server={server};Port={port};Database={TShock.Config.Settings.MySqlDbName};" +
                                   $"Uid={TShock.Config.Settings.MySqlUsername};Pwd={TShock.Config.Settings.MySqlPassword}";

            Database = new MySqlConnection(connectionString);
        }
        if (!File.Exists(path))
        {
            Database.Query("DROP TABLE WorldEdit");
            foreach (string item in Directory.EnumerateFiles("worldedit", "undo-*.dat"))
            {
                File.Delete(item);
            }
            foreach (string item2 in Directory.EnumerateFiles("worldedit", "redo-*.dat"))
            {
                File.Delete(item2);
            }
            foreach (string item3 in Directory.EnumerateFiles("worldedit", "clipboard-*.dat"))
            {
                File.Delete(item3);
            }
            File.Create(path).Close();
            TShock.Log.ConsoleInfo("WorldEdit doesn't support undo/redo/clipboard files that were saved by plugin below version 1.7.");
            TShock.Log.ConsoleInfo("These files had been deleted. However, we still support old schematic files (*.dat)");
            TShock.Log.ConsoleInfo("Do not delete deteted.lock inside worldedit folder; this message will only show once.");
        }
        IDbConnection database = Database;
        SqlTableCreator sqlTableCreator = new SqlTableCreator(database, Database.GetSqlQueryBuilder());
        sqlTableCreator.EnsureTableStructure(new SqlTable("WorldEdit",
            new SqlColumn("Account", (MySqlDbType) 3) { Primary = true },
            new SqlColumn("RedoLevel", (MySqlDbType) 3),
            new SqlColumn("UndoLevel", (MySqlDbType) 3)));
        Biomes.Add("crimson", new Crimson());
        Biomes.Add("corruption", new Corruption());
        Biomes.Add("hallow", new Hallow());
        Biomes.Add("jungle", new Jungle());
        Biomes.Add("mushroom", new Mushroom());
        Biomes.Add("normal", new Forest());
        Biomes.Add("forest", new Forest());
        Biomes.Add("snow", new Snow());
        Biomes.Add("ice", new Snow());
        Biomes.Add("desert", new Desert());
        Biomes.Add("sand", new Desert());
        Biomes.Add("hell", new Hell());
        Colors.Add("blank", 0);
        Main.player[Main.myPlayer] = new Player();
        Item val3 = new Item();
        for (int k = 1; k < 5456; k++)
        {
            val3.netDefaults(k);
            if (val3.paint > 0)
            {
                string itemNameById = EnglishLanguage.GetItemNameById(k);
                Colors.Add(itemNameById.Substring(0, itemNameById.Length - 6).ToLowerInvariant(), val3.paint);
            }
        }
        Selections.Add("altcheckers", (int i, int j, TSPlayer plr) => ((i + j) & 1) == 0);
        Selections.Add("checkers", (int i, int j, TSPlayer plr) => ((i + j) & 1) == 1);
        Selections.Add("ellipse", delegate (int i, int j, TSPlayer plr)
        {
            PlayerInfo playerInfo2 = plr.GetPlayerInfo();
            return Tools.InEllipse(Math.Min(playerInfo2.X, playerInfo2.X2), Math.Min(playerInfo2.Y, playerInfo2.Y2), Math.Max(playerInfo2.X, playerInfo2.X2), Math.Max(playerInfo2.Y, playerInfo2.Y2), i, j);
        });
        Selections.Add("normal", (int i, int j, TSPlayer plr) => true);
        Selections.Add("border", delegate (int i, int j, TSPlayer plr)
        {
            PlayerInfo playerInfo = plr.GetPlayerInfo();
            return i == playerInfo.X || i == playerInfo.X2 || j == playerInfo.Y || j == playerInfo.Y2;
        });
        Selections.Add("outline", (int i, int j, TSPlayer plr) => i > 0 && j > 0 && i < Main.maxTilesX - 1 && j < Main.maxTilesY - 1 && Main.tile[i, j].active() && (!Main.tile[i - 1, j].active() || !Main.tile[i, j - 1].active() || !Main.tile[i + 1, j].active() || !Main.tile[i, j + 1].active() || !Main.tile[i + 1, j + 1].active() || !Main.tile[i - 1, j - 1].active() || !Main.tile[i - 1, j + 1].active() || !Main.tile[i + 1, j - 1].active()));
        Selections.Add("random", (int i, int j, TSPlayer plr) => Main.rand.NextDouble() >= 0.5);
        Tiles.Add("air", -1);
        Tiles.Add("lava", -2);
        Tiles.Add("honey", -3);
        Tiles.Add("water", -4);
        FieldInfo[] fields = typeof(TileID).GetFields();
        foreach (FieldInfo fieldInfo in fields)
        {
            if (fieldInfo.FieldType != typeof(ushort) || !fieldInfo.IsLiteral || fieldInfo.Name == "Count")
            {
                continue;
            }
            string name = fieldInfo.Name;
            StringBuilder stringBuilder = new StringBuilder();
            for (int m = 0; m < name.Length; m++)
            {
                if (char.IsUpper(name[m]))
                {
                    stringBuilder.Append(" ").Append(char.ToLower(name[m]));
                }
                else
                {
                    stringBuilder.Append(name[m]);
                }
            }
            Tiles.Add(stringBuilder.ToString(1, stringBuilder.Length - 1), (ushort) fieldInfo.GetValue(null));
        }
        Walls.Add("air", 0);
        FieldInfo[] fields2 = typeof(WallID).GetFields();
        foreach (FieldInfo fieldInfo2 in fields2)
        {
            if (fieldInfo2.FieldType != typeof(ushort) || !fieldInfo2.IsLiteral || fieldInfo2.Name == "None" || fieldInfo2.Name == "Count")
            {
                continue;
            }
            string name2 = fieldInfo2.Name;
            StringBuilder stringBuilder2 = new StringBuilder();
            for (int num = 0; num < name2.Length; num++)
            {
                if (char.IsUpper(name2[num]))
                {
                    stringBuilder2.Append(" ").Append(char.ToLower(name2[num]));
                }
                else
                {
                    stringBuilder2.Append(name2[num]);
                }
            }
            Walls.Add(stringBuilder2.ToString(1, stringBuilder2.Length - 1), (ushort) fieldInfo2.GetValue(null));
        }
        Slopes.Add("none", 0);
        Slopes.Add("t", 1);
        Slopes.Add("tr", 2);
        Slopes.Add("ur", 2);
        Slopes.Add("tl", 3);
        Slopes.Add("ul", 3);
        Slopes.Add("br", 4);
        Slopes.Add("dr", 4);
        Slopes.Add("bl", 5);
        Slopes.Add("dl", 5);
        ThreadPool.QueueUserWorkItem(QueueCallback);
    }

    private void QueueCallback(object context)
    {
        while (!Netplay.Disconnect)
        {
            WECommand item = null;
            try
            {
                if (!_commandQueue.TryTake(out item, -1, _cancel.Token))
                {
                    break;
                }
                if (Main.rand == null)
                {
                    Main.rand = new UnifiedRandom();
                }
                item.Position();
                item.Execute();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex2)
            {
                TShock.Log.ConsoleError(ex2.ToString());
                TSPlayer tSPlayer = item?.plr;
                if (tSPlayer != null && tSPlayer.Active)
                {
                    tSPlayer.SendErrorMessage("WorldEdit command failed, check logs for more details.");
                }
            }
        }
    }

    private void EditConfig(CommandArgs e)
    {
        if (e.Parameters.Count < 1 || e.Parameters.Count > 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //worldedit <option> [value]");
            e.Player.SendInfoMessage("Config options: MagicWandTileLimit (wand), MaxUndoCount (undocount),\nDisableUndoSystemForUnrealPlayers (undodisable), StartSchematicNamesWithCreatorUserID (schematic)");
            return;
        }
        switch (e.Parameters.ElementAtOrDefault(0).ToLower())
        {
            case "wand":
            case "magicwandtilelimit":
            {
                int result2;
                if (e.Parameters.Count == 1)
                {
                    e.Player.SendSuccessMessage($"Magic wand tile limit is {Config.MagicWandTileLimit}.");
                }
                else if (!int.TryParse(e.Parameters[1], out result2) || result2 < 0)
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //worldedit <magicwandtilelimit/wand> <amount>");
                }
                else
                {
                    Config.MagicWandTileLimit = result2;
                    Config.Write(ConfigPath);
                    e.Player.SendSuccessMessage($"Magic wand tile limit set to {result2}.");
                }
                break;
            }
            case "undocount":
            case "maxundocount":
            {
                int result4;
                if (e.Parameters.Count == 1)
                {
                    e.Player.SendSuccessMessage($"Max undo count is {Config.MaxUndoCount}.");
                }
                else if (!int.TryParse(e.Parameters[1], out result4) || result4 < 0)
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //worldedit <maxundocount/undocount> <amount>");
                }
                else
                {
                    Config.MaxUndoCount = result4;
                    Config.Write(ConfigPath);
                    e.Player.SendSuccessMessage($"Max undo count set to {result4}.");
                }
                break;
            }
            case "undodisable":
            case "disableundosystemforunrealplayers":
            {
                bool result3;
                if (e.Parameters.Count == 1)
                {
                    e.Player.SendSuccessMessage($"Disable undo system for unreal players is {Config.DisableUndoSystemForUnrealPlayers}.");
                }
                else if (!bool.TryParse(e.Parameters[1], out result3))
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //worldedit <disableundosystemforunrealplayers/undodisable> <true/false>");
                }
                else
                {
                    Config.DisableUndoSystemForUnrealPlayers = result3;
                    Config.Write(ConfigPath);
                    e.Player.SendSuccessMessage($"Disable undo system for unreal players set to {result3}.");
                }
                break;
            }
            case "schematic":
            case "startschematicnameswithcreatoruserid":
            {
                bool result;
                if (e.Parameters.Count == 1)
                {
                    e.Player.SendSuccessMessage($"Start schematic names with creator user id is {Config.StartSchematicNamesWithCreatorUserID}.");
                }
                else if (!bool.TryParse(e.Parameters[1], out result))
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //worldedit <startschematicnameswithcreatoruserid/schematic> <true/false>");
                }
                else
                {
                    Config.StartSchematicNamesWithCreatorUserID = result;
                    Config.Write(ConfigPath);
                    e.Player.SendSuccessMessage($"Start schematic names with creator user id set to {result}.");
                }
                break;
            }
            default:
                e.Player.SendErrorMessage("Config options: MagicWandTileLimit (wand), MaxUndoCount (undocount),\nDisableUndoSystemForUnrealPlayers (undodisable), StartSchematicNamesWithCreatorUserID (schematic)");
                break;
        }
    }

    private void Activate(CommandArgs e)
    {
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //activate <sign/chest/itemframe/sensor/dummy/all>");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
            return;
        }
        byte action;
        switch (e.Parameters[0].ToLowerInvariant())
        {
            case "s":
            case "sign":
                action = 0;
                break;
            case "c":
            case "chest":
                action = 1;
                break;
            case "i":
            case "item":
            case "frame":
            case "itemframe":
                action = 2;
                break;
            case "l":
            case "logic":
            case "sensor":
            case "logicsensor":
                action = 3;
                break;
            case "d":
            case "dummy":
            case "targetdummy":
                action = 4;
                break;
            case "a":
            case "all":
                action = byte.MaxValue;
                break;
            default:
                e.Player.SendErrorMessage("Invalid activation type '{0}'.", e.Parameters[0]);
                return;
        }
        _commandQueue.Add(new Activate(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, action));
    }

    private void Actuator(CommandArgs e)
    {
        string text = ((e.Parameters.Count == 0) ? "" : e.Parameters[0].ToLowerInvariant());
        if (text != "off" && text != "on")
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //actuator <on/off> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
            return;
        }
        bool remove = text == "off";
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Actuator(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, expression, remove));
        }
    }

    private void All(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        int x = (playerInfo.Y = 0);
        playerInfo.X = x;
        playerInfo.X2 = Main.maxTilesX - 1;
        playerInfo.Y2 = Main.maxTilesY - 1;
        e.Player.SendSuccessMessage("Selected all tiles.");
    }

    private void Biome(CommandArgs e)
    {
        if (e.Parameters.Count != 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //biome <biome 1> <biome 2>");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
            return;
        }
        string text = e.Parameters[0].ToLowerInvariant();
        string text2 = e.Parameters[1].ToLowerInvariant();
        if (!Biomes.ContainsKey(text) || !Biomes.ContainsKey(text2))
        {
            e.Player.SendErrorMessage("Invalid biome.");
        }
        else
        {
            _commandQueue.Add(new Commands.Biome(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, text, text2));
        }
    }

    private void Copy(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new Copy(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, null));
        }
    }

    private void Cut(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
        }
        else
        {
            _commandQueue.Add(new Cut(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void Drain(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
        }
        else
        {
            _commandQueue.Add(new Drain(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void Fill(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
            return;
        }
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("//fill <tile> [=> boolean expr...]");
            return;
        }
        List<int> tileID = Tools.GetTileID(e.Parameters[0].ToLowerInvariant());
        if (tileID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[0]);
            return;
        }
        if (tileID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        Expression expression;
        if (e.Parameters.Count > 1)
        {
            if (!Parser.TryParseTree(e.Parameters.Skip(1), out expression))
            {
                e.Player.SendErrorMessage("Invalid expression!");
                return;
            }
        }
        else
        {
            Parser.TryParseTree(new[] { "=>", "!t" }, out expression);
        }
        _commandQueue.Add(new Set(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, tileID[0], expression));
    }

    private void FillWall(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection.");
            return;
        }
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("//fill <tile> [=> boolean expr...]");
            return;
        }
        List<int> wallID = Tools.GetWallID(e.Parameters[0].ToLowerInvariant());
        if (wallID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid wall '{0}'!", e.Parameters[0]);
            return;
        }
        if (wallID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one wall matched!");
            return;
        }
        Expression expression;
        if (e.Parameters.Count > 1)
        {
            if (!Parser.TryParseTree(e.Parameters.Skip(1), out expression))
            {
                e.Player.SendErrorMessage("Invalid expression!");
                return;
            }
        }
        else
        {
            Parser.TryParseTree(new[] { "=>", "!w" }, out expression);
        }
        _commandQueue.Add(new SetWall(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, wallID[0], expression));
    }

    private void FixGhosts(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new FixGhosts(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void FixGrass(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new FixGrass(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void FixHalves(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new FixHalves(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void FixSlopes(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new FixSlopes(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void Flood(CommandArgs e)
    {
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //flood <liquid>");
            return;
        }
        int liquid = 0;
        if (string.Equals(e.Parameters[0], "lava", StringComparison.OrdinalIgnoreCase))
        {
            liquid = 1;
        }
        else if (string.Equals(e.Parameters[0], "honey", StringComparison.OrdinalIgnoreCase))
        {
            liquid = 2;
        }
        else if (!string.Equals(e.Parameters[0], "water", StringComparison.OrdinalIgnoreCase))
        {
            e.Player.SendErrorMessage("Invalid liquid type '{0}'!", e.Parameters[0]);
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        _commandQueue.Add(new Flood(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, liquid));
    }

    private void Flip(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //flip <direction>");
            return;
        }
        if (!Tools.HasClipboard(e.Player.Account.ID))
        {
            e.Player.SendErrorMessage("Invalid clipboard!");
            return;
        }
        bool flag = false;
        bool flag2 = false;
        string text = e.Parameters[0].ToLowerInvariant();
        foreach (char c in text)
        {
            switch (c)
            {
                case 'x':
                    flag = !flag;
                    break;
                case 'y':
                    flag2 = !flag2;
                    break;
                default:
                    e.Player.SendErrorMessage("Invalid direction '{0}'!", c);
                    return;
            }
        }
        _commandQueue.Add(new Flip(e.Player, flag, flag2));
    }

    private void MagicWandTool(CommandArgs e)
    {
        string msg = "Invalid syntax! Proper syntax: //magicwand [<X> <Y>] => boolean expr...";
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage(msg);
            return;
        }
        int count = 0;
        int result = -1;
        int result2 = -1;
        if (e.Parameters[0] != "=>")
        {
            if (e.Parameters.Count < 4)
            {
                e.Player.SendErrorMessage(msg);
                return;
            }
            if (!int.TryParse(e.Parameters[0], out result) || !int.TryParse(e.Parameters[1], out result2) || result < 0 || result2 < 0 || result >= Main.maxTilesX || result2 >= Main.maxTilesY)
            {
                e.Player.SendErrorMessage(msg);
                return;
            }
            count = 2;
        }
        if (!Parser.TryParseTree(e.Parameters.Skip(count), out var expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (result != -1 && result2 != -1)
        {
            if (!MagicWand.GetMagicWandSelection(result, result2, expression, e.Player, out var magicWand))
            {
                e.Player.SendErrorMessage("Can't start counting magic wand selection from this tile.");
            }
            else
            {
                playerInfo.MagicWand = magicWand;
                e.Player.SendSuccessMessage("Set magic wand selection.");
            }
            playerInfo.SavedExpression = null;
        }
        else
        {
            playerInfo.SavedExpression = expression;
            playerInfo.Point = 4;
            e.Player.SendInfoMessage("Modify a block to count hard selection.");
        }
    }

    private void KillEmpty(CommandArgs e)
    {
        byte action;
        switch (e.Parameters.ElementAtOrDefault(0)?.ToLower())
        {
            case "s":
            case "sign":
            case "signs":
                action = 0;
                break;
            case "c":
            case "chest":
            case "chests":
                action = 1;
                break;
            case "a":
            case "all":
                action = byte.MaxValue;
                break;
            default:
                e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //killempty <signs/chests/all>");
                return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        _commandQueue.Add(new KillEmpty(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, action));
    }

    private void Move(CommandArgs e)
    {
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //move <right> <down> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (!int.TryParse(e.Parameters[0], out var result) || !int.TryParse(e.Parameters[1], out var result2))
        {
            e.Player.SendErrorMessage("Invalid distance!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 2 && !Parser.TryParseTree(e.Parameters.Skip(2), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Move(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, result2, result, expression));
        }
    }

    private void Mow(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new Mow(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player));
        }
    }

    private void Near(CommandArgs e)
    {
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //near <radius>");
            return;
        }
        if (!int.TryParse(e.Parameters[0], out var result) || result <= 0)
        {
            e.Player.SendErrorMessage("Invalid radius '{0}'!", e.Parameters[0]);
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        playerInfo.X = e.Player.TileX - result;
        playerInfo.X2 = e.Player.TileX + result + 1;
        playerInfo.Y = e.Player.TileY - result;
        playerInfo.Y2 = e.Player.TileY + result + 2;
        e.Player.SendSuccessMessage("Selected tiles around you!");
    }

    private void Outline(CommandArgs e)
    {
        if (e.Parameters.Count < 3)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //outline <tile> <color> <state> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> colorID = Tools.GetColorID(e.Parameters[1].ToLowerInvariant());
        if (colorID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid color '{0}'!", e.Parameters[0]);
            return;
        }
        if (colorID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one color matched!");
            return;
        }
        bool active = false;
        if (string.Equals(e.Parameters[2], "active", StringComparison.OrdinalIgnoreCase))
        {
            active = true;
        }
        else if (string.Equals(e.Parameters[2], "a", StringComparison.OrdinalIgnoreCase))
        {
            active = true;
        }
        else if (string.Equals(e.Parameters[2], "na", StringComparison.OrdinalIgnoreCase))
        {
            active = false;
        }
        else if (!string.Equals(e.Parameters[2], "nactive", StringComparison.OrdinalIgnoreCase))
        {
            e.Player.SendErrorMessage("Invalid active state '{0}'!", e.Parameters[1]);
            return;
        }
        List<int> tileID = Tools.GetTileID(e.Parameters[0].ToLowerInvariant());
        if (tileID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[0]);
            return;
        }
        if (tileID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 3 && !Parser.TryParseTree(e.Parameters.Skip(3), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Outline(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, tileID[0], colorID[0], active, expression));
        }
    }

    private void OutlineWall(CommandArgs e)
    {
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //outlinewall <wall> [color] [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> colorID = Tools.GetColorID(e.Parameters[1].ToLowerInvariant());
        if (colorID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid color '{0}'!", e.Parameters[0]);
            return;
        }
        if (colorID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one color matched!");
            return;
        }
        List<int> wallID = Tools.GetWallID(e.Parameters[0].ToLowerInvariant());
        if (wallID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid wall '{0}'!", e.Parameters[0]);
            return;
        }
        if (wallID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one wall matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 2 && !Parser.TryParseTree(e.Parameters.Skip(2), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new OutlineWall(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, wallID[0], colorID[0], expression));
        }
    }

    private void Paint(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //paint <color> [where] [conditions...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> colorID = Tools.GetColorID(e.Parameters[0].ToLowerInvariant());
        if (colorID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid color '{0}'!", e.Parameters[0]);
            return;
        }
        if (colorID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one color matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Paint(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, colorID[0], expression));
        }
    }

    private void PaintWall(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //paintwall <color> [where] [conditions...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> colorID = Tools.GetColorID(e.Parameters[0].ToLowerInvariant());
        if (colorID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid color '{0}'!", e.Parameters[0]);
            return;
        }
        if (colorID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one color matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new PaintWall(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, colorID[0], expression));
        }
    }

    private void Paste(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        e.Player.SendInfoMessage("X: {0}, Y: {1}", playerInfo.X, playerInfo.Y);
        if (playerInfo.X == -1 || playerInfo.Y == -1)
        {
            e.Player.SendErrorMessage("Invalid first point!");
            return;
        }
        if (!Tools.HasClipboard(e.Player.Account.ID))
        {
            e.Player.SendErrorMessage("Invalid clipboard!");
            return;
        }
        int num = 0;
        bool mode_MainBlocks = true;
        Expression expression = null;
        int num2 = 0;
        if (e.Parameters.Count > num2)
        {
            if (!e.Parameters[num2].ToLowerInvariant().StartsWith("-") && !e.Parameters[num2].ToLowerInvariant().StartsWith("="))
            {
                string text = e.Parameters[0].ToLowerInvariant();
                foreach (char c in text)
                {
                    switch (c)
                    {
                        case 'l':
                            num &= 2;
                            break;
                        case 'r':
                            num |= 1;
                            break;
                        case 't':
                            num &= 1;
                            break;
                        case 'b':
                            num |= 2;
                            break;
                        default:
                            e.Player.SendErrorMessage("Invalid paste alignment '{0}'!", c);
                            return;
                    }
                }
                num2++;
            }
            if (e.Parameters.Count > num2 && (e.Parameters[num2].ToLowerInvariant() == "-f" || e.Parameters[num2].ToLowerInvariant() == "-file"))
            {
                mode_MainBlocks = false;
                num2++;
            }
            if (e.Parameters.Count > num2 && !Parser.TryParseTree(e.Parameters.Skip(num2), out expression))
            {
                e.Player.SendErrorMessage("Invalid expression!");
                return;
            }
        }
        _commandQueue.Add(new Paste(playerInfo.X, playerInfo.Y, e.Player, Tools.GetClipboardPath(e.Player.Account.ID), num, expression, mode_MainBlocks, prepareUndo: true));
    }

    private void SPaste(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        e.Player.SendInfoMessage("X: {0}, Y: {1}", playerInfo.X, playerInfo.Y);
        if (playerInfo.X == -1 || playerInfo.Y == -1)
        {
            e.Player.SendErrorMessage("Invalid first point!");
            return;
        }
        if (!Tools.HasClipboard(e.Player.Account.ID))
        {
            e.Player.SendErrorMessage("Invalid clipboard!");
            return;
        }
        if (e.Parameters.Count < 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //spaste [alignment] [-flag -flag ...] [=> boolean expr...]");
            return;
        }
        int num = 0;
        Expression expression = null;
        int i = 0;
        bool tiles = true;
        bool tilePaints = true;
        bool emptyTiles = true;
        bool walls = true;
        bool wallPaints = true;
        bool wires = true;
        bool liquids = true;
        if (e.Parameters.Count > i)
        {
            if (!e.Parameters[i].ToLowerInvariant().StartsWith("-"))
            {
                string text = e.Parameters[i].ToLowerInvariant();
                foreach (char c in text)
                {
                    switch (c)
                    {
                        case 'l':
                            num &= 2;
                            break;
                        case 'r':
                            num |= 1;
                            break;
                        case 't':
                            num &= 1;
                            break;
                        case 'b':
                            num |= 2;
                            break;
                        default:
                            e.Player.SendErrorMessage("Invalid paste alignment '{0}'!", c);
                            return;
                    }
                }
                i++;
            }
            List<string> list = new List<string>();
            for (; e.Parameters.Count > i && e.Parameters[i] != "=>"; i++)
            {
                switch (e.Parameters[i].ToLower())
                {
                    case "-t":
                        tiles = false;
                        break;
                    case "-tp":
                        tilePaints = false;
                        break;
                    case "-et":
                        emptyTiles = false;
                        break;
                    case "-w":
                        walls = false;
                        break;
                    case "-wp":
                        wallPaints = false;
                        break;
                    case "-wi":
                        wires = false;
                        break;
                    case "-l":
                        liquids = false;
                        break;
                    default:
                        list.Add(e.Parameters[i]);
                        break;
                }
            }
            if (e.Parameters.Count > i && !Parser.TryParseTree(e.Parameters.Skip(i), out expression))
            {
                e.Player.SendErrorMessage("Invalid expression!");
                return;
            }
        }
        _commandQueue.Add(new SPaste(playerInfo.X, playerInfo.Y, e.Player, num, expression, tiles, tilePaints, emptyTiles, walls, wallPaints, wires, liquids));
    }

    private void Point1(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        int result;
        int result2;
        if (e.Parameters.Count == 0)
        {
            if (!e.Player.RealPlayer)
            {
                e.Player.SendErrorMessage("You must use this command in-game.");
                return;
            }
            playerInfo.Point = 1;
            e.Player.SendInfoMessage("Modify a block to set point 1.");
        }
        else if (e.Parameters.Count != 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //point1 <x> <y>");
        }
        else if (!int.TryParse(e.Parameters[0], out result) || result < 0 || result >= Main.maxTilesX || !int.TryParse(e.Parameters[1], out result2) || result2 < 0 || result2 >= Main.maxTilesY)
        {
            e.Player.SendErrorMessage("Invalid coordinates.");
        }
        else
        {
            playerInfo.X = result;
            playerInfo.Y = result2;
            e.Player.SendInfoMessage("Set point 1.");
        }
    }

    private void Point2(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        int result;
        int result2;
        if (e.Parameters.Count == 0)
        {
            if (!e.Player.RealPlayer)
            {
                e.Player.SendErrorMessage("You must use this command in-game.");
                return;
            }
            playerInfo.Point = 2;
            e.Player.SendInfoMessage("Modify a block to set point 2.");
        }
        else if (e.Parameters.Count != 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //point2 [x] [y]");
        }
        else if (!int.TryParse(e.Parameters[0], out result) || result < 0 || result >= Main.maxTilesX || !int.TryParse(e.Parameters[1], out result2) || result2 < 0 || result2 >= Main.maxTilesY)
        {
            e.Player.SendErrorMessage("Invalid coordinates '({0}, {1})'!", e.Parameters[0], e.Parameters[1]);
        }
        else
        {
            playerInfo.X2 = result;
            playerInfo.Y2 = result2;
            e.Player.SendInfoMessage("Set point 2.");
        }
    }

    private void Redo(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        if (e.Parameters.Count > 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //redo [steps] [account]");
            return;
        }
        int result = 1;
        int iD = e.Player.Account.ID;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out result) || result <= 0))
        {
            e.Player.SendErrorMessage("Invalid redo steps '{0}'!", e.Parameters[0]);
        }
        else if (e.Parameters.Count > 1)
        {
            if (!e.Player.HasPermission("worldedit.usage.otheraccounts"))
            {
                e.Player.SendErrorMessage("You do not have permission to redo other player's actions.");
                return;
            }
            UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(e.Parameters[1]);
            if (userAccountByName == null)
            {
                e.Player.SendErrorMessage("Invalid account name!");
                return;
            }
            iD = userAccountByName.ID;
        }
        _commandQueue.Add(new Redo(e.Player, iD, result));
    }

    private void RegionCmd(CommandArgs e)
    {
        if (e.Parameters.Count > 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //region [region name]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (e.Parameters.Count == 0)
        {
            playerInfo.Point = 3;
            e.Player.SendInfoMessage("Hit a block to select that region.");
            return;
        }
        Region regionByName = TShock.Regions.GetRegionByName(e.Parameters[0]);
        if (regionByName == null)
        {
            e.Player.SendErrorMessage("Invalid region '{0}'!", e.Parameters[0]);
            return;
        }
        Rectangle area = regionByName.Area;
        playerInfo.X = area.Left;
        playerInfo.Y = area.Top;
        playerInfo.X2 = area.Right;
        playerInfo.Y2 = area.Bottom;
        e.Player.SendSuccessMessage("Set selection to region '{0}'.", regionByName.Name);
    }

    private void Replace(CommandArgs e)
    {
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //replace <from tile> <to tile> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> tileID = Tools.GetTileID(e.Parameters[0].ToLowerInvariant());
        if (tileID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[0]);
            return;
        }
        if (tileID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        List<int> tileID2 = Tools.GetTileID(e.Parameters[1].ToLowerInvariant());
        if (tileID2.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[1]);
            return;
        }
        if (tileID2.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 2 && !Parser.TryParseTree(e.Parameters.Skip(2), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Replace(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, tileID[0], tileID2[0], expression));
        }
    }

    private void ReplaceWall(CommandArgs e)
    {
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //replace <from tile> <to tile> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> tileID = Tools.GetTileID(e.Parameters[0].ToLowerInvariant());
        if (tileID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[0]);
            return;
        }
        if (tileID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        List<int> wallID = Tools.GetWallID(e.Parameters[1].ToLowerInvariant());
        if (wallID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid wall '{0}'!", e.Parameters[1]);
            return;
        }
        if (wallID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one wall matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 2 && !Parser.TryParseTree(e.Parameters.Skip(2), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new ReplaceWall(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, tileID[0], wallID[0], expression));
        }
    }

    private void Resize(CommandArgs e)
    {
        if (e.Parameters.Count != 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //resize <direction(s)> <amount>");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (!int.TryParse(e.Parameters[1], out var result))
        {
            e.Player.SendErrorMessage("Invalid resize amount '{0}'!", e.Parameters[0]);
            return;
        }
        string text = e.Parameters[0].ToLowerInvariant();
        foreach (char c in text)
        {
            switch (c)
            {
                case 'd':
                    if (playerInfo.Y < playerInfo.Y2)
                    {
                        playerInfo.Y2 += result;
                    }
                    else
                    {
                        playerInfo.Y += result;
                    }
                    break;
                case 'l':
                    if (playerInfo.X < playerInfo.X2)
                    {
                        playerInfo.X -= result;
                    }
                    else
                    {
                        playerInfo.X2 -= result;
                    }
                    break;
                case 'r':
                    if (playerInfo.X < playerInfo.X2)
                    {
                        playerInfo.X2 += result;
                    }
                    else
                    {
                        playerInfo.X += result;
                    }
                    break;
                case 'u':
                    if (playerInfo.Y < playerInfo.Y2)
                    {
                        playerInfo.Y -= result;
                    }
                    else
                    {
                        playerInfo.Y2 -= result;
                    }
                    break;
                default:
                    e.Player.SendErrorMessage("Invalid direction '{0}'!", c);
                    return;
            }
        }
        e.Player.SendSuccessMessage("Resized selection.");
    }

    private void Rotate(CommandArgs e)
    {
        int result;
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
        }
        else if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //rotate <angle>");
        }
        else if (!Tools.HasClipboard(e.Player.Account.ID))
        {
            e.Player.SendErrorMessage("Invalid clipboard!");
        }
        else if (!int.TryParse(e.Parameters[0], out result) || result % 90 != 0)
        {
            e.Player.SendErrorMessage("Invalid angle '{0}'!", e.Parameters[0]);
        }
        else
        {
            _commandQueue.Add(new Rotate(e.Player, result));
        }
    }

    private void Scale(CommandArgs e)
    {
        int result;
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
        }
        else if (e.Parameters.Count != 2 || (e.Parameters[0] != "+" && e.Parameters[0] != "-"))
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //scale <+/-> <amount>");
        }
        else if (!Tools.HasClipboard(e.Player.Account.ID))
        {
            e.Player.SendErrorMessage("Invalid clipboard!");
        }
        else if (!int.TryParse(e.Parameters[1], out result))
        {
            e.Player.SendErrorMessage("Invalid amount!");
        }
        else
        {
            _commandQueue.Add(new Scale(e.Player, e.Parameters[0] == "+", result));
        }
    }

    private void Schematic(CommandArgs e)
    {
        switch ((e.Parameters.Count == 0) ? "help" : e.Parameters[0].ToLowerInvariant())
        {
            case "del":
            case "delete":
            {
                if (!e.Player.HasPermission("worldedit.schematic.delete"))
                {
                    e.Player.SendErrorMessage("You do not have permission to delete schematics.");
                    break;
                }
                if (e.Parameters.Count != 3 || e.Parameters[1].ToLower() != "-confirm")
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic delete -confirm <name>");
                    break;
                }
                string path = Path.Combine(Config.SchematicFolderPath, $"schematic-{e.Parameters[2]}.dat");
                if (!File.Exists(path))
                {
                    e.Player.SendErrorMessage("Invalid schematic '{0}'!", e.Parameters[2]);
                }
                else
                {
                    File.Delete(path);
                    e.Player.SendErrorMessage("Deleted schematic '{0}'.", e.Parameters[2]);
                }
                break;
            }
            case "list":
            {
                int pageNumber;
                if (e.Parameters.Count > 2)
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic list [page]");
                }
                else if (PaginationTools.TryParsePageNumber(e.Parameters, 1, e.Player, out pageNumber))
                {
                    IEnumerable<string> terms = from s in Directory.EnumerateFiles(Config.SchematicFolderPath, string.Format("schematic-{0}.dat", "*"))
                                                select Path.GetFileNameWithoutExtension(s).Substring(10);
                    PaginationTools.SendPage(e.Player, pageNumber, PaginationTools.BuildLinesFromTerms(terms), new PaginationTools.Settings
                    {
                        HeaderFormat = "Schematics ({0}/{1}):",
                        FooterFormat = "Type //schematic list {0} for more."
                    });
                }
                break;
            }
            case "l":
            case "load":
            {
                if (e.Player.Account == null)
                {
                    e.Player.SendErrorMessage("You have to be logged in to use this command.");
                    break;
                }
                if (e.Parameters.Count != 2)
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic load <name>");
                    break;
                }
                string text5 = Path.Combine(Config.SchematicFolderPath, $"schematic-{e.Parameters[1]}.dat");
                string clipboardPath2 = Tools.GetClipboardPath(e.Player.Account.ID);
                if (File.Exists(text5))
                {
                    File.Copy(text5, clipboardPath2, overwrite: true);
                    e.Player.SendSuccessMessage("Loaded schematic '{0}' to clipboard.", e.Parameters[1]);
                }
                else
                {
                    e.Player.SendErrorMessage("Invalid schematic '{0}'!", e.Parameters[1]);
                }
                break;
            }
            case "s":
            case "save":
            {
                if (e.Player.Account == null)
                {
                    e.Player.SendErrorMessage("You have to be logged in to use this command.");
                    break;
                }
                if (!e.Player.HasPermission("worldedit.schematic.save"))
                {
                    e.Player.SendErrorMessage("You do not have permission to save schematics.");
                    break;
                }
                if (Config.StartSchematicNamesWithCreatorUserID && e.Parameters.ElementAtOrDefault(1)?.ToLower() == "id")
                {
                    string text = ((e.Parameters.Count > 2) ? e.Parameters[2] : e.Player.Account.Name);
                    UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(text);
                    if (userAccountByName == null)
                    {
                        e.Player.SendErrorMessage("Invalid user '" + text + "'!");
                        break;
                    }
                    e.Player.SendSuccessMessage($"{userAccountByName.Name}'s ID: {userAccountByName.ID}.");
                    break;
                }
                string text2 = e.Parameters.ElementAtOrDefault(1)?.ToLower();
                bool flag = text2 == "-force" || text2 == "-f";
                string text3 = e.Parameters.ElementAtOrDefault((!flag) ? 1 : 2);
                if (string.IsNullOrWhiteSpace(text3))
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic save [-force/-f] <name>");
                    break;
                }
                string clipboardPath = Tools.GetClipboardPath(e.Player.Account.ID);
                if (!File.Exists(clipboardPath))
                {
                    e.Player.SendErrorMessage("Invalid clipboard!");
                    break;
                }
                if (!Tools.IsCorrectName(text3))
                {
                    e.Player.SendErrorMessage("Name should not contain these symbols: \"{0}\".", string.Join("\", \"", Path.GetInvalidFileNameChars()));
                    break;
                }
                if (Config.StartSchematicNamesWithCreatorUserID)
                {
                    text3 = $"{e.Player.Account.ID}-{text3}";
                }
                string text4 = Path.Combine(Config.SchematicFolderPath, $"schematic-{text3}.dat");
                if (File.Exists(text4))
                {
                    if (!e.Player.HasPermission("worldedit.schematic.overwrite"))
                    {
                        e.Player.SendErrorMessage("You do not have permission to overwrite schematics.");
                        break;
                    }
                    if (!flag)
                    {
                        e.Player.SendErrorMessage("Schematic '" + text3 + "' already exists, write '//schematic save <-force/-f> " + text3 + "' to overwrite it.");
                        break;
                    }
                }
                File.Copy(clipboardPath, text4, overwrite: true);
                e.Player.SendSuccessMessage("Saved clipboard to schematic '{0}'.", text3);
                break;
            }
            case "cs":
            case "copysave":
            {
                if (e.Player.Account == null)
                {
                    e.Player.SendErrorMessage("You have to be logged in to use this command.");
                    break;
                }
                if (!e.Player.HasPermission("worldedit.schematic.save"))
                {
                    e.Player.SendErrorMessage("You do not have permission to save schematics.");
                    break;
                }
                PlayerInfo playerInfo2 = e.Player.GetPlayerInfo();
                if (playerInfo2.X == -1 || playerInfo2.Y == -1 || playerInfo2.X2 == -1 || playerInfo2.Y2 == -1)
                {
                    e.Player.SendErrorMessage("Invalid selection!");
                    break;
                }
                string text7 = e.Parameters.ElementAtOrDefault(1)?.ToLower();
                bool flag2 = text7 == "-force" || text7 == "-f";
                string text8 = e.Parameters.ElementAtOrDefault((!flag2) ? 1 : 2);
                if (string.IsNullOrWhiteSpace(text8))
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic copysave [-force/-f] <name>");
                    break;
                }
                if (!Tools.IsCorrectName(text8))
                {
                    e.Player.SendErrorMessage("Name should not contain these symbols: \"{0}\".", string.Join("\", \"", Path.GetInvalidFileNameChars()));
                    break;
                }
                if (Config.StartSchematicNamesWithCreatorUserID)
                {
                    text8 = $"{e.Player.Account.ID}-{text8}";
                }
                string text9 = Path.Combine(Config.SchematicFolderPath, $"schematic-{text8}.dat");
                if (File.Exists(text9))
                {
                    if (!e.Player.HasPermission("worldedit.schematic.overwrite"))
                    {
                        e.Player.SendErrorMessage("You do not have permission to overwrite schematics.");
                        break;
                    }
                    if (!flag2)
                    {
                        e.Player.SendErrorMessage("Schematic '" + text8 + "' already exists, write '//schematic copysave <-force/-f> " + text8 + "' to overwrite it.");
                        break;
                    }
                }
                _commandQueue.Add(new Copy(playerInfo2.X, playerInfo2.Y, playerInfo2.X2, playerInfo2.Y2, e.Player, text9));
                break;
            }
            case "p":
            case "paste":
            {
                if (!e.Player.HasPermission("worldedit.schematic.paste"))
                {
                    e.Player.SendErrorMessage("//schematic paste is for server console only.\nInstead, you should use //schematic load and //paste.");
                    break;
                }
                if (e.Parameters.Count < 2)
                {
                    e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //schematic paste <name> [alignment] [-f] [=> boolean expr...]");
                    break;
                }
                string path2 = Path.Combine(Config.SchematicFolderPath, $"schematic-{e.Parameters[1]}.dat");
                if (!File.Exists(path2))
                {
                    e.Player.SendErrorMessage("Invalid schematic '{0}'!", e.Parameters[1]);
                    break;
                }
                PlayerInfo playerInfo = e.Player.GetPlayerInfo();
                if (playerInfo.X == -1 || playerInfo.Y == -1)
                {
                    e.Player.SendErrorMessage("Invalid first point!");
                    break;
                }
                int num = 0;
                bool mode_MainBlocks = true;
                Expression expression = null;
                int num2 = 2;
                if (e.Parameters.Count > num2)
                {
                    if (!e.Parameters[num2].ToLowerInvariant().StartsWith("-") && !e.Parameters[num2].ToLowerInvariant().StartsWith("="))
                    {
                        string text6 = e.Parameters[0].ToLowerInvariant();
                        foreach (char c in text6)
                        {
                            switch (c)
                            {
                                case 'l':
                                    num &= 2;
                                    break;
                                case 'r':
                                    num |= 1;
                                    break;
                                case 't':
                                    num &= 1;
                                    break;
                                case 'b':
                                    num |= 2;
                                    break;
                                default:
                                    e.Player.SendErrorMessage("Invalid paste alignment '{0}'!", c);
                                    return;
                            }
                        }
                        num2++;
                    }
                    if (e.Parameters.Count > num2 && (e.Parameters[num2].ToLowerInvariant() == "-f" || e.Parameters[num2].ToLowerInvariant() == "-file"))
                    {
                        mode_MainBlocks = false;
                        num2++;
                    }
                    if (e.Parameters.Count > num2 && !Parser.TryParseTree(e.Parameters.Skip(num2), out expression))
                    {
                        e.Player.SendErrorMessage("Invalid expression!");
                        break;
                    }
                }
                _commandQueue.Add(new Paste(playerInfo.X, playerInfo.Y, e.Player, path2, num, expression, mode_MainBlocks, prepareUndo: false));
                break;
            }
            default:
                e.Player.SendSuccessMessage("Schematics Subcommands:");
                e.Player.SendInfoMessage("/sc delete/del <name>\n/sc list [page]\n/sc load/l <name>\n/sc save/s <name>\n" + (Config.StartSchematicNamesWithCreatorUserID ? "/sc save/s id\n" : "") + "/sc copysave/cs <name>\n/sc paste/p <name> [alignment] [-f] [=> boolean expr...]");
                break;
        }
    }

    private void Select(CommandArgs e)
    {
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //select <selection type>");
            e.Player.SendInfoMessage("Available selections: " + string.Join(", ", Selections.Keys) + ".");
            return;
        }
        if (e.Parameters[0].ToLowerInvariant() == "help")
        {
            e.Player.SendInfoMessage("Proper syntax: //select <selection type>");
            e.Player.SendInfoMessage("Available selections: " + string.Join(", ", Selections.Keys) + ".");
            return;
        }
        string text = e.Parameters[0].ToLowerInvariant();
        if (!Selections.ContainsKey(text))
        {
            string text2 = "Available selections: " + string.Join(", ", Selections.Keys) + ".";
            e.Player.SendErrorMessage("Invalid selection type '{0}'!\r\n{1}", text, text2);
        }
        else
        {
            e.Player.GetPlayerInfo().Select = Selections[text];
            e.Player.SendSuccessMessage("Set selection type to '{0}'.", text);
        }
    }

    private void Set(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //set <tile> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> tileID = Tools.GetTileID(e.Parameters[0].ToLowerInvariant());
        if (tileID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[0]);
            return;
        }
        if (tileID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one tile matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Set(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, tileID[0], expression));
        }
    }

    private void SetWall(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //setwall <wall> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        List<int> wallID = Tools.GetWallID(e.Parameters[0].ToLowerInvariant());
        if (wallID.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid wall '{0}'!", e.Parameters[0]);
            return;
        }
        if (wallID.Count > 1)
        {
            e.Player.SendErrorMessage("More than one wall matched!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new SetWall(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, wallID[0], expression));
        }
    }

    private void SetGrass(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //setgrass <grass> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (!Biomes.Keys.Contains(e.Parameters[0].ToLowerInvariant()) || e.Parameters[0].ToLowerInvariant() == "snow")
        {
            e.Player.SendErrorMessage("Invalid grass '{0}'!", e.Parameters[0]);
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new SetGrass(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, e.Parameters[0].ToLowerInvariant(), expression));
        }
    }

    private void SetWire(CommandArgs e)
    {
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //setwire <wire> <wire state> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (!int.TryParse(e.Parameters[0], out var result) || result < 1 || result > 4)
        {
            e.Player.SendErrorMessage("Invalid wire '{0}'!", e.Parameters[0]);
            return;
        }
        bool state = false;
        if (string.Equals(e.Parameters[1], "on", StringComparison.OrdinalIgnoreCase))
        {
            state = true;
        }
        else if (!string.Equals(e.Parameters[1], "off", StringComparison.OrdinalIgnoreCase))
        {
            e.Player.SendErrorMessage("Invalid wire state '{0}'!", e.Parameters[1]);
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 2 && !Parser.TryParseTree(e.Parameters.Skip(2), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new SetWire(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, result, state, expression));
        }
    }

    private void Shape(CommandArgs e)
    {
        bool flag = false;
        bool flag2 = false;
        switch (e.Message.Split(' ')[0].Substring(6).ToLower())
        {
            case "f":
            case "fill":
                flag2 = true;
                break;
            case "w":
            case "wall":
                flag = true;
                break;
            case "wf":
            case "wallfill":
                flag = true;
                flag2 = true;
                break;
        }
        string msg = "Invalid syntax! Proper syntax: //shape" + (flag ? "wall" : "") + (flag2 ? "fill" : "") + " <shape> [rotate type] [flip type] <tile/wall> [=> boolean expr...]";
        if (e.Parameters.ElementAtOrDefault(0)?.ToLower() == "help")
        {
            e.Player.SendInfoMessage("Allowed shape types: line/l, rectangle/r, ellipse/e, isoscelestriangle/it, righttriangle/rt.");
            return;
        }
        if (e.Parameters.Count < 2)
        {
            e.Player.SendErrorMessage(msg);
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        int rotateType = 0;
        int flipType = 0;
        int num = 1;
        int shapeType;
        switch (e.Parameters[0].ToLower())
        {
            case "l":
            case "line":
                shapeType = 0;
                break;
            case "r":
            case "rect":
            case "rectangle":
                shapeType = 1;
                break;
            case "e":
            case "ellipse":
                shapeType = 2;
                break;
            case "it":
            case "itriangle":
            case "isoscelestriangle":
                shapeType = 3;
                if (e.Parameters.Count < num + 2)
                {
                    e.Player.SendErrorMessage(msg);
                    e.Player.SendInfoMessage("Allowed rotate types: up/u, down/d, left/l, right/r.");
                    return;
                }
                switch (e.Parameters[num])
                {
                    case "u":
                    case "up":
                        rotateType = 0;
                        break;
                    case "d":
                    case "down":
                        rotateType = 1;
                        break;
                    case "l":
                    case "left":
                        rotateType = 2;
                        break;
                    case "r":
                    case "right":
                        rotateType = 3;
                        break;
                    default:
                        e.Player.SendErrorMessage("Invalid rotate type! Allowed types: up/u, down/d, left/l, right/r.");
                        break;
                }
                num++;
                break;
            case "rt":
            case "rtriangle":
            case "righttriangle":
                shapeType = 4;
                if (e.Parameters.Count < num + 3)
                {
                    e.Player.SendErrorMessage(msg);
                    e.Player.SendInfoMessage("Allowed rotate types: up/u, down/d.");
                    e.Player.SendInfoMessage("Allowed flip types: left/l, right/r.");
                    return;
                }
                switch (e.Parameters[num])
                {
                    case "u":
                    case "up":
                        rotateType = 0;
                        break;
                    case "d":
                    case "down":
                        rotateType = 1;
                        break;
                    default:
                        e.Player.SendErrorMessage("Invalid rotate type! Allowed types: up/u, down/d.");
                        break;
                }
                switch (e.Parameters[num + 1])
                {
                    case "l":
                    case "left":
                        flipType = 0;
                        break;
                    case "r":
                    case "right":
                        flipType = 1;
                        break;
                    default:
                        e.Player.SendErrorMessage("Invalid flip type! Allowed types: left/l, right/r.");
                        break;
                }
                num += 2;
                break;
            default:
                e.Player.SendErrorMessage("Invalid shape type! Allowed types: line/l, rectangle/r, ellipse/e, isoscelestriangle/it, righttriangle/rt.");
                return;
        }
        if (e.Parameters.Count < num)
        {
            e.Player.SendErrorMessage(msg);
            return;
        }
        int materialType;
        if (flag)
        {
            List<int> wallID = Tools.GetWallID(e.Parameters[num].ToLowerInvariant());
            if (wallID.Count == 0)
            {
                e.Player.SendErrorMessage("Invalid wall '{0}'!", e.Parameters[num]);
                return;
            }
            if (wallID.Count > 1)
            {
                e.Player.SendErrorMessage("More than one wall matched!");
                return;
            }
            materialType = wallID[0];
        }
        else
        {
            List<int> tileID = Tools.GetTileID(e.Parameters[num].ToLowerInvariant());
            if (tileID.Count == 0)
            {
                e.Player.SendErrorMessage("Invalid tile '{0}'!", e.Parameters[num]);
                return;
            }
            if (tileID.Count > 1)
            {
                e.Player.SendErrorMessage("More than one tile matched!");
                return;
            }
            materialType = tileID[0];
        }
        Expression expression = null;
        if (e.Parameters.Count > ++num && !Parser.TryParseTree(e.Parameters.Skip(num), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Shape(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, shapeType, rotateType, flipType, flag, flag2, materialType, expression));
        }
    }

    private void Size(CommandArgs e)
    {
        switch (e.Parameters.ElementAtOrDefault(0)?.ToLower())
        {
            case "c":
            case "clipboard":
            {
                if (e.Player.Account == null)
                {
                    e.Player.SendErrorMessage("You have to be logged in to use this command.");
                    break;
                }
                UserAccount userAccount = e.Player.Account;
                if (e.Parameters.Count > 1)
                {
                    if (!e.Player.HasPermission("worldedit.usage.otheraccounts"))
                    {
                        e.Player.SendErrorMessage("You do not have permission to view other player's clipboards.");
                        break;
                    }
                    userAccount = TShock.UserAccounts.GetUserAccountByName(e.Parameters[1]);
                    if (userAccount == null)
                    {
                        e.Player.SendErrorMessage("Invalid account name!");
                        break;
                    }
                }
                if (!Tools.HasClipboard(userAccount.ID))
                {
                    e.Player.SendErrorMessage(userAccount.Name + " doesn't have a clipboard.");
                    break;
                }
                WorldSectionData worldSectionData2 = Tools.LoadWorldData(Tools.GetClipboardPath(userAccount.ID));
                e.Player.SendSuccessMessage($"{userAccount.Name}'s clipboard size: {worldSectionData2.Tiles.GetLength(0) - 1}x{worldSectionData2.Tiles.GetLength(1) - 1}.");
                break;
            }
            case "s":
            case "schematic":
            {
                if (!e.Player.HasPermission("worldedit.schematic"))
                {
                    e.Player.SendErrorMessage("You do not have permission to use this command.");
                    break;
                }
                string path = Path.Combine("worldedit", $"schematic-{e.Parameters[1]}.dat");
                if (!File.Exists(path))
                {
                    e.Player.SendErrorMessage("Invalid schematic '{0}'!", e.Parameters[1]);
                    break;
                }
                WorldSectionData worldSectionData = Tools.LoadWorldData(path);
                e.Player.SendSuccessMessage($"Schematic's size ('{e.Parameters[1]}'): {worldSectionData.Tiles.GetLength(0) - 1}x{worldSectionData.Tiles.GetLength(1) - 1}.");
                break;
            }
            default:
                e.Player.SendErrorMessage("//size <clipboard/c> [user name]\n//size <schematic/s> <name>");
                break;
        }
    }

    private void Slope(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //slope <type> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        int slopeID = Tools.GetSlopeID(e.Parameters[0].ToLowerInvariant());
        if (slopeID == -1)
        {
            e.Player.SendErrorMessage("Invalid type '{0}'! Available slopes: none (0), t (1), tr (2), tl (3), br (4), bl (5)", e.Parameters[0]);
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Slope(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, slopeID, expression));
        }
    }

    private void SlopeDelete(CommandArgs e)
    {
        int num = 255;
        Expression expression = null;
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (e.Parameters.Count >= 1)
        {
            num = Tools.GetSlopeID(e.Parameters[0].ToLowerInvariant());
            if (num == -1)
            {
                e.Player.SendErrorMessage("Invalid type '{0}'! Available slopes: none (0), t (1), tr (2), tl (3), br (4), bl (5)", e.Parameters[0]);
                return;
            }
            if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
            {
                e.Player.SendErrorMessage("Invalid expression!");
                return;
            }
        }
        _commandQueue.Add(new SlopeDelete(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, num, expression));
    }

    private void Smooth(CommandArgs e)
    {
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 0 && !Parser.TryParseTree(e.Parameters, out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Smooth(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, expression));
        }
    }

    private void Inactive(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //inactive <status(on/off/reverse)> [=> boolean expr...]");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        int inacType = 2;
        string text = e.Parameters[0].ToLower();
        if (text == "on")
        {
            inacType = 0;
        }
        else if (text == "off")
        {
            inacType = 1;
        }
        else if (text != "reverse")
        {
            e.Player.SendErrorMessage("Invalid status! Proper: on, off, reverse");
            return;
        }
        Expression expression = null;
        if (e.Parameters.Count > 1 && !Parser.TryParseTree(e.Parameters.Skip(1), out expression))
        {
            e.Player.SendErrorMessage("Invalid expression!");
        }
        else
        {
            _commandQueue.Add(new Inactive(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, playerInfo.MagicWand, e.Player, inacType, expression));
        }
    }

    private void Shift(CommandArgs e)
    {
        if (e.Parameters.Count != 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //shift <direction> <amount>");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
            return;
        }
        if (!int.TryParse(e.Parameters[1], out var result) || result < 0)
        {
            e.Player.SendErrorMessage("Invalid shift amount '{0}'!", e.Parameters[0]);
            return;
        }
        string text = e.Parameters[0].ToLowerInvariant();
        foreach (char c in text)
        {
            switch (c)
            {
                case 'd':
                    playerInfo.Y += result;
                    playerInfo.Y2 += result;
                    break;
                case 'l':
                    playerInfo.X -= result;
                    playerInfo.X2 -= result;
                    break;
                case 'r':
                    playerInfo.X += result;
                    playerInfo.X2 += result;
                    break;
                case 'u':
                    playerInfo.Y -= result;
                    playerInfo.Y2 -= result;
                    break;
                default:
                    e.Player.SendErrorMessage("Invalid direction '{0}'!", c);
                    return;
            }
        }
        e.Player.SendSuccessMessage("Shifted selection.");
    }

    private void Text(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //text <text> (\\n for new line)");
            e.Player.SendInfoMessage("In the beginning of new line:");
            e.Player.SendSuccessMessage("\\m for middle position\n\\r for right position\n\\s<num> (for example \\s3) for line spacing\n\\c for cropped statue (2 blocks heigh, without stand)");
            return;
        }
        PlayerInfo playerInfo = e.Player.GetPlayerInfo();
        if (playerInfo.X == -1 || playerInfo.Y == -1 || playerInfo.X2 == -1 || playerInfo.Y2 == -1)
        {
            e.Player.SendErrorMessage("Invalid selection!");
        }
        else
        {
            _commandQueue.Add(new Text(playerInfo.X, playerInfo.Y, playerInfo.X2, playerInfo.Y2, e.Player, e.Message.Substring(5).TrimStart()));
        }
    }

    private void Undo(CommandArgs e)
    {
        if (e.Player.Account == null)
        {
            e.Player.SendErrorMessage("You have to be logged in to use this command.");
            return;
        }
        if (e.Parameters.Count > 2)
        {
            e.Player.SendErrorMessage("Invalid syntax! Proper syntax: //undo [steps] [account]");
            return;
        }
        int result = 1;
        int iD = e.Player.Account.ID;
        if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out result) || result <= 0))
        {
            e.Player.SendErrorMessage("Invalid undo steps '{0}'!", e.Parameters[0]);
        }
        else if (e.Parameters.Count > 1)
        {
            if (!e.Player.HasPermission("worldedit.usage.otheraccounts"))
            {
                e.Player.SendErrorMessage("You do not have permission to undo other player's actions.");
                return;
            }
            UserAccount userAccountByName = TShock.UserAccounts.GetUserAccountByName(e.Parameters[1]);
            if (userAccountByName == null)
            {
                e.Player.SendErrorMessage("Invalid account name!");
                return;
            }
            iD = userAccountByName.ID;
        }
        _commandQueue.Add(new Undo(e.Player, iD, result));
    }
}
