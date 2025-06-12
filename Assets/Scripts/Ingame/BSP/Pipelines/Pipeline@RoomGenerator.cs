using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using UnityEngine;

namespace Ingame.Pipelines
{
    public class RoomGenerator : IBSP3DGenerationStep
    {
        private void GenerateRooms(MT19937 rng, BSP3DMapAsset mapAsset)
        {


            List<BSP3DTreeNode> leafs = mapAsset.GetLeafs();
            foreach (var leaf in leafs)
            {
                leaf.GenerateRoom(rng, Vector3.one * 0.5f, Vector3.one * 0.75f);
            }
        }

        private bool IsStartableRoom(BSP3DRoom room)
            => true;

        private void SelectStartRoom(MT19937 rng, BSP3DMapAsset mapAsset)
        {
            List<BSP3DRoom> rooms = mapAsset.GetRooms();
            BSP3DRoom startRoom = rooms.Choice();
            startRoom.roomType = BSP3DRoomType.Start;
        }

        private bool IsPortalableRoom(BSP3DRoom room)
            => room.size.GreaterEqual(Vector3Int.one * 3);

        private void SelectPortalRoom(MT19937 rng, BSP3DMapAsset mapAsset)
        {
            BSP3DRoom startRoom = mapAsset.FindStartRoom();

            List<BSP3DRoom> rooms = mapAsset.GetRooms()
            .Where(room => room.roomType != BSP3DRoomType.Start)
            .Where(room => IsPortalableRoom(room))
            .ToList();

            rooms.Sort((a, b) =>
                ((b.center - startRoom.center).sqrMagnitude)
                .CompareTo((a.center - startRoom.center).sqrMagnitude)
            );

            int catchCount = Math.Min(10, rooms.Count);
            BSP3DRoom portalRoom = rooms.Take(catchCount).ToList().Choice();
            portalRoom.roomType = BSP3DRoomType.Portal;
        }

        public void Execute(MT19937 rng, BSP3DModel model)
        {

        }

        public IEnumerator ExecuteAsync(MT19937 rng, BSP3DModel model)
        {
            GenerateRooms(rng, model.mapAsset);
            yield return null;

            SelectStartRoom(rng, model.mapAsset);
            yield return null;

            SelectPortalRoom(rng, model.mapAsset);
            yield return null;

        }
    }
}