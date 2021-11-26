using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Poker
{
    public class CardsZone : MonoBehaviour
    {
        private const float POKER_SPACE = 0.3f;

        public GameObject root;
        public GameObject cardPrefab;
        public int alignType;
        public bool clickable;

        // Start is called before the first frame update
        protected void Awake()
        {
            cardChange = false;
        }

        // Update is called once per frame
        protected void Update()
        {
            if (cardChange)
            {
                var childCount = root.transform.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    var n = root.transform.GetChild(i);
                    var c = n.GetComponent<Card>();
                    var width = c.CardRect.width;
                    var height = c.CardRect.height;
                    if (c == null)
                    {
                        Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                    }
                    switch (alignType)
                    {
                        case 0: // todo 自己的手牌,需要处理换行
                            n.transform.localPosition = new Vector3(-width * (i * POKER_SPACE + 0.5f), height / 2, 0.01f*i);
                            break;
                        case 1:
                            n.transform.localPosition = new Vector3(width * (i * POKER_SPACE + 0.5f), -height / 2, 0.01f * i);
                            break;
                        case 2:
                            n.transform.localPosition = new Vector3(width * (i  - childCount / 2f - 0.5f) * POKER_SPACE, -height / 2, 0.01f * i);
                            break;
                        case 3:
                            n.transform.localPosition = new Vector3(-width * (i * POKER_SPACE + 0.5f), -height / 2, 0.01f * i);
                            break;
                        case 4:
                            n.transform.localPosition = new Vector3(width * (i * POKER_SPACE + 0.5f), 0, 0.01f * i);
                            break;
                        case 5:
                            n.transform.localPosition = new Vector3(width * (i - childCount / 2f - 0.5f) * POKER_SPACE, 0, 0.01f * i);
                            break;
                        case 6:
                            n.transform.localPosition = new Vector3(-width * (i * POKER_SPACE + 0.5f), 0, 0.01f * i);
                            break;
                        case 7:
                            n.transform.localPosition = new Vector3(width * (i * POKER_SPACE + 0.5f), height / 2, 0.01f * i);
                            break;
                        case 8:
                            n.transform.localPosition = new Vector3(width * (i - childCount / 2f - 0.5f) * POKER_SPACE, height / 2, 0.01f * i);
                            break;
                        case 9:
                            n.transform.localPosition = new Vector3(-width * (i * POKER_SPACE + 0.5f), height / 2, 0.01f * i);
                            break;
                        default:
                            break;
                    }
                }
                cardChange = false;
            }
        }

        public int[] CardList
        {
            get
            {
                int[] ret = new int[root.transform.childCount];
                for(int i = 0; i < ret.Length; ++i)
                {
                    var c = root.transform.GetChild(i).GetComponent<Card>();
                    if(c == null)
                    {
                        Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                    }
                    ret[i] = c.id;
                }
                return ret;
            }
        }

        public int[] SelectedCards
        {
            get
            {
                List<int> ret = new List<int>();
                for (int i = 0; i < root.transform.childCount; ++i)
                {
                    var c = root.transform.GetChild(i).GetComponent<Card>();
                    if (c == null)
                    {
                        Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                    }
                    if (c.Clicked)
                    {
                        ret.Add(c.id);
                    }
                }
                return ret.ToArray();
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    for (int i = 0; i < root.transform.childCount; ++i)
                    {
                        var c = root.transform.GetChild(i).GetComponent<Card>();
                        if (c == null)
                        {
                            Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                        }
                        if (c.Clicked)
                        {
                            if (c.Clicked)
                            {
                                c.OnClick();
                            }
                        }
                    }
                }
                else
                {
                    List<int> ret = new List<int>(value);
                    for (int i = 0; i < root.transform.childCount; ++i)
                    {
                        var c = root.transform.GetChild(i).GetComponent<Card>();
                        if (c == null)
                        {
                            Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                        }
                        if (c.Clicked != ret.Contains(c.id))
                        {
                            c.OnClick();
                        }
                    }
                }
            }
        }

        public void AddCard(params int[] card)
        {
            if(card == null)
            {
                Common.PlatformInterface.Base.DebugError("添加牌时产生错误！");
            }
            else
            {
                foreach(var c in card)
                {
                    var goCard = Instantiate(cardPrefab, root.transform);
                    var compCard = goCard.GetComponent<Card>();
                    compCard.SetCard(c, MainScene.Instance.GetCardSprite(c));
                    compCard.enableClick = clickable;
                }
                SortCards();
            }
        }

        public void DeleteCards(params int[] cards)
        {
            var delList = new List<int>(cards);
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                }
                if (delList.Contains(c.id))
                {
                    c.transform.SetParent(null);
                }
            }
            SortCards();
        }

        public void DeleteCardByColorPoint(int card)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                }
                if (Common.Core.Poker.Helper.GetColorPoint(c.id) != Common.Core.Poker.Helper.GetColorPoint(card))
                {
                    c.transform.SetParent(null);
                }
            }
            SortCards();
        }

        public void DeleteCardByColor(Common.Core.Poker.CardColor color)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                }
                if (Common.Core.Poker.Helper.GetColor(c.id) != color)
                {
                    c.transform.SetParent(null);
                }
            }
            SortCards();
        }

        public void DeleteAllCards()
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                }
                c.transform.SetParent(null);
            }
            SortCards();
        }

        public void AddSortFunc(System.Comparison<int> sortFunc)
        {
            this.sortFunc = sortFunc;
        }

        private void SortCards()
        {
            var cards = CardList;
            if (cards != null && cards.Length > 1)
            {
                System.Array.Sort(cards, Comparer<int>.Create(sortFunc));
            }
            var lastSelected = SelectedCards;
            SelectedCards = null;
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("检索牌时发现错误！");
                }
                c.SetCard(cards[i], MainScene.Instance.GetCardSprite(cards[i]));
            }
            SelectedCards = lastSelected;
            cardChange = true;
        }

        private bool cardChange = false;
        private System.Comparison<int> sortFunc = null;
    }
}