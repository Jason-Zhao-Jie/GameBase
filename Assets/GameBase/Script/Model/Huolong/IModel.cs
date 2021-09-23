using System.Linq;
using GameBase.Core.Poker;
using GameBase.Core.Poker.Huolong;

namespace GameBase.Model.Huolong
{
    public interface IModel
    {
        #region 游戏状态

        public GameState GameState { get; }
        public MatchState MatchState { get; }

        #endregion 游戏状态

        #region 对局数据

        public CardColor MainColor { get; }
        public int MatchIndex { get; }
        public int RoundIndex { get; }
        public int CurrentCamp { get; }
        public int MainPoint { get; }
        public int OftenMainPoint { get; }
        public int MainPlayer { get; }

        #endregion 对局数据

        #region 回合状态

        public int LeadPlayer { get; }
        public int CurrentPlayer { get; }
        public int ShowingPlayer { get; }
        public int ShowedPlayer { get; }
        public int MainCardsCount { get; }
        public int ThrowNeedNum { get; }

        #endregion 回合状态

        public int GetCampLevel(int camp);
        public int GetCampScore(int camp);
    }
}
