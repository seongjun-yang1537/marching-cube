using System;
using System.Collections.Generic;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public class InventoryContainer : IItemContainer<InventorySlotID>
    {
        public List<ItemStack> slots = new();
        public int SlotCount => slots.Count;

        public InventoryContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(new ItemStack(null, 0));
        }

        public ItemStack this[InventorySlotID e]
        {
            get => GetItem(e);
            set => SetItem(e, value);
        }

        public ItemStack this[int e]
        {
            get => GetItem((InventorySlotID)e);
            set => SetItem((InventorySlotID)e, value);
        }

        public ItemStack GetItem(InventorySlotID slotID) => slots[(int)slotID];

        public ItemStack SetItem(InventorySlotID slotID, ItemStack itemStack)
            => slots[(int)slotID] = itemStack;

        public bool HasItemAt(InventorySlotID slotID)
            => !GetItem(slotID).IsEmpty;

        public bool CanAcceptItem(InventorySlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return true;
            if (slot.itemID == itemStack.itemID && itemStack.count <= slot.Remain)
                return true;
            return false;
        }

        public ItemStack RemoveItem(InventorySlotID slotID)
        {
            ItemStack itemStack = GetItem(slotID);
            SetItem(slotID, ItemStack.Empty());
            return itemStack;
        }

        public ItemStack AddToSlot(InventorySlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.itemID != itemStack.itemID)
                return itemStack;

            int count = Math.Min(itemStack.count, GetItem(slotID).Remain);
            slot.count += count;
            itemStack.count -= count;
            return itemStack;
        }

        public ItemStack TakeToSlot(InventorySlotID slotID, int count)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return ItemStack.Empty();

            count = Math.Min(slot.Remain, count);
            slot.count -= count;

            return new ItemStack(slot.itemData, count);
        }

        public void SwapSlot(InventorySlotID from, InventorySlotID to)
        {

        }
    }
}