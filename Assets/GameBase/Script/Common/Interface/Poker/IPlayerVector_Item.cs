namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerVector_Item : Interface.IPlayerVector_Item
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}