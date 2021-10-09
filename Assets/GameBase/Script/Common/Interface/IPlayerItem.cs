namespace GameBase.Common.Interface
{
    /// <summary>
    /// ��Ϸ��Ҵ�����(PlayerItem)��������(PlayerVector)�Ľӿ�
    /// ��Ϸ��Ҵ�����(PlayerItem)���ŵ�����Ҵ�����Ϸ�¼���ʵ���߼������������û����������������û���Э�鴦���Լ�AI�û��Ĳ��Դ���
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