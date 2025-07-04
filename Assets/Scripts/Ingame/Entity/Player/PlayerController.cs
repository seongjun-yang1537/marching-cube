using UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Ingame
{
    public class PlayerController : AgentController
    {
        public PlayerModel playerModel { get; private set; }

        private IJetpackable jetpackable;

        public void Construct(IJetpackable jetpackable)
        {
            this.jetpackable = jetpackable;
        }

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

            playerModel.isSprint = Input.GetKey(playerModel.sprintKey) && playerModel.stemina > 0.5f;

            if (Input.GetKey(playerModel.sprintKey) && playerModel.stemina > 0f)
                playerModel.ReduceStemina(Time.deltaTime);
            else
                playerModel.RecoverStemina(Time.deltaTime * playerModel.steminaMax);

            playerModel.nowSpeed = playerModel.isSprint ? playerModel.sprintSpeed : playerModel.groundSpeed;

            if (Input.GetKeyDown(KeyCode.E))
            {
                UIManager.Instance.ToggleInventoryUI();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                DropItem(playerModel.heldItemSlot);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                jetpackable.ActivateJetpack();
            }
            else
            {
                jetpackable.DeactivateJetpack();
            }
        }

        private void FixedUpdate()
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            Vector3 forward = transform.forward;

            if (Physics.Raycast(origin, forward, out hit, 1f))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle <= playerModel.slopeLimit)
                {
                    Vector3 climbDir = Vector3.ProjectOnPlane(forward, hit.normal).normalized + Vector3.up * 0.3f;
                    float climbSpeed = playerModel.nowSpeed * 0.5f;
                    GetComponent<Rigidbody>().MovePosition(transform.position + climbDir * climbSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}