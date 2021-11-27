using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;

using System.Collections.Generic;

using CardLayout = GameBase.Common.Core.Poker.Huolong.CardLayout;

namespace GameBase.Common.Model.Poker.Huolong
{
    public class Model : IModel
    {
        #region 游戏状态

        public GameState GameState { get; private set; } = GameState.Idle;
        public MatchState MatchState { get; private set; } = MatchState.Idle;
        public GameSetting Setting => settings;

        #endregion 游戏状态

        #region 对局数据

        public CardColor MainColor { get; private set; } = CardColor.Unknown;
        public int MatchIndex { get; private set; } = 0;
        public int RoundIndex { get; private set; } = 0;
        public int CurrentCamp { get; private set; } = 0;
        public int MainPoint => level[CurrentCamp];
        public int OftenMainPoint => settings.isConstantMain ? (Core.Poker.Helper.GetNextPoint(settings.endLevel) == settings.startLevel ? settings.endLevel : Core.Poker.Helper.GetNextPoint(settings.endLevel)) : 0;
        public int MainPlayer => mainPlayers[CurrentCamp];

        #endregion 对局数据

        #region 回合状态

        public int LeadPlayer { get; private set; } = 0;
        public int CurrentPlayer { get; private set; } = 0;
        public int ShowingPlayer { get; private set; } = -1;
        public int ShowedPlayer { get; private set; } = 0;
        public int[] ShowedCards => showedCards;
        public int MainCardsCount => mainCardLayout.Count;
        public int ThrowNeedNum => roundCardLayout[LeadPlayer].Count;
        public int CurrentHandCardsNum => playerCardLayout[LeadPlayer].Count;

