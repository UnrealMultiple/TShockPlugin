using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using Microsoft.Xna.Framework;
using System.Reflection;
using ConfigPlugin;

namespace ItemChatPlugin
{
    [ApiVersion(2, 1)]
    public class ItemChatPlugin : TerrariaPlugin
    {
        public static ConfigChat Config;

        public ItemChatPlugin(Main game) : base(game)
        {
            string configDirectory = Path.Combine(TShock.SavePath, "ItemDeco");
            Config = ConfigManager.LoadConfig(
                configDirectory,
                "ItemChatConfig.json",
                new ConfigChat
                {
                    CONFIGURATION = new Configuration
                    {
                        ShowItem = true,
                        ShowDamage = true,
                        ItemColor = new Color(255, 255, 255), // Blanco por defecto
                        DamageColor = new Color(0, 255, 255)  // Azul claro por defecto
                    }
                });
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, OnServerChat);
        }

        private void OnServerChat(ServerChatEventArgs args)
        {
            if (args.Text.StartsWith("/") || args.Text.StartsWith(".") || args.Text.StartsWith("!"))
                return;

            TSPlayer player = TShock.Players[args.Who];
            if (player == null) return;

            string message = args.Text;
            message = ItemSuffixPlaceholder.ReplacePlaceholderWithItem(player, message);

            PropertyInfo? propertyInfo = args.GetType().GetProperty("Text");
            propertyInfo?.SetValue(args, message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ServerApi.Hooks.ServerChat.Deregister(this, OnServerChat);

            base.Dispose(disposing);
        }
    }

    public static class ItemSuffixPlaceholder
    {
        public static string ReplacePlaceholderWithItem(TSPlayer player, string message)
        {
            var selectedItem = player.TPlayer.inventory[player.TPlayer.selectedItem];
            string suffix = "";

            if (selectedItem != null && selectedItem.type > 0)
            {
                // Convertir colores a formato hexadecimal
                string itemColorHex = ItemChatPlugin.Config.CONFIGURATION.ItemColor.ToHex();
                string damageColorHex = ItemChatPlugin.Config.CONFIGURATION.DamageColor.ToHex();

                // Agregar el ítem si está habilitado
                if (ItemChatPlugin.Config.CONFIGURATION.ShowItem)
                {
                    suffix += $"[i:{selectedItem.netID}]";
                }

                // Agregar el daño si está habilitado
                if (ItemChatPlugin.Config.CONFIGURATION.ShowDamage && selectedItem.damage > 0)
                {
                    if (!string.IsNullOrEmpty(suffix))
                        suffix += " ";

                    suffix += $"[c/{damageColorHex}:{selectedItem.damage}]";
                }
            }

            return !string.IsNullOrEmpty(suffix) ? $"[ {suffix} ] {message}." : message;
        }
    }

    public class ConfigChat
    {
        public Configuration CONFIGURATION { get; set; }
    }

    public class Configuration
    {
        public bool ShowItem { get; set; }
        public bool ShowDamage { get; set; }
        public Color ItemColor { get; set; }
        public Color DamageColor { get; set; }
    }

    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public Color() { }

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public string ToHex()
        {
            return $"{R:X2}{G:X2}{B:X2}";
        }
    }
}