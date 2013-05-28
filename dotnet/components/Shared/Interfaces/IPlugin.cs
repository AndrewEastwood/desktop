using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Interfaces
{
    /// <summary>
    /// Main plugin interface.
    /// Base interface for other interfaces.
    /// </summary>
    public interface IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }
    }
}
