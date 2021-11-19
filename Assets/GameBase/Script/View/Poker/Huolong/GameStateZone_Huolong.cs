using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker.Huolong
{
    public class GameStateZone_Huolong : MonoBehaviour
    {
        public Text txtMatchIndex;
        public Text txtMainColorTitle;
        public Image mainColor;
        public Text txtMainCampLevel;
        public Text txtCardsNum;
        public Text txtState;

        public Sprite[] colorSprites;

        public void SetMatchIndex(int index)
        {
            txtMatchIndex.text = string.Format("第{0}局", index);
        }

        public void SetMainColor(Common.Core.Poker.CardColor color)
        {
            mainColor.sprite = colorSprites[(int)color];
        }

        public void SetMainPoint(int level)
        {
            txtMainCampLevel.text = string.Format("庄家等级: {0}", level);
        }

        public void SetCardsNum(int cardsNum)
        {
            txtCardsNum.text = string.Format("剩余{0}张牌", cardsNum);
        }

        public void SetState(string state)
        {
            txtState.text = state;
        }
    }
}