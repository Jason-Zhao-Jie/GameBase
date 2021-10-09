
namespace GameBase.Common.Interface
{
    /// <summary>
    /// 玩家外壳(PlayerVector)对总线控制器(GameMain)的接口
    /// 玩家外壳(PlayerVector)是用于连接控制器(Controller)和玩家处理器(PlayerItem)之间的中转，负责处理与玩家类型无关的逻辑，例如消息发送、玩家流程控制
    /// </summary>
    public interface IPlayerVector<T_PlayerItem, T_PlayerVector_Item> where T_PlayerItem : IPlayerItem<T_PlayerVector_Item> where T_PlayerVector_Item:IPlayerVector_Item
    {
        public Core.CharacterInfo PlayerInfo { get; set; }
        void SetPlayerItem(T_PlayerItem playerItem);
    }
}
