using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerVector_Item : Interface.IPlayerVector_Item
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}