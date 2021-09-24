﻿using System.Collections.Generic;

namespace GameBase.Common.Core
{
    [System.Serializable]
    public class CharacterInfo_AI : CharacterInfo
    {
        public int[] baseWeight;
        public Dictionary<int, int> friendOthersRate;
        public Dictionary<int, int> enemyOthersRate;
    }
}