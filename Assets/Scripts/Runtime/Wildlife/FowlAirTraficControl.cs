using UnityEngine;

namespace ColbyO.Untitled.Wildlife
{
    [RequireComponent(typeof(BoxCollider))]
    public class FowlAirTraficControl : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject gooseModelPrefab;

        [Header("Spawn Settings")]
        public float spawnInterval = 10f;
        public Vector2 flockSizeRange = new Vector2(5, 12);

        [Header("Flight Settings")]
        public float flightSpeed = 15f;
        public float gooseSpacing = 2.0f;

        private BoxCollider _flightArea;

        private void Awake()
        {
            _flightArea = GetComponent<BoxCollider>();
            _flightArea.isTrigger = true;
        }

        private void Start()
        {
            InvokeRepeating(nameof(SpawnNewFlock), 0f, spawnInterval);
        }

        void SpawnNewFlock()
        {
            Bounds bounds = _flightArea.bounds;

            int side = Random.Range(0, 4);
            Vector3 spawnPos = Vector3.zero;
            Vector3 targetPos = Vector3.zero;

            float margin = 2.0f;

            if (side == 0)
            {
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.min.z - margin);
                targetPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.max.z);
            }
            else if (side == 1)
            {
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.max.z + margin);
                targetPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.min.z);
            }
            else if (side == 2)
            {
                spawnPos = new Vector3(bounds.min.x - margin, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
                targetPos = new Vector3(bounds.max.x, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
            }
            else
            {
                spawnPos = new Vector3(bounds.max.x + margin, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
                targetPos = new Vector3(bounds.min.x, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
            }

            GameObject flockObj = new GameObject("FlyingFlock_Group");

            Vector3 travelDir = (targetPos - spawnPos).normalized;
            flockObj.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(travelDir));

            flockObj.transform.parent = transform;
            FlockFlying script = flockObj.AddComponent<FlockFlying>();

            int size = (int)Random.Range(flockSizeRange.x, flockSizeRange.y);

            Bounds destructionBounds = bounds;
            destructionBounds.Expand(margin * 2.1f);

            script.Initialize(gooseModelPrefab, size, gooseSpacing, flightSpeed, destructionBounds);
        }
    }
}
