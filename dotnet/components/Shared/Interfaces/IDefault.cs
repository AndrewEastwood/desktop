using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Interfaces
{
    /// <summary>
    /// Defulat plugin interface
    /// </summary>
    public interface IDefault : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        object Execute(string param);

        // ... under developing
    }
}
