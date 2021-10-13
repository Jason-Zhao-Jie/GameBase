using GameBase.Common.Core;
using GameBase.Common.Interface;

using UnityEngine;

namespace GameBase.View
{
    public abstract class AGamePanel : MonoBehaviour, IPlayerItem
    {
        public abstract GameType GameType { get; }
        public abstract int GameSubType { get; }

        public PlayerType PlayerType => PlayerType.HostPlayer;

        public virtual void OnDispose()
        {
            Close(true);
        }

        public virtual GameObject Close(bool destroy = true)
        {
            transform.SetParent(null);
            if (destroy)
            {
                Destroy(gameObject);
            }
            return gameObject;
        }

    }
}