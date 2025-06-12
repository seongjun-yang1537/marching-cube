using UnityEngine;

namespace Ingame
{
    public class ItemData
    {
        public ItemID itemID;
        public int maxStackable;

        public ItemStack ToStack() => new ItemStack(this);
    }
}