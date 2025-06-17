using DG.Tweening;
using Ingame;
using UnityEngine;

namespace UI
{
    public class UIIngameQuickSelection : UIComponentBahaviour
    {
        public float moveTimer = 0.1f;

        public UIQuickSlots uiQuickSlots;
        private UIItemSlot selectedUIItemSlot;

        public void SetIndex(int idx)
        {
            selectedUIItemSlot = uiQuickSlots.uiSlotModels[idx];
            Render();
        }

        public override void Render()
        {
            transform.DOMove(selectedUIItemSlot.transform.position, moveTimer);
        }
    }
}