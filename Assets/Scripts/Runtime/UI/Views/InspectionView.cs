using InteractionSystem;
using InteractionSystem.UI;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace ColbyO.Untitled.UI
{
    public class InspectionView : View
    {
        [SerializeField] private InspectionUIController _controller;

        public override void Init()
        {
            _controller.OnShow.AddListener(() => GameManager.GetMonoSystem<IUIMonoSystem>().Show<InspectionView>());
            _controller.OnHide.AddListener(() => GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast());
        }

        public override void Show()
        {
            base.Show();
            VirtualCaster.ShowCursor();
        }

        public override void Hide()
        {
            base.Hide();
            VirtualCaster.HideCursor();
        }
    }
}
