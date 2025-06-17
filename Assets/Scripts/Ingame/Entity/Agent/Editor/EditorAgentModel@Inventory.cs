using System;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer.Lock;
using Corelib.SUI;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    public partial class EditorAgentModel : InnerEditor<AgentModel>
    {
        private class BagGUI : InnerGUI<AgentModel>
        {
            private Inventory inventory { get => script.inventory; }
            private BagGUIDrawer drawer;

            public override void OnEnable()
            {
                drawer = new(inventory);
            }

            public override void OnInspectorGUI()
            {
                drawer.OnInspectorGUI();
            }
        }
    }
}