using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Core.Poker
{
    public enum GameSubType
    {
        Unknown,
        Huolong,        // ߫��
        Chudadi,        // �����
        Doudizhu,       // ������
        Zhuohongsan,    // ׽����
        Paodekuai,      // �ܵÿ�
        Gongzhu,        // ����
    }

    public enum CardColor
    {
        Unknown,
        Spades, // ����
        Heart, // ����
        Cube, // �ݻ�
        Diamond, // ��Ƭ
        Joker, //����
    }
}