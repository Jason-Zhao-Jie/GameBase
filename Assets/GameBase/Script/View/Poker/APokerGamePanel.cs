using GameBase.Common.Core;
using GameBase.Common.Core.Poker;
using GameBase.Common.Interface.Poker;

using UnityEngine;

namespace GameBase.View
{
    public abstract class APokerGamePanel : AGamePanel, IPlayerItem<IPlayerVector_Item>
    {
        public override sealed GameType GameType => GameType.Poker;

        GameSubType IPlayerItem<IPlayerVector_Item>.GameSubType => (GameSubType)GameSubType;

        public void SetVector(IPlayerVector_Item vector)
        {
            throw new System.NotImplementedException();
        }
    }
}