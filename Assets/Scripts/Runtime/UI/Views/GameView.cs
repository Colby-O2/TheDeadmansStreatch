using PlazmaGames.Core;
using PlazmaGames.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColbyO.Untitled.UI
{
    public class GameView : View
    {
        [SerializeField] private InputAction _pauseAction;

        public InputAction Action { get => _pauseAction; }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
        }

        public override void Init()
        {
            _pauseAction.performed -= HandlePause;
            _pauseAction.performed += HandlePause;
            _pauseAction.Enable();
        }

        private void HandlePause(InputAction.CallbackContext e)
        {
            if (!UTGameManager.HasStarted || GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<InspectionView>()) return;

            if (UTGameManager.IsPaused)
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<PausedView>()) GameManager.GetMonoSystem<IUIMonoSystem>().GetView<PausedView>().Resume();
                else GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<PausedView>();
            }
        }
    }
}