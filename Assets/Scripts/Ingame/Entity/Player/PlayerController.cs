using UnityEngine;

namespace Ingame
{
    public class PlayerController : AgentController
    {
        protected PlayerModel playerModel;

        protected void Awake()
        {
            base.Awake();

            playerModel = GetComponent<PlayerModel>();
        }

        public void Update()
        {
            Vector3 normal =
                new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))
                .normalized;
            Move(normal);

            if (Input.GetKeyDown(playerModel.jumpKey))
            {
                Jump();
            }
        }
    }
}