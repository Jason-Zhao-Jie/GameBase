using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Common.Interface
{
    /// <summary>
    /// ��Ϸ�淨������(Controller)�Ľӿ�
    /// ��Ϸ�淨������(Controller)�︺�������Ϸ������㷨�Լ������̽��п���
    /// ��Ϸ�淨������(Controller)ͨ������ IModel �������淨�����㷨, �������Խ��м���
    /// �����ڲ�ͬ��Ϸ�淨�����̲�ͬ, �����Ȼ��Ҫÿ���淨����������Ϸ�淨������(Controller)
    /// </summary>
    public interface IController
    {
        public Core.GameType GameType { get; }
        public bool SetPlayer(int index, object player);
        public bool StartGame();
        public void OnDispose();
        public CharacterInfo GetPlayerInfo(int player);
        public CharacterInfo[] GetAllPlayersInfo();
    }

}