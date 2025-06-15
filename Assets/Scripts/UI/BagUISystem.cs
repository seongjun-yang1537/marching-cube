using Ingame;
using UnityEngine;

namespace UI
{
    public class BagUISystem : UIComponentBahaviour
    {
        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }

        private BagSlotUISystem slotUI { get => GetChildUI<BagSlotUISystem>(); }

        protected void Awake()
        {
            base.Awake();

            inventory.onChangedBag.AddListener((slotID, itemStack) => Render());
            inventory.onChangedQuickSlot.AddListener((slotID, itemStack) => Render());
            inventory.onChangedEquipment.AddListener((slotID, itemStack) => Render());
        }

        public override void Render()
        {
            slotUI.SetContainer(inventory.bagContainer);
        }
    }
}