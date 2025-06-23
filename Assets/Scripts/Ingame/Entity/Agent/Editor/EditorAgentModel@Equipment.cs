using Corelib.SUI;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    public partial class EditorAgentModel : EditorEntityModel
    {
        private class EquipmentGUI : InnerGUI<AgentModel>
        {
            public override void OnInspectorGUI()
            {

            }
        }
    }
}