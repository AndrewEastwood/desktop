using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Interfaces;

namespace components.Shared.Defaults
{
    public class DefaultComponent : IComponent
    {
        private string _name;
        private Version _componentVersion;

        public DefaultComponent()
        {
            _componentVersion = new Version("1.0");
            _name = "defaultComponent";
        }
        public DefaultComponent(Version v)
            : this()
        {
            _componentVersion = v;
        }
        public DefaultComponent(string version, string name)
        {
            _componentVersion = new Version(version);
            _name = name;
        }

        public Version Version
        {
            get { return _componentVersion; }
        }

        public string ComponentName
        {
            get { return _name; }
        }

        public string GetComponentInfo
        {
            get { return string.Format("{0} v{1}", this.ComponentName, this.Version.ToString(2)); }
        }
    }
}
