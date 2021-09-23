using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface.Poker
{
    public interface IController : Interface.IController
    {
        Core.Poker.GameSubType GameSubType { get; }
    }

}