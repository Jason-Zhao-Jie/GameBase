using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Poker
{
    /// <summary>
    /// 世界场景扑克相关，用于处理场上的扑克牌显示，调整计算扑克牌位置
    /// </summary>
    public class WorldPokerManager : MonoBehaviour
    {
        public enum CardType
        {
            All,
            MyHandCard,
            CenterCards,
            ThrownCards,
        }

        public CardsZone MyPokerCards;
        public CardsZone CenterPokerCards;
        public CardsZone[] ThrownPokerCards;

        public int PlayerNum => playerNum;

        public void Init(int playerNum)
        {
            this.playerNum = playerNum;
            ClearCards(CardType.All);
        }

        public void ClearCards(CardType type)
        {
            switch (type)
            {
                case CardType.All:
                    MyPokerCards.DeleteAllCards();
                    CenterPokerCards.DeleteAllCards();
                    foreach (var l in ThrownPokerCards)
                    {
                        l.DeleteAllCards();
                    }
                    break;
                case CardType.MyHandCard:
                    MyPokerCards.DeleteAllCards();
                    break;
                case CardType.CenterCards:
                    CenterPokerCards.DeleteAllCards();
                    break;
                case CardType.ThrownCards:
                    foreach (var l in ThrownPokerCards)
                    {
                        l.DeleteAllCards();
                    }
                    break;
            }
        }

        public void AddMyCards(params int[] cards)
        {
            MyPokerCards.AddCard(cards);
        }

        public void SetSelectMyCards(params int[] cards)
        {
            MyPokerCards.SelectedCards = cards;
        }

        public void AddCenterCards(params int[] cards)
        {
            CenterPokerCards.AddCard(cards);
        }

        public void AddThrownCards(int playerIndex, params int[] cards)
        {
            ThrownPokerCards[headIndexes[playerNum][playerIndex]].AddCard(cards);
        }

        public void AddThrownCards(int playerIndex, int[] cards, params int[] addOnCards)
        {
            ThrownPokerCards[headIndexes[playerNum][playerIndex]].AddCard(cards);
            ThrownPokerCards[headIndexes[playerNum][playerIndex]].AddCard(addOnCards);
        }

        public void ClearThrownCards(int playerIndex)
        {
            ThrownPokerCards[headIndexes[playerNum][playerIndex]].DeleteAllCards();
        }

        // Start is called before the first frame update
        protected void Start()
        {

        }

        // Update is called once per frame
        protected void Update()
        {

        }

        protected int playerNum = 0;

        private static readonly int[][] headIndexes = new int[][]
        {
            new int[]{},
            new int[]{0},
            new int[]{0, 4},
            new int[]{0, 3, 5},
            new int[]{0, 2, 4, 6},
            new int[]{0, 1, 3, 5, 7},
            new int[]{0, 1, 3, 4, 5, 7},
            new int[]{0, 1, 2, 3, 5, 6, 7},
            new int[]{0, 1, 2, 3, 4, 5, 6, 7},
        };
    }
}