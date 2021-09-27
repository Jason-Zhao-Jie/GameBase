using GameBase.Common.Core;

namespace GameBase.Present
{
    public interface IGameMain
    {
        bool IsDebug { get; }
        SystemSettings SystemSettings { get; set; }
        CharacterInfo LocalUserInfo { get; set; }
        System.ValueType GetGameSetting(GameType type, int subType);
        void SetGameSetting(GameType type, int subType, System.ValueType settings);
        void PlayMusic(string audioKey, bool isUrl = false);
        void PlaySound(string audioKey, bool isUrl = false);
        int Listen(SystemEventType _event, System.Action<object[]> callback);
        bool Unlisten(SystemEventType _event, int id);
        bool Notify<T>(SystemEventType _event, T[] data);
    }

    public static class GameMain
    {
        public static IGameMain Instance = null;
    }
}
