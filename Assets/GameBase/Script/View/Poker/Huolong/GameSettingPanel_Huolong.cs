using GameBase.Common.Interface.Poker.Huolong;

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

        public override PanelType PanelType => PanelType.GameSettingPanel;

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
                dropStartLevel.value = System.Convert.ToInt32(gameSetting.startLevel - 1);
                OnStartLevelSelected();
                tgNeedJokerLevel.isOn = gameSetting.jokerAfterEndLevel;
                OnJokerLevelSelected();
                dropMainColorWay.value = System.Convert.ToInt32(gameSetting.mainColorGetWay);
                OnMainColorWaySelected();
                inputForceLastCardsJokerNum.text = gameSetting.forceSendJokerLastCardsNum.ToString();
                OnForceShowJokerNumChanged();
                tgDoubleLastCardsScore.isOn = gameSetting.lastCardsScoreDouble;
                OnDoubleLastCardsScoreSelected();
                dropWinWay.value = System.Convert.ToInt32(gameSetting.winMatchWay);
                OnWinWaySelected();
                inputFullScore.text = gameSetting.fullScore.ToString();
                OnFullScoreChanged();
                inputNoLastCardsUpgrade.text = gameSetting.noLastCardsBaseUpgrade.ToString();
                OnUpgradeScoreItemChanged(0);
                inputNoStarsUpgrade.text = gameSetting.noJokerBaseUpgrade.ToString();
                OnUpgradeScoreItemChanged(1);
                inputNoHalfScoreUpgrade.text = gameSetting.noHalfScroreAddUpgrade.ToString();
                OnUpgradeScoreItemChanged(2);
                inputZeroScoreUpgrade.text = gameSetting.noScoreAddUpgrade.ToString();
                OnUpgradeScoreItemChanged(3);
                inputStarBaseUpgrade.text = gameSetting.haveJokerBaseUpgrade.ToString();
                OnUpgradeScoreItemChanged(4);
                inputJoker1Upgrade.text = gameSetting.joker1AddUpgrade.ToString();
                OnUpgradeScoreItemChanged(5);
                inputJoker2Upgrade.text = gameSetting.joker2AddUpgrade.ToString();
                OnUpgradeScoreItemChanged(6);
                tgRecord.isOn = gameSetting.recordThisMatch;
                OnRecordSelected();
                inputGiveCardDelay.text = gameSetting.firstRoundGiveCardsDelay.ToString();
                OnGiveCardDelayChanged();
                inputRoundDelay.text = gameSetting.aroundOverDelay.ToString();
                OnRoundDelayChanged();
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
            dropPlayerCampNum.options.RemoveAt(5);  // 去掉9人模式
            dropPlayerCampNum.options.RemoveAt(4);  // 去掉8人模式
            dropPlayerCampNum.options.RemoveAt(3);  // 去掉8人模式
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
            gameSetting.startLevel = dropStartLevel.value + 1;
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
            if (dropEndLevel.value == 11)
            {
                textConstantMain.text = Common.Core.Poker.Helper.PointToString(gameSetting.endLevel) + "为常主";
            }
            else
            {
                textConstantMain.text = gameSetting.startLevel == 1 ? "K为常主" : (Common.Core.Poker.Helper.PointToString(gameSetting.startLevel - 1) + "为常主");
            }
            var found = new List<int>();
            for (int i = 0; i < tgThresholds.Length; ++i)
            {
                int p = gameSetting.startLevel + i;
                if (p > 13)
                {
                    p -= 13;
                }
                textThresholds[i].text = Common.Core.Poker.Helper.PointToString(p);
                if (i > dropEndLevel.value + 1)
                {
                    tgThresholds[i].isOn = false;
                    tgThresholds[i].gameObject.SetActive(false);
                }
                else
                {
                    tgThresholds[i].gameObject.SetActive(true);
                    if (gameSetting.thresholdsLevels != null && gameSetting.thresholdsLevels.Length > 0)
                    {
                        for (int n = 0; n < gameSetting.thresholdsLevels.Length; ++n)
                        {
                            if (gameSetting.thresholdsLevels[n] == p)
                            {
                                found.Add(i);
                                break;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < tgThresholds.Length; ++i)
            {
                tgThresholds[i].isOn = false;
            }
                foreach (var p in found)
            {
                tgThresholds[p].isOn = true;
            }
            OnConstantMainSelected();
        }

        public void OnJokerLevelSelected()
        {
            if (gameSetting.campNum > 0)
            {
                gameSetting.jokerAfterEndLevel = tgNeedJokerLevel.isOn;
                RefreshComments();
            }
        }

        public void OnConstantMainSelected()
        {
            if (gameSetting.campNum > 0)
            {
                gameSetting.isConstantMain = tgConstantMain.isOn;
                RefreshComments();
            }
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
            gameSetting.mainColorGetWay = (Common.Core.Poker.Huolong.MainColorGetWay)dropMainColorWay.value;
            switch (gameSetting.mainColorGetWay)
            {
                case Common.Core.Poker.Huolong.MainColorGetWay.AllRandom:
                case Common.Core.Poker.Huolong.MainColorGetWay.EveryMatchRandom:
                case Common.Core.Poker.Huolong.MainColorGetWay.RandomMainPlayerWithColorSpade:
                case Common.Core.Poker.Huolong.MainColorGetWay.RandomMainPlayerWithColorHeart:
                case Common.Core.Poker.Huolong.MainColorGetWay.RandomMainPlayerWithColorCube:
                case Common.Core.Poker.Huolong.MainColorGetWay.RandomMainPlayerWithColorDiamond:
                    inputGiveCardDelay.interactable = false;
                    break;
                case Common.Core.Poker.Huolong.MainColorGetWay.FirstMatchShowStar:
                case Common.Core.Poker.Huolong.MainColorGetWay.FirstMatchShowMain:
                case Common.Core.Poker.Huolong.MainColorGetWay.EveryMatchShowStar:
                case Common.Core.Poker.Huolong.MainColorGetWay.EveryMatchShowMain:
                    inputGiveCardDelay.interactable = true;
                    break;
            }
        }

        public void OnForceShowJokerNumChanged()
        {
            gameSetting.forceSendJokerLastCardsNum = System.Convert.ToInt32(inputForceLastCardsJokerNum.text);
        }

        public void OnDoubleLastCardsScoreSelected()
        {
            gameSetting.lastCardsScoreDouble = tgDoubleLastCardsScore.isOn;
        }

        public void OnWinWaySelected()
        {
            gameSetting.winMatchWay = (Common.Core.Poker.Huolong.WinMatchWay)dropWinWay.value;
        }

        public void OnFullScoreChanged()
        {
            gameSetting.fullScore = System.Convert.ToInt32(inputFullScore.text);
        }

        public void OnUpgradeScoreItemChanged(int id)
        {
            var type = (UpgradeScoreItemType)id;
            switch (type)
            {
                case UpgradeScoreItemType.NoLastCards:
                    gameSetting.noLastCardsBaseUpgrade = System.Convert.ToInt32(inputNoLastCardsUpgrade.text);
                    break;
                case UpgradeScoreItemType.NoStar:
                    gameSetting.noJokerBaseUpgrade = System.Convert.ToInt32(inputNoStarsUpgrade.text);
                    break;
                case UpgradeScoreItemType.NoHalfScore:
                    gameSetting.noHalfScroreAddUpgrade = System.Convert.ToInt32(inputNoHalfScoreUpgrade.text);
                    break;
                case UpgradeScoreItemType.ZeroScore:
                    gameSetting.noScoreAddUpgrade = System.Convert.ToInt32(inputZeroScoreUpgrade.text);
                    break;
                case UpgradeScoreItemType.BaseStar:
                    gameSetting.haveJokerBaseUpgrade = System.Convert.ToInt32(inputStarBaseUpgrade.text);
                    break;
                case UpgradeScoreItemType.Joker1:
                    gameSetting.joker1AddUpgrade = System.Convert.ToInt32(inputJoker1Upgrade.text);
                    break;
                case UpgradeScoreItemType.Joker2:
                    gameSetting.joker2AddUpgrade = System.Convert.ToInt32(inputJoker2Upgrade.text);
                    break;
            }
        }

        public void OnThresholdLevelSelected(Toggle sender)
        {
            var list = new List<int>();
            for (int i = 0; i < tgThresholds.Length; ++i)
            {
                int p = gameSetting.startLevel + i;
                if (p > 13)
                {
                    p -= 13;
                }
                if (tgThresholds[i].isOn)
                {
                    list.Add(p);
                }
            }

            var settings = new List<bool>();
            settings.Add(gameSetting.downgradeByJoker1);
            settings.Add(gameSetting.downgradeByJoker2);
            settings.Add(gameSetting.downgradeByMainCP);
            settings.Add(gameSetting.downgradeByUnMainCP);
            settings.Add(gameSetting.downgradeByMainConstantMain);
            settings.Add(gameSetting.downgradeByUnMainConstantMain);

            tgJoker1Downgrade.isOn = settings[0];
            tgJoker2Downgrade.isOn = settings[1];
            tgMPDowngrade.isOn = settings[2];
            tgUMPDowngrade.isOn = settings[3];
            tgMCDowngrade.isOn = settings[4];
            tgUMCDowngrade.isOn = settings[5];

            tgJoker1Downgrade.interactable = list.Count > 0;
            tgJoker2Downgrade.interactable = list.Count > 0;
            tgMPDowngrade.interactable = list.Count > 0;
            tgUMPDowngrade.interactable = list.Count > 0;
            tgMCDowngrade.interactable = list.Count > 0;
            tgUMCDowngrade.interactable = list.Count > 0;

            gameSetting.thresholdsLevels = list.ToArray();
            OnDowngradeWaySelected(0);
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
            MainScene.Instance.StartGame(Common.Core.GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong, gameSetting, gameSetting.playerNum);
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
            {HelpTipType.NeedJokerLevel, "\u3000\u3000如果选择打王级，则在最终级顺利过关后，还需要打一级王级才能通关。\n\u3000\u3000打王级时，没有主花色，主牌只有王牌和常主牌。\n\u3000\u3000同门槛级一样，王级不可跨越，但不会降级。" },
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