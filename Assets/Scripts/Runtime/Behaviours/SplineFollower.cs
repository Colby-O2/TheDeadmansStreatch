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
        public float _speed = 10f;
        public float _heightOffset = 0f;
        public bool _allowRoate = true;
        private SplineContainer _targetSpline;
        private float _distanceTraveled = 0f;
        private int _splineIndex;
        private float _splineLength;
        private float _endDist;

        private Promise _promise;

        private List<KeyFrame> _keyframes = new List<KeyFrame>();

        public float HeightOffset { get => _heightOffset; set => _heightOffset = value;}
        public bool AllowRotate { get => _allowRoate; set => _allowRoate = value; }

        public Promise Initialize(SplineContainer spline, int index, float moveSpeed, float startDst = 0f, float endDist = 1.0f)
        {
            _targetSpline = spline;
            _speed = moveSpeed;
            _splineIndex = index;

            _splineLength = _targetSpline.Splines[index].GetLength();

            _distanceTraveled = startDst * _splineLength;
            _endDist = endDist;

            Promise.CreateExisting(ref _promise);

            return _promise;
        }

        public Promise WaitFor(float t = 0.5f)
        {
            KeyFrame key = new KeyFrame();
            key.t = t;
            key.promise = new Promise();

            _keyframes.Add(key);

            return key.promise;
        }

        private void Update()
        {
            if (_targetSpline == null) return;

            _distanceTraveled += _speed * Time.deltaTime;
            float t = _distanceTraveled / _splineLength;

            _targetSpline.Evaluate(_splineIndex, t, out float3 position, out float3 tangent, out float3 upVector);

            transform.position = position + upVector * _heightOffset;

            if (!tangent.Equals(float3.zero))
            {
                if (AllowRotate) transform.rotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
            }

            foreach (KeyFrame key in _keyframes)
            {
                if (key.promise != null && t >= key.t)
                {
                    Promise.ResolveExisting(ref key.promise);
                }
            }

            if (t >= _endDist)
            {
                Promise.ResolveExisting(ref _promise);
                _targetSpline = null;

                foreach (KeyFrame key in _keyframes)
                {
                    if (key.promise != null)
                    {
                        Promise.ResolveExisting(ref key.promise);
                    }
                }
                _keyframes.Clear();

                //Destroy(gameObject);
            }
        }

        private class KeyFrame
        {
            public float t;
            public Promise promise;
        }
    }
}