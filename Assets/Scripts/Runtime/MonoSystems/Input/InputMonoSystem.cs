using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ColbyO.Untitled.MonoSystems
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputMonoSystem : MonoBehaviour, IInputMonoSystem
    {
        [SerializeField] private PlayerInput _input;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;

        public Vector2 RawMovement { get; private set; }
        public Vector2 RawLook { get; private set; }

        public UnityEvent OnShift { get; private set; }

        private void Awake()
        {
            if (!_input) _input = GetComponent<PlayerInput>();

            OnShift = new UnityEvent();

            _moveAction = _input.actions["Move"];
            _lookAction = _input.actions["Look"];
            _sprintAction = _input.actions["Sprint"];
        }

        private void OnEnable()
        {
            _moveAction.performed   += HandleMoveAction;
            _lookAction.performed   += HandleLookAction;
            _sprintAction.performed += HandleSprintAction;
            _sprintAction.canceled  += HandleSprintAction;
        }

        private void OnDisable()
        {
            _moveAction.performed   -= HandleMoveAction;
            _lookAction.performed   -= HandleLookAction;
            _sprintAction.performed -= HandleSprintAction;
            _sprintAction.canceled  -= HandleSprintAction;
        }

        private void HandleMoveAction(InputAction.CallbackContext e)
        {
            RawMovement = e.ReadValue<Vector2>();
        }

        private void HandleLookAction(InputAction.CallbackContext e)
        {
            RawLook = e.ReadValue<Vector2>();
        }

        private void HandleSprintAction(InputAction.CallbackContext e)
        {
            OnShift?.Invoke();
        }


        public void EnableMovement()
        {
            _moveAction.Enable();
            _lookAction.Enable();
        }

        public void DisableMovement()
        {
            RawMovement = Vector2.zero;
            RawLook = Vector2.zero;

            _moveAction.Disable();
            _lookAction.Disable();
        }
    }
}