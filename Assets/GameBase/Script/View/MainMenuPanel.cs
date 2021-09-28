using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View
{
    public class MainMenuPanel : APanel
    {
        public enum MenuType
        {
            Start,
            InGame,
        }

        public GameObject btnBackClose;
        public GameObject btnStartSingle;
        public GameObject btnStartLocal;
        public GameObject btnLoginToServer;
        public GameObject btnMyInfo;
        public GameObject btnSettings;
        public GameObject btnHelp;
        public GameObject btnQuitBackToStart;
        public GameObject btnExitProgram;

        public override PanelType PanelType => PanelType.MessageBox;

        public MenuType Type
        {
            get => type;
            set{
                if(btnBackClose != null)
                {
                    switch (value)
                    {
                        case MenuType.Start:
                            btnBackClose.SetActive(false);
                            btnStartSingle.SetActive(true);
                            btnStartLocal.SetActive(true);
                            btnLoginToServer.SetActive(true);
                            btnMyInfo.SetActive(true);
                            btnSettings.SetActive(true);
                            btnHelp.SetActive(true);
                            btnQuitBackToStart.SetActive(false);
                            btnExitProgram.SetActive(true);
                            break;
                        case MenuType.InGame:
                            btnBackClose.SetActive(true);
                            btnStartSingle.SetActive(false);
                            btnStartLocal.SetActive(false);
                            btnLoginToServer.SetActive(false);
                            btnMyInfo.SetActive(true);
                            btnSettings.SetActive(true);
                            btnHelp.SetActive(true);
                            btnQuitBackToStart.SetActive(true);
                            btnExitProgram.SetActive(false);
                            break;
                    }
                }
                type = value;
            }
        }

        // Start is called before the first frame update
        protected void Awake()
        {
            Type = type;
        }

        public void OnClickBackClose()
        {
            Close();
        }

        public void OnClickStartSingle()
        {
            MainScene.Instance.ShowTips("单机游戏功能正在开发中");
        }

        public void OnClickStartLocal()
        {
            MainScene.Instance.ShowTips("直连对战功能正在开发中");
        }

        public void OnClickLoginToServer()
        {
            MainScene.Instance.ShowTips("服务器对战功能正在开发中");
        }

        public void OnClickMyInfo()
        {
            MainScene.Instance.ShowPlayerInfoPanel(Present.GameMain.Instance.LocalUserInfo, true);
        }

        public void OnClickSettings()
        {
            MainScene.Instance.ShowSystemSettingPanel();
        }

        public void OnClickHelp()
        {
            MainScene.Instance.ShowTips("帮助说明正在开发中");
        }

        public void OnClickQuitBackToStart()
        {
            MainScene.Instance.ShowTips("退回主界面功能正在开发中");
        }

        public void OnClickExitProgram()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private MenuType type;
    }
}
