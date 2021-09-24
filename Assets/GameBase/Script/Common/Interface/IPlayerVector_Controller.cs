using GameBase.Common.Core;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// 玩家外壳(PlayerVector)对游戏玩法控制器(Controller)的接口
    /// 玩家外壳(PlayerVector)是用于连接控制器(Controller)和玩家处理器(PlayerItem)之间的中转，负责处理与玩家类型无关的逻辑，例如消息发送、玩家流程控制
    /// </summary>
    public interface IPlayerVector_Controller
    {
        public Core.GameType GameType { get; }
        public CharacterInfo PlayerInfo { get; }
        public int PlayerIndex { get; set; }
        public void OnDispose();
    }
}