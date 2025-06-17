using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public enum ItemModelState
    {
        Held,
        Dropped,
    }

    public class ItemModel : MonoBehaviour
    {
        public UnityEvent<ItemModelState> onState = new();

        public ItemStack itemStack;
        public ItemID itemID { get => itemStack.itemID; }
        public int count { get => itemStack.count; }

        public ItemModelState state = ItemModelState.Held;

        public void SetState(ItemModelState state)
        {
            this.state = state;
            onState.Invoke(state);
        }
    }
}