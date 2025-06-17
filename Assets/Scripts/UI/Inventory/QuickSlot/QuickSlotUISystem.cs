using Ingame;
using UnityEngine;

namespace UI
{
    public class QuickSlotUISystem : UIComponentBahaviour
    {
        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }

        private UIQuickSlots slots { get => GetChildUI<UIQuickSlots>(); }

        protected void Awake()
        {
            base.Awake();

            inventory.onChangedQuickSlot.AddListener(itemSlot => Render());
        }

        protected void Start()
        {
            Render();
        }

        public override void Render()
        {
            slots.SetContainer(inventory.quickSlotContainer);
        }
    }
}