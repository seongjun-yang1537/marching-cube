using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;
using System;

namespace UI
{
    public class BagSlotUISystem : UIComponentBahaviour
    {
        public BagContainer bagContainer { get; private set; }

        public List<BagSlotUIModel> slotUIModels;

        protected void Awake()
        {
            base.Awake();

            slotUIModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<BagSlotUIModel>())
                .Where(model => model != null)
                .ToList();
        }

        public void SetContainer(BagContainer container)
        {
            bagContainer = container;

            int slotCount = bagContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                slotUIModels[i].SetItemStack(bagContainer.GetItem((BagSlotID)i));

            Render();
        }

        public override void Render()
        {
            foreach (BagSlotUIModel model in slotUIModels)
                model.Render();
        }
    }
}