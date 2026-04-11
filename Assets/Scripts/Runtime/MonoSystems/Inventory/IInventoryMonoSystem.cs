using PlazmaGames.Core.MonoSystem;

namespace ColbyO.Untitled
{
    public interface IInventoryMonoSystem : IMonoSystem
    {
        public void GiveItem(string tag);
        public void TakeItem(string tag);
        public bool HasItem(string tag);
    }
}