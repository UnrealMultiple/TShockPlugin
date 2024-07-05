using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Main = Terraria.Main;
using Version = System.Version;
using Color = Microsoft.Xna.Framework.Color;
using System.Net.Sockets;

namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        private TerrariaPlugin join;
        public override string Author => "cmgy";
        public override string Description => "";
        public override string Name => "BedSet 床设置";
        public override Version Version => new(1, 0, 0, 1);
        public Plugin(Main game) : base(game) { }

        //hook
        public override void Initialize()
        {
            GetDataHandlers.PlayerUpdate.Register(backbed);//使用回城物品  set
            PlayerHooks.PlayerPostLogin += PlayerHooks_PlayerPostLogin;//登录返回床位置  login
            GetDataHandlers.KillMe.Register(deadafter);//死亡返回床位置 death
        }
        //set
        private async void backbed(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
        {
            //魔镜
            if (e.Player.TPlayer.controlUseItem &&
                (e.Player.SelectedItem.netID == 50 ||
                e.Player.SelectedItem.netID == 3199 ||
                e.Player.SelectedItem.netID == 3124 ||
                e.Player.SelectedItem.netID == 5358
                )
                )
            {
                await Task.Delay(300);
                string path = $@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt";
                if (!Directory.Exists(@".\重生点"))
                {
                    Directory.CreateDirectory(@".\重生点");
                }
                if (!File.Exists(path))
                {
                    FileStream bed = File.Create($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                    StreamWriter xy = new StreamWriter(bed);
                    xy.Close();
                    bed.Close();
                }
                FileStream clear = new FileStream($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt", FileMode.Truncate, FileAccess.ReadWrite);
                clear.Close();
                e.Player.SendData(PacketTypes.CreateCombatTextExtended, $"updata", 255255255, e.Player.X, e.Player.Y, 3000);
                NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(e.Player.TPlayer.Center, 159), e.Player.Index, -1);
                StreamWriter set = File.AppendText($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                set.WriteLine($"{e.Player.TileX+1}    {e.Player.TileY}");
                set.Close();
            }
            //回忆
            if (e.Player.TPlayer.controlUseItem &&
                (e.Player.SelectedItem.netID == 2350 ||
                e.Player.SelectedItem.netID == 4870)
                )
            {
                await Task.Delay(300);
                string path = $@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt";
                if (!Directory.Exists(@".\重生点"))
                {
                    Directory.CreateDirectory(@".\重生点");
                }
                if (!File.Exists(path))
                {
                    FileStream bed = File.Create($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                    StreamWriter xy = new StreamWriter(bed);
                    xy.Close();
                    bed.Close();
                }
                FileStream clear = new FileStream($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt", FileMode.Truncate, FileAccess.ReadWrite);
                clear.Close();
                e.Player.SendData(PacketTypes.CreateCombatTextExtended, $"updata", 255255255, e.Player.X, e.Player.Y, 3000);
                NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(e.Player.TPlayer.Center, 159), e.Player.Index, -1);
                StreamWriter set = File.AppendText($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                set.WriteLine($"{e.Player.TileX+1}    {e.Player.TileY}");
                set.Close();
            }
        }
        //login
        private async void PlayerHooks_PlayerPostLogin(PlayerPostLoginEventArgs e)
        {
            while (true)
            {
                await Task.Delay(1000);
                if (e.Player.X != 0)
                {
                    string path = $@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt";
                    if (!Directory.Exists(@".\重生点"))
                    {
                        Directory.CreateDirectory(@".\重生点");
                    }
                    if (!File.Exists(path))
                    {
                        FileStream bed = File.Create($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                        StreamWriter xy = new StreamWriter(bed);
                        xy.Close();
                        bed.Close();
                    }
                    string[] line = File.ReadAllLines($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                    if (line.Length > 0)
                    {
                        string tile = line[0];
                        string x = tile.Remove(4, tile.Length - 4);
                        string y = tile.Remove(0, tile.Length - 4);
                        string x2 = x.Replace(" ", "");
                        string y2 = y.Replace(" ", "");
                        int x3 = Convert.ToInt32(x2);
                        int y3 = Convert.ToInt32(y2);
                        e.Player.SendInfoMessage($" 传送至重生点,记得重新保存床重生点");
                        e.Player.Teleport(x3 * 16, y3 * 16, 6);
                        Color c = new Color(0, 169, 255);
                        e.Player.SendData(PacketTypes.CreateCombatTextExtended, "return", (int)c.packedValue, e.Player.X, e.Player.Y - 16);
                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(e.Player.TPlayer.Center, 114), e.Player.Index, -1); 
                    }
                    break;
                }
            }
        }
        //death
        private async void deadafter(object? sender, GetDataHandlers.KillMeEventArgs e)
        {
            while (true)
            {
                await Task.Delay(500);
                if (e.Player.Dead == false)
                {
                    string path = $@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt";
                    if (!Directory.Exists(@".\重生点"))
                    {
                        Directory.CreateDirectory(@".\重生点");
                    }
                    if (!File.Exists(path))
                    {
                        FileStream bed = File.Create($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                        StreamWriter xy = new StreamWriter(bed);
                        xy.Close();
                        bed.Close();
                    }
                    string[] line = File.ReadAllLines($@".\重生点\{e.Player.Name}{Main.worldName}重生点.txt");
                    if (line.Length > 0)
                    {
                        string tile = line[0];
                        string x = tile.Remove(4, tile.Length - 4);
                        string y = tile.Remove(0, tile.Length - 4);
                        string x2 = x.Replace(" ", "");
                        string y2 = y.Replace(" ", "");
                        int x3 = Convert.ToInt32(x2);
                        int y3 = Convert.ToInt32(y2);
                        e.Player.Teleport(x3 * 16, y3 * 16, 6);
                        Color c = new Color(0, 169, 255);
                        e.Player.SendData(PacketTypes.CreateCombatTextExtended, "return", (int)c.packedValue, e.Player.X, e.Player.Y - 16);
                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(e.Player.TPlayer.Center, 114), e.Player.Index, -1);
                    }
                    break;
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var asm = Assembly.GetExecutingAssembly();
                Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method?.DeclaringType?.Assembly == asm);
            }
            base.Dispose(disposing);
        }

        private class List
        {
        }
    }
}