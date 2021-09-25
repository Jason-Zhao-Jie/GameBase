using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View
{
    public class MainMenuPanel : MonoBehaviour
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
            transform.SetParent(null);
            Destroy(gameObject);
        }

        public void OnClickStartSingle()
        {

        }

        public void OnClickStartLocal()
        {

        }

        public void OnClickLoginToServer()
        {

        }

        public void OnClickMyInfo()
        {

        }

        public void OnClickSettings()
        {

        }

        public void OnClickHelp()
        {

        }

        public void OnClickQuitBackToStart()
        {

        }

        public void OnClickExitProgram()
        {

        }

        private MenuType type;
    }
}
