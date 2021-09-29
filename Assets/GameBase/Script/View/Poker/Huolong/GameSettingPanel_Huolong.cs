using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker.Huolong
{
    public class GameSettingPanel_Huolong : APanel
    {
        #region Common UI Case

        public RectTransform[] panels;

        #endregion Common UI Case

        #region Panel 1 UI

        public Dropdown dropPlayerCampNum;
        public Dropdown dropGroupNum;
        public Dropdown dropLastCardsNum;
        public Dropdown dropStartLevel;
        public Dropdown dropEndLevel;
        public Toggle tgNeedJokerLevel;
        public Toggle tgConstantMain;
        public Text textConstantMain;
        public Text textComments;
        public Dropdown dropMainColorWay;
        public InputField inputForceLastCardsJokerNum;
        public Toggle tgDoubleLastCardsScore;
        public Dropdown dropWinWay;
        public InputField inputFullScore;

        #endregion Panel 1 UI

        #region Panel 2 UI

        public InputField inputNoLastCardsUpgrade;
        public InputField inputNoStarsUpgrade;
        public InputField inputNoHalfScoreUpgrade;
        public InputField inputZeroScoreUpgrade;
        public InputField inputStarBaseUpgrade;
        public InputField inputJoker1Upgrade;
        public InputField inputJoker2Upgrade;
        public Toggle[] tgThresholds;
        public Text[] textThresholds;

        #endregion Panel 2 UI

        #region Panel 3 UI

        public Text textDowngradeTitle;
        public Toggle tgJoker1Downgrade;
        public Toggle tgJoker2Downgrade;
        public Toggle tgMPDowngrade;
        public Text textMPDowngrade;
        public Toggle tgUMPDowngrade;
        public Text textUMPDowngrade;
        public Toggle tgMCDowngrade;
        public Text textMCDowngrade;
        public Toggle tgUMCDowngrade;
        public Text textUMCDowngrade;
        public Toggle tgRecord;
        public InputField inputGiveCardDelay;
        public InputField inputRoundDelay;

        #endregion Panel 3 UI

        #region Public Fields

        public override PanelType PanelType => PanelType.GameSubPanel;

        public Common.Core.Poker.Huolong.GameSetting GameSetting
        {
            get
            {
                return gameSetting;
            }
            set
            {
                gameSetting = value;
                switch (gameSetting.playerNum)
                {
                    case 4:
                        if (gameSetting.campNum == 2)
                        {
                            dropPlayerCampNum.value = 0;
                        }
                        else
                        {
                            Common.PlatformInterface.Base.DebugError("错误的设定数据");
                        }
                        break;
                    case 6:
                        if (gameSetting.campNum == 2)
                        {
                            dropPlayerCampNum.value = 1;
                        }
                        else if (gameSetting.campNum == 3)
                        {
                            dropPlayerCampNum.value = 2;
                        }
                        else
                        {
                            Common.PlatformInterface.Base.DebugError("错误的设定数据");
                        }
                        dropPlayerCampNum.value = 0;
                        break;
                    case 8:
                        if (gameSetting.campNum == 2)
                        {
                            dropPlayerCampNum.value = 3;
                        }
                        else if (gameSetting.campNum == 4)
                        {
                            dropPlayerCampNum.value = 4;
                        }
                        else
                        {
                            Common.PlatformInterface.Base.DebugError("错误的设定数据");
                        }
                        break;
                    case 9:
                        if (gameSetting.campNum == 3)
                        {
                            dropPlayerCampNum.value = 5;
                        }
                        else
                        {
                            Common.PlatformInterface.Base.DebugError("错误的设定数据");
                        }
                        break;
                    default:
                        Common.PlatformInterface.Base.DebugError("错误的设定数据");
                        break;
                }
                OnPlayerCampNumSelected();
                // todo set ui
            }
        }

        #endregion Public Fields

        #region Components Events

        public void GoToPanel(int index)
        {
            for (int i = 0; i < panels.Length; ++i)
            {
                panels[i].gameObject.SetActive(index == i);
            }
        }

        public void ShowHelpInfo(int id)
        {
            var type = (HelpTipType)id;
            MainScene.Instance.ShowMessageBox(helpTips[type], "了解", () => true);
        }

        public void SetDefaultSetting(int id)
        {
            GameSetting = (Common.Core.Poker.Huolong.GameSetting)Present.GameMain.Instance.GetDefaultGameSetting(Common.Core.GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong, id);
        }

        public void OnPlayerCampNumSelected()
        {
            switch (dropPlayerCampNum.value)
            {
                case 0:     // 4人2队
                    gameSetting.playerNum = 4;
                    gameSetting.campNum = 2;
                    break;
                case 1:     // 6人2队
                    gameSetting.playerNum = 6;
                    gameSetting.campNum = 2;
                    break;
                case 2:     // 6人3队
                    gameSetting.playerNum = 6;
                    gameSetting.campNum = 3;
                    break;
                case 3:     // 8人2队
                    gameSetting.playerNum = 8;
                    gameSetting.campNum = 2;
                    break;
                case 4:     // 8人4队
                    gameSetting.playerNum = 8;
                    gameSetting.campNum = 4;
                    break;
                case 5:     // 9人3队
                    gameSetting.playerNum = 9;
                    gameSetting.campNum = 3;
                    break;
            }
            dropGroupNum.ClearOptions();
            var options = new List<string>();
            int found = -1;
            for(int i = 0; i < groupNumItemsOfPlayerCampNum[dropPlayerCampNum.value].Length; ++i)
            {
                options.Add(groupNumItemsOfPlayerCampNum[dropPlayerCampNum.value][i].ToString() + "副");
                if (gameSetting.groupNum == groupNumItemsOfPlayerCampNum[dropPlayerCampNum.value][i])
                {
                    found = i;
                }
            }
            dropGroupNum.AddOptions(options);
            if(found == -1)
            {
                found = 0;
                gameSetting.groupNum = groupNumItemsOfPlayerCampNum[dropPlayerCampNum.value][0];
            }
            dropGroupNum.value = found;
            OnGroupNumSelected();
        }

        public void OnGroupNumSelected()
        {
            gameSetting.groupNum = groupNumItemsOfPlayerCampNum[dropPlayerCampNum.value][dropGroupNum.value];
            dropLastCardsNum.ClearOptions();
            var options = new List<string>();
            int found = -1;
            for (int i = 0; i < lastCardsNumItems[dropPlayerCampNum.value][dropGroupNum.value].Length; ++i)
            {
                options.Add(lastCardsNumItems[dropPlayerCampNum.value][dropGroupNum.value][i].ToString() + "张");
                if (gameSetting.lastCardsNum == lastCardsNumItems[dropPlayerCampNum.value][dropGroupNum.value][i])
                {
                    found = i;
                }
            }
            dropLastCardsNum.AddOptions(options);
            if (found == -1)
            {
                found = 0;
                gameSetting.lastCardsNum = lastCardsNumItems[dropPlayerCampNum.value][dropGroupNum.value][0];
            }
            dropLastCardsNum.value = found;
            OnLastCardsNumSelected();
        }

        public void OnLastCardsNumSelected()
        {
            gameSetting.lastCardsNum = lastCardsNumItems[dropPlayerCampNum.value][dropGroupNum.value][dropLastCardsNum.value];
            RefreshComments();
        }

        public void OnStartLevelSelected()
        {
            gameSetting.startLevel = dropStartLevel.value + 3;
            if (dropStartLevel.value > 10)
            {
                gameSetting.startLevel = dropStartLevel.value - 10;
            }
            dropEndLevel.ClearOptions();
            var options = new List<string>();
            int found = -1;
            for(int i = 0; i < 12; ++i)
            {
                int lv = gameSetting.startLevel + 1 + i;
                if(lv > 13)
                {
                    lv -= 13;
                }
                options.Add(Common.Core.Poker.Helper.PointToString(lv));
                if(gameSetting.endLevel == lv)
                {
                    found = i;
                }
            }
            dropEndLevel.AddOptions(options);
            if (found == -1)
            {
                found = 10;
                gameSetting.endLevel = gameSetting.startLevel + 11;
            }
            dropEndLevel.value = found;
            for (int i = 0; i < 13; ++i)
            {
                int lv = gameSetting.startLevel + i;
                if (lv > 13)
                {
                    lv -= 13;
                }
                textThresholds[i].text = Common.Core.Poker.Helper.PointToString(lv);
            }
            OnEndLevelSelected();
        }

        public void OnEndLevelSelected()
        {
            gameSetting.endLevel = gameSetting.startLevel + 1 + dropEndLevel.value;
            if (gameSetting.endLevel > 13)
            {
                gameSetting.endLevel -= 13;
            }
            if(dropEndLevel.value == 11)
            {
                textConstantMain.text = Common.Core.Poker.Helper.PointToString(gameSetting.endLevel) + "为常主";
            }
            else
            {
                textConstantMain.text = gameSetting.startLevel == 1 ? "K为常主" : (Common.Core.Poker.Helper.PointToString(gameSetting.startLevel - 1) + "为常主");
            }
            for(int i = 1; i < 13; ++i)
            {
                if(i< dropEndLevel.value)
                {
                    tgThresholds[i].isOn = false;
                    OnThresholdLevelSelected(tgThresholds[i]);
                    tgThresholds[i].gameObject.SetActive(false);
                }
                else
                {
                    tgThresholds[i].gameObject.SetActive(true);
                }
            }
            OnConstantMainSelected();
        }

        public void OnJokerLevelSelected()
        {
            gameSetting.jokerAfterEndLevel = tgNeedJokerLevel.isOn;
            RefreshComments();
        }

        public void OnConstantMainSelected()
        {
            gameSetting.isConstantMain = tgConstantMain.isOn;
            RefreshComments();
        }

        public void RefreshComments()
        {
            var strstr = new System.Text.StringBuilder();
            bool increaseJoker = false;
            if ((54 * gameSetting.groupNum - gameSetting.lastCardsNum) % gameSetting.playerNum != 0)
            {
                increaseJoker = true;
                strstr.Append("去掉:1大王1小王, ");   // todo 需要追加实装这个去掉大小王牌的功能
            }
            int totalMain = gameSetting.groupNum * (18 + (gameSetting.isConstantMain ? 3 : 0)) - (increaseJoker ? 2 : 0);
            strstr.Append("初始牌数: ");
            strstr.Append(((54 * gameSetting.groupNum - gameSetting.lastCardsNum) / gameSetting.playerNum).ToString());
            strstr.Append(", 平均主牌数: ");
            strstr.AppendFormat("{0}", totalMain * 1f / gameSetting.playerNum);
            textComments.text = strstr.ToString();
        }

        public void OnMainColorWaySelected()
        {

        }

        public void OnForceShowJokerNumChanged()
        {

        }

        public void OnDoubleLastCardsScoreSelected()
        {

        }

        public void OnWinWaySelected()
        {

        }

        public void OnFullScoreChanged()
        {

        }

        public void OnUpgradeScoreItemChanged(int id)
        {
            var type = (UpgradeScoreItemType)id;

        }

        public void OnThresholdLevelSelected(Toggle sender)
        {

        }

        public void OnDowngradeWaySelected(int id)
        {
            gameSetting.downgradeByJoker1 = tgJoker1Downgrade.isOn;
            gameSetting.downgradeByJoker2 = tgJoker2Downgrade.isOn;
            gameSetting.downgradeByMainCP = tgMPDowngrade.isOn;
            gameSetting.downgradeByUnMainCP = tgUMPDowngrade.isOn;
            gameSetting.downgradeByMainConstantMain = tgMCDowngrade.isOn;
            gameSetting.downgradeByUnMainConstantMain = tgUMCDowngrade.isOn;
        }

        public void OnRecordSelected()
        {
            gameSetting.recordThisMatch = tgRecord.isOn;
        }

        public void OnGiveCardDelayChanged()
        {
            if (inputGiveCardDelay.text == "")
            {
                inputGiveCardDelay.text = "0";
            }
            gameSetting.firstRoundGiveCardsDelay = System.Convert.ToInt32(inputGiveCardDelay.text);
        }

        public void OnRoundDelayChanged()
        {
            if (inputRoundDelay.text == "")
            {
                inputRoundDelay.text = "0";
            }
            gameSetting.aroundOverDelay = System.Convert.ToInt32(inputRoundDelay.text);
        }

        public void OnClickGo()
        {

        }

        #endregion Components Events

        #region Unity Events

        // Start is called before the first frame update
        protected void Awake()
        {
            GameSetting = (Common.Core.Poker.Huolong.GameSetting)Present.GameMain.Instance.GetGameSetting(Common.Core.GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong);
            GoToPanel(0);
        }

        // Update is called once per frame
        protected void Start()
        {

        }

        #endregion Unity Events

        #region Private Fields

        private Common.Core.Poker.Huolong.GameSetting gameSetting;
        private readonly Dictionary<HelpTipType, string> helpTips = new Dictionary<HelpTipType, string>() {
            {HelpTipType.NeedJokerLevel, "" },
            {HelpTipType.ColorWay, "" },
            {HelpTipType.ForceJokerLastCards, "" },
            {HelpTipType.WinWay, "" },
            {HelpTipType.UpgradeNum, "" },
            {HelpTipType.ThresholdLevel, "" },
            {HelpTipType.DowngradeWay, "" },
        };   // todo 填充提示文字
        private readonly int[][] groupNumItemsOfPlayerCampNum = new int[][]
        {
            new int[]{2,3,4,5,6},
            new int[]{3,4,5,6,7,8,9},
            new int[]{3,4,5,6,7,8,9},
            new int[]{4,5,6,7,8,9,10},
            new int[]{4,5,6,7,8,9,10},
            new int[]{5,6,7,8,9,10,11,12},
        };
        private readonly int[][][] lastCardsNumItems = new int[][][]
        {
            new int[][]{
                new int[]{2,4,6},
                new int[]{4,6,8},
                new int[]{6,8,10},
                new int[]{6,8,10,12},
                new int[]{8,10,12,14},
            },
            new int[][]{
                new int[]{4,6},
                new int[]{4,6},
                new int[]{4,6,10},
                new int[]{6,10,12},
                new int[]{6,10,12},
                new int[]{6,10,12},
                new int[]{10,12,16},
            },
            new int[][]{
                new int[]{4,6},
                new int[]{4,6},
                new int[]{4,6,10},
                new int[]{6,10,12},
                new int[]{6,10,12},
                new int[]{6,10,12},
                new int[]{10,12,16},
            },
            new int[][]{
                new int[]{6},
                new int[]{6},
                new int[]{4,10,12},
                new int[]{8,10},
                new int[]{6,8,14},
                new int[]{6,12,14},
                new int[]{10,12,18},
            },
            new int[][]{
                new int[]{6},
                new int[]{6},
                new int[]{4,10,12},
                new int[]{8,10},
                new int[]{6,8,14},
                new int[]{6,12,14},
                new int[]{10,12,18},
            },
            new int[][]{
                new int[]{7,9},
                new int[]{7,9},
                new int[]{7,9},
                new int[]{7,9},
                new int[]{7,9},
                new int[]{7,9},
                new int[]{7,9,16,18},
                new int[]{7,9,16,18},
            },
        };

        #endregion Private Fields

        #region Classes And Enums

        public enum HelpTipType : int
        {
            NeedJokerLevel,
            ColorWay,
            ForceJokerLastCards,
            WinWay,
            UpgradeNum,
            ThresholdLevel,
            DowngradeWay,
        }

        public enum UpgradeScoreItemType : int
        {
            NoLastCards,
            NoStar,
            NoHalfScore,
            ZeroScore,
            BaseStar,
            Joker1,
            Joker2, // todo 检查结算逻辑有没有排除无大王的单小王等级, 应该是有
        }

        public enum DowngradeWayType : int
        {
            Joker1,
            Joker2,
            MP,
            UMP,
            MC,
            UMC,
        }


        #endregion Classes And Enums
    }
}