using System;
using System.Collections.Generic;

namespace DI
{
    public interface IServiceProvider
    {
        IEnumerable<(Type, object)> ProvideServices();
    }
}
