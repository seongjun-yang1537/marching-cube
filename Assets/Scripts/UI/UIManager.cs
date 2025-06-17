using Corelib.Utils;
using Ingame;
using UnityEngine;

namespace UI
{
    public class UIManager : Singleton<UIManager>
    {
        public InventoryUIController inventoryUI;

        public void ToggleInventoryUI() => inventoryUI.ToggleInventoryUI();
    }
}