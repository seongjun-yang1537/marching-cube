using UnityEngine;

namespace Ingame
{
    public class ItemData
    {
        public static ItemData Empty() => new ItemData(ItemID.None, 0);

        public ItemID itemID;
        public EquipmentType equipType;
        public int maxStackable = 1;

        public ItemStack ToStack(int count = 1) => new ItemStack(this, count);

        public ItemData(ItemID itemID)
        {
            this.itemID = itemID;
        }
        public ItemData(ItemID itemID, int maxStackable) : this(itemID)
        {
            this.maxStackable = maxStackable;
        }
    }
}