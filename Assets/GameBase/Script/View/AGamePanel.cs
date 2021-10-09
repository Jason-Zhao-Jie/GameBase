using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View
{
    public abstract class AGamePanel : MonoBehaviour
    {
        public abstract Common.Core.GameType GameType { get; }
        public abstract int GameSubType { get; }

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