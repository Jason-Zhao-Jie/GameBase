using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;
using CardLayout = GameBase.Common.Core.Poker.Huolong.CardLayout;
using PokerHelper = GameBase.Common.Core.Poker.Helper;
using HuolongHelper = GameBase.Common.Core.Poker.Huolong.Helper;
using System.Threading.Tasks;

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

        public WorldPokerManager PokerWorld { get; set; }

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
            switch (_event)
            {
                case GameOperationEvent.ShowStar:
                case GameOperationEvent.LastCardsThrow:
                case GameOperationEvent.CardsThrew:
                    break;
                default:
                    Common.PlatformInterface.Base.DebugError("Unknown Operation Response received by Huolong local player, player:" + PlayerIndex + ", operation code:" + (int)_event + ", response code:" + (int)response);
                    break;
            }
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
            myCards = new CardLayout();
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
            UpdatePokerWorldSortFunc();
            // 告知 Controller 已准备就绪
            vector.Response(GameNoticeResponse.StartMatch_Ready);
        }

        public void OnGetOneCard(int card)
        {
            PokerWorld.AddMyCards(card);
            myCards.PushCard(card);
            myStateZone.gameObject.SetActive(true);
            myStateZone.SetInGame(false);
            myStateZone.SetMainNum(myCards.GetMainCount(Model.MainColor, Model.MainPoint, Model.OftenMainPoint));
            var (showAbleCards, showAbleColor) = vector.CheckShowAble(myCards);
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
            UpdateMyState();
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
            UpdatePokerWorldSortFunc();
        }

        public void OnPlayerShowResult(int player, int[] cards)
        {
            int relativeIndex = GetPlayerRelativeIndex(player);
            PokerWorld.ClearCards(WorldPokerManager.CardType.ThrownCards);
            PokerWorld.AddThrownCards(relativeIndex, cards);
            int headIndex = headIndexes[setting.playerNum][relativeIndex];
            var head = heads[headIndex];
            head.IsMain = Model.MainPlayer == player;
            gameStateZone.SetMainColor(Model.MainColor);
            gameStateZone.SetMainPoint(Model.MainPoint);
            UpdatePokerWorldSortFunc();
        }

        public void OnGetAllCards(int[] cards)
        {
            PokerWorld.AddMyCards(cards);
            myCards.PushCard(cards);
            myStateZone.gameObject.SetActive(true);
            myStateZone.SetInGame(false);
            UpdateMyState();
        }

        public void OnAskForLastCards(int[] mainPlayerLastCards)
        {
            PokerWorld.AddCenterCards(mainPlayerLastCards);
            if(Model.MainPlayer == PlayerIndex)
            {
                PokerWorld.AddMyCards(mainPlayerLastCards);
                myCards.PushCard(mainPlayerLastCards);
                PokerWorld.SetSelectMyCards(mainPlayerLastCards);
                tipZone.ShowTip("您是庄家, 请埋底", ("埋底", () =>
                {
                    var cards = PokerWorld.GetSelectMyCards();
                    if (cards.Length != setting.lastCardsNum)
                    {
                        MainScene.Instance.ShowTips(string.Format("埋底数量错误，必须是{0}张底牌，请检查！", setting.lastCardsNum));
                        return false;
                    }
                    else
                    {
                        vector.Operate(GameOperationEvent.LastCardsThrow, cards);
                        return true;
                    }
                }
                ), ("重置", () =>
                {
                    PokerWorld.SetSelectMyCards();
                    return false;
                }
                ));
            }
            else
            {
                tipZone.ShowTip("", ("摘星", () =>
                {
                    var cards = PokerWorld.GetSelectMyCards();
                    if (cards.Length <= 0)
                    {
                        MainScene.Instance.ShowTips("请选择要摘星的王牌！");
                        return false;
                    }
                    else
                    {
                        bool notJoker = false;
                        bool onlyJoker2 = true;
                        foreach (var c in cards)
                        {
                            if(PokerHelper.GetColor(c)!= CardColor.Joker)
                            {
                                notJoker = true;
                                break;
                            }
                            else if(PokerHelper.GetPoint(c) == 1)
                            {
                                onlyJoker2 = false;
                            }
                        }
                        if (notJoker)
                        {
                            MainScene.Instance.ShowTips("摘星必须选择王牌！");
                            return false;
                        }
                        else if (onlyJoker2)
                        {
                            MainScene.Instance.ShowTips("摘星必须包含至少1张大王牌！");
                            return false;
                        }
                        else
                        {
                            vector.Operate(GameOperationEvent.LastCardsThrow, cards);
                            return true;
                        }
                    }
                }
                ), ("不摘星", ()=> {
                    vector.Operate(GameOperationEvent.LastCardsThrow, new int[0]);
                    return false;
                }
                ));
            }
            UpdateMyState();
        }

        public void OnLastCardsOver(LastCardsReport report)
        {
            PokerWorld.RemoveMyCards(report.pain[PlayerIndex]);
            myCards.RemoveCards(report.pain[PlayerIndex]);
            PokerWorld.AddMyCards(report.gain[PlayerIndex]);
            myCards.PushCard(report.gain[PlayerIndex]);
            PokerWorld.SetSelectMyCards(report.gain[PlayerIndex]);
            PokerWorld.ClearCards(WorldPokerManager.CardType.CenterCards);
            PokerWorld.AddCenterCards(report.lastCards);
            for(int i = 0; i < setting.playerNum; ++i)
            {
                PokerWorld.AddThrownCards(GetPlayerRelativeIndex(i), report.pain[i], report.gain[i]);
            }
            tipZone.ShowTip("这是埋底和摘星的结果，请确认！", ("确认", () =>
            {
                PokerWorld.ClearCards(WorldPokerManager.CardType.CenterCards);
                vector.Response(GameNoticeResponse.PainLastCards_Confirm);
                gameStateZone.SetState("行牌中");
                myStateZone.SetInGame(true);
                return true;
            }
            ));
            UpdateMyState();
        }

        public void OnAskForThrow(int[] leaderCards)
        {
            gameStateZone.SetState("请您出牌");
            if (leaderCards == null || leaderCards.Length == 0)
            {
                tipZone.ShowTip("", ("出牌", () =>
                {
                    var selected = PokerWorld.GetSelectMyCards();
                    if (selected == null)
                    {
                        MainScene.Instance.ShowTips("请选择要打出的牌!");
                        return false;
                    }
                    var card = PokerHelper.GetColorPoint(selected[0]);
                    foreach(var c in selected)
                    {
                        if (PokerHelper.GetColorPoint(c) != card)
                        {
                            MainScene.Instance.ShowTips("首家只能出单牌或相同的多张牌！");
                            return false;
                        }
                    }
                    vector.Operate(GameOperationEvent.CardsThrew, selected);
                    return true;
                }
                ), ("重置", () =>
                {
                    PokerWorld.SetSelectMyCards();
                    return false;
                }
                ));
            }
            else
            {
                var num = leaderCards.Length;
                var color = PokerHelper.GetColor(leaderCards[0]);
                var isMain = HuolongHelper.GetIsMain(leaderCards[0], Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
                var suitCards = myCards.GetSuitableCards(leaderCards, Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
                tipZone.ShowTip("", ("出牌", () =>
                {
                    var selected = PokerWorld.GetSelectMyCards();
                    if (selected == null || selected.Length != num)
                    {
                        MainScene.Instance.ShowTips(string.Format("必须出{0}张牌，请检查！", num));
                        return false;
                    }
                    int numColor = 0;
                    foreach (var c in selected)
                    {
                        if (HuolongHelper.GetIsMain(c, Model.MainColor, Model.MainPoint, Model.OftenMainPoint))
                        {
                            if (isMain)
                            {
                                numColor++;
                            }
                        }
                        else
                        {
                            if (!isMain && PokerHelper.GetColor(c) == color)
                            {
                                numColor++;
                            }
                        }
                    }
                    if (suitCards.Length > numColor && num > numColor)
                    {
                        if (isMain)
                        {
                            if (suitCards.Length >= num)
                            {
                                MainScene.Instance.ShowTips("你必须出主牌");
                                return false;
                            }
                            else
                            {
                                MainScene.Instance.ShowTips("你必须先打出所有的主牌!");
                                return false;
                            }
                        }
                        else
                        {
                            if (suitCards.Length >= num)
                            {
                                MainScene.Instance.ShowTips(string.Format("你必须出{0}", PokerHelper.ColorToString(color)));
                                return false;
                            }
                            else
                            {
                                MainScene.Instance.ShowTips(string.Format("你必须先打出所有的{0}", PokerHelper.ColorToString(color)));
                                return false;
                            }
                        }
                    }
                    vector.Operate(GameOperationEvent.CardsThrew, selected);
                    return true;
                }
                ), ("重置", () =>
                 {
                     PokerWorld.SetSelectMyCards();
                     return false;
                 }
                ));
            }
        }

        public void OnPlayerThrew(int player, int[] threw)
        {
            gameStateZone.SetState("行牌中");
            PokerWorld.AddThrownCards(GetPlayerRelativeIndex(player), threw);
            if(player == PlayerIndex)
            {
                PokerWorld.RemoveMyCards(threw);
                myCards.RemoveCards(threw);
            }
            UpdateMyState();
        }

        public void OnMatchAborted()
        {
            MainScene.Instance.ShowMessageBox("无人亮牌，本局重新发牌！", "确定", () =>
            {
                vector.Response(GameNoticeResponse.MatchAborted_NobodyShowed_Confirm);
                return true;
            });
        }

        public void OnGameAborted()
        {
            gameStateZone.SetState("游戏被中止");
            MainScene.Instance.ShowMessageBox("游戏被中止！", "确定", () =>
            {
                MainScene.Instance.ShowTips("退回主界面功能正在开发中");
                return false;
            });
        }

        public void OnRoundOver(RoundReport report)
        {
            gameStateZone.SetState("回合结算");
            tipZone.HideTip();
            int playerNum = setting.playerNum;
            int campNum = setting.campNum;
            for (int i = 0; i < playerNum; ++i)
            {
                int absolutlyIndex = GetPlayerAbsolutlyIndex(i);
                int campIndex = absolutlyIndex % campNum;
                int headIndex = headIndexes[playerNum][i];
                var head = heads[headIndex];
                head.SetScore(Model.GetCampScore(campIndex));
            }
            UpdateMyState();
            Task.Delay(setting.aroundOverDelay).ContinueWith((Task task) =>
            {
                // 清理
                PokerWorld.ClearCards(WorldPokerManager.CardType.ThrownCards);
                vector.Response(GameNoticeResponse.Round_Confirm);
            });
        }

        public void OnMatchOver(MatchReport report)
        {
            // todo 弹出单局结算
        }

        public void OnGameOver(GameReport report)
        {
            // todo 弹出游戏结算
        }

        public void OnPlayerInfoChanged(int index, Common.Core.CharacterInfo newInfo)
        {
            int relativeIndex = GetPlayerRelativeIndex(index);
            int headIndex = headIndexes[setting.playerNum][relativeIndex];
            var head = heads[headIndex];
            head.Info = vector.GetPlayerInfo(index);
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

        private void UpdateMyState()
        {
            myStateZone.SetMainNum(myCards.GetMainCount(Model.MainColor, Model.MainPoint, Model.OftenMainPoint));
            gameStateZone.SetCardsNum(myCards.Count);
            myStateZone.SetMainNum(myCards.GetMainCount(Model.MainColor, Model.MainPoint, Model.OftenMainPoint));
        }

        private void UpdatePokerWorldSortFunc()
        {
            PokerWorld.SetCardsSortFunc(WorldPokerManager.CardType.MyHandCard, (int a, int b) =>
            {
                return HuolongHelper.CompareAsHandCard(a, b, Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
            });
            PokerWorld.SetCardsSortFunc(WorldPokerManager.CardType.CenterCards, (int a, int b) =>
            {
                return HuolongHelper.CompareAsLastCards(a, b, Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
            });
            PokerWorld.SetCardsSortFunc(WorldPokerManager.CardType.ThrownCards, (int a, int b) =>
            {
                return HuolongHelper.CompareAsHandCard(a, b, Model.MainColor, Model.MainPoint, Model.OftenMainPoint);
            });
        }

        private IPlayerVector_Item vector;
        private GameSetting setting;
        private CardLayout myCards;

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