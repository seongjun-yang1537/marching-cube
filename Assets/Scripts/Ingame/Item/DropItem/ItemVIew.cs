using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(ItemModel))]
    public class ItemView : MonoBehaviour
    {
        public Transform body;
        public float rotateSpeed = 30f;
        public float hoverHeight = 0.1f;
        public float hoverSpeed = 2.5f;

        private ItemModel itemModel;

        protected void Awake()
        {
            body = transform.FindInChild(nameof(body));

            itemModel = GetComponent<ItemModel>();
        }

        protected void Update()
        {
            switch (itemModel.state)
            {
                case ItemModelState.Dropped:
                    {
                        UpdateAnimation();
                    }
                    break;
            }
        }

        private void UpdateAnimation()
        {
            transform.eulerAngles += Vector3.up * rotateSpeed * Time.deltaTime;

            float hoverOffset = (Mathf.Sin(Time.time * hoverSpeed) + 1) / 2 * hoverHeight;
            Vector3 offset = new Vector3(0, hoverOffset, 0);
            body.localPosition = offset;
        }
    }
}