using GameBase.Common.Core;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Model.Poker.Huolong;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameBase.Present.Poker.Huolong
{
    public class PlayerVector : IPlayerVector_Controller, IPlayerVector_Item, Common.Interface.IPlayerVector<IPlayerItem<IPlayerVector_Item>, IPlayerVector_Item>
    {

        public GameType GameType => GameType.Poker;
        public GameSubType GameSubType => GameSubType.Huolong;

        public GameSetting GameSetting => controller.GameSetting;
        public IModel Model => controller.Model;

        public CharacterInfo PlayerInfo { get; set; } = null;
        public int PlayerIndex { get; set; }
        public PlayerType PlayerType => playerItem.PlayerType;

        public PlayerVector()
        {
            messages = new Queue<MessageData>();
            token = new System.Threading.CancellationTokenSource();
            task = Update();
        }

        public void OnDispose()
        {
            playerItem.OnDispose();
            token.Cancel();
        }

        public CharacterInfo[] GetAllPlayersInfo()
        {
            return controller.GetAllPlayersInfo();
        }

        public CharacterInfo GetPlayerInfo(int player)
        {
            return controller.GetPlayerInfo(player);
        }

        public bool SetController(IController controller)
        {
            if (controller.GameType != GameType || controller.GameSubType != GameSubType)
            {
                return false;
            }
            this.controller = controller;
            return true;
        }

        public void SetPlayerItem(IPlayerItem<IPlayerVector_Item> playerItem)
        {
            this.playerItem = playerItem;
        }

        public bool Operate<T>(GameOperationEvent _event, T data) where T : class
        {
            return controller.Operate(PlayerIndex, _event, data);
        }

        public void ResponseOperation(GameOperationEvent _event, GameOperationResponse response)
        {
            messages.Enqueue(new MessageData
            {
                isNotice = false,
                code = System.Convert.ToInt32(_event),
                resp = response,
                data = null
            });
        }

        public void Notice<T>(GameNoticeEvent _event, params T[] data) where T : class
        {
            messages.Enqueue(new MessageData
            {
                isNotice = true,
                code = System.Convert.ToInt32(_event),
                resp = GameOperationResponse.Success,
                data = data
            });
        }

        public bool Response(GameNoticeResponse response)
        {
            return controller.Response(PlayerIndex, response);
        }

        private async Task<int> Update()
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(GameMain.Instance.SystemSettings.aiDelay);
                if (messages.Count > 0)
                {
                    var msg = messages.Dequeue();
                    if (msg.isNotice)
                    {
                        var code = (GameNoticeEvent)msg.code;
                        switch (code)
                        {
                            case GameNoticeEvent.StartGame:
                                playerItem.OnGameStart();
                                break;
                            case GameNoticeEvent.StartMatch:
                                playerItem.OnMatchStart();
                                break;
                            case GameNoticeEvent.GiveOneCard:
                                playerItem.OnGetOneCard((msg.data[0] as int[])[0]);
                                break;
                            case GameNoticeEvent.GiveAllCards:
                                playerItem.OnGetAllCards(msg.data[0] as int[]);
                                break;
                            case GameNoticeEvent.PlayerShowingStar:
                                playerItem.OnPlayerShow((msg.data[0] as int[])[0], msg.data[1] as int[]);
                                break;
                            case GameNoticeEvent.PlayerShowedStar:
                                playerItem.OnPlayerShowResult((msg.data[0] as int[])[0], msg.data[1] as int[], (msg.data[0] as int[])[2]);
                                break;
                            case GameNoticeEvent.MatchAborted_NobodyShowed:
                                playerItem.OnMatchAborted();
                                break;
                            case GameNoticeEvent.GainLastCards:
                                playerItem.OnAskForLastCards(msg.data[0] as int[]);
                                break;
                            case GameNoticeEvent.PainLastCards:
                                playerItem.OnLastCardsOver(msg.data[0] as LastCardsReport);
                                break;
                            case GameNoticeEvent.PlayerThrew:
                                playerItem.OnPlayerThrew((msg.data[0] as int[])[0], msg.data[1] as int[]);
                                break;
                            case GameNoticeEvent.AskForThrow:
                                playerItem.OnAskForThrow(msg.data[0] as int[]);
                                break;
                            case GameNoticeEvent.RoundReport:
                                playerItem.OnRoundOver(msg.data[0] as RoundReport);
                                break;
                            case GameNoticeEvent.MatchReport:
                                playerItem.OnMatchOver(msg.data[0] as MatchReport);
                                break;
                            case GameNoticeEvent.GameReport:
                                playerItem.OnGameOver(msg.data[0] as GameReport);
                                break;
                            case GameNoticeEvent.GameAborted_PlayerLeft:
                                playerItem.OnGameAborted();
                                break;
                            case GameNoticeEvent.PlayerInfoChanged:
                                {
                                    var index = (msg.data[0] as int[])[0];
                                    var info = msg.data[1] as CharacterInfo;
                                    if (index == PlayerIndex)
                                    {
                                        PlayerInfo = info;
                                    }
                                    playerItem.OnPlayerInfoChanged(index, info);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        playerItem.OnResponse((GameOperationEvent)msg.code, msg.resp);
                    }
                }
            }

            return 0;
        }

        public (int[] cards, CardColor color) CheckShowAble(Common.Core.Poker.Huolong.CardLayout myCards)
        {
            if (Model.AllowShow)
            {
                var min = Model.MinShowCardsNum;
                if (Model.IsShowMainPoint)
                {
                    var spades = myCards.GetColorPointCards(Common.Core.Poker.Helper.GetCardId(CardColor.Spades, Model.MainPoint));
                    var heart = myCards.GetColorPointCards(Common.Core.Poker.Helper.GetCardId(CardColor.Heart, Model.MainPoint));
                    var cube = myCards.GetColorPointCards(Common.Core.Poker.Helper.GetCardId(CardColor.Cube, Model.MainPoint));
                    var diamond = myCards.GetColorPointCards(Common.Core.Poker.Helper.GetCardId(CardColor.Diamond, Model.MainPoint));
                    if (spades.Length >= min && spades.Length >= heart.Length && spades.Length >= cube.Length && spades.Length >= diamond.Length)
                    {
                        return (spades, CardColor.Spades);
                    }
                    else if (heart.Length >= min && heart.Length >= spades.Length && heart.Length >= cube.Length && heart.Length >= diamond.Length)
                    {
                        return (heart, CardColor.Heart);
                    }
                    else if (cube.Length >= min && cube.Length >= spades.Length && cube.Length >= heart.Length && cube.Length >= diamond.Length)
                    {
                        return (cube, CardColor.Cube);
                    }
                    else if (diamond.Length >= min && diamond.Length >= spades.Length && diamond.Length >= heart.Length && diamond.Length >= cube.Length)
                    {
                        return (diamond, CardColor.Diamond);
                    }
                }
                else
                {
                    var joker = myCards.GetColorPointCards(Common.Core.Poker.Helper.Joker1);
                    if (joker.Length >= min)
                    {
                        return (joker, CardColor.Joker);
                    }
                }
            }
            return (null, CardColor.Unknown);
        }

        private IController controller = null;
        private IPlayerItem<IPlayerVector_Item> playerItem = null;
        private System.Threading.CancellationTokenSource token;
        private readonly Task<int> task = null;
        private readonly Queue<MessageData> messages = null;

        private struct MessageData
        {
            public bool isNotice;
            public int code;
            public GameOperationResponse resp;
            public object[] data;
        }
    }
}