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

        // TODO: move to ItemSystem
        private GameObject CreateHeldItem(ItemStack itemStack)
        {
            GameObject go = Instantiate(ItemDB.GetItemModel(itemStack.itemID));
            return go;
        }

        protected virtual void OnItemStack(ItemStack itemStack)
        {
            ClearHeldItem();
            GameObject go = CreateHeldItem(itemStack);
            Transform tr = go.transform;
            tr.SetParent(heldItemSocket);
            tr.ResetLocal();
        }

        // TODO: move to ItemSystem

        private GameObject CreateDropItem(ItemStack itemStack)
        {
            GameObject go = Instantiate(ItemDB.GetItemModel(itemStack.itemID));
            ItemController itemController = go.GetComponent<ItemController>();
            itemController.ChangeItemState(ItemModelState.Dropped);
            return go;
        }

        protected virtual void OnDropItem(ItemStack itemStack)
        {
            DropItemByForward(itemStack, transform.forward);
        }

        protected virtual void DropItemByForward(ItemStack itemStack, Vector3 forward)
        {
            GameObject go = CreateDropItem(itemStack);
            Transform tr = go.transform;
            tr.position = transform.position + 1.5f * Vector3.up;

            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            rigidbody.AddForce(forward * dropForce, ForceMode.Impulse);
        }
    }
}