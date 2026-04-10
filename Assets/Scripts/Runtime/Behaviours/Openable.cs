using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    public class Openable : MonoBehaviour
    {
        [Header("Translation")]
        [SerializeField] private bool _canTranslate = true;
        [SerializeField] private Vector3 _endPos;

        [Header("Rotation")]
        [SerializeField] private bool _canRotate;
        [SerializeField] private Vector3 _endRot;

        [Header("Settings")]
        [SerializeField] private float _openTime = 1f;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;

        [SerializeField, ReadOnly] private Vector3 _startPos;
        [SerializeField, ReadOnly] private Vector3 _startRot;

        [SerializeField, ReadOnly] private bool _isOpen = false;
        [SerializeField, InspectorButton("Open")] private bool _open = false;
        [SerializeField, InspectorButton("Close")] private bool _close = false;

        private void Awake()
        {
            _isOpen = false;
            _startPos = transform.localPosition;
            _startRot = transform.localRotation.eulerAngles;
        }

        void OpenStep(float t, Vector3 startRot, Vector3 endRot, Vector3 startPos, Vector3 endPos)
        {
            if (_canRotate)
            {
                Quaternion start = Quaternion.Euler(startRot);
                Quaternion end = Quaternion.Euler(endRot);
                transform.localRotation = Quaternion.Lerp(start, end, t);
            }
            if (_canTranslate)
            {
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            }
        }

        public Promise Open()
        {
            if (_isOpen) return null;
            if (_audioSource) _audioSource.PlayOneShot(_openSound);
            _isOpen = true;
            return GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(this, _openTime, (float t) => OpenStep(t, _startRot, _endRot, _startPos, _endPos));
        }

        public Promise Close(bool overrideAudio = false)
        {
            if (!_isOpen) return null;
            if (!overrideAudio && _audioSource) _audioSource.PlayOneShot(_closeSound);
            _isOpen = false;
            return GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(this, _openTime, (float t) => OpenStep(t, _endRot, _startRot, _endPos, _startPos));
        }
    }
}
