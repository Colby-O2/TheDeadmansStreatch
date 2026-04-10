using InteractionSystem;
using InteractionSystem.Actions;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    [System.Serializable]
    public class CarGetOutAction : InteractionAction
    {
        [SerializeField] private Openable _door;

        public override string ActionName => "Leave";

        private Promise _promise;

        public Openable Door { get => _door; }

        public Promise WaitForDoorToOpen()
        {
            return Promise.CreateExisting(ref _promise);
        } 

        public override void Execute(InteractorController interactor)
        {
            IsEnabled = false;

            _door.Open()
            .Then(_ =>
            {
                Promise.ResolveExisting(ref _promise);
            });
        }
    }
}