using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Ingame
{
    [CreateAssetMenu(fileName = "New Room Object Table", menuName = "ScriptableObject/Room Object Table")]
    public class RoomObjectTable : SerializedScriptableObject
    {
        public Dictionary<string, GameObject> prefabs;

        public GameObject this[string name]
        {
            get => prefabs.ContainsKey(name) ? prefabs[name] : null;
        }
    }
}