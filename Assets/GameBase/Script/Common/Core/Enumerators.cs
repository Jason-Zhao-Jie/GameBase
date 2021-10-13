
namespace GameBase.Common.Core
{
    public enum SystemEventType : int
    {
        OnMyInfoChanged,
        OnPlayerInfoChanged,
        OnSystemSettingChanged,
        OnPlayerEnterGame,
        OnPlayerExitGame,
        OnPlayerOnline,
        OnPlayerOffline,
    }

    public enum Gender
    {
        Unknown,
        Male,
        Female,
        Other
    }

    public enum SourceType
    {
        Unknown,
        LocalId,
        LocalUrl,
        RemoteUrl,
    }

    public enum PlayerType
    {
        HostPlayer,
        HostAI,
        Network,
        Others,
    }

    public enum GameType
    {
        Unknown,
        Poker,
        Chess,
        ChineseChess,
        Mahjong,
    }

}