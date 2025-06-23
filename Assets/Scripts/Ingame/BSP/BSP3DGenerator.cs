using System.Collections;
using Corelib.Utils;
using Ingame.Pipelines;
using MCube;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public enum BSP3DGenerationPreset
    {
        CAVE,
        TESTBED,
    }

    public class BSP3DGenerator
    {
        private static readonly BSP3DGenerationPipeline cavePipeline =
            BSP3DGenerationPipeline.Create()
            .AddStep(new Pipelines.BSPTreeBuilder())
            .AddStep(new Pipelines.RoomGenerator())
            .AddStep(new Pipelines.MapGraphGenerator())
            .AddStep(new Pipelines.RoomGraphGenerator())
            .AddStep(new Pipelines.ScalarFieldGenerator())
            .AddStep(new Pipelines.CellularAutomataProcessor())
            .AddStep(new Pipelines.ScalaFieldPostProcessor())
            .AddStep(new Pipelines.RoomDataInjection())
            .AddStep(new Pipelines.ScalarFieldMeshBuilder())
            .AddStep(new Pipelines.RoomObjectSpawner())
            .AddStep(new Pipelines.RoomEntitySpawner());

        private static readonly BSP3DGenerationPipeline testbedPipeline =
            BSP3DGenerationPipeline.Create()
            .AddStep(new Pipelines.BSPTestbedTreeBuilder())
            .AddStep(new Pipelines.TestbedRoomGenerator())
            .AddStep(new Pipelines.MapGraphGenerator())
            .AddStep(new Pipelines.RoomGraphGenerator())
            .AddStep(new Pipelines.ScalarFieldGenerator())
            .AddStep(new Pipelines.ScalaFieldPostProcessor())
            .AddStep(new Pipelines.RoomDataInjection())
            .AddStep(new Pipelines.ScalarFieldMeshBuilder())
            .AddStep(new Pipelines.RoomObjectSpawner());

        public static IEnumerator GenerateAsync(BSP3DGenerationContext context)
        {
            yield return GenerateFactory(context);
        }

        private static IEnumerator GenerateFactory(BSP3DGenerationContext context)
        {
            switch (context.preset)
            {
                case BSP3DGenerationPreset.CAVE:
                    yield return cavePipeline.ExecuteAsync(context);
                    break;
                case BSP3DGenerationPreset.TESTBED:
                    yield return testbedPipeline.ExecuteAsync(context);
                    break;
            }
            yield return null;
        }
    }
}