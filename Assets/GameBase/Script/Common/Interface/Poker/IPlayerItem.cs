namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerItem<T_PlayerVector>:Interface.IPlayerItem<T_PlayerVector> where T_PlayerVector:IPlayerVector_Item
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}