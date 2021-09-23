using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// 玩家外壳(PlayerVector)对玩家处理器(PlayerItem)的接口
    /// 玩家外壳(PlayerVector)是用于连接控制器(Controller)和玩家处理器(PlayerItem)之间的中转，负责处理与玩家类型无关的逻辑，例如消息发送、玩家流程控制
    /// </summary>
    public interface IPlayerVector_Item
    {
        public Core.GameType GameType { get; }
        public CharacterInfo PlayerInfo { get; }
        public int PlayerIndex { get; }
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }
}