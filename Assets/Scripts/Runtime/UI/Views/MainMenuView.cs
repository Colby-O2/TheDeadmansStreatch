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
            _play.onPointerDown.AddListener(Play);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            UTGameManager.LockMovement = true;
            UTGameManager.ShowCursor();
        }

        private void Play()
        {
            UTGameManager.HideCursor();
            GameManager.GetMonoSystem<IVisualEffectMonoSystem>().FadeOut(3f)
            .Then(_ =>
            {
                _mainMenuCamera.SetActive(false);
                _playerCamera.SetActive(true);

                GameManager.GetMonoSystem<IVisualEffectMonoSystem>().FadeIn(5f);

                GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().TriggerEvent("Act1");
            });
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
