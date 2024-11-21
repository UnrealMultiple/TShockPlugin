using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using Microsoft.Xna.Framework;
using System.Reflection;
using ConfigPlugin;

namespace ItemChatPlugin;

[ApiVersion(2, 1)]
public class ItemChatPlugin : TerrariaPlugin
{
    public static ConfigChat Config = null!;

    public ItemChatPlugin(Main game) : base(game)
    {
        var configDirectory = Path.Combine(TShock.SavePath, "ItemDeco");
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
        {
            return;
        }

        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        var message = args.Text;
        message = ItemSuffixPlaceholder.ReplacePlaceholderWithItem(player, message);

        var propertyInfo = args.GetType().GetProperty("Text");
        propertyInfo?.SetValue(args, message);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, OnServerChat);
        }

        base.Dispose(disposing);
    }
}

public static class ItemSuffixPlaceholder
{
    public static string ReplacePlaceholderWithItem(TSPlayer player, string message)
    {
        var selectedItem = player.TPlayer.inventory[player.TPlayer.selectedItem];
        var suffix = "";

        if (selectedItem != null && selectedItem.type > 0)
        {
            // Convertir colores a formato hexadecimal
            var itemColorHex = ItemChatPlugin.Config.CONFIGURATION.ItemColor.ToHex();
            var damageColorHex = ItemChatPlugin.Config.CONFIGURATION.DamageColor.ToHex();

            // Agregar el ítem si está habilitado
            if (ItemChatPlugin.Config.CONFIGURATION.ShowItem)
            {
                suffix += $"[i:{selectedItem.netID}]";
            }

            // Agregar el daño si está habilitado
            if (ItemChatPlugin.Config.CONFIGURATION.ShowDamage && selectedItem.damage > 0)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    suffix += " ";
                }

                suffix += $"[c/{damageColorHex}:{selectedItem.damage}]";
            }
        }

        return !string.IsNullOrEmpty(suffix) ? $"[ {suffix} ] {message}." : message;
    }
}

public class ConfigChat
{
    public Configuration CONFIGURATION { get; set; } = new();
}

public class Configuration
{
    public bool ShowItem { get; set; }
    public bool ShowDamage { get; set; }
    public Color ItemColor { get; set; } = new();
    public Color DamageColor { get; set; } = new();
}

public class Color
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }

    public Color() { }

    public Color(int r, int g, int b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public string ToHex()
    {
        return $"{this.R:X2}{this.G:X2}{this.B:X2}";
    }
}