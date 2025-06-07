using TMPro;
using UnityEngine;

namespace Ingame
{
    public class PlayerMoveable : AgentMoveable
    {
        protected PlayerController playerController;
        protected PlayerModel playerModel;

        private Camera mainCamera;

        protected void Awake()
        {
            base.Awake();

            mainCamera = Camera.main;

            playerController = GetComponent<PlayerController>();
            playerModel = GetComponent<PlayerModel>();
        }

        protected void Update()
        {
            base.Update();
            UpdateBodyRotation();
        }

        private void UpdateBodyRotation()
        {
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            if (forward.sqrMagnitude < 0.01f) return;

            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}