using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class RoomEntitySpawner : IBSP3DGenerationStep
    {
        public EntityPresetTable GetTestTable() => EntityPresetDB.GetTable("Test");

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

        private void SpawnOres(MT19937 rng, BSP3DModel model)
        {
            EntityPresetTable table = GetTestTable();
            string[] entityNames = new[]{
                "Beholder",
                "Slime",
                "Turtle",
            };
            GameObject PickEntityGameObject()
            {
                string pickEntityName = rng.Choice(entityNames);
                GameObject ore = table[pickEntityName];
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
                        GameObject go = Spawn(PickEntityGameObject(), model.transform);
                        Transform tr = go.transform;
                        tr.position = centerPosition;
                    }
                }
            }
        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            SpawnOres(rng, model);
            yield return null;
        }
    }
}