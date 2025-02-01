using History.Commands;
using Microsoft.Data.Sqlite;
using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using System.Data;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace History;

[ApiVersion(2, 1)]
public class History : TerrariaPlugin
{
    public static List<Action> Actions = new(SaveCount);
    public static IDbConnection? Database;
    public static DateTime Date = DateTime.UtcNow;
    public const int SaveCount = 10;

    private readonly bool[] AwaitingHistory = new bool[256];
    public override string Author => "Cracker64 & Zaicon & Cai & 肝帝熙恩";

    readonly CancellationTokenSource Cancel = new();
    private readonly BlockingCollection<HCommand> CommandQueue = new();
    private Thread CommandQueueThread = null!;
    public override string Description => GetString("记录图格操作.");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 7);

    public History(Main game) : base(game)
    {
        this.Order = 5;
    }
    public override void Initialize()
    {
        Connect();
        this.InitBreaks();
        TShockAPI.Commands.ChatCommands.Add(new Command("history.get", this.HistoryCmd, "history", "历史"));
        TShockAPI.Commands.ChatCommands.Add(new Command("history.prune", this.Prune, "prunehist", "删除历史"));
        TShockAPI.Commands.ChatCommands.Add(new Command("history.reenact", this.Reenact, "reenact", "复现"));
        TShockAPI.Commands.ChatCommands.Add(new Command("history.rollback", this.Rollback, "rollback", "回溯"));
        TShockAPI.Commands.ChatCommands.Add(new Command("history.rollback", this.Undo, "rundo1", "撤销"));
        TShockAPI.Commands.ChatCommands.Add(new Command("history.admin", this.ResetCmd, "hreset", "重置历史"));
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData, int.MinValue);
        ServerApi.Hooks.WorldSave.Register(this, this.OnSaveWorld);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TShockAPI.Commands.ChatCommands.RemoveAll(
    x => x.CommandDelegate == this.HistoryCmd ||
    x.CommandDelegate == this.Prune ||
    x.CommandDelegate == this.Reenact ||
    x.CommandDelegate == this.Rollback ||
    x.CommandDelegate == this.Undo ||
    x.CommandDelegate == this.ResetCmd
    );
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            ServerApi.Hooks.WorldSave.Deregister(this, this.OnSaveWorld);

            this.Cancel.Cancel();
        }
    }
    private static void Connect()
    {
        var text = Path.Combine(TShock.SavePath, "HistoryDB.sqlite");
        Database = new SqliteConnection($"Data Source={text}");
    }
    void Queue(string account, int X, int Y, byte action, ushort data = 0, byte style = 0, short paint = 0, string text = null!, int alternate = 0, int random = 0, bool direction = false)
    {
        if (Actions.Count == SaveCount)
        {
            this.CommandQueue.Add(new SaveCommand(Actions.ToArray()));
            Actions.Clear();
        }
        Actions.Add(new Action { account = account, action = action, data = data, time = (int) (DateTime.UtcNow - Date).TotalSeconds, x = X, y = Y, paint = paint, style = style, text = text, alt = alternate, direction = direction, random = (sbyte) random });
    }
    // 334 weapon rack done? weapon styles?
    static void GetPlaceData(ushort type, ref int which, ref int div)
    {
        switch (type)
        {
            //WHICH block style is in 0:X   1:Y
            case 314: //minecart ????
                which = 0;
                div = 1;
                break;
            case 13: //bottle
            case 36: //present
                     //case 49: //water candle Removing - No different styles?
            case 82: //herb
            case 83: //herb
            case 84: //herb
            case 91: //banner
            case 144: //timer
            case 149: //christmas light
            case 174: //platinum candle
                      //case 78: //clay pot
            case 178: //gems
            case 184:
            case 239: //bars
            case 419:
                which = 0;
                div = 18;
                break;
            case 19: //platforms
            case 135: //pressure plates
            case 136: //switch (state)
            case 137: //traps
            case 141: //explosives
            case 210: //land mine
            case 380: //planter box
            case 420: //L Gate
            case 423: // L Sensor
            case 424: //Junction Box
            case 428: // Weighted Presure Plate
            case 429: //Wire bulb
            case 445: //Pixel Box
                which = 1;
                div = 18;
                break;
            case 4: //torch
            case 33: //candle
            case 324: //beach piles
                which = 1;
                div = 22;
                break;
            case 227: //dye plants
                which = 0;
                div = 34;
                break;
            case 16: //anvil
            case 18: //work bench
            case 21: //chest
            case 27: //sunflower (randomness)
            case 29:
            case 55: // sign
            case 85: //tombstone
            case 103:
            case 104: //grandfather
            case 128: // mannequin (orient)
            case 132: //lever (state)
            case 134:
            case 207: // water fountains
            case 245: //2x3 wall picture
            case 254:
            case 269: // womannequin
            case 320: //more statues
            case 337: //     statues
            case 376: //fishing crates
            case 378: //target dummy
            case 386: //trapdoor open
            case 395:
            case 410: //lunar monolith
            case 411: //Detonator
            case 425: // Announcement (Sign)
            case 441:
            case 443: //Geyser
                which = 0;
                div = 36;
                break;
            case 35://jack 'o lantern??
            case 42://lanterns
            case 79://beds (orient)
            case 90://bathtub (orient)
            case 139://music box
            case 246:// 3x2 wall painting
            case 270:
            case 271:
                which = 1;
                div = 36;
                break;
            case 172: //sinks
                which = 1;
                div = 38;
                break;
            case 15://chair
            case 20:
            case 216:
            case 338:
                which = 1;
                div = 40;
                break;
            case 14:
            case 17:
            case 26:
            case 86:
            case 87:
            case 88:
            case 89:
            case 114:
            case 186:
            case 187:
            case 215:
            case 217:
            case 218:
            case 237:
            case 244:
            case 285:
            case 286:
            case 298:
            case 299:
            case 310:
            case 106:
            case 170:
            case 171:
            //case 172:
            case 212:
            case 219:
            case 220:
            case 228:
            case 231:
            case 243:
            case 247:
            case 283:
            case 300:
            case 301:
            case 302:
            case 303:
            case 304:
            case 305:
            case 306:
            case 307:
            case 308:
            case 77:
            case 101:
            case 102:
            case 133:
            case 339:
            case 235: //teleporter
            case 377: //sharpening station
            case 405: //fireplace
                which = 0;
                div = 54;
                break;
            case 10:
            case 11: //door
            case 34: //chandelier
            case 93: //tikitorch
            case 241: //4x3 wall painting
                which = 1;
                div = 54;
                break;
            case 240: //3x3 painting, style stored in both
            case 440:
                which = 2;
                div = 54;
                break;
            case 209:
                which = 0;
                div = 72;
                break;
            case 242:
                which = 1;
                div = 72;
                break;
            case 388: //tall gate closed
            case 389: //tall gate open
                which = 1;
                div = 94;
                break;
            case 92: // lamp post
                which = 1;
                div = 108;
                break;
            case 105: // Statues
                which = 3;
                break;
            default:
                break;
        }
    }
    //This returns where the furniture is expected to be placed for worldgen
    static Vector2 DestFrame(ushort type)
    {
        var dest = type switch//(x,y) is from top left
        {
            16 or 18 or 29 or 42 or 91 or 103 or 134 or 270 or 271 or 386 or 387 or 388 or 389 or 395 or 443 => new Vector2(0, 0),
            15 or 21 or 35 or 55 or 85 or 139 or 216 or 245 or 338 or 390 or 425 => new Vector2(0, 1),
            34 or 95 or 126 or 246 or 235 => new Vector2(1, 0),
            14 or 17 or 26 or 77 or 79 or 86 or 87 or 88 or 89 or 90 or 94 or 96 or 97 or 98 or 99 or 100 or 114 or 125 or 132 or 133 or 138 or 142 or 143 or 172 or 173 or 186 or 187 or 215 or 217 or 218 or 237 or 240 or 241 or 244 or 254 or 282 or 285 or 286 or 287 or 288 or 289 or 290 or 291 or 292 or 293 or 294 or 295 or 298 or 299 or 310 or 316 or 317 or 318 or 319 or 334 or 335 or 339 or 354 or 355 or 360 or 361 or 362 or 363 or 364 or 376 or 377 or 391 or 392 or 393 or 394 or 405 or 411 or 440 or 441 => new Vector2(1, 1),
            // Statues use (0,2) from PlaceTile, but (1,2) from PlaceObject, strange
            105 or 106 or 209 or 212 or 219 or 220 or 228 or 231 or 243 or 247 or 283 or 300 or 301 or 302 or 303 or 304 or 305 or 306 or 307 or 308 or 349 or 356 or 378 or 406 or 410 or 412 => new Vector2(1, 2),
            101 or 102 => new Vector2(1, 3),
            // (2,2)
            242 => new Vector2(2, 2),
            10 or 11 or 93 or 128 or 269 or 320 or 337 => new Vector2(0, 2),
            207 or 27 => new Vector2(0, 3),
            // (0,4)
            104 => new Vector2(0, 4),
            // (0,5)
            92 => new Vector2(0, 5),
            275 or 276 or 277 or 278 or 279 or 280 or 281 or 296 or 297 or 309 => new Vector2(3, 1),
            358 or 359 or 413 or 414 => new Vector2(3, 2),
            _ => new Vector2(-1, -1),
        };
        return dest;
    }
    static Vector2 FurnitureDimensions(ushort type, byte style)
    {
        var dim = new Vector2(0, 0);
        switch (type)
        {
            case 15: //1x2
            case 20:
            case 42:
            case 216:
            case 270://top
            case 271://top
            case 338:
            case 390:
                dim = new Vector2(0, 1);
                break;
            case 91: //1x3
            case 93: //1x3
                dim = new Vector2(0, 2);
                break;
            case 388:
            case 389:
                dim = new Vector2(0, 4);
                break;
            case 92: //1x4
                dim = new Vector2(0, 5);
                break;
            case 16: //2x1
            case 18:
            case 29:
            case 103:
            case 134:
            case 387:
            case 443:
                dim = new Vector2(1, 0);
                break;
            case 21: //2x2
            case 35:
            case 55:
            case 85:
            case 94:
            case 95:
            case 96:
            case 97:
            case 98:
            case 99:
            case 100:
            case 125:
            case 126:
            case 132:
            case 138:
            case 139:
            case 142:
            case 143:
            case 173:
            case 254:
            case 287:
            case 288:
            case 289:
            case 290:
            case 291:
            case 292:
            case 293:
            case 294:
            case 295:
            case 316:
            case 317:
            case 318:
            case 319:
            case 335:
            case 360:
            case 376:
            case 386:
            case 395:
            case 411:
            case 425:
            case 441:
                dim = new Vector2(1, 1);
                break;
            case 105: //2x3
            case 128:
            case 245:
            case 269:
            case 320:
            case 337:
            case 349:
            case 356:
            case 378:
            case 410:
                dim = new Vector2(1, 2);
                break;
            case 27:
            case 207:
                dim = new Vector2(1, 3);
                break;
            case 104:
                dim = new Vector2(1, 4);
                break;
            case 10:
                dim = new Vector2(0, 2);
                break;
            case 14:
                dim = style == 25 ? new Vector2(2, 0) : new Vector2(2, 1);
                break;
            case 17:
            case 26:
            case 77:
            case 86:
            case 87:
            case 88:
            case 89:
            case 114:
            case 133:
            case 186:
            case 187:
            case 215:
            case 217:
            case 218:
            case 237:
            case 244:
            case 246:
            case 285:
            case 286:
            case 298:
            case 299:
            case 310:
            case 339:
            case 354:
            case 355:
            case 361:
            case 362:
            case 363:
            case 364:
            case 377:
            case 391:
            case 392:
            case 393:
            case 394:
            case 405:
                dim = new Vector2(2, 1);
                break;
            case 235:
                dim = new Vector2(2, 0);
                break;
            case 34:
            case 106:
            case 212:
            case 219:
            case 220:
            case 228:
            case 231:
            case 240:
            case 243:
            case 247:
            case 283:
            case 300:
            case 301:
            case 302:
            case 303:
            case 304:
            case 305:
            case 306:
            case 307:
            case 308:
            case 334:
            case 406:
            case 412:
            case 440:
                dim = new Vector2(2, 2);
                break;
            case 101:
            case 102:
                dim = new Vector2(2, 3);
                break;
            case 79:
            case 90:
                dim = new Vector2(3, 1);
                break;
            case 209:
            case 241:
                dim = new Vector2(3, 2);
                break;
            case 275:
            case 276:
            case 277:
            case 278:
            case 279:
            case 280:
            case 281:
            case 296:
            case 297:
            case 309:
            case 358:
            case 359:
            case 413:
            case 414:
                dim = new Vector2(5, 2);
                break;
            case 242:
                dim = new Vector2(5, 3);
                break;
            default:
                break;
        }
        return dim;
    }
    //This finds the 0,0 of a furniture
    static Vector2 AdjustDest(ref Vector2 dest, ITile tile, int which, int div, byte style)
    {
        var relative = new Vector2(0, 0);
        if (dest.X < 0)
        {
            //no destination
            dest.X = dest.Y = 0;
            return relative;
        }
        int frameX = tile.frameX;
        int frameY = tile.frameY;
        var relx = 0;
        var rely = 0;
        //Remove data from Mannequins before adjusting
        if (tile.type == 128 || tile.type == 269)
        {
            frameX %= 100;
        }

        switch (which)
        {
            case 0:
                relx = frameX % div / 18;
                rely = frameY / 18;
                break;
            case 1:
                relx = frameX / 18;
                rely = frameY % div / 18;
                break;
            case 2:
                relx = frameX % div / 18;
                rely = frameY % div / 18;
                break;
            case 3: // Statues have style split, possibly more use this?
                rely = frameY % 54 / 18;
                relx = frameX % 36 / 18;
                break;
            default:
                relx = frameX / 18;
                rely = frameY / 18;
                break;
        }
        if (tile.type == 55 || tile.type == 425)//sign
        {
            switch (style)
            {
                case 1:
                case 2:
                    dest.Y--;
                    break;
                case 3:
                    dest.Y--;
                    dest.X++;
                    break;
            }
        }
        else if (tile.type == 11)//opened door
        {
            if (frameX / 36 > 0)
            {
                relx -= 2;
                dest.X++;
            }
        }
        else if (tile.type == 10 || tile.type == 15)// random frames, ignore X
        {
            relx = 0;
        }
        else if (tile.type == 209)//cannoonn
        {
            rely = frameY % 37 / 18;
        }
        else if (tile.type == 79 || tile.type == 90)//bed,bathtub
        {
            relx = frameX % 72 / 18;
        }
        else if (tile.type == 14 && style == 25)
        {
            dest.Y--;
        }
        else if (tile.type == 334)
        {
            rely = frameY / 18;
            var tx = frameX;
            if (frameX > 5000)
            {
                tx = ((frameX / 5000) - 1) * 18;
            }

            if (tx >= 54)
            {
                tx -= 54;
            }

            relx = tx / 18;
        }
        relative = new Vector2(relx, rely);

        return relative;
    }
    //This takes a coordinate on top of a furniture and returns the correct "placement" location it would have used.
    static void AdjustFurniture(ref int x, ref int y, ref byte style, bool origin = false)
    {
        var which = 10; // An invalid which, to skip cases if it never changes.
        var div = 1;
        var tile = Main.tile[x, y];
        GetPlaceData(tile.type, ref which, ref div);
        switch (which)
        {
            case 0:
                style = (byte) (tile.frameX / div);
                break;
            case 1:
                style = (byte) (tile.frameY / div);
                break;
            case 2:
                style = (byte) ((tile.frameY / div * 36) + (tile.frameX / div));
                break;
            case 3: //Just statues for now
                style = (byte) ((tile.frameX / 36) + (tile.frameY / 54 * 55));
                break;
            default:
                break;
        }
        if (style < 0)
        {
            style = 0;
        }

        if (!Main.tileFrameImportant[tile.type])
        {
            return;
        }

        if (div == 1)
        {
            div = 0xFFFF;
        }

        var dest = DestFrame(tile.type);
        var relative = AdjustDest(ref dest, tile, which, div, style);
        if (origin)
        {
            dest = new Vector2(0, 0);
        }

        x += (int) (dest.X - relative.X);
        y += (int) (dest.Y - relative.Y);

    }
    public static void PaintFurniture(ushort type, int x, int y, byte paint)
    {

        byte style = 0;
        AdjustFurniture(ref x, ref y, ref style, true);
        var size = FurnitureDimensions(type, style);
        for (var j = x; j <= x + size.X; j++)
        {
            for (var k = y; k <= y + size.Y; k++)
            {
                if (Main.tile[j, k].type == type)
                {
                    Main.tile[j, k].color(paint);
                }
            }
        }
    }

    readonly bool[] breakableBottom = new bool[693];
    readonly bool[] breakableTop = new bool[693];
    readonly bool[] breakableSides = new bool[693];
    readonly bool[] breakableWall = new bool[693];
    public static readonly int[] breakableBottomIndex = { 4, 10, 11, 13, 14, 15, 16, 17, 18, 21, 26, 27, 29, 33, 35, 49, 50, 55, 77, 78,
        79, 81, 82, 85, 86, 87, 88, 89, 90, 92, 93, 94, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 114, 125, 128, 129, 132, 133,
        134, 135, 136, 138, 139, 142, 143, 144, 149, 173, 174, 178, 186, 187, 207, 209, 212, 215, 216, 217, 218, 219, 220, 228, 231, 235,
        237, 239, 243, 244, 247, 254, 269, 275, 276, 278, 279, 280, 281, 283, 285, 286, 287, 296, 297, 298, 299, 300, 301, 302, 303, 304,
        305, 306, 307, 308, 309, 310, 316, 317, 318, 319, 320, 335, 337, 338, 339, 349, 354, 355, 356, 358, 359, 360, 361, 362, 363, 364,
        372, 376, 377, 378, 380, 380, 388, 389, 390, 391, 392, 393, 394, 395, 405, 406, 410, 413, 414, 419, 425, 441, 442, 443 };
    public static readonly int[] breakableTopIndex = { 10, 11, 34, 42, 55, 91, 95, 126, 129, 149, 270, 271, 380, 388, 389, 395, 425, 443 };
    public static readonly int[] breakableSidesIndex = { 4, 55, 129, 136, 149, 380, 386, 387, 395, 425 };
    public static readonly int[] breakableWallIndex = { 4, 132, 136, 240, 241, 242, 245, 246, 334, 380, 395, 440 };
    void InitBreaks()
    {
        foreach (var index in breakableBottomIndex) { this.breakableBottom[index] = true; }

        foreach (var index in breakableTopIndex) { this.breakableTop[index] = true; }

        foreach (var index in breakableSidesIndex) { this.breakableSides[index] = true; }

        foreach (var index in breakableWallIndex) { this.breakableWall[index] = true; }
    }
    void LogEdit(byte etype, ITile tile, int X, int Y, ushort type, string account, List<Vector2> done, byte style = 0, int alt = 0, int random = -1, bool direction = false)
    {
        switch (etype)
        {
            case 0: //killtile
            case 4: //killtilenoitem
            case 20: //trykilltile
                var tileType = Main.tile[X, Y].type;
                byte pStyle = 0;
                if (Main.tile[X, Y].active() && !Main.tileCut[tileType] && tileType != 127)
                {
                    AdjustFurniture(ref X, ref Y, ref pStyle);
                    //Don't repeat the same tile, and it is possible to create something that breaks thousands of tiles with one edit, is this a sane limit?
                    if (done.Contains(new Vector2(X, Y)) || done.Count > 2000)
                    {
                        return;
                    }

                    done.Add(new Vector2(X, Y));
                    //TODO: Sand falling from a solid tile broken below
                    switch (tileType)
                    {
                        case 10: //doors don't break anything anyway
                        case 11: //close open doors
                            tileType = 10;
                            break;
                        case 55: //Signs
                        case 85: //Gravestones
                        case 425: // Announcement
                            var signI = Sign.ReadSign(X, Y);
                            this.Queue(account, X, Y, 0, tileType, pStyle, Main.tile[X, Y].color(), text: Main.sign[signI].text);
                            return;
                        case 124: //wooden beam, breaks sides only
                            if (Main.tile[X - 1, Y].active() && this.breakableSides[Main.tile[X - 1, Y].type])
                            {
                                this.LogEdit(0, Main.tile[X - 1, Y], X - 1, Y, 0, account, done);
                            }

                            if (Main.tile[X + 1, Y].active() && this.breakableSides[Main.tile[X + 1, Y].type])
                            {
                                this.LogEdit(0, Main.tile[X + 1, Y], X + 1, Y, 0, account, done);
                            }

                            break;
                        case 128:
                        case 269: //Mannequins
                            var headSlot = Main.tile[X, Y - 2].frameX / 100;
                            var bodySlot = Main.tile[X, Y - 1].frameX / 100;
                            var legSlot = Main.tile[X, Y].frameX / 100;
                            // The vars 'style' and 'random' cause mannequins to place improperly and can't be used.
                            this.Queue(account, X, Y, 0, tileType, paint: (short) headSlot, alternate: bodySlot + (legSlot << 10), direction: (Main.tile[X, Y].frameX % 100) > 0);
                            return;
                        case 138: //boulder, 2x2
                            for (var i = -1; i <= 0; i++)
                            {
                                if (Main.tile[X + i, Y - 2].active() && this.breakableBottom[Main.tile[X + i, Y - 2].type])
                                {
                                    this.LogEdit(0, Main.tile[X + i, Y - 2], X + i, Y - 2, 0, account, done);
                                }
                            }

                            for (var i = -1; i <= 0; i++)
                            {
                                if (Main.tile[X - 2, Y + i].active() && this.breakableSides[Main.tile[X - 2, Y + i].type])
                                {
                                    this.LogEdit(0, Main.tile[X - 2, Y + i], X - 2, Y + i, 0, account, done);
                                }

                                if (Main.tile[X, Y + i].active() && this.breakableSides[Main.tile[X, Y + i].type])
                                {
                                    this.LogEdit(0, Main.tile[X, Y + i], X, Y + i, 0, account, done);
                                }
                            }
                            break;
                        case 235: //teleporter, 3x1
                            for (var i = -1; i <= 1; i++)
                            {
                                if (Main.tile[X + i, Y - 1].active() && this.breakableBottom[Main.tile[X + i, Y - 1].type])
                                {
                                    this.LogEdit(0, Main.tile[X + i, Y - 1], X + i, Y - 1, 0, account, done);
                                }
                            }

                            if (Main.tile[X - 2, Y].active() && this.breakableSides[Main.tile[X - 2, Y].type])
                            {
                                this.LogEdit(0, Main.tile[X - 2, Y], X - 2, Y, 0, account, done);
                            }

                            if (Main.tile[X + 2, Y].active() && this.breakableSides[Main.tile[X + 2, Y].type])
                            {
                                this.LogEdit(0, Main.tile[X + 2, Y], X + 2, Y, 0, account, done);
                            }

                            break;
                        case 53: //sand, silt, slush
                        case 112:
                        case 116:
                        case 123:
                        case 224:
                        case 234:
                            var types = new List<int>() { 53, 112, 116, 123, 224, 234 };
                            var topY = Y;//Find top of stack
                            while (topY >= 0 && Main.tile[X, topY].active() && types.Contains(Main.tile[X, topY].type))
                            {
                                topY--;
                            }
                            //Break anything at top
                            if (Main.tile[X, topY].active() && this.breakableBottom[Main.tile[X, topY].type])
                            {
                                this.LogEdit(0, Main.tile[X, topY], X, topY, 0, account, done);
                            }
                            //TO-DO: Atm, we'll just keep the record saying they broke the top block. We lose some data (type of sand), but I don't feel like
                            // making a workaround for that just yet.
                            topY++;
                            return;
                        case 239: //bars
                            topY = Y;//Find top of stack
                            while (topY >= 0 && Main.tile[X, topY].active() && Main.tile[X, topY].type == 239)
                            {
                                topY--;
                            }
                            //Break anything at top
                            if (Main.tile[X, topY].active() && this.breakableBottom[Main.tile[X, topY].type])
                            {
                                this.LogEdit(0, Main.tile[X, topY], X, topY, 0, account, done);
                            }

                            topY++;
                            while (topY <= Y)
                            {
                                //log from top of stack down, so reverting goes bottom->top
                                if (Main.tile[X - 1, topY].active() && this.breakableSides[Main.tile[X - 1, topY].type])
                                {
                                    this.LogEdit(0, Main.tile[X - 1, topY], X - 1, topY, 0, account, done);
                                }

                                if (Main.tile[X + 1, topY].active() && this.breakableSides[Main.tile[X + 1, topY].type])
                                {
                                    this.LogEdit(0, Main.tile[X + 1, topY], X + 1, topY, 0, account, done);
                                }

                                this.Queue(account, X, topY, 0, 239, pStyle, (short) (Main.tile[X, topY].color() + ((Main.tile[X, topY].halfBrick() ? 1 : 0) << 7)));
                                topY++;
                            }
                            return;
                        case 314: //Minecart Track
                            for (var i = -1; i < 2; i++)
                            {
                                for (var j = -1; j < 2; j++)
                                {
                                    if (Main.tile[X + i, Y + j].active() && Main.tile[X + i, Y + j].type == 314)
                                    {
                                        this.Queue(account, X + i, Y + j, 0, 314, (byte) (Main.tile[X + i, Y + j].frameX + 1), (short) (Main.tile[X + i, Y + j].color() + ((Main.tile[X + i, Y + j].frameY + 1) << 8)));
                                    }
                                }
                            }

                            return;
                        case 334: //Weapon Racks
                                  //X and Y are already normalized to the center, Center is item prefix, X-1 is NetID
                            var prefix = (short) (Main.tile[X, Y].frameX % 5000);
                            var netID = (Main.tile[X - 1, Y].frameX % 5000) - 100;
                            if (netID < 0)
                            {
                                break;
                            }

                            this.Queue(account, X, Y, 0, 334, paint: prefix, alternate: netID, direction: Main.tile[X, Y + 1].frameX > 54);
                            return;
                        case 395: //Item Frame, These are still disabled in History because of some serious TShock/Vanilla issues preventing proper logging at the moment.
                            /*TileEntity TE;
								if (TileEntity.ByPosition.TryGetValue(new Point16(X, Y), out TE))
								{
									TEItemFrame tEItemFrame = (TEItemFrame)TE;
									Console.WriteLine(tEItemFrame.ToString());
									Queue(account, X, Y, 0, 395, paint: (short)(Main.tile[X, Y].color()), random: tEItemFrame.item.prefix, alternate: tEItemFrame.item.netID);
									return;
								}*/
                            break;
                        default:
                            if (Main.tileSolid[tileType])
                            {
                                if (Main.tile[X, Y - 1].active() && this.breakableBottom[Main.tile[X, Y - 1].type])
                                {
                                    this.LogEdit(0, Main.tile[X, Y - 1], X, Y - 1, 0, account, done);
                                }

                                if (Main.tile[X, Y + 1].active() && this.breakableTop[Main.tile[X, Y + 1].type])
                                {
                                    this.LogEdit(0, Main.tile[X, Y + 1], X, Y + 1, 0, account, done);
                                }

                                if (Main.tile[X - 1, Y].active() && this.breakableSides[Main.tile[X - 1, Y].type])
                                {
                                    this.LogEdit(0, Main.tile[X - 1, Y], X - 1, Y, 0, account, done);
                                }

                                if (Main.tile[X + 1, Y].active() && this.breakableSides[Main.tile[X + 1, Y].type])
                                {
                                    this.LogEdit(0, Main.tile[X + 1, Y], X + 1, Y, 0, account, done);
                                }
                            }
                            else if (Main.tileTable[tileType])
                            {
                                var baseStart = -1;
                                var baseEnd = 1;
                                var height = 2;
                                switch (tileType)
                                {
                                    case 18://workbench
                                        baseStart = 0;
                                        height = 1;
                                        break;
                                    case 19://platform
                                        baseStart = baseEnd = 0;
                                        height = 1;
                                        break;
                                    case 101://bookcase
                                        height = 4;
                                        break;
                                    case 14://table
                                        if (style == 25)//dynasty table
                                        {
                                            height = 1;
                                        }

                                        break;
                                    default://3X2
                                        break;
                                }
                                for (var i = baseStart; i <= baseEnd; i++)
                                {
                                    if (Main.tile[X + i, Y - height].active() && this.breakableBottom[Main.tile[X + i, Y - height].type])
                                    {
                                        this.LogEdit(0, Main.tile[X + i, Y - height], X + i, Y - height, 0, account, done);
                                    }
                                }
                            }
                            break;
                    }
                    this.Queue(account, X, Y, 0, tileType, pStyle, (short) (Main.tile[X, Y].color() + (Main.tile[X, Y].halfBrick() ? 128 : 0) + (Main.tile[X, Y].slope() << 8)), null!, alt, random, direction);
                }
                break;
            case 1://add tile
                if ((!Main.tile[X, Y].active() || Main.tileCut[Main.tile[X, Y].type]) && type != 127)
                {
                    this.Queue(account, X, Y, 1, type, style);
                }
                break;
            case 2://del wall
                if (Main.tile[X, Y].wall != 0)
                {
                    //break things on walls
                    if (Main.tile[X, Y].active() && this.breakableWall[Main.tile[X, Y].type])
                    {
                        this.LogEdit(0, tile, X, Y, 0, account, done);
                    }

                    this.Queue(account, X, Y, 2, Main.tile[X, Y].wall, 0, Main.tile[X, Y].wallColor());
                }
                break;
            case 3://add wall
                if (Main.tile[X, Y].wall == 0)
                {
                    this.Queue(account, X, Y, 3, type);
                }
                break;
            case 5:
                if (!Main.tile[X, Y].wire())
                {
                    this.Queue(account, X, Y, 5);
                }
                break;
            case 6:
                if (Main.tile[X, Y].wire())
                {
                    this.Queue(account, X, Y, 6);
                }
                break;
            case 7:
                this.Queue(account, X, Y, 7);
                break;
            case 8:
                if (!Main.tile[X, Y].actuator())
                {
                    this.Queue(account, X, Y, 8);
                }
                break;
            case 9:
                if (Main.tile[X, Y].actuator())
                {
                    this.Queue(account, X, Y, 9);
                }
                break;
            case 10:
                if (!Main.tile[X, Y].wire2())
                {
                    this.Queue(account, X, Y, 10);
                }
                break;
            case 11:
                if (Main.tile[X, Y].wire2())
                {
                    this.Queue(account, X, Y, 11);
                }
                break;
            case 12:
                if (!Main.tile[X, Y].wire3())
                {
                    this.Queue(account, X, Y, 12);
                }
                break;
            case 13:
                if (Main.tile[X, Y].wire3())
                {
                    this.Queue(account, X, Y, 13);
                }
                break;
            case 14:
                //save previous state of slope
                this.Queue(account, X, Y, 14, type, 0, (short) (((Main.tile[X, Y].halfBrick() ? 1 : 0) << 7) + (Main.tile[X, Y].slope() << 8)));
                break;
            case 15:
                this.Queue(account, X, Y, 15);
                break;
            case 16:
                if (!Main.tile[X, Y].wire4())
                {
                    this.Queue(account, X, Y, 16);
                }
                break;
            case 17:
                if (Main.tile[X, Y].wire4())
                {
                    this.Queue(account, X, Y, 17);
                }
                break;
            case 19: //Acutate

                break;
            case 21: //ReplaceTile
                if (Main.tile[X, Y].active())
                {
                    var combinedInt = Main.tile[X, Y].type | (type << 16);
                    this.Queue(account, X, Y, 21, (ushort) combinedInt, style);
                }
                break;
            case 22: //ReplaceWall
                if (Main.tile[X, Y].wall > 0)
                {
                    var combinedInt = Main.tile[X, Y].wall | (type << 16);
                    this.Queue(account, X, Y, 22, (ushort) combinedInt);
                }
                break;
            case 23: //SlopePoundTile

                break;
        }
    }

    void OnGetData(GetDataEventArgs e)
    {
        if (!e.Handled)
        {

            switch (e.MsgID)
            {
                case PacketTypes.PlaceItemFrame:
                    //TSPlayer.All.SendInfoMessage("Placing item frame!");
                    break;
                case PacketTypes.PlaceTileEntity:
                    //TSPlayer.All.SendInfoMessage("Placing tile entity!");
                    break;
                case PacketTypes.UpdateTileEntity:
                    //TSPlayer.All.SendInfoMessage("Updating tile entity!");
                    break;
                case PacketTypes.Tile:
                {
                    var etype = e.Msg.readBuffer[e.Index];
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 1);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 3);
                    var type = BitConverter.ToUInt16(e.Msg.readBuffer, e.Index + 5);
                    var style = e.Msg.readBuffer[e.Index + 7];
                    if (type == 1 && (etype == 0 || etype == 4))
                    {
                        if (Main.tile[X, Y].type == 21 || Main.tile[X, Y].type == 88)
                        {
                            return; //Chests and dressers handled separately
                        }
                        //else if (Main.tile[X, Y].type == 2699)
                        //TSPlayer.All.SendInfoMessage("Weapon rack place");
                    }
                    //DEBUG
                    //TSPlayer.All.SendInfoMessage($"Type: {type}");

                    var ply = TShock.Players[e.Msg.whoAmI];
                    var logName = ply.Account == null ? "unregistered" : ply.Account.Name;
                    //Checking history now requires build permission in the area due to load order, if needed this could be fixed by having an alternate function check this packet on a higher order.
                    if (this.AwaitingHistory[e.Msg.whoAmI])
                    {
                        this.AwaitingHistory[e.Msg.whoAmI] = false;
                        ply.SendTileSquareCentered(X, Y, 5);
                        //DEBUG
                        //TSPlayer.All.SendInfoMessage($"X: {X}, Y: {Y}, FrameX: {Main.tile[X, Y].frameX}, FrameY: {Main.tile[X, Y].frameY}");
                        //END DEBUG
                        if (type == 0 && (etype == 0 || etype == 4))
                        {
                            AdjustFurniture(ref X, ref Y, ref style);
                        }

                        this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                        e.Handled = true;
                    }
                    else
                    {
                        //effect only
                        if (type == 1 && (etype == 0 || etype == 2 || etype == 4))
                        {
                            return;
                        }

                        this.LogEdit(etype, Main.tile[X, Y], X, Y, type, logName, new List<Vector2>(), style);
                    }
                }
                break;
                case PacketTypes.PlaceObject:
                {
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2);
                    var type = BitConverter.ToUInt16(e.Msg.readBuffer, e.Index + 4);
                    int style = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 6);
                    //DEBUG:
                    //TSPlayer.All.SendInfoMessage($"Style: {style}");
                    int alt = e.Msg.readBuffer[e.Index + 8];
                    //TSPlayer.All.SendInfoMessage($"Alternate: {alt}");
                    int rand = (sbyte) e.Msg.readBuffer[e.Index + 9];
                    //TSPlayer.All.SendInfoMessage($"Random: {rand}");
                    var dir = BitConverter.ToBoolean(e.Msg.readBuffer, e.Index + 10);

                    var ply = TShock.Players[e.Msg.whoAmI];
                    var logName = ply.Account == null ? "unregistered" : ply.Account.Name;
                    //Checking history now requires build permission in the area due to load order, if needed this could be fixed by having an alternate function check this packet on a higher order.
                    if (this.AwaitingHistory[e.Msg.whoAmI])
                    {
                        this.AwaitingHistory[e.Msg.whoAmI] = false;
                        ply.SendTileSquareCentered(X, Y, 5);
                        this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                        e.Handled = true;
                    }
                    else
                    {
                        this.LogEdit(1, Main.tile[X, Y], X, Y, type, logName, new List<Vector2>(), (byte) style, alt, rand, dir);
                    }
                }
                break;
                //chest delete
                case PacketTypes.PlaceChest:
                {
                    var flag = e.Msg.readBuffer[e.Index];
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 1);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 3);
                    int style = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 5);
                    var style2 = (byte) style;
                    var ply = TShock.Players[e.Msg.whoAmI];
                    var logName = ply.Account == null ? "unregistered" : ply.Account.Name;
                    //PlaceChest
                    if (flag == 0)
                    {
                        if (this.AwaitingHistory[e.Msg.whoAmI])
                        {
                            this.AwaitingHistory[e.Msg.whoAmI] = false;
                            ply.SendTileSquareCentered(X, Y, 5);
                            this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                            e.Handled = true;
                        }
                        else
                        {
                            this.LogEdit(1, Main.tile[X, Y], X, Y, 21, logName, new List<Vector2>(), style2);
                        }
                        return;
                    }
                    //KillChest
                    if (flag == 1 && Main.tile[X, Y].type == 21)
                    {
                        if (this.AwaitingHistory[e.Msg.whoAmI])
                        {
                            this.AwaitingHistory[e.Msg.whoAmI] = false;
                            ply.SendTileSquareCentered(X, Y, 5);
                            AdjustFurniture(ref X, ref Y, ref style2);
                            this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                            e.Handled = true;
                            return;
                        }
                        AdjustFurniture(ref X, ref Y, ref style2);
                        this.Queue(logName, X, Y, 0, Main.tile[X, Y].type, style2, Main.tile[X, Y].color());
                        return;
                    }
                    //PlaceDresser
                    if (flag == 2)
                    {
                        if (this.AwaitingHistory[e.Msg.whoAmI])
                        {
                            this.AwaitingHistory[e.Msg.whoAmI] = false;
                            ply.SendTileSquareCentered(X, Y, 5);
                            this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                            e.Handled = true;
                        }
                        else
                        {
                            this.LogEdit(1, Main.tile[X, Y], X, Y, 88, logName, new List<Vector2>(), style2);
                        }
                        return;
                    }
                    //KillDresser
                    if (flag == 3 && Main.tile[X, Y].type == 88)
                    {
                        if (this.AwaitingHistory[e.Msg.whoAmI])
                        {
                            this.AwaitingHistory[e.Msg.whoAmI] = false;
                            ply.SendTileSquareCentered(X, Y, 5);
                            AdjustFurniture(ref X, ref Y, ref style2);
                            this.CommandQueue.Add(new HistoryCommand(X, Y, ply));
                            e.Handled = true;
                            return;
                        }
                        AdjustFurniture(ref X, ref Y, ref style2);
                        this.Queue(logName, X, Y, 0, Main.tile[X, Y].type, style2, Main.tile[X, Y].color());
                        return;
                    }
                }
                break;
                case PacketTypes.PaintTile:
                {
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2);
                    var color = e.Msg.readBuffer[e.Index + 4];

                    var logName = TShock.Players[e.Msg.whoAmI].Account == null ? "unregistered" : TShock.Players[e.Msg.whoAmI].Account.Name;
                    this.Queue(logName, X, Y, 25, color, 0, Main.tile[X, Y].color());
                }
                break;
                case PacketTypes.PaintWall:
                {
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2);
                    var color = e.Msg.readBuffer[e.Index + 4];

                    var logName = TShock.Players[e.Msg.whoAmI].Account == null ? "unregistered" : TShock.Players[e.Msg.whoAmI].Account.Name;
                    this.Queue(logName, X, Y, 26, color, 0, Main.tile[X, Y].wallColor());
                }
                break;
                case PacketTypes.SignNew:
                {
                    var signI = BitConverter.ToUInt16(e.Msg.readBuffer, e.Index);
                    int X = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2);
                    int Y = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 4);
                    byte s = 0;
                    AdjustFurniture(ref X, ref Y, ref s); //Adjust coords so history picks it up, readSign() adjusts back to origin anyway
                    var logName = TShock.Players[e.Msg.whoAmI].Account == null ? "unregistered" : TShock.Players[e.Msg.whoAmI].Account.Name;
                    this.Queue(logName, X, Y, 27, data: signI, text: Main.sign[signI].text);
                }
                break;
                case PacketTypes.MassWireOperation:
                {
                    int X1 = BitConverter.ToInt16(e.Msg.readBuffer, e.Index);
                    int Y1 = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2);
                    int X2 = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 4);
                    int Y2 = BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 6);
                    var toolMode = e.Msg.readBuffer[e.Index + 8];
                    //Modes Red=1, Green=2, Blue=4, Yellow=8, Actuator=16, Cutter=32

                    var direction = Main.player[e.Msg.whoAmI].direction == 1;
                    var ply = TShock.Players[e.Msg.whoAmI];
                    var logName = ply.Account == null ? "unregistered" : ply.Account.Name;
                    int minX = X1, maxX = X2, minY = Y1, maxY = Y2;
                    var drawX = direction ? minX : maxX;
                    var drawY = direction ? maxY : minY;
                    if (X2 < X1)
                    {
                        minX = X2;
                        maxX = X1;
                    }
                    if (Y2 < Y1)
                    {
                        minY = Y2;
                        maxY = Y1;
                    }
                    int wires = 0, acts = 0;
                    //We count our own wires since the client may only be able to place a few or even none.
                    if ((toolMode & 32) == 0)
                    {
                        this.CountPlayerWires(Main.player[e.Msg.whoAmI], ref wires, ref acts);
                    }

                    for (var starty = minY; starty <= maxY; starty++)
                    {
                        this.LogAdvancedWire(drawX, starty, toolMode, logName, ref wires, ref acts);
                    }
                    for (var startx = minX; startx <= maxX; startx++)
                    {
                        if (startx == drawX)
                        {
                            continue;
                        }

                        this.LogAdvancedWire(startx, drawY, toolMode, logName, ref wires, ref acts);
                    }
                }
                break;
            }
        }
    }
    void CountPlayerWires(Player p, ref int wires, ref int acts)
    {
        wires = 0;
        acts = 0;
        for (var i = 0; i < 58; i++)
        {
            if (p.inventory[i].type == 530)
            {
                wires += p.inventory[i].stack;
            }
            if (p.inventory[i].type == 849)
            {
                acts += p.inventory[i].stack;
            }
        }
    }
    void LogAdvancedWire(int x, int y, byte mode, string account, ref int wires, ref int acts)
    {
        var delete = (mode & 32) == 32;
        if ((mode & 1) == 1 && Main.tile[x, y].wire() == delete) // RED
        {
            if (!delete)
            {
                if (wires <= 0)
                {
                    return;
                }

                wires--;
            }
            //5 6
            this.Queue(account, x, y, (byte) (delete ? 6 : 5));
        }
        if ((mode & 2) == 2 && Main.tile[x, y].wire3() == delete) // GREEN
        {
            if (!delete)
            {
                if (wires <= 0)
                {
                    return;
                }

                wires--;
            }
            //12 13
            this.Queue(account, x, y, (byte) (delete ? 13 : 12));
        }
        if ((mode & 4) == 4 && Main.tile[x, y].wire2() == delete) // BLUE
        {
            if (!delete)
            {
                if (wires <= 0)
                {
                    return;
                }

                wires--;
            }
            //10 11
            this.Queue(account, x, y, (byte) (delete ? 11 : 10));
        }
        if ((mode & 8) == 8 && Main.tile[x, y].wire4() == delete) // YELLOW
        {
            if (!delete)
            {
                if (wires <= 0)
                {
                    return;
                }

                wires--;
            }
            //16 and 17
            this.Queue(account, x, y, (byte) (delete ? 17 : 16));
        }
        if ((mode & 16) == 16 && Main.tile[x, y].actuator() == delete) // ACTUATOR
        {
            if (!delete)
            {
                if (acts <= 0)
                {
                    return;
                }

                acts--;
            }
            //8 9
            this.Queue(account, x, y, (byte) (delete ? 9 : 8));
        }
    }
    void OnInitialize(EventArgs e)
    {
        var sqlcreator = new SqlTableCreator(Database, new SqliteQueryCreator());
        sqlcreator.EnsureTableStructure(new SqlTable("History",
            new SqlColumn("Time", MySqlDbType.Int32),
            new SqlColumn("Account", MySqlDbType.VarChar) { Length = 50 },
            new SqlColumn("Action", MySqlDbType.Int32),
            new SqlColumn("XY", MySqlDbType.Int32),
            new SqlColumn("Data", MySqlDbType.Int32),
            new SqlColumn("Style", MySqlDbType.Int32),
            new SqlColumn("Paint", MySqlDbType.Int32),
            new SqlColumn("WorldID", MySqlDbType.Int32),
            new SqlColumn("Text", MySqlDbType.VarChar) { Length = 500 },
            new SqlColumn("Alternate", MySqlDbType.Int32),
            new SqlColumn("Random", MySqlDbType.Int32),
            new SqlColumn("Direction", MySqlDbType.Int32)));

        var datePath = Path.Combine(TShock.SavePath, "date.dat");
        if (!File.Exists(datePath))
        {
            File.WriteAllText(datePath, Date.ToString());
        }
        else
        {
            if (!DateTime.TryParse(File.ReadAllText(datePath), out Date))
            {
                Date = DateTime.UtcNow;
                File.WriteAllText(datePath, Date.ToString());
            }
        }
        this.CommandQueueThread = new Thread(this.QueueCallback!);
        this.CommandQueueThread.Start();
    }

    void OnSaveWorld(WorldSaveEventArgs e)
    {
        new SaveCommand(Actions.ToArray()).Execute();
        Actions.Clear();
    }

    void QueueCallback(object t)
    {
        while (!Netplay.Disconnect)
        {
            try
            {
                if (!this.CommandQueue.TryTake(out var command, -1, this.Cancel.Token))
                {
                    return;
                }

                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    command.Error(GetString("发生错误.检查日志以了解更多详细信息."));
                    TShock.Log.ConsoleError(ex.ToString());
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    void HistoryCmd(CommandArgs e)
    {
        if (e.Parameters.Count > 0)
        {
            if (e.Parameters.Count != 2 && e.Parameters.Count != 3)
            {
                e.Player.SendErrorMessage(GetString("用法错误! 正确用法: \n/history [账号名] [时间] [范围]"));
                return;
            }
            var radius = 10000;
            if (!TShock.Utils.TryParseTime(e.Parameters[1], out int time) || time <= 0)
            {
                e.Player.SendErrorMessage(GetString("时间无效."));
            }
            else if (e.Parameters.Count == 3 && (!int.TryParse(e.Parameters[2], out radius) || radius <= 0))
            {
                e.Player.SendErrorMessage(GetString("范围无效."));
            }
            else
            {
                this.CommandQueue.Add(new InfoCommand(e.Parameters[0], time, radius, e.Player));
            }
        }
        else
        {
            if (e.Player.Index == -1)
            {
                TShock.Log.ConsoleError(GetString("单独的/history命令只能在服务器内使用."));
                TShock.Log.ConsoleError(GetString("你也可以在后台使用/history <账号名> <时间> <范围>"));
                return;
            }
            else
            {
                e.Player.SendMessage(GetString("敲击(放置)一个方块查询这个坐标的历史记录."), Color.LimeGreen);
                this.AwaitingHistory[e.Player.Index] = true;
            }

        }
    }
    void Reenact(CommandArgs e)
    {
        if (e.Parameters.Count != 2 && e.Parameters.Count != 3)
        {
            e.Player.SendErrorMessage(GetString("用法错误! 正确用法: /reenact <账号名> <时间> [范围]"));
            return;
        }
        var radius = 10000;
        if (!TShock.Utils.TryParseTime(e.Parameters[1], out int time) || time <= 0)
        {
            e.Player.SendErrorMessage(GetString("时间无效."));
        }
        else if (e.Parameters.Count == 3 && (!int.TryParse(e.Parameters[2], out radius) || radius <= 0))
        {
            e.Player.SendErrorMessage(GetString("范围无效."));
        }
        else
        {
            this.CommandQueue.Add(new RollbackCommand(e.Parameters[0], time, radius, e.Player, true));
        }
    }
    void Rollback(CommandArgs e)
    {
        if (e.Parameters.Count != 2 && e.Parameters.Count != 3)
        {
            e.Player.SendErrorMessage(GetString("用法错误! 正确用法: /rollback <账号名> <时间> [范围]"));
            return;
        }
        var radius = 10000;
        if (!TShock.Utils.TryParseTime(e.Parameters[1], out int time) || time <= 0)
        {
            e.Player.SendErrorMessage(GetString("时间无效."));
        }
        else if (e.Parameters.Count == 3 && (!int.TryParse(e.Parameters[2], out radius) || radius <= 0))
        {
            e.Player.SendErrorMessage(GetString("范围无效."));
        }
        else
        {
            this.CommandQueue.Add(new RollbackCommand(e.Parameters[0], time, radius, e.Player));
        }
    }
    void Prune(CommandArgs e)
    {
        if (e.Parameters.Count != 1)
        {
            e.Player.SendErrorMessage(GetString("用法错误! 正确用法: /prunehist <时间>"));
            return;
        }
        if (TShock.Utils.TryParseTime(e.Parameters[0], out int time) && time > 0)
        {
            this.CommandQueue.Add(new PruneCommand(time, e.Player));
        }
        else
        {
            e.Player.SendErrorMessage(GetString("时间无效."));
        }
    }
    void Undo(CommandArgs e)
    {
        if (UndoCommand.LastRollBack != null)
        {
            this.CommandQueue.Add(new UndoCommand(e.Player));
        }
        else
        {
            e.Player.SendErrorMessage(GetString("没有要撤消的内容!"));
        }
    }

    #region 使用指令清理数据库
    private void ResetCmd(CommandArgs e)
    {
        if (e.Parameters.Count > 0) { e.Player.SendErrorMessage(GetString("用法错误! 正确用法: /hreset")); return; }
        else
        {
            if (!e.Player.HasPermission("history.admin"))
            {
                e.Player.SendErrorMessage(GetString("你没有重置【History】数据表的权限。"));
                TShock.Log.ConsoleInfo(GetString($"{e.Player.Name}试图执行 [History] 数据重置指令"));
                return;
            }
            else
            {
                ClearAllData(e);
                return;
            }
        }
    }

    private static void ClearAllData(CommandArgs args)
    {
        if (HistoryCommand.ClearData())
        {
            TShock.Utils.Broadcast(GetString($"[History] 数据库已被 [{args.Player.Name}] 成功清除"), Color.DarkRed);
        }
        else
        {
            TShock.Log.ConsoleInfo(GetString("[History] 数据库清除失败"));
        }
    }
    #endregion
}