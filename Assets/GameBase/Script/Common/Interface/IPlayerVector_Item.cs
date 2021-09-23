using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// ������(PlayerVector)����Ҵ�����(PlayerItem)�Ľӿ�
    /// ������(PlayerVector)���������ӿ�����(Controller)����Ҵ�����(PlayerItem)֮�����ת������������������޹ص��߼���������Ϣ���͡�������̿���
    /// </summary>
    public interface IPlayerVector_Item
    {
        public Core.GameType GameType { get; }
        public CharacterInfo PlayerInfo { get; }
        public int PlayerIndex { get; }
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }
}