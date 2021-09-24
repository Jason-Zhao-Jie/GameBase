namespace GameBase.Common.Interface.Poker
{
    public interface IController<T_PlayerVector_Controller, T_GameSetting> : Interface.IController<T_PlayerVector_Controller, T_GameSetting> where T_PlayerVector_Controller : IPlayerVector_Controller
    {
        Core.Poker.GameSubType GameSubType { get; }
    }

}