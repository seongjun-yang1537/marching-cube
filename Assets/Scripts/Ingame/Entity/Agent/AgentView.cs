using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(AgentModel))]
    public class AgentView : EntityView
    {
        protected AgentModel agentModel;

        public Transform heldItemSocket;

        protected void Awake()
        {
            base.Awake();

            agentModel = (AgentModel)entityModel;

            agentModel.onHeldItem.AddListener(itemSlot => OnItemStack(itemSlot.itemStack));
        }

        private void ClearHeldItem()
            => heldItemSocket.DestroyAllChild();

        private GameObject CreateHeldItem(ItemStack itemStack)
        {
            GameObject go = Instantiate(ItemDB.GetItemModel(itemStack.itemID));
            return go;
        }

        private void OnItemStack(ItemStack itemStack)
        {
            ClearHeldItem();
            GameObject go = CreateHeldItem(itemStack);
            Transform tr = go.transform;
            tr.SetParent(heldItemSocket);
            tr.ResetLocal();
        }
    }
}