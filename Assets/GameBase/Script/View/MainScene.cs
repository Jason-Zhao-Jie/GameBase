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
        Utility.TipBar ShowTips(string text, float aliveTime = 2.0f);
        Utility.TipBar ShowWaiting(string text);
        T PushPanel<T>() where T : APanel;
        APanel PopPanel(bool destroy = true);
        Utility.MessageBox ShowMessageBox(string contentText, string textBtn1, System.Func<bool> callbackBtn1, string textBtn2 = null, System.Func<bool> callbackBtn2 = null, string textBtn3 = null, System.Func<bool> callbackBtn3 = null);
        Utility.PlayerInfoPanel ShowPlayerInfoPanel(CharacterInfo info, bool isMe = false);
        Utility.SystemSettingPanel ShowSystemSettingPanel();
        MainMenuPanel ShowMainMenuPanel(MainMenuPanel.MenuType type);
        APanel StartSettingGame(GameType type, int subType);
        void StartGame<T>(GameType type, int subType, T gameSettings, int playerNum) where T : struct;
    }

    public class MainScene : MonoBehaviour, Present.IGameMain, IMain
    {
        public static IMain Instance { get; private set; }

        #region 面板控件

        public AudioSource musicSource;
        public AudioSource soundSource;

        public GameObject worldRoot;
        public Canvas guiRoot;
        public Transform uiPanelRoot;
        public Transform uiDialogRoot;
        public Transform uiTopRoot;

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
                return localUserInfo;
            }
            set
            {
                Notify(SystemEventType.OnPlayerInfoChanged, value);
            }
        }

        public System.ValueType GetDefaultGameSetting(GameType type, int subType, int index)
        {
            switch (type)
            {
                case GameType.Poker:
                    var pokerSubType = (Common.Core.Poker.GameSubType)subType;
                    switch (pokerSubType)
                    {
                        case Common.Core.Poker.GameSubType.Huolong:
                            return initialConfig.huolongSetting[index];
                        default:
                            return default;
                    }
                default:
                    return default;
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

        public void PlaySound(AudioClip audio)
        {
            soundSource.PlayOneShot(audio);
        }

        public int Listen(SystemEventType _event, System.Action<object[]> callback)
        {
            if (callback == null)
            {
                return -1;
            }
            var ld = new ListenerData
            {
                _event = _event,
                callback = callback
            };
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

        public Utility.TipBar ShowTips(string text, float aliveTime = 2.0f)
        {
            var bar = Instantiate(pc.tipBar, uiTopRoot);
            var comp = bar.GetComponent<Utility.TipBar>();
            comp.text.text = text;
            comp.autoRemove = true;
            comp.secondsBeforeRemove = aliveTime;
            return comp;
        }

        public Utility.TipBar ShowWaiting(string text)
        {
            var bar = Instantiate(pc.waitingBar, uiTopRoot);
            var comp = bar.GetComponent<Utility.TipBar>();
            comp.text.text = text;
            comp.autoRemove = false;
            comp.secondsBeforeRemove = 0;
            return comp;
        }

        public T PushPanel<T>() where T : APanel
        {
            var bar = Instantiate(prefabList[typeof(T)], uiDialogRoot);
            var ret = bar.GetComponent<T>();
            if (panelStack.Count>0)
            {
                var last = panelStack.Peek();
                last.gameObject.SetActive(false);
            }
            panelStack.Push(ret);
            return ret;
        }

        public APanel PopPanel(bool destroy = true)
        {
            var ret = panelStack.Pop();
            ret.transform.SetParent(null);
            if (destroy)
            {
                Destroy(ret.gameObject);
            }
            if (panelStack.Count > 0)
            {
                var last = panelStack.Peek();
                last.gameObject.SetActive(true);
            }
            return ret;
        }

        public Utility.MessageBox ShowMessageBox(string contentText, string textBtn1, System.Func<bool> callbackBtn1, string textBtn2 = null, System.Func<bool> callbackBtn2 = null, string textBtn3 = null, System.Func<bool> callbackBtn3 = null)
        {
            var comp = PushPanel<Utility.MessageBox>();
            comp.Init(contentText, textBtn1, callbackBtn1, textBtn2, callbackBtn2, textBtn3, callbackBtn3);
            return comp;
        }

        public Utility.PlayerInfoPanel ShowPlayerInfoPanel(CharacterInfo info, bool isMe = false)
        {
            var comp = PushPanel<Utility.PlayerInfoPanel>();
            comp.Init(info, isMe);
            return comp;
        }

        public Utility.SystemSettingPanel ShowSystemSettingPanel()
        {
            var comp = PushPanel<Utility.SystemSettingPanel>();
            return comp;
        }

        public MainMenuPanel ShowMainMenuPanel(MainMenuPanel.MenuType type)
        {
            var comp = PushPanel<MainMenuPanel>();
            comp.Type = type;
            return comp;
        }

        public APanel StartSettingGame(GameType type, int subType)
        {
            switch (type)
            {
                case GameType.Poker:
                    var pokerSubType = (Common.Core.Poker.GameSubType)subType;
                    switch (pokerSubType)
                    {
                        case Common.Core.Poker.GameSubType.Huolong:
                            return PushPanel<Poker.Huolong.GameSettingPanel_Huolong>();
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public void StartGame<T>(GameType type, int subType, T gameSettings, int playerNum) where T : struct
        {
            bool ok = false;
            var gameId = GetGameId(type, subType);
            var panel = gamePanelList[gameId];
            var world = gameWorldList[gameId];
            if (panel != null)
            {
                ok = true;
                var bar = Instantiate(panel, uiPanelRoot);
                var ret = bar.GetComponent<AGamePanel>();
                currentGamePanel = ret;
                var worldManager = Instantiate(world, worldRoot.transform);
                currentGameWorldManager = worldManager;
                var playerIds = new int[playerNum];
                switch (type)
                {
                    case GameType.Poker:
                        var pokerSubType = (Common.Core.Poker.GameSubType)subType;
                        switch (pokerSubType)
                        {
                            case Common.Core.Poker.GameSubType.Huolong:
                                {
                                    // controller
                                    var ctrl = new Present.Poker.Huolong.Controller();
                                    if (currentGameController == null)
                                    {
                                        currentGameController = ctrl;
                                    }

                                    // 本机 player
                                    var hostItem = ret.GetComponent<Poker.Huolong.GamePanel_Huolong>();
                                    hostItem.PokerWorld = worldManager.GetComponent<Poker.WorldPokerManager>();
                                    var hostVec = new Present.Poker.Huolong.PlayerVector();
                                    hostVec.SetPlayerItem(hostItem);
                                    hostItem.SetVector(hostVec);
                                    hostVec.PlayerInfo = localUserInfo;
                                    playerIds[0] = hostVec.PlayerInfo.id;
                                    ctrl.SetPlayer(0, hostVec);
                                    Notify(SystemEventType.OnPlayerOnline, playerIds[0]);
                                    Notify(SystemEventType.OnPlayerEnterGame, playerIds[0], (int)type, subType, 0);

                                    // AI player
                                    for (int i = 1; i < playerNum; ++i)
                                    {
                                        var item = new Present.Poker.Huolong.AIPlayerItem();
                                        var vec = new Present.Poker.Huolong.PlayerVector();
                                        vec.SetPlayerItem(item);
                                        item.SetVector(vec);
                                        vec.PlayerInfo = GetRandomAIInfo(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong, playerIds, i, gameSettings);
                                        playerIds[i] = vec.PlayerInfo.id;
                                        ctrl.SetPlayer(i, vec);
                                    }

                                    // start
                                    if (gameSettings is Common.Core.Poker.Huolong.GameSetting gs)
                                    {
                                        if (!ctrl.StartGame(gs))
                                        {
                                            Common.PlatformInterface.Base.DebugError("玩家没有全部准备就绪");
                                            ok = false;
                                            // todo 处理后续清理
                                        }
                                        else
                                        {
                                            // 开始游戏成功, 这里理论上不需要什么处理
                                        }
                                    }
                                    else
                                    {
                                        Common.PlatformInterface.Base.DebugError("传入配置参数不正确");
                                        ok = false;
                                    }
                                }
                                break;
                            default:
                                ok = false;
                                break;
                        }
                        break;
                    default:
                        ok = false;
                        break;
                }

            }
            if (ok)
            {
                while (panelStack.Count > 0)
                {
                    PopPanel();
                }
            }
        }

        #endregion Interfaces

        #region Unity Events

        // Start is called before the first frame update
        protected void Awake()
        {
            Present.GameMain.Instance = this;
            Instance = this;
        }

        // Update is called once per frame
        protected void Start()
        {
            // debug 参数优先
            var debugText = ro.GetConfigText(ResourcesObject.config_debug).text;
            debug = JsonUtility.FromJson<DebugData>(debugText);
            if (debug.forceClearStorage)
            {
                storage.ClearAllStorage();
            }

            // 读取配置-AI玩家
            var aiText = ro.GetConfigText(ResourcesObject.config_ai).text;
            var aiConfig = JsonUtility.FromJson<AIConfigData>(aiText);
            foreach (var ai in aiConfig.ais)
            {
                aiCharactersMap.Add(ai.id, ai);
                aiCharacterStatesMap.Add(ai.id, new PlayerState() { playerId = ai.id, game = GameType.Unknown, subType = 0, seat = 0, online = false });
            }

            // 读取配置(临时)-初始化配置
            var initialText = ro.GetConfigText(ResourcesObject.config_initial).text;
            initialConfig = JsonUtility.FromJson<InitialData>(initialText);

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
                savedSetting_huolong = initialConfig.huolongSetting[0];
                storage.SetGameSetting(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong, savedSetting_huolong);
            }

            // UI 注册
            prefabList.Add(typeof(Utility.MessageBox), pc.messageBox);
            prefabList.Add(typeof(Utility.PlayerInfoPanel), pc.playerInfoPanel);
            prefabList.Add(typeof(Utility.SystemSettingPanel), pc.systemSettingPanel);
            prefabList.Add(typeof(MainMenuPanel), pc.mainMenuPanel);
            prefabList.Add(typeof(Poker.Huolong.GameSettingPanel_Huolong), pc.gameSettingPanel_huolong);

            gamePanelList.Add(GetGameId(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong), pc.gamePanel_huolong);

            gameWorldList.Add(GetGameId(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong), pc.gameWorld_traditionalPoker);

            // 自监听事件
            Listen(SystemEventType.OnSystemSettingChanged, OnSystemSettingChanged);
            Listen(SystemEventType.OnMyInfoChanged, OnPlayerInfoChanged);
            Listen(SystemEventType.OnPlayerInfoChanged, OnPlayerInfoChanged);
            Listen(SystemEventType.OnPlayerEnterGame, OnPlayerEnterGame);
            Listen(SystemEventType.OnPlayerExitGame, OnPlayerExitGame);
            Listen(SystemEventType.OnPlayerOnline, OnPlayerOnline);
            Listen(SystemEventType.OnPlayerOffline, OnPlayerOffline);

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

        private void OnPlayerEnterGame(object[] data)
        {
            var id = (int)data[0];
            var gameType = (GameType)data[1];
            var gameSubType = (int)data[2];
            var seat = (int)data[3];
            if (localUserInfo.id == id)
            {
                localUserState.game = gameType;
                localUserState.subType = gameSubType;
                localUserState.seat = seat;
                Common.PlatformInterface.Base.DebugInfo("本机玩家加入游戏");
            }
            else if (aiCharacterStatesMap.ContainsKey(id))
            {
                Common.PlatformInterface.Base.DebugInfo("AI玩家【" + aiCharactersMap[id].name + "】" + "加入游戏");
            }
            else
            {
                Common.PlatformInterface.Base.DebugInfo("未知类型玩家加入游戏，id：" + id);
            }
        }

        private void OnPlayerExitGame(object[] data)
        {
            var id = (int)data[0]; 
            if(localUserInfo.id == id)
            {
                localUserState.game = GameType.Unknown;
                localUserState.subType = 0;
                localUserState.seat = 0;
                Common.PlatformInterface.Base.DebugInfo("本机玩家离开游戏");
            }
            else if (aiCharacterStatesMap.ContainsKey(id))
            {
                SetAIOnline(id, false, GameType.Unknown, 0, 0);
                Notify(SystemEventType.OnPlayerOffline, id);
                Common.PlatformInterface.Base.DebugInfo("AI玩家【" + aiCharactersMap[id].name + "】" + "离开游戏");
            }
            else
            {
                Common.PlatformInterface.Base.DebugInfo("未知类型玩家离开游戏，id：" + id);
            }
        }

        private void OnPlayerOnline(object[] data)
        {
            var id = (int)data[0];
            if (localUserInfo.id == id)
            {
                Common.PlatformInterface.Base.DebugWarning("为什么会收到本机玩家上线的事件？");
                localUserState.online = true;
            }
            else if (aiCharacterStatesMap.ContainsKey(id))
            {
                Common.PlatformInterface.Base.DebugInfo("AI玩家上线：" + aiCharactersMap[id].name);
            }
            else
            {
                Common.PlatformInterface.Base.DebugInfo("未知类型玩家上线，id：" + id);
            }
        }

        private void OnPlayerOffline(object[] data)
        {
            var id = (int)data[0];
            if (localUserInfo.id == id)
            {
                Common.PlatformInterface.Base.DebugWarning("为什么会收到本机玩家下线的事件？");
                localUserState.online = false;
            }
            else if (aiCharacterStatesMap.ContainsKey(id))
            {
                Common.PlatformInterface.Base.DebugInfo("AI玩家下线：" + aiCharactersMap[id].name);
            }
            else
            {
                Common.PlatformInterface.Base.DebugInfo("未知类型玩家下线，id：" + id);
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

        private CharacterInfo_AI GetRandomAIInfo(GameType game, int gameSubType, int[] currentPlayers, int seat, System.ValueType gameSetting)
        {
            var gameId = GetGameId(GameType.Poker, (int)Common.Core.Poker.GameSubType.Huolong);
            var offlineList = new List<WeightRollingData>();
            int weightTotal = 0;
            // 挑选可选的AI
            foreach (var i in aiCharactersMap)
            {
                if (!aiCharacterStatesMap[i.Key].online)
                {
                    var baseWeight = i.Value.GetBaseWeight(gameId);
                    if (baseWeight > 0)
                    {
                        var dt = new WeightRollingData();
                        dt.id = i.Key;
                        dt.min = weightTotal;
                        weightTotal += baseWeight;
                        dt.max = weightTotal;
                        offlineList.Add(dt);
                    }
                }
            }
            CharacterInfo_AI ret = null;
            while(ret == null)
            {
                // 随机数抓取AI
                var randRes = new System.Random();
                var randVal = randRes.Next(0, weightTotal - 1);
                int rolledId = 0;
                int rolledIndex = -1;
                for(int i = 0; i < offlineList.Count; ++i)
                {
                    var dt = offlineList[i];
                    if(dt.min<=randVal && dt.max > randVal)
                    {
                        rolledId = dt.id;
                        rolledIndex = i;
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
                                    Common.Core.Poker.Huolong.GameSetting huolongSetting = (Common.Core.Poker.Huolong.GameSetting)gameSetting;
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
                                        bool haveSpecial = false;
                                        foreach (var id in friends)
                                        {
                                            if (ret.friendOthersRate.ContainsKey(id))
                                            {
                                                haveSpecial = true;
                                                break;
                                            }
                                        }
                                        foreach (var id in friends)
                                        {
                                            if (ret.friendOthersRate.ContainsKey(id))
                                            {
                                                finalRate *= ret.friendOthersRate[id];
                                            }
                                            else if (ret.friendOthersRate.ContainsKey(0))
                                            {
                                                if (ret.friendOthersRate[0] > 0 || !haveSpecial)
                                                {
                                                    finalRate *= ret.friendOthersRate[0];
                                                }
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
                                        SetAIOnline(ret.id, true, game, gameSubType, seat);
                                        Notify(SystemEventType.OnPlayerOnline, ret.id);
                                        Notify(SystemEventType.OnPlayerEnterGame, ret.id, (int)game, gameSubType, seat);
                                    }
                                    else
                                    {
                                        Common.PlatformInterface.Base.DebugInfo("摇到了【" + ret.name + "】,但" + (ret.gender == Gender.Female ? "她" : "他") + "不愿意加入游戏，重新寻找");
                                        int minusValue = ret.GetBaseWeight(gameId);
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
                                        offlineList.RemoveAt(rolledIndex);
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

        private readonly Dictionary<System.Type, GameObject> prefabList = new Dictionary<System.Type, GameObject>();
        private readonly Dictionary<int, GameObject> gamePanelList = new Dictionary<int, GameObject>();
        private readonly Dictionary<int, GameObject> gameWorldList = new Dictionary<int, GameObject>();
        private readonly Stack<APanel> panelStack = new Stack<APanel>();
        private AGamePanel currentGamePanel = null;
        private GameObject currentGameWorldManager = null;
        private Common.Interface.IController currentGameController = null;

        private string currentPlayingMusic;
        private bool isCurrentPlayingMusicUrl;

        private StorageManager storage = new StorageManager(new UnityStorage());
        private DebugData debug = new DebugData();

        private InitialData initialConfig;
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

        [System.Serializable]
        private struct AIConfigData
        {
            [SerializeField]
            public CharacterInfo_AI[] ais;
        }

        [System.Serializable]
        private class InitialData
        {
            public SystemSettings systemSetting;
            public CharacterInfo localUserInfo;
            public Common.Core.Poker.Huolong.GameSetting[] huolongSetting;
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

        private class WeightRollingData
        {
            public int id;
            public int min;
            public int max;
        }

        #endregion
    }
}