using System.Text;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace History;

public class Action
{
    public string? account;
    public byte action;
    public ushort data;
    public byte style;
    public short paint;
    public int time;
    public int x;
    public int y;
    public string? text;
    public int alt;
    public int random;
    public bool direction;

    public void Reenact()
    {
        switch (this.action)
        {
            case 0:
            case 4://del tile
                if (Main.tile[this.x, this.y].active())
                {
                    Main.tile[this.x, this.y].active(false);
                    Main.tile[this.x, this.y].type = 0;
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 1://add tile
                   //Don't place if already there
                if (Main.tile[this.x, this.y].active() && Main.tile[this.x, this.y].type == this.data)
                {
                    break;
                }

                WorldGen.PlaceTile(this.x, this.y, this.data, false, true, style: this.style);
                if (Main.tileFrameImportant[this.data])
                {
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 8);
                }
                else
                {
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }

                break;
            case 2://del wall
                if (Main.tile[this.x, this.y].wall != 0)
                {
                    Main.tile[this.x, this.y].wall = 0;
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 3://add wall
                if (Main.tile[this.x, this.y].wall == 0)
                {
                    Main.tile[this.x, this.y].wall = (byte) this.data;
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 5://placewire
                if (!Main.tile[this.x, this.y].wire())
                {
                    WorldGen.PlaceWire(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 6://killwire
                if (Main.tile[this.x, this.y].wire())
                {
                    WorldGen.KillWire(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 7://poundtile
                WorldGen.PoundTile(this.x, this.y);
                TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                break;
            case 8://placeactuator
                if (!Main.tile[this.x, this.y].actuator())
                {
                    WorldGen.PlaceActuator(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 9://killactuator
                if (Main.tile[this.x, this.y].actuator())
                {
                    WorldGen.KillActuator(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 10://placewire2
                if (!Main.tile[this.x, this.y].wire2())
                {
                    WorldGen.PlaceWire2(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 11://killwire2
                if (Main.tile[this.x, this.y].wire2())
                {
                    WorldGen.KillWire2(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 12://placewire3
                if (!Main.tile[this.x, this.y].wire3())
                {
                    WorldGen.PlaceWire3(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 13://killwire3
                if (Main.tile[this.x, this.y].wire3())
                {
                    WorldGen.KillWire3(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 14://slopetile
                WorldGen.SlopeTile(this.x, this.y, this.data);
                TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                break;
            case 15:
                //Uh wtf does "frame track" mean
                //Too lazy to find atm
                break;
            case 16:
                if (!Main.tile[this.x, this.y].wire4())
                {
                    WorldGen.PlaceWire4(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 17:
                if (Main.tile[this.x, this.y].wire4())
                {
                    WorldGen.KillWire4(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 25://paint tile
                if (Main.tile[this.x, this.y].active())
                {
                    Main.tile[this.x, this.y].color((byte) this.data);
                    NetMessage.SendData(63, -1, -1, NetworkText.Empty, this.x, this.y, this.data, 0f, 0);
                }
                break;
            case 26://paint wall
                if (Main.tile[this.x, this.y].wall > 0)
                {
                    Main.tile[this.x, this.y].wallColor((byte) this.data);
                    NetMessage.SendData(64, -1, -1, NetworkText.Empty, this.x, this.y, this.data, 0f, 0);
                }
                break;
            case 27://update sign
                break;//Does not save the new text currently, can't reenact.
        }
    }
    public void Rollback()
    {
        switch (this.action)
        {
            case 0:
            case 4://del tile
            case 20://trykilltile 
                if (Main.tileSand[this.data])//sand falling compensation (need to check up for top of sand)
                {
                    var newY = this.y;
                    while (newY > 0 && Main.tile[this.x, newY].active() && Main.tile[this.x, newY].type == this.data)
                    {
                        newY--;
                    }
                    if (Main.tile[this.x, newY].active())
                    {
                        break;
                    }

                    this.y = newY;
                }
                else if (this.data == 5)//tree, grow another?
                {
                    WorldGen.GrowTree(this.x, this.y + 1);
                    break;
                }
                else if (this.data == 2 || this.data == 23 || this.data == 60 || this.data == 70 || this.data == 109 || this.data == 199)// grasses need to place manually, not from placeTile
                {
                    Main.tile[this.x, this.y].type = this.data;
                    Main.tile[this.x, this.y].color((byte) (this.paint & 127));
                    Main.tile[this.x, this.y].active(true);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                    break;
                }
                //maybe already repaired?
                if (Main.tile[this.x, this.y].active() && Main.tile[this.x, this.y].type == this.data)
                {
                    if (this.data == 314 || this.data == 395)
                    {
                        goto frameOnly;
                    }

                    break;
                }
                if (Terraria.ObjectData.TileObjectData.CustomPlace(this.data, this.style) && this.data != 82)
                {
                    WorldGen.PlaceObject(this.x, this.y, this.data, false, style: this.style, alternate: this.alt, random: this.random, direction: this.direction ? 1 : -1);
                }
                else
                {
                    WorldGen.PlaceTile(this.x, this.y, this.data, false, true, -1, style: this.style);
                }

                History.PaintFurniture(this.data, this.x, this.y, (byte) (this.paint & 127));

            frameOnly:
                //restore slopes
                if ((this.paint & 128) == 128)
                {
                    Main.tile[this.x, this.y].halfBrick(true);
                }
                else if (this.data == 314)
                {
                    Main.tile[this.x, this.y].frameX = (short) (this.style - 1);
                    Main.tile[this.x, this.y].frameY = (short) ((this.paint >> 8) - 1);
                }
                else
                {
                    Main.tile[this.x, this.y].slope((byte) (this.paint >> 8));
                }

                //restore sign text
                if (this.data == 55 || this.data == 85 || this.data == 425)
                {
                    var signI = Sign.ReadSign(this.x, this.y);
                    if (signI >= 0)
                    {
                        Sign.TextSign(signI, this.text);
                    }
                }
                //Mannequins
                else if (this.data == 128 || this.data == 269)
                {
                    //x,y should be bottom left, Direction is already done via PlaceObject so we add the item values.
                    Main.tile[this.x, this.y - 2].frameX += (short) (this.paint * 100);
                    Main.tile[this.x, this.y - 1].frameX += (short) ((this.alt & 0x3FF) * 100);
                    Main.tile[this.x, this.y].frameX += (short) ((this.alt >> 10) * 100);
                }
                // Restore Weapon Rack if it had a netID
                else if (this.data == 334 && this.alt > 0)
                {
                    var mask = 5000;// +(direction ? 15000 : 0);
                    Main.tile[this.x - 1, this.y].frameX = (short) (this.alt + mask + 100);
                    Main.tile[this.x, this.y].frameX = (short) (this.paint + mask + 5000);
                }
                // Restore Item Frame
                else if (this.data == 395)
                {
                    /*TileEntity TE;
						// PlaceObject should already place a blank entity.
						if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TE))
						{
							Console.WriteLine("Frame had Entity, changing item.");
							TEItemFrame frame = (TEItemFrame)TE;
							frame.item.netDefaults(alt);
							frame.item.Prefix(random);
							frame.item.stack = 1;
							NetMessage.SendData(86, -1, -1, "", frame.ID, (float)x, (float)y, 0f, 0, 0, 0);
						}
						else
							Console.WriteLine("This Frame restore had no entity");*/
                }
                //Send larger area for furniture
                if (Main.tileFrameImportant[this.data])
                {
                    if (this.data == 104)
                    {
                        TSPlayer.All.SendTileSquareCentered(this.x, this.y - 2, 8);
                    }
                    else
                    {
                        TSPlayer.All.SendTileSquareCentered(this.x, this.y, 8);//This can be very large, or too small in some cases
                    }
                }
                else
                {
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }

                break;
            case 1://add tile
                var delete = Main.tile[this.x, this.y].active();
                if (!delete && Main.tileSand[this.data])//sand falling compensation (it may have fallen down)
                {
                    var newY = this.y + 1;
                    while (newY < Main.maxTilesY - 1 && !Main.tile[this.x, newY].active())
                    {
                        newY++;
                    }
                    if (Main.tile[this.x, newY].type == this.data)
                    {
                        this.y = newY;
                        delete = true;
                    }
                }
                if (delete)
                {
                    WorldGen.KillTile(this.x, this.y, false, false, true);
                    NetMessage.SendData(17, -1, -1, NetworkText.Empty, 0, this.x, this.y);
                }
                break;
            case 2://del wall
                if (Main.tile[this.x, this.y].wall != this.data) //change if not what was deleted
                {
                    Main.tile[this.x, this.y].wall = (byte) this.data;
                    Main.tile[this.x, this.y].wallColor((byte) this.paint);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 3://add wall
                if (Main.tile[this.x, this.y].wall != 0)
                {
                    Main.tile[this.x, this.y].wall = 0;
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 5://placewire
                if (Main.tile[this.x, this.y].wire())
                {
                    WorldGen.KillWire(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 6://killwire
                if (!Main.tile[this.x, this.y].wire())
                {
                    WorldGen.PlaceWire(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 7://poundtile
                WorldGen.PoundTile(this.x, this.y);
                TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                break;
            case 8://placeactuator
                if (Main.tile[this.x, this.y].actuator())
                {
                    WorldGen.KillActuator(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 9://killactuator
                if (!Main.tile[this.x, this.y].actuator())
                {
                    WorldGen.PlaceActuator(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 10://placewire2
                if (Main.tile[this.x, this.y].wire2())
                {
                    WorldGen.KillWire2(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 11://killwire2
                if (!Main.tile[this.x, this.y].wire2())
                {
                    WorldGen.PlaceWire2(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 12://placewire3
                if (Main.tile[this.x, this.y].wire3())
                {
                    WorldGen.KillWire3(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 13://killwire3
                if (!Main.tile[this.x, this.y].wire3())
                {
                    WorldGen.PlaceWire3(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 14://slopetile
                Main.tile[this.x, this.y].slope((byte) (this.paint >> 8));
                Main.tile[this.x, this.y].halfBrick((this.paint & 128) == 128);
                TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                break;
            case 15: //frame track
                     //see above
                break;
            case 16:
                if (Main.tile[this.x, this.y].wire4())
                {
                    WorldGen.KillWire4(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 17:
                if (!Main.tile[this.x, this.y].wire4())
                {
                    WorldGen.PlaceWire4(this.x, this.y);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 21:
                if (Main.tile[this.x, this.y].active())
                {
                    var prevTile = this.data & 0xFFFF;
                    //int placedTile = (data >> 16) & 0xFFFF;

                    WorldGen.PlaceTile(this.x, this.y, prevTile, false, true, -1, style: this.style);
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 22:
                var prevWall = this.data & 0xFFFF;
                //int placedWall = (data >> 16) & 0xFFFF;
                if (Main.tile[this.x, this.y].wall != prevWall) //change if not what was replaced
                {
                    Main.tile[this.x, this.y].wall = (byte) prevWall;
                    TSPlayer.All.SendTileSquareCentered(this.x, this.y, 1);
                }
                break;
            case 25://paint tile
                if (Main.tile[this.x, this.y].active())
                {
                    Main.tile[this.x, this.y].color((byte) this.paint);
                    NetMessage.SendData(63, -1, -1, NetworkText.Empty, this.x, this.y, this.paint, 0f, 0);
                }
                break;
            case 26://paint wall
                if (Main.tile[this.x, this.y].wall > 0)
                {
                    Main.tile[this.x, this.y].wallColor((byte) this.paint);
                    NetMessage.SendData(64, -1, -1, NetworkText.Empty, this.x, this.y, this.paint, 0f, 0);
                }
                break;
            case 27://updatesign
                var sI = Sign.ReadSign(this.x, this.y); //This should be an existing sign, but use coords instead of index anyway
                if (sI >= 0)
                {
                    Sign.TextSign(sI, this.text);
                }
                break;
        }
    }
    public override string ToString()
    {
        var dateTime = History.Date.AddSeconds(this.time);
        var date = string.Format("{0}-{1} @ {3}:{4}:{5} (UTC):",
            dateTime.Month, dateTime.Day, dateTime.Year,
            dateTime.Hour, dateTime.Minute.ToString("00"), dateTime.Second.ToString("00"));

        var timeDiff = DateTime.UtcNow - dateTime;
        var dhms = new StringBuilder();
        if (timeDiff.Days != 0)
        {
            dhms.Append(timeDiff.Days + "天");
        }
        if (timeDiff.Hours != 0)
        {
            dhms.Append(timeDiff.Hours + "小时");
        }
        if (timeDiff.Minutes != 0)
        {
            dhms.Append(timeDiff.Minutes + "分钟");
        }
        if (timeDiff.Seconds != 0)
        {
            dhms.Append(timeDiff.Seconds + "秒");
        }

        return this.action switch
        {
            0 or 4 or 20 => string.Format("{0} {1} 破坏方块 {2}. ({3})", date, this.account, this.data, dhms),
            1 => string.Format("{0} {1} 放置方块 {2}. ({3})", date, this.account, this.data, dhms),
            2 => string.Format("{0} {1} 破坏墙 {2}. ({3})", date, this.account, this.data, dhms),
            3 => string.Format("{0} {1} 放置墙 {2}. ({3})", date, this.account, this.data, dhms),
            5 or 10 or 12 or 16 => string.Format("{0} {1} 放置电线. ({2})", date, this.account, dhms),
            6 or 11 or 13 or 17 => string.Format("{0} {1} 破坏电线. ({2})", date, this.account, dhms),
            7 or 14 => string.Format("{0} {1} 改变图格形状. ({2})", date, this.account, dhms),
            8 => string.Format("{0} {1} 放置致动器. ({2})", date, this.account, dhms),
            21 => string.Format("{0} {1} 放置物块 ({2})", date, this.account, dhms),
            22 => string.Format("{0} {1} 放置墙. ({2})", date, this.account, dhms),
            25 or 26 => string.Format("{0} {1} 涂漆物块/墙. ({2})", date, this.account, dhms),
            27 => string.Format("{0} {1} 修改了告示牌文字. ({2})", date, this.account, dhms),
            _ => "",
        };
    }
}