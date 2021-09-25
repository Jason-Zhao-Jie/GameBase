using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.View.Component
{
    public class ResourcesObject : MonoBehaviour
    {
        public const string audio_main_bg = "main_bg";
        public const string audio_poker_bg = "poker_bg";
        public const string audio_poker_match_start = "poker_match_start";
        public const string audio_poker_match_win = "poker_match_win";
        public const string audio_poker_match_lose = "poker_match_lose";
        public const string audio_poker_game_win = "poker_game_win";
        public const string audio_poker_game_lose = "poker_game_lose";
        public const string audio_poker_send_card = "poker_send_card";

        public const string config_denug = "config_debug";
        public const string config_initial = "config_initial";
        public const string config_ai = "config_ai";

        [System.Serializable]
        public struct EditorKeyValueStruct<T_Key, T_Value>
        {
            public T_Key id;
            public T_Value value;
        }

        public EditorKeyValueStruct<int, Sprite>[] cardSprites;
        public EditorKeyValueStruct<int, Sprite>[] aiPlayerHeadIconSprites;
        public EditorKeyValueStruct<string, AudioClip>[] audioClips;
        public EditorKeyValueStruct<string, TextAsset>[] configTexts;

        public Sprite GetCardSprite(int id)
        {
            return cardSpritesMap[id];
        }

        public Sprite GetAIPlayerHeadIconSprite(int id)
        {
            return aiPlayerHeadIconSpritesMap[id];
        }

        public AudioClip GetAudioClip(string id)
        {
            return audioClipsMap[id];
        }

        protected void Awake()
        {
            cardSpritesMap = new Dictionary<int, Sprite>();
            foreach (var v in cardSprites)
            {
                if (v.value != null)
                {
                    cardSpritesMap.Add(v.id, v.value);
                }
            }
            aiPlayerHeadIconSpritesMap = new Dictionary<int, Sprite>();
            foreach (var v in aiPlayerHeadIconSprites)
            {
                if (v.value != null)
                {
                    aiPlayerHeadIconSpritesMap.Add(v.id, v.value);
                }
            }
            audioClipsMap = new Dictionary<string, AudioClip>();
            foreach (var v in audioClips)
            {
                if (v.value != null)
                {
                    audioClipsMap.Add(v.id, v.value);
                }
            }
            configTextsMap = new Dictionary<string, TextAsset>();
            foreach (var v in configTexts)
            {
                if (v.value != null)
                {
                    configTextsMap.Add(v.id, v.value);
                }
            }
        }

        private Dictionary<int, Sprite> cardSpritesMap;
        private Dictionary<int, Sprite> aiPlayerHeadIconSpritesMap;
        private Dictionary<string, AudioClip> audioClipsMap;
        private Dictionary<string, TextAsset> configTextsMap;
    }
}
