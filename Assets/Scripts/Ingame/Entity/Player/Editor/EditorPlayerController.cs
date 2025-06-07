using UnityEngine;
using UnityEditor;

namespace Ingame
{
    [CustomEditor(typeof(PlayerController), true)]
    public class EditorPlayerController : EditorAgentController
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}