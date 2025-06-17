using System;
using Ingame;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class ItemSlot
    {
        [NonSerialized]
        public IItemContainer ownerContainer;

        public InventorySlotType slotType;
        public int slotIdx;

        public ItemStack itemStack;

        public ItemSlot()
        {

        }

        public ItemSlot(ItemSlot other)
        {
            this.ownerContainer = other.ownerContainer;
            this.slotType = other.slotType;
            this.slotIdx = other.slotIdx;
            this.itemStack = new ItemStack(other.itemStack);
        }

        public bool EqualSlot(ItemSlot other)
            => slotType == other.slotType && slotIdx == other.slotIdx;
    }
}