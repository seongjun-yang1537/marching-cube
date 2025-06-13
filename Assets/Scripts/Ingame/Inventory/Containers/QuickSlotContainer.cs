using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public class QuickSlotContainer : IItemContainer<QuickSlotID>
    {
        public List<ItemStack> slots = new();
        public int SlotCount => slots.Count;

        public QuickSlotContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(new ItemStack(null, 0));
        }

        public ItemStack this[QuickSlotID e]
        {
            get => GetItem(e);
            set => SetItem(e, value);
        }

        public ItemStack GetItem(QuickSlotID slotID) => slots[(int)slotID];

        public ItemStack SetItem(QuickSlotID slotID, ItemStack itemStack)
            => slots[(int)slotID] = itemStack;

        public bool HasItemAt(QuickSlotID slotID)
            => !GetItem(slotID).IsEmpty;

        public bool CanAcceptItem(QuickSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return true;
            if (slot.itemID == itemStack.itemID && itemStack.count <= slot.Remain)
                return true;
            return false;
        }

        public ItemStack RemoveItem(QuickSlotID slotID)
        {
            ItemStack itemStack = GetItem(slotID);
            SetItem(slotID, ItemStack.Empty());
            return itemStack;
        }

        public ItemStack AddToSlot(QuickSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.itemID != itemStack.itemID)
                return itemStack;

            int count = Math.Min(itemStack.count, GetItem(slotID).Remain);
            slot.count += count;
            itemStack.count -= count;
            return itemStack;
        }

        public ItemStack TakeToSlot(QuickSlotID slotID, int count)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return ItemStack.Empty();

            count = Math.Min(slot.Remain, count);
            slot.count -= count;

            return new ItemStack(slot.itemData, count);
        }

        public void SwapSlot(QuickSlotID from, QuickSlotID to)
        {

        }
    }
}