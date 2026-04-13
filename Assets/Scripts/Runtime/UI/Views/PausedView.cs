using ColbyO.Untitled.UI;
using InteractionSystem.Controls;
using InteractionSystem.UI;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ColbyO.Untitled
{
    public class PausedView : View
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private GameObject _backdrop;

        [SerializeField] private EventButton _resume;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        private void Update()
        {
            HandleCursor();
        }

        public override void Init()
        {
            _resume.onPointerDown.AddListener(Resume);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            _backdrop.SetActive(true);
            UTGameManager.PlayerInteractiorController.Controls.InspectionClickAction.performed += OnClick;
            UTGameManager.IsPaused = true;
            VirtualCaster.ShowCursor();
        }

        public override void Hide()
        {
            base.Hide();
            UTGameManager.PlayerInteractiorController.Controls.InspectionClickAction.performed -= OnClick;
        }

        public void OnClick(InputAction.CallbackContext ctx)
        {
            ClickUI();
        }

        private void HandleCursor()
        {
            float sensitivity = InputDeviceHandler.IsCurrentGamepad
                ? UTGameManager.PlayerInteractiorController.Controls.ControllerMouseSensitivity
                : UTGameManager.PlayerInteractiorController.Controls.KeybaordMouseSensitivity;

            Vector2 delta = UTGameManager.PlayerInteractiorController.Controls.InspectionCursorAction.ReadValue<Vector2>() * sensitivity;

            VirtualCaster.WrapCursorPosition(delta);
        }

        public void ClickUI()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = VirtualCaster.GetVirtualMousePosition(),
                button = PointerEventData.InputButton.Left
            };

            List<RaycastResult> results = new List<RaycastResult>();
            _canvas.GetComponent<GraphicRaycaster>().Raycast(eventData, results);

            if (results.Count == 0) return;

            GameObject go = results[0].gameObject;

            ExecuteEvents.Execute(go, eventData, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(go, eventData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(go, eventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(go, eventData, ExecuteEvents.pointerClickHandler);
        }

        public void Resume()
        {
            VirtualCaster.HideCursor();
            UTGameManager.IsPaused = false;
            _backdrop.SetActive(false);
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
