using UnityEngine;

using ColbyO.Untitled.Player;

namespace ColbyO.Untitled
{
    public class SnowFallController : MonoBehaviour
    {
        [SerializeField] private MovementController _playerController;

        private ParticleSystem _system;
        private ParticleSystem.VelocityOverLifetimeModule _velLifetime;

        private float _vlXMin;
        private float _vlXMax;
        private float _vlZMin;
        private float _vlZMax;

        private void Awake()
        {
            _system = GetComponent<ParticleSystem>();
            _velLifetime = _system.velocityOverLifetime;
            _velLifetime.enabled = true;
            _vlXMin = _velLifetime.x.constantMin;
            _vlXMax = _velLifetime.x.constantMax;
            _vlZMin = _velLifetime.z.constantMin;
            _vlZMax = _velLifetime.z.constantMax;
        }

        private void Update()
        {
            Vector3 d = _playerController.Velocity;
            _velLifetime.x = new ParticleSystem.MinMaxCurve(_vlXMin - d.x, _vlXMax - d.x);
            _velLifetime.z = new ParticleSystem.MinMaxCurve(_vlZMin - d.z, _vlZMax - d.z);
        }
    }
}
