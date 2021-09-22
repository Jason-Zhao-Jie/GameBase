using GameBase.Core.Poker;
using GameBase.Core.Poker.Huolong;

using System.Collections.Generic;

using CardLayout = GameBase.Core.Poker.Huolong.CardLayout;

namespace GameBase.Model.Huolong
{
    public class Model : IModel
    {
        #region ��Ϸ״̬

        public GameState GameState { get; private set; } = GameState.Idle;
        public MatchState MatchState { get; private set; } = MatchState.Idle;

        #endregion ��Ϸ״̬

        #region �Ծ�����

        public CardColor MainColor { get; private set; } = CardColor.Unknown;
        public int MatchIndex { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public int RoundIndex { get; private set; } = 0;
        public int CurrentCamp { get; private set; } = 0;
        public int MainPoint => level[CurrentCamp];
        public int OftenMainPoint => settings.isConstantMain ? Core.Poker.Helper.GetNextPoint(settings.endLevel) : 0;
        public int MainPlayer => mainPlayers[CurrentCamp];

        #endregion �Ծ�����

        #region �غ�״̬

        public int LeadPlayer { get; private set; } = 0;
        public int CurrentPlayer { get; private set; } = 0;
        public int ShowingPlayer { get; private set; } = 0;
        public int ShowedPlayer { get; private set; } = 0;
        public int MainCardsCount => mainCardLayout.Count;
        public int ThrowNeedNum => roundCardLayout[LeadPlayer].Count;

        #endregion �غ�״̬


        public Model()
        {
        }

        public int GetCampLevel(int camp)
        {
            return level[camp];
        }

        public CardLayout GetPlayerCardLayout(int player)
        {
            return playerCardLayout[player];
        }

        public CardLayout GetThrewCardLayout(int player)
        {
            return roundCardLayout[player];
        }

        public CardLayout GetLastCardsLayout()
        {
            return lastCardLayout;
        }

        public void InitGame(GameSetting s)
        {
            settings = s;
            mainPlayers = new int[s.playerNum];
            level = new int[s.campNum];
            mainCardLayout = new CardLayout();
            lastCardLayout = new CardLayout();
            roundCardLayout = new CardLayout[s.playerNum];
            playerCardLayout = new CardLayout[s.playerNum];
            for (var i = 0; i < s.playerNum; ++i)
            {
                roundCardLayout[i] = new CardLayout();
                playerCardLayout[i] = new CardLayout();
            }
            GameState = GameState.GameRunning;
        }

        public bool InitNewMatch(int matchIndex)
        {
            if (matchIndex != 0 && matchIndex != MatchIndex && matchIndex != MatchIndex + 1)
            {
                return false;
            }
            MatchIndex = matchIndex;
            showingCards = new int[0];
            showedCards = new int[0];
            mainCardLayout.Init(settings.groupNum);
            mainCardLayout.RandomAllCards();
            MatchState = MatchState.Starting;
            return true;
        }

        public int SendOneCardToPlayer(int player)
        {
            MatchState = MatchState.GivingHandCards;
            var card = mainCardLayout.PopCard();
            playerCardLayout[player].PushCard(card);
            return card;
        }

        public int[] SendCardsToLast()
        {
            var ret = new List<int>();
            while (mainCardLayout.Count > 0)
            {
                var card = mainCardLayout.PopCard();
                ret.Add(card);
                lastCardLayout.PushCard(card);
            }
            MatchState = MatchState.GivingLastCards;
            return ret.ToArray();
        }

        public int[] SendAllCardsOver()
        {
            MatchState = MatchState.GivingHandCards;
            while (mainCardLayout.Count > settings.lastCardsNum)
            {
                foreach (var p in playerCardLayout)
                {
                    var card = mainCardLayout.PopCard();
                    p.PushCard(card);
                }
            }
            return SendCardsToLast();
        }

        public GameOperationResponse SetWillShowJoker(int player)
        {
            // ���ʱ��
            if (GameState != GameState.GameRunning || MatchState != MatchState.GivingHandCards)
            {
                // todo ��Ҫ�ж����ǲ������������ĶԾ�
                return GameOperationResponse.ShowStar_CannotShow;
            }
            // ��������������
            var num = playerCardLayout[player].GetColorPointNum(Core.Poker.Helper.Joker1);
            if (num < showedCards.Length || num <= showingCards.Length)
            {
                return GameOperationResponse.ShowStar_CardsNotEnough;
            }
            // �ɹ�, ִ�����ݼ�¼
            ShowingPlayer = player;
            showingCards = playerCardLayout[player].GetColorPointCards(Core.Poker.Helper.Joker1);
            return GameOperationResponse.Success;
        }

        public bool ShowJokerResult(int player, int card)
        {
            if (player != ShowingPlayer || showingCards.Length == 0)
            {
                return false;
            }
            var newCards = new int[showingCards.Length + 1];
            for (var i = 0; i < showingCards.Length; ++i)
            {
                newCards[i] = showingCards[i];
            }
            newCards[showedCards.Length] = card;
            var color = Core.Poker.Helper.GetColor(card);
            if (color == CardColor.Joker)
            {
                showingCards = newCards;
            }
            else
            {
                showedCards = newCards;
                showingCards = new int[0];
                ShowedPlayer = player;
                // todo �ж����ú;���, �����Ƿ��趨�µ�ׯ��
            }
            // ���� true ��, ͨ���ж��µ� showedPlayer �ж��������ɹ����Ǽ�������
            return true;
        }

        /// <summary>
        /// �����ƽ���ׯ��, �ⲻ��ɾ���������Ļ���, �Ա���Ҽ����鿴. ֻ������ײ������ʱ���������滻���µĵ���
        /// </summary>
        public void SendLastCardToMainPlayer()
        {
            playerCardLayout[MainPlayer].PushCard(lastCardLayout.GetAll());
            MatchState = MatchState.SendingLastCards;
        }

        /// <summary>
        /// ��¼���, ׯ�ұ���������ȷ, ��ׯ�ұ���ֻ��������
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cards"></param>
        public GameOperationResponse MakeOnesLastCards(int player, int[] cards)
        {
            // ���ʱ��
            if (GameState != GameState.GameRunning || MatchState != MatchState.SendingLastCards)
            {
                return GameOperationResponse.LastCards_CannotThrow;
            }
            if (player == MainPlayer && cards.Length != settings.lastCardsNum)
            {
                // ׯ������������ȷ
                return GameOperationResponse.LastCards_NumberWrong;
            }
            else
            {
                // ��ׯ��ֻ��������
                if (player != MainPlayer && cards != null && cards.Length > 0)
                {
                    foreach (var c in cards)
                    {
                        if (Core.Poker.Helper.GetColor(c) != CardColor.Joker)
                        {
                            return GameOperationResponse.LastCards_TypeError;
                        }
                    }
                }
            }
            // ���������
            foreach (var c in cards)
            {
                // ����Ƿ�ӵ����������
                if (!playerCardLayout[player].Contains(c))
                {
                    return GameOperationResponse.LastCards_YouDonnotHave;
                }
                // ����Ƿ������ظ�����
                int repeat = 0;
                for (int i = 0; i < cards.Length; ++i)
                {
                    if (c == cards[i])
                    {
                        ++repeat;
                    }
                }
                if (repeat > 1)
                {
                    return GameOperationResponse.LastCards_Repeated;
                }
            }
            if (cards != null && cards.Length > 0)
            {
                roundCardLayout[player].PushCard(cards);
            }
            return GameOperationResponse.Success;
        }

        /// <summary>
        /// ��׽���
        /// </summary>
        /// <returns></returns>
        public LastCardsReport MakeLastCardsReport()
        {
            // ���������ժ��

            int numJoker1 = 0;  // ��¼����������, �Ա�����Ƿ�ǿ��ժ��
            foreach (var p in roundCardLayout)
            {
                foreach (var c in p)
                {
                    if (Core.Poker.Helper.GetColorPoint(c) == Core.Poker.Helper.Joker1)
                    {
                        ++numJoker1;
                    }
                }
            }
            if (numJoker1 == 0)
            {
                // ����ժ����, ��С����Ч, �������з�ׯ�ҵ���
                for (var p = 0; p < roundCardLayout.Length; ++p)
                {
                    if (p != MainPlayer)
                    {
                        roundCardLayout[p].Clear();
                    }
                }
            }
            else if (numJoker1 >= settings.forceSendJokerLastCardsNum)
            {
                // ����ǿ��ժ����������, ǿ������δժ���������ժ����
                for (var p = 0; p < roundCardLayout.Length; ++p)
                {
                    bool hasJoker1 = false;
                    foreach (var c in roundCardLayout[p])
                    {
                        if (Core.Poker.Helper.GetColorPoint(c) == Core.Poker.Helper.Joker1)
                        {
                            hasJoker1 = true;
                            break;
                        }
                    }
                    if (!hasJoker1)
                    {
                        roundCardLayout[p].PushCard(playerCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1));
                    }
                }
            }

            // ժ�ǽ����¼
            var ret = new LastCardsReport
            {
                pain = new int[settings.playerNum][],
                gain = new int[settings.playerNum][]
            };
            for (var p = 0; p < roundCardLayout.Length; ++p)
            {
                ret.pain[p] = roundCardLayout[p].PopAll();
            }
            var allCards = new List<int>();
            foreach (var cs in ret.pain)
            {
                allCards.AddRange(cs);
            }
            allCards.Sort((int a, int b) =>
            {
                return Core.Poker.Huolong.Helper.CompareAsLastCards(a, b, MainColor, MainPoint, OftenMainPoint);
            });

            // ժ�Ƿ����Ƽ���
            // ѡ�񾡿��ܲ����������������Ƶķ������������ȼ�Ϊ��������>�Ƿ���>��5>��4>��3>��2>С��>����  
            // 1��ʾ������5�֣�2��ʾ������������3��ʾ������������С��
            for (int allowedRules = 0; allowedRules < 4; ++allowedRules)
            {
                bool needContinue = true;
                // ���ٵ��࣬����ظ����ٵķ�����
                for (int allowed = 1; allowed <= settings.groupNum; ++allowed)
                {
                    bool allSuccess = true;
                    var tryingCards = new List<int>();
                    for (int i = 0; i < allCards.Count; ++i)
                    {
                        tryingCards[i] = allCards[i];
                    }
                    //������
                    for (int player = 0; player < settings.playerNum; ++player)
                    {
                        int num = ret.pain[player].Length;
                        var gained = new List<int>();
                        bool gainedSuccess = true;
                        if (player != MainPlayer)
                        {
                            //�����Ӧ��������
                            for (int i = 0; i < num; ++i)
                            {
                                // ���β���ÿһ�ŵ���
                                bool found = false;
                                for (int cIndex = 0; cIndex < tryingCards.Count; ++cIndex)
                                {
                                    int c = tryingCards[cIndex];
                                    // �ȼ���Ƶ��ظ�����
                                    int repeated = 0;
                                    // ֮ǰ�ѷ����������ظ�
                                    foreach (var gainedC in gained)
                                    {
                                        if (Core.Poker.Helper.GetColorPoint(c) == Core.Poker.Helper.GetColorPoint(gainedC))
                                        {
                                            repeated++;
                                        }
                                    }
                                    // �������ظ�
                                    foreach (var handC in playerCardLayout[player])
                                    {
                                        if (Core.Poker.Helper.GetColorPoint(c) == Core.Poker.Helper.GetColorPoint(handC))
                                        {
                                            repeated++;
                                        }
                                    }
                                    if (repeated < allowed
                                        && (Core.Poker.Helper.GetScore(c) == 0 || allowedRules > 0)
                                        && (Core.Poker.Helper.GetScore(c) <= 5 || allowedRules > 1)
                                        && (Core.Poker.Helper.GetColor(c) != CardColor.Joker || allowedRules > 2)
                                        && (Core.Poker.Helper.GetColor(c) != CardColor.Joker || Core.Poker.Helper.GetPoint(c) != 1))
                                    {
                                        gained.Add(tryingCards[cIndex]);
                                        tryingCards.RemoveAt(cIndex);
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    gainedSuccess = false;
                                    break;
                                }
                            }
                        }
                        if (gainedSuccess)
                        {
                            ret.gain[player] = gained.ToArray();
                        }
                        else
                        {
                            allSuccess = false;
                            break;
                        }
                    }
                    if (allSuccess)
                    {
                        needContinue = false;
                        allCards = tryingCards;
                        break;
                    }
                }
                if (!needContinue)
                {
                    break;
                }
            }
            // ʣ�µ������յ���
            ret.lastCards = allCards.ToArray();

            // ִ�н���
            lastCardLayout.Clear();
            lastCardLayout.PushCard(ret.lastCards);
            for (int player = 0; player < settings.playerNum; ++player)
            {
                playerCardLayout[player].RemoveCards(ret.pain[player]);
                playerCardLayout[player].PushCard(ret.gain[player]);
            }

            MatchState = MatchState.Rounding;
            return ret;
        }

        public void ClearRoundCards()
        {
            foreach (var p in roundCardLayout)
            {
                mainCardLayout.PushCard(p.PopAll());
            }
        }

        public GameOperationResponse MakeOnesThrew(int player, int[] cards)
        {
            // ���ʱ��
            if (GameState != GameState.GameRunning || MatchState != MatchState.Rounding || CurrentPlayer != player)
            {
                return GameOperationResponse.CardsThrew_Unavailable;
            }
            // �������е��ظ���
            if (cards != null)
            {
                foreach (var c in cards)
                {
                    int repeated = 0;
                    for (var i = 0; i < cards.Length; ++i)
                    {
                        if (c == cards[i])
                        {
                            ++repeated;
                        }
                    }
                    if (repeated > 1)
                    {
                        return GameOperationResponse.CardsThrew_Repeated;
                    }
                }
            }
            // ����Ƿ�ӵ����������
            foreach (var c in cards)
            {
                if (!playerCardLayout[player].Contains(c))
                {
                    return GameOperationResponse.LastCards_YouDonnotHave;
                }
            }
            // ���������� (�׼Ҳ���Ϊ0�����׼ұ������׼�����һ��)
            if (cards == null || cards.Length == 0 || (player != LeadPlayer && cards.Length != roundCardLayout[LeadPlayer].Count))
            {
                return GameOperationResponse.CardsThrew_NumberWrong;
            }
            if (player == LeadPlayer)
            {
                // ����׼ҳ�������, �׼ҵ������Ʊ���һ��
                for (var i = 1; i < cards.Length; ++i)
                {
                    if (Core.Poker.Helper.GetColorPoint(cards[i - 1]) != Core.Poker.Helper.GetColorPoint(cards[i]))
                    {
                        return GameOperationResponse.CardsThrew_TypeWrong;
                    }
                }
            }
            else
            {
                // ���׼ұ����ͬ��ɫ�Ƴ���, ���ܳ�������ɫ
                var suitCards = new List<int>(playerCardLayout[player].GetSuitableCards(roundCardLayout[LeadPlayer], MainColor, MainPoint, OftenMainPoint));
                int targetNum = System.Math.Min(suitCards.Count, cards.Length);
                foreach (var c in cards)
                {
                    if (suitCards.Contains(c))
                    {
                        --targetNum;
                    }
                }
                if (targetNum > 0)
                {
                    return GameOperationResponse.CardsThrew_ColorWrong;
                }
            }

            // ִ�н��
            playerCardLayout[player].RemoveCards(cards);
            roundCardLayout[player].PushCard(cards);
            ++CurrentPlayer;
            if (CurrentPlayer >= settings.playerNum)
            {
                CurrentPlayer = 0;
            }
            return GameOperationResponse.Success;
        }

        public RoundReport MakeRoundCalculate()
        {
            // todo
            return default;
        }

        public MatchReport MakeMatchCalculate()
        {
            // todo
            MatchState = MatchState.Idle;
            return default;
        }

        public GameReport GetGameResult()
        {
            //todo
            GameState = GameState.Idle;
            return default;
        }

        // �趨
        private GameSetting settings = null;

        // ��Ϸ����

        private int[] mainPlayers = null;
        private int[] level = null;

        // �غ�����

        private int[] showingCards = null;
        private int[] showedCards = null;

        // ����

        private CardLayout mainCardLayout = null;
        private CardLayout lastCardLayout = null;
        private CardLayout[] roundCardLayout = null;
        private CardLayout[] playerCardLayout = null;
    }
}
