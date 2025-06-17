using System;
using System.Linq;
using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public static class ItemSystem
    {
        public static void Swap(ItemSlot from, ItemSlot to)
        {
            if (from == null || to == null) return;
            if (from.ownerContainer == to.ownerContainer)
            {
                from.ownerContainer.SwapSlot(from.slotIdx, to.slotIdx);
            }
            else
            {
                var fromContainer = from.ownerContainer;
                var toContainer = to.ownerContainer;
                ItemStack fromItem = new ItemStack(from.itemStack);
                ItemStack toItem = new ItemStack(to.itemStack);
                if (!fromContainer.CanAcceptItem(from.slotIdx, toItem) ||
                    !toContainer.CanAcceptItem(to.slotIdx, fromItem))
                {
                    return;
                }

                fromContainer.SetItem(from.slotIdx, toItem);
                toContainer.SetItem(to.slotIdx, fromItem);
            }
        }

        public static void Move(ItemSlot from, ItemSlot to)
        {
            if (from == null || to == null || from.itemStack.IsEmpty) return;
            if (to.itemStack.IsEmpty || to.itemStack.itemID == from.itemStack.itemID)
            {
                ItemStack remainder = to.ownerContainer.AddToSlot(to.slotIdx, from.itemStack);
                from.ownerContainer.SetItem(from.slotIdx, remainder);
            }
            else
            {
                Swap(from, to);
            }
        }
    }

    [Serializable]
    public class Inventory
    {
        public BagContainer bagContainer { get; private set; }
        public QuickSlotContainer quickSlotContainer { get; private set; }
        public EquipmentContainer equipmentContainer { get; private set; }

        public UnityEvent<ItemSlot> onChangedBag => bagContainer.onSlotChanged;
        public UnityEvent<ItemSlot> onChangedQuickSlot => quickSlotContainer.onSlotChanged;
        public UnityEvent<ItemSlot> onChangedEquipment => equipmentContainer.onSlotChanged;

        public Inventory()
        {
            InitializeContainers();
        }

        private void InitializeContainers()
        {
            bagContainer = new BagContainer(ExEnum.Count<BagSlotID>());
            quickSlotContainer = new QuickSlotContainer(ExEnum.Count<QuickSlotID>());
            equipmentContainer = new EquipmentContainer(ExEnum.Count<EquipmentSlotID>());
        }

        public ItemSlot GetItemSlot(EquipmentSlotID slotID) => equipmentContainer.GetSlot(slotID);
        public ItemSlot GetItemSlot(BagSlotID slotID) => bagContainer.GetSlot(slotID);
        public ItemSlot GetItemSlot(QuickSlotID slotID) => quickSlotContainer.GetSlot(slotID);

        public void SetItemSlot(EquipmentSlotID slotID, ItemStack itemStack) => equipmentContainer.SetItem((int)slotID, itemStack);
        public void SetItemSlot(EquipmentSlotID slotID, ItemData itemData) => SetItemSlot(slotID, itemData.ToStack());
        public void SetItemSlot(BagSlotID slotID, ItemStack itemStack) => bagContainer.SetItem((int)slotID, itemStack);
        public void SetItemSlot(BagSlotID slotID, ItemData itemData) => SetItemSlot(slotID, itemData.ToStack());
        public void SetItemSlot(QuickSlotID slotID, ItemStack itemStack) => quickSlotContainer.SetItem((int)slotID, itemStack);
        public void SetItemSlot(QuickSlotID slotID, ItemData itemData) => SetItemSlot(slotID, itemData.ToStack());
        public void SetItemSlot(ItemSlot itemSlot)
        {
            if (itemSlot.ownerContainer == null) return;
            itemSlot.ownerContainer.SetItem(itemSlot.slotIdx, itemSlot.itemStack);
        }

        public void SwapItemSlot(ItemSlot from, ItemSlot to) => ItemSystem.Swap(from, to);

        public ItemStack AddItem(ItemStack itemStack)
        {
            ItemStack remainder = new ItemStack(itemStack.itemData, itemStack.count);

            for (int i = 0; i < quickSlotContainer.SlotCount; i++)
            {
                if (remainder.IsEmpty) return remainder;
                remainder = quickSlotContainer.AddToSlot(i, remainder);
            }

            for (int i = 0; i < bagContainer.SlotCount; i++)
            {
                if (remainder.IsEmpty) return remainder;
                remainder = bagContainer.AddToSlot(i, remainder);
            }

            return remainder;
        }

        public void RemoveItemSlot(EquipmentSlotID slotID) => equipmentContainer.RemoveItem((int)slotID);
        public void RemoveItemSlot(BagSlotID slotID) => bagContainer.RemoveItem((int)slotID);
        public void RemoveItemSlot(QuickSlotID slotID) => quickSlotContainer.RemoveItem((int)slotID);
        public void RemoveItemSlot(ItemSlot itemSlot)
        {
            if (itemSlot?.ownerContainer == null) return;
            itemSlot.ownerContainer.RemoveItem(itemSlot.slotIdx);
        }

        public ItemStack TakeItem(EquipmentSlotID slotID, int count) => equipmentContainer.TakeToSlot((int)slotID, count);
        public ItemStack TakeItem(BagSlotID slotID, int count) => bagContainer.TakeToSlot((int)slotID, count);
        public ItemStack TakeItem(QuickSlotID slotID, int count) => quickSlotContainer.TakeToSlot((int)slotID, count);

        public void Equip(ItemSlot fromBagSlot)
        {
            if (fromBagSlot == null || fromBagSlot.itemStack.IsEmpty) return;

            var itemData = fromBagSlot.itemStack.itemData;
            if (itemData.equipType == EquipmentType.None) return;

            ItemSlot toEquipSlot = equipmentContainer.GetSlot((int)Enum.Parse(typeof(EquipmentSlotID), itemData.equipType.ToString()));

            if (toEquipSlot != null)
            {
                ItemSystem.Swap(fromBagSlot, toEquipSlot);
            }
        }

        public void UnEquip(ItemSlot fromEquipSlot)
        {
            if (fromEquipSlot == null || fromEquipSlot.itemStack.IsEmpty) return;
            if (fromEquipSlot.slotType != InventorySlotType.Equipment) return;

            ItemSlot emptyBagSlot = FindEmptySlotInBag();
            if (emptyBagSlot != null)
            {
                ItemSystem.Swap(fromEquipSlot, emptyBagSlot);
            }
        }

        public void Assign(QuickSlotID quickSlotID, ItemSlot fromSlot)
        {
            if (fromSlot == null) return;
            ItemSlot toQuickSlot = quickSlotContainer.GetSlot(quickSlotID);
            if (toQuickSlot == null) return;

            ItemSystem.Swap(fromSlot, toQuickSlot);
        }

        public void UnAssign(QuickSlotID quickSlotID)
        {
            ItemSlot fromQuickSlot = quickSlotContainer.GetSlot(quickSlotID);
            UnEquip(fromQuickSlot);
        }

        private ItemSlot FindEmptySlotInBag()
        {
            for (int i = 0; i < bagContainer.SlotCount; i++)
            {
                ItemSlot slot = bagContainer.GetSlot(i);
                if (slot.itemStack.IsEmpty)
                {
                    return slot;
                }
            }
            return null;
        }
    }
}