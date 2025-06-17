using Corelib.Utils;
using UnityEngine;
namespace Ingame
{
    public class DropItemController : MonoBehaviour
    {
        public float rotateSpeed = 30f;
        public float hoverHeight = 0.1f;
        public float hoverSpeed = 2.5f;

        public Transform body;

        protected void Awake()
        {
            body = transform.FindInChild(nameof(body));
        }

        protected void Update()
        {
            UpdateAnimation();
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