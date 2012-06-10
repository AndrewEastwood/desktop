using System;
using System.Collections.Generic;
using System.Text;

namespace components.Components.DataContainer
{
    public class DataContainer
    {
        StorageSource _storages;
        StorageStructure _structures;

        public DataContainer()
        {
            _storages = new StorageSource();
            _structures = new StorageStructure();
        }

        public StorageSource Storages { get { return this._storages; } set { this._storages = value; } }
        public StorageStructure Structures { get { return this._structures; } set { this._structures = value; } }

    }
}
