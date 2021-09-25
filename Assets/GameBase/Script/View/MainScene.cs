using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase.View.Component;

namespace GameBase.View
{
    public class MainScene : MonoBehaviour, Present.IGameMain
    {
        #region 面板控件

        public AudioSource musicSource;
        public AudioSource soundSource;

        public Canvas bgRoot;
        public GameObject worldRoot;
        public Canvas guiRoot;

        public ResourcesObject ro;
        public PrefabCollector pc;

        #endregion

        #region 接口

        public void PlayMusic(string audioKey, bool isUrl = false)
        {
            if(isUrl != isCurrentPlayingMusicUrl || audioKey != currentPlayingMusic)
            {
                musicSource.clip = ro.GetAudioClip(audioKey);
            }
            musicSource.Play();
            currentPlayingMusic = audioKey;
            isCurrentPlayingMusicUrl = isUrl;
        }

        public void PlaySound(string audioKey, bool isUrl = false)
        {
            soundSource.PlayOneShot(ro.GetAudioClip(audioKey));
        }

        public MainMenuPanel ShowMainMenuPanel(MainMenuPanel.MenuType type)
        {
            var obj = Instantiate(pc.mainMenuPanel,guiRoot.transform);
            var ret = obj.GetComponent<MainMenuPanel>();
            ret.Type = type;
            return ret;
        }

        #endregion

        // Start is called before the first frame update
        protected void Awake()
        {
            Present.GameMain.Instance = this;
            ShowMainMenuPanel(MainMenuPanel.MenuType.Start);
        }

        // Update is called once per frame
        protected void Start()
        {
            PlayMusic(ResourcesObject.audio_main_bg);
        }

        private string currentPlayingMusic;
        private bool isCurrentPlayingMusicUrl;
    }
}