using GameBase.Core.Poker;
using GameBase.Core.Poker.Huolong;

using System.Collections.Generic;

using CardLayout = GameBase.Core.Poker.Huolong.CardLayout;

namespace GameBase.Model.Huolong
{
    public class HuolongMain
    {
        // 游戏状态

        public GameState GameState { get; set; } = GameState.Idle;
        public MatchState MatchState { get; set; } = MatchState.Idle;

        // 对局数据

        public CardColor MainColor { get; private set; } = CardColor.Unknown;
        public int MatchIndex { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public int RoundIndex { get; private set; } = 0;
        public int CurrentCamp { get; private set; } = 0;

        public int MainPoint => level[CurrentCamp];

        // 回合状态

        public int LeadPlayer { get; private set; } = 0;
        public int CurrentPlayer { get; private set; } = 0;
        public int ShowingPlayer { get; private set; } = 0;
        public int ShowedPlayer { get; private set; } = 0;
        public int MainCardsCount => mainCardLayout.Count;

        public HuolongMain()
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

        public CardLayout GetLastCardsLayout()
        {
            return lastCardLayout;
        }
        public int getThrowNeedNum()
        {
            return roundCardLayout[LeadPlayer].Count;
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
        }

        public bool InitNewMatch(int matchIndex)
        {
            if(matchIndex != 0 && matchIndex != MatchIndex && matchIndex != MatchIndex + 1)
            {
                return false;
            }
            MatchIndex = matchIndex;
            showingCards = new int[0];
            showedCards = new int[0];
            mainCardLayout.Init(settings.groupNum);
            mainCardLayout.RandomAllCards();
            return true;
        }

        public int SendOneCardToPlayer(int player)
        {
            var card = mainCardLayout.PopCard();
            playerCardLayout[player].PushCard(card);
            return card;
        }

        public int[] SendCardsToLast()
        {
            var ret = new List<int>();
            while(mainCardLayout.Count > 0)
            {
                var card = mainCardLayout.PopCard();
                ret.Add(card);
                lastCardLayout.PushCard(card);
            }
            return ret.ToArray();
        }

        public int[] SendAllCardsOver()
        {
            while(mainCardLayout.Count > settings.lastCardsNum)
            {
                foreach(var p in playerCardLayout)
                {
                    var card = mainCardLayout.PopCard();
                    p.PushCard(card);
                }
            }
            return SendCardsToLast();
        }

        public bool SetShowJoker(int player)
        {
            var num = playerCardLayout[player].GetColorPointNum(Core.Poker.Helper.Joker1);
            if (num < showedCards.Length || num <= showingCards.Length)
            {
                return false;
            }
            ShowingPlayer = player;
            showingCards = playerCardLayout[player].GetColorPointCards(Core.Poker.Helper.Joker1);
            return true;
        }

        public bool ShowJokerResult(int player, int card)
        {
            if(player != ShowingPlayer)
            {
                return false;
            }
            showedCards = new int[showingCards.Length + 1];
            for(var i = 0; i < showingCards.Length; ++i)
            {
                showedCards[i] = showingCards[i];
            }
            showedCards[showedCards.Length] = card;
            // todo 验证牌的花色（是王牌则继续亮），花色正确则记录新主花色和庄家
            return true;
        }

        public LastCardsReport MakeLastCards()
        {
            // todo
            // todo 包含旧代码 makeLastCards 之后的全部
            return default;
        }

        public MatchReport MakeMatchCalculate()
        {
            // todo
            return default;
        }

        public GameReport GetGameResult()
        {
            //todo
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
