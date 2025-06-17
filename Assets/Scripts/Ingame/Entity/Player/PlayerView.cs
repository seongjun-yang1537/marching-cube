using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(PlayerModel))]
    public class PlayerView : AgentView
    {
        protected PlayerModel playerModel;

        private Camera mainCamera;

        protected void Awake()
        {
            base.Awake();
            playerModel = (PlayerModel)agentModel;

            mainCamera = Camera.main;
        }

        protected override void OnDropItem(ItemStack itemStack)
        {
            DropItemByForward(itemStack, mainCamera.transform.forward);
        }
    }
}