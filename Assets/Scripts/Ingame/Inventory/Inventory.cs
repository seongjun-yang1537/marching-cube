using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    [Serializable]
    public class Inventory
    {
        [NonSerialized]
        private Dictionary<InventorySlotType, IItemContainer> containers;
        public Inventory()
        {
            containers = InitializeContainers();
        }

        private Dictionary<InventorySlotType, IItemContainer> InitializeContainers()
        {
            return new()
            {
                {InventorySlotType.Inventory, new InventoryContainer((int)InventorySlotID.COUNT)},
                {InventorySlotType.QuickSlot, new QuickSlotContainer((int)QuickSlotID.COUNT)},
                {InventorySlotType.Equipment, new InventoryContainer((int)EquipmentSlotID.COUNT)},
            };
        }

        public IItemContainer GetContainer(InventorySlotType type) => containers[type];
        public IItemContainer this[InventorySlotType type]
        {
            get => GetContainer(type);
        }
    }
}