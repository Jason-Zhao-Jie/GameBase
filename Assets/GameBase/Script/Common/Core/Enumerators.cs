
namespace GameBase.Common.Core
{
    public enum SystemEventType : int
    {
        OnMyInfoChanged,
        OnPlayerInfoChanged,
        OnSystemSettingChanged,
        OnPlayerExitGame,
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
        Poker,
        Chess,
        ChineseChess,
        Mahjong,
    }

}