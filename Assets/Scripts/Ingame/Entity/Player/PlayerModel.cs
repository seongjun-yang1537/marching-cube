using UnityEngine;
using System;

namespace Ingame
{

    public class PlayerModel : AgentModel
    {
        public float rotationSpeed = 1.0f;

        public float sprintSpeed;
        public float characterTurnSpeed;
        public float observerSpeed;

        public KeyCode sprintKey = KeyCode.LeftControl;
        public KeyCode jumpKey = KeyCode.Space;

        public bool isSprint;
        public float slopeLimit;
    }
}