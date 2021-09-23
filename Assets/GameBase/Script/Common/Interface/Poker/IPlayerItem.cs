using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerItem:Interface.IPlayerItem
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}