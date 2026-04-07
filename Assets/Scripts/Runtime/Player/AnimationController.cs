using UnityEngine;

namespace ColbyO.Untitled.Player
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private void OnEnable()
        {
            UTGameManager.PlayerAnimationController = this;
        }

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
