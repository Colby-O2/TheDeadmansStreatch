using UnityEngine;

namespace ColbyO.Untitled
{
    public class CameraRenderLimiter : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _renderInterval = 0.1f;

        private void OnEnable()
        {
            InvokeRepeating(nameof(Render), 0f, _renderInterval + _renderInterval * Random.value);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(Render));
        }

        private void Awake()
        {
            if (!_camera) _camera = GetComponent<Camera>();
            if (_camera) _camera.enabled = false;
        }


        private void Render()
        {
            if (!_camera) return;
            _camera.Render();
        }
    }
}
