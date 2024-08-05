using UnityEngine;

namespace Game.Environment
{
    public class SecurityCamera : MonoBehaviour
    {
        private Camera _camera;
        public string cameraName;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _camera.enabled = false;
        }

        public void ToggleCamera(bool state)
        {
            if (InformantBase.Instance.activeCamera != null && InformantBase.Instance.activeCamera != this)
            {
                InformantBase.Instance.activeCamera.ToggleCamera(false);
            }
            InformantBase.Instance.activeCamera = state ? this : null;
            _camera.targetTexture = state ? InformantBase.Instance.cameraRenderTexture : null;
            _camera.enabled = state;
        }
    }
}
