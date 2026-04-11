using ColbyO.Untitled.MonoSystems;
using InteractionSystem;
using InteractionSystem.Actions;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    [System.Serializable]
    public abstract class WithDialogueAction : InteractionAction
    {
        [Header("Dialogue")]
        [SerializeField] private string _dialogueName;
        [SerializeField] private bool _onlyPlayOnce = true;
        [SerializeField] private bool _isPassive = true;
        private bool _hasPlayed;
        public override void Execute(InteractorController interactor)
        {
            if (!_hasPlayed || !_onlyPlayOnce)
            {
                GameManager.GetMonoSystem<IDialogueMonoSystem>().StartDialoguePromise(_dialogueName, passive: _isPassive);
                _hasPlayed = true;
            }
        }
    }
}