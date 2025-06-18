using DG.Tweening;
using Ingame;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPlayerFuelGauge : UIComponentBahaviour
    {
        public float animDuration = 0.25f;
        public float ratio = 1.0f;

        private Image img;

        protected void Awake()
        {
            base.Awake();
            img = GetComponent<Image>();
        }

        protected void Start()
        {
            SetRatio(1.0f);
        }

        public void SetRatio(float ratio)
        {
            this.ratio = ratio;
            Render();
        }

        public override void Render()
        {
            img.DOFillAmount(ratio, animDuration);
        }
    }
}