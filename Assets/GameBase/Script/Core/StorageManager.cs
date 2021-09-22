
namespace GameBase.Core
{
    public class StorageManager
    {
        const string key_systemSetting = "systemSetting";
        const string key_localUserInfo = "local_userInfo";
        const string key_gameSetting = "game_setting_";

        public SystemSettings SystemSettings
        {
            get
            {
                return LoadItem<SystemSettings>(key_systemSetting);
            }
            set
            {
                SaveItem(key_systemSetting, value);
            }
        }

        public CharacterInfo LocalUserInfo
        {
            get
            {
                return LoadItem<CharacterInfo>(key_localUserInfo);
            }
            set
            {
                SaveItem(key_localUserInfo, value);
            }
        }


        public T GetGameSetting<T>(GameType type, int subType)
        {
            return this.LoadItem<T>(key_gameSetting + type + subType);
        }

        public void SetGameSetting<T>(GameType type, int subType, T value)
        {
            this.SaveItem(key_gameSetting + type + subType, value);
        }

        public void ClearAllStorage()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        private void SaveItem<T>(string key, T value)
        {
            UnityEngine.PlayerPrefs.SetString(key, UnityEngine.JsonUtility.ToJson(value));
        }

        private T LoadItem<T>(string key)
        {
            var json = UnityEngine.PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }
    }
}
