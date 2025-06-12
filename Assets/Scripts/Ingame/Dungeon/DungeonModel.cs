using System.Collections;
using System.Collections.Generic;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(BSP3DModel))]
    public class DungeonModel : MonoBehaviour
    {
        public int seed;

        private BSP3DModel _bspModel;
        public BSP3DModel BSPModel { get => _bspModel ??= GetComponent<BSP3DModel>(); }

        public PlayerManager playerManager { get => PlayerManager.Instance; }
        public DungeonManager dungeonManager { get => DungeonManager.Instance; }

        private void Awake()
        {
            dungeonManager.onNextFloorStart.AddListener(() =>
            {
                Reseed();
            });

            dungeonManager.onNextFloorEntered.AddListener(() =>
            {
                TeleportPlayerAtStartRoom();
            });
        }

        private void Start()
        {
            TeleportPlayerAtStartRoom();
        }

        public void TeleportPlayerAtStartRoom()
            => TeleportPlayerRoom(BSPModel.FindStartRoom());

        public void TeleportPlayerRoom(BSP3DRoom room, MT19937 rng = null)
            => playerManager.TeleportPlayer(room.SelectSpawnPosition(rng));

        public void Reseed()
        {
            seed = MT19937.Create().NextInt(0, 10000);
        }

        public IEnumerator GenerateAsync(BSP3DGenerationContext context)
        {
            yield return BSPModel.GenerateAsync(context);
        }
    }
}