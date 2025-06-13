using System;

namespace Ingame
{
    public interface IItemContainer<T> where T : Enum
    {
        public int SlotCount { get; }

        public ItemStack GetItem(T e);

        public ItemStack SetItem(T e, ItemData itemData) => SetItem(e, itemData.ToStack());
        public ItemStack SetItem(T e, ItemStack itemStack);

        public bool HasItemAt(T e);

        public bool CanAcceptItem(T e, ItemData itemData)
            => CanAcceptItem(e, itemData.ToStack());
        public bool CanAcceptItem(T e, ItemStack itemStack);

        public ItemStack RemoveItem(T e);

        public ItemStack AddToSlot(T e, ItemData itemData)
            => AddToSlot(e, itemData.ToStack());
        public ItemStack AddToSlot(T e, ItemStack itemStack);

        public ItemStack TakeToSlot(T e, int count);

        public void SwapSlot(T from, T to);
    }
}