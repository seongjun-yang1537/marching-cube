using Ingame;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class UIInventorySelection : UIComponentBahaviour
    {
        public float moveTimer = 0.1f;

        public UIItemSlot nowSelectSlot;

        public override void Render()
        {
            transform.DOMove(nowSelectSlot.transform.position, moveTimer);
        }
    }
}