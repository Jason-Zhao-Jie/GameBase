using GameBase.Common.Core;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// ������(PlayerVector)����Ϸ�淨������(Controller)�Ľӿ�
    /// ������(PlayerVector)���������ӿ�����(Controller)����Ҵ�����(PlayerItem)֮�����ת������������������޹ص��߼���������Ϣ���͡�������̿���
    /// </summary>
    public interface IPlayerVector_Controller
    {
        public Core.GameType GameType { get; }
        public CharacterInfo PlayerInfo { get; }
        public int PlayerIndex { get; set; }
        public void OnDispose();
    }
}