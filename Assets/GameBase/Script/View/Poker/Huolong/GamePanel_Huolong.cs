using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Poker.Huolong
{
    public class GamePanel_Huolong : AGamePanel
    {
        public override Common.Core.GameType GameType => Common.Core.GameType.Poker;
        public override int GameSubType => (int)Common.Core.Poker.GameSubType.Huolong;

    }
}