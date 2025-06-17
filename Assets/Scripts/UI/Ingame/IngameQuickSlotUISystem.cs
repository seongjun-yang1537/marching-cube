using Corelib.Utils;
using Ingame;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class IngameQuickSlotUISystem : UIComponentBahaviour
    {
        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }
        public UnityEvent<int> onKeyDownQuick = new();

        private UIQuickSlots slots { get => GetChildUI<UIQuickSlots>(); }
        private UIIngameQuickSelection uiSelection { get => GetChildUI<UIIngameQuickSelection>(); }

        protected void Awake()
        {
            base.Awake();

            inventory.onChangedQuickSlot.AddListener(itemSlot => Render());
            onKeyDownQuick.AddListener(num => OnKeyDownQuick(num));
        }

        protected void Start()
        {
            Render();
        }

        protected void Update()
        {
            int count = ExEnum.Count<QuickSlotID>();
            for (int i = 1; i <= count; i++)
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    onKeyDownQuick.Invoke(i - 1);
        }

        public override void Render()
        {
            slots.SetContainer(inventory.quickSlotContainer);
        }

        private void OnKeyDownQuick(int num)
        {
            uiSelection.SetIndex(num);
        }
    }
}