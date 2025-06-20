using Ingame;
using UnityEngine;

namespace UI
{
    public class PlayerHUDUISystem : UIComponentBahaviour
    {
        private UIPlayerLifeGauge uiPlayerLifeGauge;
        private UIPlayerSteminaGauge uiPlayerSteminaGauge;
        private UIPlayerLifeText uiPlayerLifeText;
        private UIPlayerFuelGauge uiPlayerFuelGauge;

        private PlayerModel playerModel { get => PlayerManager.Instance.PlayerModel; }

        protected void Start()
        {
            uiPlayerLifeGauge = GetChildUI<UIPlayerLifeGauge>();
            uiPlayerSteminaGauge = GetChildUI<UIPlayerSteminaGauge>();
            uiPlayerLifeText = GetChildUI<UIPlayerLifeText>();
            uiPlayerFuelGauge = GetChildUI<UIPlayerFuelGauge>();

            playerModel.onLife.AddListener(life => Render());
            playerModel.onLifeMax.AddListener(lifeMax => Render());
            playerModel.onStemina.AddListener(stemina => Render());
            playerModel.onSteminaMax.AddListener(steminaMax => Render());

            Render();
        }

        public override void Render()
        {
            uiPlayerLifeGauge.SetRatio(playerModel.lifeRatio);
            uiPlayerSteminaGauge.SetRatio(playerModel.steminaRatio);
            uiPlayerLifeText.SetLifeData(playerModel.life, playerModel.lifeMax);
            uiPlayerFuelGauge.SetRatio(playerModel.fuelRatio); ;
        }
    }
}