using System.Collections.Generic;

namespace Ingame
{
    public class EquipmentContainer : IItemContainer
    {
        public List<ItemStack> slots = new();
        public int SlotCount => throw new System.NotImplementedException();
        public EquipmentContainer(int count)
        {
            for (int i = 0; i < count; i++) slots.Add(new ItemStack(null, 0));
        }

        public int AddToSlot(int idx, ItemStack itemStack)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAcceptItem(int idx, ItemStack itemStack)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack GetItem(int idx)
        {
            throw new System.NotImplementedException();
        }

        public bool HasItemAt(int idx)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSlotValid(int idx)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack RemoveItem(int idx)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack SetItem(int idx, ItemStack itemData)
        {
            throw new System.NotImplementedException();
        }

        public void SwapSlot(int from, int to)
        {
            throw new System.NotImplementedException();
        }
    }
}