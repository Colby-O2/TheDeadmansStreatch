using ColbyO.Untitled.MonoSystems;
using InteractionSystem;
using InteractionSystem.Actions;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    [System.Serializable]
    public class InspectWithDialogueAction : InspectAction
    {
        [Header("Dialogue")]
        [SerializeField] private string _dialogueName;
        [SerializeField] private bool _onlyPlayOnce;
        private bool _hasPlayed;

        public override void Execute(InteractorController interactor)
        {
            base.Execute(interactor);

            if (!_hasPlayed || !_onlyPlayOnce)
            {
                GameManager.GetMonoSystem<IDialogueMonoSystem>().StartDialoguePromise(_dialogueName, passive: true);
                _hasPlayed = true;
            }
        }
    }
}