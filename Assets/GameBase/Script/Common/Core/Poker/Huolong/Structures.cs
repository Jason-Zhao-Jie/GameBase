
namespace GameBase.Common.Core.Poker.Huolong
{
    [System.Serializable]
    public struct GameSetting
    {
        public int playerNum;
        public int campNum;
        public int groupNum;
        public int lastCardsNum;
        public bool isConstantMain;
        public int startLevel;
        public int endLevel;
        public bool jokerAfterEndLevel;
        public MainColorGetWay mainColorGetWay;
        public int forceSendJokerLastCardsNum;
        public bool lastCardsScoreDouble;
        public int fullScore;
        public WinMatchWay winMatchWay;
        public int noHalfScroreAddUpgrade;
        public int noScoreAddUpgrade;
        public int joker1AddUpgrade;
        public int joker2AddUpgrade;
        public int haveJokerBaseUpgrade;
        public int noJokerBaseUpgrade;
        public int noLastCardsBaseUpgrade;
        public int[] thresholdsLevels;
        public bool downgradeByJoker1;
        public bool downgradeByJoker2;
        public bool downgradeByMainCP;
        public bool downgradeByUnMainCP;
        public bool downgradeByMainConstantMain;
        public bool downgradeByUnMainConstantMain;
        public bool recordThisMatch;
        public int firstRoundGiveCardsDelay;
        public int aroundOverDelay;

        /// <summary>
        /// 判断是否需要去掉大小王各一张
        /// </summary>
        /// <returns></returns>
        public bool InCreaseJokers()
        {
            return (54 * groupNum - lastCardsNum) % playerNum != 0;
        }
    }

    [System.Serializable]
    public class LastCardsReport
    {
        public int[][] pain;
        public int[][] gain;
        public int[] lastCards;
    }

    [System.Serializable]
    public class RoundReport
    {
        public int roundIndex;
        public int[][] threwCards;
        public int leader;
        public int winner;
        public int score;
    }

    [System.Serializable]
    public class MatchReport
    {
        public int matchIndex;
        public int winner;
        public int[] totalScore;
        public int[] lastCards;
        public int[][] finallyThrew;
        public int oldMainPlayer;
        public int newMainPlayer;
        public int[] oldLevels;
        public int[] newLevels;
        public int upgradedLevelNumber;
        public bool gameover;
    }

    [System.Serializable]
    public class GameReport
    {
        public int winner;
        public int totalMatches;
        public int[] finalLevels;
    }
}
