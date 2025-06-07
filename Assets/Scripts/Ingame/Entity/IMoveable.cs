using UnityEngine;

namespace Ingame
{
    public interface IMoveable
    {
        public void Move(Vector3 direction);

        public bool IsGrounded();
        public void Jump();
    }
}