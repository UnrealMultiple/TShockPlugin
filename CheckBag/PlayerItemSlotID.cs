using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 检查背包
{
    public static class PlayerItemSlotID
    {
        public static readonly int Inventory0;

        public static readonly int InventoryMouseItem;

        public static readonly int Armor0;

        public static readonly int Dye0;

        public static readonly int Misc0;

        public static readonly int MiscDye0;

        public static readonly int Bank1_0;

        public static readonly int Bank2_0;

        public static readonly int TrashItem;

        public static readonly int Bank3_0;

        public static readonly int Bank4_0;

        public static readonly int Loadout1_Armor_0;

        public static readonly int Loadout1_Dye_0;

        public static readonly int Loadout2_Armor_0;

        public static readonly int Loadout2_Dye_0;

        public static readonly int Loadout3_Armor_0;

        public static readonly int Loadout3_Dye_0;

        public static bool[] CanRelay;

        public static int _nextSlotId;

        static PlayerItemSlotID()
        {
            CanRelay = new bool[0];
            Inventory0 = AllocateSlots(58, canNetRelay: true);
            InventoryMouseItem = AllocateSlots(1, canNetRelay: true);
            Armor0 = AllocateSlots(20, canNetRelay: true);
            Dye0 = AllocateSlots(10, canNetRelay: true);
            Misc0 = AllocateSlots(5, canNetRelay: true);
            MiscDye0 = AllocateSlots(5, canNetRelay: true);
            Bank1_0 = AllocateSlots(40, canNetRelay: false);
            Bank2_0 = AllocateSlots(40, canNetRelay: false);
            TrashItem = AllocateSlots(1, canNetRelay: false);
            Bank3_0 = AllocateSlots(40, canNetRelay: false);
            Bank4_0 = AllocateSlots(40, canNetRelay: false);
            Loadout1_Armor_0 = AllocateSlots(20, canNetRelay: true);
            Loadout1_Dye_0 = AllocateSlots(10, canNetRelay: true);
            Loadout2_Armor_0 = AllocateSlots(20, canNetRelay: true);
            Loadout2_Dye_0 = AllocateSlots(10, canNetRelay: true);
            Loadout3_Armor_0 = AllocateSlots(20, canNetRelay: true);
            Loadout3_Dye_0 = AllocateSlots(10, canNetRelay: true);
        }

        public static int AllocateSlots(int amount, bool canNetRelay)
        {
            int nextSlotId = _nextSlotId;
            _nextSlotId += amount;
            int num = CanRelay.Length;
            Array.Resize(ref CanRelay, num + amount);
            for (int i = num; i < _nextSlotId; i++)
            {
                CanRelay[i] = canNetRelay;
            }

            return nextSlotId;
        }
    }
}
