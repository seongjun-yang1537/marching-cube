using Corelib.Utils;
using Ingame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIQuickSlotModel : UIItemSlot
    {
        public Image imgIcon;
        public TextMeshProUGUI txtCount;

        private void Start()
        {
            Render();
        }

        public override void Render()
        {
            if (itemStack == null)
            {
                SetCountUI(0);
                SetIconUI(null);
                return;
            }

            SetCountUI(itemStack.count);
            SetIconUI(ItemDB.GetIconSprite(itemStack.itemID));
        }

        private void SetCountUI(int count)
        {
            if (count <= 1) txtCount.text = "";
            else txtCount.text = $"{count}";
        }

        private void SetIconUI(Sprite sprite)
        {
            imgIcon.sprite = sprite;
            imgIcon.color = sprite == null ? Color.white.SetAlpha(0) : Color.white;
        }
    }
}