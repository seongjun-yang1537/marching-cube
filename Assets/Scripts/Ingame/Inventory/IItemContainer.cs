using System;

namespace Ingame
{
    public interface IItemContainer
    {
        public int SlotCount { get; }

        public ItemStack this[int idx]
        {
            get => GetItem(idx);
            set => SetItem(idx, value);
        }
        public ItemStack this[Enum e]
        {
            get => GetItem(e);
            set => SetItem(e, value);
        }

        public ItemStack GetItem(Enum e) => GetItem(Convert.ToInt32(e));
        public ItemStack GetItem(int idx);

        public ItemStack SetItem(Enum e, ItemData itemData) => SetItem(e, itemData.ToStack());
        public ItemStack SetItem(Enum e, ItemStack itemStack) => SetItem(Convert.ToInt32(e), itemStack);

        public ItemStack SetItem(int idx, ItemData itemData)
            => SetItem(idx, itemData.ToStack());
        public ItemStack SetItem(int idx, ItemStack itemData);

        public bool HasItemAt(Enum e) => HasItemAt(Convert.ToInt32(e));
        public bool HasItemAt(int idx);

        public bool CanAcceptItem(Enum e, ItemData itemData)
            => CanAcceptItem(e, itemData.ToStack());
        public bool CanAcceptItem(Enum e, ItemStack itemStack)
            => CanAcceptItem(Convert.ToInt32(e), itemStack);

        public bool CanAcceptItem(int idx, ItemData itemData)
            => CanAcceptItem(idx, itemData.ToStack());
        public bool CanAcceptItem(int idx, ItemStack itemStack);

        public ItemStack RemoveItem(Enum e) => RemoveItem(Convert.ToInt32(e));
        public ItemStack RemoveItem(int idx);

        public bool IsSlotValid(Enum e) => IsSlotValid(Convert.ToInt32(e));
        public bool IsSlotValid(int idx);

        public int AddToSlot(Enum e, ItemData itemData)
            => AddToSlot(e, itemData.ToStack());
        public int AddToSlot(Enum e, ItemStack itemStack)
            => AddToSlot(Convert.ToInt32(e), itemStack);

        public int AddToSlot(int idx, ItemData itemData)
            => AddToSlot(idx, itemData.ToStack());
        public int AddToSlot(int idx, ItemStack itemStack);

        public void SwapSlot(Enum from, Enum to)
            => SwapSlot(Convert.ToInt32(from), Convert.ToInt32(to));
        public void SwapSlot(int from, int to);
    }
}