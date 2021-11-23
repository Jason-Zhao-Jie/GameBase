using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker.Huolong
{
    public class MyStateZone_Huolong : MonoBehaviour
    {
        public Text mainNum;
        public GameObject inGameRoot;

        public void SetMainNum(int num)
        {
            mainNum.text = string.Format("{0}������", num);
        }

        public void SetInGame(bool inGame)
        {
            inGameRoot.SetActive(inGame);
        }

        public void OnClickPrevious()
        {
            MainScene.Instance.ShowTips("�鿴��һ�ֹ�����δʵ��");
        }

        public void OnClickLastCards()
        {
            MainScene.Instance.ShowTips("�鿴���ƹ�����δʵ��");
        }
    }
}