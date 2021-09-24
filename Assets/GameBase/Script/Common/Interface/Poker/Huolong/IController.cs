using GameBase.Common.Core.Poker.Huolong;

namespace GameBase.Common.Interface.Poker.Huolong
{
    public interface IController : IController<IPlayerVector_Controller, GameSetting>
    {
        public Model.Poker.Huolong.IModel Model { get; }
        public bool Response(int player, GameNoticeResponse response);
        public bool Operate<T>(int player, GameOperationEvent _event, T data);
    }
}