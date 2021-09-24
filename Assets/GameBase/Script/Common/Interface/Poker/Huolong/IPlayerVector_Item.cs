namespace GameBase.Common.Interface.Poker.Huolong
{
    public interface IPlayerVector_Item : Poker.IPlayerVector_Item
    {
        public Core.Poker.Huolong.GameSetting GameSetting { get; }
        public Model.Poker.Huolong.IModel Model { get; }
        public bool Response(Core.Poker.Huolong.GameNoticeResponse response);
        public bool Operate<T>(Core.Poker.Huolong.GameOperationEvent _event, T data) where T : class;
    }
}