        #endregion 回合状态


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
                // 去掉大小王
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
        /// 本局发牌时是否允许亮牌
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
        /// 是否以亮主打牌代替亮王牌
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
        /// 亮牌，根据设置需要亮王牌或者主牌。亮主牌则直接操作，亮王牌需要等下一轮摸牌
        /// </summary>
        /// <param name="player"> 亮牌操作的玩家 </param>
        /// <param name="color"> 亮主牌时的花色，亮王牌无须此参数 </param>
        /// <returns></returns>
        public GameOperationResponse SetShow(int player, CardColor color = CardColor.Joker)
        {
            // 检查时机
            if (GameState != GameState.GameRunning || MatchState != MatchState.GivingHandCards || !AllowShow)
            {
                return GameOperationResponse.ShowStar_CannotShow;
            }
            // 检查玩家王牌数量
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
                // 成功, 执行数据记录
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
        /// 将底牌交给庄家, 这不会删除底牌区的缓存, 以便玩家继续查看. 只有在埋底操作完成时底牌区会替换成新的底牌
        /// </summary>
        public void SendLastCardToMainPlayer()
        {
            playerCardLayout[MainPlayer].PushCard(lastCardLayout.GetAll());
            MatchState = MatchState.SendingLastCards;
        }

        /// <summary>
        /// 记录埋底, 庄家必须数量正确, 非庄家必须只能埋王牌
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cards"></param>
        public GameOperationResponse MakeOnesLastCards(int player, int[] cards)
        {
            // 检查时机
            if (GameState != GameState.GameRunning || MatchState != MatchState.SendingLastCards)
            {
                return GameOperationResponse.LastCards_CannotThrow;
            }
            if (player == MainPlayer && cards.Length != settings.lastCardsNum)
            {
                // 庄家数量必须正确
                return GameOperationResponse.LastCards_NumberWrong;
            }
            else
            {
                // 非庄家只能是王牌
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
            // 检查牌内容
            foreach (var c in cards)
            {
                // 检查是否拥有声明的牌
                if (!playerCardLayout[player].Contains(c))
                {
                    return GameOperationResponse.LastCards_YouDonnotHave;
                }
                // 检查是否声明重复的牌
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
        /// 埋底结算
        /// </summary>
        /// <returns></returns>
        public LastCardsReport MakeLastCardsReport()
        {
            // 检验和修正摘星

            int numJoker1 = 0;  // 记录大王牌数量, 以便决定是否强制摘星
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
                // 无人摘大王, 则小王无效, 返还所有非庄家的牌
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
                // 超过强制摘星限制数量, 强制所有玩家摘大王
                for (var p = 0; p < roundCardLayout.Length; ++p)
                {
                    if (p != MainPlayer)
                    {
                        // 非庄家所有大王
                        roundCardLayout[p].RemoveCards(roundCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1));
                        roundCardLayout[p].PushCard(playerCardLayout[p].GetColorPointCards(Core.Poker.Helper.Joker1));
                    }
                    else
                    {
                        // 庄家交出未出的大王
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

            // 摘星结果记录
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

            // 摘星返还牌计算
            // 选择尽可能不与玩家手牌配成重牌的返还方案，优先级为：非王牌>非分数>非5>非4>非3>非2>小牌>大牌  
            // 1表示允许返还5分，2表示允许返还分数，3表示允许返还分数和小王
            for (int allowedRules = 0; allowedRules < 4; ++allowedRules)
            {
                bool needContinue = true;
                // 从少到多，检测重复最少的方案，
                for (int allowed = 1; allowed <= settings.groupNum; ++allowed)
                {
                    bool allSuccess = true;
                    var tryingCards = new List<int>();
                    for (int i = 0; i < allCards.Count; ++i)
                    {
                        tryingCards[i] = allCards[i];
                    }
                    //依座次
                    for (int player = 0; player < settings.playerNum; ++player)
                    {
                        int num = player == MainPlayer ? ret.pain[player].Length - settings.lastCardsNum : ret.pain[player].Length;
                        var gained = new List<int>();
                        bool gainedSuccess = true;
                        //补足对应数量的牌
                        for (int i = 0; i < num; ++i)
                        {
                            // 依次查找每一张底牌
                            bool found = false;
                            for (int cIndex = 0; cIndex < tryingCards.Count; ++cIndex)
                            {
                                int c = tryingCards[cIndex];
                                // 先检查牌的重复次数
                                int repeated = 0;
                                // 之前已返还的牌中重复
                                foreach (var gainedC in gained)
                                {
                                    if (Core.Poker.Helper.GetColorPoint(c) == Core.Poker.Helper.GetColorPoint(gainedC))
                                    {
                                        repeated++;
                                    }
                                }
                                // 手牌中重复
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
            // 剩下的是最终底牌
            ret.lastCards = allCards.ToArray();

            // 执行交换
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
            // 检查时机
            if (GameState != GameState.GameRunning || MatchState != MatchState.Rounding || CurrentPlayer != player)
            {
                return GameOperationResponse.CardsThrew_Unavailable;
            }
            // 检查参数中的重复牌
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
            // 检查是否拥有声明的牌
            foreach (var c in cards)
            {
                if (!playerCardLayout[player].Contains(c))
                {
                    return GameOperationResponse.LastCards_YouDonnotHave;
                }
            }
            // 检查出牌数量 (首家不能为0，非首家必须与首家数量一致)
            if (cards == null || cards.Length == 0 || (player != LeadPlayer && cards.Length != roundCardLayout[LeadPlayer].Count))
            {
                return GameOperationResponse.CardsThrew_NumberWrong;
            }
            if (player == LeadPlayer)
            {
                // 检查首家出牌牌型, 首家的所有牌必须一样
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
                // 非首家必须把同花色牌出完, 才能出其他花色
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

            // 执行结果
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
            // 设定返回值
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
            // 计算结果
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
            // 应用结果
            LeadPlayer = ret.winner;
            score[LeadPlayer % settings.campNum] += ret.score;
            ++RoundIndex;
            return ret;
        }

        public MatchReport MakeMatchCalculate()
        {
            // 记录信息
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
            // 底牌分
            var lastScore = lastCardLayout.GetScoreCount() * (settings.lastCardsScoreDouble ? 2 : 1);
            score[LeadPlayer % settings.campNum] += lastScore;
            // 检测分数情况
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
            // 根据规则检测最终赢家
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
            // 计算升级数
            int totalLevel;
            if (ret.winner == LeadPlayer % settings.campNum)
            {
                // 抠底升级
                var numJoker1 = lastCardLayout.GetColorPointCards(Core.Poker.Helper.Joker1).Length;
                var numJoker2 = lastCardLayout.GetColorPointCards(Core.Poker.Helper.Joker2).Length;
                if (numJoker1 == 0)
                {
                    // 无星抠底
                    totalLevel = settings.noJokerBaseUpgrade;
                }
                else
                {
                    // 摘星抠底连升
                    totalLevel = settings.haveJokerBaseUpgrade + numJoker1 * settings.joker1AddUpgrade + numJoker2 * settings.joker2AddUpgrade;
                }
            }
            else
            {
                // 未抠底
                totalLevel = settings.noLastCardsBaseUpgrade;
            }
            if (ret.winner == CurrentCamp)
            {
                // 坐庄成功,根据闲家分数情况测算加升
                if (halfScoreNum == 0)
                {
                    totalLevel += settings.noHalfScroreAddUpgrade;
                }
                totalLevel += noScoreNum * settings.noScoreAddUpgrade;
            }
            else if (LeadPlayer % settings.campNum != CurrentCamp && fullScore[LeadPlayer % settings.campNum])
            {
                // 坐庄失败检测是否被降级, 打庄家降级要求抠底者自己分数足够(但不必分数最高）
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
                    // 门槛级,有可能降级,看看抠底一手的牌型是否能导致降级
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
                // 处理禁止超越门槛级
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

        // 设定
        private GameSetting settings;

        // 游戏数据

        private int[] mainPlayers = null;
        private int[] level = null;
        private int[] score = null;

        // 回合数据

        private int[] showingCards = null;
        private int[] showedCards = null;

        // 牌区

        private CardLayout mainCardLayout = null;
        private CardLayout lastCardLayout = null;
        private CardLayout[] roundCardLayout = null;
        private CardLayout[] playerCardLayout = null;
    }
}
