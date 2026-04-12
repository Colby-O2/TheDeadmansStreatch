using System.Collections.Generic;
using ColbyO.Untitled.Wildlife;
using PlazmaGames.Core.MonoSystem;

namespace ColbyO.Untitled.MonoSystems
{
    public interface IFowlMonoSystem : IMonoSystem
    {
        public List<FlockController> GetFlocks();
        public void ForceAllToFlyOff();
    }
}