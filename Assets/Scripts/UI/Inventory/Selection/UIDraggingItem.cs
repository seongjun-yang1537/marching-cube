using Corelib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ingame
{
    public class UIDraggingItem : UIComponentBahaviour
    {
        public ItemStack itemStack { get; private set; }

        public Image imgIcon;
        public TextMeshProUGUI txtCount;

        protected void OnEnable()
        {
            SetActive(false);
        }

        public void SetItemStack(ItemStack itemStack)
        {
            this.itemStack = itemStack;
            Render();
        }

        public override void Render()
        {
            if (itemStack == null)
            {
                SetActive(false);
                SetCountUI(0);
                SetIconUI(null);
                return;
            }

            SetActive(true);
            SetCountUI(itemStack.count);
            SetIconUI(ItemDB.GetIconSprite(itemStack.itemID));
        }

        private void SetActive(bool active)
        {
            imgIcon.gameObject.SetActive(active);
            txtCount.gameObject.SetActive(active);
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