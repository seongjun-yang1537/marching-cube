// Ingame/AgentModel.cs

using UnityEngine;
using System;
using Unity.VisualScripting;

namespace Ingame
{
    public class AgentModel : EntityModel
    {
        public float groundSpeed;
        public float flySpeed;
        public float jumpForce;

        public Transform body;
    }
}