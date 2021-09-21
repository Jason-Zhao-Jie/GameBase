using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Core.Poker
{
    public class CardLayout
    {
        protected List<int> cards;

        public CardLayout()
        {
            cards = new List<int>();
        }

        public int Count
        {
            get
            {
                return cards.Count;
            }
        }

        public int TotalScore
        {
            get
            {
                var ret = 0;
                if (cards.Count != 0)
                {
                    foreach (var c in this.cards)
                    {
                        ret += Helper.GetScore(c);
                    }
                }
                return ret;
            }
        }

        public void Init(int group)
        {
            for (int i = 0; i < group; ++i)
            {
                cards.Add(Helper.GetCardId(CardColor.Joker, 1, i));
                cards.Add(Helper.GetCardId(CardColor.Joker, 2, i));

                for (CardColor c = CardColor.Spades; c < CardColor.Joker; ++c)
                {
                    for (int p = 0; p < 10; ++p)
                    {
                        cards.Add(Helper.GetCardId(c, p + 1, i));
                    }
                    for (int p = 10; p < 13; ++p)
                    {
                        cards.Add(Helper.GetCardId(c, p + 1, i));
                    }
                }
            }
        }

        public void Clear()
        {
            cards.Clear();
        }

        public void RandomAllCards()
        {
            var initCard = cards;
            cards = new List<int>();
            var r = new Random();
            while (initCard.Count > 0)
            {
                var num = r.Next(0, initCard.Count - 1);
                cards.Add(initCard[num]);
                initCard.RemoveAt(num);
            }
        }

        public bool Contains(int card)
        {
            return cards.Contains(card);
        }

        public bool PushCard(params int[] args)
        {
            if (args != null)
            {
                foreach (var c in args)
                {
                    if (Contains(c))
                    {
                        return false;
                    }
                }
                foreach (var c in args)
                {
                    cards.Add(c);
                }
                return true;
            }
            return true;
        }

        public int PopCard()
        {
            if (cards.Count == 0)
            {
                return 0;
            }
            var ret = cards.Last();
            cards.RemoveAt(cards.Count - 1);
            return ret;
        }

        public int DequeueCard()
        {
            if (cards.Count == 0)
            {
                return 0;
            }
            var ret = cards.First();
            cards.RemoveAt(0);
            return ret;
        }

        public bool RemoveCards(params int[] args)
        {
            if (args != null)
            {
                foreach (var c in args)
                {
                    if (!Contains(c))
                    {
                        return false;
                    }
                }
                foreach (var c in args)
                {
                    cards.Remove(c);
                }
                return true;
            }
            return true;
        }

        public int First()
        {
            if (cards.Count == 0)
            {
                return 0;
            }
            return cards.First();
        }

        public int Last()
        {
            if (cards.Count == 0)
            {
                return 0;
            }
            return cards.Last();
        }

        public int GetColorPointNum(int card)
        {
            var ret = 0;
            foreach (var c in cards)
            {
                if (Helper.GetColorPoint(c) == Helper.GetColorPoint(card))
                {
                    ++ret;
                }
            }
            return ret;
        }

        public int[] GetColorPointCards(int card)
        {
            var ret = new List<int>();
            foreach (var c in cards)
            {
                if (Helper.GetColorPoint(c) == Helper.GetColorPoint(card))
                {
                    ret.Add(c);
                }
            }
            return ret.ToArray();
        }

    }
}
