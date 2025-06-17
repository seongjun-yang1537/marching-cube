using Ingame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIItemSlot : UIComponentBahaviour,
        IPointerDownHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public UnityEvent onClick = new();
        public UnityEvent<PointerEventData> onDragStart = new();
        public UnityEvent<PointerEventData> onDrag = new();
        public UnityEvent<PointerEventData> onDragEnd = new();

        public ItemSlot itemSlot { get; private set; }
        protected ItemStack itemStack { get => itemSlot?.itemStack; }

        public virtual void SetItemSlot(ItemSlot itemSlot)
        {
            this.itemSlot = itemSlot;
            Render();
        }

        public virtual void SetItemStack(ItemStack itemStack)
        {
            itemSlot.itemStack = itemStack;
            Render();
        }

        public override void Render()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClick.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onDragStart.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onDragEnd.Invoke(eventData);
        }
    }
}