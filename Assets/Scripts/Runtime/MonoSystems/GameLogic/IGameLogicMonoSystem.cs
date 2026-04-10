using PlazmaGames.Core.MonoSystem;

namespace ColbyO.Untitled
{
    public interface IGameLogicMonoSystem : IMonoSystem
    {
        public void TriggerEvent(string eventName);
        public void Trigger(string triggerName);
    }
}