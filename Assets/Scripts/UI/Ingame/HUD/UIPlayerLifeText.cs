using DG.Tweening;
using Ingame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPlayerLifeText : UIComponentBahaviour
    {
        public float animDuration = 0.25f;

        public float life = 0f;
        public float lifeMax = 0f;

        private TextMeshProUGUI txt;

        protected void Awake()
        {
            base.Awake();
            txt = GetComponent<TextMeshProUGUI>();
        }

        protected void Start()
        {
            SetLifeData(0f, 0f);
        }

        public void SetLifeData(float life, float lifeMax)
        {
            this.life = life;
            this.lifeMax = lifeMax;
            Render();
        }

        public override void Render()
        {
            txt.text = $"{Mathf.RoundToInt(life)} / {Mathf.RoundToInt(lifeMax)}";
        }
    }
}