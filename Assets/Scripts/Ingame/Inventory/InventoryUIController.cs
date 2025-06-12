using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(InventoryUIModel))]
    public class InventoryUIController : MonoBehaviour
    {
        private InventoryUIModel _model;
        public InventoryUIModel Model { get => _model ??= GetComponent<InventoryUIModel>(); }

        public void ToggleInventoryUI()
        {
            Model.SetVisible(!Model.visible);

            Cursor.lockState = Model.visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Model.visible;
        }
    }
}