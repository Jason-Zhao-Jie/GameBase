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
            // ��ʼ������
            setting = vector.GameSetting;
            // ��ʼ����Ϸ���ݵ���ʾ
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
            // ��֪ Controller ��׼������
            vector.Response(GameNoticeResponse.StartGame_Ready);
        }

        public void OnMatchStart()
        {
            myCards = new CardLayout();
            // �������
            PokerWorld.ClearCards(WorldPokerManager.CardType.MyHandCard);
            // ���½���
            gameStateZone.SetMatchIndex(Model.MatchIndex);
            gameStateZone.SetMainPoint(Model.MainPoint);
            gameStateZone.SetState("��һ��׼����");
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
            // ��֪ Controller ��׼������
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
                tipZone.ShowTip("", ("��ׯ", () =>
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
                tipZone.ShowTip("����ׯ��, �����", ("���", () =>
                {
                    var cards = PokerWorld.GetSelectMyCards();
                    if (cards.Length != setting.lastCardsNum)
                    {
                        MainScene.Instance.ShowTips(string.Format("����������󣬱�����{0}�ŵ��ƣ����飡", setting.lastCardsNum));
                        return false;
                    }
                    else
                    {
                        vector.Operate(GameOperationEvent.LastCardsThrow, cards);
                        return true;
                    }
                }
                ), ("����", () =>
                {
                    PokerWorld.SetSelectMyCards();
                    return false;
                }
                ));
            }
            else
            {
                tipZone.ShowTip("", ("ժ��", () =>
                {
                    var cards = PokerWorld.GetSelectMyCards();
                    if (cards.Length <= 0)
                    {
                        MainScene.Instance.ShowTips("��ѡ��Ҫժ�ǵ����ƣ�");
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
                            MainScene.Instance.ShowTips("ժ�Ǳ���ѡ�����ƣ�");
                            return false;
                        }
                        else if (onlyJoker2)
                        {
                            MainScene.Instance.ShowTips("ժ�Ǳ����������1�Ŵ����ƣ�");
                            return false;
                        }
                        else
                        {
                            vector.Operate(GameOperationEvent.LastCardsThrow, cards);
                            return true;
                        }
                    }
                }
                ), ("��ժ��", ()=> {
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
            tipZone.ShowTip("������׺�ժ�ǵĽ������ȷ�ϣ�", ("ȷ��", () =>
            {
                PokerWorld.ClearCards(WorldPokerManager.CardType.CenterCards);
                vector.Response(GameNoticeResponse.PainLastCards_Confirm);
                gameStateZone.SetState("������");
                myStateZone.SetInGame(true);
                return true;
            }
            ));
            UpdateMyState();
        }

        public void OnAskForThrow(int[] leaderCards)
        {
            gameStateZone.SetState("��������");
            if (leaderCards == null || leaderCards.Length == 0)
            {
                tipZone.ShowTip("", ("����", () =>
                {
                    var selected = PokerWorld.GetSelectMyCards();
                    if (selected == null)
                    {
                        MainScene.Instance.ShowTips("��ѡ��Ҫ�������!");
                        return false;
                    }
                    var card = PokerHelper.GetColorPoint(selected[0]);
                    foreach(var c in selected)
                    {
                        if (PokerHelper.GetColorPoint(c) != card)
                        {
                            MainScene.Instance.ShowTips("�׼�ֻ�ܳ����ƻ���ͬ�Ķ����ƣ�");
                            return false;
                        }
                    }
                    vector.Operate(GameOperationEvent.CardsThrew, selected);
                    return true;
                }
                ), ("����", () =>
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
                tipZone.ShowTip("", ("����", () =>
                {
                    var selected = PokerWorld.GetSelectMyCards();
                    if (selected == null || selected.Length != num)
                    {
                        MainScene.Instance.ShowTips(string.Format("�����{0}���ƣ����飡", num));
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
                                MainScene.Instance.ShowTips("����������");
                                return false;
                            }
                            else
                            {
                                MainScene.Instance.ShowTips("������ȴ�����е�����!");
                                return false;
                            }
                        }
                        else
                        {
                            if (suitCards.Length >= num)
                            {
                                MainScene.Instance.ShowTips(string.Format("������{0}", PokerHelper.ColorToString(color)));
                                return false;
                            }
                            else
                            {
                                MainScene.Instance.ShowTips(string.Format("������ȴ�����е�{0}", PokerHelper.ColorToString(color)));
                                return false;
                            }
                        }
                    }
                    vector.Operate(GameOperationEvent.CardsThrew, selected);
                    return true;
                }
                ), ("����", () =>
                 {
                     PokerWorld.SetSelectMyCards();
                     return false;
                 }
                ));
            }
        }

        public void OnPlayerThrew(int player, int[] threw)
        {
            gameStateZone.SetState("������");
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
            MainScene.Instance.ShowMessageBox("�������ƣ��������·��ƣ�", "ȷ��", () =>
            {
                vector.Response(GameNoticeResponse.MatchAborted_NobodyShowed_Confirm);
                return true;
            });
        }

        public void OnGameAborted()
        {
            gameStateZone.SetState("��Ϸ����ֹ");
            MainScene.Instance.ShowMessageBox("��Ϸ����ֹ��", "ȷ��", () =>
            {
                MainScene.Instance.ShowTips("�˻������湦�����ڿ�����");
                return false;
            });
        }

        public void OnRoundOver(RoundReport report)
        {
            gameStateZone.SetState("�غϽ���");
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
                // ����
                PokerWorld.ClearCards(WorldPokerManager.CardType.ThrownCards);
                vector.Response(GameNoticeResponse.Round_Confirm);
            });
        }

        public void OnMatchOver(MatchReport report)
        {
            // todo �������ֽ���
        }

        public void OnGameOver(GameReport report)
        {
            // todo ������Ϸ����
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