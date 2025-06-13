using System;
using System.Collections.Generic;
using Corelib.Utils;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class Inventory
    {
        private InventoryContainer inventoryContainer;
        private QuickSlotContainer quickSlotContainer;
        private EquipmentContainer equipmentContainer;

        public Inventory()
        {
            InitializeContainers();
        }

        private void InitializeContainers()
        {
            inventoryContainer = new InventoryContainer(ExEnum.Count<InventorySlotID>());
            quickSlotContainer = new QuickSlotContainer(ExEnum.Count<QuickSlotID>());
            equipmentContainer = new EquipmentContainer(ExEnum.Count<EquipmentSlotID>());
        }

        public ItemStack GetItemSlot(InventorySlotID slotID)
            => inventoryContainer.GetItem(slotID);

        public void SetItemSlot(InventorySlotID slotID, ItemData itemData)
            => SetItemSlot(slotID, itemData.ToStack());
        public void SetItemSlot(InventorySlotID slotID, ItemStack itemStack)
        {
            inventoryContainer.RemoveItem(slotID);
            inventoryContainer.SetItem(slotID, itemStack);
        }

        #region Inventory Methods
        public ItemStack AddItem(ItemStack itemStack)
        {
            if (itemStack == null || itemStack.IsEmpty) return ItemStack.Empty();

            itemStack = new ItemStack(itemStack);

            for (int i = 0; i < inventoryContainer.SlotCount; i++)
            {
                var slot = inventoryContainer.slots[i];
                if (!slot.IsEmpty && slot.itemID == itemStack.itemID)
                {
                    int added = slot.Add(itemStack.count);
                    itemStack.count -= added;
                    if (itemStack.count <= 0) return ItemStack.Empty();
                }
            }

            for (int i = 0; i < inventoryContainer.SlotCount; i++)
            {
                if (inventoryContainer[i].IsEmpty)
                {
                    int toPlace = Math.Min(itemStack.count, itemStack.maxStackable);
                    inventoryContainer.slots[i] = new ItemStack(itemStack.itemData, toPlace);
                    itemStack.count -= toPlace;
                    if (itemStack.count <= 0) return ItemStack.Empty();
                }
            }

            return itemStack.count > 0 ? itemStack : ItemStack.Empty();
        }

        public ItemStack AddItem(InventorySlotID slotID, ItemStack itemStack)
        {
            return inventoryContainer.AddToSlot(slotID, itemStack);
        }

        public ItemStack TakeItem(InventorySlotID slotID, int count)
        {
            return inventoryContainer.TakeToSlot(slotID, count);
        }

        public ItemStack RemoveItem(InventorySlotID slotID)
        {
            return inventoryContainer.RemoveItem(slotID);
        }

        #endregion

        #region QuickSlot Methods

        public ItemStack Assign(ItemStack itemStack)
        {
            for (int i = 0; i < quickSlotContainer.SlotCount; i++)
            {
                var slotId = (QuickSlotID)i;
                if (!quickSlotContainer.HasItemAt(slotId))
                {
                    quickSlotContainer.SetItem(slotId, itemStack);
                    return ItemStack.Empty();
                }
            }
            return itemStack;
        }

        public ItemStack Assign(QuickSlotID quickSlotID, ItemStack itemStack)
        {
            var oldItem = quickSlotContainer.GetItem(quickSlotID);
            quickSlotContainer.SetItem(quickSlotID, itemStack);
            return oldItem;
        }

        public ItemStack TakeQuickSlot(QuickSlotID quickSlotID, int count)
        {
            return quickSlotContainer.TakeToSlot(quickSlotID, count);
        }

        public ItemStack UnAssign(QuickSlotID quickSlotID)
        {
            return quickSlotContainer.RemoveItem(quickSlotID);
        }

        #endregion

        #region Equipment Methods

        public ItemStack Equip(ItemStack itemStack)
        {
            if (itemStack == null || itemStack.IsEmpty) return null;

            var equipType = itemStack.itemData.equipType;
            if (equipType == EquipmentType.None) return itemStack; // 장착 불가 아이템

            var slotId = (EquipmentSlotID)equipType;

            var oldEquipment = equipmentContainer.GetItem(slotId);
            equipmentContainer.SetItem(slotId, itemStack);

            if (!oldEquipment.IsEmpty)
            {
                return AddItem(oldEquipment);
            }

            return null;
        }

        public ItemStack Equip(InventorySlotID slotID)
        {
            var itemToEquip = inventoryContainer.GetItem(slotID);
            if (itemToEquip.IsEmpty) return ItemStack.Empty();

            RemoveItem(slotID);

            return Equip(itemToEquip);
        }

        public ItemStack Equip(QuickSlotID quickSlotID)
        {
            var itemToEquip = quickSlotContainer.GetItem(quickSlotID);
            if (itemToEquip.IsEmpty) return ItemStack.Empty();

            UnAssign(quickSlotID);

            return Equip(itemToEquip);
        }

        public ItemStack UnEquip(EquipmentType equipType)
        {
            if (equipType == EquipmentType.None) return null;

            var slotId = (EquipmentSlotID)equipType;
            var itemToUnEquip = equipmentContainer.RemoveItem(equipType.ToSlotID());

            if (!itemToUnEquip.IsEmpty)
            {
                return AddItem(itemToUnEquip);
            }

            return null;
        }

        #endregion
    }
}