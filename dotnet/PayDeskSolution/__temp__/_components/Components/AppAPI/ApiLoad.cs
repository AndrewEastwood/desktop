using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace driver.Components.API
{
    public class ApiLoad
    {
        private Hashtable inParam;
        private string[] origArgs;

        public ApiLoad()
        {
            this.inParam = new Hashtable();
            this.origArgs = new string[0];
        }
        public ApiLoad(string[] args) :
            this()
        {
            this.InputParser(args);
            this.origArgs = args;
        }

        public void InputParser(string[] args)
        {
            this.inParam = new System.Collections.Hashtable();
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 > args.Length)
                    this.inParam.Add(args[i], null);
                else
                    this.inParam.Add(args[i], args[i + 1]);
            }
        }

        public object GetValue(string key)
        {
            return this.GetValue(key, new object(), false);
        }

        public object GetValue(string key, object defaultValue)
        {
            return this.GetValue(key, defaultValue, false);
        }

        public object GetValue(string key, object defaultValue, bool acceptNull)
        {
            object value = defaultValue;
            if (this.Parameters.ContainsKey(key) && ((acceptNull && this.Parameters.ContainsKey(key) == null) || !this.Parameters.ContainsKey(key)))
                value = this.Parameters[key];
            return value;
        }

        public Hashtable Parameters
        {
            get { return this.inParam; }
        }
        
        public string[] OrigArguments
        {
            get { return this.origArgs; }
        }
    }
}
