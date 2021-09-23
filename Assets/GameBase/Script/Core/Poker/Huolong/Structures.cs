
namespace GameBase.Core.Poker.Huolong
{
    [System.Serializable]
    public class GameSetting
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
        public float firstRoundGiveCardsDelay;
        public int aroundOverDelay;
    }

    [System.Serializable]
    public struct LastCardsReport
    {
        public int[][] pain;
        public int[][] gain;
        public int[] lastCards;
    }

    [System.Serializable]
    public struct RoundReport
    {
        public int roundIndex;
        public int[][] threwCards;
        public int leader;
        public int winner;
        public int score;
    }

    [System.Serializable]
    public struct MatchReport
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
    public struct GameReport
    {
        public int winner;
        public int totalMatches;
        public int[] finalLevels;
    }
}
