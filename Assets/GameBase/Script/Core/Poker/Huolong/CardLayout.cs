using System.Collections.Generic;

namespace GameBase.Core.Poker.Huolong
{
    public class CardLayout : Poker.CardLayout
    {
        public int GetMainCount(CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            int ret = 0;
            foreach(var i in this)
            {
                if(Helper.GetIsMain(i, mainColor, mainPoint, oftenMainPoint))
                {
                    ++ret;
                }
            }
            return ret;
        }

        public int GetScoreCount()
        {
            int ret = 0;
            foreach (var i in this)
            {
                ret += Poker.Helper.GetScore(i);
            }
            return ret;
        }

        public bool GetIsAllSame()
        {
            for (int i = 1; i < Count; ++i)
            {
                if (Poker.Helper.GetColorPoint(cards[i - 1]) != Poker.Helper.GetColorPoint(cards[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int[] GetSuitableCards(CardLayout leadCards, CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            if (leadCards.Count == 0 || Count < leadCards.Count)
            {
                return null;
            }
            if (mainPoint == 0)
            {
                mainColor = CardColor.Joker;
            }
            var isMain = Helper.GetIsMain(leadCards[0], mainColor, mainPoint, oftenMainPoint);
            var color = Poker.Helper.GetColor(leadCards[0]);
            var ret = new List<int>();
            if (isMain)
            {
                foreach (var i in this)
                {
                    if (Helper.GetIsMain(i, mainColor, mainPoint, oftenMainPoint))
                    {
                        ret.Add(i);
                    }
                }
            }
            else
            {
                foreach (var i in this)
                {
                    if (!Helper.GetIsMain(i, mainColor, mainPoint, oftenMainPoint) && Poker.Helper.GetColor(i) == color)
                    {
                        ret.Add(i);
                    }
                }
            }
            ret.Sort((int a, int b) =>
            {
                return Helper.CompareAsHandCard(a, b, mainColor, mainPoint, oftenMainPoint);
            });
            return ret.ToArray();
        }


    }
}
