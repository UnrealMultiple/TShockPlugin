using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using ConfigPlugin;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Timer = System.Timers.Timer;

namespace ItemTextPlugin;

[ApiVersion(2, 1)]
public class InfoPlugin : TerrariaPlugin
{
    private readonly Dictionary<string, int> lastSelectedItems = new();
    private ItemTextConfig config;
    private readonly Timer itemCheckTimer;  // Timer para revisar el cambio de ítem

    public InfoPlugin(Main game) : base(game)
    {
        var configDirectory = Path.Combine(TShock.SavePath, "ItemDeco");
        var configPath = Path.Combine(configDirectory, "ItemTextConfig.json");

        // Cargar configuración o regenerar si está desactualizada
        this.config = ConfigManager.LoadConfig(
            configDirectory,
            "ItemTextConfig.json",
            new ItemTextConfig
            {
                ShowName = true,
                ShowDamage = true,
                DamageText = "Damage",  // Valor predeterminado
                DefaultColor = new ColorConfig { r = 255, g = 255, b = 255 },
                RarityColors = new Dictionary<int, ColorConfig>
                {
                    { -1, new ColorConfig { r = 169, g = 169, b = 169 } }, // Gris para sin rareza
                    { 0, new ColorConfig { r = 255, g = 255, b = 255 } },  // Blanco
                    { 1, new ColorConfig { r = 0, g = 128, b = 0 } },      // Verde
                    { 2, new ColorConfig { r = 0, g = 112, b = 221 } },    // Azul
                    { 3, new ColorConfig { r = 128, g = 0, b = 128 } },    // Morado
                    { 4, new ColorConfig { r = 255, g = 128, b = 0 } },    // Naranja
                    { 5, new ColorConfig { r = 255, g = 0, b = 0 } },      // Rojo
                    { 6, new ColorConfig { r = 255, g = 215, b = 0 } },    // Amarillo (Mythical)
                    { 7, new ColorConfig { r = 255, g = 105, b = 180 } },  // Rosa (Para misiones)
                    { 8, new ColorConfig { r = 255, g = 215, b = 0 } },    // Dorado (Especial)
                    { 9, new ColorConfig { r = 0, g = 255, b = 255 } },    // Cian (Único)
                    { 10, new ColorConfig { r = 255, g = 105, b = 180 } }, // Magenta (Raro)
                    { 11, new ColorConfig { r = 75, g = 0, b = 130 } },    // Índigo (Épico)
                }
            });

        // Verificar si la configuración está desactualizada
        if (this.IsConfigOutdated(this.config))
        {
            // Borrar la configuración desactualizada
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }

            // Regenerar configuración predeterminada
            this.config = ConfigManager.LoadConfig(
                configDirectory,
                "ItemTextConfig.json",
                new ItemTextConfig
                {
                    ShowName = true,
                    ShowDamage = true,
                    DamageText = "Damage",  // Valor predeterminado
                    DefaultColor = new ColorConfig { r = 255, g = 255, b = 255 },
                    RarityColors = new Dictionary<int, ColorConfig>()
                });
            TShock.Log.Info("[ ITEMS DECORATION ] Configuración desactualizada detectada. Archivo regenerado.");
        }

