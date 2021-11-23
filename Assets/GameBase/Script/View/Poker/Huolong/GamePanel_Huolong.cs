using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using CardLayout = GameBase.Common.Core.Poker.Huolong.CardLayout;

namespace GameBase.View.Poker.Huolong
{
    public class GamePanel_Huolong : APokerGamePanel, IPlayerItem<IPlayerVector_Item>
    {
        #region Inspector Fields

        public GameStateZone_Huolong gameStateZone;
        public MenuZone_Huolong menuZone;
        public MyStateZone_Huolong myStateZone;
        public TipZone_Huolong tipZone;
        public PlayerHead_Huolong[] heads;

        #endregion Inspector Fields

        #region Properties

        public WorldPokerManager PokerWorld
        {
            get;
            set;
        }

        public Common.Model.Poker.Huolong.IModel Model => vector.Model;

        public int PlayerIndex => vector.PlayerIndex;

        #endregion Properties

        #region Intefaces

        public override int GameSubType => (int)Common.Core.Poker.GameSubType.Huolong;

        GameSubType Common.Interface.Poker.IPlayerItem<IPlayerVector_Item>.GameSubType => Common.Core.Poker.GameSubType.Huolong;

        public void SetVector(IPlayerVector_Item vector)
        {
            this.vector = vector;
        }

        public void OnResponse(GameOperationEvent _event, GameOperationResponse response)
        {
            Common.PlatformInterface.Base.DebugError("Unknown Operation Response received by Huolong local player, player:" + PlayerIndex + ", operation code:" + (int)_event + ", response code:" + (int)response);
        }

        public void OnGameStart()
        {
            // 初始化数据
            setting = vector.GameSetting;
            // 初始化游戏内容的显示
            gameStateZone.InitNewGame(setting.startLevel);
            myStateZone.gameObject.SetActive(false);
            tipZone.HideTip();
            foreach(var h in heads)
            {
                h.gameObject.SetActive(false);
            }
            int playerNum = setting.playerNum;
            int campNum = setting.campNum;
            for (int i = 0; i < playerNum; ++i)
            {
                int absolutlyIndex = GetPlayerAbsolutlyIndex(i);
                int campIndex = absolutlyIndex % campNum;
                int headIndex = headIndexes[playerNum][i];
                var head = heads[headIndex];
                head.gameObject.SetActive(true);
                head.Info = vector.GetPlayerInfo(absolutlyIndex);
                head.IsMain = false;
                head.SetCampIndex(campIndex);
                head.SetLevel(Model.GetCampLevel(campIndex));
                head.SetScore(Model.GetCampScore(campIndex));
            }
            PokerWorld.Init(playerNum);
            // 告知 Controller 已准备就绪
            vector.Response(GameNoticeResponse.StartGame_Ready);
        }

        public void OnMatchStart()
        {
            // 清理界面
            PokerWorld.ClearCards(WorldPokerManager.CardType.MyHandCard);
            // 更新界面
            gameStateZone.SetMatchIndex(Model.MatchIndex);
            gameStateZone.SetMainPoint(Model.MainPoint);
            gameStateZone.SetState("新一局准备中");
            myStateZone.gameObject.SetActive(false);
            tipZone.HideTip();
            int playerNum = setting.playerNum;
            int campNum = setting.campNum;
            for (int i = 0; i < playerNum; ++i)
            {
                int absolutlyIndex = GetPlayerAbsolutlyIndex(i);
                int campIndex = absolutlyIndex % campNum;
                int headIndex = headIndexes[playerNum][i];
                var head = heads[headIndex];
                head.IsMain = Model.MainPlayer == absolutlyIndex;
                head.SetLevel(Model.GetCampLevel(campIndex));
                head.SetScore(Model.GetCampScore(campIndex));
            }
            PokerWorld.Init(playerNum);
            // 告知 Controller 已准备就绪
            vector.Response(GameNoticeResponse.StartMatch_Ready);
        }

