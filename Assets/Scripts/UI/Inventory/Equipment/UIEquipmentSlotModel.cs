using Corelib.Utils;
using Ingame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEquipmentSlotModel : UIItemSlot
    {
        public Image imgIcon;

        private void Start()
        {
            Render();
        }

        public override void Render()
        {
            if (itemStack == null)
            {
                SetIconUI(null);
                return;
            }

            SetIconUI(ItemDB.GetIconSprite(itemStack.itemID));
        }

        private void SetIconUI(Sprite sprite)
        {
            imgIcon.sprite = sprite;
            imgIcon.color = sprite == null ? Color.white.SetAlpha(0) : Color.white;
        }
    }
}