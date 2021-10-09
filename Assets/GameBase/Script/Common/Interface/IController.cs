using GameBase.Common.Core;

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
        public GameType GameType { get; }
        public void OnDispose();
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }

    public interface IController<T_PlayerVector_Controller, T_GameSetting>: IController where T_PlayerVector_Controller:IPlayerVector_Controller
    {
        public T_GameSetting GameSetting { get; }
        public bool SetPlayer(int index, T_PlayerVector_Controller player);
        public bool StartGame(T_GameSetting setting);
    }

}