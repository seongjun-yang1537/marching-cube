using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(AgentModel))]
    [RequireComponent(typeof(AgentMoveable))]
    public class AgentController : EntityController
    {
        public AgentModel agentModel;

        protected IMoveable moveable;

        protected void Awake()
        {
            base.Awake();

            agentModel = (AgentModel)entityModel;
            moveable = GetComponent<AgentMoveable>();
        }

        public virtual void Move(Vector3 direction)
        {
            moveable?.Move(direction);
        }

        public virtual void Jump()
        {
            moveable?.Jump();
        }

        public bool IsGrounded()
        {
            return moveable?.IsGrounded() ?? false;
        }
    }
}