        public void OnGetOneCard(int card)
        {
            PokerWorld.AddMyCards(card);
            var (showAbleCards, showAbleColor) = vector.CheckShowAble(new CardLayout(PokerWorld.MyPokerCards.CardList));
            if (showAbleCards != null && showAbleColor != CardColor.Unknown)
            {
                tipZone.ShowTip("", ("抢庄", () =>
                {
                    vector.Operate(GameOperationEvent.ShowStar, showAbleCards);
                    return true;
                }
                ));
            }
            else
            {
                tipZone.HideTip();
            }
        }

        public void OnPlayerShow(int player, int[] cards)
        {
            int relativeIndex = GetPlayerRelativeIndex(player);
            PokerWorld.ClearCards(WorldPokerManager.CardType.ThrownCards);
            PokerWorld.AddThrownCards(relativeIndex, cards);
            int headIndex = headIndexes[setting.playerNum][relativeIndex];
            var head = heads[headIndex];
            head.IsMain = Model.MainPlayer == player;
            gameStateZone.SetMainColor(Model.MainColor);
            gameStateZone.SetMainPoint(Model.MainPoint);
        }

        public void OnPlayerShowResult(int player, int[] jokers, int target)
        {
            int relativeIndex = GetPlayerRelativeIndex(player);
            PokerWorld.ClearCards(WorldPokerManager.CardType.ThrownCards);
            PokerWorld.AddThrownCards(relativeIndex, jokers, target);
            int headIndex = headIndexes[setting.playerNum][relativeIndex];
            var head = heads[headIndex];
            head.IsMain = Model.MainPlayer == player;
            gameStateZone.SetMainColor(Model.MainColor);
            gameStateZone.SetMainPoint(Model.MainPoint);
        }

        public void OnGetAllCards(int[] cards)
        {
            PokerWorld.AddMyCards(cards);
        }

        public void OnAskForLastCards(int[] mainPlayerLastCards)
        {
            PokerWorld.AddCenterCards(mainPlayerLastCards);
            if(Model.MainPlayer == PlayerIndex)
            {
                PokerWorld.AddMyCards(mainPlayerLastCards);
                PokerWorld.SetSelectMyCards(mainPlayerLastCards);
                // todo 询问用户埋底
            }
            else
            {
                // todo 询问用户摘星
            }
        }

        public void OnLastCardsOver(LastCardsReport report)
        {
        }

        public void OnAskForThrow(int[] leaderCards)
        {

        }

        public void OnPlayerThrew(int player, int[] threw)
        {

        }

        public void OnMatchAborted()
        {

        }

        public void OnGameAborted()
        {

        }

        public void OnRoundOver(RoundReport report)
        {

        }

        public void OnMatchOver(MatchReport report)
        {

        }

        public void OnGameOver(GameReport report)
        {

        }

        public void OnPlayerInfoChanged(int report, Common.Core.CharacterInfo oldInfo, Common.Core.CharacterInfo newInfo)
        {

        }

        #endregion Interfaces

        #region View Callbacks



        #endregion View Callbacks

        #region Unity Events



        #endregion Unity Events

        private int GetPlayerRelativeIndex(int absolutlyIndex)
        {
            if (absolutlyIndex < PlayerIndex)
            {
                return absolutlyIndex + setting.playerNum - PlayerIndex;
            }
            else
            {
                return absolutlyIndex - PlayerIndex;
            }
        }

        private int GetPlayerAbsolutlyIndex(int relativeIndex)
        {
            int absolutlyIndex = relativeIndex + PlayerIndex;
            if (absolutlyIndex >= setting.playerNum)
            {
                absolutlyIndex -= setting.playerNum;
            }
            return absolutlyIndex;
        }

        private IPlayerVector_Item vector;
        private GameSetting setting;

        private static readonly int[][] headIndexes = new int[][]
        {
            new int[]{},
            new int[]{0},
            new int[]{0, 3},
            new int[]{0, 1, 5},
            new int[]{0, 1, 3, 5},
            new int[]{0, 1, 2, 4, 5},
            new int[]{0, 1, 2, 3, 4, 5},
        };
    }
}