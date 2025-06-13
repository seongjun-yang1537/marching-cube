using System;
using System.Collections.Generic;

namespace Ingame
{
    public class EquipmentContainer : IItemContainer<EquipmentSlotID>
    {
        public List<ItemStack> slots = new();
        public int SlotCount => slots.Count;

        public EquipmentContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(new ItemStack(null, 0));
        }

        public ItemStack this[EquipmentSlotID e]
        {
            get => GetItem(e);
            set => SetItem(e, value);
        }

        public ItemStack GetItem(EquipmentSlotID slotID) => slots[(int)slotID];

        public ItemStack SetItem(EquipmentSlotID slotID, ItemStack itemStack)
            => slots[(int)slotID] = itemStack;

        public bool HasItemAt(EquipmentSlotID slotID)
            => !GetItem(slotID).IsEmpty;

        public bool CanAcceptItem(EquipmentSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return true;
            if (slot.itemID == itemStack.itemID && itemStack.count <= slot.Remain)
                return true;
            return false;
        }

        public ItemStack RemoveItem(EquipmentSlotID slotID)
        {
            ItemStack itemStack = GetItem(slotID);
            SetItem(slotID, ItemStack.Empty());
            return itemStack;
        }

        public ItemStack AddToSlot(EquipmentSlotID slotID, ItemStack itemStack)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.itemID != itemStack.itemID)
                return itemStack;

            int count = Math.Min(itemStack.count, GetItem(slotID).Remain);
            slot.count += count;
            itemStack.count -= count;
            return itemStack;
        }

        public ItemStack TakeToSlot(EquipmentSlotID slotID, int count)
        {
            ItemStack slot = GetItem(slotID);
            if (slot.IsEmpty) return ItemStack.Empty();

            count = Math.Min(slot.Remain, count);
            slot.count -= count;

            return new ItemStack(slot.itemData, count);
        }

        public void SwapSlot(EquipmentSlotID from, EquipmentSlotID to)
        {

        }
    }
}