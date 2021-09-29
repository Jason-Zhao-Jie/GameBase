
namespace GameBase.Common.Core.Poker.Huolong
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
        GivingHandCards,    // ������
        GivingLastCards,    // ȷ�ϵ�����
        SendingLastCards,   // �����
        Rounding,
    }

    public enum GameNoticeEvent
    {
        StartGame,  // ��Ϸ��ʼ, ��Ҫ���� StartGame_Ready ��ŻῪ���׾�
        StartMatch, // ��Ϸ��ʼ, ��Ҫ���� StartMatch_Ready ��ŻῪʼ����
        GiveOneCard,    // �õ�һ�ŷ���, ����Ҫ����
        GiveAllCards,   // �õ������ķ���, ����Ҫ����
        PlayerShowingStar,  // ����������֪ͨ, ��ʱ��δ������֮��Ķ�����, ����Ҫ����
        PlayerShowedStar,   // �����ƶ���֪ͨ, ����Ҫ����
        MatchAborted_NobodyShowed,  // �Ծ�����: ����������, ��Ҫ���� matchAborted_nobodyShown_confirm ȷ�Ϻ����������� (�ؿ��׾�)
        GainLastCards,  // ������֪ͨ, ����Ҫ����, ����Ҫ�����������/ժ�ǲ���
        PainLastCards,  // ��׽��֪ͨ, ��Ҫ���� PainLastCards_Confirm ȷ�Ϻ���ܽ�����Ʋ���
        PlayerThrew,    // ��ҳ������֪ͨ, ����Ҫ����    
        AskForThrow,    // Ҫ����ҳ���, ����Ҫ����, ����Ҫ��ҽ��г��Ʋ���
        RoundReport,    // һ�غϳ��ƽ���, ͨ�����غϽ��, Ҫ�󷵻� RoundConfirm ֮��Ż������һ�غ�
        MatchReport,    // һ�ֽ���, ͨ�����ֽ��, Ҫ�󷵻� MatchConfirm �ŻῪʼ��һ��
        GameReport,     // ��Ϸ����, ����Ҫ����
        GameAborted_PlayerLeft,     // ��Ϸ��ֹ: ����뿪, ����Ҫ����
        PlayerInfoChanged,  // �����Ϣ���ʱ����, ����Ҫ����
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
        ShowStar,   // �׾������Ʋ���
        LastCardsThrow,  // ����ƻ�ժ�ǲ���, �����ƺ�ÿ����Ҷ����뷢��, ��ׯ����Ҳ�ժ�ǵ���Ҫ�������Ʋ���
        CardsThrew,  // ����
    }

    public enum GameOperationResponse
    {
        Success,

        CannotPushEvent,            // ��ʱ���������κβ����¼�

        ShowStar_CannotShow,         // ��ʱ��������
        ShowStar_CardsNotEnough,     // ����Ҫ���Ĵ�������������Ҫ��

        LastCards_CannotThrow,      // ��ʱ�������
        LastCards_Repeated,         // ���������������ظ���
        LastCards_NumberWrong,      // ׯ�������������
        LastCards_TypeError,        // ��ׯ����Ҳ�����������������
        LastCards_YouDonnotHave,    // ��û����������Ҫ��׵���

        CardsThrew_Unavailable,     // ���ǳ���ʱ��
        CardsThrew_Repeated,        // �����������������ظ���
        CardsThrew_TypeWrong,       // �׼ҳ������Ͳ�����Ҫ�󣨶����Ʋ�ͬ��
        CardsThrew_NumberWrong,     // ��������������Ҫ��
        CardsThrew_ColorWrong,      // ���ƻ�ɫ������Ҫ��
        CardsThrew_YouDonnotHave,   // ��û����������Ҫ�������

        UnknownOperation,           // δ֪����Ҳ�������
    }

    public enum MainColorGetWay
    {
        AllRandom,              // ��ׯ���������ɫ����̶�
        FirstMatchShowStar,     // �׾���������
        FirstMatchShowMain,     // �׾��������ƾ���
        EveryMatchShowStar,     // ÿ������������ɫ
        EveryMatchShowMain,     // ÿ���������ƾ�������ɫ
        EveryMatchRandom,       // ��ׯ���������ɫÿ�����
        RandomMainPlayerWithColorSpade,     // ��ׯ���������ɫ�̶�Ϊ����
        RandomMainPlayerWithColorHeart,     // ��ׯ���������ɫ�̶�Ϊ����
        RandomMainPlayerWithColorCube,      // ��ׯ���������ɫ�̶�Ϊ�ݻ�
        RandomMainPlayerWithColorDiamond,   // ��ׯ���������ɫ�̶�Ϊ��Ƭ
    }

    public enum WinMatchWay
    {
        GotLastCards,
        FullMostScore,
        GotLastCardsAndFullScore,
        GotLastCardsOrFullMostScore,
    }
}