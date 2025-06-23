using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public static class EntityPresetDB
    {
        private const string PATH_PREFIX = "Dungeon/EntityPreset";
        private static EntityPresetTable table;

        public static EntityPresetTable GetTable(string path)
        {
            if (table == null)
            {
                table = Resources.Load<EntityPresetTable>($"{PATH_PREFIX}/{path}");
                if (table == null)
                {
                    Debug.LogWarning($"[EntityPresetDB] Failed to load: {PATH_PREFIX}/{path}");
                }
            }
            return table;
        }
    }
}