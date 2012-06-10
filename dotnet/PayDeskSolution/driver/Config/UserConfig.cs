using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using driver.Components;

namespace driver.Config
{
    /// <summary>
    /// User's configuration class
    /// </summary>
    [Serializable]
    public class UserConfig
    {
        // Private fields
        private static string _userID = string.Empty;
        private static string _userLogin = string.Empty;
        private static string _userPassword = string.Empty;
        private static string _userFLogin = string.Empty;
        private static string _userFPassword = string.Empty;
        private static bool _adminState = false;
        private static bool[] _properties = new bool[UserSchema.ITEMS_COUNT];
        private static string _schemaName = string.Empty;

        // Public methods
        /// <summary>
        /// Load user's settings
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadData(string filePath)
        {
            // Loading data
            BinaryFormatter binF = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            object[] data = (object[])binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();

            // Return if data is not loaded
            if (data == null)
                return;

            // Updating data
            _userID = data[0].ToString();
            _userLogin = data[1].ToString();
            _userPassword = data[2].ToString();
            UserFpLogin = data[3] != null ? data[3].ToString() : string.Empty;
            UserFpPassword = data[4] != null ? data[4].ToString() : string.Empty;
            _adminState = (bool)data[5];
            _properties = (bool[])data[6];
            _schemaName = data[7].ToString();
        }
        /// <summary>
        /// Save user's settings
        /// </summary>
        /// <param name="filePath">Path of configuration file</param>
        public static void SaveData(string filePath)
        {
            // Creating data
            object[] data = new object[8];
            data[0] = _userID;
            data[1] = _userLogin;
            data[2] = _userPassword;
            data[3] = _userFLogin;
            data[4] = _userFPassword;
            data[5] = _adminState;
            data[6] = _properties;
            data[7] = _schemaName;

            // Saving data
            BinaryFormatter binF = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            binF.Serialize(stream, data);
            stream.Close();
            stream.Dispose();
        }

        // Properties
        /// <summary>
        /// Get or set user ID
        /// </summary>
        public static string UserID { get { return _userID; } set { _userID = value; } }
        /// <summary>
        /// Get or set user login
        /// </summary>
        public static string UserLogin { get { return _userLogin; } set { _userLogin = value; } }
        /// <summary>
        /// GetOr set user password
        /// </summary>
        public static string UserPassword { get { return _userPassword; } set { _userPassword = value; } }
        /// <summary>
        /// Get or set user login
        /// </summary>
        public static string UserFpLogin { get { return _userFLogin; } set { _userFLogin = value; } }
        /// <summary>
        /// GetOr set user password
        /// </summary>
        public static string UserFpPassword { get { return _userFPassword; } set { _userFPassword = value; } }
        /// <summary>
        /// Get or set administrator's activity
        /// </summary>
        public static bool AdminState { get { return _adminState; } set { _adminState = value; } }
        /// <summary>
        /// Get or set user access properties
        /// </summary>
        public static bool[] Properties { get { return _properties; } set { _properties = value; } }
        /// <summary>
        /// Get or set user definded schema
        /// </summary>
        public static string SchemaName { get { return _schemaName; } set { _schemaName = value; } }
    }
}
