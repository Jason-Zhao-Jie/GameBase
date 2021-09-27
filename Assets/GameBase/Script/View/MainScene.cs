using System.Collections.Generic;
using UnityEngine;
using GameBase.View.Component;
using GameBase.Common.Core;
using CharacterInfo = GameBase.Common.Core.CharacterInfo;

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
        public static IMain Instance { get; private set; }

        #region 面板控件

        public AudioSource musicSource;
        public AudioSource soundSource;

        public GameObject worldRoot;
        public Canvas guiRoot;

        public ResourcesObject ro;
        public PrefabCollector pc;

        #endregion

        #region Interfaces

        public bool IsDebug => debug.debug;

        public SystemSettings SystemSettings
        {
            get
            {
                return systemSettings;
            }
            set
            {
                Notify(SystemEventType.OnSystemSettingChanged, value);
            }
        }
        public CharacterInfo LocalUserInfo
        {
            get
            {
                return LocalUserInfo;
            }
            set
            {
                Notify(SystemEventType.OnPlayerInfoChanged, value);
            }
        }

        public System.ValueType GetGameSetting(GameType type, int subType)
        {
            switch (type)
            {
                case GameType.Poker:
                    var pokerSubType = (Common.Core.Poker.GameSubType)subType;
                    switch (pokerSubType)
                    {
                        case Common.Core.Poker.GameSubType.Huolong:
                            return savedSetting_huolong;
                        default:
                            return default;
                    }
                default:
                    return default;
            }
        }

        public void SetGameSetting(GameType type, int subType, System.ValueType settings)
        {
            switch (type)
            {
                case GameType.Poker:
                    var pokerSubType = (Common.Core.Poker.GameSubType)subType;
                    switch (pokerSubType)
                    {
                        case Common.Core.Poker.GameSubType.Huolong:
                            savedSetting_huolong = (Common.Core.Poker.Huolong.GameSetting)settings;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void PlayMusic(string audioKey, bool isUrl = false)
        {
            if (isUrl != isCurrentPlayingMusicUrl || audioKey != currentPlayingMusic)
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

        public int Listen(SystemEventType _event, System.Action<object[]> callback)
        {
            if (callback == null)
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

        public bool Notify<T>(SystemEventType _event, params T[] data)
        {
            var msg = new EventData
            {
                _event = _event,
                classData = new object[data.Length],
            };
            for (int i = 0; i < data.Length; ++i)
            {
                msg.classData[i] = data[i];
            }
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

        #endregion Interfaces

        #region Unity Events

        // Start is called before the first frame update
        protected void Awake()
        {
            Present.GameMain.Instance = this;
            Instance = this;

            // debug 参数优先
            var debugText = ro.GetConfigText(ResourcesObject.config_debug).text;
            debug = JsonUtility.FromJson<DebugData>(debugText);
            if (debug.forceClearStorage)
            {
                storage.ClearAllStorage();
            }

            // 读取配置-AI玩家
            var aiText = ro.GetConfigText(ResourcesObject.config_ai).text;
            var aiConfig = JsonUtility.FromJson<CharacterInfo_AI[]>(aiText);
            foreach (var ai in aiConfig)
            {
                aiCharactersMap.Add(ai.id, ai);
                aiCharacterStatesMap.Add(ai.id, new PlayerState() { playerId = ai.id, game = GameType.Unknown, subType = 0, seat = 0, online = false });
            }

            // 读取配置(临时)-初始化配置
            var initialText = ro.GetConfigText(ResourcesObject.config_initial).text;
            var initialConfig = JsonUtility.FromJson<InitialData>(initialText);

            // 读取存储-系统设定
            systemSettings = storage.SystemSettings;
            if (systemSettings.aiDelay == 0f)
            {
                systemSettings = initialConfig.systemSetting;
                storage.SystemSettings = systemSettings;
            }
            OnSystemSettingChanged(new object[] { systemSettings });

            // 读取存储-个人资料
            localUserInfo = storage.LocalUserInfo;
            if (localUserInfo == null)
            {
                localUserInfo = initialConfig.localUserInfo;
                storage.LocalUserInfo = localUserInfo;
            }
            localUserState = new PlayerState() { playerId = localUserInfo.id, game = GameType.Unknown, subType = 0, seat = 0, online = true };

            // 读取存储-游戏设定-攉龙
            savedSetting_huolong = storage.GetGameSetting<Common.Core.Poker.Huolong.GameSetting>(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong);
            if (savedSetting_huolong.playerNum == 0)
            {
                savedSetting_huolong = initialConfig.huolongSetting;
                storage.SetGameSetting(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong, savedSetting_huolong);
            }

            // 自监听事件
            Listen(SystemEventType.OnSystemSettingChanged, OnSystemSettingChanged);
            Listen(SystemEventType.OnPlayerInfoChanged, OnPlayerInfoChanged);
            Listen(SystemEventType.OnPlayerExitGame, OnPlayerExitGame);
            Listen(SystemEventType.OnPlayerOffline, OnPlayerOffline);
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

        private void OnSystemSettingChanged(object[] data)
        {
            var set = (SystemSettings)data[0];
            musicSource.mute = set.musicMute;
            musicSource.volume = set.musicVolume;
            soundSource.mute = set.soundMute;
            soundSource.volume = set.soundVolume;
            systemSettings = set;
            storage.SystemSettings = set;
        }

        private void OnPlayerInfoChanged(object[] data)
        {
            var info = (CharacterInfo)data[0];
            if (info.id == localUserInfo.id)
            {
                localUserInfo = info;
                storage.LocalUserInfo = info;
            }
        }

        private void OnPlayerExitGame(object[] data)
        {
            var id = (int)data[0];
            if (!aiCharacterStatesMap.ContainsKey(id))
            {
                var ai_state = aiCharacterStatesMap[id];
                ai_state.game = GameType.Unknown;
                ai_state.subType = 0;
                Notify(SystemEventType.OnPlayerOffline, id);
            }
            else if (localUserInfo.id == id)
            {
                localUserState.game = GameType.Unknown;
                localUserState.subType = 0;
            }
        }

        private void OnPlayerOffline(object[] data)
        {
            var id = (int)data[0];
            if (!aiCharacterStatesMap.ContainsKey(id))
            {
                var ai_state = aiCharacterStatesMap[id];
                ai_state.online = false;
            }
            else if (localUserInfo.id == id)
            {
                Common.PlatformInterface.Base.DebugWarning("为什么会收到本机玩家下线的事件？");
                localUserState.online = false;
            }
        }

        #endregion System Events

        #region Private Functions

        private void SetAIOnline(int id, bool online, GameType game, int subType, int seat)
        {
            var state = aiCharacterStatesMap[id];
            state.online = online;
            state.game = game;
            state.subType = subType;
            state.seat = seat;
        }

        private int GetGameId(GameType game, int subType)
        {
            return (int)game * 1000 + subType;
        }

        private GameType GetGameType(int gameId)
        {
            return (GameType)(gameId / 1000);
        }

        private int GetGameSubType(int gameId)
        {
            return gameId % 1000;
        }

        private CharacterInfo_AI GetRandomAIInfo(int gameId, int[] currentPlayers, int seat, System.ValueType gameSetting)
        {
            var offlineList = new List<WeightRollingData>();
            int weightTotal = 0;
            // 挑选可选的AI
            foreach (var i in aiCharactersMap)
            {
                if (!aiCharacterStatesMap[i.Key].online)
                {
                    var dt = new WeightRollingData();
                    dt.id = i.Key;
                    dt.min = weightTotal;
                    dt.max = weightTotal + i.Value.baseWeight[gameId];
                    weightTotal += i.Value.baseWeight[gameId];
                    offlineList.Add(dt);
                }
            }
            CharacterInfo_AI ret = null;
            while(ret == null)
            {
                // 随机数抓取AI
                var randRes = new System.Random();
                var randVal = randRes.Next(0, weightTotal - 1);
                int rolledId = 0;
                for(int i = 0; i < offlineList.Count; ++i)
                {
                    var dt = offlineList[i];
                    if(dt.min<=randVal && dt.max > randVal)
                    {
                        rolledId = dt.id;
                    }
                }
                // 被抓取的AI根据已入座的情况，选择是否加入游戏，这同时会检测当前已在场玩家的意愿（只检测是否为0）
                ret = aiCharactersMap[rolledId];
                switch (GetGameType(gameId))
                {
                    case GameType.Poker:
                        switch ((Common.Core.Poker.GameSubType)GetGameSubType(gameId))
                        {
                            case Common.Core.Poker.GameSubType.Huolong:
                                {
                                    var huolongSetting = (Common.Core.Poker.Huolong.GameSetting)gameSetting;
                                    var friends = new List<int>();
                                    var enemies = new List<int>();

                                    // 检查敌友玩家
                                    for (int i = 0; i < currentPlayers.Length; ++i)
                                    {
                                        if (i != seat && currentPlayers[i] != 0)
                                        {
                                            if (i % huolongSetting.campNum == seat % huolongSetting.campNum)
                                            {
                                                friends.Add(i);
                                            }
                                            else
                                            {
                                                enemies.Add(i);
                                            }
                                        }
                                    }

                                    // 根据敌友玩家决定意愿概率
                                    float finalRate = 1;
                                    if (friends.Count == 0 && ret.friendOthersRate.ContainsKey(0))
                                    {
                                        finalRate *= ret.friendOthersRate[0];
                                    }
                                    else
                                    {
                                        foreach (var id in friends)
                                        {
                                            if (ret.friendOthersRate.ContainsKey(id))
                                            {
                                                finalRate *= ret.friendOthersRate[id];
                                            }
                                            else if (ret.friendOthersRate.ContainsKey(0))
                                            {
                                                finalRate *= ret.friendOthersRate[0];
                                            }
                                            if (aiCharactersMap.ContainsKey(id))
                                            {
                                                var hisInfo = aiCharactersMap[id];
                                                if (hisInfo.friendOthersRate.ContainsKey(rolledId))
                                                {
                                                    if (hisInfo.friendOthersRate[id] == 0)
                                                    {
                                                        finalRate = 0;
                                                    }
                                                }
                                                else if (hisInfo.friendOthersRate.ContainsKey(0))
                                                {
                                                    if (hisInfo.friendOthersRate[0] == 0)
                                                    {
                                                        finalRate = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (enemies.Count == 0 && ret.enemyOthersRate.ContainsKey(0))
                                    {
                                        finalRate *= ret.enemyOthersRate[0];
                                    }
                                    else
                                    {
                                        foreach (var id in enemies)
                                        {
                                            if (ret.enemyOthersRate.ContainsKey(id))
                                            {
                                                finalRate *= ret.enemyOthersRate[id];
                                            }
                                            else if (ret.enemyOthersRate.ContainsKey(0))
                                            {
                                                finalRate *= ret.enemyOthersRate[0];
                                            }
                                            if (aiCharactersMap.ContainsKey(id))
                                            {
                                                var hisInfo = aiCharactersMap[id];
                                                if (hisInfo.enemyOthersRate.ContainsKey(rolledId))
                                                {
                                                    if (hisInfo.enemyOthersRate[id] == 0)
                                                    {
                                                        finalRate = 0;
                                                    }
                                                }
                                                else if (hisInfo.enemyOthersRate.ContainsKey(0))
                                                {
                                                    if (hisInfo.enemyOthersRate[0] == 0)
                                                    {
                                                        finalRate = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    var agree = randRes.NextDouble() < finalRate;
                                    if (agree)
                                    {
                                        Common.PlatformInterface.Base.DebugInfo("AI玩家【" + ret.name + "】" + "加入了游戏");
                                    }
                                    else
                                    {
                                        Common.PlatformInterface.Base.DebugInfo("摇到了【" + ret.name + "】,但" + (ret.gender == 2 ? "她" : "他") + "不愿意加入游戏，重新寻找");
                                        int minusValue = ret.baseWeight[gameId];
                                        ret = null;
                                        for (int i = 0; i < offlineList.Count; ++i)
                                        {
                                            var dt = offlineList[i];
                                            if (dt.min > randVal)
                                            {
                                                dt.min -= minusValue;
                                                dt.max -= minusValue;
                                            }
                                        }
                                        weightTotal -= minusValue;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        #endregion

        #region Private fields

        private string currentPlayingMusic;
        private bool isCurrentPlayingMusicUrl;

        private StorageManager storage = new StorageManager(new UnityStorage());
        private DebugData debug = new DebugData();

        private SystemSettings systemSettings;
        private CharacterInfo localUserInfo;
        private PlayerState localUserState;

        private Common.Core.Poker.Huolong.GameSetting savedSetting_huolong;

        private Dictionary<int, CharacterInfo_AI> aiCharactersMap = new Dictionary<int, CharacterInfo_AI>();
        private Dictionary<int, PlayerState> aiCharacterStatesMap = new Dictionary<int, PlayerState>();

        private readonly Queue<EventData> eventMap = new Queue<EventData>();
        private readonly Dictionary<SystemEventType, Dictionary<int, ListenerData>> listenerMap = new Dictionary<SystemEventType, Dictionary<int, ListenerData>>();

        #endregion Private fields

        #region Private classes

        [System.Serializable]
        private struct DebugData
        {
            public bool debug;
            public bool hideCopyrightElements;
            public bool forceClearStorage;
        }

        private class InitialData
        {
            public SystemSettings systemSetting;
            public CharacterInfo localUserInfo;
            public Common.Core.Poker.Huolong.GameSetting huolongSetting;
        }

        private struct EventData
        {
            public SystemEventType _event;
            public object[] classData;
        }

        private struct ListenerData
        {
            public SystemEventType _event;
            public System.Action<object[]> callback;
        }

        private struct WeightRollingData
        {
            public int id;
            public int min;
            public int max;
        }

        #endregion
    }
}