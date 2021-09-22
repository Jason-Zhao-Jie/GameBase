
namespace GameBase.Core.Poker.Huolong
{
    public static class Helper
    {
        public static bool GetIsMain(int card, CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            var color = Poker.Helper.GetColor(card);
            var point = Poker.Helper.GetPoint(card);
            return color == CardColor.Joker || color == mainColor || point == mainPoint || point == oftenMainPoint;
        }

        public static int CompareAsHandCard(int a, int b, CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            var powerA = GetCardPowerLevel(a, mainColor, mainPoint, oftenMainPoint);
            var powerB = GetCardPowerLevel(b, mainColor, mainPoint, oftenMainPoint);
            if(powerA != powerB)
            {
                return powerA - powerB;
            }
            else
            {
                var colorA = Poker.Helper.GetColor(a);
                var colorB = Poker.Helper.GetColor(b);
                if(colorA != colorB)
                {
                    return colorA - colorB;
                }
                else
                {
                    var pointA = Poker.Helper.GetPoint(a);
                    var pointB = Poker.Helper.GetPoint(b);
                    if (pointA == 1)
                    {
                        pointA = 14;
                    }
                    if (pointB == 1)
                    {
                        pointB = 14;
                    }
                    if (pointA != pointB)
                    {
                        return pointA - pointB;
                    }
                    else
                    {
                        return Poker.Helper.GetGroup(a) - Poker.Helper.GetGroup(b);
                    }
                }
            }
        }

        public static int CompareAsThrewCards(CardLayout a, CardLayout b, CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            var failA = a.GetIsAllSame() ? 1 : 0;
            var failB = b.GetIsAllSame() ? 1 : 0;
            if(failA!=1 || failB != 1)
            {
                return failA - failB;
            }
            var powerA = GetCardPowerLevel(a[0], mainColor, mainPoint, oftenMainPoint);
            var powerB = GetCardPowerLevel(b[0], mainColor, mainPoint, oftenMainPoint);
            var colorA = Poker.Helper.GetColor(a[0]);
            var colorB = Poker.Helper.GetColor(b[0]);

            if (powerA != powerB)
            {
                return powerA - powerB;
            }
            if(colorA == colorB)
            {
                var pointA = Poker.Helper.GetPoint(a[0]);
                var pointB = Poker.Helper.GetPoint(b[0]);
                if (pointA == 1)
                {
                    pointA = 14;
                }
                if (pointB == 1)
                {
                    pointB = 14;
                }
                return pointA - pointB;
            }
            return 0;
        }

        private static int GetCardPowerLevel(int card, CardColor mainColor, int mainPoint, int oftenMainPoint)
        {
            var color = Poker.Helper.GetColor(card);
            var point = Poker.Helper.GetPoint(card);
            if (color == CardColor.Joker)
            {
                return point == 1 ? 7 : 6;
            }
            else if (point == mainPoint)
            {
                return color == mainColor ? 5 : 4;
            }
            else if (point == oftenMainPoint)
            {
                return color == mainColor ? 3 : 2;
            }
            else
            {
                return color == mainColor ? 1 : 0;
            }
        }
    }
}
