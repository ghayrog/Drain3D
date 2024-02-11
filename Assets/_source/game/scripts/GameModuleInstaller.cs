using DI;
using System.Reflection;
using System.Collections.Generic;

namespace Game
{
    public abstract class GameModuleInstaller : DependencyInstaller,
        IGameListenerProvider
    {
        public virtual IEnumerable<IGameListener> ProvideListeners()
        {
            var fields = ReflectionTools.GetFields(this);

            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(ListenerAttribute)) &&
                    field.GetValue(this) is IGameListener gameListener)
                {
                    yield return gameListener;
                }
            }
        }
    }
}