        // Inicializar el Timer para verificar constantemente el cambio de ítem
        this.itemCheckTimer = new Timer(100); // Revisa cada 100 ms
        this.itemCheckTimer.Elapsed += this.CheckItemChanges;
        this.itemCheckTimer.Start();
    }

    private bool IsConfigOutdated(ItemTextConfig config)
    {
        // Comprobar si las rarezas están definidas
        return this.config.RarityColors == null || this.config.RarityColors.Count == 0;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnNetGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
        Commands.ChatCommands.Add(new Command("itemSuffix.reload", this.ReloadConfig, "reload"));
    }

    private void OnNetGreetPlayer(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && !this.lastSelectedItems.ContainsKey(player.Name))
        {
            this.lastSelectedItems.Add(player.Name, player.TPlayer.inventory[player.TPlayer.selectedItem].type);
        }
    }

    private void OnServerLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && this.lastSelectedItems.ContainsKey(player.Name))
        {
            this.lastSelectedItems.Remove(player.Name);
        }
    }

    private void CheckItemChanges(object? sender, ElapsedEventArgs e)
    {
        foreach (var player in TShock.Players)
        {
            if (player != null)
            {
                var selectedItem = player.TPlayer.inventory[player.TPlayer.selectedItem];

                // Verificamos si el ítem ha cambiado
                if (!this.lastSelectedItems.ContainsKey(player.Name))
                {
                    this.lastSelectedItems.Add(player.Name, selectedItem.type);
                }
                else if (selectedItem.type != this.lastSelectedItems[player.Name])
                {
                    // Solo enviar mensaje si el ítem ha cambiado
                    if (this.config.ShowName || this.config.ShowDamage)
                    {
                        var message = "";
                        if (this.config.ShowName)
                        {
                            message += selectedItem.Name;
                        }
                        if (this.config.ShowDamage && selectedItem.damage > 0)
                        {
                            if (!string.IsNullOrEmpty(message))
                            {
                                message += " - ";
                            }

                            message += $"{this.config.DamageText}: {selectedItem.damage}";
                        }

                        // Enviar el mensaje de texto flotante
                        this.SendFloatingMsg(player, message, this.GetColorByRarity(selectedItem.rare));
                    }

                    // Actualizamos el ítem seleccionado para el jugador
                    this.lastSelectedItems[player.Name] = selectedItem.type;
                }
            }
        }
    }

    private void SendFloatingMsg(TSPlayer plr, string msg, Microsoft.Xna.Framework.Color color)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            NetMessage.SendData((int)PacketTypes.CreateCombatTextExtended, -1, -1,
                Terraria.Localization.NetworkText.FromLiteral(msg),
                (int)color.PackedValue,
                plr.TPlayer.position.X + 16, plr.TPlayer.position.Y + 33);
        }
    }

    private Microsoft.Xna.Framework.Color GetColorByRarity(int rarity)
    {
        return this.config.RarityColors.TryGetValue(rarity, out var colorConfig)
            ? new Microsoft.Xna.Framework.Color(colorConfig.r, colorConfig.g, colorConfig.b)
            : new Microsoft.Xna.Framework.Color(this.config.DefaultColor.r, this.config.DefaultColor.g, this.config.DefaultColor.b);
    }

    private void ReloadConfig(CommandArgs args)
    {
        var configDirectory = Path.Combine(TShock.SavePath, "ItemDeco");
        this.config = ConfigManager.LoadConfig(
            configDirectory,
            "ItemTextConfig.json",
            new ItemTextConfig
            {
                ShowName = true,
                ShowDamage = true,
                DamageText = "Damage",  // Valor predeterminado
                DefaultColor = new ColorConfig { r = 255, g = 255, b = 255 },
                RarityColors = new Dictionary<int, ColorConfig>()
            });

        args.Player.SendMessage("[ ITEMS DECORATION ] La configuración ha sido recargada.", Microsoft.Xna.Framework.Color.LightGreen);
        TShock.Log.Info("[ ITEMS DECORATION ] La configuración ha sido recargada correctamente.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.itemCheckTimer.Stop();
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnNetGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);
        }
        base.Dispose(disposing);
    }
}

public class ItemTextConfig
{
    public bool ShowName { get; set; }
    public bool ShowDamage { get; set; }
    public string DamageText { get; set; } = string.Empty;  // Texto configurable para el daño
    public ColorConfig DefaultColor { get; set; } = new();
    public Dictionary<int, ColorConfig> RarityColors { get; set; } = new();
}

public class ColorConfig
{
    public int r { get; set; }
    public int g { get; set; }
    public int b { get; set; }
}
