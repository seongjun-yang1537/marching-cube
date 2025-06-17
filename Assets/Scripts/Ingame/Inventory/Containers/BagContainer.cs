using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class BagContainer : IItemContainer, IEnumerable<ItemSlot>
    {
        public UnityEvent<ItemSlot> onSlotChanged;

        private readonly List<ItemSlot> slots = new();
        public int SlotCount => slots.Count;

        public BagContainer(int count)
        {
            onSlotChanged = new();
            slots.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                slots.Add(new ItemSlot
                {
                    ownerContainer = this,
                    slotType = InventorySlotType.Bag,
                    slotIdx = i,
                    itemStack = ItemStack.Empty()
                });
            }
        }

        public ItemSlot GetSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return null;
            return slots[slotIndex];
        }

        public ItemStack GetItem(int slotIndex) => GetSlot(slotIndex)?.itemStack ?? ItemStack.Empty();

        public ItemStack SetItem(int slotIndex, ItemStack itemStack)
        {
            ItemSlot slot = GetSlot(slotIndex);
            if (slot == null) return itemStack;

            slot.itemStack = itemStack;
            onSlotChanged.Invoke(slot);
            return itemStack;
        }

        public bool HasItemAt(int slotIndex) => !GetItem(slotIndex).IsEmpty;

        public bool CanAcceptItem(int slotIndex, ItemStack itemStack)
        {
            if (itemStack == null || itemStack.IsEmpty) return true;

            ItemSlot slot = GetSlot(slotIndex);
            if (slot == null) return false;

            ItemStack currentItem = slot.itemStack;
            if (currentItem.IsEmpty) return true;

            return currentItem.itemID == itemStack.itemID && itemStack.count <= currentItem.Remain;
        }

        public ItemStack RemoveItem(int slotIndex)
        {
            ItemStack originalItem = GetItem(slotIndex);
            if (originalItem.IsEmpty) return ItemStack.Empty();

            SetItem(slotIndex, ItemStack.Empty());
            return originalItem;
        }

        public ItemStack AddToSlot(int slotIndex, ItemStack itemStack)
        {
            if (itemStack.IsEmpty) return itemStack;

            ItemSlot slot = GetSlot(slotIndex);
            if (slot == null) return itemStack;

            ItemStack currentItem = slot.itemStack;
            if (currentItem.IsEmpty)
            {
                SetItem(slotIndex, itemStack);
                return ItemStack.Empty();
            }

            if (currentItem.itemID != itemStack.itemID) return itemStack;

            int amountToAdd = Math.Min(itemStack.count, currentItem.Remain);
            if (amountToAdd <= 0) return itemStack;

            currentItem.count += amountToAdd;
            itemStack.count -= amountToAdd;

            SetItem(slotIndex, currentItem);

            return itemStack;
        }

        public ItemStack TakeToSlot(int slotIndex, int count)
        {
            ItemSlot slot = GetSlot(slotIndex);
            if (slot == null || slot.itemStack.IsEmpty || count <= 0)
                return ItemStack.Empty();

            ItemStack currentItem = slot.itemStack;
            int amountToTake = Math.Min(currentItem.count, count);

            ItemStack takenStack = currentItem.itemData.ToStack(amountToTake);

            currentItem.count -= amountToTake;
            SetItem(slotIndex, currentItem);

            return takenStack;
        }

        public void SwapSlot(int fromSlotIndex, int toSlotIndex)
        {
            if (fromSlotIndex < 0 || fromSlotIndex >= slots.Count ||
                toSlotIndex < 0 || toSlotIndex >= slots.Count ||
                fromSlotIndex == toSlotIndex)
            {
                return;
            }

            ItemSlot fromSlot = GetSlot(fromSlotIndex);
            ItemSlot toSlot = GetSlot(toSlotIndex);

            if (fromSlot == null || toSlot == null) return;

            (toSlot.itemStack, fromSlot.itemStack) = (fromSlot.itemStack, toSlot.itemStack);

            onSlotChanged.Invoke(fromSlot);
            onSlotChanged.Invoke(toSlot);
        }

        public ItemSlot GetSlot(BagSlotID slotID) => GetSlot((int)slotID);
        public ItemStack GetItem(BagSlotID slotID) => GetItem((int)slotID);
        public ItemStack SetItem(BagSlotID slotID, ItemStack itemStack) => SetItem((int)slotID, itemStack);
        public bool HasItemAt(BagSlotID slotID) => HasItemAt((int)slotID);
        public bool CanAcceptItem(BagSlotID slotID, ItemStack itemStack) => CanAcceptItem((int)slotID, itemStack);
        public ItemStack RemoveItem(BagSlotID slotID) => RemoveItem((int)slotID);
        public ItemStack AddToSlot(BagSlotID slotID, ItemStack itemStack) => AddToSlot((int)slotID, itemStack);
        public ItemStack TakeToSlot(BagSlotID slotID, int count) => TakeToSlot((int)slotID, count);
        public void SwapSlot(BagSlotID from, BagSlotID to) => SwapSlot((int)from, (int)to);

        public IEnumerator<ItemSlot> GetEnumerator()
        {
            foreach (var slot in slots)
            {
                yield return slot;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ItemStack this[int slotIndex]
        {
            get => GetItem(slotIndex);
            set => SetItem(slotIndex, value);
        }

        public ItemStack this[BagSlotID slotID]
        {
            get => this[(int)slotID];
            set => this[(int)slotID] = value;
        }
    }
}