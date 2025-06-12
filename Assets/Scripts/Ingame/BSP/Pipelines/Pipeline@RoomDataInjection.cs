using System;
using System.Collections;
using System.Collections.Generic;
using Corelib.Utils;
using MCube;
using Sirenix.Utilities;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class RoomDataInjection : IBSP3DGenerationStep
    {
        private List<Vector3Int> RoomNodeBFS(ScalarField scalarField, BSP3DTreeNode roomNode)
        {
            BSP3DRoom room = roomNode.room;
            Vector3Int startPos = room.center.RoundToInt();
            List<Vector3Int> poses = new();

            Dictionary<Vector3Int, bool> vit = new();
            Queue<Vector3Int> queue = new();
            queue.Enqueue(startPos);
            while (queue.Count > 0)
            {
                Vector3Int top = queue.Dequeue();
                poses.Add(top);

                foreach (Vector3Int dir in ExVector3Int.DIR6)
                {
                    Vector3Int to = top + dir;
                    if (!Mathf.Approximately(scalarField[to], 0.0f) || vit.ContainsKey(to)) continue;
                    if (!scalarField.InRange(to) || !roomNode.cube.Contains(to)) continue;
                    vit.Add(to, true);
                    queue.Enqueue(to);
                }
            }

            return poses;
        }

        private void InjectionRoomProjectionGrid(ScalarField scalarField, BSP3DRoom room, List<Vector3Int> poses)
        {
            room.InitializeProjectionGrind(poses);
            foreach (var grid in room.ProjectionGrids)
            {
                Vector3Int normal = grid.plane.normal;
                CIterator.GetArray2D(grid.size).ForEach(idx =>
                {
                    if (grid[idx] == -Vector3Int.one) return;
                    Vector3Int to = grid[idx] + normal;
                    if (!scalarField.InRange(to) || Mathf.Approximately(scalarField[to], 0.0f))
                    {
                        grid[idx] = -Vector3Int.one;
                    }
                });
            }
        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            BSP3DMapAsset mapAsset = model.mapAsset;
            List<BSP3DTreeNode> roomNodes = mapAsset.GetRoomNodes();
            foreach (var roomNode in roomNodes)
            {
                List<Vector3Int> poses = RoomNodeBFS(mapAsset.scalarField, roomNode);
                InjectionRoomProjectionGrid(mapAsset.scalarField, roomNode.room, poses);
                yield return null;
            }
        }
    }
}