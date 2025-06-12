// Ingame/AgentModel.cs

using UnityEngine;
using System;
using Unity.VisualScripting;

namespace Ingame
{
    public class AgentModel : EntityModel
    {
        public float nowSpeed;
        public float groundSpeed;
        public float flySpeed;
        public float jumpForce;

        public Transform body;

        public Inventory inventory;

        protected void Awake()
        {
            nowSpeed = groundSpeed;
        }
    }
}