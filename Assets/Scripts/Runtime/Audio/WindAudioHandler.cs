using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using UnityEngine;

namespace ColbyO.Untitled
{
    public class WindAudioHandler : MonoBehaviour
    {
        [Header("Volume Modulation")]
        [SerializeField] private float _baseVolume = 0.5f;
        [SerializeField] private float _volumeRange = 0.2f;
        [SerializeField] private float _volumeSpeed = 0.5f;

        [Header("Pitch Modulation")]
        [SerializeField] private float _basePitch = 1.0f;
        [SerializeField] private float _pitchRange = 0.05f;
        [SerializeField] private float _pitchSpeed = 0.3f;

        private AudioSource _as;
        private IAudioMonoSystem _audioMonoSystem;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(ModulateAudio());
        }

        private void OnDisable()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        private void Start()
        {
            _as = GetComponent<AudioSource>();
            _as.loop = true;
            _as.Play();

            _audioMonoSystem = GameManager.GetMonoSystem<IAudioMonoSystem>();
        }

        private IEnumerator ModulateAudio()
        {
            float t = 0;
            while (true)
            {
                if (_audioMonoSystem == null)
                {
                    yield return null;
                    continue;
                }

                t += Time.deltaTime;

                float vNoise = Mathf.PerlinNoise(t * _volumeSpeed, 0f);
                float pNoise = Mathf.PerlinNoise(0f, t * _pitchSpeed);

                _as.volume = (_baseVolume + (vNoise - 0.5f) * _volumeRange) * _audioMonoSystem.GetOverallVolume() * _audioMonoSystem.GetSfXVolume();
                _as.pitch = _basePitch + (pNoise - 0.5f) * _pitchRange;

                yield return null;

            }
        }
    }
}
