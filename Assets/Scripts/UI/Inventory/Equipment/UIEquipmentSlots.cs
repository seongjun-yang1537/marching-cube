using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;
using System;
using UnityEngine.UI;

namespace UI
{
    public class UIEquipmentSlots : UIComponentBahaviour
    {
        public EquipmentContainer equipmentContainer { get; private set; }

        public List<UIEquipmentSlotModel> uiSlotModels;

        protected void Awake()
        {
            base.Awake();

            uiSlotModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<UIEquipmentSlotModel>())
                .Where(model => model != null)
                .ToList();
        }

        private void Start()
        {
            Render();
        }

        public void SetContainer(EquipmentContainer container)
        {
            equipmentContainer = container;

            int slotCount = equipmentContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                uiSlotModels[i].SetItemSlot(equipmentContainer.GetSlot((EquipmentSlotID)i));

            Render();
        }

        public override void Render()
        {
            foreach (UIEquipmentSlotModel model in uiSlotModels)
                model.Render();
        }
    }
}