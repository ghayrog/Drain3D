using System.Collections.Generic;

namespace Game
{
    public interface IGameListenerProvider
    {
        IEnumerable<IGameListener> ProvideListeners();
    }
}
