using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Components.ArgumentParser
{
    public class Com_ArgumentParser
    {
        public static Hashtable Arguments { get; set; }

        static Com_ArgumentParser()
        {
            Arguments = new Hashtable();
        }

        public Com_ArgumentParser()
        {

        }

        public Com_ArgumentParser(string argumentString)
            :this()
        {
            TransformArguments(Arguments, argumentString.Split(' '));
        }

        public Com_ArgumentParser(string[] argumentsArray)
            :this()
        {
            TransformArguments(Arguments, argumentsArray);
        }

        public static void TransformArguments(Hashtable argContainer, string[] argumentsArray)
        {
            int idx=0;
            string prevKey = string.Empty;
            foreach (string argItem in argumentsArray)
            {
                if (argItem.Contains("="))
                {
                    string[] keyValue = argItem.Split('=');
                    argContainer[keyValue[0]] = keyValue[1];
                }
                else
                {
                    if (idx++ % 2 == 0)
                    {
                        prevKey = argItem;
                        argContainer[argItem] = string.Empty;
                    }
                    else
                        argContainer[prevKey] = argItem;
                }
            }
        }

        public static void TransformArguments(string argumentString)
        {
            TransformArguments(Arguments, argumentString.Split(' '));
        }

        public static void TransformArguments(string[] argumentsArray)
        {
            TransformArguments(Arguments, argumentsArray);
        }

    }
}
