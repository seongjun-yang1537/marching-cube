using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public static class RoomObjectDB
    {
        private const string PATH_PREFIX = "RoomObjects";
        private static RoomObjectTable table;

        public static RoomObjectTable GetTable(string path)
        {
            if (table == null)
            {
                table = Resources.Load<RoomObjectTable>($"{PATH_PREFIX}/{path}");
                if (table == null)
                {
                    Debug.LogWarning($"[RoomObjectDB] Failed to load: {PATH_PREFIX}/{path}");
                }
            }
            return table;
        }
    }
}