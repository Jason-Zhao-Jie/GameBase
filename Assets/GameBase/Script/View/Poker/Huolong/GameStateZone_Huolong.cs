using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker.Huolong
{
    public class GameStateZone_Huolong : MonoBehaviour
    {
        public Text txtMatchIndex;
        public Text txtMainColorTitle;
        public Image mainColor;
        public Text txtCardsNum;
        public Text txtState;

        public Sprite[] colorSprites;

        public void SetMatchIndex(int index)
        {
            txtMatchIndex.text = string.Format("µ⁄{0}æ÷", index);
        }

        public void SetMainColor(Common.Core.Poker.CardColor color)
        {
            mainColor.sprite = colorSprites[(int)color];
        }

        public void SetCardsNum(int cardsNum)
        {
            txtCardsNum.text = string.Format(" £”‡{0}’≈≈∆", cardsNum);
        }

        public void SetState(string state)
        {
            txtState.text = state;
        }
    }
}