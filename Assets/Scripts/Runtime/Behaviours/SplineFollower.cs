using InteractionSystem.Helpers;
using PlazmaGames.Core;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace ColbyO.Untitled
{
    public class SplineFollower : MonoBehaviour
    {
        [SerializeField] private float _duration = 5f;
        public float _heightOffset = 0f;
        public bool _allowRoate = true;

        private SplineContainer _targetSpline;
        private float _progress = 0f;
        private int _splineIndex;
        private float _endDist;

        private Promise _promise;
        private List<KeyFrame> _keyframes = new List<KeyFrame>();

        public int SplineIndex => _splineIndex;
        public float HeightOffset { get => _heightOffset; set => _heightOffset = value; }
        public bool AllowRotate { get => _allowRoate; set => _allowRoate = value; }

        public Promise Initialize(SplineContainer spline, int index, float moveDuration, float startT = 0f, float endT = 1.0f)
        {
            _targetSpline = spline;
            _duration = moveDuration;
            _splineIndex = index;

            _progress = startT;
            _endDist = endT;

            Promise.CreateExisting(ref _promise);
            return _promise;
        }

        public Promise WaitFor(float t = 0.5f)
        {
            KeyFrame key = new KeyFrame { t = t, promise = new Promise() };
            _keyframes.Add(key);
            return key.promise;
        }

        private void Update()
        {
            if (_targetSpline == null || _duration <= 0) return;

            _progress += Time.deltaTime / _duration;

            float evalT = math.clamp(_progress, 0f, 1f);

            _targetSpline.Evaluate(_splineIndex, evalT, out float3 position, out float3 tangent, out float3 upVector);

            transform.position = position + upVector * _heightOffset;

            if (_allowRoate && !tangent.Equals(float3.zero))
            {
                transform.rotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
            }

            for (int i = _keyframes.Count - 1; i >= 0; i--)
            {
                if (_progress >= _keyframes[i].t)
                {
                    Promise.ResolveExisting(ref _keyframes[i].promise);
                    _keyframes.RemoveAt(i);
                }
            }

            if (_progress >= _endDist)
            {
                CompleteJourney();
            }
        }

        private void CompleteJourney()
        {
            Promise.ResolveExisting(ref _promise);
            _targetSpline = null;

            foreach (KeyFrame key in _keyframes)
            {
                if (key.promise != null) Promise.ResolveExisting(ref key.promise);
            }
            _keyframes.Clear();
        }

        private class KeyFrame
        {
            public float t;
            public Promise promise;
        }
    }
}