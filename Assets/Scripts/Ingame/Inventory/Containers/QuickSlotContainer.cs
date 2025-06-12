using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public class QuickSlotContainer : IItemContainer
    {
        public List<ItemStack> slots = new();
        public int SlotCount => slots.Count;

        public QuickSlotContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(new ItemStack(null, 0));
        }

        public ItemStack GetItem(int idx)
        {
            ValidateIndex(idx);
            return slots[idx];
        }

        public ItemStack SetItem(int idx, ItemStack itemStack)
        {
            ValidateIndex(idx);
            var prev = slots[idx];
            slots[idx] = itemStack;
            return prev;
        }

        public bool HasItemAt(int idx)
        {
            if (!IsSlotValid(idx)) return false;
            var item = slots[idx];
            return item != null && !item.IsEmpty;
        }

        public bool IsSlotValid(int idx)
        {
            return idx >= 0 && idx < SlotCount;
        }

        public ItemStack RemoveItem(int idx)
        {
            ValidateIndex(idx);
            var prev = slots[idx];
            slots[idx] = new ItemStack(null, 0);
            return prev;
        }

        public bool CanAcceptItem(int idx, ItemStack itemStack)
        {
            if (!IsSlotValid(idx)) return false;
            var slot = slots[idx];

            if (slot.IsEmpty) return true;

            return slot.itemData == itemStack.itemData && slot.count < slot.itemData.maxStackable;
        }

        public int AddToSlot(int idx, ItemStack itemStack)
        {
            if (!CanAcceptItem(idx, itemStack)) return 0;

            var slot = slots[idx];

            if (slot.IsEmpty)
            {
                slots[idx] = new ItemStack(itemStack.itemData, Mathf.Min(itemStack.count, itemStack.itemData.maxStackable));
                return slots[idx].count;
            }
            else
            {
                int added = slot.Add(itemStack.count);
                return added;
            }
        }

        private void ValidateIndex(int idx)
        {
            if (!IsSlotValid(idx))
                throw new IndexOutOfRangeException($"Invalid slot index: {idx}");
        }

        public void SwapSlot(int from, int to)
        {
            throw new NotImplementedException();
        }
    }
}