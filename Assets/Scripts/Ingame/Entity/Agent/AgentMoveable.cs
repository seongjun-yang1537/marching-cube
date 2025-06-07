using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class AgentMoveable : MonoBehaviour, IMoveable
    {
        private const float MOVE_SPEED_CONSTANT = 3.0f;
        private const float JUMP_FORCE_CONSTANT = 500.0f;

        protected AgentController agentController;
        protected AgentModel agentModel { get => agentController.agentModel; }

        protected Rigidbody rigidbody;
        protected CapsuleCollider capsuleCollider;

        protected Vector3 moveDirection;
        private float moveSpeed;

        protected void Awake()
        {
            agentController = GetComponent<AgentController>();
            rigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        protected void Update()
        {

        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        protected virtual void ApplyMovement()
        {
            moveSpeed = Mathf.Lerp(moveSpeed, agentModel.groundSpeed * MOVE_SPEED_CONSTANT, 0.1f);
            rigidbody.velocity = new Vector3(
                moveDirection.x * moveSpeed,
                rigidbody.velocity.y,
                moveDirection.z * moveSpeed
            );
        }

        public void Move(Vector3 direction)
        {
            direction = Camera.main.transform.TransformDirection(direction);
            moveDirection = direction.normalized;
        }

        public bool IsGrounded()
        {
            return Physics.CheckSphere(
                transform.position,
                capsuleCollider.radius,
                LayerMask.GetMask("Landscape")
            );
        }

        public void Jump()
        {
            if (!IsGrounded())
                return;

            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            rigidbody.AddForce(
                Vector3.up * agentModel.jumpForce * JUMP_FORCE_CONSTANT,
                ForceMode.Impulse
            );
        }
    }
}