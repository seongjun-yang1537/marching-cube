using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(AgentModel))]
    public class AgentView : EntityView
    {
        public float dropForce = 100.0f;

        protected AgentModel agentModel;

        public Transform heldItemSocket;

        protected void Awake()
        {
            base.Awake();

            agentModel = (AgentModel)entityModel;

            agentModel.onHeldItem.AddListener(itemSlot => OnItemStack(itemSlot.itemStack));
            agentModel.onDropItem.AddListener(itemStack => OnDropItem(itemStack));
        }

        private void ClearHeldItem()
            => heldItemSocket.DestroyAllChild();

        protected virtual void OnItemStack(ItemStack itemStack)
        {
            ClearHeldItem();
            Vector3 spawnPosition = heldItemSocket.position;
            ItemController itemController = ItemSystem.SpawnHeldItem(spawnPosition, itemStack);

            GameObject go = itemController.gameObject;
            Transform tr = go.transform;
            tr.SetParent(heldItemSocket);
            tr.ResetLocal();
        }

        protected virtual void OnDropItem(ItemStack itemStack)
        {
            DropItemByForward(itemStack, transform.forward);
        }

        protected virtual void DropItemByForward(ItemStack itemStack, Vector3 forward)
        {
            Vector3 spawnPositoin = transform.position + 1.5f * Vector3.up;
            ItemController itemController = ItemSystem.SpawnDropItem(spawnPositoin, itemStack);

            GameObject go = itemController.gameObject;

            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            rigidbody.AddForce(forward * dropForce, ForceMode.Impulse);
        }
    }
}