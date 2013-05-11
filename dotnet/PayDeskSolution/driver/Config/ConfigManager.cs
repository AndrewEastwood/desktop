using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace driver.Config
{
    public class ConfigManager
    {
        private static ConfigManager instance;
        private Dictionary<string, AppConfig> profiles = new Dictionary<string, AppConfig>();
        private AppConfig commonConfig = new AppConfig();

        public static ConfigManager getInstance()
        {
            if (instance == null)
                instance = new ConfigManager();
            return instance;
        }

        public ConfigManager() { }

        public string getConfigNormalFileName { get { return "Config";  } }

        public string getConfigBackupFileName { get { return "ConfigBackup"; } }

        public string getConfigNormalFilePath { get { return Application.StartupPath + "\\" + this.getConfigNormalFileName + ".cfg"; } }

        public string getConfigBackupFilePath { get { return Application.StartupPath + "\\backup\\" + this.getConfigBackupFileName + ".cfg"; } }

        // will add en empty profile
        public void AddNewProfile()
        {
            AppConfig ac = new AppConfig();
            this.profiles.Add(ac.p_key, ac);
        }
        // will add a specific profile
        public void AddNewProfile(string profileName, string profileKey)
        {
            AppConfig ac = new AppConfig();
            ac.p_name = profileName;
            ac.p_key = profileKey;
            this.profiles.Add(profileKey, ac);
        }

        public bool DeleteProfile(string profileKey)
        {
            bool rez = true;
            try
            {
                if (this.profiles.Count == 0)
                    throw new Exception("Could not remove all profiles. One profile entry has to exist.");
                this.profiles.Remove(profileKey);
            }
            catch (Exception ex)
            {
                Lib.CoreLib.WriteLog(ex, "DeleteProfile(" + profileKey + ")");
                rez = false;
            }
            return rez;
        }

        public bool CloneExisted(string newProfileKey, string existedProfileKey)
        {
            bool rez = true;
            try
            {
                this.profiles.Add(newProfileKey, this.profiles[existedProfileKey]);
            }
            catch (Exception ex)
            {
                Lib.CoreLib.WriteLog(ex, "CloneExisted(" + newProfileKey + "," + existedProfileKey + ")");
                rez = false;
            }
            return rez;
        }

        private bool _saveConfiguration()
        {
            return _saveNormalConfig() && _saveBackupConfig();
        }

        private bool _loadConfiguration()
        {
            bool loadOK = _loadNormalConfig();

            if (loadOK)
            {
                _saveBackupConfig();
                return true;
            }

            loadOK = _loadBackupConfig();
            
            if (!loadOK)
                MessageBox.Show("Помилка завантаження конфігурації. Звяжіться з постачальником програмного забезпечення.", Application.ProductName,
                 MessageBoxButtons.OK, MessageBoxIcon.Error);

            return loadOK;
        }

        private object[] _readConfigFile(string configFilePath)
        {
            BinaryFormatter binF = new BinaryFormatter();
            object[] all = null;

            if (!File.Exists(configFilePath))
                return all;

            using (FileStream stream = new FileStream(configFilePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    all = (object[])binF.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            }
            return all;
        }

        private bool _saveConfigFile(string configFilePath, object[] config)
        {
            bool saveOK = true;
            BinaryFormatter binF = new BinaryFormatter();

            if (!Directory.Exists(Path.GetDirectoryName(configFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));

            using (FileStream stream = new FileStream(configFilePath, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    binF.Serialize(stream, config);
                }
                catch (Exception ex)
                {
                    saveOK = false;
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            }
            return saveOK;
        }

        public static bool SaveConfiguration()
        {
            return instance._saveConfiguration();
        }

        public static bool LoadConfiguration()
        {
            bool loadState = instance._loadConfiguration();
            return loadState;
        }

        private bool _loadNormalConfig()
        {
            bool loadOK = true;
            object[] cfg = _readConfigFile(this.getConfigNormalFilePath);
            try
            {
                if (cfg.Length == 2)
                {
                    this.commonConfig = (AppConfig)cfg[0];
                    this.profiles = (Dictionary<string, AppConfig>)cfg[1];
                }
                else loadOK = false;
            }
            catch (Exception ex)
            {
                loadOK = false;
                // this.commonConfig = new AppConfig();
                // this.profiles = new Dictionary<string, AppConfig>();
                Lib.CoreLib.WriteLog(ex, "LoadData");
            }

            return loadOK;
        }

        private bool _loadBackupConfig()
        {
            bool loadOK = false;
            if (File.Exists(this.getConfigBackupFilePath))
                try
                {
                    File.Copy(this.getConfigBackupFilePath, this.getConfigNormalFilePath, true);
                    object[]  cfg = _readConfigFile(this.getConfigBackupFilePath);
                    if (cfg.Length == 2)
                    {
                        this.commonConfig = (AppConfig)cfg[0];
                        this.profiles = (Dictionary<string, AppConfig>)cfg[1];
                        loadOK = true;
                    }
                }
                catch (Exception ex)
                {
                    Lib.CoreLib.WriteLog(ex, "LoadData");
                }
            return loadOK;
        }

        private bool _saveNormalConfig()
        {
            return _saveConfigFile(this.getConfigNormalFilePath, new object[] { this.commonConfig, this.profiles });
        }

        private bool _saveBackupConfig()
        {
            return _saveConfigFile(this.getConfigBackupFilePath, new object[] { this.commonConfig, this.profiles });
        }

        public AppConfig this[string profile]
        {
            get
            {
                return (AppConfig)this.profiles[profile];
            }
            set
            {
                this.profiles[profile] = value;
            }
        }
        public Dictionary<string, AppConfig> Profiles { get { return this.profiles; } set { this.profiles = value; } }
        public AppConfig[] ProfileConfigurations
        {
            get
            {

                List<AppConfig> items = new List<AppConfig>();
                foreach (KeyValuePair<string, AppConfig> c in this.profiles)
                    items.Add(c.Value);
                return items.ToArray();
            }
        }
        public AppConfig CommonConfiguration { get { return this.commonConfig; } set { this.commonConfig = value; } }
        public static ConfigManager Instance { get { return instance; } }

    }
}
