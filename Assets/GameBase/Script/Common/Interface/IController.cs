using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// 游戏玩法控制器(Controller)的接口
    /// 游戏玩法控制器(Controller)里负责调度游戏规则的算法以及对流程进行控制
    /// 游戏玩法控制器(Controller)通过创建 IModel 来调度玩法规则算法, 而不亲自进行计算
    /// 但由于不同游戏玩法的流程不同, 因此仍然需要每个玩法单独创建游戏玩法控制器(Controller)
    /// </summary>
    public interface IController
    {
        public Core.GameType GameType { get; }
        public bool SetPlayer(int index, object player);
        public bool StartGame();
        public void OnDispose();
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }

}