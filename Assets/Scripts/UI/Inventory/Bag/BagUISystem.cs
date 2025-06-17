using Ingame;
using UnityEngine;

namespace UI
{
    public class BagUISystem : UIComponentBahaviour
    {
        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }

        private UIBagSlots slots { get => GetChildUI<UIBagSlots>(); }

        protected void Awake()
        {
            base.Awake();

            inventory.onChangedBag.AddListener(itemSlot => Render());
        }

        protected void Start()
        {
            Render();
        }

        public override void Render()
        {
            slots.SetContainer(inventory.bagContainer);
        }
    }
}