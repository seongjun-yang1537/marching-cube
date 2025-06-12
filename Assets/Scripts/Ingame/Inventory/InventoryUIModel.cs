using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class InventoryUIModel : MonoBehaviour
    {
        public UnityEvent<bool> onVisible;

        public bool visible { get; private set; }

        public void SetVisible(bool visible)
        {
            this.visible = visible;
            onVisible.Invoke(visible);
        }
    }
}