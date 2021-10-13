using System.Collections.Generic;
using UnityEngine;
using ArmyAnt.Common;

namespace GameBase.Common.Core
{
    [System.Serializable]
    public class WrapSerializedWeightDic : Serialized<int, int>
    {
        public WrapSerializedWeightDic(Dictionary<int, int> target) : base(target)
        {
        }
    }
    [System.Serializable]
    public class WrapSerializedRateDic : Serialized<int, float>
    {
        public WrapSerializedRateDic(Dictionary<int, float> target) : base(target)
        {
        }
    }

    [System.Serializable]
    public class CharacterInfo_AI : CharacterInfo
    {
        [SerializeField]
        public WrapSerializedWeightDic baseWeight;
        [SerializeField]
        public WrapSerializedRateDic friendOthersRate;
        [SerializeField]
        public WrapSerializedRateDic enemyOthersRate;

        public int GetBaseWeight(int gameId)
        {
            int ret = 0;
            if (baseWeight.ContainsKey(gameId))
            {
                ret = baseWeight[gameId];
            }
            else if (baseWeight.ContainsKey(0))
            {
                ret = baseWeight[0];
            }
            return ret;
        }
    }
}
