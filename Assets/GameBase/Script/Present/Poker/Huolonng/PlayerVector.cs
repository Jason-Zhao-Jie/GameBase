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
            if(controller.GameType != GameType || controller.GameSubType != GameSubType)
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

        public bool Operate<T>(GameOperationEvent _event, T data)
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

        public void Notice<T>(GameNoticeEvent _event, T data)
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
            // todo
            return 0;
        }

        private IController controller = null;
        private IPlayerItem playerItem = null;
        private System.Threading.CancellationTokenSource token;
        private Task<int> task = null;
        private Queue<MessageData> messages = null;

        private struct MessageData
        {
            public bool isNotice;
            public int code;
            public GameOperationResponse resp;
            public object data;
        }
    }
}