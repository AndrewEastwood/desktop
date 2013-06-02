using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace VirtualKeyboard.Config
{
    public class manager
    {
        private sobj settingObject;

        public manager()
        {
            this.settingObject = new sobj();
        }

        public void Read()
        {
            BinaryFormatter binF = new BinaryFormatter();
            FileStream fs = new FileStream("VirtualKeyboard.cfg", FileMode.OpenOrCreate);
            try
            {
                this.settingObject = (sobj)binF.Deserialize(fs);
            }
            catch
            {
                this.settingObject = new sobj();
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

        }

        public void Save()
        {
            BinaryFormatter binF = new BinaryFormatter();
            FileStream fs = new FileStream("VirtualKeyboard.cfg", FileMode.OpenOrCreate);
            try
            {
                binF.Serialize(fs, this.settingObject);
            }
            catch
            {
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        public sobj Settings { get { return this.settingObject; } set { this.settingObject = value; } }
    }
}
