using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker
{
    public interface IPlayerVector_Controller:Interface.IPlayerVector_Controller
    {
        Core.Poker.GameSubType GameSubType { get; }
    }
}