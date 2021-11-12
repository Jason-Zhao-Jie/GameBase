using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Poker
{
    public class CardsZone : MonoBehaviour
    {
        public GameObject root;
        public GameObject cardPrefab;
        public int alignType;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
                        Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
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
                        Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
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
                            Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
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
                            Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
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
                Common.PlatformInterface.Base.DebugError("Ìí¼ÓÅÆÊ±²úÉú´íÎó£¡");
            }
            else
            {
                foreach(var c in card)
                {
                    var goCard = Instantiate(cardPrefab, root.transform);
                    var compCard = goCard.GetComponent<Card>();
                    compCard.SetCard(c, MainScene.Instance.GetCardSprite(c));
                }
            }
        }

        public void DeleteCard(int card)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
                }
                if (c.id != card)
                {
                    c.transform.SetParent(null);
                }
            }
        }

        public void DeleteCardByColorPoint(int card)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
                }
                if (Common.Core.Poker.Helper.GetColorPoint(c.id) != Common.Core.Poker.Helper.GetColorPoint(card))
                {
                    c.transform.SetParent(null);
                }
            }
        }

        public void DeleteCardByColor(Common.Core.Poker.CardColor color)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
                }
                if (Common.Core.Poker.Helper.GetColor(c.id) != color)
                {
                    c.transform.SetParent(null);
                }
            }
        }

        public void DeleteAllCards()
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var c = root.transform.GetChild(i).GetComponent<Card>();
                if (c == null)
                {
                    Common.PlatformInterface.Base.DebugError("¼ìË÷ÅÆÊ±·¢ÏÖ´íÎó£¡");
                }
                c.transform.SetParent(null);
            }
        }
    }
}