
namespace GameBase.Common.Core
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

        public StorageManager(PlatformInterface.IQuickStorage iqs)
        {
            this.iqs = iqs;
        }


        public T GetGameSetting<T>(GameType type, int subType)
        {
            return LoadItem<T>(key_gameSetting + type + subType);
        }

        public void SetGameSetting<T>(GameType type, int subType, T value)
        {
            SaveItem(key_gameSetting + type + subType, value);
        }

        public void ClearAllStorage()
        {
            iqs.Clear();
        }

        private void SaveItem<T>(string key, T value)
        {
            iqs.SetItem(key, value);
        }

        private T LoadItem<T>(string key)
        {
            return iqs.GetItem<T>(key);
        }

        private readonly PlatformInterface.IQuickStorage iqs;
    }
}
