using GameBase.Core.Poker;
using GameBase.Core.Poker.Huolong;

using System.Collections.Generic;

using CardLayout = GameBase.Core.Poker.Huolong.CardLayout;

namespace GameBase.Model.Huolong
{
    public class Model : IModel
    {
        #region 游戏状态

        public GameState GameState { get; private set; } = GameState.Idle;
        public MatchState MatchState { get; private set; } = MatchState.Idle;

        #endregion 游戏状态

        #region 对局数据

        public CardColor MainColor { get; private set; } = CardColor.Unknown;
        public int MatchIndex { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public int RoundIndex { get; private set; } = 0;
        public int CurrentCamp { get; private set; } = 0;
        public int MainPoint => level[CurrentCamp];
        public int OftenMainPoint => settings.isConstantMain ? Core.Poker.Helper.GetNextPoint(settings.endLevel) : 0;
        public int MainPlayer => mainPlayers[CurrentCamp];

        #endregion 对局数据

        #region 回合状态

        public int LeadPlayer { get; private set; } = 0;
        public int CurrentPlayer { get; private set; } = 0;
        public int ShowingPlayer { get; private set; } = 0;
        public int ShowedPlayer { get; private set; } = 0;
        public int MainCardsCount => mainCardLayout.Count;
        public int ThrowNeedNum => roundCardLayout[LeadPlayer].Count;

        #endregion 回合状态


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
            // 检查时机
            if (GameState != GameState.GameRunning || MatchState != MatchState.GivingHandCards)
            {
                // todo 还要判断这是不是允许亮王的对局
                return GameOperationResponse.ShowStar_CannotShow;
            }
            // 检查玩家王牌数量
            var num = playerCardLayout[player].GetColorPointNum(Core.Poker.Helper.Joker1);
            if (num < showedCards.Length || num <= showingCards.Length)
            {
                return GameOperationResponse.ShowStar_CardsNotEnough;
            }
            // 成功, 执行数据记录
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
                // todo 判断配置和局数, 决定是否设定新的庄家
            }
            // 返回 true 后, 通过判断新的 showedPlayer 判断是亮王成功还是继续加亮
            return true;
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
                // 超过强制摘星限制数量, 强制所有未摘大王的玩家摘大王
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
                        int num = ret.pain[player].Length;
                        var gained = new List<int>();
                        bool gainedSuccess = true;
                        if (player != MainPlayer)
                        {
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

        // 设定
        private GameSetting settings = null;

        // 游戏数据

        private int[] mainPlayers = null;
        private int[] level = null;

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
