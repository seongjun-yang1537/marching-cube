using Ingame;
using UnityEngine;

namespace UI
{
    public class EquipmentUISystem : UIComponentBahaviour
    {

        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }

        private UIEquipmentSlots slots { get => GetChildUI<UIEquipmentSlots>(); }

        protected void Awake()
        {
            base.Awake();

            inventory.onChangedEquipment.AddListener(itemSlot => Render());
        }

        protected void Start()
        {
            Render();
        }

        public override void Render()
        {
            slots.SetContainer(inventory.equipmentContainer);
        }
    }
}