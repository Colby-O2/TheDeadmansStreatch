using System.Collections.Generic;
using UnityEngine;

namespace ColbyO.Untitled.Wildlife
{
    public class FlockFlying : MonoBehaviour
    {
        private List<AirMember> _members = new List<AirMember>();
        private float _speed;
        private Bounds _limitBounds;

        [Header("Dynamic Movement")]
        [SerializeField] private float _driftAmount = 0.4f;
        [SerializeField] private float _driftSpeed = 1.5f;

        public void Initialize(GameObject prefab, int size, float spacing, float speed, Bounds bounds)
        {
            _speed = speed;
            _limitBounds = bounds;

            float horizontalStagger = spacing * 0.4f;
            float verticalStagger = 0.5f;

            for (int i = 0; i < size; i++)
            {
                int side = (i % 2 == 0) ? 1 : -1;
                int row = (i + 1) / 2;
                if (i == 0) row = 0;

                Vector3 vOffset = new Vector3(side * row * spacing, 0, -row * spacing);

                if (i > 0)
                {
                    vOffset.x += Random.Range(-horizontalStagger, horizontalStagger);
                    vOffset.y += Random.Range(-verticalStagger, verticalStagger);
                    vOffset.z += Random.Range(-horizontalStagger, horizontalStagger);
                }

                GameObject go = Instantiate(prefab, transform.TransformPoint(vOffset), transform.rotation, this.transform);

                _members.Add(new AirMember
                {
                    transform = go.transform,
                    vOffset = vOffset,
                    noiseOffset = new Vector3(Random.value * 100, Random.value * 100, Random.value * 100)
                });
            }
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);

            UpdateIndividualBirds();

            if (!_limitBounds.Contains(transform.position))
            {
                Destroy(gameObject);
            }
        }

        private void UpdateIndividualBirds()
        {
            float time = Time.time * _driftSpeed;

            foreach (var bird in _members)
            {
                float x = (Mathf.PerlinNoise(time + bird.noiseOffset.x, 0) - 0.5f) * _driftAmount;
                float y = (Mathf.PerlinNoise(time + bird.noiseOffset.y, 0) - 0.5f) * _driftAmount;
                float z = (Mathf.PerlinNoise(time + bird.noiseOffset.z, 0) - 0.5f) * _driftAmount;

                Vector3 dynamicLocalPos = bird.vOffset + new Vector3(x, y, z);

                bird.transform.localPosition = dynamicLocalPos;
            }
        }

        private class AirMember
        {
            public Transform transform;
            public Vector3 vOffset;
            public Vector3 noiseOffset;
        }
    }
}