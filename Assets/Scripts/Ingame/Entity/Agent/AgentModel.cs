// Ingame/AgentModel.cs

using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class AgentModel : EntityModel
    {
        public UnityEvent<ItemStack> onHeldItem = new();

        public float nowSpeed;
        public float groundSpeed;
        public float flySpeed;
        public float jumpForce;

        public Transform body;

        public Inventory inventory;

        public ItemStack heldItem { get; private set; }

        protected void Awake()
        {
            nowSpeed = groundSpeed;
        }

        public void SetHeldItem(ItemStack itemStack)
        {
            heldItem = itemStack;
            onHeldItem.Invoke(heldItem);
        }
    }
}