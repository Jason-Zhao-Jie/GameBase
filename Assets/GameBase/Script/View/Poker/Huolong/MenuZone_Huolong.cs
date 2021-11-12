using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Poker.Huolong
{
    public class MenuZone_Huolong : MonoBehaviour
    {
        public void OnClickOpenMainMenu()
        {
            MainScene.Instance.ShowMainMenuPanel(MainMenuPanel.MenuType.InGame);
        }
    }
}