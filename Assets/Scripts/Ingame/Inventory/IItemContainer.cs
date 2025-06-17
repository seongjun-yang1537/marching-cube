using Ingame;

namespace Ingame
{
    public interface IItemContainer
    {
        public int SlotCount { get; }

        public ItemStack GetItem(int slotIndex);

        public ItemStack SetItem(int slotIndex, ItemData itemData) => SetItem(slotIndex, itemData.ToStack());
        public ItemStack SetItem(int slotIndex, ItemStack itemStack);

        public bool HasItemAt(int slotIndex);

        public bool CanAcceptItem(int slotIndex, ItemData itemData)
            => CanAcceptItem(slotIndex, itemData.ToStack());
        public bool CanAcceptItem(int slotIndex, ItemStack itemStack);
        public bool CanAcceptItem(ItemSlot itemSlot)
            => CanAcceptItem(itemSlot.slotIdx, itemSlot.itemStack);

        public ItemStack RemoveItem(int slotIndex);

        public ItemStack AddToSlot(int slotIndex, ItemData itemData)
            => AddToSlot(slotIndex, itemData.ToStack());
        public ItemStack AddToSlot(int slotIndex, ItemStack itemStack);

        public ItemStack TakeToSlot(int slotIndex, int count);

        public void SwapSlot(int fromSlotIndex, int toSlotIndex);
    }
}