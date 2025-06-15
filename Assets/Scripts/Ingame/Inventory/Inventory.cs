using System;
using System.Collections.Generic;
using Corelib.Utils;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    [Serializable]
    public class Inventory
    {
        public BagContainer bagContainer { get; private set; }
        public QuickSlotContainer quickSlotContainer { get; private set; }
        public EquipmentContainer equipmentContainer { get; private set; }

        public UnityEvent<BagSlotID, ItemStack> onChangedBag { get => bagContainer.onValueChanged; }
        public UnityEvent<QuickSlotID, ItemStack> onChangedQuickSlot { get => quickSlotContainer.onValueChanged; }
        public UnityEvent<EquipmentSlotID, ItemStack> onChangedEquipment { get => equipmentContainer.onValueChanged; }

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

        public ItemStack GetItemSlot(BagSlotID slotID)
            => bagContainer.GetItem(slotID);

        public void SetItemSlot(BagSlotID slotID, ItemData itemData)
            => SetItemSlot(slotID, itemData.ToStack());
        public void SetItemSlot(BagSlotID slotID, ItemStack itemStack)
        {
            bagContainer.RemoveItem(slotID);
            bagContainer.SetItem(slotID, itemStack);
        }

        #region Inventory Methods
        public ItemStack AddItem(ItemStack itemStack)
        {
            if (itemStack == null || itemStack.IsEmpty) return ItemStack.Empty();

            itemStack = new ItemStack(itemStack);

            for (int i = 0; i < bagContainer.SlotCount; i++)
            {
                var slot = bagContainer.slots[i];
                if (!slot.IsEmpty && slot.itemID == itemStack.itemID)
                {
                    int added = slot.Add(itemStack.count);
                    itemStack.count -= added;
                    if (itemStack.count <= 0) return ItemStack.Empty();
                }
            }

            for (int i = 0; i < bagContainer.SlotCount; i++)
            {
                if (bagContainer[i].IsEmpty)
                {
                    int toPlace = Math.Min(itemStack.count, itemStack.maxStackable);
                    bagContainer.slots[i] = new ItemStack(itemStack.itemData, toPlace);
                    itemStack.count -= toPlace;
                    if (itemStack.count <= 0) return ItemStack.Empty();
                }
            }

            return itemStack.count > 0 ? itemStack : ItemStack.Empty();
        }

        public ItemStack AddItem(BagSlotID slotID, ItemStack itemStack)
        {
            return bagContainer.AddToSlot(slotID, itemStack);
        }

        public ItemStack TakeItem(BagSlotID slotID, int count)
        {
            return bagContainer.TakeToSlot(slotID, count);
        }

        public ItemStack RemoveItem(BagSlotID slotID)
        {
            return bagContainer.RemoveItem(slotID);
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

        public ItemStack Equip(BagSlotID slotID)
        {
            var itemToEquip = bagContainer.GetItem(slotID);
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