using System.Collections;
using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class DungeonManager : Singleton<DungeonManager>
    {
        public UnityEvent onNextFloorStart = new();
        public UnityEvent<int> onDungeonCreated = new();
        public UnityEvent onNextFloorEntered = new();

        public UnityEvent<float> onDungeonGenerationProgress;

        public DungeonModel dungeonModel;

        public DungeonScreenFadeView screenFadeView;

        public int floor;

        public void EnterNextFloor() => StartCoroutine(NextFloorProgress());

        IEnumerator NextFloorProgress()
        {
            onNextFloorStart.Invoke();

            BSP3DGenerationContext context = new BSP3DGenerationContext.Builder(
                MT19937.Create(dungeonModel.seed)
            )
            .ProgressCallback(progress => onDungeonGenerationProgress.Invoke(progress))
            .Build();

            yield return screenFadeView.FadeIn();
            yield return dungeonModel.GenerateAsync(context);
            yield return screenFadeView.FadeOut();

            onNextFloorEntered.Invoke();
        }
    }
}