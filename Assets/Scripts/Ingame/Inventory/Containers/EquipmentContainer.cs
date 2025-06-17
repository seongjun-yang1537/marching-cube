using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Ingame
{
    public class EquipmentContainer : IItemContainer
    {
        public UnityEvent<ItemSlot> onSlotChanged;

        private readonly List<ItemSlot> slots = new();
        public int SlotCount => slots.Count;

        public EquipmentContainer(int count)
        {
            onSlotChanged = new();
            slots.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                slots.Add(new ItemSlot
                {
                    ownerContainer = this,
                    slotType = InventorySlotType.Equipment,
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

            if (itemStack.count > 1) itemStack.count = 1;

            slot.itemStack = itemStack;
            onSlotChanged.Invoke(slot);
            return itemStack;
        }

        public bool HasItemAt(int slotIndex) => !GetItem(slotIndex).IsEmpty;

        public bool CanAcceptItem(int slotIndex, ItemStack itemStack)
        {
            if (itemStack.IsEmpty) return true;

            return GetItem(slotIndex).IsEmpty;
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
            return itemStack;
        }

        public ItemStack TakeToSlot(int slotIndex, int count)
        {
            if (count > 0 && HasItemAt(slotIndex))
            {
                return RemoveItem(slotIndex);
            }
            return ItemStack.Empty();
        }

        public void SwapSlot(int fromSlotIndex, int toSlotIndex)
        {
            ItemSlot fromSlot = GetSlot(fromSlotIndex);
            ItemSlot toSlot = GetSlot(toSlotIndex);
            if (fromSlot == null || toSlot == null) return;

            (toSlot.itemStack, fromSlot.itemStack) = (fromSlot.itemStack, toSlot.itemStack);

            onSlotChanged.Invoke(fromSlot);
            onSlotChanged.Invoke(toSlot);
        }

        public ItemSlot GetSlot(EquipmentSlotID slotID) => GetSlot((int)slotID);
        public ItemStack GetItem(EquipmentSlotID slotID) => GetItem((int)slotID);
        public ItemStack SetItem(EquipmentSlotID slotID, ItemStack itemStack) => SetItem((int)slotID, itemStack);
        public bool HasItemAt(EquipmentSlotID slotID) => HasItemAt((int)slotID);
        public bool CanAcceptItem(EquipmentSlotID slotID, ItemStack itemStack) => CanAcceptItem((int)slotID, itemStack);
        public ItemStack RemoveItem(EquipmentSlotID slotID) => RemoveItem((int)slotID);
        public ItemStack AddToSlot(EquipmentSlotID slotID, ItemStack itemStack) => AddToSlot((int)slotID, itemStack);
        public ItemStack TakeToSlot(EquipmentSlotID slotID, int count) => TakeToSlot((int)slotID, count);
        public void SwapSlot(EquipmentSlotID from, EquipmentSlotID to) => SwapSlot((int)from, (int)to);

        public ItemStack this[int slotIndex]
        {
            get => GetItem(slotIndex);
            set => SetItem(slotIndex, value);
        }

        public ItemStack this[EquipmentSlotID slotID]
        {
            get => this[(int)slotID];
            set => this[(int)slotID] = value;
        }
    }
}