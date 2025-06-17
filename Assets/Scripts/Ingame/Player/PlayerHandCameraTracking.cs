using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public class PlayerHandCameraTracking : MonoBehaviour
    {
        private Camera mainCamera;

        protected void Awake()
        {
            mainCamera = Camera.main;
        }

        protected void Update()
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = mainCamera.transform.eulerAngles.x;
            transform.eulerAngles = eulerAngles;
        }
    }
}