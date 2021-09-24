using GameBase.Common.Core;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// ��Ϸ�淨������(Controller)�Ľӿ�
    /// ��Ϸ�淨������(Controller)�︺�������Ϸ������㷨�Լ������̽��п���
    /// ��Ϸ�淨������(Controller)ͨ������ IModel �������淨�����㷨, �������Խ��м���
    /// �����ڲ�ͬ��Ϸ�淨�����̲�ͬ, �����Ȼ��Ҫÿ���淨����������Ϸ�淨������(Controller)
    /// </summary>
    public interface IController<T_PlayerVector_Controller, T_GameSetting> where T_PlayerVector_Controller:IPlayerVector_Controller
    {
        public GameType GameType { get; }
        public T_GameSetting GameSetting { get; }
        public bool SetPlayer(int index, T_PlayerVector_Controller player);
        public bool StartGame(T_GameSetting setting);
        public void OnDispose();
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }

}