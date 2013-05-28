using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Interfaces
{
    /// <summary>
    /// Service plugin interface
    /// </summary>
    public interface IService : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        // ... under developing
    }
}
