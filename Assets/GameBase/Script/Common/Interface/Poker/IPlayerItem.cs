namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerItem:Interface.IPlayerItem
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}