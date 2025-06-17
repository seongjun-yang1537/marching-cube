// Ingame/AgentModel.cs

using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class AgentModel : EntityModel
    {
        public UnityEvent<ItemSlot> onHeldItem = new();
        public UnityEvent<ItemStack> onDropItem = new();

        public float nowSpeed;
        public float groundSpeed;
        public float flySpeed;
        public float jumpForce;

        public Transform body;

        public Inventory inventory;

        public ItemSlot heldItemSlot { get; private set; }

        protected void Awake()
        {
            nowSpeed = groundSpeed;
        }

        public void SetHeldItem(ItemSlot itemSlot)
        {
            heldItemSlot = itemSlot;
            onHeldItem.Invoke(heldItemSlot);
        }

        public void DropItem(ItemSlot itemSlot, int count = 1)
        {
            if (itemSlot.itemStack.IsEmpty) return;
            ItemStack itemStack = new ItemStack(itemSlot.itemStack);
            inventory.TakeItem(itemSlot, count);

            onDropItem.Invoke(itemStack);
            if (itemSlot.EqualSlot(heldItemSlot))
                onHeldItem.Invoke(heldItemSlot);
        }
    }
}