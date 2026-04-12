using UnityEngine;

namespace ColbyO.Untitled
{
    public class WaterCamera : MonoBehaviour
    {
        [SerializeField] private Transform _water;
        [SerializeField] private Transform _camera;
        [SerializeField] private GameObject _waterScreen;

        private void Update()
        {
            if (_camera.position.y <= _water.position.y)
            {
                if (!_waterScreen.activeSelf) _waterScreen.SetActive(true);
            }
            else
            {
                if (_waterScreen.activeSelf) _waterScreen.SetActive(false);
            }
        }
    }
}
