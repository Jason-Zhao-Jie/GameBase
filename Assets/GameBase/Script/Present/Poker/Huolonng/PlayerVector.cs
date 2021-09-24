using GameBase.Common.Core;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Model.Poker.Huolong;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameBase.Present.Poker.Huolong
{
    public class PlayerVector : IPlayerVector_Controller, IPlayerVector_Item
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

        public void SetPlayerItem(IPlayerItem playerItem)
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
                // todo 这里要读取系统设定或者系统常量
                await Task.Delay(GameSetting.aroundOverDelay);
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
                                playerItem.OnPlayerInfoChanged((msg.data[0] as int[])[0], msg.data[1] as CharacterInfo, msg.data[0] as CharacterInfo);
                                break;
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

        private IController controller = null;
        private IPlayerItem playerItem = null;
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