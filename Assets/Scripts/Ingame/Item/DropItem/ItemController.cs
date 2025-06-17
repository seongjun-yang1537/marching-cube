using Corelib.Utils;
using UnityEngine;
namespace Ingame
{
    [RequireComponent(typeof(ItemModel))]
    public class ItemController : MonoBehaviour
    {
        public float rotateSpeed = 30f;
        public float hoverHeight = 0.1f;
        public float hoverSpeed = 2.5f;

        private ItemModel itemModel;

        private Rigidbody rigidbody;
        private BoxCollider boxCollider;

        protected void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            boxCollider = GetComponent<BoxCollider>();

            itemModel = GetComponent<ItemModel>();
            ChangeItemState(ItemModelState.Held);
        }

        public void ChangeItemState(ItemModelState state)
        {
            itemModel.SetState(state);
            UpdatePhysicalState(state);
        }

        private void UpdatePhysicalState(ItemModelState state)
        {
            switch (state)
            {
                case ItemModelState.Held:
                    {
                        rigidbody.isKinematic = true;
                        boxCollider.enabled = false;
                    }
                    break;
                case ItemModelState.Dropped:
                    {
                        rigidbody.isKinematic = false;
                        boxCollider.enabled = true;
                    }
                    break;
            }
        }
    }
}