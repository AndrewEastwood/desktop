using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace components.Shared.Interfaces
{
    /// <summary>
    /// UI plugin interface
    /// </summary>
    public interface IAppUI : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        object Execute(params Control[] param);

        // ... under developing
    }
}
