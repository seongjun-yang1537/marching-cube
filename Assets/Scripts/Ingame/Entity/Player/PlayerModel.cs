using UnityEngine;
using System;
using UnityEngine.Events;

namespace Ingame
{

    public class PlayerModel : AgentModel
    {
        public UnityEvent<float> onStemina = new();
        public UnityEvent<float> onSteminaMax = new();

        public float rotationSpeed = 1.0f;

        public float sprintSpeed;
        public float characterTurnSpeed;
        public float observerSpeed;

        public KeyCode sprintKey = KeyCode.LeftControl;
        public KeyCode jumpKey = KeyCode.Space;

        public bool isSprint;
        public float slopeLimit;

        public float stemina { get; private set; }
        public float steminaMax { get; private set; } = 5f;
        public float steminaRatio { get => Mathf.Approximately(0f, steminaMax) ? 0f : stemina / steminaMax; }

        public float fuel { get; private set; }
        public float fuelMax { get; private set; }
        public float fuelRatio { get => Mathf.Approximately(0f, fuelMax) ? 0f : fuel / fuelMax; }

        protected void Awake()
        {
            base.Awake();
            stemina = steminaMax;
        }

        public void SetStemina(float newStemina)
        {
            stemina = newStemina;
            onStemina.Invoke(newStemina);
        }

        public void SetSteminaMax(float newSteminaMax)
        {
            steminaMax = newSteminaMax;
            onSteminaMax.Invoke(newSteminaMax);
        }

        public void ReduceStemina(float delta)
        {
            SetStemina(Mathf.Max(0f, stemina - delta));
        }

        public void RecoverStemina(float delta)
        {
            SetStemina(Mathf.Min(steminaMax, stemina + delta));
        }
    }
}