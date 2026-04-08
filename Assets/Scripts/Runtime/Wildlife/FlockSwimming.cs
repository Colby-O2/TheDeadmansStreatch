using InteractionSystem.Helpers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace ColbyO.Untitled.Wildlife
{
    public class FlockSwimming : MonoBehaviour
    {
        [Header("Group Settings")]
        [SerializeField] private GameObject _fowlPrefab;
        [SerializeField] private int _flockSize = 8;
        [SerializeField] private float _flockSpread = 2.0f;
        [SerializeField] private float _moveSpeed = 2.5f;
        [SerializeField] private float _turnSpeed = 5.0f;

        [Header("Waypoint travel settings")]
        [SerializeField] private Vector2 _waitAtWaypointTime = new Vector2(10.0f, 25.0f);

        [Header("Idle Settings")]
        [SerializeField] private float _idleRadius = 3.0f;
        [SerializeField] private Vector2 _idleWait = new Vector2(2.0f, 7.0f);

        [Header("Patrol Points")]
        [SerializeField] private List<Transform> _waypoints;

        private readonly List<GooseMember> _flock = new List<GooseMember>();
        private Vector3 _groupDestination;
        private bool _isMoving = false;

        private float _waitTime;
        private float _elapsedTimeSinceMove;

        private void Awake()
        {
            if (_waypoints.Count == 0 || _fowlPrefab == null) return;
            SpawnFlock();
        }

        private void Start()
        {
            _groupDestination = _waypoints[Random.Range(0, _waypoints.Count)].position.SetY(transform.position.y);
            _isMoving = true;
        }

        private void Update()
        {
            if (_waypoints.Count == 0 || _fowlPrefab == null) return;

            if (_isMoving && Vector3.Distance(transform.position, _groupDestination) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _groupDestination, _moveSpeed * Time.deltaTime);
                UpdateIndividualGeese();
            }
            else if (_isMoving)
            {
                _isMoving = false;
                foreach (GooseMember goose in _flock) PickNewIdleSpot(goose);

                _waitTime = Random.Range(_waitAtWaypointTime.x, _waitAtWaypointTime.y);
                _elapsedTimeSinceMove = 0.0f;
            }
            else if (_elapsedTimeSinceMove < _waitTime)
            {
                UpdateIndividualGeese();
                _elapsedTimeSinceMove += Time.deltaTime;
            }
            else
            {
                foreach (GooseMember goose in _flock)
                {
                    goose.groupOffset = goose.transform.localPosition;
                }

                _groupDestination = _waypoints[Random.Range(0, _waypoints.Count)].position.SetY(transform.position.y);
                _isMoving = true;
            }
        }

        private void SpawnFlock()
        {
            for (int i = 0; i < _flockSize; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * _flockSpread;
                Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);

                GameObject go = Instantiate(_fowlPrefab, transform.position + offset, Quaternion.identity, this.transform);

                _flock.Add(new GooseMember
                {
                    transform = go.transform,
                    groupOffset = offset,
                    speedMultiplier = Random.Range(0.8f, 1.2f),
                    phaseShift = Random.Range(0f, Mathf.PI * 2),
                    nextIdleChangeTime = 0
                });
            }
        }

        private void UpdateIndividualGeese()
        {
            for (int i = 0; i < _flock.Count; i++)
            {
                GooseMember goose = _flock[i];
                Vector3 targetWorldPos;

                if (_isMoving)
                {
                    float bobble = Mathf.Sin(Time.time + goose.phaseShift) * 0.1f;
                    targetWorldPos = transform.position + goose.groupOffset + (transform.forward * bobble);
                }
                else
                {
                    if (Time.time >= goose.nextIdleChangeTime) PickNewIdleSpot(goose);
                    targetWorldPos = transform.position + goose.idleLocalTarget;
                }

                float finalSpeed = _isMoving ? _moveSpeed * goose.speedMultiplier : _moveSpeed * 0.5f;
                goose.transform.position = Vector3.MoveTowards(goose.transform.position, targetWorldPos, finalSpeed * Time.deltaTime);

                Vector3 lookDir = _isMoving ? (_groupDestination - goose.transform.position) : (targetWorldPos - goose.transform.position);

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    lookDir.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir.normalized);
                    goose.transform.rotation = Quaternion.Slerp(goose.transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
                }
            }
        }

        private void PickNewIdleSpot(GooseMember goose)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _idleRadius;
            goose.idleLocalTarget = new Vector3(randomCircle.x, 0, randomCircle.y);
            goose.nextIdleChangeTime = Time.time + Random.Range(_idleWait.x, _idleWait.y);
        }

        private class GooseMember
        {
            public Transform transform;
            public float speedMultiplier;
            public Vector3 groupOffset;
            public float phaseShift;

            public Vector3 idleLocalTarget;
            public float nextIdleChangeTime;
        }
    }
}
