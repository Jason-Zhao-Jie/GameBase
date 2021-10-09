namespace GameBase.Common.Interface
{
    /// <summary>
    /// 游戏玩家处理器(PlayerItem)对玩家外壳(PlayerVector)的接口
    /// 游戏玩家处理器(PlayerItem)里存放的是玩家处理游戏事件的实际逻辑，包括本地用户操作反馈、网络用户的协议处理，以及AI用户的策略处理
    /// </summary>
    public interface IPlayerItem
    {
        public Core.GameType GameType { get; }
        public Core.PlayerType PlayerType { get; }
        public void OnDispose();
    }
    public interface IPlayerItem<T_PlayerVector>: IPlayerItem where T_PlayerVector : IPlayerVector_Item
    {
        void SetVector(T_PlayerVector vector);
    }
}