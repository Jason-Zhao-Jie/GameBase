using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBase.Core.Poker
{
    public class CardLayout : IEnumerator<int>, IList<int>
    {
        protected List<int> cards;

        public CardLayout()
        {
            cards = new List<int>();
            Current = 0;
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
                    foreach (var c in cards)
                    {
                        ret += Helper.GetScore(c);
                    }
                }
                return ret;
            }
        }

        public int Current
        {
            get;
            protected set;
        }

        object IEnumerator.Current => Current;

        public bool IsReadOnly => false;

        public int this[int index]
        {
            get => cards[index];
            set => cards[index] = value;
        }

        public void Init(int group)
        {
            for (int i = 0; i < group; ++i)
            {
                cards.Add(Helper.GetCardId(CardColor.Joker, 1, i));
                cards.Add(Helper.GetCardId(CardColor.Joker, 2, i));

                for (CardColor c = CardColor.Spades; c < CardColor.Joker; ++c)
                {
                    for (int p = 0; p < 13; ++p)
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

        public bool MoveNext()
        {
            if (Current >= cards.Count)
            {
                return false;
            }
            else
            {
                ++Current;
                return true;
            }
        }

        public void Reset()
        {
            Current = 0;
        }

        public void Dispose()
        {
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public int IndexOf(int item)
        {
            return cards.IndexOf(item);
        }

        public void Insert(int index, int item)
        {
            cards.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            cards.RemoveAt(index);
        }

        public void Add(int item)
        {
            cards.Add(item);
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            cards.CopyTo(array, arrayIndex);
        }

        public bool Remove(int item)
        {
            return cards.Remove(item);
        }
    }
}
