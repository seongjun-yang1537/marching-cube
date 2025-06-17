using UnityEngine;
using UnityEngine.EventSystems;

public class CinemachineToggleController : MonoBehaviour
{
    [SerializeField]
    private Behaviour cinemachineComponent;

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     EnableCameraControl();
        // }

        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     DisableCameraControl();
        // }
    }

    private void EnableCameraControl()
    {
        if (cinemachineComponent != null)
        {
            cinemachineComponent.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void DisableCameraControl()
    {
        if (cinemachineComponent != null)
        {
            cinemachineComponent.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}