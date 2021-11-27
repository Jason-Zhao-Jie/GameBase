using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;

using System.Collections.Generic;

using CardLayout = GameBase.Common.Core.Poker.Huolong.CardLayout;

namespace GameBase.Common.Model.Poker.Huolong
{
    public class Model : IModel
    {
        #region ��Ϸ״̬

        public GameState GameState { get; private set; } = GameState.Idle;
        public MatchState MatchState { get; private set; } = MatchState.Idle;
        public GameSetting Setting => settings;

        #endregion ��Ϸ״̬

        #region �Ծ�����

        public CardColor MainColor { get; private set; } = CardColor.Unknown;
        public int MatchIndex { get; private set; } = 0;
        public int RoundIndex { get; private set; } = 0;
        public int CurrentCamp { get; private set; } = 0;
        public int MainPoint => level[CurrentCamp];
        public int OftenMainPoint => settings.isConstantMain ? (Core.Poker.Helper.GetNextPoint(settings.endLevel) == settings.startLevel ? settings.endLevel : Core.Poker.Helper.GetNextPoint(settings.endLevel)) : 0;
        public int MainPlayer => mainPlayers[CurrentCamp];

        #endregion �Ծ�����

        #region �غ�״̬

        public int LeadPlayer { get; private set; } = 0;
        public int CurrentPlayer { get; private set; } = 0;
        public int ShowingPlayer { get; private set; } = -1;
        public int ShowedPlayer { get; private set; } = 0;
        public int[] ShowedCards => showedCards;
        public int MainCardsCount => mainCardLayout.Count;
        public int ThrowNeedNum => roundCardLayout[LeadPlayer].Count;
        public int CurrentHandCardsNum => playerCardLayout[LeadPlayer].Count;

        #endregion �غ�״̬


        public Model()
        {
        }

        public int GetCampLevel(int camp)
        {
            return level[camp];
        }

        public int GetCampScore(int camp)
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
            mainPlayers = new int[s.campNum];
            level = new int[s.campNum];
            for (int i = 0; i < s.campNum; ++i)
            {
                mainPlayers[i] = i;
                level[i] = s.startLevel;
            }
            mainCardLayout = new CardLayout();
            mainCardLayout.Init(settings.groupNum);
            if (settings.InCreaseJokers())
            {
                // ȥ����С��
                mainCardLayout.Remove(Core.Poker.Helper.GetCardId(CardColor.Joker, 1, settings.groupNum - 1));
                mainCardLayout.Remove(Core.Poker.Helper.GetCardId(CardColor.Joker, 2, settings.groupNum - 1));
            }
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
            RoundIndex = 0;
            LeadPlayer = MainPlayer;
            CurrentPlayer = MainPlayer;
            if(matchIndex == 0)
            {
                ShowingPlayer = -1;
                ShowedPlayer = -1;
            }
            else
            {
                ShowingPlayer = MainPlayer;
                ShowedPlayer = MainPlayer;
            }
            score = new int[settings.campNum];
            showingCards = new int[0];
            showedCards = new int[0];
            mainCardLayout.PushCard(lastCardLayout.PopAll());
            foreach (var p in roundCardLayout)
            {
                mainCardLayout.PushCard(p.PopAll());
            }
            foreach (var p in playerCardLayout)
            {
                mainCardLayout.PushCard(p.PopAll());
            }
            mainCardLayout.RandomAllCards();
            MatchState = MatchState.Starting;
            return true;
        }

        public int MinShowCardsNum => System.Math.Max(showingCards.Length, showedCards.Length - (IsShowMainPoint ? 0 : 1)) + 1;

