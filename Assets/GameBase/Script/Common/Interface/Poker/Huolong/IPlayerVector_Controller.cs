using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker.Huolong
{
    public interface IPlayerVector_Controller:Poker.IPlayerVector_Controller
    {
        public bool SetController(IController controller);
        public void Notice<T>(Core.Poker.Huolong.GameNoticeEvent _event, T data);
        public void ResponseOperation(Core.Poker.Huolong.GameOperationEvent _event, Core.Poker.Huolong.GameOperationResponse response);
    }
}