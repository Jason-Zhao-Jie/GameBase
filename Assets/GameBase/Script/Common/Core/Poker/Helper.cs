
namespace GameBase.Common.Core.Poker
{
    public static class Helper
    {
        public const int Joker1 = (int)CardColor.Joker * 100 + 1;
        public const int Joker2 = (int)CardColor.Joker * 100 + 2;

        public static int GetCardId(CardColor color, int point, int group = 0)
        {
            return group * 1000 + System.Convert.ToInt32(color) * 100 + point;
        }

        public static CardColor GetColor(int card)
        {
            return (CardColor)(card % 1000 / 100);
        }

        public static int GetPoint(int card)
        {
            return card % 100;
        }

        public static int GetGroup(int card)
        {
            return card / 1000;
        }

        public static int GetScore(int card)
        {
            var color = GetColor(card);
            var point = GetPoint(card);
            if (color == CardColor.Joker)
            {
                return 0;
            }
            else
            {
                switch (point)
                {
                    case 5:
                        return 5;

                    case 10:
                    case 13:
                        return 10;
                    default:
                        return 0;
                }
            }
        }

        public static int GetColorPoint(int card)
        {
            return card % 1000;
        }

        public static int GetNextPoint(int point)
        {
            if(point == 13)
            {
                return 1;
            }
            else
            {
                return point + 1;
            }
        }

        public static string ColorToString(CardColor color)
        {
            switch (color)
            {
                case CardColor.Spades:
                    return "黑桃";
                case CardColor.Heart:
                    return "红桃";
                case CardColor.Cube:
                    return "草花";
                case CardColor.Diamond:
                    return "方块";
                case CardColor.Joker:
                    return "王牌";
                default:
                    return "未知花色";
            }
        }

        public static string ColorToString(int card)
        {
            return ColorToString(GetColor(card));
        }

        public static string PointToString(int card)
        {
            var p = GetPoint(card);
            switch (GetColor(card))
            {
                case CardColor.Joker:
                    switch (GetPoint(card))
                    {
                        case 1:
                            return "大王";
                        case 2:
                            return "小王";
                        default:
                            return "未知王牌[" + GetPoint(card) + "]";
                    }
                default:
                    switch (p)
                    {
                        case 1:
                            return "A";
                        case 11:
                            return "J";
                        case 12:
                            return "Q";
                        case 13:
                            return "K";
                        default:
                            return (p > 1 && p < 11) ? p.ToString() : "未知点数";
                    }
            };
        }

        public static string CardToString(int card)
        {
            switch (GetColor(card))
            {
                case CardColor.Joker:
                    return PointToString(card);
                default:
                    return ColorToString(card) + PointToString(card);
            }
        }
    }
}
