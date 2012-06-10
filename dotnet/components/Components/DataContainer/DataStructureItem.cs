using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Components.DataContainer
{
    public class DataStructureItem: BaseObject
    {
        private Hashtable props;

        /* delegates */

        public delegate Hashtable UpdateFunctionDelegate();
        public delegate void ResetFunctionDelegate();
        public UpdateFunctionDelegate UpdateMethod;
        public ResetFunctionDelegate ResetMethod;

        /* constructors */

        public DataStructureItem()
        {
            props = new Hashtable();
            // init actions
            this.ResetMethod = this.DefaultResetMethod;
            this.UpdateMethod = this.DefaultUpdateMethod;
            
        }

        public DataStructureItem(string name)
            : this()
        {
            base.Name = name;
        }

        public DataStructureItem(Hashtable props)
            : this()
        {
            this.props = props;
        }

        public DataStructureItem(string name, Hashtable props)
            : this(name)
        {
            this.props = props;
        }


        /* accessors */

        public object this[string propertyKey]
        {
            get { return GetPropertyItem(propertyKey); }
            set { SetProperty(propertyKey, value); }
        }

        public object GetPropertyItem(string propertyName)
        {
            return this.props[propertyName];
        }

        public Hashtable GetPropertySection(string propertyName)
        {
            Hashtable ht = new Hashtable();

            try
            {
                ht = (Hashtable)this.props[propertyName];
            }
            catch { }

            return ht;
        }

        public T GetTypedProperty<T>(string propertyName)
        {
            T retVal = default(T);
            try
            {
                retVal = (T)Convert.ChangeType(this.props[propertyName], retVal.GetType());
            }
            catch { }

            return retVal;
        }

        public void SetProperty(string propertyName, object value)
        {
            this.props[propertyName] = value;
        }

        public Hashtable Properties
        {
            get { return this.props; }
            set { this.props = value; }
        }

        /* actions */

        public void Update()
        {
            Hashtable newData = this.UpdateMethod.Invoke();
            foreach (DictionaryEntry de in newData)
            {
                this.Properties[de.Key] = de.Value;
            }
        }

        public void Reset()
        {
            this.ResetMethod.Invoke();
        }

        public void Clear()
        {
            this.Properties.Clear();
        }

        public void DefaultResetMethod()
        {
            foreach (DictionaryEntry de in this.Properties)
                this.Properties[de.Key] = null;
        }

        public Hashtable DefaultUpdateMethod()
        {
            // self reassign
            // nothig to do this
            return this.Properties;
        }
        /*
        public UpdateFunctionDelegate UpdateMethod
        {
            get { return this._updateMethod; }
            set { this._updateMethod = value; }
        }

        public ResetFunctionDelegate ResetMethod
        {
            get { return this._resetMethod; }
            set { this._resetMethod = value; }
        }
        */
    }
}
