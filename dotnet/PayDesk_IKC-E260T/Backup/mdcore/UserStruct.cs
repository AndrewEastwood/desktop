using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace mdcore
{
    [Serializable]
    public class UserStruct
    {
        public static string UserID = string.Empty;
        public static string UserLogin = string.Empty;
        public static string UserPassword = string.Empty;
        public static byte UserFLogin = 0;
        public static uint UserFPassword = 0;
        public static bool AdminState = false;
        public static bool[] Properties = new bool[UserSchema.ItemsCount];
        public static string SchemaName = string.Empty;

        public static void LoadData(string filePath)
        {
            BinaryFormatter binF = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            object[] data = (object[])binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();

            if (data == null)
                return;

            //Updating data
            UserID = data[0].ToString();
            UserLogin = data[1].ToString();
            UserPassword = data[2].ToString();
            UserFLogin = (byte)data[3];
            UserFPassword = (uint)data[4];
            AdminState = (bool)data[5];
            Properties = (bool[])data[6];
            SchemaName = data[7].ToString();
        }

        public static void SaveData(string filePath)
        {
            //Creating data
            object[] data = new object[8];
            data[0] = UserID;
            data[1] = UserLogin;
            data[2] = UserPassword;
            data[3] = UserFLogin;
            data[4] = UserFPassword;
            data[5] = AdminState;
            data[6] = Properties;
            data[7] = SchemaName;

            BinaryFormatter binF = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            binF.Serialize(stream, data);
            stream.Close();
            stream.Dispose();
        }
    }
}
