using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class EntityModel : MonoBehaviour
    {
        public UnityEvent onSpawn = new();
        public UnityEvent onDeSpawn = new();
        public UnityEvent<EntityModel, float> onDamaged = new();
        public UnityEvent<EntityModel> onDead = new();

        public float life;
        public float lifeMax;

        public bool isDead { get => isSpanwed && life <= 0; }

        public bool isSpanwed;
        public bool isInvincible;
    }
}
