using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase.View.Component;
using GameBase.Common.Core;

namespace GameBase.View
{
    public interface IMain
    {
        Sprite GetCardSprite(int id);
        Sprite GetPlayerHeadIcon(int id);
        GameObject CreatePokerCard(int id, Transform transform = null);
        GameObject CreateUIPokerCard(int id, Transform transform = null);
        GameObject ShowTips(string text, float aliveTime = 2.0f);
        GameObject ShowWaiting(string text);
        MainMenuPanel ShowMainMenuPanel(MainMenuPanel.MenuType type);
    }

    public class MainScene : MonoBehaviour, Present.IGameMain, IMain
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

        public static IMain Instance { get; private set; }

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

        public int Listen(SystemEventType _event, System.Action<object> callback)
        {
            if(callback== null)
            {
                return -1;
            }
            var ld = new ListenerData();
            ld._event = _event;
            ld.callback = callback;
            if (!listenerMap.ContainsKey(_event))
            {
                listenerMap.Add(_event, new Dictionary<int, ListenerData>());
            }
            var list = listenerMap[_event];
            int id = 1;
            while (list.ContainsKey(id))
            {
                ++id;
            }
            list.Add(id, ld);
            return id;
        }

        public bool Unlisten(SystemEventType _event, int id)
        {
            if (listenerMap.ContainsKey(_event))
            {
                var list = listenerMap[_event];
                if (list.ContainsKey(id))
                {
                    list.Remove(id);
                    return true;
                }
            }
            return false;
        }

        public bool Notify<T>(SystemEventType _event, T data) where T : class
        {
            var msg = new EventData
            {
                _event = _event,
                classData = data,
            };
            eventMap.Enqueue(msg);
            return true;
        }

        public Sprite GetCardSprite(int id)
        {
            return ro.GetCardSprite(id);
        }

        public Sprite GetPlayerHeadIcon(int id)
        {
            return ro.GetAIPlayerHeadIconSprite(id);
        }

        public GameObject CreatePokerCard(int id, Transform transform = null)
        {
            var card = Instantiate(pc.poker_pokerCard, transform);
            card.GetComponent<Poker.Card>().SetCard(id, GetCardSprite(id));
            return card;
        }

        public GameObject CreateUIPokerCard(int id, Transform transform = null)
        {
            var card = Instantiate(pc.poker_uiPokerCard, transform);
            card.GetComponent<Poker.Card>().SetCard(id, GetCardSprite(id));
            return card;
        }

        public GameObject ShowTips(string text, float aliveTime = 2.0f)
        {
            var bar = Instantiate(pc.tipBar, guiRoot.transform);
            var comp = bar.GetComponent<Utility.TipBar>();
            comp.text.text = text;
            comp.autoRemove = true;
            comp.secondsBeforeRemove = aliveTime;
            return bar;
        }

        public GameObject ShowWaiting(string text)
        {
            var bar = Instantiate(pc.waitingBar, guiRoot.transform);
            var comp = bar.GetComponent<Utility.TipBar>();
            comp.text.text = text;
            comp.autoRemove = false;
            comp.secondsBeforeRemove = 0;
            return bar;
        }

        public GameObject ShowMessageBox(string textBtn1, System.Action callbackBtn1, string textBtn2 = null, System.Action callbackBtn2 = null, string textBtn3 = null, System.Action callbackBtn3 = null)
        {
            var bar = Instantiate(pc.messageBox, guiRoot.transform);
            var comp = bar.GetComponent<Utility.MessageBox>();
            comp.Init(textBtn1, callbackBtn1, textBtn2, callbackBtn2, textBtn3, callbackBtn3);
            return bar;
        }

        public MainMenuPanel ShowMainMenuPanel(MainMenuPanel.MenuType type)
        {
            var obj = Instantiate(pc.mainMenuPanel, guiRoot.transform);
            var ret = obj.GetComponent<MainMenuPanel>();
            ret.Type = type;
            return ret;
        }

        #endregion

        #region Unity Events

        // Start is called before the first frame update
        protected void Awake()
        {
            Present.GameMain.Instance = this;
            Instance = this;
            Listen(SystemEventType.OnSystemSettingChanged, OnSystemSettingChanged);
        }

        // Update is called once per frame
        protected void Start()
        {
            ShowMainMenuPanel(MainMenuPanel.MenuType.Start);
            PlayMusic(ResourcesObject.audio_main_bg);
        }

        protected void Update()
        {
            if (eventMap.Count > 0)
            {
                var msg = eventMap.Dequeue();
                if (listenerMap.ContainsKey(msg._event))
                {
                    var list = listenerMap[msg._event];
                    foreach (var listener in list)
                    {
                        listener.Value.callback(msg.classData);
                    }
                }
            }
        }

        #endregion Unity Events

        #region System Events

        private void OnSystemSettingChanged(object data) {
            var set = data as SystemSettings;
            musicSource.mute = set.musicMute;
            musicSource.volume = set.musicVolume;
            soundSource.mute = set.soundMute;
            soundSource.volume = set.soundVolume;
        }

        #endregion System Events

        private string currentPlayingMusic;
        private bool isCurrentPlayingMusicUrl;
        private readonly Queue<EventData> eventMap = new Queue<EventData>();
        private readonly Dictionary<SystemEventType, Dictionary<int, ListenerData>> listenerMap = new Dictionary<SystemEventType, Dictionary<int, ListenerData>>();

        private struct EventData
        {
            public SystemEventType _event;
            public object classData;
        }

        private struct ListenerData
        {
            public SystemEventType _event;
            public System.Action<object> callback;
        }
    }
}