using GameBase.Common.Core;

namespace GameBase.Present
{
    public interface IGameMain
    {
        void PlayMusic(string audioKey, bool isUrl = false);
        void PlaySound(string audioKey, bool isUrl = false);
        int Listen(SystemEventType _event, System.Action<object> callback);
        bool Unlisten(SystemEventType _event, int id);
        bool Notify<T>(SystemEventType _event, T data) where T : class;
    }

    public static class GameMain
    {
        public static IGameMain Instance = null;
    }
}
