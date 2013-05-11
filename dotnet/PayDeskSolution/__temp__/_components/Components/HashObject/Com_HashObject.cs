using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Components.HashObject
{
    public class Com_HashObject : Hashtable
    {
        public Com_HashObject this[object key]
        {
            get
            {
                if (!base.ContainsKey(key)) 
                    base.Add(key, new Com_HashObject());
                return (Com_HashObject)base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public void SetValue(object key, object value)
        {
            base[key] = value;
        }

        public object GetValue(object key)
        {
            return base[key];
        }

        public T GetValue<T>(object key)
        {
            return Lib.TypeConv.ConvertTo<T>(base[key]);
        }

        public Hashtable GetHashtable()
        {
            return GetHashtable(this);
        }

        public Hashtable GetHashtable(Com_HashObject data)
        {
            Hashtable result = new Hashtable();
            foreach (DictionaryEntry de in data)
            {
                if (de.Value is Com_HashObject)
                    result[de.Key] = GetHashtable((Com_HashObject)de.Value);
                else
                    result[de.Key] = de.Value;
            }
            return result;
        }

        public object[] KeysToArray()
        {
            List<object> keyArr = new List<object>();

            foreach (DictionaryEntry obj in this)
                keyArr.Add(obj.Key);

            return keyArr.ToArray();
        }

        public object[] ValuesToArray()
        {
            List<object> valArr = new List<object>();

            foreach (DictionaryEntry obj in this)
                valArr.Add(obj.Value);

            return valArr.ToArray();
        }
    }
}
