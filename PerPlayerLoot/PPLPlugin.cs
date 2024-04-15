using System;
using System.Collections.Generic;
using System.Text;

using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.IO;
using System.IO.Streams;

namespace PerPlayerLoot
{
    [ApiVersion(2, 1)]
    public class PPLPlugin : TerrariaPlugin
    {
        #region info
        public override string Name => "PerPlayerLoot";

        public override Version Version => new Version(2, 0);

        public override string Author => "Codian,肝帝熙恩汉化1449";

        public override string Description => "玩家战利品单独箱子.";
        #endregion

        public static FakeChestDatabase fakeChestDb = new FakeChestDatabase();

        public static bool enablePpl = true;

        public PPLPlugin(Main game) : base(game) {}

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, OnWorldLoaded);
            ServerApi.Hooks.WorldSave.Register(this, OnWorldSave);

            TShockAPI.GetDataHandlers.PlaceChest += OnChestPlace;
            TShockAPI.GetDataHandlers.ChestOpen += OnChestOpen;
            TShockAPI.GetDataHandlers.ChestItemChange += OnChestItemChange;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnWorldLoaded);
                ServerApi.Hooks.WorldSave.Deregister(this, OnWorldSave);

                TShockAPI.GetDataHandlers.PlaceChest -= OnChestPlace;
                TShockAPI.GetDataHandlers.ChestOpen -= OnChestOpen;
                TShockAPI.GetDataHandlers.ChestItemChange -= OnChestItemChange;
            }

            base.Dispose(disposing);
        }


        private void OnWorldSave(WorldSaveEventArgs args)
        {
            fakeChestDb.SaveFakeChests();
        }

        private void OnWorldLoaded(EventArgs args)
        {
            fakeChestDb.Initialize();
            Commands.ChatCommands.Add(new Command("perplayerloot.toggle", ToggleCommand, "ppltoggle"));
        }

        private void ToggleCommand(CommandArgs args)
        {
            enablePpl = !enablePpl;
            if (enablePpl) {
                args.Player.SendSuccessMessage("现在启用了每个玩家单独的宝箱!");
            } else {
                args.Player.SendSuccessMessage("每个玩家单独的宝箱现在被禁用！您现在可以修改宝箱，它们将被视为普通宝箱.");
            }
        }

        private void OnChestItemChange(object sender, GetDataHandlers.ChestItemEventArgs e)
        {
            if (!enablePpl) return;

            // get the chest object from id
            Chest realChest = Main.chest[e.ID];
            if (realChest == null) 
                return;
            
            // check if it's a piggy bank or safe transaction
            if (realChest.bankChest)
                return;

            // check if this is a player placed chest
            if (fakeChestDb.IsChestPlayerPlaced(realChest.x, realChest.y))
                return;

            // construct an item from the event data
            Item item = new Item();
            item.netDefaults(e.Type);
            item.stack = e.Stacks;
            item.prefix = e.Prefix;

            // get the per-player chest
            Chest fakeChest = fakeChestDb.GetOrCreateFakeChest(e.ID, e.Player.UUID);
            
            // update the slot with the item
            fakeChest.item[e.Slot] = item;

            e.Handled = true;
        }

        private byte[] ConstructSpoofedChestItemPacket(int chestId, int slot, Item item)
        {
            // NetMessage.SendData is hardcode tied to Main.chest, so this method is necessary to reimplement stuff :(

            MemoryStream memoryStream = new MemoryStream();
            OTAPI.PacketWriter packetWriter = new OTAPI.PacketWriter(memoryStream);

            packetWriter.BaseStream.Position = 0L;
            long position = packetWriter.BaseStream.Position;

            packetWriter.BaseStream.Position += 2L;
            packetWriter.Write((byte) PacketTypes.ChestItem);

            packetWriter.Write((short) chestId);
            packetWriter.Write((byte) slot);

            short netId = (short)item.netID;
            if (item.Name == null)
            {
                netId = 0;
            }

            packetWriter.Write((short) item.stack);
            packetWriter.Write(item.prefix);
            packetWriter.Write(netId);

            int positionAfter = (int) packetWriter.BaseStream.Position;

            packetWriter.BaseStream.Position = position;
            packetWriter.Write((ushort)positionAfter);
            packetWriter.BaseStream.Position = positionAfter;

            return memoryStream.ToArray();
        }

        private void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs e)
        {
            if (e.Handled) return;
            if (!enablePpl) return;

            // get the chest's id
            int chestId = Chest.FindChest(e.X, e.Y);
            if (chestId == -1) return;

            // retreive the chest object
            Chest realChest = Main.chest[chestId];

            // make sure it exists
            if (realChest == null) 
                return;

            // piggy bank, safe, etc.
            if (realChest.bankChest) 
                return;

            // check if it's player placed
            if (fakeChestDb.IsChestPlayerPlaced(realChest.x, realChest.y))
                return;

            // make a per-player chest
            Chest fakeChest = fakeChestDb.GetOrCreateFakeChest(chestId, e.Player.UUID);

            // Console.WriteLine($"Opening a fake chest for {e.Player.Name}.");
            e.Player.SendInfoMessage("这个箱子里的战利品是每个玩家单独的!");

            // spoof chest slots
            for (int slot = 0; slot < Chest.maxItems; slot++)
            {
                // make a fake item stack
                Item item = fakeChest.item[slot];

                // spoof clientside slot
                byte[] payload = ConstructSpoofedChestItemPacket(chestId, slot, item);
                e.Player.SendRawData(payload);
            }

            // trigger chest open
            e.Player.SendData(PacketTypes.ChestOpen, "", chestId);

            // set the active chest on serverside
            e.Player.ActiveChest = chestId;
            Main.player[e.Player.Index].chest = chestId;
            // notify the client to also update the clientside state
            e.Player.SendData(PacketTypes.SyncPlayerChestIndex, null, e.Player.Index, chestId);

            // prevent anything else grabbing control
            e.Handled = true;
            return;
        }

        private void OnChestPlace(object sender, GetDataHandlers.PlaceChestEventArgs e)
        {
            if (!enablePpl) return;

            if (!fakeChestDb.IsChestPlayerPlaced(e.TileX, e.TileY - 1))
            {
                int chestId = Chest.FindChest(e.TileX, e.TileY - 1);
                if (chestId != -1)
                    Main.chest[chestId].item = new Item[Chest.maxItems];
            }

            fakeChestDb.SetChestPlayerPlaced(e.TileX, e.TileY - 1); // this -1 is mysterious
        }
    }
}
