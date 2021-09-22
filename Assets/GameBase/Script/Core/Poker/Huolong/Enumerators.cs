
namespace GameBase.Core.Poker.Huolong
{
    public enum GameState
    {
        Idle,
        GameRunning,
        GamePaused,
    }
    public enum MatchState
    {
        Idle,
        Starting,
        GivingHandCards,
        GivingLastCards,
        Rounding,
    }

    public enum GameNoticeEvent
    {
        StartGame,  // 游戏开始, 需要返回 StartGame_Ready 后才会开启首局
        StartMatch, // 游戏开始, 需要返回 StartMatch_Ready 后才会开始发牌
        GiveOneCard,    // 得到一张发牌, 不需要返回
        GiveAllCards,   // 得到批量的发牌, 不需要返回
        PlayerShowingStar,  // 有人亮王牌通知, 此时还未亮王牌之后的定主牌, 不需要返回
        PlayerShowedStar,   // 亮王牌定主通知, 不需要返回
        MatchAborted_NobodyShowed,  // 对局重置: 无人亮王牌, 需要返回 matchAborted_nobodyShown_confirm 确认后开启后续步骤 (重开首局)
        GameAborted_PlayerLeft,     // 游戏中止: 玩家离开, 不需要返回
        GainLastCards,  // 发底牌通知, 不需要返回, 但需要后续发送埋底/摘星操作
        PainLastCards,  // 埋底结果通知, 需要返回 PainLastCards_Confirm 确认后才能进入出牌操作
        AskForThrow,    // 要求玩家出牌, 不需要返回, 但需要玩家进行出牌操作
        PlayerThrew,    // 玩家出牌情况通知, 不需要返回    
        RoundReport,    // 一回合出牌结束, 通报本回合结果, 要求返回 RoundConfirm 之后才会进行下一回合
        MatchReport,    // 一局结束, 通报本局结果, 要求返回 MatchConfirm 才会开始新一局
        GameReport,     // 游戏结束, 不需要返回
        PlayerInfoChanged,  // 玩家信息变更时发送, 不需要返回
    }

    public enum GameNoticeResponse
    {
        StartGame_Ready,
        StartMatch_Ready,
        MatchAborted_NobodyShowed_Confirm,
        PainLastCards_Confirm,
        Round_Confirm,
        Match_Confirm,
    }

    public enum GameOperationEvent
    {
        ShowStar,   // 首局亮王牌操作
        LastCardsThrow,  // 埋底牌或摘星操作, 发完牌后每个玩家都必须发送, 非庄家玩家不摘星的需要发送无牌操作
        CardsThrew,  // 出牌
    }

    public enum GameOperationResponse
    {
        Success,

        CannotPushEvent,            // 此时不能受理任何操作事件

        ShowStar_CannotShow,         // 此时不能亮王
        ShowStar_CardsNotEnough,     // 你所要亮的大王牌数量不达要求
        ShowStar_CardsYouDonnotHave, // 你没有你所声称的那么多大王牌

        LastCards_CannotThrow,      // 此时不能埋底
        LastCards_Repeated,         // 埋底牌数组里出现重复牌
        LastCards_NumberWrong,      // 庄家埋底数量错误
        LastCards_TypeError,        // 非庄家玩家不能埋除大王以外的牌
        LastCards_YouDonnotHave,    // 你没有你所声称要埋底的牌

        CardsThrew_Unavailable,     // 不是出牌时机
        CardsThrew_Repeated,        // 打出的牌数组里出现重复牌
        CardsThrew_TypeWrong,       // 首家出牌牌型不符合要求（多张牌不同）
        CardsThrew_NumberWrong,     // 出牌数量不符合要求
        CardsThrew_ColorWrong,      // 出牌花色不符合要求
        CardsThrew_YouDonnotHave,   // 你没有你所声称要打出的牌

        UnknownOperation,           // 未知的玩家操作命令
    }

    public enum MainColorGetWay
    {
        AllRandom,
        FirstMatchShowStar,
        FirstMatchShowMain,
        EveryMatchShowStar,
        EveryMatchShowMain,
        EveryMatchRandom,
        RandomMainPlayerWithColorSpade,
        RandomMainPlayerWithColorHeart,
        RandomMainPlayerWithColorCube,
        RandomMainPlayerWithColorDiamond,
    }
}