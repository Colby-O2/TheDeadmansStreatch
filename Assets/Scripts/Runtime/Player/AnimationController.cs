using UnityEngine;

namespace ColbyO.Untitled
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        public void SetWalking(bool state)
        {
            _animator.SetBool("IsWalking", state);
        }

        public void SetSprinting(bool state)
        {
            _animator.SetBool("IsSprinting", state);
        }
    }
}
