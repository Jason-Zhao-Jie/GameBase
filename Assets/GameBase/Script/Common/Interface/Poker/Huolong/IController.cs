using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker.Huolong
{
    public interface IController : Poker.IController
    {
        public Core.Poker.Huolong.GameSetting GameSetting { get; }
        public Model.Poker.Huolong.IModel Model { get; }
        public bool Response(int player, Core.Poker.Huolong.GameNoticeResponse response);
        public bool Operate<T>(int player, Core.Poker.Huolong.GameOperationEvent _event, T data);
    }
}