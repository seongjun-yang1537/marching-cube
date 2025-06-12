using System.Collections;
using System.Collections.Generic;
using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class BSP3DGenerationContext
    {
        public BSP3DGenerationPreset preset;
        public BSP3DModel model;
        public MT19937 rng;
        public UnityAction<float> onProgress;

        private BSP3DGenerationContext() { }

        public class Builder
        {
            private readonly BSP3DGenerationContext context = new();

            public Builder(MT19937 rng)
            {
                context.rng = rng;
            }
            public Builder(MT19937 rng, BSP3DModel model)
            {
                context.rng = rng;
                context.model = model;
            }

            public Builder Preset(BSP3DGenerationPreset preset)
            {
                context.preset = preset;
                return this;
            }

            public Builder ProgressCallback(UnityAction<float> onProgress)
            {
                context.onProgress = onProgress;
                return this;
            }

            public BSP3DGenerationContext Build()
            {
                return context;
            }
        }
    }
    public class BSP3DGenerationPipeline
    {
        private List<IBSP3DGenerationStep> steps = new();

        public static BSP3DGenerationPipeline Create()
            => new BSP3DGenerationPipeline();

        public BSP3DGenerationPipeline AddStep(IBSP3DGenerationStep step)
        {
            steps.Add(step);
            return this;
        }

        public IEnumerator ExecuteAsync(BSP3DGenerationContext context)
        {
            int maxSteps = steps.Count;
            context.onProgress?.Invoke(0f);
            for (int i = 0; i < maxSteps; i++)
            {
                IBSP3DGenerationStep step = steps[i];

                var sw = System.Diagnostics.Stopwatch.StartNew();
                {
                    yield return step.ExecuteAsync(context.rng, context.model);
                }
                sw.Stop();
                Debug.Log($"[Pipeline] Step {step.GetType().Name} took {sw.ElapsedMilliseconds} ms");

                context.onProgress?.Invoke(1.0f * (i + 1) / maxSteps);
            }
        }
    }
}