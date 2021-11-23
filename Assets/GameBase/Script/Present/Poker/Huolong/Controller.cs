using GameBase.Common.Core;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Model.Poker.Huolong;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameBase.Present.Poker.Huolong
{
    public class Controller : IController
    {
        public GameType GameType => GameType.Poker;
        public GameSubType GameSubType => GameSubType.Huolong;

        public GameSetting GameSetting => model.Setting;
        public IModel Model => model;

        public Controller()
        {
            model = new Model();
            players = new Dictionary<int, IPlayerVector_Controller>();
            playerFlags = new List<bool>();
            waitingResponse = false;
            token = new System.Threading.CancellationTokenSource();
            GameMain.Instance.Listen(SystemEventType.OnPlayerInfoChanged, OnPlayerInfoChanged);
        }

        public CharacterInfo[] GetAllPlayersInfo()
        {
            var ret = new CharacterInfo[players.Count];
            for(int i = 0; i < ret.Length; ++i)
            {
                ret[i] = players[i].PlayerInfo;
            }
            return ret;
        }

        public CharacterInfo GetPlayerInfo(int player)
        {
            return players[player].PlayerInfo;
        }

        public bool SetPlayer(int index, IPlayerVector_Controller player)
        {
            if (players.ContainsKey(index) && players[index] != null)
            {
                players[index].OnDispose();
                players.Remove(index);
            }
            players.Add(index, player);
            player.PlayerIndex = index;
            player.SetController(this);
            return true;
        }

        public bool StartGame(GameSetting setting)
        {
            for (int i = 0; i < setting.playerNum; ++i)
            {
                if (!players.ContainsKey(i) || players[i] == null)
                {
                    return false;
                }
            }
            model.InitGame(setting);
            for (int i = 0; i < model.Setting.playerNum; ++i)
            {
                playerFlags.Add(false);
            }
            messages = new Queue<MessageData>();
            gameover = false;
            task = Update();
            return true;
        }

        public void OnDispose()
        {
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].OnDispose();
            }
            token.Cancel();
        }

        public bool Operate<T>(int player, GameOperationEvent _event, T data) where T : class
        {
            if (messages == null || token == null || player < 0 || player >= GameSetting.playerNum || token.IsCancellationRequested || data as int[] == null)
            {
                players[player].ResponseOperation(_event, GameOperationResponse.CannotPushEvent);
                return false;
            }
            messages.Enqueue(new MessageData
            {
                isOperation = true,
                player = player,
                code = System.Convert.ToInt32(_event),
                data = data as int[]
            });
            return true;
        }

        public bool Response(int player, GameNoticeResponse response)
        {
            if (messages == null || token == null || !waitingResponse || waitingResponseType != response || player < 0 || player >= GameSetting.playerNum || token.IsCancellationRequested)
            {
                return false;
            }
            messages.Enqueue(new MessageData
            {
                isOperation = false,
                player = player,
                code = System.Convert.ToInt32(response),
                data = null
            });
            return true;
        }

        private async Task<int> Update()
        {
            // 检查玩家是否到齐
            for(int i = 0; i < model.Setting.playerNum; ++i)
            {
                if(!players.ContainsKey(i) || players[i] == null)
                {
                    return -1;
                }
            }
            // 通知玩家开始游戏
            waitingResponse = true;
            waitingResponseType = GameNoticeResponse.StartGame_Ready;
            for (int i = 0; i < model.Setting.playerNum; ++i)
            {
                players[i].Notice<object>(GameNoticeEvent.StartGame, null);
            }
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(GameSetting.aroundOverDelay);
                if (messages.Count > 0)
                {
                    var msg = messages.Dequeue();
                    if (msg.isOperation)
                    {
                        var type = (GameOperationEvent)msg.code;
                        switch (type)
                        {
                            case GameOperationEvent.ShowStar:
                                OnShowStar(msg.player, msg.data);
                                break;
                            case GameOperationEvent.LastCardsThrow:
                                OnThrewLastCards(msg.player, msg.data);
                                break;
                            case GameOperationEvent.CardsThrew:
                                OnThrew(msg.player, msg.data);
                                break;
                        }
                    }
                    else
                    {
                        var type = (GameNoticeResponse)msg.code;
                        switch (type)
                        {
                            case GameNoticeResponse.StartGame_Ready:
                                OnPlayerStartGameReady(msg.player);
                                break;
                            case GameNoticeResponse.StartMatch_Ready:
                                // todo: 注意这里的 Task 保存
                                OnPlayerStartMatchReady(msg.player);
                                break;
                            case GameNoticeResponse.MatchAborted_NobodyShowed_Confirm:
                                OnPlayerMatchAbortedConfirm(msg.player);
                                break;
                            case GameNoticeResponse.PainLastCards_Confirm:
                                OnPlayerPainLastCardsConfirm(msg.player);
                                break;
                            case GameNoticeResponse.Round_Confirm:
                                OnPlayerRoundConfirm(msg.player);
                                break;
                            case GameNoticeResponse.Match_Confirm:
                                OnPlayerMatchConfirm(msg.player);
                                break;
                            default:
                                Common.PlatformInterface.Base.DebugError("Unknown Notice Response received by Huolong Controller, player:" + msg.player + ", response code:" + msg.code);
                                break;
                        }
                    }
                }
            }

            return 0;
        }

        private void OnPlayerInfoChanged(object[] data)
        {
            var info = (CharacterInfo)data[0];
            int index = -1;
            for (int i = 0; i < model.Setting.playerNum; ++i)
            {
                if (players[i].PlayerInfo != null && players[i].PlayerInfo.id == info.id)
                {
                    index = i;
                }
            }
            if (index >= 0)
            {
                players[index].Notice<object>(GameNoticeEvent.PlayerInfoChanged, new int[] { index }, info);
                for (int i = 0; i < model.Setting.playerNum; ++i)
                {
                    if (i != index)
                    {
                        players[i].Notice<object>(GameNoticeEvent.PlayerInfoChanged, new int[] { index }, info);
                    }
                }
            }
        }

        private void ResetFlags()
        {
            for (int i = 0; i < model.Setting.playerNum; ++i)
            {
                playerFlags[i] = false;
            }
        }

        private bool CheckAllFlags()
        {
            for (int i = 0; i < model.Setting.playerNum; ++i)
            {
                if (!playerFlags[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void OnShowStar(int player, int[] cards)
        {
            var ret = model.SetShow(player, Common.Core.Poker.Helper.GetColor(cards[0]));
            players[player].ResponseOperation(GameOperationEvent.ShowStar, ret);
            if (ret == GameOperationResponse.Success)
            {
                for (int i = 0; i < model.Setting.playerNum; ++i)
                {
                    players[i].Notice(GameNoticeEvent.PlayerShowingStar, new int[] { player }, model.GetPlayerCardLayout(player).GetAll());
                }
            }
        }

        private void OnThrewLastCards(int player, int[] cards)
        {
            var ret = model.MakeOnesLastCards(player, cards);
            players[player].ResponseOperation(GameOperationEvent.LastCardsThrow, ret);
            if (ret == GameOperationResponse.Success)
            {
                playerFlags[player] = true;
                if (CheckAllFlags())
                {
                    waitingResponse = false;
                    ResetFlags();
                    var notice = model.MakeLastCardsReport();
                    waitingResponse = true;
                    waitingResponseType = GameNoticeResponse.PainLastCards_Confirm;
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice(GameNoticeEvent.PainLastCards, notice);
                    }
                }
            }
        }

        private void OnThrew(int player, int[] cards)
        {
            var ret = model.MakeOnesThrew(player, cards);
            players[player].ResponseOperation(GameOperationEvent.CardsThrew, ret);
            if (ret== GameOperationResponse.Success)
            {
                for (int i = 0; i < model.Setting.playerNum; ++i)
                {
                    players[i].Notice<int[]>(GameNoticeEvent.PlayerThrew, cards);
                }
                if (model.CurrentPlayer == model.LeadPlayer)
                {
                    var round = model.MakeRoundCalculate();
                    waitingResponse = true;
                    waitingResponseType = GameNoticeResponse.Round_Confirm;
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice(GameNoticeEvent.RoundReport, round);
                    }
                }
                else
                {
                    players[model.CurrentPlayer].Notice<int[]>(GameNoticeEvent.AskForThrow, model.GetPlayerCardLayout(model.LeadPlayer).GetAll());
                }
            }
        }

        private void OnPlayerStartGameReady(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                model.InitNewMatch(0);
                // 通知玩家开始新一局
                waitingResponse = true;
                waitingResponseType = GameNoticeResponse.StartMatch_Ready;
                for (int i = 0; i < model.Setting.playerNum; ++i)
                {
                    players[i].Notice<object>(GameNoticeEvent.StartMatch, null);
                }
            }
        }

        private async Task OnPlayerStartMatchReady(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                if (model.AllowShow)
                {
                    while (true)
                    {
                        var p = model.CurrentPlayer;
                        var card = model.SendOneCardToPlayer();
                        if(card == 0)
                        {
                            if (model.MainColor == CardColor.Unknown)
                            {
                                waitingResponse = true;
                                waitingResponseType = GameNoticeResponse.MatchAborted_NobodyShowed_Confirm;
                                for (int i = 0; i < model.Setting.playerNum; ++i)
                                {
                                    players[i].Notice<object>(GameNoticeEvent.MatchAborted_NobodyShowed, null);
                                }
                            }
                            else
                            {
                                var lastCards = model.SendCardsToLast();
                                model.SendLastCardToMainPlayer();
                                for (int i = 0; i < model.Setting.playerNum; ++i)
                                {
                                    players[i].Notice<int[]>(GameNoticeEvent.GainLastCards, lastCards);
                                }
                            }
                            break;
                        }
                        else
                        {
                            players[p].Notice<int[]>(GameNoticeEvent.GiveOneCard, new int[] { card });
                            if(!model.IsShowMainPoint && model.ShowingPlayer == p)
                            {
                                if(model.ShowJokerResult(p, card))
                                {
                                    if (model.ShowingPlayer == p)
                                    {
                                        for (int i = 0; i < model.Setting.playerNum; ++i)
                                        {
                                            players[i].Notice(GameNoticeEvent.PlayerShowingStar, new int[] { player }, model.GetPlayerCardLayout(player).GetAll());
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < model.Setting.playerNum; ++i)
                                        {
                                            players[i].Notice(GameNoticeEvent.PlayerShowedStar, new int[] { player }, model.GetPlayerCardLayout(player).GetAll(), new int[] { card });
                                        }
                                    }
                                }
                            }
                            await Task.Delay(GameSetting.firstRoundGiveCardsDelay);
                        }
                    }
                }
                else
                {
                    var lastCards = model.SendAllCardsOver();
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice<int[]>(GameNoticeEvent.GiveAllCards, model.GetPlayerCardLayout(i).GetAll());
                    }
                    await Task.Delay(GameSetting.firstRoundGiveCardsDelay);
                    model.SendLastCardToMainPlayer();
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice<int[]>(GameNoticeEvent.GainLastCards, lastCards);
                    }
                }
            }
        }

        private void OnPlayerMatchAbortedConfirm(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                model.InitNewMatch(model.MatchIndex);
                // 通知玩家开始新一局
                waitingResponse = true;
                waitingResponseType = GameNoticeResponse.StartMatch_Ready;
                for (int i = 0; i < model.Setting.playerNum; ++i)
                {
                    players[i].Notice<object>(GameNoticeEvent.StartMatch, null);
                }
            }
        }

        private void OnPlayerPainLastCardsConfirm(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                model.ClearRoundCards();
                players[model.CurrentPlayer].Notice<object>(GameNoticeEvent.AskForThrow, null);
            }
        }

        private void OnPlayerRoundConfirm(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                model.ClearRoundCards();
                if (model.GetPlayerCardLayout(model.MainPlayer).Count > 0)
                {
                    players[model.CurrentPlayer].Notice<object>(GameNoticeEvent.AskForThrow, null);
                }
                else
                {
                    var match = model.MakeMatchCalculate();
                    gameover = match.gameover;
                    waitingResponse = true;
                    waitingResponseType = GameNoticeResponse.Match_Confirm;
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice(GameNoticeEvent.MatchReport, match);
                    }
                }
            }
        }

        private async void OnPlayerMatchConfirm(int player)
        {
            playerFlags[player] = true;
            if (CheckAllFlags())
            {
                waitingResponse = false;
                ResetFlags();
                model.ClearRoundCards();
                if (gameover)
                {
                    var game = model.GetGameResult();
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice(GameNoticeEvent.GameReport, game);
                    }
                    token.Cancel();
                    await task;
                    task = null;
                    messages = null;
                    token = new System.Threading.CancellationTokenSource();
                }
                else
                {
                    model.InitNewMatch(model.MatchIndex + 1);
                    waitingResponse = true;
                    waitingResponseType = GameNoticeResponse.StartMatch_Ready;
                    for (int i = 0; i < model.Setting.playerNum; ++i)
                    {
                        players[i].Notice<object>(GameNoticeEvent.StartMatch, null);
                    }
                }
            }
        }

        private readonly Model model;
        private readonly Dictionary<int, IPlayerVector_Controller> players;
        private readonly List<bool> playerFlags;
        private bool waitingResponse;
        private GameNoticeResponse waitingResponseType;
        private System.Threading.CancellationTokenSource token;
        private Task<int> task = null;
        private Queue<MessageData> messages = null;
        private bool gameover = false;

        private struct MessageData
        {
            public bool isOperation;
            public int player;
            public int code;
            public int[] data;
        }
    }
}