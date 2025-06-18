using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEditor;
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

            Vector3 centerPosition;
            if (projGrid.RaycastLandscape(centerIdx, out centerPosition, model.transform.localToWorldMatrix))
            {
                GameObject go = Spawn(portal, model.transform);
                Transform tr = go.transform;
                tr.position = centerPosition;
            }

            // GameObject go = Spawn(portal, model.transform);
            // Transform tr = go.transform;
            // tr.position = projGrid[centerIdx];
        }

        private void SpawnTreasureChecst(MT19937 rng, BSP3DModel model)
        {
            RoomObjectTable table = GetTestTable();
            GameObject chest = table["TreasureChest"];
            if (chest == null)
                return;

            BSP3DMapAsset mapAsset = model.mapAsset;

            List<BSP3DRoom> rooms = mapAsset.GetRooms();
            foreach (BSP3DRoom room in rooms)
            {
                BSP3DProjectionGrid projGrid = room.projectionGrid[BSP3DCubeFace.BOTTOM];

                int count = rng.NextFloat() < 0.95f ? 0 : rng.NextInt(1, 2);
                List<Vector2Int> indices = projGrid.GetValidIndicies().Shuffle(rng).Take(count).ToList();

                foreach (Vector2Int idx in indices)
                {
                    Vector3 centerPosition;
                    if (projGrid.RaycastLandscape(idx, out centerPosition, model.transform.localToWorldMatrix))
                    {
                        GameObject go = Spawn(chest, model.transform);
                        Transform tr = go.transform;
                        tr.position = centerPosition;
                    }
                }
            }
        }

        private void SpawnOres(MT19937 rng, BSP3DModel model)
        {
            RoomObjectTable table = GetTestTable();
            string[] oreNames = new[]{
                "CrystalOre",
                "DiamondOre",
                "GoldOre",
                "RapiseOre",
            };
            GameObject PickOreGameObject()
            {
                string pickOreName = rng.Choice(oreNames);
                GameObject ore = table[pickOreName];
                return ore;
            }

            BSP3DMapAsset mapAsset = model.mapAsset;

            List<BSP3DRoom> rooms = mapAsset.GetRooms();
            foreach (BSP3DRoom room in rooms)
            {
                BSP3DProjectionGrid projGrid = room.projectionGrid[BSP3DCubeFace.BOTTOM];

                int count = rng.NextFloat() < 0.75f ? 0 : rng.NextInt(1, 3);
                List<Vector2Int> indices = projGrid.GetValidIndicies().Shuffle(rng).Take(count).ToList();

                foreach (Vector2Int idx in indices)
                {
                    Vector3 centerPosition;
                    if (projGrid.RaycastLandscape(idx, out centerPosition, model.transform.localToWorldMatrix))
                    {
                        GameObject go = Spawn(PickOreGameObject(), model.transform);
                        Transform tr = go.transform;
                        tr.position = centerPosition;
                    }
                }
            }
        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            SpawnStartPoint(rng, model);
            yield return null;
            SpawnPortalRoom(rng, model);
            yield return null;
            SpawnTreasureChecst(rng, model);
            yield return null;
            SpawnOres(rng, model);
            yield return null;
        }
    }
}