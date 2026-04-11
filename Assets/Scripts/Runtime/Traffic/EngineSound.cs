using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColbyO.Untitled.Traffic
{
    
    public class ProceduralAudio : MonoBehaviour
    {
    }

    [RequireComponent(typeof(AudioSource))]
    public class EngineSound : MonoBehaviour
    {
        private float _sampleRate;

        [System.Serializable]
        class EngineSoundWave
        {
            public float volume;
            public float overtone;
            public float offset;
            public float dutyScale;
            public float baseDuty;
            public float throttleDutyScale;
        }
        
        [SerializeField] float _freqScale;
        [SerializeField] float _baseDuty;
        [SerializeField] float _throttleDuty;
        [SerializeField] EngineSoundWave[] _waves;

        [System.Serializable]
        class JsSettings
        {
            public float sampleRate;
            public float freqScale;
            public float baseDuty;
            public float throttleDuty;
            public EngineSoundWave[] waves;
        }

        public bool engineOn = true;
        
        private float _rpm = 0;
        private float _throttle;
        private float[] _phases;
        [SerializeField] private float _rpmLerpSpeed = 10;
        
        private readonly ConcurrentQueue<float> audioBuffer = new();
        private AudioSource _audioSource;

#if PLATFORM_WEBGL && !UNITY_EDITOR
        private void StartSoundEngine() 
        {
            JsSettings jss = new JsSettings()
            {
                sampleRate = _sampleRate,
                freqScale = _freqScale,
                baseDuty = _baseDuty,
                throttleDuty = _throttleDuty,
                waves = _waves,
            };
            JsEngineSoundInit(JsonUtility.ToJson(jss), _sampleRate);
        }

        IEnumerator InitSoundEngineJS()
        {
            yield return new WaitUntil(() => Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed);
            StartSoundEngine();
        }
#endif
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _sampleRate = AudioSettings.outputSampleRate;
            _phases = new float[_waves.Length];
            for (int i = 0; i < _phases.Length; i++) _phases[i] = 0;

#if PLATFORM_WEBGL && !UNITY_EDITOR
            StartSoundEngine();
            StartCoroutine(InitSoundEngineJS());
#endif
        }

        public void SetRpmAndThrottle(float rpm, float throttle)
        {
            _rpm = Mathf.Lerp(_rpm, rpm, _rpmLerpSpeed * Time.deltaTime);
            _throttle = throttle;
#if PLATFORM_WEBGL && !UNITY_EDITOR
            JsEngineSoundSetRpmAndThrottle(_rpm, _throttle);
#endif
        }


#if PLATFORM_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern IntPtr JsEngineSoundInit(string settings, float sampleRate);
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern IntPtr JsEngineSoundSetRpmAndThrottle(float rpm, float throttle);
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern IntPtr JsEngineSoundSetVolume(float volume);

        private void FixedUpdate()
        {
            JsEngineSoundSetVolume(_audioSource.volume);
        }
        
#else
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!engineOn) return;

            // Iterate through the buffer, jumping by the number of channels
            for (int i = 0; i < data.Length; i += channels)
            {
                // 1. Calculate the engine sound ONCE for this moment in time
                float v = Process();

                // 2. Assign that same value to every channel (Left, Right, etc.)
                for (int j = 0; j < channels; j++)
                {
                    data[i + j] = v;
                }
            }
        }

        float Process()
        {
            float baseFreq = _rpm * _freqScale;
            float sub = 0;
            float totalVolume = 0;

            for (int j = 0; j < _waves.Length; j++)
            {
                EngineSoundWave wave = _waves[j];

                _phases[j] += (baseFreq * wave.overtone) / _sampleRate;

                if (_phases[j] > Mathf.PI * 2) _phases[j] -= Mathf.PI * 2;

                totalVolume += wave.volume;

                float dutyExp = wave.baseDuty + _baseDuty * wave.dutyScale +
                                (_throttle > 0.1f ? 1f : 0f) * _throttleDuty * wave.throttleDutyScale;

                float p = Mathf.Sin(_phases[j] + wave.offset);
                sub += wave.volume * Mathf.Pow(Mathf.Abs(p), Mathf.Floor(dutyExp)) * Mathf.Sign(p);
            }

            return totalVolume > 0 ? (sub / totalVolume) : 0;
        }
#endif

    }
}
