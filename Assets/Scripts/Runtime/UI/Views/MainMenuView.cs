using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace ColbyO.Untitled.UI
{
    public class MainMenuView : View
    {
        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        [SerializeField] private GameObject _mainMenuCamera;
        [SerializeField] private GameObject _playerCamera;

        public override void Init()
        {
            if (_playerCamera && _playerCamera.activeSelf) _playerCamera.SetActive(false);

            _play.onPointerDown.AddListener(Play);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            UTGameManager.ShowCursor();
        }

        private void Play()
        {
            if (_mainMenuCamera && _mainMenuCamera.activeSelf) _mainMenuCamera.SetActive(false);
            if (_playerCamera && !_playerCamera.activeSelf) _playerCamera.SetActive(true);
            UTGameManager.HideCursor();
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
        }

        private void Settings()
        {
            throw new System.NotImplementedException();
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
