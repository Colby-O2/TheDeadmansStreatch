using System.Collections.Generic;
using ColbyO.Untitled.Wildlife;
using PlazmaGames.Core.MonoSystem;

namespace ColbyO.Untitled.MonoSystems
{
    public interface IFowlMonoSystem : IMonoSystem
    {
        List<FlockController> GetFlocks();
    }
}