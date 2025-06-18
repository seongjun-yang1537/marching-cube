using UnityEngine;
using Zenject;

namespace Ingame
{
    public class DefaultJetpackable : MonoBehaviour, IJetpackable
    {


        private PlayerModel playerModel;

        [Inject]
        public void Construct(PlayerModel model)
        {
            playerModel = model;
        }

        public bool CanJetpack()
        {
            return true;
        }

        public void ActivateJetpack()
        {
            if (!CanJetpack()) return;
        }

        public void DeactivateJetpack()
        {

        }
    }
}