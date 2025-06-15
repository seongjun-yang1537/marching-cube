using System;
using System.Collections.Generic;
using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class BagContainer : IItemContainer<BagSlotID>
    {
        public UnityEvent<BagSlotID, ItemStack> onValueChanged;

        public List<ItemStack> slots = new();
        public int SlotCount => slots.Count;

        public BagContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(ItemStack.Empty());
            onValueChanged = new();
        }

        public ItemStack this[BagSlotID e]
        {
            get => GetItem(e);
            set => SetItem(e, value);
        }

        public ItemStack this[int e]
        {
            get => GetItem((BagSlotID)e);
            set => SetItem((BagSlotID)e, value);
        }

        public ItemStack GetItem(BagSlotID slotID) => slots[(int)slotID];

        public ItemStack SetItem(BagSlotID slotID, ItemStack itemStack)
        {
            slots[(int)slotID] = itemStack;
            onValueChanged.Invoke(slotID, itemStack);
            return itemStack;
        }

        public bool HasItemAt(BagSlotID slotID)
            => !GetItem(slotID).IsEmpty;

        public bool CanAcceptItem(BagSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return true;
            if (slot.itemID == itemStack.itemID && itemStack.count <= slot.Remain)
                return true;
            return false;
        }

        public ItemStack RemoveItem(BagSlotID slotID)
        {
            ItemStack itemStack = GetItem(slotID);
            SetItem(slotID, ItemStack.Empty());
            return itemStack;
        }

        public ItemStack AddToSlot(BagSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.itemID != itemStack.itemID)
                return itemStack;

            int count = Math.Min(itemStack.count, GetItem(slotID).Remain);
            slot.count += count;
            itemStack.count -= count;
            onValueChanged.Invoke(slotID, itemStack);
            return itemStack;
        }

        public ItemStack TakeToSlot(BagSlotID slotID, int count)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return ItemStack.Empty();

            count = Math.Min(slot.Remain, count);
            slot.count -= count;

            ItemStack remain = new ItemStack(slot.itemData, count);
            onValueChanged.Invoke(slotID, remain);
            return remain;
        }

        public void SwapSlot(BagSlotID from, BagSlotID to)
        {

        }
    }
}