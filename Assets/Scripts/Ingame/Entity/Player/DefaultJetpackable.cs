using UnityEngine;
using VContainer;

namespace Ingame
{
    public class DefaultJetpackable : MonoBehaviour, IJetpackable
    {
        private PlayerModel playerModel;

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