// Ingame/AgentModel.cs

using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class AgentModel : EntityModel
    {
        public UnityEvent<ItemSlot> onHeldItem = new();

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

        public void DropHeldItem()
        {

        }
    }
}