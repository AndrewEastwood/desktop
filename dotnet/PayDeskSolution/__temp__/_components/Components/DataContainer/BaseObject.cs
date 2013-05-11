using System;
using System.Collections.Generic;
using System.Text;

namespace components.Components.DataContainer
{
    public class BaseObject
    {
        string name;

        public BaseObject()
        {
            name = string.Empty;
        }

        public string Name { get { return this.name; } set { this.name = value; } }
    }
}
