using ItemDecoration.Configured;
using LazyAPI;
using LazyAPI.Extensions;
using Microsoft.Xna.Framework;
using On.OTAPI;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace ItemDecoration;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "FrankV22, Soofa, 少司命";

    public override string Description => GetString("Show Item Decoration and More!!!");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(3, 0, 1);

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerChat.Register(this, this.OnServerChat);
        Hooks.MessageBuffer.InvokeGetData += this.MessageBuffer_InvokeGetData;
    }
    

    private void OnServerChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (args.Handled || player == null
            || args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)
            || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier)
            || string.IsNullOrWhiteSpace(args.Text)) // Verificar si el texto está vacío o es solo espacios en blanco.
        {
            return;
        }
        var msg = ReplacePlaceholderWithItem(player, args.Text);
        if (!string.IsNullOrWhiteSpace(msg)) // Verificar que el mensaje procesado no esté vacío.
        {
            TShock.Utils.Broadcast(string.Format(
                TShock.Config.Settings.ChatFormat,
                player.Group.Name,
                player.Group.Prefix,
                player.Name,
                player.Group.Suffix,
                msg),
                player.Group.R,
                player.Group.G,
                player.Group.B);
        }
        args.Handled = true;
    }

    private bool MessageBuffer_InvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {
        if (packetId != (byte) PacketTypes.PlayerUpdate) // Packet ID 13 - Item select
        {
            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }

        using var ms = new MemoryStream(instance.readBuffer);
        ms.Position = readOffset;
        using var reader = new BinaryReader(ms);
        var index = reader.ReadByte();
        var player = TShock.Players[index];

        if (player is not { Active: true } || player.Dead)
        {
            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }

        reader.BaseStream.Seek(4, SeekOrigin.Current);
        var selectSlot = reader.ReadByte();
        if (player.TPlayer.selectedItem == selectSlot)
        {
            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }

        var newSelectItem = player.TPlayer.inventory[selectSlot];
        if (Setting.Instance.ItemTextConfig.ShowName || Setting.Instance.ItemTextConfig.ShowDamage)
        {
            var message = "";
            if (Setting.Instance.ItemTextConfig.ShowName)
            {
                message += newSelectItem.Name;
            }
            if (Setting.Instance.ItemTextConfig.ShowDamage && newSelectItem.damage > 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += " - ";
                }

                message += $"{Setting.Instance.ItemTextConfig.DamageText}: {newSelectItem.damage}";
            }
            player.SendCombatText(message, GetColorByRarity(newSelectItem.rare));
        }

        if (!Setting.Instance.ItemAboveHeadConfig.ItemAboveHead)
        {
            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }

        if (newSelectItem == null || newSelectItem.type == ItemID.None)
        {
            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }
        
        
        if (!this._lastSelectedItem.ContainsKey(player.Index) || this._lastSelectedItem[player.Index] != newSelectItem.type)
        {
            this._lastSelectedItem[player.Index] = newSelectItem.type;

            ParticleOrchestraSettings settings = new()
            {
                IndexOfPlayerWhoInvokedThis = (byte)player.Index,
                MovementVector = new Vector2(0, -24),
                PositionInWorld = player.TPlayer.Center + new Vector2(0, -24),
                UniqueInfoPiece = newSelectItem.type
            };
            ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.ItemTransfer, settings);
        }

        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }
    private readonly Dictionary<int, int> _lastSelectedItem = new ();

    private static Color GetColorByRarity(int rarity)
    {
        return Setting.Instance.ItemTextConfig.RarityColors.TryGetValue(rarity, out var colorConfig)
            ? new Color(colorConfig.R, colorConfig.G, colorConfig.B)
            : new Color(Setting.Instance.ItemTextConfig.DefaultColor.R, Setting.Instance.ItemTextConfig.DefaultColor.G, Setting.Instance.ItemTextConfig.DefaultColor.B);
    }

    private static string ReplacePlaceholderWithItem(TSPlayer player, string message)
    {
        var selectedItem = player.TPlayer.inventory[player.TPlayer.selectedItem];
        var stringBuilder = new StringBuilder();

        if (selectedItem is { type: > 0 })
        {
            var damageColorHex = Setting.Instance.ItemChatConfig.DamageColor.ToHex();

            // Agregar el ítem si está habilitado
            if (Setting.Instance.ItemChatConfig.ShowName)
            {
                stringBuilder.Append($"[i:{selectedItem.type}]");
            }

            // Agregar el daño si está habilitado
            if (Setting.Instance.ItemChatConfig.ShowDamage && selectedItem.damage > 0)
            {
                if (Setting.Instance.ItemChatConfig.ShowName)
                {
                    stringBuilder.Append(' ');
                }
                
                stringBuilder.Append($"[c/{damageColorHex}:{selectedItem.damage}]");
            }
        }
        return stringBuilder.Length!=0 ? $"[ {stringBuilder} ] {message}." : message;
    }
}
