using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Ingame
{
    [CreateAssetMenu(fileName = "New Entity Preset Table", menuName = "ScriptableObject/Entity Preset Table")]
    public class EntityPresetTable : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> prefabs;

        public GameObject this[string name]
        {
            get => prefabs.ContainsKey(name) ? prefabs[name] : null;
        }
    }
}