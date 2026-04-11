using InteractionSystem;
using InteractionSystem.Actions;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    [System.Serializable]
    public class TakeAction : WithDialogueAction
    {
        public override string ActionName => "Take";

        [SerializeField] private string _itemTag;

        public override void Execute(InteractorController interactor)
        {
            base.Execute(interactor);

            GameManager.GetMonoSystem<IInventoryMonoSystem>().GiveItem(_itemTag);
            _owner.CanInteract = false;
            _owner.gameObject.SetActive(false);
        }
    }
}