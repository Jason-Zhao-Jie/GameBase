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

        public void InitNewGame(int firstLevel)
        {
            SetMatchIndex(0);
            SetMainColor(Common.Core.Poker.CardColor.Unknown);
            SetMainPoint(firstLevel);
            SetState("��Ϸ׼����");
        }

        public void SetMatchIndex(int index)
        {
            txtMatchIndex.text = string.Format("��{0}��", index);
            txtCardsNum.gameObject.SetActive(false);
        }

        public void SetMainColor(Common.Core.Poker.CardColor color)
        {
            mainColor.sprite = colorSprites[(int)color];
        }

        public void SetMainPoint(int level)
        {
            txtMainCampLevel.text = string.Format("ׯ�ҵȼ�: {0}", Common.Core.Poker.Helper.PointToString(level));
        }

        public void SetCardsNum(int cardsNum)
        {
            txtCardsNum.gameObject.SetActive(true);
            txtCardsNum.text = string.Format("ʣ��{0}����", cardsNum);
        }

        public void SetState(string state)
        {
            txtState.text = state;
        }
    }
}