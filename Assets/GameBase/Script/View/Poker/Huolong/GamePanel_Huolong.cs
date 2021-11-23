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
            // ��֪ Controller ��׼������
            vector.Response(GameNoticeResponse.StartMatch_Ready);
        }

        public void OnGetOneCard(int card)
        {
            PokerWorld.AddMyCards(card);
            var (showAbleCards, showAbleColor) = vector.CheckShowAble(new CardLayout(PokerWorld.GetAllMyCards()));
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
            UpdateMyState();
        }

        public void OnAskForLastCards(int[] mainPlayerLastCards)
        {
            PokerWorld.AddCenterCards(mainPlayerLastCards);
            if(Model.MainPlayer == PlayerIndex)
            {
                PokerWorld.AddMyCards(mainPlayerLastCards);
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
                ));
            }
            else
            {
                tipZone.ShowTip("ׯ��������ף�������ѡ��ժ��", ("ժ��", () =>
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
                            if(Common.Core.Poker.Helper.GetColor(c)!= CardColor.Joker)
                            {
                                notJoker = true;
                                break;
                            }
                            else if(Common.Core.Poker.Helper.GetPoint(c) == 1)
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
                ), ("ȡ��", ()=> {
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
            PokerWorld.AddMyCards(report.gain[PlayerIndex]);
            PokerWorld.SetSelectMyCards(report.gain[PlayerIndex]);
            PokerWorld.ClearCards(WorldPokerManager.CardType.CenterCards);
            PokerWorld.AddCenterCards(report.lastCards);
            for(int i = 0; i < setting.playerNum; ++i)
            {
                PokerWorld.AddThrownCards(GetPlayerRelativeIndex(i), report.pain[i], report.gain[i]);
            }
            tipZone.ShowTip("����ׯ����׺�������ժ�ǵĽ������ȷ�ϣ�", ("ȷ��", () =>
            {
                vector.Response(GameNoticeResponse.PainLastCards_Confirm);
                return true;
            }
            ));
            UpdateMyState();
        }

        public void OnAskForThrow(int[] leaderCards)
        {
            // todo ��ʾ����,���ѡ�������
        }

        public void OnPlayerThrew(int player, int[] threw)
        {
            PokerWorld.AddThrownCards(GetPlayerRelativeIndex(player), threw);
            UpdateMyState();
        }

        public void OnMatchAborted()
        {
            // todo ����������ֹ����
        }

        public void OnGameAborted()
        {
            // todo ������Ϸ��ֹ
        }

        public void OnRoundOver(RoundReport report)
        {
            // todo ����غϽ�������½�����ʾ
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
            var myCards = PokerWorld.GetAllMyCards();
            myStateZone.SetMainNum(new CardLayout(myCards).GetMainCount(Model.MainColor, Model.MainPoint, Model.OftenMainPoint));
            gameStateZone.SetCardsNum(myCards.Length);
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