        /// <summary>
        /// ���ַ���ʱ�Ƿ���������
        /// </summary>
        public bool AllowShow
        {
            get
            {
                switch (settings.mainColorGetWay)
                {
                    case MainColorGetWay.EveryMatchShowStar:
                    case MainColorGetWay.EveryMatchShowMain:
                        return MainPoint != 0;
                    case MainColorGetWay.FirstMatchShowStar:
                    case MainColorGetWay.FirstMatchShowMain:
                        return MatchIndex == 0;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// �Ƿ����������ƴ���������
        /// </summary>
        public bool IsShowMainPoint
        {
            get
            {
                switch (settings.mainColorGetWay)
                {
                    case MainColorGetWay.EveryMatchShowMain:
                        return MainPoint != 0;
                    case MainColorGetWay.FirstMatchShowMain:
                        return MatchIndex == 0;
                    default:
                        return false;
                }
            }
        }

        public int SendOneCardToPlayer()
        {
            if(settings.lastCardsNum == mainCardLayout.Count)
            {
                return 0;
            }
            MatchState = MatchState.GivingHandCards;
            var card = mainCardLayout.PopCard();
            playerCardLayout[CurrentPlayer].PushCard(card);
            if(++CurrentPlayer >= settings.playerNum)
            {
                CurrentPlayer = 0;
            }
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

        /// <summary>
        /// ���ƣ�����������Ҫ�����ƻ������ơ���������ֱ�Ӳ�������������Ҫ����һ������
        /// </summary>
        /// <param name="player"> ���Ʋ�������� </param>
        /// <param name="color"> ������ʱ�Ļ�ɫ������������˲��� </param>
        /// <returns></returns>
        public GameOperationResponse SetShow(int player, CardColor color = CardColor.Joker)
        {
            // ���ʱ��
            if (GameState != GameState.GameRunning || MatchState != MatchState.GivingHandCards || !AllowShow)
            {
                return GameOperationResponse.ShowStar_CannotShow;
            }
            // ��������������
            int num;
            int card = Core.Poker.Helper.Joker1;
            if (IsShowMainPoint)
            {
                card = Core.Poker.Helper.GetCardId(color, MainPoint);
                num = playerCardLayout[player].GetColorPointNum(card);
            }
            else
            {
                num = playerCardLayout[player].GetColorPointNum(Core.Poker.Helper.Joker1);
            }
            if (num < MinShowCardsNum)
            {
                return GameOperationResponse.ShowStar_CardsNotEnough;
            }
            if (IsShowMainPoint)
            {
                ShowedPlayer = player;
                showedCards = playerCardLayout[player].GetColorPointCards(card);
                switch (settings.mainColorGetWay)
                {
                    case MainColorGetWay.FirstMatchShowMain:
                    case MainColorGetWay.EveryMatchShowMain:
                        MainColor = color;
                        if (MatchIndex == 0)
                        {
                            CurrentCamp = player % settings.campNum;
                            mainPlayers[CurrentCamp] = player;
                        }
                        break;
                }
            }
            else
            {
                // �ɹ�, ִ�����ݼ�¼
                ShowingPlayer = player;
                showingCards = playerCardLayout[player].GetColorPointCards(card);
            }
            return GameOperationResponse.Success;
        }

        public int[] ShowJokerResult(int player, int card)
        {
            if (player != ShowingPlayer || showingCards.Length == 0)
            {
                return null;
            }
            var newCards = new int[showingCards.Length + 1];
            for (var i = 0; i < showingCards.Length; ++i)
            {
                newCards[i] = showingCards[i];
            }
            newCards[showingCards.Length] = card;
            var color = Core.Poker.Helper.GetColor(card);
            if (color == CardColor.Joker)
            {
                showingCards = newCards;
                return showedCards;
            }
            else
            {
                showedCards = newCards;
                showingCards = new int[0];
                ShowedPlayer = player;
                ShowingPlayer = -1;
                switch (settings.mainColorGetWay)
                {
                    case MainColorGetWay.FirstMatchShowStar:
                    case MainColorGetWay.EveryMatchShowStar:
                        MainColor = color;
                        if (MatchIndex == 0)
                        {
                            CurrentCamp = player % settings.campNum;
                            mainPlayers[CurrentCamp] = player;
                        }
                        break;
                }
                return showedCards;
            }
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
                // ����ǿ��ժ����������, ǿ���������ժ����
                for (var p = 0; p < roundCardLayout.Length; ++p)
                {
                    if (p != MainPlayer)
                    {
                        // ��ׯ�����д���
                        roundCardLayout[p].RemoveCards(roundCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1));
                        roundCardLayout[p].PushCard(playerCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1));
                    }
                    else
                    {
                        // ׯ�ҽ���δ���Ĵ���
                        var joker1s = playerCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1);
                        foreach(var c in joker1s)
                        {
                            if (!roundCardLayout[p].Contains(c))
                            {
                                roundCardLayout[p].PushCard(c);
                            }
                        }
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
                        int num = player == MainPlayer ? ret.pain[player].Length - settings.lastCardsNum : ret.pain[player].Length;
                        var gained = new List<int>();
                        bool gainedSuccess = true;
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
                var suitCards = new List<int>(playerCardLayout[player].GetSuitableCards(roundCardLayout[LeadPlayer].GetAll(), MainColor, MainPoint, OftenMainPoint));
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
            // �趨����ֵ
            var ret = new RoundReport
            {
                roundIndex = RoundIndex,
                score = 0,
                winner = LeadPlayer,
                threwCards = new int[settings.playerNum][]
            };
            for (int i = 0; i < settings.playerNum; ++i)
            {
                ret.threwCards[i] = roundCardLayout[i].GetAll();
                foreach (var c in ret.threwCards[i])
                {
                    ret.score += Core.Poker.Helper.GetScore(c);
                }
            }
            // ������
            ret.winner = LeadPlayer;
            for (int c = LeadPlayer + 1; c != LeadPlayer; ++c)
            {
                if (c >= settings.playerNum)
                {
                    c = -1;
                    continue;
                }
                if (Core.Poker.Huolong.Helper.CompareAsThrewCards(roundCardLayout[ret.winner], roundCardLayout[c], MainColor, MainPoint, OftenMainPoint) < 0)
                {
                    ret.winner = c;
                }
            }
            // Ӧ�ý��
            LeadPlayer = ret.winner;
            score[LeadPlayer % settings.campNum] += ret.score;
            ++RoundIndex;
            return ret;
        }

        public MatchReport MakeMatchCalculate()
        {
            // ��¼��Ϣ
            var ret = new MatchReport
            {
                matchIndex = MatchIndex,
                totalScore = new int[settings.campNum],
                lastCards = lastCardLayout.GetAll(),
                oldMainPlayer = MainPlayer,
                oldLevels = new int[settings.campNum],
                finallyThrew = new int[settings.playerNum][]
            };
            for (int i = 0; i < settings.campNum; ++i)
            {
                ret.oldLevels[i] = level[i];
            }
            for (int i = 0; i < settings.playerNum; ++i)
            {
                ret.finallyThrew[i] = roundCardLayout[i].GetAll();
            }
            // ���Ʒ�
            var lastScore = lastCardLayout.GetScoreCount() * (settings.lastCardsScoreDouble ? 2 : 1);
            score[LeadPlayer % settings.campNum] += lastScore;
            // ���������
            var fullScore = new bool[settings.campNum];
            var halfScoreNum = 0;
            var noScoreNum = 0;
            int maxFullScore = 0;
            int maxFullScoreId = 0;
            for (int i = 0; i < settings.campNum; ++i)
            {
                ret.totalScore[i] = score[i];
                fullScore[i] = score[i] >= settings.fullScore;
                if (score[i] >= settings.fullScore / 2)
                {
                    ++halfScoreNum;
                }
                else if (score[i] == 0)
                {
                    ++noScoreNum;
                }
                if (i != CurrentCamp)
                {
                    if (score[i] > maxFullScore)
                    {
                        maxFullScore = score[i];
                        maxFullScoreId = i;
                    }
                    else if (score[i] == maxFullScore)
                    {
                        if (maxFullScore == 0 || i + settings.campNum - CurrentCamp < maxFullScoreId + settings.campNum - CurrentCamp)
                        {
                            maxFullScore = score[i];
                            maxFullScoreId = i;
                        }
                    }
                }
            }
            // ���ݹ���������Ӯ��
            switch (settings.winMatchWay)
            {
                case WinMatchWay.GotLastCards:
                    ret.winner = LeadPlayer % settings.campNum;
                    break;
                case WinMatchWay.FullMostScore:
                    ret.winner = maxFullScore == 0 ? CurrentCamp : maxFullScoreId;
                    break;
                case WinMatchWay.GotLastCardsAndFullScore:
                    ret.winner = fullScore[LeadPlayer % settings.campNum] ? LeadPlayer % settings.campNum : CurrentCamp;
                    break;
                case WinMatchWay.GotLastCardsOrFullMostScore:
                    if (LeadPlayer % settings.campNum != CurrentCamp)
                    {
                        ret.winner = LeadPlayer % settings.campNum;
                    }
                    else
                    {
                        ret.winner = maxFullScore == 0 ? CurrentCamp : maxFullScoreId;
                    }
                    break;
            }
            // ����������
            int totalLevel;
            if (ret.winner == LeadPlayer % settings.campNum)
            {
                // �ٵ�����
                var numJoker1 = lastCardLayout.GetColorPointCards(Core.Poker.Helper.Joker1).Length;
                var numJoker2 = lastCardLayout.GetColorPointCards(Core.Poker.Helper.Joker2).Length;
                if (numJoker1 == 0)
                {
                    // ���ǿٵ�
                    totalLevel = settings.noJokerBaseUpgrade;
                }
                else
                {
                    // ժ�ǿٵ�����
                    totalLevel = settings.haveJokerBaseUpgrade + numJoker1 * settings.joker1AddUpgrade + numJoker2 * settings.joker2AddUpgrade;
                }
            }
            else
            {
                // δ�ٵ�
                totalLevel = settings.noLastCardsBaseUpgrade;
            }
            if (ret.winner == CurrentCamp)
            {
                // ��ׯ�ɹ�,�����мҷ�������������
                if (halfScoreNum == 0)
                {
                    totalLevel += settings.noHalfScroreAddUpgrade;
                }
                totalLevel += noScoreNum * settings.noScoreAddUpgrade;
            }
            else if (LeadPlayer % settings.campNum != CurrentCamp && fullScore[LeadPlayer % settings.campNum])
            {
                // ��ׯʧ�ܼ���Ƿ񱻽���, ��ׯ�ҽ���Ҫ��ٵ����Լ������㹻(�����ط�����ߣ�
                int thresholdIndex = -1;
                if (settings.thresholdsLevels != null)
                {
                    for (int i = 0; i < settings.thresholdsLevels.Length; ++i)
                    {
                        if (MainPoint == settings.thresholdsLevels[i])
                        {
                            thresholdIndex = i;
                            break;
                        }
                    }
                }
                if (thresholdIndex >= 0)
                {
                    // �ż���,�п��ܽ���,�����ٵ�һ�ֵ������Ƿ��ܵ��½���
                    bool downGrade = false;
                    var winCard = roundCardLayout[LeadPlayer].First();
                    if (Core.Poker.Helper.GetColorPoint(winCard) == Core.Poker.Helper.Joker1 && settings.downgradeByJoker1)
                    {
                        downGrade = true;
                    }
                    if (Core.Poker.Helper.GetColorPoint(winCard) == Core.Poker.Helper.Joker2 && settings.downgradeByJoker2)
                    {
                        downGrade = true;
                    }
                    if (Core.Poker.Helper.GetColor(winCard) == MainColor && Core.Poker.Helper.GetPoint(winCard) == MainPoint && settings.downgradeByMainCP)
                    {
                        downGrade = true;
                    }
                    if (Core.Poker.Helper.GetColor(winCard) != MainColor && Core.Poker.Helper.GetPoint(winCard) == MainPoint && settings.downgradeByUnMainCP)
                    {
                        downGrade = true;
                    }
                    if (Core.Poker.Helper.GetColor(winCard) != MainColor && Core.Poker.Helper.GetPoint(winCard) == OftenMainPoint && settings.downgradeByMainConstantMain)
                    {
                        downGrade = true;
                    }
                    if (Core.Poker.Helper.GetColor(winCard) != MainColor && Core.Poker.Helper.GetPoint(winCard) == OftenMainPoint && settings.downgradeByUnMainConstantMain)
                    {
                        downGrade = true;
                    }
                    if (downGrade)
                    {
                        if (thresholdIndex == 0)
                        {
                            totalLevel = settings.startLevel - settings.thresholdsLevels[thresholdIndex];
                        }
                        else
                        {
                            totalLevel = settings.thresholdsLevels[thresholdIndex - 1] - settings.thresholdsLevels[thresholdIndex];
                        }
                        if (totalLevel > 0)
                        {
                            totalLevel -= 13;
                        }
                    }
                }
            }
            bool onJokerLevel = false;
            if (totalLevel < 0)
            {
            }
            else
            {
                CurrentCamp = ret.winner;
                // �����ֹ��Խ�ż���
                if (ret.winner == CurrentCamp && MainPoint == 0)
                {
                    ret.gameover = true;
                }
                else if (settings.thresholdsLevels != null)
                {
                    for (int i = 1; i < totalLevel; ++i)
                    {
                        int targetLevel = level[ret.winner] + i;
                        while (targetLevel > 13)
                        {
                            targetLevel -= 13;
                        }
                        for (int n = 0; n < settings.thresholdsLevels.Length; ++n)
                        {
                            if (targetLevel == settings.thresholdsLevels[n])
                            {
                                totalLevel = i;
                                break;
                            }
                        }
                        if (totalLevel - 1 > i)
                        {
                            if (i == settings.endLevel)
                            {
                                if (settings.jokerAfterEndLevel)
                                {
                                    onJokerLevel = true;
                                    totalLevel = i + 1;
                                    break;
                                }
                                else
                                {
                                    ret.gameover = true;
                                }
                            }
                        }
                    }
                }
            }
            if (ret.gameover)
            {
                ret.upgradedLevelNumber = 1;
                level[CurrentCamp] = settings.jokerAfterEndLevel ? 0 : settings.endLevel;
            }
            else
            {
                ret.upgradedLevelNumber = totalLevel;
                level[CurrentCamp] = onJokerLevel ? 0 : (level[CurrentCamp] + totalLevel);
                while (level[CurrentCamp] > 13)
                {
                    level[CurrentCamp] -= 13;
                }
            }
            mainPlayers[CurrentCamp] += settings.campNum;
            if (mainPlayers[CurrentCamp] >= settings.playerNum)
            {
                mainPlayers[CurrentCamp] = CurrentCamp;
            }
            ret.newMainPlayer = MainPlayer;
            ret.newLevels = new int[settings.campNum];
            for (int i = 0; i < settings.campNum; ++i)
            {
                ret.newLevels[i] = level[i];
            }

            MatchState = MatchState.Idle;
            return ret;
        }

        public GameReport GetGameResult()
        {
            var ret = new GameReport
            {
                finalLevels = new int[settings.campNum],
                totalMatches = MatchIndex,
                winner = CurrentCamp
            };
            for (int i = 0; i < ret.finalLevels.Length; ++i)
            {
                ret.finalLevels[i] = GetCampLevel(i);
            }
            GameState = GameState.Idle;
            return ret;
        }

        // �趨
        private GameSetting settings;

        // ��Ϸ����

        private int[] mainPlayers = null;
        private int[] level = null;
        private int[] score = null;

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
