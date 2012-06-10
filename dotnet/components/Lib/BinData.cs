using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace components.Lib
{
    public class BinData
    {
        /// <summary>
        /// Perform saving data into file using binary formatter
        /// </summary>
        /// <param name="path">Path to file where data would be saved</param>
        /// <param name="data">Data which would be saved in selected file</param>
        public void SaveData(string path, object data)
        {
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binF.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
            try
            {
                binF.Serialize(stream, data);
            }
            catch { }

            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Perform loading data from selected file
        /// </summary>
        /// <param name="path">Path of binary file which was saved using binary formatter</param>
        /// <returns>Return parsed data from selected file otherwise return null</returns>
        public object LoadData(string path)
        {
            if (!File.Exists(path))
                return null;

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            object _data = new object();

            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binF = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                _data = binF.Deserialize(stream);

            }
            catch { _data = null; }

            stream.Close();
            stream.Dispose();

            return _data;
        }
    }
}
