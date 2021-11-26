using GameBase.Common.Core;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Model.Poker.Huolong;
using CardLayout = GameBase.Common.Core.Poker.Huolong.CardLayout;

namespace GameBase.Present.Poker.Huolong
{
    public class AIPlayerItem : IPlayerItem<IPlayerVector_Item>
    {
        public GameType GameType => GameType.Poker;
        public GameSubType GameSubType => GameSubType.Huolong;
        public PlayerType PlayerType => PlayerType.HostAI;
        public CharacterInfo_AI CharacterInfo => vector.PlayerInfo as CharacterInfo_AI;
        public IModel Model => vector.Model;
        public int PlayerIndex => vector.PlayerIndex;

        public void SetVector(IPlayerVector_Item vector)
        {
            this.vector = vector;
        }

        public void OnDispose()
        {
        }

        public void OnGameStart()
        {
            setting = vector.GameSetting;
            myCards = new CardLayout();
            roundCards = new CardLayout[setting.playerNum];
            vector.Response(GameNoticeResponse.StartGame_Ready);
        }

        public void OnMatchStart()
        {
            myCards.Clear();
            vector.Response(GameNoticeResponse.StartMatch_Ready);
        }

        public void OnGetOneCard(int card)
        {
            myCards.PushCard(card);
            var (showAbleCards, showAbleColor) = vector.CheckShowAble(myCards);
            if (showAbleCards != null && showAbleColor != CardColor.Unknown)
            {
                vector.Operate(GameOperationEvent.ShowStar, showAbleCards);
            }
        }

        public void OnGetAllCards(int[] cards)
        {
            myCards.PushCard(cards);
        }

        public void OnPlayerShow(int player, int[] cards)
        {
        }

        public void OnPlayerShowResult(int player, int[] jokers, int target)
        {
        }

        public void OnMatchAborted()
        {
            vector.Response(GameNoticeResponse.MatchAborted_NobodyShowed_Confirm);
        }

        public void OnAskForLastCards(int[] mainPlayerLastCards)
        {
            myCards.SortAsHandCards(Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
            // todo 测试期间AI默认不摘星，摸到的底原样埋回，如需测试其他，更改此处
            if(Model.MainPlayer == PlayerIndex)
            {
                vector.Operate(GameOperationEvent.LastCardsThrow, mainPlayerLastCards);
            }
            else
            {
                vector.Operate(GameOperationEvent.LastCardsThrow, new int[0]);
            }
        }

        public void OnLastCardsOver(LastCardsReport report)
        {
            myCards.SortAsHandCards(Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
            vector.Response(GameNoticeResponse.PainLastCards_Confirm);
        }

        public void OnAskForThrow(int[] leaderCards)
        {
            if (leaderCards == null || leaderCards.Length == 0)
            {
                vector.Operate(GameOperationEvent.CardsThrew, new int[] { myCards.PopCard() });
            }
            else
            {
                var suitCards = myCards.GetSuitableCards(leaderCards, Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
                var target = new System.Collections.Generic.List<int>();
                int[] threw;
                if (leaderCards.Length > suitCards.Length)
                {
                    target.AddRange(suitCards);
                    myCards.RemoveCards(suitCards);
                    for (int i = 0; target.Count < leaderCards.Length; ++i)
                    {
                        target.Add(myCards.PopCard());
                    }
                    threw = target.ToArray();
                    myCards.PushCard(threw);
                    myCards.SortAsHandCards(Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
                }
                else
                {
                    for (int i = 0; target.Count < leaderCards.Length; ++i)
                    {
                        target.Add(suitCards[i]);
                    }
                    threw = target.ToArray();
                }
                vector.Operate(GameOperationEvent.CardsThrew, threw);
            }

        }

        public void OnPlayerThrew(int player, int[] threw)
        {
            if (player == PlayerIndex)
            {
                myCards.RemoveCards(threw);
            }
            roundCards[player].PushCard(threw);
        }

        public void OnRoundOver(RoundReport report)
        {
            foreach (var p in roundCards)
            {
                p.Clear();
            }
            vector.Response(GameNoticeResponse.Round_Confirm);
        }

        public void OnMatchOver(MatchReport report)
        {
            vector.Response(GameNoticeResponse.Match_Confirm);
        }

        public void OnGameOver(GameReport report)
        {
        }

        public void OnGameAborted()
        {
        }

        public void OnPlayerInfoChanged(int index, Common.Core.CharacterInfo newInfo)
        {
        }

        public void OnResponse(GameOperationEvent _event, GameOperationResponse response)
        {
            switch (_event)
            {
                case GameOperationEvent.ShowStar:
                case GameOperationEvent.LastCardsThrow:
                case GameOperationEvent.CardsThrew:
                    break;
                default:
                    Common.PlatformInterface.Base.DebugError("Unknown Operation Response received by Huolong AI player, player:" + PlayerIndex + ", operation code:" + (int)_event + ", response code:" + (int)response);
                    break;
            }
            }

        private IPlayerVector_Item vector;
        private GameSetting setting;
        private CardLayout myCards;
        private CardLayout[] roundCards;
    }
}