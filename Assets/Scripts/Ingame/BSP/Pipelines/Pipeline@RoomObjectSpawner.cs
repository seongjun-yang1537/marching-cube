using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class RoomObjectSpawner : IBSP3DGenerationStep
    {
        public RoomObjectTable GetTestTable() => RoomObjectDB.GetTable("test");

        private GameObject Spawn(GameObject prefab, Transform parent = null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var instance = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
                UnityEditor.Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
                return instance;
            }
#endif
            return UnityEngine.Object.Instantiate(prefab, parent);
        }

        private void SpawnStartPoint(MT19937 rng, BSP3DModel model)
        {
        }

        private void SpawnPortalRoom(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;

            BSP3DRoom portalRoom = mapAsset.FindPortalRoom();
            if (portalRoom == null) return;

            RoomObjectTable table = GetTestTable();
            GameObject portal = table["DungeonPortal"];
            if (portal == null)
                return;

            BSP3DProjectionGrid projGrid = portalRoom.projectionGrid[BSP3DCubeFace.BOTTOM];
            Vector3 center = new Vector3()
            .Centroid(
                CIterator.GetArray2D(projGrid.size)
                .Where(idx => projGrid[idx] != -Vector3Int.one)
                .Select(idx => projGrid[idx].ToVector3())
                .ToList()
            );

            // TODO: Optimize O(nlogn) -> O(n)
            Vector2Int centerIdx = CIterator.GetArray2D(projGrid.size)
                .Where(idx => projGrid[idx] != -Vector3Int.one)
                .OrderBy(idx => (projGrid[idx].ToVector3() - center).sqrMagnitude)
                .First();

            // Vector3 centerPosition;
            // if (projGrid.RaycastLandscape(centerIdx, out centerPosition, model.transform.localToWorldMatrix))
            // {
            //     GameObject go = Spawn(portal, model.transform);
            //     Transform tr = go.transform;
            //     tr.position = centerPosition;
            // }

            GameObject go = Spawn(portal, model.transform);
            Transform tr = go.transform;
            tr.position = projGrid[centerIdx];
        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            SpawnStartPoint(rng, model);
            yield return null;
            SpawnPortalRoom(rng, model);
            yield return null;
        }
    }
}